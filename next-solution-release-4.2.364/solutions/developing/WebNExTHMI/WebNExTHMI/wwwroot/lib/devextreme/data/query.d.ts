/**
* DevExtreme (data/query.d.ts)
* Version: 21.2.6
* Build date: Tue Mar 01 2022
*
* Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
* Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
*/
import {
    DxPromise,
} from '../core/utils/deferred';

/**
 * The Query is an object that provides a chainable interface for making data queries.
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface Query {
    /**
     * Calculates a custom summary for all data items.
     */
    aggregate(seed: any, step: Function, finalize: Function): DxPromise<any>;
    /**
     * Calculates a custom summary for all data items.
     */
    aggregate(step: Function): DxPromise<any>;
    /**
     * Calculates the average of all values. Applies only to numeric arrays.
     */
    avg(): DxPromise<number>;
    /**
     * Calculates the average of all values found using a getter.
     */
    avg(getter: any): DxPromise<number>;
    /**
     * Calculates the number of data items.
     */
    count(): DxPromise<number>;
    /**
     * Executes the Query. This is an asynchronous alternative to the toArray() method.
     */
    enumerate(): DxPromise<any>;
    /**
     * Filters data items using a filter expression.
     */
    filter(criteria: Array<any>): Query;
    /**
     * Filters data items using a custom function.
     */
    filter(predicate: Function): Query;
    /**
     * Groups data items by the specified getter.
     */
    groupBy(getter: any): Query;
    /**
     * Calculates the maximum value. Applies only to numeric arrays.
     */
    max(): DxPromise<number | Date>;
    /**
     * Calculates the maximum of all values found using a getter.
     */
    max(getter: any): DxPromise<number | Date>;
    /**
     * Calculates the minimum value. Applies only to numeric arrays.
     */
    min(): DxPromise<number | Date>;
    /**
     * Calculates the minumum of all values found using a getter.
     */
    min(getter: any): DxPromise<number | Date>;
    /**
     * Selects individual fields from data objects.
     */
    select(getter: any): Query;
    /**
     * Gets a specified number of data items starting from a given index.
     */
    slice(skip: number, take?: number): Query;
    /**
     * Sorts data items by the specified getter in ascending order.
     */
    sortBy(getter: any): Query;
    /**
     * Sorts data items by the specified getter in the specified sorting order.
     */
    sortBy(getter: any, desc: boolean): Query;
    /**
     * Calculates the sum of all values.
     */
    sum(): DxPromise<number>;
    /**
     * Calculates the sum of all values found using a getter.
     */
    sum(getter: any): DxPromise<number>;
    /**
     * Sorts data items by one more getter in ascending order.
     */
    thenBy(getter: any): Query;
    /**
     * Sorts data items by one more getter in the specified sorting order.
     */
    thenBy(getter: any, desc: boolean): Query;
    /**
     * Gets data items associated with the Query. This is a synchronous alternative to the enumerate() method.
     */
    toArray(): Array<any>;
}

/**
 * Creates a Query instance.
 */
declare function query(array: Array<any>): Query;

/**
 * Creates a Query instance that accesses a remote data service using its URL.
 */
declare function query(url: string, queryOptions: any): Query;

export default query;
