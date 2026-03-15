/**
 * DevExtreme (esm/renovation/ui/common/icon.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import _objectWithoutPropertiesLoose from "@babel/runtime/helpers/esm/objectWithoutPropertiesLoose";
import _extends from "@babel/runtime/helpers/esm/extends";
var _excluded = ["iconTemplate", "position", "source"];
import {
    createVNode,
    createFragment,
    createComponentVNode,
    normalizeProps
} from "inferno";
import {
    Fragment
} from "inferno";
import {
    BaseInfernoComponent
} from "@devextreme/runtime/inferno";
import {
    getImageSourceType
} from "../../../core/utils/icon";
import {
    combineClasses
} from "../../utils/combine_classes";
export var viewFunction = _ref => {
    var {
        iconClassName: iconClassName,
        props: {
            iconTemplate: IconTemplate,
            source: source
        },
        sourceType: sourceType
    } = _ref;
    return createFragment(["dxIcon" === sourceType && createVNode(1, "i", iconClassName), "fontIcon" === sourceType && createVNode(1, "i", iconClassName), "image" === sourceType && createVNode(1, "img", iconClassName, null, 1, {
        alt: "",
        src: source
    }), IconTemplate && createVNode(1, "i", iconClassName, IconTemplate({}), 0)], 0)
};
export var IconProps = {
    position: "left",
    source: ""
};
var getTemplate = TemplateProp => TemplateProp && (TemplateProp.defaultProps ? props => normalizeProps(createComponentVNode(2, TemplateProp, _extends({}, props))) : TemplateProp);
export class Icon extends BaseInfernoComponent {
    constructor(props) {
        super(props);
        this.state = {}
    }
    get sourceType() {
        return getImageSourceType(this.props.source)
    }
    get cssClass() {
        return "left" !== this.props.position ? "dx-icon-right" : ""
    }
    get iconClassName() {
        var generalClasses = {
            "dx-icon": true,
            [this.cssClass]: !!this.cssClass
        };
        var {
            source: source
        } = this.props;
        if ("dxIcon" === this.sourceType) {
            return combineClasses(_extends({}, generalClasses, {
                ["dx-icon-".concat(source)]: true
            }))
        }
        if ("fontIcon" === this.sourceType) {
            return combineClasses(_extends({}, generalClasses, {
                [String(source)]: !!source
            }))
        }
        if ("image" === this.sourceType) {
            return combineClasses(generalClasses)
        }
        if ("svg" === this.sourceType) {
            return combineClasses(_extends({}, generalClasses, {
                "dx-svg-icon": true
            }))
        }
        return ""
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
                iconTemplate: getTemplate(props.iconTemplate)
            }),
            sourceType: this.sourceType,
            cssClass: this.cssClass,
            iconClassName: this.iconClassName,
            restAttributes: this.restAttributes
        })
    }
}
Icon.defaultProps = IconProps;
