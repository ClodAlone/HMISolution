/**
 * DevExtreme (esm/ui/list/ui.list.edit.decorator.static.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import $ from "../../core/renderer";
import Button from "../button";
import {
    register as registerDecorator
} from "./ui.list.edit.decorator_registry";
import EditDecorator from "./ui.list.edit.decorator";
var STATIC_DELETE_BUTTON_CONTAINER_CLASS = "dx-list-static-delete-button-container";
var STATIC_DELETE_BUTTON_CLASS = "dx-list-static-delete-button";
registerDecorator("delete", "static", EditDecorator.inherit({
    afterBag: function(config) {
        var $itemElement = config.$itemElement;
        var $container = config.$container;
        var $button = $("<div>").addClass(STATIC_DELETE_BUTTON_CLASS);
        this._list._createComponent($button, Button, {
            icon: "remove",
            onClick: function(args) {
                args.event.stopPropagation();
                this._deleteItem($itemElement)
            }.bind(this),
            integrationOptions: {}
        });
        $container.addClass(STATIC_DELETE_BUTTON_CONTAINER_CLASS).append($button)
    },
    _deleteItem: function($itemElement) {
        if ($itemElement.is(".dx-state-disabled, .dx-state-disabled *")) {
            return
        }
        this._list.deleteItem($itemElement)
    }
}));
