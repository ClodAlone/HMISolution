/**
 * DevExtreme (esm/renovation/ui/common/utils/get_updated_options.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import _extends from "@babel/runtime/helpers/esm/extends";
import {
    isPlainObject,
    type
} from "../../../../core/utils/type";
var defaultNotDeepCopyArrays = ["dataSource", "selectedRowKeys"];
var propsToIgnore = {
    integrationOptions: true
};

function getDiffItem(key, value, previousValue) {
    return {
        path: key,
        value: value,
        previousValue: previousValue
    }
}

function compare(resultPaths, item1, item2, key, fullPropName, notDeepCopyArrays) {
    if (propsToIgnore[key]) {
        return
    }
    var type1 = type(item1);
    var type2 = type(item2);
    if (item1 === item2) {
        return
    }
    if (type1 !== type2) {
        resultPaths.push(getDiffItem(key, item2, item1))
    } else if ("object" === type1) {
        if (!isPlainObject(item2)) {
            resultPaths.push(getDiffItem(key, item2, item1))
        } else {
            var diffPaths = objectDiffs(item1, item2, fullPropName, notDeepCopyArrays);
            resultPaths.push(...diffPaths.map(item => _extends({}, item, {
                path: "".concat(key, ".").concat(item.path)
            })))
        }
    } else if ("array" === type1) {
        var notDeepCopy = notDeepCopyArrays.some(prop => fullPropName.includes(prop));
        if (notDeepCopy && item1 !== item2) {
            resultPaths.push(getDiffItem(key, item2, item1))
        } else if (item1.length !== item2.length) {
            resultPaths.push(getDiffItem(key, item2, item1))
        } else {
            var _diffPaths = objectDiffs(item1, item2, fullPropName, notDeepCopyArrays);
            [].push.apply(resultPaths, _diffPaths.map(item => _extends({}, item, {
                path: "".concat(key).concat(item.path)
            })))
        }
    } else {
        resultPaths.push(getDiffItem(key, item2, item1))
    }
}
var objectDiffsFiltered = propsEnumerator => (oldProps, props, fullPropName, notDeepCopyArrays) => {
    var resultPaths = [];
    var processItem = !Array.isArray(oldProps) ? propName => {
        compare(resultPaths, oldProps[propName], props[propName], propName, "".concat(fullPropName, ".").concat(propName), notDeepCopyArrays)
    } : propName => {
        compare(resultPaths, oldProps[propName], props[propName], "[".concat(propName, "]"), "".concat(fullPropName, ".").concat(propName), notDeepCopyArrays)
    };
    propsEnumerator(oldProps).forEach(processItem);
    Object.keys(props).filter(propName => !Object.prototype.hasOwnProperty.call(oldProps, propName) && oldProps[propName] !== props[propName]).forEach(propName => {
        resultPaths.push({
            path: propName,
            value: props[propName],
            previousValue: oldProps[propName]
        })
    });
    return resultPaths
};
var objectDiffs = objectDiffsFiltered(oldProps => Object.keys(oldProps));
var reactProps = {
    key: true,
    ref: true,
    children: true,
    style: true
};
var objectDiffsWithoutReactProps = objectDiffsFiltered(prop => Object.keys(prop).filter(p => !reactProps[p]));
export function getUpdatedOptions(oldProps, props) {
    var notDeepCopyArrays = arguments.length > 2 && void 0 !== arguments[2] ? arguments[2] : defaultNotDeepCopyArrays;
    return objectDiffsWithoutReactProps(oldProps, props, "", notDeepCopyArrays)
}
