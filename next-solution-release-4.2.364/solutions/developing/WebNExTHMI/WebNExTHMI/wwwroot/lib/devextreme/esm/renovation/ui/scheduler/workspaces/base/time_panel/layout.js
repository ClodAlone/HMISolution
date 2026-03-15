/**
 * DevExtreme (esm/renovation/ui/scheduler/workspaces/base/time_panel/layout.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import _objectWithoutPropertiesLoose from "@babel/runtime/helpers/esm/objectWithoutPropertiesLoose";
import _extends from "@babel/runtime/helpers/esm/extends";
var _excluded = ["groupOrientation", "tableRef", "timeCellTemplate", "timePanelData"];
import {
    createFragment,
    createComponentVNode,
    normalizeProps
} from "inferno";
import {
    Fragment
} from "inferno";
import {
    InfernoWrapperComponent
} from "@devextreme/runtime/inferno";
import {
    Row
} from "../row";
import {
    TimePanelCell as Cell
} from "./cell";
import {
    CellBase
} from "../cell";
import {
    getKeyByGroup,
    getIsGroupedAllDayPanel
} from "../../utils";
import {
    Table
} from "../table";
import {
    AllDayPanelTitle
} from "../date_table/all_day_panel/title";
export var viewFunction = _ref => {
    var {
        bottomVirtualRowHeight: bottomVirtualRowHeight,
        props: {
            groupOrientation: groupOrientation,
            tableRef: tableRef,
            timeCellTemplate: timeCellTemplate,
            timePanelData: timePanelData
        },
        restAttributes: restAttributes,
        topVirtualRowHeight: topVirtualRowHeight
    } = _ref;
    return normalizeProps(createComponentVNode(2, Table, _extends({}, restAttributes, {
        topVirtualRowHeight: topVirtualRowHeight,
        bottomVirtualRowHeight: bottomVirtualRowHeight,
        virtualCellsCount: 1,
        className: "dx-scheduler-time-panel",
        tableRef: tableRef,
        children: timePanelData.groupedData.map((_ref2, index) => {
            var {
                dateTable: dateTable,
                groupIndex: groupIndex
            } = _ref2;
            return createFragment([getIsGroupedAllDayPanel(timePanelData, index) && createComponentVNode(2, Row, {
                children: createComponentVNode(2, CellBase, {
                    className: "dx-scheduler-time-panel-title-cell",
                    children: createComponentVNode(2, AllDayPanelTitle)
                })
            }), dateTable.map(cell => {
                var {
                    groups: groups,
                    index: cellIndex,
                    isFirstGroupCell: isFirstGroupCell,
                    isLastGroupCell: isLastGroupCell,
                    key: key,
                    startDate: startDate,
                    text: text
                } = cell;
                return createComponentVNode(2, Row, {
                    className: "dx-scheduler-time-panel-row",
                    children: createComponentVNode(2, Cell, {
                        startDate: startDate,
                        text: text,
                        groups: groups,
                        groupIndex: groupIndex,
                        isFirstGroupCell: isFirstGroupCell,
                        isLastGroupCell: isLastGroupCell,
                        index: cellIndex,
                        timeCellTemplate: timeCellTemplate
                    })
                }, key)
            })], 0, getKeyByGroup(groupIndex, groupOrientation))
        })
    })))
};
export var TimePanelLayoutProps = {
    get timePanelData() {
        return {
            groupedData: [],
            leftVirtualCellCount: 0,
            rightVirtualCellCount: 0,
            topVirtualRowCount: 0,
            bottomVirtualRowCount: 0
        }
    }
};
import {
    createReRenderEffect
} from "@devextreme/runtime/inferno";
var getTemplate = TemplateProp => TemplateProp && (TemplateProp.defaultProps ? props => normalizeProps(createComponentVNode(2, TemplateProp, _extends({}, props))) : TemplateProp);
export class TimePanelTableLayout extends InfernoWrapperComponent {
    constructor(props) {
        super(props);
        this.state = {}
    }
    createEffects() {
        return [createReRenderEffect()]
    }
    get topVirtualRowHeight() {
        var _this$props$timePanel;
        return null !== (_this$props$timePanel = this.props.timePanelData.topVirtualRowHeight) && void 0 !== _this$props$timePanel ? _this$props$timePanel : 0
    }
    get bottomVirtualRowHeight() {
        var _this$props$timePanel2;
        return null !== (_this$props$timePanel2 = this.props.timePanelData.bottomVirtualRowHeight) && void 0 !== _this$props$timePanel2 ? _this$props$timePanel2 : 0
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
                timeCellTemplate: getTemplate(props.timeCellTemplate)
            }),
            topVirtualRowHeight: this.topVirtualRowHeight,
            bottomVirtualRowHeight: this.bottomVirtualRowHeight,
            restAttributes: this.restAttributes
        })
    }
}
TimePanelTableLayout.defaultProps = TimePanelLayoutProps;
