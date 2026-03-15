/**
 * DevExtreme (esm/ui/file_manager/ui.file_manager.adaptivity.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import {
    getWidth
} from "../../core/utils/size";
import $ from "../../core/renderer";
import {
    extend
} from "../../core/utils/extend";
import {
    isFunction
} from "../../core/utils/type";
import {
    getWindow,
    hasWindow
} from "../../core/utils/window";
import Widget from "../widget/ui.widget";
import Drawer from "../drawer/ui.drawer";
import SplitterControl from "../splitter";
var window = getWindow();
var ADAPTIVE_STATE_SCREEN_WIDTH = 573;
var FILE_MANAGER_ADAPTIVITY_DRAWER_PANEL_CLASS = "dx-filemanager-adaptivity-drawer-panel";
var DRAWER_PANEL_CONTENT_INITIAL = "dx-drawer-panel-content-initial";
var DRAWER_PANEL_CONTENT_ADAPTIVE = "dx-drawer-panel-content-adaptive";
class FileManagerAdaptivityControl extends Widget {
    _initMarkup() {
        super._initMarkup();
        this._initActions();
        this._isInAdaptiveState = false;
        var $drawer = $("<div>").appendTo(this.$element());
        $("<div>").addClass(FILE_MANAGER_ADAPTIVITY_DRAWER_PANEL_CLASS).appendTo($drawer);
        this._drawer = this._createComponent($drawer, Drawer);
        this._drawer.option({
            opened: true,
            template: this._createDrawerTemplate.bind(this)
        });
        $(this._drawer.content()).addClass(DRAWER_PANEL_CONTENT_INITIAL);
        var $drawerContent = $drawer.find(".".concat(FILE_MANAGER_ADAPTIVITY_DRAWER_PANEL_CLASS)).first();
        var contentRenderer = this.option("contentTemplate");
        if (isFunction(contentRenderer)) {
            contentRenderer($drawerContent)
        }
        this._updateDrawerMaxSize()
    }
    _createDrawerTemplate(container) {
        this.option("drawerTemplate")(container);
        this._splitter = this._createComponent("<div>", SplitterControl, {
            container: this.$element(),
            leftElement: $(this._drawer.content()),
            rightElement: $(this._drawer.viewContent()),
            onApplyPanelSize: this._onApplyPanelSize.bind(this),
            onActiveStateChanged: this._onActiveStateChanged.bind(this)
        });
        this._splitter.$element().appendTo(container);
        this._splitter.disableSplitterCalculation(true)
    }
    _render() {
        super._render();
        this._checkAdaptiveState()
    }
    _onApplyPanelSize(e) {
        if (!hasWindow()) {
            return
        }
        if (!this._splitter.isSplitterMoved()) {
            this._setDrawerWidth("");
            return
        }
        $(this._drawer.content()).removeClass(DRAWER_PANEL_CONTENT_INITIAL);
        this._setDrawerWidth(e.leftPanelWidth)
    }
    _onActiveStateChanged(_ref) {
        var {
            isActive: isActive
        } = _ref;
        this._splitter.disableSplitterCalculation(!isActive);
        !isActive && this._splitter.$element().css("left", "auto")
    }
    _setDrawerWidth(width) {
        $(this._drawer.content()).css("width", width);
        this._updateDrawerMaxSize();
        this._drawer.resizeViewContent()
    }
    _updateDrawerMaxSize() {
        this._drawer.option("maxSize", this._drawer.getRealPanelWidth())
    }
    _dimensionChanged(dimension) {
        if (!dimension || "height" !== dimension) {
            this._checkAdaptiveState()
        }
    }
    _checkAdaptiveState() {
        var oldState = this._isInAdaptiveState;
        this._isInAdaptiveState = this._isSmallScreen();
        if (oldState !== this._isInAdaptiveState) {
            this.toggleDrawer(!this._isInAdaptiveState, true);
            $(this._drawer.content()).toggleClass(DRAWER_PANEL_CONTENT_ADAPTIVE, this._isInAdaptiveState);
            this._raiseAdaptiveStateChanged(this._isInAdaptiveState)
        }
        if (this._isInAdaptiveState && this._isDrawerOpened()) {
            this._updateDrawerMaxSize()
        }
    }
    _isSmallScreen() {
        return getWidth(window) <= ADAPTIVE_STATE_SCREEN_WIDTH
    }
    _isDrawerOpened() {
        return this._drawer.option("opened")
    }
    _initActions() {
        this._actions = {
            onAdaptiveStateChanged: this._createActionByOption("onAdaptiveStateChanged")
        }
    }
    _raiseAdaptiveStateChanged(enabled) {
        this._actions.onAdaptiveStateChanged({
            enabled: enabled
        })
    }
    _getDefaultOptions() {
        return extend(super._getDefaultOptions(), {
            drawerTemplate: null,
            contentTemplate: null,
            onAdaptiveStateChanged: null
        })
    }
    _optionChanged(args) {
        var name = args.name;
        switch (name) {
            case "drawerTemplate":
            case "contentTemplate":
                this.repaint();
                break;
            case "onAdaptiveStateChanged":
                this._actions[name] = this._createActionByOption(name);
                break;
            default:
                super._optionChanged(args)
        }
    }
    isInAdaptiveState() {
        return this._isInAdaptiveState
    }
    toggleDrawer(showing, skipAnimation) {
        this._updateDrawerMaxSize();
        this._drawer.option("animationEnabled", !skipAnimation);
        this._drawer.toggle(showing);
        var isSplitterActive = this._isDrawerOpened() && !this.isInAdaptiveState();
        this._splitter.toggleDisabled(!isSplitterActive)
    }
    getSplitterElement() {
        return this._splitter.getSplitterBorderElement().get(0)
    }
}
export default FileManagerAdaptivityControl;
