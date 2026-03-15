/**
 * DevExtreme (cjs/viz/tree_map/tree_map.base.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.default = void 0;
var _common = require("./common");
var _node = _interopRequireDefault(require("./node"));
var _tiling = require("./tiling");
var _colorizing = require("./colorizing");
var _utils = require("../core/utils");
var _common2 = require("../../core/utils/common");
var _base_widget = _interopRequireDefault(require("../core/base_widget"));
require("./tiling.squarified");
require("./colorizing.discrete");
var _component_registrator = _interopRequireDefault(require("../../core/component_registrator"));
var _data_source = require("../core/data_source");

function _interopRequireDefault(obj) {
    return obj && obj.__esModule ? obj : {
        default: obj
    }
}
var _max = Math.max;
var directions = {
    lefttoprightbottom: [1, 1],
    leftbottomrighttop: [1, -1],
    righttopleftbottom: [-1, 1],
    rightbottomlefttop: [-1, -1]
};
(0, _tiling.setDefaultAlgorithm)("squarified");
(0, _colorizing.setDefaultColorizer)("discrete");

function pickPositiveInteger(val) {
    return val > 0 ? Math.round(val) : 0
}
var dxTreeMap = _base_widget.default.inherit({
    _handlers: {
        beginBuildNodes: _common2.noop,
        buildNode: _common2.noop,
        endBuildNodes: _common2.noop,
        setTrackerData: _common2.noop,
        calculateState: function(options) {
            return (0, _common.buildRectAppearance)(options)
        }
    },
    _rootClass: "dxtm-tree-map",
    _rootClassPrefix: "dxtm",
    _getDefaultSize: function() {
        return {
            width: 400,
            height: 400
        }
    },
    _themeSection: "treeMap",
    _fontFields: ["tile.label.font", "group.label.font"],
    _init: function() {
        var that = this;
        that._rectOffsets = {};
        that._handlers = Object.create(that._handlers);
        that._context = {
            suspend: function() {
                if (!that._applyingChanges) {
                    that._suspendChanges()
                }
            },
            resume: function() {
                if (!that._applyingChanges) {
                    that._resumeChanges()
                }
            },
            change: function(codes) {
                that._change(codes)
            },
            settings: [{}, {}],
            calculateState: that._handlers.calculateState,
            calculateLabelState: _common.buildTextAppearance
        };
        that._root = that._topNode = {
            nodes: []
        };
        that.callBase.apply(that, arguments)
    },
    _initialChanges: ["DATA_SOURCE"],
    _initCore: function() {
        var renderer = this._renderer;
        this._createProxyType();
        this._tilesGroup = renderer.g().linkOn(renderer.root, "tiles").linkAppend();
        this._labelsGroup = renderer.g().linkOn(renderer.root, "labels").linkAppend()
    },
    _createProxyType: _common2.noop,
    _disposeCore: function() {
        this._filter && this._filter.dispose();
        this._labelsGroup.linkOff();
        this._tilesGroup.linkOff()
    },
    _applySize: function(rect) {
        this._tilingRect = rect.slice();
        this._change(["TILING"])
    },
    _optionChangesMap: {
        dataSource: "DATA_SOURCE",
        valueField: "NODES_CREATE",
        childrenField: "NODES_CREATE",
        colorField: "TILES",
        colorizer: "TILES",
        labelField: "LABELS",
        tile: "TILE_SETTINGS",
        group: "GROUP_SETTINGS",
        maxDepth: "MAX_DEPTH",
        layoutAlgorithm: "TILING",
        layoutDirection: "TILING"
    },
    _themeDependentChanges: ["TILE_SETTINGS", "GROUP_SETTINGS", "MAX_DEPTH"],
    _changeDataSource: function() {
        this._isDataExpected = this._isSyncData = true;
        this._updateDataSource();
        this._isSyncData = false;
        if (this._isDataExpected) {
            this._suspendChanges()
        }
    },
    _dataSourceChangedHandler: function() {
        if (this._isDataExpected) {
            this._isDataExpected = false;
            this._change(["NODES_CREATE"]);
            if (!this._isSyncData) {
                this._resumeChanges()
            }
        } else {
            this._requestChange(["NODES_CREATE"])
        }
    },
    _optionChangesOrder: ["DATA_SOURCE", "TILE_SETTINGS", "GROUP_SETTINGS", "MAX_DEPTH"],
    _change_DATA_SOURCE: function() {
        this._changeDataSource()
    },
    _change_TILE_SETTINGS: function() {
        this._changeTileSettings()
    },
    _change_GROUP_SETTINGS: function() {
        this._changeGroupSettings()
    },
    _change_MAX_DEPTH: function() {
        this._changeMaxDepth()
    },
    _customChangesOrder: ["NODES_CREATE", "NODES_RESET", "TILES", "LABELS", "TILING", "LABELS_LAYOUT"],
    _change_NODES_CREATE: function() {
        this._buildNodes()
    },
    _change_NODES_RESET: function() {
        this._resetNodes()
    },
    _change_TILES: function() {
        this._applyTilesAppearance()
    },
    _change_LABELS: function() {
        this._applyLabelsAppearance()
    },
    _change_TILING: function() {
        this._performTiling()
    },
    _change_LABELS_LAYOUT: function() {
        this._performLabelsLayout()
    },
    _applyChanges: function() {
        var that = this;
        that.callBase.apply(that, arguments);
        if (!that._isDataExpected) {
            that._drawn()
        }
        that._context.forceReset = false
    },
    _buildNodes: function() {
        var root = this._root = this._topNode = new _node.default;
        root._id = 0;
        root.parent = {};
        root.data = {};
        root.level = root.index = -1;
        root.ctx = this._context;
        root.label = null;
        this._nodes = [root];
        this._handlers.beginBuildNodes();
        var processedData = this._processDataSourceItems(this._dataSourceItems() || []);
        traverseDataItems(root, processedData.items, 0, {
            itemsField: !processedData.isPlain && this._getOption("childrenField", true) || "items",
            valueField: this._getOption("valueField", true) || "value",
            buildNode: this._handlers.buildNode,
            ctx: this._context,
            nodes: this._nodes
        });
        this._onNodesCreated();
        this._handlers.endBuildNodes();
        this._change(["NODES_RESET"])
    },
    _onNodesCreated: _common2.noop,
    _processDataSourceItems: function(items) {
        return {
            items: items,
            isPlain: false
        }
    },
    _changeTileSettings: function() {
        var options = this._getOption("tile");
        var offsets = this._rectOffsets;
        var borderWidth = pickPositiveInteger(options.border.width);
        var edgeOffset = borderWidth / 2;
        var innerOffset = 1 & borderWidth ? .5 : 0;
        var labelOptions = options.label;
        var settings = this._context.settings[0];
        this._change(["TILES", "LABELS"]);
        settings.state = this._handlers.calculateState(options);
        this._filter = this._filter || this._renderer.shadowFilter("-50%", "-50%", "200%", "200%");
        this._filter.attr(labelOptions.shadow);
        this._calculateLabelSettings(settings, labelOptions, this._filter.id);
        if (offsets.tileEdge !== edgeOffset || offsets.tileInner !== innerOffset) {
            offsets.tileEdge = edgeOffset;
            offsets.tileInner = innerOffset;
            this._change(["TILING"])
        }
    },
    _changeGroupSettings: function() {
        var options = this._getOption("group");
        var labelOptions = options.label;
        var offsets = this._rectOffsets;
        var borderWidth = pickPositiveInteger(options.border.width);
        var edgeOffset = borderWidth / 2;
        var innerOffset = 1 & borderWidth ? .5 : 0;
        var headerHeight = 0;
        var groupPadding = pickPositiveInteger(options.padding);
        var settings = this._context.settings[1];
        this._change(["TILES", "LABELS"]);
        settings.state = this._handlers.calculateState(options);
        this._calculateLabelSettings(settings, labelOptions);
        if (options.headerHeight >= 0) {
            headerHeight = pickPositiveInteger(options.headerHeight)
        } else {
            headerHeight = settings.labelParams.height + 2 * pickPositiveInteger(labelOptions.paddingTopBottom)
        }
        if (this._headerHeight !== headerHeight) {
            this._headerHeight = headerHeight;
            this._change(["TILING"])
        }
        if (this._groupPadding !== groupPadding) {
            this._groupPadding = groupPadding;
            this._change(["TILING"])
        }
        if (offsets.headerEdge !== edgeOffset || offsets.headerInner !== innerOffset) {
            offsets.headerEdge = edgeOffset;
            offsets.headerInner = innerOffset;
            this._change(["TILING"])
        }
    },
    _calculateLabelSettings: function(settings, options, filter) {
        var bBox = this._getTextBBox(options.font);
        var paddingLeftRight = pickPositiveInteger(options.paddingLeftRight);
        var paddingTopBottom = pickPositiveInteger(options.paddingTopBottom);
        var tileLabelOptions = this._getOption("tile.label");
        var groupLabelOptions = this._getOption("group.label");
        settings.labelState = (0, _common.buildTextAppearance)(options, filter);
        settings.labelState.visible = !("visible" in options) || !!options.visible;
        settings.labelParams = {
            height: bBox.height,
            rtlEnabled: this._getOption("rtlEnabled", true),
            paddingTopBottom: paddingTopBottom,
            paddingLeftRight: paddingLeftRight,
            tileLabelWordWrap: tileLabelOptions.wordWrap,
            tileLabelOverflow: tileLabelOptions.textOverflow,
            groupLabelOverflow: groupLabelOptions.textOverflow
        }
    },
    _changeMaxDepth: function() {
        var maxDepth = this._getOption("maxDepth", true);
        maxDepth = maxDepth >= 1 ? Math.round(maxDepth) : 1 / 0;
        if (this._maxDepth !== maxDepth) {
            this._maxDepth = maxDepth;
            this._change(["NODES_RESET"])
        }
    },
    _resetNodes: function() {
        this._tilesGroup.clear();
        this._renderer.initHatching();
        this._context.forceReset = true;
        this._context.minLevel = this._topNode.level + 1;
        this._context.maxLevel = this._context.minLevel + this._maxDepth - 1;
        this._change(["TILES", "LABELS", "TILING"])
    },
    _processNodes: function(context, process) {
        processNodes(context, this._topNode, process)
    },
    _applyTilesAppearance: function() {
        var colorizer = (0, _colorizing.getColorizer)(this._getOption("colorizer"), this._themeManager, this._topNode);
        this._processNodes({
            renderer: this._renderer,
            group: this._tilesGroup,
            setTrackerData: this._handlers.setTrackerData,
            colorField: this._getOption("colorField", true) || "color",
            getColor: colorizer
        }, processTileAppearance)
    },
    _applyLabelsAppearance: function() {
        this._labelsGroup.clear();
        this._processNodes({
            renderer: this._renderer,
            group: this._labelsGroup,
            setTrackerData: this._handlers.setTrackerData,
            labelField: this._getOption("labelField", true) || "name"
        }, processLabelAppearance);
        this._change(["LABELS_LAYOUT"])
    },
    _performTiling: function() {
        var context = {
            algorithm: (0, _tiling.getAlgorithm)(this._getOption("layoutAlgorithm", true)),
            directions: directions[String(this._getOption("layoutDirection", true)).toLowerCase()] || directions.lefttoprightbottom,
            headerHeight: this._headerHeight,
            groupPadding: this._groupPadding,
            rectOffsets: this._rectOffsets
        };
        this._topNode.innerRect = this._tilingRect;
        calculateRects(context, this._topNode);
        this._processNodes(context, processTiling);
        this._change(["LABELS_LAYOUT"]);
        this._onTilingPerformed()
    },
    _onTilingPerformed: _common2.noop,
    _performLabelsLayout: function() {
        this._processNodes(null, processLabelsLayout)
    },
    _getTextBBox: function(fontOptions) {
        var renderer = this._renderer;
        var text = this._textForCalculations || renderer.text("0", 0, 0);
        this._textForCalculations = text;
        text.css((0, _utils.patchFontOptions)(fontOptions)).append(renderer.root);
        var bBox = text.getBBox();
        text.remove();
        return bBox
    }
});

function traverseDataItems(root, dataItems, level, params) {
    var nodes = [];
    var allNodes = params.nodes;
    var node;
    var i;
    var ii = dataItems.length;
    var dataItem;
    var totalValue = 0;
    var items;
    for (i = 0; i < ii; ++i) {
        var _items;
        dataItem = dataItems[i];
        node = new _node.default;
        node._id = allNodes.length;
        node.ctx = params.ctx;
        node.parent = root;
        node.level = level;
        node.index = nodes.length;
        node.data = dataItem;
        params.buildNode(node);
        allNodes.push(node);
        nodes.push(node);
        items = dataItem[params.itemsField];
        if (null !== (_items = items) && void 0 !== _items && _items.length) {
            traverseDataItems(node, items, level + 1, params)
        }
        if (dataItem[params.valueField] > 0) {
            node.value = Number(dataItem[params.valueField])
        }
        totalValue += node.value
    }
    root.nodes = nodes;
    root.value = totalValue
}

function processNodes(context, root, process) {
    var nodes = root.nodes;
    var node;
    var i;
    var ii = nodes.length;
    for (i = 0; i < ii; ++i) {
        node = nodes[i];
        process(context, node);
        if (node.isNode()) {
            processNodes(context, node, process)
        }
    }
}
var createTile = [createLeaf, createGroup];

function processTileAppearance(context, node) {
    node.color = node.data[context.colorField] || context.getColor(node) || node.parent.color;
    node.updateStyles();
    node.tile = !node.ctx.forceReset && node.tile || createTile[Number(node.isNode())](context, node);
    node.applyState()
}

function createLeaf(context, node) {
    var tile = context.renderer.simpleRect().append(context.group);
    context.setTrackerData(node, tile);
    return tile
}

function createGroup(context, node) {
    var outer = context.renderer.simpleRect().append(context.group);
    var inner = context.renderer.simpleRect().append(context.group);
    context.setTrackerData(node, inner);
    return {
        outer: outer,
        inner: inner
    }
}

function processLabelAppearance(context, node) {
    node.updateLabelStyle();
    if (node.labelState.visible) {
        createLabel(context, node, node.labelState, node.labelParams)
    }
}

function createLabel(context, currentNode, settings, params) {
    var textData = currentNode.data[context.labelField];
    currentNode.label = textData ? String(textData) : null;
    textData = currentNode.customLabel || currentNode.label;
    if (textData) {
        currentNode.text = context.renderer.text(textData).attr(settings.attr).css(settings.css).append(context.group);
        context.setTrackerData(currentNode, currentNode.text)
    }
}
var emptyRect = [0, 0, 0, 0];

function calculateRects(context, root) {
    var nodes = root.nodes;
    var items = [];
    var rects = [];
    var sum = 0;
    var i;
    var ii = items.length = rects.length = nodes.length;
    for (i = 0; i < ii; ++i) {
        sum += nodes[i].value;
        items[i] = {
            value: nodes[i].value,
            i: i
        }
    }
    if (sum > 0) {
        context.algorithm({
            items: items.slice(),
            sum: sum,
            rect: root.innerRect.slice(),
            isRotated: 1 & nodes[0].level,
            directions: context.directions
        })
    }
    for (i = 0; i < ii; ++i) {
        rects[i] = items[i].rect || emptyRect
    }
    root.rects = rects
}

function processTiling(context, node) {
    var rect = node.parent.rects[node.index];
    var rectOffsets = context.rectOffsets;
    var headerHeight;
    if (node.isNode()) {
        setRectAttrs(node.tile.outer, buildTileRect(rect, node.parent.innerRect, rectOffsets.headerEdge, rectOffsets.headerInner));
        rect = marginateRect(rect, context.groupPadding);
        headerHeight = Math.min(context.headerHeight, rect[3] - rect[1]);
        node.rect = [rect[0], rect[1], rect[2], rect[1] + headerHeight];
        setRectAttrs(node.tile.inner, marginateRect(node.rect, rectOffsets.headerEdge));
        rect[1] += headerHeight;
        node.innerRect = rect;
        calculateRects(context, node)
    } else {
        node.rect = rect;
        setRectAttrs(node.tile, buildTileRect(rect, node.parent.innerRect, rectOffsets.tileEdge, rectOffsets.tileInner))
    }
}

function marginateRect(rect, margin) {
    return [rect[0] + margin, rect[1] + margin, rect[2] - margin, rect[3] - margin]
}

function buildTileRect(rect, outer, edgeOffset, innerOffset) {
    return [rect[0] + (rect[0] === outer[0] ? edgeOffset : +innerOffset), rect[1] + (rect[1] === outer[1] ? edgeOffset : +innerOffset), rect[2] - (rect[2] === outer[2] ? edgeOffset : -innerOffset), rect[3] - (rect[3] === outer[3] ? edgeOffset : -innerOffset)]
}

function setRectAttrs(element, rect) {
    element.attr({
        x: rect[0],
        y: rect[1],
        width: _max(rect[2] - rect[0], 0),
        height: _max(rect[3] - rect[1], 0)
    })
}

function processLabelsLayout(context, node) {
    if (node.text && node.labelState.visible) {
        layoutTextNode(node, node.labelParams)
    }
}

function layoutTextNode(node, params) {
    var rect = node.rect;
    var text = node.text;
    var bBox = text.getBBox();
    var paddingLeftRight = params.paddingLeftRight;
    var paddingTopBottom = params.paddingTopBottom;
    var effectiveWidth = rect[2] - rect[0] - 2 * paddingLeftRight;
    text.setMaxSize(effectiveWidth, rect[3] - rect[1] - paddingTopBottom, node.isNode() ? {
        textOverflow: params.groupLabelOverflow,
        wordWrap: "none"
    } : {
        textOverflow: params.tileLabelOverflow,
        wordWrap: params.tileLabelWordWrap,
        hideOverflowEllipsis: true
    });
    text.move(params.rtlEnabled ? rect[2] - paddingLeftRight - bBox.x - bBox.width : rect[0] + paddingLeftRight - bBox.x, rect[1] + paddingTopBottom - bBox.y)
}(0, _component_registrator.default)("dxTreeMap", dxTreeMap);
var _default = dxTreeMap;
exports.default = _default;
dxTreeMap.addPlugin(_data_source.plugin);
module.exports = exports.default;
module.exports.default = exports.default;
