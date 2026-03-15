/**
 * DevExtreme (cjs/ui/gantt/ui.gantt.mapping_helper.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.GanttMappingHelper = void 0;
var _data = require("../../core/utils/data");
var GANTT_TASKS = "tasks";
var GANTT_MAPPED_FIELD_REGEX = /(\w*)Expr/;
var GanttMappingHelper = function() {
    function GanttMappingHelper(gantt) {
        this._gantt = gantt
    }
    var _proto = GanttMappingHelper.prototype;
    _proto._getMappedFieldName = function(optionName, coreField) {
        var coreFieldName = coreField;
        if ("id" === coreField) {
            coreFieldName = "key"
        }
        return this._gantt.option("".concat(optionName, ".").concat(coreFieldName, "Expr"))
    };
    _proto.getTaskMappedFieldNames = function() {
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
    };
    _proto.convertCoreToMappedData = function(optionName, coreData) {
        var _this = this;
        return Object.keys(coreData).reduce((function(previous, f) {
            var mappedField = _this._getMappedFieldName(optionName, f);
            if (mappedField) {
                var setter = (0, _data.compileSetter)(mappedField);
                setter(previous, coreData[f])
            }
            return previous
        }), {})
    };
    _proto.convertMappedToCoreData = function(optionName, mappedData) {
        var coreData = {};
        if (mappedData) {
            var mappedFields = this._gantt.option(optionName);
            for (var field in mappedFields) {
                var exprMatches = field.match(GANTT_MAPPED_FIELD_REGEX);
                var mappedFieldName = exprMatches && mappedFields[exprMatches[0]];
                if (mappedFieldName && void 0 !== mappedData[mappedFieldName]) {
                    var getter = (0, _data.compileGetter)(mappedFieldName);
                    var coreFieldName = exprMatches[1];
                    coreData[coreFieldName] = getter(mappedData)
                }
            }
        }
        return coreData
    };
    _proto.convertCoreToMappedFields = function(optionName, fields) {
        var _this2 = this;
        return fields.reduce((function(previous, f) {
            var mappedField = _this2._getMappedFieldName(optionName, f);
            if (mappedField) {
                previous.push(mappedField)
            }
            return previous
        }), [])
    };
    _proto.convertMappedToCoreFields = function(optionName, fields) {
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
    };
    return GanttMappingHelper
}();
exports.GanttMappingHelper = GanttMappingHelper;
