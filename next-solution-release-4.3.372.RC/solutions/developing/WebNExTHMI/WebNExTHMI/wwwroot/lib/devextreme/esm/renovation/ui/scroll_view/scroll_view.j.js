/**
 * DevExtreme (esm/renovation/ui/scroll_view/scroll_view.j.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import registerComponent from "../../../core/component_registrator";
import {
    ScrollViewWrapper
} from "../../component_wrapper/navigation/scroll_view";
import {
    ScrollView as ScrollViewComponent
} from "./scroll_view";
export default class ScrollView extends ScrollViewWrapper {
    release(preventScrollBottom) {
        var _this$viewRef;
        return null === (_this$viewRef = this.viewRef) || void 0 === _this$viewRef ? void 0 : _this$viewRef.release(...arguments)
    }
    refresh() {
        var _this$viewRef2;
        return null === (_this$viewRef2 = this.viewRef) || void 0 === _this$viewRef2 ? void 0 : _this$viewRef2.refresh(...arguments)
    }
    content() {
        var _this$viewRef3;
        return this._toPublicElement(null === (_this$viewRef3 = this.viewRef) || void 0 === _this$viewRef3 ? void 0 : _this$viewRef3.content(...arguments))
    }
    container() {
        var _this$viewRef4;
        return this._toPublicElement(null === (_this$viewRef4 = this.viewRef) || void 0 === _this$viewRef4 ? void 0 : _this$viewRef4.container(...arguments))
    }
    scrollBy(distance) {
        var _this$viewRef5;
        return null === (_this$viewRef5 = this.viewRef) || void 0 === _this$viewRef5 ? void 0 : _this$viewRef5.scrollBy(...arguments)
    }
    scrollTo(targetLocation) {
        var _this$viewRef6;
        return null === (_this$viewRef6 = this.viewRef) || void 0 === _this$viewRef6 ? void 0 : _this$viewRef6.scrollTo(...arguments)
    }
    scrollToElement(element, offset) {
        var _this$viewRef7;
        var params = [this._patchElementParam(element), offset];
        return null === (_this$viewRef7 = this.viewRef) || void 0 === _this$viewRef7 ? void 0 : _this$viewRef7.scrollToElement(...params.slice(0, arguments.length))
    }
    scrollHeight() {
        var _this$viewRef8;
        return null === (_this$viewRef8 = this.viewRef) || void 0 === _this$viewRef8 ? void 0 : _this$viewRef8.scrollHeight(...arguments)
    }
    scrollWidth() {
        var _this$viewRef9;
        return null === (_this$viewRef9 = this.viewRef) || void 0 === _this$viewRef9 ? void 0 : _this$viewRef9.scrollWidth(...arguments)
    }
    scrollOffset() {
        var _this$viewRef10;
        return null === (_this$viewRef10 = this.viewRef) || void 0 === _this$viewRef10 ? void 0 : _this$viewRef10.scrollOffset(...arguments)
    }
    scrollTop() {
        var _this$viewRef11;
        return null === (_this$viewRef11 = this.viewRef) || void 0 === _this$viewRef11 ? void 0 : _this$viewRef11.scrollTop(...arguments)
    }
    scrollLeft() {
        var _this$viewRef12;
        return null === (_this$viewRef12 = this.viewRef) || void 0 === _this$viewRef12 ? void 0 : _this$viewRef12.scrollLeft(...arguments)
    }
    clientHeight() {
        var _this$viewRef13;
        return null === (_this$viewRef13 = this.viewRef) || void 0 === _this$viewRef13 ? void 0 : _this$viewRef13.clientHeight(...arguments)
    }
    clientWidth() {
        var _this$viewRef14;
        return null === (_this$viewRef14 = this.viewRef) || void 0 === _this$viewRef14 ? void 0 : _this$viewRef14.clientWidth(...arguments)
    }
    toggleLoading(showOrHide) {
        var _this$viewRef15;
        return null === (_this$viewRef15 = this.viewRef) || void 0 === _this$viewRef15 ? void 0 : _this$viewRef15.toggleLoading(...arguments)
    }
    startLoading() {
        var _this$viewRef16;
        return null === (_this$viewRef16 = this.viewRef) || void 0 === _this$viewRef16 ? void 0 : _this$viewRef16.startLoading(...arguments)
    }
    finishLoading() {
        var _this$viewRef17;
        return null === (_this$viewRef17 = this.viewRef) || void 0 === _this$viewRef17 ? void 0 : _this$viewRef17.finishLoading(...arguments)
    }
    updateHandler() {
        var _this$viewRef18;
        return null === (_this$viewRef18 = this.viewRef) || void 0 === _this$viewRef18 ? void 0 : _this$viewRef18.updateHandler(...arguments)
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
            props: ["pullDownEnabled", "reachBottomEnabled", "useNative", "useSimulatedScrollbar", "refreshStrategy", "inertiaEnabled", "useKeyboard", "showScrollbar", "scrollByThumb", "onVisibilityChange", "onStart", "onEnd", "onBounce", "scrollLocationChange", "aria", "addWidgetClass", "disabled", "height", "width", "visible", "rtlEnabled", "classes", "direction", "bounceEnabled", "scrollByContent", "forceGeneratePockets", "needScrollViewContentWrapper", "needScrollViewLoadPanel", "needRenderScrollbars", "onScroll", "onUpdated", "onPullDown", "onReachBottom", "pullingDownText", "pulledDownText", "refreshingText", "reachBottomText"]
        }
    }
    get _viewComponent() {
        return ScrollViewComponent
    }
}
registerComponent("dxScrollView", ScrollView);
