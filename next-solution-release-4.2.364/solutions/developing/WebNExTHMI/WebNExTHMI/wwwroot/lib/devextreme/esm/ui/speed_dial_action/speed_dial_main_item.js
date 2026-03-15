/**
 * DevExtreme (esm/ui/speed_dial_action/speed_dial_main_item.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import {
    getHeight
} from "../../core/utils/size";
import $ from "../../core/renderer";
import config from "../../core/config";
import {
    extend
} from "../../core/utils/extend";
import eventsEngine from "../../events/core/events_engine";
import errors from "../widget/ui.errors";
import swatchContainer from "../widget/swatch_container";
import SpeedDialItem from "./speed_dial_item";
import {
    isMaterial
} from "../themes";
var {
    getSwatchContainer: getSwatchContainer
} = swatchContainer;
var FAB_MAIN_CLASS = "dx-fa-button-main";
var FAB_MAIN_CLASS_WITH_LABEL = "dx-fa-button-with-label";
var FAB_CLOSE_ICON_CLASS = "dx-fa-button-icon-close";
var INVISIBLE_STATE_CLASS = "dx-state-invisible";
var speedDialMainItem = null;
var modifyActionOptions = action => {
    var {
        icon: icon,
        onClick: onClick,
        label: label,
        visible: visible,
        index: index,
        onContentReady: onContentReady,
        activeStateEnabled: activeStateEnabled,
        hoverStateEnabled: hoverStateEnabled,
        animation: animation,
        id: id,
        actions: actions,
        actionComponent: actionComponent,
        actionVisible: actionVisible,
        zIndex: zIndex,
        position: position,
        hint: hint,
        parentPosition: parentPosition,
        direction: direction
    } = action.option();
    return extend({}, {
        icon: icon,
        onClick: onClick,
        label: label,
        visible: visible,
        index: index,
        onContentReady: onContentReady,
        activeStateEnabled: activeStateEnabled,
        hoverStateEnabled: hoverStateEnabled,
        animation: animation,
        id: id,
        actions: actions,
        actionComponent: actionComponent,
        actionVisible: actionVisible,
        zIndex: zIndex,
        position: position,
        hint: hint,
        parentPosition: parentPosition,
        direction: direction
    }, {
        onInitialized: null,
        onDisposing: null
    })
};
class SpeedDialMainItem extends SpeedDialItem {
    _getDefaultOptions() {
        return extend(super._getDefaultOptions(), extend({
            icon: "add",
            closeIcon: "close",
            position: {
                at: "right bottom",
                my: "right bottom",
                offset: {
                    x: -16,
                    y: -16
                }
            },
            maxSpeedDialActionCount: 5,
            hint: "",
            label: "",
            direction: "auto",
            actions: [],
            activeStateEnabled: true,
            hoverStateEnabled: true,
            indent: 55,
            childIndent: 40,
            childOffset: 9,
            callOverlayRenderShading: true,
            closeOnOutsideClick: true
        }, config().floatingActionButtonConfig, {
            shading: false
        }))
    }
    _defaultOptionsRules() {
        return super._defaultOptionsRules().concat([{
            device: () => isMaterial(),
            options: {
                indent: 72,
                childIndent: 56,
                childOffset: 8
            }
        }])
    }
    _render() {
        this.$element().addClass(FAB_MAIN_CLASS);
        super._render();
        this._moveToContainer();
        this._renderCloseIcon();
        this._renderClick()
    }
    _renderLabel() {
        super._renderLabel();
        this.$element().toggleClass(FAB_MAIN_CLASS_WITH_LABEL, !!this._$label)
    }
    _renderCloseIcon() {
        this._$closeIcon = this._renderButtonIcon(this._$closeIcon, this._options.silent("closeIcon"), FAB_CLOSE_ICON_CLASS);
        this._$closeIcon.addClass(INVISIBLE_STATE_CLASS)
    }
    _renderClick() {
        this._clickAction = 1 === this._getVisibleActions().length ? this._getActionComponent()._createActionByOption("onClick") : this._createAction(this._clickHandler.bind(this));
        this._setClickAction()
    }
    _getVisibleActions(actions) {
        var currentActions = actions || this.option("actions");
        return currentActions.filter(action => action.option("visible"))
    }
    _getCurrentOptions(actions) {
        var visibleActions = speedDialMainItem._getVisibleActions(actions);
        var defaultOptions = this._getDefaultOptions();
        delete defaultOptions.elementAttr;
        return 1 === visibleActions.length ? extend(modifyActionOptions(visibleActions[0]), {
            position: this._getPosition()
        }) : extend(defaultOptions, {
            visible: 0 !== visibleActions.length
        })
    }
    _clickHandler() {
        var actions = this._actionItems.filter(action => action.option("actionVisible")).sort((action, nextAction) => action.option("index") - nextAction.option("index"));
        if (1 === actions.length) {
            return
        }
        var lastActionIndex = actions.length - 1;
        for (var i = 0; i < actions.length; i++) {
            actions[i].option("animation", this._getActionAnimation(actions[i], i, lastActionIndex));
            actions[i].option("position", this._getActionPosition(actions, i));
            actions[i]._$wrapper.css("position", this._$wrapper.css("position"));
            actions[i].toggle()
        }
        if (config().floatingActionButtonConfig.shading) {
            this._isShadingShown = !this.option("shading");
            this.option("shading", this._isShadingShown)
        }
        this._$icon.toggleClass(INVISIBLE_STATE_CLASS);
        this._$closeIcon.toggleClass(INVISIBLE_STATE_CLASS)
    }
    _updateZIndexStackPosition() {
        super._updateZIndexStackPosition();
        var overlayStack = this._overlayStack();
        overlayStack.push(this)
    }
    _renderActions() {
        var actions = this.option("actions");
        if (this._actionItems && this._actionItems.length) {
            this._actionItems.forEach(actionItem => {
                actionItem.dispose();
                actionItem.$element().remove()
            });
            this._actionItems = []
        }
        this._actionItems = [];
        if (1 === actions.length) {
            return
        }
        for (var i = 0; i < actions.length; i++) {
            var action = actions[i];
            var $actionElement = $("<div>").appendTo(getSwatchContainer(action.$element()));
            eventsEngine.off($actionElement, "click");
            eventsEngine.on($actionElement, "click", () => {
                this._clickHandler()
            });
            action._options.silent("actionComponent", action);
            action._options.silent("parentPosition", this._getPosition());
            action._options.silent("actionVisible", action._options.silent("visible"));
            this._actionItems.push(this._createComponent($actionElement, SpeedDialItem, extend({}, modifyActionOptions(action), {
                visible: false
            })))
        }
    }
    _getActionAnimation(action, index, lastActionIndex) {
        action._options.silent("animation.show.delay", 30 * index);
        action._options.silent("animation.hide.delay", 30 * (lastActionIndex - index));
        return action._options.silent("animation")
    }
    _getDirectionIndex(actions, direction) {
        if ("auto" === direction) {
            var contentHeight = getHeight(this.$content());
            var actionsHeight = this.initialOption("indent") + this.initialOption("childIndent") * actions.length - contentHeight;
            var offsetTop = this.$content().offset().top;
            if (actionsHeight < offsetTop) {
                return -1
            } else {
                var offsetBottom = getHeight(this._positionController._$wrapperCoveredElement) - contentHeight - offsetTop;
                return offsetTop >= offsetBottom ? -1 : 1
            }
        }
        return "down" !== direction ? -1 : 1
    }
    _getActionPosition(actions, index) {
        var action = actions[index];
        var actionOffsetXValue = this.initialOption("childOffset");
        var actionOffsetX = action._options.silent("label") && !this._$label ? this._isPositionLeft(this._getPosition()) ? actionOffsetXValue : -actionOffsetXValue : 0;
        var actionOffsetYValue = this.initialOption("indent") + this.initialOption("childIndent") * index;
        var actionOffsetY = this._getDirectionIndex(actions, this.option("direction")) * actionOffsetYValue;
        var actionPositionAtMy = action._options.silent("label") ? this._isPositionLeft(this._getPosition()) ? "left" : "right" : "center";
        return {
            of: this.$content(),
            at: actionPositionAtMy,
            my: actionPositionAtMy,
            offset: {
                x: actionOffsetX,
                y: actionOffsetY
            }
        }
    }
    _outsideClickHandler(e) {
        if (this._isShadingShown) {
            var isShadingClick = $(e.target)[0] === this._$wrapper[0];
            if (isShadingClick) {
                e.preventDefault();
                this._clickHandler()
            }
        }
    }
    _setPosition() {
        if (this.option("visible")) {
            this._hide();
            this._show()
        }
    }
    _getPosition() {
        return this._getDefaultOptions().position
    }
    _getInkRippleContainer() {
        return this.$content()
    }
    _optionChanged(args) {
        switch (args.name) {
            case "actions":
                if (this._isVisible()) {
                    this._renderIcon();
                    this._renderLabel()
                }
                this._renderCloseIcon();
                this._renderClick();
                this._renderActions();
                break;
            case "maxSpeedDialActionCount":
                this._renderActions();
                break;
            case "closeIcon":
                this._renderCloseIcon();
                break;
            case "position":
                super._optionChanged(args);
                this._setPosition();
                break;
            case "label":
                if (this._isVisible()) {
                    this._renderLabel()
                }
                this._setPosition();
                break;
            case "icon":
                if (this._isVisible()) {
                    this._renderIcon()
                }
                break;
            default:
                super._optionChanged(args)
        }
    }
}
export function initAction(newAction) {
    newAction._options.silent("onInitializing", null);
    var isActionExist = false;
    if (!speedDialMainItem) {
        var $fabMainElement = $("<div>").appendTo(getSwatchContainer(newAction.$element()));
        speedDialMainItem = newAction._createComponent($fabMainElement, SpeedDialMainItem, extend({}, modifyActionOptions(newAction), {
            actions: [newAction]
        }))
    } else {
        var savedActions = speedDialMainItem.option("actions");
        savedActions.forEach(action => {
            if (action._options.silent("id") === newAction._options.silent("id")) {
                isActionExist = true;
                return newAction
            }
        });
        speedDialMainItem._options.silent("position"), true;
        if (!isActionExist) {
            if (speedDialMainItem._getVisibleActions(savedActions).length >= speedDialMainItem.option("maxSpeedDialActionCount")) {
                newAction.dispose();
                errors.log("W1014");
                return
            }
            savedActions.push(newAction);
            speedDialMainItem.option(extend(speedDialMainItem._getCurrentOptions(savedActions), {
                actions: savedActions
            }))
        } else if (1 === savedActions.length) {
            speedDialMainItem.option(extend({}, modifyActionOptions(savedActions[0]), {
                actions: savedActions,
                position: speedDialMainItem._getPosition()
            }))
        } else {
            speedDialMainItem.option(extend(speedDialMainItem._getCurrentOptions(savedActions), {
                actions: savedActions
            }))
        }
    }
}
export function disposeAction(actionId) {
    if (!speedDialMainItem) {
        return
    }
    var savedActions = speedDialMainItem.option("actions");
    var savedActionsCount = savedActions.length;
    savedActions = savedActions.filter(action => action._options.silent("id") !== actionId);
    if (savedActionsCount === savedActions.length) {
        return
    }
    if (!savedActions.length) {
        speedDialMainItem.dispose();
        speedDialMainItem.$element().remove();
        speedDialMainItem = null
    } else if (1 === savedActions.length) {
        speedDialMainItem.option(extend({}, modifyActionOptions(savedActions[0]), {
            actions: savedActions
        }))
    } else {
        speedDialMainItem.option({
            actions: savedActions
        })
    }
}
export function repaint() {
    if (!speedDialMainItem) {
        return
    }
    var visibleActions = speedDialMainItem._getVisibleActions();
    var icon = 1 === visibleActions.length ? visibleActions[0].option("icon") : speedDialMainItem._getDefaultOptions().icon;
    var label = 1 === visibleActions.length ? visibleActions[0].option("label") : speedDialMainItem._getDefaultOptions().label;
    speedDialMainItem.option({
        actions: speedDialMainItem.option("actions"),
        icon: icon,
        closeIcon: speedDialMainItem._getDefaultOptions().closeIcon,
        position: speedDialMainItem._getPosition(),
        label: label,
        maxSpeedDialActionCount: speedDialMainItem._getDefaultOptions().maxSpeedDialActionCount,
        direction: speedDialMainItem._getDefaultOptions().direction
    })
}
