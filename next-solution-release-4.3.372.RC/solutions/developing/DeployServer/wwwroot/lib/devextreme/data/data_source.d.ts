/**
* DevExtreme (data/data_source.d.ts)
* Version: 21.2.6
* Build date: Tue Mar 01 2022
*
* Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
* Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
*/
import {
 FilterDescriptor,
 GroupDescriptor,
 LoadOptions,
 SearchOperation,
 SelectDescriptor,
 SortDescriptor,
 Store,
 StoreOptions,
} from './index';
import { DxExtendedPromise } from '../core/utils/deferred';
import { Options as CustomStoreOptions } from './custom_store';

export type Options<
    TStoreItem = any,
    TMappedItem = TStoreItem,
    TItem = TMappedItem,
    TKey = any,
> = DataSourceOptions<TStoreItem, TItem, TMappedItem, TKey>;

/**
 * @deprecated Use Options instead
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface DataSourceOptions<
    TStoreItem = any,
    TMappedItem = TStoreItem,
    TItem = TMappedItem,
    TKey = any,
> {
    /**
     * Custom parameters that should be passed to an OData service with the load query. Available only for the ODataStore.
     */
    customQueryParams?: any;
    /**
     * Specifies the navigation properties to be loaded with the OData entity. Available only for the ODataStore.
     */
    expand?: Array<string> | string;
    /**
     * Specifies data filtering conditions.
     */
    filter?: FilterDescriptor | Array<FilterDescriptor>;
    /**
     * Specifies data grouping properties.
     */
    group?: GroupDescriptor<TItem> | Array<GroupDescriptor<TItem>>;
    /**
     * Specifies an item mapping function.
     */
    map?: ((dataItem: TStoreItem) => TMappedItem);
    /**
     * A function that is executed after data is loaded.
     */
    onChanged?: ((e: { readonly changes?: Array<TMappedItem> }) => void);
    /**
     * A function that is executed when data loading fails.
     */
    onLoadError?: ((error: { readonly message?: string }) => void);
    /**
     * A function that is executed when the data loading status changes.
     */
    onLoadingChanged?: ((isLoading: boolean) => void);
    /**
     * Specifies the maximum number of data items per page. Applies only if paginate is true.
     */
    pageSize?: number;
    /**
     * Specifies whether the DataSource loads data items by pages or all at once. Defaults to false if group is set; otherwise, true.
     */
    paginate?: boolean;
    /**
     * Specifies a post processing function.
     */
    postProcess?: ((data: Array<TMappedItem>) => Array<TItem>);
    /**
     * Specifies the period (in milliseconds) when changes are aggregated before pushing them to the DataSource.
     */
    pushAggregationTimeout?: number;
    /**
     * Specifies whether the DataSource requests the total count of data items in the storage.
     */
    requireTotalCount?: boolean;
    /**
     * Specifies whether to reapply sorting, filtering, grouping, and other data processing operations after receiving a push.
     */
    reshapeOnPush?: boolean;
    /**
     * Specifies the fields to search.
     */
    searchExpr?: string | Function | Array<string | Function>;
    /**
     * Specifies the comparison operation used in searching.
     */
    searchOperation?: SearchOperation;
    /**
     * Specifies the value to which the search expression is compared.
     */
    searchValue?: any;
    /**
     * Specifies the fields to select from data objects.
     */
    select?: SelectDescriptor<TItem>;
    /**
     * Specifies data sorting properties.
     */
    sort?: SortDescriptor<TItem> | Array<SortDescriptor<TItem>>;
    /**
     * Configures the store underlying the DataSource.
     */
    store?: Array<TStoreItem> | Store<TStoreItem, TKey> | StoreOptions<TStoreItem, TKey>;
}
/**
 * The DataSource is an object that provides an API for processing data from an underlying store.
 */
export default class DataSource<
    TItem = any,
    TKey = any,
> {
    constructor(data: Array<TItem>);
    constructor(options: CustomStoreOptions<TItem, TKey> | Options<any, any, TItem, TKey>);
    constructor(store: Store<TItem, TKey>);
    constructor(url: string);
    /**
     * Cancels the load operation with a specific identifier.
     */
    cancel(operationId: number): boolean;
    /**
     * Disposes of all the resources allocated to the DataSource instance.
     */
    dispose(): void;
    /**
     * Gets the filter property&apos;s value.
     */
    filter(): FilterDescriptor | Array<FilterDescriptor>;
    /**
     * Sets the filter property&apos;s value.
     */
    filter(filterExpr: FilterDescriptor | Array<FilterDescriptor>): void;
    /**
     * Gets the group property&apos;s value.
     */
    group(): GroupDescriptor<TItem> | Array<GroupDescriptor<TItem>>;
    /**
     * Sets the group property&apos;s value.
     */
    group(groupExpr: GroupDescriptor<TItem> | Array<GroupDescriptor<TItem>>): void;
    /**
     * Checks whether the count of items on the current page is less than the pageSize. Takes effect only with enabled paging.
     */
    isLastPage(): boolean;
    /**
     * Checks whether data is loaded in the DataSource.
     */
    isLoaded(): boolean;
    /**
     * Checks whether data is being loaded in the DataSource.
     */
    isLoading(): boolean;
    /**
     * Gets an array of data items on the current page.
     */
    items(): Array<any>;
    /**
     * Gets the value of the underlying store&apos;s key property.
     */
    key(): string | Array<string>;
    /**
     * Starts loading data.
     */
    load(): DxExtendedPromise<any>;
    /**
     * Gets an object with current data processing settings.
     */
    loadOptions(): LoadOptions<TItem>;
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
     * Gets the current page index.
     */
    pageIndex(): number;
    /**
     * Sets the index of the page that should be loaded on the next load() method call.
     */
    pageIndex(newIndex: number): void;
    /**
     * Gets the page size.
     */
    pageSize(): number;
    /**
     * Sets the page size.
     */
    pageSize(value: number): void;
    /**
     * Gets the paginate property&apos;s value.
     */
    paginate(): boolean;
    /**
     * Sets the paginate property&apos;s value.
     */
    paginate(value: boolean): void;
    /**
     * Clears currently loaded DataSource items and calls the load() method.
     */
    reload(): DxExtendedPromise<any>;
    /**
     * Gets the requireTotalCount property&apos;s value.
     */
    requireTotalCount(): boolean;
    /**
     * Sets the requireTotalCount property&apos;s value.
     */
    requireTotalCount(value: boolean): void;
    /**
     * Gets the searchExpr property&apos;s value.
     */
    searchExpr(): string & Function & Array<string | Function>;
    /**
     * Sets the searchExpr property&apos;s value.
     */
    searchExpr(expr: string | Function | Array<string | Function>): void;
    /**
     * Gets the searchOperation property&apos;s value.
     */
    searchOperation(): string;
    /**
     * Sets the searchOperation property&apos;s value.
     */
    searchOperation(op: string): void;
    /**
     * Gets the searchValue property&apos;s value.
     */
    searchValue(): any;
    /**
     * Sets the searchValue property&apos;s value.
     */
    searchValue(value: any): void;
    /**
     * Gets the select property&apos;s value.
     */
    select(): SelectDescriptor<TItem>;
    /**
     * Sets the select property&apos;s value.
     */
    select(expr: SelectDescriptor<TItem>): void;
    /**
     * Gets the sort property&apos;s value.
     */
    sort(): SortDescriptor<TItem> | Array<SortDescriptor<TItem>>;
    /**
     * Sets the sort property&apos;s value.
     */
    sort(sortExpr: SortDescriptor<TItem> | Array<SortDescriptor<TItem>>): void;
    /**
     * Gets the instance of the store underlying the DataSource.
     */
    store(): Store<TItem, TKey>;
    /**
     * Gets the number of data items in the store after the last load() operation without paging. Takes effect only if requireTotalCount is true
     */
    totalCount(): number;
}

/**
 * 
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export type DataSourceLike<TItem, TKey = any> =
    string |
    Array<TItem> |
    Store<TItem, TKey> |
    DataSourceOptionsStub<any, any, TItem> |
    DataSource<TItem, TKey>;

/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
interface DataSourceOptionsStub<
    TStoreItem = any,
    TMappedItem = TStoreItem,
    TItem = TMappedItem,
> {
    customQueryParams?: any;
    expand?: Array<string> | string;
    filter?: FilterDescriptor | Array<FilterDescriptor>;
    group?: GroupDescriptor<TItem> | Array<GroupDescriptor<TItem>>;
    map?: ((dataItem: TStoreItem) => TMappedItem);
    onChanged?: ((e: { readonly changes?: Array<TMappedItem> }) => void);
    onLoadError?: ((error: { readonly message?: string }) => void);
    onLoadingChanged?: ((isLoading: boolean) => void);
    pageSize?: number;
    paginate?: boolean;
    postProcess?: ((data: Array<TMappedItem>) => Array<TItem>);
    pushAggregationTimeout?: number;
    requireTotalCount?: boolean;
    reshapeOnPush?: boolean;
    searchExpr?: string | Function | Array<string | Function>;
    searchOperation?: SearchOperation;
    searchValue?: any;
    select?: SelectDescriptor<TItem>;
    sort?: SortDescriptor<TItem> | Array<SortDescriptor<TItem>>;
    store?: Array<TStoreItem> | Store<TStoreItem, any> | StoreOptions<TStoreItem, any>;
}

/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
type EventName = 'changed' | 'loadError' | 'loadingChanged';
