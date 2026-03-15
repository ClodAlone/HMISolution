/**
 * DevExtreme (esm/renovation/ui/scroll_view/scrollbar/scrollbar.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import _extends from "@babel/runtime/helpers/esm/extends";
import _objectWithoutPropertiesLoose from "@babel/runtime/helpers/esm/objectWithoutPropertiesLoose";
var _excluded = ["bounceEnabled", "containerHasSizes", "containerSize", "contentSize", "direction", "maxOffset", "minOffset", "rtlEnabled", "scrollByThumb", "scrollLocation", "scrollLocationChange", "showScrollbar", "visible"];
import {
    createVNode
} from "inferno";
import {
    InfernoEffect,
    InfernoComponent,
    normalizeStyles
} from "@devextreme/runtime/inferno";
import {
    combineClasses
} from "../../../utils/combine_classes";
import domAdapter from "../../../../core/dom_adapter";
import {
    DIRECTION_HORIZONTAL,
    SCROLLABLE_SCROLLBAR_CLASS,
    SCROLLABLE_SCROLL_CLASS,
    SCROLLABLE_SCROLL_CONTENT_CLASS,
    SCROLLABLE_SCROLLBAR_ACTIVE_CLASS,
    HOVER_ENABLED_STATE,
    ShowScrollbarMode
} from "../common/consts";
import {
    subscribeToDXPointerDownEvent,
    subscribeToDXPointerUpEvent,
    subscribeToMouseEnterEvent,
    subscribeToMouseLeaveEvent
} from "../../../utils/subscribe_to_event";
import {
    BaseWidgetProps
} from "../../common/base_props";
import {
    ScrollbarProps
} from "../common/scrollbar_props";
import {
    ScrollableSimulatedProps
} from "../common/simulated_strategy_props";
export var THUMB_MIN_SIZE = 15;
export var viewFunction = viewModel => {
    var {
        hidden: hidden,
        scrollbarClasses: scrollbarClasses,
        scrollbarRef: scrollbarRef,
        thumbClasses: thumbClasses,
        thumbRef: thumbRef,
        thumbStyles: thumbStyles
    } = viewModel;
    return createVNode(1, "div", scrollbarClasses, createVNode(1, "div", thumbClasses, createVNode(1, "div", SCROLLABLE_SCROLL_CONTENT_CLASS), 2, {
        style: normalizeStyles(thumbStyles)
    }, null, thumbRef), 2, {
        hidden: hidden
    }, null, scrollbarRef)
};
export var ScrollbarPropsType = {
    get direction() {
        return ScrollbarProps.direction
    },
    get containerSize() {
        return ScrollbarProps.containerSize
    },
    get contentSize() {
        return ScrollbarProps.contentSize
    },
    get visible() {
        return ScrollbarProps.visible
    },
    get containerHasSizes() {
        return ScrollbarProps.containerHasSizes
    },
    get scrollLocation() {
        return ScrollbarProps.scrollLocation
    },
    get minOffset() {
        return ScrollbarProps.minOffset
    },
    get maxOffset() {
        return ScrollbarProps.maxOffset
    },
    get rtlEnabled() {
        return BaseWidgetProps.rtlEnabled
    },
    get showScrollbar() {
        return ScrollableSimulatedProps.showScrollbar
    },
    get scrollByThumb() {
        return ScrollableSimulatedProps.scrollByThumb
    },
    get bounceEnabled() {
        return ScrollableSimulatedProps.bounceEnabled
    }
};
import {
    createRef as infernoCreateRef
} from "inferno";
export class Scrollbar extends InfernoComponent {
    constructor(props) {
        super(props);
        this.scrollbarRef = infernoCreateRef();
        this.scrollRef = infernoCreateRef();
        this.rightScrollLocation = 0;
        this.prevScrollLocation = 0;
        this.prevMaxOffset = 0;
        this.thumbRef = infernoCreateRef();
        this.__getterCache = {};
        this.state = {
            hovered: false,
            active: false
        };
        this.pointerDownEffect = this.pointerDownEffect.bind(this);
        this.pointerUpEffect = this.pointerUpEffect.bind(this);
        this.mouseEnterEffect = this.mouseEnterEffect.bind(this);
        this.mouseLeaveEffect = this.mouseLeaveEffect.bind(this);
        this.isThumb = this.isThumb.bind(this);
        this.isScrollbar = this.isScrollbar.bind(this);
        this.setActiveState = this.setActiveState.bind(this);
        this.moveToMouseLocation = this.moveToMouseLocation.bind(this);
        this.moveTo = this.moveTo.bind(this);
        this.syncScrollLocation = this.syncScrollLocation.bind(this)
    }
    createEffects() {
        return [new InfernoEffect(this.pointerDownEffect, []), new InfernoEffect(this.pointerUpEffect, []), new InfernoEffect(this.mouseEnterEffect, [this.props.showScrollbar, this.props.scrollByThumb]), new InfernoEffect(this.mouseLeaveEffect, [this.props.showScrollbar, this.props.scrollByThumb]), new InfernoEffect(this.syncScrollLocation, [this.props.containerHasSizes, this.props.scrollLocation, this.props.maxOffset, this.props.direction, this.props.rtlEnabled, this.props.scrollLocationChange])]
    }
    updateEffects() {
        var _this$_effects$, _this$_effects$2, _this$_effects$3;
        null === (_this$_effects$ = this._effects[2]) || void 0 === _this$_effects$ ? void 0 : _this$_effects$.update([this.props.showScrollbar, this.props.scrollByThumb]);
        null === (_this$_effects$2 = this._effects[3]) || void 0 === _this$_effects$2 ? void 0 : _this$_effects$2.update([this.props.showScrollbar, this.props.scrollByThumb]);
        null === (_this$_effects$3 = this._effects[4]) || void 0 === _this$_effects$3 ? void 0 : _this$_effects$3.update([this.props.containerHasSizes, this.props.scrollLocation, this.props.maxOffset, this.props.direction, this.props.rtlEnabled, this.props.scrollLocationChange])
    }
    pointerDownEffect() {
        return subscribeToDXPointerDownEvent(this.thumbRef.current, () => {
            this.setState(__state_argument => ({
                active: true
            }))
        })
    }
    pointerUpEffect() {
        return subscribeToDXPointerUpEvent(domAdapter.getDocument(), () => {
            this.setState(__state_argument => ({
                active: false
            }))
        })
    }
    mouseEnterEffect() {
        if (this.isExpandable) {
            return subscribeToMouseEnterEvent(this.scrollbarRef.current, () => {
                this.setState(__state_argument => ({
                    hovered: true
                }))
            })
        }
        return
    }
    mouseLeaveEffect() {
        if (this.isExpandable) {
            return subscribeToMouseLeaveEvent(this.scrollbarRef.current, () => {
                this.setState(__state_argument => ({
                    hovered: false
                }))
            })
        }
        return
    }
    syncScrollLocation() {
        if (this.props.containerHasSizes) {
            var newScrollLocation = this.props.scrollLocation;
            var maxOffsetChanged = Math.abs(this.props.maxOffset - this.prevMaxOffset) > 0;
            this.prevMaxOffset = this.props.maxOffset;
            if (maxOffsetChanged && this.isHorizontal && this.props.rtlEnabled) {
                if (0 === this.props.maxOffset) {
                    this.rightScrollLocation = 0
                }
                newScrollLocation = this.props.maxOffset - this.rightScrollLocation
            }
            if (this.prevScrollLocation !== newScrollLocation) {
                this.moveTo(newScrollLocation)
            }
        }
    }
    get axis() {
        return this.isHorizontal ? "x" : "y"
    }
    get fullScrollProp() {
        return this.isHorizontal ? "scrollLeft" : "scrollTop"
    }
    get dimension() {
        return this.isHorizontal ? "width" : "height"
    }
    get isHorizontal() {
        return this.props.direction === DIRECTION_HORIZONTAL
    }
    get scrollSize() {
        return Math.max(this.props.containerSize * this.containerToContentRatio, THUMB_MIN_SIZE)
    }
    get containerToContentRatio() {
        return this.props.contentSize ? this.props.containerSize / this.props.contentSize : this.props.containerSize
    }
    get scrollRatio() {
        var scrollOffsetMax = Math.abs(this.props.maxOffset);
        if (scrollOffsetMax) {
            return (this.props.containerSize - this.scrollSize) / scrollOffsetMax
        }
        return 1
    }
    get scrollbarClasses() {
        var classesMap = {
            [SCROLLABLE_SCROLLBAR_CLASS]: true,
            ["dx-scrollbar-".concat(this.props.direction)]: true,
            [SCROLLABLE_SCROLLBAR_ACTIVE_CLASS]: this.state.active,
            [HOVER_ENABLED_STATE]: this.isExpandable,
            "dx-state-invisible": this.hidden,
            "dx-state-hover": this.isExpandable && this.state.hovered
        };
        return combineClasses(classesMap)
    }
    get thumbStyles() {
        if (void 0 !== this.__getterCache.thumbStyles) {
            return this.__getterCache.thumbStyles
        }
        return this.__getterCache.thumbStyles = (() => ({
            [this.dimension]: Math.round(this.scrollSize) || THUMB_MIN_SIZE,
            transform: this.isNeverMode ? "none" : this.thumbTransform
        }))()
    }
    get thumbTransform() {
        var translateValue = -this.props.scrollLocation * this.scrollRatio;
        if (this.isHorizontal) {
            return "translate(".concat(translateValue, "px, 0px)")
        }
        return "translate(0px, ".concat(translateValue, "px)")
    }
    get thumbClasses() {
        return combineClasses({
            [SCROLLABLE_SCROLL_CLASS]: true,
            "dx-state-invisible": !this.isThumbVisible
        })
    }
    get hidden() {
        return this.isNeverMode || 0 === this.props.maxOffset || this.props.containerSize < 15
    }
    get isThumbVisible() {
        if (this.hidden) {
            return false
        }
        if (this.isHoverMode) {
            return this.props.visible || this.state.hovered || this.state.active
        }
        if (this.isAlwaysMode) {
            return true
        }
        return this.props.visible
    }
    get isExpandable() {
        return (this.isHoverMode || this.isAlwaysMode) && this.props.scrollByThumb
    }
    get isHoverMode() {
        return this.props.showScrollbar === ShowScrollbarMode.HOVER
    }
    get isAlwaysMode() {
        return this.props.showScrollbar === ShowScrollbarMode.ALWAYS
    }
    get isNeverMode() {
        return this.props.showScrollbar === ShowScrollbarMode.NEVER
    }
    get restAttributes() {
        var _this$props = this.props,
            restProps = _objectWithoutPropertiesLoose(_this$props, _excluded);
        return restProps
    }
    isThumb(element) {
        return this.scrollbarRef.current.querySelector(".".concat(SCROLLABLE_SCROLL_CLASS)) === element || this.scrollbarRef.current.querySelector(".".concat(SCROLLABLE_SCROLL_CONTENT_CLASS)) === element
    }
    isScrollbar(element) {
        return element === this.scrollbarRef.current
    }
    setActiveState() {
        this.setState(__state_argument => ({
            active: true
        }))
    }
    moveToMouseLocation(event, offset) {
        var mouseLocation = event["page".concat(this.axis.toUpperCase())] - offset;
        var delta = mouseLocation / this.containerToContentRatio - this.props.containerSize / 2;
        this.moveTo(Math.round(-delta))
    }
    moveTo(location) {
        var _this$props$scrollLoc, _this$props2;
        var scrollDelta = Math.abs(this.prevScrollLocation - location);
        this.prevScrollLocation = location;
        this.rightScrollLocation = this.props.maxOffset - location;
        null === (_this$props$scrollLoc = (_this$props2 = this.props).scrollLocationChange) || void 0 === _this$props$scrollLoc ? void 0 : _this$props$scrollLoc.call(_this$props2, {
            fullScrollProp: this.fullScrollProp,
            location: -location,
            needFireScroll: scrollDelta > 0
        })
    }
    componentWillUpdate(nextProps, nextState, context) {
        super.componentWillUpdate();
        if (this.props.direction !== nextProps.direction || this.props.containerSize !== nextProps.containerSize || this.props.contentSize !== nextProps.contentSize || this.props.showScrollbar !== nextProps.showScrollbar || this.props.scrollLocation !== nextProps.scrollLocation || this.props.maxOffset !== nextProps.maxOffset) {
            this.__getterCache.thumbStyles = void 0
        }
    }
    render() {
        var props = this.props;
        return viewFunction({
            props: _extends({}, props),
            hovered: this.state.hovered,
            active: this.state.active,
            scrollbarRef: this.scrollbarRef,
            scrollRef: this.scrollRef,
            thumbRef: this.thumbRef,
            axis: this.axis,
            fullScrollProp: this.fullScrollProp,
            dimension: this.dimension,
            isHorizontal: this.isHorizontal,
            scrollSize: this.scrollSize,
            containerToContentRatio: this.containerToContentRatio,
            scrollRatio: this.scrollRatio,
            scrollbarClasses: this.scrollbarClasses,
            thumbStyles: this.thumbStyles,
            thumbTransform: this.thumbTransform,
            thumbClasses: this.thumbClasses,
            hidden: this.hidden,
            isThumbVisible: this.isThumbVisible,
            isExpandable: this.isExpandable,
            isHoverMode: this.isHoverMode,
            isAlwaysMode: this.isAlwaysMode,
            isNeverMode: this.isNeverMode,
            restAttributes: this.restAttributes
        })
    }
}
Scrollbar.defaultProps = ScrollbarPropsType;
