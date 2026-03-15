/**
* DevExtreme (ui/context_menu/ui.menu_base.d.ts)
* Version: 21.2.6
* Build date: Tue Mar 01 2022
*
* Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
* Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
*/
import { Skip } from '../../core';
import { DataSourceLike } from '../../data/data_source';
import {
    AnimationConfig,
} from '../../animation/fx';

import HierarchicalCollectionWidget, {
    HierarchicalCollectionWidgetOptions,
} from '../hierarchical_collection/ui.hierarchical_collection_widget';

import {
    dxMenuBaseItem,
} from '../menu';

/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxMenuBaseOptions<
  TComponent extends dxMenuBase<any, TItem, TKey>,
  TItem extends dxMenuBaseItem = dxMenuBaseItem,
  TKey = any,
> extends Skip<HierarchicalCollectionWidgetOptions<TComponent, TItem, TKey>, 'dataSource'> {
    /**
     * A Boolean value specifying whether or not the UI component changes its state when interacting with a user.
     */
    activeStateEnabled?: boolean;
    /**
     * Configures UI component visibility animations. This object contains two fields: show and hide.
     */
    animation?: {
      /**
       * An object that defines the animation properties used when the UI component is being hidden.
       */
      hide?: AnimationConfig;
      /**
       * An object that defines the animation properties used when the UI component is being shown.
       */
      show?: AnimationConfig;
    };
    /**
     * Specifies the name of the CSS class to be applied to the root menu level and all submenus.
     */
    cssClass?: string;
    /**
     * Binds the UI component to data.
     */
    dataSource?: DataSourceLike<TItem, TKey>;
    /**
     * Holds an array of menu items.
     */
    items?: Array<TItem>;
    /**
     * Specifies whether or not an item becomes selected if a user clicks it.
     */
    selectByClick?: boolean;
    /**
     * Specifies the selection mode supported by the menu.
     */
    selectionMode?: 'none' | 'single';
    /**
     * Specifies properties of submenu showing and hiding.
     */
    showSubmenuMode?: {
      /**
       * Specifies the delay of submenu show and hiding.
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
}
/**
 * The base class for UI components containing an item collection.
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export default class dxMenuBase<
  TProperties extends dxMenuBaseOptions<any, TItem, TKey>,
  TItem extends dxMenuBaseItem = dxMenuBaseItem,
  TKey = any,
> extends HierarchicalCollectionWidget<TProperties, TItem, TKey> {
    /**
     * Selects an item found using its DOM node.
     */
    selectItem(itemElement: Element): void;
    /**
     * Cancels the selection of an item found using its DOM node.
     */
    unselectItem(itemElement: Element): void;
}
