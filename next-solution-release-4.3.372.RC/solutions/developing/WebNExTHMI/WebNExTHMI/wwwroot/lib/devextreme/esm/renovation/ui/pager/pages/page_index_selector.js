/**
 * DevExtreme (esm/renovation/ui/pager/pages/page_index_selector.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import _extends from "@babel/runtime/helpers/esm/extends";
import _objectWithoutPropertiesLoose from "@babel/runtime/helpers/esm/objectWithoutPropertiesLoose";
var _excluded = ["hasKnownLastPage", "isLargeDisplayMode", "maxPagesCount", "pageCount", "pageIndex", "pageIndexChange", "pagesCountText", "showNavigationButtons", "totalCount"];
import {
    createFragment,
    createComponentVNode
} from "inferno";
import {
    Fragment
} from "inferno";
import {
    BaseInfernoComponent
} from "@devextreme/runtime/inferno";
import {
    LightButton
} from "../common/light_button";
import {
    PagesLarge
} from "./large";
import {
    PagesSmall
} from "./small";
import {
    InternalPagerProps
} from "../common/pager_props";
import {
    ConfigContext
} from "../../../common/config_context";
var PAGER_NAVIGATE_BUTTON = "dx-navigate-button";
var PAGER_PREV_BUTTON_CLASS = "dx-prev-button";
var PAGER_NEXT_BUTTON_CLASS = "dx-next-button";
export var PAGER_BUTTON_DISABLE_CLASS = "dx-button-disable";
var nextButtonClassName = "".concat(PAGER_NAVIGATE_BUTTON, " ").concat(PAGER_NEXT_BUTTON_CLASS);
var prevButtonClassName = "".concat(PAGER_NAVIGATE_BUTTON, " ").concat(PAGER_PREV_BUTTON_CLASS);
var nextButtonDisabledClassName = "".concat(PAGER_BUTTON_DISABLE_CLASS, " ").concat(PAGER_NAVIGATE_BUTTON, " ").concat(PAGER_NEXT_BUTTON_CLASS);
var prevButtonDisabledClassName = "".concat(PAGER_BUTTON_DISABLE_CLASS, " ").concat(PAGER_NAVIGATE_BUTTON, " ").concat(PAGER_PREV_BUTTON_CLASS);
export var viewFunction = _ref => {
    var {
        navigateToNextPage: navigateToNextPage,
        navigateToPrevPage: navigateToPrevPage,
        nextClassName: nextClassName,
        pageIndexChange: pageIndexChange,
        prevClassName: prevClassName,
        props: {
            isLargeDisplayMode: isLargeDisplayMode,
            maxPagesCount: maxPagesCount,
            pageCount: pageCount,
            pageIndex: pageIndex,
            pagesCountText: pagesCountText
        },
        renderNextButton: renderNextButton,
        renderPrevButton: renderPrevButton
    } = _ref;
    return createFragment([renderPrevButton && createComponentVNode(2, LightButton, {
        className: prevClassName,
        label: "Previous page",
        onClick: navigateToPrevPage
    }), isLargeDisplayMode && createComponentVNode(2, PagesLarge, {
        maxPagesCount: maxPagesCount,
        pageCount: pageCount,
        pageIndex: pageIndex,
        pageIndexChange: pageIndexChange
    }), !isLargeDisplayMode && createComponentVNode(2, PagesSmall, {
        pageCount: pageCount,
        pageIndex: pageIndex,
        pageIndexChange: pageIndexChange,
        pagesCountText: pagesCountText
    }), renderNextButton && createComponentVNode(2, LightButton, {
        className: nextClassName,
        label: "Next page",
        onClick: navigateToNextPage
    })], 0)
};

function getIncrement(direction) {
    return "next" === direction ? 1 : -1
}
export var PageIndexSelectorProps = {
    isLargeDisplayMode: true
};
var PageIndexSelectorPropsType = {
    get pageIndex() {
        return InternalPagerProps.pageIndex
    },
    get maxPagesCount() {
        return InternalPagerProps.maxPagesCount
    },
    get pageCount() {
        return InternalPagerProps.pageCount
    },
    get hasKnownLastPage() {
        return InternalPagerProps.hasKnownLastPage
    },
    get showNavigationButtons() {
        return InternalPagerProps.showNavigationButtons
    },
    get totalCount() {
        return InternalPagerProps.totalCount
    },
    get isLargeDisplayMode() {
        return PageIndexSelectorProps.isLargeDisplayMode
    }
};
export class PageIndexSelector extends BaseInfernoComponent {
    constructor(props) {
        super(props);
        this.state = {};
        this.pageIndexChange = this.pageIndexChange.bind(this);
        this.navigateToNextPage = this.navigateToNextPage.bind(this);
        this.navigateToPrevPage = this.navigateToPrevPage.bind(this);
        this.getNextDirection = this.getNextDirection.bind(this);
        this.getPrevDirection = this.getPrevDirection.bind(this);
        this.canNavigateToPage = this.canNavigateToPage.bind(this);
        this.getNextPageIndex = this.getNextPageIndex.bind(this);
        this.canNavigateTo = this.canNavigateTo.bind(this);
        this.navigateToPage = this.navigateToPage.bind(this)
    }
    get config() {
        if ("ConfigContext" in this.context) {
            return this.context.ConfigContext
        }
        return ConfigContext
    }
    pageIndexChange(pageIndex) {
        if (this.canNavigateToPage(pageIndex)) {
            this.props.pageIndexChange(pageIndex)
        }
    }
    navigateToNextPage() {
        this.navigateToPage(this.getNextDirection())
    }
    navigateToPrevPage() {
        this.navigateToPage(this.getPrevDirection())
    }
    getNextDirection() {
        var _this$config;
        return !(null !== (_this$config = this.config) && void 0 !== _this$config && _this$config.rtlEnabled) ? "next" : "prev"
    }
    getPrevDirection() {
        var _this$config2;
        return !(null !== (_this$config2 = this.config) && void 0 !== _this$config2 && _this$config2.rtlEnabled) ? "prev" : "next"
    }
    canNavigateToPage(pageIndex) {
        if (!this.props.hasKnownLastPage) {
            return pageIndex >= 0
        }
        return pageIndex >= 0 && pageIndex <= this.props.pageCount - 1
    }
    getNextPageIndex(direction) {
        return this.props.pageIndex + getIncrement(direction)
    }
    canNavigateTo(direction) {
        return this.canNavigateToPage(this.getNextPageIndex(direction))
    }
    navigateToPage(direction) {
        this.pageIndexChange(this.getNextPageIndex(direction))
    }
    get renderPrevButton() {
        var {
            isLargeDisplayMode: isLargeDisplayMode,
            showNavigationButtons: showNavigationButtons
        } = this.props;
        return !isLargeDisplayMode || showNavigationButtons
    }
    get renderNextButton() {
        return this.renderPrevButton || !this.props.hasKnownLastPage
    }
    get nextClassName() {
        var direction = this.getNextDirection();
        var canNavigate = this.canNavigateTo(direction);
        return canNavigate ? nextButtonClassName : nextButtonDisabledClassName
    }
    get prevClassName() {
        var direction = this.getPrevDirection();
        var canNavigate = this.canNavigateTo(direction);
        return canNavigate ? prevButtonClassName : prevButtonDisabledClassName
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
            config: this.config,
            pageIndexChange: this.pageIndexChange,
            navigateToNextPage: this.navigateToNextPage,
            navigateToPrevPage: this.navigateToPrevPage,
            renderPrevButton: this.renderPrevButton,
            renderNextButton: this.renderNextButton,
            nextClassName: this.nextClassName,
            prevClassName: this.prevClassName,
            restAttributes: this.restAttributes
        })
    }
}
PageIndexSelector.defaultProps = PageIndexSelectorPropsType;
