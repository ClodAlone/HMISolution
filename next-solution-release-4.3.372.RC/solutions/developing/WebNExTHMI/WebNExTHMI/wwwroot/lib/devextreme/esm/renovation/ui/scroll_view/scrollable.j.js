/**
 * DevExtreme (esm/renovation/ui/scroll_view/scrollable.j.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import registerComponent from "../../../core/component_registrator";
import {
    ScrollableWrapper
} from "../../component_wrapper/navigation/scrollable";
import {
    Scrollable as ScrollableComponent
} from "./scrollable";
export default class Scrollable extends ScrollableWrapper {
    content() {
        var _this$viewRef;
        return this._toPublicElement(null === (_this$viewRef = this.viewRef) || void 0 === _this$viewRef ? void 0 : _this$viewRef.content(...arguments))
    }
    container() {
        var _this$viewRef2;
        return this._toPublicElement(null === (_this$viewRef2 = this.viewRef) || void 0 === _this$viewRef2 ? void 0 : _this$viewRef2.container(...arguments))
    }
    scrollTo(targetLocation) {
        var _this$viewRef3;
        return null === (_this$viewRef3 = this.viewRef) || void 0 === _this$viewRef3 ? void 0 : _this$viewRef3.scrollTo(...arguments)
    }
    scrollBy(distance) {
        var _this$viewRef4;
        return null === (_this$viewRef4 = this.viewRef) || void 0 === _this$viewRef4 ? void 0 : _this$viewRef4.scrollBy(...arguments)
    }
    updateHandler() {
        var _this$viewRef5;
        return null === (_this$viewRef5 = this.viewRef) || void 0 === _this$viewRef5 ? void 0 : _this$viewRef5.updateHandler(...arguments)
    }
    release() {
        var _this$viewRef6;
        return null === (_this$viewRef6 = this.viewRef) || void 0 === _this$viewRef6 ? void 0 : _this$viewRef6.release(...arguments)
    }
    refresh() {
        var _this$viewRef7;
        return null === (_this$viewRef7 = this.viewRef) || void 0 === _this$viewRef7 ? void 0 : _this$viewRef7.refresh(...arguments)
    }
    scrollToElement(element, offset) {
        var _this$viewRef8;
        var params = [this._patchElementParam(element), offset];
        return null === (_this$viewRef8 = this.viewRef) || void 0 === _this$viewRef8 ? void 0 : _this$viewRef8.scrollToElement(...params.slice(0, arguments.length))
    }
    scrollHeight() {
        var _this$viewRef9;
        return null === (_this$viewRef9 = this.viewRef) || void 0 === _this$viewRef9 ? void 0 : _this$viewRef9.scrollHeight(...arguments)
    }
    scrollWidth() {
        var _this$viewRef10;
        return null === (_this$viewRef10 = this.viewRef) || void 0 === _this$viewRef10 ? void 0 : _this$viewRef10.scrollWidth(...arguments)
    }
    scrollOffset() {
        var _this$viewRef11;
        return null === (_this$viewRef11 = this.viewRef) || void 0 === _this$viewRef11 ? void 0 : _this$viewRef11.scrollOffset(...arguments)
    }
    scrollTop() {
        var _this$viewRef12;
        return null === (_this$viewRef12 = this.viewRef) || void 0 === _this$viewRef12 ? void 0 : _this$viewRef12.scrollTop(...arguments)
    }
    scrollLeft() {
        var _this$viewRef13;
        return null === (_this$viewRef13 = this.viewRef) || void 0 === _this$viewRef13 ? void 0 : _this$viewRef13.scrollLeft(...arguments)
    }
    clientHeight() {
        var _this$viewRef14;
        return null === (_this$viewRef14 = this.viewRef) || void 0 === _this$viewRef14 ? void 0 : _this$viewRef14.clientHeight(...arguments)
    }
    clientWidth() {
        var _this$viewRef15;
        return null === (_this$viewRef15 = this.viewRef) || void 0 === _this$viewRef15 ? void 0 : _this$viewRef15.clientWidth(...arguments)
    }
    getScrollElementPosition(targetElement, direction, offset) {
        var _this$viewRef16;
        var params = [this._patchElementParam(targetElement), direction, offset];
        return null === (_this$viewRef16 = this.viewRef) || void 0 === _this$viewRef16 ? void 0 : _this$viewRef16.getScrollElementPosition(...params.slice(0, arguments.length))
    }
    startLoading() {
        var _this$viewRef17;
        return null === (_this$viewRef17 = this.viewRef) || void 0 === _this$viewRef17 ? void 0 : _this$viewRef17.startLoading(...arguments)
    }
    finishLoading() {
        var _this$viewRef18;
        return null === (_this$viewRef18 = this.viewRef) || void 0 === _this$viewRef18 ? void 0 : _this$viewRef18.finishLoading(...arguments)
    }
    _getActionConfigs() {
        return {
            onVisibilityChange: {},
            onStart: {},
            onEnd: {},
            onBounce: {},
            scrollLocationChange: {},
            onScroll: {},
            onUpdated: {},
            onPullDown: {},
            onReachBottom: {}
        }
    }
    get _propsInfo() {
        return {
            twoWay: [],
            allowNull: [],
            elements: [],
            templates: [],
            props: ["useNative", "useSimulatedScrollbar", "refreshStrategy", "inertiaEnabled", "useKeyboard", "showScrollbar", "scrollByThumb", "onVisibilityChange", "onStart", "onEnd", "onBounce", "scrollLocationChange", "aria", "addWidgetClass", "disabled", "height", "width", "visible", "rtlEnabled", "classes", "direction", "bounceEnabled", "scrollByContent", "pullDownEnabled", "reachBottomEnabled", "forceGeneratePockets", "needScrollViewContentWrapper", "needScrollViewLoadPanel", "needRenderScrollbars", "onScroll", "onUpdated", "onPullDown", "onReachBottom", "pullingDownText", "pulledDownText", "refreshingText", "reachBottomText"]
        }
    }
    get _viewComponent() {
        return ScrollableComponent
    }
}
registerComponent("dxScrollable", Scrollable);
