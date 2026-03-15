/**
 * DevExtreme (esm/ui/grid_core/ui.grid_core.editor_factory.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import {
    setOuterWidth,
    getOuterWidth,
    setOuterHeight,
    getOuterHeight
} from "../../core/utils/size";
import $ from "../../core/renderer";
import domAdapter from "../../core/dom_adapter";
import eventsEngine from "../../events/core/events_engine";
import modules from "./ui.grid_core.modules";
import {
    name as clickEventName
} from "../../events/click";
import pointerEvents from "../../events/pointer";
import positionUtils from "../../animation/position";
import {
    addNamespace,
    normalizeKeyName
} from "../../events/utils/index";
import browser from "../../core/utils/browser";
import {
    extend
} from "../../core/utils/extend";
import {
    getBoundingRect
} from "../../core/utils/position";
import EditorFactoryMixin from "../shared/ui.editor_factory_mixin";
import gridCoreUtils from "./ui.grid_core.utils";
var EDITOR_INLINE_BLOCK = "dx-editor-inline-block";
var CELL_FOCUS_DISABLED_CLASS = "dx-cell-focus-disabled";
var FOCUS_OVERLAY_CLASS = "focus-overlay";
var CONTENT_CLASS = "content";
var FOCUSED_ELEMENT_CLASS = "dx-focused";
var ROW_CLASS = "dx-row";
var MODULE_NAMESPACE = "dxDataGridEditorFactory";
var UPDATE_FOCUS_EVENTS = addNamespace([pointerEvents.down, "focusin", clickEventName].join(" "), MODULE_NAMESPACE);
var DX_HIDDEN = "dx-hidden";
var EditorFactory = modules.ViewController.inherit({
    _getFocusedElement: function($dataGridElement) {
        var rowSelector = this.option("focusedRowEnabled") ? "tr[tabindex]:focus" : "tr[tabindex]:not(.dx-data-row):focus";
        var focusedElementSelector = "td[tabindex]:focus, ".concat(rowSelector, ", input:focus, textarea:focus, .dx-lookup-field:focus, .dx-checkbox:focus, .dx-switch:focus, .dx-dropdownbutton .dx-buttongroup:focus");
        return $dataGridElement.find(focusedElementSelector)
    },
    _getFocusCellSelector: function() {
        return ".dx-row > td"
    },
    _updateFocusCore: function() {
        var $focus = this._$focusedElement;
        var $dataGridElement = this.component && this.component.$element();
        var $focusCell;
        var hideBorders;
        if ($dataGridElement) {
            $focus = this._getFocusedElement($dataGridElement);
            if ($focus.length) {
                if (!$focus.hasClass(CELL_FOCUS_DISABLED_CLASS) && !$focus.hasClass(ROW_CLASS)) {
                    $focusCell = $focus.closest(this._getFocusCellSelector() + ", ." + CELL_FOCUS_DISABLED_CLASS);
                    hideBorders = $focusCell.get(0) !== $focus.get(0) && $focusCell.hasClass(EDITOR_INLINE_BLOCK);
                    $focus = $focusCell
                }
                if ($focus.length && !$focus.hasClass(CELL_FOCUS_DISABLED_CLASS)) {
                    this.focus($focus, hideBorders);
                    return
                }
            }
        }
        this.loseFocus()
    },
    _updateFocus: function(e) {
        var that = this;
        var isFocusOverlay = e && e.event && $(e.event.target).hasClass(that.addWidgetPrefix(FOCUS_OVERLAY_CLASS));
        that._isFocusOverlay = that._isFocusOverlay || isFocusOverlay;
        clearTimeout(that._updateFocusTimeoutID);
        that._updateFocusTimeoutID = setTimeout((function() {
            delete that._updateFocusTimeoutID;
            if (!that._isFocusOverlay) {
                that._updateFocusCore()
            }
            that._isFocusOverlay = false
        }))
    },
    _updateFocusOverlaySize: function($element, position) {
        $element.hide();
        var location = positionUtils.calculate($element, extend({
            collision: "fit"
        }, position));
        if (location.h.oversize > 0) {
            setOuterWidth($element, getOuterWidth($element) - location.h.oversize)
        }
        if (location.v.oversize > 0) {
            setOuterHeight($element, getOuterHeight($element) - location.v.oversize)
        }
        $element.show()
    },
    callbackNames: function() {
        return ["focused"]
    },
    focus: function($element, hideBorder) {
        var that = this;
        if (void 0 === $element) {
            return that._$focusedElement
        } else if ($element) {
            if (!$element.is(that._$focusedElement)) {
                that._$focusedElement && that._$focusedElement.removeClass(FOCUSED_ELEMENT_CLASS)
            }
            that._$focusedElement = $element;
            clearTimeout(that._focusTimeoutID);
            that._focusTimeoutID = setTimeout((function() {
                delete that._focusTimeoutID;
                that.renderFocusOverlay($element, hideBorder);
                $element.addClass(FOCUSED_ELEMENT_CLASS);
                that.focused.fire($element)
            }))
        }
    },
    refocus: function() {
        var $focus = this.focus();
        this.focus($focus)
    },
    renderFocusOverlay: function($element, hideBorder) {
        if (!gridCoreUtils.isElementInCurrentGrid(this, $element)) {
            return
        }
        if (!this._$focusOverlay) {
            this._$focusOverlay = $("<div>").addClass(this.addWidgetPrefix(FOCUS_OVERLAY_CLASS))
        }
        if (hideBorder) {
            this._$focusOverlay.addClass(DX_HIDDEN)
        } else if ($element.length) {
            var align = browser.mozilla ? "right bottom" : "left top";
            var $content = $element.closest("." + this.addWidgetPrefix(CONTENT_CLASS));
            var elemCoord = getBoundingRect($element.get(0));
            this._$focusOverlay.removeClass(DX_HIDDEN).appendTo($content);
            setOuterHeight(this._$focusOverlay, elemCoord.bottom - elemCoord.top + 1);
            setOuterWidth(this._$focusOverlay, elemCoord.right - elemCoord.left + 1);
            var focusOverlayPosition = {
                precise: true,
                my: align,
                at: align,
                of: $element,
                boundary: $content.length && $content
            };
            this._updateFocusOverlaySize(this._$focusOverlay, focusOverlayPosition);
            positionUtils.setup(this._$focusOverlay, focusOverlayPosition);
            this._$focusOverlay.css("visibility", "visible")
        }
    },
    resize: function() {
        var $focusedElement = this._$focusedElement;
        if ($focusedElement) {
            this.focus($focusedElement)
        }
    },
    loseFocus: function() {
        this._$focusedElement && this._$focusedElement.removeClass(FOCUSED_ELEMENT_CLASS);
        this._$focusedElement = null;
        this._$focusOverlay && this._$focusOverlay.addClass(DX_HIDDEN)
    },
    init: function() {
        this.createAction("onEditorPreparing", {
            excludeValidators: ["disabled", "readOnly"],
            category: "rendering"
        });
        this.createAction("onEditorPrepared", {
            excludeValidators: ["disabled", "readOnly"],
            category: "rendering"
        });
        this._updateFocusHandler = this._updateFocusHandler || this.createAction(this._updateFocus.bind(this));
        eventsEngine.on(domAdapter.getDocument(), UPDATE_FOCUS_EVENTS, this._updateFocusHandler);
        this._attachContainerEventHandlers()
    },
    _attachContainerEventHandlers: function() {
        var that = this;
        var $container = that.component && that.component.$element();
        if ($container) {
            eventsEngine.on($container, addNamespace("keydown", MODULE_NAMESPACE), (function(e) {
                if ("tab" === normalizeKeyName(e)) {
                    that._updateFocusHandler(e)
                }
            }))
        }
    },
    dispose: function() {
        clearTimeout(this._focusTimeoutID);
        clearTimeout(this._updateFocusTimeoutID);
        eventsEngine.off(domAdapter.getDocument(), UPDATE_FOCUS_EVENTS, this._updateFocusHandler)
    }
}).include(EditorFactoryMixin);
export var editorFactoryModule = {
    defaultOptions: function() {
        return {}
    },
    controllers: {
        editorFactory: EditorFactory
    }
};
