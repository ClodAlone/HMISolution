/**
 * DevExtreme (esm/renovation/ui/scheduler/workspaces/base/header_panel/date_header/cell.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import _objectWithoutPropertiesLoose from "@babel/runtime/helpers/esm/objectWithoutPropertiesLoose";
import _extends from "@babel/runtime/helpers/esm/extends";
var _excluded = ["allDay", "ariaLabel", "children", "className", "colSpan", "contentTemplate", "contentTemplateProps", "dateCellTemplate", "endDate", "groupIndex", "groups", "index", "isFirstGroupCell", "isLastGroupCell", "isTimeCellTemplate", "isWeekDayCell", "startDate", "text", "timeCellTemplate", "today"];
import {
    createVNode,
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
    CellBaseProps
} from "../../cell";
import {
    combineClasses
} from "../../../../../../utils/combine_classes";
import {
    getGroupCellClasses
} from "../../../utils";
export var viewFunction = _ref => {
    var {
        classes: classes,
        props: {
            colSpan: colSpan,
            dateCellTemplate: DateCellTemplate,
            groupIndex: groupIndex,
            groups: groups,
            index: index,
            isTimeCellTemplate: isTimeCellTemplate,
            startDate: startDate,
            text: text,
            timeCellTemplate: TimeCellTemplate
        },
        useTemplate: useTemplate
    } = _ref;
    return createVNode(1, "th", classes, useTemplate ? createFragment([isTimeCellTemplate && TimeCellTemplate && TimeCellTemplate({
        data: {
            date: startDate,
            text: text,
            groups: groups,
            groupIndex: groupIndex
        },
        index: index
    }), !isTimeCellTemplate && DateCellTemplate && DateCellTemplate({
        data: {
            date: startDate,
            text: text,
            groups: groups,
            groupIndex: groupIndex
        },
        index: index
    })], 0) : text, 0, {
        colSpan: colSpan,
        title: text
    })
};
export var DateHeaderCellProps = Object.create(Object.prototype, _extends(Object.getOwnPropertyDescriptors(CellBaseProps), Object.getOwnPropertyDescriptors({
    today: false,
    colSpan: 1,
    isWeekDayCell: false,
    isTimeCellTemplate: false
})));
var getTemplate = TemplateProp => TemplateProp && (TemplateProp.defaultProps ? props => normalizeProps(createComponentVNode(2, TemplateProp, _extends({}, props))) : TemplateProp);
export class DateHeaderCell extends BaseInfernoComponent {
    constructor(props) {
        super(props);
        this.state = {}
    }
    get classes() {
        var {
            className: className,
            isFirstGroupCell: isFirstGroupCell,
            isLastGroupCell: isLastGroupCell,
            isWeekDayCell: isWeekDayCell,
            today: today
        } = this.props;
        var cellClasses = combineClasses({
            "dx-scheduler-header-panel-cell": true,
            "dx-scheduler-cell-sizes-horizontal": true,
            "dx-scheduler-header-panel-current-time-cell": today,
            "dx-scheduler-header-panel-week-cell": isWeekDayCell,
            [className]: !!className
        });
        return getGroupCellClasses(isFirstGroupCell, isLastGroupCell, cellClasses)
    }
    get useTemplate() {
        var {
            dateCellTemplate: dateCellTemplate,
            isTimeCellTemplate: isTimeCellTemplate,
            timeCellTemplate: timeCellTemplate
        } = this.props;
        return !isTimeCellTemplate && !!dateCellTemplate || isTimeCellTemplate && !!timeCellTemplate
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
                timeCellTemplate: getTemplate(props.timeCellTemplate),
                dateCellTemplate: getTemplate(props.dateCellTemplate),
                contentTemplate: getTemplate(props.contentTemplate)
            }),
            classes: this.classes,
            useTemplate: this.useTemplate,
            restAttributes: this.restAttributes
        })
    }
}
DateHeaderCell.defaultProps = DateHeaderCellProps;
