/**
 * DevExtreme (esm/renovation/component_wrapper/common/template_wrapper.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import _extends from "@babel/runtime/helpers/esm/extends";
import {
    InfernoComponent,
    InfernoEffect
} from "@devextreme/runtime/inferno";
import {
    findDOMfromVNode
} from "inferno";
import {
    shallowEquals
} from "../../utils/shallow_equals";
import {
    replaceWith
} from "../../../core/utils/dom";
import $ from "../../../core/renderer";
import domAdapter from "../../../core/dom_adapter";
import {
    getPublicElement
} from "../../../core/element";
import {
    removeDifferentElements
} from "../utils/utils";
import Number from "../../../core/polyfills/number";
import {
    isDefined
} from "../../../core/utils/type";
export class TemplateWrapper extends InfernoComponent {
    constructor(props) {
        super(props);
        this.renderTemplate = this.renderTemplate.bind(this)
    }
    renderTemplate() {
        var _this$props$model;
        var node = findDOMfromVNode(this.$LI, true);
        var parentNode = node.parentNode;
        var $parent = $(parentNode);
        var $children = $parent.contents();
        var {
            data: data,
            index: index
        } = null !== (_this$props$model = this.props.model) && void 0 !== _this$props$model ? _this$props$model : {
            data: {}
        };
        if (data) {
            Object.keys(data).forEach(name => {
                if (data[name] && domAdapter.isNode(data[name])) {
                    data[name] = getPublicElement($(data[name]))
                }
            })
        }
        var $result = $(this.props.template.render(_extends({
            container: getPublicElement($parent),
            transclude: this.props.transclude
        }, !this.props.transclude ? {
            model: data
        } : {}, !this.props.transclude && Number.isFinite(index) ? {
            index: index
        } : {})));
        replaceWith($(node), $result);
        return () => {
            removeDifferentElements($children, $parent.contents());
            parentNode.appendChild(node)
        }
    }
    shouldComponentUpdate(nextProps) {
        var {
            model: model,
            template: template
        } = this.props;
        var {
            model: nextModel,
            template: nextTemplate
        } = nextProps;
        var sameTemplate = template === nextTemplate;
        if (!sameTemplate) {
            return true
        }
        if (isDefined(model) && isDefined(nextModel)) {
            var {
                data: data,
                index: index
            } = model;
            var {
                data: nextData,
                index: nextIndex
            } = nextModel;
            return index !== nextIndex || !shallowEquals(data, nextData)
        }
        var sameModel = model === nextModel;
        return !sameModel
    }
    createEffects() {
        return [new InfernoEffect(this.renderTemplate, [this.props.template, this.props.model])]
    }
    updateEffects() {
        this._effects[0].update([this.props.template, this.props.model])
    }
    componentWillUnmount() {}
    render() {
        return null
    }
}
