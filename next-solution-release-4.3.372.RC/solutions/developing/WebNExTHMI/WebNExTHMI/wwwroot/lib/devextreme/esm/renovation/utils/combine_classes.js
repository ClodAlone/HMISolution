/**
 * DevExtreme (esm/renovation/utils/combine_classes.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
export function combineClasses(classesMap) {
    return Object.keys(classesMap).filter(p => classesMap[p]).join(" ")
}
