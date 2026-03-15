/**
 * DevExtreme (esm/exporter/excel/excel.file.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import {
    isDefined
} from "../../core/utils/type";
import tagHelper from "./excel.tag_helper";
import cellFormatHelper from "./excel.cell_format_helper";
import fillHelper from "./excel.fill_helper";
import fontHelper from "./excel.font_helper";
import numberFormatHelper from "./excel.number_format_helper";
export default class ExcelFile {
    constructor() {
        this._cellFormatTags = [];
        this._fillTags = [];
        this._fontTags = [];
        this._numberFormatTags = [];
        this._fillTags.push(fillHelper.tryCreateTag({
            patternFill: {
                patternType: "none"
            }
        }))
    }
    registerCellFormat(cellFormat) {
        var result;
        var cellFormatTag = cellFormatHelper.tryCreateTag(cellFormat, {
            registerFill: this.registerFill.bind(this),
            registerFont: this.registerFont.bind(this),
            registerNumberFormat: this.registerNumberFormat.bind(this)
        });
        if (isDefined(cellFormatTag)) {
            for (var i = 0; i < this._cellFormatTags.length; i++) {
                if (cellFormatHelper.areEqual(this._cellFormatTags[i], cellFormatTag)) {
                    result = i;
                    break
                }
            }
            if (void 0 === result) {
                result = this._cellFormatTags.push(cellFormatTag) - 1
            }
        }
        return result
    }
    static copyCellFormat(source) {
        return cellFormatHelper.copy(source)
    }
    generateCellFormatsXml() {
        var cellFormatTagsAsXmlStringsArray = this._cellFormatTags.map(tag => cellFormatHelper.toXml(tag));
        return tagHelper.toXml("cellXfs", {
            count: cellFormatTagsAsXmlStringsArray.length
        }, cellFormatTagsAsXmlStringsArray.join(""))
    }
    registerFill(fill) {
        var result;
        var fillTag = fillHelper.tryCreateTag(fill);
        if (isDefined(fillTag)) {
            for (var i = 0; i < this._fillTags.length; i++) {
                if (fillHelper.areEqual(this._fillTags[i], fillTag)) {
                    result = i;
                    break
                }
            }
            if (void 0 === result) {
                if (this._fillTags.length < 2) {
                    this._fillTags.push(fillHelper.tryCreateTag({
                        patternFill: {
                            patternType: "Gray125"
                        }
                    }))
                }
                result = this._fillTags.push(fillTag) - 1
            }
        }
        return result
    }
    generateFillsXml() {
        var tagsAsXmlStringsArray = this._fillTags.map(tag => fillHelper.toXml(tag));
        return tagHelper.toXml("fills", {
            count: tagsAsXmlStringsArray.length
        }, tagsAsXmlStringsArray.join(""))
    }
    registerFont(font) {
        var result;
        var fontTag = fontHelper.tryCreateTag(font);
        if (isDefined(fontTag)) {
            for (var i = 0; i < this._fontTags.length; i++) {
                if (fontHelper.areEqual(this._fontTags[i], fontTag)) {
                    result = i;
                    break
                }
            }
            if (void 0 === result) {
                result = this._fontTags.push(fontTag) - 1
            }
        }
        return result
    }
    generateFontsXml() {
        var xmlStringsArray = this._fontTags.map(tag => fontHelper.toXml(tag));
        return tagHelper.toXml("fonts", {
            count: xmlStringsArray.length
        }, xmlStringsArray.join(""))
    }
    _convertNumberFormatIndexToId(index) {
        return 165 + index
    }
    registerNumberFormat(numberFormat) {
        var result;
        var tag = numberFormatHelper.tryCreateTag(numberFormat);
        if (isDefined(tag)) {
            for (var i = 0; i < this._numberFormatTags.length; i++) {
                if (numberFormatHelper.areEqual(this._numberFormatTags[i], tag)) {
                    result = this._numberFormatTags[i][numberFormatHelper.ID_PROPERTY_NAME];
                    break
                }
            }
            if (void 0 === result) {
                tag[numberFormatHelper.ID_PROPERTY_NAME] = this._convertNumberFormatIndexToId(this._numberFormatTags.length);
                result = tag[numberFormatHelper.ID_PROPERTY_NAME];
                this._numberFormatTags.push(tag)
            }
        }
        return result
    }
    generateNumberFormatsXml() {
        if (this._numberFormatTags.length > 0) {
            var xmlStringsArray = this._numberFormatTags.map(tag => numberFormatHelper.toXml(tag));
            return tagHelper.toXml("numFmts", {
                count: xmlStringsArray.length
            }, xmlStringsArray.join(""))
        } else {
            return ""
        }
    }
}
