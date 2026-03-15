/**
 * DevExtreme (esm/ui/drawer/ui.drawer.rendering.strategy.push.js)
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
    move
} from "../../animation/translator";
class PushStrategy extends DrawerStrategy {
    _internalRenderPosition(changePositionUsingFxAnimation, whenAnimationCompleted) {
        var drawer = this.getDrawerInstance();
        var openedPanelSize = this._getPanelSize(true);
        var contentPosition = this._getPanelSize(drawer.option("opened")) * drawer._getPositionCorrection();
        $(drawer.content()).css(drawer.isHorizontalDirection() ? "width" : "height", openedPanelSize);
        if (drawer.getMinSize()) {
            var paddingCssPropertyName = "padding";
            switch (drawer.calcTargetPosition()) {
                case "left":
                    paddingCssPropertyName += "Right";
                    break;
                case "right":
                    paddingCssPropertyName += "Left";
                    break;
                case "top":
                    paddingCssPropertyName += "Bottom";
                    break;
                case "bottom":
                    paddingCssPropertyName += "Top"
            }
            $(drawer.viewContent()).css(paddingCssPropertyName, drawer.getMinSize())
        }
        if (changePositionUsingFxAnimation) {
            animation.moveTo({
                $element: $(drawer.viewContent()),
                position: contentPosition,
                direction: drawer.calcTargetPosition(),
                duration: drawer.option("animationDuration"),
                complete: () => {
                    whenAnimationCompleted.resolve()
                }
            })
        } else if (drawer.isHorizontalDirection()) {
            move($(drawer.viewContent()), {
                left: contentPosition
            })
        } else {
            move($(drawer.viewContent()), {
                top: contentPosition
            })
        }
    }
    onPanelContentRendered() {
        $(this.getDrawerInstance().viewContent()).addClass("dx-theme-background-color")
    }
}
export default PushStrategy;
