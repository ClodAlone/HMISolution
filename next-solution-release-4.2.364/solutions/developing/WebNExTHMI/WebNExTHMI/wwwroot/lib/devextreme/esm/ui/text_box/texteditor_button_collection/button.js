/**
 * DevExtreme (esm/ui/text_box/texteditor_button_collection/button.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import $ from "../../../core/renderer";
export default class TextEditorButton {
    constructor(name, editor, options) {
        this.instance = null;
        this.$container = null;
        this.$placeMarker = null;
        this.editor = editor;
        this.name = name;
        this.options = options || {}
    }
    _addPlaceMarker($container) {
        this.$placeMarker = $("<div>").appendTo($container)
    }
    _addToContainer($element) {
        var {
            $placeMarker: $placeMarker,
            $container: $container
        } = this;
        $placeMarker ? $placeMarker.replaceWith($element) : $element.appendTo($container)
    }
    _attachEvents() {
        throw "Not implemented"
    }
    _create() {
        throw "Not implemented"
    }
    _isRendered() {
        return !!this.instance
    }
    _isVisible() {
        var {
            editor: editor,
            options: options
        } = this;
        return options.visible || !editor.option("readOnly")
    }
    _isDisabled() {
        throw "Not implemented"
    }
    _shouldRender() {
        return this._isVisible() && !this._isRendered()
    }
    dispose() {
        var {
            instance: instance,
            $placeMarker: $placeMarker
        } = this;
        if (instance) {
            instance.dispose ? instance.dispose() : instance.remove();
            this.instance = null
        }
        $placeMarker && $placeMarker.remove()
    }
    render() {
        var $container = arguments.length > 0 && void 0 !== arguments[0] ? arguments[0] : this.$container;
        this.$container = $container;
        if (this._isVisible()) {
            var {
                instance: instance,
                $element: $element
            } = this._create();
            this.instance = instance;
            this._attachEvents(instance, $element)
        } else {
            this._addPlaceMarker($container)
        }
    }
    update() {
        if (this._shouldRender()) {
            this.render()
        }
        return !!this.instance
    }
}
