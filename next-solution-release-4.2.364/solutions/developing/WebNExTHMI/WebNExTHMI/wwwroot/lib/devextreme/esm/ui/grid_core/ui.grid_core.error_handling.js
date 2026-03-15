/**
 * DevExtreme (esm/ui/grid_core/ui.grid_core.error_handling.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import $ from "../../core/renderer";
import eventsEngine from "../../events/core/events_engine";
import {
    name as clickEventName
} from "../../events/click";
import {
    each
} from "../../core/utils/iterator";
import modules from "./ui.grid_core.modules";
var ERROR_ROW_CLASS = "dx-error-row";
var ERROR_MESSAGE_CLASS = "dx-error-message";
var ERROR_CLOSEBUTTON_CLASS = "dx-closebutton";
var ACTION_CLASS = "action";
var ErrorHandlingController = modules.ViewController.inherit({
    init: function() {
        this._columnHeadersView = this.getView("columnHeadersView");
        this._rowsView = this.getView("rowsView")
    },
    _createErrorRow: function(error, $tableElements) {
        var that = this;
        var $errorRow;
        var $closeButton;
        var $errorMessage = this._renderErrorMessage(error);
        if ($tableElements) {
            $errorRow = $("<tr>").addClass(ERROR_ROW_CLASS);
            $closeButton = $("<div>").addClass(ERROR_CLOSEBUTTON_CLASS).addClass(that.addWidgetPrefix(ACTION_CLASS));
            eventsEngine.on($closeButton, clickEventName, that.createAction((function(args) {
                var e = args.event;
                var $errorRow;
                var errorRowIndex = $(e.currentTarget).closest("." + ERROR_ROW_CLASS).index();
                e.stopPropagation();
                each($tableElements, (function(_, tableElement) {
                    $errorRow = $(tableElement).children("tbody").children("tr").eq(errorRowIndex);
                    that.removeErrorRow($errorRow)
                }));
                that.getController("resizing") && that.getController("resizing").fireContentReadyAction()
            })));
            $("<td>").attr({
                colSpan: that.getController("columns").getVisibleColumns().length,
                role: "presentation"
            }).prepend($closeButton).append($errorMessage).appendTo($errorRow);
            return $errorRow
        }
        return $errorMessage
    },
    _renderErrorMessage: function(error) {
        var message = error.url ? error.message.replace(error.url, "") : error.message || error;
        var $message = $("<div>").addClass(ERROR_MESSAGE_CLASS).text(message);
        if (error.url) {
            $("<a>").attr("href", error.url).text(error.url).appendTo($message)
        }
        return $message
    },
    renderErrorRow: function(error, rowIndex, $popupContent) {
        var that = this;
        var $errorMessageElement;
        var $firstErrorRow;
        if ($popupContent) {
            $popupContent.find("." + ERROR_MESSAGE_CLASS).remove();
            $errorMessageElement = that._createErrorRow(error);
            $popupContent.prepend($errorMessageElement);
            return $errorMessageElement
        }
        var viewElement = rowIndex >= 0 || !that._columnHeadersView.isVisible() ? that._rowsView : that._columnHeadersView;
        var $tableElements = viewElement.getTableElements();
        each($tableElements, (function(_, tableElement) {
            $errorMessageElement = that._createErrorRow(error, $tableElements);
            $firstErrorRow = $firstErrorRow || $errorMessageElement;
            if (rowIndex >= 0) {
                var $row = viewElement._getRowElements($(tableElement)).eq(rowIndex);
                that.removeErrorRow($row.next());
                $errorMessageElement.insertAfter($row)
            } else {
                var $tbody = $(tableElement).children("tbody");
                var rowElements = $tbody.children("tr");
                if (that._columnHeadersView.isVisible()) {
                    that.removeErrorRow(rowElements.last());
                    $(tableElement).append($errorMessageElement)
                } else {
                    that.removeErrorRow(rowElements.first());
                    $tbody.first().prepend($errorMessageElement)
                }
            }
        }));
        var resizingController = that.getController("resizing");
        resizingController && resizingController.fireContentReadyAction();
        return $firstErrorRow
    },
    removeErrorRow: function($row) {
        if (!$row) {
            var $columnHeaders = this._columnHeadersView && this._columnHeadersView.element();
            $row = $columnHeaders && $columnHeaders.find("." + ERROR_ROW_CLASS);
            if (!$row || !$row.length) {
                var $rowsViewElement = this._rowsView.element();
                $row = $rowsViewElement && $rowsViewElement.find("." + ERROR_ROW_CLASS)
            }
        }
        $row && $row.hasClass(ERROR_ROW_CLASS) && $row.remove()
    },
    optionChanged: function(args) {
        switch (args.name) {
            case "errorRowEnabled":
                args.handled = true;
                break;
            default:
                this.callBase(args)
        }
    }
});
export var errorHandlingModule = {
    defaultOptions: function() {
        return {
            errorRowEnabled: true
        }
    },
    controllers: {
        errorHandling: ErrorHandlingController
    },
    extenders: {
        controllers: {
            data: {
                init: function() {
                    var that = this;
                    var errorHandlingController = that.getController("errorHandling");
                    that.callBase();
                    that.dataErrorOccurred.add((function(error, $popupContent) {
                        if (that.option("errorRowEnabled")) {
                            errorHandlingController.renderErrorRow(error, void 0, $popupContent)
                        }
                    }));
                    that.changed.add((function(e) {
                        if (e && "loadError" === e.changeType) {
                            return
                        }
                        var errorHandlingController = that.getController("errorHandling");
                        var editingController = that.getController("editing");
                        if (editingController && !editingController.hasChanges()) {
                            errorHandlingController && errorHandlingController.removeErrorRow()
                        }
                    }))
                }
            }
        }
    }
};
