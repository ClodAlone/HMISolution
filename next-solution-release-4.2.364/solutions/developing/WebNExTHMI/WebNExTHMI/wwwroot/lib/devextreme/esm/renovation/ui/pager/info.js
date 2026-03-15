/**
 * DevExtreme (esm/renovation/ui/pager/info.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import _extends from "@babel/runtime/helpers/esm/extends";
import _objectWithoutPropertiesLoose from "@babel/runtime/helpers/esm/objectWithoutPropertiesLoose";
var _excluded = ["infoText", "pageCount", "pageIndex", "rootElementRef", "totalCount"];
import {
    createVNode
} from "inferno";
import {
    BaseInfernoComponent
} from "@devextreme/runtime/inferno";
import {
    format
} from "../../../core/utils/string";
import messageLocalization from "../../../localization/message";
import {
    InternalPagerProps
} from "./common/pager_props";
export var PAGER_INFO_CLASS = "dx-info";
export var viewFunction = _ref => {
    var {
        props: {
            rootElementRef: rootElementRef
        },
        text: text
    } = _ref;
    return createVNode(1, "div", PAGER_INFO_CLASS, text, 0, null, null, rootElementRef)
};
export var InfoTextProps = {};
var InfoTextPropsType = {
    get pageIndex() {
        return InternalPagerProps.pageIndex
    },
    get pageCount() {
        return InternalPagerProps.pageCount
    },
    get totalCount() {
        return InternalPagerProps.totalCount
    }
};
export class InfoText extends BaseInfernoComponent {
    constructor(props) {
        super(props);
        this.state = {}
    }
    get infoText() {
        var _this$props$infoText;
        return (null !== (_this$props$infoText = this.props.infoText) && void 0 !== _this$props$infoText ? _this$props$infoText : "") || messageLocalization.getFormatter("dxPager-infoText")()
    }
    get text() {
        var {
            pageCount: pageCount,
            pageIndex: pageIndex,
            totalCount: totalCount
        } = this.props;
        return format(this.infoText, (pageIndex + 1).toString(), pageCount.toString(), totalCount.toString())
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
            infoText: this.infoText,
            text: this.text,
            restAttributes: this.restAttributes
        })
    }
}
InfoText.defaultProps = InfoTextPropsType;
