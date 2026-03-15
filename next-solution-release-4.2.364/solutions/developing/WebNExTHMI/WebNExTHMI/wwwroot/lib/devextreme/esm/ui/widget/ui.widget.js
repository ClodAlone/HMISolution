/**
 * DevExtreme (esm/ui/widget/ui.widget.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import $ from "../../core/renderer";
import Action from "../../core/action";
import DOMComponent from "../../core/dom_component";
import {
    active,
    focus,
    hover,
    keyboard
} from "../../events/short";
import {
    deferRender,
    deferRenderer,
    noop
} from "../../core/utils/common";
import {
    each
} from "../../core/utils/iterator";
import {
    extend
} from "../../core/utils/extend";
import {
    focusable as focusableSelector
} from "./selectors";
import {
    inArray
} from "../../core/utils/array";
import {
    isPlainObject,
    isDefined
} from "../../core/utils/type";
import devices from "../../core/devices";
import {
    compare as compareVersions
} from "../../core/utils/version";
import "../../events/click";
import "../../events/core/emitter.feedback";
import "../../events/hover";

function setAttribute(name, value, target) {
    name = "role" === name || "id" === name ? name : "aria-".concat(name);
    value = isDefined(value) ? value.toString() : null;
    target.attr(name, value)
}
var Widget = DOMComponent.inherit({
    _feedbackHideTimeout: 400,
    _feedbackShowTimeout: 30,
    _supportedKeys: () => ({}),
    _getDefaultOptions() {
        return extend(this.callBase(), {
            hoveredElement: null,
            isActive: false,
            disabled: false,
            visible: true,
            hint: void 0,
            activeStateEnabled: false,
            onContentReady: null,
            hoverStateEnabled: false,
            focusStateEnabled: false,
            tabIndex: 0,
            accessKey: void 0,
            onFocusIn: null,
            onFocusOut: null,
            onKeyboardHandled: null,
            ignoreParentReadOnly: false,
            useResizeObserver: true
        })
    },
    _defaultOptionsRules: function() {
        return this.callBase().concat([{
            device: function() {
                var device = devices.real();
                var platform = device.platform;
                var version = device.version;
                return "ios" === platform && compareVersions(version, "13.3") <= 0 || "android" === platform && compareVersions(version, "4.4.4") <= 0
            },
            options: {
                useResizeObserver: false
            }
        }])
    },
    _init() {
        this.callBase();
        this._initContentReadyAction()
    },
    _innerWidgetOptionChanged: function(innerWidget, args) {
        var options = Widget.getOptionsFromContainer(args);
        innerWidget && innerWidget.option(options);
        this._options.cache(args.name, options)
    },
    _bindInnerWidgetOptions(innerWidget, optionsContainer) {
        var syncOptions = () => this._options.silent(optionsContainer, extend({}, innerWidget.option()));
        syncOptions();
        innerWidget.on("optionChanged", syncOptions)
    },
    _getAriaTarget() {
        return this._focusTarget()
    },
    _initContentReadyAction() {
        this._contentReadyAction = this._createActionByOption("onContentReady", {
            excludeValidators: ["disabled", "readOnly"]
        })
    },
    _initMarkup() {
        var {
            disabled: disabled,
            visible: visible
        } = this.option();
        this.$element().addClass("dx-widget");
        this._toggleDisabledState(disabled);
        this._toggleVisibility(visible);
        this._renderHint();
        this._isFocusable() && this._renderFocusTarget();
        this.callBase()
    },
    _render() {
        this.callBase();
        this._renderContent();
        this._renderFocusState();
        this._attachFeedbackEvents();
        this._attachHoverEvents();
        this._toggleIndependentState()
    },
    _renderHint() {
        var {
            hint: hint
        } = this.option();
        this.$element().attr("title", hint || null)
    },
    _renderContent() {
        deferRender(() => !this._disposed ? this._renderContentImpl() : void 0).done(() => !this._disposed ? this._fireContentReadyAction() : void 0)
    },
    _renderContentImpl: noop,
    _fireContentReadyAction: deferRenderer((function() {
        return this._contentReadyAction()
    })),
    _dispose() {
        this._contentReadyAction = null;
        this._detachKeyboardEvents();
        this.callBase()
    },
    _resetActiveState() {
        this._toggleActiveState(this._eventBindingTarget(), false)
    },
    _clean() {
        this._cleanFocusState();
        this._resetActiveState();
        this.callBase();
        this.$element().empty()
    },
    _toggleVisibility(visible) {
        this.$element().toggleClass("dx-state-invisible", !visible);
        this.setAria("hidden", !visible || void 0)
    },
    _renderFocusState() {
        this._attachKeyboardEvents();
        if (this._isFocusable()) {
            this._renderFocusTarget();
            this._attachFocusEvents();
            this._renderAccessKey()
        }
    },
    _renderAccessKey() {
        var $el = this._focusTarget();
        var {
            accessKey: accessKey
        } = this.option();
        $el.attr("accesskey", accessKey)
    },
    _isFocusable() {
        var {
            focusStateEnabled: focusStateEnabled,
            disabled: disabled
        } = this.option();
        return focusStateEnabled && !disabled
    },
    _eventBindingTarget() {
        return this.$element()
    },
    _focusTarget() {
        return this._getActiveElement()
    },
    _getActiveElement() {
        var activeElement = this._eventBindingTarget();
        if (this._activeStateUnit) {
            return activeElement.find(this._activeStateUnit).not(".dx-state-disabled")
        }
        return activeElement
    },
    _renderFocusTarget() {
        var {
            tabIndex: tabIndex
        } = this.option();
        this._focusTarget().attr("tabIndex", tabIndex)
    },
    _keyboardEventBindingTarget() {
        return this._eventBindingTarget()
    },
    _refreshFocusEvent() {
        this._detachFocusEvents();
        this._attachFocusEvents()
    },
    _focusEventTarget() {
        return this._focusTarget()
    },
    _focusInHandler(event) {
        if (!event.isDefaultPrevented()) {
            this._createActionByOption("onFocusIn", {
                beforeExecute: () => this._updateFocusState(event, true),
                excludeValidators: ["readOnly"]
            })({
                event: event
            })
        }
    },
    _focusOutHandler(event) {
        if (!event.isDefaultPrevented()) {
            this._createActionByOption("onFocusOut", {
                beforeExecute: () => this._updateFocusState(event, false),
                excludeValidators: ["readOnly", "disabled"]
            })({
                event: event
            })
        }
    },
    _updateFocusState(_ref, isFocused) {
        var {
            target: target
        } = _ref;
        if (-1 !== inArray(target, this._focusTarget())) {
            this._toggleFocusClass(isFocused, $(target))
        }
    },
    _toggleFocusClass(isFocused, $element) {
        var $focusTarget = $element && $element.length ? $element : this._focusTarget();
        $focusTarget.toggleClass("dx-state-focused", isFocused)
    },
    _hasFocusClass(element) {
        var $focusTarget = $(element || this._focusTarget());
        return $focusTarget.hasClass("dx-state-focused")
    },
    _isFocused() {
        return this._hasFocusClass()
    },
    _getKeyboardListeners: () => [],
    _attachKeyboardEvents() {
        this._detachKeyboardEvents();
        var {
            focusStateEnabled: focusStateEnabled,
            onKeyboardHandled: onKeyboardHandled
        } = this.option();
        var hasChildListeners = this._getKeyboardListeners().length;
        var hasKeyboardEventHandler = !!onKeyboardHandled;
        var shouldAttach = focusStateEnabled || hasChildListeners || hasKeyboardEventHandler;
        if (shouldAttach) {
            this._keyboardListenerId = keyboard.on(this._keyboardEventBindingTarget(), this._focusTarget(), opts => this._keyboardHandler(opts))
        }
    },
    _keyboardHandler(options, onlyChildProcessing) {
        if (!onlyChildProcessing) {
            var {
                originalEvent: originalEvent,
                keyName: keyName,
                which: which
            } = options;
            var keys = this._supportedKeys(originalEvent);
            var func = keys[keyName] || keys[which];
            if (void 0 !== func) {
                var handler = func.bind(this);
                var result = handler(originalEvent, options);
                if (!result) {
                    return false
                }
            }
        }
        var keyboardListeners = this._getKeyboardListeners();
        var {
            onKeyboardHandled: onKeyboardHandled
        } = this.option();
        keyboardListeners.forEach(listener => listener && listener._keyboardHandler(options));
        onKeyboardHandled && onKeyboardHandled(options);
        return true
    },
    _refreshFocusState() {
        this._cleanFocusState();
        this._renderFocusState()
    },
    _cleanFocusState() {
        var $element = this._focusTarget();
        $element.removeAttr("tabIndex");
        this._toggleFocusClass(false);
        this._detachFocusEvents();
        this._detachKeyboardEvents()
    },
    _detachKeyboardEvents() {
        keyboard.off(this._keyboardListenerId);
        this._keyboardListenerId = null
    },
    _attachHoverEvents() {
        var {
            hoverStateEnabled: hoverStateEnabled
        } = this.option();
        var selector = this._activeStateUnit;
        var $el = this._eventBindingTarget();
        hover.off($el, {
            selector: selector,
            namespace: "UIFeedback"
        });
        if (hoverStateEnabled) {
            hover.on($el, new Action(_ref2 => {
                var {
                    event: event,
                    element: element
                } = _ref2;
                this._hoverStartHandler(event);
                this.option("hoveredElement", $(element))
            }, {
                excludeValidators: ["readOnly"]
            }), event => {
                this.option("hoveredElement", null);
                this._hoverEndHandler(event)
            }, {
                selector: selector,
                namespace: "UIFeedback"
            })
        }
    },
    _attachFeedbackEvents() {
        var {
            activeStateEnabled: activeStateEnabled
        } = this.option();
        var selector = this._activeStateUnit;
        var $el = this._eventBindingTarget();
        active.off($el, {
            namespace: "UIFeedback",
            selector: selector
        });
        if (activeStateEnabled) {
            active.on($el, new Action(_ref3 => {
                var {
                    event: event,
                    element: element
                } = _ref3;
                return this._toggleActiveState($(element), true, event)
            }), new Action(_ref4 => {
                var {
                    event: event,
                    element: element
                } = _ref4;
                return this._toggleActiveState($(element), false, event)
            }, {
                excludeValidators: ["disabled", "readOnly"]
            }), {
                showTimeout: this._feedbackShowTimeout,
                hideTimeout: this._feedbackHideTimeout,
                selector: selector,
                namespace: "UIFeedback"
            })
        }
    },
    _detachFocusEvents() {
        var $el = this._focusEventTarget();
        focus.off($el, {
            namespace: "".concat(this.NAME, "Focus")
        })
    },
    _attachFocusEvents() {
        var $el = this._focusEventTarget();
        focus.on($el, e => this._focusInHandler(e), e => this._focusOutHandler(e), {
            namespace: "".concat(this.NAME, "Focus"),
            isFocusable: (index, el) => $(el).is(focusableSelector)
        })
    },
    _hoverStartHandler: noop,
    _hoverEndHandler: noop,
    _toggleActiveState($element, value) {
        this.option("isActive", value);
        $element.toggleClass("dx-state-active", value)
    },
    _updatedHover() {
        var hoveredElement = this._options.silent("hoveredElement");
        this._hover(hoveredElement, hoveredElement)
    },
    _findHoverTarget($el) {
        return $el && $el.closest(this._activeStateUnit || this._eventBindingTarget())
    },
    _hover($el, $previous) {
        var {
            hoverStateEnabled: hoverStateEnabled,
            disabled: disabled,
            isActive: isActive
        } = this.option();
        $previous = this._findHoverTarget($previous);
        $previous && $previous.toggleClass("dx-state-hover", false);
        if ($el && hoverStateEnabled && !disabled && !isActive) {
            var newHoveredElement = this._findHoverTarget($el);
            newHoveredElement && newHoveredElement.toggleClass("dx-state-hover", true)
        }
    },
    _toggleDisabledState(value) {
        this.$element().toggleClass("dx-state-disabled", Boolean(value));
        this.setAria("disabled", value || void 0)
    },
    _toggleIndependentState() {
        this.$element().toggleClass("dx-state-independent", this.option("ignoreParentReadOnly"))
    },
    _setWidgetOption(widgetName, args) {
        if (!this[widgetName]) {
            return
        }
        if (isPlainObject(args[0])) {
            each(args[0], (option, value) => this._setWidgetOption(widgetName, [option, value]));
            return
        }
        var optionName = args[0];
        var value = args[1];
        if (1 === args.length) {
            value = this.option(optionName)
        }
        var widgetOptionMap = this["".concat(widgetName, "OptionMap")];
        this[widgetName].option(widgetOptionMap ? widgetOptionMap(optionName) : optionName, value)
    },
    _optionChanged(args) {
        var {
            name: name,
            value: value,
            previousValue: previousValue
        } = args;
        switch (name) {
            case "disabled":
                this._toggleDisabledState(value);
                this._updatedHover();
                this._refreshFocusState();
                break;
            case "hint":
                this._renderHint();
                break;
            case "ignoreParentReadOnly":
                this._toggleIndependentState();
                break;
            case "activeStateEnabled":
                this._attachFeedbackEvents();
                break;
            case "hoverStateEnabled":
                this._attachHoverEvents();
                this._updatedHover();
                break;
            case "tabIndex":
            case "focusStateEnabled":
                this._refreshFocusState();
                break;
            case "onFocusIn":
            case "onFocusOut":
            case "useResizeObserver":
                break;
            case "accessKey":
                this._renderAccessKey();
                break;
            case "hoveredElement":
                this._hover(value, previousValue);
                break;
            case "isActive":
                this._updatedHover();
                break;
            case "visible":
                this._toggleVisibility(value);
                if (this._isVisibilityChangeSupported()) {
                    this._checkVisibilityChanged(value ? "shown" : "hiding")
                }
                break;
            case "onKeyboardHandled":
                this._attachKeyboardEvents();
                break;
            case "onContentReady":
                this._initContentReadyAction();
                break;
            default:
                this.callBase(args)
        }
    },
    _isVisible() {
        var {
            visible: visible
        } = this.option();
        return this.callBase() && visible
    },
    beginUpdate() {
        this._ready(false);
        this.callBase()
    },
    endUpdate() {
        this.callBase();
        if (this._initialized) {
            this._ready(true)
        }
    },
    _ready(value) {
        if (0 === arguments.length) {
            return this._isReady
        }
        this._isReady = value
    },
    setAria() {
        if (!isPlainObject(arguments.length <= 0 ? void 0 : arguments[0])) {
            setAttribute(arguments.length <= 0 ? void 0 : arguments[0], arguments.length <= 1 ? void 0 : arguments[1], (arguments.length <= 2 ? void 0 : arguments[2]) || this._getAriaTarget())
        } else {
            var target = (arguments.length <= 1 ? void 0 : arguments[1]) || this._getAriaTarget();
            each(arguments.length <= 0 ? void 0 : arguments[0], (name, value) => setAttribute(name, value, target))
        }
    },
    isReady() {
        return this._ready()
    },
    repaint() {
        this._refresh()
    },
    focus() {
        focus.trigger(this._focusTarget())
    },
    registerKeyHandler(key, handler) {
        var currentKeys = this._supportedKeys();
        this._supportedKeys = () => extend(currentKeys, {
            [key]: handler
        })
    }
});
Widget.getOptionsFromContainer = _ref5 => {
    var {
        name: name,
        fullName: fullName,
        value: value
    } = _ref5;
    var options = {};
    if (name === fullName) {
        options = value
    } else {
        var option = fullName.split(".").pop();
        options[option] = value
    }
    return options
};
export default Widget;
