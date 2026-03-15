/**
 * DevExtreme (esm/renovation/ui/scheduler/workspaces/base/date_table/cell.js)
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
    CellBase as Cell,
    CellBaseProps
} from "../cell";
import {
    combineClasses
} from "../../../../../utils/combine_classes";
var ADD_APPOINTMENT_LABEL = "Add appointment";
export var viewFunction = _ref => {
    var {
        ariaLabel: ariaLabel,
        classes: classes,
        dataCellTemplateProps: dataCellTemplateProps,
        props: {
            children: children,
            dataCellTemplate: dataCellTemplate,
            isFirstGroupCell: isFirstGroupCell,
            isLastGroupCell: isLastGroupCell
        }
    } = _ref;
    return createComponentVNode(2, Cell, {
        isFirstGroupCell: isFirstGroupCell,
        isLastGroupCell: isLastGroupCell,
        contentTemplate: dataCellTemplate,
        contentTemplateProps: dataCellTemplateProps,
        className: classes,
        ariaLabel: ariaLabel,
        children: children
    })
};
export var DateTableCellBaseProps = Object.create(Object.prototype, _extends(Object.getOwnPropertyDescriptors(CellBaseProps), Object.getOwnPropertyDescriptors({
    otherMonth: false,
    today: false,
    firstDayOfMonth: false,
    isSelected: false,
    isFocused: false
})));
var getTemplate = TemplateProp => TemplateProp && (TemplateProp.defaultProps ? props => normalizeProps(createComponentVNode(2, TemplateProp, _extends({}, props))) : TemplateProp);
export class DateTableCellBase extends BaseInfernoComponent {
    constructor(props) {
        super(props);
        this.state = {};
        this.__getterCache = {}
    }
    get classes() {
        var {
            allDay: allDay,
            className: className,
            isFocused: isFocused,
            isSelected: isSelected
        } = this.props;
        return combineClasses({
            "dx-scheduler-cell-sizes-horizontal": true,
            "dx-scheduler-cell-sizes-vertical": !allDay,
            "dx-scheduler-date-table-cell": !allDay,
            "dx-state-focused": isSelected,
            "dx-scheduler-focused-cell": isFocused,
            [className]: true
        })
    }
    get dataCellTemplateProps() {
        if (void 0 !== this.__getterCache.dataCellTemplateProps) {
            return this.__getterCache.dataCellTemplateProps
        }
        return this.__getterCache.dataCellTemplateProps = (() => {
            var {
                allDay: allDay,
                contentTemplateProps: contentTemplateProps,
                endDate: endDate,
                groupIndex: groupIndex,
                groups: groups,
                index: index,
                startDate: startDate
            } = this.props;
            return {
                data: _extends({
                    startDate: startDate,
                    endDate: endDate,
                    groups: groups,
                    groupIndex: groups ? groupIndex : void 0,
                    text: "",
                    allDay: !!allDay || void 0
                }, contentTemplateProps.data),
                index: index
            }
        })()
    }
    get ariaLabel() {
        return this.props.isSelected ? ADD_APPOINTMENT_LABEL : void 0
    }
    get restAttributes() {
        var _this$props = this.props,
            restProps = _objectWithoutPropertiesLoose(_this$props, _excluded);
        return restProps
    }
    componentWillUpdate(nextProps, nextState, context) {
        if (this.props.allDay !== nextProps.allDay || this.props.contentTemplateProps !== nextProps.contentTemplateProps || this.props.endDate !== nextProps.endDate || this.props.groupIndex !== nextProps.groupIndex || this.props.groups !== nextProps.groups || this.props.index !== nextProps.index || this.props.startDate !== nextProps.startDate) {
            this.__getterCache.dataCellTemplateProps = void 0
        }
    }
    render() {
        var props = this.props;
        return viewFunction({
            props: _extends({}, props, {
                dataCellTemplate: getTemplate(props.dataCellTemplate),
                contentTemplate: getTemplate(props.contentTemplate)
            }),
            classes: this.classes,
            dataCellTemplateProps: this.dataCellTemplateProps,
            ariaLabel: this.ariaLabel,
            restAttributes: this.restAttributes
        })
    }
}
DateTableCellBase.defaultProps = DateTableCellBaseProps;
