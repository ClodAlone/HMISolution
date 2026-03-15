/**
 * DevExtreme (esm/ui/list/ui.list.edit.decorator_registry.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import {
    extend
} from "../../core/utils/extend";
export var registry = {};
export function register(option, type, decoratorClass) {
    var decoratorsRegistry = registry;
    var decoratorConfig = {};
    decoratorConfig[option] = decoratorsRegistry[option] ? decoratorsRegistry[option] : {};
    decoratorConfig[option][type] = decoratorClass;
    extend(decoratorsRegistry, decoratorConfig)
}
