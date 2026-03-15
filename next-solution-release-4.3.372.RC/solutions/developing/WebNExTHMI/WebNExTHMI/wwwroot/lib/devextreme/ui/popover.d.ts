/**
* DevExtreme (ui/popover.d.ts)
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
    UserDefinedElement,
} from '../core/element';

import {
    DxPromise,
} from '../core/utils/deferred';

import {
    DxEvent,
    Cancelable,
    EventInfo,
    InitializedEventInfo,
    ChangedOptionInfo,
} from '../events/index';

import dxPopup, {
    dxPopupAnimation,
    dxPopupOptions,
    TitleRenderedInfo,
} from './popup';

export type ContentReadyEvent = EventInfo<dxPopover>;

export type DisposingEvent = EventInfo<dxPopover>;

export type HidingEvent = Cancelable & EventInfo<dxPopover>;

export type HiddenEvent = EventInfo<dxPopover>;

export type InitializedEvent = InitializedEventInfo<dxPopover>;

export type OptionChangedEvent = EventInfo<dxPopover> & ChangedOptionInfo;

export type ShowingEvent = Cancelable & EventInfo<dxPopover>;

export type ShownEvent = EventInfo<dxPopover>;

export type TitleRenderedEvent = EventInfo<dxPopover> & TitleRenderedInfo;

/**
 * @deprecated use Properties instead
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxPopoverOptions<TComponent> extends dxPopupOptions<TComponent> {
    /**
     * Configures UI component visibility animations. This object contains two fields: show and hide.
     */
    animation?: dxPopoverAnimation;
    /**
     * Specifies whether to close the UI component if a user clicks outside the popover window or outside the target element.
     */
    closeOnOutsideClick?: boolean | ((event: DxEvent<MouseEvent | PointerEvent | TouchEvent>) => boolean);
    /**
     * Specifies the UI component&apos;s height.
     */
    height?: number | string | (() => number | string);
    /**
     * Specifies properties of popover hiding. Ignored if the shading property is set to true.
     */
    hideEvent?: {
      /**
       * The delay in milliseconds after which the UI component is hidden.
       */
      delay?: number;
      /**
       * Specifies the event names on which the UI component is hidden.
       */
      name?: string;
    } | string;
    /**
     * An object defining UI component positioning properties.
     */
    position?: 'bottom' | 'left' | 'right' | 'top' | PositionConfig;
    /**
     * Specifies whether to shade the background when the UI component is active.
     */
    shading?: boolean;
    /**
     * Specifies properties for displaying the UI component.
     */
    showEvent?: {
      /**
       * The delay in milliseconds after which the UI component is displayed.
       */
      delay?: number;
      /**
       * Specifies the event names on which the UI component is shown.
       */
      name?: string;
    } | string;
    /**
     * A Boolean value specifying whether or not to display the title in the overlay window.
     */
    showTitle?: boolean;
    /**
     * Specifies the element against which to position the widget.
     */
    target?: string | UserDefinedElement;
    /**
     * Specifies the UI component&apos;s width.
     */
    width?: number | string | (() => number | string);
    /**
     * Specifies whether to hide the widget when users scroll one of its parent elements.
     */
    hideOnParentScroll?: boolean;
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxPopoverAnimation extends dxPopupAnimation {
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
 * The Popover is a UI component that shows notifications within a box with an arrow pointing to a specified UI element.
 */
export default class dxPopover<TProperties = Properties> extends dxPopup<TProperties> {
    show(): DxPromise<boolean>;
    /**
     * Shows the UI component for a target element.
     */
    show(target: string | UserDefinedElement): DxPromise<boolean>;
}

/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
interface PopoverInstance extends dxPopover<Properties> { }

export type Properties = dxPopoverOptions<PopoverInstance>;

/**
 * @deprecated use Properties instead
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export type Options = Properties;
