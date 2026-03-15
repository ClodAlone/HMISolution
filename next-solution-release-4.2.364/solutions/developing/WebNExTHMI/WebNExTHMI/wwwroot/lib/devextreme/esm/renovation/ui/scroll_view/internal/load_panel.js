/**
 * DevExtreme (esm/renovation/ui/scroll_view/internal/load_panel.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import _extends from "@babel/runtime/helpers/esm/extends";
import _objectWithoutPropertiesLoose from "@babel/runtime/helpers/esm/objectWithoutPropertiesLoose";
var _excluded = ["refreshingText", "targetElement", "visible"];
import {
    createComponentVNode
} from "inferno";
import {
    BaseInfernoComponent
} from "@devextreme/runtime/inferno";
import {
    isDefined
} from "../../../../core/utils/type";
import messageLocalization from "../../../../localization/message";
import {
    LoadPanel,
    LoadPanelProps
} from "../../overlays/load_panel";
var SCROLLVIEW_LOADPANEL = "dx-scrollview-loadpanel";
export var viewFunction = viewModel => {
    var {
        position: position,
        props: {
            visible: visible
        },
        refreshingText: refreshingText
    } = viewModel;
    return createComponentVNode(2, LoadPanel, {
        className: SCROLLVIEW_LOADPANEL,
        shading: false,
        delay: 400,
        message: refreshingText,
        position: position,
        visible: visible
    })
};
export var ScrollViewLoadPanelProps = {};
export var ScrollViewLoadPanelPropsType = {
    get visible() {
        return LoadPanelProps.visible
    }
};
export class ScrollViewLoadPanel extends BaseInfernoComponent {
    constructor(props) {
        super(props);
        this.state = {};
        this.__getterCache = {}
    }
    get refreshingText() {
        var {
            refreshingText: refreshingText
        } = this.props;
        if (isDefined(refreshingText)) {
            return refreshingText
        }
        return messageLocalization.format("dxScrollView-refreshingText")
    }
    get position() {
        if (void 0 !== this.__getterCache.position) {
            return this.__getterCache.position
        }
        return this.__getterCache.position = (() => {
            if (this.props.targetElement) {
                return {
                    of: this.props.targetElement.current
                }
            }
            return
        })()
    }
    get restAttributes() {
        var _this$props = this.props,
            restProps = _objectWithoutPropertiesLoose(_this$props, _excluded);
        return restProps
    }
    componentWillUpdate(nextProps, nextState, context) {
        if (this.props["targetElement?.current"] !== nextProps["targetElement?.current"]) {
            this.__getterCache.position = void 0
        }
    }
    render() {
        var props = this.props;
        return viewFunction({
            props: _extends({}, props),
            refreshingText: this.refreshingText,
            position: this.position,
            restAttributes: this.restAttributes
        })
    }
}
ScrollViewLoadPanel.defaultProps = ScrollViewLoadPanelPropsType;
