/**
* DevExtreme (ui/scheduler.d.ts)
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
    template,
} from '../core/templates/template';

import DataSource, { DataSourceLike } from '../data/data_source';

import {
    DxEvent,
    EventInfo,
    NativeEventInfo,
    InitializedEventInfo,
    ChangedOptionInfo,
    Cancelable,
} from '../events/index';

import {
    CollectionWidgetItem,
} from './collection/ui.collection_widget.base';

import dxDraggable from './draggable';

import dxForm from './form';
import dxPopup from './popup';

import dxSortable from './sortable';

import Widget, {
    WidgetOptions,
} from './widget/ui.widget';

/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
interface AppointmentDraggingEvent {
  readonly component: dxScheduler;
  readonly event?: DxEvent<MouseEvent | TouchEvent>;
  readonly itemData?: any;
  readonly itemElement?: DxElement;
  readonly fromData?: any;
}

/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
interface TargetedAppointmentInfo {
  readonly appointmentData: dxSchedulerAppointment;
  readonly targetedAppointmentData?: dxSchedulerAppointment;
}

export type AppointmentAddedEvent = EventInfo<dxScheduler> & {
  readonly appointmentData: dxSchedulerAppointment;
  readonly error?: Error;
};

export type AppointmentAddingEvent = EventInfo<dxScheduler> & {
  readonly appointmentData: dxSchedulerAppointment;
  cancel: boolean | PromiseLike<boolean>;
};

export type AppointmentClickEvent = Cancelable & NativeEventInfo<dxScheduler, KeyboardEvent | MouseEvent | PointerEvent> & TargetedAppointmentInfo & {
  readonly appointmentElement: DxElement;
};

export type AppointmentContextMenuEvent = NativeEventInfo<dxScheduler, MouseEvent | PointerEvent | TouchEvent> & TargetedAppointmentInfo & {
  readonly appointmentElement: DxElement;
};

export type AppointmentDblClickEvent = Cancelable & NativeEventInfo<dxScheduler, MouseEvent | PointerEvent> & TargetedAppointmentInfo & {
  readonly appointmentElement: DxElement;
};

export type AppointmentDeletedEvent = EventInfo<dxScheduler> & {
  readonly appointmentData: dxSchedulerAppointment;
  readonly error?: Error;
};

export type AppointmentDeletingEvent = EventInfo<dxScheduler> & {
  readonly appointmentData: dxSchedulerAppointment;
  cancel: boolean | PromiseLike<boolean>;
};

export type AppointmentFormOpeningEvent = Cancelable & EventInfo<dxScheduler> & {
  readonly appointmentData?: dxSchedulerAppointment;
  readonly form: dxForm;
  readonly popup: dxPopup;
};

export type AppointmentRenderedEvent = EventInfo<dxScheduler> & TargetedAppointmentInfo & {
  readonly appointmentElement: DxElement;
};

export type AppointmentUpdatedEvent = EventInfo<dxScheduler> & {
  readonly appointmentData: dxSchedulerAppointment;
  readonly error?: Error;
};

export type AppointmentUpdatingEvent = EventInfo<dxScheduler> & {
  readonly oldData: any;
  readonly newData: any;
  cancel?: boolean | PromiseLike<boolean>;
};

export type CellClickEvent = Cancelable & NativeEventInfo<dxScheduler, KeyboardEvent | MouseEvent | PointerEvent> & {
  readonly cellData: any;
  readonly cellElement: DxElement;
};

export type CellContextMenuEvent = NativeEventInfo<dxScheduler, MouseEvent | PointerEvent | TouchEvent> & {
  readonly cellData: any;
  readonly cellElement: DxElement;
};

export type ContentReadyEvent = EventInfo<dxScheduler>;

export type DisposingEvent = EventInfo<dxScheduler>;

export type InitializedEvent = InitializedEventInfo<dxScheduler>;

export type OptionChangedEvent = EventInfo<dxScheduler> & ChangedOptionInfo;

export type AppointmentDraggingAddEvent = AppointmentDraggingEvent & {
  readonly fromComponent?: dxSortable | dxDraggable;
  readonly toComponent?: dxSortable | dxDraggable;
  readonly toData?: any;
};

export type AppointmentDraggingEndEvent = Cancelable & AppointmentDraggingEvent & {
  readonly fromComponent?: dxSortable | dxDraggable;
  readonly toComponent?: dxSortable | dxDraggable;
  readonly toData?: any;
};

export type AppointmentDraggingMoveEvent = Cancelable & AppointmentDraggingEvent & {
  readonly fromComponent?: dxSortable | dxDraggable;
  readonly toComponent?: dxSortable | dxDraggable;
  readonly toData?: any;
};

export type AppointmentDraggingStartEvent = Cancelable & AppointmentDraggingEvent;

export type AppointmentDraggingRemoveEvent = AppointmentDraggingEvent & {
  readonly fromComponent?: dxSortable | dxDraggable;
  readonly toComponent?: dxSortable | dxDraggable;
};

export type AppointmentTemplateData = TargetedAppointmentInfo;

export type AppointmentTooltipTemplateData = TargetedAppointmentInfo;

export type AppointmentCollectorTemplateData = {
  readonly appointmentCount: number;
  readonly isCompact: boolean;
};

export type DateNavigatorTextInfo = {
  readonly startDate: Date;
  readonly endDate: Date;
  readonly text: string;
};

/**
 * @deprecated use Properties instead
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxSchedulerOptions extends WidgetOptions<dxScheduler> {
    /**
     * Specifies whether the UI component adapts to small screens.
     */
    adaptivityEnabled?: boolean;
    /**
     * Specifies the name of the data source item field whose value defines whether or not the corresponding appointment is an all-day appointment.
     */
    allDayExpr?: string;
    /**
     * Specifies a custom template for cell overflow indicators.
     */
    appointmentCollectorTemplate?: template | ((data: AppointmentCollectorTemplateData, collectorElement: DxElement) => string | UserDefinedElement);
    /**
     * Configures appointment reordering using drag and drop gestures.
     */
    appointmentDragging?: {
      /**
       * Enables automatic scrolling while dragging an appointment beyond the viewport.
       */
      autoScroll?: boolean;
      /**
       * A container for custom data.
       */
      data?: any;
      /**
       * Allows you to group several UI components so that users can drag and drop appointments between them.
       */
      group?: string;
      /**
       * A function that is called when a new appointment is added.
       */
      onAdd?: ((e: AppointmentDraggingAddEvent) => void);
      /**
       * A function that is called when the dragged appointment&apos;s position is changed.
       */
      onDragEnd?: ((e: AppointmentDraggingEndEvent) => void);
      /**
       * A function that is called every time a draggable appointment is moved.
       */
      onDragMove?: ((e: AppointmentDraggingMoveEvent) => void);
      /**
       * A function that is called when the drag gesture is initialized.
       */
      onDragStart?: ((e: AppointmentDraggingStartEvent) => void);
      /**
       * A function that is called when a draggable appointment is removed.
       */
      onRemove?: ((e: AppointmentDraggingRemoveEvent) => void);
      /**
       * Specifies the distance in pixels from the edge of viewport at which scrolling should start. Applies only if autoScroll is true.
       */
      scrollSensitivity?: number;
      /**
       * Specifies the scrolling speed when dragging an appointment beyond the viewport. Applies only if autoScroll is true.
       */
      scrollSpeed?: number;
    };
    /**
     * Specifies a custom template for appointments.
     */
    appointmentTemplate?: template | ((model: AppointmentTemplateData, itemIndex: number, contentElement: DxElement) => string | UserDefinedElement);
    /**
     * Specifies a custom template for tooltips displayed when users click an appointment or cell overflow indicator.
     */
    appointmentTooltipTemplate?: template | ((model: AppointmentTemplateData, itemIndex: number, contentElement: DxElement) => string | UserDefinedElement);
    /**
     * Specifies cell duration in minutes. This property&apos;s value should divide the interval between startDayHour and endDayHour into even parts.
     */
    cellDuration?: number;
    /**
     * Specifies whether or not an end-user can scroll the view in both directions at the same time.
     */
    crossScrollingEnabled?: boolean;
    /**
     * Specifies the current date.
     */
    currentDate?: Date | number | string;
    /**
     * Specifies the displayed view. Accepts name or type of a view available in the views array.
     */
    currentView?: 'agenda' | 'day' | 'month' | 'timelineDay' | 'timelineMonth' | 'timelineWeek' | 'timelineWorkWeek' | 'week' | 'workWeek';
    /**
     * Customizes the date navigator&apos;s text.
     */
    customizeDateNavigatorText?: ((info: DateNavigatorTextInfo) => string);
    /**
     * Specifies a custom template for table cells.
     */
    dataCellTemplate?: template | ((itemData: any, itemIndex: number, itemElement: DxElement) => string | UserDefinedElement);
    /**
     * Binds the UI component to data.
     */
    dataSource?: DataSourceLike<Appointment>;
    /**
     * Specifies a custom template for day scale items.
     */
    dateCellTemplate?: template | ((itemData: any, itemIndex: number, itemElement: DxElement) => string | UserDefinedElement);
    /**
     * Specifies the format in which date-time values should be sent to the server.
     */
    dateSerializationFormat?: string;
    /**
     * Specifies the name of the data source item field whose value holds the description of the corresponding appointment.
     */
    descriptionExpr?: string;
    /**
     * Specifies a custom template for tooltips displayed when users click a cell overflow indicator.
     * @deprecated Use the appointmentTooltipTemplate option instead.
     */
    dropDownAppointmentTemplate?: template | ((itemData: any, itemIndex: number, contentElement: DxElement) => string | UserDefinedElement);
    /**
     * Specifies which editing operations a user can perform on appointments.
     */
    editing?: boolean | {
      /**
       * Specifies whether or not an end-user can add appointments.
       */
      allowAdding?: boolean;
      /**
       * Specifies whether or not an end-user can delete appointments.
       */
      allowDeleting?: boolean;
      /**
       * Specifies whether users can drag and drop appointments.
       */
      allowDragging?: boolean;
      /**
       * Specifies whether or not an end-user can change an appointment duration.
       */
      allowResizing?: boolean;
      /**
       * Specifies whether users can edit appointment time zones.
       */
      allowTimeZoneEditing?: boolean;
      /**
       * Specifies whether or not an end-user can change appointment properties.
       */
      allowUpdating?: boolean;
    };
    /**
     * Specifies the name of the data source item field that defines the ending of an appointment.
     */
    endDateExpr?: string;
    /**
     * Specifies the name of the data source item field that defines the timezone of the appointment end date.
     */
    endDateTimeZoneExpr?: string;
    /**
     * Specifies the last hour on the time scale. Accepts integer values from 0 to 24.
     */
    endDayHour?: number;
    /**
     * Specifies the first day of a week. Does not apply to the agenda view.
     */
    firstDayOfWeek?: 0 | 1 | 2 | 3 | 4 | 5 | 6;
    /**
     * Specifies whether the UI component can be focused using keyboard navigation.
     */
    focusStateEnabled?: boolean;
    /**
     * If true, appointments are grouped by date first and then by resource; opposite if false. Applies only if appointments are grouped and groupOrientation is &apos;horizontal&apos;.
     */
    groupByDate?: boolean;
    /**
     * Specifies the resource kinds by which the scheduler&apos;s appointments are grouped in a timetable.
     */
    groups?: Array<string>;
    /**
     * Specifies the time interval between when the date-time indicator changes its position, in milliseconds.
     */
    indicatorUpdateInterval?: number;
    /**
     * The latest date the UI component allows you to select.
     */
    max?: Date | number | string;
    /**
     * Specifies the limit of full-sized appointments displayed per cell. Applies to all views except &apos;agenda&apos;.
     */
    maxAppointmentsPerCell?: number | 'auto' | 'unlimited';
    /**
     * The earliest date the UI component allows you to select.
     */
    min?: Date | number | string;
    /**
     * Specifies the text or HTML markup displayed by the UI component if the item collection is empty. Available for the Agenda view only.
     */
    noDataText?: string;
    /**
     * A function that is executed after an appointment is added to the data source.
     */
    onAppointmentAdded?: ((e: AppointmentAddedEvent) => void);
    /**
     * A function that is executed before an appointment is added to the data source.
     */
    onAppointmentAdding?: ((e: AppointmentAddingEvent) => void);
    /**
     * A function that is executed when an appointment is clicked or tapped.
     */
    onAppointmentClick?: ((e: AppointmentClickEvent) => void) | string;
    /**
     * A function that is executed when a user attempts to open the browser&apos;s context menu for an appointment. Allows you to replace this context menu with a custom context menu.
     */
    onAppointmentContextMenu?: ((e: AppointmentContextMenuEvent) => void) | string;
    /**
     * A function that is executed when an appointment is double-clicked or double-tapped.
     */
    onAppointmentDblClick?: ((e: AppointmentDblClickEvent) => void) | string;
    /**
     * A function that is executed after an appointment is deleted from the data source.
     */
    onAppointmentDeleted?: ((e: AppointmentDeletedEvent) => void);
    /**
     * A function that is executed before an appointment is deleted from the data source.
     */
    onAppointmentDeleting?: ((e: AppointmentDeletingEvent) => void);
    /**
     * A function that is executed before an appointment details form appears. Use this function to customize the form.
     */
    onAppointmentFormOpening?: ((e: AppointmentFormOpeningEvent) => void);
    /**
     * A function that is executed when an appointment is rendered.
     */
    onAppointmentRendered?: ((e: AppointmentRenderedEvent) => void);
    /**
     * A function that is executed after an appointment is updated in the data source.
     */
    onAppointmentUpdated?: ((e: AppointmentUpdatedEvent) => void);
    /**
     * A function that is executed before an appointment is updated in the data source.
     */
    onAppointmentUpdating?: ((e: AppointmentUpdatingEvent) => void);
    /**
     * A function that is executed when a view cell is clicked.
     */
    onCellClick?: ((e: CellClickEvent) => void) | string;
    /**
     * A function that is executed when a user attempts to open the browser&apos;s context menu for a cell. Allows you to replace this context menu with a custom context menu.
     */
    onCellContextMenu?: ((e: CellContextMenuEvent) => void) | string;
    /**
     * Specifies the edit mode for recurring appointments.
     */
    recurrenceEditMode?: 'dialog' | 'occurrence' | 'series';
    /**
     * Specifies the name of the data source item field that defines exceptions for the current recurring appointment.
     */
    recurrenceExceptionExpr?: string;
    /**
     * Specifies the name of the data source item field that defines a recurrence rule for generating recurring appointments.
     */
    recurrenceRuleExpr?: string;
    /**
     * Specifies whether filtering is performed on the server or client side.
     */
    remoteFiltering?: boolean;
    /**
     * Specifies a custom template for resource headers.
     */
    resourceCellTemplate?: template | ((itemData: any, itemIndex: number, itemElement: DxElement) => string | UserDefinedElement);
    /**
     * Specifies an array of resources available in the scheduler.
     */
    resources?: Array<{
      /**
       * Specifies whether you can assign several resources of this kind to an appointment.
       */
      allowMultiple?: boolean;
      /**
       * Specifies the resource object field that is used as a resource color.
       */
      colorExpr?: string;
      /**
       * Specifies available resource instances.
       */
      dataSource?: DataSourceLike<any>;
      /**
       * Specifies which field from the resource data objects provides values to be displayed in the resource editor.
       */
      displayExpr?: string | ((resource: any) => string);
      /**
       * The name of the appointment object field that specifies a resource of this kind.
       */
      fieldExpr?: string;
      /**
       * Specifies the label of the Appointment popup window field that allows end users to assign a resource of this kind.
       */
      label?: string;
      /**
       * Specifies whether appointments are colored like this resource kind.
       */
      useColorAsDefault?: boolean;
      /**
       * Specifies the resource object field that is used as a value of the Resource editor in the Appointment popup window.
       */
      valueExpr?: string | Function;
    }>;
    /**
     * Configures scrolling.
     */
    scrolling?: dxSchedulerScrolling;
    /**
     * Currently selected cells&apos; data.
     */
    selectedCellData?: Array<any>;
    /**
     * Specifies whether to apply shading to cover the timetable up to the current time.
     */
    shadeUntilCurrentTime?: boolean;
    /**
     * Specifies the &apos;All-day&apos; panel&apos;s visibility. Setting this property to false hides the panel along with the all-day appointments.
     */
    showAllDayPanel?: boolean;
    /**
     * Specifies the current date-time indicator&apos;s visibility.
     */
    showCurrentTimeIndicator?: boolean;
    /**
     * Specifies the name of the data source item field that defines the start of an appointment.
     */
    startDateExpr?: string;
    /**
     * Specifies the name of the data source item field that defines the timezone of the appointment start date.
     */
    startDateTimeZoneExpr?: string;
    /**
     * Specifies the first hour on the time scale. Accepts integer values from 0 to 24.
     */
    startDayHour?: number;
    /**
     * Specifies the name of the data source item field that holds the subject of an appointment.
     */
    textExpr?: string;
    /**
     * Specifies a custom template for time scale items.
     */
    timeCellTemplate?: template | ((itemData: any, itemIndex: number, itemElement: DxElement) => string | UserDefinedElement);
    /**
     * Specifies the time zone for the Scheduler&apos;s grid. Accepts values from the IANA time zone database.
     */
    timeZone?: string;
    /**
     * Specifies whether a user can switch views using tabs or a drop-down menu.
     */
    useDropDownViewSwitcher?: boolean;
    /**
     * Specifies and configures the views to be available in the view switcher.
     */
    views?: Array<'day' | 'week' | 'workWeek' | 'month' | 'timelineDay' | 'timelineWeek' | 'timelineWorkWeek' | 'timelineMonth' | 'agenda' | {
      /**
       * Specifies the number of dates that can be shown at a time in the agenda view.
       */
      agendaDuration?: number;
      /**
       * Specifies a custom template for cell overflow indicators in this view.
       */
      appointmentCollectorTemplate?: template | ((data: AppointmentCollectorTemplateData, collectorElement: DxElement) => string | UserDefinedElement);
      /**
       * Specifies a custom template for appointments.
       */
      appointmentTemplate?: template | ((model: AppointmentTemplateData, itemIndex: number, contentElement: DxElement) => string | UserDefinedElement);
      /**
       * Specifies a custom template for tooltips displayed when users click an appointment or cell overflow indicator in this view.
       */
      appointmentTooltipTemplate?: template | ((model: AppointmentTooltipTemplateData, itemIndex: number, contentElement: DxElement) => string | UserDefinedElement);
      /**
       * Specifies a custom template for tooltips displayed when users click a cell overflow indicator in this view.
       * @deprecated Use the views.appointmentTooltipTemplate option instead.
       */
      dropDownAppointmentTemplate?: template | ((itemData: any, itemIndex: number, contentElement: DxElement) => string | UserDefinedElement);
      /**
       * The cell duration in minutes.
       */
      cellDuration?: number;
      /**
       * Specifies a custom template for table cells.
       */
      dataCellTemplate?: template | ((itemData: any, itemIndex: number, itemElement: DxElement) => string | UserDefinedElement);
      /**
       * Specifies a custom template for date scale items.
       */
      dateCellTemplate?: template | ((itemData: any, itemIndex: number, itemElement: DxElement) => string | UserDefinedElement);
      /**
       * Specifies the last hour on the view&apos;s time scale. Accepts integer values from 0 to 24.
       */
      endDayHour?: number;
      /**
       * The first day of a week. Does not apply to the agenda view.
       */
      firstDayOfWeek?: 0 | 1 | 2 | 3 | 4 | 5 | 6;
      /**
       * If true, appointments are grouped by date first and then by resource; opposite if false. Applies only if appointments are grouped and groupOrientation is &apos;horizontal&apos;.
       */
      groupByDate?: boolean;
      /**
       * Arranges resource headers vertically (in a column) or horizontally (in a row).
       */
      groupOrientation?: 'horizontal' | 'vertical';
      /**
       * The resource kinds by which appointments are grouped.
       */
      groups?: Array<string>;
      /**
       * Multiplies the default view interval. Applies to all view types except &apos;agenda&apos;.
       */
      intervalCount?: number;
      /**
       * Specifies the limit of full-sized appointments displayed per cell. Applies to all views except &apos;agenda&apos;.
       */
      maxAppointmentsPerCell?: number | 'auto' | 'unlimited';
      /**
       * A custom view name displayed in the view switcher.
       */
      name?: string;
      /**
       * Specifies a custom template for resource headers.
       */
      resourceCellTemplate?: template | ((itemData: any, itemIndex: number, itemElement: DxElement) => string | UserDefinedElement);
      /**
       * Specifies the date from which to start counting the view interval. Applies to all view types except &apos;agenda&apos;.
       */
      startDate?: Date | number | string;
      /**
       * Specifies the first hour on the view&apos;s time scale. Accepts integer values from 0 to 24.
       */
      startDayHour?: number;
      /**
       * Specifies a custom template for time scale items.
       */
      timeCellTemplate?: template | ((itemData: any, itemIndex: number, itemElement: DxElement) => string | UserDefinedElement);
      /**
       * The type of the view.
       */
      type?: 'agenda' | 'day' | 'month' | 'timelineDay' | 'timelineMonth' | 'timelineWeek' | 'timelineWorkWeek' | 'week' | 'workWeek';
      /**
       * Configures scrolling for a view.
       */
      scrolling?: dxSchedulerScrolling;
    }>;
}
/**
 * The Scheduler is a UI component that represents scheduled data and allows a user to manage and edit it.
 */
export default class dxScheduler extends Widget<dxSchedulerOptions> {
    /**
     * Adds an appointment.
     */
    addAppointment(appointment: dxSchedulerAppointment): void;
    /**
     * Deletes an appointment from the timetable and its object from the data source.
     */
    deleteAppointment(appointment: dxSchedulerAppointment): void;
    getDataSource(): DataSource;
    /**
     * Gets the current view&apos;s end date.
     */
    getEndViewDate(): Date;
    /**
     * Gets the current view&apos;s start date.
     */
    getStartViewDate(): Date;
    /**
     * Hides an appointment details form.
     */
    hideAppointmentPopup(saveChanges?: boolean): void;
    /**
     * Hides an appointment&apos;s or cell overflow indicator&apos;s tooltip.
     */
    hideAppointmentTooltip(): void;
    /**
     * Scrolls the current view to a specified position. Available for all views except &apos;agenda&apos;. You should specify the height property to use this method.
     */
    scrollTo(date: Date, group?: object, allDay?: boolean): void;
    /**
     * Scrolls the current view to a specific day and time.
     * @deprecated Use scrollTo instead.
     */
    scrollToTime(hours: number, minutes: number, date?: Date): void;
    /**
     * Shows the appointment details form.
     */
    showAppointmentPopup(appointmentData?: dxSchedulerAppointment, createNewAppointment?: boolean, currentAppointmentData?: dxSchedulerAppointment): void;
    /**
     * Shows a tooltip for a target element.
     */
    showAppointmentTooltip(appointmentData: dxSchedulerAppointment, target: string | UserDefinedElement, currentAppointmentData?: dxSchedulerAppointment): void;
    /**
     * Updates an appointment.
     */
    updateAppointment(target: dxSchedulerAppointment, appointment: dxSchedulerAppointment): void;
}

export type Appointment = dxSchedulerAppointment;

/**
 * @deprecated Use the Scheduler's Appointment type instead
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxSchedulerAppointment extends CollectionWidgetItem {
    /**
     * Specifies whether the appointment lasts all day.
     */
    allDay?: boolean;
    /**
     * Specifies a detail description of the appointment.
     */
    description?: string;
    /**
     * Specifies whether the appointment responds to user interaction.
     */
    disabled?: boolean;
    /**
     * Specifies the ending of the appointment.
     */
    endDate?: Date | string;
    /**
     * Specifies the time zone for an appointment&apos;s endDate. Accepts values from the IANA time zone database.
     */
    endDateTimeZone?: string;
    /**
     * Specifies the HTML markup to be inserted into the appointment element.
     */
    html?: string;
    /**
     * Specifies the start date and time of one or more appointments to exclude from a series. This property requires that you also set recurrenceRule.
     */
    recurrenceException?: string;
    /**
     * Specifies a recurrence rule based on which the Scheduler generates an appointment series.
     */
    recurrenceRule?: string;
    /**
     * Specifies the start of the appointment.
     */
    startDate?: Date | string;
    /**
     * Specifies the time zone for an appointment&apos;s startDate. Accepts values from the IANA time zone database.
     */
    startDateTimeZone?: string;
    /**
     * Specifies a template that should be used to render this appointment only.
     */
    template?: template;
    /**
     * Specifies the subject of the appointment.
     */
    text?: string;
    /**
     * Specifies whether or not an appointment must be displayed.
     */
    visible?: boolean;
}

export type Properties = dxSchedulerOptions;

/**
 * @deprecated use Properties instead
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export type Options = dxSchedulerOptions;

/**
 * 
 */
export interface dxSchedulerScrolling {
  /**
   * Specifies the scrolling mode.
   */
  mode?: 'standard' | 'virtual';
}
