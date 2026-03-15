/**
* DevExtreme (exporter/excel/excel.doc_comments.d.ts)
* Version: 21.2.6
* Build date: Tue Mar 01 2022
*
* Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
* Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
*/
/**
 * An object that configures the font in an Excel cell.
 * @deprecated 
 * @deprecated Attention! This type is for internal purposes only. If you used it previously, please describe your scenario in the following GitHub Issue, and we will suggest a public alternative: {@link https://github.com/DevExpress/DevExtreme/issues/17885|Internal Types}.
 */
export interface ExcelFont {
    /**
     * Specifies whether the text should be in bold.
     */
    bold?: boolean;
    /**
     * The text&apos;s color in hexadecimal characters.
     */
    color?: string;
    /**
     * Specifies whether the text should be in italic.
     */
    italic?: boolean;
    /**
     * The name of the typeface that should be applied to the text.
     */
    name?: string;
    /**
     * The font size specified in points (1/72 of an inch).
     */
    size?: number;
    /**
     * The underline formatting style.
     */
    underline?: 'double' | 'doubleAccounting' | 'none' | 'single' | 'singleAccounting';
}
