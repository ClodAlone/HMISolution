/**
 * DevExtreme (esm/ui/scheduler/header/dateNavigator.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import _extends from "@babel/runtime/helpers/esm/extends";
import {
    isMaterial
} from "../../themes";
import dateUtils from "../../../core/utils/date";
var {
    trimTime: trimTime
} = dateUtils;
var DATE_NAVIGATOR_CLASS = "dx-scheduler-navigator";
var PREVIOUS_BUTTON_CLASS = "dx-scheduler-navigator-previous";
var CALENDAR_BUTTON_CLASS = "dx-scheduler-navigator-caption";
var NEXT_BUTTON_CLASS = "dx-scheduler-navigator-next";
var DIRECTION_LEFT = -1;
var DIRECTION_RIGHT = 1;
export var getDateNavigator = (header, item) => {
    var items = [getPreviousButtonOptions(header), getCalendarButtonOptions(header), getNextButtonOptions(header)];
    var stylingMode = isMaterial() ? "text" : "contained";
    return _extends({
        widget: "dxButtonGroup",
        cssClass: DATE_NAVIGATOR_CLASS,
        options: {
            items: items,
            stylingMode: stylingMode,
            selectionMode: "none",
            onItemClick: e => {
                e.itemData.clickHandler(e)
            }
        }
    }, item)
};
var getPreviousButtonOptions = header => ({
    key: "previous",
    icon: "chevronprev",
    elementAttr: {
        class: PREVIOUS_BUTTON_CLASS
    },
    clickHandler: () => header._updateDateByDirection(DIRECTION_LEFT),
    onContentReady: e => {
        var previousButton = e.component;
        previousButton.option("disabled", isPreviousButtonDisabled(header));
        header._addEvent("min", () => {
            previousButton.option("disabled", isPreviousButtonDisabled(header))
        });
        header._addEvent("currentDate", () => {
            previousButton.option("disabled", isPreviousButtonDisabled(header))
        });
        header._addEvent("startViewDate", () => {
            previousButton.option("disabled", isPreviousButtonDisabled(header))
        })
    }
});
var getCalendarButtonOptions = header => ({
    key: "calendar",
    text: header.captionText,
    elementAttr: {
        class: CALENDAR_BUTTON_CLASS
    },
    clickHandler: e => header._showCalendar(e),
    onContentReady: e => {
        var calendarButton = e.component;
        header._addEvent("currentView", () => {
            calendarButton.option("text", header.captionText)
        });
        header._addEvent("currentDate", () => {
            calendarButton.option("text", header.captionText)
        });
        header._addEvent("startViewDate", () => {
            calendarButton.option("text", header.captionText)
        });
        header._addEvent("views", () => {
            calendarButton.option("text", header.captionText)
        });
        header._addEvent("firstDayOfWeek", () => {
            calendarButton.option("text", header.captionText)
        })
    }
});
var getNextButtonOptions = header => ({
    key: "next",
    icon: "chevronnext",
    elementAttr: {
        class: NEXT_BUTTON_CLASS
    },
    clickHandler: () => header._updateDateByDirection(DIRECTION_RIGHT),
    onContentReady: e => {
        var nextButton = e.component;
        nextButton.option("disabled", isNextButtonDisabled(header));
        header._addEvent("min", () => {
            nextButton.option("disabled", isNextButtonDisabled(header))
        });
        header._addEvent("currentDate", () => {
            nextButton.option("disabled", isNextButtonDisabled(header))
        });
        header._addEvent("startViewDate", () => {
            nextButton.option("disabled", isNextButtonDisabled(header))
        })
    }
});
var isPreviousButtonDisabled = header => {
    var min = header.option("min");
    if (!min) {
        return false
    }
    min = new Date(min);
    var caption = header._getCaption();
    min = trimTime(min);
    var previousDate = header._getNextDate(-1, caption.endDate);
    return previousDate < min
};
var isNextButtonDisabled = header => {
    var max = header.option("max");
    if (!max) {
        return false
    }
    max = new Date(max);
    var caption = header._getCaption();
    max = max.setHours(23, 59, 59);
    var nextDate = header._getNextDate(1, caption.startDate);
    return nextDate > max
};
