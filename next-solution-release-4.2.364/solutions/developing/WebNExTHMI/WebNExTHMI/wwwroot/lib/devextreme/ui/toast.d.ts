/**
* DevExtreme (ui/toast.d.ts)
* Version: 21.2.6
* Build date: Tue Mar 01 2022
*
* Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
* Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
*/
import {
    AnimationConfig,
} from '../animation/fx';

import {
    PositionConfig,
} from '../animation/position';

import {
    DxEvent,
    Cancelable,
    EventInfo,
    InitializedEventInfo,
    ChangedOptionInfo,
} from '../events/index';

import dxOverlay, {
    dxOverlayAnimation,
    dxOverlayOptions,
} from './overlay';

export type ContentReadyEvent = EventInfo<dxToast>;

export type DisposingEvent = EventInfo<dxToast>;

export type HidingEvent = Cancelable & EventInfo<dxToast>;

export type HiddenEvent = EventInfo<dxToast>;

export type InitializedEvent = InitializedEventInfo<dxToast>;

export type OptionChangedEvent = EventInfo<dxToast> & ChangedOptionInfo;

export type ShowingEvent = Cancelable & EventInfo<dxToast>;

export type ShownEvent = EventInfo<dxToast>;

/**
 * @deprecated use Properties instead
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxToastOptions extends dxOverlayOptions<dxToast> {
    /**
     * Configures UI component visibility animations. This object contains two fields: show and hide.
     */
    animation?: dxToastAnimation;
    /**
     * A Boolean value specifying whether or not the toast is closed if a user clicks it.
     */
    closeOnClick?: boolean;
    /**
     * Specifies whether to close the UI component if a user clicks outside it.
     */
    closeOnOutsideClick?: boolean | ((event: DxEvent<MouseEvent | PointerEvent | TouchEvent>) => boolean);
    /**
     * A Boolean value specifying whether or not the toast is closed if a user swipes it out of the screen boundaries.
     */
    closeOnSwipe?: boolean;
    /**
     * The time span in milliseconds during which the Toast UI component is visible.
     */
    displayTime?: number;
    /**
     * Specifies the UI component&apos;s height.
     */
    height?: number | string | (() => number | string);
    /**
     * Specifies the maximum width the UI component can reach while resizing.
     */
    maxWidth?: number | string | (() => number | string);
    /**
     * The Toast message text.
     */
    message?: string;
    /**
     * Specifies the minimum width the UI component can reach while resizing.
     */
    minWidth?: number | string | (() => number | string);
    /**
     * Positions the UI component.
     */
    position?: PositionConfig | string;
    /**
     * Specifies whether to shade the background when the UI component is active.
     */
    shading?: boolean;
    /**
     * Specifies the Toast UI component type.
     */
    type?: 'custom' | 'error' | 'info' | 'success' | 'warning';
    /**
     * Specifies the UI component&apos;s width.
     */
    width?: number | string | (() => number | string);
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxToastAnimation extends dxOverlayAnimation {
    /**
     * An object that defines the animation properties used when the UI component is being hidden.
     */
    hide?: AnimationConfig;
    /**
     * An object that defines the animation properties used when the UI component is being shown.
     */
    show?: AnimationConfig;
}
/**
 * The Toast is a UI component that provides pop-up notifications.
 */
export default class dxToast extends dxOverlay<dxToastOptions> { }

export type Properties = dxToastOptions;

/**
 * @deprecated use Properties instead
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export type Options = dxToastOptions;
