/**
 * DevExtreme (esm/renovation/ui/scheduler/appointment_tooltip/item_layout.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import _objectWithoutPropertiesLoose from "@babel/runtime/helpers/esm/objectWithoutPropertiesLoose";
import _extends from "@babel/runtime/helpers/esm/extends";
var _excluded = ["className", "getTextAndFormatDate", "index", "item", "itemContentTemplate", "onDelete", "onHide", "showDeleteButton", "singleAppointment"];
import {
    createVNode,
    createComponentVNode,
    normalizeProps
} from "inferno";
import {
    BaseInfernoComponent
} from "@devextreme/runtime/inferno";
import {
    Marker
} from "./marker";
import {
    Button
} from "../../button";
import {
    TooltipItemContent
} from "./item_content";
import getCurrentAppointment from "./utils/get_current_appointment";
import {
    defaultGetTextAndFormatDate
} from "./utils/default_functions";
export var viewFunction = viewModel => {
    var ItemContentTemplate = viewModel.props.itemContentTemplate;
    return viewModel.props.itemContentTemplate ? ItemContentTemplate({
        model: {
            appointmentData: viewModel.props.item.data,
            targetedAppointmentData: viewModel.currentAppointment
        },
        index: viewModel.props.index
    }) : normalizeProps(createVNode(1, "div", "dx-tooltip-appointment-item ".concat(viewModel.props.className), [createComponentVNode(2, Marker), createComponentVNode(2, TooltipItemContent, {
        text: viewModel.formattedContent.text,
        formattedDate: viewModel.formattedContent.formatDate
    }), viewModel.props.showDeleteButton && createVNode(1, "div", "dx-tooltip-appointment-item-delete-button-container", createComponentVNode(2, Button, {
        className: "dx-tooltip-appointment-item-delete-button",
        icon: "trash",
        stylingMode: "text",
        onClick: viewModel.onDeleteButtonClick
    }), 2)], 0, _extends({}, viewModel.restAttributes)))
};
export var TooltipItemLayoutProps = {
    className: "",
    get item() {
        return {
            data: {}
        }
    },
    index: 0,
    showDeleteButton: true,
    getTextAndFormatDate: defaultGetTextAndFormatDate,
    get singleAppointment() {
        return {}
    }
};
var getTemplate = TemplateProp => TemplateProp && (TemplateProp.defaultProps ? props => normalizeProps(createComponentVNode(2, TemplateProp, _extends({}, props))) : TemplateProp);
export class TooltipItemLayout extends BaseInfernoComponent {
    constructor(props) {
        super(props);
        this.state = {};
        this.__getterCache = {}
    }
    get currentAppointment() {
        var {
            item: item
        } = this.props;
        return getCurrentAppointment(item)
    }
    get onDeleteButtonClick() {
        if (void 0 !== this.__getterCache.onDeleteButtonClick) {
            return this.__getterCache.onDeleteButtonClick
        }
        return this.__getterCache.onDeleteButtonClick = (() => {
            var {
                item: item,
                onDelete: onDelete,
                onHide: onHide,
                singleAppointment: singleAppointment
            } = this.props;
            return e => {
                null === onHide || void 0 === onHide ? void 0 : onHide();
                e.event.stopPropagation();
                null === onDelete || void 0 === onDelete ? void 0 : onDelete(item.data, singleAppointment)
            }
        })()
    }
    get formattedContent() {
        if (void 0 !== this.__getterCache.formattedContent) {
            return this.__getterCache.formattedContent
        }
        return this.__getterCache.formattedContent = (() => {
            var {
                getTextAndFormatDate: getTextAndFormatDate,
                item: item
            } = this.props;
            var {
                data: data
            } = item;
            return getTextAndFormatDate(data, this.currentAppointment)
        })()
    }
    get restAttributes() {
        var _this$props = this.props,
            restProps = _objectWithoutPropertiesLoose(_this$props, _excluded);
        return restProps
    }
    componentWillUpdate(nextProps, nextState, context) {
        if (this.props.item !== nextProps.item || this.props.onDelete !== nextProps.onDelete || this.props.onHide !== nextProps.onHide || this.props.singleAppointment !== nextProps.singleAppointment) {
            this.__getterCache.onDeleteButtonClick = void 0
        }
        if (this.props.getTextAndFormatDate !== nextProps.getTextAndFormatDate || this.props.item !== nextProps.item) {
            this.__getterCache.formattedContent = void 0
        }
    }
    render() {
        var props = this.props;
        return viewFunction({
            props: _extends({}, props, {
                itemContentTemplate: getTemplate(props.itemContentTemplate)
            }),
            currentAppointment: this.currentAppointment,
            onDeleteButtonClick: this.onDeleteButtonClick,
            formattedContent: this.formattedContent,
            restAttributes: this.restAttributes
        })
    }
}
TooltipItemLayout.defaultProps = TooltipItemLayoutProps;
