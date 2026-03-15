/**
 * DevExtreme (esm/ui/gantt/ui.gantt.mapping_helper.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import {
    compileGetter,
    compileSetter
} from "../../core/utils/data";
var GANTT_TASKS = "tasks";
var GANTT_MAPPED_FIELD_REGEX = /(\w*)Expr/;
export class GanttMappingHelper {
    constructor(gantt) {
        this._gantt = gantt
    }
    _getMappedFieldName(optionName, coreField) {
        var coreFieldName = coreField;
        if ("id" === coreField) {
            coreFieldName = "key"
        }
        return this._gantt.option("".concat(optionName, ".").concat(coreFieldName, "Expr"))
    }
    getTaskMappedFieldNames() {
        var mappedFields = [];
        var mappedFieldsData = this._gantt.option(GANTT_TASKS);
        for (var field in mappedFieldsData) {
            var exprMatches = field.match(GANTT_MAPPED_FIELD_REGEX);
            var mappedFieldName = exprMatches && mappedFieldsData[exprMatches[0]];
            if (mappedFieldName) {
                mappedFields.push(mappedFieldName)
            }
        }
        return mappedFields
    }
    convertCoreToMappedData(optionName, coreData) {
        return Object.keys(coreData).reduce((previous, f) => {
            var mappedField = this._getMappedFieldName(optionName, f);
            if (mappedField) {
                var setter = compileSetter(mappedField);
                setter(previous, coreData[f])
            }
            return previous
        }, {})
    }
    convertMappedToCoreData(optionName, mappedData) {
        var coreData = {};
        if (mappedData) {
            var mappedFields = this._gantt.option(optionName);
            for (var field in mappedFields) {
                var exprMatches = field.match(GANTT_MAPPED_FIELD_REGEX);
                var mappedFieldName = exprMatches && mappedFields[exprMatches[0]];
                if (mappedFieldName && void 0 !== mappedData[mappedFieldName]) {
                    var getter = compileGetter(mappedFieldName);
                    var coreFieldName = exprMatches[1];
                    coreData[coreFieldName] = getter(mappedData)
                }
            }
        }
        return coreData
    }
    convertCoreToMappedFields(optionName, fields) {
        return fields.reduce((previous, f) => {
            var mappedField = this._getMappedFieldName(optionName, f);
            if (mappedField) {
                previous.push(mappedField)
            }
            return previous
        }, [])
    }
    convertMappedToCoreFields(optionName, fields) {
        var coreFields = [];
        var mappedFields = this._gantt.option(optionName);
        for (var field in mappedFields) {
            var exprMatches = field.match(GANTT_MAPPED_FIELD_REGEX);
            var mappedFieldName = exprMatches && mappedFields[exprMatches[0]];
            if (mappedFieldName && fields.indexOf(mappedFieldName) > -1) {
                var coreFieldName = exprMatches[1];
                coreFields.push(coreFieldName)
            }
        }
        return coreFields
    }
}
