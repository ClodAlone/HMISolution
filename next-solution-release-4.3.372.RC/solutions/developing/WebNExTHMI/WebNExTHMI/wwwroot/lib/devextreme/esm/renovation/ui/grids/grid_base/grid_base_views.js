/**
 * DevExtreme (esm/renovation/ui/grids/grid_base/grid_base_views.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import _extends from "@babel/runtime/helpers/esm/extends";
import _objectWithoutPropertiesLoose from "@babel/runtime/helpers/esm/objectWithoutPropertiesLoose";
var _excluded = ["className", "onRendered", "role", "showBorders", "views"];
import {
    createVNode,
    createComponentVNode
} from "inferno";
import {
    BaseInfernoComponent
} from "@devextreme/runtime/inferno";
import {
    combineClasses
} from "../../../utils/combine_classes";
import {
    GridBaseViewWrapper
} from "./grid_base_view_wrapper";
import {
    DataGridProps
} from "../data_grid/common/data_grid_props";
var GRIDBASE_CONTAINER_CLASS = "dx-gridbase-container";
var BORDERS_CLASS = "borders";
export var viewFunction = _ref => {
    var {
        className: className,
        props: {
            views: views
        },
        viewRendered: viewRendered
    } = _ref;
    return createVNode(1, "div", className, views.map(_ref2 => {
        var {
            name: name,
            view: view
        } = _ref2;
        return createComponentVNode(2, GridBaseViewWrapper, {
            view: view,
            onRendered: viewRendered
        }, name)
    }), 0, {
        role: "grid"
    })
};
var GridBaseViewPropsType = {
    get showBorders() {
        return DataGridProps.showBorders
    }
};
export class GridBaseViews extends BaseInfernoComponent {
    constructor(props) {
        super(props);
        this.state = {};
        this.viewRenderedCount = 0;
        this.viewRendered = this.viewRendered.bind(this)
    }
    get className() {
        var {
            showBorders: showBorders
        } = this.props;
        return combineClasses({
            [GRIDBASE_CONTAINER_CLASS]: true,
            ["".concat(this.props.className)]: !!this.props.className,
            ["".concat(this.props.className, "-").concat(BORDERS_CLASS)]: !!showBorders
        })
    }
    viewRendered() {
        this.viewRenderedCount += 1;
        if (this.viewRenderedCount === this.props.views.length) {
            var _this$props$onRendere, _this$props;
            null === (_this$props$onRendere = (_this$props = this.props).onRendered) || void 0 === _this$props$onRendere ? void 0 : _this$props$onRendere.call(_this$props)
        }
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
            className: this.className,
            viewRendered: this.viewRendered,
            restAttributes: this.restAttributes
        })
    }
}
GridBaseViews.defaultProps = GridBaseViewPropsType;
