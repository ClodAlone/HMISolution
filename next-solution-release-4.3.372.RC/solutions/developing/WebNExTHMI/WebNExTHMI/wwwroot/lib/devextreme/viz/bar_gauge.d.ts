/**
* DevExtreme (viz/bar_gauge.d.ts)
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
    PaletteType,
    PaletteExtensionModeType,
} from './palette';

import {
    template,
} from '../core/templates/template';

import {
    EventInfo,
    InitializedEventInfo,
    ChangedOptionInfo,
} from '../events/index';

import {
    Format,
} from '../localization';

import {
    BaseLegend,
    BaseLegendItem,
} from './common';

import BaseWidget, {
    BaseWidgetLoadingIndicator,
    BaseWidgetOptions,
    BaseWidgetTooltip,
    Font,
    FileSavingEventInfo,
    ExportInfo,
    IncidentInfo,
} from './core/base_widget';

/**
 * An object that provides information about a bar in the BarGauge UI component.
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface BarGaugeBarInfo {
    /**
     * The bar&apos;s hexadecimal color code.
     */
    color?: string;
    /**
     * The bar&apos;s zero-based index. Bars closest to the gauge&apos;s center have higher indexes.
     */
    index?: number;
    /**
     * The bar&apos;s value.
     */
    value?: number;
}

export type LegendItem = BarGaugeLegendItem;

/**
 * @deprecated Use LegendItem instead
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface BarGaugeLegendItem extends BaseLegendItem {
    /**
     * The bar that the legend item represents.
     */
    item?: BarGaugeBarInfo;
}

/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface TooltipInfo {
    target?: any;
}

export type DisposingEvent = EventInfo<dxBarGauge>;

export type DrawnEvent = EventInfo<dxBarGauge>;

export type ExportedEvent = EventInfo<dxBarGauge>;

export type ExportingEvent = EventInfo<dxBarGauge> & ExportInfo;

export type FileSavingEvent = FileSavingEventInfo<dxBarGauge>;

export type IncidentOccurredEvent = EventInfo<dxBarGauge> & IncidentInfo;

export type InitializedEvent = InitializedEventInfo<dxBarGauge>;

export type OptionChangedEvent = EventInfo<dxBarGauge> & ChangedOptionInfo;

export type TooltipHiddenEvent = EventInfo<dxBarGauge> & TooltipInfo;

export type TooltipShownEvent = EventInfo<dxBarGauge> & TooltipInfo;

/**
 * @deprecated use Properties instead
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxBarGaugeOptions extends BaseWidgetOptions<dxBarGauge> {
    /**
     * Specifies animation properties.
     */
    animation?: any;
    /**
     * Specifies a color for the remaining segment of the bar&apos;s track.
     */
    backgroundColor?: string;
    /**
     * Specifies a distance between bars in pixels.
     */
    barSpacing?: number;
    /**
     * Specifies a base value for bars.
     */
    baseValue?: number;
    /**
     * Specifies an end value for the gauge&apos;s invisible scale.
     */
    endValue?: number;
    /**
     * Defines the shape of the gauge&apos;s arc.
     */
    geometry?: {
      /**
       * Specifies the end angle of the bar gauge&apos;s arc.
       */
      endAngle?: number;
      /**
       * Specifies the start angle of the bar gauge&apos;s arc.
       */
      startAngle?: number;
    };
    /**
     * Specifies the properties of the labels that accompany gauge bars.
     */
    label?: {
      /**
       * Specifies a color for the label connector text.
       */
      connectorColor?: string;
      /**
       * Specifies the width of the label connector in pixels.
       */
      connectorWidth?: number;
      /**
       * Specifies a callback function that returns a text for labels.
       */
      customizeText?: ((barValue: { value?: number; valueText?: string }) => string);
      /**
       * Specifies font properties for bar labels.
       */
      font?: Font;
      /**
       * Formats a value before it is displayed in a label. Accepts only numeric formats.
       */
      format?: Format;
      /**
       * Specifies the distance between the upper bar and bar labels in pixels.
       */
      indent?: number;
      /**
       * Specifies whether bar labels appear on a gauge or not.
       */
      visible?: boolean;
    };
    /**
     * Configures the legend.
     */
    legend?: dxBarGaugeLegend;
    /**
     * Configures the loading indicator.
     */
    loadingIndicator?: dxBarGaugeLoadingIndicator;
    /**
     * A function that is executed when a tooltip becomes hidden.
     */
    onTooltipHidden?: ((e: TooltipHiddenEvent) => void);
    /**
     * A function that is executed when a tooltip appears.
     */
    onTooltipShown?: ((e: TooltipShownEvent) => void);
    /**
     * Sets the palette to be used for colorizing bars in the gauge.
     */
    palette?: Array<string> | PaletteType;
    /**
     * Specifies what to do with colors in the palette when their number is less than the number of bars in the gauge.
     */
    paletteExtensionMode?: PaletteExtensionModeType;
    /**
     * Defines the radius of the bar that is closest to the center relatively to the radius of the topmost bar.
     */
    relativeInnerRadius?: number;
    /**
     * Specifies how the UI component should behave when bar labels overlap: hide certain labels or leave them overlapped.
     */
    resolveLabelOverlapping?: 'hide' | 'none';
    /**
     * Specifies a start value for the gauge&apos;s invisible scale.
     */
    startValue?: number;
    /**
     * Configures tooltips.
     */
    tooltip?: dxBarGaugeTooltip;
    /**
     * Specifies the array of values to be indicated on a bar gauge.
     */
    values?: Array<number>;
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxBarGaugeLegend extends BaseLegend {
    /**
     * Specifies the hint that appears when a user hovers the mouse pointer over a legend item.
     */
    customizeHint?: ((arg: { item?: BarGaugeBarInfo; text?: string }) => string);
    /**
     * Allows you to change the order, text, and visibility of legend items.
     */
    customizeItems?: ((items: Array<LegendItem>) => Array<LegendItem>);
    /**
     * Customizes the text displayed by legend items.
     */
    customizeText?: ((arg: { item?: BarGaugeBarInfo; text?: string }) => string);
    /**
     * Formats the item text before it is displayed. Accepts only numeric formats. When unspecified, it inherits the label&apos;s format.
     */
    itemTextFormat?: Format;
    /**
     * Specifies an SVG element that serves as a custom legend item marker.
     */
    markerTemplate?: template | ((legendItem: LegendItem, element: SVGGElement) => string | UserDefinedElement<SVGElement>);
    /**
     * Specifies the legend&apos;s visibility.
     */
    visible?: boolean;
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxBarGaugeLoadingIndicator extends BaseWidgetLoadingIndicator {
    /**
     * Specifies whether the loading indicator should be displayed and hidden automatically.
     */
    enabled?: boolean;
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxBarGaugeTooltip extends BaseWidgetTooltip {
    /**
     * Specifies a custom template for a tooltip.
     */
    contentTemplate?: template | ((scaleValue: { value?: number; valueText?: string; index?: number }, element: DxElement) => string | UserDefinedElement);
    /**
     * Allows you to change tooltip appearance.
     */
    customizeTooltip?: ((scaleValue: { value?: number; valueText?: string; index?: number }) => any);
    /**
     * Allows users to interact with the tooltip content.
     */
    interactive?: boolean;
}
/**
 * The BarGauge UI component contains several circular bars that each indicates a single value.
 */
export default class dxBarGauge extends BaseWidget<dxBarGaugeOptions> {
    /**
     * Gets all the values.
     */
    values(): Array<number>;
    /**
     * Updates all the values.
     */
    values(values: Array<number>): void;
}

export type Properties = dxBarGaugeOptions;

/**
 * @deprecated use Properties instead
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export type Options = dxBarGaugeOptions;
