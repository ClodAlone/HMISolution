/**
 * DevExtreme (esm/core/templates/template_engines.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import {
    extractTemplateMarkup
} from "../utils/dom";
import {
    registerTemplateEngine
} from "./template_engine_registry";
registerTemplateEngine("jquery-tmpl", {
    compile: element => extractTemplateMarkup(element),
    render: (template, data) => jQuery.tmpl(template, data)
});
registerTemplateEngine("jsrender", {
    compile: element => (jQuery ? jQuery : jsrender).templates(extractTemplateMarkup(element)),
    render: (template, data) => template.render(data)
});
registerTemplateEngine("mustache", {
    compile: element => extractTemplateMarkup(element),
    render: (template, data) => Mustache.render(template, data)
});
registerTemplateEngine("hogan", {
    compile: element => Hogan.compile(extractTemplateMarkup(element)),
    render: (template, data) => template.render(data)
});
registerTemplateEngine("underscore", {
    compile: element => _.template(extractTemplateMarkup(element)),
    render: (template, data) => template(data)
});
registerTemplateEngine("handlebars", {
    compile: element => Handlebars.compile(extractTemplateMarkup(element)),
    render: (template, data) => template(data)
});
registerTemplateEngine("doT", {
    compile: element => doT.template(extractTemplateMarkup(element)),
    render: (template, data) => template(data)
});
