/**
* DevExtreme (ui/load_indicator.d.ts)
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

import Widget, {
    WidgetOptions,
} from './widget/ui.widget';

export type ContentReadyEvent = EventInfo<dxLoadIndicator>;

export type DisposingEvent = EventInfo<dxLoadIndicator>;

export type InitializedEvent = InitializedEventInfo<dxLoadIndicator>;

export type OptionChangedEvent = EventInfo<dxLoadIndicator> & ChangedOptionInfo;

/**
 * @deprecated use Properties instead
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxLoadIndicatorOptions extends WidgetOptions<dxLoadIndicator> {
    /**
     * Specifies the path to an image used as the indicator.
     */
    indicatorSrc?: string;
}
/**
 * The LoadIndicator is a UI element notifying the viewer that a process is in progress.
 */
export default class dxLoadIndicator extends Widget<dxLoadIndicatorOptions> { }

export type Properties = dxLoadIndicatorOptions;

/**
 * @deprecated use Properties instead
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export type Options = dxLoadIndicatorOptions;
