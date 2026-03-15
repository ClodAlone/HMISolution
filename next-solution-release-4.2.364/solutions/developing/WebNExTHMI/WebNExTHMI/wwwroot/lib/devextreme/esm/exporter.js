/**
 * DevExtreme (esm/exporter.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import {
    fileSaver
} from "./exporter/file_saver";
import {
    ExcelCreator,
    getData as getExcelData
} from "./exporter/excel_creator";
import {
    imageCreator,
    testFormats,
    getData as getImageData
} from "./exporter/image_creator";
import {
    svgCreator,
    getData as getSvgData
} from "./exporter/svg_creator";
import {
    isFunction as _isFunction
} from "./core/utils/type";
import {
    Deferred
} from "./core/utils/deferred";
import formatConverter from "./exporter/excel_format_converter";
import {
    getData
} from "./exporter/pdf_creator";

function _export(data, options, getData) {
    if (!data) {
        return (new Deferred).resolve()
    }
    var exportingAction = options.exportingAction;
    var exportedAction = options.exportedAction;
    var fileSavingAction = options.fileSavingAction;
    var eventArgs = {
        fileName: options.fileName,
        format: options.format,
        cancel: false
    };
    _isFunction(exportingAction) && exportingAction(eventArgs);
    if (!eventArgs.cancel) {
        return getData(data, options).then(blob => {
            _isFunction(exportedAction) && exportedAction();
            if (_isFunction(fileSavingAction)) {
                eventArgs.data = blob;
                fileSavingAction(eventArgs)
            }
            if (!eventArgs.cancel) {
                fileSaver.saveAs(eventArgs.fileName, options.format, blob, options.proxyUrl, options.forceProxy)
            }
        })
    }
    return (new Deferred).resolve()
}
export {
    _export as
    export, fileSaver
};
export var excel = {
    creator: ExcelCreator,
    getData: getExcelData,
    formatConverter: formatConverter
};
export var image = {
    creator: imageCreator,
    getData: getImageData,
    testFormats: testFormats
};
export var pdf = {
    getData: getData
};
export var svg = {
    creator: svgCreator,
    getData: getSvgData
};
