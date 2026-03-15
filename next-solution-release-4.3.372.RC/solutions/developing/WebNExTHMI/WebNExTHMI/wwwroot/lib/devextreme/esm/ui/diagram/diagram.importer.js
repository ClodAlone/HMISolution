/**
 * DevExtreme (esm/ui/diagram/diagram.importer.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import Errors from "../widget/ui.errors";
import DiagramDefault, * as Diagram from "devexpress-diagram";
export function getDiagram() {
    if (!DiagramDefault) {
        throw Errors.Error("E1041", "devexpress-diagram")
    }
    return Diagram
}
