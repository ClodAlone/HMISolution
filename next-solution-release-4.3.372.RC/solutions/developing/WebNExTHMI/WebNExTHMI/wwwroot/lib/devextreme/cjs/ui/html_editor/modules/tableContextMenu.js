/**
 * DevExtreme (cjs/ui/html_editor/modules/tableContextMenu.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.default = void 0;
var _devextremeQuill = _interopRequireDefault(require("devextreme-quill"));
var _renderer = _interopRequireDefault(require("../../../core/renderer"));
var _base = _interopRequireDefault(require("./base"));
var _events_engine = _interopRequireDefault(require("../../../events/core/events_engine"));
var _index = require("../../../events/utils/index");
var _context_menu = _interopRequireDefault(require("../../context_menu"));
var _message = _interopRequireDefault(require("../../../localization/message"));
var _table_helper = require("../utils/table_helper");
var _toolbar_helper = require("../utils/toolbar_helper");
var _iterator = require("../../../core/utils/iterator");
var _type = require("../../../core/utils/type");
var _inflector = require("../../../core/utils/inflector");
var _extend = require("../../../core/utils/extend");

function _interopRequireDefault(obj) {
    return obj && obj.__esModule ? obj : {
        default: obj
    }
}

function _assertThisInitialized(self) {
    if (void 0 === self) {
        throw new ReferenceError("this hasn't been initialised - super() hasn't been called")
    }
    return self
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
var MODULE_NAMESPACE = "dxHtmlEditorTableContextMenu";
var CONTEXT_MENU_EVENT = (0, _index.addNamespace)("dxcontextmenu", MODULE_NAMESPACE);
var TableContextMenuModule = _base.default;
var localize = function(name) {
    return _message.default.format("dxHtmlEditor-".concat((0, _inflector.camelize)(name)))
};
if (_devextremeQuill.default) {
    TableContextMenuModule = function(_BaseModule) {
        _inheritsLoose(TableContextMenuModule, _BaseModule);

        function TableContextMenuModule(quill, options) {
            var _this;
            _this = _BaseModule.call(this, quill, options) || this;
            _this.enabled = !!options.enabled;
            _this._quillContainer = _this.editorInstance._getQuillContainer();
            _this.addCleanCallback(_this.prepareCleanCallback());
            _this._formatHandlers = (0, _toolbar_helper.getFormatHandlers)(_assertThisInitialized(_this));
            _this._tableFormats = (0, _table_helper.getTableFormats)(quill);
            if (_this.enabled) {
                _this._enableContextMenu(options.items)
            }
            return _this
        }
        var _proto = TableContextMenuModule.prototype;
        _proto._enableContextMenu = function(items) {
            var _this$_contextMenu;
            null === (_this$_contextMenu = this._contextMenu) || void 0 === _this$_contextMenu ? void 0 : _this$_contextMenu.dispose();
            this._contextMenu = this._createContextMenu(items);
            this._attachEvents()
        };
        _proto._attachEvents = function() {
            _events_engine.default.on(this.editorInstance._getContent(), CONTEXT_MENU_EVENT, this._prepareContextMenuHandler())
        };
        _proto._detachEvents = function() {
            _events_engine.default.off(this.editorInstance._getContent(), CONTEXT_MENU_EVENT)
        };
        _proto._createContextMenu = function(items) {
            var $container = (0, _renderer.default)("<div>").appendTo(this.editorInstance.$element());
            var menuConfig = this._getMenuConfig(items);
            return this.editorInstance._createComponent($container, _context_menu.default, menuConfig)
        };
        _proto.showPropertiesForm = function() {
            var type = arguments.length > 0 && void 0 !== arguments[0] ? arguments[0] : "cell";
            var $element = (0, _renderer.default)(this._targetElement).closest("cell" === type ? "th, td" : "table");
            this._contextMenu.hide();
            this._formatHandlers["".concat(type, "Properties")]($element);
            this._targetElement = null
        };
        _proto._isAcceptableItem = function(widget, acceptableWidgetName) {
            return !widget || widget === acceptableWidgetName
        };
        _proto._handleObjectItem = function(item) {
            if (item.name && this._isAcceptableItem(item.widget, "dxButton")) {
                var defaultButtonItemConfig = this._prepareMenuItemConfig(item.name);
                var buttonItemConfig = (0, _extend.extend)(true, defaultButtonItemConfig, item);
                return buttonItemConfig
            } else if (item.items) {
                item.items = this._prepareMenuItems(item.items);
                return item
            } else {
                return item
            }
        };
        _proto._prepareMenuItemConfig = function(name) {
            var _ICON_MAP$name, _this$_formatHandlers;
            var iconName = null !== (_ICON_MAP$name = _toolbar_helper.ICON_MAP[name]) && void 0 !== _ICON_MAP$name ? _ICON_MAP$name : name;
            var buttonText = (0, _inflector.titleize)(name);
            return {
                text: localize(buttonText),
                icon: iconName.toLowerCase(),
                onClick: null !== (_this$_formatHandlers = this._formatHandlers[name]) && void 0 !== _this$_formatHandlers ? _this$_formatHandlers : (0, _toolbar_helper.getDefaultClickHandler)(this, name)
            }
        };
        _proto._prepareMenuItems = function(items) {
            var _this2 = this;
            var resultItems = [];
            (0, _iterator.each)(items, (function(_, item) {
                var newItem;
                if ((0, _type.isObject)(item)) {
                    newItem = _this2._handleObjectItem(item)
                } else if ((0, _type.isString)(item)) {
                    newItem = _this2._prepareMenuItemConfig(item)
                }
                if (newItem) {
                    resultItems.push(newItem)
                }
            }));
            return resultItems
        };
        _proto._getMenuConfig = function(items) {
            var _this3 = this;
            var defaultItems = [{
                text: localize("insert"),
                items: ["insertHeaderRow", "insertRowAbove", "insertRowBelow", (0, _extend.extend)(this._prepareMenuItemConfig("insertColumnLeft"), {
                    beginGroup: true
                }), "insertColumnRight"]
            }, {
                text: localize("delete"),
                items: ["deleteColumn", "deleteRow", "deleteTable"]
            }, (0, _extend.extend)(this._prepareMenuItemConfig("cellProperties"), {
                onClick: function(e) {
                    _this3.showPropertiesForm("cell")
                }
            }), (0, _extend.extend)(this._prepareMenuItemConfig("tableProperties"), {
                onClick: function(e) {
                    _this3.showPropertiesForm("table")
                }
            })];
            var customItems = this._prepareMenuItems(null !== items && void 0 !== items && items.length ? items : defaultItems);
            return {
                target: this._quillContainer,
                showEvent: null,
                hideOnParentScroll: false,
                items: customItems
            }
        };
        _proto._prepareContextMenuHandler = function() {
            var _this4 = this;
            return function(event) {
                if (_this4._isTableTarget(event.target)) {
                    _this4._targetElement = event.target;
                    _this4._setContextMenuPosition(event);
                    _this4._contextMenu.show();
                    event.preventDefault()
                }
            }
        };
        _proto._setContextMenuPosition = function(event) {
            var startPosition = this._quillContainer.get(0).getBoundingClientRect();
            this._contextMenu.option({
                position: {
                    my: "left top",
                    at: "left top",
                    collision: "fit fit",
                    offset: {
                        x: event.clientX - startPosition.left,
                        y: event.clientY - startPosition.top
                    }
                }
            })
        };
        _proto._isTableTarget = function(targetElement) {
            return !!(0, _renderer.default)(targetElement).closest(".dx-htmleditor-content td, .dx-htmleditor-content th").length
        };
        _proto.clean = function() {
            this._detachEvents()
        };
        _proto.option = function(_option, value) {
            if ("tableContextMenu" === _option) {
                this.handleOptionChangeValue(value);
                return
            }
            if ("enabled" === _option) {
                this.enabled = value;
                value ? this._enableContextMenu() : this.clean()
            } else if ("items" === _option) {
                var _this$_contextMenu2;
                null === (_this$_contextMenu2 = this._contextMenu) || void 0 === _this$_contextMenu2 ? void 0 : _this$_contextMenu2.dispose();
                this._contextMenu = this._createContextMenu(value)
            }
        };
        _proto.prepareCleanCallback = function() {
            var _this5 = this;
            return function() {
                _this5.clean()
            }
        };
        return TableContextMenuModule
    }(_base.default)
}
var _default = TableContextMenuModule;
exports.default = _default;
module.exports = exports.default;
module.exports.default = exports.default;
