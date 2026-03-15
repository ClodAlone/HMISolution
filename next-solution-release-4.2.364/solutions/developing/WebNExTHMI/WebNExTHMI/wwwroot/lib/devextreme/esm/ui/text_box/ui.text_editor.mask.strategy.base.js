/**
 * DevExtreme (esm/ui/text_box/ui.text_editor.mask.strategy.base.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import EventsEngine from "../../events/core/events_engine";
import {
    addNamespace
} from "../../events/utils/index";
import browser from "../../core/utils/browser";
import {
    inArray
} from "../../core/utils/array";
import {
    clipboardText as getClipboardText
} from "../../core/utils/dom";
var MASK_EVENT_NAMESPACE = "dxMask";
var BLUR_EVENT = "blur beforedeactivate";
var EMPTY_CHAR = " ";
export default class BaseMaskStrategy {
    constructor(editor) {
        this.editor = editor;
        this.DIRECTION = {
            FORWARD: "forward",
            BACKWARD: "backward"
        };
        this.NAME = this._getStrategyName()
    }
    _getStrategyName() {
        return "base"
    }
    editorOption() {
        return this.editor.option(...arguments)
    }
    editorInput() {
        return this.editor._input()
    }
    editorCaret(newCaret) {
        if (!newCaret) {
            return this.editor._caret()
        }
        this.editor._caret(newCaret)
    }
    getHandler(handlerName) {
        var handler = this["_".concat(handlerName, "Handler")] || function() {};
        return handler.bind(this)
    }
    attachEvents() {
        var $input = this.editorInput();
        this.getHandleEventNames().forEach(eventName => {
            var subscriptionName = addNamespace(eventName.toLowerCase(), MASK_EVENT_NAMESPACE);
            EventsEngine.on($input, subscriptionName, this.getEventHandler(eventName))
        });
        this._attachChangeEventHandlers()
    }
    getHandleEventNames() {
        return ["focusIn", "focusOut", "keyDown", "input", "paste", "cut", "drop"]
    }
    getEventHandler(eventName) {
        return this["_".concat(eventName, "Handler")].bind(this)
    }
    detachEvents() {
        EventsEngine.off(this.editorInput(), ".".concat(MASK_EVENT_NAMESPACE))
    }
    _attachChangeEventHandlers() {
        if (-1 === inArray("change", this.editorOption("valueChangeEvent").split(" "))) {
            return
        }
        EventsEngine.on(this.editorInput(), addNamespace(BLUR_EVENT, MASK_EVENT_NAMESPACE), function(e) {
            this._suppressCaretChanging(this._changeHandler, [e]);
            this._changeHandler(e)
        }.bind(this.editor))
    }
    _focusInHandler() {
        this.editor._showMaskPlaceholder();
        this.editor._direction(this.DIRECTION.FORWARD);
        if (!this.editor._isValueEmpty() && this.editorOption("isValid")) {
            this.editor._adjustCaret()
        } else {
            var caret = this.editor._maskRulesChain.first();
            this._caretTimeout = setTimeout(function() {
                this._caret({
                    start: caret,
                    end: caret
                })
            }.bind(this.editor), 0)
        }
    }
    _focusOutHandler(event) {
        this.editor._changeHandler(event);
        if ("onFocus" === this.editorOption("showMaskMode") && this.editor._isValueEmpty()) {
            this.editorOption("text", "");
            this.editor._renderDisplayText("")
        }
    }
    _cutHandler(event) {
        var caret = this.editorCaret();
        var selectedText = this.editorInput().val().substring(caret.start, caret.end);
        this.editor._maskKeyHandler(event, () => getClipboardText(event, selectedText))
    }
    _dropHandler() {
        this._clearDragTimer();
        this._dragTimer = setTimeout(function() {
            this.option("value", this._convertToValue(this._input().val()))
        }.bind(this.editor))
    }
    _clearDragTimer() {
        clearTimeout(this._dragTimer)
    }
    _keyDownHandler() {
        this._keyPressHandled = false
    }
    _pasteHandler(event) {
        var {
            editor: editor
        } = this;
        this._keyPressHandled = true;
        var caret = this.editorCaret();
        editor._maskKeyHandler(event, () => {
            var pastedText = getClipboardText(event);
            var restText = editor._maskRulesChain.text().substring(caret.end);
            var accepted = editor._handleChain({
                text: pastedText,
                start: caret.start,
                length: pastedText.length
            });
            var newCaret = caret.start + accepted;
            editor._handleChain({
                text: restText,
                start: newCaret,
                length: restText.length
            });
            editor._caret({
                start: newCaret,
                end: newCaret
            })
        })
    }
    _autoFillHandler(event) {
        var {
            editor: editor
        } = this;
        var inputVal = this.editorInput().val();
        this._inputHandlerTimer = setTimeout(() => {
            this._keyPressHandled = true;
            if (this._isAutoFill()) {
                this._keyPressHandled = true;
                editor._maskKeyHandler(event, () => {
                    editor._handleChain({
                        text: inputVal,
                        start: 0,
                        length: inputVal.length
                    })
                });
                editor._validateMask()
            }
        })
    }
    _isAutoFill() {
        var $input = this.editor._input();
        var result = false;
        if (browser.webkit) {
            var input = $input.get(0);
            result = input && input.matches(":-webkit-autofill")
        }
        return result
    }
    runWithoutEventProcessing(action) {
        var keyPressHandled = this._keyPressHandled;
        this._keyPressHandled = true;
        action();
        this._keyPressHandled = keyPressHandled
    }
    _backspaceHandler() {}
    _delHandler(event) {
        var {
            editor: editor
        } = this;
        this._keyPressHandled = true;
        editor._maskKeyHandler(event, () => !editor._hasSelection() && editor._handleKey(EMPTY_CHAR))
    }
    clean() {
        this._clearDragTimer();
        clearTimeout(this._backspaceHandlerTimeout);
        clearTimeout(this._caretTimeout);
        clearTimeout(this._inputHandlerTimer)
    }
}
