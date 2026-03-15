/**
 * DevExtreme (esm/renovation/ui/scheduler/workspaces/base/cross_scrolling_layout.js)
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
    BaseInfernoComponent,
    normalizeStyles
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
import {
    ScrollSemaphore
} from "../../utils/semaphore/scrollSemaphore";
export var viewFunction = _ref => {
    var {
        dateTableScrollableRef: dateTableScrollableRef,
        headerScrollableRef: headerScrollableRef,
        headerStyles: headerStyles,
        onDateTableScroll: onDateTableScroll,
        onHeaderScroll: onHeaderScroll,
        onSideBarScroll: onSideBarScroll,
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
            tablesWidth: tablesWidth,
            timeCellTemplate: timeCellTemplate,
            timePanelData: timePanelData,
            timePanelRef: timePanelRef,
            timePanelTemplate: TimePanel,
            viewData: viewData,
            widgetElementRef: widgetElementRef
        },
        sideBarScrollableRef: sideBarScrollableRef
    } = _ref;
    return createComponentVNode(2, Widget, {
        className: className,
        rootElementRef: widgetElementRef,
        children: [createVNode(1, "div", "dx-scheduler-fixed-appointments"), createVNode(1, "div", "dx-scheduler-header-panel-container", [isRenderHeaderEmptyCell && createComponentVNode(2, HeaderPanelEmptyCell, {
            width: headerEmptyCellWidth,
            isRenderAllDayTitle: isStandaloneAllDayPanel
        }), createVNode(1, "div", "dx-scheduler-header-tables-container", createComponentVNode(2, Scrollable, {
            className: "dx-scheduler-header-scrollable",
            useKeyboard: false,
            showScrollbar: "never",
            direction: "horizontal",
            useNative: false,
            bounceEnabled: false,
            onScroll: onHeaderScroll,
            children: [createVNode(1, "table", "dx-scheduler-header-panel", HeaderPanel({
                dateHeaderData: dateHeaderData,
                groupPanelData: groupPanelData,
                timeCellTemplate: timeCellTemplate,
                dateCellTemplate: dateCellTemplate,
                isRenderDateHeader: isRenderDateHeader,
                groupOrientation: groupOrientation,
                groupByDate: groupByDate,
                groups: groups,
                resourceCellTemplate: resourceCellTemplate
            }), 0, {
                style: normalizeStyles(headerStyles)
            }), isStandaloneAllDayPanel && createComponentVNode(2, AllDayPanelLayout, {
                viewData: viewData,
                dataCellTemplate: dataCellTemplate,
                tableRef: allDayPanelRef,
                allDayAppointments: allDayAppointments,
                width: tablesWidth
            })]
        }, null, headerScrollableRef), 2)], 0), createVNode(1, "div", "dx-scheduler-work-space-flex-container", [createComponentVNode(2, Scrollable, {
            className: "dx-scheduler-sidebar-scrollable",
            useKeyboard: false,
            showScrollbar: "never",
            direction: "vertical",
            useNative: false,
            bounceEnabled: false,
            onScroll: onSideBarScroll,
            children: createVNode(1, "div", "dx-scheduler-side-bar-scrollable-content", [isRenderGroupPanel && createComponentVNode(2, GroupPanel, {
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
            })], 0)
        }, null, sideBarScrollableRef), createComponentVNode(2, Scrollable, {
            useKeyboard: false,
            bounceEnabled: false,
            direction: "both",
            className: "dx-scheduler-date-table-scrollable",
            onScroll: onDateTableScroll,
            children: createVNode(1, "div", "dx-scheduler-date-table-scrollable-content", createVNode(1, "div", "dx-scheduler-date-table-container", [DateTable({
                tableRef: dateTableRef,
                viewData: viewData,
                groupOrientation: groupOrientation,
                dataCellTemplate: dataCellTemplate,
                width: tablesWidth
            }), appointments], 0), 2)
        }, null, dateTableScrollableRef)], 4)]
    })
};
import {
    createRef as infernoCreateRef
} from "inferno";
var getTemplate = TemplateProp => TemplateProp && (TemplateProp.defaultProps ? props => normalizeProps(createComponentVNode(2, TemplateProp, _extends({}, props))) : TemplateProp);
export class CrossScrollingLayout extends BaseInfernoComponent {
    constructor(props) {
        super(props);
        this.state = {};
        this.dateTableScrollableRef = infernoCreateRef();
        this.headerScrollableRef = infernoCreateRef();
        this.sideBarScrollableRef = infernoCreateRef();
        this.__getterCache = {};
        this.getScrollableWidth = this.getScrollableWidth.bind(this);
        this.onDateTableScroll = this.onDateTableScroll.bind(this);
        this.onHeaderScroll = this.onHeaderScroll.bind(this);
        this.onSideBarScroll = this.onSideBarScroll.bind(this)
    }
    get dateTableSemaphore() {
        if (void 0 !== this.__getterCache.dateTableSemaphore) {
            return this.__getterCache.dateTableSemaphore
        }
        return this.__getterCache.dateTableSemaphore = new ScrollSemaphore
    }
    get headerSemaphore() {
        if (void 0 !== this.__getterCache.headerSemaphore) {
            return this.__getterCache.headerSemaphore
        }
        return this.__getterCache.headerSemaphore = new ScrollSemaphore
    }
    get sideBarSemaphore() {
        if (void 0 !== this.__getterCache.sideBarSemaphore) {
            return this.__getterCache.sideBarSemaphore
        }
        return this.__getterCache.sideBarSemaphore = new ScrollSemaphore
    }
    get headerStyles() {
        return {
            width: this.props.tablesWidth
        }
    }
    onDateTableScroll(e) {
        this.dateTableSemaphore.take(e.scrollOffset);
        this.sideBarSemaphore.isFree(e.scrollOffset) && this.sideBarScrollableRef.current.scrollTo({
            top: e.scrollOffset.top
        });
        this.headerSemaphore.isFree(e.scrollOffset) && this.headerScrollableRef.current.scrollTo({
            left: e.scrollOffset.left
        });
        this.props.onScroll(e);
        this.dateTableSemaphore.release()
    }
    onHeaderScroll(e) {
        this.headerSemaphore.take(e.scrollOffset);
        this.dateTableSemaphore.isFree(e.scrollOffset) && this.dateTableScrollableRef.current.scrollTo({
            left: e.scrollOffset.left
        });
        this.headerSemaphore.release()
    }
    onSideBarScroll(e) {
        this.sideBarSemaphore.take(e.scrollOffset);
        this.dateTableSemaphore.isFree(e.scrollOffset) && this.dateTableScrollableRef.current.scrollTo({
            top: e.scrollOffset.top
        });
        this.sideBarSemaphore.release()
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
            headerScrollableRef: this.headerScrollableRef,
            sideBarScrollableRef: this.sideBarScrollableRef,
            dateTableSemaphore: this.dateTableSemaphore,
            headerSemaphore: this.headerSemaphore,
            sideBarSemaphore: this.sideBarSemaphore,
            headerStyles: this.headerStyles,
            onDateTableScroll: this.onDateTableScroll,
            onHeaderScroll: this.onHeaderScroll,
            onSideBarScroll: this.onSideBarScroll,
            restAttributes: this.restAttributes
        })
    }
}
CrossScrollingLayout.defaultProps = MainLayoutProps;
