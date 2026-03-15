/**
* DevExtreme (viz/gauges/base_gauge.d.ts)
* Version: 21.2.6
* Build date: Tue Mar 01 2022
*
* Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
* Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
*/
import {
    UserDefinedElement,
    DxElement,
} from '../../core/element';

import {
    PaletteType,
    PaletteExtensionModeType,
} from '../palette';

import {
    template,
} from '../../core/templates/template';

import {
    EventInfo,
} from '../../events/index';

import {
    Format,
} from '../../localization';

import BaseWidget, {
    BaseWidgetLoadingIndicator,
    BaseWidgetOptions,
    BaseWidgetTooltip,
    Font,
} from '../core/base_widget';

/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface TooltipInfo {
    target: any;
}

/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface BaseGaugeOptions<TComponent> extends BaseWidgetOptions<TComponent> {
    /**
     * Specifies animation properties.
     */
    animation?: BaseGaugeAnimation;
    /**
     * Specifies the color of the parent page element.
     */
    containerBackgroundColor?: string;
    /**
     * Configures the loading indicator.
     */
    loadingIndicator?: BaseGaugeLoadingIndicator;
    /**
     * A function that is executed when a tooltip becomes hidden.
     */
    onTooltipHidden?: ((e: EventInfo<TComponent> & TooltipInfo) => void);
    /**
     * A function that is executed when a tooltip appears.
     */
    onTooltipShown?: ((e: EventInfo<TComponent> & TooltipInfo) => void);
    /**
     * Specifies properties of the gauge&apos;s range container.
     */
    rangeContainer?: BaseGaugeRangeContainer;
    /**
     * Specifies properties of the gauge&apos;s scale.
     */
    scale?: BaseGaugeScale;
    /**
     * Specifies a set of subvalues to be designated by the subvalue indicators.
     */
    subvalues?: Array<number>;
    /**
     * Configures tooltips.
     */
    tooltip?: BaseGaugeTooltip;
    /**
     * Specifies the main value on a gauge.
     */
    value?: number;
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface BaseGaugeAnimation {
    /**
     * Determines how long animation runs.
     */
    duration?: number;
    /**
     * Specifies the animation easing mode.
     */
    easing?: 'easeOutCubic' | 'linear';
    /**
     * Indicates whether or not animation is enabled.
     */
    enabled?: boolean;
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface BaseGaugeLoadingIndicator extends BaseWidgetLoadingIndicator {
    /**
     * Specifies whether the loading indicator should be displayed and hidden automatically.
     */
    enabled?: boolean;
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface BaseGaugeRangeContainer {
    /**
     * Specifies a range container&apos;s background color.
     */
    backgroundColor?: string;
    /**
     * Specifies the offset of the range container from an invisible scale line in pixels.
     */
    offset?: number;
    /**
     * Specifies the palette to be used for colorizing ranges in the range container.
     */
    palette?: Array<string> | PaletteType;
    /**
     * Specifies what to do with colors in the palette when their number is less than the number of ranges in the range container.
     */
    paletteExtensionMode?: PaletteExtensionModeType;
    /**
     * An array of objects representing ranges contained in the range container.
     */
    ranges?: Array<{
      /**
       * Specifies a color of a range.
       */
      color?: string;
      /**
       * Specifies an end value of a range.
       */
      endValue?: number;
      /**
       * Specifies a start value of a range.
       */
      startValue?: number;
    }>;
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface BaseGaugeScale {
    /**
     * Specifies whether to allow decimal values on the scale. When false, the scale contains integer values only.
     */
    allowDecimals?: boolean;
    /**
     * Specifies an array of custom minor ticks.
     */
    customMinorTicks?: Array<number>;
    /**
     * Specifies an array of custom major ticks.
     */
    customTicks?: Array<number>;
    /**
     * Specifies the end value for the scale of the gauge.
     */
    endValue?: number;
    /**
     * Specifies common properties for scale labels.
     */
    label?: BaseGaugeScaleLabel;
    /**
     * Specifies properties of the gauge&apos;s minor ticks.
     */
    minorTick?: {
      /**
       * Specifies the color of the scale&apos;s minor ticks.
       */
      color?: string;
      /**
       * Specifies the length of the scale&apos;s minor ticks.
       */
      length?: number;
      /**
       * Specifies the opacity of the scale&apos;s minor ticks.
       */
      opacity?: number;
      /**
       * Indicates whether scale minor ticks are visible or not.
       */
      visible?: boolean;
      /**
       * Specifies the width of the scale&apos;s minor ticks.
       */
      width?: number;
    };
    /**
     * Specifies an interval between minor ticks.
     */
    minorTickInterval?: number;
    /**
     * Specifies the minimum distance between two neighboring major ticks in pixels.
     */
    scaleDivisionFactor?: number;
    /**
     * Specifies the start value for the scale of the gauge.
     */
    startValue?: number;
    /**
     * Specifies properties of the gauge&apos;s major ticks.
     */
    tick?: {
      /**
       * Specifies the color of the scale&apos;s major ticks.
       */
      color?: string;
      /**
       * Specifies the length of the scale&apos;s major ticks.
       */
      length?: number;
      /**
       * Specifies the opacity of the scale&apos;s major ticks.
       */
      opacity?: number;
      /**
       * Indicates whether scale major ticks are visible or not.
       */
      visible?: boolean;
      /**
       * Specifies the width of the scale&apos;s major ticks.
       */
      width?: number;
    };
    /**
     * Specifies an interval between major ticks.
     */
    tickInterval?: number;
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface BaseGaugeScaleLabel {
    /**
     * Specifies a callback function that returns the text to be displayed in scale labels.
     */
    customizeText?: ((scaleValue: { value?: number; valueText?: string }) => string);
    /**
     * Specifies font properties for the text displayed in the scale labels of the gauge.
     */
    font?: Font;
    /**
     * Formats a value before it is displayed in a scale label. Accepts only numeric formats.
     */
    format?: Format;
    /**
     * Decides how to arrange scale labels when there is not enough space to keep all of them.
     */
    overlappingBehavior?: 'hide' | 'none';
    /**
     * Specifies whether or not scale labels should be colored similarly to their corresponding ranges in the range container.
     */
    useRangeColors?: boolean;
    /**
     * Specifies whether or not scale labels are visible on the gauge.
     */
    visible?: boolean;
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface BaseGaugeTooltip extends BaseWidgetTooltip {
    /**
     * Specifies a custom template for a tooltip.
     */
    contentTemplate?: template | ((scaleValue: { value?: number; valueText?: string }, element: DxElement) => string | UserDefinedElement);
    /**
     * Allows you to change the appearance of specified tooltips.
     */
    customizeTooltip?: ((scaleValue: { value?: number; valueText?: string }) => any);
    /**
     * Allows users to interact with the tooltip content.
     */
    interactive?: boolean;
}
/**
 * A gauge UI component.
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export class BaseGauge<TProperties> extends BaseWidget<TProperties> {
    /**
     * Gets subvalues.
     */
    subvalues(): Array<number>;
    /**
     * Updates subvalues.
     */
    subvalues(subvalues: Array<number>): void;
    /**
     * Gets the main value.
     */
    value(): number;
    /**
     * Updates the main value.
     */
    value(value: number): void;
}

/**
 * A base object for gauge value and subvalue indicators. Includes the properties of indicators of all types.
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface CommonIndicator {
    /**
     * Specifies the length of an arrow for the indicator of the textCloud type in pixels.
     */
    arrowLength?: number;
    /**
     * Specifies the background color for the indicator of the rangeBar type.
     */
    backgroundColor?: string;
    /**
     * Specifies the base value for the indicator of the rangeBar type.
     */
    baseValue?: number;
    /**
     * Specifies a radius small enough for the indicator to begin adapting.
     */
    beginAdaptingAtRadius?: number;
    /**
     * Specifies a color of the indicator.
     */
    color?: string;
    /**
     * Specifies the orientation of the rangeBar indicator. Applies only if the geometry.orientation property is &apos;vertical&apos;.
     */
    horizontalOrientation?: 'left' | 'right';
    /**
     * Specifies the distance between the needle and the center of a gauge for the indicator of a needle-like type.
     */
    indentFromCenter?: number;
    /**
     * Specifies the indicator length.
     */
    length?: number;
    /**
     * Specifies the distance between the indicator and the invisible scale line.
     */
    offset?: number;
    /**
     * Sets the palette to be used to colorize indicators differently.
     */
    palette?: Array<string> | PaletteType;
    /**
     * Specifies the second color for the indicator of the twoColorNeedle type.
     */
    secondColor?: string;
    /**
     * Specifies the length of a twoNeedleColor type indicator tip as a percentage.
     */
    secondFraction?: number;
    /**
     * Specifies the range bar size for an indicator of the rangeBar type.
     */
    size?: number;
    /**
     * Specifies the inner diameter in pixels, so that the spindle has the shape of a ring.
     */
    spindleGapSize?: number;
    /**
     * Specifies the spindle&apos;s diameter in pixels for the indicator of a needle-like type.
     */
    spindleSize?: number;
    /**
     * Specifies the appearance of the text displayed in an indicator of the rangeBar type.
     */
    text?: {
      /**
       * Specifies a callback function that returns the text to be displayed in an indicator.
       */
      customizeText?: ((indicatedValue: { value?: number; valueText?: string }) => string);
      /**
       * Specifies font properties for the text displayed by the indicator.
       */
      font?: Font;
      /**
       * Formats a value before it is displayed in an indicator. Accepts only numeric formats.
       */
      format?: Format;
      /**
       * Specifies the range bar&apos;s label indent in pixels.
       */
      indent?: number;
    };
    /**
     * Specifies the orientation of the rangeBar indicator. Applies only if the geometry.orientation property is &apos;horizontal&apos;.
     */
    verticalOrientation?: 'bottom' | 'top';
    /**
     * Specifies the width of an indicator in pixels.
     */
    width?: number;
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export type GaugeIndicatorType = 'circle' | 'rangeBar' | 'rectangle' | 'rectangleNeedle' | 'rhombus' | 'textCloud' | 'triangleMarker' | 'triangleNeedle' | 'twoColorNeedle';

/**
 * A base object for gauge value and subvalue indicators. Includes the properties of indicators of all types.
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface GaugeIndicator extends CommonIndicator {
    /**
     * Specifies the type of gauge indicators.
     */
    type?: GaugeIndicatorType;
}
