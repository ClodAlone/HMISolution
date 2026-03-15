/**
 * DevExtreme (cjs/ui/scheduler/appointments/appointmentCollection.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.default = void 0;
var _size = require("../../../core/utils/size");
var _renderer = _interopRequireDefault(require("../../../core/renderer"));
var _dom_adapter = _interopRequireDefault(require("../../../core/dom_adapter"));
var _events_engine = _interopRequireDefault(require("../../../events/core/events_engine"));
var _element_data = require("../../../core/element_data");
var _translator = require("../../../animation/translator");
var _date = _interopRequireDefault(require("../../../core/utils/date"));
var _common = require("../../../core/utils/common");
var _type = require("../../../core/utils/type");
var _iterator = require("../../../core/utils/iterator");
var _object = require("../../../core/utils/object");
var _array = require("../../../core/utils/array");
var _extend = require("../../../core/utils/extend");
var _element = require("../../../core/element");
var _recurrence = require("../recurrence");
var _component_registrator = _interopRequireDefault(require("../../../core/component_registrator"));
var _appointment = require("./appointment");
var _index = require("../../../events/utils/index");
var _double_click = require("../../../events/double_click");
var _uiCollection_widget = _interopRequireDefault(require("../../collection/ui.collection_widget.edit"));
var _utils = _interopRequireDefault(require("../utils.timeZone"));
var _constants = require("../constants");
var _classes = require("../classes");
var _appointmentLayout = require("./appointmentLayout");
var _expressionUtils = require("../expressionUtils");
var _appointmentAdapter = require("../appointmentAdapter");
var _utils2 = require("./dataProvider/utils");
var _position = require("../../../core/utils/position");
var _core = require("./resizing/core");

function _interopRequireDefault(obj) {
    return obj && obj.__esModule ? obj : {
        default: obj
    }
}

function _extends() {
    _extends = Object.assign || function(target) {
        for (var i = 1; i < arguments.length; i++) {
            var source = arguments[i];
            for (var key in source) {
                if (Object.prototype.hasOwnProperty.call(source, key)) {
                    target[key] = source[key]
                }
            }
        }
        return target
    };
    return _extends.apply(this, arguments)
}

function _defineProperties(target, props) {
    for (var i = 0; i < props.length; i++) {
        var descriptor = props[i];
        descriptor.enumerable = descriptor.enumerable || false;
        descriptor.configurable = true;
        if ("value" in descriptor) {
            descriptor.writable = true
        }
        Object.defineProperty(target, descriptor.key, descriptor)
    }
}

function _createClass(Constructor, protoProps, staticProps) {
    if (protoProps) {
        _defineProperties(Constructor.prototype, protoProps)
    }
    if (staticProps) {
        _defineProperties(Constructor, staticProps)
    }
    Object.defineProperty(Constructor, "prototype", {
        writable: false
    });
    return Constructor
}

function _inheritsLoose(subClass, superClass) {
    subClass.prototype = Object.create(superClass.prototype);
    subClass.prototype.constructor = subClass;
    _setPrototypeOf(subClass, superClass)
}

function _setPrototypeOf(o, p) {
    _setPrototypeOf = Object.setPrototypeOf || function(o, p) {
        o.__proto__ = p;
        return o
    };
    return _setPrototypeOf(o, p)
}
var COMPONENT_CLASS = "dx-scheduler-scrollable-appointments";
var DBLCLICK_EVENT_NAME = (0, _index.addNamespace)(_double_click.name, "dxSchedulerAppointment");
var toMs = _date.default.dateToMilliseconds;
var SchedulerAppointments = function(_CollectionWidget) {
    _inheritsLoose(SchedulerAppointments, _CollectionWidget);

    function SchedulerAppointments(element, options) {
        var _this;
        _this = _CollectionWidget.call(this, element, options) || this;
        _this._virtualAppointments = {};
        return _this
    }
    var _proto = SchedulerAppointments.prototype;
    _proto.notifyObserver = function(subject, args) {
        var observer = this.option("observer");
        if (observer) {
            observer.fire(subject, args)
        }
    };
    _proto.invoke = function() {
        var observer = this.option("observer");
        if (observer) {
            return observer.fire.apply(observer, arguments)
        }
    };
    _proto._dispose = function() {
        clearTimeout(this._appointmentClickTimeout);
        _CollectionWidget.prototype._dispose.call(this)
    };
    _proto._supportedKeys = function() {
        var parent = _CollectionWidget.prototype._supportedKeys.call(this);
        return (0, _extend.extend)(parent, {
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
                var index = focusedAppointment.data(_constants.APPOINTMENT_SETTINGS_KEY).sortedIndex;
                var lastIndex = appointments.length - 1;
                if (index > 0 && e.shiftKey || index < lastIndex && !e.shiftKey) {
                    e.preventDefault();
                    e.shiftKey ? index-- : index++;
                    var $nextAppointment = this._getAppointmentByIndex(index);
                    this._resetTabIndex($nextAppointment);
                    _events_engine.default.trigger($nextAppointment, "focus")
                }
            }
        })
    };
    _proto._getAppointmentByIndex = function(sortedIndex) {
        var appointments = this._getAccessAppointments();
        return appointments.filter((function(_, $item) {
            return (0, _element_data.data)($item, _constants.APPOINTMENT_SETTINGS_KEY).sortedIndex === sortedIndex
        })).eq(0)
    };
    _proto._getAccessAppointments = function() {
        return this._itemElements().filter(":visible").not(".dx-state-disabled")
    };
    _proto._resetTabIndex = function($appointment) {
        this._focusTarget().attr("tabIndex", -1);
        $appointment.attr("tabIndex", this.option("tabIndex"))
    };
    _proto._moveFocus = function() {};
    _proto._focusTarget = function() {
        return this._itemElements()
    };
    _proto._renderFocusTarget = function() {
        var $appointment = this._getAppointmentByIndex(0);
        this._resetTabIndex($appointment)
    };
    _proto._focusInHandler = function(e) {
        _CollectionWidget.prototype._focusInHandler.call(this, e);
        this._$currentAppointment = (0, _renderer.default)(e.target);
        this.option("focusedElement", (0, _element.getPublicElement)((0, _renderer.default)(e.target)))
    };
    _proto._focusOutHandler = function(e) {
        var $appointment = this._getAppointmentByIndex(0);
        this.option("focusedElement", (0, _element.getPublicElement)($appointment));
        _CollectionWidget.prototype._focusOutHandler.call(this, e)
    };
    _proto._eventBindingTarget = function() {
        return this._itemContainer()
    };
    _proto._getDefaultOptions = function() {
        return (0, _extend.extend)(_CollectionWidget.prototype._getDefaultOptions.call(this), {
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
    };
    _proto._optionChanged = function(args) {
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
                this._resetTabIndex((0, _renderer.default)(args.value));
                _CollectionWidget.prototype._optionChanged.call(this, args);
                break;
            case "allowDelete":
                break;
            case "focusStateEnabled":
                this._clearDropDownItemsElements();
                this._renderDropDownAppointments();
                _CollectionWidget.prototype._optionChanged.call(this, args);
                break;
            default:
                _CollectionWidget.prototype._optionChanged.call(this, args)
        }
    };
    _proto._isAllDayAppointment = function(appointment) {
        return appointment.settings.length && appointment.settings[0].allDay || false
    };
    _proto._isRepaintAppointment = function(appointment) {
        return !(0, _type.isDefined)(appointment.needRepaint) || true === appointment.needRepaint
    };
    _proto._isRepaintAll = function(appointments) {
        if (this.isAgendaView) {
            return true
        }
        for (var i = 0; i < appointments.length; i++) {
            if (!this._isRepaintAppointment(appointments[i])) {
                return false
            }
        }
        return true
    };
    _proto._applyFragment = function(fragment, allDay) {
        if (fragment.children().length > 0) {
            this._getAppointmentContainer(allDay).append(fragment)
        }
    };
    _proto._onEachAppointment = function(appointment, index, container, isRepaintAll) {
        var _this2 = this;
        if (true === (null === appointment || void 0 === appointment ? void 0 : appointment.needRemove)) {
            this._clearItem(appointment)
        } else if (isRepaintAll || this._isRepaintAppointment(appointment)) {
            ! function() {
                appointment.needRepaint = false;
                _this2._clearItem(appointment);
                _this2._renderItem(index, appointment, container)
            }()
        }
    };
    _proto._repaintAppointments = function(appointments) {
        var _this3 = this;
        this._renderByFragments((function($commonFragment, $allDayFragment) {
            var isRepaintAll = _this3._isRepaintAll(appointments);
            if (isRepaintAll) {
                _this3._getAppointmentContainer(true).html("");
                _this3._getAppointmentContainer(false).html("")
            }!appointments.length && _this3._cleanItemContainer();
            appointments.forEach((function(appointment, index) {
                var container = _this3._isAllDayAppointment(appointment) ? $allDayFragment : $commonFragment;
                _this3._onEachAppointment(appointment, index, container, isRepaintAll)
            }))
        }))
    };
    _proto._renderByFragments = function(renderFunction) {
        if (this.isVirtualScrolling) {
            var $commonFragment = (0, _renderer.default)(_dom_adapter.default.createDocumentFragment());
            var $allDayFragment = (0, _renderer.default)(_dom_adapter.default.createDocumentFragment());
            renderFunction($commonFragment, $allDayFragment);
            this._applyFragment($commonFragment, false);
            this._applyFragment($allDayFragment, true)
        } else {
            renderFunction(this._getAppointmentContainer(false), this._getAppointmentContainer(true))
        }
    };
    _proto._attachAppointmentsEvents = function() {
        this._attachClickEvent();
        this._attachHoldEvent();
        this._attachContextMenuEvent();
        this._attachAppointmentDblClick();
        this._renderFocusState();
        this._attachFeedbackEvents();
        this._attachHoverEvents()
    };
    _proto._clearItem = function(item) {
        var $items = this._findItemElementByItem(item.itemData);
        if (!$items.length) {
            return
        }(0, _iterator.each)($items, (function(_, $item) {
            $item.detach();
            $item.remove()
        }))
    };
    _proto._clearDropDownItems = function() {
        this._virtualAppointments = {}
    };
    _proto._clearDropDownItemsElements = function() {
        this.invoke("clearCompactAppointments")
    };
    _proto._findItemElementByItem = function(item) {
        var result = [];
        var that = this;
        this.itemElements().each((function() {
            var $item = (0, _renderer.default)(this);
            if ($item.data(that._itemDataKey()) === item) {
                result.push($item)
            }
        }));
        return result
    };
    _proto._itemClass = function() {
        return _classes.APPOINTMENT_ITEM_CLASS
    };
    _proto._itemContainer = function() {
        var $container = _CollectionWidget.prototype._itemContainer.call(this);
        var $result = $container;
        var $allDayContainer = this.option("allDayContainer");
        if ($allDayContainer) {
            $result = $container.add($allDayContainer)
        }
        return $result
    };
    _proto._cleanItemContainer = function() {
        _CollectionWidget.prototype._cleanItemContainer.call(this);
        var $allDayContainer = this.option("allDayContainer");
        if ($allDayContainer) {
            $allDayContainer.empty()
        }
        this._virtualAppointments = {}
    };
    _proto._clean = function() {
        _CollectionWidget.prototype._clean.call(this);
        delete this._$currentAppointment;
        delete this._initialSize;
        delete this._initialCoordinates
    };
    _proto._init = function() {
        _CollectionWidget.prototype._init.call(this);
        this.$element().addClass(COMPONENT_CLASS);
        this._preventSingleAppointmentClick = false
    };
    _proto._renderAppointmentTemplate = function($container, appointment, model) {
        var config = {
            isAllDay: appointment.allDay,
            isRecurrence: appointment.recurrenceRule,
            html: (0, _type.isPlainObject)(appointment) && appointment.html ? appointment.html : void 0
        };
        var formatText = this.invoke("getTextAndFormatDate", model.appointmentData, this._currentAppointmentSettings.agendaSettings || model.targetedAppointmentData, "TIME");
        $container.append(this.isAgendaView ? (0, _appointmentLayout.createAgendaAppointmentLayout)(formatText, config) : (0, _appointmentLayout.createAppointmentLayout)(formatText, config))
    };
    _proto._executeItemRenderAction = function(index, itemData, itemElement) {
        var action = this._getItemRenderAction();
        if (action) {
            action(this.invoke("mapAppointmentFields", {
                itemData: itemData,
                itemElement: itemElement
            }))
        }
        delete this._currentAppointmentSettings
    };
    _proto._itemClickHandler = function(e) {
        _CollectionWidget.prototype._itemClickHandler.call(this, e, {}, {
            afterExecute: function(e) {
                this._processItemClick(e.args[0].event)
            }.bind(this)
        })
    };
    _proto._processItemClick = function(e) {
        var $target = (0, _renderer.default)(e.currentTarget);
        var data = this._getItemData($target);
        if ("keydown" === e.type || (0, _index.isFakeClickEvent)(e)) {
            this.notifyObserver("showEditAppointmentPopup", {
                data: data,
                target: $target
            });
            return
        }
        this._appointmentClickTimeout = setTimeout(function() {
            if (!this._preventSingleAppointmentClick && _dom_adapter.default.getBody().contains($target[0])) {
                this.notifyObserver("showAppointmentTooltip", {
                    data: data,
                    target: $target
                })
            }
            this._preventSingleAppointmentClick = false
        }.bind(this), 300)
    };
    _proto._extendActionArgs = function($itemElement) {
        var args = _CollectionWidget.prototype._extendActionArgs.call(this, $itemElement);
        return this.invoke("mapAppointmentFields", args)
    };
    _proto._render = function() {
        _CollectionWidget.prototype._render.call(this);
        this._attachAppointmentDblClick()
    };
    _proto._attachAppointmentDblClick = function() {
        var that = this;
        var itemSelector = that._itemSelector();
        var itemContainer = this._itemContainer();
        _events_engine.default.off(itemContainer, DBLCLICK_EVENT_NAME, itemSelector);
        _events_engine.default.on(itemContainer, DBLCLICK_EVENT_NAME, itemSelector, (function(e) {
            that._itemDXEventHandler(e, "onAppointmentDblClick", {}, {
                afterExecute: function(e) {
                    that._dblClickHandler(e.args[0].event)
                }
            })
        }))
    };
    _proto._dblClickHandler = function(e) {
        var $targetAppointment = (0, _renderer.default)(e.currentTarget);
        var appointmentData = this._getItemData($targetAppointment);
        clearTimeout(this._appointmentClickTimeout);
        this._preventSingleAppointmentClick = true;
        this.notifyObserver("showEditAppointmentPopup", {
            data: appointmentData,
            target: $targetAppointment
        })
    };
    _proto._renderItem = function(index, item, container) {
        var itemData = item.itemData;
        var $items = [];
        for (var i = 0; i < item.settings.length; i++) {
            var setting = item.settings[i];
            this._currentAppointmentSettings = setting;
            var $item = _CollectionWidget.prototype._renderItem.call(this, index, itemData, container);
            $item.data(_constants.APPOINTMENT_SETTINGS_KEY, setting);
            $items.push($item)
        }
        return $items
    };
    _proto._getItemContent = function($itemFrame) {
        $itemFrame.data(_constants.APPOINTMENT_SETTINGS_KEY, this._currentAppointmentSettings);
        var $itemContent = _CollectionWidget.prototype._getItemContent.call(this, $itemFrame);
        return $itemContent
    };
    _proto._createItemByTemplate = function(itemTemplate, renderArgs) {
        var itemData = renderArgs.itemData,
            container = renderArgs.container,
            index = renderArgs.index;
        return itemTemplate.render({
            model: {
                appointmentData: itemData,
                targetedAppointmentData: this.invoke("getTargetedAppointmentData", itemData, (0, _renderer.default)(container).parent())
            },
            container: container,
            index: index
        })
    };
    _proto._getAppointmentContainer = function(allDay) {
        var $allDayContainer = this.option("allDayContainer");
        var $container = this.itemsContainer().not($allDayContainer);
        if (allDay && $allDayContainer) {
            $container = $allDayContainer
        }
        return $container
    };
    _proto._postprocessRenderItem = function(args) {
        this._renderAppointment(args.itemElement, this._currentAppointmentSettings)
    };
    _proto._renderAppointment = function(element, settings) {
        element.data(_constants.APPOINTMENT_SETTINGS_KEY, settings);
        this._applyResourceDataAttr(element);
        var rawAppointment = this._getItemData(element);
        var geometry = this.invoke("getAppointmentGeometry", settings);
        var allowResize = this.option("allowResize") && (!(0, _type.isDefined)(settings.skipResizing) || (0, _type.isString)(settings.skipResizing));
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
                config.createPlainResourceListAsync = function(rawAppointment) {
                    return agendaResourceProcessor.createListAsync(rawAppointment)
                }
            }
            this._createComponent(element, this.isAgendaView ? _appointment.AgendaAppointment : _appointment.Appointment, _extends({}, config, {
                dataAccessors: this.option("dataAccessors"),
                getResizableStep: this.option("getResizableStep")
            }))
        }
    };
    _proto._applyResourceDataAttr = function($appointment) {
        var dataAccessors = this.option("getResourceDataAccessors")();
        var rawAppointment = this._getItemData($appointment);
        (0, _iterator.each)(dataAccessors.getter, (function(key) {
            var value = dataAccessors.getter[key](rawAppointment);
            if ((0, _type.isDefined)(value)) {
                var prefix = "data-".concat((0, _common.normalizeKey)(key.toLowerCase()), "-");
                (0, _array.wrapToArray)(value).forEach((function(value) {
                    return $appointment.attr(prefix + (0, _common.normalizeKey)(value), true)
                }))
            }
        }))
    };
    _proto._resizableConfig = function(appointmentData, itemSetting) {
        return {
            area: this._calculateResizableArea(itemSetting, appointmentData),
            onResizeStart: function(e) {
                this._$currentAppointment = (0, _renderer.default)(e.element);
                if (this.invoke("needRecalculateResizableArea")) {
                    var updatedArea = this._calculateResizableArea(this._$currentAppointment.data(_constants.APPOINTMENT_SETTINGS_KEY), this._$currentAppointment.data("dxItemData"));
                    e.component.option("area", updatedArea);
                    e.component._renderDragOffsets(e.event)
                }
                this._initialSize = {
                    width: e.width,
                    height: e.height
                };
                this._initialCoordinates = (0, _translator.locate)(this._$currentAppointment)
            }.bind(this),
            onResizeEnd: function(e) {
                if (this._escPressed) {
                    e.event.cancel = true;
                    return
                }
                this._resizeEndHandler(e)
            }.bind(this)
        }
    };
    _proto._calculateResizableArea = function(itemSetting, appointmentData) {
        var area = this.$element().closest(".dx-scrollable-content");
        return this.invoke("getResizableAppointmentArea", {
            coordinates: {
                left: itemSetting.left,
                top: 0,
                groupIndex: itemSetting.groupIndex
            },
            allDay: itemSetting.allDay
        }) || area
    };
    _proto._resizeEndHandler = function(e) {
        var $element = (0, _renderer.default)(e.element);
        var _$element$data = $element.data("dxAppointmentSettings"),
            allDay = _$element$data.allDay,
            info = _$element$data.info;
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
    };
    _proto.resizeAllDay = function(e) {
        var $element = (0, _renderer.default)(e.element);
        var timeZoneCalculator = this.option("timeZoneCalculator");
        var dataAccessors = this.option("dataAccessors");
        return (0, _core.getAppointmentDateRange)({
            handles: e.handles,
            appointmentSettings: $element.data("dxAppointmentSettings"),
            isVerticalViewDirection: this.option("isVerticalViewDirection")(),
            isVerticalGroupedWorkSpace: this.option("isVerticalGroupedWorkSpace")(),
            appointmentRect: (0, _position.getBoundingRect)($element[0]),
            parentAppointmentRect: (0, _position.getBoundingRect)($element.parent()[0]),
            viewDataProvider: this.option("getViewDataProvider")(),
            isDateAndTimeView: this.option("isDateAndTimeView")(),
            startDayHour: this.invoke("getStartDayHour"),
            endDayHour: this.invoke("getEndDayHour"),
            timeZoneCalculator: timeZoneCalculator,
            dataAccessors: dataAccessors,
            rtlEnabled: this.option("rtlEnabled"),
            DOMMetaData: this.option("getDOMElementsMetaData")()
        })
    };
    _proto.updateResizedAppointment = function($element, dateRange, dataAccessors, timeZoneCalculator) {
        var sourceAppointment = this._getItemData($element);
        var modifiedAppointmentAdapter = (0, _appointmentAdapter.createAppointmentAdapter)(sourceAppointment, dataAccessors, timeZoneCalculator).clone();
        modifiedAppointmentAdapter.startDate = new Date(dateRange.startDate);
        modifiedAppointmentAdapter.endDate = new Date(dateRange.endDate);
        this.notifyObserver("updateAppointmentAfterResize", {
            target: sourceAppointment,
            data: modifiedAppointmentAdapter.clone({
                pathTimeZone: "fromGrid"
            }).source(),
            $appointment: $element
        })
    };
    _proto._getEndResizeAppointmentStartDate = function(e, rawAppointment, appointmentInfo) {
        var timeZoneCalculator = this.option("timeZoneCalculator");
        var appointmentAdapter = (0, _appointmentAdapter.createAppointmentAdapter)(rawAppointment, this.option("dataAccessors"), timeZoneCalculator);
        var startDate = appointmentInfo.startDate;
        var recurrenceProcessor = (0, _recurrence.getRecurrenceProcessor)();
        var recurrenceRule = appointmentAdapter.recurrenceRule,
            startDateTimeZone = appointmentAdapter.startDateTimeZone;
        var isAllDay = this.invoke("isAllDay", rawAppointment);
        var isRecurrent = recurrenceProcessor.isValidRecurrenceRule(recurrenceRule);
        if (!e.handles.top && !isRecurrent && !isAllDay) {
            startDate = timeZoneCalculator.createDate(appointmentAdapter.startDate, {
                appointmentTimeZone: startDateTimeZone,
                path: "toGrid"
            })
        }
        return startDate
    };
    _proto._getDateRange = function(e, startDate, endDate) {
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
            startTime += _utils.default.getTimezoneOffsetChangeInMs(startDate, endDate, startTime, endDate);
            endTime = endDate.getTime()
        } else {
            startTime = startDate.getTime();
            endTime = needCorrectDates ? this._correctEndDateByDelta(endDate, deltaTime) : endDate.getTime() + deltaTime;
            endTime -= _utils.default.getTimezoneOffsetChangeInMs(startDate, endDate, startDate, endTime)
        }
        return {
            startDate: new Date(startTime),
            endDate: new Date(endTime)
        }
    };
    _proto._correctEndDateByDelta = function(endDate, deltaTime) {
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
    };
    _proto._correctStartDateByDelta = function(startDate, deltaTime) {
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
    };
    _proto._processVirtualAppointment = function(appointmentSetting, $appointment, appointmentData, color) {
        var virtualAppointment = appointmentSetting.virtual;
        var virtualGroupIndex = virtualAppointment.index;
        if (!(0, _type.isDefined)(this._virtualAppointments[virtualGroupIndex])) {
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
    };
    _proto._renderContentImpl = function() {
        _CollectionWidget.prototype._renderContentImpl.call(this);
        this._renderDropDownAppointments()
    };
    _proto._renderDropDownAppointments = function() {
        var _this4 = this;
        this._renderByFragments((function($commonFragment, $allDayFragment) {
            (0, _iterator.each)(_this4._virtualAppointments, function(groupIndex) {
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
            }.bind(_this4))
        }))
    };
    _proto._isGroupCompact = function(virtualGroup) {
        return !virtualGroup.isAllDay && this.invoke("supportCompactDropDownAppointments")
    };
    _proto._sortAppointmentsByStartDate = function(appointments) {
        return (0, _utils2.sortAppointmentsByStartDate)(appointments, this.option("dataAccessors"))
    };
    _proto._processRecurrenceAppointment = function(appointment, index, skipLongAppointments) {
        var recurrenceRule = _expressionUtils.ExpressionUtils.getField(this.option("dataAccessors"), "recurrenceRule", appointment);
        var result = {
            parts: [],
            indexes: []
        };
        if (recurrenceRule) {
            var dates = appointment.settings || appointment;
            var startDate = new Date(_expressionUtils.ExpressionUtils.getField(this.option("dataAccessors"), "startDate", dates));
            var endDate = new Date(_expressionUtils.ExpressionUtils.getField(this.option("dataAccessors"), "endDate", dates));
            var appointmentDuration = endDate.getTime() - startDate.getTime();
            var recurrenceException = _expressionUtils.ExpressionUtils.getField(this.option("dataAccessors"), "recurrenceException", appointment);
            var startViewDate = this.invoke("getStartViewDate");
            var endViewDate = this.invoke("getEndViewDate");
            var recurrentDates = (0, _recurrence.getRecurrenceProcessor)().generateDates({
                rule: recurrenceRule,
                exception: recurrenceException,
                start: startDate,
                end: endDate,
                min: startViewDate,
                max: endViewDate
            });
            var recurrentDateCount = appointment.settings ? 1 : recurrentDates.length;
            for (var i = 0; i < recurrentDateCount; i++) {
                var appointmentPart = (0, _extend.extend)({}, appointment, true);
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
    };
    _proto._processLongAppointment = function(appointment, result) {
        var parts = this.splitAppointmentByDay(appointment);
        var partCount = parts.length;
        var endViewDate = this.invoke("getEndViewDate").getTime();
        var startViewDate = this.invoke("getStartViewDate").getTime();
        var timeZoneCalculator = this.option("timeZoneCalculator");
        result = result || {
            parts: []
        };
        if (partCount > 1) {
            (0, _extend.extend)(appointment, parts[0]);
            for (var i = 1; i < partCount; i++) {
                var startDate = _expressionUtils.ExpressionUtils.getField(this.option("dataAccessors"), "startDate", parts[i].settings).getTime();
                startDate = timeZoneCalculator.createDate(startDate, {
                    path: "toGrid"
                });
                if (startDate < endViewDate && startDate > startViewDate) {
                    result.parts.push(parts[i])
                }
            }
        }
        return result
    };
    _proto._reduceRecurrenceAppointments = function(recurrenceIndexes, appointments) {
        (0, _iterator.each)(recurrenceIndexes, (function(i, index) {
            appointments.splice(index - i, 1)
        }))
    };
    _proto._combineAppointments = function(appointments, additionalAppointments) {
        if (additionalAppointments.length) {
            (0, _array.merge)(appointments, additionalAppointments)
        }
        this._sortAppointmentsByStartDate(appointments)
    };
    _proto._applyStartDateToObj = function(startDate, obj) {
        _expressionUtils.ExpressionUtils.setField(this.option("dataAccessors"), "startDate", obj, startDate);
        return obj
    };
    _proto._applyEndDateToObj = function(endDate, obj) {
        _expressionUtils.ExpressionUtils.setField(this.option("dataAccessors"), "endDate", obj, endDate);
        return obj
    };
    _proto.moveAppointmentBack = function(dragEvent) {
        var $appointment = this._$currentAppointment;
        var size = this._initialSize;
        var coords = this._initialCoordinates;
        if (dragEvent) {
            this._removeDragSourceClassFromDraggedAppointment();
            if ((0, _type.isDeferred)(dragEvent.cancel)) {
                dragEvent.cancel.resolve(true)
            } else {
                dragEvent.cancel = true
            }
        }
        if ($appointment && !dragEvent) {
            if (coords) {
                (0, _translator.move)($appointment, coords);
                delete this._initialSize
            }
            if (size) {
                (0, _size.setOuterWidth)($appointment, size.width);
                (0, _size.setOuterHeight)($appointment, size.height);
                delete this._initialCoordinates
            }
        }
    };
    _proto.focus = function() {
        if (this._$currentAppointment) {
            var focusedElement = (0, _element.getPublicElement)(this._$currentAppointment);
            this.option("focusedElement", focusedElement);
            _events_engine.default.trigger(focusedElement, "focus")
        }
    };
    _proto.splitAppointmentByDay = function(appointment) {
        var dates = appointment.settings || appointment;
        var dataAccessors = this.option("dataAccessors");
        var originalStartDate = new Date(_expressionUtils.ExpressionUtils.getField(dataAccessors, "startDate", dates));
        var startDate = _date.default.makeDate(originalStartDate);
        var endDate = _date.default.makeDate(_expressionUtils.ExpressionUtils.getField(dataAccessors, "endDate", dates));
        var maxAllowedDate = this.invoke("getEndViewDate");
        var startDayHour = this.invoke("getStartDayHour");
        var endDayHour = this.invoke("getEndDayHour");
        var timeZoneCalculator = this.option("timeZoneCalculator");
        var adapter = (0, _appointmentAdapter.createAppointmentAdapter)(appointment, dataAccessors, timeZoneCalculator);
        var appointmentIsLong = (0, _utils2.getAppointmentTakesSeveralDays)(adapter);
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
            var appointmentData = (0, _object.deepExtendArraySafe)({}, appointment, true);
            var appointmentSettings = {};
            this._applyStartDateToObj(currentStartDate, appointmentSettings);
            this._applyEndDateToObj(currentEndDate, appointmentSettings);
            appointmentData.settings = appointmentSettings;
            result.push(appointmentData);
            startDate = _date.default.trimTime(startDate);
            startDate.setDate(startDate.getDate() + 1);
            startDate.setHours(startDayHour)
        }
        return result
    };
    _proto._checkStartDate = function(currentDate, originalDate, startDayHour) {
        if (!_date.default.sameDate(currentDate, originalDate) || currentDate.getHours() <= startDayHour) {
            currentDate.setHours(startDayHour, 0, 0, 0)
        } else {
            currentDate.setHours(originalDate.getHours(), originalDate.getMinutes(), originalDate.getSeconds(), originalDate.getMilliseconds())
        }
    };
    _proto._checkEndDate = function(currentDate, originalDate, endDayHour) {
        if (!_date.default.sameDate(currentDate, originalDate) || currentDate.getHours() > endDayHour) {
            currentDate.setHours(endDayHour, 0, 0, 0)
        } else {
            currentDate.setHours(originalDate.getHours(), originalDate.getMinutes(), originalDate.getSeconds(), originalDate.getMilliseconds())
        }
    };
    _proto._removeDragSourceClassFromDraggedAppointment = function() {
        var $appointments = this._itemElements().filter(".".concat(_classes.APPOINTMENT_DRAG_SOURCE_CLASS));
        $appointments.each((function(_, element) {
            var appointmentInstance = (0, _renderer.default)(element).dxSchedulerAppointment("instance");
            appointmentInstance.option("isDragSource", false)
        }))
    };
    _proto._setDragSourceAppointment = function(appointment, settings) {
        var $appointments = this._findItemElementByItem(appointment);
        var _settings$info$source = settings.info.sourceAppointment,
            startDate = _settings$info$source.startDate,
            endDate = _settings$info$source.endDate;
        var groupIndex = settings.groupIndex;
        $appointments.forEach((function($item) {
            var _$item$data = $item.data(_constants.APPOINTMENT_SETTINGS_KEY),
                itemInfo = _$item$data.info,
                itemGroupIndex = _$item$data.groupIndex;
            var _itemInfo$sourceAppoi = itemInfo.sourceAppointment,
                itemStartDate = _itemInfo$sourceAppoi.startDate,
                itemEndDate = _itemInfo$sourceAppoi.endDate;
            var appointmentInstance = $item.dxSchedulerAppointment("instance");
            var isDragSource = startDate.getTime() === itemStartDate.getTime() && endDate.getTime() === itemEndDate.getTime() && groupIndex === itemGroupIndex;
            appointmentInstance.option("isDragSource", isDragSource)
        }))
    };
    _proto.updateResizableArea = function() {
        var _this5 = this;
        var $allResizableElements = this.$element().find(".dx-scheduler-appointment.dx-resizable");
        var horizontalResizables = (0, _common.grep)($allResizableElements, (function(el) {
            var $el = (0, _renderer.default)(el);
            var resizableInst = $el.dxResizable("instance");
            var area = resizableInst.option("area");
            return (0, _array.inArray)(resizableInst.option("handles"), ["right left", "left right"]) > -1 && (0, _type.isPlainObject)(area)
        }));
        (0, _iterator.each)(horizontalResizables, (function(_, el) {
            var $el = (0, _renderer.default)(el);
            var position = (0, _translator.locate)($el);
            var appointmentData = _this5._getItemData($el);
            var area = _this5._calculateResizableArea({
                left: position.left
            }, appointmentData);
            $el.dxResizable("instance").option("area", area)
        }))
    };
    _createClass(SchedulerAppointments, [{
        key: "isAgendaView",
        get: function() {
            return this.invoke("isCurrentViewAgenda")
        }
    }, {
        key: "isVirtualScrolling",
        get: function() {
            return this.invoke("isVirtualScrolling")
        }
    }, {
        key: "appointmentDataProvider",
        get: function() {
            return this.option("getAppointmentDataProvider")()
        }
    }]);
    return SchedulerAppointments
}(_uiCollection_widget.default);
(0, _component_registrator.default)("dxSchedulerAppointments", SchedulerAppointments);
var _default = SchedulerAppointments;
exports.default = _default;
module.exports = exports.default;
module.exports.default = exports.default;
