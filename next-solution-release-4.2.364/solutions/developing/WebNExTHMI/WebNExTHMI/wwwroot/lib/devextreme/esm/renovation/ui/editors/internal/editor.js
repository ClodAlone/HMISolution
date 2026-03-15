/**
 * DevExtreme (esm/renovation/ui/editors/internal/editor.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import _objectWithoutPropertiesLoose from "@babel/runtime/helpers/esm/objectWithoutPropertiesLoose";
import _extends from "@babel/runtime/helpers/esm/extends";
var _excluded = ["accessKey", "activeStateEnabled", "aria", "children", "className", "classes", "defaultValue", "disabled", "focusStateEnabled", "height", "hint", "hoverStateEnabled", "isValid", "name", "onClick", "onFocusIn", "onKeyDown", "readOnly", "rtlEnabled", "tabIndex", "validationError", "validationErrors", "validationMessageMode", "validationStatus", "value", "valueChange", "visible", "width"];
import {
    createFragment,
    createComponentVNode,
    normalizeProps
} from "inferno";
import {
    Fragment
} from "inferno";
import {
    InfernoEffect,
    InfernoWrapperComponent
} from "@devextreme/runtime/inferno";
import Guid from "../../../../core/guid";
import {
    Widget,
    WidgetProps
} from "../../common/widget";
import {
    BaseWidgetProps
} from "../../common/base_props";
import {
    combineClasses
} from "../../../utils/combine_classes";
import {
    ValidationMessage
} from "../../overlays/validation_message";
var getCssClasses = model => {
    var {
        classes: classes,
        isValid: isValid,
        readOnly: readOnly
    } = model;
    var classesMap = {
        "dx-state-readonly": !!readOnly,
        "dx-invalid": !isValid,
        ["".concat(classes)]: !!classes
    };
    return combineClasses(classesMap)
};
export var viewFunction = viewModel => {
    var {
        aria: aria,
        cssClasses: classes,
        isValidationMessageVisible: isValidationMessageVisible,
        onFocusIn: onFocusIn,
        props: {
            accessKey: accessKey,
            activeStateEnabled: activeStateEnabled,
            children: children,
            className: className,
            disabled: disabled,
            focusStateEnabled: focusStateEnabled,
            height: height,
            hint: hint,
            hoverStateEnabled: hoverStateEnabled,
            onClick: onClick,
            onKeyDown: onKeyDown,
            rtlEnabled: rtlEnabled,
            tabIndex: tabIndex,
            validationMessageMode: validationMessageMode,
            visible: visible,
            width: width
        },
        restAttributes: restAttributes,
        rootElementRef: rootElementRef,
        validationErrors: validationErrors,
        validationMessageGuid: validationMessageGuid,
        validationMessageTarget: validationMessageTarget,
        widgetRef: widgetRef
    } = viewModel;
    return normalizeProps(createComponentVNode(2, Widget, _extends({
        rootElementRef: rootElementRef,
        aria: aria,
        classes: classes,
        activeStateEnabled: activeStateEnabled,
        focusStateEnabled: focusStateEnabled,
        hoverStateEnabled: hoverStateEnabled,
        accessKey: accessKey,
        className: className,
        rtlEnabled: rtlEnabled,
        hint: hint,
        disabled: disabled,
        height: height,
        width: width,
        onFocusIn: onFocusIn,
        onClick: onClick,
        onKeyDown: onKeyDown,
        tabIndex: tabIndex,
        visible: visible
    }, restAttributes, {
        children: createFragment([children, isValidationMessageVisible && createComponentVNode(2, ValidationMessage, {
            validationErrors: validationErrors,
            mode: validationMessageMode,
            positionRequest: "below",
            rtlEnabled: rtlEnabled,
            target: validationMessageTarget,
            boundary: validationMessageTarget,
            container: validationMessageTarget,
            contentId: validationMessageGuid
        })], 0)
    }), null, widgetRef))
};
export var EditorProps = Object.create(Object.prototype, _extends(Object.getOwnPropertyDescriptors(BaseWidgetProps), Object.getOwnPropertyDescriptors({
    readOnly: false,
    name: "",
    validationError: null,
    validationErrors: null,
    validationMessageMode: "auto",
    validationStatus: "valid",
    isValid: true,
    defaultValue: null,
    valueChange: () => {}
})));
export var EditorPropsType = {
    get readOnly() {
        return EditorProps.readOnly
    },
    get name() {
        return EditorProps.name
    },
    get validationError() {
        return EditorProps.validationError
    },
    get validationErrors() {
        return EditorProps.validationErrors
    },
    get validationMessageMode() {
        return EditorProps.validationMessageMode
    },
    get validationStatus() {
        return EditorProps.validationStatus
    },
    get isValid() {
        return EditorProps.isValid
    },
    get defaultValue() {
        return EditorProps.defaultValue
    },
    get valueChange() {
        return EditorProps.valueChange
    },
    get className() {
        return EditorProps.className
    },
    get activeStateEnabled() {
        return EditorProps.activeStateEnabled
    },
    get disabled() {
        return EditorProps.disabled
    },
    get focusStateEnabled() {
        return EditorProps.focusStateEnabled
    },
    get hoverStateEnabled() {
        return EditorProps.hoverStateEnabled
    },
    get rtlEnabled() {
        return EditorProps.rtlEnabled
    },
    get tabIndex() {
        return EditorProps.tabIndex
    },
    get visible() {
        return EditorProps.visible
    },
    get aria() {
        return WidgetProps.aria
    },
    get classes() {
        return WidgetProps.classes
    }
};
import {
    convertRulesToOptions
} from "../../../../core/options/utils";
import {
    createReRenderEffect
} from "@devextreme/runtime/inferno";
import {
    createRef as infernoCreateRef
} from "inferno";
export class Editor extends InfernoWrapperComponent {
    constructor(props) {
        super(props);
        this.widgetRef = infernoCreateRef();
        this.rootElementRef = infernoCreateRef();
        this.__getterCache = {};
        this.state = {
            validationMessageGuid: "dx-".concat(new Guid),
            isValidationMessageVisible: false,
            value: void 0 !== this.props.value ? this.props.value : this.props.defaultValue
        };
        this.updateValidationMessageVisibility = this.updateValidationMessageVisibility.bind(this);
        this.focus = this.focus.bind(this);
        this.blur = this.blur.bind(this);
        this.onFocusIn = this.onFocusIn.bind(this)
    }
    createEffects() {
        return [new InfernoEffect(this.updateValidationMessageVisibility, [this.props.isValid, this.props.validationStatus, this.props.validationError, this.props.validationErrors]), createReRenderEffect()]
    }
    updateEffects() {
        var _this$_effects$;
        null === (_this$_effects$ = this._effects[0]) || void 0 === _this$_effects$ ? void 0 : _this$_effects$.update([this.props.isValid, this.props.validationStatus, this.props.validationError, this.props.validationErrors])
    }
    updateValidationMessageVisibility() {
        this.setState(__state_argument => ({
            isValidationMessageVisible: this.shouldShowValidationMessage
        }))
    }
    onFocusIn(event) {
        var {
            onFocusIn: onFocusIn
        } = this.props;
        null === onFocusIn || void 0 === onFocusIn ? void 0 : onFocusIn(event)
    }
    get cssClasses() {
        return "".concat(getCssClasses(_extends({}, this.props, {
            value: void 0 !== this.props.value ? this.props.value : this.state.value
        })))
    }
    get shouldShowValidationMessage() {
        var _this$validationError;
        var {
            isValid: isValid,
            validationStatus: validationStatus
        } = this.props;
        var validationErrors = null !== (_this$validationError = this.validationErrors) && void 0 !== _this$validationError ? _this$validationError : [];
        var isEditorValid = isValid && "invalid" !== validationStatus;
        return !isEditorValid && validationErrors.length > 0
    }
    get aria() {
        var {
            isValid: isValid,
            readOnly: readOnly
        } = this.props;
        var result = {
            readonly: readOnly ? "true" : "false",
            invalid: !isValid ? "true" : "false"
        };
        if (this.shouldShowValidationMessage) {
            result.describedBy = this.state.validationMessageGuid
        }
        return _extends({}, result, this.props.aria)
    }
    get validationErrors() {
        if (void 0 !== this.__getterCache.validationErrors) {
            return this.__getterCache.validationErrors
        }
        return this.__getterCache.validationErrors = (() => {
            var {
                validationError: validationError,
                validationErrors: validationErrors
            } = this.props;
            var allValidationErrors = validationErrors && [...validationErrors];
            if (!allValidationErrors && validationError) {
                allValidationErrors = [_extends({}, validationError)]
            }
            return allValidationErrors
        })()
    }
    get validationMessageTarget() {
        var _this$rootElementRef;
        return null === (_this$rootElementRef = this.rootElementRef) || void 0 === _this$rootElementRef ? void 0 : _this$rootElementRef.current
    }
    get restAttributes() {
        var _this$props$value = _extends({}, this.props, {
                value: void 0 !== this.props.value ? this.props.value : this.state.value
            }),
            restProps = _objectWithoutPropertiesLoose(_this$props$value, _excluded);
        return restProps
    }
    focus() {
        this.widgetRef.current.focus()
    }
    blur() {
        this.widgetRef.current.blur()
    }
    componentWillUpdate(nextProps, nextState, context) {
        super.componentWillUpdate();
        if (this.props.validationError !== nextProps.validationError || this.props.validationErrors !== nextProps.validationErrors) {
            this.__getterCache.validationErrors = void 0
        }
    }
    render() {
        var props = this.props;
        return viewFunction({
            props: _extends({}, props, {
                value: void 0 !== this.props.value ? this.props.value : this.state.value
            }),
            validationMessageGuid: this.state.validationMessageGuid,
            isValidationMessageVisible: this.state.isValidationMessageVisible,
            rootElementRef: this.rootElementRef,
            widgetRef: this.widgetRef,
            onFocusIn: this.onFocusIn,
            cssClasses: this.cssClasses,
            shouldShowValidationMessage: this.shouldShowValidationMessage,
            aria: this.aria,
            validationErrors: this.validationErrors,
            validationMessageTarget: this.validationMessageTarget,
            restAttributes: this.restAttributes
        })
    }
}

function __processTwoWayProps(defaultProps) {
    var twoWayProps = ["value"];
    return Object.keys(defaultProps).reduce((props, propName) => {
        var propValue = defaultProps[propName];
        var defaultPropName = twoWayProps.some(p => p === propName) ? "default" + propName.charAt(0).toUpperCase() + propName.slice(1) : propName;
        props[defaultPropName] = propValue;
        return props
    }, {})
}
Editor.defaultProps = EditorPropsType;
var __defaultOptionRules = [];
export function defaultOptions(rule) {
    __defaultOptionRules.push(rule);
    Editor.defaultProps = Object.create(Object.prototype, _extends(Object.getOwnPropertyDescriptors(Editor.defaultProps), Object.getOwnPropertyDescriptors(__processTwoWayProps(convertRulesToOptions(__defaultOptionRules)))))
}
