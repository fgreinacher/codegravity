// This code is distributed under MIT license. 
// Copyright (c) 2014 George Mamaladze, Florian Greinacher
// See license.txt or http://opensource.org/licenses/mit-license.php

var w = 1280,
    h = 800,
    node,
    link,
    root;

var force = d3.layout.force()
    .on("tick", tick)
    .size([w, h - 160]);

var vis = d3.select("#viewport")
    .append("svg:svg")
    .attr("width", w)
    .attr("height", h);

var root;
var rawLinks = [];
var verticesById = [];

d3.json("api/treeview/deep", function(jsonRoot) {

    root = new Vertex(jsonRoot);
    root.fixed = true;
    root.x = w / 2;
    root.y = h / 2 - 80;

    verticesById = root
        .subtree()
        .indexById();

    $('#typetree')
        .jstree({
            'core': {
                'data':
                   jsonRoot
            }
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
        });


    var top20 = verticesById
            .slice(0)
            .sort(function(n) { return n.children ? -n.size : 0; })
            .slice(0, 20);

    var colors20 = d3.scale.category20();

    for (var i = 0; i < top20.length; i++) {
        if (top20[i].size != 1)
            top20[i].color = colors20(i);
    }

    d3.json("api/dependencies/links", function(jsonLinks) {
        rawLinks = jsonLinks;
        update();
    });
});


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
        .attr("y2", function(d) { return d.target.y; });

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
        .on("click", click)
        .on("mouseover", mouseOver)
        .call(force.drag);

    node.exit().remove();
}

function tick() {
    link.attr("x1", function(d) { return d.source.x; })
        .attr("y1", function(d) { return d.source.y; })
        .attr("x2", function(d) { return d.target.x; })
        .attr("y2", function(d) { return d.target.y; });

    node.attr("cx", function(d) { return d.x; })
        .attr("cy", function(d) { return d.y; });
}

function mouseOver(d) {
    var tree = $('#typetree').jstree(true);
    tree.deselect_all();
    tree.select_node(d, true);
}

function click(d) {
    d.toggle();
    update();
}