/**
 * DevExtreme (esm/renovation/ui/scroll_view/internal/pocket/bottom.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import _extends from "@babel/runtime/helpers/esm/extends";
import _objectWithoutPropertiesLoose from "@babel/runtime/helpers/esm/objectWithoutPropertiesLoose";
var _excluded = ["bottomPocketRef", "reachBottomText", "visible"];
import {
    createVNode,
    createComponentVNode
} from "inferno";
import {
    BaseInfernoComponent
} from "@devextreme/runtime/inferno";
import {
    LoadIndicator
} from "../../../load_indicator";
import {
    SCROLLVIEW_BOTTOM_POCKET_CLASS,
    SCROLLVIEW_REACHBOTTOM_CLASS,
    SCROLLVIEW_REACHBOTTOM_INDICATOR_CLASS,
    SCROLLVIEW_REACHBOTTOM_TEXT_CLASS
} from "../../common/consts";
import {
    current,
    isMaterial
} from "../../../../../ui/themes";
import {
    combineClasses
} from "../../../../utils/combine_classes";
import messageLocalization from "../../../../../localization/message";
export var viewFunction = viewModel => {
    var {
        props: {
            bottomPocketRef: bottomPocketRef,
            reachBottomText: reachBottomText
        },
        reachBottomClasses: reachBottomClasses
    } = viewModel;
    return createVNode(1, "div", SCROLLVIEW_BOTTOM_POCKET_CLASS, createVNode(1, "div", reachBottomClasses, [createVNode(1, "div", SCROLLVIEW_REACHBOTTOM_INDICATOR_CLASS, createComponentVNode(2, LoadIndicator), 2), createVNode(1, "div", SCROLLVIEW_REACHBOTTOM_TEXT_CLASS, createVNode(1, "div", null, reachBottomText, 0), 2)], 4), 2, null, null, bottomPocketRef)
};
export var BottomPocketProps = {
    get reachBottomText() {
        return isMaterial(current()) ? "" : messageLocalization.format("dxScrollView-reachBottomText")
    },
    visible: true
};
export class BottomPocket extends BaseInfernoComponent {
    constructor(props) {
        super(props);
        this.state = {}
    }
    get reachBottomClasses() {
        var {
            visible: visible
        } = this.props;
        var classesMap = {
            [SCROLLVIEW_REACHBOTTOM_CLASS]: true,
            "dx-state-invisible": !visible
        };
        return combineClasses(classesMap)
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
            reachBottomClasses: this.reachBottomClasses,
            restAttributes: this.restAttributes
        })
    }
}
BottomPocket.defaultProps = BottomPocketProps;
