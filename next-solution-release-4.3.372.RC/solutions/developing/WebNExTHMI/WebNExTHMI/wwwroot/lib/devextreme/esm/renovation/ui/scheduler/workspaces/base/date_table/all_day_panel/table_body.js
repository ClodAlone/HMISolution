/**
 * DevExtreme (esm/renovation/ui/scheduler/workspaces/base/date_table/all_day_panel/table_body.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import _objectWithoutPropertiesLoose from "@babel/runtime/helpers/esm/objectWithoutPropertiesLoose";
import _extends from "@babel/runtime/helpers/esm/extends";
var _excluded = ["className", "dataCellTemplate", "isVerticalGroupOrientation", "leftVirtualCellCount", "leftVirtualCellWidth", "rightVirtualCellCount", "rightVirtualCellWidth", "viewData"];
import {
    createComponentVNode,
    normalizeProps
} from "inferno";
import {
    BaseInfernoComponent
} from "@devextreme/runtime/inferno";
import {
    Row
} from "../../row";
import {
    AllDayPanelCell as Cell
} from "./cell";
import {
    combineClasses
} from "../../../../../../utils/combine_classes";
export var viewFunction = viewModel => createComponentVNode(2, Row, {
    leftVirtualCellWidth: viewModel.props.leftVirtualCellWidth,
    rightVirtualCellWidth: viewModel.props.rightVirtualCellWidth,
    leftVirtualCellCount: viewModel.props.leftVirtualCellCount,
    rightVirtualCellCount: viewModel.props.rightVirtualCellCount,
    className: viewModel.classes,
    children: viewModel.props.viewData.map(_ref => {
        var {
            endDate: endDate,
            groupIndex: cellGroupIndex,
            groups: groups,
            index: cellIndex,
            isFirstGroupCell: isFirstGroupCell,
            isFocused: isFocused,
            isLastGroupCell: isLastGroupCell,
            isSelected: isSelected,
            key: key,
            startDate: startDate
        } = _ref;
        return createComponentVNode(2, Cell, {
            isFirstGroupCell: !viewModel.props.isVerticalGroupOrientation && isFirstGroupCell,
            isLastGroupCell: !viewModel.props.isVerticalGroupOrientation && isLastGroupCell,
            startDate: startDate,
            endDate: endDate,
            groups: groups,
            groupIndex: cellGroupIndex,
            index: cellIndex,
            dataCellTemplate: viewModel.props.dataCellTemplate,
            isSelected: isSelected,
            isFocused: isFocused
        }, key)
    })
});
export var AllDayPanelTableBodyProps = {
    get viewData() {
        return []
    },
    isVerticalGroupOrientation: false,
    className: "",
    leftVirtualCellWidth: 0,
    rightVirtualCellWidth: 0
};
var getTemplate = TemplateProp => TemplateProp && (TemplateProp.defaultProps ? props => normalizeProps(createComponentVNode(2, TemplateProp, _extends({}, props))) : TemplateProp);
export class AllDayPanelTableBody extends BaseInfernoComponent {
    constructor(props) {
        super(props);
        this.state = {}
    }
    get classes() {
        var {
            className: className
        } = this.props;
        return combineClasses({
            "dx-scheduler-all-day-table-row": true,
            [className]: !!className
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
            props: _extends({}, props, {
                dataCellTemplate: getTemplate(props.dataCellTemplate)
            }),
            classes: this.classes,
            restAttributes: this.restAttributes
        })
    }
}
AllDayPanelTableBody.defaultProps = AllDayPanelTableBodyProps;
