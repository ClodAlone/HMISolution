/**
 * DevExtreme (esm/renovation/ui/scheduler/workspaces/base/group_panel/horizontal/cell.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import _objectWithoutPropertiesLoose from "@babel/runtime/helpers/esm/objectWithoutPropertiesLoose";
import _extends from "@babel/runtime/helpers/esm/extends";
var _excluded = ["cellTemplate", "className", "colSpan", "color", "data", "id", "index", "isFirstGroupCell", "isLastGroupCell", "text"];
import {
    createVNode,
    createComponentVNode,
    normalizeProps
} from "inferno";
import {
    BaseInfernoComponent
} from "@devextreme/runtime/inferno";
import {
    combineClasses
} from "../../../../../../utils/combine_classes";
import {
    GroupPanelCellProps
} from "../cell_props";
export var viewFunction = _ref => {
    var {
        classes: classes,
        props: {
            cellTemplate: CellTemplate,
            colSpan: colSpan,
            color: color,
            data: data,
            id: id,
            index: index,
            text: text
        }
    } = _ref;
    return createVNode(1, "th", classes, createVNode(1, "div", "dx-scheduler-group-header-content", [!!CellTemplate && CellTemplate({
        data: {
            data: data,
            id: id,
            color: color,
            text: text
        },
        index: index
    }), !CellTemplate && createVNode(1, "div", null, text, 0)], 0), 2, {
        colSpan: colSpan
    })
};
export var GroupPanelHorizontalCellProps = Object.create(Object.prototype, _extends(Object.getOwnPropertyDescriptors(GroupPanelCellProps), Object.getOwnPropertyDescriptors({
    isFirstGroupCell: false,
    isLastGroupCell: false,
    colSpan: 1
})));
var getTemplate = TemplateProp => TemplateProp && (TemplateProp.defaultProps ? props => normalizeProps(createComponentVNode(2, TemplateProp, _extends({}, props))) : TemplateProp);
export class GroupPanelHorizontalCell extends BaseInfernoComponent {
    constructor(props) {
        super(props);
        this.state = {}
    }
    get classes() {
        var {
            className: className,
            isFirstGroupCell: isFirstGroupCell,
            isLastGroupCell: isLastGroupCell
        } = this.props;
        return combineClasses({
            "dx-scheduler-group-header": true,
            "dx-scheduler-first-group-cell": isFirstGroupCell,
            "dx-scheduler-last-group-cell": isLastGroupCell,
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
                cellTemplate: getTemplate(props.cellTemplate)
            }),
            classes: this.classes,
            restAttributes: this.restAttributes
        })
    }
}
GroupPanelHorizontalCell.defaultProps = GroupPanelHorizontalCellProps;
