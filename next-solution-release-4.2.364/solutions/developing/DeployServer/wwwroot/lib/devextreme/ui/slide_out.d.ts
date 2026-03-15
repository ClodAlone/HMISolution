/**
* DevExtreme (ui/slide_out.d.ts)
* Version: 21.2.6
* Build date: Tue Mar 01 2022
*
* Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
* Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
*/
import { DataSourceLike } from '../data/data_source';
import {
    UserDefinedElement,
    DxElement,
} from '../core/element';

import {
    template,
} from '../core/templates/template';

import {
    DxPromise,
} from '../core/utils/deferred';

import {
    EventInfo,
    NativeEventInfo,
    InitializedEventInfo,
    ChangedOptionInfo,
    ItemInfo,
} from '../events/index';

import CollectionWidget, {
    CollectionWidgetItem,
    CollectionWidgetOptions,
    SelectionChangedInfo,
} from './collection/ui.collection_widget.base';

/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
type ItemLike = string | Item | any;

export type ContentReadyEvent<TItem extends ItemLike = any, TKey = any> = EventInfo<dxSlideOut<TItem, TKey>>;

export type DisposingEvent<TItem extends ItemLike = any, TKey = any> = EventInfo<dxSlideOut<TItem, TKey>>;

export type InitializedEvent<TItem extends ItemLike = any, TKey = any> = InitializedEventInfo<dxSlideOut<TItem, TKey>>;

export type ItemClickEvent<TItem extends ItemLike = any, TKey = any> = NativeEventInfo<dxSlideOut<TItem, TKey>, MouseEvent | PointerEvent> & ItemInfo<TItem>;

export type ItemContextMenuEvent<TItem extends ItemLike = any, TKey = any> = NativeEventInfo<dxSlideOut<TItem, TKey>, MouseEvent | PointerEvent | TouchEvent> & ItemInfo<TItem>;

export type ItemHoldEvent<TItem extends ItemLike = any, TKey = any> = NativeEventInfo<dxSlideOut<TItem, TKey>, MouseEvent | PointerEvent | TouchEvent> & ItemInfo<TItem>;

export type ItemRenderedEvent<TItem extends ItemLike = any, TKey = any> = EventInfo<dxSlideOut<TItem, TKey>> & ItemInfo<TItem>;

export type MenuGroupRenderedEvent<TItem extends ItemLike = any, TKey = any> = EventInfo<dxSlideOut<TItem, TKey>>;

export type MenuItemRenderedEvent<TItem extends ItemLike = any, TKey = any> = EventInfo<dxSlideOut<TItem, TKey>>;

export type OptionChangedEvent<TItem extends ItemLike = any, TKey = any> = EventInfo<dxSlideOut<TItem, TKey>> & ChangedOptionInfo;

export type SelectionChangedEvent<TItem extends ItemLike = any, TKey = any> = EventInfo<dxSlideOut<TItem, TKey>> & SelectionChangedInfo<TItem>;

/**
 * @deprecated use Properties instead
 */
export interface dxSlideOutOptions<
    TItem extends ItemLike = any,
    TKey = any,
> extends CollectionWidgetOptions<dxSlideOut<TItem, TKey>, TItem, TKey> {
    /**
     * A Boolean value specifying whether or not the UI component changes its state when interacting with a user.
     */
    activeStateEnabled?: boolean;
    /**
     * Specifies a custom template for the UI component content. Rendered only once - when the UI component is created.
     */
    contentTemplate?: template | ((container: DxElement) => string | UserDefinedElement);
    /**
     * Binds the UI component to data.
     */
    dataSource?: DataSourceLike<TItem, TKey>;
    /**
     * An array of items displayed by the UI component.
     */
    items?: Array<TItem>;
    /**
     * Specifies a custom template for group captions.
     */
    menuGroupTemplate?: template | ((groupData: any, groupIndex: number, groupElement: any) => string | UserDefinedElement);
    /**
     * A Boolean value specifying whether or not to display a grouped menu.
     */
    menuGrouped?: boolean;
    /**
     * Specifies a custom template for menu items.
     */
    menuItemTemplate?: template | ((itemData: TItem, itemIndex: number, itemElement: DxElement) => string | UserDefinedElement);
    /**
     * Specifies the current menu position.
     */
    menuPosition?: 'inverted' | 'normal';
    /**
     * Specifies whether or not the slide-out menu is displayed.
     */
    menuVisible?: boolean;
    /**
     * A function that is executed when a group menu item is rendered.
     */
    onMenuGroupRendered?: ((e: MenuGroupRenderedEvent<TItem, TKey>) => void);
    /**
     * A function that is executed when a regular menu item is rendered.
     */
    onMenuItemRendered?: ((e: MenuItemRenderedEvent<TItem, TKey>) => void);
    /**
     * The index number of the currently selected item.
     */
    selectedIndex?: number;
    /**
     * Indicates whether the menu can be shown/hidden by swiping the UI component&apos;s main panel.
     */
    swipeEnabled?: boolean;
}
/**
 * The SlideOut UI component is a classic slide-out menu paired with a view. An end user opens the menu by swiping away the view.
 * @deprecated 
 */
export default class dxSlideOut<
    TItem extends ItemLike = any,
    TKey = any,
> extends CollectionWidget<dxSlideOutOptions<TItem, TKey>, TItem, TKey> {
    /**
     * Hides the UI component&apos;s slide-out menu.
     */
    hideMenu(): DxPromise<void>;
    /**
     * Displays the UI component&apos;s slide-out menu.
     */
    showMenu(): DxPromise<void>;
    /**
     * Shows or hides the slide-out menu depending on the argument.
     */
    toggleMenuVisibility(showing?: boolean): DxPromise<void>;
}

export type Item = dxSlideOutItem;

/**
 * @deprecated Use Item instead
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxSlideOutItem extends CollectionWidgetItem {
    /**
     * Specifies a template that should be used to render a menu item.
     */
    menuTemplate?: template | (() => string | UserDefinedElement);
}

export type ExplicitTypes<
    TItem extends ItemLike,
    TKey,
> = {
    Properties: Properties<TItem, TKey>;
    ContentReadyEvent: ContentReadyEvent<TItem, TKey>;
    DisposingEvent: DisposingEvent<TItem, TKey>;
    InitializedEvent: InitializedEvent<TItem, TKey>;
    ItemClickEvent: ItemClickEvent<TItem, TKey>;
    ItemContextMenuEvent: ItemContextMenuEvent<TItem, TKey>;
    ItemHoldEvent: ItemHoldEvent<TItem, TKey>;
    ItemRenderedEvent: ItemRenderedEvent<TItem, TKey>;
    MenuGroupRenderedEvent: MenuGroupRenderedEvent<TItem, TKey>;
    MenuItemRenderedEvent: MenuItemRenderedEvent<TItem, TKey>;
    OptionChangedEvent: OptionChangedEvent<TItem, TKey>;
    SelectionChangedEvent: SelectionChangedEvent<TItem, TKey>;
};

export type Properties<
    TItem extends ItemLike = any,
    TKey = any,
> = dxSlideOutOptions<TItem, TKey>;

/**
 * @deprecated use Properties instead
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export type Options<
    TItem extends ItemLike = any,
    TKey = any,
> = Properties<TItem, TKey>;
