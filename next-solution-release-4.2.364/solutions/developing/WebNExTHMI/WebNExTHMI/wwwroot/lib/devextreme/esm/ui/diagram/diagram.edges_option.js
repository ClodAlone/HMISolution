/**
 * DevExtreme (esm/ui/diagram/diagram.edges_option.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import ItemsOption from "./diagram.items_option";
class EdgesOption extends ItemsOption {
    _getKeyExpr() {
        return this._diagramWidget._createOptionGetter("edges.keyExpr")
    }
}
export default EdgesOption;
