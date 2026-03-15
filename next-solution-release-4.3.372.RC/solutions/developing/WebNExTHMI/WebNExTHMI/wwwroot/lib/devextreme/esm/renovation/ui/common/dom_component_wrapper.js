/**
 * DevExtreme (esm/renovation/ui/common/dom_component_wrapper.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import _objectWithoutPropertiesLoose from "@babel/runtime/helpers/esm/objectWithoutPropertiesLoose";
import _extends from "@babel/runtime/helpers/esm/extends";
var _excluded = ["valueChange"],
    _excluded2 = ["componentProps", "componentType", "rootElementRef", "templateNames"];
import {
    createVNode,
    normalizeProps
} from "inferno";
import {
    InfernoEffect,
    InfernoComponent
} from "@devextreme/runtime/inferno";
import {
    renderTemplate,
    hasTemplate
} from "@devextreme/runtime/inferno";
import {
    ConfigContext
} from "../../common/config_context";
import {
    getUpdatedOptions
} from "./utils/get_updated_options";
export var viewFunction = _ref => {
    var {
        props: {
            componentProps: {
                className: className
            }
        },
        restAttributes: restAttributes,
        widgetRef: widgetRef
    } = _ref;
    return normalizeProps(createVNode(1, "div", className, null, 1, _extends({}, restAttributes), null, widgetRef))
};
export var DomComponentWrapperProps = {};
import {
    createRef as infernoCreateRef
} from "inferno";
export class DomComponentWrapper extends InfernoComponent {
    constructor(props) {
        super(props);
        this.state = {};
        this.widgetRef = infernoCreateRef();
        this.getInstance = this.getInstance.bind(this);
        this.setupWidget = this.setupWidget.bind(this);
        this.setRootElementRef = this.setRootElementRef.bind(this);
        this.updateWidget = this.updateWidget.bind(this)
    }
    get config() {
        if ("ConfigContext" in this.context) {
            return this.context.ConfigContext
        }
        return ConfigContext
    }
    createEffects() {
        return [new InfernoEffect(this.setupWidget, []), new InfernoEffect(this.setRootElementRef, []), new InfernoEffect(this.updateWidget, [this.props.componentProps, this.config, this.props.templateNames])]
    }
    updateEffects() {
        var _this$_effects$;
        null === (_this$_effects$ = this._effects[2]) || void 0 === _this$_effects$ ? void 0 : _this$_effects$.update([this.props.componentProps, this.config, this.props.templateNames])
    }
    setupWidget() {
        var componentInstance = new this.props.componentType(this.widgetRef.current, this.properties);
        this.instance = componentInstance;
        return () => {
            componentInstance.dispose();
            this.instance = null
        }
    }
    setRootElementRef() {
        var {
            rootElementRef: rootElementRef
        } = this.props;
        if (rootElementRef) {
            rootElementRef.current = this.widgetRef.current
        }
    }
    updateWidget() {
        var instance = this.getInstance();
        if (!instance) {
            return
        }
        var updatedOptions = getUpdatedOptions(this.prevProps || {}, this.properties);
        if (updatedOptions.length) {
            instance.beginUpdate();
            updatedOptions.forEach(_ref2 => {
                var {
                    path: path,
                    value: value
                } = _ref2;
                instance.option(path, value)
            });
            instance.endUpdate()
        }
        this.prevProps = this.properties
    }
    get properties() {
        var _this$config;
        var _this$props$component = this.props.componentProps,
            {
                valueChange: valueChange
            } = _this$props$component,
            restProps = _objectWithoutPropertiesLoose(_this$props$component, _excluded);
        var properties = _extends({
            rtlEnabled: !!(null !== (_this$config = this.config) && void 0 !== _this$config && _this$config.rtlEnabled)
        }, restProps);
        if (valueChange) {
            properties.onValueChanged = _ref3 => {
                var {
                    value: value
                } = _ref3;
                return valueChange(value)
            }
        }
        var templates = this.props.templateNames;
        templates.forEach(name => {
            if (hasTemplate(name, properties, this)) {
                properties[name] = (item, index, container) => {
                    renderTemplate(this.props.componentProps[name], {
                        item: item,
                        index: index,
                        container: container
                    }, this)
                }
            }
        });
        return properties
    }
    get restAttributes() {
        var _this$props = this.props,
            restProps = _objectWithoutPropertiesLoose(_this$props, _excluded2);
        return restProps
    }
    getInstance() {
        return this.instance
    }
    render() {
        var props = this.props;
        return viewFunction({
            props: _extends({}, props),
            widgetRef: this.widgetRef,
            config: this.config,
            properties: this.properties,
            restAttributes: this.restAttributes
        })
    }
}
DomComponentWrapper.defaultProps = DomComponentWrapperProps;
