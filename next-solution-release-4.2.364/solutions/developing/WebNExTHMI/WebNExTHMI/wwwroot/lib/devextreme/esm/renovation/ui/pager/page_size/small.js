/**
 * DevExtreme (esm/renovation/ui/pager/page_size/small.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import _extends from "@babel/runtime/helpers/esm/extends";
import _objectWithoutPropertiesLoose from "@babel/runtime/helpers/esm/objectWithoutPropertiesLoose";
var _excluded = ["pageSize", "pageSizeChange", "pageSizes", "parentRef"];
import {
    createComponentVNode
} from "inferno";
import {
    InfernoEffect,
    InfernoComponent
} from "@devextreme/runtime/inferno";
import {
    SelectBox
} from "../../editors/drop_down_editors/select_box";
import {
    calculateValuesFittedWidth
} from "../utils/calculate_values_fitted_width";
import {
    getElementMinWidth
} from "../utils/get_element_width";
import {
    InternalPagerProps
} from "../common/pager_props";
export var viewFunction = _ref => {
    var {
        props: {
            pageSize: pageSize,
            pageSizeChange: pageSizeChange,
            pageSizes: pageSizes
        },
        width: width
    } = _ref;
    return createComponentVNode(2, SelectBox, {
        displayExpr: "text",
        valueExpr: "value",
        dataSource: pageSizes,
        value: pageSize,
        valueChange: pageSizeChange,
        width: width
    })
};
export var PageSizeSmallProps = {};
var PageSizeSmallPropsType = {
    get pageSize() {
        return InternalPagerProps.pageSize
    }
};
export class PageSizeSmall extends InfernoComponent {
    constructor(props) {
        super(props);
        this.state = {
            minWidth: 10
        };
        this.updateWidth = this.updateWidth.bind(this)
    }
    createEffects() {
        return [new InfernoEffect(this.updateWidth, [this.props, this.state.minWidth, this.props.pageSize, this.props.pageSizeChange, this.props.pageSizes])]
    }
    updateEffects() {
        var _this$_effects$;
        null === (_this$_effects$ = this._effects[0]) || void 0 === _this$_effects$ ? void 0 : _this$_effects$.update([this.props, this.state.minWidth, this.props.pageSize, this.props.pageSizeChange, this.props.pageSizes])
    }
    updateWidth() {
        this.setState(__state_argument => ({
            minWidth: getElementMinWidth(this.props.parentRef.current) || __state_argument.minWidth
        }))
    }
    get width() {
        return calculateValuesFittedWidth(this.state.minWidth, this.props.pageSizes.map(p => p.value))
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
            width: this.width,
            restAttributes: this.restAttributes
        })
    }
}
PageSizeSmall.defaultProps = PageSizeSmallPropsType;
