/**
 * DevExtreme (esm/renovation/ui/grids/data_grid/data_grid_views.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import _extends from "@babel/runtime/helpers/esm/extends";
import _objectWithoutPropertiesLoose from "@babel/runtime/helpers/esm/objectWithoutPropertiesLoose";
var _excluded = ["instance", "showBorders"];
import {
    createComponentVNode
} from "inferno";
import {
    BaseInfernoComponent
} from "@devextreme/runtime/inferno";
import {
    GridBaseViews
} from "../grid_base/grid_base_views";
import {
    gridViewModule
} from "../../../../ui/grid_core/ui.grid_core.grid_view";
import {
    DataGridProps
} from "./common/data_grid_props";
import {
    deferRender
} from "../../../../core/utils/common";
import {
    hasWindow
} from "../../../../core/utils/window";
var {
    VIEW_NAMES: VIEW_NAMES
} = gridViewModule;
var DATA_GRID_CLASS = "dx-datagrid";
var DATA_GRID_ROLE_NAME = "grid";
export var viewFunction = _ref => {
    var {
        props: {
            showBorders: showBorders
        },
        update: update,
        views: views
    } = _ref;
    return createComponentVNode(2, GridBaseViews, {
        views: views,
        className: DATA_GRID_CLASS,
        showBorders: showBorders,
        role: DATA_GRID_ROLE_NAME,
        onRendered: update
    })
};
var DataGridPropsType = {
    get showBorders() {
        return DataGridProps.showBorders
    }
};
export class DataGridViews extends BaseInfernoComponent {
    constructor(props) {
        super(props);
        this.state = {};
        this.__getterCache = {};
        this.update = this.update.bind(this)
    }
    get views() {
        if (void 0 !== this.__getterCache.views) {
            return this.__getterCache.views
        }
        return this.__getterCache.views = (() => {
            if (!this.props.instance) {
                return []
            }
            var views = VIEW_NAMES.map(viewName => this.props.instance.getView(viewName)).filter(view => view);
            return views.map(view => ({
                name: view.name,
                view: view
            }))
        })()
    }
    update() {
        var gridInstance = this.props.instance;
        var dataController = gridInstance.getController("data");
        var resizingController = gridInstance.getController("resizing");
        if (hasWindow()) {
            deferRender(() => {
                resizingController.resize();
                if (dataController.isLoaded()) {
                    resizingController.fireContentReadyAction()
                }
            })
        }
    }
    get restAttributes() {
        var _this$props = this.props,
            restProps = _objectWithoutPropertiesLoose(_this$props, _excluded);
        return restProps
    }
    componentWillUpdate(nextProps, nextState, context) {
        if (this.props.instance !== nextProps.instance) {
            this.__getterCache.views = void 0
        }
    }
    render() {
        var props = this.props;
        return viewFunction({
            props: _extends({}, props),
            views: this.views,
            update: this.update,
            restAttributes: this.restAttributes
        })
    }
}
DataGridViews.defaultProps = DataGridPropsType;
