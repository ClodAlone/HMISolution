/**
 * DevExtreme (esm/ui/text_box/ui.text_editor.base.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import $ from "../../core/renderer";
import domAdapter from "../../core/dom_adapter";
import eventsEngine from "../../events/core/events_engine";
import {
    focused
} from "../widget/selectors";
import {
    isDefined
} from "../../core/utils/type";
import {
    extend
} from "../../core/utils/extend";
import {
    inArray
} from "../../core/utils/array";
import {
    each
} from "../../core/utils/iterator";
import {
    current,
    isMaterial
} from "../themes";
import devices from "../../core/devices";
import Editor from "../editor/editor";
import {
    addNamespace,
    normalizeKeyName
} from "../../events/utils/index";
import pointerEvents from "../../events/pointer";
import ClearButton from "./ui.text_editor.clear";
import TextEditorButtonCollection from "./texteditor_button_collection/index";
import config from "../../core/config";
import errors from "../widget/ui.errors";
import {
    Deferred
} from "../../core/utils/deferred";
import LoadIndicator from "../load_indicator";
import {
    TextEditorLabel
} from "./ui.text_editor.label";
import {
    getWidth
} from "../../core/utils/size";
import resizeObserverSingleton from "../../core/resize_observer";
var TEXTEDITOR_CLASS = "dx-texteditor";
var TEXTEDITOR_INPUT_CONTAINER_CLASS = "dx-texteditor-input-container";
var TEXTEDITOR_INPUT_CLASS = "dx-texteditor-input";
var TEXTEDITOR_INPUT_SELECTOR = "." + TEXTEDITOR_INPUT_CLASS;
var TEXTEDITOR_CONTAINER_CLASS = "dx-texteditor-container";
var TEXTEDITOR_BUTTONS_CONTAINER_CLASS = "dx-texteditor-buttons-container";
var TEXTEDITOR_PLACEHOLDER_CLASS = "dx-placeholder";
var TEXTEDITOR_EMPTY_INPUT_CLASS = "dx-texteditor-empty";
var STATE_INVISIBLE_CLASS = "dx-state-invisible";
var TEXTEDITOR_PENDING_INDICATOR_CLASS = "dx-pending-indicator";
var TEXTEDITOR_VALIDATION_PENDING_CLASS = "dx-validation-pending";
var TEXTEDITOR_VALID_CLASS = "dx-valid";
var EVENTS_LIST = ["KeyDown", "KeyPress", "KeyUp", "Change", "Cut", "Copy", "Paste", "Input"];
var CONTROL_KEYS = ["tab", "enter", "shift", "control", "alt", "escape", "pageUp", "pageDown", "end", "home", "leftArrow", "upArrow", "rightArrow", "downArrow"];
var TextEditorLabelCreator = TextEditorLabel;

function checkButtonsOptionType(buttons) {
    if (isDefined(buttons) && !Array.isArray(buttons)) {
        throw errors.Error("E1053")
    }
}
var TextEditorBase = Editor.inherit({
    ctor: function(_, options) {
        if (options) {
            checkButtonsOptionType(options.buttons)
        }
        this._buttonCollection = new TextEditorButtonCollection(this, this._getDefaultButtons());
        this._$beforeButtonsContainer = null;
        this._$afterButtonsContainer = null;
        this._labelContainerElement = null;
        this.callBase.apply(this, arguments)
    },
    _getDefaultOptions: function() {
        return extend(this.callBase(), {
            buttons: void 0,
            value: "",
            spellcheck: false,
            showClearButton: false,
            valueChangeEvent: "change",
            placeholder: "",
            inputAttr: {},
            onFocusIn: null,
            onFocusOut: null,
            onKeyDown: null,
            onKeyUp: null,
            onChange: null,
            onInput: null,
            onCut: null,
            onCopy: null,
            onPaste: null,
            onEnterKey: null,
            mode: "text",
            hoverStateEnabled: true,
            focusStateEnabled: true,
            text: void 0,
            displayValueFormatter: function(value) {
                return isDefined(value) && false !== value ? value : ""
            },
            stylingMode: config().editorStylingMode || "outlined",
            showValidationMark: true,
            label: "",
            labelMode: "static",
            labelMark: ""
        })
    },
    _defaultOptionsRules: function() {
        var themeName = current();
        return this.callBase().concat([{
            device: function() {
                return isMaterial(themeName)
            },
            options: {
                stylingMode: config().editorStylingMode || "filled",
                labelMode: "floating"
            }
        }])
    },
    _getDefaultButtons: function() {
        return [{
            name: "clear",
            Ctor: ClearButton
        }]
    },
    _isClearButtonVisible: function() {
        return this.option("showClearButton") && !this.option("readOnly")
    },
    _input: function() {
        return this.$element().find(TEXTEDITOR_INPUT_SELECTOR).first()
    },
    _isFocused: function() {
        return focused(this._input()) || this.callBase()
    },
    _inputWrapper: function() {
        return this.$element()
    },
    _buttonsContainer: function() {
        return this._inputWrapper().find("." + TEXTEDITOR_BUTTONS_CONTAINER_CLASS).eq(0)
    },
    _isControlKey: function(key) {
        return -1 !== CONTROL_KEYS.indexOf(key)
    },
    _renderStylingMode: function() {
        this.callBase();
        this._updateButtonsStyling(this.option("stylingMode"))
    },
    _initMarkup: function() {
        this.$element().addClass(TEXTEDITOR_CLASS);
        this._renderInput();
        this._renderStylingMode();
        this._renderInputType();
        this._renderPlaceholder();
        this._renderProps();
        this.callBase();
        this._renderValue();
        this._renderLabel()
    },
    _render: function() {
        this.callBase();
        this._refreshValueChangeEvent();
        this._renderEvents();
        this._renderEnterKeyAction();
        this._renderEmptinessEvent()
    },
    _renderInput: function() {
        this._$buttonsContainer = this._$textEditorContainer = $("<div>").addClass(TEXTEDITOR_CONTAINER_CLASS).appendTo(this.$element());
        this._$textEditorInputContainer = $("<div>").addClass(TEXTEDITOR_INPUT_CONTAINER_CLASS).appendTo(this._$textEditorContainer);
        this._$textEditorInputContainer.append(this._createInput());
        this._renderButtonContainers()
    },
    _getInputContainer() {
        return this._$textEditorInputContainer
    },
    _renderPendingIndicator: function() {
        this.$element().addClass(TEXTEDITOR_VALIDATION_PENDING_CLASS);
        var $inputContainer = this._getInputContainer();
        var $indicatorElement = $("<div>").addClass(TEXTEDITOR_PENDING_INDICATOR_CLASS).appendTo($inputContainer);
        this._pendingIndicator = this._createComponent($indicatorElement, LoadIndicator)
    },
    _disposePendingIndicator: function() {
        if (!this._pendingIndicator) {
            return
        }
        this._pendingIndicator.dispose();
        this._pendingIndicator.$element().remove();
        this._pendingIndicator = null;
        this.$element().removeClass(TEXTEDITOR_VALIDATION_PENDING_CLASS)
    },
    _renderValidationState: function() {
        this.callBase();
        var isPending = "pending" === this.option("validationStatus");
        var $element = this.$element();
        if (isPending) {
            !this._pendingIndicator && this._renderPendingIndicator();
            this._showValidMark = false
        } else {
            if ("invalid" === this.option("validationStatus")) {
                this._showValidMark = false
            }
            if (!this._showValidMark && true === this.option("showValidationMark")) {
                this._showValidMark = "valid" === this.option("validationStatus") && !!this._pendingIndicator
            }
            this._disposePendingIndicator()
        }
        $element.toggleClass(TEXTEDITOR_VALID_CLASS, !!this._showValidMark)
    },
    _renderButtonContainers: function() {
        var buttons = this.option("buttons");
        this._$beforeButtonsContainer = this._buttonCollection.renderBeforeButtons(buttons, this._$buttonsContainer);
        this._$afterButtonsContainer = this._buttonCollection.renderAfterButtons(buttons, this._$buttonsContainer)
    },
    _cleanButtonContainers: function() {
        var _this$_$beforeButtons, _this$_$afterButtonsC;
        null === (_this$_$beforeButtons = this._$beforeButtonsContainer) || void 0 === _this$_$beforeButtons ? void 0 : _this$_$beforeButtons.remove();
        null === (_this$_$afterButtonsC = this._$afterButtonsContainer) || void 0 === _this$_$afterButtonsC ? void 0 : _this$_$afterButtonsC.remove();
        this._buttonCollection.clean()
    },
    _clean() {
        this._buttonCollection.clean();
        this._disposePendingIndicator();
        this._unobserveLabelContainerResize();
        this._$beforeButtonsContainer = null;
        this._$afterButtonsContainer = null;
        this._$textEditorContainer = null;
        this._$buttonsContainer = null;
        this.callBase()
    },
    _createInput: function() {
        var $input = $("<input>");
        this._applyInputAttributes($input, this.option("inputAttr"));
        return $input
    },
    _setSubmitElementName: function(name) {
        var inputAttrName = this.option("inputAttr.name");
        return this.callBase(name || inputAttrName || "")
    },
    _applyInputAttributes: function($input, customAttributes) {
        var inputAttributes = extend(this._getDefaultAttributes(), customAttributes);
        $input.attr(inputAttributes).addClass(TEXTEDITOR_INPUT_CLASS).css("minHeight", this.option("height") ? "0" : "")
    },
    _getDefaultAttributes: function() {
        var defaultAttributes = {
            autocomplete: "off"
        };
        var {
            ios: ios,
            mac: mac
        } = devices.real();
        if (ios || mac) {
            defaultAttributes.placeholder = " "
        }
        return defaultAttributes
    },
    _updateButtons: function(names) {
        this._buttonCollection.updateButtons(names)
    },
    _updateButtonsStyling: function(editorStylingMode) {
        each(this.option("buttons"), (_, _ref) => {
            var {
                options: options,
                name: buttonName
            } = _ref;
            if (options && !options.stylingMode && this.option("visible")) {
                var buttonInstance = this.getButton(buttonName);
                buttonInstance.option && buttonInstance.option("stylingMode", "underlined" === editorStylingMode ? "text" : "contained")
            }
        })
    },
    _renderValue: function() {
        var renderInputPromise = this._renderInputValue();
        return renderInputPromise.promise()
    },
    _renderInputValue: function(value) {
        var _value;
        value = null !== (_value = value) && void 0 !== _value ? _value : this.option("value");
        var text = this.option("text");
        var displayValue = this.option("displayValue");
        var displayValueFormatter = this.option("displayValueFormatter");
        if (void 0 !== displayValue && null !== value) {
            text = displayValueFormatter(displayValue)
        } else if (!isDefined(text)) {
            text = displayValueFormatter(value)
        }
        this.option("text", text);
        if (this._input().val() !== (isDefined(text) ? text : "")) {
            this._renderDisplayText(text)
        } else {
            this._toggleEmptinessEventHandler()
        }
        return (new Deferred).resolve()
    },
    _renderDisplayText: function(text) {
        this._input().val(text);
        this._toggleEmptinessEventHandler()
    },
    _isValueValid: function() {
        if (this._input().length) {
            var validity = this._input().get(0).validity;
            if (validity) {
                return validity.valid
            }
        }
        return true
    },
    _toggleEmptiness: function(isEmpty) {
        this.$element().toggleClass(TEXTEDITOR_EMPTY_INPUT_CLASS, isEmpty);
        this._togglePlaceholder(isEmpty)
    },
    _togglePlaceholder: function(isEmpty) {
        this.$element().find(".".concat(TEXTEDITOR_PLACEHOLDER_CLASS)).eq(0).toggleClass(STATE_INVISIBLE_CLASS, !isEmpty)
    },
    _renderProps: function() {
        this._toggleReadOnlyState();
        this._toggleSpellcheckState();
        this._toggleTabIndex()
    },
    _toggleDisabledState: function(value) {
        this.callBase.apply(this, arguments);
        var $input = this._input();
        $input.prop("disabled", value)
    },
    _toggleTabIndex: function() {
        var $input = this._input();
        var disabled = this.option("disabled");
        var focusStateEnabled = this.option("focusStateEnabled");
        if (disabled || !focusStateEnabled) {
            $input.attr("tabIndex", -1)
        } else {
            $input.removeAttr("tabIndex")
        }
    },
    _toggleReadOnlyState: function() {
        this._input().prop("readOnly", this._readOnlyPropValue());
        this.callBase()
    },
    _readOnlyPropValue: function() {
        return this.option("readOnly")
    },
    _toggleSpellcheckState: function() {
        this._input().prop("spellcheck", this.option("spellcheck"))
    },
    _unobserveLabelContainerResize: function() {
        if (this._labelContainerElement) {
            resizeObserverSingleton.unobserve(this._labelContainerElement);
            this._labelContainerElement = null
        }
    },
    _getLabelContainer: function() {
        return this._input()
    },
    _getLabelContainerWidth: function() {
        return getWidth(this._getLabelContainer())
    },
    _getLabelBeforeWidth: function() {
        var buttonsBeforeWidth = this._$beforeButtonsContainer && getWidth(this._$beforeButtonsContainer);
        return null !== buttonsBeforeWidth && void 0 !== buttonsBeforeWidth ? buttonsBeforeWidth : 0
    },
    _updateLabelWidth: function() {
        this._label.updateBeforeWidth(this._getLabelBeforeWidth());
        this._label.updateMaxWidth(this._getLabelContainerWidth())
    },
    _setLabelContainerAria: function() {
        this.setAria("labelledby", this._label.getId(), this._getLabelContainer())
    },
    _renderLabel: function() {
        this._unobserveLabelContainerResize();
        this._labelContainerElement = $(this._getLabelContainer()).get(0);
        var {
            label: label,
            labelMode: labelMode,
            labelMark: labelMark
        } = this.option();
        var labelConfig = {
            $editor: this.$element(),
            text: label,
            mark: labelMark,
            mode: labelMode,
            containsButtonsBefore: !!this._$beforeButtonsContainer,
            containerWidth: this._getLabelContainerWidth(),
            beforeWidth: this._getLabelBeforeWidth()
        };
        this._label = new TextEditorLabelCreator(labelConfig);
        this._setLabelContainerAria();
        if (this._labelContainerElement) {
            resizeObserverSingleton.observe(this._labelContainerElement, this._updateLabelWidth.bind(this))
        }
    },
    _renderPlaceholder: function() {
        this._renderPlaceholderMarkup();
        this._attachPlaceholderEvents()
    },
    _renderPlaceholderMarkup: function() {
        if (this._$placeholder) {
            this._$placeholder.remove();
            this._$placeholder = null
        }
        var $input = this._input();
        var placeholderText = this.option("placeholder");
        var $placeholder = this._$placeholder = $("<div>").attr("data-dx_placeholder", placeholderText);
        $placeholder.insertAfter($input);
        $placeholder.addClass(TEXTEDITOR_PLACEHOLDER_CLASS)
    },
    _attachPlaceholderEvents: function() {
        var startEvent = addNamespace(pointerEvents.up, this.NAME);
        eventsEngine.on(this._$placeholder, startEvent, () => {
            eventsEngine.trigger(this._input(), "focus")
        });
        this._toggleEmptinessEventHandler()
    },
    _placeholder: function() {
        return this._$placeholder || $()
    },
    _clearValueHandler: function(e) {
        var $input = this._input();
        e.stopPropagation();
        this._saveValueChangeEvent(e);
        this._clearValue();
        !this._isFocused() && eventsEngine.trigger($input, "focus");
        eventsEngine.trigger($input, "input")
    },
    _clearValue: function() {
        this.reset()
    },
    _renderEvents: function() {
        var $input = this._input();
        each(EVENTS_LIST, (_, event) => {
            if (this.hasActionSubscription("on" + event)) {
                var action = this._createActionByOption("on" + event, {
                    excludeValidators: ["readOnly"]
                });
                eventsEngine.on($input, addNamespace(event.toLowerCase(), this.NAME), e => {
                    if (this._disposed) {
                        return
                    }
                    action({
                        event: e
                    })
                })
            }
        })
    },
    _refreshEvents: function() {
        var $input = this._input();
        each(EVENTS_LIST, (_, event) => {
            eventsEngine.off($input, addNamespace(event.toLowerCase(), this.NAME))
        });
        this._renderEvents()
    },
    _keyPressHandler: function() {
        this.option("text", this._input().val())
    },
    _keyDownHandler: function(e) {
        var $input = this._input();
        var isCtrlEnter = e.ctrlKey && "enter" === normalizeKeyName(e);
        var isNewValue = $input.val() !== this.option("value");
        if (isCtrlEnter && isNewValue) {
            eventsEngine.trigger($input, "change")
        }
    },
    _renderValueChangeEvent: function() {
        var keyPressEvent = addNamespace(this._renderValueEventName(), "".concat(this.NAME, "TextChange"));
        var valueChangeEvent = addNamespace(this.option("valueChangeEvent"), "".concat(this.NAME, "ValueChange"));
        var keyDownEvent = addNamespace("keydown", "".concat(this.NAME, "TextChange"));
        var $input = this._input();
        eventsEngine.on($input, keyPressEvent, this._keyPressHandler.bind(this));
        eventsEngine.on($input, valueChangeEvent, this._valueChangeEventHandler.bind(this));
        eventsEngine.on($input, keyDownEvent, this._keyDownHandler.bind(this))
    },
    _cleanValueChangeEvent: function() {
        var valueChangeNamespace = ".".concat(this.NAME, "ValueChange");
        var textChangeNamespace = ".".concat(this.NAME, "TextChange");
        eventsEngine.off(this._input(), valueChangeNamespace);
        eventsEngine.off(this._input(), textChangeNamespace)
    },
    _refreshValueChangeEvent: function() {
        this._cleanValueChangeEvent();
        this._renderValueChangeEvent()
    },
    _renderValueEventName: function() {
        return "input change keypress"
    },
    _focusTarget: function() {
        return this._input()
    },
    _focusEventTarget: function() {
        return this.element()
    },
    _isInput: function(element) {
        return element === this._input().get(0)
    },
    _preventNestedFocusEvent: function(event) {
        if (event.isDefaultPrevented()) {
            return true
        }
        var shouldPrevent = this._isNestedTarget(event.relatedTarget);
        if ("focusin" === event.type) {
            shouldPrevent = shouldPrevent && this._isNestedTarget(event.target) && !this._isInput(event.target)
        } else if (!shouldPrevent) {
            this._toggleFocusClass(false, this.$element())
        }
        shouldPrevent && event.preventDefault();
        return shouldPrevent
    },
    _isNestedTarget: function(target) {
        return !!this.$element().find(target).length
    },
    _focusClassTarget: function() {
        return this.$element()
    },
    _focusInHandler: function(event) {
        this._preventNestedFocusEvent(event);
        this.callBase.apply(this, arguments)
    },
    _focusOutHandler: function(event) {
        this._preventNestedFocusEvent(event);
        this.callBase.apply(this, arguments)
    },
    _toggleFocusClass: function(isFocused, $element) {
        this.callBase(isFocused, this._focusClassTarget($element))
    },
    _hasFocusClass: function(element) {
        return this.callBase($(element || this.$element()))
    },
    _renderEmptinessEvent: function() {
        var $input = this._input();
        eventsEngine.on($input, "input blur", this._toggleEmptinessEventHandler.bind(this))
    },
    _toggleEmptinessEventHandler: function() {
        var text = this._input().val();
        var isEmpty = ("" === text || null === text) && this._isValueValid();
        this._toggleEmptiness(isEmpty)
    },
    _valueChangeEventHandler: function(e, formattedValue) {
        if (this.option("readOnly")) {
            return
        }
        this._saveValueChangeEvent(e);
        this.option("value", arguments.length > 1 ? formattedValue : this._input().val());
        this._saveValueChangeEvent(void 0)
    },
    _renderEnterKeyAction: function() {
        this._enterKeyAction = this._createActionByOption("onEnterKey", {
            excludeValidators: ["readOnly"]
        });
        eventsEngine.off(this._input(), "keyup.onEnterKey.dxTextEditor");
        eventsEngine.on(this._input(), "keyup.onEnterKey.dxTextEditor", this._enterKeyHandlerUp.bind(this))
    },
    _enterKeyHandlerUp: function(e) {
        if (this._disposed) {
            return
        }
        if ("enter" === normalizeKeyName(e)) {
            this._enterKeyAction({
                event: e
            })
        }
    },
    _updateValue: function() {
        this._options.silent("text", null);
        this._renderValue()
    },
    _dispose: function() {
        this._enterKeyAction = void 0;
        this.callBase()
    },
    _getSubmitElement: function() {
        return this._input()
    },
    _hasActiveElement: function() {
        return this._input().is(domAdapter.getActiveElement())
    },
    _optionChanged: function(args) {
        var {
            name: name,
            fullName: fullName,
            value: value
        } = args;
        if (inArray(name.replace("on", ""), EVENTS_LIST) > -1) {
            this._refreshEvents();
            return
        }
        switch (name) {
            case "valueChangeEvent":
                this._refreshValueChangeEvent();
                this._refreshFocusEvent();
                this._refreshEvents();
                break;
            case "onValueChanged":
                this._createValueChangeAction();
                break;
            case "focusStateEnabled":
                this.callBase(args);
                this._toggleTabIndex();
                break;
            case "spellcheck":
                this._toggleSpellcheckState();
                break;
            case "mode":
                this._renderInputType();
                break;
            case "onEnterKey":
                this._renderEnterKeyAction();
                break;
            case "placeholder":
                this._renderPlaceholder();
                break;
            case "label":
                this._label.updateText(value);
                this._setLabelContainerAria();
                break;
            case "labelMark":
                this._label.updateMark(value);
                break;
            case "labelMode":
                this._label.updateMode(value);
                this._setLabelContainerAria();
                break;
            case "width":
                this.callBase(args);
                this._label.updateMaxWidth(this._getLabelContainerWidth());
                break;
            case "readOnly":
            case "disabled":
                this._updateButtons();
                this.callBase(args);
                break;
            case "showClearButton":
                this._updateButtons(["clear"]);
                break;
            case "text":
                break;
            case "value":
                this._updateValue();
                this.callBase(args);
                break;
            case "inputAttr":
                this._applyInputAttributes(this._input(), this.option(name));
                break;
            case "stylingMode":
                this._renderStylingMode();
                this._updateLabelWidth();
                break;
            case "buttons":
                if (fullName === name) {
                    checkButtonsOptionType(value)
                }
                this._cleanButtonContainers();
                this._renderButtonContainers();
                this._updateButtonsStyling(this.option("stylingMode"));
                this._updateLabelWidth();
                this._label.updateContainsButtonsBefore(!!this._$beforeButtonsContainer);
                break;
            case "visible":
                this.callBase(args);
                if (value && this.option("buttons")) {
                    this._cleanButtonContainers();
                    this._renderButtonContainers();
                    this._updateButtonsStyling(this.option("stylingMode"))
                }
                break;
            case "displayValueFormatter":
                this._invalidate();
                break;
            case "showValidationMark":
                break;
            default:
                this.callBase(args)
        }
    },
    _renderInputType: function() {
        this._setInputType(this.option("mode"))
    },
    _setInputType: function(type) {
        var input = this._input();
        if ("search" === type) {
            type = "text"
        }
        try {
            input.prop("type", type)
        } catch (e) {
            input.prop("type", "text")
        }
    },
    getButton(name) {
        return this._buttonCollection.getButton(name)
    },
    focus: function() {
        eventsEngine.trigger(this._input(), "focus")
    },
    reset: function() {
        if (this._showValidMark) {
            this._showValidMark = false;
            this._renderValidationState()
        }
        var defaultOptions = this._getDefaultOptions();
        if (this.option("value") === defaultOptions.value) {
            this._options.silent("text", "");
            this._renderValue()
        } else {
            this.option("value", defaultOptions.value)
        }
    },
    on: function(eventName, eventHandler) {
        var result = this.callBase(eventName, eventHandler);
        var event = eventName.charAt(0).toUpperCase() + eventName.substr(1);
        if (EVENTS_LIST.indexOf(event) >= 0) {
            this._refreshEvents()
        }
        return result
    }
});
export default TextEditorBase;
