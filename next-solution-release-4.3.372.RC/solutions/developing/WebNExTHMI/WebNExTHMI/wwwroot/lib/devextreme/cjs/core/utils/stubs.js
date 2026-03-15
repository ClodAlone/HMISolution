/**
 * DevExtreme (cjs/core/utils/stubs.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.stubComponent = stubComponent;

function stubComponent(componentName) {
    return function() {
        function NoComponent() {
            throw new Error("Module '".concat(componentName, "' not found"))
        }
        NoComponent.getInstance = function() {};
        return NoComponent
    }()
}
