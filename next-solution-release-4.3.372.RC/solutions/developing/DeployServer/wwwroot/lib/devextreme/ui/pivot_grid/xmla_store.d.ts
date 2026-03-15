/**
* DevExtreme (ui/pivot_grid/xmla_store.d.ts)
* Version: 21.2.6
* Build date: Tue Mar 01 2022
*
* Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
* Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
*/
/**
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface XmlaStoreOptions {
    /**
     * Specifies a function that customizes the request before it is sent to the server.
     */
    beforeSend?: ((options: { url?: string; method?: string; headers?: any; xhrFields?: any; data?: string; dataType?: string }) => void);
    /**
     * Specifies the database (or initial catalog) that contains the OLAP cube to use.
     */
    catalog?: string;
    /**
     * Specifies the name of the OLAP cube to use from the catalog.
     */
    cube?: string;
    /**
     * Specifies the OLAP server&apos;s URL.
     */
    url?: string;
}
/**
                                                                     * The XmlaStore is a store that provides an interface for accessing an OLAP cube according to the XMLA standard.
                                                                     */
                                                                    export default class XmlaStore {
    constructor(options?: XmlaStoreOptions)
}
