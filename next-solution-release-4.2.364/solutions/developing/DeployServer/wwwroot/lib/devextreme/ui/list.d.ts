/**
* DevExtreme (ui/list.d.ts)
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

import {
    dxSortableOptions,
} from './sortable';

import {
    SearchBoxMixinOptions,
} from './widget/ui.search_box_mixin';

/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
type ItemLike = string | Item | any;

/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
interface ListItemInfo<TItem extends ItemLike> {
    readonly itemData?: TItem;
    readonly itemElement: DxElement;
    readonly itemIndex: number | { group: number; item: number };
}

/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface ScrollInfo {
    readonly scrollOffset?: any;
    readonly reachedLeft: boolean;
    readonly reachedRight: boolean;
    readonly reachedTop: boolean;
    readonly reachedBottom: boolean;
}

export type ContentReadyEvent<TItem extends ItemLike = any, TKey = any> = EventInfo<dxList<TItem, TKey>>;

export type DisposingEvent<TItem extends ItemLike = any, TKey = any> = EventInfo<dxList<TItem, TKey>>;

export type GroupRenderedEvent<TItem extends ItemLike = any, TKey = any> = EventInfo<dxList<TItem, TKey>> & {
    readonly groupData?: any;
    readonly groupElement?: DxElement;
    readonly groupIndex?: number;
};

export type InitializedEvent<TItem extends ItemLike = any, TKey = any> = InitializedEventInfo<dxList<TItem, TKey>>;

export type ItemClickEvent<TItem extends ItemLike = any, TKey = any> = NativeEventInfo<dxList<TItem, TKey>, KeyboardEvent | MouseEvent | PointerEvent> & ListItemInfo<TItem>;

export type ItemContextMenuEvent<TItem extends ItemLike = any, TKey = any> = NativeEventInfo<dxList<TItem, TKey>, MouseEvent | PointerEvent | TouchEvent> & ListItemInfo<TItem>;

export type ItemDeletedEvent<TItem extends ItemLike = any, TKey = any> = EventInfo<dxList<TItem, TKey>> & ListItemInfo<TItem>;

export type ItemDeletingEvent<TItem extends ItemLike = any, TKey = any> = EventInfo<dxList<TItem, TKey>> & ListItemInfo<TItem> & {
    cancel?: boolean | PromiseLike<void>;
};

export type ItemHoldEvent<TItem extends ItemLike = any, TKey = any> = NativeEventInfo<dxList<TItem, TKey>, MouseEvent | PointerEvent | TouchEvent> & ListItemInfo<TItem>;

export type ItemRenderedEvent<TItem extends Item | any = any, TKey = any> = EventInfo<dxList<TItem, TKey>> & ItemInfo<TItem>;

export type ItemReorderedEvent<TItem extends ItemLike = any, TKey = any> = EventInfo<dxList<TItem, TKey>> & ListItemInfo<TItem> & {
    readonly fromIndex: number;
    readonly toIndex: number;
};

export type ItemSwipeEvent<TItem extends ItemLike = any, TKey = any> = NativeEventInfo<dxList<TItem, TKey>, MouseEvent | PointerEvent | TouchEvent> & ListItemInfo<TItem> & {
    readonly direction: string;
};

export type OptionChangedEvent<TItem extends ItemLike = any, TKey = any> = EventInfo<dxList<TItem, TKey>> & ChangedOptionInfo;

export type PageLoadingEvent<TItem extends ItemLike = any, TKey = any> = EventInfo<dxList<TItem, TKey>>;

export type PullRefreshEvent<TItem extends ItemLike = any, TKey = any> = EventInfo<dxList<TItem, TKey>>;

export type ScrollEvent<TItem extends ItemLike = any, TKey = any> = NativeEventInfo<dxList<TItem, TKey>, Event> & ScrollInfo;

export type SelectAllValueChangedEvent<TItem extends ItemLike = any, TKey = any> = EventInfo<dxList<TItem, TKey>> & {
    readonly value: boolean;
};

export type SelectionChangedEvent<TItem extends ItemLike = any, TKey = any> = EventInfo<dxList<TItem, TKey>> & SelectionChangedInfo<TItem>;

/**
 * @deprecated use Properties instead
 */
export interface dxListOptions<
    TItem extends ItemLike = any,
    TKey = any,
> extends CollectionWidgetOptions<dxList<TItem, TKey>, TItem, TKey>, SearchBoxMixinOptions {
    /**
     * Specifies whether or not the UI component changes its state when interacting with a user.
     */
    activeStateEnabled?: boolean;
    /**
     * Specifies whether or not an end user can delete list items.
     */
    allowItemDeleting?: boolean;
    /**
     * A Boolean value specifying whether to enable or disable the bounce-back effect.
     */
    bounceEnabled?: boolean;
    /**
     * Specifies whether or not an end-user can collapse groups.
     */
    collapsibleGroups?: boolean;
    /**
     * Binds the UI component to data.
     */
    dataSource?: DataSourceLike<TItem, TKey>;
    /**
     * Specifies the data field whose values should be displayed. Defaults to &apos;text&apos; when the data source contains objects.
     */
    displayExpr?: string | ((item: TItem) => string);
    /**
     * Specifies whether the UI component can be focused using keyboard navigation.
     */
    focusStateEnabled?: boolean;
    /**
     * Specifies a custom template for group captions.
     */
    groupTemplate?: template | ((groupData: any, groupIndex: number, groupElement: DxElement) => string | UserDefinedElement);
    /**
     * Specifies whether data items should be grouped.
     */
    grouped?: boolean;
    /**
     * Specifies whether the UI component changes its state when a user pauses on it.
     */
    hoverStateEnabled?: boolean;
    /**
     * Specifies whether or not to show the loading panel when the DataSource bound to the UI component is loading data.
     */
    indicateLoading?: boolean;
    /**
     * Specifies the way a user can delete items from the list.
     */
    itemDeleteMode?: 'context' | 'slideButton' | 'slideItem' | 'static' | 'swipe' | 'toggle';
    /**
     * Configures item reordering using drag and drop gestures.
     */
    itemDragging?: dxSortableOptions;
    /**
     * An array of items displayed by the UI component.
     */
    items?: Array<TItem>;
    /**
     * Specifies the array of items for a context menu called for a list item.
     */
    menuItems?: Array<{
      /**
       * Holds on a function called when the item is clicked.
       */
      action?: ((itemElement: DxElement, itemData: TItem) => any);
      /**
       * Specifies the menu item text.
       */
      text?: string;
    }>;
    /**
     * Specifies whether an item context menu is shown when a user holds or swipes an item.
     */
    menuMode?: 'context' | 'slide';
    /**
     * The text displayed on the button used to load the next page from the data source.
     */
    nextButtonText?: string;
    /**
     * A function that is executed when a group element is rendered.
     */
    onGroupRendered?: ((e: GroupRenderedEvent<TItem, TKey>) => void);
    /**
     * A function that is executed when a collection item is clicked or tapped.
     */
    onItemClick?: ((e: ItemClickEvent<TItem, TKey>) => void) | string;
    /**
     * A function that is executed when a collection item is right-clicked or pressed.
     */
    onItemContextMenu?: ((e: ItemContextMenuEvent<TItem, TKey>) => void);
    /**
     * A function that is executed after a list item is deleted from the data source.
     */
    onItemDeleted?: ((e: ItemDeletedEvent<TItem, TKey>) => void);
    /**
     * A function that is executed before a collection item is deleted from the data source.
     */
    onItemDeleting?: ((e: ItemDeletingEvent<TItem, TKey>) => void);
    /**
     * A function that is executed when a collection item has been held for a specified period.
     */
    onItemHold?: ((e: ItemHoldEvent<TItem, TKey>) => void);
    /**
     * A function that is executed after a list item is moved to another position.
     */
    onItemReordered?: ((e: ItemReorderedEvent<TItem, TKey>) => void);
    /**
     * A function that is executed when a list item is swiped.
     */
    onItemSwipe?: ((e: ItemSwipeEvent<TItem, TKey>) => void);
    /**
     * A function that is executed before the next page is loaded.
     */
    onPageLoading?: ((e: PageLoadingEvent<TItem, TKey>) => void);
    /**
     * A function that is executed when the &apos;pull to refresh&apos; gesture is performed. Supported on mobile devices only.
     */
    onPullRefresh?: ((e: PullRefreshEvent<TItem, TKey>) => void);
    /**
     * A function that is executed on each scroll gesture.
     */
    onScroll?: ((e: ScrollEvent<TItem, TKey>) => void);
    /**
     * A function that is executed when the &apos;Select All&apos; check box value is changed. Applies only if the selectionMode is &apos;all&apos;.
     */
    onSelectAllValueChanged?: ((e: SelectAllValueChangedEvent<TItem, TKey>) => void);
    /**
     * Specifies whether the next page is loaded when a user scrolls the UI component to the bottom or when the &apos;next&apos; button is clicked.
     */
    pageLoadMode?: 'nextButton' | 'scrollBottom';
    /**
     * Specifies the text shown in the pullDown panel, which is displayed when the list is scrolled to the bottom.
     */
    pageLoadingText?: string;
    /**
     * A Boolean value specifying whether or not the UI component supports the &apos;pull down to refresh&apos; gesture.
     */
    pullRefreshEnabled?: boolean;
    /**
     * Specifies the text displayed in the pullDown panel when the list is pulled below the refresh threshold.
     */
    pulledDownText?: string;
    /**
     * Specifies the text shown in the pullDown panel while the list is being pulled down to the refresh threshold.
     */
    pullingDownText?: string;
    /**
     * Specifies the text displayed in the pullDown panel while the list is being refreshed.
     */
    refreshingText?: string;
    /**
     * Specifies whether to repaint only those elements whose data changed.
     */
    repaintChangesOnly?: boolean;
    /**
     * A Boolean value specifying if the list is scrolled by content.
     */
    scrollByContent?: boolean;
    /**
     * Specifies whether a user can scroll the content with the scrollbar. Applies only if useNativeScrolling is false.
     */
    scrollByThumb?: boolean;
    /**
     * A Boolean value specifying whether to enable or disable list scrolling.
     */
    scrollingEnabled?: boolean;
    /**
     * Specifies the mode in which all items are selected.
     */
    selectAllMode?: 'allPages' | 'page';
    /**
     * Specifies item selection mode.
     */
    selectionMode?: 'all' | 'multiple' | 'none' | 'single';
    /**
     * Specifies when the UI component shows the scrollbar.
     */
    showScrollbar?: 'always' | 'never' | 'onHover' | 'onScroll';
    /**
     * Specifies whether or not to display controls used to select list items.
     */
    showSelectionControls?: boolean;
    /**
     * Specifies the text displayed at the &apos;Select All&apos; check box.
     */
    selectAllText?: string;
    /**
     * Specifies whether or not the UI component uses native scrolling.
     */
    useNativeScrolling?: boolean;
}
/**
 * The List is a UI component that represents a collection of items in a scrollable list.
 */
export default class dxList<
    TItem extends ItemLike = any,
    TKey = any,
> extends CollectionWidget<dxListOptions<TItem, TKey>, TItem, TKey> {
    /**
     * Gets the UI component&apos;s height in pixels.
     */
    clientHeight(): number;
    /**
     * Collapses a group with a specific index.
     */
    collapseGroup(groupIndex: number): DxPromise<void>;
    /**
     * Removes an item found using its DOM node.
     */
    deleteItem(itemElement: Element): DxPromise<void>;
    /**
     * Removes an item with a specific index.
     */
    deleteItem(itemIndex: number | any): DxPromise<void>;
    /**
     * Expands a group with a specific index.
     */
    expandGroup(groupIndex: number): DxPromise<void>;
    /**
     * Checks whether an item found using its DOM node is selected.
     */
    isItemSelected(itemElement: Element): boolean;
    /**
     * Checks whether an item with a specific index is selected.
     */
    isItemSelected(itemIndex: number | any): boolean;
    /**
     * Reloads list data.
     */
    reload(): void;
    /**
     * Reorders items found using their DOM nodes.
     */
    reorderItem(itemElement: Element, toItemElement: Element): DxPromise<void>;
    /**
     * Reorders items with specific indexes.
     */
    reorderItem(itemIndex: number | any, toItemIndex: number | any): DxPromise<void>;
    /**
     * Scrolls the content by a specified distance.
     */
    scrollBy(distance: number): void;
    /**
     * Gets the content&apos;s height in pixels.
     */
    scrollHeight(): number;
    /**
     * Scrolls the content to a specific position.
     */
    scrollTo(location: number): void;
    /**
     * Scrolls the content to an item found using its DOM node.
     */
    scrollToItem(itemElement: Element): void;
    /**
     * Scrolls the content to an item with a specific index.
     */
    scrollToItem(itemIndex: number | any): void;
    /**
     * Gets the top scroll offset.
     */
    scrollTop(): number;
    /**
     * Selects all items.
     */
    selectAll(): void;
    /**
     * Selects an item found using its DOM node.
     */
    selectItem(itemElement: Element): void;
    /**
     * Selects an item with a specific index.
     */
    selectItem(itemIndex: number | any): void;
    /**
     * Cancels the selection of all items.
     */
    unselectAll(): void;
    /**
     * Cancels the selection of an item found using its DOM node.
     */
    unselectItem(itemElement: Element): void;
    /**
     * Cancels the selection of an item with a specific index.
     */
    unselectItem(itemIndex: number | any): void;
    /**
     * Updates the UI component scrollbar according to UI component content size.
     */
    updateDimensions(): DxPromise<void>;
}

export type Item = dxListItem;

/**
 * @deprecated Use Item instead
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxListItem extends CollectionWidgetItem {
    /**
     * Specifies the text of a badge displayed for the list item.
     */
    badge?: string;
    /**
     * Specifies the list item&apos;s icon.
     */
    icon?: string;
    /**
     * Specifies the name of the list items group in a grouped list.
     */
    key?: string;
    /**
     * Specifies whether or not to display a chevron for the list item.
     */
    showChevron?: boolean;
}

export type ExplicitTypes<
    TItem extends ItemLike,
    TKey,
> = {
    Properties: Properties<TItem, TKey>;
    ContentReadyEvent: ContentReadyEvent<TItem, TKey>;
    DisposingEvent: DisposingEvent<TItem, TKey>;
    GroupRenderedEvent: GroupRenderedEvent<TItem, TKey>;
    InitializedEvent: InitializedEvent<TItem, TKey>;
    ItemClickEvent: ItemClickEvent<TItem, TKey>;
    ItemContextMenuEvent: ItemContextMenuEvent<TItem, TKey>;
    ItemDeletedEvent: ItemDeletedEvent<TItem, TKey>;
    ItemDeletingEvent: ItemDeletingEvent<TItem, TKey>;
    ItemHoldEvent: ItemHoldEvent<TItem, TKey>;
    ItemRenderedEvent: ItemRenderedEvent<TItem, TKey>;
    ItemReorderedEvent: ItemReorderedEvent<TItem, TKey>;
    ItemSwipeEvent: ItemSwipeEvent<TItem, TKey>;
    OptionChangedEvent: OptionChangedEvent<TItem, TKey>;
    PageLoadingEvent: PageLoadingEvent<TItem, TKey>;
    PullRefreshEvent: PullRefreshEvent<TItem, TKey>;
    ScrollEvent: ScrollEvent<TItem, TKey>;
    SelectAllValueChangedEvent: SelectAllValueChangedEvent<TItem, TKey>;
    SelectionChangedEvent: SelectionChangedEvent<TItem, TKey>;
};

export type Properties<
    TItem extends ItemLike = any,
    TKey = any,
> = dxListOptions<TItem, TKey>;

/**
 * @deprecated use Properties instead
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export type Options<
    TItem extends ItemLike = any,
    TKey = any,
> = Properties<TItem, TKey>;
