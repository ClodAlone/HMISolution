/**
 * DevExtreme (esm/ui/diagram/ui.diagram.properties_panel.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import {
    getHeight,
    getOuterHeight
} from "../../core/utils/size";
import $ from "../../core/renderer";
import {
    extend
} from "../../core/utils/extend";
import ScrollView from "../scroll_view";
import TabPanel from "../tab_panel";
import DiagramFloatingPanel from "./ui.diagram.floating_panel";
import DiagramCommandsManager from "./diagram.commands_manager";
var DIAGRAM_PROPERTIES_POPUP_WIDTH = 420;
var DIAGRAM_PROPERTIES_POPUP_HEIGHT = 340;
var DIAGRAM_PROPERTIES_POPUP_CLASS = "dx-diagram-properties-popup";
var DIAGRAM_PROPERTIES_POPUP_NOTABS_CLASS = "dx-diagram-properties-popup-notabs";
var DIAGRAM_PROPERTIES_PANEL_CLASS = "dx-diagram-properties-panel";
var DIAGRAM_PROPERTIES_PANEL_GROUP_TITLE_CLASS = "dx-diagram-properties-panel-group-title";
var DIAGRAM_PROPERTIES_PANEL_GROUP_TOOLBAR_CLASS = "dx-diagram-properties-panel-group-toolbar";
class DiagramPropertiesPanel extends DiagramFloatingPanel {
    _init() {
        super._init();
        this._commandTabs = DiagramCommandsManager.getPropertyPanelCommandTabs(this.option("propertyTabs"));
        this._createOnCreateToolbar();
        this._createOnSelectedGroupChanged()
    }
    _initMarkup() {
        this._toolbars = [];
        this._selectedToolbar = void 0;
        super._initMarkup()
    }
    _getPopupClass() {
        var className = DIAGRAM_PROPERTIES_POPUP_CLASS;
        if (!this._hasTabPanel()) {
            className += " " + DIAGRAM_PROPERTIES_POPUP_NOTABS_CLASS
        }
        return className
    }
    _getPopupWidth() {
        return this.isMobileView() ? "100%" : DIAGRAM_PROPERTIES_POPUP_WIDTH
    }
    _getPopupHeight() {
        return DIAGRAM_PROPERTIES_POPUP_HEIGHT
    }
    _getPopupPosition() {
        var $parent = this.option("offsetParent");
        if (this.isMobileView()) {
            return {
                my: "left bottom",
                at: "left bottom",
                of: $parent
            }
        }
        return {
            my: "right bottom",
            at: "right bottom",
            of: $parent,
            offset: "-" + this.option("offsetX") + " -" + this.option("offsetY")
        }
    }
    _getPopupAnimation() {
        var $parent = this.option("offsetParent");
        if (this.isMobileView()) {
            return {
                hide: this._getPopupSlideAnimationObject({
                    direction: "bottom",
                    from: {
                        position: {
                            my: "left bottom",
                            at: "left bottom",
                            of: $parent
                        }
                    },
                    to: {
                        position: {
                            my: "left top",
                            at: "left bottom",
                            of: $parent
                        }
                    }
                }),
                show: this._getPopupSlideAnimationObject({
                    direction: "top",
                    from: {
                        position: {
                            my: "left top",
                            at: "left bottom",
                            of: $parent
                        }
                    },
                    to: {
                        position: {
                            my: "left bottom",
                            at: "left bottom",
                            of: $parent
                        }
                    }
                })
            }
        }
        return super._getPopupAnimation()
    }
    _getPopupOptions() {
        return extend(super._getPopupOptions(), {
            showTitle: this.isMobileView(),
            showCloseButton: this.isMobileView()
        })
    }
    _renderPopupContent($parent) {
        if (!this._commandTabs.length) {
            return
        }
        var $panel = $("<div>").addClass(DIAGRAM_PROPERTIES_PANEL_CLASS).appendTo($parent);
        if (this._hasTabPanel()) {
            this._renderTabPanel($panel)
        } else {
            this._renderTabContent($panel, this._commandTabs[0], 0, true)
        }
    }
    _hasTabPanel() {
        return this._commandTabs.length > 1
    }
    _renderTabPanel($parent) {
        var $tabPanel = $("<div>").appendTo($parent);
        this._tabPanel = this._createComponent($tabPanel, TabPanel, {
            focusStateEnabled: false,
            dataSource: this._commandTabs,
            itemTemplate: (data, index, $element) => {
                this._renderTabContent($element, data, index)
            },
            onSelectionChanged: e => {
                this._onSelectedGroupChangedAction();
                this._onPointerUpAction()
            },
            onContentReady: e => {
                this._popup.option("height", getHeight(e.component.$element()) + this._getVerticalPaddingsAndBorders());
                if (this._firstScrollView) {
                    this._scrollViewHeight = getOuterHeight(this._firstScrollView.$element());
                    this._firstScrollView.option("height", this._scrollViewHeight)
                }
            }
        })
    }
    _renderTabContent($parent, tab, index, isSingleTab) {
        var $scrollViewWrapper = $("<div>").appendTo($parent);
        var scrollView = this._createComponent($scrollViewWrapper, ScrollView, {
            height: this._scrollViewHeight
        });
        this._renderTabInnerContent(scrollView.content(), tab, index);
        if (isSingleTab) {
            this._popup.option("height", getHeight(scrollView.$element()) + this._getVerticalPaddingsAndBorders())
        } else {
            this._firstScrollView = this._firstScrollView || scrollView
        }
    }
    _renderTabInnerContent($parent, group, index) {
        if (group.groups) {
            group.groups.forEach((sg, si) => {
                this._renderTabGroupContent($parent, index, sg.title, sg.commands)
            })
        } else if (group.commands) {
            this._renderTabGroupContent($parent, index, void 0, group.commands)
        }
    }
    _renderTabGroupContent($parent, index, title, commands) {
        if (title) {
            $("<div>").addClass(DIAGRAM_PROPERTIES_PANEL_GROUP_TITLE_CLASS).appendTo($parent).text(title)
        }
        var $toolbar = $("<div>").addClass(DIAGRAM_PROPERTIES_PANEL_GROUP_TOOLBAR_CLASS).appendTo($parent);
        var args = {
            $parent: $toolbar,
            commands: commands
        };
        this._onCreateToolbarAction(args);
        if (!this._toolbars[index]) {
            this._toolbars[index] = []
        }
        this._toolbars[index].push(args.toolbar);
        this._selectedToolbar = args.toolbar
    }
    getActiveToolbars() {
        var index = this._tabPanel ? this._tabPanel.option("selectedIndex") : 0;
        return this._toolbars[index]
    }
    _createOnCreateToolbar() {
        this._onCreateToolbarAction = this._createActionByOption("onCreateToolbar")
    }
    _createOnSelectedGroupChanged() {
        this._onSelectedGroupChangedAction = this._createActionByOption("onSelectedGroupChanged")
    }
    _optionChanged(args) {
        switch (args.name) {
            case "onCreateToolbar":
                this._createOnCreateToolbar();
                break;
            case "onSelectedGroupChanged":
                this._createOnSelectedGroupChanged();
                break;
            case "propertyTabs":
                this._invalidate();
                break;
            default:
                super._optionChanged(args)
        }
    }
}
export default DiagramPropertiesPanel;
