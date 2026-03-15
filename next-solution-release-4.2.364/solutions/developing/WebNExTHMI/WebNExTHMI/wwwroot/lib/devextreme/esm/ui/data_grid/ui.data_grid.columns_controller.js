/**
 * DevExtreme (esm/ui/data_grid/ui.data_grid.columns_controller.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import gridCore from "./ui.data_grid.core";
import {
    columnsControllerModule
} from "../grid_core/ui.grid_core.columns_controller";
import {
    extend
} from "../../core/utils/extend";
gridCore.registerModule("columns", {
    defaultOptions: function() {
        return extend(true, {}, columnsControllerModule.defaultOptions(), {
            commonColumnSettings: {
                allowExporting: true
            }
        })
    },
    controllers: columnsControllerModule.controllers
});
