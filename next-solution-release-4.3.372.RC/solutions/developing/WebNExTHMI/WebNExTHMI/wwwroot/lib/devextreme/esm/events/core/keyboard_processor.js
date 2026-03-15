/**
 * DevExtreme (esm/events/core/keyboard_processor.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import $ from "../../core/renderer";
import eventsEngine from "../../events/core/events_engine";
import Class from "../../core/class";
import {
    inArray
} from "../../core/utils/array";
import {
    addNamespace,
    normalizeKeyName
} from "../../events/utils/index";
var COMPOSITION_START_EVENT = "compositionstart";
var COMPOSITION_END_EVENT = "compositionend";
var KEYDOWN_EVENT = "keydown";
var NAMESPACE = "KeyboardProcessor";
var createKeyDownOptions = e => ({
    keyName: normalizeKeyName(e),
    key: e.key,
    code: e.code,
    ctrl: e.ctrlKey,
    location: e.location,
    metaKey: e.metaKey,
    shift: e.shiftKey,
    alt: e.altKey,
    which: e.which,
    originalEvent: e
});
var KeyboardProcessor = Class.inherit({
    _keydown: addNamespace(KEYDOWN_EVENT, NAMESPACE),
    _compositionStart: addNamespace(COMPOSITION_START_EVENT, NAMESPACE),
    _compositionEnd: addNamespace(COMPOSITION_END_EVENT, NAMESPACE),
    ctor: function(options) {
        options = options || {};
        if (options.element) {
            this._element = $(options.element)
        }
        if (options.focusTarget) {
            this._focusTarget = options.focusTarget
        }
        this._handler = options.handler;
        if (this._element) {
            this._processFunction = e => {
                var isNotFocusTarget = this._focusTarget && this._focusTarget !== e.target && inArray(e.target, $(this._focusTarget)) < 0;
                var shouldSkipProcessing = this._isComposingJustFinished && 229 === e.which || this._isComposing || isNotFocusTarget;
                this._isComposingJustFinished = false;
                if (!shouldSkipProcessing) {
                    this.process(e)
                }
            };
            this._toggleProcessingWithContext = this.toggleProcessing.bind(this);
            eventsEngine.on(this._element, this._keydown, this._processFunction);
            eventsEngine.on(this._element, this._compositionStart, this._toggleProcessingWithContext);
            eventsEngine.on(this._element, this._compositionEnd, this._toggleProcessingWithContext)
        }
    },
    dispose: function() {
        if (this._element) {
            eventsEngine.off(this._element, this._keydown, this._processFunction);
            eventsEngine.off(this._element, this._compositionStart, this._toggleProcessingWithContext);
            eventsEngine.off(this._element, this._compositionEnd, this._toggleProcessingWithContext)
        }
        this._element = void 0;
        this._handler = void 0
    },
    process: function(e) {
        this._handler(createKeyDownOptions(e))
    },
    toggleProcessing: function(_ref) {
        var {
            type: type
        } = _ref;
        this._isComposing = type === COMPOSITION_START_EVENT;
        this._isComposingJustFinished = !this._isComposing
    }
});
KeyboardProcessor.createKeyDownOptions = createKeyDownOptions;
export default KeyboardProcessor;
