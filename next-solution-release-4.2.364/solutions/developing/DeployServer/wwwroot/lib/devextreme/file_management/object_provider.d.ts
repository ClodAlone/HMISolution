/**
* DevExtreme (file_management/object_provider.d.ts)
* Version: 21.2.6
* Build date: Tue Mar 01 2022
*
* Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
* Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
*/
import FileSystemProviderBase, {
    FileSystemProviderBaseOptions,
} from './provider_base';

export type Options = ObjectFileSystemProviderOptions;

/**
 * @deprecated Use Options instead
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface ObjectFileSystemProviderOptions extends FileSystemProviderBaseOptions<ObjectFileSystemProvider> {
    /**
     * Specifies which data field provides information about files content.
     */
    contentExpr?: string | Function;
    /**
     * Specifies an array of data objects that represent files and directories.
     */
    data?: Array<any>;
    /**
     * Specifies which data field provides information about nested files and directories.
     */
    itemsExpr?: string | Function;
}
/**
 * The Object file system provider works with a file system represented by an in-memory array of JSON objects.
 */
export default class ObjectFileSystemProvider extends FileSystemProviderBase {
    constructor(options?: Options)
}
