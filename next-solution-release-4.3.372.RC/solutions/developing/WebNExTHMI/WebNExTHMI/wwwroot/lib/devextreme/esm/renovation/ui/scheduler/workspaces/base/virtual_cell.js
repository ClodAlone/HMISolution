/**
 * DevExtreme (esm/renovation/ui/scheduler/workspaces/base/virtual_cell.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import _extends from "@babel/runtime/helpers/esm/extends";
import _objectWithoutPropertiesLoose from "@babel/runtime/helpers/esm/objectWithoutPropertiesLoose";
var _excluded = ["colSpan", "isHeaderCell", "width"];
import {
    createComponentVNode
} from "inferno";
import {
    BaseInfernoComponent
} from "@devextreme/runtime/inferno";
import {
    addWidthToStyle
} from "../utils";
import {
    HeaderCell
} from "./header_cell";
import {
    OrdinaryCell
} from "./ordinary_cell";
export var viewFunction = _ref => {
    var {
        cellComponent: Cell,
        props: {
            colSpan: colSpan
        },
        style: style
    } = _ref;
    return createComponentVNode(2, Cell, {
        className: "dx-scheduler-virtual-cell",
        styles: style,
        colSpan: colSpan
    })
};
export var VirtualCellProps = {
    width: 0,
    isHeaderCell: false
};
export class VirtualCell extends BaseInfernoComponent {
    constructor(props) {
        super(props);
        this.state = {}
    }
    get style() {
        var {
            width: width
        } = this.props;
        var {
            style: style
        } = this.restAttributes;
        return addWidthToStyle(width, style)
    }
    get cellComponent() {
        return this.props.isHeaderCell ? HeaderCell : OrdinaryCell
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
            style: this.style,
            cellComponent: this.cellComponent,
            restAttributes: this.restAttributes
        })
    }
}
VirtualCell.defaultProps = VirtualCellProps;
