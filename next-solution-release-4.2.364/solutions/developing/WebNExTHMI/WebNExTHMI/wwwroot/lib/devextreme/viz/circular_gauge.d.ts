/**
* DevExtreme (viz/circular_gauge.d.ts)
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

export type DisposingEvent = EventInfo<dxCircularGauge>;

export type DrawnEvent = EventInfo<dxCircularGauge>;

export type ExportedEvent = EventInfo<dxCircularGauge>;

export type ExportingEvent = EventInfo<dxCircularGauge> & ExportInfo;

export type FileSavingEvent = FileSavingEventInfo<dxCircularGauge>;

export type IncidentOccurredEvent = EventInfo<dxCircularGauge> & IncidentInfo;

export type InitializedEvent = InitializedEventInfo<dxCircularGauge>;

export type OptionChangedEvent = EventInfo<dxCircularGauge> & ChangedOptionInfo;

export type TooltipHiddenEvent = EventInfo<dxCircularGauge> & TooltipInfo;

export type TooltipShownEvent = EventInfo<dxCircularGauge> & TooltipInfo;

/**
 * @deprecated use Properties instead
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxCircularGaugeOptions extends BaseGaugeOptions<dxCircularGauge> {
    /**
     * Specifies the properties required to set the geometry of the CircularGauge UI component.
     */
    geometry?: {
      /**
       * Specifies the end angle of the circular gauge&apos;s arc.
       */
      endAngle?: number;
      /**
       * Specifies the start angle of the circular gauge&apos;s arc.
       */
      startAngle?: number;
    };
    /**
     * Specifies gauge range container properties.
     */
    rangeContainer?: dxCircularGaugeRangeContainer;
    /**
     * Specifies a gauge&apos;s scale properties.
     */
    scale?: dxCircularGaugeScale;
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
export interface dxCircularGaugeRangeContainer extends BaseGaugeRangeContainer {
    /**
     * Specifies the orientation of the range container in the CircularGauge UI component.
     */
    orientation?: 'center' | 'inside' | 'outside';
    /**
     * Specifies the range container&apos;s width in pixels.
     */
    width?: number;
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxCircularGaugeScale extends BaseGaugeScale {
    /**
     * Specifies common properties for scale labels.
     */
    label?: dxCircularGaugeScaleLabel;
    /**
     * Specifies the orientation of scale ticks.
     */
    orientation?: 'center' | 'inside' | 'outside';
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxCircularGaugeScaleLabel extends BaseGaugeScaleLabel {
    /**
     * Specifies which label to hide in case of overlapping.
     */
    hideFirstOrLast?: 'first' | 'last';
    /**
     * Specifies the spacing between scale labels and ticks.
     */
    indentFromTick?: number;
}
/**
 * The CircularGauge is a UI component that indicates values on a circular numeric scale.
 */
export default class dxCircularGauge extends BaseGauge<dxCircularGaugeOptions> { }

export type Properties = dxCircularGaugeOptions;

/**
 * @deprecated use Properties instead
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export type Options = dxCircularGaugeOptions;
