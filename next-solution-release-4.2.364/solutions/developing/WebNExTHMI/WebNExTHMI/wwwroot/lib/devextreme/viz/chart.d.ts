/**
* DevExtreme (viz/chart.d.ts)
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
    Cancelable,
    EventInfo,
    NativeEventInfo,
    InitializedEventInfo,
    ChangedOptionInfo,
} from '../events/index';

import {
    Format,
  } from '../localization';

import {
    template,
} from '../core/templates/template';

import {
    BaseChart,
    BaseChartLegend,
    BaseChartOptions,
    BaseChartTooltip,
    BaseChartAnnotationConfig,
    PointInteractionInfo,
    TooltipInfo,
} from './chart_components/base_chart';

import {
    ChartSeries,
    ScaleBreak,
    VizRange,
    ChartSeriesType,
    DashStyleType,
    HatchingDirectionType,
    VizTimeInterval,
} from './common';

import {
    Font,
    WordWrapType,
    VizTextOverflowType,
    FileSavingEventInfo,
    ExportInfo,
    IncidentInfo,
} from './core/base_widget';

/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export type ChartSingleValueSeriesAggregationMethodType = 'avg' | 'count' | 'max' | 'min' | 'sum' | 'custom';

/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
interface SeriesInteractionInfo {
    target: chartSeriesObject;
}

export type ArgumentAxisClickEvent = NativeEventInfo<dxChart, MouseEvent | PointerEvent> & {
    readonly argument: Date | number | string;
};

export type DisposingEvent = EventInfo<dxChart>;

export type DoneEvent = EventInfo<dxChart>;

export type DrawnEvent = EventInfo<dxChart>;

export type ExportedEvent = EventInfo<dxChart>;

export type ExportingEvent = EventInfo<dxChart> & ExportInfo;

export type FileSavingEvent = FileSavingEventInfo<dxChart>;

export type IncidentOccurredEvent = EventInfo<dxChart> & IncidentInfo;

export type InitializedEvent = InitializedEventInfo<dxChart>;

export type LegendClickEvent = NativeEventInfo<dxChart, MouseEvent | PointerEvent> & {
    readonly target: chartSeriesObject;
};

export type OptionChangedEvent = EventInfo<dxChart> & ChangedOptionInfo;

export type PointClickEvent = NativeEventInfo<dxChart, MouseEvent | PointerEvent> & PointInteractionInfo;

export type PointHoverChangedEvent = EventInfo<dxChart> & PointInteractionInfo;

export type PointSelectionChangedEvent = EventInfo<dxChart> & PointInteractionInfo;

export type SeriesClickEvent = NativeEventInfo<dxChart, MouseEvent | PointerEvent> & {
    readonly target: chartSeriesObject;
};

export type SeriesHoverChangedEvent = EventInfo<dxChart> & SeriesInteractionInfo;

export type SeriesSelectionChangedEvent = EventInfo<dxChart> & SeriesInteractionInfo;

export type TooltipHiddenEvent = EventInfo<dxChart> & TooltipInfo;

export type TooltipShownEvent = EventInfo<dxChart> & TooltipInfo;

export type ZoomEndEvent = Cancelable & NativeEventInfo<dxChart, MouseEvent | TouchEvent> & {
    readonly rangeStart: Date | number;
    readonly rangeEnd: Date | number;
    readonly axis: chartAxisObject;
    readonly range: VizRange;
    readonly previousRange: VizRange;
    readonly actionType: 'zoom' | 'pan';
    readonly zoomFactor: number;
    readonly shift: number;
};

export type ZoomStartEvent = Cancelable & NativeEventInfo<dxChart, MouseEvent | TouchEvent> & {
    readonly axis: chartAxisObject;
    readonly range: VizRange;
    readonly actionType?: 'zoom' | 'pan';
};

/**
 * This section describes the Label object, which represents a point label.
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface baseLabelObject {
    /**
     * Gets the parameters of the label&apos;s minimum bounding rectangle (MBR).
     */
    getBoundingRect(): any;
    /**
     * Hides the point label.
     */
    hide(): void;
    /**
     * Hides the point label and keeps it invisible until the show() method is called.
     */
    hide(holdInvisible: boolean): void;
    /**
     * Checks whether the point label is visible.
     */
    isVisible(): boolean;
    /**
     * Shows the point label.
     */
    show(): void;
    /**
     * Shows the point label and keeps it visible until the hide() method is called.
     */
    show(holdVisible: boolean): void;
}

/**
 * This section describes the Point object, which represents a series point.
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface basePointObject {
    /**
     * Switches the point from the hover state back to normal.
     */
    clearHover(): void;
    /**
     * Deselects the point.
     */
    clearSelection(): void;
    /**
     * Contains the data object that the series point represents.
     */
    data?: any;
    /**
     * Provides information about the state of the point object.
     */
    fullState?: number;
    /**
     * Gets the color of a particular point.
     */
    getColor(): string;
    /**
     * Allows you to obtain the label(s) of the series point.
     */
    getLabel(): baseLabelObject & Array<baseLabelObject>;
    /**
     * Hides the tooltip of the point.
     */
    hideTooltip(): void;
    /**
     * Switches the point into the hover state, the same as when a user places the mouse pointer on it.
     */
    hover(): void;
    /**
     * Provides information about the hover state of a point.
     */
    isHovered(): boolean;
    /**
     * Provides information about the selection state of a point.
     */
    isSelected(): boolean;
    /**
     * Returns the point&apos;s argument value that was set in the data source.
     */
    originalArgument?: string | number | Date;
    /**
     * Returns the point&apos;s value that was set in the data source.
     */
    originalValue?: string | number | Date;
    /**
     * Selects the point. The point is displayed in a &apos;selected&apos; style until another point is selected or the current point is deselected programmatically.
     */
    select(): void;
    /**
     * Returns the series object to which the point belongs.
     */
    series?: any;
    /**
     * Shows the tooltip of the point.
     */
    showTooltip(): void;
    /**
     * Returns the tag of the point.
     */
    tag?: any;
}

/**
 * This section describes the Series object, which represents a series.
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface baseSeriesObject {
    /**
     * Switches the series from the hover state back to normal.
     */
    clearHover(): void;
    /**
     * Cancels the selection of this particular series. The series is displayed in its initial style.
     */
    clearSelection(): void;
    /**
     * Deselects the specified point. The point is displayed in an initial style.
     */
    deselectPoint(point: basePointObject): void;
    /**
     * Provides information about the state of the series object.
     */
    fullState?: number;
    /**
     * Gets all points in the series.
     */
    getAllPoints(): Array<basePointObject>;
    /**
     * Gets the color of a particular series.
     */
    getColor(): string;
    /**
     * Gets a series point with the specified index.
     */
    getPointByPos(positionIndex: number): basePointObject;
    /**
     * Gets a series point with the specified argument value.
     */
    getPointsByArg(pointArg: number | string | Date): Array<basePointObject>;
    /**
     * Gets visible series points.
     */
    getVisiblePoints(): Array<basePointObject>;
    /**
     * Hides a series at runtime.
     */
    hide(): void;
    /**
     * Switches the series into the hover state, the same as when a user places the mouse pointer on it.
     */
    hover(): void;
    /**
     * Provides information about the hover state of a series.
     */
    isHovered(): boolean;
    /**
     * Provides information about the selection state of a series.
     */
    isSelected(): boolean;
    /**
     * Provides information about the visibility state of a series.
     */
    isVisible(): boolean;
    /**
     * Returns the name of the series.
     */
    name?: any;
    /**
     * Selects the series.
     */
    select(): void;
    /**
     * Selects the specified point. The point is displayed in a &apos;selected&apos; style.
     */
    selectPoint(point: basePointObject): void;
    /**
     * Makes a particular series visible.
     */
    show(): void;
    /**
     * Returns the tag of the series.
     */
    tag?: any;
    /**
     * Returns the type of the series.
     */
    type?: string;
}

/**
 * This section describes the Axis object. This object represents a chart axis.
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface chartAxisObject {
    /**
     * Gets the axis&apos; displayed range.
     */
    visualRange(): VizRange;
    /**
     * Sets the axis&apos;s displayed range.
     */
    visualRange(visualRange: Array<number | string | Date> | VizRange): void;
}

/**
 * 
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface chartPointAggregationInfoObject {
    /**
     * Contains the length of the aggregation interval in axis units (numbers or dates). If the interval is set in pixels (using the aggregationGroupWidth property), it will be converted to axis units.
     */
    aggregationInterval?: any;
    /**
     * Contains data objects that were aggregated into this point.
     */
    data?: Array<any>;
    /**
     * Contains the end value of the interval to which the point belongs.
     */
    intervalEnd?: any;
    /**
     * Contains the start value of the interval to which the point belongs.
     */
    intervalStart?: any;
}

/**
 * This section describes the Point object, which represents a series point.
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface chartPointObject extends basePointObject {
    /**
     * Provides information about the aggregation interval and the data objects that fall within it.
     */
    aggregationInfo?: chartPointAggregationInfoObject;
    /**
     * Gets the parameters of the point&apos;s minimum bounding rectangle (MBR).
     */
    getBoundingRect(): any;
    /**
     * Contains the close value of the point. This field is useful for points belonging to a series of the candle stick or stock type only.
     */
    originalCloseValue?: number | string;
    /**
     * Contains the high value of the point. This field is useful for points belonging to a series of the candle stick or stock type only.
     */
    originalHighValue?: number | string;
    /**
     * Contains the low value of the point. This field is useful for points belonging to a series of the candle stick or stock type only.
     */
    originalLowValue?: number | string;
    /**
     * Contains the first value of the point. This field is useful for points belonging to a series of the range area or range bar type only.
     */
    originalMinValue?: string | number | Date;
    /**
     * Contains the open value of the point. This field is useful for points belonging to a series of the candle stick or stock type only.
     */
    originalOpenValue?: number | string;
    /**
     * Contains the size of the bubble as it was set in the data source. This field is useful for points belonging to a series of the bubble type only.
     */
    size?: number | string;
}
/**
 * This section describes the Series object, which represents a series.
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface chartSeriesObject extends baseSeriesObject {
    /**
     * Returns the name of the value axis of the series.
     */
    axis?: string;
    /**
     * The name of the series&apos; barOverlapGroup.
     */
    barOverlapGroup?: string;
    /**
     * Gets the argument axis to which the series belongs.
     */
    getArgumentAxis(): chartAxisObject;
    /**
     * Gets the value axis to which the series belongs.
     */
    getValueAxis(): chartAxisObject;
    /**
     * Returns the name of the series pane.
     */
    pane?: string;
    /**
     * The name of the series&apos; stack.
     */
    stack?: string;
}

/**
 * @deprecated use Properties instead
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxChartOptions extends BaseChartOptions<dxChart> {
    /**
     * Specifies whether to adjust the value axis&apos;s visualRange when the argument axis is being zoomed or panned.
     */
    adjustOnZoom?: boolean;
    /**
     * Specifies the annotation collection.
     */
    annotations?: Array<dxChartAnnotationConfig | any>;
    /**
     * Configures the argument axis.
     */
    argumentAxis?: dxChartArgumentAxis;
    /**
     * Specifies whether to hide series point markers automatically to reduce visual clutter.
     */
    autoHidePointMarkers?: boolean;
    /**
     * Controls the padding and consequently the width of a group of bars with the same argument using relative units. Ignored if the barGroupWidth property is set.
     */
    barGroupPadding?: number;
    /**
     * Specifies a fixed width for groups of bars with the same argument, measured in pixels. Takes precedence over the barGroupPadding property.
     */
    barGroupWidth?: number;
    /**
     * Specifies settings common for all annotations in the chart.
     */
    commonAnnotationSettings?: dxChartCommonAnnotationConfig;
    /**
     * Defines common settings for both the argument and value axis in a chart.
     */
    commonAxisSettings?: dxChartCommonAxisSettings;
    /**
     * Defines common settings for all panes in a chart.
     */
    commonPaneSettings?: dxChartCommonPaneSettings;
    /**
     * Specifies settings common for all series in the chart.
     */
    commonSeriesSettings?: dxChartCommonSeriesSettings;
    /**
     * Colors the background of the chart container.
     */
    containerBackgroundColor?: string;
    /**
     * Configures the crosshair feature.
     */
    crosshair?: {
      /**
       * Specifies the color of the crosshair lines.
       */
      color?: string;
      /**
       * Specifies the dash style of the crosshair lines.
       */
      dashStyle?: DashStyleType;
      /**
       * Enables the crosshair.
       */
      enabled?: boolean;
      /**
       * Configures the horizontal crosshair line individually.
       */
      horizontalLine?: {
        /**
         * Specifies the color of the horizontal crosshair line.
         */
        color?: string;
        /**
         * Specifies the dash style of the horizontal crosshair line.
         */
        dashStyle?: DashStyleType;
        /**
         * Configures the label that belongs to the horizontal crosshair line.
         */
        label?: {
          /**
           * Paints the background of the label that belongs to the horizontal crosshair line.
           */
          backgroundColor?: string;
          /**
           * Customizes the text displayed by the label that belongs to the horizontal crosshair line.
           */
          customizeText?: ((info: { value?: Date | number | string; valueText?: string; point?: chartPointObject }) => string);
          /**
           * Specifies font properties for the label of the horizontal crosshair line.
           */
          font?: Font;
          /**
           * Formats a point value before it is displayed in the crosshair label.
           */
          format?: Format;
          /**
           * Makes the label of the horizontal crosshair line visible. Applies only if the crosshair feature is enabled and the horizontal line is visible.
           */
          visible?: boolean;
        };
        /**
         * Specifies how transparent the horizontal crosshair line should be.
         */
        opacity?: number;
        /**
         * Specifies whether to show the horizontal crosshair line or not.
         */
        visible?: boolean;
        /**
         * Specifies the width of the horizontal crosshair line in pixels.
         */
        width?: number;
      } | boolean;
      /**
       * Configures the crosshair labels.
       */
      label?: {
        /**
         * Paints the background of the crosshair labels.
         */
        backgroundColor?: string;
        /**
         * Customizes the text displayed by the crosshair labels.
         */
        customizeText?: ((info: { value?: Date | number | string; valueText?: string; point?: chartPointObject }) => string);
        /**
         * Specifies font properties for the crosshair labels.
         */
        font?: Font;
        /**
         * Formats a point value/argument before it is displayed in the crosshair label.
         */
        format?: Format;
        /**
         * Makes the crosshair labels visible. Applies only if the crosshair feature is enabled.
         */
        visible?: boolean;
      };
      /**
       * Specifies how transparent the crosshair lines should be.
       */
      opacity?: number;
      /**
       * Configures the vertical crosshair line individually.
       */
      verticalLine?: {
        /**
         * Specifies the color of the vertical crosshair line.
         */
        color?: string;
        /**
         * Specifies the dash style of the vertical crosshair line.
         */
        dashStyle?: DashStyleType;
        /**
         * Configures the label that belongs to the vertical crosshair line.
         */
        label?: {
          /**
           * Paints the background of the label that belongs to the vertical crosshair line.
           */
          backgroundColor?: string;
          /**
           * Customizes the text displayed by the label that belongs to the vertical crosshair line.
           */
          customizeText?: ((info: { value?: Date | number | string; valueText?: string; point?: chartPointObject }) => string);
          /**
           * Specifies font properties for the label of the vertical crosshair line.
           */
          font?: Font;
          /**
           * Formats the point argument before it is displayed in the crosshair label.
           */
          format?: Format;
          /**
           * Makes the label of the vertical crosshair line visible. Applies only if the crosshair feature is enabled and the vertical line is visible.
           */
          visible?: boolean;
        };
        /**
         * Specifies how transparent the vertical crosshair line should be.
         */
        opacity?: number;
        /**
         * Specifies whether to show the vertical crosshair line or not.
         */
        visible?: boolean;
        /**
         * Specifies the width of the vertical crosshair line in pixels.
         */
        width?: number;
      } | boolean;
      /**
       * Specifies the width of the crosshair lines.
       */
      width?: number;
    };
    /**
     * Customizes an individual annotation.
     */
    customizeAnnotation?: ((annotation: dxChartAnnotationConfig | any) => dxChartAnnotationConfig);
    /**
     * Processes data before visualizing it.
     */
    dataPrepareSettings?: {
      /**
       * Validates the type of each value coming from the data source.
       */
      checkTypeForAllData?: boolean;
      /**
       * Converts data coming from a data source into a data type supported by the axis.
       */
      convertToAxisDataType?: boolean;
      /**
       * Specifies the sorting order in which series points should be drawn.
       */
      sortingMethod?: boolean | ((a: any, b: any) => number);
    };
    /**
     * Specifies which pane should be used by default.
     */
    defaultPane?: string;
    /**
     * Specifies the properties of a chart&apos;s legend.
     */
    legend?: dxChartLegend;
    /**
     * Specifies a coefficient determining the diameter of the largest bubble.
     */
    maxBubbleSize?: number;
    /**
     * Specifies the diameter of the smallest bubble measured in pixels.
     */
    minBubbleSize?: number;
    /**
     * Forces the UI component to treat negative values as zeroes. Applies to stacked-like series only.
     */
    negativesAsZeroes?: boolean;
    /**
     * A function that is executed when a label on the argument axis is clicked or tapped.
     */
    onArgumentAxisClick?: ((e: ArgumentAxisClickEvent) => void) | string;
    /**
     * A function that is executed when a legend item is clicked or tapped.
     */
    onLegendClick?: ((e: LegendClickEvent) => void) | string;
    /**
     * A function that is executed when a series is clicked or tapped.
     */
    onSeriesClick?: ((e: SeriesClickEvent) => void) | string;
    /**
     * A function that is executed after the pointer enters or leaves a series.
     */
    onSeriesHoverChanged?: ((e: SeriesHoverChangedEvent) => void);
    /**
     * A function that is executed when a series is selected or selection is canceled.
     */
    onSeriesSelectionChanged?: ((e: SeriesSelectionChangedEvent) => void);
    /**
     * A function that is executed when zooming or panning ends.
     */
    onZoomEnd?: ((e: ZoomEndEvent) => void);
    /**
     * A function that is executed when zooming or panning begins.
     */
    onZoomStart?: ((e: ZoomStartEvent) => void);
    /**
     * Declares a collection of panes.
     */
    panes?: dxChartPanes | Array<dxChartPanes>;
    /**
     * Specifies whether panes can be resized if other chart elements require more space after zooming or panning.
     */
    resizePanesOnZoom?: boolean;
    /**
     * Specifies how the chart must behave when series point labels overlap.
     */
    resolveLabelOverlapping?: 'hide' | 'none' | 'stack';
    /**
     * Swaps the axes around making the value axis horizontal and the argument axis vertical.
     */
    rotated?: boolean;
    /**
     * Specifies the settings of the scroll bar.
     */
    scrollBar?: {
      /**
       * Specifies the color of the scroll bar.
       */
      color?: string;
      /**
       * Specifies the spacing between the scroll bar and the chart&apos;s plot in pixels.
       */
      offset?: number;
      /**
       * Specifies the opacity of the scroll bar.
       */
      opacity?: number;
      /**
       * Specifies the position of the scroll bar in the chart.
       */
      position?: 'bottom' | 'left' | 'right' | 'top';
      /**
       * Specifies whether the scroll bar is visible or not.
       */
      visible?: boolean;
      /**
       * Specifies the width of the scroll bar in pixels.
       */
      width?: number;
    };
    /**
     * Specifies properties for Chart UI component series.
     */
    series?: ChartSeries | Array<ChartSeries>;
    /**
     * Specifies whether a single series or multiple series can be selected in the chart.
     */
    seriesSelectionMode?: 'multiple' | 'single';
    /**
     * Defines properties for the series template.
     */
    seriesTemplate?: {
      /**
       * Specifies a callback function that returns a series object with individual series settings.
       */
      customizeSeries?: ((seriesName: any) => ChartSeries);
      /**
       * Specifies a data source field that represents the series name.
       */
      nameField?: string;
    };
    /**
     * Specifies whether a point should remain in the hover state when the mouse pointer moves away.
     */
    stickyHovering?: boolean;
    /**
     * Indicates whether or not to synchronize value axes when they are displayed on a single pane.
     */
    synchronizeMultiAxes?: boolean;
    /**
     * Configures tooltips.
     */
    tooltip?: dxChartTooltip;
    /**
     * Configures the value axis.
     */
    valueAxis?: dxChartValueAxis | Array<dxChartValueAxis>;
    /**
     * Configures zooming and panning.
     */
    zoomAndPan?: {
      /**
       * Specifies whether users can use the mouse wheel to zoom the chart. Applies only if zoom is allowed for the argument or value axis.
       */
      allowMouseWheel?: boolean;
      /**
       * Specifies whether users can use touch gestures to zoom or pan the chart. Applies only if zoom and pan are allowed for the argument or value axis.
       */
      allowTouchGestures?: boolean;
      /**
       * Specifies whether users are allowed to zoom and/or pan the argument axis.
       */
      argumentAxis?: 'both' | 'none' | 'pan' | 'zoom';
      /**
       * Configures the box that appears when users zoom the chart by selecting an area with the drag gesture. Applies only if dragToZoom is true.
       */
      dragBoxStyle?: {
        /**
         * Specifies the drag box&apos;s color.
         */
        color?: string;
        /**
         * Specifies the drag box&apos;s transparency.
         */
        opacity?: number;
      };
      /**
       * Enables users to zoom the chart by selecting an area with the drag gesture. Applies only on devices that use a mouse.
       */
      dragToZoom?: boolean;
      /**
       * Specifies the key that enables panning when dragToZoom is true. Applies only on devices that use a mouse.
       */
      panKey?: 'alt' | 'ctrl' | 'meta' | 'shift';
      /**
       * Specifies whether users are allowed to zoom and/or pan the value axis.
       */
      valueAxis?: 'both' | 'none' | 'pan' | 'zoom';
    };
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxChartArgumentAxis extends dxChartCommonAxisSettings {
    /**
     * Aggregates series points that fall into the same category.
     */
    aggregateByCategory?: boolean;
    /**
     * Specifies the length of aggregation intervals in pixels. Does not apply if aggregateByCategory is true. May be ignored in favor of the aggregationInterval property.
     */
    aggregationGroupWidth?: number;
    /**
     * Specifies the length of aggregation intervals in axis units. Applies only to axes of continuous and logarithmic types.
     */
    aggregationInterval?: VizTimeInterval;
    /**
     * Casts arguments to a specified data type.
     */
    argumentType?: 'datetime' | 'numeric' | 'string';
    /**
     * Specifies the minimum distance between two neighboring major ticks in pixels. Applies only to the axes of the &apos;continuous&apos; and &apos;logarithmic&apos; types.
     */
    axisDivisionFactor?: number;
    /**
     * Declares a scale break collection. Applies only if the axis&apos; type is &apos;continuous&apos; or &apos;logarithmic&apos;.
     */
    breaks?: Array<ScaleBreak>;
    /**
     * Specifies the order of categories on an axis of the &apos;discrete&apos; type.
     */
    categories?: Array<number | string | Date>;
    /**
     * Specifies the appearance of those constant lines that belong to the argument axis.
     */
    constantLineStyle?: dxChartArgumentAxisConstantLineStyle;
    /**
     * Declares a collection of constant lines belonging to the argument axis.
     */
    constantLines?: Array<dxChartArgumentAxisConstantLines>;
    /**
     * Specifies whether to force the axis to start and end on ticks.
     */
    endOnTick?: boolean;
    /**
     * Dates to be excluded from the axis when workdaysOnly is true.
     */
    holidays?: Array<Date | string> | Array<number>;
    /**
     * Specifies chart elements to be highlighted when a user points to an axis label.
     */
    hoverMode?: 'allArgumentPoints' | 'none';
    /**
     * Configures the labels of the argument axis.
     */
    label?: dxChartArgumentAxisLabel;
    /**
     * Specifies a value used to calculate the range on a logarithmic axis within which the axis should be linear. Applies only if the data source contains negative values or zeroes.
     */
    linearThreshold?: number;
    /**
     * Specifies the value to be raised to a power when generating ticks for an axis of the &apos;logarithmic&apos; type.
     */
    logarithmBase?: number;
    /**
     * Specifies the minimum length of the visual range.
     */
    minVisualRangeLength?: VizTimeInterval;
    /**
     * Specifies how many minor ticks to place between two neighboring major ticks.
     */
    minorTickCount?: number;
    /**
     * Specifies the interval between minor ticks. Applies only to the axes of the &apos;continuous&apos; type.
     */
    minorTickInterval?: VizTimeInterval;
    /**
     * Relocates the argument axis.
     */
    position?: 'bottom' | 'left' | 'right' | 'top';
    /**
     * Specifies the position of the argument axis on the value axis.
     */
    customPosition?: number | Date | string;
    /**
     * Specifies the name of a value axis on which the argument axis should be positioned. Applies only to multi-axis charts.
     */
    customPositionAxis?: string;
    /**
     * Specifies the shift in pixels of the argument axis.
     */
    offset?: number;
    /**
     * Dates to be included on the axis when workdaysOnly is true.
     */
    singleWorkdays?: Array<Date | string> | Array<number>;
    /**
     * Declares a collection of strips belonging to the argument axis.
     */
    strips?: Array<dxChartArgumentAxisStrips>;
    /**
     * Specifies the interval between major ticks.
     */
    tickInterval?: VizTimeInterval;
    /**
     * Configures the axis title.
     */
    title?: dxChartArgumentAxisTitle;
    /**
     * Specifies the type of the argument axis.
     */
    type?: 'continuous' | 'discrete' | 'logarithmic';
    /**
     * Defines the axis&apos; displayed range. Cannot be wider than the wholeRange.
     */
    visualRange?: VizRange | Array<number | string | Date>;
    /**
     * Specifies how the axis&apos;s visual range should behave when chart data is updated.
     */
    visualRangeUpdateMode?: 'auto' | 'keep' | 'reset' | 'shift';
    /**
     * Defines the range where the axis can be zoomed and panned.
     */
    wholeRange?: VizRange | Array<number | string | Date>;
    /**
     * Specifies which days are workdays. The array can contain values from 0 (Sunday) to 6 (Saturday). Applies only if workdaysOnly is true.
     */
    workWeek?: Array<number>;
    /**
     * Leaves only workdays on the axis: the work week days plus single workdays minus holidays. Applies only if the axis&apos; argumentType is &apos;datetime&apos;.
     */
    workdaysOnly?: boolean;
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxChartArgumentAxisConstantLineStyle extends dxChartCommonAxisSettingsConstantLineStyle {
    /**
     * Specifies the appearance of the labels of those constant lines that belong to the argument axis.
     */
    label?: dxChartArgumentAxisConstantLineStyleLabel;
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxChartArgumentAxisConstantLineStyleLabel extends dxChartCommonAxisSettingsConstantLineStyleLabel {
    /**
     * Aligns constant line labels in the horizontal direction.
     */
    horizontalAlignment?: 'center' | 'left' | 'right';
    /**
     * Aligns constant line labels in the vertical direction.
     */
    verticalAlignment?: 'bottom' | 'center' | 'top';
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxChartArgumentAxisConstantLines extends dxChartCommonAxisSettingsConstantLineStyle {
    /**
     * Specifies whether to display the constant line behind or in front of the series.
     */
    displayBehindSeries?: boolean;
    /**
     * Specifies whether to extend the axis&apos;s default visual range to display the constant line.
     */
    extendAxis?: boolean;
    /**
     * Configures the constant line label.
     */
    label?: dxChartArgumentAxisConstantLinesLabel;
    /**
     * Specifies the value indicated by a constant line. Setting this property is necessary.
     */
    value?: number | Date | string;
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxChartArgumentAxisConstantLinesLabel extends dxChartCommonAxisSettingsConstantLineStyleLabel {
    /**
     * Aligns constant line labels in the horizontal direction.
     */
    horizontalAlignment?: 'center' | 'left' | 'right';
    /**
     * Specifies the text of a constant line label. By default, equals to the value of the constant line.
     */
    text?: string;
    /**
     * Aligns constant line labels in the vertical direction.
     */
    verticalAlignment?: 'bottom' | 'center' | 'top';
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxChartArgumentAxisLabel extends dxChartCommonAxisSettingsLabel {
    /**
     * Specifies the hint that appears when a user points to an axis label.
     */
    customizeHint?: ((argument: { value?: Date | number | string; valueText?: string }) => string);
    /**
     * Customizes the text displayed by axis labels.
     */
    customizeText?: ((argument: { value?: Date | number | string; valueText?: string }) => string);
    /**
     * Formats a value before it is displayed in an axis label.
     */
    format?: Format;
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxChartArgumentAxisStrips extends dxChartCommonAxisSettingsStripStyle {
    /**
     * Specifies the color of the strip.
     */
    color?: string;
    /**
     * Along with the startValue property, limits the strip.
     */
    endValue?: number | Date | string;
    /**
     * Configures the strip label.
     */
    label?: dxChartArgumentAxisStripsLabel;
    /**
     * Along with the endValue property, limits the strip.
     */
    startValue?: number | Date | string;
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxChartArgumentAxisStripsLabel extends dxChartCommonAxisSettingsStripStyleLabel {
    /**
     * Specifies the text of the strip label.
     */
    text?: string;
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxChartArgumentAxisTitle extends dxChartCommonAxisSettingsTitle {
    /**
     * Specifies the text of the axis title.
     */
    text?: string;
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxChartCommonAxisSettings {
    /**
     * Specifies whether to allow decimal values on the axis. When false, the axis contains integer values only.
     */
    allowDecimals?: boolean;
    /**
     * Configures the scale breaks&apos; appearance.
     */
    breakStyle?: {
      /**
       * Specifies the scale breaks&apos; color.
       */
      color?: string;
      /**
       * Specifies the scale breaks&apos; line style.
       */
      line?: 'straight' | 'waved';
      /**
       * Specifies the scale breaks&apos; width in pixels.
       */
      width?: number;
    };
    /**
     * Specifies the color of the axis line.
     */
    color?: string;
    /**
     * Configures the appearance of all constant lines in the UI component.
     */
    constantLineStyle?: dxChartCommonAxisSettingsConstantLineStyle;
    /**
     * Specifies whether ticks and grid lines should cross axis labels or lie between them. Applies only to the axes of the &apos;discrete&apos; type.
     */
    discreteAxisDivisionMode?: 'betweenLabels' | 'crossLabels';
    /**
     * Specifies whether to force the axis to start and end on ticks.
     */
    endOnTick?: boolean;
    /**
     * Configures the grid.
     */
    grid?: {
      /**
       * Specifies the color of grid lines.
       */
      color?: string;
      /**
       * Specifies how transparent grid lines should be.
       */
      opacity?: number;
      /**
       * Makes grid lines visible.
       */
      visible?: boolean;
      /**
       * Specifies the width of grid lines in pixels.
       */
      width?: number;
    };
    /**
     * Inverts the axis.
     */
    inverted?: boolean;
    /**
     * Configures axis labels.
     */
    label?: dxChartCommonAxisSettingsLabel;
    /**
     * Controls the empty space between the maximum series points and the axis. Applies only to the axes of the &apos;continuous&apos; and &apos;logarithmic&apos; type.
     */
    maxValueMargin?: number;
    /**
     * Controls the empty space between the minimum series points and the axis. Applies only to the axes of the &apos;continuous&apos; and &apos;logarithmic&apos; type.
     */
    minValueMargin?: number;
    /**
     * Configures the minor grid.
     */
    minorGrid?: {
      /**
       * Specifies a color for the lines of the minor grid.
       */
      color?: string;
      /**
       * Specifies how transparent the lines of the minor grid should be.
       */
      opacity?: number;
      /**
       * Makes the minor grid visible.
       */
      visible?: boolean;
      /**
       * Specifies a width for the lines of the minor grid in pixels.
       */
      width?: number;
    };
    /**
     * Configures the appearance of minor axis ticks.
     */
    minorTick?: {
      /**
       * Specifies the color of minor ticks.
       */
      color?: string;
      /**
       * Specifies the length of minor ticks in pixels.
       */
      length?: number;
      /**
       * Specifies how transparent minor ticks should be.
       */
      opacity?: number;
      /**
       * Shifts minor ticks from the reference position.
       */
      shift?: number;
      /**
       * Makes minor ticks visible.
       */
      visible?: boolean;
      /**
       * Specifies the width of minor ticks in pixels.
       */
      width?: number;
    };
    /**
     * Specifies how transparent the axis line should be.
     */
    opacity?: number;
    /**
     * Reserves a pixel-measured space for the axis.
     */
    placeholderSize?: number;
    /**
     * Configures the appearance of strips.
     */
    stripStyle?: dxChartCommonAxisSettingsStripStyle;
    /**
     * Configures the appearance of major axis ticks.
     */
    tick?: {
      /**
       * Specifies the color of ticks.
       */
      color?: string;
      /**
       * Specifies the length of ticks in pixels.
       */
      length?: number;
      /**
       * Specifies how transparent ticks should be.
       */
      opacity?: number;
      /**
       * Shifts ticks from the reference position.
       */
      shift?: number;
      /**
       * Makes ticks visible.
       */
      visible?: boolean;
      /**
       * Specifies the width of ticks in pixels.
       */
      width?: number;
    };
    /**
     * Configures axis titles.
     */
    title?: dxChartCommonAxisSettingsTitle;
    /**
     * Adds an empty space between the axis and the minimum and maximum series points.
     */
    valueMarginsEnabled?: boolean;
    /**
     * Makes the axis line visible.
     */
    visible?: boolean;
    /**
     * Specifies the width of the axis line in pixels.
     */
    width?: number;
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxChartCommonAxisSettingsConstantLineStyle {
    /**
     * Specifies the color of constant lines.
     */
    color?: string;
    /**
     * Specifies the dash style of constant lines.
     */
    dashStyle?: DashStyleType;
    /**
     * Configures constant line labels.
     */
    label?: dxChartCommonAxisSettingsConstantLineStyleLabel;
    /**
     * Generates a pixel-measured empty space between the left/right side of a constant line and the constant line label.
     */
    paddingLeftRight?: number;
    /**
     * Generates a pixel-measured empty space between the top/bottom side of a constant line and the constant line label.
     */
    paddingTopBottom?: number;
    /**
     * Specifies the width of constant lines in pixels.
     */
    width?: number;
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxChartCommonAxisSettingsConstantLineStyleLabel {
    /**
     * Specifies font properties for constant line labels.
     */
    font?: Font;
    /**
     * Specifies the position of constant line labels on the chart plot.
     */
    position?: 'inside' | 'outside';
    /**
     * Makes constant line labels visible.
     */
    visible?: boolean;
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxChartCommonAxisSettingsLabel {
    /**
     * Specifies a custom template for axis labels.
     */
    template?: template | ((data: object, element: SVGGElement) => string | UserDefinedElement<SVGElement>);
    /**
     * Aligns axis labels in relation to ticks.
     */
    alignment?: 'center' | 'left' | 'right';
    /**
     * Allows you to rotate or stagger axis labels. Applies to the horizontal axis only.
     */
    displayMode?: 'rotate' | 'stagger' | 'standard';
    /**
     * Specifies font properties for axis labels.
     */
    font?: Font;
    /**
     * Adds a pixel-measured empty space between an axis and its labels.
     */
    indentFromAxis?: number;
    /**
     * Decides how to arrange axis labels when there is not enough space to keep all of them.
     */
    overlappingBehavior?: 'rotate' | 'stagger' | 'none' | 'hide';
    /**
     * Specifies the position of labels relative to the chart or its axis.
     */
    position?: 'inside' | 'outside' | 'bottom' | 'left' | 'right' | 'top';
    /**
     * Specifies the rotation angle of axis labels. Applies only if displayMode or overlappingBehavior is &apos;rotate&apos;.
     */
    rotationAngle?: number;
    /**
     * Adds a pixel-measured empty space between two staggered rows of axis labels. Applies only if displayMode or overlappingBehavior is &apos;stagger&apos;.
     */
    staggeringSpacing?: number;
    /**
     * Specifies what to do with axis labels that overflow the allocated space after applying wordWrap: hide, truncate them and display an ellipsis, or do nothing.
     */
    textOverflow?: VizTextOverflowType;
    /**
     * Shows/hides axis labels.
     */
    visible?: boolean;
    /**
     * Specifies how to wrap texts that do not fit into a single line.
     */
    wordWrap?: WordWrapType;
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxChartCommonAxisSettingsStripStyle {
    /**
     * Configures the appearance of strip labels.
     */
    label?: dxChartCommonAxisSettingsStripStyleLabel;
    /**
     * Generates a pixel-measured empty space between the left/right border of a strip and the strip label.
     */
    paddingLeftRight?: number;
    /**
     * Generates a pixel-measured empty space between the top/bottom border of a strip and the strip label.
     */
    paddingTopBottom?: number;
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxChartCommonAxisSettingsStripStyleLabel {
    /**
     * Specifies font properties for strip labels.
     */
    font?: Font;
    /**
     * Aligns strip labels in the horizontal direction.
     */
    horizontalAlignment?: 'center' | 'left' | 'right';
    /**
     * Aligns strip labels in the vertical direction.
     */
    verticalAlignment?: 'bottom' | 'center' | 'top';
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxChartCommonAxisSettingsTitle {
    /**
     * Aligns the axis title to the left, center, or right of the axis.
     */
    alignment?: 'center' | 'left' | 'right';
    /**
     * Specifies font properties for the axis title.
     */
    font?: Font;
    /**
     * Adds a pixel-measured empty space between the axis title and axis labels.
     */
    margin?: number;
    /**
     * Specifies what to do with the axis title when it overflows the allocated space after applying wordWrap: hide, truncate them and display an ellipsis, or do nothing.
     */
    textOverflow?: VizTextOverflowType;
    /**
     * Specifies how to wrap the axis title if it does not fit into a single line.
     */
    wordWrap?: WordWrapType;
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxChartCommonPaneSettings {
    /**
     * Specifies the color of the pane&apos;s background.
     */
    backgroundColor?: string;
    /**
     * Configures the pane border.
     */
    border?: {
      /**
       * Shows/hides the bottom border of the pane. Applies only when the border.visible property is true.
       */
      bottom?: boolean;
      /**
       * Specifies the color of the pane border.
       */
      color?: string;
      /**
       * Specifies the dash style of the pane border.
       */
      dashStyle?: DashStyleType;
      /**
       * Shows/hides the left border of the pane. Applies only when the border.visible property is true.
       */
      left?: boolean;
      /**
       * Specifies how transparent the pane border should be.
       */
      opacity?: number;
      /**
       * Shows/hides the right border of the pane. Applies only when the border.visible property is true.
       */
      right?: boolean;
      /**
       * Shows/hides the top border of the pane. Applies only when the border.visible property is true.
       */
      top?: boolean;
      /**
       * Shows the pane border.
       */
      visible?: boolean;
      /**
       * Specifies the width of the pane border in pixels.
       */
      width?: number;
    };
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxChartCommonSeriesSettings extends dxChartSeriesTypesCommonSeries {
    /**
     * Defines common settings for all area series.
     */
    area?: any;
    /**
     * Defines common settings for all bar series.
     */
    bar?: any;
    /**
     * Defines common settings for all bubble series.
     */
    bubble?: any;
    /**
     * Defines common settings for all candlestick series.
     */
    candlestick?: any;
    /**
     * Defines common settings for all full-stacked area series.
     */
    fullstackedarea?: any;
    /**
     * Defines common settings for all full-stacked bar series.
     */
    fullstackedbar?: any;
    /**
     * Defines common settings for all full-stacked line series.
     */
    fullstackedline?: any;
    /**
     * Defines common settings for all full-stacked spline series.
     */
    fullstackedspline?: any;
    /**
     * Defines common settings for all full-stacked spline area series.
     */
    fullstackedsplinearea?: any;
    /**
     * Defines common settings for all line series.
     */
    line?: any;
    /**
     * Defines common settings for all range area series.
     */
    rangearea?: any;
    /**
     * Defines common settings for all range bar series.
     */
    rangebar?: any;
    /**
     * Defines common settings for all scatter series.
     */
    scatter?: any;
    /**
     * Defines common settings for all spline series.
     */
    spline?: any;
    /**
     * Defines common settings for all spline area series.
     */
    splinearea?: any;
    /**
     * Defines common settings for all stacked area series.
     */
    stackedarea?: any;
    /**
     * Defines common settings for all stacked bar series.
     */
    stackedbar?: any;
    /**
     * Defines common settings for all stacked line series.
     */
    stackedline?: any;
    /**
     * Defines common settings for all stacked spline series.
     */
    stackedspline?: any;
    /**
     * Defines common settings for all stacked spline area series.
     */
    stackedsplinearea?: any;
    /**
     * Defines common settings for all step area series.
     */
    steparea?: any;
    /**
     * Defines common settings for all step line series.
     */
    stepline?: any;
    /**
     * Defines common settings for all stock series.
     */
    stock?: any;
    /**
     * Specifies the type of the series.
     */
    type?: ChartSeriesType;
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxChartLegend extends BaseChartLegend {
    /**
     * Specifies the text for a hint that appears when a user hovers the mouse pointer over a legend item.
     */
    customizeHint?: ((seriesInfo: { seriesName?: any; seriesIndex?: number; seriesColor?: string }) => string);
    /**
     * Specifies a callback function that returns the text to be displayed by a legend item.
     */
    customizeText?: ((seriesInfo: { seriesName?: any; seriesIndex?: number; seriesColor?: string }) => string);
    /**
     * Specifies what series elements to highlight when a corresponding item in the legend is hovered over.
     */
    hoverMode?: 'excludePoints' | 'includePoints' | 'none';
    /**
     * Specifies whether the legend is located outside or inside the chart&apos;s plot.
     */
    position?: 'inside' | 'outside';
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxChartPanes extends dxChartCommonPaneSettings {
    /**
     * Specifies the pane&apos;s height (or width when the chart is rotated) in a multi-pane chart.
     */
    height?: number | string;
    /**
     * Specifies the name of the pane.
     */
    name?: string;
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxChartTooltip extends BaseChartTooltip {
    /**
     * Specifies whether the tooltip must be located in the center of a series point or on its edge. Applies to bar-like and bubble series only.
     */
    location?: 'center' | 'edge';
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxChartValueAxis extends dxChartCommonAxisSettings {
    /**
     * Enables auto-calculated scale breaks. Applies only if the axis&apos; type is &apos;continuous&apos; or &apos;logarithmic&apos; and valueType is &apos;numeric&apos;.
     */
    autoBreaksEnabled?: boolean;
    /**
     * Specifies the minimum distance between two neighboring major ticks in pixels. Applies only to the axes of the &apos;continuous&apos; and &apos;logarithmic&apos; types.
     */
    axisDivisionFactor?: number;
    /**
     * Declares a custom scale break collection. Applies only if the axis&apos; type is &apos;continuous&apos; or &apos;logarithmic&apos;.
     */
    breaks?: Array<ScaleBreak>;
    /**
     * Specifies the order of categories on an axis of the &apos;discrete&apos; type.
     */
    categories?: Array<number | string | Date>;
    /**
     * Specifies the appearance of those constant lines that belong to the value axis.
     */
    constantLineStyle?: dxChartValueAxisConstantLineStyle;
    /**
     * Declares a collection of constant lines belonging to the value axis.
     */
    constantLines?: Array<dxChartValueAxisConstantLines>;
    /**
     * Specifies whether to force the axis to start and end on ticks.
     */
    endOnTick?: boolean;
    /**
     * Configures the labels of the value axis.
     */
    label?: dxChartValueAxisLabel;
    /**
     * Specifies a value used to calculate the range on a logarithmic axis within which the axis should be linear. Applies only if the data source contains negative values or zeroes.
     */
    linearThreshold?: number;
    /**
     * Specifies the value to be raised to a power when generating ticks for an axis of the &apos;logarithmic&apos; type.
     */
    logarithmBase?: number;
    /**
     * Sets a limit on auto-calculated scale breaks. Custom scale breaks are not counted.
     */
    maxAutoBreakCount?: number;
    /**
     * Specifies the minimum length of the visual range.
     */
    minVisualRangeLength?: VizTimeInterval;
    /**
     * Specifies how many minor ticks to place between two neighboring major ticks.
     */
    minorTickCount?: number;
    /**
     * Specifies the interval between minor ticks. Applies only to continuous axes.
     */
    minorTickInterval?: VizTimeInterval;
    /**
     * Adds a pixel-measured empty space between two side-by-side value axes. Applies if several value axes are located on one side of the chart.
     */
    multipleAxesSpacing?: number;
    /**
     * Specifies the name of the value axis.
     */
    name?: string;
    /**
     * Binds the value axis to a pane.
     */
    pane?: string;
    /**
     * Relocates the value axis.
     */
    position?: 'bottom' | 'left' | 'right' | 'top';
    /**
     * Specifies the position of the value axis on the argument axis.
     */
    customPosition?: number | Date | string;
    /**
     * Specifies the shift in pixels of the value axis.
     */
    offset?: number;
    /**
     * Specifies whether or not to show zero on the value axis.
     */
    showZero?: boolean;
    /**
     * Declares a collection of strips belonging to the value axis.
     */
    strips?: Array<dxChartValueAxisStrips>;
    /**
     * Synchronizes two or more value axes with each other at a specific value.
     */
    synchronizedValue?: number;
    /**
     * Specifies the interval between major ticks. Does not apply to discrete axes.
     */
    tickInterval?: VizTimeInterval;
    /**
     * Configures the axis title.
     */
    title?: dxChartValueAxisTitle;
    /**
     * Specifies the type of the value axis.
     */
    type?: 'continuous' | 'discrete' | 'logarithmic';
    /**
     * Casts values to a specified data type.
     */
    valueType?: 'datetime' | 'numeric' | 'string';
    /**
     * Defines the axis&apos; displayed range. Cannot be wider than the wholeRange.
     */
    visualRange?: VizRange | Array<number | string | Date>;
    /**
     * Specifies how the axis&apos;s visual range should behave when chart data is updated.
     */
    visualRangeUpdateMode?: 'auto' | 'keep' | 'reset' | 'shift';
    /**
     * Defines the range where the axis can be zoomed and panned.
     */
    wholeRange?: VizRange | Array<number | string | Date>;
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxChartValueAxisConstantLineStyle extends dxChartCommonAxisSettingsConstantLineStyle {
    /**
     * Specifies the appearance of the labels of those constant lines that belong to the value axis.
     */
    label?: dxChartValueAxisConstantLineStyleLabel;
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxChartValueAxisConstantLineStyleLabel extends dxChartCommonAxisSettingsConstantLineStyleLabel {
    /**
     * Aligns constant line labels in the horizontal direction.
     */
    horizontalAlignment?: 'center' | 'left' | 'right';
    /**
     * Aligns constant line labels in the vertical direction.
     */
    verticalAlignment?: 'bottom' | 'center' | 'top';
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxChartValueAxisConstantLines extends dxChartCommonAxisSettingsConstantLineStyle {
    /**
     * Specifies whether to display the constant line behind or in front of the series.
     */
    displayBehindSeries?: boolean;
    /**
     * Specifies whether to extend the axis&apos;s default visual range to display the constant line.
     */
    extendAxis?: boolean;
    /**
     * Configures the constant line label.
     */
    label?: dxChartValueAxisConstantLinesLabel;
    /**
     * Specifies the value indicated by a constant line. Setting this property is necessary.
     */
    value?: number | Date | string;
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxChartValueAxisConstantLinesLabel extends dxChartCommonAxisSettingsConstantLineStyleLabel {
    /**
     * Aligns constant line labels in the horizontal direction.
     */
    horizontalAlignment?: 'center' | 'left' | 'right';
    /**
     * Specifies the text of a constant line label. By default, equals to the value of the constant line.
     */
    text?: string;
    /**
     * Aligns constant line labels in the vertical direction.
     */
    verticalAlignment?: 'bottom' | 'center' | 'top';
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxChartValueAxisLabel extends dxChartCommonAxisSettingsLabel {
    /**
     * Specifies the hint that appears when a user points to an axis label.
     */
    customizeHint?: ((axisValue: { value?: Date | number | string; valueText?: string }) => string);
    /**
     * Customizes the text displayed by axis labels.
     */
    customizeText?: ((axisValue: { value?: Date | number | string; valueText?: string }) => string);
    /**
     * Formats a value before it is displayed in an axis label.
     */
    format?: Format;
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxChartValueAxisStrips extends dxChartCommonAxisSettingsStripStyle {
    /**
     * Specifies the color of the strip.
     */
    color?: string;
    /**
     * Along with the startValue property, limits the strip.
     */
    endValue?: number | Date | string;
    /**
     * Configures the strip label.
     */
    label?: dxChartValueAxisStripsLabel;
    /**
     * Along with the endValue property, limits the strip.
     */
    startValue?: number | Date | string;
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxChartValueAxisStripsLabel extends dxChartCommonAxisSettingsStripStyleLabel {
    /**
     * Specifies the text of the strip label.
     */
    text?: string;
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxChartValueAxisTitle extends dxChartCommonAxisSettingsTitle {
    /**
     * Specifies the text of the axis title.
     */
    text?: string;
}
/**
 * The Chart is a UI component that visualizes data from a local or remote storage using a great variety of series types along with different interactive elements, such as tooltips, crosshair pointer, legend, etc.
 */
export default class dxChart extends BaseChart<dxChartOptions> {
    /**
     * Gets the argument axis.
     */
    getArgumentAxis(): chartAxisObject;
    /**
     * Gets a value axis.
     */
    getValueAxis(): chartAxisObject;
    /**
     * Gets a value axis with the specified name.
     */
    getValueAxis(name: string): chartAxisObject;
    /**
     * Resets the visual ranges of both axes to the data range or to the whole range if it is within the data range.
     */
    resetVisualRange(): void;
    /**
     * Sets the argument axis&apos; start and end values.
     */
    zoomArgument(startValue: number | Date | string, endValue: number | Date | string): void;
}

/**
 * 
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxChartAnnotationConfig extends dxChartCommonAnnotationConfig {
    /**
     * Specifies the annotation&apos;s name.
     */
    name?: string;
}

/**
 * 
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxChartCommonAnnotationConfig extends BaseChartAnnotationConfig {
    /**
     * Specifies the name of the value axis on which the value is specified. Useful for a multi-axis chart.
     */
    axis?: string;
    /**
     * Customizes the text and appearance of the annotation&apos;s tooltip.
     */
    customizeTooltip?: ((annotation: dxChartAnnotationConfig | any) => any);
    /**
     * Specifies a custom template for the annotation. Applies only if the type is &apos;custom&apos;.
     */
    template?: template | ((annotation: dxChartAnnotationConfig | any, element: SVGGElement) => string | UserDefinedElement<SVGElement>);
    /**
     * Specifies a custom template for an annotation&apos;s tooltip.
     */
    tooltipTemplate?: template | ((annotation: dxChartAnnotationConfig | any, element: DxElement) => string | UserDefinedElement);
}

/**
 * This section lists objects that define properties used to configure series of specific types.
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxChartSeriesTypes {
    /**
     * Describes settings supported by a series of the area type.
     */
    AreaSeries?: dxChartSeriesTypesAreaSeries;
    /**
     * Describes settings supported by a series of the bar type.
     */
    BarSeries?: dxChartSeriesTypesBarSeries;
    /**
     * Describes settings supported by a series of the bubble type.
     */
    BubbleSeries?: dxChartSeriesTypesBubbleSeries;
    /**
     * Describes settings supported by a series of the candlestick type.
     */
    CandleStickSeries?: dxChartSeriesTypesCandleStickSeries;
    /**
     * An object that defines configuration properties for chart series.
     */
    CommonSeries?: dxChartSeriesTypesCommonSeries;
    /**
     * Describes settings supported by a series of the full-stacked area type.
     */
    FullStackedAreaSeries?: dxChartSeriesTypesFullStackedAreaSeries;
    /**
     * Describes settings supported by a series of the full-stacked bar type.
     */
    FullStackedBarSeries?: dxChartSeriesTypesFullStackedBarSeries;
    /**
     * Describes settings supported by a series of the full-stacked line type.
     */
    FullStackedLineSeries?: dxChartSeriesTypesFullStackedLineSeries;
    /**
     * Describes settings supported by a series of the full-stacked spline area type.
     */
    FullStackedSplineAreaSeries?: dxChartSeriesTypesFullStackedSplineAreaSeries;
    /**
     * Describes settings supported by a series of the full-stacked spline area type. An object defining a series of the fullStackedSpline type.
     */
    FullStackedSplineSeries?: dxChartSeriesTypesFullStackedSplineSeries;
    /**
     * Describes settings supported by a series of the line type.
     */
    LineSeries?: dxChartSeriesTypesLineSeries;
    /**
     * Describes settings supported by a series of the range area type.
     */
    RangeAreaSeries?: dxChartSeriesTypesRangeAreaSeries;
    /**
     * Describes settings supported by a series of the range bar type.
     */
    RangeBarSeries?: dxChartSeriesTypesRangeBarSeries;
    /**
     * Describes settings supported by a series of the scatter type.
     */
    ScatterSeries?: dxChartSeriesTypesScatterSeries;
    /**
     * Describes settings supported by a series of the spline area type.
     */
    SplineAreaSeries?: dxChartSeriesTypesSplineAreaSeries;
    /**
     * Describes settings supported by a series of the spline type.
     */
    SplineSeries?: dxChartSeriesTypesSplineSeries;
    /**
     * Describes settings supported by a series of the stacked area type.
     */
    StackedAreaSeries?: dxChartSeriesTypesStackedAreaSeries;
    /**
     * Describes settings supported by a series of the stacked bar type.
     */
    StackedBarSeries?: dxChartSeriesTypesStackedBarSeries;
    /**
     * Describes settings supported by a series of the stacked line type.
     */
    StackedLineSeries?: dxChartSeriesTypesStackedLineSeries;
    /**
     * Describes settings supported by a series of the stacked spline area type.
     */
    StackedSplineAreaSeries?: dxChartSeriesTypesStackedSplineAreaSeries;
    /**
     * Describes settings supported by a series of the stacked spline type.
     */
    StackedSplineSeries?: dxChartSeriesTypesStackedSplineSeries;
    /**
     * Describes settings supported by a series of the step rea type.
     */
    StepAreaSeries?: dxChartSeriesTypesStepAreaSeries;
    /**
     * Describes settings supported by a series of the step line type.
     */
    StepLineSeries?: dxChartSeriesTypesStepLineSeries;
    /**
     * Describes settings supported by a series of the stock type.
     */
    StockSeries?: dxChartSeriesTypesStockSeries;
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxChartSeriesTypesAreaSeries extends dxChartSeriesTypesCommonSeries {
    /**
     * Configures data aggregation for the series.
     */
    aggregation?: dxChartSeriesTypesAreaSeriesAggregation;
    /**
     * Specifies series elements to be highlighted when a user points to a series.
     */
    hoverMode?: 'nearestPoint' | 'includePoints' | 'excludePoints' | 'none';
    /**
     * Configures point labels.
     */
    label?: dxChartSeriesTypesAreaSeriesLabel;
    /**
     * Configures series points in scatter, line- and area-like series.
     */
    point?: dxChartSeriesTypesAreaSeriesPoint;
    /**
     * Specifies series elements to be highlighted when a user selects a series.
     */
    selectionMode?: 'includePoints' | 'excludePoints' | 'none';
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxChartSeriesTypesAreaSeriesAggregation extends dxChartSeriesTypesCommonSeriesAggregation {
    /**
     * Specifies how to aggregate series points.
     */
    method?: ChartSingleValueSeriesAggregationMethodType;
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxChartSeriesTypesAreaSeriesLabel extends dxChartSeriesTypesCommonSeriesLabel {
    /**
     * Customizes the text displayed by point labels.
     */
    customizeText?: ((pointInfo: any) => string);
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxChartSeriesTypesAreaSeriesPoint extends dxChartSeriesTypesCommonSeriesPoint {
    /**
     * Makes the series points visible.
     */
    visible?: boolean;
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxChartSeriesTypesBarSeries extends dxChartSeriesTypesCommonSeries {
    /**
     * Configures data aggregation for the series.
     */
    aggregation?: dxChartSeriesTypesBarSeriesAggregation;
    /**
     * Specifies series elements to be highlighted when a user points to a series.
     */
    hoverMode?: 'onlyPoint' | 'allSeriesPoints' | 'allArgumentPoints' | 'none';
    /**
     * Configures point labels.
     */
    label?: dxChartSeriesTypesBarSeriesLabel;
    /**
     * Specifies series elements to be highlighted when a user selects a bar.
     */
    selectionMode?: 'onlyPoint' | 'allSeriesPoints' | 'allArgumentPoints' | 'none';
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxChartSeriesTypesBarSeriesAggregation extends dxChartSeriesTypesCommonSeriesAggregation {
    /**
     * Specifies how to aggregate series points.
     */
    method?: ChartSingleValueSeriesAggregationMethodType;
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxChartSeriesTypesBarSeriesLabel extends dxChartSeriesTypesCommonSeriesLabel {
    /**
     * Customizes the text displayed by point labels.
     */
    customizeText?: ((pointInfo: any) => string);
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxChartSeriesTypesBubbleSeries extends dxChartSeriesTypesCommonSeries {
    /**
     * Configures data aggregation for the series.
     */
    aggregation?: dxChartSeriesTypesBubbleSeriesAggregation;
    /**
     * Specifies series elements to be highlighted when a user points to a series.
     */
    hoverMode?: 'onlyPoint' | 'allSeriesPoints' | 'allArgumentPoints' | 'none';
    /**
     * Configures point labels.
     */
    label?: dxChartSeriesTypesBubbleSeriesLabel;
    /**
     * Specifies series elements to be highlighted when a user selects a bubble.
     */
    selectionMode?: 'onlyPoint' | 'allSeriesPoints' | 'allArgumentPoints' | 'none';
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxChartSeriesTypesBubbleSeriesAggregation extends dxChartSeriesTypesCommonSeriesAggregation {
    /**
     * Specifies how to aggregate series points.
     */
    method?: 'avg' | 'custom';
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxChartSeriesTypesBubbleSeriesLabel extends dxChartSeriesTypesCommonSeriesLabel {
    /**
     * Customizes the text displayed by point labels.
     */
    customizeText?: ((pointInfo: any) => string);
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxChartSeriesTypesCandleStickSeries extends dxChartSeriesTypesCommonSeries {
    /**
     * Configures data aggregation for the series.
     */
    aggregation?: dxChartSeriesTypesCandleStickSeriesAggregation;
    /**
     * Specifies which data source field provides arguments for series points.
     */
    argumentField?: string;
    /**
     * Specifies series elements to be highlighted when a user pauses on a series.
     */
    hoverMode?: 'onlyPoint' | 'allSeriesPoints' | 'allArgumentPoints' | 'none';
    /**
     * Configures the appearance adopted by the series when a user points to it.
     */
    hoverStyle?: dxChartSeriesTypesCandleStickSeriesHoverStyle;
    /**
     * Configures point labels.
     */
    label?: dxChartSeriesTypesCandleStickSeriesLabel;
    /**
     * Specifies series elements to be highlighted when a user selects a point.
     */
    selectionMode?: 'onlyPoint' | 'allSeriesPoints' | 'allArgumentPoints' | 'none';
    /**
     * Configures the appearance adopted by the series when a user selects it.
     */
    selectionStyle?: dxChartSeriesTypesCandleStickSeriesSelectionStyle;
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxChartSeriesTypesCandleStickSeriesAggregation extends dxChartSeriesTypesCommonSeriesAggregation {
    /**
     * Specifies how to aggregate series points.
     */
    method?: 'ohlc' | 'custom';
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxChartSeriesTypesCandleStickSeriesHoverStyle extends dxChartSeriesTypesCommonSeriesHoverStyle {
    /**
     * Configures hatching that applies when a user points to the series.
     */
    hatching?: dxChartSeriesTypesCandleStickSeriesHoverStyleHatching;
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxChartSeriesTypesCandleStickSeriesHoverStyleHatching extends dxChartSeriesTypesCommonSeriesHoverStyleHatching {
    /**
     * Specifies the direction of hatching lines.
     */
    direction?: HatchingDirectionType;
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxChartSeriesTypesCandleStickSeriesLabel extends dxChartSeriesTypesCommonSeriesLabel {
    /**
     * Customizes the text displayed by point labels.
     */
    customizeText?: ((pointInfo: any) => string);
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxChartSeriesTypesCandleStickSeriesSelectionStyle extends dxChartSeriesTypesCommonSeriesSelectionStyle {
    /**
     * Configures hatching that applies when a user selects the series.
     */
    hatching?: dxChartSeriesTypesCandleStickSeriesSelectionStyleHatching;
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxChartSeriesTypesCandleStickSeriesSelectionStyleHatching extends dxChartSeriesTypesCommonSeriesSelectionStyleHatching {
    /**
     * Specifies the direction of hatching lines.
     */
    direction?: HatchingDirectionType;
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxChartSeriesTypesCommonSeries {
    /**
     * Configures data aggregation for the series.
     */
    aggregation?: dxChartSeriesTypesCommonSeriesAggregation;
    /**
     * Specifies which data source field provides arguments for series points.
     */
    argumentField?: string;
    /**
     * Binds the series to a value axis.
     */
    axis?: string;
    /**
     * Allows you to group bar series so that bars with the same argument overlap.
     */
    barOverlapGroup?: string;
    /**
     * Controls the padding and consequently the width of all bars in a series using relative units. Ignored if the barWidth property is set.
     */
    barPadding?: number;
    /**
     * Specifies a fixed width for all bars in a series, measured in pixels. Takes precedence over the barPadding property.
     */
    barWidth?: number;
    /**
     * Configures the series border (in area-like series) or the series point border (in bar-like and bubble series).
     */
    border?: dxChartSeriesTypesCommonSeriesBorder;
    /**
     * Specifies which data source field provides close values for points of a financial series.
     */
    closeValueField?: string;
    /**
     * Specifies the color of the series.
     */
    color?: string;
    /**
     * Makes bars look rounded. Applies only to bar-like series.
     */
    cornerRadius?: number;
    /**
     * Specifies the dash style of the series line. Applies only to line-like series.
     */
    dashStyle?: DashStyleType;
    /**
     * Specifies which data source field provides high values for points of a financial series.
     */
    highValueField?: string;
    /**
     * Specifies series elements to be highlighted when a user points to a series.
     */
    hoverMode?: 'allArgumentPoints' | 'allSeriesPoints' | 'excludePoints' | 'includePoints' | 'nearestPoint' | 'none' | 'onlyPoint';
    /**
     * Configures the appearance adopted by the series when a user points to it.
     */
    hoverStyle?: dxChartSeriesTypesCommonSeriesHoverStyle;
    /**
     * Specifies whether the series should ignore null data points.
     */
    ignoreEmptyPoints?: boolean;
    /**
     * Specifies a filling color for the body of a series point that visualizes a non-reduced value. Applies only to candlestick series.
     */
    innerColor?: string;
    /**
     * Configures point labels.
     */
    label?: dxChartSeriesTypesCommonSeriesLabel;
    /**
     * Specifies which data source field provides low values for points of a financial series.
     */
    lowValueField?: string;
    /**
     * Specifies a limit for the number of point labels.
     */
    maxLabelCount?: number;
    /**
     * Specifies the minimal possible height (or length if the chart is rotated) of a bar in pixels. Applies only to bar-like series.
     */
    minBarSize?: number;
    /**
     * Specifies how transparent the series should be.
     */
    opacity?: number;
    /**
     * Specifies which data source field provides open values for points of a financial series.
     */
    openValueField?: string;
    /**
     * Specifies which pane the series should belong to. Accepts the name of the pane.
     */
    pane?: string;
    /**
     * Configures series points in scatter, line- and area-like series.
     */
    point?: dxChartSeriesTypesCommonSeriesPoint;
    /**
     * Coupled with the rangeValue2Field property, specifies which data source field provides values for a range-like series.
     */
    rangeValue1Field?: string;
    /**
     * Coupled with the rangeValue1Field property, specifies which data source field provides values for a range-like series.
     */
    rangeValue2Field?: string;
    /**
     * Specifies reduction properties for financial series.
     */
    reduction?: {
      /**
       * Specifies a color for the points whose price has decreased in comparison to the price of the previous point.
       */
      color?: string;
      /**
       * Specifies whether high, low, open or close prices of points should be compared.
       */
      level?: 'close' | 'high' | 'low' | 'open';
    };
    /**
     * Specifies series elements to be highlighted when a user selects a series.
     */
    selectionMode?: 'allArgumentPoints' | 'allSeriesPoints' | 'excludePoints' | 'includePoints' | 'none' | 'onlyPoint';
    /**
     * Configures the appearance adopted by the series when a user selects it.
     */
    selectionStyle?: dxChartSeriesTypesCommonSeriesSelectionStyle;
    /**
     * Specifies whether to show the series in the legend or not.
     */
    showInLegend?: boolean;
    /**
     * Specifies which data source field provides size values for bubbles. Required by and applies only to bubble series.
     */
    sizeField?: string;
    /**
     * Specifies which stack the series should belongs to. Applies only to stacked bar and full-stacked bar series.
     */
    stack?: string;
    /**
     * Specifies which data source field provides auxiliary data for series points.
     */
    tagField?: string;
    /**
     * Configures error bars.
     */
    valueErrorBar?: {
      /**
       * Specifies the color of error bars.
       */
      color?: string;
      /**
       * Specifies whether error bars must be displayed in full or partially.
       */
      displayMode?: 'auto' | 'high' | 'low' | 'none';
      /**
       * Specifies the length of the lines that indicate error bar edges.
       */
      edgeLength?: number;
      /**
       * Specifies which data field provides high error values.
       */
      highValueField?: string;
      /**
       * Specifies the width of the error bar line.
       */
      lineWidth?: number;
      /**
       * Specifies which data field provides low error values.
       */
      lowValueField?: string;
      /**
       * Specifies how trasparent error bars should be.
       */
      opacity?: number;
      /**
       * Specifies how error bar values must be calculated.
       */
      type?: 'fixed' | 'percent' | 'stdDeviation' | 'stdError' | 'variance';
      /**
       * Specifies the value to be used for generating error bars.
       */
      value?: number;
    };
    /**
     * Specifies which data source field provides values for series points.
     */
    valueField?: string;
    /**
     * Specifies whether the series is visible or not.
     */
    visible?: boolean;
    /**
     * Specifies the width of the series line in pixels. Applies only to line-like series.
     */
    width?: number;
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxChartSeriesTypesCommonSeriesAggregation {
    /**
     * Specifies a custom aggregate function. Applies only if the aggregation method is &apos;custom&apos;.
     */
    calculate?: ((aggregationInfo: chartPointAggregationInfoObject, series: chartSeriesObject) => any | Array<any>);
    /**
     * Enables data aggregation for the series.
     */
    enabled?: boolean;
    /**
     * Specifies how to aggregate series points.
     */
    method?: 'avg' | 'count' | 'max' | 'min' | 'ohlc' | 'range' | 'sum' | 'custom';
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxChartSeriesTypesCommonSeriesBorder {
    /**
     * Colors the series border (in area-like series) or the series point border (in bar-like and bubble series).
     */
    color?: string;
    /**
     * Sets a dash style for the series border (in area-like series) or for the series point border (in bar-like and bubble series).
     */
    dashStyle?: DashStyleType;
    /**
     * Shows the series border (in area-like series) or the series point border (in bar-like and bubble series).
     */
    visible?: boolean;
    /**
     * Sets a pixel-measured width for the series border (in area-like series) or for the series point border (in bar-like and bubble series).
     */
    width?: number;
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxChartSeriesTypesCommonSeriesHoverStyle {
    /**
     * Configures the appearance adopted by the series border (in area-like series) or the series point border (in bar-like and bubble series) when a user points to the series.
     */
    border?: dxChartSeriesTypesCommonSeriesHoverStyleBorder;
    /**
     * Specifies the color of the series in the hovered state.
     */
    color?: string;
    /**
     * Specifies the dash style of the series line when the series is in the hovered state. Applies only to line-like series.
     */
    dashStyle?: DashStyleType;
    /**
     * Configures hatching that applies when a user points to the series.
     */
    hatching?: dxChartSeriesTypesCommonSeriesHoverStyleHatching;
    /**
     * Specifies the pixel-measured width of the series line when the series is in the hovered state.
     */
    width?: number;
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxChartSeriesTypesCommonSeriesHoverStyleBorder {
    /**
     * Colors the series border (in area-like series) or the series point border (in bar-like and bubble series) when a user points to the series.
     */
    color?: string;
    /**
     * Sets a dash style for the series border (in area-like series) or for the series point border (in bar-like and bubble series) when a user points to the series.
     */
    dashStyle?: DashStyleType;
    /**
     * Shows the series border (in area-like series) or the series point border (in bar-like and bubble series) when a user points to the series.
     */
    visible?: boolean;
    /**
     * Sets a pixel-measured width for the series border (in area-like series) or for the series point border (in bar-like and bubble series) when a user points to the series.
     */
    width?: number;
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxChartSeriesTypesCommonSeriesHoverStyleHatching {
    /**
     * Specifies the direction of hatching lines.
     */
    direction?: HatchingDirectionType;
    /**
     * Specifies how transparent hatching lines should be.
     */
    opacity?: number;
    /**
     * Specifies the distance between two side-by-side hatching lines in pixels.
     */
    step?: number;
    /**
     * Specifies the width of hatching lines in pixels.
     */
    width?: number;
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxChartSeriesTypesCommonSeriesLabel {
    /**
     * Aligns point labels in relation to their points.
     */
    alignment?: 'center' | 'left' | 'right';
    /**
     * Formats the point argument before it is displayed in the point label. To format the point value, use the format property.
     */
    argumentFormat?: Format;
    /**
     * Colors the point labels&apos; background. The default color is inherited from the points.
     */
    backgroundColor?: string;
    /**
     * Configures the borders of point labels.
     */
    border?: {
      /**
       * Colors the border.
       */
      color?: string;
      /**
       * Specifies the dash style of the border.
       */
      dashStyle?: DashStyleType;
      /**
       * Makes the border visible.
       */
      visible?: boolean;
      /**
       * Specifies the width of the border in pixels.
       */
      width?: number;
    };
    /**
     * Configures the label connectors.
     */
    connector?: {
      /**
       * Colors the connectors.
       */
      color?: string;
      /**
       * Makes the connectors visible. Applies only if label.visible is set to true.
       */
      visible?: boolean;
      /**
       * Specifies the width of the connectors in pixels.
       */
      width?: number;
    };
    /**
     * Customizes the text displayed by point labels.
     */
    customizeText?: ((pointInfo: any) => string);
    /**
     * Specifies font properties for point labels.
     */
    font?: Font;
    /**
     * Formats the point value before it is displayed in the point label.
     */
    format?: Format;
    /**
     * Along with verticalOffset, shifts point labels from their initial positions.
     */
    horizontalOffset?: number;
    /**
     * Specifies whether to display point labels inside or outside of series points. Applies only to bubble, range-like and bar-like series.
     */
    position?: 'inside' | 'outside';
    /**
     * Rotates point labels.
     */
    rotationAngle?: number;
    /**
     * Specifies whether or not to show labels for points with zero value. Applies only to bar-like series.
     */
    showForZeroValues?: boolean;
    /**
     * Along with horizontalOffset, shifts point labels from their initial positions.
     */
    verticalOffset?: number;
    /**
     * Makes the point labels visible.
     */
    visible?: boolean;
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxChartSeriesTypesCommonSeriesPoint {
    /**
     * Configures the appearance of the series point border in scatter, line- and area-like series.
     */
    border?: {
      /**
       * Colors the border.
       */
      color?: string;
      /**
       * Makes the border visible.
       */
      visible?: boolean;
      /**
       * Sets the width of the border in pixels.
       */
      width?: number;
    };
    /**
     * Colors the series points.
     */
    color?: string;
    /**
     * Specifies series elements to be highlighted when a user pauses on a series point.
     */
    hoverMode?: 'allArgumentPoints' | 'allSeriesPoints' | 'none' | 'onlyPoint';
    /**
     * Configures the appearance adopted by a series point when a user pauses on it.
     */
    hoverStyle?: {
      /**
       * Configures the appearance of the point border when a user pauses on the point.
       */
      border?: {
        /**
         * Specifies the color of the point border when the point is in the hovered state.
         */
        color?: string;
        /**
         * Makes the border visible when a user pauses on the series point.
         */
        visible?: boolean;
        /**
         * Specifies the width of the point border when the point is in the hovered state.
         */
        width?: number;
      };
      /**
       * Specifies the color of series points in the hovered state.
       */
      color?: string;
      /**
       * Specfies the diameter of series points in the hovered state.
       */
      size?: number;
    };
    /**
     * Substitutes the standard point symbols with an image.
     */
    image?: string | {
      /**
       * Specifies the height of the image used instead of a point marker.
       */
      height?: number | {
        /**
         * Specifies the height of the image that represents the maximum point in a range area series.
         */
        rangeMaxPoint?: number;
        /**
         * Specifies the height of the image that represents the minimum point in a range area series.
         */
        rangeMinPoint?: number;
      };
      /**
       * Specifies the URL of the image to be used as a point marker.
       */
      url?: string | {
        /**
         * Specifies the URL of the image to be used as a maximum point marker.
         */
        rangeMaxPoint?: string;
        /**
         * Specifies the URL of the image to be used as a maximum point marker.
         */
        rangeMinPoint?: string;
      };
      /**
       * Specifies the width of an image that is used as a point marker.
       */
      width?: number | {
        /**
         * Specifies the width of the image that represents the maximum point in a range area series.
         */
        rangeMaxPoint?: number;
        /**
         * Specifies the width of the image that represents the minimum point in a range area series.
         */
        rangeMinPoint?: number;
      };
    };
    /**
     * Specifies series elements to be highlighted when a user selects a series point.
     */
    selectionMode?: 'allArgumentPoints' | 'allSeriesPoints' | 'none' | 'onlyPoint';
    /**
     * Configures the appearance of a selected series point.
     */
    selectionStyle?: {
      /**
       * Configures the border of a selected point.
       */
      border?: {
        /**
         * Specifies the color of the point border when the point is selected.
         */
        color?: string;
        /**
         * Makes the border of a selected point visible.
         */
        visible?: boolean;
        /**
         * Specifies the width of the point border when the point is selected.
         */
        width?: number;
      };
      /**
       * Specifies the color of series points in the selected state.
       */
      color?: string;
      /**
       * Specfies the diameter of series points in the selected state.
       */
      size?: number;
    };
    /**
     * Specifies the diameter of series points in pixels.
     */
    size?: number;
    /**
     * Specifies which symbol should represent series points in scatter, line- and area-like series.
     */
    symbol?: 'circle' | 'cross' | 'polygon' | 'square' | 'triangleDown' | 'triangleUp';
    /**
     * Makes the series points visible.
     */
    visible?: boolean;
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxChartSeriesTypesCommonSeriesSelectionStyle {
    /**
     * Configures the appearance adopted by the series border (in area-like series) or the series point border (in bar-like and bubble series) when a user selects the series.
     */
    border?: dxChartSeriesTypesCommonSeriesSelectionStyleBorder;
    /**
     * Specifies the color of the series in the selected state.
     */
    color?: string;
    /**
     * Specifies the dash style of the series line when the series is in the selected state. Applies only to line-like series.
     */
    dashStyle?: DashStyleType;
    /**
     * Configures hatching that applies when a user selects the series.
     */
    hatching?: dxChartSeriesTypesCommonSeriesSelectionStyleHatching;
    /**
     * Specifies the pixel-measured width of the series line when the series is in the selected state.
     */
    width?: number;
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxChartSeriesTypesCommonSeriesSelectionStyleBorder {
    /**
     * Colors the series border (in area-like series) or the series point border (in bar-like and bubble series) when a user selects the series.
     */
    color?: string;
    /**
     * Sets a dash style for the series border (in area-like series) or for the series point border (in bar-like and bubble series) when a user selects the series.
     */
    dashStyle?: DashStyleType;
    /**
     * Shows the series border (in area-like series) or the series point border (in bar-like and bubble series) when a user selects the series.
     */
    visible?: boolean;
    /**
     * Sets a pixel-measured width for the series border (in area-like series) or for the series point border (in bar-like and bubble series) when a user selects the series.
     */
    width?: number;
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxChartSeriesTypesCommonSeriesSelectionStyleHatching {
    /**
     * Specifies the direction of hatching lines.
     */
    direction?: HatchingDirectionType;
    /**
     * Specifies how transparent hatching lines should be.
     */
    opacity?: number;
    /**
     * Specifies the distance between two side-by-side hatching lines in pixels.
     */
    step?: number;
    /**
     * Specifies the width of hatching lines in pixels.
     */
    width?: number;
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxChartSeriesTypesFullStackedAreaSeries extends dxChartSeriesTypesCommonSeries {
    /**
     * Configures data aggregation for the series.
     */
    aggregation?: dxChartSeriesTypesFullStackedAreaSeriesAggregation;
    /**
     * Specifies series elements to be highlighted when a user points to a series.
     */
    hoverMode?: 'nearestPoint' | 'includePoints' | 'excludePoints' | 'none';
    /**
     * An object defining the label configuration properties.
     */
    label?: dxChartSeriesTypesFullStackedAreaSeriesLabel;
    /**
     * Configures series points in scatter, line- and area-like series.
     */
    point?: dxChartSeriesTypesFullStackedAreaSeriesPoint;
    /**
     * Specifies series elements to be highlighted when a user selects a series.
     */
    selectionMode?: 'includePoints' | 'excludePoints' | 'none';
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxChartSeriesTypesFullStackedAreaSeriesAggregation extends dxChartSeriesTypesCommonSeriesAggregation {
    /**
     * Specifies how to aggregate series points.
     */
    method?: ChartSingleValueSeriesAggregationMethodType;
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxChartSeriesTypesFullStackedAreaSeriesLabel extends dxChartSeriesTypesCommonSeriesLabel {
    /**
     * Customizes the text displayed by point labels.
     */
    customizeText?: ((pointInfo: any) => string);
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxChartSeriesTypesFullStackedAreaSeriesPoint extends dxChartSeriesTypesCommonSeriesPoint {
    /**
     * Makes the series points visible.
     */
    visible?: boolean;
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxChartSeriesTypesFullStackedBarSeries extends dxChartSeriesTypesCommonSeries {
    /**
     * Configures data aggregation for the series.
     */
    aggregation?: dxChartSeriesTypesFullStackedBarSeriesAggregation;
    /**
     * Specifies series elements to be highlighted when a user points to a series.
     */
    hoverMode?: 'onlyPoint' | 'allSeriesPoints' | 'allArgumentPoints' | 'none';
    /**
     * Configures point labels.
     */
    label?: dxChartSeriesTypesFullStackedBarSeriesLabel;
    /**
     * Specifies series elements to be highlighted when a user selects a bar.
     */
    selectionMode?: 'onlyPoint' | 'allSeriesPoints' | 'allArgumentPoints' | 'none';
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxChartSeriesTypesFullStackedBarSeriesAggregation extends dxChartSeriesTypesCommonSeriesAggregation {
    /**
     * Specifies how to aggregate series points.
     */
    method?: ChartSingleValueSeriesAggregationMethodType;
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxChartSeriesTypesFullStackedBarSeriesLabel extends dxChartSeriesTypesCommonSeriesLabel {
    /**
     * Customizes the text displayed by point labels.
     */
    customizeText?: ((pointInfo: any) => string);
    /**
     * Specifies whether to display point labels inside or outside of series points. Applies only to bubble, range-like and bar-like series.
     */
    position?: 'inside' | 'outside';
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxChartSeriesTypesFullStackedLineSeries extends dxChartSeriesTypesCommonSeries {
    /**
     * Configures data aggregation for the series.
     */
    aggregation?: dxChartSeriesTypesFullStackedLineSeriesAggregation;
    /**
     * Specifies series elements to be highlighted when a user points to a series.
     */
    hoverMode?: 'nearestPoint' | 'includePoints' | 'excludePoints' | 'none';
    /**
     * Configures point labels.
     */
    label?: dxChartSeriesTypesFullStackedLineSeriesLabel;
    /**
     * Specifies series elements to be highlighted when a user selects a series.
     */
    selectionMode?: 'includePoints' | 'excludePoints' | 'none';
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxChartSeriesTypesFullStackedLineSeriesAggregation extends dxChartSeriesTypesCommonSeriesAggregation {
    /**
     * Specifies how to aggregate series points.
     */
    method?: ChartSingleValueSeriesAggregationMethodType;
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxChartSeriesTypesFullStackedLineSeriesLabel extends dxChartSeriesTypesCommonSeriesLabel {
    /**
     * Customizes the text displayed by point labels.
     */
    customizeText?: ((pointInfo: any) => string);
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxChartSeriesTypesFullStackedSplineAreaSeries extends dxChartSeriesTypesCommonSeries {
    /**
     * Configures data aggregation for the series.
     */
    aggregation?: dxChartSeriesTypesFullStackedSplineAreaSeriesAggregation;
    /**
     * Specifies series elements to be highlighted when a user points to a series.
     */
    hoverMode?: 'nearestPoint' | 'includePoints' | 'excludePoints' | 'none';
    /**
     * Configures point labels.
     */
    label?: dxChartSeriesTypesFullStackedSplineAreaSeriesLabel;
    /**
     * Configures series points in scatter, line- and area-like series.
     */
    point?: dxChartSeriesTypesFullStackedSplineAreaSeriesPoint;
    /**
     * Specifies series elements to be highlighted when a user selects a series.
     */
    selectionMode?: 'includePoints' | 'excludePoints' | 'none';
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxChartSeriesTypesFullStackedSplineAreaSeriesAggregation extends dxChartSeriesTypesCommonSeriesAggregation {
    /**
     * Specifies how to aggregate series points.
     */
    method?: ChartSingleValueSeriesAggregationMethodType;
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxChartSeriesTypesFullStackedSplineAreaSeriesLabel extends dxChartSeriesTypesCommonSeriesLabel {
    /**
     * Customizes the text displayed by point labels.
     */
    customizeText?: ((pointInfo: any) => string);
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxChartSeriesTypesFullStackedSplineAreaSeriesPoint extends dxChartSeriesTypesCommonSeriesPoint {
    /**
     * Makes the series points visible.
     */
    visible?: boolean;
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxChartSeriesTypesFullStackedSplineSeries extends dxChartSeriesTypesCommonSeries {
    /**
     * Configures data aggregation for the series.
     */
    aggregation?: dxChartSeriesTypesFullStackedSplineSeriesAggregation;
    /**
     * Specifies series elements to be highlighted when a user points to a series.
     */
    hoverMode?: 'nearestPoint' | 'includePoints' | 'excludePoints' | 'none';
    /**
     * Configures point labels.
     */
    label?: dxChartSeriesTypesFullStackedSplineSeriesLabel;
    /**
     * Specifies series elements to be highlighted when a user selects a series.
     */
    selectionMode?: 'includePoints' | 'excludePoints' | 'none';
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxChartSeriesTypesFullStackedSplineSeriesAggregation extends dxChartSeriesTypesCommonSeriesAggregation {
    /**
     * Specifies how to aggregate series points.
     */
    method?: ChartSingleValueSeriesAggregationMethodType;
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxChartSeriesTypesFullStackedSplineSeriesLabel extends dxChartSeriesTypesCommonSeriesLabel {
    /**
     * Customizes the text displayed by point labels.
     */
    customizeText?: ((pointInfo: any) => string);
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxChartSeriesTypesLineSeries extends dxChartSeriesTypesCommonSeries {
    /**
     * Configures data aggregation for the series.
     */
    aggregation?: dxChartSeriesTypesLineSeriesAggregation;
    /**
     * Specifies series elements to be highlighted when a user points to a series.
     */
    hoverMode?: 'nearestPoint' | 'includePoints' | 'excludePoints' | 'none';
    /**
     * Configures point labels.
     */
    label?: dxChartSeriesTypesLineSeriesLabel;
    /**
     * Specifies series elements to be highlighted when a user selects a series.
     */
    selectionMode?: 'includePoints' | 'excludePoints' | 'none';
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxChartSeriesTypesLineSeriesAggregation extends dxChartSeriesTypesCommonSeriesAggregation {
    /**
     * Specifies how to aggregate series points.
     */
    method?: ChartSingleValueSeriesAggregationMethodType;
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxChartSeriesTypesLineSeriesLabel extends dxChartSeriesTypesCommonSeriesLabel {
    /**
     * Customizes the text displayed by point labels.
     */
    customizeText?: ((pointInfo: any) => string);
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxChartSeriesTypesRangeAreaSeries extends dxChartSeriesTypesCommonSeries {
    /**
     * Configures data aggregation for the series.
     */
    aggregation?: dxChartSeriesTypesRangeAreaSeriesAggregation;
    /**
     * Specifies series elements to be highlighted when a user points to a series.
     */
    hoverMode?: 'nearestPoint' | 'includePoints' | 'excludePoints' | 'none';
    /**
     * Configures point labels.
     */
    label?: dxChartSeriesTypesRangeAreaSeriesLabel;
    /**
     * Configures series points in scatter, line- and area-like series.
     */
    point?: dxChartSeriesTypesRangeAreaSeriesPoint;
    /**
     * Specifies series elements to be highlighted when a user selects a series.
     */
    selectionMode?: 'includePoints' | 'excludePoints' | 'none';
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxChartSeriesTypesRangeAreaSeriesAggregation extends dxChartSeriesTypesCommonSeriesAggregation {
    /**
     * Specifies how to aggregate series points.
     */
    method?: 'range' | 'custom';
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxChartSeriesTypesRangeAreaSeriesLabel extends dxChartSeriesTypesCommonSeriesLabel {
    /**
     * Customizes the text displayed by point labels.
     */
    customizeText?: ((pointInfo: any) => string);
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxChartSeriesTypesRangeAreaSeriesPoint extends dxChartSeriesTypesCommonSeriesPoint {
    /**
     * Makes the series points visible.
     */
    visible?: boolean;
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxChartSeriesTypesRangeBarSeries extends dxChartSeriesTypesCommonSeries {
    /**
     * Configures data aggregation for the series.
     */
    aggregation?: dxChartSeriesTypesRangeBarSeriesAggregation;
    /**
     * Specifies series elements to be highlighted when a user points to a series.
     */
    hoverMode?: 'onlyPoint' | 'allSeriesPoints' | 'allArgumentPoints' | 'none';
    /**
     * Configures point labels.
     */
    label?: dxChartSeriesTypesRangeBarSeriesLabel;
    /**
     * Specifies series elements to be highlighted when a user selects a range bar.
     */
    selectionMode?: 'onlyPoint' | 'allSeriesPoints' | 'allArgumentPoints' | 'none';
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxChartSeriesTypesRangeBarSeriesAggregation extends dxChartSeriesTypesCommonSeriesAggregation {
    /**
     * Specifies how to aggregate series points.
     */
    method?: 'range' | 'custom';
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxChartSeriesTypesRangeBarSeriesLabel extends dxChartSeriesTypesCommonSeriesLabel {
    /**
     * Customizes the text displayed by point labels.
     */
    customizeText?: ((pointInfo: any) => string);
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxChartSeriesTypesScatterSeries extends dxChartSeriesTypesCommonSeries {
    /**
     * Configures data aggregation for the series.
     */
    aggregation?: dxChartSeriesTypesScatterSeriesAggregation;
    /**
     * Configures point labels.
     */
    label?: dxChartSeriesTypesScatterSeriesLabel;
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxChartSeriesTypesScatterSeriesAggregation extends dxChartSeriesTypesCommonSeriesAggregation {
    /**
     * Specifies how to aggregate series points.
     */
    method?: ChartSingleValueSeriesAggregationMethodType;
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxChartSeriesTypesScatterSeriesLabel extends dxChartSeriesTypesCommonSeriesLabel {
    /**
     * Customizes the text displayed by point labels.
     */
    customizeText?: ((pointInfo: any) => string);
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxChartSeriesTypesSplineAreaSeries extends dxChartSeriesTypesCommonSeries {
    /**
     * Configures data aggregation for the series.
     */
    aggregation?: dxChartSeriesTypesSplineAreaSeriesAggregation;
    /**
     * Specifies series elements to be highlighted when a user points to a series.
     */
    hoverMode?: 'nearestPoint' | 'includePoints' | 'excludePoints' | 'none';
    /**
     * Configures point labels.
     */
    label?: dxChartSeriesTypesSplineAreaSeriesLabel;
    /**
     * Configures series points in scatter, line- and area-like series.
     */
    point?: dxChartSeriesTypesSplineAreaSeriesPoint;
    /**
     * Specifies series elements to be highlighted when a user selects a series.
     */
    selectionMode?: 'includePoints' | 'excludePoints' | 'none';
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxChartSeriesTypesSplineAreaSeriesAggregation extends dxChartSeriesTypesCommonSeriesAggregation {
    /**
     * Specifies how to aggregate series points.
     */
    method?: ChartSingleValueSeriesAggregationMethodType;
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxChartSeriesTypesSplineAreaSeriesLabel extends dxChartSeriesTypesCommonSeriesLabel {
    /**
     * Customizes the text displayed by point labels.
     */
    customizeText?: ((pointInfo: any) => string);
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxChartSeriesTypesSplineAreaSeriesPoint extends dxChartSeriesTypesCommonSeriesPoint {
    /**
     * Makes the series points visible.
     */
    visible?: boolean;
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxChartSeriesTypesSplineSeries extends dxChartSeriesTypesCommonSeries {
    /**
     * Configures data aggregation for the series.
     */
    aggregation?: dxChartSeriesTypesSplineSeriesAggregation;
    /**
     * Specifies series elements to be highlighted when a user points to a series.
     */
    hoverMode?: 'nearestPoint' | 'includePoints' | 'excludePoints' | 'none';
    /**
     * Configures point labels.
     */
    label?: dxChartSeriesTypesSplineSeriesLabel;
    /**
     * Specifies series elements to be highlighted when a user selects a series.
     */
    selectionMode?: 'includePoints' | 'excludePoints' | 'none';
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxChartSeriesTypesSplineSeriesAggregation extends dxChartSeriesTypesCommonSeriesAggregation {
    /**
     * Specifies how to aggregate series points.
     */
    method?: ChartSingleValueSeriesAggregationMethodType;
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxChartSeriesTypesSplineSeriesLabel extends dxChartSeriesTypesCommonSeriesLabel {
    /**
     * Customizes the text displayed by point labels.
     */
    customizeText?: ((pointInfo: any) => string);
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxChartSeriesTypesStackedAreaSeries extends dxChartSeriesTypesCommonSeries {
    /**
     * Configures data aggregation for the series.
     */
    aggregation?: dxChartSeriesTypesStackedAreaSeriesAggregation;
    /**
     * Specifies series elements to be highlighted when a user points to a series.
     */
    hoverMode?: 'nearestPoint' | 'includePoints' | 'excludePoints' | 'none';
    /**
     * Configures point labels.
     */
    label?: dxChartSeriesTypesStackedAreaSeriesLabel;
    /**
     * Configures series points in scatter, line- and area-like series.
     */
    point?: dxChartSeriesTypesStackedAreaSeriesPoint;
    /**
     * Specifies series elements to be highlighted when a user selects a series.
     */
    selectionMode?: 'includePoints' | 'excludePoints' | 'none';
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxChartSeriesTypesStackedAreaSeriesAggregation extends dxChartSeriesTypesCommonSeriesAggregation {
    /**
     * Specifies how to aggregate series points.
     */
    method?: ChartSingleValueSeriesAggregationMethodType;
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxChartSeriesTypesStackedAreaSeriesLabel extends dxChartSeriesTypesCommonSeriesLabel {
    /**
     * Customizes the text displayed by point labels.
     */
    customizeText?: ((pointInfo: any) => string);
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxChartSeriesTypesStackedAreaSeriesPoint extends dxChartSeriesTypesCommonSeriesPoint {
    /**
     * Makes the series points visible.
     */
    visible?: boolean;
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxChartSeriesTypesStackedBarSeries extends dxChartSeriesTypesCommonSeries {
    /**
     * Configures data aggregation for the series.
     */
    aggregation?: dxChartSeriesTypesStackedBarSeriesAggregation;
    /**
     * Specifies series elements to be highlighted when a user points to a series.
     */
    hoverMode?: 'onlyPoint' | 'allSeriesPoints' | 'allArgumentPoints' | 'none';
    /**
     * Configures point labels.
     */
    label?: dxChartSeriesTypesStackedBarSeriesLabel;
    /**
     * Specifies series elements to be highlighted when a user selects a bar.
     */
    selectionMode?: 'onlyPoint' | 'allSeriesPoints' | 'allArgumentPoints' | 'none';
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxChartSeriesTypesStackedBarSeriesAggregation extends dxChartSeriesTypesCommonSeriesAggregation {
    /**
     * Specifies how to aggregate series points.
     */
    method?: ChartSingleValueSeriesAggregationMethodType;
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxChartSeriesTypesStackedBarSeriesLabel extends dxChartSeriesTypesCommonSeriesLabel {
    /**
     * Customizes the text displayed by point labels.
     */
    customizeText?: ((pointInfo: any) => string);
    /**
     * Specifies whether to display point labels inside or outside of series points. Applies only to bubble, range-like and bar-like series.
     */
    position?: 'inside' | 'outside';
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxChartSeriesTypesStackedLineSeries extends dxChartSeriesTypesCommonSeries {
    /**
     * Configures data aggregation for the series.
     */
    aggregation?: dxChartSeriesTypesStackedLineSeriesAggregation;
    /**
     * Specifies series elements to be highlighted when a user points to a series.
     */
    hoverMode?: 'nearestPoint' | 'includePoints' | 'excludePoints' | 'none';
    /**
     * Configures point labels.
     */
    label?: dxChartSeriesTypesStackedLineSeriesLabel;
    /**
     * Specifies series elements to be highlighted when a user selects a series.
     */
    selectionMode?: 'includePoints' | 'excludePoints' | 'none';
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxChartSeriesTypesStackedLineSeriesAggregation extends dxChartSeriesTypesCommonSeriesAggregation {
    /**
     * Specifies how to aggregate series points.
     */
    method?: ChartSingleValueSeriesAggregationMethodType;
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxChartSeriesTypesStackedLineSeriesLabel extends dxChartSeriesTypesCommonSeriesLabel {
    /**
     * Customizes the text displayed by point labels.
     */
    customizeText?: ((pointInfo: any) => string);
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxChartSeriesTypesStackedSplineAreaSeries extends dxChartSeriesTypesCommonSeries {
    /**
     * Configures data aggregation for the series.
     */
    aggregation?: dxChartSeriesTypesStackedSplineAreaSeriesAggregation;
    /**
     * Specifies series elements to be highlighted when a user points to a series.
     */
    hoverMode?: 'nearestPoint' | 'includePoints' | 'excludePoints' | 'none';
    /**
     * Configures point labels.
     */
    label?: dxChartSeriesTypesStackedSplineAreaSeriesLabel;
    /**
     * Configures series points in scatter, line- and area-like series.
     */
    point?: dxChartSeriesTypesStackedSplineAreaSeriesPoint;
    /**
     * Specifies series elements to be highlighted when a user selects a series.
     */
    selectionMode?: 'includePoints' | 'excludePoints' | 'none';
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxChartSeriesTypesStackedSplineAreaSeriesAggregation extends dxChartSeriesTypesCommonSeriesAggregation {
    /**
     * Specifies how to aggregate series points.
     */
    method?: ChartSingleValueSeriesAggregationMethodType;
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxChartSeriesTypesStackedSplineAreaSeriesLabel extends dxChartSeriesTypesCommonSeriesLabel {
    /**
     * Customizes the text displayed by point labels.
     */
    customizeText?: ((pointInfo: any) => string);
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxChartSeriesTypesStackedSplineAreaSeriesPoint extends dxChartSeriesTypesCommonSeriesPoint {
    /**
     * Makes the series points visible.
     */
    visible?: boolean;
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxChartSeriesTypesStackedSplineSeries extends dxChartSeriesTypesCommonSeries {
    /**
     * Configures data aggregation for the series.
     */
    aggregation?: dxChartSeriesTypesStackedSplineSeriesAggregation;
    /**
     * Specifies series elements to be highlighted when a user points to a series.
     */
    hoverMode?: 'nearestPoint' | 'includePoints' | 'excludePoints' | 'none';
    /**
     * Configures point labels.
     */
    label?: dxChartSeriesTypesStackedSplineSeriesLabel;
    /**
     * Specifies series elements to be highlighted when a user selects a series.
     */
    selectionMode?: 'includePoints' | 'excludePoints' | 'none';
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxChartSeriesTypesStackedSplineSeriesAggregation extends dxChartSeriesTypesCommonSeriesAggregation {
    /**
     * Specifies how to aggregate series points.
     */
    method?: ChartSingleValueSeriesAggregationMethodType;
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxChartSeriesTypesStackedSplineSeriesLabel extends dxChartSeriesTypesCommonSeriesLabel {
    /**
     * Customizes the text displayed by point labels.
     */
    customizeText?: ((pointInfo: any) => string);
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxChartSeriesTypesStepAreaSeries extends dxChartSeriesTypesCommonSeries {
    /**
     * Configures data aggregation for the series.
     */
    aggregation?: dxChartSeriesTypesStepAreaSeriesAggregation;
    /**
     * Configures the series border (in area-like series) or the series point border (in bar-like and bubble series).
     */
    border?: dxChartSeriesTypesStepAreaSeriesBorder;
    /**
     * Specifies series elements to be highlighted when a user points to a series.
     */
    hoverMode?: 'nearestPoint' | 'includePoints' | 'excludePoints' | 'none';
    /**
     * Configures the appearance adopted by the series when a user points to it.
     */
    hoverStyle?: dxChartSeriesTypesStepAreaSeriesHoverStyle;
    /**
     * Configures point labels.
     */
    label?: dxChartSeriesTypesStepAreaSeriesLabel;
    /**
     * Configures series points in scatter, line- and area-like series.
     */
    point?: dxChartSeriesTypesStepAreaSeriesPoint;
    /**
     * Specifies series elements to be highlighted when a user selects a series.
     */
    selectionMode?: 'includePoints' | 'excludePoints' | 'none';
    /**
     * Configures the appearance adopted by the series when a user selects it.
     */
    selectionStyle?: dxChartSeriesTypesStepAreaSeriesSelectionStyle;
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxChartSeriesTypesStepAreaSeriesAggregation extends dxChartSeriesTypesCommonSeriesAggregation {
    /**
     * Specifies how to aggregate series points.
     */
    method?: ChartSingleValueSeriesAggregationMethodType;
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxChartSeriesTypesStepAreaSeriesBorder extends dxChartSeriesTypesCommonSeriesBorder {
    /**
     * Shows the series border (in area-like series) or the series point border (in bar-like and bubble series).
     */
    visible?: boolean;
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxChartSeriesTypesStepAreaSeriesHoverStyle extends dxChartSeriesTypesCommonSeriesHoverStyle {
    /**
     * Configures the appearance adopted by the series border (in area-like series) or the series point border (in bar-like and bubble series) when a user points to the series.
     */
    border?: dxChartSeriesTypesStepAreaSeriesHoverStyleBorder;
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxChartSeriesTypesStepAreaSeriesHoverStyleBorder extends dxChartSeriesTypesCommonSeriesHoverStyleBorder {
    /**
     * Shows the series border (in area-like series) or the series point border (in bar-like and bubble series) when a user points to the series.
     */
    visible?: boolean;
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxChartSeriesTypesStepAreaSeriesLabel extends dxChartSeriesTypesCommonSeriesLabel {
    /**
     * Customizes the text displayed by point labels.
     */
    customizeText?: ((pointInfo: any) => string);
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxChartSeriesTypesStepAreaSeriesPoint extends dxChartSeriesTypesCommonSeriesPoint {
    /**
     * Makes the series points visible.
     */
    visible?: boolean;
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxChartSeriesTypesStepAreaSeriesSelectionStyle extends dxChartSeriesTypesCommonSeriesSelectionStyle {
    /**
     * Configures the appearance adopted by the series border (in area-like series) or the series point border (in bar-like and bubble series) when a user selects the series.
     */
    border?: dxChartSeriesTypesStepAreaSeriesSelectionStyleBorder;
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxChartSeriesTypesStepAreaSeriesSelectionStyleBorder extends dxChartSeriesTypesCommonSeriesSelectionStyleBorder {
    /**
     * Shows the series border (in area-like series) or the series point border (in bar-like and bubble series) when a user selects the series.
     */
    visible?: boolean;
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxChartSeriesTypesStepLineSeries extends dxChartSeriesTypesCommonSeries {
    /**
     * Configures data aggregation for the series.
     */
    aggregation?: dxChartSeriesTypesStepLineSeriesAggregation;
    /**
     * Specifies series elements to be highlighted when a user points to a series.
     */
    hoverMode?: 'nearestPoint' | 'includePoints' | 'excludePoints' | 'none';
    /**
     * Configures point labels.
     */
    label?: dxChartSeriesTypesStepLineSeriesLabel;
    /**
     * Specifies series elements to be highlighted when a user selects a series.
     */
    selectionMode?: 'includePoints' | 'excludePoints' | 'none';
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxChartSeriesTypesStepLineSeriesAggregation extends dxChartSeriesTypesCommonSeriesAggregation {
    /**
     * Specifies how to aggregate series points.
     */
    method?: ChartSingleValueSeriesAggregationMethodType;
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxChartSeriesTypesStepLineSeriesLabel extends dxChartSeriesTypesCommonSeriesLabel {
    /**
     * Customizes the text displayed by point labels.
     */
    customizeText?: ((pointInfo: any) => string);
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxChartSeriesTypesStockSeries extends dxChartSeriesTypesCommonSeries {
    /**
     * Configures data aggregation for the series.
     */
    aggregation?: dxChartSeriesTypesStockSeriesAggregation;
    /**
     * Specifies which data source field provides arguments for series points.
     */
    argumentField?: string;
    /**
     * Specifies series elements to be highlighted when a user pauses on a series.
     */
    hoverMode?: 'onlyPoint' | 'allSeriesPoints' | 'allArgumentPoints' | 'none';
    /**
     * Configures point labels.
     */
    label?: dxChartSeriesTypesStockSeriesLabel;
    /**
     * Specifies series elements to be highlighted when a user selects a point.
     */
    selectionMode?: 'onlyPoint' | 'allSeriesPoints' | 'allArgumentPoints' | 'none';
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxChartSeriesTypesStockSeriesAggregation extends dxChartSeriesTypesCommonSeriesAggregation {
    /**
     * Specifies how to aggregate series points.
     */
    method?: 'ohlc' | 'custom';
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxChartSeriesTypesStockSeriesLabel extends dxChartSeriesTypesCommonSeriesLabel {
    /**
     * Customizes the text displayed by point labels.
     */
    customizeText?: ((pointInfo: any) => string);
}

export type Properties = dxChartOptions;

/**
 * @deprecated use Properties instead
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export type Options = dxChartOptions;
