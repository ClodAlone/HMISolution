/**
* DevExtreme (data/array_store.d.ts)
* Version: 21.2.6
* Build date: Tue Mar 01 2022
*
* Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
* Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
*/
import Store, {
    Options as StoreOptions,
} from './abstract_store';
import { Query } from './query';

export type Options<
    TItem = any,
    TKey = any,
> = ArrayStoreOptions<TItem, TKey>;

/**
 * @deprecated Use Options instead
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface ArrayStoreOptions<
    TItem = any,
    TKey = any,
> extends StoreOptions<TItem, TKey> {
    /**
     * Specifies the store&apos;s associated array.
     */
    data?: Array<TItem>;
}
/**
 * The ArrayStore is a store that provides an interface for loading and editing an in-memory array and handling related events.
 */
export default class ArrayStore<
    TItem = any,
    TKey = any,
> extends Store<TItem, TKey> {
    constructor(options?: Options<TItem, TKey>)
    /**
     * Clears all the ArrayStore&apos;s associated data.
     */
    clear(): void;
    /**
     * Creates a Query for the underlying array.
     */
    createQuery(): Query;
}
