/**
 * DevExtreme (esm/ui/popup/popup_position_controller.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import _extends from "@babel/runtime/helpers/esm/extends";
import _objectWithoutPropertiesLoose from "@babel/runtime/helpers/esm/objectWithoutPropertiesLoose";
var _excluded = ["fullScreen", "forceApplyBindings"];
import $ from "../../core/renderer";
import {
    move
} from "../../animation/translator";
import {
    getWindow
} from "../../core/utils/window";
import {
    OverlayPositionController
} from "../overlay/overlay_position_controller";
var window = getWindow();
class PopupPositionController extends OverlayPositionController {
    constructor(_ref) {
        var {
            fullScreen: fullScreen,
            forceApplyBindings: forceApplyBindings
        } = _ref, args = _objectWithoutPropertiesLoose(_ref, _excluded);
        super(args);
        this._props = _extends({}, this._props, {
            fullScreen: fullScreen,
            forceApplyBindings: forceApplyBindings
        });
        this._lastPositionBeforeFullScreen = void 0
    }
    set fullScreen(fullScreen) {
        this._props.fullScreen = fullScreen;
        if (fullScreen) {
            this._fullScreenEnabled()
        } else {
            this._fullScreenDisabled()
        }
    }
    positionContent() {
        if (this._props.fullScreen) {
            move(this._$content, {
                top: 0,
                left: 0
            });
            this.detectVisualPositionChange()
        } else {
            var _this$_props$forceApp, _this$_props;
            null === (_this$_props$forceApp = (_this$_props = this._props).forceApplyBindings) || void 0 === _this$_props$forceApp ? void 0 : _this$_props$forceApp.call(_this$_props);
            if (!this._shouldRenderContentInitialPosition && this._lastPositionBeforeFullScreen) {
                move(this._$content, this._lastPositionBeforeFullScreen);
                this._lastPositionBeforeFullScreen = void 0;
                this.detectVisualPositionChange()
            } else {
                super.positionContent()
            }
        }
    }
    _getWrapperCoveredElement() {
        if (this._props.fullScreen) {
            return $(window)
        }
        return super._getWrapperCoveredElement()
    }
    _fullScreenEnabled() {
        this.restorePositionOnNextRender(false);
        this._lastPositionBeforeFullScreen = this._visualPosition
    }
    _fullScreenDisabled() {
        this.restorePositionOnNextRender(false)
    }
}
export {
    PopupPositionController
};
