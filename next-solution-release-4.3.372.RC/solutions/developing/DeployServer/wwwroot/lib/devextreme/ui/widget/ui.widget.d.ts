/**
* DevExtreme (ui/widget/ui.widget.d.ts)
* Version: 21.2.6
* Build date: Tue Mar 01 2022
*
* Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
* Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
*/
import { Format } from '../../localization';
import DOMComponent, {
    DOMComponentOptions,
} from '../../core/dom_component';

import {
    EventInfo,
} from '../../events/index';

/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface WidgetOptions<TComponent> extends DOMComponentOptions<TComponent> {
    /**
     * Specifies the shortcut key that sets focus on the UI component.
     */
    accessKey?: string;
    /**
     * Specifies whether or not the UI component changes its state when interacting with a user.
     */
    activeStateEnabled?: boolean;
    /**
     * Specifies whether the UI component responds to user interaction.
     */
    disabled?: boolean;
    /**
     * Specifies whether the UI component can be focused using keyboard navigation.
     */
    focusStateEnabled?: boolean;
    /**
     * Specifies text for a hint that appears when a user pauses on the UI component.
     */
    hint?: string;
    /**
     * Specifies whether the UI component changes its state when a user pauses on it.
     */
    hoverStateEnabled?: boolean;
    /**
     * A function that is executed when the UI component&apos;s content is ready and each time the content is changed.
     */
    onContentReady?: ((e: EventInfo<TComponent>) => void);
    /**
     * Specifies the number of the element when the Tab key is used for navigating.
     */
    tabIndex?: number;
    /**
     * Specifies whether the UI component is visible.
     */
    visible?: boolean;
}
/**
 * The base class for UI components.
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export default class Widget<TProperties> extends DOMComponent<TProperties> {
    /**
     * Sets focus on the UI component.
     */
    focus(): void;
    /**
     * Registers a handler to be executed when a user presses a specific key.
     */
    registerKeyHandler(key: string, handler: Function): void;
    /**
     * Repaints the UI component without reloading data. Call it to update the UI component&apos;s markup.
     */
    repaint(): void;
}

/**
                                                                                                                                                      * Specifies markup for a UI component item.
                                                                                                                                                      */
                                                                                                                                                     export var dxItem: any;

/**
 * 
 * @deprecated 
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export type format = Format;
