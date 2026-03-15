/**
 * DevExtreme (esm/renovation/ui/pager/pages/page.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import _extends from "@babel/runtime/helpers/esm/extends";
import _objectWithoutPropertiesLoose from "@babel/runtime/helpers/esm/objectWithoutPropertiesLoose";
var _excluded = ["className", "index", "onClick", "selected"];
import {
    createComponentVNode
} from "inferno";
import {
    BaseInfernoComponent
} from "@devextreme/runtime/inferno";
import {
    LightButton
} from "../common/light_button";
import {
    PAGER_PAGE_CLASS,
    PAGER_SELECTION_CLASS
} from "../common/consts";
import {
    combineClasses
} from "../../../utils/combine_classes";
export var viewFunction = _ref => {
    var {
        className: className,
        label: label,
        props: {
            onClick: onClick
        },
        value: value
    } = _ref;
    return createComponentVNode(2, LightButton, {
        className: className,
        label: label,
        onClick: onClick,
        children: value
    })
};
export var PageProps = {
    index: 0,
    selected: false,
    className: PAGER_PAGE_CLASS
};
export class Page extends BaseInfernoComponent {
    constructor(props) {
        super(props);
        this.state = {}
    }
    get label() {
        return "Page ".concat(this.value)
    }
    get value() {
        return this.props.index + 1
    }
    get className() {
        var {
            selected: selected
        } = this.props;
        return combineClasses({
            ["".concat(this.props.className)]: !!this.props.className,
            [PAGER_SELECTION_CLASS]: !!selected
        })
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
            label: this.label,
            value: this.value,
            className: this.className,
            restAttributes: this.restAttributes
        })
    }
}
Page.defaultProps = PageProps;
