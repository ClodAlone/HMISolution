/**
 * DevExtreme (esm/ui/scheduler/appointments/appointmentCollection.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import _extends from "@babel/runtime/helpers/esm/extends";
import {
    setOuterWidth,
    setOuterHeight
} from "../../../core/utils/size";
import $ from "../../../core/renderer";
import domAdapter from "../../../core/dom_adapter";
import eventsEngine from "../../../events/core/events_engine";
import {
    data as elementData
} from "../../../core/element_data";
import {
    locate,
    move
} from "../../../animation/translator";
import dateUtils from "../../../core/utils/date";
import {
    normalizeKey,
    grep
} from "../../../core/utils/common";
import {
    isDefined,
    isDeferred,
    isString,
    isPlainObject
} from "../../../core/utils/type";
import {
    each
} from "../../../core/utils/iterator";
import {
    deepExtendArraySafe
} from "../../../core/utils/object";
import {
    merge,
    inArray,
    wrapToArray
} from "../../../core/utils/array";
import {
    extend
} from "../../../core/utils/extend";
import {
    getPublicElement
} from "../../../core/element";
import {
    getRecurrenceProcessor
} from "../recurrence";
import registerComponent from "../../../core/component_registrator";
import {
    Appointment,
    AgendaAppointment
} from "./appointment";
import {
    addNamespace,
    isFakeClickEvent
} from "../../../events/utils/index";
import {
    name as dblclickEvent
} from "../../../events/double_click";
import CollectionWidget from "../../collection/ui.collection_widget.edit";
import timeZoneUtils from "../utils.timeZone";
import {
    APPOINTMENT_SETTINGS_KEY
} from "../constants";
import {
    APPOINTMENT_ITEM_CLASS,
    APPOINTMENT_DRAG_SOURCE_CLASS
} from "../classes";
import {
    createAgendaAppointmentLayout,
    createAppointmentLayout
} from "./appointmentLayout";
import {
    ExpressionUtils
} from "../expressionUtils";
import {
    createAppointmentAdapter
} from "../appointmentAdapter";
import {
    getAppointmentTakesSeveralDays,
    sortAppointmentsByStartDate
} from "./dataProvider/utils";
import {
    getBoundingRect
} from "../../../core/utils/position";
import {
    getAppointmentDateRange
} from "./resizing/core";
var COMPONENT_CLASS = "dx-scheduler-scrollable-appointments";
var DBLCLICK_EVENT_NAME = addNamespace(dblclickEvent, "dxSchedulerAppointment");
var toMs = dateUtils.dateToMilliseconds;
class SchedulerAppointments extends CollectionWidget {
    get isAgendaView() {
        return this.invoke("isCurrentViewAgenda")
    }
    get isVirtualScrolling() {
        return this.invoke("isVirtualScrolling")
    }
    get appointmentDataProvider() {
        return this.option("getAppointmentDataProvider")()
    }
    constructor(element, options) {
        super(element, options);
        this._virtualAppointments = {}
    }
    notifyObserver(subject, args) {
        var observer = this.option("observer");
        if (observer) {
            observer.fire(subject, args)
        }
    }
    invoke() {
        var observer = this.option("observer");
        if (observer) {
            return observer.fire.apply(observer, arguments)
        }
    }
    _dispose() {
        clearTimeout(this._appointmentClickTimeout);
        super._dispose()
    }
    _supportedKeys() {
        var parent = super._supportedKeys();
        return extend(parent, {
            escape: function() {
                this.moveAppointmentBack();
                this._escPressed = true
            }.bind(this),
            del: function(e) {
                if (this.option("allowDelete")) {
                    e.preventDefault();
                    var data = this._getItemData(e.target);
                    this.notifyObserver("onDeleteButtonPress", {
                        data: data,
                        target: e.target
                    })
                }
            }.bind(this),
            tab: function(e) {
                var appointments = this._getAccessAppointments();
                var focusedAppointment = appointments.filter(".dx-state-focused");
                var index = focusedAppointment.data(APPOINTMENT_SETTINGS_KEY).sortedIndex;
                var lastIndex = appointments.length - 1;
                if (index > 0 && e.shiftKey || index < lastIndex && !e.shiftKey) {
                    e.preventDefault();
                    e.shiftKey ? index-- : index++;
                    var $nextAppointment = this._getAppointmentByIndex(index);
                    this._resetTabIndex($nextAppointment);
                    eventsEngine.trigger($nextAppointment, "focus")
                }
            }
        })
    }
    _getAppointmentByIndex(sortedIndex) {
        var appointments = this._getAccessAppointments();
        return appointments.filter((function(_, $item) {
            return elementData($item, APPOINTMENT_SETTINGS_KEY).sortedIndex === sortedIndex
        })).eq(0)
    }
    _getAccessAppointments() {
        return this._itemElements().filter(":visible").not(".dx-state-disabled")
    }
    _resetTabIndex($appointment) {
        this._focusTarget().attr("tabIndex", -1);
        $appointment.attr("tabIndex", this.option("tabIndex"))
    }
    _moveFocus() {}
    _focusTarget() {
        return this._itemElements()
    }
    _renderFocusTarget() {
        var $appointment = this._getAppointmentByIndex(0);
        this._resetTabIndex($appointment)
    }
    _focusInHandler(e) {
        super._focusInHandler(e);
        this._$currentAppointment = $(e.target);
        this.option("focusedElement", getPublicElement($(e.target)))
    }
    _focusOutHandler(e) {
        var $appointment = this._getAppointmentByIndex(0);
        this.option("focusedElement", getPublicElement($appointment));
        super._focusOutHandler(e)
    }
    _eventBindingTarget() {
        return this._itemContainer()
    }
    _getDefaultOptions() {
        return extend(super._getDefaultOptions(), {
            noDataText: null,
            activeStateEnabled: true,
            hoverStateEnabled: true,
            tabIndex: 0,
            fixedContainer: null,
            allDayContainer: null,
            allowDrag: true,
            allowResize: true,
            allowAllDayResize: true,
            onAppointmentDblClick: null,
            _collectorOffset: 0,
            groups: [],
            resources: []
        })
    }
    _optionChanged(args) {
        if (this.option("isRenovatedAppointments")) {
            return
        }
        switch (args.name) {
            case "items":
                this._cleanFocusState();
                this._clearDropDownItems();
                this._clearDropDownItemsElements();
                this._repaintAppointments(args.value);
                this._renderDropDownAppointments();
                this._attachAppointmentsEvents();
                break;
            case "fixedContainer":
            case "allDayContainer":
            case "onAppointmentDblClick":
                break;
            case "allowDrag":
            case "allowResize":
            case "allowAllDayResize":
                this._invalidate();
                break;
            case "focusedElement":
                this._resetTabIndex($(args.value));
                super._optionChanged(args);
                break;
            case "allowDelete":
                break;
            case "focusStateEnabled":
                this._clearDropDownItemsElements();
                this._renderDropDownAppointments();
                super._optionChanged(args);
                break;
            default:
                super._optionChanged(args)
        }
    }
    _isAllDayAppointment(appointment) {
        return appointment.settings.length && appointment.settings[0].allDay || false
    }
    _isRepaintAppointment(appointment) {
        return !isDefined(appointment.needRepaint) || true === appointment.needRepaint
    }
    _isRepaintAll(appointments) {
        if (this.isAgendaView) {
            return true
        }
        for (var i = 0; i < appointments.length; i++) {
            if (!this._isRepaintAppointment(appointments[i])) {
                return false
            }
        }
        return true
    }
    _applyFragment(fragment, allDay) {
        if (fragment.children().length > 0) {
            this._getAppointmentContainer(allDay).append(fragment)
        }
    }
    _onEachAppointment(appointment, index, container, isRepaintAll) {
        if (true === (null === appointment || void 0 === appointment ? void 0 : appointment.needRemove)) {
            this._clearItem(appointment)
        } else if (isRepaintAll || this._isRepaintAppointment(appointment)) {
            (() => {
                appointment.needRepaint = false;
                this._clearItem(appointment);
                this._renderItem(index, appointment, container)
            })()
        }
    }
    _repaintAppointments(appointments) {
        this._renderByFragments(($commonFragment, $allDayFragment) => {
            var isRepaintAll = this._isRepaintAll(appointments);
            if (isRepaintAll) {
                this._getAppointmentContainer(true).html("");
                this._getAppointmentContainer(false).html("")
            }!appointments.length && this._cleanItemContainer();
            appointments.forEach((appointment, index) => {
                var container = this._isAllDayAppointment(appointment) ? $allDayFragment : $commonFragment;
                this._onEachAppointment(appointment, index, container, isRepaintAll)
            })
        })
    }
    _renderByFragments(renderFunction) {
        if (this.isVirtualScrolling) {
            var $commonFragment = $(domAdapter.createDocumentFragment());
            var $allDayFragment = $(domAdapter.createDocumentFragment());
            renderFunction($commonFragment, $allDayFragment);
            this._applyFragment($commonFragment, false);
            this._applyFragment($allDayFragment, true)
        } else {
            renderFunction(this._getAppointmentContainer(false), this._getAppointmentContainer(true))
        }
    }
    _attachAppointmentsEvents() {
        this._attachClickEvent();
        this._attachHoldEvent();
        this._attachContextMenuEvent();
        this._attachAppointmentDblClick();
        this._renderFocusState();
        this._attachFeedbackEvents();
        this._attachHoverEvents()
    }
    _clearItem(item) {
        var $items = this._findItemElementByItem(item.itemData);
        if (!$items.length) {
            return
        }
        each($items, (function(_, $item) {
            $item.detach();
            $item.remove()
        }))
    }
    _clearDropDownItems() {
        this._virtualAppointments = {}
    }
    _clearDropDownItemsElements() {
        this.invoke("clearCompactAppointments")
    }
    _findItemElementByItem(item) {
        var result = [];
        var that = this;
        this.itemElements().each((function() {
            var $item = $(this);
            if ($item.data(that._itemDataKey()) === item) {
                result.push($item)
            }
        }));
        return result
    }
    _itemClass() {
        return APPOINTMENT_ITEM_CLASS
    }
    _itemContainer() {
        var $container = super._itemContainer();
        var $result = $container;
        var $allDayContainer = this.option("allDayContainer");
        if ($allDayContainer) {
            $result = $container.add($allDayContainer)
        }
        return $result
    }
    _cleanItemContainer() {
        super._cleanItemContainer();
        var $allDayContainer = this.option("allDayContainer");
        if ($allDayContainer) {
            $allDayContainer.empty()
        }
        this._virtualAppointments = {}
    }
    _clean() {
        super._clean();
        delete this._$currentAppointment;
        delete this._initialSize;
        delete this._initialCoordinates
    }
    _init() {
        super._init();
        this.$element().addClass(COMPONENT_CLASS);
        this._preventSingleAppointmentClick = false
    }
    _renderAppointmentTemplate($container, appointment, model) {
        var config = {
            isAllDay: appointment.allDay,
            isRecurrence: appointment.recurrenceRule,
            html: isPlainObject(appointment) && appointment.html ? appointment.html : void 0
        };
        var formatText = this.invoke("getTextAndFormatDate", model.appointmentData, this._currentAppointmentSettings.agendaSettings || model.targetedAppointmentData, "TIME");
        $container.append(this.isAgendaView ? createAgendaAppointmentLayout(formatText, config) : createAppointmentLayout(formatText, config))
    }
    _executeItemRenderAction(index, itemData, itemElement) {
        var action = this._getItemRenderAction();
        if (action) {
            action(this.invoke("mapAppointmentFields", {
                itemData: itemData,
                itemElement: itemElement
            }))
        }
        delete this._currentAppointmentSettings
    }
    _itemClickHandler(e) {
        super._itemClickHandler(e, {}, {
            afterExecute: function(e) {
                this._processItemClick(e.args[0].event)
            }.bind(this)
        })
    }
    _processItemClick(e) {
        var $target = $(e.currentTarget);
        var data = this._getItemData($target);
        if ("keydown" === e.type || isFakeClickEvent(e)) {
            this.notifyObserver("showEditAppointmentPopup", {
                data: data,
                target: $target
            });
            return
        }
        this._appointmentClickTimeout = setTimeout(function() {
            if (!this._preventSingleAppointmentClick && domAdapter.getBody().contains($target[0])) {
                this.notifyObserver("showAppointmentTooltip", {
                    data: data,
                    target: $target
                })
            }
            this._preventSingleAppointmentClick = false
        }.bind(this), 300)
    }
    _extendActionArgs($itemElement) {
        var args = super._extendActionArgs($itemElement);
        return this.invoke("mapAppointmentFields", args)
    }
    _render() {
        super._render();
        this._attachAppointmentDblClick()
    }
    _attachAppointmentDblClick() {
        var that = this;
        var itemSelector = that._itemSelector();
        var itemContainer = this._itemContainer();
        eventsEngine.off(itemContainer, DBLCLICK_EVENT_NAME, itemSelector);
        eventsEngine.on(itemContainer, DBLCLICK_EVENT_NAME, itemSelector, (function(e) {
            that._itemDXEventHandler(e, "onAppointmentDblClick", {}, {
                afterExecute: function(e) {
                    that._dblClickHandler(e.args[0].event)
                }
            })
        }))
    }
    _dblClickHandler(e) {
        var $targetAppointment = $(e.currentTarget);
        var appointmentData = this._getItemData($targetAppointment);
        clearTimeout(this._appointmentClickTimeout);
        this._preventSingleAppointmentClick = true;
        this.notifyObserver("showEditAppointmentPopup", {
            data: appointmentData,
            target: $targetAppointment
        })
    }
    _renderItem(index, item, container) {
        var itemData = item.itemData;
        var $items = [];
        for (var i = 0; i < item.settings.length; i++) {
            var setting = item.settings[i];
            this._currentAppointmentSettings = setting;
            var $item = super._renderItem(index, itemData, container);
            $item.data(APPOINTMENT_SETTINGS_KEY, setting);
            $items.push($item)
        }
        return $items
    }
    _getItemContent($itemFrame) {
        $itemFrame.data(APPOINTMENT_SETTINGS_KEY, this._currentAppointmentSettings);
        var $itemContent = super._getItemContent($itemFrame);
        return $itemContent
    }
    _createItemByTemplate(itemTemplate, renderArgs) {
        var {
            itemData: itemData,
            container: container,
            index: index
        } = renderArgs;
        return itemTemplate.render({
            model: {
                appointmentData: itemData,
                targetedAppointmentData: this.invoke("getTargetedAppointmentData", itemData, $(container).parent())
            },
            container: container,
            index: index
        })
    }
    _getAppointmentContainer(allDay) {
        var $allDayContainer = this.option("allDayContainer");
        var $container = this.itemsContainer().not($allDayContainer);
        if (allDay && $allDayContainer) {
            $container = $allDayContainer
        }
        return $container
    }
    _postprocessRenderItem(args) {
        this._renderAppointment(args.itemElement, this._currentAppointmentSettings)
    }
    _renderAppointment(element, settings) {
        element.data(APPOINTMENT_SETTINGS_KEY, settings);
        this._applyResourceDataAttr(element);
        var rawAppointment = this._getItemData(element);
        var geometry = this.invoke("getAppointmentGeometry", settings);
        var allowResize = this.option("allowResize") && (!isDefined(settings.skipResizing) || isString(settings.skipResizing));
        var allowDrag = this.option("allowDrag");
        var allDay = settings.allDay;
        this.invoke("setCellDataCacheAlias", this._currentAppointmentSettings, geometry);
        if (settings.virtual) {
            var appointmentConfig = {
                itemData: rawAppointment,
                groupIndex: settings.groupIndex,
                groups: this.option("groups")
            };
            var deferredColor = this.option("getAppointmentColor")(appointmentConfig);
            this._processVirtualAppointment(settings, element, rawAppointment, deferredColor)
        } else {
            var _settings$info;
            var config = {
                data: rawAppointment,
                groupIndex: settings.groupIndex,
                observer: this.option("observer"),
                geometry: geometry,
                direction: settings.direction || "vertical",
                allowResize: allowResize,
                allowDrag: allowDrag,
                allDay: allDay,
                reduced: settings.appointmentReduced,
                isCompact: settings.isCompact,
                startDate: new Date(null === (_settings$info = settings.info) || void 0 === _settings$info ? void 0 : _settings$info.appointment.startDate),
                cellWidth: this.invoke("getCellWidth"),
                cellHeight: this.invoke("getCellHeight"),
                resizableConfig: this._resizableConfig(rawAppointment, settings),
                groups: this.option("groups"),
                getAppointmentColor: this.option("getAppointmentColor"),
                getResourceDataAccessors: this.option("getResourceDataAccessors")
            };
            if (this.isAgendaView) {
                var agendaResourceProcessor = this.option("getAgendaResourceProcessor")();
                config.createPlainResourceListAsync = rawAppointment => agendaResourceProcessor.createListAsync(rawAppointment)
            }
            this._createComponent(element, this.isAgendaView ? AgendaAppointment : Appointment, _extends({}, config, {
                dataAccessors: this.option("dataAccessors"),
                getResizableStep: this.option("getResizableStep")
            }))
        }
    }
    _applyResourceDataAttr($appointment) {
        var dataAccessors = this.option("getResourceDataAccessors")();
        var rawAppointment = this._getItemData($appointment);
        each(dataAccessors.getter, (function(key) {
            var value = dataAccessors.getter[key](rawAppointment);
            if (isDefined(value)) {
                var prefix = "data-".concat(normalizeKey(key.toLowerCase()), "-");
                wrapToArray(value).forEach(value => $appointment.attr(prefix + normalizeKey(value), true))
            }
        }))
    }
    _resizableConfig(appointmentData, itemSetting) {
        return {
            area: this._calculateResizableArea(itemSetting, appointmentData),
            onResizeStart: function(e) {
                this._$currentAppointment = $(e.element);
                if (this.invoke("needRecalculateResizableArea")) {
                    var updatedArea = this._calculateResizableArea(this._$currentAppointment.data(APPOINTMENT_SETTINGS_KEY), this._$currentAppointment.data("dxItemData"));
                    e.component.option("area", updatedArea);
                    e.component._renderDragOffsets(e.event)
                }
                this._initialSize = {
                    width: e.width,
                    height: e.height
                };
                this._initialCoordinates = locate(this._$currentAppointment)
            }.bind(this),
            onResizeEnd: function(e) {
                if (this._escPressed) {
                    e.event.cancel = true;
                    return
                }
                this._resizeEndHandler(e)
            }.bind(this)
        }
    }
    _calculateResizableArea(itemSetting, appointmentData) {
        var area = this.$element().closest(".dx-scrollable-content");
        return this.invoke("getResizableAppointmentArea", {
            coordinates: {
                left: itemSetting.left,
                top: 0,
                groupIndex: itemSetting.groupIndex
            },
            allDay: itemSetting.allDay
        }) || area
    }
    _resizeEndHandler(e) {
        var $element = $(e.element);
        var {
            allDay: allDay,
            info: info
        } = $element.data("dxAppointmentSettings");
        var sourceAppointment = this._getItemData($element);
        var dateRange = {};
        if (allDay) {
            dateRange = this.resizeAllDay(e)
        } else {
            var startDate = this._getEndResizeAppointmentStartDate(e, sourceAppointment, info.appointment);
            var endDate = info.appointment.endDate;
            dateRange = this._getDateRange(e, startDate, endDate)
        }
        this.updateResizedAppointment($element, dateRange, this.option("dataAccessors"), this.option("timeZoneCalculator"))
    }
    resizeAllDay(e) {
        var $element = $(e.element);
        var timeZoneCalculator = this.option("timeZoneCalculator");
        var dataAccessors = this.option("dataAccessors");
        return getAppointmentDateRange({
            handles: e.handles,
            appointmentSettings: $element.data("dxAppointmentSettings"),
            isVerticalViewDirection: this.option("isVerticalViewDirection")(),
            isVerticalGroupedWorkSpace: this.option("isVerticalGroupedWorkSpace")(),
            appointmentRect: getBoundingRect($element[0]),
            parentAppointmentRect: getBoundingRect($element.parent()[0]),
            viewDataProvider: this.option("getViewDataProvider")(),
            isDateAndTimeView: this.option("isDateAndTimeView")(),
            startDayHour: this.invoke("getStartDayHour"),
            endDayHour: this.invoke("getEndDayHour"),
            timeZoneCalculator: timeZoneCalculator,
            dataAccessors: dataAccessors,
            rtlEnabled: this.option("rtlEnabled"),
            DOMMetaData: this.option("getDOMElementsMetaData")()
        })
    }
    updateResizedAppointment($element, dateRange, dataAccessors, timeZoneCalculator) {
        var sourceAppointment = this._getItemData($element);
        var modifiedAppointmentAdapter = createAppointmentAdapter(sourceAppointment, dataAccessors, timeZoneCalculator).clone();
        modifiedAppointmentAdapter.startDate = new Date(dateRange.startDate);
        modifiedAppointmentAdapter.endDate = new Date(dateRange.endDate);
        this.notifyObserver("updateAppointmentAfterResize", {
            target: sourceAppointment,
            data: modifiedAppointmentAdapter.clone({
                pathTimeZone: "fromGrid"
            }).source(),
            $appointment: $element
        })
    }
    _getEndResizeAppointmentStartDate(e, rawAppointment, appointmentInfo) {
        var timeZoneCalculator = this.option("timeZoneCalculator");
        var appointmentAdapter = createAppointmentAdapter(rawAppointment, this.option("dataAccessors"), timeZoneCalculator);
        var startDate = appointmentInfo.startDate;
        var recurrenceProcessor = getRecurrenceProcessor();
        var {
            recurrenceRule: recurrenceRule,
            startDateTimeZone: startDateTimeZone
        } = appointmentAdapter;
        var isAllDay = this.invoke("isAllDay", rawAppointment);
        var isRecurrent = recurrenceProcessor.isValidRecurrenceRule(recurrenceRule);
        if (!e.handles.top && !isRecurrent && !isAllDay) {
            startDate = timeZoneCalculator.createDate(appointmentAdapter.startDate, {
                appointmentTimeZone: startDateTimeZone,
                path: "toGrid"
            })
        }
        return startDate
    }
    _getDateRange(e, startDate, endDate) {
        var itemData = this._getItemData(e.element);
        var deltaTime = this.invoke("getDeltaTime", e, this._initialSize, itemData);
        var renderingStrategyDirection = this.invoke("getRenderingStrategyDirection");
        var isStartDateChanged = false;
        var isAllDay = this.invoke("isAllDay", itemData);
        var needCorrectDates = this.invoke("needCorrectAppointmentDates") && !isAllDay;
        var startTime;
        var endTime;
        if ("vertical" !== renderingStrategyDirection || isAllDay) {
            isStartDateChanged = this.option("rtlEnabled") ? e.handles.right : e.handles.left
        } else {
            isStartDateChanged = e.handles.top
        }
        if (isStartDateChanged) {
            startTime = needCorrectDates ? this._correctStartDateByDelta(startDate, deltaTime) : startDate.getTime() - deltaTime;
            startTime += timeZoneUtils.getTimezoneOffsetChangeInMs(startDate, endDate, startTime, endDate);
            endTime = endDate.getTime()
        } else {
            startTime = startDate.getTime();
            endTime = needCorrectDates ? this._correctEndDateByDelta(endDate, deltaTime) : endDate.getTime() + deltaTime;
            endTime -= timeZoneUtils.getTimezoneOffsetChangeInMs(startDate, endDate, startDate, endTime)
        }
        return {
            startDate: new Date(startTime),
            endDate: new Date(endTime)
        }
    }
    _correctEndDateByDelta(endDate, deltaTime) {
        var endDayHour = this.invoke("getEndDayHour");
        var startDayHour = this.invoke("getStartDayHour");
        var result = endDate.getTime() + deltaTime;
        var visibleDayDuration = (endDayHour - startDayHour) * toMs("hour");
        var daysCount = deltaTime > 0 ? Math.ceil(deltaTime / visibleDayDuration) : Math.floor(deltaTime / visibleDayDuration);
        var maxDate = new Date(endDate);
        var minDate = new Date(endDate);
        minDate.setHours(startDayHour, 0, 0, 0);
        maxDate.setHours(endDayHour, 0, 0, 0);
        if (result > maxDate.getTime() || result <= minDate.getTime()) {
            var tailOfCurrentDay = maxDate.getTime() - endDate.getTime();
            var tailOfPrevDays = deltaTime - tailOfCurrentDay;
            var lastDay = new Date(endDate.setDate(endDate.getDate() + daysCount));
            lastDay.setHours(startDayHour, 0, 0, 0);
            result = lastDay.getTime() + tailOfPrevDays - visibleDayDuration * (daysCount - 1)
        }
        return result
    }
    _correctStartDateByDelta(startDate, deltaTime) {
        var endDayHour = this.invoke("getEndDayHour");
        var startDayHour = this.invoke("getStartDayHour");
        var result = startDate.getTime() - deltaTime;
        var visibleDayDuration = (endDayHour - startDayHour) * toMs("hour");
        var daysCount = deltaTime > 0 ? Math.ceil(deltaTime / visibleDayDuration) : Math.floor(deltaTime / visibleDayDuration);
        var maxDate = new Date(startDate);
        var minDate = new Date(startDate);
        minDate.setHours(startDayHour, 0, 0, 0);
        maxDate.setHours(endDayHour, 0, 0, 0);
        if (result < minDate.getTime() || result >= maxDate.getTime()) {
            var tailOfCurrentDay = startDate.getTime() - minDate.getTime();
            var tailOfPrevDays = deltaTime - tailOfCurrentDay;
            var firstDay = new Date(startDate.setDate(startDate.getDate() - daysCount));
            firstDay.setHours(endDayHour, 0, 0, 0);
            result = firstDay.getTime() - tailOfPrevDays + visibleDayDuration * (daysCount - 1)
        }
        return result
    }
    _processVirtualAppointment(appointmentSetting, $appointment, appointmentData, color) {
        var virtualAppointment = appointmentSetting.virtual;
        var virtualGroupIndex = virtualAppointment.index;
        if (!isDefined(this._virtualAppointments[virtualGroupIndex])) {
            this._virtualAppointments[virtualGroupIndex] = {
                coordinates: {
                    top: virtualAppointment.top,
                    left: virtualAppointment.left
                },
                items: {
                    data: [],
                    colors: [],
                    settings: []
                },
                isAllDay: virtualAppointment.isAllDay ? true : false,
                buttonColor: color
            }
        }
        appointmentSetting.targetedAppointmentData = this.invoke("getTargetedAppointmentData", appointmentData, $appointment);
        this._virtualAppointments[virtualGroupIndex].items.settings.push(appointmentSetting);
        this._virtualAppointments[virtualGroupIndex].items.data.push(appointmentData);
        this._virtualAppointments[virtualGroupIndex].items.colors.push(color);
        $appointment.remove()
    }
    _renderContentImpl() {
        super._renderContentImpl();
        this._renderDropDownAppointments()
    }
    _renderDropDownAppointments() {
        this._renderByFragments(($commonFragment, $allDayFragment) => {
            each(this._virtualAppointments, function(groupIndex) {
                var virtualGroup = this._virtualAppointments[groupIndex];
                var virtualItems = virtualGroup.items;
                var virtualCoordinates = virtualGroup.coordinates;
                var $fragment = virtualGroup.isAllDay ? $allDayFragment : $commonFragment;
                var left = virtualCoordinates.left;
                var buttonWidth = this.invoke("getDropDownAppointmentWidth", virtualGroup.isAllDay);
                var buttonHeight = this.invoke("getDropDownAppointmentHeight");
                var rtlOffset = this.option("rtlEnabled") ? buttonWidth : 0;
                this.notifyObserver("renderCompactAppointments", {
                    $container: $fragment,
                    coordinates: {
                        top: virtualCoordinates.top,
                        left: left + rtlOffset
                    },
                    items: virtualItems,
                    buttonColor: virtualGroup.buttonColor,
                    width: buttonWidth - this.option("_collectorOffset"),
                    height: buttonHeight,
                    onAppointmentClick: this.option("onItemClick"),
                    allowDrag: this.option("allowDrag"),
                    cellWidth: this.invoke("getCellWidth"),
                    isCompact: this.invoke("isAdaptive") || this._isGroupCompact(virtualGroup)
                })
            }.bind(this))
        })
    }
    _isGroupCompact(virtualGroup) {
        return !virtualGroup.isAllDay && this.invoke("supportCompactDropDownAppointments")
    }
    _sortAppointmentsByStartDate(appointments) {
        return sortAppointmentsByStartDate(appointments, this.option("dataAccessors"))
    }
    _processRecurrenceAppointment(appointment, index, skipLongAppointments) {
        var recurrenceRule = ExpressionUtils.getField(this.option("dataAccessors"), "recurrenceRule", appointment);
        var result = {
            parts: [],
            indexes: []
        };
        if (recurrenceRule) {
            var dates = appointment.settings || appointment;
            var startDate = new Date(ExpressionUtils.getField(this.option("dataAccessors"), "startDate", dates));
            var endDate = new Date(ExpressionUtils.getField(this.option("dataAccessors"), "endDate", dates));
            var appointmentDuration = endDate.getTime() - startDate.getTime();
            var recurrenceException = ExpressionUtils.getField(this.option("dataAccessors"), "recurrenceException", appointment);
            var startViewDate = this.invoke("getStartViewDate");
            var endViewDate = this.invoke("getEndViewDate");
            var recurrentDates = getRecurrenceProcessor().generateDates({
                rule: recurrenceRule,
                exception: recurrenceException,
                start: startDate,
                end: endDate,
                min: startViewDate,
                max: endViewDate
            });
            var recurrentDateCount = appointment.settings ? 1 : recurrentDates.length;
            for (var i = 0; i < recurrentDateCount; i++) {
                var appointmentPart = extend({}, appointment, true);
                if (recurrentDates[i]) {
                    var appointmentSettings = this._applyStartDateToObj(recurrentDates[i], {});
                    this._applyEndDateToObj(new Date(recurrentDates[i].getTime() + appointmentDuration), appointmentSettings);
                    appointmentPart.settings = appointmentSettings
                } else {
                    appointmentPart.settings = dates
                }
                result.parts.push(appointmentPart);
                if (!skipLongAppointments) {
                    this._processLongAppointment(appointmentPart, result)
                }
            }
            result.indexes.push(index)
        }
        return result
    }
    _processLongAppointment(appointment, result) {
        var parts = this.splitAppointmentByDay(appointment);
        var partCount = parts.length;
        var endViewDate = this.invoke("getEndViewDate").getTime();
        var startViewDate = this.invoke("getStartViewDate").getTime();
        var timeZoneCalculator = this.option("timeZoneCalculator");
        result = result || {
            parts: []
        };
        if (partCount > 1) {
            extend(appointment, parts[0]);
            for (var i = 1; i < partCount; i++) {
                var startDate = ExpressionUtils.getField(this.option("dataAccessors"), "startDate", parts[i].settings).getTime();
                startDate = timeZoneCalculator.createDate(startDate, {
                    path: "toGrid"
                });
                if (startDate < endViewDate && startDate > startViewDate) {
                    result.parts.push(parts[i])
                }
            }
        }
        return result
    }
    _reduceRecurrenceAppointments(recurrenceIndexes, appointments) {
        each(recurrenceIndexes, (function(i, index) {
            appointments.splice(index - i, 1)
        }))
    }
    _combineAppointments(appointments, additionalAppointments) {
        if (additionalAppointments.length) {
            merge(appointments, additionalAppointments)
        }
        this._sortAppointmentsByStartDate(appointments)
    }
    _applyStartDateToObj(startDate, obj) {
        ExpressionUtils.setField(this.option("dataAccessors"), "startDate", obj, startDate);
        return obj
    }
    _applyEndDateToObj(endDate, obj) {
        ExpressionUtils.setField(this.option("dataAccessors"), "endDate", obj, endDate);
        return obj
    }
    moveAppointmentBack(dragEvent) {
        var $appointment = this._$currentAppointment;
        var size = this._initialSize;
        var coords = this._initialCoordinates;
        if (dragEvent) {
            this._removeDragSourceClassFromDraggedAppointment();
            if (isDeferred(dragEvent.cancel)) {
                dragEvent.cancel.resolve(true)
            } else {
                dragEvent.cancel = true
            }
        }
        if ($appointment && !dragEvent) {
            if (coords) {
                move($appointment, coords);
                delete this._initialSize
            }
            if (size) {
                setOuterWidth($appointment, size.width);
                setOuterHeight($appointment, size.height);
                delete this._initialCoordinates
            }
        }
    }
    focus() {
        if (this._$currentAppointment) {
            var focusedElement = getPublicElement(this._$currentAppointment);
            this.option("focusedElement", focusedElement);
            eventsEngine.trigger(focusedElement, "focus")
        }
    }
    splitAppointmentByDay(appointment) {
        var dates = appointment.settings || appointment;
        var dataAccessors = this.option("dataAccessors");
        var originalStartDate = new Date(ExpressionUtils.getField(dataAccessors, "startDate", dates));
        var startDate = dateUtils.makeDate(originalStartDate);
        var endDate = dateUtils.makeDate(ExpressionUtils.getField(dataAccessors, "endDate", dates));
        var maxAllowedDate = this.invoke("getEndViewDate");
        var startDayHour = this.invoke("getStartDayHour");
        var endDayHour = this.invoke("getEndDayHour");
        var timeZoneCalculator = this.option("timeZoneCalculator");
        var adapter = createAppointmentAdapter(appointment, dataAccessors, timeZoneCalculator);
        var appointmentIsLong = getAppointmentTakesSeveralDays(adapter);
        var result = [];
        startDate = timeZoneCalculator.createDate(startDate, {
            path: "toGrid"
        });
        endDate = timeZoneCalculator.createDate(endDate, {
            path: "toGrid"
        });
        if (startDate.getHours() <= endDayHour && startDate.getHours() >= startDayHour && !appointmentIsLong) {
            result.push(this._applyStartDateToObj(new Date(startDate), {
                appointmentData: appointment
            }));
            startDate.setDate(startDate.getDate() + 1)
        }
        while (appointmentIsLong && startDate.getTime() < endDate.getTime() && startDate < maxAllowedDate) {
            var currentStartDate = new Date(startDate);
            var currentEndDate = new Date(startDate);
            this._checkStartDate(currentStartDate, originalStartDate, startDayHour);
            this._checkEndDate(currentEndDate, endDate, endDayHour);
            var appointmentData = deepExtendArraySafe({}, appointment, true);
            var appointmentSettings = {};
            this._applyStartDateToObj(currentStartDate, appointmentSettings);
            this._applyEndDateToObj(currentEndDate, appointmentSettings);
            appointmentData.settings = appointmentSettings;
            result.push(appointmentData);
            startDate = dateUtils.trimTime(startDate);
            startDate.setDate(startDate.getDate() + 1);
            startDate.setHours(startDayHour)
        }
        return result
    }
    _checkStartDate(currentDate, originalDate, startDayHour) {
        if (!dateUtils.sameDate(currentDate, originalDate) || currentDate.getHours() <= startDayHour) {
            currentDate.setHours(startDayHour, 0, 0, 0)
        } else {
            currentDate.setHours(originalDate.getHours(), originalDate.getMinutes(), originalDate.getSeconds(), originalDate.getMilliseconds())
        }
    }
    _checkEndDate(currentDate, originalDate, endDayHour) {
        if (!dateUtils.sameDate(currentDate, originalDate) || currentDate.getHours() > endDayHour) {
            currentDate.setHours(endDayHour, 0, 0, 0)
        } else {
            currentDate.setHours(originalDate.getHours(), originalDate.getMinutes(), originalDate.getSeconds(), originalDate.getMilliseconds())
        }
    }
    _removeDragSourceClassFromDraggedAppointment() {
        var $appointments = this._itemElements().filter(".".concat(APPOINTMENT_DRAG_SOURCE_CLASS));
        $appointments.each((_, element) => {
            var appointmentInstance = $(element).dxSchedulerAppointment("instance");
            appointmentInstance.option("isDragSource", false)
        })
    }
    _setDragSourceAppointment(appointment, settings) {
        var $appointments = this._findItemElementByItem(appointment);
        var {
            startDate: startDate,
            endDate: endDate
        } = settings.info.sourceAppointment;
        var {
            groupIndex: groupIndex
        } = settings;
        $appointments.forEach($item => {
            var {
                info: itemInfo,
                groupIndex: itemGroupIndex
            } = $item.data(APPOINTMENT_SETTINGS_KEY);
            var {
                startDate: itemStartDate,
                endDate: itemEndDate
            } = itemInfo.sourceAppointment;
            var appointmentInstance = $item.dxSchedulerAppointment("instance");
            var isDragSource = startDate.getTime() === itemStartDate.getTime() && endDate.getTime() === itemEndDate.getTime() && groupIndex === itemGroupIndex;
            appointmentInstance.option("isDragSource", isDragSource)
        })
    }
    updateResizableArea() {
        var $allResizableElements = this.$element().find(".dx-scheduler-appointment.dx-resizable");
        var horizontalResizables = grep($allResizableElements, el => {
            var $el = $(el);
            var resizableInst = $el.dxResizable("instance");
            var area = resizableInst.option("area");
            return inArray(resizableInst.option("handles"), ["right left", "left right"]) > -1 && isPlainObject(area)
        });
        each(horizontalResizables, (_, el) => {
            var $el = $(el);
            var position = locate($el);
            var appointmentData = this._getItemData($el);
            var area = this._calculateResizableArea({
                left: position.left
            }, appointmentData);
            $el.dxResizable("instance").option("area", area)
        })
    }
}
registerComponent("dxSchedulerAppointments", SchedulerAppointments);
export default SchedulerAppointments;
