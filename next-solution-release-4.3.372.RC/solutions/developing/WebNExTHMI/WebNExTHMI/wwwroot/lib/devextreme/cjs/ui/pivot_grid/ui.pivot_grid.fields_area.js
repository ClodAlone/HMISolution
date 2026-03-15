/**
 * DevExtreme (cjs/ui/pivot_grid/ui.pivot_grid.fields_area.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.FieldsArea = void 0;
var _renderer = _interopRequireDefault(require("../../core/renderer"));
var _common = require("../../core/utils/common");
var _iterator = require("../../core/utils/iterator");
var _uiPivot_grid = require("./ui.pivot_grid.area_item");
var _uiPivot_grid2 = require("./ui.pivot_grid.utils");
var _style = require("../../core/utils/style");
var _popup = _interopRequireDefault(require("../popup"));
var _button = _interopRequireDefault(require("../button"));
require("./ui.pivot_grid.field_chooser_base");

function _interopRequireDefault(obj) {
    return obj && obj.__esModule ? obj : {
        default: obj
    }
}
var DIV = "<div>";
var AREA_DRAG_CLASS = "dx-pivotgrid-drag-action";

function renderGroupConnector(field, nextField, prevField, $container) {
    if (prevField && prevField.groupName && prevField.groupName === field.groupName) {
        (0, _renderer.default)(DIV).addClass("dx-group-connector").addClass("dx-group-connector-prev").appendTo($container)
    }
    if (nextField && nextField.groupName && nextField.groupName === field.groupName) {
        (0, _renderer.default)(DIV).addClass("dx-group-connector").addClass("dx-group-connector-next").appendTo($container)
    }
}
var FieldsArea = _uiPivot_grid.AreaItem.inherit({
    ctor: function(component, area) {
        this.callBase(component);
        this._area = area
    },
    _getAreaName: function() {
        return "fields"
    },
    _createGroupElement: function() {
        return (0, _renderer.default)(DIV).addClass("dx-pivotgrid-fields-area").addClass("dx-area-fields").addClass(AREA_DRAG_CLASS).attr("group", this._area)
    },
    isVisible: function() {
        return !!this.option("fieldPanel.visible") && this.option("fieldPanel.show" + (0, _uiPivot_grid2.capitalizeFirstLetter)(this._area) + "Fields")
    },
    _renderButton: function(element) {
        var that = this;
        var container = (0, _renderer.default)("<td>").appendTo((0, _renderer.default)("<tr>").appendTo(element));
        var button = that.component._createComponent((0, _renderer.default)(DIV).appendTo(container), _button.default, {
            text: "Fields",
            icon: "menu",
            width: "auto",
            onClick: function() {
                var popup = that.tableElement().find(".dx-fields-area-popup").dxPopup("instance");
                if (!popup.option("visible")) {
                    popup.show()
                }
            }
        });
        button.$element().addClass("dx-pivotgrid-fields-area-hamburger")
    },
    _getPopupOptions: function(row, button) {
        return {
            contentTemplate: function() {
                return (0, _renderer.default)("<table>").addClass("dx-area-field-container").append((0, _renderer.default)("<thead>").addClass("dx-pivotgrid-fields-area-head").append(row))
            },
            height: "auto",
            width: "auto",
            position: {
                at: "left",
                my: "left",
                of: button
            },
            dragEnabled: false,
            animation: {
                show: {
                    type: "pop",
                    duration: 200
                }
            },
            shading: false,
            showTitle: false,
            closeOnOutsideClick: true,
            container: button.parent()
        }
    },
    _renderPopup: function(tableElement, row) {
        var button = tableElement.find(".dx-button");
        var popupOptions = this._getPopupOptions(row, button);
        var FieldChooserBase = this.component.$element().dxPivotGridFieldChooserBase("instance");
        if (this._rowPopup) {
            this._rowPopup.$element().remove()
        }
        this._rowPopup = this.component._createComponent((0, _renderer.default)(DIV).appendTo(tableElement), _popup.default, popupOptions);
        this._rowPopup.$element().addClass("dx-fields-area-popup");
        this._rowPopup.content().addClass("dx-pivotgrid-fields-container");
        this._rowPopup.content().parent().attr("group", "row");
        FieldChooserBase.subscribeToEvents(this._rowPopup.content());
        FieldChooserBase.renderSortable(this._rowPopup.content())
    },
    _shouldCreateButton: function() {
        return false
    },
    _renderTableContent: function(tableElement, data) {
        var that = this;
        var groupElement = this.groupElement();
        var isVisible = this.isVisible();
        var fieldChooserBase = that.component.$element().dxPivotGridFieldChooserBase("instance");
        var head = (0, _renderer.default)("<thead>").addClass("dx-pivotgrid-fields-area-head").appendTo(tableElement);
        var area = that._area;
        var row = (0, _renderer.default)("<tr>");
        groupElement.toggleClass("dx-hidden", !isVisible);
        tableElement.addClass("dx-area-field-container");
        if (!isVisible) {
            return
        }(0, _iterator.each)(data, (function(index, field) {
            if (field.area === area && false !== field.visible) {
                var td = (0, _renderer.default)("<td>").append(fieldChooserBase.renderField(field, "row" === field.area));
                var indicators = td.find(".dx-column-indicators");
                if (indicators.length && that._shouldCreateButton()) {
                    indicators.insertAfter(indicators.next())
                }
                td.appendTo(row);
                renderGroupConnector(field, data[index + 1], data[index - 1], td)
            }
        }));
        if (!row.children().length) {
            (0, _renderer.default)("<td>").append((0, _renderer.default)(DIV).addClass("dx-empty-area-text").text(this.option("fieldPanel.texts." + area + "FieldArea"))).appendTo(row)
        }
        if (that._shouldCreateButton()) {
            that._renderButton(head);
            that._renderPopup(tableElement, row)
        } else {
            head.append(row)
        }
    },
    setGroupWidth: function(value) {
        (0, _style.setWidth)(this.groupElement(), value)
    },
    setGroupHeight: function(value) {
        (0, _style.setHeight)(this.groupElement(), value)
    },
    reset: function() {
        this.callBase();
        this.groupElement().css("marginTop", 0)
    },
    _renderVirtualContent: _common.noop
});
exports.FieldsArea = FieldsArea;
