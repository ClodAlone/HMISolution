/**
 * DevExtreme (esm/core/templates/function_template.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import {
    TemplateBase
} from "./template_base";
import {
    normalizeTemplateElement
} from "../utils/dom";
export class FunctionTemplate extends TemplateBase {
    constructor(render) {
        super();
        this._render = render
    }
    _renderCore(options) {
        return normalizeTemplateElement(this._render(options))
    }
}
