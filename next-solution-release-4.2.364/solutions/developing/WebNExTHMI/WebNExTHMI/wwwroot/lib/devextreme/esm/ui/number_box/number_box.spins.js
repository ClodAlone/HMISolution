/**
 * DevExtreme (esm/ui/number_box/number_box.spins.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import $ from "../../core/renderer";
import eventsEngine from "../../events/core/events_engine";
import TextEditorButton from "../text_box/texteditor_button_collection/button";
import SpinButton from "./number_box.spin";
import {
    addNamespace
} from "../../events/utils/index";
import pointer from "../../events/pointer";
import {
    extend
} from "../../core/utils/extend";
var SPIN_CLASS = "dx-numberbox-spin";
var SPIN_CONTAINER_CLASS = "dx-numberbox-spin-container";
var SPIN_TOUCH_FRIENDLY_CLASS = "dx-numberbox-spin-touch-friendly";
export default class SpinButtons extends TextEditorButton {
    _attachEvents(instance, $spinContainer) {
        var {
            editor: editor
        } = this;
        var eventName = addNamespace(pointer.down, editor.NAME);
        var $spinContainerChildren = $spinContainer.children();
        var pointerDownAction = editor._createAction(e => editor._spinButtonsPointerDownHandler(e));
        eventsEngine.off($spinContainer, eventName);
        eventsEngine.on($spinContainer, eventName, e => pointerDownAction({
            event: e
        }));
        SpinButton.getInstance($spinContainerChildren.eq(0)).option("onChange", e => editor._spinUpChangeHandler(e));
        SpinButton.getInstance($spinContainerChildren.eq(1)).option("onChange", e => editor._spinDownChangeHandler(e))
    }
    _create() {
        var {
            editor: editor
        } = this;
        var $spinContainer = $("<div>").addClass(SPIN_CONTAINER_CLASS);
        var $spinUp = $("<div>").appendTo($spinContainer);
        var $spinDown = $("<div>").appendTo($spinContainer);
        var options = this._getOptions();
        this._addToContainer($spinContainer);
        editor._createComponent($spinUp, SpinButton, extend({
            direction: "up"
        }, options));
        editor._createComponent($spinDown, SpinButton, extend({
            direction: "down"
        }, options));
        this._legacyRender(editor.$element(), this._isTouchFriendly(), options.visible);
        return {
            instance: $spinContainer,
            $element: $spinContainer
        }
    }
    _getOptions() {
        var {
            editor: editor
        } = this;
        var visible = this._isVisible();
        var disabled = editor.option("disabled");
        return {
            visible: visible,
            disabled: disabled
        }
    }
    _isVisible() {
        var {
            editor: editor
        } = this;
        return super._isVisible() && editor.option("showSpinButtons")
    }
    _isTouchFriendly() {
        var {
            editor: editor
        } = this;
        return editor.option("showSpinButtons") && editor.option("useLargeSpinButtons")
    }
    _legacyRender($editor, isTouchFriendly, isVisible) {
        $editor.toggleClass(SPIN_TOUCH_FRIENDLY_CLASS, isTouchFriendly);
        $editor.toggleClass(SPIN_CLASS, isVisible)
    }
    update() {
        var shouldUpdate = super.update();
        if (shouldUpdate) {
            var {
                editor: editor,
                instance: instance
            } = this;
            var $editor = editor.$element();
            var isVisible = this._isVisible();
            var isTouchFriendly = this._isTouchFriendly();
            var $spinButtons = instance.children();
            var spinUp = SpinButton.getInstance($spinButtons.eq(0));
            var spinDown = SpinButton.getInstance($spinButtons.eq(1));
            var options = this._getOptions();
            spinUp.option(options);
            spinDown.option(options);
            this._legacyRender($editor, isTouchFriendly, isVisible)
        }
    }
}
