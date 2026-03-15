/**
 * DevExtreme (esm/integration/knockout/template.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import $ from "../../core/renderer";
import domAdapter from "../../core/dom_adapter";
import ko from "knockout";
import {
    isDefined
} from "../../core/utils/type";
import {
    TemplateBase
} from "../../core/templates/template_base";
import {
    normalizeTemplateElement
} from "../../core/utils/dom";
import {
    getClosestNodeWithContext
} from "./utils";
var getParentContext = function(data) {
    var parentNode = domAdapter.createElement("div");
    ko.applyBindingsToNode(parentNode, null, data);
    var parentContext = ko.contextFor(parentNode);
    ko.cleanNode(parentNode);
    return parentContext
};
export var KoTemplate = class extends TemplateBase {
    constructor(element) {
        super();
        this._element = element;
        this._template = $("<div>").append(normalizeTemplateElement(element));
        this._registerKoTemplate()
    }
    _registerKoTemplate() {
        var template = this._template.get(0);
        new ko.templateSources.anonymousTemplate(template).nodes(template)
    }
    _prepareDataForContainer(data, container) {
        if (container && container.length) {
            var node = getClosestNodeWithContext(container.get(0));
            var containerContext = ko.contextFor(node);
            data = void 0 !== data ? data : ko.dataFor(node) || {};
            if (containerContext) {
                return data === containerContext.$data ? containerContext : containerContext.createChildContext(data)
            }
        }
        return getParentContext(data).createChildContext(data)
    }
    _renderCore(options) {
        var model = this._prepareDataForContainer(options.model, $(options.container));
        if (isDefined(options.index)) {
            model.$index = options.index
        }
        var $placeholder = $("<div>").appendTo(options.container);
        var $result;
        ko.renderTemplate(this._template.get(0), model, {
            afterRender: function(nodes) {
                $result = $(nodes)
            }
        }, $placeholder.get(0), "replaceNode");
        return $result
    }
    source() {
        return $(this._element).clone()
    }
    dispose() {
        this._template.remove()
    }
};
