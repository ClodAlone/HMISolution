/**
 * DevExtreme (cjs/core/templates/template_engine_registry.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.getCurrentTemplateEngine = getCurrentTemplateEngine;
exports.registerTemplateEngine = registerTemplateEngine;
exports.setTemplateEngine = setTemplateEngine;
var _type = require("../utils/type");
var _errors = _interopRequireDefault(require("../errors"));

function _interopRequireDefault(obj) {
    return obj && obj.__esModule ? obj : {
        default: obj
    }
}
var templateEngines = {};
var currentTemplateEngine;

function registerTemplateEngine(name, templateEngine) {
    templateEngines[name] = templateEngine
}

function setTemplateEngine(templateEngine) {
    if ((0, _type.isString)(templateEngine)) {
        currentTemplateEngine = templateEngines[templateEngine];
        if (!currentTemplateEngine) {
            throw _errors.default.Error("E0020", templateEngine)
        }
    } else {
        currentTemplateEngine = templateEngine
    }
}

function getCurrentTemplateEngine() {
    return currentTemplateEngine
}
