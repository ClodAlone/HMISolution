/**
 * DevExtreme (esm/renovation/ui/scheduler/workspaces/base/date_table/table_body.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import _objectWithoutPropertiesLoose from "@babel/runtime/helpers/esm/objectWithoutPropertiesLoose";
import _extends from "@babel/runtime/helpers/esm/extends";
var _excluded = ["addDateTableClass", "addVerticalSizesClassToRows", "bottomVirtualRowHeight", "cellTemplate", "dataCellTemplate", "groupOrientation", "leftVirtualCellWidth", "rightVirtualCellWidth", "topVirtualRowHeight", "viewData", "width"];
import {
    createFragment,
    createComponentVNode,
    normalizeProps
} from "inferno";
import {
    Fragment
} from "inferno";
import {
    BaseInfernoComponent
} from "@devextreme/runtime/inferno";
import {
    Row
} from "../row";
import {
    getKeyByGroup,
    getIsGroupedAllDayPanel
} from "../../utils";
import {
    AllDayPanelTableBody
} from "./all_day_panel/table_body";
import {
    LayoutProps
} from "../layout_props";
import {
    DateTableCellBase
} from "./cell";
import {
    combineClasses
} from "../../../../../utils/combine_classes";
export var viewFunction = _ref => {
    var {
        props: {
            cellTemplate: Cell,
            dataCellTemplate: dataCellTemplate,
            groupOrientation: groupOrientation,
            viewData: viewData
        },
        rowClasses: rowClasses
    } = _ref;
    return createFragment(viewData.groupedData.map((_ref2, index) => {
        var {
            allDayPanel: allDayPanel,
            dateTable: dateTable,
            groupIndex: groupIndex
        } = _ref2;
        return createFragment([getIsGroupedAllDayPanel(viewData, index) && createComponentVNode(2, AllDayPanelTableBody, {
            viewData: allDayPanel,
            dataCellTemplate: dataCellTemplate,
            isVerticalGroupOrientation: true,
            leftVirtualCellWidth: viewData.leftVirtualCellWidth,
            rightVirtualCellWidth: viewData.rightVirtualCellWidth,
            leftVirtualCellCount: viewData.leftVirtualCellCount,
            rightVirtualCellCount: viewData.rightVirtualCellCount
        }), dateTable.map(cellsRow => createComponentVNode(2, Row, {
            className: rowClasses,
            leftVirtualCellWidth: viewData.leftVirtualCellWidth,
            rightVirtualCellWidth: viewData.rightVirtualCellWidth,
            leftVirtualCellCount: viewData.leftVirtualCellCount,
            rightVirtualCellCount: viewData.rightVirtualCellCount,
            children: cellsRow.map(_ref3 => {
                var {
                    endDate: endDate,
                    firstDayOfMonth: firstDayOfMonth,
                    groupIndex: cellGroupIndex,
                    groups: groups,
                    index: cellIndex,
                    isFirstGroupCell: isFirstGroupCell,
                    isFocused: isFocused,
                    isLastGroupCell: isLastGroupCell,
                    isSelected: isSelected,
                    key: key,
                    otherMonth: otherMonth,
                    startDate: startDate,
                    text: text,
                    today: today
                } = _ref3;
                return Cell({
                    isFirstGroupCell: isFirstGroupCell,
                    isLastGroupCell: isLastGroupCell,
                    startDate: startDate,
                    endDate: endDate,
                    groups: groups,
                    groupIndex: cellGroupIndex,
                    index: cellIndex,
                    dataCellTemplate: dataCellTemplate,
                    key: key,
                    text: text,
                    today: today,
                    otherMonth: otherMonth,
                    firstDayOfMonth: firstDayOfMonth,
                    isSelected: isSelected,
                    isFocused: isFocused
                })
            })
        }, cellsRow[0].key - viewData.leftVirtualCellCount))], 0, getKeyByGroup(groupIndex, groupOrientation))
    }), 0)
};
export var DateTableBodyProps = Object.create(Object.prototype, _extends(Object.getOwnPropertyDescriptors(LayoutProps), Object.getOwnPropertyDescriptors({
    cellTemplate: DateTableCellBase
})));
var getTemplate = TemplateProp => TemplateProp && (TemplateProp.defaultProps ? props => normalizeProps(createComponentVNode(2, TemplateProp, _extends({}, props))) : TemplateProp);
export class DateTableBody extends BaseInfernoComponent {
    constructor(props) {
        super(props);
        this.state = {}
    }
    get rowClasses() {
        var {
            addVerticalSizesClassToRows: addVerticalSizesClassToRows
        } = this.props;
        return combineClasses({
            "dx-scheduler-date-table-row": true,
            "dx-scheduler-cell-sizes-vertical": addVerticalSizesClassToRows
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
                cellTemplate: getTemplate(props.cellTemplate),
                dataCellTemplate: getTemplate(props.dataCellTemplate)
            }),
            rowClasses: this.rowClasses,
            restAttributes: this.restAttributes
        })
    }
}
DateTableBody.defaultProps = DateTableBodyProps;
