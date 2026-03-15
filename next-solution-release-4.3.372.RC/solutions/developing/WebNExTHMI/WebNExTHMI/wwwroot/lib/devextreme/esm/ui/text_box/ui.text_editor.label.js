/**
 * DevExtreme (esm/ui/text_box/ui.text_editor.label.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import $ from "../../core/renderer";
import Guid from "../../core/guid";
var TEXTEDITOR_LABEL_CLASS = "dx-texteditor-label";
var TEXTEDITOR_WITH_LABEL_CLASS = "dx-texteditor-with-label";
var TEXTEDITOR_WITH_FLOATING_LABEL_CLASS = "dx-texteditor-with-floating-label";
var TEXTEDITOR_WITH_BEFORE_BUTTONS_CLASS = "dx-texteditor-with-before-buttons";
var LABEL_BEFORE_CLASS = "dx-label-before";
var LABEL_CLASS = "dx-label";
var LABEL_AFTER_CLASS = "dx-label-after";
class TextEditorLabel {
    constructor(_ref) {
        var {
            $editor: $editor,
            text: text,
            mode: mode,
            mark: mark,
            containsButtonsBefore: containsButtonsBefore,
            containerWidth: containerWidth,
            beforeWidth: beforeWidth
        } = _ref;
        this._props = {
            $editor: $editor,
            text: text,
            mode: mode,
            mark: mark,
            containsButtonsBefore: containsButtonsBefore,
            containerWidth: containerWidth,
            beforeWidth: beforeWidth
        };
        this._id = "".concat(TEXTEDITOR_LABEL_CLASS, "-").concat(new Guid);
        this._render();
        this._toggleMarkupVisibility()
    }
    _isVisible() {
        return !!this._props.text && "hidden" !== this._props.mode
    }
    _render() {
        this._$before = $("<div>").addClass(LABEL_BEFORE_CLASS);
        this._$labelSpan = $("<span>");
        this._$label = $("<div>").addClass(LABEL_CLASS).append(this._$labelSpan);
        this._$after = $("<div>").addClass(LABEL_AFTER_CLASS);
        this._$root = $("<div>").addClass(TEXTEDITOR_LABEL_CLASS).attr("id", this._id).append(this._$before).append(this._$label).append(this._$after);
        this._updateMark();
        this._updateText();
        this._updateBeforeWidth();
        this._updateMaxWidth()
    }
    _toggleMarkupVisibility() {
        var visible = this._isVisible();
        this._updateEditorBeforeButtonsClass(visible);
        this._updateEditorLabelClass(visible);
        visible ? this._$root.appendTo(this._props.$editor) : this._$root.detach()
    }
    _updateEditorLabelClass(visible) {
        this._props.$editor.removeClass(TEXTEDITOR_WITH_FLOATING_LABEL_CLASS).removeClass(TEXTEDITOR_WITH_LABEL_CLASS);
        if (visible) {
            var labelClass = "floating" === this._props.mode ? TEXTEDITOR_WITH_FLOATING_LABEL_CLASS : TEXTEDITOR_WITH_LABEL_CLASS;
            this._props.$editor.addClass(labelClass)
        }
    }
    _updateEditorBeforeButtonsClass() {
        var visible = arguments.length > 0 && void 0 !== arguments[0] ? arguments[0] : this._isVisible();
        this._props.$editor.removeClass(TEXTEDITOR_WITH_BEFORE_BUTTONS_CLASS);
        if (visible) {
            var beforeButtonsClass = this._props.containsButtonsBefore ? TEXTEDITOR_WITH_BEFORE_BUTTONS_CLASS : "";
            this._props.$editor.addClass(beforeButtonsClass)
        }
    }
    _updateMark() {
        this._$labelSpan.attr("data-mark", this._props.mark)
    }
    _updateText() {
        this._$labelSpan.text(this._props.text)
    }
    _updateBeforeWidth() {
        this._$before.css({
            width: this._props.beforeWidth
        })
    }
    _updateMaxWidth() {
        this._$label.css({
            maxWidth: this._props.containerWidth
        })
    }
    $element() {
        return this._$root
    }
    isVisible() {
        return this._isVisible()
    }
    getId() {
        if (this._isVisible()) {
            return this._id
        }
    }
    updateMode(mode) {
        this._props.mode = mode;
        this._toggleMarkupVisibility()
    }
    updateText(text) {
        this._props.text = text;
        this._updateText();
        this._toggleMarkupVisibility()
    }
    updateMark(mark) {
        this._props.mark = mark;
        this._updateMark()
    }
    updateContainsButtonsBefore(containsButtonsBefore) {
        this._props.containsButtonsBefore = containsButtonsBefore;
        this._updateEditorBeforeButtonsClass()
    }
    updateBeforeWidth(beforeWidth) {
        this._props.beforeWidth = beforeWidth;
        this._updateBeforeWidth()
    }
    updateMaxWidth(containerWidth) {
        this._props.containerWidth = containerWidth;
        this._updateMaxWidth()
    }
}
export {
    TextEditorLabel
};
