/**
 * DevExtreme (esm/ui/shared/ui.editor_factory_mixin.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import $ from "../../core/renderer";
import eventsEngine from "../../events/core/events_engine";
import {
    isDefined,
    isObject,
    isFunction
} from "../../core/utils/type";
import variableWrapper from "../../core/utils/variable_wrapper";
import {
    compileGetter
} from "../../core/utils/data";
import browser from "../../core/utils/browser";
import {
    extend
} from "../../core/utils/extend";
import devices from "../../core/devices";
import {
    getPublicElement
} from "../../core/element";
import {
    normalizeDataSourceOptions
} from "../../data/data_source/utils";
import {
    normalizeKeyName
} from "../../events/utils/index";
var {
    isWrapped: isWrapped
} = variableWrapper;
import "../text_box";
import "../number_box";
import "../check_box";
import "../select_box";
import "../date_box";
var CHECKBOX_SIZE_CLASS = "checkbox-size";
var EDITOR_INLINE_BLOCK = "dx-editor-inline-block";
var EditorFactoryMixin = function() {
    var getResultConfig = function(config, options) {
        return extend(config, {
            readOnly: options.readOnly,
            placeholder: options.placeholder,
            inputAttr: {
                id: options.id
            },
            tabIndex: options.tabIndex
        }, options.editorOptions)
    };
    var checkEnterBug = function() {
        return browser.mozilla || devices.real().ios
    };
    var getTextEditorConfig = function(options) {
        var data = {};
        var isEnterBug = checkEnterBug();
        var sharedData = options.sharedData || data;
        return getResultConfig({
            placeholder: options.placeholder,
            width: options.width,
            value: options.value,
            onValueChanged: function(e) {
                var needDelayedUpdate = "filterRow" === options.parentType || "searchPanel" === options.parentType;
                var isInputOrKeyUpEvent = e.event && ("input" === e.event.type || "keyup" === e.event.type);
                var updateValue = function(e, notFireEvent) {
                    options && options.setValue(e.value, notFireEvent)
                };
                clearTimeout(data.valueChangeTimeout);
                if (isInputOrKeyUpEvent && needDelayedUpdate) {
                    sharedData.valueChangeTimeout = data.valueChangeTimeout = setTimeout((function() {
                        updateValue(e, data.valueChangeTimeout !== sharedData.valueChangeTimeout)
                    }), isDefined(options.updateValueTimeout) ? options.updateValueTimeout : 0)
                } else {
                    updateValue(e)
                }
            },
            onKeyDown: function(e) {
                if (isEnterBug && "enter" === normalizeKeyName(e.event)) {
                    eventsEngine.trigger($(e.component._input()), "change")
                }
            },
            valueChangeEvent: "change" + ("filterRow" === options.parentType ? " keyup input" : "")
        }, options)
    };
    var prepareBooleanEditor = function(options) {
        if ("filterRow" === options.parentType || "filterBuilder" === options.parentType) {
            prepareSelectBox(extend(options, {
                lookup: {
                    displayExpr: function(data) {
                        if (true === data) {
                            return options.trueText || "true"
                        } else if (false === data) {
                            return options.falseText || "false"
                        }
                    },
                    dataSource: [true, false]
                }
            }))
        } else {
            ! function(options) {
                options.editorName = "dxCheckBox";
                options.editorOptions = getResultConfig({
                    elementAttr: {
                        id: options.id
                    },
                    value: isDefined(options.value) ? options.value : void 0,
                    hoverStateEnabled: !options.readOnly,
                    focusStateEnabled: !options.readOnly,
                    activeStateEnabled: false,
                    onValueChanged: function(e) {
                        options.setValue && options.setValue(e.value, e)
                    }
                }, options)
            }(options)
        }
    };

    function prepareSelectBox(options) {
        var lookup = options.lookup;
        var displayGetter;
        var dataSource;
        var postProcess;
        var isFilterRow = "filterRow" === options.parentType;
        if (lookup) {
            displayGetter = compileGetter(lookup.displayExpr);
            dataSource = lookup.dataSource;
            if (isFunction(dataSource) && !isWrapped(dataSource)) {
                dataSource = dataSource(options.row || {});
                ! function(options) {
                    if (options.row && options.row.watch && "dataRow" === options.parentType) {
                        var editorOptions = options.editorOptions || {};
                        options.editorOptions = editorOptions;
                        var selectBox;
                        var onInitialized = editorOptions.onInitialized;
                        editorOptions.onInitialized = function(e) {
                            onInitialized && onInitialized.apply(this, arguments);
                            selectBox = e.component;
                            selectBox.on("disposing", stopWatch)
                        };
                        var dataSource;
                        var stopWatch = options.row.watch(() => {
                            dataSource = options.lookup.dataSource(options.row);
                            return dataSource && dataSource.filter
                        }, () => {
                            selectBox.option("dataSource", dataSource)
                        }, row => {
                            options.row = row
                        })
                    }
                }(options)
            }
            if (isObject(dataSource) || Array.isArray(dataSource)) {
                dataSource = normalizeDataSourceOptions(dataSource);
                if (isFilterRow) {
                    postProcess = dataSource.postProcess;
                    dataSource.postProcess = function(items) {
                        if (0 === this.pageIndex()) {
                            items = items.slice(0);
                            items.unshift(null)
                        }
                        if (postProcess) {
                            return postProcess.call(this, items)
                        }
                        return items
                    }
                }
            }
            var allowClearing = Boolean(lookup.allowClearing && !isFilterRow);
            options.editorName = "dxSelectBox";
            options.editorOptions = getResultConfig({
                searchEnabled: true,
                value: options.value,
                valueExpr: options.lookup.valueExpr,
                searchExpr: options.lookup.searchExpr || options.lookup.displayExpr,
                allowClearing: allowClearing,
                showClearButton: allowClearing,
                displayExpr: function(data) {
                    if (null === data) {
                        return options.showAllText
                    }
                    return displayGetter(data)
                },
                dataSource: dataSource,
                onValueChanged: function(e) {
                    var params = [e.value];
                    !isFilterRow && params.push(e.component.option("text"));
                    options.setValue.apply(this, params)
                }
            }, options)
        }
    }
    return {
        createEditor: function($container, options) {
            options.cancel = false;
            options.editorElement = getPublicElement($container);
            if (!isDefined(options.tabIndex)) {
                options.tabIndex = this.option("tabIndex")
            }
            if (options.lookup) {
                prepareSelectBox(options)
            } else {
                switch (options.dataType) {
                    case "date":
                    case "datetime":
                        ! function(options) {
                            options.editorName = "dxDateBox";
                            options.editorOptions = getResultConfig({
                                value: options.value,
                                onValueChanged: function(args) {
                                    options.setValue(args.value)
                                },
                                onKeyDown: function(e) {
                                    if (checkEnterBug() && "enter" === normalizeKeyName(e.event)) {
                                        e.component.blur();
                                        e.component.focus()
                                    }
                                },
                                displayFormat: options.format,
                                type: options.dataType,
                                dateSerializationFormat: null,
                                width: "filterBuilder" === options.parentType ? void 0 : "auto"
                            }, options)
                        }(options);
                        break;
                    case "boolean":
                        prepareBooleanEditor(options);
                        break;
                    case "number":
                        ! function(options) {
                            var config = getTextEditorConfig(options);
                            config.value = isDefined(options.value) ? options.value : null;
                            options.editorName = "dxNumberBox";
                            options.editorOptions = config
                        }(options);
                        break;
                    default:
                        ! function(options) {
                            var config = getTextEditorConfig(options);
                            var isSearching = "searchPanel" === options.parentType;
                            if (options.editorType && "dxTextBox" !== options.editorType) {
                                config.value = options.value
                            } else {
                                config.value = (value = options.value, isDefined(value) ? value.toString() : "")
                            }
                            var value;
                            config.valueChangeEvent += isSearching ? " keyup input search" : "";
                            config.mode = config.mode || (isSearching ? "search" : "text");
                            options.editorName = "dxTextBox";
                            options.editorOptions = config
                        }(options)
                }
            }
            if ("dataRow" === options.parentType && options.editorType) {
                options.editorName = options.editorType
            }
            this.executeAction("onEditorPreparing", options);
            if (options.cancel) {
                return
            }
            if ("dataRow" === options.parentType && !options.isOnForm && !isDefined(options.editorOptions.showValidationMark)) {
                options.editorOptions.showValidationMark = false
            }! function(that, options) {
                var $editorElement = $(options.editorElement);
                if (options.editorName && options.editorOptions && $editorElement[options.editorName]) {
                    if ("dxCheckBox" === options.editorName || "dxSwitch" === options.editorName) {
                        if (!options.isOnForm) {
                            $editorElement.addClass(that.addWidgetPrefix(CHECKBOX_SIZE_CLASS));
                            $editorElement.parent().addClass(EDITOR_INLINE_BLOCK)
                        }
                    }
                    that._createComponent($editorElement, options.editorName, options.editorOptions);
                    if ("dxDateBox" === options.editorName) {
                        var dateBox = $editorElement.dxDateBox("instance");
                        var defaultEnterKeyHandler = dateBox._supportedKeys().enter;
                        dateBox.registerKeyHandler("enter", e => {
                            if (dateBox.option("opened")) {
                                defaultEnterKeyHandler(e)
                            }
                            return true
                        })
                    }
                    if ("dxTextArea" === options.editorName) {
                        $editorElement.dxTextArea("instance").registerKeyHandler("enter", (function(event) {
                            if ("enter" === normalizeKeyName(event) && !event.ctrlKey && !event.shiftKey) {
                                event.stopPropagation()
                            }
                        }))
                    }
                }
            }(this, options);
            this.executeAction("onEditorPrepared", options)
        }
    }
}();
export default EditorFactoryMixin;
