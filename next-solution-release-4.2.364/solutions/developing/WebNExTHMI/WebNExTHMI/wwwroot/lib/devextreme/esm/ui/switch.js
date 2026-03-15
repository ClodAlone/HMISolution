/**
 * DevExtreme (esm/ui/switch.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import {
    getOuterWidth
} from "../core/utils/size";
import $ from "../core/renderer";
import eventsEngine from "../events/core/events_engine";
import devices from "../core/devices";
import {
    extend
} from "../core/utils/extend";
import registerComponent from "../core/component_registrator";
import Editor from "./editor/editor";
import {
    addNamespace
} from "../events/utils/index";
import {
    lock
} from "../events/core/emitter.feedback";
import {
    getBoundingRect
} from "../core/utils/position";
import fx from "../animation/fx";
import messageLocalization from "../localization/message";
import {
    name as clickEventName
} from "../events/click";
import Swipeable from "../events/gesture/swipeable";
import {
    Deferred
} from "../core/utils/deferred";
var SWITCH_CLASS = "dx-switch";
var SWITCH_WRAPPER_CLASS = SWITCH_CLASS + "-wrapper";
var SWITCH_CONTAINER_CLASS = SWITCH_CLASS + "-container";
var SWITCH_INNER_CLASS = SWITCH_CLASS + "-inner";
var SWITCH_HANDLE_CLASS = SWITCH_CLASS + "-handle";
var SWITCH_ON_VALUE_CLASS = SWITCH_CLASS + "-on-value";
var SWITCH_ON_CLASS = SWITCH_CLASS + "-on";
var SWITCH_OFF_CLASS = SWITCH_CLASS + "-off";
var SWITCH_ANIMATION_DURATION = 100;
var Switch = Editor.inherit({
    _supportedKeys: function() {
        var isRTL = this.option("rtlEnabled");
        var click = function(e) {
            e.preventDefault();
            this._clickAction({
                event: e
            })
        };
        var move = function(value, e) {
            e.preventDefault();
            e.stopPropagation();
            this._saveValueChangeEvent(e);
            this._animateValue(value)
        };
        return extend(this.callBase(), {
            space: click,
            enter: click,
            leftArrow: move.bind(this, isRTL ? true : false),
            rightArrow: move.bind(this, isRTL ? false : true)
        })
    },
    _useTemplates: function() {
        return false
    },
    _getDefaultOptions: function() {
        return extend(this.callBase(), {
            hoverStateEnabled: true,
            activeStateEnabled: true,
            switchedOnText: messageLocalization.format("dxSwitch-switchedOnText"),
            switchedOffText: messageLocalization.format("dxSwitch-switchedOffText"),
            value: false
        })
    },
    _defaultOptionsRules: function() {
        return this.callBase().concat([{
            device: function() {
                return "desktop" === devices.real().deviceType && !devices.isSimulator()
            },
            options: {
                focusStateEnabled: true
            }
        }])
    },
    _feedbackHideTimeout: 0,
    _animating: false,
    _initMarkup: function() {
        this._renderContainers();
        this.$element().addClass(SWITCH_CLASS).append(this._$switchWrapper);
        this._renderSubmitElement();
        this._renderClick();
        this.setAria("role", "button");
        this._renderSwipeable();
        this.callBase();
        this._renderSwitchInner();
        this._renderLabels();
        this._renderValue()
    },
    _getInnerOffset: function(value, offset) {
        var ratio = (offset - this._offsetDirection() * Number(!value)) / 2;
        return 100 * ratio + "%"
    },
    _getHandleOffset: function(value, offset) {
        if (this.option("rtlEnabled")) {
            value = !value
        }
        if (value) {
            var calcValue = 100 * -offset - 100;
            return calcValue + "%"
        } else {
            return 100 * -offset + "%"
        }
    },
    _renderSwitchInner: function() {
        this._$switchInner = $("<div>").addClass(SWITCH_INNER_CLASS).appendTo(this._$switchContainer);
        this._$handle = $("<div>").addClass(SWITCH_HANDLE_CLASS).appendTo(this._$switchInner)
    },
    _renderLabels: function() {
        this._$labelOn = $("<div>").addClass(SWITCH_ON_CLASS).prependTo(this._$switchInner);
        this._$labelOff = $("<div>").addClass(SWITCH_OFF_CLASS).appendTo(this._$switchInner);
        this._setLabelsText()
    },
    _renderContainers: function() {
        this._$switchContainer = $("<div>").addClass(SWITCH_CONTAINER_CLASS);
        this._$switchWrapper = $("<div>").addClass(SWITCH_WRAPPER_CLASS).append(this._$switchContainer)
    },
    _renderSwipeable: function() {
        this._createComponent(this.$element(), Swipeable, {
            elastic: false,
            immediate: true,
            onStart: this._swipeStartHandler.bind(this),
            onUpdated: this._swipeUpdateHandler.bind(this),
            onEnd: this._swipeEndHandler.bind(this),
            itemSizeFunc: this._getItemSizeFunc.bind(this)
        })
    },
    _getItemSizeFunc: function() {
        return getOuterWidth(this._$switchContainer, true) - getBoundingRect(this._$handle.get(0)).width
    },
    _renderSubmitElement: function() {
        this._$submitElement = $("<input>").attr("type", "hidden").appendTo(this.$element())
    },
    _getSubmitElement: function() {
        return this._$submitElement
    },
    _offsetDirection: function() {
        return this.option("rtlEnabled") ? -1 : 1
    },
    _renderPosition: function(state, swipeOffset) {
        var innerOffset = this._getInnerOffset(state, swipeOffset);
        var handleOffset = this._getHandleOffset(state, swipeOffset);
        this._$switchInner.css("transform", " translateX(" + innerOffset + ")");
        this._$handle.css("transform", " translateX(" + handleOffset + ")")
    },
    _validateValue: function() {
        var check = this.option("value");
        if ("boolean" !== typeof check) {
            this._options.silent("value", !!check)
        }
    },
    _renderClick: function() {
        var eventName = addNamespace(clickEventName, this.NAME);
        var $element = this.$element();
        this._clickAction = this._createAction(this._clickHandler.bind(this));
        eventsEngine.off($element, eventName);
        eventsEngine.on($element, eventName, function(e) {
            this._clickAction({
                event: e
            })
        }.bind(this))
    },
    _clickHandler: function(args) {
        var e = args.event;
        this._saveValueChangeEvent(e);
        if (this._animating || this._swiping) {
            return
        }
        this._animateValue(!this.option("value"))
    },
    _animateValue: function(value) {
        var startValue = this.option("value");
        var endValue = value;
        if (startValue === endValue) {
            return
        }
        this._animating = true;
        var fromInnerOffset = this._getInnerOffset(startValue, 0);
        var toInnerOffset = this._getInnerOffset(endValue, 0);
        var fromHandleOffset = this._getHandleOffset(startValue, 0);
        var toHandleOffset = this._getHandleOffset(endValue, 0);
        var that = this;
        var fromInnerConfig = {};
        var toInnerConfig = {};
        var fromHandleConfig = {};
        var toHandlerConfig = {};
        fromInnerConfig.transform = " translateX(" + fromInnerOffset + ")";
        toInnerConfig.transform = " translateX(" + toInnerOffset + ")";
        fromHandleConfig.transform = " translateX(" + fromHandleOffset + ")";
        toHandlerConfig.transform = " translateX(" + toHandleOffset + ")";
        this.$element().toggleClass(SWITCH_ON_VALUE_CLASS, endValue);
        fx.animate(this._$handle, {
            from: fromHandleConfig,
            to: toHandlerConfig,
            duration: SWITCH_ANIMATION_DURATION
        });
        fx.animate(this._$switchInner, {
            from: fromInnerConfig,
            to: toInnerConfig,
            duration: SWITCH_ANIMATION_DURATION,
            complete: function() {
                that._animating = false;
                that.option("value", endValue)
            }
        })
    },
    _swipeStartHandler: function(e) {
        var state = this.option("value");
        var rtlEnabled = this.option("rtlEnabled");
        var maxOffOffset = rtlEnabled ? 0 : 1;
        var maxOnOffset = rtlEnabled ? 1 : 0;
        e.event.maxLeftOffset = state ? maxOffOffset : maxOnOffset;
        e.event.maxRightOffset = state ? maxOnOffset : maxOffOffset;
        this._swiping = true;
        this._feedbackDeferred = new Deferred;
        lock(this._feedbackDeferred);
        this._toggleActiveState(this.$element(), this.option("activeStateEnabled"))
    },
    _swipeUpdateHandler: function(e) {
        this._renderPosition(this.option("value"), e.event.offset)
    },
    _swipeEndHandler: function(e) {
        var that = this;
        var offsetDirection = this._offsetDirection();
        var toInnerConfig = {};
        var toHandleConfig = {};
        var innerOffset = this._getInnerOffset(that.option("value"), e.event.targetOffset);
        var handleOffset = this._getHandleOffset(that.option("value"), e.event.targetOffset);
        toInnerConfig.transform = " translateX(" + innerOffset + ")";
        toHandleConfig.transform = " translateX(" + handleOffset + ")";
        fx.animate(this._$handle, {
            to: toHandleConfig,
            duration: SWITCH_ANIMATION_DURATION
        });
        fx.animate(this._$switchInner, {
            to: toInnerConfig,
            duration: SWITCH_ANIMATION_DURATION,
            complete: function() {
                that._swiping = false;
                var pos = that.option("value") + offsetDirection * e.event.targetOffset;
                that._saveValueChangeEvent(e.event);
                that.option("value", Boolean(pos));
                that._feedbackDeferred.resolve();
                that._toggleActiveState(that.$element(), false)
            }
        })
    },
    _renderValue: function() {
        this._validateValue();
        var val = this.option("value");
        this._renderPosition(val, 0);
        this.$element().toggleClass(SWITCH_ON_VALUE_CLASS, val);
        this._getSubmitElement().val(val);
        this.setAria({
            pressed: val,
            label: val ? this.option("switchedOnText") : this.option("switchedOffText")
        })
    },
    _setLabelsText: function() {
        this._$labelOn && this._$labelOn.text(this.option("switchedOnText"));
        this._$labelOff && this._$labelOff.text(this.option("switchedOffText"))
    },
    _visibilityChanged: function(visible) {
        if (visible) {
            this.repaint()
        }
    },
    _optionChanged: function(args) {
        switch (args.name) {
            case "width":
                delete this._marginBound;
                this._refresh();
                break;
            case "switchedOnText":
            case "switchedOffText":
                this._setLabelsText();
                break;
            case "value":
                this._renderValue();
                this.callBase(args);
                break;
            default:
                this.callBase(args)
        }
    }
});
registerComponent("dxSwitch", Switch);
export default Switch;
