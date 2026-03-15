/**
 * DevExtreme (esm/ui/drawer/ui.drawer.rendering.strategy.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import {
    setWidth,
    setHeight
} from "../../core/utils/size";
import $ from "../../core/renderer";
import {
    animation
} from "./ui.drawer.animation";
import {
    Deferred,
    when
} from "../../core/utils/deferred";
class DrawerStrategy {
    constructor(drawer) {
        this._drawer = drawer
    }
    getDrawerInstance() {
        return this._drawer
    }
    renderPanelContent(whenPanelContentRendered) {
        var drawer = this.getDrawerInstance();
        var template = drawer._getTemplate(drawer.option("template"));
        if (template) {
            template.render({
                container: drawer.content(),
                onRendered: () => {
                    whenPanelContentRendered.resolve()
                }
            })
        }
    }
    renderPosition(changePositionUsingFxAnimation, animationDuration) {
        var whenPositionAnimationCompleted = new Deferred;
        var whenShaderAnimationCompleted = new Deferred;
        var drawer = this.getDrawerInstance();
        if (changePositionUsingFxAnimation) {
            when.apply($, [whenPositionAnimationCompleted, whenShaderAnimationCompleted]).done(() => {
                drawer._animationCompleteHandler()
            })
        }
        this._internalRenderPosition(changePositionUsingFxAnimation, whenPositionAnimationCompleted);
        if (!changePositionUsingFxAnimation) {
            drawer.resizeViewContent()
        }
        this.renderShaderVisibility(changePositionUsingFxAnimation, animationDuration, whenShaderAnimationCompleted)
    }
    _getPanelOffset(isDrawerOpened) {
        var drawer = this.getDrawerInstance();
        var size = drawer.isHorizontalDirection() ? drawer.getRealPanelWidth() : drawer.getRealPanelHeight();
        if (isDrawerOpened) {
            return -(size - drawer.getMaxSize())
        } else {
            return -(size - drawer.getMinSize())
        }
    }
    _getPanelSize(isDrawerOpened) {
        return isDrawerOpened ? this.getDrawerInstance().getMaxSize() : this.getDrawerInstance().getMinSize()
    }
    renderShaderVisibility(changePositionUsingFxAnimation, duration, whenAnimationCompleted) {
        var drawer = this.getDrawerInstance();
        var isShaderVisible = drawer.option("opened");
        var fadeConfig = isShaderVisible ? {
            from: 0,
            to: 1
        } : {
            from: 1,
            to: 0
        };
        if (changePositionUsingFxAnimation) {
            animation.fade($(drawer._$shader), fadeConfig, duration, () => {
                this._drawer._toggleShaderVisibility(isShaderVisible);
                whenAnimationCompleted.resolve()
            })
        } else {
            drawer._toggleShaderVisibility(isShaderVisible);
            drawer._$shader.css("opacity", fadeConfig.to)
        }
    }
    getPanelContent() {
        return $(this.getDrawerInstance().content())
    }
    setPanelSize(calcFromRealPanelSize) {
        this.refreshPanelElementSize(calcFromRealPanelSize)
    }
    refreshPanelElementSize(calcFromRealPanelSize) {
        var drawer = this.getDrawerInstance();
        var panelSize = this._getPanelSize(drawer.option("opened"));
        if (drawer.isHorizontalDirection()) {
            setWidth($(drawer.content()), calcFromRealPanelSize ? drawer.getRealPanelWidth() : panelSize)
        } else {
            setHeight($(drawer.content()), calcFromRealPanelSize ? drawer.getRealPanelHeight() : panelSize)
        }
    }
    isViewContentFirst() {
        return false
    }
    onPanelContentRendered() {}
}
export default DrawerStrategy;
