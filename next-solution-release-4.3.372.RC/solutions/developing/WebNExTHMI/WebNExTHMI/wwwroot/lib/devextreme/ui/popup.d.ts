/**
* DevExtreme (ui/popup.d.ts)
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
    DxElement,
} from '../core/element';

import {
    template,
} from '../core/templates/template';

import {
    Cancelable,
    EventInfo,
    NativeEventInfo,
    InitializedEventInfo,
    ChangedOptionInfo,
} from '../events/index';

import dxOverlay, {
    dxOverlayAnimation,
    dxOverlayOptions,
} from './overlay';

import {
    ResizeInfo,
} from './resizable';

/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface TitleRenderedInfo {
    readonly titleElement: DxElement;
}

export type ContentReadyEvent = EventInfo<dxPopup>;

export type DisposingEvent = EventInfo<dxPopup>;

export type HidingEvent = Cancelable & EventInfo<dxPopup>;

export type HiddenEvent = EventInfo<dxPopup>;

export type InitializedEvent = InitializedEventInfo<dxPopup>;

export type ShownEvent = EventInfo<dxPopup>;

export type ResizeEvent = NativeEventInfo<dxPopup, MouseEvent | TouchEvent> & ResizeInfo;

export type ResizeStartEvent = NativeEventInfo<dxPopup, MouseEvent | TouchEvent> & ResizeInfo;

export type ResizeEndEvent = NativeEventInfo<dxPopup, MouseEvent | TouchEvent> & ResizeInfo;

export type OptionChangedEvent = EventInfo<dxPopup> & ChangedOptionInfo;

export type ShowingEvent = Cancelable & EventInfo<dxPopup>;

export type TitleRenderedEvent = EventInfo<dxPopup> & TitleRenderedInfo;

/**
 * @deprecated use Properties instead
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxPopupOptions<TComponent> extends dxOverlayOptions<TComponent> {
    /**
     * Configures UI component visibility animations. This object contains two fields: show and hide.
     */
    animation?: dxPopupAnimation;
    /**
     * Specifies the container in which to render the UI component.
     */
    container?: string | UserDefinedElement;
    /**
     * Specifies whether or not to allow a user to drag the popup window.
     */
    dragEnabled?: boolean;
    /**
     * Specifies whether the UI component can be focused using keyboard navigation.
     */
    focusStateEnabled?: boolean;
    /**
     * Specifies whether to display the Popup in full-screen mode.
     */
    fullScreen?: boolean;
    /**
     * Specifies the UI component&apos;s height.
     */
    height?: number | string | (() => number | string);
    /**
     * A function that is executed each time the UI component is resized by one pixel.
     */
    onResize?: ((e: ResizeEvent) => void);
    /**
     * A function that is executed when resizing ends.
     */
    onResizeEnd?: ((e: ResizeEndEvent) => void);
    /**
     * A function that is executed when resizing starts.
     */
    onResizeStart?: ((e: ResizeStartEvent) => void);
    /**
     * A function that is executed when the UI component&apos;s title is rendered.
     */
    onTitleRendered?: ((e: EventInfo<TComponent> & TitleRenderedInfo) => void);
    /**
     * Positions the UI component.
     */
    position?: 'bottom' | 'center' | 'left' | 'left bottom' | 'left top' | 'right' | 'right bottom' | 'right top' | 'top' | PositionConfig | Function;
    /**
     * Specifies whether or not an end user can resize the UI component.
     */
    resizeEnabled?: boolean;
    /**
     * Specifies whether to display the widget at the initial position when users reopen it.
     */
    restorePosition?: boolean;
    /**
     * Specifies whether or not the UI component displays the Close button.
     */
    showCloseButton?: boolean;
    /**
     * A Boolean value specifying whether or not to display the title in the popup window.
     */
    showTitle?: boolean;
    /**
     * The title in the overlay window.
     */
    title?: string;
    /**
     * Specifies a custom template for the UI component title. Does not apply if the title is defined.
     */
    titleTemplate?: template | ((titleElement: DxElement) => string | UserDefinedElement);
    /**
     * Configures toolbar items.
     */
    toolbarItems?: Array<ToolbarItem>;
    /**
     * Specifies the UI component&apos;s width.
     */
    width?: number | string | (() => number | string);
}
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxPopupAnimation extends dxOverlayAnimation {
    /**
     * An object that defines the animation properties used when the UI component is being hidden.
     */
    hide?: AnimationConfig;
    /**
     * An object that defines the animation properties used when the UI component is being shown.
     */
    show?: AnimationConfig;
}

export type ToolbarItem = dxPopupToolbarItem;

/**
 * @deprecated Use ToolbarItem instead
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxPopupToolbarItem {
    /**
     * Specifies whether the toolbar item responds to user interaction.
     */
    disabled?: boolean;
    /**
     * Specifies the HTML markup to be inserted into the toolbar item element.
     */
    html?: string;
    /**
     * Specifies a location for the item on the toolbar.
     */
    location?: 'after' | 'before' | 'center';
    /**
     * Configures the DevExtreme UI component used as a toolbar item.
     */
    options?: any;
    /**
     * Specifies a template that should be used to render this item only.
     */
    template?: template;
    /**
     * Specifies text displayed for the toolbar item.
     */
    text?: string;
    /**
     * Specifies whether the item is displayed on a top or bottom toolbar.
     */
    toolbar?: 'bottom' | 'top';
    /**
     * Specifies whether or not a UI component item must be displayed.
     */
    visible?: boolean;
    /**
     * A UI component that presents a toolbar item.
     */
    widget?: 'dxAutocomplete' | 'dxButton' | 'dxCheckBox' | 'dxDateBox' | 'dxMenu' | 'dxSelectBox' | 'dxTabs' | 'dxTextBox' | 'dxButtonGroup' | 'dxDropDownButton';
}
/**
 * The Popup UI component is a pop-up window overlaying the current view.
 */
export default class dxPopup<TProperties = Properties> extends dxOverlay<TProperties> {}

/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
interface PopupInstance extends dxPopup<Properties> { }

export type Properties = dxPopupOptions<PopupInstance>;

/**
 * @deprecated use Properties instead
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export type Options = Properties;
