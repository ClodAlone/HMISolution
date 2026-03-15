/**
* DevExtreme (ui/file_uploader.d.ts)
* Version: 21.2.6
* Build date: Tue Mar 01 2022
*
* Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
* Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
*/
import {
    UserDefinedElement,
    DxElement,
} from '../core/element';

import {
    EventInfo,
    NativeEventInfo,
    InitializedEventInfo,
    ChangedOptionInfo,
} from '../events/index';

import Editor, {
    EditorOptions,
} from './editor/editor';

import UploadInfo from '../file_management/upload_info';

export type BeforeSendEvent = EventInfo<dxFileUploader> & {
    readonly request: XMLHttpRequest;
    readonly file: File;
    readonly uploadInfo?: UploadInfo;
};

export type ContentReadyEvent = EventInfo<dxFileUploader>;

export type DisposingEvent = EventInfo<dxFileUploader>;

export type DropZoneEnterEvent = NativeEventInfo<dxFileUploader, PointerEvent | MouseEvent> & {
    readonly dropZoneElement: DxElement;
};

export type DropZoneLeaveEvent = NativeEventInfo<dxFileUploader, PointerEvent | MouseEvent> & {
    readonly dropZoneElement: DxElement;
};

export type FilesUploadedEvent = EventInfo<dxFileUploader>;

export type InitializedEvent = InitializedEventInfo<dxFileUploader>;

export type OptionChangedEvent = EventInfo<dxFileUploader> & ChangedOptionInfo;

export type ProgressEvent = NativeEventInfo<dxFileUploader> & {
    readonly file: File;
    readonly segmentSize: number;
    readonly bytesLoaded: number;
    readonly bytesTotal: number;
    readonly request: XMLHttpRequest;
};

export type UploadAbortedEvent = NativeEventInfo<dxFileUploader> & {
    readonly file: File;
    readonly request: XMLHttpRequest;
    message: string;
};

export type UploadedEvent = NativeEventInfo<dxFileUploader> & {
    readonly file: File;
    readonly request: XMLHttpRequest;
    message: string;
};

export type UploadErrorEvent = NativeEventInfo<dxFileUploader> & {
    readonly file: File;
    readonly request: XMLHttpRequest;
    readonly error: any;
    message: string;
};

export type UploadStartedEvent = NativeEventInfo<dxFileUploader> & {
    readonly file: File;
    readonly request: XMLHttpRequest;
};

export type ValueChangedEvent = NativeEventInfo<dxFileUploader> & {
    readonly value?: Array<File>;
    readonly previousValue?: Array<File>;
};

/**
 * @deprecated use Properties instead
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface dxFileUploaderOptions extends EditorOptions<dxFileUploader> {
    /**
     * A function that cancels the file upload.
     */
    abortUpload?: ((file: File, uploadInfo?: UploadInfo) => PromiseLike<any> | any);
    /**
     * Specifies a file type or several types accepted by the UI component.
     */
    accept?: string;
    /**
     * Specifies if an end user can remove a file from the selection and interrupt uploading.
     */
    allowCanceling?: boolean;
    /**
     * Restricts file extensions that can be uploaded to the server.
     */
    allowedFileExtensions?: Array<string>;
    /**
     * Specifies the chunk size in bytes. Applies only if uploadMode is &apos;instantly&apos; or &apos;useButtons&apos;. Requires a server that can process file chunks.
     */
    chunkSize?: number;
    /**
     * Specifies whether the UI component can be focused using keyboard navigation.
     */
    focusStateEnabled?: boolean;
    /**
     * Specifies whether the FileUploader component changes the state of all its buttons when users hover over them.
     */
    hoverStateEnabled?: boolean;
    /**
     * The text displayed when the extension of the file being uploaded is not an allowed file extension.
     */
    invalidFileExtensionMessage?: string;
    /**
     * The text displayed when the size of the file being uploaded is greater than the maxFileSize.
     */
    invalidMaxFileSizeMessage?: string;
    /**
     * The text displayed when the size of the file being uploaded is less than the minFileSize.
     */
    invalidMinFileSizeMessage?: string;
    /**
     * Specifies the attributes to be passed on to the underlying `` element of the `file` type.
     */
    inputAttr?: any;
    /**
     * Specifies the text displayed on the area to which an end-user can drop a file.
     */
    labelText?: string;
    /**
     * Specifies the maximum file size (in bytes) allowed for uploading. Applies only if uploadMode is &apos;instantly&apos; or &apos;useButtons&apos;.
     */
    maxFileSize?: number;
    /**
     * Specifies the minimum file size (in bytes) allowed for uploading. Applies only if uploadMode is &apos;instantly&apos; or &apos;useButtons&apos;.
     */
    minFileSize?: number;
    /**
     * Specifies whether the UI component enables an end-user to select a single file or multiple files.
     */
    multiple?: boolean;
    /**
     * Specifies the value passed to the name attribute of the underlying input element. Required to access uploaded files on the server.
     */
    name?: string;
    /**
     * A function that allows you to customize the request before it is sent to the server.
     */
    onBeforeSend?: ((e: BeforeSendEvent) => void);
    /**
     * A function that is executed when the mouse enters a drop zone while dragging a file.
     */
    onDropZoneEnter?: ((e: DropZoneEnterEvent) => void);
    /**
     * A function that is executed when the mouse leaves a drop zone as it drags a file.
     */
    onDropZoneLeave?: ((e: DropZoneLeaveEvent) => void);
    /**
     * A function that is executed when all files are successfully uploaded.
     */
    onFilesUploaded?: ((e: FilesUploadedEvent) => void);
    /**
     * A function that is executed when a file segment is uploaded.
     */
    onProgress?: ((e: ProgressEvent) => void);
    /**
     * A function that is executed when the file upload is aborted.
     */
    onUploadAborted?: ((e: UploadAbortedEvent) => void);
    /**
     * A function that is executed when an error occurs during the file upload.
     */
    onUploadError?: ((e: UploadErrorEvent) => void);
    /**
     * A function that is executed when the file upload is started.
     */
    onUploadStarted?: ((e: UploadStartedEvent) => void);
    /**
     * A function that is executed when a file is successfully uploaded.
     */
    onUploaded?: ((e: UploadedEvent) => void);
    /**
     * A function that is executed when one or several files are added to or removed from the selection.
     */
    onValueChanged?: ((e: ValueChangedEvent) => void);
    /**
     * Gets the current progress in percentages.
     */
    progress?: number;
    /**
     * The message displayed by the UI component when it is ready to upload the specified files.
     */
    readyToUploadMessage?: string;
    /**
     * The text displayed on the button that opens the file browser.
     */
    selectButtonText?: string;
    /**
     * Specifies whether or not the UI component displays the list of selected files.
     */
    showFileList?: boolean;
    /**
     * Specifies the HTML element which invokes the file upload dialog.
     */
    dialogTrigger?: string | UserDefinedElement;
    /**
     * Specifies the HTML element in which users can drag and drop files for upload.
     */
    dropZone?: string | UserDefinedElement;
    /**
     * The text displayed on the button that starts uploading.
     */
    uploadButtonText?: string;
    /**
     * A function that uploads a file in chunks.
     */
    uploadChunk?: ((file: File, uploadInfo: UploadInfo) => PromiseLike<any> | any);
    /**
     * The message displayed by the UI component on uploading failure.
     */
    uploadFailedMessage?: string;
    /**
     * The message displayed by the UI component when the file upload is cancelled.
     */
    uploadAbortedMessage?: string;
    /**
     * A function that uploads a file.
     */
    uploadFile?: ((file: File, progressCallback: Function) => PromiseLike<any> | any);
    /**
     * Specifies headers for the upload request.
     */
    uploadHeaders?: any;
    /**
     * Specifies custom data for the upload request.
     */
    uploadCustomData?: any;
    /**
     * Specifies the method for the upload request.
     */
    uploadMethod?: 'POST' | 'PUT';
    /**
     * Specifies how the UI component uploads files.
     */
    uploadMode?: 'instantly' | 'useButtons' | 'useForm';
    /**
     * Specifies a target Url for the upload request.
     */
    uploadUrl?: string;
    /**
     * The message displayed by the UI component when uploading is finished.
     */
    uploadedMessage?: string;
    /**
     * Specifies a File instance representing the selected file. Read-only when uploadMode is &apos;useForm&apos;.
     */
    value?: Array<File>;
}
/**
 * The FileUploader UI component enables an end user to upload files to the server. An end user can select files in the file explorer or drag and drop files to the FileUploader area on the page.
 */
export default class dxFileUploader extends Editor<dxFileUploaderOptions> {
    /**
     * Uploads all the selected files.
     */
    upload(): void;
    /**
     * Uploads a file with the specified index.
     */
    upload(fileIndex: number): void;
    /**
     * Uploads the specified file.
     */
    upload(file: File): void;
    /**
     * Cancels the file upload.
     */
    abortUpload(): void;
    /**
     * Cancels the file upload.
     */
    abortUpload(fileIndex: number): void;
    /**
     * Cancels the file upload.
     */
    abortUpload(file: File): void;
    /**
     * Removes a file with the specified index.
     */
    removeFile(fileIndex: number): void;
    /**
     * Removes a file.
     */
    removeFile(file: File): void;
}

export type Properties = dxFileUploaderOptions;

/**
 * @deprecated use Properties instead
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export type Options = dxFileUploaderOptions;
