/**
* DevExtreme (data/local_store.d.ts)
* Version: 21.2.6
* Build date: Tue Mar 01 2022
*
* Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
* Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
*/
import ArrayStore, {
    ArrayStoreOptions,
} from './array_store';

export type Options<
    TItem = any,
    TKey = any,
> = LocalStoreOptions<TItem, TKey>;

/**
 * @deprecated Use Options instead
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface LocalStoreOptions<
    TItem = any,
    TKey = any,
> extends ArrayStoreOptions<TItem, TKey> {
    /**
     * Specifies a delay in milliseconds between when data changes and the moment these changes are saved in the local storage. Applies only if immediate is false.
     */
    flushInterval?: number;
    /**
     * Specifies whether the LocalStore saves changes in the local storage immediately.
     */
    immediate?: boolean;
    /**
     * Specifies the name under which data should be saved in the local storage. The `dx-data-localStore-` prefix will be added to the name.
     */
    name?: string;
}
/**
 * The LocalStore is a store that provides an interface for loading and editing data from HTML Web Storage (also known as window.localStorage) and handling related events.
 */
export default class LocalStore<
    TItem = any,
    TKey = any,
> extends ArrayStore<TItem, TKey> {
    constructor(options?: Options<TItem, TKey>)
    /**
     * Removes data from the local storage.
     */
    clear(): void;
}
