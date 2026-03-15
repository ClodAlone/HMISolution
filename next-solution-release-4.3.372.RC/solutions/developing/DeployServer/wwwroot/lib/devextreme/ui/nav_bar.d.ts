/**
* DevExtreme (ui/nav_bar.d.ts)
* Version: 21.2.6
* Build date: Tue Mar 01 2022
*
* Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
* Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
*/
import {
    EventInfo,
    NativeEventInfo,
    InitializedEventInfo,
    ChangedOptionInfo,
    ItemInfo,
} from '../events/index';

import {
    SelectionChangedInfo,
} from './collection/ui.collection_widget.base';

import dxTabs, {
    Item as dxTabsItem,
    dxTabsBaseOptions,
} from './tabs';

/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
type ItemLike = string | Item | any;

export type ContentReadyEvent<TItem extends ItemLike = any, TKey = any> = EventInfo<dxNavBar<TItem, TKey>>;

export type DisposingEvent<TItem extends ItemLike = any, TKey = any> = EventInfo<dxNavBar<TItem, TKey>>;

export type InitializedEvent<TItem extends ItemLike = any, TKey = any> = InitializedEventInfo<dxNavBar<TItem, TKey>>;

export type ItemClickEvent<TItem extends ItemLike = any, TKey = any> = NativeEventInfo<dxNavBar<TItem, TKey>, KeyboardEvent | MouseEvent | PointerEvent> & ItemInfo<TItem>;

export type ItemContextMenuEvent<TItem extends ItemLike = any, TKey = any> = NativeEventInfo<dxNavBar<TItem, TKey>, MouseEvent | PointerEvent | TouchEvent> & ItemInfo<TItem>;

export type ItemHoldEvent<TItem extends ItemLike = any, TKey = any> = NativeEventInfo<dxNavBar<TItem, TKey>, MouseEvent | PointerEvent | TouchEvent> & ItemInfo<TItem>;

export type ItemRenderedEvent<TItem extends ItemLike = any, TKey = any> = EventInfo<dxNavBar<TItem, TKey>> & ItemInfo<TItem>;

export type OptionChangedEvent<TItem extends ItemLike = any, TKey = any> = EventInfo<dxNavBar<TItem, TKey>> & ChangedOptionInfo;

export type SelectionChangedEvent<TItem extends ItemLike = any, TKey = any> = EventInfo<dxNavBar<TItem, TKey>> & SelectionChangedInfo<TItem>;

/**
 * @deprecated use Properties instead
 */
export interface dxNavBarOptions<
    TItem extends ItemLike = any,
    TKey = any,
> extends dxTabsBaseOptions<dxNavBar<TItem, TKey>, TItem, TKey> {
    /**
     * Specifies whether or not an end-user can scroll tabs by swiping.
     */
    scrollByContent?: boolean;
}
/**
 * The NavBar is a UI component that navigates the application views.
 * @deprecated 
 */
export default class dxNavBar<
    TItem extends ItemLike = any,
    TKey = any,
> extends dxTabs<dxNavBarOptions<TItem, TKey>, TItem, TKey> { }

export type Item = dxNavBarItem;

/**
 * @deprecated Use Item instead
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxNavBarItem extends dxTabsItem {
    /**
     * Specifies a badge text for the navbar item.
     */
    badge?: string;
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
    SelectionChangedEvent: SelectionChangedEvent<TItem, TKey>;
};

export type Properties<
    TItem extends ItemLike = any,
    TKey = any,
> = dxNavBarOptions<TItem, TKey>;

/**
 * @deprecated use Properties instead
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export type Options<
    TItem extends ItemLike = any,
    TKey = any,
> = Properties<TItem, TKey>;
