/**
 * DevExtreme (esm/ui/html_editor/themes/base.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import Quill from "devextreme-quill";
var BaseTheme;
if (Quill) {
    var Theme = Quill.import("core/theme");
    BaseTheme = class extends Theme {
        constructor(quill, options) {
            super(quill, options);
            this.quill.root.classList.add("dx-htmleditor-content");
            this.quill.root.setAttribute("role", "textbox")
        }
    }
} else {
    BaseTheme = {}
}
export default BaseTheme;
