/**
 * DevExtreme (esm/ui/diagram/ui.diagram.context_toolbox.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import $ from "../../core/renderer";
import Widget from "../widget/ui.widget";
import Popover from "../popover";
import {
    getDiagram
} from "./diagram.importer";
import {
    hasWindow,
    getWindow
} from "../../core/utils/window";
var DIAGRAM_CONTEXT_TOOLBOX_TARGET_CLASS = "dx-diagram-context-toolbox-target";
var DIAGRAM_CONTEXT_TOOLBOX_CLASS = "dx-diagram-context-toolbox";
var DIAGRAM_TOUCH_CONTEXT_TOOLBOX_CLASS = "dx-diagram-touch-context-toolbox";
var DIAGRAM_CONTEXT_TOOLBOX_CONTENT_CLASS = "dx-diagram-context-toolbox-content";
class DiagramContextToolbox extends Widget {
    _init() {
        super._init();
        this._onShownAction = this._createActionByOption("onShown");
        this._popoverPositionData = [{
            my: {
                x: "center",
                y: "top"
            },
            at: {
                x: "center",
                y: "bottom"
            },
            offset: {
                x: 0,
                y: 5
            }
        }, {
            my: {
                x: "right",
                y: "center"
            },
            at: {
                x: "left",
                y: "center"
            },
            offset: {
                x: -5,
                y: 0
            }
        }, {
            my: {
                x: "center",
                y: "bottom"
            },
            at: {
                x: "center",
                y: "top"
            },
            offset: {
                x: 0,
                y: -5
            }
        }, {
            my: {
                x: "left",
                y: "center"
            },
            at: {
                x: "right",
                y: "center"
            },
            offset: {
                x: 5,
                y: 0
            }
        }]
    }
    _initMarkup() {
        super._initMarkup();
        this._$popoverTargetElement = $("<div>").addClass(DIAGRAM_CONTEXT_TOOLBOX_TARGET_CLASS).appendTo(this.$element());
        var $popoverElement = $("<div>").appendTo(this.$element());
        var popoverClass = DIAGRAM_CONTEXT_TOOLBOX_CLASS;
        if (this._isTouchMode()) {
            popoverClass += " " + DIAGRAM_TOUCH_CONTEXT_TOOLBOX_CLASS
        }
        this._popoverInstance = this._createComponent($popoverElement, Popover, {
            closeOnOutsideClick: false,
            container: this.$element()
        });
        this._popoverInstance.$element().addClass(popoverClass)
    }
    _isTouchMode() {
        var {
            Browser: Browser
        } = getDiagram();
        if (Browser.TouchUI) {
            return true
        }
        if (!hasWindow()) {
            return false
        }
        var window = getWindow();
        return window.navigator && window.navigator.maxTouchPoints > 0
    }
    _show(x, y, side, category, callback) {
        this._popoverInstance.hide();
        var $content = $("<div>").addClass(DIAGRAM_CONTEXT_TOOLBOX_CONTENT_CLASS);
        if (void 0 !== this.option("toolboxWidth")) {
            $content.css("width", this.option("toolboxWidth"))
        }
        this._$popoverTargetElement.css({
            left: x + this._popoverPositionData[side].offset.x,
            top: y + this._popoverPositionData[side].offset.y
        }).show();
        var window = getWindow();
        var targetDiv = this._$popoverTargetElement.get(0);
        this._$popoverTargetElement.css({
            left: targetDiv.offsetLeft - (targetDiv.getBoundingClientRect().left + window.scrollX - targetDiv.offsetLeft),
            top: targetDiv.offsetTop - (targetDiv.getBoundingClientRect().top + window.scrollY - targetDiv.offsetTop)
        });
        this._popoverInstance.option({
            position: {
                my: this._popoverPositionData[side].my,
                at: this._popoverPositionData[side].at,
                of: this._$popoverTargetElement
            },
            contentTemplate: $content,
            onContentReady: function() {
                var $element = this.$element().find("." + DIAGRAM_CONTEXT_TOOLBOX_CONTENT_CLASS);
                this._onShownAction({
                    category: category,
                    callback: callback,
                    $element: $element,
                    hide: () => this._popoverInstance.hide()
                })
            }.bind(this)
        });
        this._popoverInstance.show()
    }
    _hide() {
        this._$popoverTargetElement.hide();
        this._popoverInstance.hide()
    }
}
export default DiagramContextToolbox;
