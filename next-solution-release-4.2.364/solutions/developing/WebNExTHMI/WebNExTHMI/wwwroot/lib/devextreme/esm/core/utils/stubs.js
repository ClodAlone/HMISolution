/**
 * DevExtreme (esm/core/utils/stubs.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
export function stubComponent(componentName) {
    return class {
        constructor() {
            throw new Error("Module '".concat(componentName, "' not found"))
        }
        static getInstance() {}
    }
}
