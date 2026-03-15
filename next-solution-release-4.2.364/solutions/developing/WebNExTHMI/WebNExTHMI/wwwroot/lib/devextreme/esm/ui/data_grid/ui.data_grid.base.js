/**
 * DevExtreme (esm/ui/data_grid/ui.data_grid.base.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import $ from "../../core/renderer";
import registerComponent from "../../core/component_registrator";
import {
    deferRender,
    noop
} from "../../core/utils/common";
import {
    isFunction,
    isString
} from "../../core/utils/type";
import {
    each
} from "../../core/utils/iterator";
import {
    extend
} from "../../core/utils/extend";
import {
    logger
} from "../../core/utils/console";
import browser from "../../core/utils/browser";
import Widget from "../widget/ui.widget";
import gridCore from "./ui.data_grid.core";
import {
    isMaterial
} from "../themes";
var DATAGRID_ROW_SELECTOR = ".dx-row";
var DATAGRID_DEPRECATED_TEMPLATE_WARNING = "Specifying grid templates with the jQuery selector name is now deprecated. Use the DOM Node or the jQuery object that references this selector instead.";
import "./ui.data_grid.column_headers";
import "./ui.data_grid.columns_controller";
import "./ui.data_grid.data_controller";
import "./ui.data_grid.sorting";
import "./ui.data_grid.rows";
import "./ui.data_grid.context_menu";
import "./ui.data_grid.error_handling";
import "./ui.data_grid.grid_view";
import "./ui.data_grid.header_panel";
gridCore.registerModulesOrder(["stateStoring", "columns", "selection", "editorFactory", "columnChooser", "grouping", "editing", "editingRowBased", "editingFormBased", "editingCellBased", "masterDetail", "validating", "adaptivity", "data", "virtualScrolling", "columnHeaders", "filterRow", "headerPanel", "headerFilter", "sorting", "search", "rows", "pager", "columnsResizingReordering", "contextMenu", "keyboardNavigation", "errorHandling", "summary", "columnFixing", "export", "gridView"]);
var DataGrid = Widget.inherit({
    _activeStateUnit: DATAGRID_ROW_SELECTOR,
    _getDefaultOptions: function() {
        var result = this.callBase();
        each(gridCore.modules, (function() {
            if (isFunction(this.defaultOptions)) {
                extend(true, result, this.defaultOptions())
            }
        }));
        return result
    },
    _setDeprecatedOptions: function() {
        this.callBase();
        extend(this._deprecatedOptions, {
            useKeyboard: {
                since: "19.2",
                alias: "keyboardNavigation.enabled"
            },
            rowTemplate: {
                since: "21.2",
                message: 'Use the "dataRowTemplate" option instead'
            },
            onToolbarPreparing: {
                since: "21.2",
                message: 'Use the "toolbar" option instead'
            }
        })
    },
    _defaultOptionsRules: function() {
        return this.callBase().concat([{
            device: {
                platform: "ios"
            },
            options: {
                showRowLines: true
            }
        }, {
            device: function() {
                return isMaterial()
            },
            options: {
                showRowLines: true,
                showColumnLines: false,
                headerFilter: {
                    height: 315
                },
                editing: {
                    useIcons: true
                },
                selection: {
                    showCheckBoxesMode: "always"
                }
            }
        }, {
            device: function() {
                return browser.webkit
            },
            options: {
                loadingTimeout: 30,
                loadPanel: {
                    animation: {
                        show: {
                            easing: "cubic-bezier(1, 0, 1, 0)",
                            duration: 500,
                            from: {
                                opacity: 0
                            },
                            to: {
                                opacity: 1
                            }
                        }
                    }
                }
            }
        }, {
            device: function(_device) {
                return "desktop" !== _device.deviceType
            },
            options: {
                grouping: {
                    expandMode: "rowClick"
                }
            }
        }])
    },
    _init: function() {
        this.callBase();
        gridCore.processModules(this, gridCore);
        gridCore.callModuleItemsMethod(this, "init")
    },
    _clean: noop,
    _optionChanged: function(args) {
        gridCore.callModuleItemsMethod(this, "optionChanged", [args]);
        if (!args.handled) {
            this.callBase(args)
        }
    },
    _dimensionChanged: function() {
        this.updateDimensions(true)
    },
    _visibilityChanged: function(visible) {
        if (visible) {
            this.updateDimensions()
        }
    },
    _initMarkup: function() {
        this.callBase.apply(this, arguments);
        this.getView("gridView").render(this.$element())
    },
    _renderContentImpl: function() {
        this.getView("gridView").update()
    },
    _renderContent: function() {
        var that = this;
        deferRender((function() {
            that._renderContentImpl()
        }))
    },
    _getTemplate: function(templateName) {
        var template = templateName;
        if (isString(template) && "#" === template[0]) {
            template = $(templateName);
            logger.warn(DATAGRID_DEPRECATED_TEMPLATE_WARNING)
        }
        return this.callBase(template)
    },
    _dispose: function() {
        this.callBase();
        gridCore.callModuleItemsMethod(this, "dispose")
    },
    isReady: function() {
        return this.getController("data").isReady()
    },
    beginUpdate: function() {
        this.callBase();
        gridCore.callModuleItemsMethod(this, "beginUpdate")
    },
    endUpdate: function() {
        gridCore.callModuleItemsMethod(this, "endUpdate");
        this.callBase()
    },
    getController: function(name) {
        return this._controllers[name]
    },
    getView: function(name) {
        return this._views[name]
    },
    focus: function(element) {
        this.getController("keyboardNavigation").focus(element)
    }
});
DataGrid.registerModule = gridCore.registerModule.bind(gridCore);
registerComponent("dxDataGrid", DataGrid);
export default DataGrid;
