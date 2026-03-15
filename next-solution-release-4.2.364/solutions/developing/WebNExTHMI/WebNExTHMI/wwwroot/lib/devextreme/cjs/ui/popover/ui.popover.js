/**
 * DevExtreme (cjs/ui/popover/ui.popover.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.default = void 0;
var _size = require("../../core/utils/size");
var _renderer = _interopRequireDefault(require("../../core/renderer"));
var _window = require("../../core/utils/window");
var _element = require("../../core/element");
var _dom_adapter = _interopRequireDefault(require("../../core/dom_adapter"));
var _events_engine = _interopRequireDefault(require("../../events/core/events_engine"));
var _component_registrator = _interopRequireDefault(require("../../core/component_registrator"));
var _extend = require("../../core/utils/extend");
var _translator = require("../../animation/translator");
var _position = _interopRequireDefault(require("../../animation/position"));
var _type = require("../../core/utils/type");
var _math = require("../../core/utils/math");
var _index = require("../../events/utils/index");
var _ui = _interopRequireDefault(require("../widget/ui.errors"));
var _popup = _interopRequireDefault(require("../popup"));
var _position2 = require("../../core/utils/position");
var _popover_position_controller = require("./popover_position_controller");

function _interopRequireDefault(obj) {
    return obj && obj.__esModule ? obj : {
        default: obj
    }
}

function _typeof(obj) {
    return _typeof = "function" == typeof Symbol && "symbol" == typeof Symbol.iterator ? function(obj) {
        return typeof obj
    } : function(obj) {
        return obj && "function" == typeof Symbol && obj.constructor === Symbol && obj !== Symbol.prototype ? "symbol" : typeof obj
    }, _typeof(obj)
}
var POPOVER_CLASS = "dx-popover";
var POPOVER_WRAPPER_CLASS = "dx-popover-wrapper";
var POPOVER_ARROW_CLASS = "dx-popover-arrow";
var POPOVER_WITHOUT_TITLE_CLASS = "dx-popover-without-title";
var POSITION_FLIP_MAP = {
    left: "right",
    top: "bottom",
    right: "left",
    bottom: "top",
    center: "center"
};
var getEventNameByOption = function(optionValue) {
    return (0, _type.isObject)(optionValue) ? optionValue.name : optionValue
};
var getEventName = function(that, optionName) {
    var optionValue = that.option(optionName);
    return getEventNameByOption(optionValue)
};
var getEventDelay = function(that, optionName) {
    var optionValue = that.option(optionName);
    return (0, _type.isObject)(optionValue) && optionValue.delay
};
var attachEvent = function(that, name) {
    var _that$option = that.option(),
        target = _that$option.target,
        shading = _that$option.shading,
        disabled = _that$option.disabled,
        hideEvent = _that$option.hideEvent;
    var isSelector = (0, _type.isString)(target);
    var shouldIgnoreHideEvent = shading && "hide" === name;
    var event = shouldIgnoreHideEvent ? null : getEventName(that, "".concat(name, "Event"));
    if (shouldIgnoreHideEvent && hideEvent) {
        _ui.default.log("W1020")
    }
    if (!event || disabled) {
        return
    }
    var eventName = (0, _index.addNamespace)(event, that.NAME);
    var action = that._createAction(function() {
        var delay = getEventDelay(that, name + "Event");
        this._clearEventsTimeouts();
        if (delay) {
            this._timeouts[name] = setTimeout((function() {
                that[name]()
            }), delay)
        } else {
            that[name]()
        }
    }.bind(that), {
        validatingTargetName: "target"
    });
    var handler = function(e) {
        action({
            event: e,
            target: (0, _renderer.default)(e.currentTarget)
        })
    };
    var EVENT_HANDLER_NAME = "_" + name + "EventHandler";
    if (isSelector) {
        that[EVENT_HANDLER_NAME] = handler;
        _events_engine.default.on(_dom_adapter.default.getDocument(), eventName, target, handler)
    } else {
        var targetElement = (0, _element.getPublicElement)((0, _renderer.default)(target));
        that[EVENT_HANDLER_NAME] = void 0;
        _events_engine.default.on(targetElement, eventName, handler)
    }
};
var detachEvent = function(that, target, name, event) {
    var eventName = event || getEventName(that, name + "Event");
    if (!eventName) {
        return
    }
    eventName = (0, _index.addNamespace)(eventName, that.NAME);
    var EVENT_HANDLER_NAME = "_" + name + "EventHandler";
    if (that[EVENT_HANDLER_NAME]) {
        _events_engine.default.off(_dom_adapter.default.getDocument(), eventName, target, that[EVENT_HANDLER_NAME])
    } else {
        _events_engine.default.off((0, _element.getPublicElement)((0, _renderer.default)(target)), eventName)
    }
};
var Popover = _popup.default.inherit({
    _getDefaultOptions: function() {
        return (0, _extend.extend)(this.callBase(), {
            target: void 0,
            shading: false,
            position: (0, _extend.extend)({}, _popover_position_controller.POPOVER_POSITION_ALIASES.bottom),
            closeOnOutsideClick: true,
            animation: {
                show: {
                    type: "fade",
                    from: 0,
                    to: 1
                },
                hide: {
                    type: "fade",
                    to: 0
                }
            },
            showTitle: false,
            width: "auto",
            height: "auto",
            dragEnabled: false,
            resizeEnabled: false,
            fullScreen: false,
            hideOnParentScroll: true,
            arrowPosition: "",
            arrowOffset: 0,
            _fixWrapperPosition: true
        })
    },
    _defaultOptionsRules: function() {
        return [{
            device: {
                platform: "ios"
            },
            options: {
                arrowPosition: {
                    boundaryOffset: {
                        h: 20,
                        v: -10
                    },
                    collision: "fit"
                }
            }
        }, {
            device: function() {
                return !(0, _window.hasWindow)()
            },
            options: {
                animation: null
            }
        }]
    },
    _init: function() {
        this.callBase();
        this._renderArrow();
        this._timeouts = {};
        this.$element().addClass(POPOVER_CLASS);
        this.$wrapper().addClass(POPOVER_WRAPPER_CLASS)
    },
    _render: function() {
        this.callBase.apply(this, arguments);
        this._detachEvents(this.option("target"));
        this._attachEvents()
    },
    _detachEvents: function(target) {
        detachEvent(this, target, "show");
        detachEvent(this, target, "hide")
    },
    _attachEvents: function() {
        attachEvent(this, "show");
        attachEvent(this, "hide")
    },
    _renderArrow: function() {
        this._$arrow = (0, _renderer.default)("<div>").addClass(POPOVER_ARROW_CLASS).prependTo(this.$overlayContent())
    },
    _documentDownHandler: function(e) {
        if (this._isOutsideClick(e)) {
            return this.callBase(e)
        }
        return true
    },
    _isOutsideClick: function(e) {
        return !(0, _renderer.default)(e.target).closest(this.option("target")).length
    },
    _animate: function(animation) {
        if (animation && animation.to && "object" === _typeof(animation.to)) {
            (0, _extend.extend)(animation.to, {
                position: this._getContainerPosition()
            })
        }
        this.callBase.apply(this, arguments)
    },
    _stopAnimation: function() {
        this.callBase.apply(this, arguments)
    },
    _renderTitle: function() {
        this.$wrapper().toggleClass(POPOVER_WITHOUT_TITLE_CLASS, !this.option("showTitle"));
        this.callBase()
    },
    _renderPosition: function() {
        this.callBase();
        this._renderOverlayPosition();
        this._actions.onPositioned()
    },
    _renderOverlayPosition: function() {
        this._resetOverlayPosition();
        this._updateContentSize();
        var contentPosition = this._getContainerPosition();
        var resultLocation = _position.default.setup(this.$overlayContent(), contentPosition);
        var positionSide = this._getSideByLocation(resultLocation);
        this._togglePositionClass("dx-position-" + positionSide);
        this._toggleFlippedClass(resultLocation.h.flip, resultLocation.v.flip);
        var isArrowVisible = this._isHorizontalSide() || this._isVerticalSide();
        if (isArrowVisible) {
            this._renderArrowPosition(positionSide)
        }
    },
    _resetOverlayPosition: function() {
        this._setContentHeight(true);
        this._togglePositionClass("dx-position-" + this._positionController._positionSide);
        (0, _translator.move)(this.$overlayContent(), {
            left: 0,
            top: 0
        });
        this._$arrow.css({
            top: "auto",
            right: "auto",
            bottom: "auto",
            left: "auto"
        })
    },
    _updateContentSize: function() {
        if (!this.$content()) {
            return
        }
        var containerLocation = _position.default.calculate(this.$overlayContent(), this._getContainerPosition());
        if (containerLocation.h.oversize > 0 && this._isHorizontalSide() && !containerLocation.h.fit) {
            var newContainerWidth = (0, _size.getWidth)(this.$overlayContent()) - containerLocation.h.oversize;
            (0, _size.setWidth)(this.$overlayContent(), newContainerWidth)
        }
        if (containerLocation.v.oversize > 0 && this._isVerticalSide() && !containerLocation.v.fit) {
            var newOverlayContentHeight = (0, _size.getHeight)(this.$overlayContent()) - containerLocation.v.oversize;
            var newPopupContentHeight = (0, _size.getHeight)(this.$content()) - containerLocation.v.oversize;
            (0, _size.setHeight)(this.$overlayContent(), newOverlayContentHeight);
            (0, _size.setHeight)(this.$content(), newPopupContentHeight)
        }
    },
    _getContainerPosition: function() {
        return this._positionController._getContainerPosition()
    },
    _getSideByLocation: function(location) {
        var isFlippedByVertical = location.v.flip;
        var isFlippedByHorizontal = location.h.flip;
        return this._isVerticalSide() && isFlippedByVertical || this._isHorizontalSide() && isFlippedByHorizontal || this._isPopoverInside() ? POSITION_FLIP_MAP[this._positionController._positionSide] : this._positionController._positionSide
    },
    _togglePositionClass: function(positionClass) {
        this.$wrapper().removeClass("dx-position-left dx-position-right dx-position-top dx-position-bottom").addClass(positionClass)
    },
    _toggleFlippedClass: function(isFlippedHorizontal, isFlippedVertical) {
        this.$wrapper().toggleClass("dx-popover-flipped-horizontal", isFlippedHorizontal).toggleClass("dx-popover-flipped-vertical", isFlippedVertical)
    },
    _renderArrowPosition: function(side) {
        var arrowRect = (0, _position2.getBoundingRect)(this._$arrow.get(0));
        var arrowFlip = -(this._isVerticalSide(side) ? arrowRect.height : arrowRect.width);
        this._$arrow.css(POSITION_FLIP_MAP[side], arrowFlip);
        var axis = this._isVerticalSide(side) ? "left" : "top";
        var sizeProperty = this._isVerticalSide(side) ? "width" : "height";
        var $target = (0, _renderer.default)(this._positionController._position.of);
        var targetOffset = _position.default.offset($target) || {
            top: 0,
            left: 0
        };
        var contentOffset = _position.default.offset(this.$overlayContent());
        var arrowSize = arrowRect[sizeProperty];
        var contentLocation = contentOffset[axis];
        var contentSize = (0, _position2.getBoundingRect)(this.$overlayContent().get(0))[sizeProperty];
        var targetLocation = targetOffset[axis];
        var targetElement = $target.get(0);
        var targetSize = targetElement && !targetElement.preventDefault ? (0, _position2.getBoundingRect)(targetElement)[sizeProperty] : 0;
        var min = Math.max(contentLocation, targetLocation);
        var max = Math.min(contentLocation + contentSize, targetLocation + targetSize);
        var arrowLocation;
        if ("start" === this.option("arrowPosition")) {
            arrowLocation = min - contentLocation
        } else if ("end" === this.option("arrowPosition")) {
            arrowLocation = max - contentLocation - arrowSize
        } else {
            arrowLocation = (min + max) / 2 - contentLocation - arrowSize / 2
        }
        var borderWidth = this._positionController._getContentBorderWidth(side);
        var finalArrowLocation = (0, _math.fitIntoRange)(arrowLocation - borderWidth + this.option("arrowOffset"), borderWidth, contentSize - arrowSize - 2 * borderWidth);
        this._$arrow.css(axis, finalArrowLocation)
    },
    _isPopoverInside: function() {
        return this._positionController._isPopoverInside()
    },
    _setContentHeight: function(fullUpdate) {
        if (fullUpdate) {
            this.callBase()
        }
    },
    _getPositionControllerConfig: function() {
        var _this$option = this.option(),
            shading = _this$option.shading;
        return (0, _extend.extend)({}, this.callBase(), {
            shading: shading,
            $arrow: this._$arrow
        })
    },
    _initPositionController: function() {
        this._positionController = new _popover_position_controller.PopoverPositionController(this._getPositionControllerConfig())
    },
    _renderWrapperDimensions: function() {
        if (this.option("shading")) {
            this.$wrapper().css({
                width: "100%",
                height: "100%"
            })
        }
    },
    _isVerticalSide: function(side) {
        return this._positionController._isVerticalSide(side)
    },
    _isHorizontalSide: function(side) {
        return this._positionController._isHorizontalSide(side)
    },
    _clearEventTimeout: function(name) {
        clearTimeout(this._timeouts[name])
    },
    _clearEventsTimeouts: function() {
        this._clearEventTimeout("show");
        this._clearEventTimeout("hide")
    },
    _clean: function() {
        this._detachEvents(this.option("target"));
        this.callBase.apply(this, arguments)
    },
    _optionChanged: function(args) {
        switch (args.name) {
            case "arrowPosition":
            case "arrowOffset":
                this._renderGeometry();
                break;
            case "fullScreen":
                if (args.value) {
                    this.option("fullScreen", false)
                }
                break;
            case "target":
                args.previousValue && this._detachEvents(args.previousValue);
                this.callBase(args);
                break;
            case "showEvent":
            case "hideEvent":
                var name = args.name.substring(0, 4);
                var event = getEventNameByOption(args.previousValue);
                this.hide();
                detachEvent(this, this.option("target"), name, event);
                attachEvent(this, name);
                break;
            case "visible":
                this._clearEventTimeout(args.value ? "show" : "hide");
                this.callBase(args);
                break;
            default:
                this.callBase(args)
        }
    },
    show: function(target) {
        if (target) {
            this.option("target", target)
        }
        return this.callBase()
    }
});
(0, _component_registrator.default)("dxPopover", Popover);
var _default = Popover;
exports.default = _default;
module.exports = exports.default;
module.exports.default = exports.default;
