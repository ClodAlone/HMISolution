/**
 * DevExtreme (esm/ui/text_box/ui.text_editor.mask.strategy.default.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import BaseMaskStrategy from "./ui.text_editor.mask.strategy.base";
import {
    getChar
} from "../../events/utils/index";
import Promise from "../../core/polyfills/promise";
var BACKSPACE_INPUT_TYPE = "deleteContentBackward";
var EMPTY_CHAR = " ";
class DefaultMaskStrategy extends BaseMaskStrategy {
    _getStrategyName() {
        return "default"
    }
    getHandleEventNames() {
        return [...super.getHandleEventNames(), "keyPress"]
    }
    _keyPressHandler(event) {
        if (this._keyPressHandled) {
            return
        }
        this._keyPressHandled = true;
        if (this.editor._isControlKeyFired(event)) {
            return
        }
        var {
            editor: editor
        } = this;
        editor._maskKeyHandler(event, () => editor._handleKey(getChar(event)))
    }
    _inputHandler(event) {
        if (this._backspaceInputHandled(event.originalEvent && event.originalEvent.inputType)) {
            this._handleBackspaceInput(event)
        }
        if (event.originalEvent) {
            this._autoFillHandler(event)
        }
        if (this._keyPressHandled) {
            return
        }
        this._keyPressHandled = true;
        var inputValue = this.editorInput().val();
        var caret = this.editorCaret();
        if (!caret.end) {
            return
        }
        caret.start = caret.end - 1;
        var oldValue = inputValue.substring(0, caret.start) + inputValue.substring(caret.end);
        var char = inputValue[caret.start];
        var {
            editor: editor
        } = this;
        this.editorInput().val(oldValue);
        editor._caret({
            start: caret.start,
            end: caret.start
        });
        editor._maskKeyHandler(event, () => editor._handleKey(char))
    }
    _backspaceHandler(event) {
        var {
            editor: editor
        } = this;
        this._keyPressHandled = true;
        var afterBackspaceHandler = (needAdjustCaret, callBack) => {
            if (needAdjustCaret) {
                editor._direction(this.DIRECTION.FORWARD);
                editor._adjustCaret()
            }
            var currentCaret = this.editorCaret();
            return new Promise(resolve => {
                clearTimeout(this._backspaceHandlerTimeout);
                this._backspaceHandlerTimeout = setTimeout((function() {
                    callBack(currentCaret);
                    resolve()
                }))
            })
        };
        editor._maskKeyHandler(event, () => {
            if (editor._hasSelection()) {
                return afterBackspaceHandler(true, currentCaret => {
                    editor._displayMask(currentCaret);
                    editor._maskRulesChain.reset()
                })
            }
            if (editor._tryMoveCaretBackward()) {
                return afterBackspaceHandler(false, currentCaret => {
                    this.editorCaret(currentCaret)
                })
            }
            editor._handleKey(EMPTY_CHAR, this.DIRECTION.BACKWARD);
            return afterBackspaceHandler(true, currentCaret => {
                editor._displayMask(currentCaret);
                editor._maskRulesChain.reset()
            })
        })
    }
    _backspaceInputHandled(inputType) {
        return inputType === BACKSPACE_INPUT_TYPE && !this._keyPressHandled
    }
    _handleBackspaceInput(event) {
        var {
            start: start,
            end: end
        } = this.editorCaret();
        this.editorCaret({
            start: start + 1,
            end: end + 1
        });
        this._backspaceHandler(event)
    }
}
export default DefaultMaskStrategy;
