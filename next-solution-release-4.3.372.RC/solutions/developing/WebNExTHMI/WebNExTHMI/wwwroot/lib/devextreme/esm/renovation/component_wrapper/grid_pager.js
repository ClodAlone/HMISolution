/**
 * DevExtreme (esm/renovation/component_wrapper/grid_pager.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import Component from "./common/component";
export class GridPagerWrapper extends Component {
    _optionChanged(args) {
        switch (args.name) {
            case "pageIndex":
                var pageIndexChanged = this.option("pageIndexChanged");
                if (pageIndexChanged) {
                    pageIndexChanged(args.value)
                }
                break;
            case "pageSize":
                var pageSizeChanged = this.option("pageSizeChanged");
                if (pageSizeChanged) {
                    pageSizeChanged(args.value)
                }
        }
        super._optionChanged(args)
    }
}
