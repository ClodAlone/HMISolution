/**
 * DevExtreme (esm/renovation/ui/scheduler/header/view_switcher.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import _extends from "@babel/runtime/helpers/esm/extends";
import {
    isOneView
} from "../../../../ui/scheduler/header/utils";
var VIEW_SWITCHER_CLASS = "dx-scheduler-view-switcher";
var VIEW_SWITCHER_DROP_DOWN_BUTTON_CLASS = "dx-scheduler-view-switcher-dropdown-button";
var VIEW_SWITCHER_DROP_DOWN_BUTTON_CONTENT_CLASS = "dx-scheduler-view-switcher-dropdown-button-content";
export var getViewSwitcher = (item, selectedView, views, setCurrentView) => _extends({
    widget: "dxButtonGroup",
    locateInMenu: "auto",
    cssClass: VIEW_SWITCHER_CLASS,
    options: {
        items: views,
        keyExpr: "name",
        selectedItemKeys: [selectedView],
        stylingMode: "contained",
        onItemClick: e => {
            setCurrentView(e.itemData)
        }
    }
}, item);
export var getDropDownViewSwitcher = (item, selectedView, views, setCurrentView) => {
    var oneView = isOneView(views, selectedView);
    return _extends({
        widget: "dxDropDownButton",
        locateInMenu: "never",
        cssClass: VIEW_SWITCHER_CLASS,
        options: {
            items: views,
            useSelectMode: true,
            keyExpr: "name",
            selectedItemKey: selectedView,
            displayExpr: "text",
            showArrowIcon: !oneView,
            elementAttr: {
                class: VIEW_SWITCHER_DROP_DOWN_BUTTON_CLASS
            },
            onItemClick: e => {
                setCurrentView(e.itemData)
            },
            dropDownOptions: {
                onShowing: e => {
                    if (oneView) {
                        e.cancel = true
                    }
                },
                width: "max-content",
                wrapperAttr: {
                    class: VIEW_SWITCHER_DROP_DOWN_BUTTON_CONTENT_CLASS
                }
            }
        }
    }, item)
};
