/**
 * DevExtreme (esm/ui/slider/ui.slider.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import {
    getWidth,
    setWidth
} from "../../core/utils/size";
import registerComponent from "../../core/component_registrator";
import devices from "../../core/devices";
import $ from "../../core/renderer";
import {
    applyServerDecimalSeparator
} from "../../core/utils/common";
import {
    Deferred
} from "../../core/utils/deferred";
import {
    extend
} from "../../core/utils/extend";
import {
    name as clickName
} from "../../events/click";
import {
    lock
} from "../../events/core/emitter.feedback";
import eventsEngine from "../../events/core/events_engine";
import Swipeable from "../../events/gesture/swipeable";
import pointerEvents from "../../events/pointer";
import {
    addNamespace,
    isMouseEvent,
    isTouchEvent,
    eventData
} from "../../events/utils/index";
import numberLocalization from "../../localization/number";
import {
    isMaterial,
    current as currentTheme
} from "../themes";
import TrackBar from "../track_bar";
import {
    render
} from "../widget/utils.ink_ripple";
import SliderHandle from "./ui.slider_handle";
import {
    roundFloatPart,
    getExponentLength,
    getRemainderByDivision
} from "../../core/utils/math";
var SLIDER_CLASS = "dx-slider";
var SLIDER_WRAPPER_CLASS = "dx-slider-wrapper";
var SLIDER_HANDLE_SELECTOR = ".dx-slider-handle";
var SLIDER_BAR_CLASS = "dx-slider-bar";
var SLIDER_RANGE_CLASS = "dx-slider-range";
var SLIDER_RANGE_VISIBLE_CLASS = "dx-slider-range-visible";
var SLIDER_LABEL_CLASS = "dx-slider-label";
var SLIDER_LABEL_POSITION_CLASS_PREFIX = "dx-slider-label-position-";
var SLIDER_TOOLTIP_POSITION_CLASS_PREFIX = "dx-slider-tooltip-position-";
var INVALID_MESSAGE_VISIBLE_CLASS = "dx-invalid-message-visible";
var SLIDER_VALIDATION_NAMESPACE = "Validation";
var Slider = TrackBar.inherit({
    _activeStateUnit: SLIDER_HANDLE_SELECTOR,
    _supportedKeys: function() {
        var isRTL = this.option("rtlEnabled");
        var roundedValue = (offset, isLeftDirection) => {
            offset = this._valueStep(offset);
            var step = this.option("step");
            var value = this.option("value");
            var currentPosition = value - this.option("min");
            var remainder = getRemainderByDivision(currentPosition, step, this._getValueExponentLength());
            var result = isLeftDirection ? value - offset + (remainder ? step - remainder : 0) : value + offset - remainder;
            var min = this.option("min");
            var max = this.option("max");
            if (result < min) {
                result = min
            } else if (result > max) {
                result = max
            }
            return this._roundToExponentLength(result)
        };
        var moveHandleRight = offset => {
            this.option("value", roundedValue(offset, isRTL))
        };
        var moveHandleLeft = offset => {
            this.option("value", roundedValue(offset, !isRTL))
        };
        return extend(this.callBase(), {
            leftArrow: function(e) {
                this._processKeyboardEvent(e);
                moveHandleLeft(this.option("step"))
            },
            rightArrow: function(e) {
                this._processKeyboardEvent(e);
                moveHandleRight(this.option("step"))
            },
            pageUp: function(e) {
                this._processKeyboardEvent(e);
                moveHandleRight(this.option("step") * this.option("keyStep"))
            },
            pageDown: function(e) {
                this._processKeyboardEvent(e);
                moveHandleLeft(this.option("step") * this.option("keyStep"))
            },
            home: function(e) {
                this._processKeyboardEvent(e);
                var min = this.option("min");
                this.option("value", min)
            },
            end: function(e) {
                this._processKeyboardEvent(e);
                var max = this.option("max");
                this.option("value", max)
            }
        })
    },
    _processKeyboardEvent: function(e) {
        e.preventDefault();
        e.stopPropagation();
        this._saveValueChangeEvent(e)
    },
    _getDefaultOptions: function() {
        return extend(this.callBase(), {
            value: 50,
            hoverStateEnabled: true,
            activeStateEnabled: true,
            step: 1,
            showRange: true,
            tooltip: {
                enabled: false,
                format: function(value) {
                    return value
                },
                position: "top",
                showMode: "onHover"
            },
            label: {
                visible: false,
                position: "bottom",
                format: function(value) {
                    return value
                }
            },
            keyStep: 1,
            useInkRipple: false,
            validationMessageOffset: isMaterial() ? {
                h: 18,
                v: 0
            } : {
                h: 7,
                v: 4
            },
            focusStateEnabled: true
        })
    },
    _toggleValidationMessage: function(visible) {
        if (!this.option("isValid")) {
            this.$element().toggleClass(INVALID_MESSAGE_VISIBLE_CLASS, visible)
        }
    },
    _defaultOptionsRules: function() {
        return this.callBase().concat([{
            device: function() {
                return "desktop" === devices.real().deviceType && !devices.isSimulator()
            },
            options: {
                focusStateEnabled: true
            }
        }, {
            device: function() {
                var themeName = currentTheme();
                return isMaterial(themeName)
            },
            options: {
                useInkRipple: true
            }
        }])
    },
    _initMarkup: function() {
        this.$element().addClass(SLIDER_CLASS);
        this._renderSubmitElement();
        this.option("useInkRipple") && this._renderInkRipple();
        this.callBase();
        this._renderLabels();
        this._renderStartHandler();
        this._renderAriaMinAndMax()
    },
    _attachFocusEvents: function() {
        this.callBase();
        var namespace = this.NAME + SLIDER_VALIDATION_NAMESPACE;
        var focusInEvent = addNamespace("focusin", namespace);
        var focusOutEvent = addNamespace("focusout", namespace);
        var $focusTarget = this._focusTarget();
        eventsEngine.on($focusTarget, focusInEvent, this._toggleValidationMessage.bind(this, true));
        eventsEngine.on($focusTarget, focusOutEvent, this._toggleValidationMessage.bind(this, false))
    },
    _detachFocusEvents: function() {
        this.callBase();
        var $focusTarget = this._focusTarget();
        this._toggleValidationMessage(false);
        eventsEngine.off($focusTarget, this.NAME + SLIDER_VALIDATION_NAMESPACE)
    },
    _render: function() {
        this.callBase();
        this._repaintHandle()
    },
    _renderSubmitElement: function() {
        this._$submitElement = $("<input>").attr("type", "hidden").appendTo(this.$element())
    },
    _getSubmitElement: function() {
        return this._$submitElement
    },
    _renderInkRipple: function() {
        this._inkRipple = render({
            waveSizeCoefficient: .7,
            isCentered: true,
            wavesNumber: 2,
            useHoldAnimation: false
        })
    },
    _renderInkWave: function(element, dxEvent, doRender, waveIndex) {
        if (!this._inkRipple) {
            return
        }
        var config = {
            element: element,
            event: dxEvent,
            wave: waveIndex
        };
        if (doRender) {
            this._inkRipple.showWave(config)
        } else {
            this._inkRipple.hideWave(config)
        }
    },
    _visibilityChanged: function() {
        this.repaint()
    },
    _renderWrapper: function() {
        this.callBase();
        this._$wrapper.addClass(SLIDER_WRAPPER_CLASS);
        this._createComponent(this._$wrapper, Swipeable, {
            elastic: false,
            immediate: true,
            onStart: this._swipeStartHandler.bind(this),
            onUpdated: this._swipeUpdateHandler.bind(this),
            onEnd: this._swipeEndHandler.bind(this),
            itemSizeFunc: this._itemWidthFunc.bind(this)
        })
    },
    _renderContainer: function() {
        this.callBase();
        this._$bar.addClass(SLIDER_BAR_CLASS)
    },
    _renderRange: function() {
        this.callBase();
        this._$range.addClass(SLIDER_RANGE_CLASS);
        this._renderHandle();
        this._renderRangeVisibility()
    },
    _renderRangeVisibility: function() {
        this._$range.toggleClass(SLIDER_RANGE_VISIBLE_CLASS, Boolean(this.option("showRange")))
    },
    _renderHandle: function() {
        this._$handle = this._renderHandleImpl(this.option("value"), this._$handle)
    },
    _renderHandleImpl: function(value, $element) {
        var $handle = $element || $("<div>").appendTo(this._$range);
        var tooltip = this.option("tooltip");
        this.$element().toggleClass(SLIDER_TOOLTIP_POSITION_CLASS_PREFIX + "bottom", tooltip.enabled && "bottom" === tooltip.position).toggleClass(SLIDER_TOOLTIP_POSITION_CLASS_PREFIX + "top", tooltip.enabled && "top" === tooltip.position);
        this._createComponent($handle, SliderHandle, {
            value: value,
            tooltip: tooltip
        });
        return $handle
    },
    _renderAriaMinAndMax: function() {
        this.setAria({
            valuemin: this.option("min"),
            valuemax: this.option("max")
        }, this._$handle)
    },
    _toggleActiveState: function($element, value) {
        this.callBase($element, value);
        this._renderInkWave($element, null, !!value, 1)
    },
    _toggleFocusClass: function(isFocused, $element) {
        this.callBase(isFocused, $element);
        if (this._disposed) {
            return
        }
        var $focusTarget = $($element || this._focusTarget());
        this._renderInkWave($focusTarget, null, isFocused, 0)
    },
    _renderLabels: function() {
        this.$element().removeClass(SLIDER_LABEL_POSITION_CLASS_PREFIX + "bottom").removeClass(SLIDER_LABEL_POSITION_CLASS_PREFIX + "top");
        if (this.option("label.visible")) {
            var min = this.option("min");
            var max = this.option("max");
            var position = this.option("label.position");
            var labelFormat = this.option("label.format");
            if (!this._$minLabel) {
                this._$minLabel = $("<div>").addClass(SLIDER_LABEL_CLASS).appendTo(this._$wrapper)
            }
            this._$minLabel.text(numberLocalization.format(min, labelFormat));
            if (!this._$maxLabel) {
                this._$maxLabel = $("<div>").addClass(SLIDER_LABEL_CLASS).appendTo(this._$wrapper)
            }
            this._$maxLabel.text(numberLocalization.format(max, labelFormat));
            this.$element().addClass(SLIDER_LABEL_POSITION_CLASS_PREFIX + position)
        } else {
            if (this._$minLabel) {
                this._$minLabel.remove();
                delete this._$minLabel
            }
            if (this._$maxLabel) {
                this._$maxLabel.remove();
                delete this._$maxLabel
            }
        }
    },
    _renderStartHandler: function() {
        var pointerDownEventName = addNamespace(pointerEvents.down, this.NAME);
        var clickEventName = addNamespace(clickName, this.NAME);
        var startAction = this._createAction(this._startHandler.bind(this));
        var $element = this.$element();
        eventsEngine.off($element, pointerDownEventName);
        eventsEngine.on($element, pointerDownEventName, e => {
            if (isMouseEvent(e)) {
                startAction({
                    event: e
                })
            }
        });
        eventsEngine.off($element, clickEventName);
        eventsEngine.on($element, clickEventName, e => {
            var $handle = this._activeHandle();
            if ($handle) {
                eventsEngine.trigger($handle, "focusin");
                eventsEngine.trigger($handle, "focus")
            }
            startAction({
                event: e
            })
        })
    },
    _itemWidthFunc: function() {
        return this._itemWidthRatio
    },
    _swipeStartHandler: function(e) {
        var rtlEnabled = this.option("rtlEnabled");
        if (isTouchEvent(e.event)) {
            this._createAction(this._startHandler.bind(this))({
                event: e.event
            })
        }
        this._feedbackDeferred = new Deferred;
        lock(this._feedbackDeferred);
        this._toggleActiveState(this._activeHandle(), this.option("activeStateEnabled"));
        this._startOffset = this._currentRatio;
        var startOffset = this._startOffset * this._swipePixelRatio();
        var endOffset = (1 - this._startOffset) * this._swipePixelRatio();
        e.event.maxLeftOffset = rtlEnabled ? endOffset : startOffset;
        e.event.maxRightOffset = rtlEnabled ? startOffset : endOffset;
        this._itemWidthRatio = getWidth(this.$element()) / this._swipePixelRatio();
        this._needPreventAnimation = true
    },
    _swipeEndHandler: function(e) {
        this._feedbackDeferred.resolve();
        this._toggleActiveState(this._activeHandle(), false);
        var offsetDirection = this.option("rtlEnabled") ? -1 : 1;
        delete this._needPreventAnimation;
        this._saveValueChangeEvent(e.event);
        this._changeValueOnSwipe(this._startOffset + offsetDirection * e.event.targetOffset / this._swipePixelRatio());
        delete this._startOffset;
        this._renderValue()
    },
    _activeHandle: function() {
        return this._$handle
    },
    _swipeUpdateHandler: function(e) {
        this._saveValueChangeEvent(e.event);
        this._updateHandlePosition(e)
    },
    _updateHandlePosition: function(e) {
        var offsetDirection = this.option("rtlEnabled") ? -1 : 1;
        var newRatio = Math.min(this._startOffset + offsetDirection * e.event.offset / this._swipePixelRatio(), 1);
        setWidth(this._$range, 100 * newRatio + "%");
        SliderHandle.getInstance(this._activeHandle()).fitTooltipPosition;
        this._changeValueOnSwipe(newRatio)
    },
    _swipePixelRatio: function() {
        var min = this.option("min");
        var max = this.option("max");
        var step = this._valueStep(this.option("step"));
        return (max - min) / step
    },
    _valueStep: function(step) {
        if (!step || isNaN(step)) {
            step = 1
        }
        return step
    },
    _getValueExponentLength: function() {
        var {
            step: step,
            min: min
        } = this.option();
        return Math.max(getExponentLength(step), getExponentLength(min))
    },
    _roundToExponentLength: function(value) {
        var valueExponentLength = this._getValueExponentLength();
        return roundFloatPart(value, valueExponentLength)
    },
    _changeValueOnSwipe: function(ratio) {
        var min = this.option("min");
        var max = this.option("max");
        var step = this._valueStep(this.option("step"));
        var newChange = ratio * (max - min);
        var newValue = min + newChange;
        if (step < 0) {
            return
        }
        if (newValue === max || newValue === min) {
            this._setValueOnSwipe(newValue)
        } else {
            var stepCount = Math.round((newValue - min) / step);
            newValue = this._roundToExponentLength(stepCount * step + min);
            this._setValueOnSwipe(Math.max(Math.min(newValue, max), min))
        }
    },
    _setValueOnSwipe: function(value) {
        this.option("value", value);
        this._saveValueChangeEvent(void 0)
    },
    _startHandler: function(args) {
        var e = args.event;
        this._currentRatio = (eventData(e).x - this._$bar.offset().left) / getWidth(this._$bar);
        if (this.option("rtlEnabled")) {
            this._currentRatio = 1 - this._currentRatio
        }
        this._saveValueChangeEvent(e);
        this._changeValueOnSwipe(this._currentRatio)
    },
    _renderValue: function() {
        this.callBase();
        var value = this.option("value");
        this._getSubmitElement().val(applyServerDecimalSeparator(value));
        SliderHandle.getInstance(this._activeHandle()).option("value", value)
    },
    _setRangeStyles: function(options) {
        options && this._$range.css(options)
    },
    _callHandlerMethod: function(name, args) {
        SliderHandle.getInstance(this._$handle)[name](args)
    },
    _repaintHandle: function() {
        this._callHandlerMethod("repaint")
    },
    _fitTooltip: function() {
        this._callHandlerMethod("updateTooltipPosition")
    },
    _optionChanged: function(args) {
        switch (args.name) {
            case "visible":
                this.callBase(args);
                this._renderHandle();
                this._repaintHandle();
                break;
            case "min":
            case "max":
                this._renderValue();
                this.callBase(args);
                this._renderLabels();
                this._renderAriaMinAndMax();
                this._fitTooltip();
                break;
            case "step":
                this._renderValue();
                break;
            case "keyStep":
                break;
            case "showRange":
                this._renderRangeVisibility();
                break;
            case "tooltip":
                this._renderHandle();
                break;
            case "label":
                this._renderLabels();
                break;
            case "useInkRipple":
                this._invalidate();
                break;
            default:
                this.callBase(args)
        }
    },
    _refresh: function() {
        this._toggleRTLDirection(this.option("rtlEnabled"));
        this._renderDimensions();
        this._renderValue();
        this._renderHandle();
        this._repaintHandle()
    },
    _clean: function() {
        delete this._inkRipple;
        this.callBase()
    }
});
registerComponent("dxSlider", Slider);
export default Slider;
