/**
 * DevExtreme (esm/renovation/ui/scroll_view/scroll_view.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import _objectWithoutPropertiesLoose from "@babel/runtime/helpers/esm/objectWithoutPropertiesLoose";
import _extends from "@babel/runtime/helpers/esm/extends";
var _excluded = ["addWidgetClass", "aria", "bounceEnabled", "children", "classes", "direction", "disabled", "forceGeneratePockets", "height", "inertiaEnabled", "needRenderScrollbars", "needScrollViewContentWrapper", "needScrollViewLoadPanel", "onBounce", "onEnd", "onPullDown", "onReachBottom", "onScroll", "onStart", "onUpdated", "onVisibilityChange", "pullDownEnabled", "pulledDownText", "pullingDownText", "reachBottomEnabled", "reachBottomText", "refreshStrategy", "refreshingText", "rtlEnabled", "scrollByContent", "scrollByThumb", "scrollLocationChange", "showScrollbar", "useKeyboard", "useNative", "useSimulatedScrollbar", "visible", "width"];
import {
    createComponentVNode,
    normalizeProps
} from "inferno";
import {
    InfernoWrapperComponent
} from "@devextreme/runtime/inferno";
import {
    isDefined
} from "../../../core/utils/type";
import {
    Scrollable
} from "./scrollable";
import {
    ScrollViewProps
} from "./common/scrollview_props";
export var viewFunction = viewModel => {
    var {
        props: {
            aria: aria,
            bounceEnabled: bounceEnabled,
            children: children,
            direction: direction,
            disabled: disabled,
            height: height,
            inertiaEnabled: inertiaEnabled,
            onBounce: onBounce,
            onEnd: onEnd,
            onPullDown: onPullDown,
            onReachBottom: onReachBottom,
            onScroll: onScroll,
            onStart: onStart,
            onUpdated: onUpdated,
            pullDownEnabled: pullDownEnabled,
            pulledDownText: pulledDownText,
            pullingDownText: pullingDownText,
            reachBottomText: reachBottomText,
            refreshStrategy: refreshStrategy,
            refreshingText: refreshingText,
            rtlEnabled: rtlEnabled,
            scrollByContent: scrollByContent,
            scrollByThumb: scrollByThumb,
            showScrollbar: showScrollbar,
            useKeyboard: useKeyboard,
            useNative: useNative,
            useSimulatedScrollbar: useSimulatedScrollbar,
            visible: visible,
            width: width
        },
        reachBottomEnabled: reachBottomEnabled,
        restAttributes: restAttributes,
        scrollableRef: scrollableRef
    } = viewModel;
    return normalizeProps(createComponentVNode(2, Scrollable, _extends({
        useNative: useNative,
        classes: "dx-scrollview",
        aria: aria,
        width: width,
        height: height,
        disabled: disabled,
        visible: visible,
        rtlEnabled: rtlEnabled,
        direction: direction,
        showScrollbar: showScrollbar,
        scrollByThumb: scrollByThumb,
        pullDownEnabled: pullDownEnabled,
        reachBottomEnabled: reachBottomEnabled,
        onScroll: onScroll,
        onUpdated: onUpdated,
        onPullDown: onPullDown,
        onReachBottom: onReachBottom,
        refreshStrategy: refreshStrategy,
        pulledDownText: pulledDownText,
        pullingDownText: pullingDownText,
        refreshingText: refreshingText,
        reachBottomText: reachBottomText,
        forceGeneratePockets: true,
        needScrollViewContentWrapper: true,
        needScrollViewLoadPanel: true,
        useSimulatedScrollbar: useSimulatedScrollbar,
        inertiaEnabled: inertiaEnabled,
        bounceEnabled: bounceEnabled,
        scrollByContent: scrollByContent,
        useKeyboard: useKeyboard,
        onStart: onStart,
        onEnd: onEnd,
        onBounce: onBounce
    }, restAttributes, {
        children: children
    }), null, scrollableRef))
};
import {
    createReRenderEffect
} from "@devextreme/runtime/inferno";
import {
    createRef as infernoCreateRef
} from "inferno";
export class ScrollView extends InfernoWrapperComponent {
    constructor(props) {
        super(props);
        this.scrollableRef = infernoCreateRef();
        this.state = {
            forceReachBottom: void 0
        };
        this.release = this.release.bind(this);
        this.refresh = this.refresh.bind(this);
        this.content = this.content.bind(this);
        this.container = this.container.bind(this);
        this.scrollBy = this.scrollBy.bind(this);
        this.scrollTo = this.scrollTo.bind(this);
        this.scrollToElement = this.scrollToElement.bind(this);
        this.scrollHeight = this.scrollHeight.bind(this);
        this.scrollWidth = this.scrollWidth.bind(this);
        this.scrollOffset = this.scrollOffset.bind(this);
        this.scrollTop = this.scrollTop.bind(this);
        this.scrollLeft = this.scrollLeft.bind(this);
        this.clientHeight = this.clientHeight.bind(this);
        this.clientWidth = this.clientWidth.bind(this);
        this.toggleLoading = this.toggleLoading.bind(this);
        this.startLoading = this.startLoading.bind(this);
        this.finishLoading = this.finishLoading.bind(this);
        this.updateHandler = this.updateHandler.bind(this)
    }
    createEffects() {
        return [createReRenderEffect()]
    }
    get reachBottomEnabled() {
        if (isDefined(this.state.forceReachBottom)) {
            return this.state.forceReachBottom
        }
        return this.props.reachBottomEnabled
    }
    get restAttributes() {
        var _this$props = this.props,
            restProps = _objectWithoutPropertiesLoose(_this$props, _excluded);
        return restProps
    }
    release(preventScrollBottom) {
        if (void 0 !== preventScrollBottom) {
            this.toggleLoading(!preventScrollBottom)
        }
        this.scrollableRef.current.release()
    }
    refresh() {
        if (this.props.pullDownEnabled) {
            this.scrollableRef.current.refresh()
        }
    }
    content() {
        return this.scrollableRef.current.content()
    }
    container() {
        return this.scrollableRef.current.container()
    }
    scrollBy(distance) {
        this.scrollableRef.current.scrollBy(distance)
    }
    scrollTo(targetLocation) {
        this.scrollableRef.current.scrollTo(targetLocation)
    }
    scrollToElement(element, offset) {
        this.scrollableRef.current.scrollToElement(element, offset)
    }
    scrollHeight() {
        return this.scrollableRef.current.scrollHeight()
    }
    scrollWidth() {
        return this.scrollableRef.current.scrollWidth()
    }
    scrollOffset() {
        return this.scrollableRef.current.scrollOffset()
    }
    scrollTop() {
        return this.scrollableRef.current.scrollTop()
    }
    scrollLeft() {
        return this.scrollableRef.current.scrollLeft()
    }
    clientHeight() {
        return this.scrollableRef.current.clientHeight()
    }
    clientWidth() {
        return this.scrollableRef.current.clientWidth()
    }
    toggleLoading(showOrHide) {
        this.setState(__state_argument => ({
            forceReachBottom: showOrHide
        }))
    }
    startLoading() {
        this.scrollableRef.current.startLoading()
    }
    finishLoading() {
        this.scrollableRef.current.finishLoading()
    }
    updateHandler() {
        this.scrollableRef.current.updateHandler()
    }
    render() {
        var props = this.props;
        return viewFunction({
            props: _extends({}, props),
            forceReachBottom: this.state.forceReachBottom,
            scrollableRef: this.scrollableRef,
            reachBottomEnabled: this.reachBottomEnabled,
            restAttributes: this.restAttributes
        })
    }
}
ScrollView.defaultProps = ScrollViewProps;
