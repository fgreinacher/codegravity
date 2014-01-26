
(function ($) {

    var Renderer = function(canvas) {
        var canvas = $(canvas).get(0);
        var ctx = canvas.getContext("2d");
        var particleSystem;
        var that = {
            init: function(system) {
                particleSystem = system;
                // TODO: Real diminsions und resizability 
                // inform the system of the screen dimensions so it can map coords for us.
                // if the canvas is ever resized, screenSize should be called again with
                // the new dimensions
                particleSystem.screenSize(canvas.width, canvas.height);
                particleSystem.screenPadding(80); // leave an extra 80px of whitespace per side
                that.initMouseHandling();
            },

            redraw: function() {
                ctx.fillStyle = "white";
                ctx.fillRect(0, 0, canvas.width, canvas.height);

                particleSystem.eachEdge(function(edge, pt1, pt2) {

                    // edge: {source:Node, target:Node, length:#, data:{}}
                    // pt1:  {x:#, y:#}  source position in screen coords
                    // pt2:  {x:#, y:#}  target position in screen coords

                    // draw a line from pt1 to pt2
                    ctx.strokeStyle = "rgba(0,0,0, .333)";
                    ctx.lineWidth = 1;
                    ctx.beginPath();
                    ctx.moveTo(pt1.x, pt1.y);
                    ctx.lineTo(pt2.x, pt2.y);
                    ctx.stroke();
                });

                particleSystem.eachNode(function(node, pt) {
                    // node: {mass:#, p:{x,y}, name:"", data:{}}
                    // pt:   {x:#, y:#}  node position in screen coords

                    //// draw a rectangle centered at pt
                    //var w = 10;
                    //ctx.fillStyle = (node.data.alone) ? "orange" : "black";
                    //ctx.fillRect(pt.x - w / 2, pt.y - w / 2, w, w);

                    // determine the box size and round off the coords if we'll be 
                    var parts = node.name.split(".");
                    var label = parts[parts.length - 1];

                    var w = ctx.measureText(label || "").width + 6;

                    if (!(label || "").match(/^[ \t]*$/)) {
                        pt.x = Math.floor(pt.x);
                        pt.y = Math.floor(pt.y);
                    } else {
                        label = null;
                    }

                    //Clear any edges under text
                    ctx.clearRect(pt.x - w / 2, pt.y - 7, w, 14);

                    // draw the text
                    if (label) {
                        ctx.font = "bold 11px Arial";
                        ctx.textAlign = "center";
                        ctx.fillStyle = "#888888";
                        ctx.fillText(label || "", pt.x, pt.y + 4);
                    }
                });
            },

            initMouseHandling: function() {
                // no-nonsense drag and drop (thanks springy.js)
                var dragged = null;

                // set up a handler object that will initially listen for mousedowns then
                // for moves and mouseups while dragging
                var handler = {
                    clicked: function(e) {
                        var pos = $(canvas).offset();
                        _mouseP = arbor.Point(e.pageX - pos.left, e.pageY - pos.top);
                        dragged = particleSystem.nearest(_mouseP);

                        if (dragged && dragged.node !== null) {
                            // while we're dragging, don't let physics move the node
                            dragged.node.fixed = true;
                        }

                        $(canvas).bind('mousemove', handler.dragged);
                        $(window).bind('mouseup', handler.dropped);
                        return false;
                    },

                    dragged: function(e) {
                        var pos = $(canvas).offset();
                        var s = arbor.Point(e.pageX - pos.left, e.pageY - pos.top);
                        if (dragged && dragged.node !== null) {
                            var p = particleSystem.fromScreen(s);
                            dragged.node.p = p;
                        }

                        return false;
                    },

                    dropped: function(e) {
                        if (dragged === null || dragged.node === undefined) return;
                        if (dragged.node !== null) dragged.node.fixed = false;
                        dragged.node.tempMass = 1000;
                        dragged = null;
                        $(canvas).unbind('mousemove', handler.dragged);
                        $(window).unbind('mouseup', handler.dropped);
                        _mouseP = null;
                        return false;
                    }
                };
                $(canvas).mousedown(handler.clicked);
            },
        };
        return that;
    };

    $(document).ready(function() {
        var sys = arbor.ParticleSystem(1000, 600, 0.5); // create the system with sensible repulsion/stiffness/friction
        sys.parameters({ gravity: true }); // use center-gravity to make the graph settle nicely (ymmv)
        sys.renderer = Renderer("#viewport"); // our newly created renderer will have its .init() method called shortly by sys...

        $.getJSON("api/dependencies/nodes", function(types) {
            $.each(types, function(idx, type) {
                sys.addNode(type.name);
            });

            $.each(types, function(idx, node) {
                $.getJSON("api/dependencies/edges?node=" + node.name, function(edges) {
                    $.each(edges, function(idx, edge) {
                        sys.addEdge(edge.source.name, edge.target.name, { length: 5 });
                    });
                });
            });
        });
    });
})(this.jQuery)