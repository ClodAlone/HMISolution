/**
 * DevExtreme (esm/file_management/error.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
class FileSystemError {
    constructor(errorCode, fileSystemItem, errorText) {
        this.errorCode = errorCode;
        this.fileSystemItem = fileSystemItem;
        this.errorText = errorText
    }
}
export default FileSystemError;
