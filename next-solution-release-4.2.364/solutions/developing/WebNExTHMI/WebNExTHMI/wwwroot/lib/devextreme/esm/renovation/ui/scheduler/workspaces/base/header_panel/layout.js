/**
 * DevExtreme (esm/renovation/ui/scheduler/workspaces/base/header_panel/layout.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import _objectWithoutPropertiesLoose from "@babel/runtime/helpers/esm/objectWithoutPropertiesLoose";
import _extends from "@babel/runtime/helpers/esm/extends";
var _excluded = ["className", "dateCellTemplate", "dateHeaderData", "dateHeaderTemplate", "elementRef", "groupByDate", "groupOrientation", "groupPanelData", "groups", "height", "isRenderDateHeader", "resourceCellTemplate", "timeCellTemplate"];
import {
    createVNode,
    createComponentVNode,
    normalizeProps
} from "inferno";
import {
    InfernoWrapperComponent
} from "@devextreme/runtime/inferno";
import {
    isHorizontalGroupingApplied
} from "../../utils";
import {
    GroupPanel,
    GroupPanelProps
} from "../group_panel/group_panel";
import {
    DateHeaderLayout
} from "./date_header/layout";
export var viewFunction = _ref => {
    var {
        isHorizontalGrouping: isHorizontalGrouping,
        props: {
            dateCellTemplate: dateCellTemplate,
            dateHeaderData: dateHeaderData,
            dateHeaderTemplate: DateHeader,
            groupByDate: groupByDate,
            groupOrientation: groupOrientation,
            groupPanelData: groupPanelData,
            groups: groups,
            isRenderDateHeader: isRenderDateHeader,
            resourceCellTemplate: resourceCellTemplate,
            timeCellTemplate: timeCellTemplate
        }
    } = _ref;
    return createVNode(1, "thead", null, [isHorizontalGrouping && !groupByDate && createComponentVNode(2, GroupPanel, {
        groupPanelData: groupPanelData,
        groups: groups,
        groupByDate: groupByDate,
        groupOrientation: groupOrientation,
        resourceCellTemplate: resourceCellTemplate
    }), isRenderDateHeader && DateHeader({
        groupByDate: groupByDate,
        dateHeaderData: dateHeaderData,
        groupOrientation: groupOrientation,
        groups: groups,
        dateCellTemplate: dateCellTemplate,
        timeCellTemplate: timeCellTemplate
    }), groupByDate && createComponentVNode(2, GroupPanel, {
        groupPanelData: groupPanelData,
        groups: groups,
        groupByDate: groupByDate,
        groupOrientation: groupOrientation,
        resourceCellTemplate: resourceCellTemplate
    })], 0)
};
export var HeaderPanelLayoutProps = Object.create(Object.prototype, _extends(Object.getOwnPropertyDescriptors(GroupPanelProps), Object.getOwnPropertyDescriptors({
    isRenderDateHeader: true,
    dateHeaderTemplate: DateHeaderLayout
})));
import {
    createReRenderEffect
} from "@devextreme/runtime/inferno";
var getTemplate = TemplateProp => TemplateProp && (TemplateProp.defaultProps ? props => normalizeProps(createComponentVNode(2, TemplateProp, _extends({}, props))) : TemplateProp);
export class HeaderPanelLayout extends InfernoWrapperComponent {
    constructor(props) {
        super(props);
        this.state = {}
    }
    createEffects() {
        return [createReRenderEffect()]
    }
    get isHorizontalGrouping() {
        var {
            groupOrientation: groupOrientation,
            groups: groups
        } = this.props;
        return isHorizontalGroupingApplied(groups, groupOrientation)
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
                dateCellTemplate: getTemplate(props.dateCellTemplate),
                timeCellTemplate: getTemplate(props.timeCellTemplate),
                dateHeaderTemplate: getTemplate(props.dateHeaderTemplate),
                resourceCellTemplate: getTemplate(props.resourceCellTemplate)
            }),
            isHorizontalGrouping: this.isHorizontalGrouping,
            restAttributes: this.restAttributes
        })
    }
}
HeaderPanelLayout.defaultProps = HeaderPanelLayoutProps;
