/**
 * DevExtreme (cjs/ui/html_editor/modules/toolbar.js)
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
var _toolbar = _interopRequireDefault(require("../../toolbar"));
require("../../select_box");
require("../../color_box/color_view");
require("../../number_box");
var _ui = _interopRequireDefault(require("../../widget/ui.errors"));
var _widget_collector = _interopRequireDefault(require("./widget_collector"));
var _iterator = require("../../../core/utils/iterator");
var _type = require("../../../core/utils/type");
var _extend = require("../../../core/utils/extend");
var _message = _interopRequireDefault(require("../../../localization/message"));
var _inflector = require("../../../core/utils/inflector");
var _events_engine = _interopRequireDefault(require("../../../events/core/events_engine"));
var _index = require("../../../events/utils/index");
var _table_helper = require("../utils/table_helper");
var _toolbar_helper = require("../utils/toolbar_helper");

function _interopRequireDefault(obj) {
    return obj && obj.__esModule ? obj : {
        default: obj
    }
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
var ToolbarModule = _base.default;
if (_devextremeQuill.default) {
    var TOOLBAR_WRAPPER_CLASS = "dx-htmleditor-toolbar-wrapper";
    var TOOLBAR_CLASS = "dx-htmleditor-toolbar";
    var TOOLBAR_FORMAT_WIDGET_CLASS = "dx-htmleditor-toolbar-format";
    var TOOLBAR_SEPARATOR_CLASS = "dx-htmleditor-toolbar-separator";
    var TOOLBAR_MENU_SEPARATOR_CLASS = "dx-htmleditor-toolbar-menu-separator";
    var ACTIVE_FORMAT_CLASS = "dx-format-active";
    var ICON_CLASS = "dx-icon";
    var SELECTION_CHANGE_EVENT = "selection-change";
    var USER_ACTION = "user";
    var SILENT_ACTION = "silent";
    var FORMAT_HOTKEYS = {
        66: "bold",
        73: "italic",
        85: "underline"
    };
    var KEY_CODES = {
        b: 66,
        i: 73,
        u: 85
    };
    var localize = function(name) {
        return _message.default.format("dxHtmlEditor-".concat((0, _inflector.camelize)(name)))
    };
    var localizeValue = function(value, name) {
        if ("header" === name) {
            var isHeaderValue = (0, _type.isDefined)(value) && false !== value;
            return isHeaderValue ? "".concat(localize("heading"), " ").concat(value) : localize("normalText")
        }
        return localize(value) || value
    };
    ToolbarModule = function(_BaseModule) {
        _inheritsLoose(ToolbarModule, _BaseModule);

        function ToolbarModule(quill, options) {
            var _this;
            _this = _BaseModule.call(this, quill, options) || this;
            _this._toolbarWidgets = new _widget_collector.default;
            _this._formatHandlers = (0, _toolbar_helper.getFormatHandlers)(_assertThisInitialized(_this));
            _this._tableFormats = (0, _table_helper.getTableFormats)(quill);
            if ((0, _type.isDefined)(options.items)) {
                _this._addCallbacks();
                _this._renderToolbar();
                _this.quill.on("editor-change", (function(eventName, newValue, oldValue, eventSource) {
                    var isSilentMode = eventSource === SILENT_ACTION && (0, _type.isEmptyObject)(_this.quill.getFormat());
                    if (!isSilentMode) {
                        var isSelectionChanged = eventName === SELECTION_CHANGE_EVENT;
                        _this._updateToolbar(isSelectionChanged)
                    }
                }))
            }
            return _this
        }
        var _proto = ToolbarModule.prototype;
        _proto._addCallbacks = function() {
            this.addCleanCallback(this.clean.bind(this));
            this.editorInstance.addContentInitializedCallback(this.updateHistoryWidgets.bind(this))
        };
        _proto._updateToolbar = function(isSelectionChanged) {
            this.updateFormatWidgets(isSelectionChanged);
            this.updateHistoryWidgets();
            this.updateTableWidgets()
        };
        _proto._updateFormatWidget = function(name, isApplied, formats) {
            var widget = this._toolbarWidgets.getByName(name);
            if (!widget) {
                return
            }
            if (isApplied) {
                this._markActiveFormatWidget(name, widget, formats)
            } else {
                this._resetFormatWidget(name, widget);
                if (Object.prototype.hasOwnProperty.call(name)) {
                    delete formats[name]
                }
            }
            this._toggleClearFormatting(isApplied || !(0, _type.isEmptyObject)(formats))
        };
        _proto._renderToolbar = function() {
            var _this2 = this;
            var container = this.options.container || this._getContainer();
            this._$toolbar = (0, _renderer.default)("<div>").addClass(TOOLBAR_CLASS).appendTo(container);
            this._$toolbarContainer = (0, _renderer.default)(container).addClass(TOOLBAR_WRAPPER_CLASS);
            _events_engine.default.on(this._$toolbarContainer, (0, _index.addNamespace)("mousedown", this.editorInstance.NAME), (function(e) {
                e.preventDefault()
            }));
            this._subscribeFormatHotKeys();
            this.toolbarInstance = this.editorInstance._createComponent(this._$toolbar, _toolbar.default, this.toolbarConfig);
            this.editorInstance.on("optionChanged", (function(_ref) {
                var name = _ref.name;
                if ("readOnly" === name || "disabled" === name) {
                    _this2.toolbarInstance.option("disabled", _this2.isInteractionDisabled)
                }
            }))
        };
        _proto.isMultilineMode = function() {
            var _this$options$multili;
            return null !== (_this$options$multili = this.options.multiline) && void 0 !== _this$options$multili ? _this$options$multili : true
        };
        _proto.clean = function() {
            this._toolbarWidgets.clear();
            if (this._$toolbarContainer) {
                this._$toolbarContainer.empty().removeClass(TOOLBAR_WRAPPER_CLASS)
            }
        };
        _proto.repaint = function() {
            this.toolbarInstance && this.toolbarInstance.repaint()
        };
        _proto._getContainer = function() {
            var $container = (0, _renderer.default)("<div>");
            this.editorInstance.$element().prepend($container);
            return $container
        };
        _proto._detectRenamedOptions = function(item) {
            if ((0, _type.isObject)(item)) {
                (0, _iterator.each)([{
                    newName: "name",
                    oldName: "formatName"
                }, {
                    newName: "acceptedValues",
                    oldName: "formatValues"
                }], (function(index, optionName) {
                    if (Object.prototype.hasOwnProperty.call(item, optionName.oldName)) {
                        _ui.default.log("W1016", optionName.oldName, optionName.newName)
                    }
                }))
            }
        };
        _proto._subscribeFormatHotKeys = function() {
            this.quill.keyboard.addBinding({
                which: KEY_CODES.b,
                shortKey: true
            }, this._handleFormatHotKey.bind(this));
            this.quill.keyboard.addBinding({
                which: KEY_CODES.i,
                shortKey: true
            }, this._handleFormatHotKey.bind(this));
            this.quill.keyboard.addBinding({
                which: KEY_CODES.u,
                shortKey: true
            }, this._handleFormatHotKey.bind(this))
        };
        _proto._handleFormatHotKey = function(range, context, _ref2) {
            var which = _ref2.which;
            var formatName = FORMAT_HOTKEYS[which];
            this._updateButtonState(formatName)
        };
        _proto._updateButtonState = function(formatName) {
            var formatWidget = this._toolbarWidgets.getByName(formatName);
            var currentFormat = this.quill.getFormat();
            var formatValue = currentFormat[formatName];
            if (formatValue) {
                this._markActiveFormatWidget(formatName, formatWidget, currentFormat)
            } else {
                this._resetFormatWidget(formatName, formatWidget)
            }
        };
        _proto._prepareToolbarItems = function() {
            var _this3 = this;
            var resultItems = [];
            (0, _iterator.each)(this.options.items, (function(index, item) {
                var newItem;
                _this3._detectRenamedOptions(item);
                if ((0, _type.isObject)(item)) {
                    newItem = _this3._handleObjectItem(item)
                } else if ((0, _type.isString)(item)) {
                    var buttonItemConfig = _this3._prepareButtonItemConfig(item);
                    newItem = _this3._getToolbarItem(buttonItemConfig)
                }
                if (newItem) {
                    resultItems.push(newItem)
                }
            }));
            return resultItems
        };
        _proto._handleObjectItem = function(item) {
            if (item.name && item.acceptedValues && this._isAcceptableItem(item.widget, "dxSelectBox")) {
                var selectItemConfig = this._prepareSelectItemConfig(item);
                return this._getToolbarItem(selectItemConfig)
            } else if (item.name && this._isAcceptableItem(item.widget, "dxButton")) {
                var defaultButtonItemConfig = this._prepareButtonItemConfig(item.name);
                var buttonItemConfig = (0, _extend.extend)(true, defaultButtonItemConfig, item);
                return this._getToolbarItem(buttonItemConfig)
            } else {
                return this._getToolbarItem(item)
            }
        };
        _proto._isAcceptableItem = function(widget, acceptableWidgetName) {
            return !widget || widget === acceptableWidgetName
        };
        _proto._prepareButtonItemConfig = function(name) {
            var _ICON_MAP$name;
            var iconName = null !== (_ICON_MAP$name = _toolbar_helper.ICON_MAP[name]) && void 0 !== _ICON_MAP$name ? _ICON_MAP$name : name;
            var buttonText = (0, _inflector.titleize)(name);
            return {
                widget: "dxButton",
                name: name,
                options: {
                    hint: localize(buttonText),
                    text: localize(buttonText),
                    icon: iconName.toLowerCase(),
                    onClick: this._formatHandlers[name] || (0, _toolbar_helper.getDefaultClickHandler)(this, name),
                    stylingMode: "text"
                },
                showText: "inMenu"
            }
        };
        _proto._prepareSelectItemConfig = function(item) {
            var _this4 = this;
            var name = item.name,
                acceptedValues = item.acceptedValues;
            return (0, _extend.extend)(true, {
                widget: "dxSelectBox",
                name: name,
                options: {
                    stylingMode: "filled",
                    dataSource: acceptedValues,
                    displayExpr: function(value) {
                        return localizeValue(value, name)
                    },
                    placeholder: localize(name),
                    onValueChanged: function(e) {
                        if (!_this4._isReset) {
                            _this4._hideAdaptiveMenu();
                            (0, _toolbar_helper.applyFormat)(_this4, [name, e.value, USER_ACTION], e.event);
                            _this4._setValueSilent(e.component, e.value)
                        }
                    }
                }
            }, item)
        };
        _proto._hideAdaptiveMenu = function() {
            if (this.toolbarInstance.option("overflowMenuVisible")) {
                this.toolbarInstance.option("overflowMenuVisible", false)
            }
        };
        _proto._getToolbarItem = function(item) {
            var _this5 = this;
            var baseItem = {
                options: {
                    onInitialized: function(e) {
                        if (item.name) {
                            e.component.$element().addClass(TOOLBAR_FORMAT_WIDGET_CLASS);
                            e.component.$element().toggleClass("dx-".concat(item.name.toLowerCase(), "-format"), !!item.name);
                            _this5._toolbarWidgets.add(item.name, e.component)
                        }
                    }
                }
            };
            var multilineItem = this.isMultilineMode() ? {
                location: "before",
                locateInMenu: "never"
            } : {};
            return (0, _extend.extend)(true, {
                location: "before",
                locateInMenu: "auto"
            }, this._getDefaultConfig(item.name), item, baseItem, multilineItem)
        };
        _proto._getDefaultItemsConfig = function() {
            return {
                clear: {
                    options: {
                        disabled: true
                    }
                },
                undo: {
                    options: {
                        disabled: true
                    }
                },
                redo: {
                    options: {
                        disabled: true
                    }
                },
                insertRowAbove: {
                    options: {
                        disabled: true
                    }
                },
                insertRowBelow: {
                    options: {
                        disabled: true
                    }
                },
                insertHeaderRow: {
                    options: {
                        disabled: true
                    }
                },
                insertColumnLeft: {
                    options: {
                        disabled: true
                    }
                },
                insertColumnRight: {
                    options: {
                        disabled: true
                    }
                },
                deleteRow: {
                    options: {
                        disabled: true
                    }
                },
                deleteColumn: {
                    options: {
                        disabled: true
                    }
                },
                deleteTable: {
                    options: {
                        disabled: true
                    }
                },
                cellProperties: {
                    options: {
                        disabled: true
                    }
                },
                tableProperties: {
                    options: {
                        disabled: true
                    }
                },
                separator: {
                    template: function(data, index, element) {
                        (0, _renderer.default)(element).addClass(TOOLBAR_SEPARATOR_CLASS)
                    },
                    menuItemTemplate: function(data, index, element) {
                        (0, _renderer.default)(element).addClass(TOOLBAR_MENU_SEPARATOR_CLASS)
                    }
                }
            }
        };
        _proto._getDefaultConfig = function(name) {
            return this._getDefaultItemsConfig()[name]
        };
        _proto.updateHistoryWidgets = function() {
            var historyModule = this.quill.history;
            if (!historyModule) {
                return
            }
            var _historyModule$stack = historyModule.stack,
                undoOps = _historyModule$stack.undo,
                redoOps = _historyModule$stack.redo;
            this._updateManipulationWidget(this._toolbarWidgets.getByName("undo"), Boolean(undoOps.length));
            this._updateManipulationWidget(this._toolbarWidgets.getByName("redo"), Boolean(redoOps.length))
        };
        _proto.updateTableWidgets = function() {
            var _this6 = this;
            var table = this.quill.getModule("table");
            if (!table) {
                return
            }
            var selection = this.quill.getSelection();
            var formats = selection && this.quill.getFormat(selection) || {};
            var isTableOperationsEnabled = this._tableFormats.some((function(format) {
                return Boolean(formats[format])
            }));
            _table_helper.TABLE_OPERATIONS.forEach((function(operationName) {
                var isInsertTable = "insertTable" === operationName;
                var widget = _this6._toolbarWidgets.getByName(operationName);
                _this6._updateManipulationWidget(widget, isInsertTable ? !isTableOperationsEnabled : isTableOperationsEnabled)
            }))
        };
        _proto._updateManipulationWidget = function(widget, isOperationEnabled) {
            if (!widget) {
                return
            }
            widget.option("disabled", !isOperationEnabled)
        };
        _proto.updateFormatWidgets = function(isResetRequired) {
            var selection = this.quill.getSelection();
            if (!selection) {
                return
            }
            var formats = this.quill.getFormat(selection);
            var hasFormats = !(0, _type.isEmptyObject)(formats);
            if (!hasFormats || isResetRequired) {
                this._resetFormatWidgets()
            }
            for (var formatName in formats) {
                var widgetName = this._getFormatWidgetName(formatName, formats);
                var formatWidget = this._toolbarWidgets.getByName(widgetName) || this._toolbarWidgets.getByName(formatName);
                if (!formatWidget) {
                    continue
                }
                this._markActiveFormatWidget(formatName, formatWidget, formats)
            }
            this._toggleClearFormatting(hasFormats || selection.length > 1)
        };
        _proto._markActiveFormatWidget = function(name, widget, formats) {
            if (this._isColorFormat(name)) {
                this._updateColorWidget(name, formats[name])
            }
            if ("value" in widget.option()) {
                this._setValueSilent(widget, formats[name])
            } else {
                widget.$element().addClass(ACTIVE_FORMAT_CLASS)
            }
        };
        _proto._toggleClearFormatting = function(hasFormats) {
            var clearWidget = this._toolbarWidgets.getByName("clear");
            if (clearWidget) {
                clearWidget.option("disabled", !hasFormats)
            }
        };
        _proto._isColorFormat = function(name) {
            return "color" === name || "background" === name
        };
        _proto._updateColorWidget = function(name, color) {
            var formatWidget = this._toolbarWidgets.getByName(name);
            if (!formatWidget) {
                return
            }
            formatWidget.$element().find(".".concat(ICON_CLASS)).css("borderBottomColor", color || "transparent")
        };
        _proto._getFormatWidgetName = function(name, formats) {
            var widgetName;
            switch (name) {
                case "align":
                    widgetName = name + (0, _inflector.titleize)(formats[name]);
                    break;
                case "list":
                    widgetName = formats[name] + (0, _inflector.titleize)(name);
                    break;
                case "code-block":
                    widgetName = "codeBlock";
                    break;
                case "script":
                    widgetName = formats[name] + name;
                    break;
                case "imageSrc":
                    widgetName = "image";
                    break;
                default:
                    widgetName = name
            }
            return widgetName
        };
        _proto._setValueSilent = function(widget, value) {
            this._isReset = true;
            widget.option("value", value);
            this._isReset = false
        };
        _proto._resetFormatWidgets = function() {
            var _this7 = this;
            this._toolbarWidgets.each((function(name, widget) {
                _this7._resetFormatWidget(name, widget)
            }))
        };
        _proto._resetFormatWidget = function(name, widget) {
            widget.$element().removeClass(ACTIVE_FORMAT_CLASS);
            if (this._isColorFormat(name)) {
                this._updateColorWidget(name)
            }
            if ("clear" === name) {
                widget.option("disabled", true)
            }
            if ("dxSelectBox" === widget.NAME) {
                this._setValueSilent(widget, null)
            }
        };
        _proto.addClickHandler = function(name, handler) {
            this._formatHandlers[name] = handler;
            var formatWidget = this._toolbarWidgets.getByName(name);
            if (formatWidget && "dxButton" === formatWidget.NAME) {
                formatWidget.option("onClick", handler)
            }
        };
        _createClass(ToolbarModule, [{
            key: "toolbarConfig",
            get: function() {
                return {
                    dataSource: this._prepareToolbarItems(),
                    disabled: this.isInteractionDisabled,
                    menuContainer: this._$toolbarContainer,
                    multiline: this.isMultilineMode()
                }
            }
        }, {
            key: "isInteractionDisabled",
            get: function() {
                return this.editorInstance.option("readOnly") || this.editorInstance.option("disabled")
            }
        }]);
        return ToolbarModule
    }(_base.default)
}
var _default = ToolbarModule;
exports.default = _default;
module.exports = exports.default;
module.exports.default = exports.default;
