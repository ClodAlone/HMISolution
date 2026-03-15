/**
 * DevExtreme (esm/renovation/ui/scheduler/workspaces/base/group_panel/horizontal/layout.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import _objectWithoutPropertiesLoose from "@babel/runtime/helpers/esm/objectWithoutPropertiesLoose";
import _extends from "@babel/runtime/helpers/esm/extends";
var _excluded = ["className", "elementRef", "groupByDate", "groupPanelData", "height", "resourceCellTemplate", "styles"];
import {
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
    Row
} from "./row";
import {
    GroupPanelLayoutProps
} from "../group_panel_layout_props";
export var viewFunction = _ref => {
    var {
        groupPanelItems: groupPanelItems,
        props: {
            resourceCellTemplate: resourceCellTemplate
        }
    } = _ref;
    return createFragment(groupPanelItems.map(group => createComponentVNode(2, Row, {
        groupItems: group,
        cellTemplate: resourceCellTemplate
    }, group[0].key)), 0)
};
export var HorizontalGroupPanelLayoutProps = GroupPanelLayoutProps;
var getTemplate = TemplateProp => TemplateProp && (TemplateProp.defaultProps ? props => normalizeProps(createComponentVNode(2, TemplateProp, _extends({}, props))) : TemplateProp);
export class GroupPanelHorizontalLayout extends BaseInfernoComponent {
    constructor(props) {
        super(props);
        this.state = {};
        this.__getterCache = {}
    }
    get groupPanelItems() {
        if (void 0 !== this.__getterCache.groupPanelItems) {
            return this.__getterCache.groupPanelItems
        }
        return this.__getterCache.groupPanelItems = (() => {
            var {
                groupPanelData: groupPanelData
            } = this.props;
            var {
                baseColSpan: baseColSpan,
                groupPanelItems: groupPanelItems
            } = groupPanelData;
            var colSpans = groupPanelItems.reduceRight((currentColSpans, groupsRow, index) => {
                var nextColSpans = currentColSpans;
                var currentLevelGroupCount = groupsRow.length;
                var previousColSpan = index === groupPanelItems.length - 1 ? baseColSpan : currentColSpans[index + 1];
                var previousLevelGroupCount = index === groupPanelItems.length - 1 ? currentLevelGroupCount : groupPanelItems[index + 1].length;
                var groupCountDiff = previousLevelGroupCount / currentLevelGroupCount;
                nextColSpans[index] = groupCountDiff * previousColSpan;
                return nextColSpans
            }, [...new Array(groupPanelItems.length)]);
            return groupPanelItems.map((groupsRenderRow, index) => {
                var colSpan = colSpans[index];
                return groupsRenderRow.map(groupItem => _extends({}, groupItem, {
                    colSpan: colSpan
                }))
            })
        })()
    }
    get restAttributes() {
        var _this$props = this.props,
            restProps = _objectWithoutPropertiesLoose(_this$props, _excluded);
        return restProps
    }
    componentWillUpdate(nextProps, nextState, context) {
        if (this.props.groupPanelData !== nextProps.groupPanelData) {
            this.__getterCache.groupPanelItems = void 0
        }
    }
    render() {
        var props = this.props;
        return viewFunction({
            props: _extends({}, props, {
                resourceCellTemplate: getTemplate(props.resourceCellTemplate)
            }),
            groupPanelItems: this.groupPanelItems,
            restAttributes: this.restAttributes
        })
    }
}
GroupPanelHorizontalLayout.defaultProps = HorizontalGroupPanelLayoutProps;
