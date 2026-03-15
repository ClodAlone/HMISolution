/**
 * DevExtreme (esm/ui/diagram/ui.diagram.toolbox.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import {
    getHeight,
    setHeight,
    getOuterHeight
} from "../../core/utils/size";
import $ from "../../core/renderer";
import {
    extend
} from "../../core/utils/extend";
import {
    hasWindow,
    getWindow
} from "../../core/utils/window";
import {
    Deferred
} from "../../core/utils/deferred";
import messageLocalization from "../../localization/message";
import TextBox from "../text_box";
import Accordion from "../accordion";
import ScrollView from "../scroll_view";
import Tooltip from "../tooltip";
import {
    getDiagram
} from "./diagram.importer";
import DiagramFloatingPanel from "./ui.diagram.floating_panel";
var DIAGRAM_TOOLBOX_MIN_HEIGHT = 130;
var DIAGRAM_TOOLBOX_POPUP_CLASS = "dx-diagram-toolbox-popup";
var DIAGRAM_TOOLBOX_PANEL_CLASS = "dx-diagram-toolbox-panel";
var DIAGRAM_TOOLBOX_INPUT_CONTAINER_CLASS = "dx-diagram-toolbox-input-container";
var DIAGRAM_TOOLBOX_INPUT_CLASS = "dx-diagram-toolbox-input";
var DIAGRAM_TOOLTIP_DATATOGGLE = "shape-toolbox-tooltip";
var DIAGRAM_SKIP_GESTURE_CLASS = "dx-skip-gesture-event";
class DiagramToolbox extends DiagramFloatingPanel {
    _init() {
        super._init();
        this._toolboxes = [];
        this._filterText = "";
        this._createOnShapeCategoryRenderedAction();
        this._createOnFilterChangedAction()
    }
    _getPopupClass() {
        return DIAGRAM_TOOLBOX_POPUP_CLASS
    }
    _getPopupHeight() {
        return this.isMobileView() ? "100%" : super._getPopupHeight()
    }
    _getPopupMaxHeight() {
        return this.isMobileView() ? "100%" : super._getPopupMaxHeight()
    }
    _getPopupMinHeight() {
        return DIAGRAM_TOOLBOX_MIN_HEIGHT
    }
    _getPopupPosition() {
        var $parent = this.option("offsetParent");
        var position = {
            my: "left top",
            at: "left top",
            of: $parent
        };
        if (!this.isMobileView()) {
            return extend(position, {
                offset: this.option("offsetX") + " " + this.option("offsetY")
            })
        }
        return position
    }
    _getPopupAnimation() {
        var $parent = this.option("offsetParent");
        if (this.isMobileView()) {
            return {
                hide: this._getPopupSlideAnimationObject({
                    direction: "left",
                    from: {
                        position: {
                            my: "left top",
                            at: "left top",
                            of: $parent
                        }
                    },
                    to: {
                        position: {
                            my: "right top",
                            at: "left top",
                            of: $parent
                        }
                    }
                }),
                show: this._getPopupSlideAnimationObject({
                    direction: "right",
                    from: {
                        position: {
                            my: "right top",
                            at: "left top",
                            of: $parent
                        }
                    },
                    to: {
                        position: {
                            my: "left top",
                            at: "left top",
                            of: $parent
                        }
                    }
                })
            }
        }
        return super._getPopupAnimation()
    }
    _getPopupOptions() {
        var options = super._getPopupOptions();
        if (!this.isMobileView()) {
            return extend(options, {
                showTitle: true,
                toolbarItems: [{
                    widget: "dxButton",
                    location: "center",
                    options: {
                        activeStateEnabled: false,
                        focusStateEnabled: false,
                        hoverStateEnabled: false,
                        icon: "diagram-toolbox-drag",
                        stylingMode: "outlined",
                        type: "normal"
                    }
                }]
            })
        }
        return options
    }
    _renderPopupContent($parent) {
        var panelHeight = "100%";
        if (this.option("showSearch")) {
            var $inputContainer = $("<div>").addClass(DIAGRAM_TOOLBOX_INPUT_CONTAINER_CLASS).appendTo($parent);
            this._updateElementWidth($inputContainer);
            this._renderSearchInput($inputContainer);
            if (hasWindow()) {
                panelHeight = "calc(100% - " + getHeight(this._searchInput.$element()) + "px)"
            }
        }
        var $panel = $("<div>").addClass(DIAGRAM_TOOLBOX_PANEL_CLASS).appendTo($parent);
        setHeight($panel, panelHeight);
        this._updateElementWidth($panel);
        this._renderScrollView($panel)
    }
    _updateElementWidth($element) {
        if (void 0 !== this.option("toolboxWidth")) {
            $element.css("width", this.option("toolboxWidth"))
        }
    }
    updateMaxHeight() {
        if (this.isMobileView()) {
            return
        }
        var maxHeight = 6;
        if (this._popup) {
            var $title = this._getPopupTitle();
            maxHeight += getOuterHeight($title)
        }
        if (this._accordion) {
            maxHeight += getOuterHeight(this._accordion.$element())
        }
        if (this._searchInput) {
            maxHeight += getOuterHeight(this._searchInput.$element())
        }
        this.option("maxHeight", maxHeight)
    }
    _renderSearchInput($parent) {
        var $input = $("<div>").addClass(DIAGRAM_TOOLBOX_INPUT_CLASS).appendTo($parent);
        this._searchInput = this._createComponent($input, TextBox, {
            stylingMode: "outlined",
            placeholder: messageLocalization.format("dxDiagram-uiSearch"),
            onValueChanged: data => {
                this._onInputChanged(data.value)
            },
            valueChangeEvent: "keyup",
            buttons: [{
                name: "search",
                location: "after",
                options: {
                    activeStateEnabled: false,
                    focusStateEnabled: false,
                    hoverStateEnabled: false,
                    icon: "search",
                    stylingMode: "outlined",
                    type: "normal",
                    onClick: () => {
                        this._searchInput.focus()
                    }
                }
            }]
        })
    }
    _renderScrollView($parent) {
        var $scrollViewWrapper = $("<div>").appendTo($parent);
        this._scrollView = this._createComponent($scrollViewWrapper, ScrollView);
        var $accordion = $("<div>").appendTo(this._scrollView.content());
        this._updateElementWidth($accordion);
        this._renderAccordion($accordion)
    }
    _getAccordionDataSource() {
        var result = [];
        var toolboxGroups = this.option("toolboxGroups");
        for (var i = 0; i < toolboxGroups.length; i++) {
            var category = toolboxGroups[i].category;
            var title = toolboxGroups[i].title;
            var groupObj = {
                category: category,
                title: title || category,
                expanded: toolboxGroups[i].expanded,
                displayMode: toolboxGroups[i].displayMode,
                shapes: toolboxGroups[i].shapes,
                onTemplate: (widget, $element, data) => {
                    var $toolboxElement = $($element);
                    this._onShapeCategoryRenderedAction({
                        category: data.category,
                        displayMode: data.displayMode,
                        dataToggle: DIAGRAM_TOOLTIP_DATATOGGLE,
                        shapes: data.shapes,
                        $element: $toolboxElement
                    });
                    this._toolboxes.push($toolboxElement);
                    if ("" !== this._filterText) {
                        this._onFilterChangedAction({
                            text: this._filterText,
                            filteringToolboxes: this._toolboxes.length - 1
                        })
                    }
                    this._createTooltips($toolboxElement)
                }
            };
            result.push(groupObj)
        }
        return result
    }
    _createTooltips($toolboxElement) {
        if (this._isTouchMode()) {
            return
        }
        var targets = $toolboxElement.find('[data-toggle="' + DIAGRAM_TOOLTIP_DATATOGGLE + '"]');
        var $container = this.$element();
        targets.each((index, element) => {
            var $target = $(element);
            var title = $target.attr("title");
            if (title) {
                var $tooltip = $("<div>").text(title).appendTo($container);
                this._createComponent($tooltip, Tooltip, {
                    target: $target.get(0),
                    showEvent: "mouseenter",
                    hideEvent: "mouseleave",
                    position: "top",
                    animation: {
                        show: {
                            type: "fade",
                            from: 0,
                            to: 1,
                            delay: 500
                        },
                        hide: {
                            type: "fade",
                            from: 1,
                            to: 0,
                            delay: 100
                        }
                    }
                })
            }
        })
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
    _renderAccordion($container) {
        this._accordion = this._createComponent($container, Accordion, {
            multiple: true,
            animationDuration: 0,
            activeStateEnabled: false,
            focusStateEnabled: false,
            hoverStateEnabled: false,
            collapsible: true,
            displayExpr: "title",
            dataSource: this._getAccordionDataSource(),
            disabled: this.option("disabled"),
            itemTemplate: (data, index, $element) => {
                data.onTemplate(this, $element, data)
            },
            onSelectionChanged: e => {
                this._updateScrollAnimateSubscription(e.component)
            },
            onContentReady: e => {
                e.component.option("selectedItems", []);
                var items = e.component.option("dataSource");
                for (var i = 0; i < items.length; i++) {
                    if (false === items[i].expanded) {
                        e.component.collapseItem(i)
                    } else if (true === items[i].expanded) {
                        e.component.expandItem(i)
                    }
                }
                if (items.length && void 0 === items[0].expanded) {
                    e.component.expandItem(0)
                }
                this._updateScrollAnimateSubscription(e.component)
            }
        })
    }
    _updateScrollAnimateSubscription(component) {
        component._deferredAnimate = new Deferred;
        component._deferredAnimate.done(() => {
            this.updateMaxHeight();
            this._scrollView.update();
            this._updateScrollAnimateSubscription(component)
        })
    }
    _raiseToolboxDragStart() {
        this._scrollView.$element().addClass(DIAGRAM_SKIP_GESTURE_CLASS)
    }
    _raiseToolboxDragEnd() {
        this._scrollView.$element().removeClass(DIAGRAM_SKIP_GESTURE_CLASS)
    }
    _onInputChanged(text) {
        this._filterText = text;
        this._onFilterChangedAction({
            text: this._filterText,
            filteringToolboxes: this._toolboxes.map(($element, index) => index)
        });
        this.updateTooltips();
        this.updateMaxHeight();
        this._scrollView.update()
    }
    updateFilter() {
        this._onInputChanged(this._filterText)
    }
    updateTooltips() {
        this._toolboxes.forEach($element => {
            var $tooltipContainer = $($element);
            this._createTooltips($tooltipContainer)
        })
    }
    _createOnShapeCategoryRenderedAction() {
        this._onShapeCategoryRenderedAction = this._createActionByOption("onShapeCategoryRendered")
    }
    _createOnFilterChangedAction() {
        this._onFilterChangedAction = this._createActionByOption("onFilterChanged")
    }
    _optionChanged(args) {
        switch (args.name) {
            case "onShapeCategoryRendered":
                this._createOnShapeCategoryRenderedAction();
                break;
            case "onFilterChanged":
                this._createOnFilterChangedAction();
                break;
            case "showSearch":
            case "toolboxWidth":
                this._invalidate();
                break;
            case "toolboxGroups":
                this._accordion.option("dataSource", this._getAccordionDataSource());
                break;
            default:
                super._optionChanged(args)
        }
    }
}
export default DiagramToolbox;
