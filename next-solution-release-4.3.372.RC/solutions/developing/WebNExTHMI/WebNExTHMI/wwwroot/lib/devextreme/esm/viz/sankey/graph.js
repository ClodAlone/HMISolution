/**
 * DevExtreme (esm/viz/sankey/graph.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
var WHITE = "white";
var GRAY = "gray";
var BLACK = "black";
var routines = {
    maxOfArray: function(arr, callback) {
        var m = 0;
        var callback_function = v => v;
        if (callback) {
            callback_function = callback
        }
        for (var i = 0; i < arr.length; i++) {
            if (callback_function(arr[i]) > m) {
                m = callback_function(arr[i])
            }
        }
        return m
    }
};
var getVertices = function(links) {
    var vert = [];
    links.forEach(link => {
        if (-1 === vert.indexOf(link[0])) {
            vert.push(link[0])
        }
        if (-1 === vert.indexOf(link[1])) {
            vert.push(link[1])
        }
    });
    return vert
};
var getAdjacentVertices = function(links, vertex) {
    var avert = [];
    links.forEach(link => {
        if (link[0] === vertex && -1 === avert.indexOf(link[1])) {
            avert.push(link[1])
        }
    });
    return avert
};
var getReverseAdjacentVertices = function(links, vertex) {
    var avert = [];
    links.forEach(link => {
        if (link[1] === vertex && -1 === avert.indexOf(link[0])) {
            avert.push(link[0])
        }
    });
    return avert
};
var struct = {
    _hasCycle: false,
    _sortedList: [],
    hasCycle: function(links) {
        this._hasCycle = false;
        this._sortedList = [];
        var vertices = {};
        var allVertices = getVertices(links);
        allVertices.forEach(vertex => {
            vertices[vertex] = {
                color: WHITE
            }
        });
        allVertices.forEach(vertex => {
            if (vertices[vertex].color === WHITE) {
                this._depthFirstSearch(links, vertices, vertex)
            }
        });
        this._sortedList.reverse();
        return this._hasCycle
    },
    _depthFirstSearch: function(links, vertices, vertex) {
        vertices[vertex].color = GRAY;
        var averts = getAdjacentVertices(links, vertex);
        for (var a = 0; a < averts.length; a++) {
            if (vertices[averts[a]].color === WHITE) {
                this._depthFirstSearch(links, vertices, averts[a])
            } else if (vertices[averts[a]].color === GRAY) {
                this._hasCycle = true
            }
        }
        this._sortedList.push({
            name: vertex,
            lp: null,
            incoming: getReverseAdjacentVertices(links, vertex),
            outgoing: getAdjacentVertices(links, vertex)
        });
        vertices[vertex].color = BLACK
    },
    computeLongestPaths(links) {
        var sortedVertices = this._sortedList;
        sortedVertices.forEach(vertex => {
            var averts = getReverseAdjacentVertices(links, vertex.name);
            if (0 === averts.length) {
                vertex.lp = 0
            } else {
                var maxLP = [];
                averts.forEach(adjacentVertex => {
                    maxLP.push(sortedVertices.filter(sv => sv.name === adjacentVertex)[0].lp)
                });
                vertex.lp = routines.maxOfArray(maxLP) + 1
            }
        });
        return this._sortedList
    }
};
export default {
    struct: struct,
    routines: routines,
    getVertices: getVertices,
    getAdjacentVertices: getAdjacentVertices,
    getReverseAdjacentVertices: getReverseAdjacentVertices
};
