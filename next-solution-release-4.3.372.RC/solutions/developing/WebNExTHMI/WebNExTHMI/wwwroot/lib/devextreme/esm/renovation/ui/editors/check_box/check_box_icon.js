/**
 * DevExtreme (esm/renovation/ui/editors/check_box/check_box_icon.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import _extends from "@babel/runtime/helpers/esm/extends";
import _objectWithoutPropertiesLoose from "@babel/runtime/helpers/esm/objectWithoutPropertiesLoose";
var _excluded = ["isChecked", "size"];
import {
    createVNode
} from "inferno";
import {
    InfernoEffect,
    InfernoComponent,
    normalizeStyles
} from "@devextreme/runtime/inferno";
import getElementComputedStyle from "../../../utils/get_computed_style";
import {
    hasWindow
} from "../../../../core/utils/window";
import {
    normalizeStyleProp
} from "../../../../core/utils/style";
import {
    isNumeric as isNumber
} from "../../../../core/utils/type";
import {
    getFontSizeByIconSize
} from "./utils";
export var viewFunction = viewModel => {
    var {
        cssStyles: cssStyles,
        elementRef: elementRef
    } = viewModel;
    return createVNode(1, "span", "dx-checkbox-icon", null, 1, {
        style: normalizeStyles(cssStyles)
    }, null, elementRef)
};
export var CheckBoxIconProps = {
    isChecked: false
};
import {
    createRef as infernoCreateRef
} from "inferno";
export class CheckBoxIcon extends InfernoComponent {
    constructor(props) {
        super(props);
        this.state = {};
        this.elementRef = infernoCreateRef();
        this.__getterCache = {};
        this.updateFontSize = this.updateFontSize.bind(this);
        this.setIconFontSize = this.setIconFontSize.bind(this);
        this.getIconSize = this.getIconSize.bind(this);
        this.getComputedIconSize = this.getComputedIconSize.bind(this)
    }
    createEffects() {
        return [new InfernoEffect(this.updateFontSize, [this.props.isChecked, this.props.size])]
    }
    updateEffects() {
        var _this$_effects$;
        null === (_this$_effects$ = this._effects[0]) || void 0 === _this$_effects$ ? void 0 : _this$_effects$.update([this.props.isChecked, this.props.size])
    }
    updateFontSize() {
        var {
            isChecked: isChecked,
            size: size
        } = this.props;
        if (hasWindow() && size) {
            var newIconSize = this.getIconSize(size);
            var newFontSize = getFontSizeByIconSize(newIconSize, isChecked);
            this.setIconFontSize(newFontSize)
        }
    }
    setIconFontSize(fontSize) {
        var element = this.elementRef.current;
        element.style.fontSize = "".concat(fontSize, "px")
    }
    getIconSize(size) {
        if (isNumber(size)) {
            return size
        }
        if (size.endsWith("px")) {
            return parseInt(size, 10)
        }
        return this.getComputedIconSize()
    }
    getComputedIconSize() {
        var element = this.elementRef.current;
        var iconComputedStyle = getElementComputedStyle(element);
        var computedIconSize = parseInt(null === iconComputedStyle || void 0 === iconComputedStyle ? void 0 : iconComputedStyle.width, 10);
        return computedIconSize
    }
    get cssStyles() {
        if (void 0 !== this.__getterCache.cssStyles) {
            return this.__getterCache.cssStyles
        }
        return this.__getterCache.cssStyles = (() => {
            var {
                size: size
            } = this.props;
            var width = normalizeStyleProp("width", size);
            var height = normalizeStyleProp("height", size);
            return {
                height: height,
                width: width
            }
        })()
    }
    get restAttributes() {
        var _this$props = this.props,
            restProps = _objectWithoutPropertiesLoose(_this$props, _excluded);
        return restProps
    }
    componentWillUpdate(nextProps, nextState, context) {
        super.componentWillUpdate();
        if (this.props.size !== nextProps.size) {
            this.__getterCache.cssStyles = void 0
        }
    }
    render() {
        var props = this.props;
        return viewFunction({
            props: _extends({}, props),
            elementRef: this.elementRef,
            setIconFontSize: this.setIconFontSize,
            getIconSize: this.getIconSize,
            getComputedIconSize: this.getComputedIconSize,
            cssStyles: this.cssStyles,
            restAttributes: this.restAttributes
        })
    }
}
CheckBoxIcon.defaultProps = CheckBoxIconProps;
