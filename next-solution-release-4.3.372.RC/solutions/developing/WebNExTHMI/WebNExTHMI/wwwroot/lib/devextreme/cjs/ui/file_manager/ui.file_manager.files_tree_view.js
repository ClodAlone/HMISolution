/**
 * DevExtreme (cjs/ui/file_manager/ui.file_manager.files_tree_view.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.default = void 0;
var _renderer = _interopRequireDefault(require("../../core/renderer"));
var _extend = require("../../core/utils/extend");
var _icon = require("../../core/utils/icon");
var _common = require("../../core/utils/common");
var _ui = _interopRequireDefault(require("../widget/ui.widget"));
var _uiTree_view = _interopRequireDefault(require("../tree_view/ui.tree_view.search"));
var _uiFile_manager = _interopRequireDefault(require("./ui.file_manager.file_actions_button"));
var _deferred = require("../../core/utils/deferred");
var _window = require("../../core/utils/window");
var _type = require("../../core/utils/type");

function _interopRequireDefault(obj) {
    return obj && obj.__esModule ? obj : {
        default: obj
    }
}

function _toConsumableArray(arr) {
    return _arrayWithoutHoles(arr) || _iterableToArray(arr) || _unsupportedIterableToArray(arr) || _nonIterableSpread()
}

function _nonIterableSpread() {
    throw new TypeError("Invalid attempt to spread non-iterable instance.\nIn order to be iterable, non-array objects must have a [Symbol.iterator]() method.")
}

function _unsupportedIterableToArray(o, minLen) {
    if (!o) {
        return
    }
    if ("string" === typeof o) {
        return _arrayLikeToArray(o, minLen)
    }
    var n = Object.prototype.toString.call(o).slice(8, -1);
    if ("Object" === n && o.constructor) {
        n = o.constructor.name
    }
    if ("Map" === n || "Set" === n) {
        return Array.from(o)
    }
    if ("Arguments" === n || /^(?:Ui|I)nt(?:8|16|32)(?:Clamped)?Array$/.test(n)) {
        return _arrayLikeToArray(o, minLen)
    }
}

function _iterableToArray(iter) {
    if ("undefined" !== typeof Symbol && null != iter[Symbol.iterator] || null != iter["@@iterator"]) {
        return Array.from(iter)
    }
}

function _arrayWithoutHoles(arr) {
    if (Array.isArray(arr)) {
        return _arrayLikeToArray(arr)
    }
}

function _arrayLikeToArray(arr, len) {
    if (null == len || len > arr.length) {
        len = arr.length
    }
    for (var i = 0, arr2 = new Array(len); i < len; i++) {
        arr2[i] = arr[i]
    }
    return arr2
}

function _defineProperties(target, props) {
    for (var i = 0; i < props.length; i++) {
        var descriptor = props[i];
        descriptor.enumerable = descriptor.enumerable || false;
        descriptor.configurable = true;
        if ("value" in descriptor) {
            descriptor.writable = true
        }
        Object.defineProperty(target, descriptor.key, descriptor)
    }
}

function _createClass(Constructor, protoProps, staticProps) {
    if (protoProps) {
        _defineProperties(Constructor.prototype, protoProps)
    }
    if (staticProps) {
        _defineProperties(Constructor, staticProps)
    }
    Object.defineProperty(Constructor, "prototype", {
        writable: false
    });
    return Constructor
}

function _inheritsLoose(subClass, superClass) {
    subClass.prototype = Object.create(superClass.prototype);
    subClass.prototype.constructor = subClass;
    _setPrototypeOf(subClass, superClass)
}

function _setPrototypeOf(o, p) {
    _setPrototypeOf = Object.setPrototypeOf || function(o, p) {
        o.__proto__ = p;
        return o
    };
    return _setPrototypeOf(o, p)
}
var FILE_MANAGER_DIRS_TREE_CLASS = "dx-filemanager-dirs-tree";
var FILE_MANAGER_DIRS_TREE_FOCUSED_ITEM_CLASS = "dx-filemanager-focused-item";
var FILE_MANAGER_DIRS_TREE_ITEM_TEXT_CLASS = "dx-filemanager-dirs-tree-item-text";
var TREE_VIEW_ITEM_CLASS = "dx-treeview-item";
var FileManagerFilesTreeView = function(_Widget) {
    _inheritsLoose(FileManagerFilesTreeView, _Widget);

    function FileManagerFilesTreeView() {
        return _Widget.apply(this, arguments) || this
    }
    var _proto = FileManagerFilesTreeView.prototype;
    _proto._initMarkup = function() {
        var _this = this;
        this._initActions();
        this._getCurrentDirectory = this.option("getCurrentDirectory");
        this._createFileActionsButton = _common.noop;
        this._storeExpandedState = this.option("storeExpandedState") || false;
        var $treeView = (0, _renderer.default)("<div>").addClass(FILE_MANAGER_DIRS_TREE_CLASS).appendTo(this.$element());
        var treeViewOptions = {
            dataStructure: "plain",
            rootValue: "",
            createChildren: this._onFilesTreeViewCreateSubDirectories.bind(this),
            itemTemplate: this._createFilesTreeViewItemTemplate.bind(this),
            keyExpr: "getInternalKey",
            parentIdExpr: "parentDirectory.getInternalKey",
            displayExpr: function(itemInfo) {
                return itemInfo.getDisplayName()
            },
            hasItemsExpr: "fileItem.hasSubDirectories",
            onItemClick: function(e) {
                return _this._actions.onDirectoryClick(e)
            },
            onItemExpanded: function(e) {
                return _this._onFilesTreeViewItemExpanded(e)
            },
            onItemCollapsed: function(e) {
                return _this._onFilesTreeViewItemCollapsed(e)
            },
            onItemRendered: function(e) {
                return _this._onFilesTreeViewItemRendered(e)
            },
            onContentReady: function() {
                return _this._actions.onFilesTreeViewContentReady()
            }
        };
        if (this._contextMenu) {
            this._contextMenu.option("onContextMenuHidden", (function() {
                return _this._onContextMenuHidden()
            }));
            treeViewOptions.onItemContextMenu = function(e) {
                return _this._onFilesTreeViewItemContextMenu(e)
            };
            this._createFileActionsButton = function(element, options) {
                return _this._createComponent(element, _uiFile_manager.default, options)
            }
        }
        this._filesTreeView = this._createComponent($treeView, _uiTree_view.default, treeViewOptions)
    };
    _proto._initActions = function() {
        this._actions = {
            onDirectoryClick: this._createActionByOption("onDirectoryClick"),
            onFilesTreeViewContentReady: this._createActionByOption("onFilesTreeViewContentReady")
        }
    };
    _proto._render = function() {
        _Widget.prototype._render.call(this);
        var that = this;
        setTimeout((function() {
            that._updateFocusedElement()
        }))
    };
    _proto._onFilesTreeViewCreateSubDirectories = function(rootItem) {
        var getDirectories = this.option("getDirectories");
        var directoryInfo = rootItem && rootItem.itemData || null;
        return getDirectories && getDirectories(directoryInfo, true)
    };
    _proto._onFilesTreeViewItemRendered = function(_ref) {
        var itemData = _ref.itemData;
        var currentDirectory = this._getCurrentDirectory();
        if (currentDirectory && currentDirectory.fileItem.equals(itemData.fileItem)) {
            this._updateFocusedElement();
            this._restoreScrollTopPosition()
        }
    };
    _proto._onFilesTreeViewItemExpanded = function(_ref2) {
        var itemData = _ref2.itemData;
        if (this._storeExpandedState) {
            itemData.expanded = true
        }
    };
    _proto._onFilesTreeViewItemCollapsed = function(_ref3) {
        var itemData = _ref3.itemData;
        if (this._storeExpandedState) {
            itemData.expanded = false
        }
    };
    _proto._createFilesTreeViewItemTemplate = function(itemData, itemIndex, itemElement) {
        var _this2 = this;
        var $itemElement = (0, _renderer.default)(itemElement);
        var $itemWrapper = $itemElement.closest(this._filesTreeViewItemSelector);
        $itemWrapper.data("item", itemData);
        var $image = (0, _icon.getImageContainer)(itemData.icon);
        var $text = (0, _renderer.default)("<span>").text(itemData.getDisplayName()).addClass(FILE_MANAGER_DIRS_TREE_ITEM_TEXT_CLASS);
        var $button = (0, _renderer.default)("<div>");
        $itemElement.append($image, $text, $button);
        this._createFileActionsButton($button, {
            onClick: function(e) {
                return _this2._onFileItemActionButtonClick(e)
            }
        })
    };
    _proto._onFilesTreeViewItemContextMenu = function(_ref4) {
        var itemElement = _ref4.itemElement,
            event = _ref4.event;
        event.preventDefault();
        event.stopPropagation();
        var itemData = (0, _renderer.default)(itemElement).data("item");
        this._contextMenu.showAt([itemData], itemElement, event, {
            itemData: itemData,
            itemElement: itemElement
        })
    };
    _proto._onFileItemActionButtonClick = function(_ref5) {
        var component = _ref5.component,
            element = _ref5.element,
            event = _ref5.event;
        event.stopPropagation();
        var itemElement = component.$element().closest(this._filesTreeViewItemSelector);
        var itemData = itemElement.data("item");
        var target = {
            itemData: itemData,
            itemElement: itemElement,
            isActionButton: true
        };
        this._contextMenu.showAt([itemData], element, event, target);
        this._activeFileActionsButton = component;
        this._activeFileActionsButton.setActive(true)
    };
    _proto._onContextMenuHidden = function() {
        if (this._activeFileActionsButton) {
            this._activeFileActionsButton.setActive(false)
        }
    };
    _proto.toggleNodeDisabledState = function(key, state) {
        var node = this._getNodeByKey(key);
        if (!node) {
            return
        }
        var items = this._filesTreeView.option("items");
        var itemIndex = items.map((function(item) {
            return item.getInternalKey()
        })).indexOf(node.getInternalKey());
        if (-1 !== itemIndex) {
            this._filesTreeView.option("items[".concat(itemIndex, "].disabled"), state)
        }
    };
    _proto._saveScrollTopPosition = function() {
        if (!(0, _window.hasWindow)()) {
            return
        }
        this._scrollTopPosition = this._filesTreeView.getScrollable().scrollTop()
    };
    _proto._restoreScrollTopPosition = function() {
        var _this3 = this;
        if (!(0, _window.hasWindow)() || !(0, _type.isNumeric)(this._scrollTopPosition)) {
            return
        }
        setTimeout((function() {
            return _this3._filesTreeView.getScrollable().scrollTo(_this3._scrollTopPosition)
        }))
    };
    _proto._updateFocusedElement = function() {
        var directoryInfo = this._getCurrentDirectory();
        var $element = this._getItemElementByKey(null === directoryInfo || void 0 === directoryInfo ? void 0 : directoryInfo.getInternalKey());
        if (this._$focusedElement) {
            this._$focusedElement.toggleClass(FILE_MANAGER_DIRS_TREE_FOCUSED_ITEM_CLASS, false)
        }
        this._$focusedElement = $element || (0, _renderer.default)();
        this._$focusedElement.toggleClass(FILE_MANAGER_DIRS_TREE_FOCUSED_ITEM_CLASS, true)
    };
    _proto._getNodeByKey = function(key) {
        var _this$_filesTreeView;
        return null === (_this$_filesTreeView = this._filesTreeView) || void 0 === _this$_filesTreeView ? void 0 : _this$_filesTreeView._getNode(key)
    };
    _proto._getPublicNode = function(key) {
        var _this$_filesTreeView2;
        var nodesQueue = _toConsumableArray(null === (_this$_filesTreeView2 = this._filesTreeView) || void 0 === _this$_filesTreeView2 ? void 0 : _this$_filesTreeView2.getNodes());
        while (nodesQueue.length) {
            var node = nodesQueue.shift();
            if (node.itemData.getInternalKey() === key) {
                return node
            } else if (node.children.length) {
                nodesQueue.push.apply(nodesQueue, _toConsumableArray(node.children))
            }
        }
        return
    };
    _proto._getItemElementByKey = function(key) {
        var node = this._getNodeByKey(key);
        if (node) {
            var $node = this._filesTreeView._getNodeElement(node);
            if ($node) {
                return $node.children(this._filesTreeViewItemSelector)
            }
        }
        return null
    };
    _proto._getDefaultOptions = function() {
        return (0, _extend.extend)(_Widget.prototype._getDefaultOptions.call(this), {
            storeExpandedState: false,
            initialFolder: null,
            contextMenu: null,
            getItems: null,
            getCurrentDirectory: null,
            onDirectoryClick: null
        })
    };
    _proto._optionChanged = function(args) {
        var name = args.name;
        switch (name) {
            case "storeExpandedState":
                this._storeExpandedState = this.option(name);
                break;
            case "getItems":
            case "rootFolderDisplayName":
            case "initialFolder":
            case "contextMenu":
                this.repaint();
                break;
            case "getCurrentDirectory":
                this.getCurrentDirectory = this.option(name);
                break;
            case "onDirectoryClick":
            case "onFilesTreeViewContentReady":
                this._actions[name] = this._createActionByOption(name);
                break;
            default:
                _Widget.prototype._optionChanged.call(this, args)
        }
    };
    _proto.toggleDirectoryExpandedState = function(directoryInfo, state) {
        var deferred = new _deferred.Deferred;
        var treeViewNode = this._getPublicNode(null === directoryInfo || void 0 === directoryInfo ? void 0 : directoryInfo.getInternalKey());
        if (!treeViewNode) {
            return deferred.reject().promise()
        }
        if (treeViewNode.expanded === state || treeViewNode.itemsLoaded && !treeViewNode.itemData.fileItem.hasSubDirectories) {
            return deferred.resolve().promise()
        }
        var action = state ? "expandItem" : "collapseItem";
        return this._filesTreeView[action](directoryInfo.getInternalKey())
    };
    _proto.refresh = function() {
        this._$focusedElement = null;
        this._saveScrollTopPosition();
        this._filesTreeView.option("dataSource", [])
    };
    _proto.updateCurrentDirectory = function() {
        if (this._disposed) {
            return
        }
        this._updateFocusedElement();
        this._storeExpandedState && this._updateExpandedStateToCurrentDirectory()
    };
    _proto._updateExpandedStateToCurrentDirectory = function() {
        return this.toggleDirectoryExpandedStateRecursive(this._getCurrentDirectory().parentDirectory, true)
    };
    _proto.toggleDirectoryExpandedStateRecursive = function(directoryInfo, state) {
        var dirLine = [];
        for (var dirInfo = directoryInfo; dirInfo; dirInfo = dirInfo.parentDirectory) {
            dirLine.unshift(dirInfo)
        }
        return this.toggleDirectoryLineExpandedState(dirLine, state)
    };
    _proto.toggleDirectoryLineExpandedState = function(dirLine, state) {
        var _this4 = this;
        if (!dirLine.length) {
            return (new _deferred.Deferred).resolve().promise()
        }
        return this.toggleDirectoryExpandedState(dirLine.shift(), state).then((function() {
            return _this4.toggleDirectoryLineExpandedState(dirLine, state)
        }))
    };
    _createClass(FileManagerFilesTreeView, [{
        key: "_filesTreeViewItemSelector",
        get: function() {
            return ".".concat(TREE_VIEW_ITEM_CLASS)
        }
    }, {
        key: "_contextMenu",
        get: function() {
            return this.option("contextMenu")
        }
    }]);
    return FileManagerFilesTreeView
}(_ui.default);
var _default = FileManagerFilesTreeView;
exports.default = _default;
module.exports = exports.default;
module.exports.default = exports.default;
