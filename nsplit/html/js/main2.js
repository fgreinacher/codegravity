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
var nodesById = [];

d3.json("api/treeview/deep", function(json) {

    root = json;
    root.fixed = true;
    root.x = w / 2;
    root.y = h / 2 - 80;


    $('#typetree')
        .jstree({
            'core': {
                'data':
                    root
            }
        })
        .on('open_node.jstree', function(e, data) {
            var vertex = nodesById[data.node.original.id];
            if (!vertex.isExpanded) toggleNode(vertex);
        })
        .on('close_node.jstree', function(e, data) {
            var n = data.node;
            if (!n.children) return;
            n.children.forEach(function(el) { data.instance.close_node(el); });
            var vertex = nodesById[n.original.id];
            if (vertex.isExpanded) toggleNode(vertex);
        });


    root.size = normalize(root);

    //set parent, eliminate empty children[], caclculate size - recursively

    function normalize(node) {
        node.size = 1;
        node.isExpanded = false;
        if (node.children) {
            for (var i = 0; i < node.children.length; i++) {
                var child = node.children[i];
                child.parent = node;
                node.size += normalize(child);
            }
            if (node.children.length == 0) node.children = null;
        }
        return node.size;
    }

    var top20 =
        flatten(root, function(n) { return n.children; })
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


function flatten(node, deep) {
    return deep(node)
        ? [node].concat(
            deep(node)
                .reduce(
                    function(s, e) {
                        return s.concat(flatten(e, deep));
                    }, []))
        : [node];
}

;


function getLinks() {

    var links = [];
    var matrix = [];

    var allNodes = flatten(root, function(node) {
        return node.children;
    });

    var visibleNodes = flatten(root, function(node) {
        return node.isExpanded ? node.children : [];
    });

    allNodes = indexNodes(allNodes);
    nodesById = allNodes;
    visibleNodes = indexNodes(visibleNodes);

    function indexNodes(nodes) {
        var nodesByName = [];
        nodes.forEach(function(n) {
            nodesByName[n.id] = n;
        });
        return nodesByName;
    }

    rawLinks.forEach(function(l) {

        function getVisibleNode(id, count) {
            if (count > 100) return null; //TODO Check why ?
            var node = visibleNodes[id];
            if (node) return node;
            node = allNodes[id];
            var parent = node.parent;
            if (parent) return getVisibleNode(parent.id, count + 1);
            return null;
        }

        var source = getVisibleNode(l.source, 0);
        var target = getVisibleNode(l.target, 0);

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
    var nodes = visibleNodes(root),
        links = getLinks(); // d3.layout.tree().links(nodes);

    // Restart the force layout.
    force
        .nodes(nodes)
        .links(links)
        .start();

    // Update the links…
    link = vis.selectAll("line.link")
        //.data(links, function(d) { return d.target.id; });
        .data(links);

    // Enter any new links.
    link.enter().insert("svg:line", ".node")
        .attr("class", "link")
        .attr("x1", function(d) { return d.source.x; })
        .attr("y1", function(d) { return d.source.y; })
        .attr("x2", function(d) { return d.target.x; })
        .attr("y2", function(d) { return d.target.y; });

    // Exit any old links.
    link.exit().remove();

    // Update the nodes…
    node = vis.selectAll("circle.node")
        .data(nodes, function(d) { return d.id; })
        .style("fill", colorD);

    //node.transition()
    //    .attr("r", function (d) { return d.visibleChildren ? 4.5 : Math.sqrt(1000 / root.size * d.size); });

    // Enter any new nodes.
    node.enter().append("svg:circle")
        .attr("class", "node")
        .attr("cx", function(d) { return d.x; })
        .attr("cy", function(d) { return d.y; })
        .attr("r", function(d) { return d.isExpanded ? 4.5 : Math.max(Math.sqrt(1000 / root.size * d.size), 3); })
        .style("fill", colorD)
        .on("click", toggleNode)
        .on("mouseover", mouseOver)
        .call(force.drag);
    //.append("text")
    //.attr("class", "text")
    //.attr("x", 12)
    //.attr("dy", ".35em")
    //.text(function(d) { return d.text; });

    // Exit any old nodes.
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

function colorD(d) {
    return d.color
        ? d.color
        : d.parent.color;
    //return d.children ? color(d.id) : d.visibleChildren ? "#c6dbef" : color(d.parent.id);
}


// Toggle children on click.

function mouseOver(d) {
    var tree = $('#typetree').jstree(true);
    tree.deselect_all();
    tree.select_node(d, true);
}

function toggleNode(d) {
    if (d.isExpanded) implode(d);
    else explode(d);
    d.isExpanded = !d.isExpanded;
    update();
}

function explode(d) {
    if (d.children == null) return;
    var segment = 2 * Math.PI / d.children.length;
    var angle = 0;
    d.children.forEach(function(child) {
        {
            angle += segment;
            child.x = d.x + 10 * Math.sin(angle);
            child.y = d.y + 10 * Math.cos(angle);
        }
    });
}

function implode(d) {
    if (d.children == null) return;
    var sumX = 0;
    var sumY = 0;
    d.children.forEach(function(child) {
        {
            sumX += child.x;
            sumY += child.y;
        }
        d.x = sumX / d.children.length;
        d.Y = sumY / d.children.length;
    });
}

function visibleNodes(node) {
    return node.isExpanded && node.children
        ?
        node
            .children
            .reduce(
                function(s, e) {
                    return s.concat(visibleNodes(e));
                }, [])
        : [node];
}

$(document).ready(function() {

});