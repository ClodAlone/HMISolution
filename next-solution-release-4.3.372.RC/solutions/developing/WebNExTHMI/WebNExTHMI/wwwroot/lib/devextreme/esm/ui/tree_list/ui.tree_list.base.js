/**
 * DevExtreme (esm/ui/tree_list/ui.tree_list.base.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import registerComponent from "../../core/component_registrator";
import {
    noop,
    deferRender
} from "../../core/utils/common";
import {
    isFunction,
    isDefined
} from "../../core/utils/type";
import {
    each
} from "../../core/utils/iterator";
import {
    extend
} from "../../core/utils/extend";
import Widget from "../widget/ui.widget";
import treeListCore from "./ui.tree_list.core";
import {
    isMaterial
} from "../themes";
var callModuleItemsMethod = treeListCore.callModuleItemsMethod;
var DATAGRID_ROW_SELECTOR = ".dx-row";
var TREELIST_CLASS = "dx-treelist";
import "./ui.tree_list.column_headers";
import "./ui.tree_list.columns_controller";
import "./ui.tree_list.data_controller";
import "./ui.tree_list.sorting";
import "./ui.tree_list.rows";
import "./ui.tree_list.context_menu";
import "./ui.tree_list.error_handling";
import "./ui.tree_list.grid_view";
import "./ui.tree_list.header_panel";
treeListCore.registerModulesOrder(["stateStoring", "columns", "selection", "editorFactory", "columnChooser", "editingRowBased", "editingFormBased", "editingCellBased", "editing", "grouping", "masterDetail", "validating", "adaptivity", "data", "virtualScrolling", "columnHeaders", "filterRow", "headerPanel", "headerFilter", "sorting", "search", "rows", "pager", "columnsResizingReordering", "contextMenu", "keyboardNavigation", "errorHandling", "summary", "columnFixing", "export", "gridView"]);
var TreeList = Widget.inherit({
    _activeStateUnit: DATAGRID_ROW_SELECTOR,
    _getDefaultOptions: function() {
        var result = this.callBase();
        each(treeListCore.modules, (function() {
            if (isFunction(this.defaultOptions)) {
                extend(true, result, this.defaultOptions())
            }
        }));
        return result
    },
    _defaultOptionsRules: function() {
        return this.callBase().concat([{
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
                }
            }
        }])
    },
    _init: function() {
        this.callBase();
        treeListCore.processModules(this, treeListCore);
        callModuleItemsMethod(this, "init")
    },
    _clean: noop,
    _optionChanged: function(args) {
        callModuleItemsMethod(this, "optionChanged", [args]);
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
        this.$element().addClass(TREELIST_CLASS);
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
    _dispose: function() {
        this.callBase();
        callModuleItemsMethod(this, "dispose")
    },
    isReady: function() {
        return this.getController("data").isReady()
    },
    beginUpdate: function() {
        this.callBase();
        callModuleItemsMethod(this, "beginUpdate")
    },
    endUpdate: function() {
        callModuleItemsMethod(this, "endUpdate");
        this.callBase()
    },
    getController: function(name) {
        return this._controllers[name]
    },
    getView: function(name) {
        return this._views[name]
    },
    focus: function(element) {
        this.callBase();
        if (isDefined(element)) {
            this.getController("keyboardNavigation").focus(element)
        }
    }
});
TreeList.registerModule = treeListCore.registerModule.bind(treeListCore);
registerComponent("dxTreeList", TreeList);
export default TreeList;
