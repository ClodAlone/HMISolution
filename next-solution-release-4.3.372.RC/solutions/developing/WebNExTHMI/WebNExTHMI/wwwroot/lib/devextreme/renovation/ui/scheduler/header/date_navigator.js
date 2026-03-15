/**
 * DevExtreme (renovation/ui/scheduler/header/date_navigator.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.getDateNavigator = void 0;
var _themes = require("../../../../ui/themes");

function _extends() {
    _extends = Object.assign || function(target) {
        for (var i = 1; i < arguments.length; i++) {
            var source = arguments[i];
            for (var key in source) {
                if (Object.prototype.hasOwnProperty.call(source, key)) {
                    target[key] = source[key]
                }
            }
        }
        return target
    };
    return _extends.apply(this, arguments)
}
var DATE_NAVIGATOR_CLASS = "dx-scheduler-navigator";
var PREVIOUS_BUTTON_CLASS = "dx-scheduler-navigator-previous";
var CALENDAR_BUTTON_CLASS = "dx-scheduler-navigator-caption";
var NEXT_BUTTON_CLASS = "dx-scheduler-navigator-next";
var DIRECTION_LEFT = -1;
var DIRECTION_RIGHT = 1;
var getPreviousButtonOptions = function(isPreviousButtonDisabled) {
    return {
        icon: "chevronprev",
        elementAttr: {
            class: PREVIOUS_BUTTON_CLASS
        },
        disabled: isPreviousButtonDisabled
    }
};
var getCalendarButtonOptions = function(captionText) {
    return {
        text: captionText,
        elementAttr: {
            class: CALENDAR_BUTTON_CLASS
        }
    }
};
var getNextButtonOptions = function(isNextButtonDisabled) {
    return {
        icon: "chevronnext",
        elementAttr: {
            class: NEXT_BUTTON_CLASS
        },
        disabled: isNextButtonDisabled
    }
};
var getDateNavigator = function(item, showCalendar, captionText, updateDateByDirection, isPreviousButtonDisabled, isNextButtonDisabled) {
    var items = [getPreviousButtonOptions(isPreviousButtonDisabled), getCalendarButtonOptions(captionText), getNextButtonOptions(isNextButtonDisabled)];
    var stylingMode = (0, _themes.isMaterial)((0, _themes.current)()) ? "text" : "contained";
    return _extends({
        widget: "dxButtonGroup",
        cssClass: DATE_NAVIGATOR_CLASS,
        options: {
            items: items,
            stylingMode: stylingMode,
            selectionMode: "none",
            onItemClick: function(e) {
                switch (e.itemIndex) {
                    case 0:
                        updateDateByDirection(DIRECTION_LEFT);
                        break;
                    case 1:
                        showCalendar();
                        break;
                    case 2:
                        updateDateByDirection(DIRECTION_RIGHT)
                }
            }
        }
    }, item)
};
exports.getDateNavigator = getDateNavigator;
