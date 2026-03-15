/**
 * DevExtreme (esm/ui/tree_view/ui.tree_view.search.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import $ from "../../core/renderer";
import registerComponent from "../../core/component_registrator";
import searchBoxMixin from "../widget/ui.search_box_mixin";
import TextBox from "../text_box";
import {
    extend
} from "../../core/utils/extend";
import TreeViewBase from "./ui.tree_view.base";
searchBoxMixin.setEditorClass(TextBox);
var WIDGET_CLASS = "dx-treeview";
var NODE_CONTAINER_CLASS = "".concat(WIDGET_CLASS, "-node-container");
var TreeViewSearch = TreeViewBase.inherit(searchBoxMixin).inherit({
    _addWidgetPrefix: function(className) {
        return "".concat(WIDGET_CLASS, "-").concat(className)
    },
    _optionChanged: function(args) {
        switch (args.name) {
            case "searchValue":
                if (this._showCheckboxes() && this._isRecursiveSelection()) {
                    this._removeSelection()
                }
                this._initDataAdapter();
                this._updateSearch();
                this._repaintContainer();
                this.option("focusedElement", null);
                break;
            case "searchExpr":
                this._initDataAdapter();
                this.repaint();
                break;
            case "searchMode":
                this.option("expandNodesRecursive") ? this._updateDataAdapter() : this._initDataAdapter();
                this.repaint();
                break;
            default:
                this.callBase(args)
        }
    },
    _updateDataAdapter: function() {
        this._setOptionWithoutOptionChange("expandNodesRecursive", false);
        this._initDataAdapter();
        this._setOptionWithoutOptionChange("expandNodesRecursive", true)
    },
    _getDataAdapterOptions: function() {
        return extend(this.callBase(), {
            searchValue: this.option("searchValue"),
            searchMode: this.option("searchMode") || "contains",
            searchExpr: this.option("searchExpr")
        })
    },
    _updateSearch: function() {
        if (this._searchEditor) {
            var editorOptions = this._getSearchEditorOptions();
            this._searchEditor.option(editorOptions)
        }
    },
    _repaintContainer: function() {
        var $container = this.$element().find(".".concat(NODE_CONTAINER_CLASS)).first();
        var rootNodes;
        if ($container.length) {
            $container.empty();
            rootNodes = this._dataAdapter.getRootNodes();
            this._renderEmptyMessage(rootNodes);
            this._renderItems($container, rootNodes);
            this._fireContentReadyAction()
        }
    },
    _itemContainer: function(isSearchMode) {
        if (this._scrollable && isSearchMode) {
            return $(this._scrollable.content())
        }
        return this.callBase()
    },
    _addWidgetClass: function() {
        this.$element().addClass(this._widgetClass())
    },
    _clean: function() {
        this.callBase();
        this._removeSearchBox()
    }
});
registerComponent("dxTreeView", TreeViewSearch);
export default TreeViewSearch;
