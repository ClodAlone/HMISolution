/**
 * DevExtreme (esm/renovation/ui/scheduler/header/date_navigator.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import _extends from "@babel/runtime/helpers/esm/extends";
import {
    isMaterial,
    current
} from "../../../../ui/themes";
var DATE_NAVIGATOR_CLASS = "dx-scheduler-navigator";
var PREVIOUS_BUTTON_CLASS = "dx-scheduler-navigator-previous";
var CALENDAR_BUTTON_CLASS = "dx-scheduler-navigator-caption";
var NEXT_BUTTON_CLASS = "dx-scheduler-navigator-next";
var DIRECTION_LEFT = -1;
var DIRECTION_RIGHT = 1;
var getPreviousButtonOptions = isPreviousButtonDisabled => ({
    icon: "chevronprev",
    elementAttr: {
        class: PREVIOUS_BUTTON_CLASS
    },
    disabled: isPreviousButtonDisabled
});
var getCalendarButtonOptions = captionText => ({
    text: captionText,
    elementAttr: {
        class: CALENDAR_BUTTON_CLASS
    }
});
var getNextButtonOptions = isNextButtonDisabled => ({
    icon: "chevronnext",
    elementAttr: {
        class: NEXT_BUTTON_CLASS
    },
    disabled: isNextButtonDisabled
});
export var getDateNavigator = (item, showCalendar, captionText, updateDateByDirection, isPreviousButtonDisabled, isNextButtonDisabled) => {
    var items = [getPreviousButtonOptions(isPreviousButtonDisabled), getCalendarButtonOptions(captionText), getNextButtonOptions(isNextButtonDisabled)];
    var stylingMode = isMaterial(current()) ? "text" : "contained";
    return _extends({
        widget: "dxButtonGroup",
        cssClass: DATE_NAVIGATOR_CLASS,
        options: {
            items: items,
            stylingMode: stylingMode,
            selectionMode: "none",
            onItemClick: e => {
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
