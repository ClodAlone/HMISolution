/**
 * DevExtreme (esm/renovation/ui/scheduler/workspaces/base/virtual_row.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import _objectWithoutPropertiesLoose from "@babel/runtime/helpers/esm/objectWithoutPropertiesLoose";
import _extends from "@babel/runtime/helpers/esm/extends";
var _excluded = ["cellsCount", "children", "className", "height", "isHeaderRow", "leftVirtualCellCount", "leftVirtualCellWidth", "rightVirtualCellCount", "rightVirtualCellWidth", "styles"];
import {
    createComponentVNode
} from "inferno";
import {
    BaseInfernoComponent
} from "@devextreme/runtime/inferno";
import {
    addHeightToStyle
} from "../utils";
import {
    RowProps,
    Row
} from "./row";
import {
    VirtualCell
} from "./virtual_cell";
export var viewFunction = _ref => {
    var {
        classes: classes,
        props: {
            leftVirtualCellCount: leftVirtualCellCount,
            leftVirtualCellWidth: leftVirtualCellWidth,
            rightVirtualCellCount: rightVirtualCellCount,
            rightVirtualCellWidth: rightVirtualCellWidth
        },
        style: style,
        virtualCells: virtualCells
    } = _ref;
    return createComponentVNode(2, Row, {
        styles: style,
        className: classes,
        leftVirtualCellWidth: leftVirtualCellWidth,
        rightVirtualCellWidth: rightVirtualCellWidth,
        leftVirtualCellCount: leftVirtualCellCount,
        rightVirtualCellCount: rightVirtualCellCount,
        children: virtualCells.map((_, index) => createComponentVNode(2, VirtualCell, null, index.toString()))
    })
};
export var VirtualRowProps = Object.create(Object.prototype, _extends(Object.getOwnPropertyDescriptors(RowProps), Object.getOwnPropertyDescriptors({
    leftVirtualCellWidth: 0,
    rightVirtualCellWidth: 0,
    cellsCount: 1
})));
export class VirtualRow extends BaseInfernoComponent {
    constructor(props) {
        super(props);
        this.state = {};
        this.__getterCache = {}
    }
    get style() {
        var {
            height: height
        } = this.props;
        var {
            style: style
        } = this.restAttributes;
        return addHeightToStyle(height, style)
    }
    get classes() {
        var {
            className: className
        } = this.props;
        return "dx-scheduler-virtual-row ".concat(className)
    }
    get virtualCells() {
        if (void 0 !== this.__getterCache.virtualCells) {
            return this.__getterCache.virtualCells
        }
        return this.__getterCache.virtualCells = (() => {
            var {
                cellsCount: cellsCount
            } = this.props;
            return [...Array(cellsCount)]
        })()
    }
    get restAttributes() {
        var _this$props = this.props,
            restProps = _objectWithoutPropertiesLoose(_this$props, _excluded);
        return restProps
    }
    componentWillUpdate(nextProps, nextState, context) {
        if (this.props.cellsCount !== nextProps.cellsCount) {
            this.__getterCache.virtualCells = void 0
        }
    }
    render() {
        var props = this.props;
        return viewFunction({
            props: _extends({}, props),
            style: this.style,
            classes: this.classes,
            virtualCells: this.virtualCells,
            restAttributes: this.restAttributes
        })
    }
}
VirtualRow.defaultProps = VirtualRowProps;
