/**
 * DevExtreme (cjs/ui/scheduler/workspaces/view_model/utils.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.getViewDataGeneratorByViewType = void 0;
var _constants = require("../../constants");
var _view_data_generator = require("./view_data_generator");
var _view_data_generator_day = require("./view_data_generator_day");
var _view_data_generator_month = require("./view_data_generator_month");
var _view_data_generator_timeline_month = require("./view_data_generator_timeline_month");
var _view_data_generator_week = require("./view_data_generator_week");
var _view_data_generator_work_week = require("./view_data_generator_work_week");
var getViewDataGeneratorByViewType = function(viewType) {
    switch (viewType) {
        case _constants.VIEWS.MONTH:
            return new _view_data_generator_month.ViewDataGeneratorMonth;
        case _constants.VIEWS.TIMELINE_MONTH:
            return new _view_data_generator_timeline_month.ViewDataGeneratorTimelineMonth;
        case _constants.VIEWS.DAY:
        case _constants.VIEWS.TIMELINE_DAY:
            return new _view_data_generator_day.ViewDataGeneratorDay;
        case _constants.VIEWS.WEEK:
        case _constants.VIEWS.TIMELINE_WEEK:
            return new _view_data_generator_week.ViewDataGeneratorWeek;
        case _constants.VIEWS.WORK_WEEK:
        case _constants.VIEWS.TIMELINE_WORK_WEEK:
            return new _view_data_generator_work_week.ViewDataGeneratorWorkWeek;
        default:
            return new _view_data_generator.ViewDataGenerator
    }
};
exports.getViewDataGeneratorByViewType = getViewDataGeneratorByViewType;
