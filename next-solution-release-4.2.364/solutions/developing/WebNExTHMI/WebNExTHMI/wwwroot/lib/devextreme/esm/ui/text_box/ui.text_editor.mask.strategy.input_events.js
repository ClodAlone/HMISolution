/**
 * DevExtreme (esm/ui/text_box/ui.text_editor.mask.strategy.input_events.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import BaseMaskStrategy from "./ui.text_editor.mask.strategy.base";
var DELETE_INPUT_TYPE = "deleteContentBackward";
class InputEventsMaskStrategy extends BaseMaskStrategy {
    _getStrategyName() {
        return "inputEvents"
    }
    getHandleEventNames() {
        return [...super.getHandleEventNames(), "beforeInput"]
    }
    _beforeInputHandler() {
        this._prevCaret = this.editorCaret()
    }
    _inputHandler(_ref) {
        var {
            originalEvent: originalEvent
        } = _ref;
        if (!originalEvent) {
            return
        }
        var {
            inputType: inputType,
            data: data
        } = originalEvent;
        var currentCaret = this.editorCaret();
        if (inputType === DELETE_INPUT_TYPE) {
            var length = this._prevCaret.end - this._prevCaret.start || 1;
            this.editor.setBackwardDirection();
            this._updateEditorMask({
                start: currentCaret.start,
                length: length,
                text: this._getEmptyString(length)
            })
        } else {
            var _this$_prevCaret, _this$_prevCaret2, _this$_prevCaret3;
            if (!currentCaret.end) {
                return
            }
            this._autoFillHandler(originalEvent);
            this.editorCaret(currentCaret);
            var _length = (null === (_this$_prevCaret = this._prevCaret) || void 0 === _this$_prevCaret ? void 0 : _this$_prevCaret.end) - (null === (_this$_prevCaret2 = this._prevCaret) || void 0 === _this$_prevCaret2 ? void 0 : _this$_prevCaret2.start);
            var newData = data + (_length ? this._getEmptyString(_length - data.length) : "");
            this.editor.setForwardDirection();
            var hasValidChars = this._updateEditorMask({
                start: null === (_this$_prevCaret3 = this._prevCaret) || void 0 === _this$_prevCaret3 ? void 0 : _this$_prevCaret3.start,
                length: _length || newData.length,
                text: newData
            });
            if (!hasValidChars) {
                this.editorCaret(this._prevCaret)
            }
        }
    }
    _getEmptyString(length) {
        return Array(length + 1).join(" ")
    }
    _updateEditorMask(args) {
        var textLength = args.text.length;
        var updatedCharsCount = this.editor._handleChain(args);
        if (this.editor.isForwardDirection()) {
            var {
                start: start,
                end: end
            } = this.editorCaret();
            var correction = updatedCharsCount - textLength;
            if (start <= updatedCharsCount && updatedCharsCount > 1) {
                this.editorCaret({
                    start: start + correction,
                    end: end + correction
                })
            }
            this.editor.isForwardDirection() && this.editor._adjustCaret()
        }
        this.editor._displayMask();
        return !!updatedCharsCount
    }
}
export default InputEventsMaskStrategy;
