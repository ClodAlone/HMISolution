/**
 * DevExtreme (esm/ui/date_box/ui.date_box.strategy.calendar_with_time.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import {
    getWidth
} from "../../core/utils/size";
import $ from "../../core/renderer";
import {
    getWindow
} from "../../core/utils/window";
var window = getWindow();
import CalendarStrategy from "./ui.date_box.strategy.calendar";
import TimeView from "./ui.time_view";
import dateLocalization from "../../localization/date";
import {
    extend
} from "../../core/utils/extend";
import dateUtils from "../../core/utils/date";
import Box from "../box";
import uiDateUtils from "./ui.date_utils";
var SHRINK_VIEW_SCREEN_WIDTH = 573;
var DATEBOX_ADAPTIVITY_MODE_CLASS = "dx-datebox-adaptivity-mode";
var DATEBOX_TIMEVIEW_SIDE_CLASS = "dx-datebox-datetime-time-side";
var CalendarWithTimeStrategy = CalendarStrategy.inherit({
    NAME: "CalendarWithTime",
    getDefaultOptions: function() {
        return extend(this.callBase(), {
            applyValueMode: "useButtons",
            buttonsLocation: "bottom after",
            "dropDownOptions.showTitle": false
        })
    },
    _closeDropDownByEnter: function() {
        return dateUtils.sameDate(this._getContouredValue(), this.widgetOption("value"))
    },
    getDisplayFormat: function(displayFormat) {
        return displayFormat || "shortdateshorttime"
    },
    _is24HourFormat: function() {
        return dateLocalization.is24HourFormat(this.getDisplayFormat(this.dateBox.option("displayFormat")))
    },
    _getContouredValue: function() {
        var viewDate = this.callBase();
        return this._updateDateTime(viewDate)
    },
    _renderWidget: function() {
        this.callBase();
        this._timeView = this.dateBox._createComponent($("<div>"), TimeView, {
            value: this.dateBoxValue(),
            _showClock: !this._isShrinkView(),
            use24HourFormat: this._is24HourFormat(),
            onValueChanged: this._valueChangedHandler.bind(this),
            stylingMode: this.dateBox.option("stylingMode")
        });
        this._timeView.registerKeyHandler("escape", this._escapeHandler.bind(this))
    },
    renderOpenedState: function() {
        this.callBase();
        var popup = this._getPopup();
        if (popup) {
            popup.$wrapper().toggleClass(DATEBOX_ADAPTIVITY_MODE_CLASS, this._isSmallScreen())
        }
        clearTimeout(this._repaintTimer);
        this._repaintTimer = setTimeout(function() {
            this._getPopup() && this._getPopup().repaint()
        }.bind(this), 0)
    },
    isAdaptivityChanged: function() {
        var isAdaptiveMode = this._isShrinkView();
        var currentAdaptiveMode = this._currentAdaptiveMode;
        if (isAdaptiveMode !== currentAdaptiveMode) {
            this._currentAdaptiveMode = isAdaptiveMode;
            return void 0 !== currentAdaptiveMode
        }
        return this.callBase()
    },
    _updateValue: function(preventDefaultValue) {
        var date = this.dateBoxValue();
        if (!date && !preventDefaultValue) {
            date = new Date;
            uiDateUtils.normalizeTime(date)
        }
        this.callBase();
        if (this._timeView) {
            date && this._timeView.option("value", date);
            this._timeView.option("use24HourFormat", this._is24HourFormat())
        }
    },
    _isSmallScreen: function() {
        return getWidth(window) <= SHRINK_VIEW_SCREEN_WIDTH
    },
    _isShrinkView: function() {
        return !this.dateBox.option("showAnalogClock") || this.dateBox.option("adaptivityEnabled") && this._isSmallScreen()
    },
    _getBoxItems: function() {
        var items = [{
            ratio: 0,
            shrink: 0,
            baseSize: "auto",
            name: "calendar"
        }];
        if (!this._isShrinkView()) {
            items.push({
                ratio: 0,
                shrink: 0,
                baseSize: "auto",
                name: "time"
            })
        }
        return items
    },
    renderPopupContent: function() {
        this.callBase();
        this._currentAdaptiveMode = this._isShrinkView();
        var $popupContent = this._getPopup().$content();
        this._box = this.dateBox._createComponent($("<div>").appendTo($popupContent), Box, {
            direction: "row",
            crossAlign: "stretch",
            items: this._getBoxItems(),
            itemTemplate: function(data, i, element) {
                var $container = $("<div>");
                switch (data.name) {
                    case "calendar":
                        $container.append(this._widget.$element());
                        if (this._isShrinkView()) {
                            this._timeView.$element().addClass(DATEBOX_TIMEVIEW_SIDE_CLASS);
                            $container.append(this._timeView.$element())
                        }
                        break;
                    case "time":
                        $container.append(this._timeView.$element());
                        $(element).addClass(DATEBOX_TIMEVIEW_SIDE_CLASS)
                }
                return $container
            }.bind(this)
        });
        this._attachTabHandler()
    },
    popupConfig: function(_popupConfig) {
        var calendarPopupConfig = this.callBase(_popupConfig);
        var result = extend(calendarPopupConfig, {
            width: "auto",
            onShowing: function() {
                if ("fallback" === this._box.option("_layoutStrategy")) {
                    var clockMinWidth = this._getPopup().$content().find(".dx-timeview-clock").css("minWidth");
                    this._timeView.$element().css("maxWidth", clockMinWidth)
                }
            }.bind(this)
        });
        return result
    },
    getFirstPopupElement: function() {
        return this._timeView._hourBox.$element().find("input")
    },
    _attachTabHandler: function() {
        var dateBox = this.dateBox;
        this._timeView._hourBox.registerKeyHandler("tab", (function(e) {
            if (e.shiftKey) {
                e.preventDefault();
                dateBox.focus()
            }
        }))
    },
    _preventFocusOnPopup: function(e) {
        if (!$(e.target).hasClass("dx-texteditor-input")) {
            this.callBase.apply(this, arguments);
            if (!this.dateBox._hasFocusClass()) {
                this.dateBox.focus()
            }
        }
    },
    _updateDateTime: function(date) {
        var time = this._timeView.option("value");
        date.setHours(time.getHours(), time.getMinutes(), time.getSeconds(), time.getMilliseconds());
        return date
    },
    getValue: function() {
        var _this$_widget$option;
        var date = null !== (_this$_widget$option = this._widget.option("value")) && void 0 !== _this$_widget$option ? _this$_widget$option : this._widget.getContouredDate();
        date = date ? new Date(date) : new Date;
        return this._updateDateTime(date)
    },
    dispose: function() {
        clearTimeout(this._removeMinWidthTimer);
        clearTimeout(this._repaintTimer);
        this.callBase()
    }
});
export default CalendarWithTimeStrategy;
