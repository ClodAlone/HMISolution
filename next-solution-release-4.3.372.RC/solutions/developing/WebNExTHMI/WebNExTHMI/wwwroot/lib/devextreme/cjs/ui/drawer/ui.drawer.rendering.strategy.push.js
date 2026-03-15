/**
 * DevExtreme (cjs/ui/drawer/ui.drawer.rendering.strategy.push.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.default = void 0;
var _uiDrawer = require("./ui.drawer.animation");
var _uiDrawerRendering = _interopRequireDefault(require("./ui.drawer.rendering.strategy"));
var _renderer = _interopRequireDefault(require("../../core/renderer"));
var _translator = require("../../animation/translator");

function _interopRequireDefault(obj) {
    return obj && obj.__esModule ? obj : {
        default: obj
    }
}

function _inheritsLoose(subClass, superClass) {
    subClass.prototype = Object.create(superClass.prototype);
    subClass.prototype.constructor = subClass;
    _setPrototypeOf(subClass, superClass)
}

function _setPrototypeOf(o, p) {
    _setPrototypeOf = Object.setPrototypeOf || function(o, p) {
        o.__proto__ = p;
        return o
    };
    return _setPrototypeOf(o, p)
}
var PushStrategy = function(_DrawerStrategy) {
    _inheritsLoose(PushStrategy, _DrawerStrategy);

    function PushStrategy() {
        return _DrawerStrategy.apply(this, arguments) || this
    }
    var _proto = PushStrategy.prototype;
    _proto._internalRenderPosition = function(changePositionUsingFxAnimation, whenAnimationCompleted) {
        var drawer = this.getDrawerInstance();
        var openedPanelSize = this._getPanelSize(true);
        var contentPosition = this._getPanelSize(drawer.option("opened")) * drawer._getPositionCorrection();
        (0, _renderer.default)(drawer.content()).css(drawer.isHorizontalDirection() ? "width" : "height", openedPanelSize);
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
            }(0, _renderer.default)(drawer.viewContent()).css(paddingCssPropertyName, drawer.getMinSize())
        }
        if (changePositionUsingFxAnimation) {
            _uiDrawer.animation.moveTo({
                $element: (0, _renderer.default)(drawer.viewContent()),
                position: contentPosition,
                direction: drawer.calcTargetPosition(),
                duration: drawer.option("animationDuration"),
                complete: function() {
                    whenAnimationCompleted.resolve()
                }
            })
        } else if (drawer.isHorizontalDirection()) {
            (0, _translator.move)((0, _renderer.default)(drawer.viewContent()), {
                left: contentPosition
            })
        } else {
            (0, _translator.move)((0, _renderer.default)(drawer.viewContent()), {
                top: contentPosition
            })
        }
    };
    _proto.onPanelContentRendered = function() {
        (0, _renderer.default)(this.getDrawerInstance().viewContent()).addClass("dx-theme-background-color")
    };
    return PushStrategy
}(_uiDrawerRendering.default);
var _default = PushStrategy;
exports.default = _default;
module.exports = exports.default;
module.exports.default = exports.default;
