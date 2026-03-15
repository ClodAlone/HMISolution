/**
 * DevExtreme (esm/renovation/ui/scheduler/workspaces/base/row.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import _extends from "@babel/runtime/helpers/esm/extends";
import _objectWithoutPropertiesLoose from "@babel/runtime/helpers/esm/objectWithoutPropertiesLoose";
var _excluded = ["children", "className", "isHeaderRow", "leftVirtualCellCount", "leftVirtualCellWidth", "rightVirtualCellCount", "rightVirtualCellWidth", "styles"];
import {
    createVNode,
    createComponentVNode
} from "inferno";
import {
    BaseInfernoComponent,
    normalizeStyles
} from "@devextreme/runtime/inferno";
import {
    VirtualCell
} from "./virtual_cell";
export var viewFunction = _ref => {
    var {
        hasLeftVirtualCell: hasLeftVirtualCell,
        hasRightVirtualCell: hasRightVirtualCell,
        props: {
            children: children,
            className: className,
            isHeaderRow: isHeaderRow,
            leftVirtualCellCount: leftVirtualCellCount,
            leftVirtualCellWidth: leftVirtualCellWidth,
            rightVirtualCellCount: rightVirtualCellCount,
            rightVirtualCellWidth: rightVirtualCellWidth,
            styles: styles
        }
    } = _ref;
    return createVNode(1, "tr", className, [hasLeftVirtualCell && createComponentVNode(2, VirtualCell, {
        width: leftVirtualCellWidth,
        colSpan: leftVirtualCellCount,
        isHeaderCell: isHeaderRow
    }), children, hasRightVirtualCell && createComponentVNode(2, VirtualCell, {
        width: rightVirtualCellWidth,
        colSpan: rightVirtualCellCount,
        isHeaderCell: isHeaderRow
    })], 0, {
        style: normalizeStyles(styles)
    })
};
export var RowProps = {
    className: "",
    leftVirtualCellWidth: 0,
    rightVirtualCellWidth: 0,
    isHeaderRow: false
};
export class Row extends BaseInfernoComponent {
    constructor(props) {
        super(props);
        this.state = {}
    }
    get hasLeftVirtualCell() {
        var {
            leftVirtualCellCount: leftVirtualCellCount
        } = this.props;
        return !!leftVirtualCellCount
    }
    get hasRightVirtualCell() {
        var {
            rightVirtualCellCount: rightVirtualCellCount
        } = this.props;
        return !!rightVirtualCellCount
    }
    get restAttributes() {
        var _this$props = this.props,
            restProps = _objectWithoutPropertiesLoose(_this$props, _excluded);
        return restProps
    }
    render() {
        var props = this.props;
        return viewFunction({
            props: _extends({}, props),
            hasLeftVirtualCell: this.hasLeftVirtualCell,
            hasRightVirtualCell: this.hasRightVirtualCell,
            restAttributes: this.restAttributes
        })
    }
}
Row.defaultProps = RowProps;
