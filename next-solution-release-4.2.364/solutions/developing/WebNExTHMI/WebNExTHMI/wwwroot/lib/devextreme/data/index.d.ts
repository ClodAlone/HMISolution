/**
* DevExtreme (data/index.d.ts)
* Version: 21.2.6
* Build date: Tue Mar 01 2022
*
* Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
* Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
*/
import CustomStore, { Options as CustomStoreOptions } from './custom_store';
import ArrayStore, { Options as ArrayStoreOptions } from './array_store';
import LocalStore, { Options as LocalStoreOptions } from './local_store';
import ODataStore, { Options as ODataStoreOptions } from './odata/store';

/**
 * 
 */
export type SearchOperation = '=' | '<>' | '>' | '>=' | '<' | '<=' | 'startswith' | 'endswith' | 'contains' | 'notcontains';

/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
type KeySelector<T> = string | ((source: T) => string | number | Date | Object);

/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
type BaseGroupDescriptor<T> = {
    selector: KeySelector<T>;
};

/**
 * 
 */
export type GroupDescriptor<T> = KeySelector<T> | BaseGroupDescriptor<T> & {
    desc?: boolean;
};

/**
 * 
 */
export type SortDescriptor<T> = GroupDescriptor<T>;

/**
 * 
 */
export type SelectDescriptor<T> = string | Array<string> | ((source: T) => any);
/**
  * 
  */
 export type FilterDescriptor = any;

 /**
 * 
 */
export type SummaryDescriptor<T> = KeySelector<T> | BaseGroupDescriptor<T> & {
    summaryType?: 'sum' | 'avg' | 'min' | 'max' | 'count';
};

/**
  * 
  */
 export interface LoadOptions<T = any> {
    /**
     * An object for storing additional settings that should be sent to the server. Relevant to the ODataStore only.
     */
    customQueryParams?: any;
    /**
     * An array of strings that represent the names of navigation properties to be loaded simultaneously with the ODataStore.
     */
    expand?: Array<string>;
    /**
     * A filter expression.
     */
    filter?: FilterDescriptor | Array<FilterDescriptor>;
    /**
     * A group expression.
     */
    group?: GroupDescriptor<T> | Array<GroupDescriptor<T>>;
    /**
     * A group summary expression. Used with the group setting.
     */
    groupSummary?: SummaryDescriptor<T> | Array<SummaryDescriptor<T>>;
    /**
     * The IDs of the rows being expanded. Relevant only when the CustomStore is used in the TreeList UI component.
     */
    parentIds?: Array<any>;
    /**
     * Indicates whether a top-level group count is required. Used in conjunction with the filter, take, skip, requireTotalCount, and group settings.
     */
    requireGroupCount?: boolean;
    /**
     * Indicates whether the total count of data objects is needed.
     */
    requireTotalCount?: boolean;
    /**
     * A data field or expression whose value is compared to the search value.
     */
    searchExpr?: string | Function | Array<string | Function>;
    /**
     * A comparison operation.
     */
    searchOperation?: SearchOperation;
    /**
     * The current search value.
     */
    searchValue?: any;
    /**
     * A select expression.
     */
    select?: SelectDescriptor<T>;
    /**
     * The number of data objects to be skipped from the result set&apos;s start. In conjunction with take, used to implement paging.
     */
    skip?: number;
    /**
     * A sort expression.
     */
    sort?: SortDescriptor<T> | Array<SortDescriptor<T>>;
    /**
     * The number of data objects to be loaded. In conjunction with skip, used to implement paging.
     */
    take?: number;
    /**
     * A total summary expression.
     */
    totalSummary?: SummaryDescriptor<T> | Array<SummaryDescriptor<T>>;
    /**
     * An object for storing additional settings that should be sent to the server.
     */
    userData?: any;
}

export type Store<TItem = any, TKey = any> =
    CustomStore<TItem, TKey> |
    ArrayStore<TItem, TKey> |
    LocalStore<TItem, TKey> |
    ODataStore<TItem, TKey>;

export type StoreOptions<TItem = any, TKey = any> =
    CustomStoreOptions<TItem, TKey> |
    ArrayStoreOptions<TItem, TKey> & { type: 'array' } |
    LocalStoreOptions<TItem, TKey> & { type: 'local' } |
    ODataStoreOptions<TItem, TKey> & { type: 'odata' };
