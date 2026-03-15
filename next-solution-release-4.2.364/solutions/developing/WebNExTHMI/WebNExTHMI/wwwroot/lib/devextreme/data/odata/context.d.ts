/**
* DevExtreme (data/odata/context.d.ts)
* Version: 21.2.6
* Build date: Tue Mar 01 2022
*
* Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
* Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
*/
import {
    DxPromise,
} from '../../core/utils/deferred';

/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface ODataRequestOptions {
    accepts: any;
    async: boolean;
    contentType: string | boolean;
    data: any;
    dataType: string;
    headers: any;
    jsonp?: boolean;
    method: string;
    timeout: number;
    url: string;
    xhrFields: any;
}

/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface ODataContextOptions {
    /**
     * Specifies a function that customizes the request before it is sent to the server.
     */
    beforeSend?: ((options: { url: string; async: boolean; method: string; timeout: number; params: any; payload: any; headers: any }) => void);
    /**
     * Specifies whether stores in the ODataContext serialize/parse date-time values.
     */
    deserializeDates?: boolean;
    /**
     * Specifies entity collections to be accessed.
     */
    entities?: any;
    /**
     * Specifies a function that is executed when the ODataContext throws an error.
     */
    errorHandler?: ((e: { httpStatus: number; errorDetails: any; requestOptions: ODataRequestOptions }) => void);
    /**
     * Specifies whether to convert string values to lowercase in filter and search requests. Applies to the following operations: &apos;startswith&apos;, &apos;endswith&apos;, &apos;contains&apos;, and &apos;notcontains&apos;.
     */
    filterToLower?: boolean;
    /**
     * Specifies whether data should be sent using JSONP.
     */
    jsonp?: boolean;
    /**
     * Specifies the URL of an OData service.
     */
    url?: string;
    /**
     * Specifies the OData version.
     */
    version?: number;
    /**
     * Specifies whether to send cookies, authorization headers, and client certificates in a cross-origin request.
     */
    withCredentials?: boolean;
}
/**
 * The ODataContent is an object that provides access to an entire OData service.
 */
export default class ODataContext {
    constructor(options?: ODataContextOptions)
    /**
     * Invokes an OData operation that returns a value.
     */
    get(operationName: string, params: any): DxPromise<any>;
    /**
     * Invokes an OData operation that returns nothing.
     */
    invoke(operationName: string, params: any, httpMethod: HttpMethod): DxPromise<void>;
    /**
     * Gets a link to an entity with a specific key.
     */
    objectLink(entityAlias: string, key: any | string | number): any;
}

/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
type HttpMethod = 'GET' | 'POST' | 'PATCH' | 'MERGE';
