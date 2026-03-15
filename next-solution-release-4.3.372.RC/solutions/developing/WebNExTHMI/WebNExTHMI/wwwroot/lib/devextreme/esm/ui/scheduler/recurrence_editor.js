/**
 * DevExtreme (esm/ui/scheduler/recurrence_editor.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import registerComponent from "../../core/component_registrator";
import Guid from "../../core/guid";
import $ from "../../core/renderer";
import dateUtils from "../../core/utils/date";
import {
    extend
} from "../../core/utils/extend";
import {
    isDefined
} from "../../core/utils/type";
import dateLocalization from "../../localization/date";
import messageLocalization from "../../localization/message";
import Form from "../form";
import ButtonGroup from "../button_group";
import DateBox from "../date_box";
import Editor from "../editor/editor";
import NumberBox from "../number_box";
import {
    getRecurrenceProcessor
} from "./recurrence";
import "../radio_group";
var RECURRENCE_EDITOR = "dx-recurrence-editor";
var LABEL_POSTFIX = "-label";
var WRAPPER_POSTFIX = "-wrapper";
var RECURRENCE_EDITOR_CONTAINER = "dx-recurrence-editor-container";
var REPEAT_END_EDITOR = "dx-recurrence-repeat-end";
var REPEAT_END_TYPE_EDITOR = "dx-recurrence-radiogroup-repeat-type";
var REPEAT_COUNT_EDITOR = "dx-recurrence-numberbox-repeat-count";
var REPEAT_UNTIL_DATE_EDITOR = "dx-recurrence-datebox-until-date";
var RECURRENCE_BUTTON_GROUP = "dx-recurrence-button-group";
var FREQUENCY_EDITOR = "dx-recurrence-selectbox-freq";
var INTERVAL_EDITOR = "dx-recurrence-numberbox-interval";
var REPEAT_ON_EDITOR = "dx-recurrence-repeat-on";
var DAY_OF_MONTH = "dx-recurrence-numberbox-day-of-month";
var MONTH_OF_YEAR = "dx-recurrence-selectbox-month-of-year";
var recurrentEditorNumberBoxWidth = 70;
var recurrentEditorSelectBoxWidth = 120;
var defaultRecurrenceTypeIndex = 1;
var frequenciesMessages = [{
    recurrence: "dxScheduler-recurrenceHourly",
    value: "hourly"
}, {
    recurrence: "dxScheduler-recurrenceDaily",
    value: "daily"
}, {
    recurrence: "dxScheduler-recurrenceWeekly",
    value: "weekly"
}, {
    recurrence: "dxScheduler-recurrenceMonthly",
    value: "monthly"
}, {
    recurrence: "dxScheduler-recurrenceYearly",
    value: "yearly"
}];
var frequencies = frequenciesMessages.map(item => ({
    text: () => messageLocalization.format(item.recurrence),
    value: item.value
}));
var repeatEndTypes = [{
    type: "never"
}, {
    type: "until"
}, {
    type: "count"
}];
var days = ["SU", "MO", "TU", "WE", "TH", "FR", "SA"];
class RecurrenceRule {
    constructor(rule) {
        this._recurrenceProcessor = getRecurrenceProcessor();
        this._recurrenceRule = this._recurrenceProcessor.evalRecurrenceRule(rule).rule
    }
    makeRules(string) {
        this._recurrenceRule = this._recurrenceProcessor.evalRecurrenceRule(string).rule
    }
    makeRule(field, value) {
        if (!value || Array.isArray(value) && !value.length) {
            delete this._recurrenceRule[field];
            return
        }
        if (isDefined(field)) {
            if ("until" === field) {
                delete this._recurrenceRule.count
            }
            if ("count" === field) {
                delete this._recurrenceRule.until
            }
            this._recurrenceRule[field] = value
        }
    }
    getRepeatEndRule() {
        var rules = this._recurrenceRule;
        if ("count" in rules) {
            return "count"
        }
        if ("until" in rules) {
            return "until"
        }
        return "never"
    }
    getRecurrenceString() {
        return this._recurrenceProcessor.getRecurrenceString(this._recurrenceRule)
    }
    getRules() {
        return this._recurrenceRule
    }
    getDaysFromByDayRule() {
        return this._recurrenceProcessor.daysFromByDayRule(this._recurrenceRule)
    }
}
class RecurrenceEditor extends Editor {
    _getDefaultOptions() {
        var defaultOptions = super._getDefaultOptions();
        return extend(defaultOptions, {
            value: null,
            startDate: new Date,
            firstDayOfWeek: void 0
        })
    }
    _getFirstDayOfWeek() {
        var firstDayOfWeek = this.option("firstDayOfWeek");
        return isDefined(firstDayOfWeek) ? firstDayOfWeek : dateLocalization.firstDayOfWeekIndex()
    }
    _createComponent(element, name) {
        var config = arguments.length > 2 && void 0 !== arguments[2] ? arguments[2] : {};
        this._extendConfig(config, {
            readOnly: this.option("readOnly")
        });
        return super._createComponent(element, name, config)
    }
    _init() {
        super._init();
        this._recurrenceRule = new RecurrenceRule(this.option("value"))
    }
    _render() {
        super._render();
        this.$element().addClass(RECURRENCE_EDITOR);
        this._$container = $("<div>").addClass(RECURRENCE_EDITOR_CONTAINER).appendTo(this.$element());
        this._prepareEditors();
        this._renderEditors(this._$container)
    }
    getEditorByField(fieldName) {
        var editor = this.getRecurrenceForm().getEditor(fieldName);
        if (!isDefined(editor)) {
            switch (fieldName) {
                case "byday":
                    editor = this._weekEditor;
                    break;
                case "count":
                    editor = this._repeatCountEditor;
                    break;
                case "until":
                    editor = this._repeatUntilDate
            }
        }
        return editor
    }
    _prepareEditors() {
        var freq = (this._recurrenceRule.getRules().freq || frequenciesMessages[defaultRecurrenceTypeIndex].value).toLowerCase();
        this._editors = [this._createFreqEditor(freq), this._createIntervalEditor(freq), this._createRepeatOnLabel(freq), {
            itemType: "group",
            cssClass: REPEAT_ON_EDITOR,
            colCount: 2,
            colCountByScreen: {
                xs: 2
            },
            items: this._createRepeatOnEditor(freq)
        }, {
            itemType: "group",
            items: this._createRepeatEndEditor()
        }];
        return this._editors
    }
    _createFreqEditor(freq) {
        return {
            dataField: "freq",
            name: "FREQ",
            editorType: "dxSelectBox",
            cssClass: FREQUENCY_EDITOR,
            editorOptions: {
                items: frequencies,
                value: freq,
                field: "freq",
                valueExpr: "value",
                displayExpr: "text",
                layout: "horizontal",
                elementAttr: {
                    class: FREQUENCY_EDITOR
                },
                onValueChanged: args => this._valueChangedHandler(args)
            },
            label: {
                text: messageLocalization.format("dxScheduler-editorLabelRecurrence")
            }
        }
    }
    _createIntervalEditor(freq) {
        var interval = this._recurrenceRule.getRules().interval || 1;
        return {
            itemType: "group",
            colCount: 2,
            cssClass: "".concat(INTERVAL_EDITOR).concat(WRAPPER_POSTFIX),
            colCountByScreen: {
                xs: 2
            },
            items: [{
                dataField: "interval",
                editorType: "dxNumberBox",
                editorOptions: {
                    format: "#",
                    width: recurrentEditorNumberBoxWidth,
                    min: 1,
                    field: "interval",
                    value: interval,
                    showSpinButtons: true,
                    useLargeSpinButtons: false,
                    elementAttr: {
                        class: INTERVAL_EDITOR
                    },
                    onValueChanged: args => this._valueChangedHandler(args)
                },
                label: {
                    text: messageLocalization.format("dxScheduler-recurrenceRepeatEvery")
                }
            }, {
                name: "intervalLabel",
                cssClass: "".concat(INTERVAL_EDITOR).concat(LABEL_POSTFIX),
                template: () => messageLocalization.format("dxScheduler-recurrenceRepeat".concat(freq.charAt(0).toUpperCase()).concat(freq.substr(1).toLowerCase()))
            }]
        }
    }
    _createRepeatOnLabel(freq) {
        return {
            itemType: "group",
            cssClass: "".concat(REPEAT_ON_EDITOR).concat(LABEL_POSTFIX),
            items: [{
                name: "repeatOnLabel",
                colSpan: 2,
                template: () => messageLocalization.format("dxScheduler-recurrenceRepeatOn"),
                visible: freq && "daily" !== freq && "hourly" !== freq
            }]
        }
    }
    _createRepeatOnEditor(freq) {
        return [this._createByDayEditor(freq), this._createByMonthEditor(freq), this._createByMonthDayEditor(freq)]
    }
    _createByDayEditor(freq) {
        return {
            dataField: "byday",
            colSpan: 2,
            template: (_, itemElement) => {
                var firstDayOfWeek = this._getFirstDayOfWeek();
                var byDay = this._daysOfWeekByRules();
                var localDaysNames = dateLocalization.getDayNames("abbreviated");
                var dayNames = days.slice(firstDayOfWeek).concat(days.slice(0, firstDayOfWeek));
                var itemsButtonGroup = localDaysNames.slice(firstDayOfWeek).concat(localDaysNames.slice(0, firstDayOfWeek)).map((item, index) => ({
                    text: item,
                    key: dayNames[index]
                }));
                this._$repeatOnWeek = $("<div>").addClass(RECURRENCE_BUTTON_GROUP).appendTo(itemElement);
                this._weekEditor = this._createComponent(this._$repeatOnWeek, ButtonGroup, {
                    items: itemsButtonGroup,
                    field: "byday",
                    selectionMode: "multiple",
                    selectedItemKeys: byDay,
                    keyExpr: "key",
                    onSelectionChanged: e => {
                        var selectedKeys = e.component.option("selectedItemKeys");
                        this._recurrenceRule.makeRule("byday", selectedKeys);
                        this._changeEditorValue()
                    }
                })
            },
            visible: "weekly" === freq,
            label: {
                visible: false
            }
        }
    }
    _createByMonthEditor(freq) {
        var monthsName = dateLocalization.getMonthNames("wide");
        var months = [...Array(12)].map((_, i) => ({
            value: "".concat(i + 1),
            text: monthsName[i]
        }));
        return {
            dataField: "bymonth",
            editorType: "dxSelectBox",
            editorOptions: {
                field: "bymonth",
                items: months,
                value: this._monthOfYearByRules(),
                width: recurrentEditorSelectBoxWidth,
                displayExpr: "text",
                valueExpr: "value",
                elementAttr: {
                    class: MONTH_OF_YEAR
                },
                onValueChanged: args => this._valueChangedHandler(args)
            },
            visible: "yearly" === freq,
            label: {
                visible: false
            }
        }
    }
    _createByMonthDayEditor(freq) {
        return {
            dataField: "bymonthday",
            editorType: "dxNumberBox",
            editorOptions: {
                min: 1,
                max: 31,
                format: "#",
                width: recurrentEditorNumberBoxWidth,
                field: "bymonthday",
                showSpinButtons: true,
                useLargeSpinButtons: false,
                value: this._dayOfMonthByRules(),
                elementAttr: {
                    class: DAY_OF_MONTH
                },
                onValueChanged: args => this._valueChangedHandler(args)
            },
            visible: "monthly" === freq || "yearly" === freq,
            label: {
                visible: false
            }
        }
    }
    _createRepeatEndEditor() {
        var repeatType = this._recurrenceRule.getRepeatEndRule();
        return [{
            dataField: "repeatEnd",
            editorType: "dxRadioGroup",
            editorOptions: {
                items: repeatEndTypes,
                value: repeatType,
                valueExpr: "type",
                field: "repeatEnd",
                itemTemplate: itemData => {
                    if ("count" === itemData.type) {
                        return this._renderRepeatCountEditor()
                    }
                    if ("until" === itemData.type) {
                        return this._renderRepeatUntilEditor()
                    }
                    return this._renderDefaultRepeatEnd()
                },
                layout: "vertical",
                elementAttr: {
                    class: REPEAT_END_TYPE_EDITOR
                },
                onValueChanged: args => this._repeatEndValueChangedHandler(args)
            },
            label: {
                text: messageLocalization.format("dxScheduler-recurrenceEnd")
            }
        }]
    }
    _renderEditors($container) {
        this._recurrenceForm = this._createComponent($container, Form, {
            items: this._editors,
            showValidationSummary: false,
            scrollingEnabled: true,
            showColonAfterLabel: false,
            labelLocation: "top"
        });
        this._disableRepeatEndParts()
    }
    _setAriaDescribedBy(editor, $label) {
        var labelId = "label-".concat(new Guid);
        editor.setAria("describedby", labelId);
        editor.setAria("id", labelId, $label)
    }
    getRecurrenceForm() {
        return this._recurrenceForm
    }
    changeValueByVisibility(value) {
        if (value) {
            if (!this.option("value")) {
                this._handleDefaults()
            }
        } else {
            this._recurrenceRule.makeRules("");
            this.option("value", "")
        }
    }
    _handleDefaults() {
        this._recurrenceRule.makeRule("freq", frequenciesMessages[defaultRecurrenceTypeIndex].value);
        this._changeEditorValue()
    }
    _changeEditorValue() {
        this.option("value", this._recurrenceRule.getRecurrenceString() || "")
    }
    _daysOfWeekByRules() {
        var daysByRule = this._recurrenceRule.getDaysFromByDayRule();
        if (!daysByRule.length) {
            daysByRule = [days[this.option("startDate").getDay()]]
        }
        return daysByRule
    }
    _dayOfMonthByRules() {
        var dayByRule = this._recurrenceRule.getRules().bymonthday;
        if (!dayByRule) {
            dayByRule = this.option("startDate").getDate()
        }
        return dayByRule
    }
    _monthOfYearByRules() {
        var monthByRule = this._recurrenceRule.getRules().bymonth;
        if (!monthByRule) {
            monthByRule = this.option("startDate").getMonth() + 1
        }
        return String(monthByRule)
    }
    _renderDefaultRepeatEnd() {
        var $editorTemplate = $("<div>").addClass(REPEAT_END_EDITOR + WRAPPER_POSTFIX);
        $("<div>").text(messageLocalization.format("dxScheduler-recurrenceNever")).addClass(REPEAT_END_EDITOR + LABEL_POSTFIX).appendTo($editorTemplate);
        return $editorTemplate
    }
    _repeatEndValueChangedHandler(args) {
        var value = args.value;
        this._disableRepeatEndParts(value);
        if ("until" === value) {
            this._recurrenceRule.makeRule(value, this._getUntilValue())
        }
        if ("count" === value) {
            this._recurrenceRule.makeRule(value, this._repeatCountEditor.option("value"))
        }
        if ("never" === value) {
            this._recurrenceRule.makeRule("count", "");
            this._recurrenceRule.makeRule("until", "")
        }
        this._changeEditorValue()
    }
    _disableRepeatEndParts() {
        var value = arguments.length > 0 && void 0 !== arguments[0] ? arguments[0] : this._recurrenceRule.getRepeatEndRule();
        if ("until" === value) {
            this._repeatCountEditor.option("disabled", true);
            this._repeatUntilDate.option("disabled", false)
        }
        if ("count" === value) {
            this._repeatCountEditor.option("disabled", false);
            this._repeatUntilDate.option("disabled", true)
        }
        if ("never" === value) {
            this._repeatCountEditor.option("disabled", true);
            this._repeatUntilDate.option("disabled", true)
        }
    }
    _renderRepeatCountEditor() {
        var repeatCount = this._recurrenceRule.getRules().count || 1;
        var $editorWrapper = $("<div>").addClass(REPEAT_END_EDITOR + WRAPPER_POSTFIX);
        $("<div>").text(messageLocalization.format("dxScheduler-recurrenceAfter")).addClass(REPEAT_END_EDITOR + LABEL_POSTFIX).appendTo($editorWrapper);
        this._$repeatCountEditor = $("<div>").addClass(REPEAT_COUNT_EDITOR).appendTo($editorWrapper);
        $("<div>").text(messageLocalization.format("dxScheduler-recurrenceRepeatCount")).addClass(REPEAT_END_EDITOR + LABEL_POSTFIX).appendTo($editorWrapper);
        this._repeatCountEditor = this._createComponent(this._$repeatCountEditor, NumberBox, {
            field: "count",
            format: "#",
            width: recurrentEditorNumberBoxWidth,
            min: 1,
            showSpinButtons: true,
            useLargeSpinButtons: false,
            value: repeatCount,
            onValueChanged: this._repeatCountValueChangeHandler.bind(this)
        });
        return $editorWrapper
    }
    _repeatCountValueChangeHandler(args) {
        if ("count" === this._recurrenceRule.getRepeatEndRule()) {
            var value = args.value;
            this._recurrenceRule.makeRule("count", value);
            this._changeEditorValue()
        }
    }
    _formatUntilDate(date) {
        if (this._recurrenceRule.getRules().until && dateUtils.sameDate(this._recurrenceRule.getRules().until, date)) {
            return date
        }
        return dateUtils.setToDayEnd(date)
    }
    _renderRepeatUntilEditor() {
        var repeatUntil = this._recurrenceRule.getRules().until || this._formatUntilDate(new Date);
        var $editorWrapper = $("<div>").addClass(REPEAT_END_EDITOR + WRAPPER_POSTFIX);
        $("<div>").text(messageLocalization.format("dxScheduler-recurrenceOn")).addClass(REPEAT_END_EDITOR + LABEL_POSTFIX).appendTo($editorWrapper);
        this._$repeatDateEditor = $("<div>").addClass(REPEAT_UNTIL_DATE_EDITOR).appendTo($editorWrapper);
        this._repeatUntilDate = this._createComponent(this._$repeatDateEditor, DateBox, {
            field: "until",
            value: repeatUntil,
            type: "date",
            onValueChanged: this._repeatUntilValueChangeHandler.bind(this),
            calendarOptions: {
                firstDayOfWeek: this._getFirstDayOfWeek()
            },
            useMaskBehavior: true
        });
        return $editorWrapper
    }
    _repeatUntilValueChangeHandler(args) {
        if ("until" === this._recurrenceRule.getRepeatEndRule()) {
            var untilDate = this._formatUntilDate(new Date(args.value));
            this._repeatUntilDate.option("value", untilDate);
            this._recurrenceRule.makeRule("until", untilDate);
            this._changeEditorValue()
        }
    }
    _valueChangedHandler(args) {
        var {
            value: value,
            previousValue: previousValue
        } = args;
        var field = args.component.option("field");
        if (!this.option("visible")) {
            this.option("value", "")
        } else {
            this._recurrenceRule.makeRule(field, value);
            if ("freq" === field) {
                this._makeRepeatOnRule(value);
                this._changeRepeatOnVisibility(value, previousValue)
            }
            this._changeEditorValue()
        }
    }
    _makeRepeatOnRule(value) {
        if ("daily" === value || "hourly" === value) {
            this._recurrenceRule.makeRule("byday", "");
            this._recurrenceRule.makeRule("bymonth", "");
            this._recurrenceRule.makeRule("bymonthday", "")
        }
        if ("weekly" === value) {
            this._recurrenceRule.makeRule("byday", this._daysOfWeekByRules());
            this._recurrenceRule.makeRule("bymonth", "");
            this._recurrenceRule.makeRule("bymonthday", "")
        }
        if ("monthly" === value) {
            this._recurrenceRule.makeRule("bymonthday", this._dayOfMonthByRules());
            this._recurrenceRule.makeRule("bymonth", "");
            this._recurrenceRule.makeRule("byday", "")
        }
        if ("yearly" === value) {
            this._recurrenceRule.makeRule("bymonthday", this._dayOfMonthByRules());
            this._recurrenceRule.makeRule("bymonth", this._monthOfYearByRules());
            this._recurrenceRule.makeRule("byday", "")
        }
    }
    _optionChanged(args) {
        var _this$_recurrenceForm, _this$_repeatCountEdi, _this$_weekEditor, _this$_repeatUntilDat;
        switch (args.name) {
            case "readOnly":
                null === (_this$_recurrenceForm = this._recurrenceForm) || void 0 === _this$_recurrenceForm ? void 0 : _this$_recurrenceForm.option("readOnly", args.value);
                null === (_this$_repeatCountEdi = this._repeatCountEditor) || void 0 === _this$_repeatCountEdi ? void 0 : _this$_repeatCountEdi.option("readOnly", args.value);
                null === (_this$_weekEditor = this._weekEditor) || void 0 === _this$_weekEditor ? void 0 : _this$_weekEditor.option("readOnly", args.value);
                null === (_this$_repeatUntilDat = this._repeatUntilDate) || void 0 === _this$_repeatUntilDat ? void 0 : _this$_repeatUntilDat.option("readOnly", args.value);
                super._optionChanged(args);
                break;
            case "value":
                this._recurrenceRule.makeRules(args.value);
                this._changeRepeatIntervalLabel();
                this._disableRepeatEndParts();
                this._changeEditorsValue(this._recurrenceRule.getRules());
                super._optionChanged(args);
                break;
            case "startDate":
                this._makeRepeatOnRule(this._recurrenceRule.getRules().freq);
                if (isDefined(this._recurrenceRule.getRecurrenceString())) {
                    this._changeEditorValue()
                }
                break;
            case "firstDayOfWeek":
                if (this._weekEditor) {
                    var localDaysNames = dateLocalization.getDayNames("abbreviated");
                    var dayNames = days.slice(args.value).concat(days.slice(0, args.value));
                    var itemsButtonGroup = localDaysNames.slice(args.value).concat(localDaysNames.slice(0, args.value)).map((item, index) => ({
                        text: item,
                        key: dayNames[index]
                    }));
                    this._weekEditor.option("items", itemsButtonGroup)
                }
                if (this._$repeatDateEditor) {
                    this._repeatUntilDate.option("calendarOptions.firstDayOfWeek", this._getFirstDayOfWeek())
                }
                break;
            default:
                super._optionChanged(args)
        }
    }
    _changeRepeatOnVisibility(freq, previousFreq) {
        if (freq !== previousFreq) {
            this._recurrenceForm.itemOption("byday", "visible", false);
            this._recurrenceForm.itemOption("bymonthday", "visible", false);
            this._recurrenceForm.itemOption("bymonth", "visible", false);
            this._recurrenceForm.itemOption("repeatOnLabel", "visible", freq && "daily" !== freq && "hourly" !== freq);
            if ("weekly" === freq) {
                this._recurrenceForm.itemOption("byday", "visible", true)
            }
            if ("monthly" === freq) {
                this._recurrenceForm.itemOption("bymonthday", "visible", true)
            }
            if ("yearly" === freq) {
                this._recurrenceForm.itemOption("bymonthday", "visible", true);
                this._recurrenceForm.itemOption("bymonth", "visible", true)
            }
        }
    }
    _changeRepeatIntervalLabel() {
        var freq = this._recurrenceRule.getRules().freq;
        freq && this._recurrenceForm.itemOption("intervalLabel", "template", messageLocalization.format("dxScheduler-recurrenceRepeat".concat(freq.charAt(0).toUpperCase()).concat(freq.substr(1).toLowerCase())))
    }
    _changeEditorsValue(rules) {
        this._recurrenceForm.getEditor("freq").option("value", (rules.freq || frequenciesMessages[defaultRecurrenceTypeIndex].value).toLowerCase());
        this._changeDayOfWeekValue();
        this._changeDayOfMonthValue();
        this._changeMonthOfYearValue();
        this._changeIntervalValue(rules.interval);
        this._changeRepeatCountValue();
        this._changeRepeatEndValue();
        this._changeRepeatUntilValue()
    }
    _changeIntervalValue(value) {
        this._recurrenceForm.getEditor("interval").option("value", value || 1)
    }
    _changeRepeatEndValue() {
        var repeatType = this._recurrenceRule.getRepeatEndRule();
        this._recurrenceForm.getEditor("repeatEnd").option("value", repeatType)
    }
    _changeDayOfWeekValue() {
        var isEditorVisible = this._recurrenceForm.itemOption("byday").visible;
        if (isEditorVisible) {
            var _days = this._daysOfWeekByRules();
            this.getEditorByField("byday").option("selectedItemKeys", _days)
        }
    }
    _changeDayOfMonthValue() {
        var isEditorVisible = this._recurrenceForm.itemOption("bymonthday").visible;
        if (isEditorVisible) {
            var day = this._dayOfMonthByRules();
            this._recurrenceForm.getEditor("bymonthday").option("value", day)
        }
    }
    _changeMonthOfYearValue() {
        var isEditorVisible = this._recurrenceForm.itemOption("bymonth").visible;
        if (isEditorVisible) {
            var month = this._monthOfYearByRules();
            this._recurrenceForm.getEditor("bymonth").option("value", month)
        }
    }
    _changeRepeatCountValue() {
        var count = this._recurrenceRule.getRules().count || 1;
        this._repeatCountEditor.option("value", count)
    }
    _changeRepeatUntilValue() {
        this._repeatUntilDate.option("value", this._getUntilValue())
    }
    _getUntilValue() {
        return this._recurrenceRule.getRules().until || this._formatUntilDate(new Date)
    }
    toggle() {
        this._freqEditor.focus()
    }
    setAria() {
        if (this._switchEditor) {
            this._switchEditor.setAria(arguments.length <= 0 ? void 0 : arguments[0], arguments.length <= 1 ? void 0 : arguments[1])
        }
    }
}
registerComponent("dxRecurrenceEditor", RecurrenceEditor);
export default RecurrenceEditor;
