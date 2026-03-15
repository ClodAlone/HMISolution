/**
 * DevExtreme (esm/renovation/ui/scheduler/workspaces/base/group_panel/vertical/row.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import _objectWithoutPropertiesLoose from "@babel/runtime/helpers/esm/objectWithoutPropertiesLoose";
import _extends from "@babel/runtime/helpers/esm/extends";
var _excluded = ["cellTemplate", "className", "groupItems"];
import {
    createVNode,
    createComponentVNode,
    normalizeProps
} from "inferno";
import {
    BaseInfernoComponent
} from "@devextreme/runtime/inferno";
import {
    GroupPanelVerticalCell
} from "./cell";
import {
    GroupPanelRowProps
} from "../row_props";
export var viewFunction = viewModel => createVNode(1, "div", "dx-scheduler-group-row ".concat(viewModel.props.className), viewModel.props.groupItems.map((_ref, index) => {
    var {
        color: color,
        data: data,
        id: id,
        key: key,
        text: text
    } = _ref;
    return createComponentVNode(2, GroupPanelVerticalCell, {
        text: text,
        id: id,
        data: data,
        index: index,
        color: color,
        cellTemplate: viewModel.props.cellTemplate
    }, key)
}), 0);
var getTemplate = TemplateProp => TemplateProp && (TemplateProp.defaultProps ? props => normalizeProps(createComponentVNode(2, TemplateProp, _extends({}, props))) : TemplateProp);
export class Row extends BaseInfernoComponent {
    constructor(props) {
        super(props);
        this.state = {}
    }
    get restAttributes() {
        var _this$props = this.props,
            restProps = _objectWithoutPropertiesLoose(_this$props, _excluded);
        return restProps
    }
    render() {
        var props = this.props;
        return viewFunction({
            props: _extends({}, props, {
                cellTemplate: getTemplate(props.cellTemplate)
            }),
            restAttributes: this.restAttributes
        })
    }
}
Row.defaultProps = GroupPanelRowProps;
