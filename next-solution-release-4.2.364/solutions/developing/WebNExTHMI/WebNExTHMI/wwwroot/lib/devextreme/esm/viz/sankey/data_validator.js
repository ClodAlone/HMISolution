/**
 * DevExtreme (esm/viz/sankey/data_validator.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import graphModule from "./graph";
var validator = {
    validate: function(data, incidentOccurred) {
        var result = null;
        if (this._hasCycle(data)) {
            result = "E2006";
            incidentOccurred("E2006")
        }
        return result
    },
    _hasCycle: function(data) {
        return graphModule.struct.hasCycle(data)
    }
};
export default validator;
