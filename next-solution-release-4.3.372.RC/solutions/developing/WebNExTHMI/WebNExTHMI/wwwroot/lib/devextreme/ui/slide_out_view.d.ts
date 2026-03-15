/**
* DevExtreme (ui/slide_out_view.d.ts)
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
    DxPromise,
} from '../core/utils/deferred';

import {
    template,
} from '../core/templates/template';

import {
    EventInfo,
    InitializedEventInfo,
    ChangedOptionInfo,
} from '../events/index';

import Widget, {
    WidgetOptions,
} from './widget/ui.widget';

export type DisposingEvent = EventInfo<dxSlideOutView>;

export type InitializedEvent = InitializedEventInfo<dxSlideOutView>;

export type OptionChangedEvent = EventInfo<dxSlideOutView> & ChangedOptionInfo;

/**
 * @deprecated use Properties instead
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxSlideOutViewOptions extends WidgetOptions<dxSlideOutView> {
    /**
     * Specifies a custom template for the UI component content.
     */
    contentTemplate?: template | ((contentElement: DxElement) => any);
    /**
     * Specifies the current menu position.
     */
    menuPosition?: 'inverted' | 'normal';
    /**
     * Specifies a custom template for the menu content.
     */
    menuTemplate?: template | ((menuElement: DxElement) => any);
    /**
     * Specifies whether or not the menu panel is visible.
     */
    menuVisible?: boolean;
    /**
     * Specifies whether or not the menu is shown when a user swipes the UI component content.
     */
    swipeEnabled?: boolean;
}
/**
 * The SlideOutView UI component is a classic slide-out menu paired with a view. This UI component is very similar to the SlideOut with only one difference - the SlideOut always contains the List in the slide-out menu, while the SlideOutView can hold any collection there.
 * @deprecated 
 */
export default class dxSlideOutView extends Widget<dxSlideOutViewOptions> {
    /**
     * Gets the UI component&apos;s content.
     */
    content(): DxElement;
    /**
     * Hides the UI component&apos;s slide-out menu.
     */
    hideMenu(): DxPromise<void>;
    /**
     * Gets the slide-out menu&apos;s content.
     */
    menuContent(): DxElement;
    /**
     * Shows the slide-out menu.
     */
    showMenu(): DxPromise<void>;
    /**
     * Shows or hides the slide-out menu depending on the argument.
     */
    toggleMenuVisibility(showing?: boolean): DxPromise<void>;
}

export type Properties = dxSlideOutViewOptions;

/**
 * @deprecated use Properties instead
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export type Options = dxSlideOutViewOptions;
