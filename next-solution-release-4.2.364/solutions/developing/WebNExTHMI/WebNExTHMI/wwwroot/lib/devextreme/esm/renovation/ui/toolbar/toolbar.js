/**
 * DevExtreme (esm/renovation/ui/toolbar/toolbar.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import _objectWithoutPropertiesLoose from "@babel/runtime/helpers/esm/objectWithoutPropertiesLoose";
import _extends from "@babel/runtime/helpers/esm/extends";
var _excluded = ["accessKey", "activeStateEnabled", "className", "disabled", "focusStateEnabled", "height", "hint", "hoverStateEnabled", "items", "onClick", "onKeyDown", "rtlEnabled", "tabIndex", "visible", "width"];
import {
    createComponentVNode,
    normalizeProps
} from "inferno";
import {
    BaseInfernoComponent
} from "@devextreme/runtime/inferno";
import LegacyToolbar from "../../../ui/toolbar";
import {
    DomComponentWrapper
} from "../common/dom_component_wrapper";
import {
    ToolbarProps
} from "./toolbar_props";
import {
    isObject
} from "../../../core/utils/type";
import {
    ConfigContext
} from "../../common/config_context";
import {
    resolveRtlEnabled
} from "../../utils/resolve_rtl";
export var viewFunction = _ref => {
    var {
        componentProps: componentProps,
        restAttributes: restAttributes
    } = _ref;
    return normalizeProps(createComponentVNode(2, DomComponentWrapper, _extends({
        componentType: LegacyToolbar,
        componentProps: componentProps,
        templateNames: []
    }, restAttributes)))
};
export class Toolbar extends BaseInfernoComponent {
    constructor(props) {
        super(props);
        this.state = {};
        this.__getterCache = {}
    }
    get config() {
        if ("ConfigContext" in this.context) {
            return this.context.ConfigContext
        }
        return ConfigContext
    }
    get componentProps() {
        if (void 0 !== this.__getterCache.componentProps) {
            return this.__getterCache.componentProps
        }
        return this.__getterCache.componentProps = (() => {
            var {
                items: items
            } = this.props;
            var toolbarItems = null === items || void 0 === items ? void 0 : items.map(item => {
                var _item$options, _options$rtlEnabled;
                if (!isObject(item)) {
                    return item
                }
                var options = null !== (_item$options = item.options) && void 0 !== _item$options ? _item$options : {};
                options.rtlEnabled = null !== (_options$rtlEnabled = options.rtlEnabled) && void 0 !== _options$rtlEnabled ? _options$rtlEnabled : this.resolvedRtlEnabled;
                return _extends({}, item, {
                    options: options
                })
            });
            return _extends({}, this.props, {
                items: toolbarItems
            })
        })()
    }
    get resolvedRtlEnabled() {
        var {
            rtlEnabled: rtlEnabled
        } = this.props;
        return !!resolveRtlEnabled(rtlEnabled, this.config)
    }
    get restAttributes() {
        var _this$props = this.props,
            restProps = _objectWithoutPropertiesLoose(_this$props, _excluded);
        return restProps
    }
    componentWillUpdate(nextProps, nextState, context) {
        if (this.props.items !== nextProps.items || this.props.rtlEnabled !== nextProps.rtlEnabled || this.context.ConfigContext !== context.ConfigContext || this.props !== nextProps) {
            this.__getterCache.componentProps = void 0
        }
    }
    render() {
        var props = this.props;
        return viewFunction({
            props: _extends({}, props),
            config: this.config,
            componentProps: this.componentProps,
            resolvedRtlEnabled: this.resolvedRtlEnabled,
            restAttributes: this.restAttributes
        })
    }
}
Toolbar.defaultProps = ToolbarProps;
