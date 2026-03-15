/**
 * DevExtreme (esm/ui/html_editor/converterController.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
class ConverterController {
    constructor() {
        this._converters = {}
    }
    addConverter(name, converter) {
        this._converters[name] = converter
    }
    getConverter(name) {
        return this._converters[name]
    }
}
var controller = new ConverterController;
export default controller;
