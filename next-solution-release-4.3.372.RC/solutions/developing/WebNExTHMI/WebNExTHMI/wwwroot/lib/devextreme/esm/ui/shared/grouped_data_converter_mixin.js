/**
 * DevExtreme (esm/ui/shared/grouped_data_converter_mixin.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import {
    isObject
} from "../../core/utils/type";
var isCorrectStructure = data => Array.isArray(data) && data.every(item => {
    var hasTwoFields = 2 === Object.keys(item).length;
    var hasCorrectFields = "key" in item && "items" in item;
    return hasTwoFields && hasCorrectFields && Array.isArray(item.items)
});
export default {
    _getSpecificDataSourceOption: function() {
        var dataSource = this.option("dataSource");
        var hasSimpleItems = false;
        var data = {};
        if (this._getGroupedOption() && isCorrectStructure(dataSource)) {
            data = dataSource.reduce((accumulator, item) => {
                var items = item.items.map(innerItem => {
                    if (!isObject(innerItem)) {
                        innerItem = {
                            text: innerItem
                        };
                        hasSimpleItems = true
                    }
                    if (!("key" in innerItem)) {
                        innerItem.key = item.key
                    }
                    return innerItem
                });
                return accumulator.concat(items)
            }, []);
            dataSource = {
                store: {
                    type: "array",
                    data: data
                },
                group: {
                    selector: "key",
                    keepInitialKeyOrder: true
                }
            };
            if (hasSimpleItems) {
                dataSource.searchExpr = "text"
            }
        }
        return dataSource
    }
};
