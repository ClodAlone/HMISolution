/**
* DevExtreme (ui/toolbar.d.ts)
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
    EventInfo,
    NativeEventInfo,
    InitializedEventInfo,
    ChangedOptionInfo,
    ItemInfo,
} from '../events/index';

import CollectionWidget, {
    CollectionWidgetItem,
    CollectionWidgetOptions,
} from './collection/ui.collection_widget.base';

/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
type ItemLike = string | Item | any;

export type ContentReadyEvent<TItem extends ItemLike = any, TKey = any> = EventInfo<dxToolbar<TItem, TKey>>;

export type DisposingEvent<TItem extends ItemLike = any, TKey = any> = EventInfo<dxToolbar<TItem, TKey>>;

export type InitializedEvent<TItem extends ItemLike = any, TKey = any> = InitializedEventInfo<dxToolbar<TItem, TKey>>;

export type ItemClickEvent<TItem extends ItemLike = any, TKey = any> = NativeEventInfo<dxToolbar<TItem, TKey>, MouseEvent | PointerEvent> & ItemInfo<TItem>;

export type ItemContextMenuEvent<TItem extends ItemLike = any, TKey = any> = NativeEventInfo<dxToolbar<TItem, TKey>, MouseEvent | PointerEvent | TouchEvent> & ItemInfo<TItem>;

export type ItemHoldEvent<TItem extends ItemLike = any, TKey = any> = NativeEventInfo<dxToolbar<TItem, TKey>, MouseEvent | PointerEvent | TouchEvent> & ItemInfo<TItem>;

export type ItemRenderedEvent<TItem extends ItemLike = any, TKey = any> = EventInfo<dxToolbar<TItem, TKey>> & ItemInfo<TItem>;

export type OptionChangedEvent<TItem extends ItemLike = any, TKey = any> = EventInfo<dxToolbar<TItem, TKey>> & ChangedOptionInfo;

/**
 * @deprecated use Properties instead
 */
export interface dxToolbarOptions<
    TItem extends ItemLike = any,
    TKey = any,
> extends CollectionWidgetOptions<dxToolbar<TItem, TKey>, TItem, TKey> {
    /**
     * Binds the UI component to data.
     */
    dataSource?: DataSourceLike<TItem, TKey>;
    /**
     * An array of items displayed by the UI component.
     */
    items?: Array<TItem>;
    /**
     * Specifies a custom template for menu items.
     */
    menuItemTemplate?: template | ((itemData: TItem, itemIndex: number, itemElement: DxElement) => string | UserDefinedElement);
    /**
     * Specifies the UI component&apos;s height in pixels.
     * @deprecated 
     */
    height?: number | string | (() => number | string);
}
/**
 * The Toolbar is a UI component containing items that usually manage screen content. Those items can be plain text or UI components.
 */
export default class dxToolbar<
    TItem extends ItemLike = any,
    TKey = any,
> extends CollectionWidget<dxToolbarOptions<TItem, TKey>, TItem, TKey> { }

export type Item = dxToolbarItem;

/**
 * @deprecated Use Item instead
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxToolbarItem extends CollectionWidgetItem {
    /**
     * Specifies a CSS class to be applied to the item.
     */
    cssClass?: string;
    /**
     * Specifies when to display an item in the toolbar&apos;s overflow menu.
     */
    locateInMenu?: 'always' | 'auto' | 'never';
    /**
     * Specifies a location for the item on the toolbar.
     */
    location?: 'after' | 'before' | 'center';
    /**
     * Specifies a template that should be used to render a menu item.
     */
    menuItemTemplate?: template | (() => string | UserDefinedElement);
    /**
     * Configures the DevExtreme UI component used as a toolbar item.
     */
    options?: any;
    /**
     * Specifies when to display the text for the UI component item.
     */
    showText?: 'always' | 'inMenu';
    /**
     * A UI component that presents a toolbar item. To configure it, use the options object.
     */
    widget?: 'dxAutocomplete' | 'dxButton' | 'dxCheckBox' | 'dxDateBox' | 'dxMenu' | 'dxSelectBox' | 'dxTabs' | 'dxTextBox' | 'dxButtonGroup' | 'dxDropDownButton';
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
    OptionChangedEvent: OptionChangedEvent<TItem, TKey>;
};

export type Properties<
    TItem extends ItemLike = any,
    TKey = any,
> = dxToolbarOptions<TItem, TKey>;

/**
 * @deprecated use Properties instead
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export type Options<
    TItem extends ItemLike = any,
    TKey = any,
> = Properties<TItem, TKey>;
