/**
* DevExtreme (file_management/error.d.ts)
* Version: 21.2.6
* Build date: Tue Mar 01 2022
*
* Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
* Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
*/
import FileSystemItem from './file_system_item';

/**
  * An object that contains information about the error.
  */
 export default class FileSystemError {
   constructor(errorCode?: number, fileSystemItem?: FileSystemItem, errorText?: string)
    /**
     * The processed file or directory.
     */
    fileSystemItem?: FileSystemItem;

    /**
     * The error code.
     */
    errorCode?: number;

    /**
      * The error message.
      */
     errorText?: string;
}
