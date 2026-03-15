/**
* DevExtreme (ui/responsive_box.d.ts)
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
} from './collection/ui.collection_widget.base';

/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
type ItemLike = string | Item | any;

export type ContentReadyEvent<TItem extends ItemLike = any, TKey = any> = EventInfo<dxResponsiveBox<TItem, TKey>>;

export type DisposingEvent<TItem extends ItemLike = any, TKey = any> = EventInfo<dxResponsiveBox<TItem, TKey>>;

export type InitializedEvent<TItem extends ItemLike = any, TKey = any> = InitializedEventInfo<dxResponsiveBox<TItem, TKey>>;

export type ItemClickEvent<TItem extends ItemLike = any, TKey = any> = NativeEventInfo<dxResponsiveBox<TItem, TKey>, MouseEvent | PointerEvent> & ItemInfo<TItem>;

export type ItemContextMenuEvent<TItem extends ItemLike = any, TKey = any> = NativeEventInfo<dxResponsiveBox<TItem, TKey>, MouseEvent | PointerEvent | TouchEvent> & ItemInfo<TItem>;

export type ItemHoldEvent<TItem extends ItemLike = any, TKey = any> = NativeEventInfo<dxResponsiveBox<TItem, TKey>, MouseEvent | PointerEvent | TouchEvent> & ItemInfo<TItem>;

export type ItemRenderedEvent<TItem extends ItemLike = any, TKey = any> = EventInfo<dxResponsiveBox<TItem, TKey>> & ItemInfo<TItem>;

export type OptionChangedEvent<TItem extends ItemLike = any, TKey = any> = EventInfo<dxResponsiveBox<TItem, TKey>> & ChangedOptionInfo;

/**
 * @deprecated use Properties instead
 */
export interface dxResponsiveBoxOptions<
  TItem extends ItemLike = any,
  TKey = any,
  > extends CollectionWidgetOptions<dxResponsiveBox<TItem, TKey>, TItem, TKey> {
    /**
     * Specifies the collection of columns for the grid used to position layout elements.
     */
    cols?: Array<{
      /**
       * The column&apos;s base width. Calculated automatically when the singleColumnScreen property arranges all elements in a single column.
       */
      baseSize?: number | 'auto';
      /**
       * The column width ratio.
       */
      ratio?: number;
      /**
       * Decides on which screens the current column is rendered.
       */
      screen?: string;
      /**
       * A factor that defines how much a column width shrinks relative to the rest of the columns in the container.
       */
      shrink?: number;
    }>;
    /**
     * Binds the UI component to data.
     */
    dataSource?: DataSourceLike<TItem, TKey>;
    /**
     * Specifies the UI component&apos;s height.
     */
    height?: number | string | (() => number | string);
    /**
     * An array of items displayed by the UI component.
     */
    items?: Array<TItem>;
    /**
     * Specifies the collection of rows for the grid used to position layout elements.
     */
    rows?: Array<{
      /**
       * The row&apos;s base height. Calculated automatically when the singleColumnScreen property arranges all elements in a single column.
       */
      baseSize?: number | 'auto';
      /**
       * The row height ratio.
       */
      ratio?: number;
      /**
       * Decides on which screens the current row is rendered.
       */
      screen?: string;
      /**
       * A factor that defines how much a row height shrinks relative to the rest of the rows in the container.
       */
      shrink?: number;
    }>;
    /**
     * Specifies the function returning the size qualifier depending on the screen&apos;s width.
     */
    screenByWidth?: Function;
    /**
     * Specifies on which screens all layout elements should be arranged in a single column. Accepts a single or several size qualifiers separated by a space.
     */
    singleColumnScreen?: string;
    /**
     * Specifies the UI component&apos;s width.
     */
    width?: number | string | (() => number | string);
}
/**
 * The ResponsiveBox UI component allows you to create an application or a website with a layout adapted to different screen sizes.
 */
export default class dxResponsiveBox<
  TItem extends ItemLike = any,
  TKey = any,
  > extends CollectionWidget<dxResponsiveBoxOptions<TItem, TKey>, TItem, TKey> { }

export type Item = dxResponsiveBoxItem;

/**
 * @deprecated Use Item instead
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxResponsiveBoxItem extends CollectionWidgetItem {
    /**
     * Specifies the item location and size against the UI component grid.
     */
    location?: {
      /**
       * Specifies which column the element should occupy. Accepts an index from the cols array.
       */
      col?: number;
      /**
       * Specifies how many columns the element should span.
       */
      colspan?: number;
      /**
       * Specifies which row the element should occupy. Accepts an index from the rows array.
       */
      row?: number;
      /**
       * Specifies how many rows the element should span.
       */
      rowspan?: number;
      /**
       * Decides on which screens the current location settings should be applied to the element.
       */
      screen?: string;
    } | Array<{ col?: number; colspan?: number; row?: number; rowspan?: number; screen?: string }>;
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
> = dxResponsiveBoxOptions<TItem, TKey>;

/**
 * @deprecated use Properties instead
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export type Options<
  TItem extends ItemLike = any,
  TKey = any,
> = Properties<TItem, TKey>;
