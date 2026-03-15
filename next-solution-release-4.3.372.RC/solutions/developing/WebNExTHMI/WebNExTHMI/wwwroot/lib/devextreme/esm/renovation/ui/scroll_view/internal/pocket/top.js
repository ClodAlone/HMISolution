/**
 * DevExtreme (esm/renovation/ui/scroll_view/internal/pocket/top.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import _extends from "@babel/runtime/helpers/esm/extends";
import _objectWithoutPropertiesLoose from "@babel/runtime/helpers/esm/objectWithoutPropertiesLoose";
var _excluded = ["pocketState", "pocketTop", "pullDownIconAngle", "pullDownOpacity", "pullDownTranslateTop", "pulledDownText", "pullingDownText", "refreshStrategy", "refreshingText", "topPocketRef", "topPocketTranslateTop", "visible"];
import {
    createVNode,
    createComponentVNode
} from "inferno";
import {
    BaseInfernoComponent,
    normalizeStyles
} from "@devextreme/runtime/inferno";
import {
    LoadIndicator
} from "../../../load_indicator";
import {
    combineClasses
} from "../../../../utils/combine_classes";
import messageLocalization from "../../../../../localization/message";
import {
    PULLDOWN_ICON_CLASS,
    SCROLLVIEW_PULLDOWN,
    SCROLLVIEW_PULLDOWN_IMAGE_CLASS,
    SCROLLVIEW_PULLDOWN_INDICATOR_CLASS,
    SCROLLVIEW_PULLDOWN_READY_CLASS,
    SCROLLVIEW_PULLDOWN_LOADING_CLASS,
    SCROLLVIEW_PULLDOWN_TEXT_CLASS,
    SCROLLVIEW_PULLDOWN_VISIBLE_TEXT_CLASS,
    SCROLLVIEW_TOP_POCKET_CLASS,
    TopPocketState
} from "../../common/consts";
import {
    current,
    isMaterial
} from "../../../../../ui/themes";
export var viewFunction = viewModel => {
    var {
        props: {
            pulledDownText: pulledDownText,
            pullingDownText: pullingDownText,
            refreshStrategy: refreshStrategy,
            refreshingText: refreshingText,
            topPocketRef: topPocketRef
        },
        pullDownClasses: pullDownClasses,
        pullDownIconStyles: pullDownIconStyles,
        pullDownRef: pullDownRef,
        pullDownStyles: pullDownStyles,
        readyVisibleClass: readyVisibleClass,
        refreshVisibleClass: refreshVisibleClass,
        releaseVisibleClass: releaseVisibleClass,
        topPocketClasses: topPocketClasses,
        topPocketStyles: topPocketStyles
    } = viewModel;
    return createVNode(1, "div", topPocketClasses, createVNode(1, "div", pullDownClasses, ["swipeDown" !== refreshStrategy && createVNode(1, "div", SCROLLVIEW_PULLDOWN_IMAGE_CLASS), "swipeDown" === refreshStrategy && createVNode(1, "div", PULLDOWN_ICON_CLASS, null, 1, {
        style: normalizeStyles(pullDownIconStyles)
    }), createVNode(1, "div", SCROLLVIEW_PULLDOWN_INDICATOR_CLASS, createComponentVNode(2, LoadIndicator), 2), "swipeDown" !== refreshStrategy && createVNode(1, "div", SCROLLVIEW_PULLDOWN_TEXT_CLASS, [createVNode(1, "div", releaseVisibleClass, pullingDownText, 0), createVNode(1, "div", readyVisibleClass, pulledDownText, 0), createVNode(1, "div", refreshVisibleClass, refreshingText, 0)], 4)], 0, {
        style: normalizeStyles(pullDownStyles)
    }, null, pullDownRef), 2, {
        style: normalizeStyles(topPocketStyles)
    }, null, topPocketRef)
};
export var TopPocketProps = {
    get pullingDownText() {
        return isMaterial(current()) ? "" : messageLocalization.format("dxScrollView-pullingDownText")
    },
    get pulledDownText() {
        return isMaterial(current()) ? "" : messageLocalization.format("dxScrollView-pulledDownText")
    },
    get refreshingText() {
        return isMaterial(current()) ? "" : messageLocalization.format("dxScrollView-refreshingText")
    },
    get pocketState() {
        return TopPocketState.STATE_RELEASED
    },
    pullDownTranslateTop: 0,
    pullDownIconAngle: 0,
    pullDownOpacity: 0,
    pocketTop: 0,
    topPocketTranslateTop: 0,
    visible: true
};
import {
    createRef as infernoCreateRef
} from "inferno";
export class TopPocket extends BaseInfernoComponent {
    constructor(props) {
        super(props);
        this.state = {};
        this.pullDownRef = infernoCreateRef();
        this.__getterCache = {}
    }
    get releaseVisibleClass() {
        return this.props.pocketState === TopPocketState.STATE_RELEASED ? SCROLLVIEW_PULLDOWN_VISIBLE_TEXT_CLASS : void 0
    }
    get readyVisibleClass() {
        return this.props.pocketState === TopPocketState.STATE_READY ? SCROLLVIEW_PULLDOWN_VISIBLE_TEXT_CLASS : void 0
    }
    get refreshVisibleClass() {
        return this.props.pocketState === TopPocketState.STATE_REFRESHING ? SCROLLVIEW_PULLDOWN_VISIBLE_TEXT_CLASS : void 0
    }
    get pullDownClasses() {
        var {
            pocketState: pocketState,
            visible: visible
        } = this.props;
        var classesMap = {
            [SCROLLVIEW_PULLDOWN]: true,
            [SCROLLVIEW_PULLDOWN_READY_CLASS]: pocketState === TopPocketState.STATE_READY,
            [SCROLLVIEW_PULLDOWN_LOADING_CLASS]: pocketState === TopPocketState.STATE_REFRESHING,
            "dx-state-invisible": !visible
        };
        return combineClasses(classesMap)
    }
    get topPocketClasses() {
        var classesMap = {
            [SCROLLVIEW_TOP_POCKET_CLASS]: true,
            "dx-state-invisible": !this.props.visible
        };
        return combineClasses(classesMap)
    }
    get pullDownStyles() {
        if (void 0 !== this.__getterCache.pullDownStyles) {
            return this.__getterCache.pullDownStyles
        }
        return this.__getterCache.pullDownStyles = (() => {
            if ("swipeDown" === this.props.refreshStrategy) {
                return {
                    opacity: this.props.pullDownOpacity,
                    transform: "translate(0px, ".concat(this.props.pullDownTranslateTop, "px)")
                }
            }
            return
        })()
    }
    get topPocketStyles() {
        if (void 0 !== this.__getterCache.topPocketStyles) {
            return this.__getterCache.topPocketStyles
        }
        return this.__getterCache.topPocketStyles = (() => {
            if ("pullDown" === this.props.refreshStrategy) {
                return {
                    top: "".concat(-this.props.pocketTop, "px"),
                    transform: "translate(0px, ".concat(this.props.topPocketTranslateTop, "px)")
                }
            }
            return
        })()
    }
    get pullDownIconStyles() {
        if (void 0 !== this.__getterCache.pullDownIconStyles) {
            return this.__getterCache.pullDownIconStyles
        }
        return this.__getterCache.pullDownIconStyles = (() => ({
            transform: "rotate(".concat(this.props.pullDownIconAngle, "deg)")
        }))()
    }
    get restAttributes() {
        var _this$props = this.props,
            restProps = _objectWithoutPropertiesLoose(_this$props, _excluded);
        return restProps
    }
    componentWillUpdate(nextProps, nextState, context) {
        if (this.props.refreshStrategy !== nextProps.refreshStrategy || this.props.pullDownOpacity !== nextProps.pullDownOpacity || this.props.pullDownTranslateTop !== nextProps.pullDownTranslateTop) {
            this.__getterCache.pullDownStyles = void 0
        }
        if (this.props.refreshStrategy !== nextProps.refreshStrategy || this.props.pocketTop !== nextProps.pocketTop || this.props.topPocketTranslateTop !== nextProps.topPocketTranslateTop) {
            this.__getterCache.topPocketStyles = void 0
        }
        if (this.props.pullDownIconAngle !== nextProps.pullDownIconAngle) {
            this.__getterCache.pullDownIconStyles = void 0
        }
    }
    render() {
        var props = this.props;
        return viewFunction({
            props: _extends({}, props),
            pullDownRef: this.pullDownRef,
            releaseVisibleClass: this.releaseVisibleClass,
            readyVisibleClass: this.readyVisibleClass,
            refreshVisibleClass: this.refreshVisibleClass,
            pullDownClasses: this.pullDownClasses,
            topPocketClasses: this.topPocketClasses,
            pullDownStyles: this.pullDownStyles,
            topPocketStyles: this.topPocketStyles,
            pullDownIconStyles: this.pullDownIconStyles,
            restAttributes: this.restAttributes
        })
    }
}
TopPocket.defaultProps = TopPocketProps;
