/**
 * DevExtreme (cjs/renovation/utils/render_template.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.renderTemplate = renderTemplate;
var _inferno = require("inferno");
var _infernoCreateElement = require("inferno-create-element");

function renderTemplate(template, props, container) {
    setTimeout((function() {
        (0, _inferno.render)((0, _infernoCreateElement.createElement)(template, props), null === container || void 0 === container ? void 0 : container.get(0))
    }), 0)
}
