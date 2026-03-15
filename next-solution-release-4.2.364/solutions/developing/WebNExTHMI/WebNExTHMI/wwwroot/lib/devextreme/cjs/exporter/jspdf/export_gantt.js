/**
 * DevExtreme (cjs/exporter/jspdf/export_gantt.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.exportGantt = exportGantt;

function exportGantt(options) {
    var component = options.component;
    return null === component || void 0 === component ? void 0 : component.exportToPdf(options)
}
