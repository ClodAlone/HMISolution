/**
 * DevExtreme (esm/ui/tree_list/ui.tree_list.core.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import {
    extend
} from "../../core/utils/extend";
import modules from "../grid_core/ui.grid_core.modules";
export default extend({}, modules, {
    modules: [],
    foreachNodes: function(nodes, callBack, ignoreHasChildren) {
        for (var i = 0; i < nodes.length; i++) {
            if (false !== callBack(nodes[i]) && (ignoreHasChildren || nodes[i].hasChildren) && nodes[i].children.length) {
                this.foreachNodes(nodes[i].children, callBack, ignoreHasChildren)
            }
        }
    }
});
