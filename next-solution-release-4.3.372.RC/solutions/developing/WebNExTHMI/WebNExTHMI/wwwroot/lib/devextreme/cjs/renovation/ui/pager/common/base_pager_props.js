/**
 * DevExtreme (cjs/renovation/ui/pager/common/base_pager_props.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.BasePagerProps = void 0;
var BasePagerProps = Object.defineProperties({
    gridCompatibility: true,
    showInfo: false,
    displayMode: "adaptive",
    maxPagesCount: 10,
    pageCount: 10,
    visible: true,
    hasKnownLastPage: true,
    pagesNavigatorVisible: "auto",
    showPageSizes: true,
    showNavigationButtons: false,
    totalCount: 0
}, {
    pageSizes: {
        get: function() {
            return [5, 10]
        },
        configurable: true,
        enumerable: true
    }
});
exports.BasePagerProps = BasePagerProps;
