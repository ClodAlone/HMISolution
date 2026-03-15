/**
 * DevExtreme (esm/renovation/utils/shallow_equals.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
export var shallowEquals = (firstObject, secondObject) => {
    if (Object.keys(firstObject).length !== Object.keys(secondObject).length) {
        return false
    }
    return Object.keys(firstObject).every(key => firstObject[key] === secondObject[key])
};
