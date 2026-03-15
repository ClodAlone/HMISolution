/**
 * DevExtreme (esm/renovation/ui/grids/grid_base/grid_base_view_wrapper.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import _extends from "@babel/runtime/helpers/esm/extends";
import _objectWithoutPropertiesLoose from "@babel/runtime/helpers/esm/objectWithoutPropertiesLoose";
var _excluded = ["onRendered", "view"];
import {
    createVNode
} from "inferno";
import {
    InfernoEffect,
    InfernoComponent
} from "@devextreme/runtime/inferno";
import $ from "../../../../core/renderer";
export var viewFunction = _ref => {
    var {
        viewRef: viewRef
    } = _ref;
    return createVNode(1, "div", null, null, 1, null, null, viewRef)
};
export var GridBaseViewWrapperProps = {};
import {
    createRef as infernoCreateRef
} from "inferno";
export class GridBaseViewWrapper extends InfernoComponent {
    constructor(props) {
        super(props);
        this.state = {};
        this.viewRef = infernoCreateRef();
        this.renderView = this.renderView.bind(this)
    }
    createEffects() {
        return [new InfernoEffect(this.renderView, [])]
    }
    renderView() {
        var _this$props$onRendere, _this$props;
        var $element = $(this.viewRef.current);
        this.props.view._$element = $element;
        this.props.view._$parent = $element.parent();
        this.props.view.render();
        null === (_this$props$onRendere = (_this$props = this.props).onRendered) || void 0 === _this$props$onRendere ? void 0 : _this$props$onRendere.call(_this$props)
    }
    get restAttributes() {
        var _this$props2 = this.props,
            restProps = _objectWithoutPropertiesLoose(_this$props2, _excluded);
        return restProps
    }
    render() {
        var props = this.props;
        return viewFunction({
            props: _extends({}, props),
            viewRef: this.viewRef,
            restAttributes: this.restAttributes
        })
    }
}
GridBaseViewWrapper.defaultProps = GridBaseViewWrapperProps;
