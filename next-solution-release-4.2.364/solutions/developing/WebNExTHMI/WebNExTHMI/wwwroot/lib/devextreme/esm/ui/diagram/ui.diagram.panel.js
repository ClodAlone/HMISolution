/**
 * DevExtreme (esm/ui/diagram/ui.diagram.panel.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import $ from "../../core/renderer";
import Widget from "../widget/ui.widget";
import eventsEngine from "../../events/core/events_engine";
import {
    addNamespace
} from "../../events/utils/index";
import pointerEvents from "../../events/pointer";
var POINTERUP_EVENT_NAME = addNamespace(pointerEvents.up, "dxDiagramPanel");
var PREVENT_REFOCUS_SELECTOR = ".dx-textbox";
class DiagramPanel extends Widget {
    _init() {
        super._init();
        this._createOnPointerUpAction()
    }
    _render() {
        super._render();
        this._attachPointerUpEvent()
    }
    _getPointerUpElements() {
        return [this.$element()]
    }
    _attachPointerUpEvent() {
        var elements = this._getPointerUpElements();
        elements.forEach(element => {
            eventsEngine.off(element, POINTERUP_EVENT_NAME);
            eventsEngine.on(element, POINTERUP_EVENT_NAME, e => {
                if (!$(e.target).closest(PREVENT_REFOCUS_SELECTOR).length) {
                    this._onPointerUpAction()
                }
            })
        })
    }
    _createOnPointerUpAction() {
        this._onPointerUpAction = this._createActionByOption("onPointerUp")
    }
    _optionChanged(args) {
        switch (args.name) {
            case "onPointerUp":
                this._createOnPointerUpAction();
                break;
            default:
                super._optionChanged(args)
        }
    }
}
export default DiagramPanel;
