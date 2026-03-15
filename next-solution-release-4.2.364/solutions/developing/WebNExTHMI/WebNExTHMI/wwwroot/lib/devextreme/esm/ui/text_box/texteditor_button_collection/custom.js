/**
 * DevExtreme (esm/ui/text_box/texteditor_button_collection/custom.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import $ from "../../../core/renderer";
import TextEditorButton from "./button";
import Button from "../../button";
import {
    extend
} from "../../../core/utils/extend";
import eventsEngine from "../../../events/core/events_engine";
import {
    start,
    end
} from "../../../events/hover";
import {
    name as clickEventName
} from "../../../events/click";
var CUSTOM_BUTTON_HOVERED_CLASS = "dx-custom-button-hovered";
export default class CustomButton extends TextEditorButton {
    _attachEvents(instance, $element) {
        var {
            editor: editor
        } = this;
        eventsEngine.on($element, start, () => {
            editor.$element().addClass(CUSTOM_BUTTON_HOVERED_CLASS)
        });
        eventsEngine.on($element, end, () => {
            editor.$element().removeClass(CUSTOM_BUTTON_HOVERED_CLASS)
        });
        eventsEngine.on($element, clickEventName, e => {
            e.stopPropagation()
        })
    }
    _create() {
        var {
            editor: editor
        } = this;
        var $element = $("<div>");
        this._addToContainer($element);
        var instance = editor._createComponent($element, Button, extend({}, this.options, {
            ignoreParentReadOnly: true,
            disabled: this._isDisabled(),
            integrationOptions: this._prepareIntegrationOptions(editor)
        }));
        return {
            $element: $element,
            instance: instance
        }
    }
    _prepareIntegrationOptions(editor) {
        return extend({}, editor.option("integrationOptions"), {
            skipTemplates: ["content"]
        })
    }
    update() {
        var isUpdated = super.update();
        if (this.instance) {
            this.instance.option("disabled", this._isDisabled())
        }
        return isUpdated
    }
    _isVisible() {
        var {
            editor: editor
        } = this;
        return editor.option("visible")
    }
    _isDisabled() {
        var isDefinedByUser = void 0 !== this.options.disabled;
        if (isDefinedByUser) {
            return this.instance ? this.instance.option("disabled") : this.options.disabled
        } else {
            return this.editor.option("readOnly")
        }
    }
}
