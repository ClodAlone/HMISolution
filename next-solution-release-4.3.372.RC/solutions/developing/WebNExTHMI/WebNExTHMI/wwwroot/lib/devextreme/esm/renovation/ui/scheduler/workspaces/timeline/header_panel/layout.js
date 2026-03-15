/**
 * DevExtreme (esm/renovation/ui/scheduler/workspaces/timeline/header_panel/layout.js)
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
    createComponentVNode,
    normalizeProps
} from "inferno";
import {
    InfernoWrapperComponent
} from "@devextreme/runtime/inferno";
import {
    HeaderPanelLayout,
    HeaderPanelLayoutProps
} from "../../base/header_panel/layout";
import {
    TimelineDateHeaderLayout
} from "./date_header/layout";
export var viewFunction = _ref => {
    var {
        props: {
            dateCellTemplate: dateCellTemplate,
            dateHeaderData: dateHeaderData,
            groupByDate: groupByDate,
            groupOrientation: groupOrientation,
            groupPanelData: groupPanelData,
            groups: groups,
            isRenderDateHeader: isRenderDateHeader,
            resourceCellTemplate: resourceCellTemplate,
            timeCellTemplate: timeCellTemplate
        }
    } = _ref;
    return createComponentVNode(2, HeaderPanelLayout, {
        dateHeaderTemplate: TimelineDateHeaderLayout,
        dateHeaderData: dateHeaderData,
        groupPanelData: groupPanelData,
        groupByDate: groupByDate,
        groups: groups,
        groupOrientation: groupOrientation,
        isRenderDateHeader: isRenderDateHeader,
        resourceCellTemplate: resourceCellTemplate,
        dateCellTemplate: dateCellTemplate,
        timeCellTemplate: timeCellTemplate
    })
};
import {
    createReRenderEffect
} from "@devextreme/runtime/inferno";
var getTemplate = TemplateProp => TemplateProp && (TemplateProp.defaultProps ? props => normalizeProps(createComponentVNode(2, TemplateProp, _extends({}, props))) : TemplateProp);
export class TimelineHeaderPanelLayout extends InfernoWrapperComponent {
    constructor(props) {
        super(props);
        this.state = {}
    }
    createEffects() {
        return [createReRenderEffect()]
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
            restAttributes: this.restAttributes
        })
    }
}
TimelineHeaderPanelLayout.defaultProps = HeaderPanelLayoutProps;
