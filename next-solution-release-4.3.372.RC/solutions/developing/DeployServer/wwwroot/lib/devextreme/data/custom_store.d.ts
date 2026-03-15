/**
* DevExtreme (data/custom_store.d.ts)
* Version: 21.2.6
* Build date: Tue Mar 01 2022
*
* Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
* Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
*/
import { FilterDescriptor, GroupDescriptor, LoadOptions } from './index';
import Store, { Options as StoreOptions } from './abstract_store';
import { DxPromise } from '../core/utils/deferred';

export type Options<
    TItem = any,
    TKey = any,
> = CustomStoreOptions<TItem, TKey>;

export type GroupItem<
    TItem = any,
> = { key: any | string | number; items: Array<TItem> | Array<GroupItem> | null; count?: number; summary?: Array<any> };

export type ResolvedData<
    TItem = any,
> =
  | Object
  | Array<TItem>
  | Array<GroupItem>
  | {
      data: Array<TItem> | Array<GroupItem>;
      totalCount?: number;
      summary?: Array<any>;
      groupCount?: number;
    };

/**
 * @deprecated Use Options instead
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface CustomStoreOptions<
    TItem = any,
    TKey = any,
> extends StoreOptions<TItem, TKey> {
    /**
     * Specifies a custom implementation of the byKey(key) method.
     */
    byKey?: ((key: TKey) => PromiseLike<TItem>);
    /**
     * Specifies whether raw data should be saved in the cache. Applies only if loadMode is &apos;raw&apos;.
     */
    cacheRawData?: boolean;
    /**
     * Specifies a custom implementation of the insert(values) method.
     */
    insert?: ((values: TItem) => PromiseLike<TItem>);
    /**
     * Specifies a custom implementation of the load(options) method.
     */
    load: ((options: LoadOptions<TItem>) =>
      | DxPromise<ResolvedData<TItem>>
      | PromiseLike<ResolvedData<TItem>>
      | Array<GroupItem>
      | Array<TItem>);
    /**
     * Specifies how data returned by the load function is treated.
     */
    loadMode?: 'processed' | 'raw';
    /**
     * Specifies a custom implementation of the remove(key) method.
     */
    remove?: ((key: TKey) => PromiseLike<void>);
    /**
     * Specifies a custom implementation of the totalCount(options) method.
     */
    totalCount?: ((loadOptions: { filter?: FilterDescriptor | Array<FilterDescriptor>; group?: GroupDescriptor<TItem> | Array<GroupDescriptor<TItem>> }) => PromiseLike<number>);
    /**
     * Specifies a custom implementation of the update(key, values) method.
     */
    update?: ((key: TKey, values: TItem) => PromiseLike<any>);
    /**
     * Specifies whether the store combines the search and filter expressions. Defaults to true if the loadMode is &apos;raw&apos; and false if it is &apos;processed&apos;.
     */
    useDefaultSearch?: boolean;
}
/**
 * The CustomStore enables you to implement custom data access logic for consuming data from any source.
 */
export default class CustomStore<
    TItem = any,
    TKey = any,
> extends Store<TItem, TKey> {
    constructor(options?: Options<TItem, TKey>)
    /**
     * Deletes data from the cache. Takes effect only if the cacheRawData property is true.
     */
    clearRawDataCache(): void;
}
