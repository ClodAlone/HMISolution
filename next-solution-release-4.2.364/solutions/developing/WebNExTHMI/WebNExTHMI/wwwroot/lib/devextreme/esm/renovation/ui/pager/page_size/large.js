/**
 * DevExtreme (esm/renovation/ui/pager/page_size/large.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import _extends from "@babel/runtime/helpers/esm/extends";
import _objectWithoutPropertiesLoose from "@babel/runtime/helpers/esm/objectWithoutPropertiesLoose";
var _excluded = ["pageSize", "pageSizeChange", "pageSizes"];
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
    InternalPagerProps
} from "../common/pager_props";
import {
    PAGER_SELECTED_PAGE_SIZE_CLASS,
    PAGER_PAGE_SIZE_CLASS
} from "../common/consts";
export var viewFunction = _ref => {
    var {
        pageSizesText: pageSizesText
    } = _ref;
    return createFragment(pageSizesText.map(_ref2 => {
        var {
            className: className,
            click: click,
            label: label,
            text: text
        } = _ref2;
        return createComponentVNode(2, LightButton, {
            className: className,
            label: label,
            onClick: click,
            children: text
        }, text)
    }), 0)
};
export var PageSizeLargeProps = {};
var PageSizeLargePropsType = {
    get pageSize() {
        return InternalPagerProps.pageSize
    }
};
export class PageSizeLarge extends BaseInfernoComponent {
    constructor(props) {
        super(props);
        this.state = {};
        this.__getterCache = {};
        this.onPageSizeChange = this.onPageSizeChange.bind(this)
    }
    get pageSizesText() {
        if (void 0 !== this.__getterCache.pageSizesText) {
            return this.__getterCache.pageSizesText
        }
        return this.__getterCache.pageSizesText = (() => {
            var {
                pageSize: pageSize,
                pageSizes: pageSizes
            } = this.props;
            return pageSizes.map(_ref3 => {
                var {
                    text: text,
                    value: processedPageSize
                } = _ref3;
                var selected = processedPageSize === pageSize;
                var className = selected ? PAGER_SELECTED_PAGE_SIZE_CLASS : PAGER_PAGE_SIZE_CLASS;
                return {
                    className: className,
                    click: this.onPageSizeChange(processedPageSize),
                    label: "Display ".concat(processedPageSize, " items on page"),
                    text: text
                }
            })
        })()
    }
    onPageSizeChange(processedPageSize) {
        return () => {
            this.props.pageSizeChange(processedPageSize);
            return this.props.pageSize
        }
    }
    get restAttributes() {
        var _this$props = this.props,
            restProps = _objectWithoutPropertiesLoose(_this$props, _excluded);
        return restProps
    }
    componentWillUpdate(nextProps, nextState, context) {
        if (this.props.pageSize !== nextProps.pageSize || this.props.pageSizes !== nextProps.pageSizes || this.props.pageSizeChange !== nextProps.pageSizeChange) {
            this.__getterCache.pageSizesText = void 0
        }
    }
    render() {
        var props = this.props;
        return viewFunction({
            props: _extends({}, props),
            pageSizesText: this.pageSizesText,
            restAttributes: this.restAttributes
        })
    }
}
PageSizeLarge.defaultProps = PageSizeLargePropsType;
