/**
 * DevExtreme (esm/ui/html_editor/modules/toolbar.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import Quill from "devextreme-quill";
import $ from "../../../core/renderer";
import BaseModule from "./base";
import Toolbar from "../../toolbar";
import "../../select_box";
import "../../color_box/color_view";
import "../../number_box";
import errors from "../../widget/ui.errors";
import WidgetCollector from "./widget_collector";
import {
    each
} from "../../../core/utils/iterator";
import {
    isString,
    isObject,
    isDefined,
    isEmptyObject
} from "../../../core/utils/type";
import {
    extend
} from "../../../core/utils/extend";
import localizationMessage from "../../../localization/message";
import {
    titleize,
    camelize
} from "../../../core/utils/inflector";
import eventsEngine from "../../../events/core/events_engine";
import {
    addNamespace
} from "../../../events/utils/index";
import {
    getTableFormats,
    TABLE_OPERATIONS
} from "../utils/table_helper";
import {
    getFormatHandlers,
    getDefaultClickHandler,
    ICON_MAP,
    applyFormat
} from "../utils/toolbar_helper";
var ToolbarModule = BaseModule;
if (Quill) {
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
    var localize = name => localizationMessage.format("dxHtmlEditor-".concat(camelize(name)));
    var localizeValue = (value, name) => {
        if ("header" === name) {
            var isHeaderValue = isDefined(value) && false !== value;
            return isHeaderValue ? "".concat(localize("heading"), " ").concat(value) : localize("normalText")
        }
        return localize(value) || value
    };
    ToolbarModule = class extends BaseModule {
        constructor(quill, options) {
            super(quill, options);
            this._toolbarWidgets = new WidgetCollector;
            this._formatHandlers = getFormatHandlers(this);
            this._tableFormats = getTableFormats(quill);
            if (isDefined(options.items)) {
                this._addCallbacks();
                this._renderToolbar();
                this.quill.on("editor-change", (eventName, newValue, oldValue, eventSource) => {
                    var isSilentMode = eventSource === SILENT_ACTION && isEmptyObject(this.quill.getFormat());
                    if (!isSilentMode) {
                        var isSelectionChanged = eventName === SELECTION_CHANGE_EVENT;
                        this._updateToolbar(isSelectionChanged)
                    }
                })
            }
        }
        _addCallbacks() {
            this.addCleanCallback(this.clean.bind(this));
            this.editorInstance.addContentInitializedCallback(this.updateHistoryWidgets.bind(this))
        }
        _updateToolbar(isSelectionChanged) {
            this.updateFormatWidgets(isSelectionChanged);
            this.updateHistoryWidgets();
            this.updateTableWidgets()
        }
        _updateFormatWidget(name, isApplied, formats) {
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
            this._toggleClearFormatting(isApplied || !isEmptyObject(formats))
        }
        _renderToolbar() {
            var container = this.options.container || this._getContainer();
            this._$toolbar = $("<div>").addClass(TOOLBAR_CLASS).appendTo(container);
            this._$toolbarContainer = $(container).addClass(TOOLBAR_WRAPPER_CLASS);
            eventsEngine.on(this._$toolbarContainer, addNamespace("mousedown", this.editorInstance.NAME), e => {
                e.preventDefault()
            });
            this._subscribeFormatHotKeys();
            this.toolbarInstance = this.editorInstance._createComponent(this._$toolbar, Toolbar, this.toolbarConfig);
            this.editorInstance.on("optionChanged", _ref => {
                var {
                    name: name
                } = _ref;
                if ("readOnly" === name || "disabled" === name) {
                    this.toolbarInstance.option("disabled", this.isInteractionDisabled)
                }
            })
        }
        get toolbarConfig() {
            return {
                dataSource: this._prepareToolbarItems(),
                disabled: this.isInteractionDisabled,
                menuContainer: this._$toolbarContainer,
                multiline: this.isMultilineMode()
            }
        }
        get isInteractionDisabled() {
            return this.editorInstance.option("readOnly") || this.editorInstance.option("disabled")
        }
        isMultilineMode() {
            var _this$options$multili;
            return null !== (_this$options$multili = this.options.multiline) && void 0 !== _this$options$multili ? _this$options$multili : true
        }
        clean() {
            this._toolbarWidgets.clear();
            if (this._$toolbarContainer) {
                this._$toolbarContainer.empty().removeClass(TOOLBAR_WRAPPER_CLASS)
            }
        }
        repaint() {
            this.toolbarInstance && this.toolbarInstance.repaint()
        }
        _getContainer() {
            var $container = $("<div>");
            this.editorInstance.$element().prepend($container);
            return $container
        }
        _detectRenamedOptions(item) {
            if (isObject(item)) {
                each([{
                    newName: "name",
                    oldName: "formatName"
                }, {
                    newName: "acceptedValues",
                    oldName: "formatValues"
                }], (index, optionName) => {
                    if (Object.prototype.hasOwnProperty.call(item, optionName.oldName)) {
                        errors.log("W1016", optionName.oldName, optionName.newName)
                    }
                })
            }
        }
        _subscribeFormatHotKeys() {
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
        }
        _handleFormatHotKey(range, context, _ref2) {
            var {
                which: which
            } = _ref2;
            var formatName = FORMAT_HOTKEYS[which];
            this._updateButtonState(formatName)
        }
        _updateButtonState(formatName) {
            var formatWidget = this._toolbarWidgets.getByName(formatName);
            var currentFormat = this.quill.getFormat();
            var formatValue = currentFormat[formatName];
            if (formatValue) {
                this._markActiveFormatWidget(formatName, formatWidget, currentFormat)
            } else {
                this._resetFormatWidget(formatName, formatWidget)
            }
        }
        _prepareToolbarItems() {
            var resultItems = [];
            each(this.options.items, (index, item) => {
                var newItem;
                this._detectRenamedOptions(item);
                if (isObject(item)) {
                    newItem = this._handleObjectItem(item)
                } else if (isString(item)) {
                    var buttonItemConfig = this._prepareButtonItemConfig(item);
                    newItem = this._getToolbarItem(buttonItemConfig)
                }
                if (newItem) {
                    resultItems.push(newItem)
                }
            });
            return resultItems
        }
        _handleObjectItem(item) {
            if (item.name && item.acceptedValues && this._isAcceptableItem(item.widget, "dxSelectBox")) {
                var selectItemConfig = this._prepareSelectItemConfig(item);
                return this._getToolbarItem(selectItemConfig)
            } else if (item.name && this._isAcceptableItem(item.widget, "dxButton")) {
                var defaultButtonItemConfig = this._prepareButtonItemConfig(item.name);
                var buttonItemConfig = extend(true, defaultButtonItemConfig, item);
                return this._getToolbarItem(buttonItemConfig)
            } else {
                return this._getToolbarItem(item)
            }
        }
        _isAcceptableItem(widget, acceptableWidgetName) {
            return !widget || widget === acceptableWidgetName
        }
        _prepareButtonItemConfig(name) {
            var _ICON_MAP$name;
            var iconName = null !== (_ICON_MAP$name = ICON_MAP[name]) && void 0 !== _ICON_MAP$name ? _ICON_MAP$name : name;
            var buttonText = titleize(name);
            return {
                widget: "dxButton",
                name: name,
                options: {
                    hint: localize(buttonText),
                    text: localize(buttonText),
                    icon: iconName.toLowerCase(),
                    onClick: this._formatHandlers[name] || getDefaultClickHandler(this, name),
                    stylingMode: "text"
                },
                showText: "inMenu"
            }
        }
        _prepareSelectItemConfig(item) {
            var {
                name: name,
                acceptedValues: acceptedValues
            } = item;
            return extend(true, {
                widget: "dxSelectBox",
                name: name,
                options: {
                    stylingMode: "filled",
                    dataSource: acceptedValues,
                    displayExpr: value => localizeValue(value, name),
                    placeholder: localize(name),
                    onValueChanged: e => {
                        if (!this._isReset) {
                            this._hideAdaptiveMenu();
                            applyFormat(this, [name, e.value, USER_ACTION], e.event);
                            this._setValueSilent(e.component, e.value)
                        }
                    }
                }
            }, item)
        }
        _hideAdaptiveMenu() {
            if (this.toolbarInstance.option("overflowMenuVisible")) {
                this.toolbarInstance.option("overflowMenuVisible", false)
            }
        }
        _getToolbarItem(item) {
            var baseItem = {
                options: {
                    onInitialized: e => {
                        if (item.name) {
                            e.component.$element().addClass(TOOLBAR_FORMAT_WIDGET_CLASS);
                            e.component.$element().toggleClass("dx-".concat(item.name.toLowerCase(), "-format"), !!item.name);
                            this._toolbarWidgets.add(item.name, e.component)
                        }
                    }
                }
            };
            var multilineItem = this.isMultilineMode() ? {
                location: "before",
                locateInMenu: "never"
            } : {};
            return extend(true, {
                location: "before",
                locateInMenu: "auto"
            }, this._getDefaultConfig(item.name), item, baseItem, multilineItem)
        }
        _getDefaultItemsConfig() {
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
                    template: (data, index, element) => {
                        $(element).addClass(TOOLBAR_SEPARATOR_CLASS)
                    },
                    menuItemTemplate: (data, index, element) => {
                        $(element).addClass(TOOLBAR_MENU_SEPARATOR_CLASS)
                    }
                }
            }
        }
        _getDefaultConfig(name) {
            return this._getDefaultItemsConfig()[name]
        }
        updateHistoryWidgets() {
            var historyModule = this.quill.history;
            if (!historyModule) {
                return
            }
            var {
                undo: undoOps,
                redo: redoOps
            } = historyModule.stack;
            this._updateManipulationWidget(this._toolbarWidgets.getByName("undo"), Boolean(undoOps.length));
            this._updateManipulationWidget(this._toolbarWidgets.getByName("redo"), Boolean(redoOps.length))
        }
        updateTableWidgets() {
            var table = this.quill.getModule("table");
            if (!table) {
                return
            }
            var selection = this.quill.getSelection();
            var formats = selection && this.quill.getFormat(selection) || {};
            var isTableOperationsEnabled = this._tableFormats.some(format => Boolean(formats[format]));
            TABLE_OPERATIONS.forEach(operationName => {
                var isInsertTable = "insertTable" === operationName;
                var widget = this._toolbarWidgets.getByName(operationName);
                this._updateManipulationWidget(widget, isInsertTable ? !isTableOperationsEnabled : isTableOperationsEnabled)
            })
        }
        _updateManipulationWidget(widget, isOperationEnabled) {
            if (!widget) {
                return
            }
            widget.option("disabled", !isOperationEnabled)
        }
        updateFormatWidgets(isResetRequired) {
            var selection = this.quill.getSelection();
            if (!selection) {
                return
            }
            var formats = this.quill.getFormat(selection);
            var hasFormats = !isEmptyObject(formats);
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
        }
        _markActiveFormatWidget(name, widget, formats) {
            if (this._isColorFormat(name)) {
                this._updateColorWidget(name, formats[name])
            }
            if ("value" in widget.option()) {
                this._setValueSilent(widget, formats[name])
            } else {
                widget.$element().addClass(ACTIVE_FORMAT_CLASS)
            }
        }
        _toggleClearFormatting(hasFormats) {
            var clearWidget = this._toolbarWidgets.getByName("clear");
            if (clearWidget) {
                clearWidget.option("disabled", !hasFormats)
            }
        }
        _isColorFormat(name) {
            return "color" === name || "background" === name
        }
        _updateColorWidget(name, color) {
            var formatWidget = this._toolbarWidgets.getByName(name);
            if (!formatWidget) {
                return
            }
            formatWidget.$element().find(".".concat(ICON_CLASS)).css("borderBottomColor", color || "transparent")
        }
        _getFormatWidgetName(name, formats) {
            var widgetName;
            switch (name) {
                case "align":
                    widgetName = name + titleize(formats[name]);
                    break;
                case "list":
                    widgetName = formats[name] + titleize(name);
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
        }
        _setValueSilent(widget, value) {
            this._isReset = true;
            widget.option("value", value);
            this._isReset = false
        }
        _resetFormatWidgets() {
            this._toolbarWidgets.each((name, widget) => {
                this._resetFormatWidget(name, widget)
            })
        }
        _resetFormatWidget(name, widget) {
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
        }
        addClickHandler(name, handler) {
            this._formatHandlers[name] = handler;
            var formatWidget = this._toolbarWidgets.getByName(name);
            if (formatWidget && "dxButton" === formatWidget.NAME) {
                formatWidget.option("onClick", handler)
            }
        }
    }
}
export default ToolbarModule;
