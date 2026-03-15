/**
 * DevExtreme (esm/core/utils/template_manager.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import config from "../config";
import devices from "../devices";
import Errors from "../errors";
import $ from "../renderer";
import {
    ChildDefaultTemplate
} from "../templates/child_default_template";
import {
    EmptyTemplate
} from "../templates/empty_template";
import {
    Template
} from "../templates/template";
import {
    TemplateBase
} from "../templates/template_base";
import {
    groupBy
} from "./array";
import {
    findBestMatches
} from "./common";
import {
    normalizeTemplateElement
} from "./dom";
import {
    extend
} from "./extend";
import {
    isFunction,
    isRenderer
} from "./type";
export var findTemplates = (element, name) => {
    var templates = $(element).contents().filter("[".concat("data-options", '*="').concat(name, '"]'));
    return [].slice.call(templates).map(element => {
        var optionsString = $(element).attr("data-options") || "";
        return {
            element: element,
            options: config().optionsParser(optionsString)[name]
        }
    }).filter(template => !!template.options)
};
export var suitableTemplatesByName = rawTemplates => {
    var templatesMap = groupBy(rawTemplates, template => template.options.name);
    if (templatesMap[void 0]) {
        throw Errors.Error("E0023")
    }
    var result = {};
    Object.keys(templatesMap).forEach(name => {
        var _findBestMatches$;
        var suitableTemplate = null === (_findBestMatches$ = findBestMatches(devices.current(), templatesMap[name], template => template.options)[0]) || void 0 === _findBestMatches$ ? void 0 : _findBestMatches$.element;
        if (suitableTemplate) {
            result[name] = suitableTemplate
        }
    });
    return result
};
export var addOneRenderedCall = template => {
    var render = template.render.bind(template);
    return extend({}, template, {
        render(options) {
            var templateResult = render(options);
            options && options.onRendered && options.onRendered();
            return templateResult
        }
    })
};
export var getNormalizedTemplateArgs = options => {
    var args = [];
    if ("model" in options) {
        args.push(options.model)
    }
    if ("index" in options) {
        args.push(options.index)
    }
    args.push(options.container);
    return args
};
export var validateTemplateSource = templateSource => "string" === typeof templateSource ? normalizeTemplateElement(templateSource) : templateSource;
export var templateKey = templateSource => isRenderer(templateSource) && templateSource[0] || templateSource;
export var defaultCreateElement = element => new Template(element);
export var acquireIntegrationTemplate = (templateSource, templates, isAsyncTemplate, skipTemplates) => {
    var integrationTemplate = null;
    if (!skipTemplates || -1 === skipTemplates.indexOf(templateSource)) {
        integrationTemplate = templates[templateSource];
        if (integrationTemplate && !(integrationTemplate instanceof TemplateBase) && !isAsyncTemplate) {
            integrationTemplate = addOneRenderedCall(integrationTemplate)
        }
    }
    return integrationTemplate
};
export var acquireTemplate = (templateSource, createTemplate, templates, isAsyncTemplate, skipTemplates, defaultTemplates) => {
    if (null == templateSource) {
        return new EmptyTemplate
    }
    if (templateSource instanceof ChildDefaultTemplate) {
        return defaultTemplates[templateSource.name]
    }
    if (templateSource instanceof TemplateBase) {
        return templateSource
    }
    if (isFunction(templateSource.render) && !isRenderer(templateSource)) {
        return isAsyncTemplate ? templateSource : addOneRenderedCall(templateSource)
    }
    if (templateSource.nodeType || isRenderer(templateSource)) {
        return createTemplate($(templateSource))
    }
    return acquireIntegrationTemplate(templateSource, templates, isAsyncTemplate, skipTemplates) || defaultTemplates[templateSource] || createTemplate(templateSource)
};
