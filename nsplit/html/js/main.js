// This code is distributed under MIT license. 
// Copyright (c) 2014 George Mamaladze, Florian Greinacher
// See license.txt or http://opensource.org/licenses/mit-license.php
var node;
var link;
var root;
var rawLinks = [];
var verticesById = [];

var toolTip = d3.select(document.getElementById("toolTip"));
var vis = d3.select("#viewport").append("svg:svg");
window.onresize = resize;

var force = d3.layout
    .force()
    .charge(-30)
    .linkStrength(.1)
    .linkDistance(function (link) { return 2 * (link.source.getRadius() + link.target.getRadius()); })
    .on("tick", tick);

function get(name) {
    if (name = (new RegExp('[?&]' + encodeURIComponent(name) + '=([^&]*)')).exec(location.search))
        return decodeURIComponent(name[1]);
    else return "";
}

d3.json("api/dependencies/graph?name=" + get("name"), function(json) {

    root = new Vertex(json.tree);
    root.fixed = true;
    root.selected = [];

    var vertices = root.subtree();
    assignColorsToTop20(vertices);

    verticesById = vertices.indexById();

    new Tree("#typetree", json.tree)
        .onOpen(function(id, deep) {
            var vertex = verticesById[id];
            vertex.toggle(true, deep);
            update();
        })
        .onClose(function(id, deep) {
            var vertex = verticesById[id];
            vertex.toggle(false, deep);
            update();
        })
        .onSelect(function(ids) {
            root.selected = ids;
            update();
        });

    rawLinks = json.links;
    resize();
    update();
});


function assignColorsToTop20(vertices) {
    var top20 = vertices
        .sort(function(a, b) {
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

    rawLinks.forEach(function(l) {

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
        .attr("y2", function(d) { return d.target.y; });

    link.exit().remove();

    node = vis.selectAll("circle.node")
        .data(nodes, function(d) { return d.id; })
        .style("fill", function(d) { return d.getColor(); });

    node.exit().remove();

    node.enter()
        .append("svg:circle")
        .attr("cx", function(d) { return d.x; })
        .attr("cy", function(d) { return d.y; })
        .attr("class", "node")
        .attr("r", function(d) { return d.getRadius(); })
        .style("fill", function(d) { return d.getColor(); })
        .style("stroke", function(d) { return root.selected[d.id] ? "#111" : "#fff"; })
        .on("click", click)
        .on("mouseover", vertexMouseOver)
        .on("mouseout", vertexMouseOut)
        .call(force.drag);
}

function tick() {

    link.attr("x1", function(d) { return d.source.x; })
        .attr("y1", function(d) { return d.source.y; })
        .attr("x2", function(d) { return d.target.x; })
        .attr("y2", function(d) { return d.target.y; });

    node.attr("cx", function(d) { return d.x; })
        .attr("cy", function(d) { return d.y; })
        .style("stroke", function(d) { return root.selected[d.id] ? "#111" : "#fff"; });
}

function vertexMouseOver(d) {
    panzoom.panzoom("option", "disablePan", true);
    showText(d);
    var tree = $('#typetree').jstree(true);
    tree.select_node(d, true, false);
}

function vertexMouseOut(d) {
    var tree = $('#typetree').jstree(true);
    tree.deselect_node(d, true, false);
    hideText();
    panzoom.panzoom("option", "disablePan", false);
}

function showText(d) {
    toolTip
        .transition()
        .duration(200)
        .style("opacity", ".9")
        .style("left", (d3.event.pageX + 35) + "px")
        .style("top", (d3.event.pageY - 35) + "px");

    toolTip.select("#toolTipHead")
        .text(d.text);

    toolTip.select("#toolTipBody")
        .text(d.fullName());
}

function hideText() {
    toolTip
        .transition()
        .duration(500)
        .style("opacity", "0")
        .transition();
}

function click(d) {
    if (d3.event.defaultPrevented) return;
    d.toggle(!d.isExpanded, false);
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

var panzoom;
$(document).ready(function() {
    panzoom = $("#viewport svg").panzoom();
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