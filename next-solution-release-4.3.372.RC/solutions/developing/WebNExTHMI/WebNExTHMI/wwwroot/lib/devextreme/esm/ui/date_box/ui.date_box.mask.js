/**
 * DevExtreme (esm/ui/date_box/ui.date_box.mask.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import {
    addNamespace,
    normalizeKeyName,
    isCommandKeyPressed
} from "../../events/utils/index";
import {
    isFunction,
    isString,
    isDate,
    isDefined
} from "../../core/utils/type";
import {
    clipboardText
} from "../../core/utils/dom";
import {
    extend
} from "../../core/utils/extend";
import {
    fitIntoRange,
    inRange,
    sign
} from "../../core/utils/math";
import eventsEngine from "../../events/core/events_engine";
import {
    getDatePartIndexByPosition,
    renderDateParts
} from "./ui.date_box.mask.parts";
import dateLocalization from "../../localization/date";
import {
    getRegExpInfo
} from "../../localization/ldml/date.parser";
import {
    getFormat
} from "../../localization/ldml/date.format";
import DateBoxBase from "./ui.date_box.base";
import numberLocalization from "../../localization/number";
import devices from "../../core/devices";
import browser from "../../core/utils/browser";
var MASK_EVENT_NAMESPACE = "dateBoxMask";
var FORWARD = 1;
var BACKWARD = -1;
var DateBoxMask = DateBoxBase.inherit({
    _supportedKeys(e) {
        var originalHandlers = this.callBase(e);
        var callOriginalHandler = e => {
            var originalHandler = originalHandlers[normalizeKeyName(e)];
            return originalHandler && originalHandler.apply(this, [e])
        };
        var applyHandler = (e, maskHandler) => {
            if (this._shouldUseOriginalHandler(e)) {
                return callOriginalHandler.apply(this, [e])
            } else {
                return maskHandler.apply(this, [e])
            }
        };
        return extend({}, originalHandlers, {
            del: e => applyHandler(e, event => {
                this._revertPart(FORWARD);
                this._isAllSelected() || event.preventDefault()
            }),
            backspace: e => applyHandler(e, event => {
                this._revertPart(BACKWARD);
                this._isAllSelected() || event.preventDefault()
            }),
            home: e => applyHandler(e, event => {
                this._selectFirstPart();
                event.preventDefault()
            }),
            end: e => applyHandler(e, event => {
                this._selectLastPart();
                event.preventDefault()
            }),
            escape: e => applyHandler(e, event => {
                this._revertChanges(event)
            }),
            enter: e => applyHandler(e, event => {
                this._enterHandler(event)
            }),
            leftArrow: e => applyHandler(e, event => {
                this._selectNextPart(BACKWARD);
                event.preventDefault()
            }),
            rightArrow: e => applyHandler(e, event => {
                this._selectNextPart(FORWARD);
                event.preventDefault()
            }),
            upArrow: e => applyHandler(e, event => {
                this._upDownArrowHandler(FORWARD);
                event.preventDefault()
            }),
            downArrow: e => applyHandler(e, event => {
                this._upDownArrowHandler(BACKWARD);
                event.preventDefault()
            })
        })
    },
    _shouldUseOriginalHandler(e) {
        var isNotDeletingInCalendar = this.option("opened") && e && -1 === ["backspace", "del"].indexOf(normalizeKeyName(e));
        return !this._useMaskBehavior() || isNotDeletingInCalendar || e && e.altKey
    },
    _upDownArrowHandler(step) {
        this._setNewDateIfEmpty();
        var originalValue = this._getActivePartValue(this._initialMaskValue);
        var currentValue = this._getActivePartValue();
        var delta = currentValue - originalValue;
        this._loadMaskValue(this._initialMaskValue);
        this._partIncrease(delta + step, true)
    },
    _getDefaultOptions() {
        return extend(this.callBase(), {
            useMaskBehavior: false,
            emptyDateValue: new Date(2e3, 0, 1, 0, 0, 0)
        })
    },
    _isSingleCharKey(_ref) {
        var {
            originalEvent: originalEvent,
            alt: alt
        } = _ref;
        var key = originalEvent.data || originalEvent.key;
        return "string" === typeof key && 1 === key.length && !alt && !isCommandKeyPressed(originalEvent)
    },
    _isSingleDigitKey(e) {
        var _e$originalEvent;
        var data = null === (_e$originalEvent = e.originalEvent) || void 0 === _e$originalEvent ? void 0 : _e$originalEvent.data;
        return 1 === (null === data || void 0 === data ? void 0 : data.length) && parseInt(data, 10)
    },
    _useBeforeInputEvent: function() {
        var device = devices.real();
        return device.android && device.version[0] > 4
    },
    _keyInputHandler(e, key) {
        var oldInputValue = this._input().val();
        this._processInputKey(key);
        e.preventDefault();
        var isValueChanged = oldInputValue !== this._input().val();
        isValueChanged && eventsEngine.trigger(this._input(), "input")
    },
    _keyboardHandler(e) {
        var key = e.originalEvent.key;
        var result = this.callBase(e);
        if (!this._useMaskBehavior() || this._useBeforeInputEvent()) {
            return result
        }
        if (browser.chrome && "Process" === e.key && 0 === e.code.indexOf("Digit")) {
            key = e.code.replace("Digit", "");
            this._processInputKey(key);
            this._maskInputHandler = () => {
                this._renderSelectedPart()
            }
        } else if (this._isSingleCharKey(e)) {
            this._keyInputHandler(e.originalEvent, key)
        }
        return result
    },
    _maskBeforeInputHandler(e) {
        this._maskInputHandler = null;
        var {
            inputType: inputType
        } = e.originalEvent;
        if ("insertCompositionText" === inputType) {
            this._maskInputHandler = () => {
                this._renderSelectedPart()
            }
        }
        var isBackwardDeletion = "deleteContentBackward" === inputType;
        var isForwardDeletion = "deleteContentForward" === inputType;
        if (isBackwardDeletion || isForwardDeletion) {
            var direction = isBackwardDeletion ? BACKWARD : FORWARD;
            this._maskInputHandler = () => {
                this._revertPart();
                this._selectNextPart(direction)
            }
        }
        if (!this._useMaskBehavior() || !this._isSingleCharKey(e)) {
            return
        }
        var key = e.originalEvent.data;
        this._keyInputHandler(e, key);
        return true
    },
    _keyPressHandler(e) {
        var {
            originalEvent: event
        } = e;
        if ("insertCompositionText" === (null === event || void 0 === event ? void 0 : event.inputType) && this._isSingleDigitKey(e)) {
            this._processInputKey(event.data);
            this._renderDisplayText(this._getDisplayedText(this._maskValue));
            this._selectNextPart()
        }
        this.callBase(e);
        if (this._maskInputHandler) {
            this._maskInputHandler();
            this._maskInputHandler = null
        }
    },
    _processInputKey(key) {
        if (this._isAllSelected()) {
            this._activePartIndex = 0
        }
        this._setNewDateIfEmpty();
        if (isNaN(parseInt(key))) {
            this._searchString(key)
        } else {
            this._searchNumber(key)
        }
    },
    _isAllSelected() {
        var caret = this._caret();
        return caret.end - caret.start === this.option("text").length
    },
    _getFormatPattern() {
        if (this._formatPattern) {
            return this._formatPattern
        }
        var format = this._strategy.getDisplayFormat(this.option("displayFormat"));
        var isLDMLPattern = isString(format) && !dateLocalization._getPatternByFormat(format);
        if (isLDMLPattern) {
            this._formatPattern = format
        } else {
            this._formatPattern = getFormat((function(value) {
                return dateLocalization.format(value, format)
            }))
        }
        return this._formatPattern
    },
    _setNewDateIfEmpty() {
        if (!this._maskValue) {
            var value = "time" === this.option("type") ? new Date(null) : new Date;
            this._maskValue = value;
            this._initialMaskValue = value;
            this._renderDateParts()
        }
    },
    _partLimitsReached(max) {
        var maxLimitLength = String(max).length;
        var formatLength = this._getActivePartProp("pattern").length;
        var isShortFormat = 1 === formatLength;
        var maxSearchLength = isShortFormat ? maxLimitLength : Math.min(formatLength, maxLimitLength);
        var isLengthExceeded = this._searchValue.length === maxSearchLength;
        var isValueOverflowed = parseInt(this._searchValue + "0") > max;
        return isLengthExceeded || isValueOverflowed
    },
    _searchNumber(char) {
        var {
            max: max
        } = this._getActivePartLimits();
        var maxLimitLength = String(max).length;
        this._searchValue = (this._searchValue + char).substr(-maxLimitLength);
        if (isNaN(this._searchValue)) {
            this._searchValue = char
        }
        this._setActivePartValue(this._searchValue);
        if (this._partLimitsReached(max)) {
            this._selectNextPart(FORWARD)
        }
    },
    _searchString(char) {
        if (!isNaN(parseInt(this._getActivePartProp("text")))) {
            return
        }
        var limits = this._getActivePartProp("limits")(this._maskValue);
        var startString = this._searchValue + char.toLowerCase();
        var endLimit = limits.max - limits.min;
        for (var i = 0; i <= endLimit; i++) {
            this._loadMaskValue(this._initialMaskValue);
            this._partIncrease(i + 1);
            if (0 === this._getActivePartProp("text").toLowerCase().indexOf(startString)) {
                this._searchValue = startString;
                return
            }
        }
        this._setNewDateIfEmpty();
        if (this._searchValue) {
            this._clearSearchValue();
            this._searchString(char)
        }
    },
    _clearSearchValue() {
        this._searchValue = ""
    },
    _revertPart: function(direction) {
        if (!this._isAllSelected()) {
            var actual = this._getActivePartValue(this.option("emptyDateValue"));
            this._setActivePartValue(actual);
            this._selectNextPart(direction)
        }
        this._clearSearchValue()
    },
    _useMaskBehavior() {
        return this.option("useMaskBehavior") && "text" === this.option("mode")
    },
    _prepareRegExpInfo() {
        this._regExpInfo = getRegExpInfo(this._getFormatPattern(), dateLocalization);
        var regexp = this._regExpInfo.regexp;
        var source = regexp.source;
        var flags = regexp.flags;
        var quantifierRegexp = new RegExp(/(\{[0-9]+,?[0-9]*\})/);
        var convertedSource = source.split(quantifierRegexp).map(sourcePart => quantifierRegexp.test(sourcePart) ? sourcePart : numberLocalization.convertDigits(sourcePart, false)).join("");
        this._regExpInfo.regexp = new RegExp(convertedSource, flags)
    },
    _initMaskState() {
        this._activePartIndex = 0;
        this._formatPattern = null;
        this._prepareRegExpInfo();
        this._loadMaskValue()
    },
    _renderMask() {
        this.callBase();
        this._detachMaskEvents();
        this._clearMaskState();
        if (this._useMaskBehavior()) {
            this._attachMaskEvents();
            this._initMaskState();
            this._renderDateParts()
        }
    },
    _renderDateParts() {
        if (!this._useMaskBehavior()) {
            return
        }
        var text = this.option("text") || this._getDisplayedText(this._maskValue);
        if (text) {
            this._dateParts = renderDateParts(text, this._regExpInfo);
            if (!this._input().is(":hidden")) {
                this._selectNextPart()
            }
        }
    },
    _detachMaskEvents() {
        eventsEngine.off(this._input(), "." + MASK_EVENT_NAMESPACE)
    },
    _attachMaskEvents() {
        eventsEngine.on(this._input(), addNamespace("dxclick", MASK_EVENT_NAMESPACE), this._maskClickHandler.bind(this));
        eventsEngine.on(this._input(), addNamespace("paste", MASK_EVENT_NAMESPACE), this._maskPasteHandler.bind(this));
        eventsEngine.on(this._input(), addNamespace("drop", MASK_EVENT_NAMESPACE), () => {
            this._renderSelectedPart()
        });
        eventsEngine.on(this._input(), addNamespace("compositionend", MASK_EVENT_NAMESPACE), this._maskCompositionEndHandler.bind(this));
        if (this._useBeforeInputEvent()) {
            eventsEngine.on(this._input(), addNamespace("beforeinput", MASK_EVENT_NAMESPACE), this._maskBeforeInputHandler.bind(this))
        }
    },
    _renderSelectedPart() {
        this._renderDisplayText(this._getDisplayedText(this._maskValue));
        this._selectNextPart()
    },
    _selectLastPart() {
        if (this.option("text")) {
            this._activePartIndex = this._dateParts.length;
            this._selectNextPart(BACKWARD)
        }
    },
    _selectFirstPart() {
        if (this.option("text")) {
            this._activePartIndex = -1;
            this._selectNextPart(FORWARD)
        }
    },
    _onMouseWheel(e) {
        if (this._useMaskBehavior()) {
            this._partIncrease(e.delta > 0 ? FORWARD : BACKWARD, e)
        }
    },
    _selectNextPart() {
        var step = arguments.length > 0 && void 0 !== arguments[0] ? arguments[0] : 0;
        if (!this.option("text") || this._disposed) {
            return
        }
        if (step) {
            this._initialMaskValue = new Date(this._maskValue)
        }
        var index = fitIntoRange(this._activePartIndex + step, 0, this._dateParts.length - 1);
        if (this._dateParts[index].isStub) {
            var isBoundaryIndex = 0 === index && step < 0 || index === this._dateParts.length - 1 && step > 0;
            if (!isBoundaryIndex) {
                this._selectNextPart(step >= 0 ? step + 1 : step - 1);
                return
            } else {
                index = this._activePartIndex
            }
        }
        if (this._activePartIndex !== index) {
            this._clearSearchValue()
        }
        this._activePartIndex = index;
        this._caret(this._getActivePartProp("caret"))
    },
    _getRealLimitsPattern() {
        if ("d" === this._getActivePartProp("pattern")[0]) {
            return "dM"
        }
    },
    _getActivePartLimits(lockOtherParts) {
        var limitFunction = this._getActivePartProp("limits");
        return limitFunction(this._maskValue, lockOtherParts && this._getRealLimitsPattern())
    },
    _getActivePartValue(dateValue) {
        dateValue = dateValue || this._maskValue;
        var getter = this._getActivePartProp("getter");
        return isFunction(getter) ? getter(dateValue) : dateValue[getter]()
    },
    _addLeadingZeroes(value) {
        var zeroes = this._searchValue.match(/^0+/);
        var limits = this._getActivePartLimits();
        var maxLimitLength = String(limits.max).length;
        return ((zeroes && zeroes[0] || "") + String(value)).substr(-maxLimitLength)
    },
    _setActivePartValue(value, dateValue) {
        dateValue = dateValue || this._maskValue;
        var setter = this._getActivePartProp("setter");
        var limits = this._getActivePartLimits();
        value = inRange(value, limits.min, limits.max) ? value : value % 10;
        value = this._addLeadingZeroes(fitIntoRange(value, limits.min, limits.max));
        isFunction(setter) ? setter(dateValue, value) : dateValue[setter](value);
        this._renderDisplayText(this._getDisplayedText(dateValue));
        this._renderDateParts()
    },
    _getActivePartProp(property) {
        if (!this._dateParts || !this._dateParts[this._activePartIndex]) {
            return
        }
        return this._dateParts[this._activePartIndex][property]
    },
    _loadMaskValue() {
        var value = arguments.length > 0 && void 0 !== arguments[0] ? arguments[0] : this.dateOption("value");
        this._maskValue = value && new Date(value);
        this._initialMaskValue = value && new Date(value)
    },
    _saveMaskValue() {
        var value = this._maskValue && new Date(this._maskValue);
        if (value && "date" === this.option("type")) {
            value.setHours(0, 0, 0, 0)
        }
        this._initialMaskValue = new Date(value);
        this.dateOption("value", value)
    },
    _revertChanges() {
        this._loadMaskValue();
        this._renderDisplayText(this._getDisplayedText(this._maskValue));
        this._renderDateParts()
    },
    _renderDisplayText(text) {
        this.callBase(text);
        if (this._useMaskBehavior()) {
            this.option("text", text)
        }
    },
    _partIncrease(step, lockOtherParts) {
        this._setNewDateIfEmpty();
        var {
            max: max,
            min: min
        } = this._getActivePartLimits(lockOtherParts);
        var limitDelta = max - min;
        if (1 === limitDelta) {
            limitDelta++
        }
        var newValue = step + this._getActivePartValue();
        if (newValue > max) {
            newValue = this._applyLimits(newValue, {
                limitBase: min,
                limitClosest: max,
                limitDelta: limitDelta
            })
        } else if (newValue < min) {
            newValue = this._applyLimits(newValue, {
                limitBase: max,
                limitClosest: min,
                limitDelta: limitDelta
            })
        }
        this._setActivePartValue(newValue)
    },
    _applyLimits(newValue, _ref2) {
        var {
            limitBase: limitBase,
            limitClosest: limitClosest,
            limitDelta: limitDelta
        } = _ref2;
        var delta = (newValue - limitClosest) % limitDelta;
        return delta ? limitBase + delta - 1 * sign(delta) : limitClosest
    },
    _maskClickHandler() {
        if (this.option("text")) {
            this._activePartIndex = getDatePartIndexByPosition(this._dateParts, this._caret().start);
            if (!this._isAllSelected()) {
                if (isDefined(this._activePartIndex)) {
                    this._caret(this._getActivePartProp("caret"))
                } else {
                    this._selectLastPart()
                }
            }
        }
    },
    _maskCompositionEndHandler(e) {
        this._input().val(this._getDisplayedText(this._maskValue));
        this._selectNextPart();
        this._maskInputHandler = () => {
            this._renderSelectedPart()
        }
    },
    _maskPasteHandler(e) {
        var newText = this._replaceSelectedText(this.option("text"), this._caret(), clipboardText(e));
        var date = dateLocalization.parse(newText, this._getFormatPattern());
        if (date && this._isDateValid(date)) {
            this._maskValue = date;
            this._renderDisplayText(this._getDisplayedText(this._maskValue));
            this._renderDateParts();
            this._selectNextPart()
        }
        e.preventDefault()
    },
    _isDateValid: date => isDate(date) && !isNaN(date),
    _isValueDirty() {
        var value = this.dateOption("value");
        return (this._maskValue && this._maskValue.getTime()) !== (value && value.getTime())
    },
    _fireChangeEvent() {
        this._clearSearchValue();
        if (this._isValueDirty()) {
            eventsEngine.trigger(this._input(), "change")
        }
    },
    _enterHandler(e) {
        this._fireChangeEvent();
        this._selectNextPart(FORWARD);
        e.preventDefault()
    },
    _focusOutHandler(e) {
        var shouldFireChangeEvent = this._useMaskBehavior() && !e.isDefaultPrevented();
        if (shouldFireChangeEvent) {
            this._fireChangeEvent();
            this.callBase(e);
            this._selectFirstPart(e)
        } else {
            this.callBase(e)
        }
    },
    _valueChangeEventHandler(e) {
        var text = this.option("text");
        if (this._useMaskBehavior()) {
            this._saveValueChangeEvent(e);
            if (!text) {
                this._maskValue = null
            } else if (null === this._maskValue) {
                this._loadMaskValue(text)
            }
            this._saveMaskValue()
        } else {
            this.callBase(e)
        }
    },
    _optionChanged(args) {
        switch (args.name) {
            case "useMaskBehavior":
                this._renderMask();
                break;
            case "displayFormat":
            case "mode":
                this.callBase(args);
                this._renderMask();
                break;
            case "value":
                this._loadMaskValue();
                this.callBase(args);
                this._renderDateParts();
                break;
            case "emptyDateValue":
                break;
            default:
                this.callBase(args)
        }
    },
    _clearMaskState() {
        this._clearSearchValue();
        delete this._dateParts;
        delete this._activePartIndex;
        delete this._maskValue
    },
    reset() {
        this.callBase();
        this._clearMaskState();
        this._activePartIndex = 0
    },
    _clean() {
        this.callBase();
        this._detachMaskEvents();
        this._clearMaskState()
    }
});
export default DateBoxMask;
