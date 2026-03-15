/**
 * DevExtreme (cjs/ui/drawer/ui.drawer.rendering.strategy.overlap.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.default = void 0;
var _size = require("../../core/utils/size");
var _uiDrawer = require("./ui.drawer.animation");
var _uiDrawerRendering = _interopRequireDefault(require("./ui.drawer.rendering.strategy"));
var _renderer = _interopRequireDefault(require("../../core/renderer"));
var _translator = require("../../animation/translator");
var _ui = _interopRequireDefault(require("../overlay/ui.overlay"));
var _common = require("../../core/utils/common");
var _inflector = require("../../core/utils/inflector");

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
var OverlapStrategy = function(_DrawerStrategy) {
    _inheritsLoose(OverlapStrategy, _DrawerStrategy);

    function OverlapStrategy() {
        return _DrawerStrategy.apply(this, arguments) || this
    }
    var _proto = OverlapStrategy.prototype;
    _proto.renderPanelContent = function(whenPanelContentRendered) {
        var _this = this;
        delete this._initialPosition;
        var drawer = this.getDrawerInstance();
        var _drawer$option = drawer.option(),
            opened = _drawer$option.opened,
            minSize = _drawer$option.minSize;
        drawer._overlay = drawer._createComponent(drawer.content(), _ui.default, {
            shading: false,
            container: drawer.getOverlayTarget(),
            position: this._getOverlayPosition(),
            width: opened ? "auto" : minSize || 0,
            height: "100%",
            templatesRenderAsynchronously: drawer.option("templatesRenderAsynchronously"),
            animation: {
                show: {
                    duration: 0
                }
            },
            onPositioned: function(e) {
                this._fixOverlayPosition(e.component.$content())
            }.bind(this),
            contentTemplate: drawer.option("template"),
            onContentReady: function(args) {
                whenPanelContentRendered.resolve();
                _this._processOverlayZIndex(args.component.content())
            },
            visible: true,
            propagateOutsideClick: true,
            useResizeObserver: false
        })
    };
    _proto._fixOverlayPosition = function($overlayContent) {
        var position = (0, _common.ensureDefined)(this._initialPosition, {
            left: 0,
            top: 0
        });
        (0, _translator.move)($overlayContent, position);
        if ("right" === this.getDrawerInstance().calcTargetPosition()) {
            $overlayContent.css("left", "auto")
        }
        if ("bottom" === this.getDrawerInstance().calcTargetPosition()) {
            $overlayContent.css("top", "auto");
            $overlayContent.css("bottom", "0px")
        }
    };
    _proto._getOverlayPosition = function() {
        var drawer = this.getDrawerInstance();
        var panelPosition = drawer.calcTargetPosition();
        var result = {};
        switch (panelPosition) {
            case "left":
                result = {
                    my: "top left",
                    at: "top left"
                };
                break;
            case "right":
                result = {
                    my: drawer.option("rtlEnabled") ? "top left" : "top right",
                    at: "top right"
                };
                break;
            case "top":
            case "bottom":
                result = {
                    my: panelPosition,
                    at: panelPosition
                }
        }
        result.of = drawer.getOverlayTarget();
        return result
    };
    _proto.refreshPanelElementSize = function(calcFromRealPanelSize) {
        var drawer = this.getDrawerInstance();
        var overlay = drawer.getOverlay();
        if (drawer.isHorizontalDirection()) {
            overlay.option("height", "100%");
            overlay.option("width", calcFromRealPanelSize ? drawer.getRealPanelWidth() : this._getPanelSize(drawer.option("opened")))
        } else {
            overlay.option("width", (0, _size.getWidth)(overlay.option("container")));
            overlay.option("height", calcFromRealPanelSize ? drawer.getRealPanelHeight() : this._getPanelSize(drawer.option("opened")))
        }
    };
    _proto.onPanelContentRendered = function() {
        this._updateViewContentStyles()
    };
    _proto._updateViewContentStyles = function() {
        var drawer = this.getDrawerInstance();
        (0, _renderer.default)(drawer.viewContent()).css("padding" + (0, _inflector.camelize)(drawer.calcTargetPosition(), true), drawer.option("minSize"));
        (0, _renderer.default)(drawer.viewContent()).css("transform", "inherit")
    };
    _proto._internalRenderPosition = function(changePositionUsingFxAnimation, whenAnimationCompleted) {
        var drawer = this.getDrawerInstance();
        var $panel = (0, _renderer.default)(drawer.content());
        var $panelOverlayContent = drawer.getOverlay().$content();
        var revealMode = drawer.option("revealMode");
        var targetPanelPosition = drawer.calcTargetPosition();
        var panelSize = this._getPanelSize(drawer.option("opened"));
        var panelOffset = this._getPanelOffset(drawer.option("opened")) * drawer._getPositionCorrection();
        var marginTop = drawer.getRealPanelHeight() - panelSize;
        this._updateViewContentStyles();
        if (changePositionUsingFxAnimation) {
            if ("slide" === revealMode) {
                this._initialPosition = drawer.isHorizontalDirection() ? {
                    left: panelOffset
                } : {
                    top: panelOffset
                };
                _uiDrawer.animation.moveTo({
                    complete: function() {
                        whenAnimationCompleted.resolve()
                    },
                    duration: drawer.option("animationDuration"),
                    direction: targetPanelPosition,
                    $element: $panel,
                    position: panelOffset
                })
            } else if ("expand" === revealMode) {
                this._initialPosition = {
                    left: 0
                };
                (0, _translator.move)($panelOverlayContent, this._initialPosition);
                _uiDrawer.animation.size({
                    complete: function() {
                        whenAnimationCompleted.resolve()
                    },
                    duration: drawer.option("animationDuration"),
                    direction: targetPanelPosition,
                    $element: $panelOverlayContent,
                    size: panelSize,
                    marginTop: marginTop
                })
            }
        } else if ("slide" === revealMode) {
            this._initialPosition = drawer.isHorizontalDirection() ? {
                left: panelOffset
            } : {
                top: panelOffset
            };
            (0, _translator.move)($panel, this._initialPosition)
        } else if ("expand" === revealMode) {
            this._initialPosition = {
                left: 0
            };
            (0, _translator.move)($panelOverlayContent, this._initialPosition);
            if (drawer.isHorizontalDirection()) {
                (0, _renderer.default)($panelOverlayContent).css("width", panelSize)
            } else {
                (0, _renderer.default)($panelOverlayContent).css("height", panelSize);
                if ("bottom" === targetPanelPosition) {
                    (0, _renderer.default)($panelOverlayContent).css("marginTop", marginTop)
                }
            }
        }
    };
    _proto.getPanelContent = function() {
        return (0, _renderer.default)(this.getDrawerInstance().getOverlay().content())
    };
    _proto._processOverlayZIndex = function($element) {
        var styles = (0, _renderer.default)($element).get(0).style;
        var zIndex = styles.zIndex || 1;
        this.getDrawerInstance().setZIndex(zIndex)
    };
    _proto.isViewContentFirst = function(position) {
        return "right" === position || "bottom" === position
    };
    return OverlapStrategy
}(_uiDrawerRendering.default);
var _default = OverlapStrategy;
exports.default = _default;
module.exports = exports.default;
module.exports.default = exports.default;
