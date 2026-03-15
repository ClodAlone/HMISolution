/**
* DevExtreme (ui/calendar.d.ts)
* Version: 21.2.6
* Build date: Tue Mar 01 2022
*
* Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
* Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
*/
import {
    UserDefinedElement,
    DxElement,
} from '../core/element';

import {
    ChangedOptionInfo,
    EventInfo,
    InitializedEventInfo,
    NativeEventInfo,
} from '../events/index';

import {
    template,
} from '../core/templates/template';

import Editor, {
    ValueChangedInfo,
    EditorOptions,
} from './editor/editor';

/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface ComponentDisabledDate<T> {
    component: T;
    readonly date: Date;
    readonly view: string;
}

export type ContentReadyEvent = EventInfo<dxCalendar>;

export type DisposingEvent = EventInfo<dxCalendar>;

export type InitializedEvent = InitializedEventInfo<dxCalendar>;

export type OptionChangedEvent = EventInfo<dxCalendar> & ChangedOptionInfo;

export type ValueChangedEvent = NativeEventInfo<dxCalendar, KeyboardEvent | MouseEvent | PointerEvent | TouchEvent | Event> & ValueChangedInfo;

export type CellTemplateData = {
    readonly date: Date;
    readonly view: string;
    readonly text?: string;
};

export type DisabledDate = ComponentDisabledDate<dxCalendar>;

/**
 * @deprecated use Properties instead
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxCalendarOptions extends EditorOptions<dxCalendar> {
    /**
     * Specifies whether or not the UI component changes its state when interacting with a user.
     */
    activeStateEnabled?: boolean;
    /**
     * Specifies a custom template for calendar cells.
     */
    cellTemplate?: template | ((itemData: CellTemplateData, itemIndex: number, itemElement: DxElement) => string | UserDefinedElement);
    /**
     * Specifies the date-time value serialization format. Use it only if you do not specify the value at design time.
     */
    dateSerializationFormat?: string;
    /**
     * Specifies dates that users cannot select.
     */
    disabledDates?: Array<Date> | ((data: DisabledDate) => boolean);
    /**
     * Specifies the first day of a week.
     */
    firstDayOfWeek?: 0 | 1 | 2 | 3 | 4 | 5 | 6;
    /**
     * Specifies whether the UI component can be focused using keyboard navigation.
     */
    focusStateEnabled?: boolean;
    /**
     * Specifies whether the UI component changes its state when a user pauses on it.
     */
    hoverStateEnabled?: boolean;
    /**
     * The latest date the UI component allows to select.
     */
    max?: Date | number | string;
    /**
     * Specifies the maximum zoom level of the calendar.
     */
    maxZoomLevel?: 'century' | 'decade' | 'month' | 'year';
    /**
     * The earliest date the UI component allows to select.
     */
    min?: Date | number | string;
    /**
     * Specifies the minimum zoom level of the calendar.
     */
    minZoomLevel?: 'century' | 'decade' | 'month' | 'year';
    /**
     * The value to be assigned to the `name` attribute of the underlying HTML element.
     */
    name?: string;
    /**
     * Specifies whether or not the UI component displays a button that selects the current date.
     */
    showTodayButton?: boolean;
    /**
     * An object or a value specifying the date and time currently selected in the calendar.
     */
    value?: Date | number | string;
    /**
     * Specifies the current calendar zoom level.
     */
    zoomLevel?: 'century' | 'decade' | 'month' | 'year';
}
/**
 * The Calendar is a UI component that displays a calendar and allows an end user to select the required date within a specified date range.
 */
export default class dxCalendar extends Editor<dxCalendarOptions> { }

export type Properties = dxCalendarOptions;

/**
 * @deprecated use Properties instead
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export type Options = dxCalendarOptions;
