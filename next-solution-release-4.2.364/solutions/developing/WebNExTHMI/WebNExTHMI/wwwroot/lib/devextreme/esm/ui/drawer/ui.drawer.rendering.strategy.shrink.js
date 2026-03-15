/**
 * DevExtreme (esm/ui/drawer/ui.drawer.rendering.strategy.shrink.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import {
    animation
} from "./ui.drawer.animation";
import DrawerStrategy from "./ui.drawer.rendering.strategy";
import $ from "../../core/renderer";
import {
    camelize
} from "../../core/utils/inflector";
class ShrinkStrategy extends DrawerStrategy {
    _internalRenderPosition(changePositionUsingFxAnimation, whenAnimationCompleted) {
        var drawer = this.getDrawerInstance();
        var direction = drawer.calcTargetPosition();
        var $panel = $(drawer.content());
        var panelSize = this._getPanelSize(drawer.option("opened"));
        var panelOffset = this._getPanelOffset(drawer.option("opened"));
        var revealMode = drawer.option("revealMode");
        if (changePositionUsingFxAnimation) {
            if ("slide" === revealMode) {
                animation.margin({
                    complete: () => {
                        whenAnimationCompleted.resolve()
                    },
                    $element: $panel,
                    duration: drawer.option("animationDuration"),
                    direction: direction,
                    margin: panelOffset
                })
            } else if ("expand" === revealMode) {
                animation.size({
                    complete: () => {
                        whenAnimationCompleted.resolve()
                    },
                    $element: $panel,
                    duration: drawer.option("animationDuration"),
                    direction: direction,
                    size: panelSize
                })
            }
        } else if ("slide" === revealMode) {
            $panel.css("margin" + camelize(direction, true), panelOffset)
        } else if ("expand" === revealMode) {
            $panel.css(drawer.isHorizontalDirection() ? "width" : "height", panelSize)
        }
    }
    isViewContentFirst(position, isRtl) {
        return (isRtl ? "left" === position : "right" === position) || "bottom" === position
    }
}
export default ShrinkStrategy;
