/**
 * DevExtreme (esm/core/templates/empty_template.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import $ from "../renderer";
import {
    TemplateBase
} from "./template_base";
export class EmptyTemplate extends TemplateBase {
    _renderCore() {
        return $()
    }
}
