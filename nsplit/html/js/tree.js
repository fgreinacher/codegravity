// This code is distributed under MIT license. 
// Copyright (c) 2014 George Mamaladze, Florian Greinacher
// See license.txt or http://opensource.org/licenses/mit-license.php

function Tree(element, jsonTree) {
    var instance = this;
    $(element)
    .jstree({
        "core": {
            "data":
                jsonTree,
            "themes": {
                "icons": false
            }
        },
        "checkbox": {
            "keep_selected_style": false
        },
        "contextmenu": {
            "items": function (n) {
                return {
                    "ExpandSubTree": {
                        "label": "EXPAND ALL",
                        "action": function (obj) {
                            var tree = $('#typetree').jstree(true);
                            tree.open_all(n);
                            tree.deselect_all();
                            if (instance.onOpenCallback != null) {
                                var id = n.original.id;
                                instance.onOpenCallback(id, true);
                            }
                        }
                    },
                    "CollapseSubTree": {
                        "label": "COLLAPSE ALL",
                        "action": function (obj) {
                            var tree = $('#typetree').jstree(true);
                            tree.close_all(n);
                            tree.deselect_all();
                            if (instance.onCloseCallback != null) {
                                var id = n.original.id;
                                instance.onCloseCallback(id, true);
                            }
                        }
                    }
                };
            }
        },
        "plugins": ["checkbox", "contextmenu"]
    })
    .on('open_node.jstree', function (e, data) {
        if (instance.onOpenCallback != null) {
            var id = data.node.original.id;
            instance.onOpenCallback(id, false);
        }
    })
    .on('close_node.jstree', function (e, data) {
        var n = data.node;
        if (!n.children) return;
        n.children.forEach(function (el) { data.instance.close_node(el); });
        if (instance.onCloseCallback != null) {
            var id = data.node.original.id;
            instance.onCloseCallback(id, false);
        }
    })
    .on('changed.jstree', function (e, data) {
        if (instance.onSelectCallback != null) {
            var selected = [];
            data.selected.forEach(function (el) {
                selected[el] = true;
            });
            instance.onSelectCallback(selected);
        }
    });
}

Tree.prototype.onOpen = function (callback) {
    this.onOpenCallback = callback;
    return this;
}

Tree.prototype.onClose = function (callback) {
    this.onCloseCallback = callback;
    return this;
}

Tree.prototype.onSelect = function (callback) {
    this.onSelectCallback = callback;
    return this;
}