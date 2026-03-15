/**
* DevExtreme (ui/context_menu.d.ts)
* Version: 21.2.6
* Build date: Tue Mar 01 2022
*
* Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
* Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
*/
import { DataSourceLike } from '../data/data_source';
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
    NativeEventInfo,
    InitializedEventInfo,
    ChangedOptionInfo,
    ItemInfo,
} from '../events/index';

import dxMenuBase, {
    dxMenuBaseOptions,
} from './context_menu/ui.menu_base';

import {
    dxMenuBaseItem,
} from './menu';

import {
    SelectionChangedInfo,
} from './collection/ui.collection_widget.base';

export type ContentReadyEvent<TKey = any> = EventInfo<dxContextMenu<TKey>>;

export type DisposingEvent<TKey = any> = EventInfo<dxContextMenu<TKey>>;

export type HiddenEvent<TKey = any> = EventInfo<dxContextMenu<TKey>>;

export type HidingEvent<TKey = any> = Cancelable & EventInfo<dxContextMenu<TKey>>;

export type InitializedEvent<TKey = any> = InitializedEventInfo<dxContextMenu<TKey>>;

export type ItemClickEvent<TKey = any> = NativeEventInfo<dxContextMenu<TKey>, KeyboardEvent | MouseEvent | PointerEvent> & ItemInfo<Item>;

export type ItemContextMenuEvent<TKey = any> = NativeEventInfo<dxContextMenu<TKey>, MouseEvent | PointerEvent | TouchEvent> & ItemInfo<Item>;

export type ItemRenderedEvent<TKey = any> = EventInfo<dxContextMenu<TKey>> & ItemInfo<Item>;

export type OptionChangedEvent<TKey = any> = EventInfo<dxContextMenu<TKey>> & ChangedOptionInfo;

export type PositioningEvent<TKey = any> = NativeEventInfo<dxContextMenu<TKey>, MouseEvent | PointerEvent | TouchEvent> & {
    readonly position: PositionConfig;
};

export type SelectionChangedEvent<TKey = any> = EventInfo<dxContextMenu<TKey>> & SelectionChangedInfo<Item>;

export type ShowingEvent<TKey = any> = Cancelable & EventInfo<dxContextMenu<TKey>>;

export type ShownEvent<TKey = any> = EventInfo<dxContextMenu<TKey>>;

/**
 * @deprecated use Properties instead
 */
export interface dxContextMenuOptions<
    TKey = any,
> extends dxMenuBaseOptions<dxContextMenu<TKey>, dxContextMenuItem, TKey> {
    /**
     * Specifies whether to close the UI component if a user clicks outside it.
     */
    closeOnOutsideClick?: boolean | ((event: DxEvent<MouseEvent | PointerEvent | TouchEvent>) => boolean);
    /**
     * Binds the UI component to data.
     */
    dataSource?: DataSourceLike<Item, TKey>;
    /**
     * Holds an array of menu items.
     */
    items?: Array<Item>;
    /**
     * A function that is executed after the ContextMenu is hidden.
     */
    onHidden?: ((e: HiddenEvent<TKey>) => void);
    /**
     * A function that is executed before the ContextMenu is hidden.
     */
    onHiding?: ((e: HidingEvent<TKey>) => void);
    /**
     * A function that is executed before the ContextMenu is positioned.
     */
    onPositioning?: ((e: PositioningEvent<TKey>) => void);
    /**
     * A function that is executed before the ContextMenu is shown.
     */
    onShowing?: ((e: ShowingEvent<TKey>) => void);
    /**
     * A function that is executed after the ContextMenu is shown.
     */
    onShown?: ((e: ShownEvent<TKey>) => void);
    /**
     * An object defining UI component positioning properties.
     */
    position?: PositionConfig;
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
     * Specifies the direction at which submenus are displayed.
     */
    submenuDirection?: 'auto' | 'left' | 'right';
    /**
     * The target element associated with the context menu.
     */
    target?: string | UserDefinedElement;
    /**
     * A Boolean value specifying whether or not the UI component is visible.
     */
    visible?: boolean;
}
/**
 * The ContextMenu UI component displays a single- or multi-level context menu. An end user invokes this menu by a right click or a long press.
 */
export default class dxContextMenu<
    TKey = any,
> extends dxMenuBase<dxContextMenuOptions<TKey>, dxContextMenuItem, TKey> {
    /**
     * Hides the UI component.
     */
    hide(): DxPromise<void>;
    /**
     * Shows the UI component.
     */
    show(): DxPromise<void>;
    /**
     * Shows or hides the UI component depending on the argument.
     */
    toggle(showing: boolean): DxPromise<void>;
}

export type Item = dxContextMenuItem;

/**
 * @deprecated Use Item instead
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxContextMenuItem extends dxMenuBaseItem {
    /**
     * Specifies nested menu items.
     */
    items?: Array<Item>;
}

export type ExplicitTypes<TKey = any> = {
    Properties: Properties<TKey>;
    ContentReadyEvent: ContentReadyEvent<TKey>;
    DisposingEvent: DisposingEvent<TKey>;
    HiddenEvent: HiddenEvent<TKey>;
    HidingEvent: HidingEvent<TKey>;
    InitializedEvent: InitializedEvent<TKey>;
    ItemClickEvent: ItemClickEvent<TKey>;
    ItemContextMenuEvent: ItemContextMenuEvent<TKey>;
    ItemRenderedEvent: ItemRenderedEvent<TKey>;
    OptionChangedEvent: OptionChangedEvent<TKey>;
    PositioningEvent: PositioningEvent<TKey>;
    SelectionChangedEvent: SelectionChangedEvent<TKey>;
    ShowingEvent: ShowingEvent<TKey>;
    ShownEvent: ShownEvent<TKey>;
};

export type Properties<TKey = any> = dxContextMenuOptions<TKey>;

/**
 * @deprecated use Properties instead
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export type Options<TKey = any> = Properties<TKey>;
