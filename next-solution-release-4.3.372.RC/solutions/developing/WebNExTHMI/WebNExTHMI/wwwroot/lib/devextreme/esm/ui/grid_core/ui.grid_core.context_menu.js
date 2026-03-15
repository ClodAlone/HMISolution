/**
 * DevExtreme (esm/ui/grid_core/ui.grid_core.context_menu.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import $ from "../../core/renderer";
import {
    getPublicElement
} from "../../core/element";
import {
    noop
} from "../../core/utils/common";
import {
    each
} from "../../core/utils/iterator";
import modules from "./ui.grid_core.modules";
import ContextMenu from "../context_menu";
var CONTEXT_MENU = "dx-context-menu";
var viewName = {
    columnHeadersView: "header",
    rowsView: "content",
    footerView: "footer",
    headerPanel: "headerPanel"
};
var VIEW_NAMES = ["columnHeadersView", "rowsView", "footerView", "headerPanel"];
var ContextMenuController = modules.ViewController.inherit({
    init: function() {
        this.createAction("onContextMenuPreparing")
    },
    getContextMenuItems: function(dxEvent) {
        if (!dxEvent) {
            return false
        }
        var that = this;
        var $targetElement = $(dxEvent.target);
        var $element;
        var $targetRowElement;
        var $targetCellElement;
        var menuItems;
        each(VIEW_NAMES, (function() {
            var view = that.getView(this);
            $element = view && view.element();
            if ($element && ($element.is($targetElement) || $element.find($targetElement).length)) {
                var _rowOptions$cells;
                $targetCellElement = $targetElement.closest(".dx-row > td, .dx-row > tr");
                $targetRowElement = $targetCellElement.parent();
                var rowIndex = view.getRowIndex($targetRowElement);
                var columnIndex = $targetCellElement[0] && $targetCellElement[0].cellIndex;
                var rowOptions = $targetRowElement.data("options");
                var options = {
                    event: dxEvent,
                    targetElement: getPublicElement($targetElement),
                    target: viewName[this],
                    rowIndex: rowIndex,
                    row: view._getRows()[rowIndex],
                    columnIndex: columnIndex,
                    column: null === rowOptions || void 0 === rowOptions ? void 0 : null === (_rowOptions$cells = rowOptions.cells) || void 0 === _rowOptions$cells ? void 0 : _rowOptions$cells[columnIndex].column
                };
                options.items = view.getContextMenuItems && view.getContextMenuItems(options);
                that.executeAction("onContextMenuPreparing", options);
                that._contextMenuPrepared(options);
                menuItems = options.items;
                if (menuItems) {
                    return false
                }
            }
        }));
        return menuItems
    },
    _contextMenuPrepared: noop
});
var ContextMenuView = modules.View.inherit({
    _renderCore: function() {
        var that = this;
        var $element = that.element().addClass(CONTEXT_MENU);
        this.setAria("role", "presentation", $element);
        this._createComponent($element, ContextMenu, {
            onPositioning: function(actionArgs) {
                var event = actionArgs.event;
                var contextMenuInstance = actionArgs.component;
                var items = that.getController("contextMenu").getContextMenuItems(event);
                if (items) {
                    contextMenuInstance.option("items", items);
                    event.stopPropagation()
                } else {
                    actionArgs.cancel = true
                }
            },
            onItemClick: function(params) {
                params.itemData.onItemClick && params.itemData.onItemClick(params)
            },
            cssClass: that.getWidgetContainerClass(),
            target: that.component.$element()
        })
    }
});
export var contextMenuModule = {
    defaultOptions: function() {
        return {
            onContextMenuPreparing: null
        }
    },
    controllers: {
        contextMenu: ContextMenuController
    },
    views: {
        contextMenuView: ContextMenuView
    }
};
