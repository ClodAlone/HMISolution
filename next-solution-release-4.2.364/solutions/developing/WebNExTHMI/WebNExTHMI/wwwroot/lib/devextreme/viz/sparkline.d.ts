/**
* DevExtreme (viz/sparkline.d.ts)
* Version: 21.2.6
* Build date: Tue Mar 01 2022
*
* Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
* Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
*/
import DataSource, { DataSourceLike } from '../data/data_source';

import {
    EventInfo,
    InitializedEventInfo,
    ChangedOptionInfo,
} from '../events/index';

import {
    FileSavingEventInfo,
    ExportInfo,
    IncidentInfo,
} from './core/base_widget';

import BaseSparkline, {
    BaseSparklineOptions,
} from './sparklines/base_sparkline';

export type DisposingEvent = EventInfo<dxSparkline>;

export type DrawnEvent = EventInfo<dxSparkline>;

export type ExportedEvent = EventInfo<dxSparkline>;

export type ExportingEvent = EventInfo<dxSparkline> & ExportInfo;

export type FileSavingEvent = FileSavingEventInfo<dxSparkline>;

export type IncidentOccurredEvent = EventInfo<dxSparkline> & IncidentInfo;

export type InitializedEvent = InitializedEventInfo<dxSparkline>;

export type OptionChangedEvent = EventInfo<dxSparkline> & ChangedOptionInfo;

export type TooltipHiddenEvent = EventInfo<dxSparkline>;

export type TooltipShownEvent = EventInfo<dxSparkline>;

/**
 * @deprecated use Properties instead
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxSparklineOptions extends BaseSparklineOptions<dxSparkline> {
    /**
     * Specifies the data source field that provides arguments for a sparkline.
     */
    argumentField?: string;
    /**
     * Sets a color for the bars indicating negative values. Available for a sparkline of the bar type only.
     */
    barNegativeColor?: string;
    /**
     * Sets a color for the bars indicating positive values. Available for a sparkline of the bar type only.
     */
    barPositiveColor?: string;
    /**
     * Binds the UI component to data.
     */
    dataSource?: DataSourceLike<any>;
    /**
     * Sets a color for the boundary of both the first and last points on a sparkline.
     */
    firstLastColor?: string;
    /**
     * Specifies whether the sparkline should ignore null data points.
     */
    ignoreEmptyPoints?: boolean;
    /**
     * Sets a color for a line on a sparkline. Available for the sparklines of the line- and area-like types.
     */
    lineColor?: string;
    /**
     * Specifies a width for a line on a sparkline. Available for the sparklines of the line- and area-like types.
     */
    lineWidth?: number;
    /**
     * Sets a color for the bars indicating the values that are less than the winloss threshold. Available for a sparkline of the winloss type only.
     */
    lossColor?: string;
    /**
     * Sets a color for the boundary of the maximum point on a sparkline.
     */
    maxColor?: string;
    /**
     * Specifies the maximum value of the sparkline&apos;s value axis.
     */
    maxValue?: number;
    /**
     * Sets a color for the boundary of the minimum point on a sparkline.
     */
    minColor?: string;
    /**
     * Specifies the minimum value of the sparkline value axis.
     */
    minValue?: number;
    /**
     * Sets a color for points on a sparkline. Available for the sparklines of the line- and area-like types.
     */
    pointColor?: string;
    /**
     * Specifies the diameter of sparkline points in pixels. Available for the sparklines of line- and area-like types.
     */
    pointSize?: number;
    /**
     * Specifies a symbol to use as a point marker on a sparkline. Available for the sparklines of the line- and area-like types.
     */
    pointSymbol?: 'circle' | 'cross' | 'polygon' | 'square' | 'triangle';
    /**
     * Specifies whether or not to indicate both the first and last values on a sparkline.
     */
    showFirstLast?: boolean;
    /**
     * Specifies whether or not to indicate both the minimum and maximum values on a sparkline.
     */
    showMinMax?: boolean;
    /**
     * Determines the type of a sparkline.
     */
    type?: 'area' | 'bar' | 'line' | 'spline' | 'splinearea' | 'steparea' | 'stepline' | 'winloss';
    /**
     * Specifies the data source field that provides values for a sparkline.
     */
    valueField?: string;
    /**
     * Sets a color for the bars indicating the values greater than a winloss threshold. Available for a sparkline of the winloss type only.
     */
    winColor?: string;
    /**
     * Specifies a value that serves as a threshold for the sparkline of the winloss type.
     */
    winlossThreshold?: number;
}
/**
 * The Sparkline UI component is a compact chart that contains only one series. Owing to their size, sparklines occupy very little space and can be easily collected in a table or embedded straight in text.
 */
export default class dxSparkline extends BaseSparkline<dxSparklineOptions> {
    getDataSource(): DataSource;
}

export type Properties = dxSparklineOptions;

/**
 * @deprecated use Properties instead
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export type Options = dxSparklineOptions;
