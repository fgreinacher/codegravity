// This code is distributed under MIT license. 
// Copyright (c) 2014 George Mamaladze, Florian Greinacher
// See license.txt or http://opensource.org/licenses/mit-license.php

function Vertex(node) {

    var vertex = this;
    //copy all properties
    Object.keys(node).forEach(function (key) {
        vertex[key] = node[key];
    });
    
    if (node.children == null || node.children.length == 0) {
        vertex.size = 1;
        this.children = null;
        return vertex;
    }

    vertex.children = [];
    node.children.forEach(function(childNode) {
        var childVertex = new Vertex(childNode);
        childVertex.parent = vertex;
        vertex.children.push(childVertex);
    });

    vertex.adjustSizeDeep();
    return vertex;
}


Vertex.prototype.adjustSizeDeep = function() {
    if (!this.children) return this.size;
    var sumSize = 0;
    this.children.forEach(function (child) {
        sumSize += child.adjustSizeDeep();
    });
    return this.size = sumSize;
};

Array.prototype.indexById = function() {
    var arrayById = [];
    this.forEach(function(n) {
        arrayById[n.id] = n;
    });
    return arrayById;
};

Vertex.prototype.leafsDeep = function() {
    return (this.isExpanded && this.children)
        ? this.children
            .reduce(
                function(s, e) {
                    return s.concat(e.leafsDeep());
                }, [])
        : [this];
};

Vertex.prototype.subtree = function() {
    return this.flatten(function(node) { return node.children; });
};

Vertex.prototype.visibleSubtree = function () {
    return this.flatten(function (node) { return node.isExpanded ? node.children : []; });
};

Vertex.prototype.flatten = function(deep) {
    return deep(this)
        ? [this].concat(
            deep(this)
                .reduce(
                    function(s, e) {
                        return s.concat(e.flatten(deep));
                    }, []))
        : [this];
};

Vertex.prototype.toggle = function() {
    if (this.isExpanded) this.implode();
    else this.explode();
};

Vertex.prototype.explode = function() {
    if (this.children == null) return;
    var vertex=this;
    var radius = this.getRadius();
    this.children.forEach(function(child) {
        {
            child.x = vertex.x + radius * (Math.random() - .5);
            child.y = vertex.y + radius * (Math.random() - .5);
        }
    });
    this.isExpanded = true;
};

Vertex.prototype.implode = function() {
    if (this.children == null) return;
    var sumX = 0;
    var sumY = 0;
    this.children.forEach(function(child) {
        {
            sumX += child.x;
            sumY += child.y;
        }
    });
    this.x = sumX / this.children.length;
    this.Y = sumY / this.children.length;
    this.isExpanded = false;
};

Vertex.prototype.getRadius = function() {
    return this.isExpanded
        ? 4.5
        : Math.max(Math.sqrt(1000 / root.size * this.size), 3);
};


Vertex.prototype.getColor = function() {
    return this.isSelected 
            ? "#000"
            : this.color
                ? this.color
                : this.parent.color;
};

Vertex.prototype.getClass = function() {
    return this.isSelected
        ? "selectedNode"
        : "node";
};

Vertex.prototype.setIsSelectedDeep = function(isSelected) {
    this.isSelected = isSelected;
    if (!this.children) return;
    this.children.forEach(function(child) { child.setIsSelectedDeep(isSelected); });
};


Vertex.prototype.fullName = function () {
    var current = this;
    var path = [];
    while (current != null && current.parent!=null) {
        path.push(current.text);
        current = current.parent;
    }
    return path.reverse().join(".");
};
