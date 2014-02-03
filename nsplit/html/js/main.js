// This code is distributed under MIT license. 
// Copyright (c) 2014 George Mamaladze, Florian Greinacher
// See license.txt or http://opensource.org/licenses/mit-license.php
var node;
var link;
var root;
var rawLinks = [];
var verticesById = [];

var vis = d3.select("#viewport").append("svg:svg");
window.onresize = resize;

var force = d3.layout.force()
    .on("tick", tick);

d3.json("api/treeview/deep", function(jsonRoot) {

    root = new Vertex(jsonRoot);
    root.fixed = true;
    root.selected = [];
   
    var vertices = root.subtree();
    assignColorsToTop20(vertices);

    verticesById = vertices.indexById();

    $("#typetree")
        .jstree({
            "core": {
                "data":
                    jsonRoot
            },
            "checkbox": {
                "keep_selected_style": false
            },
            "plugins": ["checkbox", "search"]
        })
        .on('open_node.jstree', function(e, data) {
            var vertex = verticesById[data.node.original.id];
            if (!vertex.isExpanded) click(vertex);
        })
        .on('close_node.jstree', function(e, data) {
            var n = data.node;
            if (!n.children) return;
            n.children.forEach(function(el) { data.instance.close_node(el); });
            var vertex = verticesById[n.original.id];
            if (vertex.isExpanded) click(vertex);
        })
        .on('changed.jstree', function (e, data) {
            root.selected = [];
            data.selected.forEach(function (el) {
                root.selected[el] = true;
            });
            update();
        });


    var timeout = false;
    $('#searchbar')
        .keyup(function() {
            if (timeout) {
                clearTimeout(timeout);
            }
            timeout = setTimeout(function() {
                var searchText = $('#searchbar').val();
                $('#typetree').jstree(true).search(searchText);
            }, 250);
        });

    d3.json("api/dependencies/links", function(jsonLinks) {
        rawLinks = jsonLinks;
        resize();
        update();
        var current = root;
        while (current.children!=null && current.children.length==1) {
            click(current);
            current = current.children[0];
        }
    });
});


function assignColorsToTop20(vertices) {
        var top20 = vertices
            .sort(function (a, b) {
                var diff = b.size - a.size;
                if (diff == 0) return b.children;
                return diff;
            })
            .slice(0, 20);

    var colors20 = d3.scale.category20();

    for (var i = 0; i < top20.length; i++) {
        if (top20[i].size != 1)
            top20[i].color = colors20(i);
    }
}


function getLinks(visibleNodes) {

    var links = [];
    var matrix = [];

    rawLinks.forEach(function (l) {
        function getVisibleSelfOrParent(id) {
            var node = visibleNodes[id];
            if (node) return node;
            node = verticesById[id];
            var parent = node.parent;
            if (parent) return getVisibleSelfOrParent(parent.id);
            return null;
        }

        var source = getVisibleSelfOrParent(l.source, 0);
        var target = getVisibleSelfOrParent(l.target, 0);

        if (!source || !target) return;
        if (!matrix[source.id]) matrix[source.id] = [];
        if (!matrix[source.id][target.id]) {
            matrix[source.id][target.id] = true;
            links.push({ source: source, target: target });
        }
    });
    return links;
}

function update() {
    var nodes = root.leafsDeep();
    var links = getLinks(nodes.indexById());

    force
        .nodes(nodes)
        .links(links)
        .start();

    link = vis
        .selectAll("line.link")
        .data(links);

    link.enter().insert("svg:line", ".node")
        .attr("class", "link")
        .attr("x1", function(d) { return d.source.x; })
        .attr("y1", function(d) { return d.source.y; })
        .attr("x2", function(d) { return d.target.x; })
        .attr("y2", function (d) { return d.target.y; })
        .on("mouseover", lineMouseOver)
        .on("mouseout", lineMouseOut);

    link.exit().remove();

    node = vis.selectAll("circle.node")
        .data(nodes, function(d) { return d.id; })
        .style("fill", function(d) { return d.getColor(); });

    node.enter().append("svg:circle")
        .attr("class", "node")
        .attr("cx", function(d) { return d.x; })
        .attr("cy", function(d) { return d.y; })
        .attr("r", function(d) { return d.getRadius(); })
        .style("fill", function (d) { return d.getColor(); })
        .style("stroke", function(d) { return root.selected[d.id] ? "#ff00ff" : "#fff"; })
        .on("click", click)
        .on("mouseover", vertexMouseOver)
        .on("mouseout", vertexMouseOut)
        .call(force.drag);

    node.exit().remove();
}

function tick() {
    
    link.attr("x1", function(d) { return d.source.x; })
        .attr("y1", function(d) { return d.source.y; })
        .attr("x2", function(d) { return d.target.x; })
        .attr("y2", function(d) { return d.target.y; });

    node.attr("cx", function(d) { return d.x; })
        .attr("cy", function (d) { return d.y; })
        .style("stroke", function(d) { return root.selected[d.id] ? "#ff00ff" : "#fff"; });
}

function lineMouseOver(l) {
    hideTexts();
    showText(l.source);
    showText(l.target);
}

function lineMouseOut(l) {
    hideTextsAsync();
}

function vertexMouseOver(d) {
    hideTexts();
    showText(d);
    var tree = $('#typetree').jstree(true);
    tree.select_node(d, true, false);
}

function vertexMouseOut(d) {
    hideTextsAsync();
    var tree = $('#typetree').jstree(true);
    tree.deselect_node(d, true, false);
}

function showText(d) {
    vis
        .insert("text")
        .attr("class", "hint")
        .attr("x", d.x + d.getRadius() + 1)
        .attr("y", d.y)
        .text(d.fullName());
}

function hideTextsAsync() {
    setTimeout(function() {
        hideTexts();
    }, 500);
}

function hideTexts() {
    vis.selectAll("text.hint").remove();
}

function click(d) {
    d.toggle();
    update();
}

function resize() {
    var viewport = $("#viewport")[0];
    var w = viewport.clientWidth;
    var h = viewport.clientHeight;
    root.x = w / 2;
    root.y = h / 2;
    vis
        .attr("width", w)
        .attr("height", h);

    force
        .size([w, h])
        .resume();
}

$(document).ready(function() {
    var panzoom = $("#viewport svg").panzoom();
    panzoom.parent().on('mousewheel.focal', function(e) {
        e.preventDefault();
        var delta = e.delta || e.originalEvent.wheelDelta;
        var zoomOut = delta ? delta < 0 : e.originalEvent.deltaY > 0;
        panzoom.panzoom('zoom', zoomOut, {
            increment: 0.1,
            focal: e
        });
    });
});