/**
* DevExtreme (ui/speed_dial_action.d.ts)
* Version: 21.2.6
* Build date: Tue Mar 01 2022
*
* Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
* Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
*/
import {
    DxElement,
} from '../core/element';

import {
    EventInfo,
    NativeEventInfo,
    InitializedEventInfo,
    ChangedOptionInfo,
} from '../events/index';

import Widget, {
    WidgetOptions,
} from './widget/ui.widget';

export type ClickEvent = NativeEventInfo<dxSpeedDialAction, MouseEvent | PointerEvent> & {
    actionElement?: DxElement;
};

export type ContentReadyEvent = EventInfo<dxSpeedDialAction> & {
    actionElement?: DxElement;
};

export type DisposingEvent = EventInfo<dxSpeedDialAction>;

export type InitializedEvent = InitializedEventInfo<dxSpeedDialAction>;

export type OptionChangedEvent = EventInfo<dxSpeedDialAction> & ChangedOptionInfo;

/**
 * @deprecated use Properties instead
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxSpeedDialActionOptions extends WidgetOptions<dxSpeedDialAction> {
    /**
     * Specifies the icon the FAB or speed dial action button displays.
     */
    icon?: string;
    /**
     * Allows you to reorder action buttons in the speed dial menu.
     */
    index?: number;
    /**
     * Specifies the text label displayed inside the FAB or near the speed dial action button.
     */
    label?: string;
    /**
     * A function that is executed when the FAB or speed dial action button is clicked or tapped.
     */
    onClick?: ((e: ClickEvent) => void);
    /**
     * A function that is executed when the UI component&apos;s content is ready and each time the content is changed.
     */
    onContentReady?: ((e: ContentReadyEvent) => void);
    /**
     * Allows you to hide the FAB from the view or the action from the speed dial menu.
     */
    visible?: boolean;
}
/**
 * The SpeedDialAction is a button that performs a custom action. It can be represented by a Floating Action Button (FAB) or a button in a speed dial menu opened with the FAB.
 */
export default class dxSpeedDialAction extends Widget<dxSpeedDialActionOptions> { }

export type Properties = dxSpeedDialActionOptions;

/**
 * @deprecated use Properties instead
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export type Options = dxSpeedDialActionOptions;
