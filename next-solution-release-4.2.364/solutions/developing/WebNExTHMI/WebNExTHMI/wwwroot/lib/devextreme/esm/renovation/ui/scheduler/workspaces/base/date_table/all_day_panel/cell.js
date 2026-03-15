/**
 * DevExtreme (esm/renovation/ui/scheduler/workspaces/base/date_table/all_day_panel/cell.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import _objectWithoutPropertiesLoose from "@babel/runtime/helpers/esm/objectWithoutPropertiesLoose";
import _extends from "@babel/runtime/helpers/esm/extends";
var _excluded = ["allDay", "ariaLabel", "children", "className", "contentTemplate", "contentTemplateProps", "dataCellTemplate", "endDate", "firstDayOfMonth", "groupIndex", "groups", "index", "isFirstGroupCell", "isFocused", "isLastGroupCell", "isSelected", "otherMonth", "startDate", "text", "today"];
import {
    createComponentVNode,
    normalizeProps
} from "inferno";
import {
    BaseInfernoComponent
} from "@devextreme/runtime/inferno";
import {
    DateTableCellBaseProps,
    DateTableCellBase
} from "../cell";
export var viewFunction = _ref => {
    var {
        props: {
            className: className,
            dataCellTemplate: dataCellTemplate,
            endDate: endDate,
            groupIndex: groupIndex,
            groups: groups,
            index: index,
            isFirstGroupCell: isFirstGroupCell,
            isFocused: isFocused,
            isLastGroupCell: isLastGroupCell,
            isSelected: isSelected,
            startDate: startDate
        }
    } = _ref;
    return createComponentVNode(2, DateTableCellBase, {
        className: "dx-scheduler-all-day-table-cell ".concat(className),
        startDate: startDate,
        endDate: endDate,
        groups: groups,
        groupIndex: groupIndex,
        allDay: true,
        isFirstGroupCell: isFirstGroupCell,
        isLastGroupCell: isLastGroupCell,
        index: index,
        dataCellTemplate: dataCellTemplate,
        isSelected: isSelected,
        isFocused: isFocused
    })
};
var getTemplate = TemplateProp => TemplateProp && (TemplateProp.defaultProps ? props => normalizeProps(createComponentVNode(2, TemplateProp, _extends({}, props))) : TemplateProp);
export class AllDayPanelCell extends BaseInfernoComponent {
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
                dataCellTemplate: getTemplate(props.dataCellTemplate),
                contentTemplate: getTemplate(props.contentTemplate)
            }),
            restAttributes: this.restAttributes
        })
    }
}
AllDayPanelCell.defaultProps = DateTableCellBaseProps;
