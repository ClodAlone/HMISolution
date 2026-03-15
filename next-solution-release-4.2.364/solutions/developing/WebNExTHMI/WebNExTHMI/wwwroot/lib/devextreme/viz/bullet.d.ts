/**
* DevExtreme (viz/bullet.d.ts)
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

import BaseSparkline, {
    BaseSparklineOptions,
} from './sparklines/base_sparkline';

export type DisposingEvent = EventInfo<dxBullet>;

export type DrawnEvent = EventInfo<dxBullet>;

export type ExportedEvent = EventInfo<dxBullet>;

export type ExportingEvent = EventInfo<dxBullet> & ExportInfo;

export type FileSavingEvent = FileSavingEventInfo<dxBullet>;

export type IncidentOccurredEvent = EventInfo<dxBullet> & IncidentInfo;

export type InitializedEvent = InitializedEventInfo<dxBullet>;

export type OptionChangedEvent = EventInfo<dxBullet> & ChangedOptionInfo;

export type TooltipHiddenEvent = EventInfo<dxBullet>;

export type TooltipShownEvent = EventInfo<dxBullet>;

/**
 * @deprecated use Properties instead
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxBulletOptions extends BaseSparklineOptions<dxBullet> {
    /**
     * Specifies a color for the bullet bar.
     */
    color?: string;
    /**
     * Specifies an end value for the invisible scale.
     */
    endScaleValue?: number;
    /**
     * Specifies whether or not to show the target line.
     */
    showTarget?: boolean;
    /**
     * Specifies whether or not to show the line indicating zero on the invisible scale.
     */
    showZeroLevel?: boolean;
    /**
     * Specifies a start value for the invisible scale.
     */
    startScaleValue?: number;
    /**
     * Specifies the value indicated by the target line.
     */
    target?: number;
    /**
     * Specifies a color for both the target and zero level lines.
     */
    targetColor?: string;
    /**
     * Specifies the width of the target line.
     */
    targetWidth?: number;
    /**
     * Specifies the primary value indicated by the bullet bar.
     */
    value?: number;
}
/**
 * The Bullet UI component is useful when you need to compare a single measure to a target value. The UI component comprises a horizontal bar indicating the measure and a vertical line indicating the target value.
 */
export default class dxBullet extends BaseSparkline<dxBulletOptions> { }

export type Properties = dxBulletOptions;

/**
 * @deprecated use Properties instead
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export type Options = dxBulletOptions;
