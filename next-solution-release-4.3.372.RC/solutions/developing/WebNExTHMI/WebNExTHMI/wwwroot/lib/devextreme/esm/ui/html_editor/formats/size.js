/**
 * DevExtreme (esm/ui/html_editor/formats/size.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import Quill from "devextreme-quill";
var SizeStyle = {};
if (Quill) {
    SizeStyle = Quill.import("attributors/style/size");
    SizeStyle.whitelist = null
}
export default SizeStyle;
