/**
* DevExtreme (viz/common.d.ts)
* Version: 21.2.6
* Build date: Tue Mar 01 2022
*
* Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
* Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
*/
import {
    dxChartSeriesTypesCommonSeries,
} from './chart';

import {
    Font,
} from './core/base_widget';

/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export type ChartSeriesType = 'area' | 'bar' | 'bubble' | 'candlestick' | 'fullstackedarea' | 'fullstackedbar' | 'fullstackedline' | 'fullstackedspline' | 'fullstackedsplinearea' | 'line' | 'rangearea' | 'rangebar' | 'scatter' | 'spline' | 'splinearea' | 'stackedarea' | 'stackedbar' | 'stackedline' | 'stackedspline' | 'stackedsplinearea' | 'steparea' | 'stepline' | 'stock';
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export type DashStyleType = 'dash' | 'dot' | 'longDash' | 'solid';
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export type TimeIntervalType = 'day' | 'hour' | 'millisecond' | 'minute' | 'month' | 'quarter' | 'second' | 'week' | 'year';
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export type HatchingDirectionType = 'left' | 'none' | 'right';
/**
 * A class describing various time intervals. Inherited by tick intervals in Chart and RangeSelector.
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export type VizTimeInterval = number | {
  /**
   * Specifies the time interval measured in days. Accepts integer values. Available only for an axis/scale that displays date-time values.
   */
  days?: number;
  /**
   * Specifies the time interval measured in hours. Accepts integer values. Available only for an axis/scale that displays date-time values.
   */
  hours?: number;
  /**
   * Specifies the time interval measured in milliseconds. Accepts integer values. Available only for an axis/scale that displays date-time values.
   */
  milliseconds?: number;
  /**
   * Specifies the time interval measured in minutes. Accepts integer values. Available only for an axis/scale that displays date-time values.
   */
  minutes?: number;
  /**
   * Specifies the time interval measured in months. Accepts integer values. Available only for an axis/scale that displays date-time values.
   */
  months?: number;
  /**
   * Specifies the time interval measured in quarters. Accepts integer values. Available only for an axis/scale that displays date-time values.
   */
  quarters?: number;
  /**
   * Specifies the time interval measured in seconds. Accepts integer values. Available only for an axis/scale that displays date-time values.
   */
  seconds?: number;
  /**
   * Specifies the time interval measured in weeks. Accepts integer values. Available only for an axis/scale that displays date-time values.
   */
  weeks?: number;
  /**
   * Specifies the time interval measured in years. Accepts integer values. Available only for an axis/scale that displays date-time values.
   */
  years?: number;
} | TimeIntervalType;

/**
 * 
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface BaseLegend {
    /**
     * Colors the legend&apos;s background.
     */
    backgroundColor?: string;
    /**
     * Configures the legend&apos;s border.
     */
    border?: {
      /**
       * Colors the legend&apos;s border.
       */
      color?: string;
      /**
       * Makes all the legend&apos;s corners rounded.
       */
      cornerRadius?: number;
      /**
       * Sets a dash style for the legend&apos;s border.
       */
      dashStyle?: DashStyleType;
      /**
       * Specifies the transparency of the legend&apos;s border.
       */
      opacity?: number;
      /**
       * Shows the legend&apos;s border.
       */
      visible?: boolean;
      /**
       * Specifies the width of the legend&apos;s border in pixels.
       */
      width?: number;
    };
    /**
     * Arranges legend items into several columns.
     */
    columnCount?: number;
    /**
     * Specifies an empty space between item columns in pixels.
     */
    columnItemSpacing?: number;
    /**
     * Specifies the legend items&apos; font properties.
     */
    font?: Font;
    /**
     * Along with verticalAlignment, specifies the legend&apos;s position.
     */
    horizontalAlignment?: 'center' | 'left' | 'right';
    /**
     * Specifies the text&apos;s position relative to the marker in a legend item.
     */
    itemTextPosition?: 'bottom' | 'left' | 'right' | 'top';
    /**
     * Aligns items in the last column or row (depending on the legend&apos;s orientation). Applies when legend items are not divided into columns or rows equally.
     */
    itemsAlignment?: 'center' | 'left' | 'right';
    /**
     * Generates an empty space, measured in pixels, around the legend.
     */
    margin?: number | {
      /**
       * Specifies the legend&apos;s bottom margin in pixels.
       */
      bottom?: number;
      /**
       * Specifies the legend&apos;s left margin in pixels.
       */
      left?: number;
      /**
       * Specifies the legend&apos;s right margin in pixels.
       */
      right?: number;
      /**
       * Specifies the legend&apos;s top margin in pixels.
       */
      top?: number;
    };
    /**
     * Specifies the marker&apos;s size in a legend item in pixels.
     */
    markerSize?: number;
    /**
     * Arranges legend items vertically (in a column) or horizontally (in a row). The default value is &apos;horizontal&apos; if the legend.horizontalAlignment is &apos;center&apos;. Otherwise, it is &apos;vertical&apos;.
     */
    orientation?: 'horizontal' | 'vertical';
    /**
     * Generates an empty space, measured in pixels, between the legend&apos;s left/right border and its items.
     */
    paddingLeftRight?: number;
    /**
     * Generates an empty space, measured in pixels, between the legend&apos;s top/bottom border and its items.
     */
    paddingTopBottom?: number;
    /**
     * Arranges legend items in several rows.
     */
    rowCount?: number;
    /**
     * Specifies an empty space between item rows in pixels.
     */
    rowItemSpacing?: number;
    /**
     * Configures the legend title.
     */
    title?: {
      /**
       * Specifies the legend title&apos;s font properties.
       */
      font?: Font;
      /**
       * Along with verticalAlignment, specifies the legend title&apos;s position.
       */
      horizontalAlignment?: 'center' | 'left' | 'right';
      /**
       * Generates space around the legend title.
       */
      margin?: {
        /**
         * Specifies the legend title&apos;s bottom margin.
         */
        bottom?: number;
        /**
         * Specifies the legend title&apos;s left margin.
         */
        left?: number;
        /**
         * Specifies the legend title&apos;s right margin.
         */
        right?: number;
        /**
         * Specifies the legend title&apos;s top margin.
         */
        top?: number;
      };
      /**
       * Reserves a pixel-measured space for the legend title.
       */
      placeholderSize?: number;
      /**
       * Configures the legend subtitle. The subtitle appears only if the title is specified.
       */
      subtitle?: {
        /**
         * Specifies the legend subtitle&apos;s font properties.
         */
        font?: Font;
        /**
         * Specifies the distance between the legend&apos;s title and subtitle in pixels.
         */
        offset?: number;
        /**
         * Specifies the subtitle&apos;s text.
         */
        text?: string;
      } | string;
      /**
       * Specifies the legend title&apos;s text.
       */
      text?: string;
      /**
       * Specifies the legend title&apos;s vertical alignment.
       */
      verticalAlignment?: 'bottom' | 'top';
    } | string;
    /**
     * Along with horizontalAlignment, specifies the legend&apos;s position.
     */
    verticalAlignment?: 'bottom' | 'top';
    /**
     * Specifies the legend&apos;s visibility.
     */
    visible?: boolean;
}

/**
 * 
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface BaseLegendItem {
    /**
     * A legend item marker.
     */
    marker?: {
      /**
       * The marker&apos;s color.
       */
      fill?: string;
      /**
       * The marker&apos;s opacity.
       */
      opacity?: number;
      /**
       * The markerSize in pixels.
       */
      size?: number;
      /**
       * The marker&apos;s state.
       */
      state?: 'normal' | 'hovered' | 'selected';
    };
    /**
     * The text that the legend item displays.
     */
    text?: string;
    /**
     * Indicates and specifies whether the legend item is visible.
     */
    visible?: boolean;
}

/**
 * Specifies properties for Chart UI component series.
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface ChartSeries extends dxChartSeriesTypesCommonSeries {
    /**
     * Specifies the name that identifies the series.
     */
    name?: string;
    /**
     * Specifies data about a series.
     */
    tag?: any;
    /**
     * Sets the series type.
     */
    type?: ChartSeriesType;
}

/**
 * A class describing a scale break range. Inherited by scale breaks in the Chart and RangeSelector.
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface ScaleBreak {
    /**
     * Along with the startValue property, limits the scale break.
     */
    endValue?: number | Date | string;
    /**
     * Along with the endValue property, limits the scale break.
     */
    startValue?: number | Date | string;
}

/**
 * 
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface VizRange {
    /**
     * The range&apos;s end value.
     */
    endValue?: number | Date | string;
    /**
     * The range&apos;s length.
     */
    length?: VizTimeInterval;
    /**
     * The range&apos;s start value.
     */
    startValue?: number | Date | string;
}
