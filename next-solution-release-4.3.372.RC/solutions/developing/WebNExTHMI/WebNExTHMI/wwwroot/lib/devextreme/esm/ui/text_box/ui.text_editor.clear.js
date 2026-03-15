/**
 * DevExtreme (esm/ui/text_box/ui.text_editor.clear.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import $ from "../../core/renderer";
import eventsEngine from "../../events/core/events_engine";
import TextEditorButton from "./texteditor_button_collection/button";
import {
    addNamespace
} from "../../events/utils/index";
import pointer from "../../events/pointer";
import {
    name as click
} from "../../events/click";
var pointerDown = pointer.down;
var STATE_INVISIBLE_CLASS = "dx-state-invisible";
var TEXTEDITOR_CLEAR_BUTTON_CLASS = "dx-clear-button-area";
var TEXTEDITOR_CLEAR_ICON_CLASS = "dx-icon-clear";
var TEXTEDITOR_ICON_CLASS = "dx-icon";
var TEXTEDITOR_SHOW_CLEAR_BUTTON_CLASS = "dx-show-clear-button";
export default class ClearButton extends TextEditorButton {
    _create() {
        var $element = $("<span>").addClass(TEXTEDITOR_CLEAR_BUTTON_CLASS).append($("<span>").addClass(TEXTEDITOR_ICON_CLASS).addClass(TEXTEDITOR_CLEAR_ICON_CLASS));
        this._addToContainer($element);
        this.update(true);
        return {
            instance: $element,
            $element: $element
        }
    }
    _isVisible() {
        var {
            editor: editor
        } = this;
        return editor._isClearButtonVisible()
    }
    _attachEvents(instance, $button) {
        var {
            editor: editor
        } = this;
        var editorName = editor.NAME;
        eventsEngine.on($button, addNamespace(pointerDown, editorName), e => {
            e.preventDefault();
            if ("mouse" !== e.pointerType) {
                editor._clearValueHandler(e)
            }
        });
        eventsEngine.on($button, addNamespace(click, editorName), e => editor._clearValueHandler(e))
    }
    _legacyRender($editor, isVisible) {
        $editor.toggleClass(TEXTEDITOR_SHOW_CLEAR_BUTTON_CLASS, isVisible)
    }
    update() {
        var rendered = arguments.length > 0 && void 0 !== arguments[0] ? arguments[0] : false;
        !rendered && super.update();
        var {
            editor: editor,
            instance: instance
        } = this;
        var $editor = editor.$element();
        var isVisible = this._isVisible();
        instance && instance.toggleClass(STATE_INVISIBLE_CLASS, !isVisible);
        this._legacyRender($editor, isVisible)
    }
}
