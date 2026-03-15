/**
 * DevExtreme (esm/renovation/ui/pager/common/pager_props.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import _extends from "@babel/runtime/helpers/esm/extends";
import {
    BasePagerProps
} from "./base_pager_props";
export var PagerProps = Object.create(Object.prototype, _extends(Object.getOwnPropertyDescriptors(BasePagerProps), Object.getOwnPropertyDescriptors({
    defaultPageSize: 5,
    pageSizeChange: () => {},
    defaultPageIndex: 1,
    pageIndexChange: () => {}
})));
export var InternalPagerProps = Object.create(Object.prototype, _extends(Object.getOwnPropertyDescriptors(BasePagerProps), Object.getOwnPropertyDescriptors({
    pageSize: 5,
    pageIndex: 1
})));
