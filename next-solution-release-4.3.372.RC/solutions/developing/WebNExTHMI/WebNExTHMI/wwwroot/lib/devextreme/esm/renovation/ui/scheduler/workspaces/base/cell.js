/**
 * DevExtreme (esm/renovation/ui/scheduler/workspaces/base/cell.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import _objectWithoutPropertiesLoose from "@babel/runtime/helpers/esm/objectWithoutPropertiesLoose";
import _extends from "@babel/runtime/helpers/esm/extends";
var _excluded = ["allDay", "ariaLabel", "children", "className", "contentTemplate", "contentTemplateProps", "endDate", "groupIndex", "groups", "index", "isFirstGroupCell", "isLastGroupCell", "startDate", "text"];
import {
    createVNode,
    createComponentVNode,
    normalizeProps
} from "inferno";
import {
    BaseInfernoComponent
} from "@devextreme/runtime/inferno";
import {
    getGroupCellClasses
} from "../utils";
export var viewFunction = viewModel => {
    var ContentTemplate = viewModel.props.contentTemplate;
    return createVNode(1, "td", viewModel.classes, [!viewModel.props.contentTemplate && viewModel.props.children, viewModel.props.contentTemplate && ContentTemplate(_extends({}, viewModel.props.contentTemplateProps))], 0, {
        "aria-label": viewModel.props.ariaLabel
    })
};
export var CellBaseProps = {
    className: "",
    isFirstGroupCell: false,
    isLastGroupCell: false,
    get startDate() {
        return new Date
    },
    get endDate() {
        return new Date
    },
    allDay: false,
    text: "",
    index: 0,
    get contentTemplateProps() {
        return {
            data: {},
            index: 0
        }
    }
};
var getTemplate = TemplateProp => TemplateProp && (TemplateProp.defaultProps ? props => normalizeProps(createComponentVNode(2, TemplateProp, _extends({}, props))) : TemplateProp);
export class CellBase extends BaseInfernoComponent {
    constructor(props) {
        super(props);
        this.state = {}
    }
    get classes() {
        var {
            className: className,
            isFirstGroupCell: isFirstGroupCell,
            isLastGroupCell: isLastGroupCell
        } = this.props;
        return getGroupCellClasses(isFirstGroupCell, isLastGroupCell, className)
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
                contentTemplate: getTemplate(props.contentTemplate)
            }),
            classes: this.classes,
            restAttributes: this.restAttributes
        })
    }
}
CellBase.defaultProps = CellBaseProps;
