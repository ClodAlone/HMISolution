function onSchedulerLoaded(obj) {
    var thisPage = $(obj).parent();
    thisPage.localize();

    var scope = thisPage.scope();
    if (!scope)
        return;
    var paratt = thisPage.data(dataparameters);
    if (!paratt)
        return;

    var thisParent = thisPage.parent();
    $(obj).remove();

    setOpacity(scope, thisParent, thisPage);

    var bInit;
    var deregisterListener;
    var deregisterListenerViewClosed;
    var deregisterListenerActiveLanguage;
    var deregisterListenerUser;
    var cursorTimeout;
    var calendarGrid;
    var holidaysGrid;
    var bWeeklyCalendarLoaded;
    $(thisPage).bind('destroyed', function () {
        if (deregisterListener)
            deregisterListener();
        if (deregisterListenerActiveLanguage)
            deregisterListenerActiveLanguage();
        if (deregisterListenerViewClosed)
            deregisterListenerViewClosed();
        if (deregisterListenerUser)
            deregisterListenerUser();
        if (cursorTimeout)
            clearTimeout(cursorTimeout);
        if (tapped)
            clearTimeout(tapped);
        if (dblClickedTimeout)
            clearTimeout(dblClickedTimeout);
        if (longTouch)
            clearTimeout(longTouch);
    });

    var parameters = JSON.parse(paratt);
    var parentScreenName = parameters.screenName;
    var thisParentId = thisParent.attr("id");
    var schedulerStorageKey = {
        "parentScreenName": parentScreenName,
        "controlID": thisParentId,
        "propertyName": "lastSelectedScheduler"
    };
    var webKitRenderCorrection = $(thisPage).hasClass("webKitRenderCorrection");
    var biOSnoFiw = scope.biOS && !webKitRenderCorrection;
    var bBadQuality;

    var mainContainer = thisPage.find("#mainContainer");
    var selectedTabContent = thisPage.find("#selectedTabContent");
    var outerContainer = thisPage.find("#outerContainer");
    var scrollingOptions = {
        mode: webKitRenderCorrection ? "standard" : "virtual"
    };
    if (scope.biOS) {
        webkitMoveRotateTransform(thisParent, thisPage, outerContainer);
        if (webKitRenderCorrection)
            thisPage.css("will-change", "opacity");
    }
    if (scope.biOS || (isMobile.Android() && parameters.bFitInWindow)) {
        scrollingOptions["useNative"] = false;
        scrollingOptions["scrollByThumb"] = true;
    }

    if (parameters.FontFamily)
        loadCustomFonts([parameters.FontFamily]);

    var connected = false;
    var updatingScheduler = false;
    var foreignObject = thisPage.parent();
    var loadingIndicatorDiv = thisPage.find("#loadingIndicator");
    var loadingIndicatorDivContainer = thisPage.find("#loadingIndicatorContainer");

    var lblSchedulerName = thisPage.find("#lblSchedulerName");
    var lblSchedulerComboDiv = thisPage.find("#lblSchedulerCombo");

    var addCalendarItemDiv = thisPage.find("#addCalendarItem");
    var addHolidayItemDiv = thisPage.find("#addHolidayItem");
    var deleteAllHolidayItemsDiv = thisPage.find("#deleteAllHolidayItems");
    var addRefreshHolidaysDiv = thisPage.find("#addRefreshHolidays");

    var btnRefreshDiv = thisPage.find("#btnRefresh");
    var btnSaveDiv = thisPage.find("#btnSave");

    var schedulerTabsDiv = thisPage.find("#schedulerTabs");
    var schedulerChooserDiv = thisPage.find("#schedulerChooserControl");
    var weeklyViewDiv = thisPage.find("#weeklyView");
    var calendarViewDiv = thisPage.find("#calendarView");
    var holidaysViewDiv = thisPage.find("#holidaysView");
    var propertiesViewDiv = thisPage.find("#propertiesView");
    var calendarGridDiv = thisPage.find("#calendarGrid");
    var holidaysGridDiv = thisPage.find("#holidaysGrid");
    var propertiesGrid = thisPage.find("#propertiesViewGrid");
    var eventPopup = thisPage.find("#eventPopup");
    var eventChoicePopup = thisPage.find("#eventChoicePopup");
    var eventEditForm;
    var contextMenuEdit = thisPage.find("#context-menu-edit");
    var weeklyContextMenu;
    var rightClickedCell;
    var tapped = false;
    var longTouch;
    var dblClickedTimeout = false;
    var gridsTopPadding = 0;

    var editingCalendarRowItem = {};
    var editingCalendarRowDayOfMonth;
    var editingHolidaysRowItem = {};
    var editingHolidaysRowDayOfMonth;
    var timezoneOffset = (new Date()).getTimezoneOffset() * 60 * 1000;
    var actYear = new Date().getFullYear();
    var currentScheduler;
    var bPendingSchedulerLoading;
    var calendarDayOfMonthEditorSelectBox;
    var editingCalendarRowMonth;
    var holidaysDayOfMonthEditorSelectBox;
    var editingHolidaysRowMonth;
    var isoLang = getBrowserLangISO3166();
    var bDirty = false;
    var editFormStartDayCombo;
    var editFormEndDayCombo;
    var editFormStartTime;
    var editFormEndTime;
    var repeatSwitch;
    var bAddingAppointmentsSerie;
    var bWeeklyViewInit;
    var weeklyCellMinutes = 15;
    var hourCells = 60 / weeklyCellMinutes;
    var weeklyCellMs = 60000 * weeklyCellMinutes;
    var dateStringOptions = { weekday: 'short', hour: '2-digit', minute: '2-digit', second: '2-digit' };
    var daysFullNames = [
        "Sunday",
        "Monday",
        "Tuesday",
        "Wednesday",
        "Thursday",
        "Friday",
        "Saturday"
    ];
    var currentWeeklyEvents = [];
    var mapColindex = [];
    var activeTooltips = 0;
    
    var timeCursorColor = parameters.SelectionColor ? parameters.SelectionColor.Color : "#F27E13";
    var currentDayColor = timeCursorColor;
    var eventBackgroundColor = timeCursorColor;
    var multiEventBackgroundColor = parameters.MultiSelectionColor ? parameters.MultiSelectionColor.Color : eventBackgroundColor;
    var rowCells = 24 * hourCells;
    var startH = 0;
    var startM = 0;
    var startM_quarterDiff = 0;
    var endM = 0;
    var finalEndM = endM;
    var firstRowCells = 25;

    var weekMondayM = moment().clone().isoWeekday(1);
    var weekMonday = {
        day: weekMondayM.date(),
        month: weekMondayM.month(),
        year: weekMondayM.year()
    };
    var weekSundayM = moment().clone().isoWeekday(7);
    var weekSunday = {
        day: weekSundayM.date(),
        month: weekSundayM.month(),
        year: weekSundayM.year()
    };

    if (parameters.ShowWorkTimeOnly) {
        var startRe = parameters.WorkViewStartTime.match(/PT.*?(\d+)H((\d+)M)?.*?/);
        startH = parseInt(startRe[1], 10);
        if (!undefinedOrNull(startRe[2]))
            startM = parseInt(startRe[2], 10);
        startM_quarterDiff = startM % weeklyCellMinutes;
        startM = Math.floor(startM / weeklyCellMinutes) * weeklyCellMinutes;
        var endRe = parameters.WorkViewEndTime.match(/PT.*?(\d+)H((\d+)M)?.*?/);
        var endH = parseInt(endRe[1], 10);
        var finalEndH = endH;
        if (!undefinedOrNull(endRe[2])) {
            endM = parseInt(endRe[2], 10);
            finalEndM = Math.ceil(endM / weeklyCellMinutes) * weeklyCellMinutes;
            if (finalEndM == 60) {
                if (endH < 23) {
                    finalEndH = finalEndH + 1;
                    finalEndM = 0;
                }
                else
                    finalEndM = 59;
            }
        }
        var weeklyViewEndDate = new Date(weekMonday.year, weekMonday.month, weekMonday.day, finalEndH, finalEndM, 0);
        rowCells = ((finalEndH - startH) * hourCells) + finalEndM / weeklyCellMinutes - startM / weeklyCellMinutes;
        firstRowCells = finalEndH - startH + 1; //Math.ceil(rowCells / hourCells) + 1;
        if (finalEndM > 0)
            firstRowCells++;
    }
    var weeklyViewStartDate = new Date(weekMonday.year, weekMonday.month, weekMonday.day, startH, startM, 0);
    var weeklyRowCells = rowCells + 1; //97
    var firstRow_firstColspan = (60 - startM) / weeklyCellMinutes;
    var firstRow_middleColspan = hourCells;
    var firstRow_endColspan = finalEndM / weeklyCellMinutes == 0 ? 4 : finalEndM / weeklyCellMinutes;

    var weeklyTotalCells = firstRowCells + weeklyRowCells * 7;
    var doubleClickDelay = 250;
    var longTouchDelay = 1000;
    var bIgnoreNextTouchEnd;

    configureTheme();

    var translations = scope.setTranslations("SchedulerControl", [
        {
            id: "SchedulerLabel",
            stringSuffix: "SchedulerName",
            defValue: "Scheduler"
        },
        {
            id: "AddItem",
            stringSuffix: "AddCalendarItem",
            defValue: "Add Calendar Item"
        },
        {
            id: "AddHolidays",
            stringSuffix: "AddDefaultHolidays",
            defValue: "Add/Update Holidays"
        },
        {
            id: "DeleteItems",
            stringSuffix: "DeleteCalendarItems",
            defValue: "Delete All Items"
        },
        {
            id: "AddNewWeekly",
            stringSuffix: "AddNewWeeklyItem",
            defValue: "Add New Weekly Item"
        },
        {
            id: "DeleteWeekly",
            stringSuffix: "DeleteWeeklyPlanItem",
            defValue: "Delete Weekly Item"
        },
        {
            id: "EditWeekly",
            stringSuffix: "EditWeeklyPlanItem",
            defValue: "Edit Weekly Item"
        },
        {
            id: "WeeklyTab",
            stringSuffix: "NewEventWeeklyPlan",
            defValue: "Weekly Plan"
        },
        {
            id: "CalendarTab",
            stringSuffix: "Calendar",
            defValue: "Calendar"
        },
        {
            id: "HolidaysTab",
            stringSuffix: "NewEventExceptions",
            defValue: "Holidays"
        },
        {
            id: "PropertiesTab",
            stringSuffix: "NewEventTitle",
            defValue: "Scheduler Properties"
        },

        {
            id: "StartDate",
            stringSuffix: "StartDate",
            defValue: "Start Date"
        },
        {
            id: "EndDate",
            stringSuffix: "EndDate",
            defValue: "End Date"
        },
        {
            id: "Type",
            stringSuffix: "Type",
            defValue: "Item Type"
        },
        {
            id: "Month",
            stringSuffix: "Month",
            defValue: "Month"
        },
        {
            id: "WeekOfMonth",
            stringSuffix: "WeekOfMonth",
            defValue: "Week Of Month"
        },
        {
            id: "DayOfWeek",
            stringSuffix: "DayOfWeek",
            defValue: "Day Of Week"
        },
        {
            id: "DayOfMonth",
            stringSuffix: "DayOfMonth",
            defValue: "Day Of Month"
        },
        {
            id: "CalendarItemType",
            stringSuffix: "CalendarItemType",
            defValue: "Item Type"
        },
        {
            id: "SingleDate",
            stringSuffix: "SingleDate",
            defValue: "Single Date",
            customPrefix: "CalendarItemType_"
        },
        {
            id: "DateRange",
            stringSuffix: "DateRange",
            defValue: "Date Range",
            customPrefix: "CalendarItemType_"
        },
        {
            id: "WeekNDate",
            stringSuffix: "WeekNDate",
            defValue: "Week N Date",
            customPrefix: "CalendarItemType_"
        },
        {
            id: "MonthDayAny_First",
            stringSuffix: "MonthDayAny_First",
            defValue: "First"
        },
        {
            id: "MonthDayAny_NoDate",
            stringSuffix: "MonthDayAny_NoDate",
            defValue: "NoDate"
        },
        {
            id: "MonthDayAny_Last",
            stringSuffix: "MonthDayAny_Last",
            defValue: "Last"
        },
        //CalendarMonthType
        {
            id: "MonthAny",
            stringSuffix: "Any",
            defValue: "Any",
            customPrefix: "CalendarMonthType_"
        },
        {
            id: "MonthOdd",
            stringSuffix: "Odd",
            defValue: "Odd",
            customPrefix: "CalendarMonthType_"
        },
        {
            id: "MonthEven",
            stringSuffix: "Even",
            defValue: "Even",
            customPrefix: "CalendarMonthType_"
        },
        {
            id: "MonthJanuary",
            stringSuffix: "January",
            defValue: "January",
            customPrefix: "CalendarMonthType_"
        },
        {
            id: "MonthFebruary",
            stringSuffix: "February",
            defValue: "February",
            customPrefix: "CalendarMonthType_"
        },
        {
            id: "MonthMarch",
            stringSuffix: "March",
            defValue: "March",
            customPrefix: "CalendarMonthType_"
        },
        {
            id: "MonthApril",
            stringSuffix: "April",
            defValue: "April",
            customPrefix: "CalendarMonthType_"
        },
        {
            id: "MonthMay",
            stringSuffix: "May",
            defValue: "May",
            customPrefix: "CalendarMonthType_"
        },
        {
            id: "MonthJune",
            stringSuffix: "June",
            defValue: "June",
            customPrefix: "CalendarMonthType_"
        },
        {
            id: "MonthJuly",
            stringSuffix: "July",
            defValue: "July",
            customPrefix: "CalendarMonthType_"
        },
        {
            id: "MonthAugust",
            stringSuffix: "August",
            defValue: "August",
            customPrefix: "CalendarMonthType_"
        },
        {
            id: "MonthSeptember",
            stringSuffix: "September",
            defValue: "September",
            customPrefix: "CalendarMonthType_"
        },
        {
            id: "MonthOctober",
            stringSuffix: "October",
            defValue: "October",
            customPrefix: "CalendarMonthType_"
        },
        {
            id: "MonthNovember",
            stringSuffix: "November",
            defValue: "November",
            customPrefix: "CalendarMonthType_"
        },
        {
            id: "MonthDecember",
            stringSuffix: "December",
            defValue: "December",
            customPrefix: "CalendarMonthType_"
        },
        //CalendarWeekType
        {
            id: "WeekAny",
            stringSuffix: "Any",
            defValue: "Any",
            customPrefix: "CalendarWeekType_"
        },
        {
            id: "WeekLast",
            stringSuffix: "Last",
            defValue: "Last",
            customPrefix: "CalendarWeekType_"
        },
        {
            id: "WeekFirst",
            stringSuffix: "First",
            defValue: "First",
            customPrefix: "CalendarWeekType_"
        },
        {
            id: "WeekSecond",
            stringSuffix: "Second",
            defValue: "Second",
            customPrefix: "CalendarWeekType_"
        },
        {
            id: "WeekThird",
            stringSuffix: "Third",
            defValue: "Third",
            customPrefix: "CalendarWeekType_"
        },
        {
            id: "WeekFourth",
            stringSuffix: "Fourth",
            defValue: "Fourth",
            customPrefix: "CalendarWeekType_"
        },
        {
            id: "WeekFifth",
            stringSuffix: "Fifth",
            defValue: "Fifth",
            customPrefix: "CalendarWeekType_"
        },
        //CalendarDayType
        {
            id: "DayAny",
            stringSuffix: "Any",
            defValue: "Any",
            customPrefix: "CalendarDayType_"
        },
        {
            id: "DaySunday",
            stringSuffix: "Sunday",
            defValue: "Sunday",
            customPrefix: "CalendarDayType_"
        },
        {
            id: "DayMonday",
            stringSuffix: "Monday",
            defValue: "Monday",
            customPrefix: "CalendarDayType_"
        },
        {
            id: "DayTuesday",
            stringSuffix: "Tuesday",
            defValue: "Tuesday",
            customPrefix: "CalendarDayType_"
        },
        {
            id: "DayWednesday",
            stringSuffix: "Wednesday",
            defValue: "Wednesday",
            customPrefix: "CalendarDayType_"
        },
        {
            id: "DayThursday",
            stringSuffix: "Thursday",
            defValue: "Thursday",
            customPrefix: "CalendarDayType_"
        },
        {
            id: "DayFriday",
            stringSuffix: "Friday",
            defValue: "Friday",
            customPrefix: "CalendarDayType_"
        },
        {
            id: "DaySaturday",
            stringSuffix: "Saturday",
            defValue: "Saturday",
            customPrefix: "CalendarDayType_"
        },

        {
            id: "WeekSunday",
            stringSuffix: "Sunday",
            defValue: "Sunday",
            customPrefix: "CDayOfWeek_"
        },
        {
            id: "WeekMonday",
            stringSuffix: "Monday",
            defValue: "Monday",
            customPrefix: "CDayOfWeek_"
        },
        {
            id: "WeekTuesday",
            stringSuffix: "Tuesday",
            defValue: "Tuesday",
            customPrefix: "CDayOfWeek_"
        },
        {
            id: "WeekWednesday",
            stringSuffix: "Wednesday",
            defValue: "Wednesday",
            customPrefix: "CDayOfWeek_"
        },
        {
            id: "WeekThursday",
            stringSuffix: "Thursday",
            defValue: "Thursday",
            customPrefix: "CDayOfWeek_"
        },
        {
            id: "WeekFriday",
            stringSuffix: "Friday",
            defValue: "Friday",
            customPrefix: "CDayOfWeek_"
        },
        {
            id: "WeekSaturday",
            stringSuffix: "Saturday",
            defValue: "Saturday",
            customPrefix: "CDayOfWeek_"
        },
        {
            id: "WeeklyItemAllDayEvent",
            stringSuffix: "WeeklyItemAllDayEvent",
            defValue: "All Day"
        },
        {
            id: "WeeklyItemEditFormStartTime",
            stringSuffix: "WeeklyItemEditFormStartTime",
            defValue: "Start Date"
        },
        {
            id: "WeeklyItemEditFormEndTime",
            stringSuffix: "WeeklyItemEditFormEndTime",
            defValue: "End Date"
        },
        {
            id: "WeeklyItemApplyRecurrence",
            stringSuffix: "WeeklyItemApplyRecurrence",
            defValue: "Apply Recurrence"
        },
        {
            id: "RecurrenceFiledEvery",
            stringSuffix: "RecurrenceFiledEvery",
            defValue: "Every"
        },
        {
            id: "RecurrenceFiledEveryWeekDays",
            stringSuffix: "RecurrenceFiledEveryWeekDays",
            defValue: "Every weekday"
        },
        {
            id: "RecurrenceFiledDays",
            stringSuffix: "RecurrenceFiledDays",
            defValue: "Days"
        },
        {
            id: "WeeklyItemEditFormOk",
            stringSuffix: "WeeklyItemEditFormOk",
            defValue: "OK"
        },
        {
            id: "WeeklyItemEditFormCancel",
            stringSuffix: "WeeklyItemEditFormCancel",
            defValue: "Cancel"
        },
        {
            id: "WeeklyItemEditFormDelete",
            stringSuffix: "WeeklyItemEditFormDelete",
            defValue: "Delete"
        },
        //end forms
        {
            id: "ReloadData",
            stringSuffix: "ReloadData",
            defValue: "Reload Data"
        },
        {
            id: "SchedulerSave",
            stringSuffix: "SchedulerSave",
            defValue: "Save Data"
        },
        //Properties tab
        { id: "NewEventName", stringSuffix: "NewEventName", defValue: "Name" },
        { id: "NewEventType", stringSuffix: "NewEventType", defValue: "Type" },
        { id: "Time", stringSuffix: "Time", defValue: "Time On" },
        { id: "TimeOff", stringSuffix: "TimeOff", defValue: "Time Off" },
        { id: "NewEventTag", stringSuffix: "NewEventTag", defValue: "Tag" },
        { id: "NewEventValueOn", stringSuffix: "NewEventValueOn", defValue: "Value for event On" },
        { id: "NewEventValueOff", stringSuffix: "NewEventValueOff", defValue: "Value for event Off" },
        { id: "NewEventEnableTag", stringSuffix: "NewEventEnableTag", defValue: "Enable Variable" },
        { id: "ExecOnAtStartup", stringSuffix: "ExecOnAtStartup", defValue: "Execute ON at startup" },
        { id: "ExecOffAtStartup", stringSuffix: "ExecOffAtStartup", defValue: "Execute OFF at startup" },
        { id: "NewEventAccessRole", stringSuffix: "NewEventAccessRole", defValue: "Access Role" },
        { id: "NewEventAccessLevel", stringSuffix: "NewEventAccessLevel", defValue: "Access Level" },
        { id: "NewEventAccessMask", stringSuffix: "NewEventAccessMask", defValue: "Access Mask" },
        //NewEventType values
        { id: "EveryDay", stringSuffix: "EveryDay", defValue: "EveryDay" },
        { id: "EveryHour", stringSuffix: "EveryHour", defValue: "EveryHour" },
        { id: "EveryMinute", stringSuffix: "EveryMinute", defValue: "EveryMinute" },
        { id: "EverySunday", stringSuffix: "EverySunday", defValue: "EverySunday" },
        { id: "EveryMonday", stringSuffix: "EveryMonday", defValue: "EveryMonday" },
        { id: "EveryTuesday", stringSuffix: "EveryTuesday", defValue: "EveryTuesday" },
        { id: "EveryWednesday", stringSuffix: "EveryWednesday", defValue: "EveryWednesday" },
        { id: "EveryThursday", stringSuffix: "EveryThursday", defValue: "EveryThursday" },
        { id: "EveryFriday", stringSuffix: "EveryFriday", defValue: "EveryFriday" },
        { id: "EverySaturday", stringSuffix: "EverySaturday", defValue: "EverySaturday" },
        { id: "NewEventCalendar", stringSuffix: "NewEventCalendar", defValue: "Calendar" },
        { id: "WeeklyPlan", stringSuffix: "WeeklyPlan", defValue: "WeeklyPlan" },
    ]);
    var translatedObjectsMap = [];

    var fontOptions = {
        "font-family": parameters.FontFamily,
        "font-size": parameters.FontSize + "px",
        "font-weight": fontWeightConverter(parameters.FontWeight),
        "color": parameters.Foreground.Color
    };
    var inlineFontOptions = "";
    for(k in fontOptions) {
        inlineFontOptions += k + ":" + fontOptions[k] + ";";
    }
    var toolbarFontOptions = {
        "font-family": parameters.FontFamily,
        "font-size": parameters.FontSize + "px",
        "font-weight": fontWeightConverter(parameters.FontWeight),
        "color": parameters.ToolbarForeground.Color
    };
    
    var thisGridSelector = "#" + thisParentId + ".SchedulerControl";
    var thisGridID = thisParentId + parameters.screenId;
    var oddRowsBackground = scope.theme.isLight ? "rgba(255, 255, 255, 0.5)" : "rgba(0, 0, 0, 0.2)";

    var gridUntranslatedColumns = [
        {
            name: "StartDate",
            dataField: "StartDate",
            /*validationRules: [{ type: "required", message: i18next.t1('EndDateMustBeGreater') }],*/
            caption: translations.StartDate(),
            cellTemplate: function (container, options) {
                var opts = {
                    disabled: true,
                    value: options.value,
                    showClearButton: false,
                    useMaskBehavior: true
                }
                var editorOptions = {
                    showClearButton: false,
                    useMaskBehavior: true
                };
                if (options.data.ItemType == "WeekNDate") {
                    opts.displayFormat = "HH:mm:ss";
                    opts.type = "time";
                    editorOptions.displayFormat = "HH:mm:ss";
                    editorOptions.type = "time";
                }
                else {
                    opts.type = "datetime";
                    editorOptions.type = "datetime";
                }
                $("<div class='startdate'/>").dxDateBox(opts).appendTo(container);
            },
            formItem: {
                editorType: "dxDateBox",
                editorOptions: {
                    showClearButton: false,
                    useMaskBehavior: true,
                    type: "datetime"
                },
                visibleIndex: 2
            },
            //setCellValue: function (newData, value) {
            //    this.defaultSetCellValue(newData, value);
            //}
        },
        {
            name: "EndDate",
            dataField: "EndDate",
            /*validationRules: [{ type: "required", message: i18next.t1('EndDateMustBeGreater') }],*/
            caption: translations.EndDate(),
            cellTemplate: function (container, options) {
                if (options.data.ItemType == "SingleDate") {
                    $("<div style='text-align:center;'>-</div>").appendTo(container);
                    return;
                }
                var opts = {
                    disabled: true,
                    value: options.value,
                    showClearButton: false,
                    useMaskBehavior: true
                }
                var editorOptions = {
                    showClearButton: false,
                    useMaskBehavior: true
                };
                if (options.data.ItemType == "WeekNDate") {
                    opts.displayFormat = "HH:mm:ss";
                    opts.type = "time";
                    editorOptions.displayFormat = "HH:mm:ss";
                    editorOptions.type = "time";
                }
                else {
                    opts.type = "datetime";
                    editorOptions.type = "datetime";
                }
                $("<div class='enddate'/>").dxDateBox(opts).appendTo(container);
            },
            formItem: {
                editorType: "dxDateBox",
                editorOptions: {
                    showClearButton: false,
                    useMaskBehavior: true,
                    type: "datetime"
                },
                visibleIndex: 3
            }
        },
        {
            name: "Type",
            dataField: "ItemType",
            /*validationRules: [{ type: "required" }],*/
            caption: translations.CalendarItemType(),
            formItem: {
                editorType: "dxSelectBox",
                editorOptions: {
                    items: ["SingleDate", "DateRange", "WeekNDate"],
                    displayExpr: function (item) {
                        if (undefinedOrNull(item))
                            return translations.SingleDate();
                        if (item in translations)
                            return translations[item]();
                        return item;
                    },
                },
                colSpan: 2,
                visibleIndex: 0
            },
            calculateDisplayValue: function (rowData) {
                return rowData.ItemType in translations ? translations[rowData.ItemType]() : rowData.ItemType;
            }
            //setCellValue: function (newData, value) {
            //    this.defaultSetCellValue(newData, value);
            //}
        },
        {
            name: "Month",
            dataField: "Month",
            caption: translations.Month(),
            cellTemplate: function (container, options) {
                if (options.data.ItemType != "WeekNDate") {
                    $("<div style='text-align:center;'>-</div>").appendTo(container);
                    return;
                }
                var cellValue = options.value;
                if ("Month" + cellValue in translations)
                    cellValue = translations["Month" + cellValue]();
                $("<div/>").text(cellValue).appendTo(container);
            },
            formItem: {
                editorType: "dxSelectBox",
                editorOptions: {
                    items: ["Any", "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December", "Odd", "Even"],
                    displayExpr: function (item) {
                        if (undefinedOrNull(item))
                            return translations.MonthAny();
                        if ("Month" + item in translations)
                            return translations["Month" + item]();
                        return item;
                    }
                },
                visibleIndex: 5
            }
        },
        {
            name: "WeekOfMonth",
            dataField: "WeekOfMonth",
            caption: translations.WeekOfMonth(),
            cellTemplate: function (container, options) {
                if (options.data.ItemType != "WeekNDate" || options.data.DayOfMonth != "NoDate") {
                    $("<div style='text-align:center;'>-</div>").appendTo(container);
                    return;
                }
                var cellValue = options.value;
                if ("Week" + cellValue in translations)
                    cellValue = translations["Week" + cellValue]();
                $("<div/>").text(cellValue).appendTo(container);
            },
            formItem: {
                editorType: "dxSelectBox",
                editorOptions: {
                    items: ["First", "Second", "Third", "Fourth", "Fifth", "Last", "Any"],
                    displayExpr: function (item) {
                        if (undefinedOrNull(item))
                            return translations.WeekAny();
                        if ("Week" + item in translations)
                            return translations["Week" + item]();
                        return item;
                    }
                },
                visibleIndex: 6
            }
        },
        {
            name: "DayOfWeek",
            dataField: "DayOfWeek",
            caption: translations.DayOfWeek(),
            cellTemplate: function (container, options) {
                if (options.data.ItemType != "WeekNDate" || options.data.DayOfMonth != "NoDate") {
                    $("<div style='text-align:center;'>-</div>").appendTo(container);
                    return;
                }
                var cellValue = options.value;
                if ("Day" + cellValue in translations)
                    cellValue = translations["Day" + cellValue]();
                $("<div/>").text(cellValue).appendTo(container);
            },
            formItem: {
                editorType: "dxSelectBox",
                editorOptions: {
                    items: daysFullNames.concat(["Any"]),
                    displayExpr: function (item) {
                        if (undefinedOrNull(item))
                            return translations.DayAny();
                        if ("Day" + item in translations)
                            return translations["Day" + item]();
                        return item;
                    }
                },
                visibleIndex: 7
            }
        },
        {
            name: "DayOfMonth",
            dataField: "DayOfMonth",
            caption: translations.DayOfMonth(),
            cellTemplate: function (container, options) {
                if (options.data.ItemType != "WeekNDate") {
                    $("<div style='text-align:center;'>-</div>").appendTo(container);
                    return;
                }
                var cellValue = options.value;
                if ("MonthDayAny_" + cellValue in translations)
                    cellValue = translations["MonthDayAny_" + cellValue]();
                $("<div/>").text(cellValue).appendTo(container);
            },
            formItem: {
                editorType: "dxSelectBox",
                editorOptions: {
                    items: ["NoDate", "First", "Last"].concat((function () { var n = []; for (var i = 1; i <= 31; i++) n.push(i); return n; })()),
                    displayExpr: function (item) {
                        if (undefinedOrNull(item)) 
                            return translations.MonthDayAny_NoDate();
                        if ("MonthDayAny_" + item in translations)
                            return translations["MonthDayAny_" + item]();
                        return item;
                    }
                },
                visibleIndex: 4
            }
        }
    ];

    var calendarGridOptions = {
        disabled: thisParent.hasClass(disabledClass),
        columnResizingMode: "widget",
        selection: {
            mode: "single"
        },
        scrolling: scrollingOptions,
        paging: {
            pageSize: 50,
        },
        loadPanel: {
            enabled: true
        },
        //width: "100%",
        columnAutoWidth: true,
        allowColumnReordering: false,
        allowColumnResizing: true,
        noDataText: noDataText,
        columns: gridUntranslatedColumns,
        onContentReady: function (e) {
            var el = e.component.element();
            el.find(".dx-header-row td[role=columnheader]").css("background", parameters.ToolbarBackground.Color);
            el.find(".dx-toolbar").css("background", parameters.Background.Color);
            el.find(".dx-datagrid").css("background", parameters.Background.Color);
            if (parameters.Background)
                $(el.find(".dx-datagrid-rowsview")[0]).css("background", parameters.Background.Color);
        },
        onInitNewRow: function (e) {
            var date = Date.now();
            e.data.StartDate = date;
            e.data.EndDate = date;
            e.data.ItemType = editingCalendarRowItem.ItemType = gridUntranslatedColumns[2].formItem.editorOptions.items[0];
            e.data.Month = gridUntranslatedColumns[3].formItem.editorOptions.items[0];
            e.data.WeekOfMonth = gridUntranslatedColumns[4].formItem.editorOptions.items[0];
            e.data.DayOfWeek = gridUntranslatedColumns[5].formItem.editorOptions.items[0];
            e.data.DayOfMonth = gridUntranslatedColumns[6].formItem.editorOptions.items[0];
        },
        onEditorPreparing: function (e) {
            if (e.dataField == "ItemType") {
                //if (e.value == null) {
                //    e.editorOptions.value = e.editorOptions.items[0];
                //    //calendarGrid.cellValue(e.row.rowIndex, "ItemType", e.editorOptions.value);
                //}
                e.editorOptions.onValueChanged = function (data) {
                    editingCalendarRowItem.ItemType = data.value;
                    calendarGrid.cellValue(e.row.rowIndex, "ItemType", data.value);
                    //calendarGrid.saveEditData();
                }
            }
            else if (e.dataField == "DayOfMonth") {
                e.editorOptions.onInitialized = function (data) {
                    calendarDayOfMonthEditorSelectBox = data.component;
                    editingCalendarRowDayOfMonth = data.component.option('value');
                };
                e.editorOptions.onValueChanged = function (data) {
                    editingCalendarRowDayOfMonth = data.value;
                    calendarGrid.cellValue(e.row.rowIndex, "DayOfMonth", data.value);
                };
            }
            else if (e.dataField == "Month") {
                e.editorOptions.onValueChanged = function (data) {
                    editingCalendarRowMonth = data.value;
                    calendarGrid.cellValue(e.row.rowIndex, "Month", data.value);
                }
            }
            else if (e.dataField == "WeekOfMonth" || e.dataField == "DayOfWeek") {
                e.editorOptions.onInitialized = function (data) {
                    data.component.option("disabled", editingCalendarRowDayOfMonth != "NoDate");
                }
            }
        },
        onCellPrepared: function (e) {
            if (e.rowType === "data" && e.column.command === "edit") {
                var $links = e.cellElement.find(".dx-link");
                var isEditing = e.row.isEditing;

                $links.text("");

                if (!isEditing) {
                    $links.filter(".dx-link-edit").dxButton({
                        icon: "edit",
                        hint: i18next.t1("Edit")
                    });

                    $links.filter(".dx-link-delete").dxButton({
                        icon: "trash",
                        hint: i18next.t1("Delete")
                    });
                }
            }
        },
        onRowValidating: function (e) {
            var itemType = e.newData.ItemType ? e.newData.ItemType : e.oldData.ItemType;
            if (itemType == "SingleDate")
                e.isValid = e.newData.StartDate || e.oldData.StartDate;
            else {
                if (!e.newData.StartDate && !e.newData.EndDate)
                    e.isValid = true;
                else {
                    var startDate = e.newData.StartDate ? e.newData.StartDate : e.oldData.StartDate;
                    var endDate = e.newData.EndDate ? e.newData.EndDate : e.oldData.EndDate;
                    e.isValid = startDate < endDate;
                }
            }
            e.errorText = i18next.t1('EndDateMustBeGreater');
        },
        onSaved: function (e) {
            bDirty = true;
        },
        editing: {
            mode: "form",
            form: {
                customizeItem: function (item) {
                    if (item.dataField == "EndDate" && editingCalendarRowItem.ItemType == "SingleDate") {
                        item.visible = false;
                    }
                    else if (item.dataField == "StartDate" || item.dataField == "EndDate") {
                        item.editorOptions.type = editingCalendarRowItem.ItemType == "WeekNDate" ? "time" : "datetime";
                        item.editorOptions.displayFormat = editingCalendarRowItem.ItemType == "WeekNDate" ? "HH:mm:ss" : null;

                        if (scope.bIsMobile) {
                            item.editorOptions.onContentReady = function (e) { $(e.component.element()).find(".dx-texteditor-input").attr("readonly", true); }
                            item.editorOptions.pickerType = item.editorOptions.type == "time" ? "rollers" : "calendar";
                            item.editorOptions.onOpened = function (e) {
                                e.component._popup.option("position", {
                                    my: "center",
                                    at: "center",
                                    of: window
                                });
                            };
                        }

                        if (item.dataField == "StartDate")
                            item.colSpan = editingCalendarRowItem.ItemType == "SingleDate" ? 2 : 1;
                    }
                    if (editingCalendarRowItem.ItemType != "WeekNDate" && (item.dataField == "Month" || item.dataField == "WeekOfMonth" || item.dataField == "DayOfWeek" || item.dataField == "DayOfMonth")) {
                        item.visible = false;
                    }
                    if (item.dataField == "DayOfMonth") {
                        var staticItems = item.editorOptions.items.slice(0, 3);
                        var daysNumbers = [];
                        var totalDays = 31;
                        var selectedMonth = moment.months().indexOf(editingCalendarRowMonth ? editingCalendarRowMonth : editingCalendarRowItem.Month);
                        if (selectedMonth !== -1) {
                            totalDays = new Date(new Date().getFullYear(), selectedMonth + 1, 0).getDate();
                        }
                        for (var i = 1; i <= totalDays; i++)
                            daysNumbers.push(i);
                        item.editorOptions.items = staticItems.concat(daysNumbers);
                        if (calendarDayOfMonthEditorSelectBox)
                            calendarDayOfMonthEditorSelectBox.option("items", staticItems.concat(daysNumbers));
                    }
                },
                onContentReady: function (e) {
                    setTimeout(function () {
                        let form = e.component.element()[0];
                        let btnSave = form.parentElement.parentElement.querySelector("[aria-label='Save']");
                        let btnCancel = form.parentElement.parentElement.querySelector("[aria-label='Cancel']");
                        btnCancel.parentNode.insertBefore(btnSave, btnCancel);
                        let saveButton = $(btnSave).dxButton("instance");
                        let cancelButton = $(btnCancel).dxButton("instance");
                        saveButton.option({
                            "icon": "save",
                            "text": ""
                        });
                        cancelButton.option({
                            "icon": "undo",
                            "text": ""
                        });
                    }, 1);
                }
                //onInitialized: function (e) {
                //    calendarEditFormInstance = e.component;
                //}
            },
            allowUpdating: true,
            allowDeleting: true,
            allowAdding: true
        },
        onEditingStart: function (e) {
            editingCalendarRowItem = e.data;
        }
    };

    var holidaysOptions = {
        disabled: thisParent.hasClass(disabledClass),
        columnResizingMode: "widget",
        selection: {
            mode: "single"
        },
        scrolling: scrollingOptions,
        paging: {
            pageSize: 50,
        },
        loadPanel: {
            enabled: true
        },
        allowColumnReordering: false,
        allowColumnResizing: true,
        noDataText: noDataText,
        allowColumnResizing: true,
        columnAutoWidth: true,
        columns: gridUntranslatedColumns,
        onContentReady: function (e) {
            var el = e.component.element();
            el.find(".dx-header-row td[role=columnheader]").css("background", parameters.ToolbarBackground.Color);
            el.find(".dx-toolbar").css("background", parameters.Background.Color);
            el.find(".dx-datagrid").css("background", parameters.Background.Color);
            if (parameters.Background)
                $(el.find(".dx-datagrid-rowsview")[0]).css("background", parameters.Background.Color);
        },
        onInitNewRow: function (e) {
            var date = Date.now();
            e.data.StartDate = date;
            e.data.EndDate = date;
            e.data.ItemType = editingHolidaysRowItem.ItemType = gridUntranslatedColumns[2].formItem.editorOptions.items[0];
            e.data.Month = gridUntranslatedColumns[3].formItem.editorOptions.items[0];
            e.data.WeekOfMonth = gridUntranslatedColumns[4].formItem.editorOptions.items[0];
            e.data.DayOfWeek = gridUntranslatedColumns[5].formItem.editorOptions.items[0];
            e.data.DayOfMonth = gridUntranslatedColumns[6].formItem.editorOptions.items[0];
        },
        onEditorPreparing: function (e) {
            if (e.dataField == "ItemType") {
                e.editorOptions.onValueChanged = function (data) {
                    editingHolidaysRowItem.ItemType = data.value;
                    holidaysGrid.cellValue(e.row.rowIndex, "ItemType", data.value);
                    //holidaysGrid.saveEditData();
                }
            }
            else if (e.dataField == "DayOfMonth") {
                e.editorOptions.onInitialized = function (data) {
                    holidaysDayOfMonthEditorSelectBox = data.component;
                    editingHolidaysRowDayOfMonth = data.component.option('value');
                };
                e.editorOptions.onValueChanged = function (data) {
                    editingHolidaysRowDayOfMonth = data.value;
                    holidaysGrid.cellValue(e.row.rowIndex, "DayOfMonth", data.value);
                };
            }
            else if (e.dataField == "Month") {
                e.editorOptions.onValueChanged = function (data) {
                    editingHolidaysRowMonth = data.value;
                    holidaysGrid.cellValue(e.row.rowIndex, "Month", data.value);
                }
            }
            else if (e.dataField == "WeekOfMonth" || e.dataField == "DayOfWeek") {
                e.editorOptions.onInitialized = function (data) {
                    data.component.option("disabled", editingHolidaysRowDayOfMonth != "NoDate");
                }
            }
        },
        onCellPrepared: function (e) {
            if (e.rowType === "data" && e.column.command === "edit" && !e.row.isEditing) {
                var $links = e.cellElement.find(".dx-link");
                $links.text("");
                $links.filter(".dx-link-edit").dxButton({
                    icon: "edit",
                    hint: i18next.t1("Edit")
                });

                $links.filter(".dx-link-delete").dxButton({
                    icon: "trash",
                    hint: i18next.t1("Delete")
                });
            }
        },
        onRowValidating: function (e) {
            var itemType = e.newData.ItemType ? e.newData.ItemType : e.oldData.ItemType;
            if (itemType == "SingleDate")
                e.isValid = e.newData.StartDate || e.oldData.StartDate;
            else {
                if (!e.newData.StartDate && !e.newData.EndDate)
                    e.isValid = true;
                else {
                    var startDate = e.newData.StartDate ? e.newData.StartDate : e.oldData.StartDate;
                    var endDate = e.newData.EndDate ? e.newData.EndDate : e.oldData.EndDate;
                    e.isValid = startDate < endDate;
                }
            }
            e.errorText = i18next.t1('EndDateMustBeGreater');
        },
        onSaved: function (e) {
            bDirty = true;
        },
        editing: {
            mode: "form",
            form: {
                customizeItem: function (item) {
                    if (item.dataField == "EndDate" && editingHolidaysRowItem.ItemType == "SingleDate") {
                        item.visible = false;
                    }
                    else if (item.dataField == "StartDate" || item.dataField == "EndDate") {
                        item.editorOptions.type = editingHolidaysRowItem.ItemType == "WeekNDate" ? "time" : "datetime";
                        item.editorOptions.displayFormat = editingHolidaysRowItem.ItemType == "WeekNDate" ? "HH:mm:ss" : null;

                        if (scope.bIsMobile) {
                            item.editorOptions.onContentReady = function (e) { $(e.component.element()).find(".dx-texteditor-input").attr("readonly", true); }
                            item.editorOptions.pickerType = item.editorOptions.type == "time" ? "rollers" : "calendar";
                            item.editorOptions.onOpened = function (e) {
                                e.component._popup.option("position", {
                                    my: "center",
                                    at: "center",
                                    of: window
                                });
                            };
                        }

                        if (item.dataField == "StartDate")
                            item.colSpan = editingHolidaysRowItem.ItemType == "SingleDate" ? 2 : 1;
                    }
                    if (editingHolidaysRowItem.ItemType != "WeekNDate" && (item.dataField == "Month" || item.dataField == "WeekOfMonth" || item.dataField == "DayOfWeek" || item.dataField == "DayOfMonth")) {
                        item.visible = false;
                    }
                    if (item.dataField == "DayOfMonth") {
                        var staticItems = item.editorOptions.items.slice(0, 3);
                        var daysNumbers = [];
                        var totalDays = 31;
                        var selectedMonth = moment.months().indexOf(editingHolidaysRowMonth ? editingHolidaysRowMonth : editingHolidaysRowItem.Month);
                        if (selectedMonth !== -1) {
                            totalDays = new Date(new Date().getFullYear(), selectedMonth + 1, 0).getDate();
                        }
                        for (var i = 1; i <= totalDays; i++)
                            daysNumbers.push(i);
                        item.editorOptions.items = staticItems.concat(daysNumbers);
                        if (holidaysDayOfMonthEditorSelectBox)
                            holidaysDayOfMonthEditorSelectBox.option("items", staticItems.concat(daysNumbers));
                    }
                },
                onContentReady: function (e) {
                    setTimeout(function () {
                        let form = e.component.element()[0];
                        let btnSave = form.parentElement.parentElement.querySelector("[aria-label='Save']");
                        let btnCancel = form.parentElement.parentElement.querySelector("[aria-label='Cancel']");
                        btnCancel.parentNode.insertBefore(btnSave, btnCancel);
                        let saveButton = $(btnSave).dxButton("instance");
                        let cancelButton = $(btnCancel).dxButton("instance");
                        saveButton.option({
                            "icon": "save",
                            "text": ""
                        });
                        cancelButton.option({
                            "icon": "undo",
                            "text": ""
                        });
                    }, 1);
                }
            },
            allowUpdating: true,
            allowDeleting: true,
            allowAdding: true
        },
        onEditingStart: function (e) {
            editingHolidaysRowItem = e.data;
        }
    };

    hideAllTabsContent();

    var tabs = [
        {
            id: 0,
            visible: false,
            translationId: "WeeklyTab",
            showContent: function () {
                selectedTabContent.add(mainContainer).addClass("weeklyViewActive");
                weeklyViewDiv.css("display", "grid");
            }
        },
        {
            id: 1,
            visible: false,
            translationId: "CalendarTab",
            showContent: function () {
                calendarViewDiv.css("display", "block");
                if (calendarGrid) {
                    calendarGrid.option("height", thisParent.height() - gridsTopPadding - addCalendarItemDiv.height() - 26 - parameters.BorderThickness.Top - parameters.BorderThickness.Bottom);
                    calendarGrid.repaint();
                }
            }
        },
        {
            id: 2,
            visible: false,
            translationId: "HolidaysTab",
            showContent: function () {
                holidaysViewDiv.css("display", "block");
                if (holidaysGrid) {
                    holidaysGrid.option("height", thisParent.height() - gridsTopPadding - addHolidayItemDiv.height() - 26 - parameters.BorderThickness.Top - parameters.BorderThickness.Bottom);
                    holidaysGrid.repaint();
                }
            }
        },
        {
            id: 3,
            visible: false,
            translationId: "PropertiesTab",
            showContent: function () {
                propertiesViewDiv.css("display", "block");
            }
        },
    ];
    $.each(tabs, function (index, item) {
        item.text = translations[item.translationId]();
    });

    var noDataText = i18next.t1('ConnectingToServer');

    var selectedTabBgColor;
    var schedulerTabOptions = {
        items: tabs,
        selectedIndex: -1,
        onContentReady: function (e) {
            schedulerTabsDiv.find("div.dx-item-content.dx-tab-content").css(fontOptions);
            if (parameters.Background)
                schedulerTabsDiv.find(".dx-item.dx-tab:not(.dx-tab-selected)").css("background", parameters.Background.Color);
            if (!selectedTabBgColor)
                selectedTabBgColor = schedulerTabsDiv.find(".dx-tab.dx-tab-selected").css("background-color");

            if (bWeeklyViewInit)
                return;

            setTimeout(function () {
                var componentHeight = schedulerTabsDiv.height();
                gridsTopPadding += componentHeight;
                if (componentHeight > 0)
                    initWeeklyView();
            }, 1);
        },
        onSelectionChanged: function (e) {
            if (e.addedItems.length == 0)
                return;
            hideAllTabsContent();
            tabs[e.addedItems[0].id].showContent();
            if (parameters.Background) {
                schedulerTabsDiv.find(".dx-item.dx-tab:not(.dx-tab-selected)").css("background", parameters.Background.Color);
                schedulerTabsDiv.find(".dx-item.dx-tab.dx-tab-selected").css("background", selectedTabBgColor);
            }
        }
    };

    var deregisterListenerUser = scope.$watch('currentUser', function (v) {
        if (typeof (v) == "undefined" || !bInit)
            return;

        refreshSchedulers();
    });

    var lblSchedulerCombo;
    var loadingIndicator;
    var schedulerTabs;
    var interactiveControls;
    var btnRefresh;
    var btnSave;
    var deleteAllHolidayItems;
    var addRefreshHolidays;
    var addCalendarItem;
    var addHolidayItem;

    var mutateFunc = function () {
        $("<style id=\"" + thisGridID + "\">" + thisGridSelector + " #weeklyView .timeCol.currentTimeCell { border-right-color: " + timeCursorColor + "; border-width: 1px 2px 0px 0px; } " + thisGridSelector + " .dx-header-row .dx-datagrid-text-content, " + thisGridSelector + " .dx-data-row.dx-column-lines td, " + thisGridSelector + " .dx-data-row.dx-column-lines td .dx-datebox .dx-texteditor-input { " + inlineFontOptions + " }</style>").appendTo("head");

        weeklyViewDiv.css(fontOptions)
        if (parameters.Background) {
            weeklyViewDiv.css("background", parameters.Background.Color);
            lblSchedulerComboDiv.css("background", parameters.Background.Color).css(fontOptions);
            btnSaveDiv.css("background", parameters.Background.Color);
            btnRefreshDiv.css("background", parameters.Background.Color);
        }
        if (parameters.ToolbarBackground)
            $(mainContainer, schedulerChooserDiv).css("background", parameters.ToolbarBackground.Color);

        loadingIndicatorDivContainer.css({
            position: scope.biOS && !parameters.bFitInWindow && !isInFittedParent(thisPage) ? "fixed" : "absolute",
            top: (parseFloat(foreignObject.attr("height")) - 60) / 2,
            left: (parseFloat(foreignObject.attr("width")) - 60) / 2
        }).children("#loadingIndicatorText").css("color", parameters.ToolbarForeground.Color).text(i18next.t1("ConnectingToServer"));
        loadingIndicator = loadingIndicatorDiv.dxLoadIndicator({
            height: 60,
            width: 60,
            visible: true
        }).dxLoadIndicator("instance");

        calendarGrid = calendarGridDiv.dxDataGrid(calendarGridOptions).dxDataGrid("instance");
        holidaysGrid = holidaysGridDiv.dxDataGrid(holidaysOptions).dxDataGrid("instance");
        schedulerTabs = schedulerTabsDiv.dxTabs(schedulerTabOptions).dxTabs("instance");

        $(lblSchedulerName).text(translations.SchedulerLabel() + " : ").css(toolbarFontOptions);

        lblSchedulerCombo = lblSchedulerComboDiv.dxSelectBox({
            disabled: true,
            height: 25,
            itemTemplate: function (item) {
                return item.FullName;
            },
            displayExpr: function (item) {
                return item && item.FullName;
            },
            onInitialized: function (e) {
                var el = e.component.element();
                el.css(fontOptions).css("color", parameters.ToolbarForeground.Color);
                setTimeout(function () {
                    var input = el.find("input.dx-texteditor-input").first();
                    input.css(fontOptions).css("color", parameters.ToolbarForeground.Color);
                    gridsTopPadding += lblSchedulerComboDiv.height();
                }, 1); //on inner elements rendering
            },
            onValueChanged: function (e) {
                var newValue = e.value;
                if (newValue) {
                    if (bDirty) {
                        DevExpress.ui.dialog.confirm(i18next.t1("SavePendingCalendarChanges"), i18next.t1("SavePendingCalendarChangesTitle"))
                            .then(function (res) {
                                if (res)
                                    saveSchedulerChanges(newValue);
                                else
                                    updateScheduler(newValue);
                            });
                    }
                    else
                        updateScheduler(newValue);
                }
            }
        }).dxSelectBox("instance");

        btnRefresh = btnRefreshDiv.dxButton({
            icon: "images/toolbox/refresh.svg",
            hint: translations.ReloadData(),
            type: "normal",
            disabled: false,
            onClick: function (e) {
                updateScheduler(currentScheduler);
                //refreshSchedulers();
            }
        }).dxButton("instance");

        btnSave = btnSaveDiv.dxButton({
            icon: "images/toolbox/save.svg",
            hint: translations.SchedulerSave(),
            type: "normal",
            disabled: true,
            onClick: function (e) {
                saveSchedulerChanges(currentScheduler);
            }
        }).dxButton("instance");

        deleteAllHolidayItems = addButton(deleteAllHolidayItemsDiv, translations.DeleteItems,
            function (e) {
                if (updatingScheduler || !currentScheduler || !holidaysGrid)
                    return;

                DevExpress.ui.dialog.confirm(i18next.t1("DeleteCalendarItems"), i18next.t1("DeleteCalendarItemsTitle"))
                    .then(function (res) {
                        if (res)
                            holidaysGrid.option('dataSource', []);
                    });
            }
        );
        addRefreshHolidays = addButton(addRefreshHolidaysDiv, translations.AddHolidays,
            function (e) {
                DevExpress.ui.dialog.confirm(i18next.t1("AddMissingHolidays"), i18next.t1("AddMissingHolidaysTitle"))
                    .then(function (res) {
                        if (res)
                            updateHolidays();
                    });
            }
        );
        addCalendarItem = addButton(addCalendarItemDiv, translations.AddItem,
            function (e) {
                if (calendarGrid)
                    calendarGrid.addRow();
            }
        );
        addHolidayItem = addButton(addHolidayItemDiv, translations.AddItem,
            function (e) {
                if (holidaysGrid)
                    holidaysGrid.addRow();
            }
        );

        interactiveControls = [/*weeklyView,*/ lblSchedulerCombo, btnRefresh, btnSave, addCalendarItem, addHolidayItem, deleteAllHolidayItems, addRefreshHolidays, schedulerTabs];

        var options = {};
        setObjectBorder(options, parameters);
        if (!$.isEmptyObject(options))
            outerContainer.css(options);

        deregisterListenerActiveLanguage = scope.$on(onActiveLanguageChanged, function (e, value) {
            $.each(translatedObjectsMap, function (index, item) {
                if (item.instance) {
                    $.each(item.options, function (optIndex, optItem) {
                        item.instance.option(optItem, item.translationFunc());
                    });
                }
            });

            if (calendarGrid)
                for (var i = 0; i < calendarGrid.columnCount(); i++)
                    calendarGrid.columnOption(i, "caption", translations[calendarGrid.columnOption(i, "name")]());

            if (holidaysGrid)
                for (var j = 0; j < holidaysGrid.columnCount(); j++)
                    holidaysGrid.columnOption(j, "caption", translations[holidaysGrid.columnOption(j, "name")]());

            $.each(thisPage.find('div.weekday'), function (index, item) {
                let $item = $(item);
                var translationKey = "Day" + $item.attr("untranslatedValue");
                if (translationKey in translations)
                    $item.text(translations[translationKey]());
            });

            $(lblSchedulerName).text(translations.SchedulerLabel() + " : ");
            $.each(tabs, function (index, item) {
                item.text = translations[item.translationId]();
            });

            if (btnSave)
                btnSave.option("hint", translations.SchedulerSave());
            if (btnRefresh)
                btnRefresh.option("hint", translations.ReloadData());

            thisPage.find(".propertiesGridCaption").each(function (index, item) {
                let $item = $(item);
                var labelData = propertiesGridFields[parseInt($item.attr("colIndex"), 10)];
                if (labelData.translationString in translations)
                    $item.html(translations[labelData.translationString]());
            });
            var propschedulertype = thisPage.find(".propertiesSchedulerType.propValue");
            propschedulertype.html(translateNewEventType(propschedulertype.attr("untranslatedValue")));
            schedulerTabs.option("dataSource", tabs);
        });

        updateQualityStatus();
        if (scope.dataValues[parameters.screenId] && scope.dataValues[parameters.screenId][parameters.SVGReferenceId] && scope.dataValues[parameters.screenId][parameters.SVGReferenceId].isGood) {
            updateQualityStatus();
            initSchedulerServerConnection();
        }

        deregisterListener = scope.$on(onDataChanged + parameters.screenId + parameters.SVGReferenceId, function (e, value) {
            if (scope.dataValues[parameters.screenId][parameters.SVGReferenceId].isGood) {
                updateQualityStatus();
                initSchedulerServerConnection();
            }
            toggleEnabledOption(connected);
        });

        deregisterListenerViewClosed = scope.$on(onViewClosed + parameters.screenId, function (value, obj) {
            scope.corehubconnection.invoke("SchedulerDispose", parameters.screenId, thisParentId)
                .then((ret) => {
                    scope.busyVisible = false;
                    scope.$apply();
                })
                .catch(err => {
                    console.error(err.toString());
                    scope.$broadcast(onError, { ex: err, value: getClientErrorText(err) });

                    scope.busyVisible = false;
                    scope.$apply();
                });
        });

        thisParent.bind(disabledClassEvent, function () {
            toggleEnabledOption(false);
        });
    };

    mutateHandler.scheduleMutate(parameters.screenId, mutateFunc);

    function updateQualityStatus() {
        bBadQuality = applyTagQualityStyle(webKitRenderCorrection ? thisPage : thisParent, scope, parameters.screenId, parameters.SVGReferenceId, false, scope.biOS ? outerContainer : null);
        if (biOSnoFiw)
            thisParent.css("overflow", bBadQuality ? "visible" : "hidden");
    }

    function configureSchedulerControl(selectedSchedulerType) {
        let newSelectedIndex = 0;
        switch (selectedSchedulerType) {
            case "weeklyPlan":
                tabs[0].visible = tabs[2].visible = tabs[3].visible = true;
                tabs[1].visible = false;
                propertiesGrid.children(".every").css("display", "none");
                propertiesGrid.children(".calendar_weekly").css("display", "block");
                break;
            case "calendar":
                newSelectedIndex = 1;
                tabs[1].visible = tabs[2].visible = tabs[3].visible = true;
                tabs[0].visible = false;
                propertiesGrid.children(".every").css("display", "none");
                propertiesGrid.children(".calendar_weekly").css("display", "block");
                break;
            default:
                newSelectedIndex = 3;
                tabs[2].visible = tabs[3].visible = true;
                tabs[0].visible = tabs[1].visible = false;
                propertiesGrid.children(".every").css("display", "block");
                propertiesGrid.children(".calendar_weekly").css("display", "none");
                break;
        }
        schedulerTabs.option("selectedIndex", -1);
        schedulerTabs.option({
            "items": tabs,
            "selectedIndex": newSelectedIndex
        });
    }

    function getDayFullName(date) {
        if (typeof date !== "number") {
            try {
                date = date.toDate();
            }
            catch { }
        }
        else
            date = new Date(date);
        return daysFullNames[date.getDay()];
    }

    function prepareCalendarItems(dataSource) {
        $.each(dataSource, function (index, item) {
            item.StartDateOffset = getGMTOffset(item.StartDate, true);
            item.EndDateOffset = getGMTOffset(item.EndDate, true);
            item.DayOfWeek = getDayFullName(item.StartDate);
        });
    }

    function saveSchedulerChanges(bSchedulerToUpdate) {
        if (updatingScheduler || !currentScheduler)
            return;
        updatingScheduler = true;

        var bIsInError = false;

        toggleEnabledOption(false);

        prepareCalendarItems(calendarGrid.option("dataSource"));
        prepareCalendarItems(holidaysGrid.option("dataSource"));
        prepareCalendarItems(currentWeeklyEvents);

        var newTimeOn = (typeof currentTimeOn == "number" ? currentTimeOn : currentScheduler["TimeOn"]) / 10000000;
        if (isNaN(newTimeOn))
            newTimeOn = null;
        var newTimeOff = (typeof currentTimeOff == "number" ? currentTimeOff : currentScheduler["TimeOff"]) / 10000000;
        if (isNaN(newTimeOff))
            newTimeOff = null;

        $.each(calendarGrid.option("dataSource"), function (i, el) {
            calendarGrid.option("dataSource")[i]["StartDate"] = momentToDate(el["StartDate"]);
            calendarGrid.option("dataSource")[i]["EndDate"] = momentToDate(el["EndDate"]);
        });
        $.each(holidaysGrid.option("dataSource"), function (i, el) {
            holidaysGrid.option("dataSource")[i]["StartDate"] = momentToDate(el["StartDate"]);
            holidaysGrid.option("dataSource")[i]["EndDate"] = momentToDate(el["EndDate"]);
        });

        scope.corehubconnection.invoke("SaveCurrentScheduler", parameters.screenId, thisParentId, timezoneOffset,
            currentScheduler.FullName, currentScheduler.NodeId, currentScheduler.Type, newTimeOn, newTimeOff, currentExecOn, currentExecOff,
            calendarGrid.option("dataSource"), holidaysGrid.option("dataSource"), currentWeeklyEvents)
            .then((ret) => {
            })
            .catch(err => {
                console.error("Error saving current scheduler:", err);
                bIsInError = true;
            })
            .finally(() => {
                updatingScheduler = false;
                toggleEnabledOption(true);
                if (!bIsInError)
                    updateScheduler(bSchedulerToUpdate);
            });
    }

    function addButton(container, textFn, onClick) {
        var btnInstance = container.dxButton({
            text: textFn(),
            hint: textFn(),
            type: "normal",
            onContentReady: function (e) {
                var el = e.component.element();
                el.css("margin", "5px 0px 5px 0px");
                el.css(fontOptions);
            },
            disabled: true,
            onClick: onClick
        }).dxButton("instance");
        translatedObjectsMap.push({
            "instance": btnInstance,
            "options": ["text", "hint"],
            "translationFunc": textFn
        });
        return btnInstance
    }

    function refreshSchedulers() {
        if (scope.dataValues[parameters.screenId][parameters.SVGReferenceId].isGood) {
            toggleEnabledOption(false);
            scope.corehubconnection.invoke("InitSchedulerOnRuntime", parameters.screenId, thisParentId, parameters.StartUpScheduler)
                .then((comboItems) => {
                    bDirty = false;
                    if (loadingIndicator)
                        loadingIndicator.option("visible", false);
                    loadingIndicatorDivContainer.css("display", "none");
                    if (comboItems.length > 0) {
                        var selected;
                        var comboDisabled = false;
                        if (parameters.StartUpScheduler) {
                            selected = comboItems.find(i => i.FullName === parameters.StartUpScheduler);
                            comboDisabled = selected !== null;
                            lblSchedulerCombo.option({
                                "items": comboItems,
                                "value": selected === null ? comboItems[0] : selected,
                                "disabled": comboDisabled,
                                "keepdisabled": comboDisabled
                            });
                        }
                        else {
                            controlPropertiesDB.get(scope, schedulerStorageKey)
                                .then(function (res) {
                                    if (res)
                                        selected = comboItems.find(i => i.FullName === res);
                                })
                                .catch(function (ex) { })
                                .finally(function (res) {
                                    lblSchedulerCombo.option({
                                        "items": comboItems,
                                        "value": undefinedOrNull(selected) ? comboItems[0] : selected,
                                        "disabled": comboDisabled,
                                        "keepdisabled": comboDisabled
                                    });
                                });
                        }
                    }
                })
                .catch(err => {
                    //dataGrid.option("loadPanel.enabled", false);
                    var errorText = i18next.t1('ErrorGettingSchedulersList') + err.toString();
                    //dataGrid.option("noDataText", errorText);
                    scope.$broadcast(onError, { ex: err, value: errorText });
                })
                .finally(() => {
                    toggleEnabledOption(true);
                    if (!bInit)
                        bInit = true;
                });
        }
    }

    function initSchedulerServerConnection() {
        if (connected)
            return;
        connected = true;

        var onNodeIdResolved = function (nodeIdViewModel) {
            scope.corehubconnection.off("GetSettingsListMethodResolved_" /*+ parameters.screenId + "_"*/ + parameters.SVGReferenceId, onNodeIdResolved);
            refreshSchedulers();
        }

        scope.corehubconnection.on("GetSettingsListMethodResolved_" /*+ parameters.screenId + "_"*/ + parameters.SVGReferenceId, onNodeIdResolved);

        scope.corehubconnection.invoke("Scheduler_InitServerConnection", parameters.screenId, parameters.SVGReferenceId, thisParentId, parameters.ConnectionString)
            .then((ret) => { })
            .catch(err => {
                console.error(err.toString());
                //dataGrid.option("loadPanel.enabled", false);
                var errorText = i18next.t1('ErrorGettingSchedulersList') + err.toString();
                //dataGrid.option("noDataText", errorText);
                scope.$broadcast(onError, { ex: err, value: errorText });
            });
    }

    function getGMTOffset(date, bIsTimestamp) {
        if (typeof date !== "number") {
            try {
                date = date.toDate().getTime();
            }
            catch {
                date = date.getTime();
            }
        }
        var date = moment(bIsTimestamp ? date : ticksToTimestamp(date));
        return date.utcOffset() * 60 * 1000
    }

    function updateHolidays() {
        if (updatingScheduler)
            return;
        updatingScheduler = true;
        toggleEnabledOption(false);
        prepareCalendarItems(holidaysGrid.option("dataSource"));
        var existingHolidays = [];
        $(holidaysGrid.option("dataSource")).each(function (index, item) {
            var row = {};
            for (var key in item)
                row[key] = moment.isMoment(item[key]) ? momentToDate(item[key]) : item[key];
            existingHolidays.push(row);
        });
        scope.corehubconnection.invoke("AddMissingYearHolidays", parameters.screenId, thisParentId, timezoneOffset,
            currentScheduler.FullName, currentScheduler.NodeId, currentScheduler.Type, isoLang, existingHolidays)
            .then((ret) => {
                var holidaysItems = [];
                $(ret.ExceptionCalendarItems).each(function (i, e) {
                    holidaysItems.push({
                        StartDate: moment(ticksToTimestamp(e["StartDate"]) - getGMTOffset(e["StartDate"])), //+ timezoneOffset),
                        EndDate: moment(ticksToTimestamp(e["EndDate"]) - getGMTOffset(e["EndDate"])), //+ timezoneOffset),
                        ItemType: e["ItemType"],
                        Month: e["Month"],
                        WeekOfMonth: e["WeekOfMonth"],
                        DayOfWeek: e["DayOfWeek"],
                        DayOfMonth: holidaysGrid.option("columns")[6].formItem.editorOptions.items[parseInt(e["DayOfMonth"], 10)]
                    });
                });
                holidaysGrid.option("dataSource", holidaysItems);
                bDirty = true;
            })
            .catch(err => {
                console.error(err.toString());
                var errorText = i18next.t1('ErrorUpdatingSchedulerData') + err.toString();
                scope.$broadcast(onError, { ex: err, value: errorText });
            })
            .finally(() => {
                updatingScheduler = false;
                toggleEnabledOption(true);
            });
    }

    function updateScheduler(scheduler) {
        if (updatingScheduler)
            return;
        updatingScheduler = true;

        if (currentScheduler == null || scheduler.FullName != currentScheduler.FullName) {
            configureSchedulerControl(scheduler.SchedulerType);
            initializePropertiesGrid();
            controlPropertiesDB.add(scope, schedulerStorageKey, scheduler.FullName);
        }
        toggleEnabledOption(false);
        scope.corehubconnection.invoke("UpdateScheduler", parameters.screenId, thisParentId, scheduler.FullName)
            .then((ret) => {
                currentScheduler = {
                    NodeId: ret.ScheduledEventNodeId,
                    FullName: ret.FullName,
                    Type: ret.Type,
                    Time: ret.Time,
                    TimeOff: ret.TimeOff,
                    ExecOn: ret.ExecOn,
                    ExecOff: ret.ExecOff,
                    CalendarItems: ret.CalendarItems,
                    ExceptionCalendarItems: ret.ExceptionCalendarItems
                };
                if (bWeeklyCalendarLoaded)
                    loadCurrentScheduler();
                else
                    bPendingSchedulerLoading = true;
            })
            .catch(err => {
                console.error(err.toString());
                var errorText = i18next.t1('ErrorUpdatingSchedulerData') + err.toString();
                scope.$broadcast(onError, { ex: err, value: errorText });

                updatingScheduler = false;
                toggleEnabledOption(true);
            });
    }

    function loadCurrentScheduler() {
        try {
            bDirty = false;

            var timeOn = propertiesGridFields[2].control.dxDateBox("instance");
            var timeOff = propertiesGridFields[3].control.dxDateBox("instance");
            var execOn = propertiesGridFields[8].control.dxCheckBox("instance");
            var execOff = propertiesGridFields[9].control.dxCheckBox("instance");
            if (timeOn)
                timeOn.option("value", moment(ticksToTimestamp(currentScheduler["Time"]) - getGMTOffset(currentScheduler["Time"])));
            if (timeOff)
                timeOff.option("value", moment(ticksToTimestamp(currentScheduler["TimeOff"]) - getGMTOffset(currentScheduler["TimeOff"])));
            if (execOn)
                execOn.option("value", currentScheduler["ExecOn"]);
            if (execOff)
                execOff.option("value", currentScheduler["ExecOff"]);

            var calendarItems = [];
            var weeklyItems = [];
            $(currentScheduler.CalendarItems).each(function (i, e) {
                if (currentScheduler.Type == "weeklyPlan") {
                    if (e["DayOfWeek"] !== null) {

                        var sDate = moment(ticksToTimestamp(e["StartDate"]) - getGMTOffset(e["StartDate"])).toDate();
                        var eDate = moment(ticksToTimestamp(e["EndDate"]) - getGMTOffset(e["EndDate"])).toDate();

                        weeklyItems.push({
                            text: "Event" + (i + 1),
                            eventIndex: i + 1,
                            eventID: 1,
                            StartDate: moment(ticksToTimestamp(e["StartDate"]) - getGMTOffset(e["StartDate"])),
                            EndDate: moment(ticksToTimestamp(e["EndDate"]) - getGMTOffset(e["EndDate"])),
                        });
                    }
                }
                else if (currentScheduler.Type == "calendar") {
                    calendarItems.push({
                        StartDate: moment(ticksToTimestamp(e["StartDate"]) - getGMTOffset(e["StartDate"])),
                        EndDate: moment(ticksToTimestamp(e["EndDate"]) - getGMTOffset(e["EndDate"])),
                        ItemType: e["ItemType"],
                        Month: e["Month"],
                        WeekOfMonth: e["WeekOfMonth"],
                        DayOfWeek: e["DayOfWeek"],
                        DayOfMonth: calendarGrid.option("columns")[6].formItem.editorOptions.items[parseInt(e["DayOfMonth"], 10)] //e["DayOfMonth"]
                    });
                }
            });
            if (currentScheduler.Type == "weeklyPlan") {
                clearWeeklyEvents();
                $.each(weeklyItems, function (index, elem) {
                    addWeeklyEvent(elem.StartDate.toDate(), elem.EndDate.toDate(), elem.text, elem.eventIndex, true);
                });
            }
            else if (currentScheduler.Type == "calendar") {
                calendarGrid.option("dataSource", calendarItems);
            }

            var holidaysItems = [];
            $(currentScheduler.ExceptionCalendarItems).each(function (i, e) {
                holidaysItems.push({
                    StartDate: moment(ticksToTimestamp(e["StartDate"]) - getGMTOffset(e["StartDate"])),
                    EndDate: moment(ticksToTimestamp(e["EndDate"]) - getGMTOffset(e["EndDate"])),
                    ItemType: e["ItemType"],
                    Month: e["Month"],
                    WeekOfMonth: e["WeekOfMonth"],
                    DayOfWeek: e["DayOfWeek"],
                    DayOfMonth: holidaysGrid.option("columns")[6].formItem.editorOptions.items[parseInt(e["DayOfMonth"], 10)]
                });
            });
            holidaysGrid.option("dataSource", holidaysItems);
        }
        finally {
            bPendingSchedulerLoading = false;
            updatingScheduler = false;
            toggleEnabledOption(true);
        }
    }

    function toggleEnabledOption(enabled) {
        $.each(interactiveControls, function (index, item) {
            if (!item.option("keepdisabled"))
                item.option("disabled", !enabled);
        });
    }

    function translateNewEventType(schedulerType) {
        var schedulerString = schedulerType.toLowerCase() === "calendar" ? "NewEventCalendar" : (schedulerType.charAt(0).toUpperCase() + schedulerType.slice(1));
        return schedulerString in translations ? translations[schedulerString]() : schedulerType;
    }

    var propertiesGridFields = [];
    var currentTimeOn;
    var currentTimeOff;
    var currentExecOn;
    var currentExecOff;
    var initialTimeOn;
    var initialTimeOff;
    function initializePropertiesGrid() {
        var currentScheduler = lblSchedulerCombo.option("value");
        propertiesGrid.css(fontOptions);
        if (parameters.Background)
            propertiesGrid.css("background", parameters.Background.Color);
        propertiesGrid.html("");
        currentExecOn = currentScheduler["ExecOn"];
        currentExecOff = currentScheduler["ExecOff"]
        currentTimeOn = moment(ticksToTimestamp(currentScheduler["Time"]) - getGMTOffset(currentScheduler["Time"]));
        currentTimeOff = moment(ticksToTimestamp(currentScheduler["TimeOff"]) - getGMTOffset(currentScheduler["TimeOff"]));
        initialTimeOn = { "ticks": currentScheduler["Time"], "dayTicks": getDayTicks(currentTimeOn.toDate()) };
        initialTimeOff = { "ticks": currentScheduler["TimeOff"], "dayTicks": getDayTicks(currentTimeOff.toDate()) };
        propertiesGridFields = [
            {
                index: 0,
                translationString: "NewEventName",
                control: currentScheduler["FullName"],
                visible: true
            },
            {
                index: 1,
                translationString: "NewEventType",
                class: "propertiesSchedulerType",
                attributes: [{ "name": "untranslatedValue", "value": currentScheduler["SchedulerType"] }],
                control: translateNewEventType(currentScheduler["SchedulerType"]),
                visible: true
            },
            {
                index: 2,
                translationString: "Time",
                control: (function () {
                    var container = $("<div/>");
                    container.dxDateBox({
                        showClearButton: false,
                        useMaskBehavior: true,
                        displayFormat: "HH:mm:ss",
                        type: "datetime",
                        calendarOptions: {
                            visible: false
                        },
                        value: currentTimeOn,
                        onContentReady: function (e) {
                            e.component.element().find("input").css(fontOptions);
                        },
                        onValueChanged: function (e) {
                            if (typeof (e.event) !== "undefined")
                                bDirty = true;
                            if (e.value instanceof Date) {
                                var dayTicks = getDayTicks(e.value);
                                currentTimeOn = initialTimeOn.ticks - initialTimeOn.dayTicks + dayTicks;
                            }
                        }
                    });
                    return container;
                })(),
                visible: currentScheduler["SchedulerType"] != "calendar" && currentScheduler["SchedulerType"] != "weeklyPlan",
                class: "every"
            },
            {
                index: 3,
                translationString: "TimeOff",
                control: (function () {
                    var container = $("<div/>");
                    container.dxDateBox({
                        showClearButton: false,
                        useMaskBehavior: true,
                        displayFormat: "HH:mm:ss",
                        type: "datetime",
                        calendarOptions: {
                            visible: false
                        },
                        value: currentTimeOff,
                        onContentReady: function (e) {
                            e.component.element().find("input").css(fontOptions);
                        },
                        onValueChanged: function (e) {
                            if (typeof (e.event) !== "undefined")
                                bDirty = true;
                            if (e.value instanceof Date) {
                                var dayTicks = getDayTicks(e.value);
                                currentTimeOff = initialTimeOff.ticks - initialTimeOff.dayTicks + dayTicks;
                            }
                        }
                    });
                    return container;
                })(),
                visible: currentScheduler["SchedulerType"] != "calendar" && currentScheduler["SchedulerType"] != "weeklyPlan",
                class: "every"
            },
            {
                index: 4,
                translationString: "NewEventTag",
                control: currentScheduler["Tag"],
                visible: true
            },
            {
                index: 5,
                translationString: "NewEventValueOn",
                control: currentScheduler["ValueOn"],
                visible: true
            },
            {
                index: 6,
                translationString: "NewEventValueOff",
                control: currentScheduler["ValueOff"],
                visible: true
            },
            {
                index: 7,
                translationString: "NewEventEnableTag",
                control: currentScheduler["EnableVar"],
                visible: true
            },
            {
                index: 8,
                translationString: "ExecOnAtStartup",
                control: (function () {
                    var container = $("<div/>");
                    container.dxCheckBox({
                        value: currentExecOn,
                        onValueChanged: function (e) {
                            if (typeof (e.event) !== "undefined")
                                bDirty = true;
                            currentExecOn = e.value;
                        }
                    });
                    return container;
                })(),
                class: "calendar_weekly",
                visible: currentScheduler["SchedulerType"] == "calendar" || currentScheduler["SchedulerType"] == "weeklyPlan",
            },
            {
                index: 9,
                translationString: "ExecOffAtStartup",
                control: (function () {
                    var container = $("<div/>");
                    container.dxCheckBox({
                        value: currentExecOff,
                        onValueChanged: function (e) {
                            if (typeof (e.event) !== "undefined")
                                bDirty = true;
                            currentExecOff = e.value;
                        }
                    });
                    return container;
                })(),
                class: "calendar_weekly",
                visible: currentScheduler["SchedulerType"] == "calendar" || currentScheduler["SchedulerType"] == "weeklyPlan",
            },
            {
                index: 10,
                translationString: "NewEventAccessRole",
                control: currentScheduler["AccessRole"],
                visible: true
            },
            {
                index: 11,
                translationString: "NewEventAccessLevel",
                control: currentScheduler["AccessLevel"],
                visible: true
            },
            {
                index: 12,
                translationString: "NewEventAccessMask",
                control: currentScheduler["AccessMasks"],
                visible: true
            }
        ];
        $.each(propertiesGridFields, function (index, item) {
            if ("translationString" in item && item.translationString in translations)
                item.caption = translations[item.translationString]();
            propertiesGrid
                .append(
                    $("<div>", {
                        colIndex: item.index,
                        class: "propertiesGridCaption" + (item.class ? " " + item.class : ""),
                        html: !item.visible ? "&nbsp;" : i18next.t1(item.caption)
                    })
                ).append(
                    (function () {
                        var divAttrs = {
                            class: "propValue" + " " + item.class,
                            html: !item.visible ? "&nbsp;" : item.control
                        };
                        $.each(item.attributes, function (attrIndex, attrItem) {
                            divAttrs[attrItem.name] = attrItem.value;
                        });
                        return $("<div>", divAttrs);
                    })()
                );
        });
        propertiesGrid.css("height", thisParent.height() - schedulerChooserDiv.height() - schedulerTabsDiv.height() - 12 - parameters.BorderThickness.Top - parameters.BorderThickness.Bottom)
    }

    function configureTheme() {
        calendarViewDiv.css("background", parameters.ToolbarBackground.Color);
        holidaysViewDiv.css("background", parameters.ToolbarBackground.Color);
        propertiesViewDiv.css("background", parameters.ToolbarBackground.Color);
        calendarViewDiv.children("div").css("background", parameters.Background.Color);
        holidaysViewDiv.children("div").css("background", parameters.Background.Color);
    }

    function hideAllTabsContent() {
        weeklyViewDiv.css("display", "none");
        calendarViewDiv.css("display", "none");
        holidaysViewDiv.css("display", "none");
        propertiesViewDiv.css("display", "none");
        selectedTabContent.add(mainContainer).removeClass("weeklyViewActive");
    }

    function getNewEventData() {
        var newIndex = 1;
        if (currentWeeklyEvents.length > 0)
            newIndex = Math.max.apply(Math, currentWeeklyEvents.map(function (o) { return o.eventIndex })) + 1;
        return {
            neweventindex: newIndex,
            neweventid: "AddedEvent" + newIndex
        };
    }

    function repeatEvent(startDate, endDate, neweventid, neweventindex, repeatEveryDays) {
        if (bAddingAppointmentsSerie)
            return false;

        bAddingAppointmentsSerie = true;
        addWeeklyEvent(startDate, endDate, neweventid, neweventindex);
        var originalStartDate = new Date(startDate.getTime());
        var originalEndDate = new Date(endDate.getTime());
        var i = 1;
        while (true) {
            var newEndDate = moment(originalEndDate).add(repeatEveryDays * i, "days").toDate();
            if (newEndDate > new Date(weekSunday.year, weekSunday.month, weekSunday.day, 23, 59, 59))
                break;
            addWeeklyEvent(moment(originalStartDate).add(repeatEveryDays * i, "days").toDate(), newEndDate);
            i++;
        }
        bAddingAppointmentsSerie = false;
        return true;
    }

    function getCurrentWeeklyCursorCells() {
        var currentWeeklyCursorCells = [];
        var now = new Date();
        if (parameters.ShowWorkTimeOnly) {
            var bDateBefore = now.getHours() < weeklyViewStartDate.getHours() || now.getHours() == weeklyViewStartDate.getHours() && now.getMinutes() < weeklyViewStartDate.getMinutes();
            var bDateAfter = now.getHours() > weeklyViewEndDate.getHours() || now.getHours() == weeklyViewEndDate.getHours() && now.getMinutes() > weeklyViewEndDate.getMinutes();
            if (bDateBefore || bDateAfter)
                return currentWeeklyCursorCells;
        }
        var currentTimeFirstCell = (Math.ceil(rowCells / hourCells) + 2) + Math.floor((new Date(weekMonday.year, weekMonday.month, weekMonday.day, now.getHours(), now.getMinutes(), 0) - weeklyViewStartDate) / weeklyCellMs);
        for (var i = currentTimeFirstCell; i <= weeklyTotalCells; i += weeklyRowCells) {
            currentWeeklyCursorCells.push(i);
        }
        return currentWeeklyCursorCells;
    }

    function updateNowCursor() {
        var currentWeeklyCursorCells = getCurrentWeeklyCursorCells();
        weeklyViewDiv.find(".timeCol").removeClass("currentTimeCell");
        $.each(currentWeeklyCursorCells, function (index, item) {
            weeklyViewDiv.find(".cellTimeCol[colindex=" + item + "]").addClass("currentTimeCell");
        });
    }

    function initWeeklyView() {
        if (bWeeklyViewInit)
            return;
        bWeeklyViewInit = true;

        weeklyViewDiv.css({
            //"display": "grid",
            "grid-template-columns": "repeat(" + weeklyRowCells + ", auto)"
        });

        var currentWeeklyCursorCells = getCurrentWeeklyCursorCells();

        function setCursorTimeout() {
            var now = new Date();
            var cursorUpdateTimeoutMS = weeklyCellMs - (now.getMinutes() % weeklyCellMinutes) * 60000 - now.getSeconds() * 1000;
            cursorTimeout = setTimeout(function () {
                updateNowCursor();
                setCursorTimeout();
            }, cursorUpdateTimeoutMS);
        }
        setCursorTimeout();

        var rowColIndex = 0;
        var rowIndex = 0;
        var now = new Date();
        var currentWeekDay = now.getDay() === 0 ? 7 : now.getDay();
        var columns = [];

        for (var i = 1; i <= weeklyTotalCells; i++) {
            var multiple = (i - firstRowCells - 1) % (rowCells + 1) === 0;
            var dayindex = rowIndex - 3;
            var column = $("<div class='timeCol" + (dayindex >= 0 ? " cellTimeCol" : "") + "' colindex='" + i + "' colrelativeindex='" + rowColIndex + "' dayindex='" + dayindex + "' " + (!multiple && (rowIndex - 2) % 2 === 1 ? "style='background-color: " + oddRowsBackground + ";'" : "") + "></div>");
            if (i === Math.ceil(rowCells / hourCells) + 2) {
                rowColIndex = 0;
                rowIndex++;
            }
            if (i > 1 && i <= firstRowCells) {
                column
                    .css({
                        "display": "flex",
                        "grid-column-start": "span " + (i === 2 ? firstRow_firstColspan : (i === firstRowCells ? firstRow_endColspan : firstRow_middleColspan))
                    })
                    .addClass("hourlabel")
                    .append("<div class='dayhour'>" + padString((i - 2) + startH, 2, 0) + "</div>");
            }
            else if (i === 1 || multiple) {
                rowColIndex = 0;
                rowIndex++;
                if (rowIndex > 1) {
                    var daycellstyle = "";
                    if (currentWeekDay === rowIndex - 2)
                        daycellstyle = "style='color: " + currentDayColor + "'";
                    var rowDay = daysFullNames[(rowIndex - 2) % daysFullNames.length];
                    column
                        .css({ "display": "flex", "overflow-x": "auto" })
                        .addClass("daylabel")
                        .append("<div class='weekday' untranslatedValue='" + rowDay + "' " + daycellstyle + ">" + translations["Day" + rowDay]() + "</div>");
                }
            }
            if (rowIndex > 1 && (rowColIndex * weeklyCellMinutes + startM) % 60 === 0) {
                column
                    .addClass("hourSeparator");
            }

            if (currentWeeklyCursorCells.indexOf(i) !== -1)
                column.addClass("currentTimeCell");

            rowColIndex++;
            //weeklyViewDiv.append(column);
            columns.push(column);
        }

        fastdom.mutate(function () {
            $(columns).each(function (index, item) {
                weeklyViewDiv.append(item);
            })

            weeklyViewDiv.find(".cellTimeCol:not(.daylabel)").on({
                dblclick: onCellSelect,
                touchstart: function (e) {
                    if (longTouch)
                        clearTimeout(longTouch);

                    if (e.touches.length > 1) {
                        bIgnoreNextTouchEnd = true;
                        return;
                    }

                    if (!tapped) {
                        tapped = setTimeout(function () {
                            tapped = null
                        }, doubleClickDelay);
                    } else {
                        bIgnoreNextTouchEnd = true;
                        onCellSelect(e);
                        return;
                    }

                    try {
                        weeklyContextMenu.hide();
                        contextMenuEdit.dxContextMenu("dispose");
                    } catch { }
                    longTouch = setTimeout(function () {
                        bIgnoreNextTouchEnd = true;
                        openWeeklyContextMenu(e.target, true);
                    }, longTouchDelay);

                    let $this = $(this);
                    if (!$this.hasClass("eventCell") && !$this.hasClass("creatingEvent")) {
                        touchSelecting = true;
                        $this.addClass("creatingEvent").css("background-color", eventBackgroundColor);
                        if (!selectionFirstCell)
                            selectionFirstCell = $this;
                    }
                    e.preventDefault();
                },
                touchmove: function (e) {
                    if (e.touches.length > 1) {
                        bIgnoreNextTouchEnd = true;
                        return;
                    }

                    var target = document.elementFromPoint(e.originalEvent.changedTouches[0].clientX, e.originalEvent.changedTouches[0].clientY);
                    let $target = $(target);
                    if ($target.hasClass("eventCell") && !selectionFirstCell)
                        return;

                    if (longTouch)
                        clearTimeout(longTouch);

                    if (touchSelecting) {
                        if (!lastLeavedCell) {
                            lastLeavedCell = $target;
                            return;
                        }
                        if ($target == lastLeavedCell) {
                            return;
                        }
                        onCellMove(target);
                        lastLeavedCell = $target;
                    }

                },
                touchend: function (e) {
                    if (longTouch)
                        clearTimeout(longTouch);

                    if (bIgnoreNextTouchEnd) {
                        bIgnoreNextTouchEnd = false;
                        return;
                    }
                    touchSelecting = false;

                    var cell = $(this);
                    if (cell.hasClass("creatingEvent"))
                        lastLeavedCell = cell;

                    var creatingEventCells = weeklyViewDiv.find(".cellTimeCol.creatingEvent");
                    if (creatingEventCells.length == 0 && cell.hasClass("eventCell")) {
                        if (cell.hasClass("multiEventCell"))
                            return;
                        if (!dblClickedTimeout) {
                            dblClickedTimeout = setTimeout(function () {
                                dblClickedTimeout = null;
                                if (weeklyContextMenu && weeklyContextMenu.option("visible") == true)
                                    return;

                                clearSingleEventCell(cell);
                            }, doubleClickDelay);
                        }
                    }
                    else
                        onDragEnd(creatingEventCells);
                },
                mouseover: function (e) {
                    if ($(this).hasClass("eventCell") && !selectionFirstCell)
                        return;

                    if (e.buttons == 1 || e.buttons == 3)
                        onCellMove(this);
                },
                mouseleave: function (e) {
                    let cell = $(this);
                    if (!cell.hasClass("creatingEvent"))
                        return;

                    if (e.buttons == 1 || e.buttons == 3)
                        lastLeavedCell = cell;
                },
                mousedown: function (e) {
                    let cell = $(this);
                    if (cell.hasClass("eventCell") || e.which == 3)
                        return;

                    if (!cell.hasClass("creatingEvent")) {
                        cell.addClass("creatingEvent").css("background-color", eventBackgroundColor);
                        if (!selectionFirstCell)
                            selectionFirstCell = cell;
                    }
                },
                mouseup: function (e) {
                    if (e.which == 3) {
                        try {
                            weeklyContextMenu.hide();
                            contextMenuEdit.dxContextMenu("dispose");
                        } catch { }
                        openWeeklyContextMenu();
                        return;
                    }

                    var cell = $(this);
                    var creatingEventCells = weeklyViewDiv.find(".cellTimeCol.creatingEvent");
                    if (creatingEventCells.length == 0 && cell.hasClass("eventCell")) {
                        if (cell.hasClass("multiEventCell"))
                            return;
                        if (!dblClickedTimeout) {
                            dblClickedTimeout = setTimeout(function () {
                                dblClickedTimeout = null;
                                clearSingleEventCell(cell);
                            }, doubleClickDelay);
                        } else {
                            onCellSelect(e);
                            return;
                        }
                    }
                    else
                        onDragEnd(creatingEventCells);
                }
            });

            bWeeklyCalendarLoaded = true;
            if (bPendingSchedulerLoading)
                loadCurrentScheduler();
        });

        function openWeeklyContextMenu(activeCell, bForceShow) {
            weeklyContextMenu = contextMenuEdit.dxContextMenu({
                dataSource: [],
                target: activeCell ? activeCell : ".cellTimeCol:not(.daylabel)",
                onPositioning: function (e) {
                    rightClickedCell = e.event ? e.event.target : activeCell;
                    var $clicked = $(rightClickedCell);
                    if ($clicked.hasClass("eventCell"))
                        e.component.option('dataSource', [
                            { text: translations.AddNewWeekly() },
                            { text: translations.EditWeekly() },
                            { text: translations.DeleteWeekly() }
                        ]);
                    else {
                        deselectCells([$clicked]);
                        bIgnoreNextTouchEnd = true;
                        selectionFirstCell = null;
                        selectionLastCell = null;
                        e.component.option('dataSource', [
                            { text: translations.AddNewWeekly() }
                        ]);
                    }
                },
                onHidden: function (e) {
                    try {
                        contextMenuEdit.dxContextMenu("dispose");
                    } catch { }
                },
                onItemClick: function (e) {
                    var cell = $(rightClickedCell);
                    var ev = findCellEvents(cell);
                    if (e.itemIndex === 2 && ev.events.length === 1) {
                        clearWeeklyEvent(ev.events[0].eventID);
                    }
                    else if (e.itemIndex === 0)
                        addNewEvent(cell, cell);
                    else
                        onCellSelect({ target: rightClickedCell });
                }
            }).dxContextMenu("instance");
            if (bForceShow)
                weeklyContextMenu.show();
        }

        var onCellSelect = function (e) {
            if (dblClickedTimeout) {
                clearTimeout(dblClickedTimeout);
                dblClickedTimeout = null;
            }
            if (tapped) {
                clearTimeout(tapped);
                tapped = null;
            }

            var cell = $(e.target);
            if (cell.hasClass("eventCell")) {
                var ret = findCellEvents(cell);
                if (ret.events.length == 1) {
                    var event = ret.events[0];
                    openWeeklyPopup(event.eventID, event.StartDate, event.EndDate);
                }
                else {
                    var choicePopup = eventChoicePopup.dxPopup({
                        visible: true,
                        title: i18next.t1('ChooseEventTitle'),
                        width: 450,
                        height: 'auto',
                        position: {
                            my: 'center',
                            at: 'center',
                            of: window
                        },
                        showCloseButton: true,
                        dragEnabled: true,
                        contentTemplate: function (e) {
                            var templateContainer = $("<div style='text-align: center;'>");
                            var html = "";
                            $.each(ret.events, function (index, item) {
                                var templateContent = $("<div style='height: 50px; width: 100%; display: table;' class='eventRow'>").appendTo(templateContainer);
                                var datesLabel = item.StartDate.toLocaleDateString(navigator.language, dateStringOptions) + " - " + item.EndDate.toLocaleDateString(navigator.language, dateStringOptions);
                                templateContent.append("<div style='display:table-cell; vertical-align:middle;'>" + datesLabel + "</div>");
                                $("<div style='margin: 5px;'>").dxButton({
                                    icon: "edit", //more
                                    onClick: function (e) {
                                        eventChoicePopup.dxPopup("dispose");
                                        openWeeklyPopup(item.eventID, item.StartDate, item.EndDate);
                                    }
                                }).appendTo(templateContent);
                                $("<div style='margin: 5px;'>").dxButton({
                                    icon: "remove",
                                    onClick: function (e) {
                                        clearWeeklyEvent(item.eventID);
                                        e.component.element().closest(".eventRow").remove();
                                    }
                                }).appendTo(templateContent);
                            });
                            var lastLine = $("<div style='width: 100%;'>").appendTo(templateContainer);
                            var plusButton = $("<div style='margin-top: 10px;'>").dxButton({
                                icon: "add",
                                width: "100%",
                                onClick: function (e) {
                                    eventChoicePopup.dxPopup("dispose");
                                    addNewEvent(cell, cell);
                                }
                            }).appendTo(lastLine);
                            var minusButton = $("<div style='margin-top: 5px;'>").dxButton({
                                icon: "minus",
                                width: "100%",
                                onClick: function (e) {
                                    var result = DevExpress.ui.dialog.confirm(i18next.t1("RemoveAllListedWeeklyItems"), i18next.t1("UserConfirmationNeededTitle"));
                                    result.done(function (dialogResult) {
                                        if (dialogResult) {
                                            $.each(ret.events, function (index, item) {
                                                clearWeeklyEvent(item.eventID);
                                            });
                                            eventChoicePopup.dxPopup("dispose");
                                        }
                                    });
                                }
                            }).appendTo(lastLine);
                            var okButton = $("<div style='margin-top: 10px;'>").dxButton({
                                text: i18next.t1("OkText"),
                                width: "33%",
                                onClick: function (e) {
                                    eventChoicePopup.dxPopup("dispose");
                                }
                            }).appendTo(lastLine);
                            return templateContainer;
                        }
                    }).dxPopup("instance");
                }
            }
            else {
                addNewEvent(cell, cell);
            }
        }

        function getCellDatesInterval(startCell, endCell) {
            var startCelldayIndex = startCell.attr("dayindex");
            var startCellRelativeIndex = startCell.attr("colrelativeindex");
            var startSelectionTime = moment(weeklyViewStartDate).add(weeklyCellMinutes * (startCellRelativeIndex - 1), "minutes").add(startCelldayIndex, "days").toDate();
            var endSelectionTime;
            if (startCell == endCell)
                endSelectionTime = moment(startSelectionTime).add(weeklyCellMinutes, "minutes").toDate();
            else {
                var endCelldayIndex = endCell.attr("dayindex");
                var endCellRelativeIndex = endCell.attr("colrelativeindex");
                endSelectionTime = moment(weeklyViewStartDate).add(weeklyCellMinutes * (endCellRelativeIndex), "minutes").add(endCelldayIndex, "days").toDate();
            }
            return {
                StartDate: startSelectionTime,
                EndDate: endSelectionTime
            };
        }

        function addNewEvent(startCell, endCell, overlappedEvents, bSilent = false) {
            var datesInterval = getCellDatesInterval(startCell, endCell);
            if (bSilent)
                addWeeklyEvent(datesInterval.StartDate, datesInterval.EndDate, undefined, undefined, false, overlappedEvents);
            else
                openWeeklyPopup(null, datesInterval.StartDate, datesInterval.EndDate);
        }

        function openWeeklyPopup(eventID, startSelectionTime, endSelectionTime) {
            var weeklypopup = eventPopup.dxPopup({
                visible: true,
                width: 550,
                height: 380,
                position: {
                    my: 'center',
                    at: 'center',
                    of: window
                },
                showCloseButton: true,
                dragEnabled: true,
                contentTemplate: function (e) {
                    var formContainer = $("<div id='form'>");
                    eventEditForm = formContainer.dxForm({
                        readOnly: false,
                        showColonAfterLabel: false,
                        labelLocation: "top",
                        minColWidth: 200,
                        showValidationSummary: true,
                        scrollingEnabled: true,
                        colCount: 3,
                        items: [
                            {
                                editorOptions: { disabled: true },
                                template: function (data, elem) {
                                    $("<span>").text(translations.WeeklyItemAllDayEvent()).appendTo(elem);
                                    return elem;
                                }
                            },
                            {
                                dataField: "allday",
                                label: { visible: false },
                                editorType: "dxSwitch",
                                editorOptions: {
                                    onValueChanged: function (e) {
                                        eventEditForm.getEditor("StartDate").option("disabled", e.value);
                                        eventEditForm.getEditor("EndDate").option("disabled", e.value);
                                    }
                                }
                            },
                            {
                                itemType: "empty",
                                colSpan: 1
                            },
                            {
                                editorOptions: { disabled: true },
                                template: function (data, elem) {
                                    $("<span>").text(translations.WeeklyItemEditFormStartTime()).appendTo(elem);
                                    return elem;
                                }
                            },
                            {
                                dataField: "startday",
                                editorType: "dxSelectBox",
                                label: { visible: false },
                                editorOptions: {
                                    items: daysFullNames,
                                    value: getDayFullName(startSelectionTime),
                                    onInitialized: function (sc) {
                                        editFormStartDayCombo = sc.component;
                                    },
                                    onSelectionChanged: function (sc) {
                                        if (!editFormStartDayCombo || !editFormEndDayCombo)
                                            return;

                                        var editFormStartDayIndex = editFormStartDayCombo.option("items").indexOf(sc.selectedItem);
                                        if (editFormStartDayIndex == 0)
                                            editFormStartDayIndex = 8;
                                        var editFormEndDayIndex = editFormEndDayCombo.option("items").indexOf(editFormEndDayCombo.option("selectedItem"));
                                        if (editFormEndDayIndex == 0)
                                            editFormEndDayIndex = 8;
                                        if (editFormEndDayIndex < editFormStartDayIndex)
                                            editFormEndDayCombo.option("value", sc.selectedItem);
                                    },
                                    displayExpr: function (item) {
                                        if ("Week" + item in translations)
                                            return translations["Week" + item]();
                                        return item;
                                    }
                                }
                            },
                            {
                                dataField: "StartDate",
                                editorType: "dxDateBox",
                                label: { visible: false },
                                editorOptions: {
                                    showClearButton: false,
                                    useMaskBehavior: true,
                                    pickerType: scope.bIsMobile ? "rollers" : "list",
                                    type: /*scope.bIsMobile ? "time" :*/ "datetime",
                                    calendarOptions: {
                                        visible: false
                                    },
                                    displayFormat: function (date) {
                                        return moment(date).locale(navigator.language).format("LTS");
                                    },
                                    onInitialized: function (sc) {
                                        editFormStartTime = sc.component;
                                        editFormStartTime.option("value", startSelectionTime);
                                    }
                                },
                                validationRules: [{
                                    type: "required",
                                    message: i18next.t1("StartDateRequired")
                                },
                                {
                                    type: "custom",
                                    validationCallback: function (e) {
                                        var startDate = e.value;
                                        var endDate = eventEditForm.getEditor("EndDate").option("value");
                                        var startDay = eventEditForm.getEditor("startday").option("value");
                                        var endDay = eventEditForm.getEditor("endday").option("value");

                                        var chosenStartDay = daysFullNames.indexOf(startDay);
                                        if (chosenStartDay === 0)
                                            chosenStartDay = 7;
                                        var distance = chosenStartDay - startDate.getDay();
                                        startDate.setDate(startDate.getDate() + distance);

                                        var chosenEndDay = daysFullNames.indexOf(endDay);
                                        if (chosenEndDay === 0)
                                            chosenEndDay = 7;
                                        var distance = chosenEndDay - endDate.getDay();
                                        endDate.setDate(endDate.getDate() + distance);

                                        if (endDate > startDate)
                                            eventEditForm.getEditor("EndDate").option("isValid", true);

                                        return endDate > startDate;
                                    },
                                    message: i18next.t1("StartDateMustBeLower")
                                }]
                            },
                            {
                                editorOptions: { disabled: true },
                                template: function (data, elem) {
                                    $("<span>").text(translations.WeeklyItemEditFormEndTime()).appendTo(elem);
                                    return elem;
                                }
                            },
                            {
                                dataField: "endday",
                                editorType: "dxSelectBox",
                                label: { visible: false },
                                editorOptions: {
                                    items: daysFullNames,
                                    value: getDayFullName(endSelectionTime),
                                    onInitialized: function (sc) {
                                        editFormEndDayCombo = sc.component;
                                    },
                                    //onContentReady: function (e) {
                                    //    e.component.element().find("input").css(fontOptions);
                                    //},
                                    onSelectionChanged: function (sc) {
                                        if (!editFormStartDayCombo || !editFormEndDayCombo)
                                            return;

                                        var editFormStartDayIndex = editFormStartDayCombo.option("items").indexOf(editFormStartDayCombo.option("selectedItem"));
                                        if (editFormStartDayIndex == 0)
                                            editFormStartDayIndex = 8;
                                        var editFormEndDayIndex = editFormEndDayCombo.option("items").indexOf(sc.selectedItem);
                                        if (editFormEndDayIndex == 0)
                                            editFormEndDayIndex = 8;
                                        if (editFormEndDayIndex < editFormStartDayIndex)
                                            editFormStartDayCombo.option("value", sc.selectedItem);
                                    },
                                    displayExpr: function (item) {
                                        if ("Week" + item in translations)
                                            return translations["Week" + item]();
                                        return item;
                                    }
                                }
                            },
                            {
                                dataField: "EndDate",
                                editorType: "dxDateBox",
                                label: { visible: false },
                                editorOptions: {
                                    showClearButton: false,
                                    useMaskBehavior: true,
                                    pickerType: scope.bIsMobile ? "rollers" : "list",
                                    type: /*scope.bIsMobile ? "time" :*/ "datetime",
                                    calendarOptions: {
                                        visible: false
                                    },
                                    displayFormat: function (date) {
                                        return moment(date).locale(navigator.language).format("LTS");
                                    },
                                    onInitialized: function (sc) {
                                        editFormEndTime = sc.component;
                                        editFormEndTime.option("value", endSelectionTime);
                                    }
                                },
                                validationRules: [{
                                    type: "required",
                                    message: i18next.t1("EndDateRequired")
                                },
                                {
                                    type: "custom",
                                    validationCallback: function (e) {
                                        var startDate = eventEditForm.getEditor("StartDate").option("value");
                                        var endDate = e.value;
                                        var startDay = eventEditForm.getEditor("startday").option("value");
                                        var endDay = eventEditForm.getEditor("endday").option("value");

                                        var chosenStartDay = daysFullNames.indexOf(startDay);
                                        if (chosenStartDay === 0)
                                            chosenStartDay = 7;
                                        var distance = chosenStartDay - startDate.getDay();
                                        startDate.setDate(startDate.getDate() + distance);

                                        var chosenEndDay = daysFullNames.indexOf(endDay);
                                        if (chosenEndDay === 0)
                                            chosenEndDay = 7;
                                        var distance = chosenEndDay - endDate.getDay();
                                        endDate.setDate(endDate.getDate() + distance);

                                        if (endDate > startDate)
                                            eventEditForm.getEditor("StartDate").option("isValid", true);

                                        return endDate > startDate;
                                    },
                                    message: i18next.t1("EndDateMustBeGreater")
                                }]
                            },
                            {
                                editorOptions: { disabled: true },
                                template: function (data, elem) {
                                    $("<span>").text(translations.WeeklyItemApplyRecurrence()).appendTo(elem);
                                    return elem;
                                }
                            },
                            {
                                label: { visible: false },
                                editorType: "dxSwitch",
                                dataField: "repeatevent",
                                editorOptions: {
                                    onInitialized: function (sc) {
                                        repeatSwitch = sc.component;
                                    },
                                    onValueChanged: function (e) {
                                        eventEditForm.getEditor("repeatmode").option("disabled", !e.value);
                                        if (!e.value)
                                            eventEditForm.getEditor("repeateverydays").option("disabled", true);
                                        else if (eventEditForm.getEditor("repeatmode").option("value") === translations.RecurrenceFiledEvery())
                                            eventEditForm.getEditor("repeateverydays").option("disabled", false);
                                    }
                                }
                            },
                            {
                                itemType: "empty",
                                colSpan: 1
                            },
                            {
                                colSpan: 1,
                                editorType: "dxRadioGroup",
                                dataField: "repeatmode",
                                label: { visible: false },
                                editorOptions: {
                                    layout: "vertical",
                                    items: [
                                        translations.RecurrenceFiledEvery(),
                                        translations.RecurrenceFiledEveryWeekDays()
                                    ],
                                    value: i18next.t1("EveryWeekdayTitle"),
                                    disabled: repeatSwitch ? (!repeatSwitch.option("value")) : true,
                                    onInitialized: function (e) {
                                        if (repeatSwitch)
                                            e.component.option("disabled", !repeatSwitch.option("value"));
                                    },
                                    onValueChanged: function (e) {
                                        if (eventEditForm.getEditor("repeateverydays"))
                                            eventEditForm.getEditor("repeateverydays").option("disabled", e.value !== translations.RecurrenceFiledEvery());
                                    }
                                }
                            },
                            {
                                colSpan: 1,
                                label: { text: translations.RecurrenceFiledDays() },
                                editorType: "dxNumberBox",
                                dataField: "repeateverydays",
                                editorOptions: {
                                    min: 1,
                                    max: 7,
                                    value: 1,
                                    showSpinButtons: true,
                                    disabled: true
                                }
                            },
                            {
                                itemType: "empty",
                                colSpan: 1
                            },
                            {
                                itemType: "empty",
                                colSpan: 3
                            },
                            {
                                colSpan: 1,
                                itemType: "button",
                                horizontalAlignment: "center",
                                buttonOptions: {
                                    text: translations.WeeklyItemEditFormOk(),
                                    width: "100%",
                                    type: "normal", //default
                                    onClick: function (e) {
                                        //var val = eventEditForm.validate();
                                        //if (!val.isValid)
                                        //    return;

                                        if (!eventEditForm.getEditor("StartDate").option("isValid") || !eventEditForm.getEditor("EndDate").option("isValid")) {
                                            DevExpress.ui.notify({ message: i18next.t1("EndDateMustBeGreater"), width: 400, shading: true }, "error", 1000);
                                            return;
                                        }

                                        var startFormDate = eventEditForm.getEditor("StartDate").option("value");
                                        var endFormDate = eventEditForm.getEditor("EndDate").option("value");

                                        if (parameters.ShowWorkTimeOnly) {
                                            var startFormDateTime = new Date(weekMonday.year, weekMonday.month, weekMonday.day, startFormDate.getHours(), startFormDate.getMinutes(), startFormDate.getSeconds());
                                            var endFormDateTime = new Date(weekMonday.year, weekMonday.month, weekMonday.day, endFormDate.getHours(), endFormDate.getMinutes(), endFormDate.getSeconds());
                                            
                                            let bTooEarly = weeklyViewStartDate > startFormDateTime;
                                            let bTooLate = weeklyViewEndDate < endFormDateTime;
                                            let msgKey = "";
                                            if (bTooEarly && bTooLate)
                                                msgKey = "EditWarningOutOfWorkView";
                                            else if (bTooEarly)
                                                msgKey = "EditWarningBeforeWorkView";
                                            else if (bTooLate)
                                                msgKey = "EditWarningAfterWorkView";
                                            if (msgKey)
                                                DevExpress.ui.notify({ message: i18next.t1(msgKey), width: 400, shading: true }, "warning", 3000);
                                        }

                                        var startDay = eventEditForm.getEditor("startday").option("value");
                                        var endDay = eventEditForm.getEditor("endday").option("value");

                                        var chosenStartDay = daysFullNames.indexOf(startDay);
                                        if (chosenStartDay === 0)
                                            chosenStartDay = 7;
                                        var distance = chosenStartDay - startFormDate.getDay();
                                        startFormDate.setDate(startFormDate.getDate() + distance);

                                        var chosenEndDay = daysFullNames.indexOf(endDay);
                                        if (chosenEndDay === 0)
                                            chosenEndDay = 7;
                                        var distance = chosenEndDay - endFormDate.getDay();
                                        endFormDate.setDate(endFormDate.getDate() + distance);

                                        if (eventID) //editing event
                                            clearWeeklyEvent(eventID);

                                        var repeatEveryDays = 0;
                                        if (eventEditForm.getEditor("repeatevent").option("value")) {
                                            var mode = eventEditForm.getEditor("repeateverydays").option("disabled") ? "everyweekday" : "everydays";
                                            if (mode === "everydays" && eventEditForm.getEditor("repeateverydays").option("value") > 0)
                                                repeatEveryDays = eventEditForm.getEditor("repeateverydays").option("value");
                                            else { //everyweekday
                                                startFormDate.setDate(7);
                                                endFormDate.setDate(7);
                                                repeatEveryDays = 1;
                                            }
                                        }

                                        if (repeatEveryDays > 0) {
                                            repeatEvent(startFormDate, endFormDate, undefined, undefined, repeatEveryDays);
                                        }
                                        else {
                                            addWeeklyEvent(startFormDate, endFormDate);
                                        }
                                        weeklypopup.dispose();
                                        eventEditForm.dispose();
                                    }
                                }
                            },
                            {
                                colSpan: 1,
                                itemType: "button",
                                horizontalAlignment: "center",
                                buttonOptions: {
                                    text: translations.WeeklyItemEditFormCancel(),
                                    width: "100%",
                                    type: "normal",
                                    onClick: function (e) {
                                        weeklypopup.dispose();
                                        eventEditForm.dispose();
                                    }
                                }
                            },
                            {
                                colSpan: 1,
                                itemType: "button",
                                horizontalAlignment: "center",
                                disabled: eventID === null,
                                buttonOptions: {
                                    text: translations.WeeklyItemEditFormDelete(),
                                    width: "100%",
                                    type: "normal", //danger
                                    onClick: function (e) {
                                        if (eventID)
                                            clearWeeklyEvent(eventID);
                                        weeklypopup.dispose();
                                        eventEditForm.dispose();
                                    }
                                }
                            }
                        ]
                    }).dxForm("instance");
                    e.append(formContainer);
                }
            }).dxPopup("instance");
        }

        var lastLeavedCell;
        var selectionFirstCell;
        var selectionLastCell;
        var touchSelecting;

        function checkAddOverlappedEvents(overlappedEvents, cell) {
            var newSet = overlappedEvents;
            if (cell.hasClass("eventCell")) {
                var events = findCellEvents(cell).events;
                newSet = [...new Set([...overlappedEvents, ...events])];
            }
            return newSet;
        }

        function onDragEnd(creatingEventCells) {
            var overlappedEvents = [];
            
            var prevCell = creatingEventCells.first().prev().filter(".cellTimeCol:not(.daylabel)"); //adjacent cells events may may be merged with the event being created
            var nextCell = creatingEventCells.last().next().filter(".cellTimeCol:not(.daylabel)");
            if (prevCell.length > 0)
                overlappedEvents = checkAddOverlappedEvents(overlappedEvents, prevCell);
            if (nextCell.length > 0)
                overlappedEvents = checkAddOverlappedEvents(overlappedEvents, nextCell);

            $.each(creatingEventCells, function (index, cell) {
                cell = $(cell);
                overlappedEvents = checkAddOverlappedEvents(overlappedEvents, cell); //events of the cells being dragged over may be merged with the event being created
                cell.removeClass("creatingEvent");
                resetBackground(cell, true);
            });

            lastLeavedCell = null;
            var bGoingBackward = false;
            if (selectionLastCell)
                bGoingBackward = selectionFirstCell.index() > selectionLastCell.index();
            var firstCell = bGoingBackward ? selectionLastCell : selectionFirstCell;
            var lastCell = bGoingBackward ? selectionFirstCell : (selectionLastCell ? selectionLastCell : selectionFirstCell);
            addNewEvent(firstCell, lastCell, overlappedEvents, true);
            selectionFirstCell = null;
            selectionLastCell = null;
        }

        function onCellMove(target) {
            let $target = $(target);
            if (!$target.hasClass("creatingEvent")) {
                    if (!selectionFirstCell)
                        selectionFirstCell = $target;
                    selectionLastCell = $target;
                    if (selectionLastCell == selectionFirstCell)
                        return;
                    if (selectionLastCell.index() > selectionFirstCell.index()) {
                        deselectCells(selectionFirstCell.prevAll(".cellTimeCol.creatingEvent"));
                        selectionFirstCell.nextUntil(selectionLastCell).filter(".cellTimeCol:not(.daylabel)").addClass("creatingEvent").css("background-color", eventBackgroundColor);
                    }
                    else if (selectionLastCell.index() < selectionFirstCell.index()) {
                        deselectCells(selectionFirstCell.nextAll(".cellTimeCol.creatingEvent"));
                        selectionLastCell.nextUntil(selectionFirstCell).filter(".cellTimeCol:not(.daylabel)").addClass("creatingEvent").css("background-color", eventBackgroundColor);
                    }
                    $target.addClass("creatingEvent").css("background-color", eventBackgroundColor);
            }
            else {
                var bForwardDeleting;
                var bBackwardDeleting;
                var deselectingCells;
                if (parseFloat(lastLeavedCell.attr("dayindex")) == parseFloat($target.attr("dayindex"))) {
                    bForwardDeleting = parseFloat(lastLeavedCell.attr("colindex")) == parseFloat($target.attr("colindex")) + 1;
                    bBackwardDeleting = parseFloat(lastLeavedCell.attr("colindex")) == parseFloat($target.attr("colindex")) - 1;
                    if (bForwardDeleting || bBackwardDeleting) {
                        lastLeavedCell.removeClass("creatingEvent");
                        resetBackground(lastLeavedCell, true);
                        if (bForwardDeleting)
                            deselectingCells = lastLeavedCell.nextAll(".cellTimeCol.creatingEvent");
                        else
                            deselectingCells = lastLeavedCell.prevAll(".cellTimeCol.creatingEvent");
                    }
                }
                else {
                    if (parseFloat(lastLeavedCell.attr("colindex")) > parseFloat($(target).attr("colindex")))
                        deselectingCells = $target.nextAll(".cellTimeCol.creatingEvent");
                    else
                        deselectingCells = $target.prevAll(".cellTimeCol.creatingEvent");
                }
                if (deselectingCells) {
                    deselectCells(deselectingCells);
                    selectionLastCell = $target;
                }
            }
        }

        function deselectCells(deselectingCells) {
            $.each(deselectingCells, function (index, item) {
                $(item).removeClass("creatingEvent");
                resetBackground(item, true);
            });
        }

        function clearSingleEventCell(cell) {
            var cellEvent = findCellEvents(cell).events[0];
            var eventInterval = findIntervalByEventID(cellEvent.eventID);
            var cellInterval = getCellDatesInterval(cell, cell);

            var bClearingFirstCell = cell[0] == weeklyViewDiv.find(".cellTimeCol[colindex=" + eventInterval.startColindex + "]")[0];
            var bClearingLastCell = cell[0] == weeklyViewDiv.find(".cellTimeCol[colindex=" + eventInterval.endColindex + "]")[0];
            var firstEventStartDate = bClearingFirstCell ? cellInterval.EndDate : cellEvent.StartDate;
            var firstEventEndDate;
            var secondEventStartDate;
            var secondEventEndDate = cellEvent.EndDate;
            var bAddFirstEvent;
            var bAddSecondEvent;
            if (!bClearingFirstCell || !bClearingLastCell) {
                bAddFirstEvent = true;
                if (bClearingFirstCell)
                    firstEventEndDate = cellEvent.EndDate;
                else if (bClearingLastCell)
                    firstEventEndDate = cellInterval.StartDate;
                else { //deleting middle cell
                    bAddSecondEvent = true;
                    firstEventEndDate = cellInterval.StartDate;
                    secondEventStartDate = cellInterval.EndDate;
                }
            }

            clearWeeklyEvent(cellEvent.eventID);

            if (bAddFirstEvent) {
                addWeeklyEvent(firstEventStartDate, firstEventEndDate);
            }
            if (bAddSecondEvent) {
                addWeeklyEvent(secondEventStartDate, secondEventEndDate);
            }
        }
    }

    function resetBackground(cell, bDeselecting = false) {
        cell = jQuerize(cell);
        if (!bDeselecting || !$(cell).hasClass("eventCell"))
            $(cell).css({
                "background": "initial",
                "background-color": parseFloat($(cell).attr("dayindex")) % 2 === 0 ? oddRowsBackground : "initial"
            });
    }

    function clearWeeklyEvent(eid) {
        bDirty = true;
        currentWeeklyEvents = currentWeeklyEvents.filter(function (obj) { return obj.eventID !== eid });

        var tooltipElem = thisPage.find("#tooltip_" + thisParentId + "_" + eid);
        tooltipElem.dxTooltip("dispose");
        tooltipElem.remove();

        var cells = weeklyViewDiv.find(".eventCell.eventid" + eid);
        cells.removeClass("eventid" + eid);

        $.each(cells, function (index, cell) {
            var cellEventsNo = $(cell).attr("class").split(" ").filter(function (item) { return item.startsWith("eventid"); }).length;
            if (cellEventsNo < 2)
                $(cell).removeClass("multiEventCell");
            if (cellEventsNo === 0) {
                $(cell).removeClass("eventCell");
                resetBackground(cell);
            }
            else if (cellEventsNo === 1) {
                $(cell).css({
                    "background": "initial",
                    "background-color": eventBackgroundColor
                });
            }
        });
        removeMapColindexItem(eid);
    }

    function clearWeeklyEvents() {
        currentWeeklyEvents = [];
        mapColindex = [];
        thisPage.find(".weeklyTooltip").each(function (index, item) {
            var tooltipElem = $("#" + $(item).attr("id"));
            tooltipElem.dxTooltip("dispose");
            tooltipElem.remove();
        });
        weeklyViewDiv.find(".eventCell").each(function (index, item) {
            $.each($(item).attr("class").split(" "), function (ci, cl) {
                if (cl == "eventCell" || cl.startsWith("eventid") || cl == "multiEventCell")
                    $(item).removeClass(cl);
            });
            resetBackground(item);
        });
        weeklyViewDiv.find(".creatingEvent").each(function (index, item) {
            $(item).removeClass("creatingEvent");
            resetBackground(item);
        });
    }

    function findIntervalByEventID(eventID) {
        var interval;
        $.each(mapColindex, function (index, item) {
            if (item.events.findIndex(function (el) { return el.eventID == eventID }) !== -1) {
                interval = item;
                return false;
            }
        });
        return interval;
    }

    var findCellEvents = function (cell) {
        var ret = { "events": [] };
        var eids;
        try {
            eids = cell.attr("class").split(" ").filter(function (item) { return item.startsWith("eventid"); }).map(function (item) { return item.substr(7); });
        }
        catch { }
        
        if (undefinedOrNull(eids))
            return ret;
        
        $(eids).each(function (index, item) {
            ret.events.push(currentWeeklyEvents.find(function (obj) { return obj.eventID === item }));
        });

        return ret;
    }

    function removeMapColindexItem(eventID) {
        var ev = findIntervalByEventID(eventID);
        if (!undefinedOrNull(ev)) {
            ev.events.splice(ev.events.findIndex(function (index, item) { return item.eventID == eventID }), 1);
            if (ev.events.length == 0)
                mapColindex.splice(mapColindex.indexOf(ev), 1);
            else { //recalculating interval's startColindex, endColindex
                ev.startColindex = ev.events.reduce(function (prev, curr) {
                    return prev.startColindex < curr.startColindex ? prev : curr;
                }).startColindex;
                ev.endColindex = ev.events.reduce(function (prev, curr) {
                    return prev.endColindex > curr.endColindex ? prev : curr;
                }).endColindex;
            }
        }
    }

    function endsAtMidnight(date) {
        return date.getHours() == date.getMinutes() == date.getSeconds() == 0;
    }

    function findStartEndColIndex(eventStartDate, eventEndDate, eventID) {
        var roundedStartMinutes = ((Math.round(eventStartDate.getMinutes() / weeklyCellMinutes) * weeklyCellMinutes) % 60) + startM_quarterDiff;
        var roundedEndMinutes = ((Math.round(eventEndDate.getMinutes() / weeklyCellMinutes) * weeklyCellMinutes) % 60) + startM_quarterDiff;
        var roundedStartDate = new Date(eventStartDate.getTime());
        var roundedEndDate = new Date(eventEndDate.getTime());
        roundedStartDate.setHours(eventStartDate.getHours(), roundedStartMinutes, 0, 0);
        roundedEndDate.setHours(eventEndDate.getHours(), roundedEndMinutes, 0, 0);
        var bEndsAtMidnight = endsAtMidnight(roundedEndDate);

        var eventStartWeekDay = roundedStartDate.getDay() == 0 ? 6 : roundedStartDate.getDay() - 1;
        var eventEndWeekDay = roundedEndDate.getDay() == 0 ? 6 : roundedEndDate.getDay() - 1;

        var dayStart_StartDate = moment(weeklyViewStartDate).add(eventStartWeekDay, "days").toDate();
        var dayEnd_StartDate = moment(weeklyViewStartDate).add(eventEndWeekDay, "days").toDate();
        
        var startColRelativeIndex = Math.floor((moment(new Date(weekMonday.year, weekMonday.month, weekMonday.day, eventStartDate.getHours(), eventStartDate.getMinutes(), eventEndDate.getSeconds())).add(eventStartWeekDay, "days").toDate() - dayStart_StartDate) / weeklyCellMs) + 1;
        if (parameters.ShowWorkTimeOnly)
            startColRelativeIndex = Math.max(startColRelativeIndex, 1);
        var endColRelativeIndex = Math.ceil((moment(new Date(weekMonday.year, weekMonday.month, weekMonday.day, eventEndDate.getHours(), eventEndDate.getMinutes(), eventEndDate.getSeconds())).add(eventEndWeekDay, "days").toDate() - dayEnd_StartDate) / weeklyCellMs) + 1;
        if (eventStartWeekDay == eventEndWeekDay)
            endColRelativeIndex = Math.max(endColRelativeIndex, startColRelativeIndex + 1);

        var startCell = weeklyViewDiv.find('.cellTimeCol[dayindex="' + eventStartWeekDay + '"][colrelativeindex="' + startColRelativeIndex + '"]');
        var bEndsOnLastCell = endColRelativeIndex == 1 && eventEndWeekDay == 0 && bEndsAtMidnight;

        var i = 0;
        var currentCell = startCell;
        while (i < weeklyTotalCells) {
            if (!bEndsOnLastCell) {
                var colRelativeIndex = currentCell.attr("colrelativeindex");
                var dayIndex = currentCell.attr("dayindex");
                var bCellFound = (colRelativeIndex == endColRelativeIndex && dayIndex == eventEndWeekDay);
                var bCellOverstepped = (colRelativeIndex > endColRelativeIndex && dayIndex == eventEndWeekDay) || dayIndex > eventEndWeekDay;
                if (bCellFound || bCellOverstepped)
                    break;
            }
            if (!currentCell.hasClass("daylabel")) {
                if (currentCell.hasClass("eventCell"))
                    currentCell.css("background", multiEventBackgroundColor).addClass("multiEventCell");
                else
                    currentCell.css("background-color", eventBackgroundColor).addClass("eventCell " + thisParentId);
                currentCell.addClass("eventid" + eventID);
            }
            currentCell = currentCell.next();
            i++;
        }

        var startColindex = parseInt(startCell.attr("colindex"), 10);
        var endColindex = parseInt(startColindex, 10) + i - 1;
        if (bEndsAtMidnight)
            endColindex -= 1;
        endColindex = endColindex >= startColindex ? Math.min(weeklyTotalCells, endColindex) : weeklyTotalCells;
        return {
            start: startColindex,
            end: endColindex
        }
    }

    function checkDatesOverlappedByDay(firstStart, firstEnd, secondStart, secondEnd) {
        var firstStartDay = getNormalizedDay(firstStart);
        var firstEndDay = getNormalizedDay(firstEnd);
        var secondStartDay = getNormalizedDay(secondStart);
        var secondEndDay = getNormalizedDay(secondEnd);
        if (firstEndDay < secondStartDay || firstStartDay > secondStartDay)
            return false;
        var firstStartDatetime = getNormalizedWeekDate(firstStart);
        var firstEndDatetime = getNormalizedWeekDate(firstEnd);
        var secondStartDatetime = getNormalizedWeekDate(secondStart);
        var secondEndDatetime = getNormalizedWeekDate(secondEnd);

        var bBeginsBeforeEnd = secondStartDay < firstEndDay || (secondStartDay == firstEndDay && secondStartDatetime <= firstEndDatetime);
        var bEndsAfterStart = secondEndDay > firstStartDay || (secondEndDay == firstStartDay && secondEndDatetime >= firstStartDatetime);
        return bBeginsBeforeEnd && bEndsAfterStart;
    }

    function getNormalizedDay(date) {
        var day = date.getDay();
        if (endsAtMidnight(date) && day == 1)
            return 8;
        return day == 0 ? 7 : day;
    }

    function getNormalizedWeekDate(date) {
        return new Date(actYear, 0, 1, date.getHours(), date.getMinutes(), date.getSeconds(), date.getMilliseconds());
    }

    var compareDateByDay = {
        min: function (date1, date2) {
            var firstDay = getNormalizedDay(date1);
            var secondDay = getNormalizedDay(date2);
            if (firstDay != secondDay)
                return firstDay < secondDay ? date1 : date2;
            var date1Datetime = getNormalizedWeekDate(date1);
            var date2Datetime = getNormalizedWeekDate(date2);
            return date1Datetime < date2Datetime ? date1 : date2;
        },
        max: function (date1, date2) {
            var firstDay = getNormalizedDay(date1);
            var secondDay = getNormalizedDay(date2);
            if (firstDay != secondDay)
                return firstDay > secondDay ? date1 : date2;
            var date1Datetime = getNormalizedWeekDate(date1);
            var date2Datetime = getNormalizedWeekDate(date2);
            return date1Datetime > date2Datetime ? date1 : date2;
        }
    }

    function addWeeklyEvent(eventStartDate, eventEndDate, eventID, eventIndex, bInitialDataLoading, overlappedEvents) {
        if (!bInitialDataLoading)
            bDirty = true;

        if (!eventID) {
            if (parameters.ShowWorkTimeOnly) {
                var startDay = eventStartDate.getDay();
                var endDay = eventEndDate.getDay();
                if (startDay != endDay) {
                    startDay = startDay == 0 ? 7 : startDay;
                    endDay = endDay == 0 ? 7 : endDay;
                    var newStartDate = eventStartDate;
                    var newEndDate = new Date(eventStartDate.getFullYear(), eventStartDate.getMonth(), eventStartDate.getDate(), weeklyViewEndDate.getHours(), weeklyViewEndDate.getMinutes(), weeklyViewEndDate.getSeconds());
                    for (var i = startDay; i <= endDay; i++) {
                        addWeeklyEvent(newStartDate, newEndDate, undefined, undefined, bInitialDataLoading, overlappedEvents);
                        var startDayPlusOne = moment(newStartDate).add(1, "days").toDate();
                        newStartDate = new Date(startDayPlusOne.getFullYear(), startDayPlusOne.getMonth(), startDayPlusOne.getDate(), weeklyViewStartDate.getHours(), weeklyViewStartDate.getMinutes(), weeklyViewStartDate.getSeconds());
                        if (i == endDay - 1)
                            newEndDate = eventEndDate;
                        else
                            newEndDate = new Date(startDayPlusOne.getFullYear(), startDayPlusOne.getMonth(), startDayPlusOne.getDate(), weeklyViewEndDate.getHours(), weeklyViewEndDate.getMinutes(), weeklyViewEndDate.getSeconds());
                    }
                    DevExpress.ui.dialog.alert(i18next.t1("SchedulerEventSplittedToWorkView"), i18next.t1("Warning"));
                    return;
                }
            }
            var newEventData = getNewEventData();
            eventID = newEventData.neweventid;
            eventIndex = newEventData.neweventindex;
        }

        var newEventEntry = {
            "eventID": eventID,
            "eventIndex": eventIndex,
            "StartDate": eventStartDate,
            "EndDate": eventEndDate
        };

        var potentialOverlaps = currentWeeklyEvents;
        if (overlappedEvents)
            potentialOverlaps = overlappedEvents;
        var eventsToMerge = [];
        if (potentialOverlaps.length > 0)
            eventsToMerge = potentialOverlaps.filter(function (item) { return checkDatesOverlappedByDay(eventStartDate, eventEndDate, item.StartDate, item.EndDate); /*item.StartDate <= eventEndDate && item.EndDate >= eventStartDate;*/ });
        
        var startColindex;
        var endColindex;

        if (eventsToMerge.length > 0) {
            eventsToMerge.push({
                "eventID": eventID,
                "eventIndex": eventIndex,
                "StartDate": eventStartDate,
                "EndDate": eventEndDate
            });
            var minStartEvent = eventsToMerge.reduce(function (prev, current) { return compareDateByDay.min(prev.StartDate, current.StartDate) == prev.StartDate ? prev : current; });
            var maxEndEvent = eventsToMerge.reduce(function (prev, current) { return compareDateByDay.max(prev.EndDate, current.EndDate) == prev.EndDate ? prev : current; });

            var startDateDay = minStartEvent.StartDate.getDay() == 0 ? 6 : minStartEvent.StartDate.getDay() - 1;
            var endDateDay = maxEndEvent.EndDate.getDay() == 0 ? 6 : maxEndEvent.EndDate.getDay() - 1;
            var dayStart_StartDate = moment(weeklyViewStartDate).add(startDateDay, "days").toDate().getDate();
            var dayEnd_StartDate = moment(weeklyViewStartDate).add(endDateDay, "days").toDate().getDate();

            newEventEntry.StartDate = new Date(weekMonday.year, weekMonday.month, dayStart_StartDate, minStartEvent.StartDate.getHours(), minStartEvent.StartDate.getMinutes(), minStartEvent.StartDate.getSeconds());
            newEventEntry.EndDate = new Date(weekMonday.year, weekMonday.month, dayEnd_StartDate, maxEndEvent.EndDate.getHours(), maxEndEvent.EndDate.getMinutes(), maxEndEvent.EndDate.getSeconds());

            eventStartDate = newEventEntry.StartDate;
            eventEndDate = newEventEntry.EndDate;

            eventsToMerge.forEach(function (item) {
                clearWeeklyEvent(item.eventID);
            });
        }

        var colIndexes = findStartEndColIndex(eventStartDate, eventEndDate, eventID);
        startColindex = colIndexes.start;
        endColindex = colIndexes.end;

        mapColindex.push({ startColindex: startColindex, endColindex: endColindex, events: [{ eventID: eventID, startColindex: startColindex, endColindex: endColindex }] });
        currentWeeklyEvents.push(newEventEntry);
        
        var tooltipSelector = "tooltip_" + thisParentId + "_" + eventID;
        var tooltipDiv = $("<div class='weeklyTooltip' id='" + tooltipSelector + "'></div>").appendTo(thisPage);
        var tooltipTarget = "." + thisParentId + ".eventCell.eventid" + eventID;

        tooltipDiv.dxTooltip({
            animation: null,
            target: tooltipTarget,
            showEvent: "mouseenter",
            hideEvent: "mouseleave",
            closeOnOutsideClick: false,
            onShowing: function (e) {
                var cell = $(thisPage).find(".eventCell.eventid" + eventID + ":hover");
                var tooltipContent = "";
                var intervalEvents = findCellEvents(cell).events;

                activeTooltips++;
                if (activeTooltips > 1) {
                    e.component.hide();
                    return;
                }

                $.each(intervalEvents, function (index, item) {
                    tooltipContent += item.StartDate.toLocaleDateString(navigator.language, dateStringOptions) + " - " + item.EndDate.toLocaleDateString(navigator.language, dateStringOptions) + "<br/>";
                });
                e.component.content().html(tooltipContent);
            },
            onHiding: function (e) {
                if (activeTooltips > 0)
                    activeTooltips--;
            }
        });
    }
}

