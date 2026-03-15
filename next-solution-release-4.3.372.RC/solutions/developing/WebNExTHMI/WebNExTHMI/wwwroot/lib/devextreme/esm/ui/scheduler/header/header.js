/**
 * DevExtreme (esm/ui/scheduler/header/header.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import _extends from "@babel/runtime/helpers/esm/extends";
import $ from "../../../core/renderer";
import {
    extend
} from "../../../core/utils/extend";
import registerComponent from "../../../core/component_registrator";
import errors from "../../../core/errors";
import devices from "../../../core/devices";
import Widget from "../../widget/ui.widget";
import Toolbar from "../../toolbar";
import SchedulerCalendar from "./calendar";
import dateUtils from "../../../core/utils/date";
import {
    getViewSwitcher,
    getDropDownViewSwitcher
} from "./viewSwitcher";
import {
    getDateNavigator
} from "./dateNavigator";
import "../../../ui/button_group";
import "../../../ui/drop_down_button";
import {
    getCaption,
    getNextIntervalDate,
    validateViews,
    getStep,
    getViewType,
    nextWeek
} from "./utils";
var DEFAULT_ELEMENT = "defaultElement";
var VIEW_SWITCHER = "viewSwitcher";
var DATE_NAVIGATOR = "dateNavigator";
var COMPONENT_CLASS = "dx-scheduler-header";
export class SchedulerHeader extends Widget {
    get views() {
        return this.option("views")
    }
    get captionText() {
        return this._getCaption().text
    }
    get intervalOptions() {
        var step = getStep(this.currentView);
        var intervalCount = this.option("intervalCount");
        var firstDayOfWeek = this.option("firstDayOfWeek");
        var agendaDuration = this.option("agendaDuration");
        return {
            step: step,
            intervalCount: intervalCount,
            firstDayOfWeek: firstDayOfWeek,
            agendaDuration: agendaDuration
        }
    }
    _getDefaultOptions() {
        return extend(super._getDefaultOptions(), {
            _useShortDateFormat: !devices.real().generic || devices.isSimulator()
        })
    }
    _createEventMap() {
        this.eventMap = new Map([
            ["currentView", [view => this.currentView = view]],
            ["items", [this.repaint.bind(this)]],
            ["views", [validateViews]],
            ["currentDate", [this._getCalendarOptionUpdater("date")]],
            ["min", [this._getCalendarOptionUpdater("min")]],
            ["max", [this._getCalendarOptionUpdater("max")]],
            ["tabIndex", [this.repaint.bind(this)]],
            ["focusStateEnabled", [this.repaint.bind(this)]],
            ["useDropDownViewSwitcher", [this.repaint.bind(this)]]
        ])
    }
    _addEvent(name, event) {
        if (!this.eventMap.has(name)) {
            this.eventMap.set(name, [])
        }
        var events = this.eventMap.get(name);
        this.eventMap.set(name, [...events, event])
    }
    _optionChanged(args) {
        var {
            name: name,
            value: value
        } = args;
        if (this.eventMap.has(name)) {
            var events = this.eventMap.get(name);
            events.forEach(event => {
                event(value)
            })
        }
    }
    _init() {
        super._init();
        this._createEventMap();
        this.$element().addClass(COMPONENT_CLASS);
        this.currentView = this.option("currentView")
    }
    _render() {
        super._render();
        this._createEventMap();
        this._renderToolbar()
    }
    _renderToolbar() {
        var config = this._createToolbarConfig();
        var toolbarElement = $("<div>");
        toolbarElement.appendTo(this.$element());
        this._toolbar = this._createComponent(toolbarElement, Toolbar, config)
    }
    _createToolbarConfig() {
        var items = this.option("items");
        var parsedItems = items.map(element => this._parseItem(element));
        return {
            items: parsedItems
        }
    }
    _parseItem(item) {
        var isDefaultElement = this._isDefaultItem(item);
        if (isDefaultElement) {
            var defaultElementType = item[DEFAULT_ELEMENT];
            switch (defaultElementType) {
                case VIEW_SWITCHER:
                    if (this.option("useDropDownViewSwitcher")) {
                        return getDropDownViewSwitcher(this, item)
                    }
                    return getViewSwitcher(this, item);
                case DATE_NAVIGATOR:
                    this._renderCalendar();
                    return getDateNavigator(this, item);
                default:
                    errors.log("Unknown default element type: ".concat(defaultElementType))
            }
        }
        return item
    }
    _callEvent(event, arg) {
        if (this.eventMap.has(event)) {
            var events = this.eventMap.get(event);
            events.forEach(event => event(arg))
        }
    }
    _updateCurrentView(view) {
        var onCurrentViewChange = this.option("onCurrentViewChange");
        onCurrentViewChange(view.name);
        this._callEvent("currentView", view)
    }
    _updateCurrentDate(date) {
        var onCurrentDateChange = this.option("onCurrentDateChange");
        onCurrentDateChange(date);
        this._callEvent("currentDate", date)
    }
    _renderCalendar() {
        this._calendar = this._createComponent("<div>", SchedulerCalendar, {
            date: this.option("currentDate"),
            min: this.option("min"),
            max: this.option("max"),
            firstDayOfWeek: this.option("firstDayOfWeek"),
            focusStateEnabled: this.option("focusStateEnabled"),
            tabIndex: this.option("tabIndex"),
            onValueChanged: e => {
                var date = e.value;
                this._updateCurrentDate(date);
                this._calendar.hide()
            }
        });
        this._calendar.$element().appendTo(this.$element())
    }
    _getCalendarOptionUpdater(name) {
        return value => {
            if (this._calendar) {
                this._calendar.option(name, value)
            }
        }
    }
    _getNextDate(direction) {
        var initialDate = arguments.length > 1 && void 0 !== arguments[1] ? arguments[1] : null;
        var date = initialDate || this.option("currentDate");
        var options = _extends({}, this.intervalOptions, {
            date: date
        });
        return getNextIntervalDate(options, direction)
    }
    _isMonth() {
        var currentView = this.currentView;
        return "month" === getViewType(currentView)
    }
    _getDisplayedDate() {
        var startViewDate = this.option("startViewDate");
        if (this._isMonth()) {
            return nextWeek(startViewDate)
        }
        return new Date(startViewDate)
    }
    _getCaption() {
        var date = this.option("currentDate");
        if (this.option("startViewDate")) {
            date = this._getDisplayedDate()
        }
        date = dateUtils.trimTime(date);
        var options = _extends({}, this.intervalOptions, {
            date: date
        });
        var customizationFunction = this.option("customizeDateNavigatorText");
        var useShortDateFormat = this.option("_useShortDateFormat");
        return getCaption(options, useShortDateFormat, customizationFunction)
    }
    _updateDateByDirection(direction) {
        var date = this._getNextDate(direction);
        this._updateCurrentDate(date)
    }
    _showCalendar(e) {
        this._calendar.show(e.element)
    }
    _hideCalendar() {
        this._calendar.hide()
    }
    _isDefaultItem(item) {
        return Object.prototype.hasOwnProperty.call(item, DEFAULT_ELEMENT)
    }
}
registerComponent("dxSchedulerHeader", SchedulerHeader);
