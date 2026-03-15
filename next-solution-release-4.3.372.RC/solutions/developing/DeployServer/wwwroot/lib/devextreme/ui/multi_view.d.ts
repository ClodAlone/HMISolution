/**
* DevExtreme (ui/multi_view.d.ts)
* Version: 21.2.6
* Build date: Tue Mar 01 2022
*
* Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
* Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
*/
import { DataSourceLike } from '../data/data_source';

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
export type ItemLike = string | Item | any;

export type ContentReadyEvent<TItem extends ItemLike = any, TKey = any> = EventInfo<MultiViewInstance<TItem, TKey>>;

export type DisposingEvent<TItem extends ItemLike = any, TKey = any> = EventInfo<MultiViewInstance<TItem, TKey>>;

export type InitializedEvent<TItem extends ItemLike = any, TKey = any> = InitializedEventInfo<MultiViewInstance<TItem, TKey>>;

export type ItemClickEvent<TItem extends ItemLike = any, TKey = any> = NativeEventInfo<MultiViewInstance<TItem, TKey>, KeyboardEvent | MouseEvent | PointerEvent> & ItemInfo<TItem>;

export type ItemContextMenuEvent<TItem extends ItemLike = any, TKey = any> = NativeEventInfo<MultiViewInstance<TItem, TKey>, MouseEvent | PointerEvent | TouchEvent> & ItemInfo<TItem>;

export type ItemHoldEvent<TItem extends ItemLike = any, TKey = any> = NativeEventInfo<MultiViewInstance<TItem, TKey>, MouseEvent | PointerEvent | TouchEvent> & ItemInfo<TItem>;

export type ItemRenderedEvent<TItem extends ItemLike = any, TKey = any> = EventInfo<MultiViewInstance<TItem, TKey>> & ItemInfo<TItem>;

export type OptionChangedEvent<TItem extends ItemLike = any, TKey = any> = EventInfo<MultiViewInstance<TItem, TKey>> & ChangedOptionInfo;

export type SelectionChangedEvent<TItem extends ItemLike = any, TKey = any> = EventInfo<MultiViewInstance<TItem, TKey>> & SelectionChangedInfo<TItem>;

/**
 * @deprecated use Properties instead
 */
export interface dxMultiViewOptions<
    TItem extends ItemLike = any,
    TKey = any,
> extends Properties<TItem, TKey> {}

/**
 * @deprecated use Properties instead
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxMultiViewBaseOptions<
    TComponent extends dxMultiView<any, TItem, TKey> = dxMultiView<any, any, any>,
    TItem extends ItemLike = any,
    TKey = any,
> extends CollectionWidgetOptions<TComponent, TItem, TKey> {
    /**
     * Specifies whether or not to animate the displayed item change.
     */
    animationEnabled?: boolean;
    /**
     * Binds the UI component to data.
     */
    dataSource?: DataSourceLike<TItem, TKey>;
    /**
     * Specifies whether to render the view&apos;s content when it is displayed. If false, the content is rendered immediately.
     */
    deferRendering?: boolean;
    /**
     * Specifies whether the UI component can be focused using keyboard navigation.
     */
    focusStateEnabled?: boolean;
    /**
     * An array of items displayed by the UI component.
     */
    items?: Array<TItem>;
    /**
     * A Boolean value specifying whether or not to scroll back to the first item after the last item is swiped.
     */
    loop?: boolean;
    /**
     * The index of the currently displayed item.
     */
    selectedIndex?: number;
    /**
     * A Boolean value specifying whether or not to allow users to change the selected index by swiping.
     */
    swipeEnabled?: boolean;
}

/**
 * The MultiView is a UI component that contains several views. An end user navigates through the views by swiping them in the horizontal direction.
 */
export default class dxMultiView<
    TProperties extends dxMultiViewOptions<TItem, TKey> = dxMultiViewOptions<any, any>,
    TItem extends ItemLike = any,
    TKey = any,
> extends CollectionWidget<TProperties, TItem, TKey> { }

export type Item = dxMultiViewItem;

 /**
 * @deprecated Use Item instead
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxMultiViewItem extends CollectionWidgetItem {}

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

/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
interface MultiViewInstance<TItem, TKey> extends dxMultiView<Properties<TItem, TKey>, TItem, TKey> { }

export type Properties<
    TItem extends ItemLike = any,
    TKey = any,
> = dxMultiViewBaseOptions<MultiViewInstance<TItem, TKey>, TItem, TKey>;

/**
 * @deprecated use Properties instead
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export type Options<
    TItem extends ItemLike = any,
    TKey = any,
> = Properties<TItem, TKey>;
