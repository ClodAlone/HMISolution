/**
 * DevExtreme (esm/renovation/ui/scheduler/workspaces/base/group_panel/horizontal/row.js)
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
    GroupPanelHorizontalCell
} from "./cell";
import {
    GroupPanelRowProps
} from "../row_props";
export var viewFunction = _ref => {
    var {
        props: {
            cellTemplate: cellTemplate,
            className: className,
            groupItems: groupItems
        }
    } = _ref;
    return createVNode(1, "tr", "dx-scheduler-group-row ".concat(className), groupItems.map((_ref2, index) => {
        var {
            colSpan: colSpan,
            color: color,
            data: data,
            id: id,
            isFirstGroupCell: isFirstGroupCell,
            isLastGroupCell: isLastGroupCell,
            key: key,
            text: text
        } = _ref2;
        return createComponentVNode(2, GroupPanelHorizontalCell, {
            text: text,
            id: id,
            data: data,
            index: index,
            color: color,
            colSpan: colSpan,
            isFirstGroupCell: !!isFirstGroupCell,
            isLastGroupCell: !!isLastGroupCell,
            cellTemplate: cellTemplate
        }, key)
    }), 0)
};
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
