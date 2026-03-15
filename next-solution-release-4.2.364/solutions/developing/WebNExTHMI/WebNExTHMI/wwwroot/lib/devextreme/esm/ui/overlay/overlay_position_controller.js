/**
 * DevExtreme (esm/ui/overlay/overlay_position_controller.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import $ from "../../core/renderer";
import {
    isDefined,
    isString,
    isEvent,
    isWindow
} from "../../core/utils/type";
import {
    extend
} from "../../core/utils/extend";
import positionUtils from "../../animation/position";
import {
    resetPosition,
    move,
    locate
} from "../../animation/translator";
import {
    getWindow
} from "../../core/utils/window";
import {
    originalViewPort,
    value as viewPort
} from "../../core/utils/view_port";
var window = getWindow();
var OVERLAY_POSITION_ALIASES = {
    top: {
        my: "top center",
        at: "top center"
    },
    bottom: {
        my: "bottom center",
        at: "bottom center"
    },
    right: {
        my: "right center",
        at: "right center"
    },
    left: {
        my: "left center",
        at: "left center"
    },
    center: {
        my: "center",
        at: "center"
    },
    "right bottom": {
        my: "right bottom",
        at: "right bottom"
    },
    "right top": {
        my: "right top",
        at: "right top"
    },
    "left bottom": {
        my: "left bottom",
        at: "left bottom"
    },
    "left top": {
        my: "left top",
        at: "left top"
    }
};
var OVERLAY_DEFAULT_BOUNDARY_OFFSET = {
    h: 0,
    v: 0
};
class OverlayPositionController {
    constructor(_ref) {
        var {
            position: position,
            target: target,
            container: container,
            $root: $root,
            $content: $content,
            $wrapper: $wrapper,
            onPositioned: onPositioned,
            onVisualPositionChanged: onVisualPositionChanged,
            dragOutsideBoundary: dragOutsideBoundary,
            dragAndResizeArea: dragAndResizeArea,
            outsideDragFactor: outsideDragFactor,
            restorePosition: restorePosition,
            _fixWrapperPosition: _fixWrapperPosition
        } = _ref;
        this._props = {
            position: position,
            target: target,
            container: container,
            dragOutsideBoundary: dragOutsideBoundary,
            dragAndResizeArea: dragAndResizeArea,
            outsideDragFactor: outsideDragFactor,
            restorePosition: restorePosition,
            onPositioned: onPositioned,
            onVisualPositionChanged: onVisualPositionChanged,
            _fixWrapperPosition: _fixWrapperPosition
        };
        this._$root = $root;
        this._$content = $content;
        this._$wrapper = $wrapper;
        this._$markupContainer = void 0;
        this._$wrapperCoveredElement = void 0;
        this._shouldRenderContentInitialPosition = true;
        this._visualPosition = void 0;
        this._initialPosition = void 0;
        this._previousVisualPosition = void 0;
        this._$dragResizeContainer = void 0;
        this._outsideDragFactor = void 0;
        this.updateContainer(container);
        this.updatePosition(position, target);
        this._updateDragResizeContainer();
        this._updateOutsideDragFactor()
    }
    get $container() {
        return this._$markupContainer
    }
    get $dragResizeContainer() {
        return this._$dragResizeContainer
    }
    get outsideDragFactor() {
        return this._outsideDragFactor
    }
    set fixWrapperPosition(fixWrapperPosition) {
        this._props._fixWrapperPosition = fixWrapperPosition;
        this.styleWrapperPosition()
    }
    set dragAndResizeArea(dragAndResizeArea) {
        this._props.dragAndResizeArea = dragAndResizeArea;
        this._updateDragResizeContainer()
    }
    set dragOutsideBoundary(dragOutsideBoundary) {
        this._props.dragOutsideBoundary = dragOutsideBoundary;
        this._updateDragResizeContainer();
        this._updateOutsideDragFactor()
    }
    set outsideDragFactor(outsideDragFactor) {
        this._props.outsideDragFactor = outsideDragFactor;
        this._updateOutsideDragFactor()
    }
    set restorePosition(restorePosition) {
        this._props.restorePosition = restorePosition
    }
    restorePositionOnNextRender(value) {
        this._shouldRenderContentInitialPosition = value || !this._visualPosition
    }
    openingHandled() {
        var shouldRestorePosition = this._props.restorePosition;
        this.restorePositionOnNextRender(shouldRestorePosition)
    }
    dragHandled() {
        this.restorePositionOnNextRender(false)
    }
    resizeHandled() {
        this.restorePositionOnNextRender(false)
    }
    updateTarget(target) {
        this._props.target = target;
        this.updatePosition(this._props.position, target)
    }
    updatePosition(positionProp) {
        var targetProp = arguments.length > 1 && void 0 !== arguments[1] ? arguments[1] : this._props.target;
        this._props.position = positionProp;
        this._position = this._normalizePosition(positionProp, targetProp);
        this._updateWrapperCoveredElement()
    }
    updateContainer(containerProp) {
        this._props.container = containerProp;
        var container = null !== containerProp && void 0 !== containerProp ? containerProp : viewPort();
        var $container = this._$root.closest(container);
        if (!$container.length) {
            $container = $(container).first()
        }
        this._$markupContainer = $container.length ? $container : this._$root.parent();
        this._updateWrapperCoveredElement();
        this._updateDragResizeContainer()
    }
    detectVisualPositionChange(event) {
        this._updateVisualPositionValue();
        this._raisePositionedEvents(event)
    }
    positionContent() {
        if (this._shouldRenderContentInitialPosition) {
            this._renderContentInitialPosition()
        } else {
            move(this._$content, this._visualPosition);
            this.detectVisualPositionChange()
        }
    }
    positionWrapper() {
        if (this._$wrapperCoveredElement) {
            positionUtils.setup(this._$wrapper, {
                my: "top left",
                at: "top left",
                of: this._$wrapperCoveredElement
            })
        }
    }
    isAllWindowCoveredByWrapper() {
        return !this._$wrapperCoveredElement || isWindow(this._$wrapperCoveredElement.get(0))
    }
    styleWrapperPosition() {
        var useFixed = this.isAllWindowCoveredByWrapper() || this._props._fixWrapperPosition;
        var positionStyle = useFixed ? "fixed" : "absolute";
        this._$wrapper.css("position", positionStyle)
    }
    _updateVisualPositionValue() {
        this._previousVisualPosition = this._visualPosition;
        this._visualPosition = locate(this._$content)
    }
    _renderContentInitialPosition() {
        this._renderBoundaryOffset();
        resetPosition(this._$content);
        var wrapperOverflow = this._$wrapper.css("overflow");
        this._$wrapper.css("overflow", "hidden");
        var resultPosition = positionUtils.setup(this._$content, this._position);
        this._$wrapper.css("overflow", wrapperOverflow);
        this._initialPosition = resultPosition;
        this.detectVisualPositionChange()
    }
    _raisePositionedEvents(event) {
        var previousPosition = this._previousVisualPosition;
        var newPosition = this._visualPosition;
        var isVisualPositionChanged = (null === previousPosition || void 0 === previousPosition ? void 0 : previousPosition.top) !== newPosition.top || (null === previousPosition || void 0 === previousPosition ? void 0 : previousPosition.left) !== newPosition.left;
        if (isVisualPositionChanged) {
            this._props.onVisualPositionChanged({
                previousPosition: previousPosition,
                position: newPosition,
                event: event
            })
        }
        this._props.onPositioned({
            position: this._initialPosition
        })
    }
    _updateOutsideDragFactor() {
        this._outsideDragFactor = this._getOutsideDragFactor()
    }
    _getOutsideDragFactor() {
        if (this._props.dragOutsideBoundary) {
            return 1
        }
        return this._props.outsideDragFactor
    }
    _updateDragResizeContainer() {
        this._$dragResizeContainer = this._getDragResizeContainer()
    }
    _getDragResizeContainer() {
        if (this._props.dragOutsideBoundary) {
            return $(window)
        }
        if (this._props.dragAndResizeArea) {
            return $(this._props.dragAndResizeArea)
        }
        var isContainerDefined = originalViewPort().get(0) || this._props.container;
        return isContainerDefined ? this._$markupContainer : $(window)
    }
    _updateWrapperCoveredElement() {
        this._$wrapperCoveredElement = this._getWrapperCoveredElement()
    }
    _renderBoundaryOffset() {
        var _this$_position;
        var boundaryOffset = null !== (_this$_position = this._position) && void 0 !== _this$_position ? _this$_position : {
            boundaryOffset: OVERLAY_DEFAULT_BOUNDARY_OFFSET
        };
        this._$content.css("margin", "".concat(boundaryOffset.v, "px ").concat(boundaryOffset.h, "px"))
    }
    _getWrapperCoveredElement() {
        var containerProp = this._props.container;
        if (containerProp) {
            return $(containerProp)
        }
        if (this._position) {
            return $(isEvent(this._position.of) ? window : this._position.of || window)
        }
    }
    _normalizePosition(positionProp, targetProp) {
        var defaultPositionConfig = {
            of: targetProp,
            boundaryOffset: OVERLAY_DEFAULT_BOUNDARY_OFFSET
        };
        if (isDefined(positionProp)) {
            return extend(true, {}, defaultPositionConfig, this._positionToObject(positionProp))
        } else {
            return defaultPositionConfig
        }
    }
    _positionToObject(position) {
        if (isString(position)) {
            return extend({}, OVERLAY_POSITION_ALIASES[position])
        }
        return position
    }
}
export {
    OVERLAY_POSITION_ALIASES,
    OverlayPositionController
};
