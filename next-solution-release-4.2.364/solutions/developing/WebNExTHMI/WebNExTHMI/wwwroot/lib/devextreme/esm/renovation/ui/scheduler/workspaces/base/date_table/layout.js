/**
 * DevExtreme (esm/renovation/ui/scheduler/workspaces/base/date_table/layout.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import _objectWithoutPropertiesLoose from "@babel/runtime/helpers/esm/objectWithoutPropertiesLoose";
import _extends from "@babel/runtime/helpers/esm/extends";
var _excluded = ["addDateTableClass", "addVerticalSizesClassToRows", "bottomVirtualRowHeight", "cellTemplate", "dataCellTemplate", "groupOrientation", "leftVirtualCellWidth", "rightVirtualCellWidth", "tableRef", "topVirtualRowHeight", "viewData", "width"];
import {
    createComponentVNode,
    normalizeProps
} from "inferno";
import {
    InfernoWrapperComponent
} from "@devextreme/runtime/inferno";
import {
    Table
} from "../table";
import {
    DateTableBody
} from "./table_body";
import {
    LayoutProps
} from "../layout_props";
import {
    DateTableCellBase
} from "./cell";
export var viewFunction = _ref => {
    var {
        bottomVirtualRowHeight: bottomVirtualRowHeight,
        classes: classes,
        leftVirtualCellWidth: leftVirtualCellWidth,
        props: {
            addVerticalSizesClassToRows: addVerticalSizesClassToRows,
            cellTemplate: cellTemplate,
            dataCellTemplate: dataCellTemplate,
            groupOrientation: groupOrientation,
            tableRef: tableRef,
            viewData: viewData,
            width: width
        },
        restAttributes: restAttributes,
        rightVirtualCellWidth: rightVirtualCellWidth,
        topVirtualRowHeight: topVirtualRowHeight,
        virtualCellsCount: virtualCellsCount
    } = _ref;
    return normalizeProps(createComponentVNode(2, Table, _extends({}, restAttributes, {
        tableRef: tableRef,
        topVirtualRowHeight: topVirtualRowHeight,
        bottomVirtualRowHeight: bottomVirtualRowHeight,
        leftVirtualCellWidth: leftVirtualCellWidth,
        rightVirtualCellWidth: rightVirtualCellWidth,
        leftVirtualCellCount: viewData.leftVirtualCellCount,
        rightVirtualCellCount: viewData.rightVirtualCellCount,
        virtualCellsCount: virtualCellsCount,
        className: classes,
        width: width,
        children: createComponentVNode(2, DateTableBody, {
            cellTemplate: cellTemplate,
            viewData: viewData,
            dataCellTemplate: dataCellTemplate,
            leftVirtualCellWidth: leftVirtualCellWidth,
            rightVirtualCellWidth: rightVirtualCellWidth,
            groupOrientation: groupOrientation,
            addVerticalSizesClassToRows: addVerticalSizesClassToRows
        })
    })))
};
export var DateTableLayoutProps = Object.create(Object.prototype, _extends(Object.getOwnPropertyDescriptors(LayoutProps), Object.getOwnPropertyDescriptors({
    cellTemplate: DateTableCellBase
})));
import {
    createReRenderEffect
} from "@devextreme/runtime/inferno";
var getTemplate = TemplateProp => TemplateProp && (TemplateProp.defaultProps ? props => normalizeProps(createComponentVNode(2, TemplateProp, _extends({}, props))) : TemplateProp);
export class DateTableLayoutBase extends InfernoWrapperComponent {
    constructor(props) {
        super(props);
        this.state = {}
    }
    createEffects() {
        return [createReRenderEffect()]
    }
    get classes() {
        var {
            addDateTableClass: addDateTableClass
        } = this.props;
        return addDateTableClass ? "dx-scheduler-date-table" : void 0
    }
    get topVirtualRowHeight() {
        var _this$props$viewData$;
        return null !== (_this$props$viewData$ = this.props.viewData.topVirtualRowHeight) && void 0 !== _this$props$viewData$ ? _this$props$viewData$ : 0
    }
    get bottomVirtualRowHeight() {
        var _this$props$viewData$2;
        return null !== (_this$props$viewData$2 = this.props.viewData.bottomVirtualRowHeight) && void 0 !== _this$props$viewData$2 ? _this$props$viewData$2 : 0
    }
    get leftVirtualCellWidth() {
        var _this$props$viewData$3;
        return null !== (_this$props$viewData$3 = this.props.viewData.leftVirtualCellWidth) && void 0 !== _this$props$viewData$3 ? _this$props$viewData$3 : 0
    }
    get rightVirtualCellWidth() {
        var _this$props$viewData$4;
        return null !== (_this$props$viewData$4 = this.props.viewData.rightVirtualCellWidth) && void 0 !== _this$props$viewData$4 ? _this$props$viewData$4 : 0
    }
    get virtualCellsCount() {
        return this.props.viewData.groupedData[0].dateTable[0].length
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
            classes: this.classes,
            topVirtualRowHeight: this.topVirtualRowHeight,
            bottomVirtualRowHeight: this.bottomVirtualRowHeight,
            leftVirtualCellWidth: this.leftVirtualCellWidth,
            rightVirtualCellWidth: this.rightVirtualCellWidth,
            virtualCellsCount: this.virtualCellsCount,
            restAttributes: this.restAttributes
        })
    }
}
DateTableLayoutBase.defaultProps = DateTableLayoutProps;
