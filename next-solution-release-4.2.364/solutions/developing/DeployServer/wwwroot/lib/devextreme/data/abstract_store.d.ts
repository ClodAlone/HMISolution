/**
* DevExtreme (data/abstract_store.d.ts)
* Version: 21.2.6
* Build date: Tue Mar 01 2022
*
* Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
* Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
*/
import { DxPromise, DxExtendedPromise } from '../core/utils/deferred';
import { DeepPartial } from '../core/index';
import { FilterDescriptor, GroupDescriptor, LoadOptions } from './index';

/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export type Options<
    TItem = any,
    TKey = any,
> = StoreOptions<TItem, TKey>;

/**
 * @deprecated Use Options instead
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface StoreOptions<
    TItem = any,
    TKey = any,
> {
    /**
     * Specifies the function that is executed when the store throws an error.
     */
    errorHandler?: Function;
    /**
     * Specifies the key property (or properties) that provide(s) key values to access data items. Each key value must be unique.
     */
    key?: string | Array<string>;
    /**
     * A function that is executed after a data item is added to the store.
     */
    onInserted?: ((values: TItem, key: TKey) => void);
    /**
     * A function that is executed before a data item is added to the store.
     */
    onInserting?: ((values: TItem) => void);
    /**
     * A function that is executed after data is loaded to the store.
     */
    onLoaded?: ((result: Array<TItem>, loadOptions: LoadOptions<TItem>) => void);
    /**
     * A function that is executed before data is loaded to the store.
     */
    onLoading?: ((loadOptions: LoadOptions<TItem>) => void);
    /**
     * A function that is executed after a data item is added, updated, or removed from the store.
     */
    onModified?: Function;
    /**
     * A function that is executed before a data item is added, updated, or removed from the store.
     */
    onModifying?: Function;
    /**
     * The function executed before changes are pushed to the store.
     */
    onPush?: ((changes: Array<TItem>) => void);
    /**
     * A function that is executed after a data item is removed from the store.
     */
    onRemoved?: ((key: TKey) => void);
    /**
     * A function that is executed before a data item is removed from the store.
     */
    onRemoving?: ((key: TKey) => void);
    /**
     * A function that is executed after a data item is updated in the store.
     */
    onUpdated?: ((key: TKey, values: TItem) => void);
    /**
     * A function that is executed before a data item is updated in the store.
     */
    onUpdating?: ((key: TKey, values: TItem) => void);
}

/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
type EventName = 'loaded' | 'loading' | 'inserted' | 'inserting' | 'updated' | 'updating' | 'push' | 'removed' | 'removing' | 'modified' | 'modifying';

/**
 * The base class for all Stores.
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export default class Store<
    TItem = any,
    TKey = any,
> {
    constructor(options?: Options<TItem, TKey>)
    /**
     * Gets a data item with a specific key.
     */
    byKey(key: TKey, extraOptions?: LoadOptions<TItem>): DxPromise<TItem>;
    /**
     * Adds a data item to the store.
     */
    insert(values: TItem): DxExtendedPromise<TItem>;
    /**
     * Gets the key property (or properties) as specified in the key property.
     */
    key(): string | Array<string>;
    /**
     * Gets a data item&apos;s key value.
     */
    keyOf(obj: TItem): TKey;
    /**
     * Starts loading data.
     */
    load(): DxExtendedPromise<Array<TItem>>;
    /**
     * Starts loading data.
     */
    load(options: LoadOptions<TItem>): DxExtendedPromise<Array<TItem>>;
    /**
     * Detaches all event handlers from a single event.
     */
    off(eventName: EventName): this;
    /**
     * Detaches a particular event handler from a single event.
     */
    off(eventName: EventName, eventHandler: Function): this;
    /**
     * Subscribes to an event.
     */
    on(eventName: EventName, eventHandler: Function): this;
    /**
     * Subscribes to events.
     */
    on(events: { [key in EventName]?: Function }): this;
    /**
     * Pushes data changes to the store and notifies the DataSource.
     */
    push(changes: Array<{ type: 'insert' | 'update' | 'remove'; data?: DeepPartial<TItem>; key?: TKey; index?: number }>): void;
    /**
     * Removes a data item with a specific key from the store.
     */
    remove(key: TKey): DxPromise<void>;
    /**
     * Gets the total count of items the load() function returns.
     */
    totalCount(obj: { filter?: FilterDescriptor | Array<FilterDescriptor>; group?: GroupDescriptor<TItem> | Array<GroupDescriptor<TItem>> }): DxPromise<number>;
    /**
     * Updates a data item with a specific key.
     */
    update(key: TKey, values: DeepPartial<TItem>): DxExtendedPromise<TItem>;
}
