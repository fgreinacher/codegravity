var showLabels = true;

(function($) {

    var renderer = function(elt) {

        var strokeStyles = {
            "Implements": "rgba(255,0,0, .333)",
            "Uses": "rgba(0,255,0, .333)",
            "Calls": "rgba(0,0,255, .333)"
        };

        var canvas = $(elt).get(0);
        var ctx = canvas.getContext("2d");
        var particleSystem;
        var that = {
            init: function(system) {

                particleSystem = system;

                system.screen({
                    size: { width: canvas.width, height: canvas.height },
                    padding: [50, 50, 50, 50],
                    step: .02
                });

                $(window).resize(that.resize);
                that.resize();
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
                    ctx.strokeStyle = strokeStyles[edge.data.kind];
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

                    var label = showLabels ? node.data.label : "x";

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

            resize: function() {
                canvas.width = .75 * $(window).width();
                canvas.height = $(window).height();
                particleSystem.screen({
                    size: { width: canvas.width, height: canvas.height }
                });
                that.redraw();
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

        var sys = arbor.ParticleSystem();
        sys.parameters({
            repulsion: 1000,
            stiffness: 600,
            friction: 0.5,
            fps: 55,
            dt: 0.02,
            precision: 0.6,
            gravity: true
        });

        sys.renderer = renderer("#viewport");
        sys.getVertex = function(path) {
            for (var i = 0; i < path.length; i++) {
                var firstVisible = sys.getNode(path[i]);
                if (firstVisible != null) return firstVisible;
            }
            return null;
        };
        sys.addVertex = function(treeNode, x, y) {
            sys.addNode(treeNode.id, { label: treeNode.text, x: x, y: y });
            $.getJSON("api/dependencies/edges?id=" + treeNode.id, function(edges) {
                $.each(edges, function(eidx, edge) {

                    var source = sys.getVertex(edge.sources);
                    var target = sys.getVertex(edge.targets);
                    sys.addEdge(
                        source,
                        target,
                        { kind: edge.kind });
                });
            });
        };


        $('#typetree')
            .on('after_open.jstree', function(e, data) {
                var typeTree = data.instance;
                var parent = data.node;
                var parentVertex = sys.getNode(parent.id);
                var x = 0;
                var y = 0;
                if (parentVertex != null) {
                    x = parentVertex.p.x;
                    y = parentVertex.p.y;
                    sys.pruneNode(parentVertex);
                }
                var segmentAngle = 2 * Math.PI / parent.children.length;
                $.each(parent.children, function(idx, childId) {
                    var childNode = typeTree.get_node(childId);
                    var angle = segmentAngle * idx;
                    var vx = x + .2 * Math.sin(angle);
                    ;
                    var vy = y + .2 * Math.cos(angle);
                    ;
                    sys.addVertex(childNode, vx, vy);
                });
            })
            .on('after_close.jstree', function(e, data) {
                var typeTree = data.instance;
                var parent = data.node;
                var x = 0;
                var y = 0;
                var childCount = parent.children.length;
                $.each(parent.children, function(idx, childId) {
                    var childNode = typeTree.get_node(childId);
                    typeTree.close_node(childNode);
                    var childVertex = sys.getNode(childId);
                    x += childVertex.p.x;
                    y += childVertex.p.y;
                    sys.pruneNode(childVertex);
                });
                x = x / childCount;
                y = y / childCount;
                sys.addVertex(parent, x, y);
            })
            .on('loaded.jstree', function(e, data) {
                var typeTree = data.instance;
                var rootNode = typeTree.get_node("#");
                typeTree.open_node(rootNode);
                $.each(rootNode.children, function(idx, childId) {
                    var childNode = data.instance.get_node(childId);
                    typeTree.open_node(childNode);
                });
            })
            .jstree({
                'core': {
                    'data': {
                        'url': function(node) {
                            return node.id === '#' ?
                                '/api/treeview/children' :
                                '/api/treeview/children';
                        },
                        'data': function(node) {
                            return { 'id': node.id };
                        }
                    }
                }
            });
    });
})(this.jQuery);