/**
* DevExtreme (viz/linear_gauge.d.ts)
* Version: 21.2.6
* Build date: Tue Mar 01 2022
*
* Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
* Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
*/
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

import {
    BaseGauge,
    BaseGaugeOptions,
    BaseGaugeRangeContainer,
    BaseGaugeScale,
    BaseGaugeScaleLabel,
    GaugeIndicator,
    TooltipInfo,
} from './gauges/base_gauge';

export type DisposingEvent = EventInfo<dxLinearGauge>;

export type DrawnEvent = EventInfo<dxLinearGauge>;

export type ExportedEvent = EventInfo<dxLinearGauge>;

export type ExportingEvent = EventInfo<dxLinearGauge> & ExportInfo;

export type FileSavingEvent = FileSavingEventInfo<dxLinearGauge>;

export type IncidentOccurredEvent = EventInfo<dxLinearGauge> & IncidentInfo;

export type InitializedEvent = InitializedEventInfo<dxLinearGauge>;

export type OptionChangedEvent = EventInfo<dxLinearGauge> & ChangedOptionInfo;

export type TooltipHiddenEvent = EventInfo<dxLinearGauge> & TooltipInfo;

export type TooltipShownEvent = EventInfo<dxLinearGauge> & TooltipInfo;

/**
 * @deprecated use Properties instead
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxLinearGaugeOptions extends BaseGaugeOptions<dxLinearGauge> {
    /**
     * Specifies the properties required to set the geometry of the LinearGauge UI component.
     */
    geometry?: {
      /**
       * Indicates whether to display the LinearGauge UI component vertically or horizontally.
       */
      orientation?: 'horizontal' | 'vertical';
    };
    /**
     * Specifies gauge range container properties.
     */
    rangeContainer?: dxLinearGaugeRangeContainer;
    /**
     * Specifies the gauge&apos;s scale properties.
     */
    scale?: dxLinearGaugeScale;
    /**
     * Specifies the appearance properties of subvalue indicators.
     */
    subvalueIndicator?: GaugeIndicator;
    /**
     * Specifies the appearance properties of the value indicator.
     */
    valueIndicator?: GaugeIndicator;
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxLinearGaugeRangeContainer extends BaseGaugeRangeContainer {
    /**
     * Specifies the orientation of the range container. Applies only if the geometry.orientation property is &apos;vertical&apos;.
     */
    horizontalOrientation?: 'center' | 'left' | 'right';
    /**
     * Specifies the orientation of the range container. Applies only if the geometry.orientation property is &apos;horizontal&apos;.
     */
    verticalOrientation?: 'bottom' | 'center' | 'top';
    /**
     * Specifies the width of the range container&apos;s start and end boundaries in the LinearGauge UI component.
     */
    width?: {
      /**
       * Specifies a start width of a range container.
       */
      start?: number;
      /**
       * Specifies an end width of a range container.
       */
      end?: number;
    } | number;
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxLinearGaugeScale extends BaseGaugeScale {
    /**
     * Specifies the orientation of scale ticks. Applies only if the geometry.orientation property is &apos;vertical&apos;.
     */
    horizontalOrientation?: 'center' | 'left' | 'right';
    /**
     * Specifies common properties for scale labels.
     */
    label?: dxLinearGaugeScaleLabel;
    /**
     * Specifies the minimum distance between two neighboring major ticks in pixels.
     */
    scaleDivisionFactor?: number;
    /**
     * Specifies the orientation of scale ticks. Applies only if the geometry.orientation property is &apos;horizontal&apos;.
     */
    verticalOrientation?: 'bottom' | 'center' | 'top';
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxLinearGaugeScaleLabel extends BaseGaugeScaleLabel {
    /**
     * Specifies the spacing between scale labels and ticks.
     */
    indentFromTick?: number;
}
/**
 * The LinearGauge is a UI component that indicates values on a linear numeric scale.
 */
export default class dxLinearGauge extends BaseGauge<dxLinearGaugeOptions> { }

export type Properties = dxLinearGaugeOptions;

/**
 * @deprecated use Properties instead
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export type Options = dxLinearGaugeOptions;
