/**
 * DevExtreme (esm/ui/pivot_grid/remote_store.utils.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
export var forEachGroup = function forEachGroup(data, callback, level) {
    data = data || [];
    level = level || 0;
    for (var i = 0; i < data.length; i++) {
        var group = data[i];
        callback(group, level);
        if (group && group.items && group.items.length) {
            forEachGroup(group.items, callback, level + 1)
        }
    }
};
