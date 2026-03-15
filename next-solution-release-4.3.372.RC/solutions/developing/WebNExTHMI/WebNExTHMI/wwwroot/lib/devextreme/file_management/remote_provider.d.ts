/**
* DevExtreme (file_management/remote_provider.d.ts)
* Version: 21.2.6
* Build date: Tue Mar 01 2022
*
* Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
* Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
*/
import FileSystemProviderBase, {
    FileSystemProviderBaseOptions,
} from './provider_base';

export type Options = RemoteFileSystemProviderOptions;

/**
 * @deprecated Use Options instead
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface RemoteFileSystemProviderOptions extends FileSystemProviderBaseOptions<RemoteFileSystemProvider> {
    /**
     * Specifies a function that customizes an Ajax request before it is sent to the server.
     */
    beforeAjaxSend?: ((options: { headers?: any; xhrFields?: any; formData?: any }) => void);
    /**
     * Specifies a function that customizes a form submit request before it is sent to the server.
     */
    beforeSubmit?: ((options: { formData?: any }) => void);
    /**
     * Specifies the URL of an endpoint used to access and modify a file system located on the server.
     */
    endpointUrl?: string;
    /**
     * Specifies which data field provides information about whether a directory has subdirectories.
     */
    hasSubDirectoriesExpr?: string | Function;
    /**
     * Specifies the request headers.
     */
    requestHeaders?: any;
}
/**
 * The Remote file system provider works with a file system located on the server.
 */
export default class RemoteFileSystemProvider extends FileSystemProviderBase {
    constructor(options?: Options)
}
