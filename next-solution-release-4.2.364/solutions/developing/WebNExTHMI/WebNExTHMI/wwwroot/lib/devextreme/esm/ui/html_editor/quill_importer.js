/**
 * DevExtreme (esm/ui/html_editor/quill_importer.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import Errors from "../widget/ui.errors";
import Quill from "devextreme-quill";
export function getQuill() {
    if (!Quill) {
        throw Errors.Error("E1041", "Quill")
    }
    return Quill
}
