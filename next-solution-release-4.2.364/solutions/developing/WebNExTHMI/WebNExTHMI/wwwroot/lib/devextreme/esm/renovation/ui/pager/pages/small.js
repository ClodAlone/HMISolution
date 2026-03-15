/**
 * DevExtreme (esm/renovation/ui/pager/pages/small.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import _extends from "@babel/runtime/helpers/esm/extends";
import _objectWithoutPropertiesLoose from "@babel/runtime/helpers/esm/objectWithoutPropertiesLoose";
var _excluded = ["pageCount", "pageIndex", "pageIndexChange", "pagesCountText"];
import {
    createVNode,
    createComponentVNode
} from "inferno";
import {
    InfernoEffect,
    InfernoComponent
} from "@devextreme/runtime/inferno";
import {
    Page
} from "./page";
import {
    PAGER_INFO_CLASS
} from "../info";
import {
    NumberBox
} from "../../editors/number_box";
import messageLocalization from "../../../../localization/message";
import {
    calculateValuesFittedWidth
} from "../utils/calculate_values_fitted_width";
import {
    getElementMinWidth
} from "../utils/get_element_width";
import {
    InternalPagerProps
} from "../common/pager_props";
var PAGER_INFO_TEXT_CLASS = "".concat(PAGER_INFO_CLASS, "  dx-info-text");
var PAGER_PAGE_INDEX_CLASS = "dx-page-index";
var LIGHT_PAGES_CLASS = "dx-light-pages";
var PAGER_PAGES_COUNT_CLASS = "dx-pages-count";
export var viewFunction = _ref => {
    var {
        pageIndexRef: pageIndexRef,
        pagesCountText: pagesCountText,
        props: {
            pageCount: pageCount
        },
        selectLastPageIndex: selectLastPageIndex,
        value: value,
        valueChange: valueChange,
        width: width
    } = _ref;
    return createVNode(1, "div", LIGHT_PAGES_CLASS, [createComponentVNode(2, NumberBox, {
        className: PAGER_PAGE_INDEX_CLASS,
        min: 1,
        max: pageCount,
        width: width,
        value: value,
        valueChange: valueChange
    }), createVNode(1, "span", PAGER_INFO_TEXT_CLASS, pagesCountText, 0), createComponentVNode(2, Page, {
        className: PAGER_PAGES_COUNT_CLASS,
        selected: false,
        index: pageCount - 1,
        onClick: selectLastPageIndex
    })], 4, null, null, pageIndexRef)
};
var PagerSmallProps = {
    get pageIndex() {
        return InternalPagerProps.pageIndex
    },
    get pageCount() {
        return InternalPagerProps.pageCount
    }
};
import {
    createRef as infernoCreateRef
} from "inferno";
export class PagesSmall extends InfernoComponent {
    constructor(props) {
        super(props);
        this.pageIndexRef = infernoCreateRef();
        this.state = {
            minWidth: 10
        };
        this.updateWidth = this.updateWidth.bind(this);
        this.selectLastPageIndex = this.selectLastPageIndex.bind(this);
        this.valueChange = this.valueChange.bind(this)
    }
    createEffects() {
        return [new InfernoEffect(this.updateWidth, [this.state.minWidth])]
    }
    updateEffects() {
        var _this$_effects$;
        null === (_this$_effects$ = this._effects[0]) || void 0 === _this$_effects$ ? void 0 : _this$_effects$.update([this.state.minWidth])
    }
    updateWidth() {
        var _this$pageIndexRef$cu;
        var el = null === (_this$pageIndexRef$cu = this.pageIndexRef.current) || void 0 === _this$pageIndexRef$cu ? void 0 : _this$pageIndexRef$cu.querySelector(".".concat(PAGER_PAGE_INDEX_CLASS));
        this.setState(__state_argument => ({
            minWidth: el && getElementMinWidth(el) || __state_argument.minWidth
        }))
    }
    get value() {
        return this.props.pageIndex + 1
    }
    get width() {
        var {
            pageCount: pageCount
        } = this.props;
        return calculateValuesFittedWidth(this.state.minWidth, [pageCount])
    }
    get pagesCountText() {
        var _this$props$pagesCoun;
        return (null !== (_this$props$pagesCoun = this.props.pagesCountText) && void 0 !== _this$props$pagesCoun ? _this$props$pagesCoun : "") || messageLocalization.getFormatter("dxPager-pagesCountText")()
    }
    selectLastPageIndex() {
        this.props.pageIndexChange(this.props.pageCount - 1)
    }
    valueChange(value) {
        this.props.pageIndexChange(value - 1)
    }
    get restAttributes() {
        var _this$props = this.props,
            restProps = _objectWithoutPropertiesLoose(_this$props, _excluded);
        return restProps
    }
    render() {
        var props = this.props;
        return viewFunction({
            props: _extends({}, props),
            pageIndexRef: this.pageIndexRef,
            value: this.value,
            width: this.width,
            pagesCountText: this.pagesCountText,
            selectLastPageIndex: this.selectLastPageIndex,
            valueChange: this.valueChange,
            restAttributes: this.restAttributes
        })
    }
}
PagesSmall.defaultProps = PagerSmallProps;
