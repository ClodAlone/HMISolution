/**
* DevExtreme (ui/menu.d.ts)
* Version: 21.2.6
* Build date: Tue Mar 01 2022
*
* Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
* Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
*/
import { DataSourceLike } from '../data/data_source';
import {
    DxElement,
} from '../core/element';

import {
    Cancelable,
    EventInfo,
    NativeEventInfo,
    InitializedEventInfo,
    ChangedOptionInfo,
    ItemInfo,
} from '../events/index';

import {
    CollectionWidgetItem,
    SelectionChangedInfo,
} from './collection/ui.collection_widget.base';

import dxMenuBase, {
    dxMenuBaseOptions,
} from './context_menu/ui.menu_base';

export type ContentReadyEvent<TKey = any> = EventInfo<dxMenu<TKey>>;

export type DisposingEvent<TKey = any> = EventInfo<dxMenu<TKey>>;

export type InitializedEvent<TKey = any> = InitializedEventInfo<dxMenu<TKey>>;

export type ItemClickEvent<TKey = any> = NativeEventInfo<dxMenu<TKey>, KeyboardEvent | MouseEvent | PointerEvent> & ItemInfo<Item>;

export type ItemContextMenuEvent<TKey = any> = NativeEventInfo<dxMenu<TKey>, MouseEvent | PointerEvent | TouchEvent> & ItemInfo<Item>;

export type ItemRenderedEvent<TKey = any> = EventInfo<dxMenu<TKey>> & ItemInfo<Item>;

export type OptionChangedEvent<TKey = any> = EventInfo<dxMenu<TKey>> & ChangedOptionInfo;

export type SelectionChangedEvent<TKey = any> = EventInfo<dxMenu<TKey>> & SelectionChangedInfo<Item>;

export type SubmenuHiddenEvent<TKey = any> = EventInfo<dxMenu<TKey>> & {
    readonly rootItem?: DxElement;
};

export type SubmenuHidingEvent<TKey = any> = Cancelable & EventInfo<dxMenu<TKey>> & {
    readonly rootItem?: DxElement;
};

export type SubmenuShowingEvent<TKey = any> = EventInfo<dxMenu<TKey>> & {
    readonly rootItem?: DxElement;
};

export type SubmenuShownEvent<TKey = any> = EventInfo<dxMenu<TKey>> & {
    readonly rootItem?: DxElement;
};

/**
 * @deprecated use Properties instead
 */
export interface dxMenuOptions<
    TKey = any,
> extends dxMenuBaseOptions<dxMenu<TKey>, dxMenuItem, TKey> {
    /**
     * Specifies whether adaptive UI component rendering is enabled on small screens. Applies only if the orientation is &apos;horizontal&apos;.
     */
    adaptivityEnabled?: boolean;
    /**
     * Binds the UI component to data.
     */
    dataSource?: DataSourceLike<Item, TKey>;
    /**
     * Specifies whether or not the submenu is hidden when the mouse pointer leaves it.
     */
    hideSubmenuOnMouseLeave?: boolean;
    /**
     * Holds an array of menu items.
     */
    items?: Array<Item>;
    /**
     * A function that is executed after a submenu is hidden.
     */
    onSubmenuHidden?: ((e: SubmenuHiddenEvent<TKey>) => void);
    /**
     * A function that is executed before a submenu is hidden.
     */
    onSubmenuHiding?: ((e: SubmenuHidingEvent<TKey>) => void);
    /**
     * A function that is executed before a submenu is displayed.
     */
    onSubmenuShowing?: ((e: SubmenuShowingEvent<TKey>) => void);
    /**
     * A function that is executed after a submenu is displayed.
     */
    onSubmenuShown?: ((e: SubmenuShownEvent<TKey>) => void);
    /**
     * Specifies whether the menu has horizontal or vertical orientation.
     */
    orientation?: 'horizontal' | 'vertical';
    /**
     * Specifies properties for showing and hiding the first level submenu.
     */
    showFirstSubmenuMode?: {
      /**
       * Specifies the delay in submenu showing and hiding.
       */
      delay?: {
        /**
         * The time span after which the submenu is hidden.
         */
        hide?: number;
        /**
         * The time span after which the submenu is shown.
         */
        show?: number;
      } | number;
      /**
       * Specifies the mode name.
       */
      name?: 'onClick' | 'onHover';
    } | 'onClick' | 'onHover';
    /**
     * Specifies the direction at which the submenus are displayed.
     */
    submenuDirection?: 'auto' | 'leftOrTop' | 'rightOrBottom';
}
/**
 * The Menu UI component is a panel with clickable items. A click on an item opens a drop-down menu, which can contain several submenus.
 */
export default class dxMenu<
    TKey = any,
> extends dxMenuBase<dxMenuOptions<TKey>, dxMenuItem, TKey> { }

/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface MenuBasePlainItem extends CollectionWidgetItem {
  /**
   * Specifies whether a group separator is displayed over the item.
   */
  beginGroup?: boolean;
  /**
   * Specifies if a menu is closed when a user clicks the item.
   */
  closeMenuOnClick?: boolean;
  /**
   * Specifies whether the menu item responds to user interaction.
   */
  disabled?: boolean;
  /**
   * Specifies the menu item&apos;s icon.
   */
  icon?: string;
  /**
   * Specifies whether or not a user can select a menu item.
   */
  selectable?: boolean;
  /**
   * Specifies whether or not the item is selected.
   */
  selected?: boolean;
  /**
   * Specifies the text inserted into the item element.
   */
  text?: string;
  /**
   * Specifies whether or not the menu item is visible.
   */
  visible?: boolean;
}

/**
 * 
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxMenuBaseItem extends MenuBasePlainItem {
    /**
     * Specifies nested menu items.
     */
    items?: Array<dxMenuBaseItem>;
}

export type Item = dxMenuItem;

/**
 * @deprecated Use Item instead
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxMenuItem extends dxMenuBaseItem {
    /**
     * Specifies nested menu items.
     */
    items?: Array<Item>;
}

export type ExplicitTypes<TKey = any> = {
    Properties: Properties<TKey>;
    ContentReadyEvent: ContentReadyEvent<TKey>;
    DisposingEvent: DisposingEvent<TKey>;
    InitializedEvent: InitializedEvent<TKey>;
    ItemClickEvent: ItemClickEvent<TKey>;
    ItemContextMenuEvent: ItemContextMenuEvent<TKey>;
    ItemRenderedEvent: ItemRenderedEvent<TKey>;
    OptionChangedEvent: OptionChangedEvent<TKey>;
    SelectionChangedEvent: SelectionChangedEvent<TKey>;
    SubmenuHiddenEvent: SubmenuHiddenEvent<TKey>;
    SubmenuHidingEvent: SubmenuHidingEvent<TKey>;
    SubmenuShowingEvent: SubmenuShowingEvent<TKey>;
    SubmenuShownEvent: SubmenuShownEvent<TKey>;
};

export type Properties<TKey = any> = dxMenuOptions<TKey>;

/**
 * @deprecated use Properties instead
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export type Options<TKey = any> = Properties<TKey>;
