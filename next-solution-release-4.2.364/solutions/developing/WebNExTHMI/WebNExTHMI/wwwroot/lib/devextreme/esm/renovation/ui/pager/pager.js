/**
 * DevExtreme (esm/renovation/ui/pager/pager.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import _objectWithoutPropertiesLoose from "@babel/runtime/helpers/esm/objectWithoutPropertiesLoose";
import _extends from "@babel/runtime/helpers/esm/extends";
var _excluded = ["className", "defaultPageIndex", "defaultPageSize", "displayMode", "gridCompatibility", "hasKnownLastPage", "infoText", "lightModeEnabled", "maxPagesCount", "onKeyDown", "pageCount", "pageIndex", "pageIndexChange", "pageSize", "pageSizeChange", "pageSizes", "pagesCountText", "pagesNavigatorVisible", "rtlEnabled", "showInfo", "showNavigationButtons", "showPageSizes", "totalCount", "visible"];
import {
    createComponentVNode,
    normalizeProps
} from "inferno";
import {
    InfernoWrapperComponent
} from "@devextreme/runtime/inferno";
import {
    ResizableContainer
} from "./resizable_container";
import {
    PagerProps
} from "./common/pager_props";
import {
    PagerContent
} from "./content";
import {
    combineClasses
} from "../../utils/combine_classes";
export var viewFunction = _ref => {
    var {
        pagerProps: pagerProps,
        restAttributes: restAttributes
    } = _ref;
    return normalizeProps(createComponentVNode(2, ResizableContainer, _extends({
        contentTemplate: PagerContent,
        pagerProps: pagerProps
    }, restAttributes)))
};
import {
    createReRenderEffect
} from "@devextreme/runtime/inferno";
export class Pager extends InfernoWrapperComponent {
    constructor(props) {
        super(props);
        this.__getterCache = {};
        this.state = {
            pageSize: void 0 !== this.props.pageSize ? this.props.pageSize : this.props.defaultPageSize,
            pageIndex: void 0 !== this.props.pageIndex ? this.props.pageIndex : this.props.defaultPageIndex
        };
        this.pageIndexChange = this.pageIndexChange.bind(this);
        this.pageSizeChange = this.pageSizeChange.bind(this)
    }
    createEffects() {
        return [createReRenderEffect()]
    }
    pageIndexChange(newPageIndex) {
        if (this.props.gridCompatibility) {
            var __newValue;
            this.setState(__state_argument => {
                __newValue = newPageIndex + 1;
                return {
                    pageIndex: __newValue
                }
            });
            this.props.pageIndexChange(__newValue)
        } else {
            var _newValue;
            this.setState(__state_argument => {
                _newValue = newPageIndex;
                return {
                    pageIndex: _newValue
                }
            });
            this.props.pageIndexChange(_newValue)
        }
    }
    get pageIndex() {
        if (this.props.gridCompatibility) {
            return (void 0 !== this.props.pageIndex ? this.props.pageIndex : this.state.pageIndex) - 1
        }
        return void 0 !== this.props.pageIndex ? this.props.pageIndex : this.state.pageIndex
    }
    pageSizeChange(newPageSize) {
        var __newValue;
        this.setState(__state_argument => {
            __newValue = newPageSize;
            return {
                pageSize: __newValue
            }
        });
        this.props.pageSizeChange(__newValue)
    }
    get className() {
        if (this.props.gridCompatibility) {
            return combineClasses({
                "dx-datagrid-pager": true,
                ["".concat(this.props.className)]: !!this.props.className
            })
        }
        return this.props.className
    }
    get pagerProps() {
        if (void 0 !== this.__getterCache.pagerProps) {
            return this.__getterCache.pagerProps
        }
        return this.__getterCache.pagerProps = (() => _extends({}, _extends({}, this.props, {
            pageSize: void 0 !== this.props.pageSize ? this.props.pageSize : this.state.pageSize,
            pageIndex: void 0 !== this.props.pageIndex ? this.props.pageIndex : this.state.pageIndex
        }), {
            className: this.className,
            pageIndex: this.pageIndex,
            pageIndexChange: pageIndex => this.pageIndexChange(pageIndex),
            pageSizeChange: pageSize => this.pageSizeChange(pageSize)
        }))()
    }
    get restAttributes() {
        var _this$props$pageSize$ = _extends({}, this.props, {
                pageSize: void 0 !== this.props.pageSize ? this.props.pageSize : this.state.pageSize,
                pageIndex: void 0 !== this.props.pageIndex ? this.props.pageIndex : this.state.pageIndex
            }),
            restProps = _objectWithoutPropertiesLoose(_this$props$pageSize$, _excluded);
        return restProps
    }
    componentWillUpdate(nextProps, nextState, context) {
        super.componentWillUpdate();
        if (this.props !== nextProps || this.props.gridCompatibility !== nextProps.gridCompatibility || this.props.className !== nextProps.className || this.state.pageIndex !== nextState.pageIndex || this.props.pageIndex !== nextProps.pageIndex || this.props.pageIndexChange !== nextProps.pageIndexChange || this.props.pageSizeChange !== nextProps.pageSizeChange) {
            this.__getterCache.pagerProps = void 0
        }
    }
    render() {
        var props = this.props;
        return viewFunction({
            props: _extends({}, props, {
                pageSize: void 0 !== this.props.pageSize ? this.props.pageSize : this.state.pageSize,
                pageIndex: void 0 !== this.props.pageIndex ? this.props.pageIndex : this.state.pageIndex
            }),
            pageIndexChange: this.pageIndexChange,
            pageIndex: this.pageIndex,
            pageSizeChange: this.pageSizeChange,
            className: this.className,
            pagerProps: this.pagerProps,
            restAttributes: this.restAttributes
        })
    }
}
Pager.defaultProps = PagerProps;
