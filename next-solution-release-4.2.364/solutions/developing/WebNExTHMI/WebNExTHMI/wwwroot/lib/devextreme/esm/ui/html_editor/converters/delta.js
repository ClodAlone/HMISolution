/**
 * DevExtreme (esm/ui/html_editor/converters/delta.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import ConverterController from "../converterController";
class DeltaConverter {
    setQuillInstance(quillInstance) {
        this.quillInstance = quillInstance
    }
    toHtml() {
        if (!this.quillInstance) {
            return
        }
        return this._isQuillEmpty() ? "" : this.quillInstance.getSemanticHTML(0, this.quillInstance.getLength() + 1)
    }
    _isQuillEmpty() {
        var delta = this.quillInstance.getContents();
        return 1 === delta.length() && this._isDeltaEmpty(delta)
    }
    _isDeltaEmpty(delta) {
        return delta.reduce((__, _ref) => {
            var {
                insert: insert
            } = _ref;
            return -1 !== insert.indexOf("\n")
        })
    }
}
ConverterController.addConverter("delta", DeltaConverter);
export default DeltaConverter;
