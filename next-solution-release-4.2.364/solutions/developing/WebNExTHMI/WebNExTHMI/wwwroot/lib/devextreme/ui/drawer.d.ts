/**
* DevExtreme (ui/drawer.d.ts)
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
    DxPromise,
} from '../core/utils/deferred';

import {
    template,
} from '../core/templates/template';

import {
    DxEvent,
    EventInfo,
    InitializedEventInfo,
    ChangedOptionInfo,
} from '../events/index';

import Widget, {
    WidgetOptions,
} from './widget/ui.widget';

export type DisposingEvent = EventInfo<dxDrawer>;

export type InitializedEvent = InitializedEventInfo<dxDrawer>;

export type OptionChangedEvent = EventInfo<dxDrawer> & ChangedOptionInfo;

/**
 * @deprecated use Properties instead
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxDrawerOptions extends WidgetOptions<dxDrawer> {
    /**
     * Specifies the duration of the drawer&apos;s opening and closing animation (in milliseconds). Applies only if animationEnabled is true.
     */
    animationDuration?: number;
    /**
     * Specifies whether to use an opening and closing animation.
     */
    animationEnabled?: boolean;
    /**
     * Specifies whether to close the drawer if a user clicks or taps the view area.
     */
    closeOnOutsideClick?: boolean | ((event: DxEvent<MouseEvent | PointerEvent | TouchEvent>) => boolean);
    /**
     * Specifies the drawer&apos;s width or height (depending on the drawer&apos;s position) in the opened state.
     */
    maxSize?: number;
    /**
     * Specifies the drawer&apos;s width or height (depending on the drawer&apos;s position) in the closed state.
     */
    minSize?: number;
    /**
     * Specifies whether the drawer is opened.
     */
    opened?: boolean;
    /**
     * Specifies how the drawer interacts with the view in the opened state.
     */
    openedStateMode?: 'overlap' | 'shrink' | 'push';
    /**
     * Specifies the drawer&apos;s position in relation to the view.
     */
    position?: 'left' | 'right' | 'top' | 'bottom' | 'before' | 'after';
    /**
     * Specifies the drawer&apos;s reveal mode.
     */
    revealMode?: 'slide' | 'expand';
    /**
     * Specifies whether to shade the view when the drawer is opened.
     */
    shading?: boolean;
    /**
     * Specifies a CSS selector for the element in which the drawer should be rendered. Applies only when the openedStateMode is &apos;overlap&apos;.
     * @deprecated 
     */
    target?: string | UserDefinedElement;
    /**
     * Specifies the drawer&apos;s content.
     */
    template?: template | ((Element: DxElement) => any);
}
/**
 * The Drawer is a dismissible or permanently visible panel used for navigation in responsive web application layouts.
 */
export default class dxDrawer extends Widget<dxDrawerOptions> {
    /**
     * Gets the drawer&apos;s content.
     */
    content(): DxElement;
    /**
     * Closes the drawer.
     */
    hide(): DxPromise<void>;
    /**
     * Opens the drawer.
     */
    show(): DxPromise<void>;
    /**
     * Opens or closes the drawer, reversing the current state.
     */
    toggle(): DxPromise<void>;
}

export type Properties = dxDrawerOptions;

/**
 * @deprecated use Properties instead
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export type Options = dxDrawerOptions;
