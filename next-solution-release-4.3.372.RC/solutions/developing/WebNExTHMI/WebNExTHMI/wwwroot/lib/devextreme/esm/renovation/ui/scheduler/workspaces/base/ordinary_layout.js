/**
 * DevExtreme (esm/renovation/ui/scheduler/workspaces/base/ordinary_layout.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import _objectWithoutPropertiesLoose from "@babel/runtime/helpers/esm/objectWithoutPropertiesLoose";
import _extends from "@babel/runtime/helpers/esm/extends";
var _excluded = ["addDateTableClass", "addVerticalSizesClassToRows", "allDayAppointments", "allDayPanelRef", "appointments", "bottomVirtualRowHeight", "className", "dataCellTemplate", "dateCellTemplate", "dateHeaderData", "dateTableRef", "dateTableTemplate", "groupByDate", "groupOrientation", "groupPanelClassName", "groupPanelData", "groupPanelHeight", "groupPanelRef", "groups", "headerEmptyCellWidth", "headerPanelTemplate", "intervalCount", "isAllDayPanelCollapsed", "isAllDayPanelVisible", "isRenderDateHeader", "isRenderGroupPanel", "isRenderHeaderEmptyCell", "isStandaloneAllDayPanel", "isWorkSpaceWithOddCells", "leftVirtualCellWidth", "onScroll", "resourceCellTemplate", "rightVirtualCellWidth", "scrollingDirection", "tablesWidth", "timeCellTemplate", "timePanelData", "timePanelRef", "timePanelTemplate", "topVirtualRowHeight", "viewData", "widgetElementRef", "width"];
import {
    createVNode,
    createComponentVNode,
    normalizeProps
} from "inferno";
import {
    BaseInfernoComponent
} from "@devextreme/runtime/inferno";
import {
    Widget
} from "../../../common/widget";
import {
    Scrollable
} from "../../../scroll_view/scrollable";
import {
    GroupPanel
} from "./group_panel/group_panel";
import {
    AllDayPanelLayout
} from "./date_table/all_day_panel/layout";
import {
    HeaderPanelEmptyCell
} from "./header_panel_empty_cell";
import {
    MainLayoutProps
} from "./main_layout_props";
export var viewFunction = _ref => {
    var {
        dateTableScrollableRef: dateTableScrollableRef,
        props: {
            allDayAppointments: allDayAppointments,
            allDayPanelRef: allDayPanelRef,
            appointments: appointments,
            className: className,
            dataCellTemplate: dataCellTemplate,
            dateCellTemplate: dateCellTemplate,
            dateHeaderData: dateHeaderData,
            dateTableRef: dateTableRef,
            dateTableTemplate: DateTable,
            groupByDate: groupByDate,
            groupOrientation: groupOrientation,
            groupPanelClassName: groupPanelClassName,
            groupPanelData: groupPanelData,
            groupPanelHeight: groupPanelHeight,
            groupPanelRef: groupPanelRef,
            groups: groups,
            headerEmptyCellWidth: headerEmptyCellWidth,
            headerPanelTemplate: HeaderPanel,
            isRenderDateHeader: isRenderDateHeader,
            isRenderGroupPanel: isRenderGroupPanel,
            isRenderHeaderEmptyCell: isRenderHeaderEmptyCell,
            isStandaloneAllDayPanel: isStandaloneAllDayPanel,
            resourceCellTemplate: resourceCellTemplate,
            scrollingDirection: scrollingDirection,
            timeCellTemplate: timeCellTemplate,
            timePanelData: timePanelData,
            timePanelRef: timePanelRef,
            timePanelTemplate: TimePanel,
            viewData: viewData,
            widgetElementRef: widgetElementRef
        }
    } = _ref;
    return createComponentVNode(2, Widget, {
        className: className,
        rootElementRef: widgetElementRef,
        children: [createVNode(1, "div", "dx-scheduler-header-panel-container", [isRenderHeaderEmptyCell && createComponentVNode(2, HeaderPanelEmptyCell, {
            width: headerEmptyCellWidth,
            isRenderAllDayTitle: isStandaloneAllDayPanel
        }), createVNode(1, "div", "dx-scheduler-header-tables-container", [createVNode(1, "table", "dx-scheduler-header-panel", HeaderPanel({
            dateHeaderData: dateHeaderData,
            groupPanelData: groupPanelData,
            timeCellTemplate: timeCellTemplate,
            dateCellTemplate: dateCellTemplate,
            isRenderDateHeader: isRenderDateHeader,
            groupOrientation: groupOrientation,
            groupByDate: groupByDate,
            groups: groups,
            resourceCellTemplate: resourceCellTemplate
        }), 0), isStandaloneAllDayPanel && createComponentVNode(2, AllDayPanelLayout, {
            viewData: viewData,
            dataCellTemplate: dataCellTemplate,
            tableRef: allDayPanelRef,
            allDayAppointments: allDayAppointments
        })], 0)], 0), createComponentVNode(2, Scrollable, {
            useKeyboard: false,
            bounceEnabled: false,
            direction: scrollingDirection,
            className: "dx-scheduler-date-table-scrollable",
            children: createVNode(1, "div", "dx-scheduler-date-table-scrollable-content", [isRenderGroupPanel && createComponentVNode(2, GroupPanel, {
                groupPanelData: groupPanelData,
                className: groupPanelClassName,
                groupOrientation: groupOrientation,
                groupByDate: groupByDate,
                groups: groups,
                resourceCellTemplate: resourceCellTemplate,
                height: groupPanelHeight,
                elementRef: groupPanelRef
            }), !!TimePanel && TimePanel({
                timePanelData: timePanelData,
                timeCellTemplate: timeCellTemplate,
                groupOrientation: groupOrientation,
                tableRef: timePanelRef
            }), createVNode(1, "div", "dx-scheduler-date-table-container", [DateTable({
                tableRef: dateTableRef,
                viewData: viewData,
                groupOrientation: groupOrientation,
                dataCellTemplate: dataCellTemplate
            }), appointments], 0)], 0)
        }, null, dateTableScrollableRef)]
    })
};
import {
    createRef as infernoCreateRef
} from "inferno";
var getTemplate = TemplateProp => TemplateProp && (TemplateProp.defaultProps ? props => normalizeProps(createComponentVNode(2, TemplateProp, _extends({}, props))) : TemplateProp);
export class OrdinaryLayout extends BaseInfernoComponent {
    constructor(props) {
        super(props);
        this.state = {};
        this.dateTableScrollableRef = infernoCreateRef();
        this.getScrollableWidth = this.getScrollableWidth.bind(this)
    }
    get restAttributes() {
        var _this$props = this.props,
            restProps = _objectWithoutPropertiesLoose(_this$props, _excluded);
        return restProps
    }
    getScrollableWidth() {
        return this.dateTableScrollableRef.current.container().getBoundingClientRect().width
    }
    render() {
        var props = this.props;
        return viewFunction({
            props: _extends({}, props, {
                headerPanelTemplate: getTemplate(props.headerPanelTemplate),
                dateTableTemplate: getTemplate(props.dateTableTemplate),
                timePanelTemplate: getTemplate(props.timePanelTemplate),
                resourceCellTemplate: getTemplate(props.resourceCellTemplate),
                timeCellTemplate: getTemplate(props.timeCellTemplate),
                dateCellTemplate: getTemplate(props.dateCellTemplate),
                dataCellTemplate: getTemplate(props.dataCellTemplate)
            }),
            dateTableScrollableRef: this.dateTableScrollableRef,
            restAttributes: this.restAttributes
        })
    }
}
OrdinaryLayout.defaultProps = MainLayoutProps;
