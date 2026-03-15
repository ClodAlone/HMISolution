/**
 * DevExtreme (esm/ui/list/ui.list.edit.provider.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import $ from "../../core/renderer";
import {
    noop
} from "../../core/utils/common";
import Class from "../../core/class";
import {
    extend
} from "../../core/utils/extend";
import {
    each
} from "../../core/utils/iterator";
import errors from "../widget/ui.errors";
import {
    registry
} from "./ui.list.edit.decorator_registry";
var editOptionsRegistry = [];
var registerOption = function(enabledFunc, decoratorTypeFunc, decoratorSubTypeFunc) {
    editOptionsRegistry.push({
        enabled: enabledFunc,
        decoratorType: decoratorTypeFunc,
        decoratorSubType: decoratorSubTypeFunc
    })
};
registerOption((function() {
    return this.option("menuItems").length
}), (function() {
    return "menu"
}), (function() {
    return this.option("menuMode")
}));
registerOption((function() {
    return !this.option("menuItems").length && this.option("allowItemDeleting")
}), (function() {
    var mode = this.option("itemDeleteMode");
    return "toggle" === mode || "slideButton" === mode || "swipe" === mode || "static" === mode ? "delete" : "menu"
}), (function() {
    var mode = this.option("itemDeleteMode");
    if ("slideItem" === mode) {
        mode = "slide"
    }
    return mode
}));
registerOption((function() {
    return "none" !== this.option("selectionMode") && this.option("showSelectionControls")
}), (function() {
    return "selection"
}), (function() {
    return "default"
}));
registerOption((function() {
    return this.option("itemDragging.allowReordering") || this.option("itemDragging.allowDropInsideItem") || this.option("itemDragging.group")
}), (function() {
    return "reorder"
}), (function() {
    return "default"
}));
var LIST_ITEM_BEFORE_BAG_CLASS = "dx-list-item-before-bag";
var LIST_ITEM_AFTER_BAG_CLASS = "dx-list-item-after-bag";
var DECORATOR_BEFORE_BAG_CREATE_METHOD = "beforeBag";
var DECORATOR_AFTER_BAG_CREATE_METHOD = "afterBag";
var DECORATOR_MODIFY_ELEMENT_METHOD = "modifyElement";
var DECORATOR_AFTER_RENDER_METHOD = "afterRender";
var DECORATOR_GET_EXCLUDED_SELECTORS_METHOD = "getExcludedSelectors";
var EditProvider = Class.inherit({
    ctor: function(list) {
        this._list = list;
        this._fetchRequiredDecorators()
    },
    dispose: function() {
        if (this._decorators && this._decorators.length) {
            each(this._decorators, (function(_, decorator) {
                decorator.dispose()
            }))
        }
    },
    _fetchRequiredDecorators: function() {
        this._decorators = [];
        each(editOptionsRegistry, function(_, option) {
            var optionEnabled = option.enabled.call(this._list);
            if (optionEnabled) {
                var decoratorType = option.decoratorType.call(this._list);
                var decoratorSubType = option.decoratorSubType.call(this._list);
                var decorator = this._createDecorator(decoratorType, decoratorSubType);
                this._decorators.push(decorator)
            }
        }.bind(this))
    },
    _createDecorator: function(type, subType) {
        var decoratorClass = this._findDecorator(type, subType);
        return new decoratorClass(this._list)
    },
    _findDecorator: function(type, subType) {
        var _registry$type;
        var foundDecorator = null === (_registry$type = registry[type]) || void 0 === _registry$type ? void 0 : _registry$type[subType];
        if (!foundDecorator) {
            throw errors.Error("E1012", type, subType)
        }
        return foundDecorator
    },
    modifyItemElement: function(args) {
        var $itemElement = $(args.itemElement);
        var config = {
            $itemElement: $itemElement
        };
        this._prependBeforeBags($itemElement, config);
        this._appendAfterBags($itemElement, config);
        this._applyDecorators(DECORATOR_MODIFY_ELEMENT_METHOD, config)
    },
    afterItemsRendered: function() {
        this._applyDecorators(DECORATOR_AFTER_RENDER_METHOD)
    },
    _prependBeforeBags: function($itemElement, config) {
        var $beforeBags = this._collectDecoratorsMarkup(DECORATOR_BEFORE_BAG_CREATE_METHOD, config, LIST_ITEM_BEFORE_BAG_CLASS);
        $itemElement.prepend($beforeBags)
    },
    _appendAfterBags: function($itemElement, config) {
        var $afterBags = this._collectDecoratorsMarkup(DECORATOR_AFTER_BAG_CREATE_METHOD, config, LIST_ITEM_AFTER_BAG_CLASS);
        $itemElement.append($afterBags)
    },
    _collectDecoratorsMarkup: function(method, config, containerClass) {
        var $collector = $("<div>");
        each(this._decorators, (function() {
            var $container = $("<div>").addClass(containerClass);
            this[method](extend({
                $container: $container
            }, config));
            if ($container.children().length) {
                $collector.append($container)
            }
        }));
        return $collector.children()
    },
    _applyDecorators: function(method, config) {
        each(this._decorators, (function() {
            this[method](config)
        }))
    },
    _handlerExists: function(name) {
        if (!this._decorators) {
            return false
        }
        var decorators = this._decorators;
        var length = decorators.length;
        for (var i = 0; i < length; i++) {
            if (decorators[i][name] !== noop) {
                return true
            }
        }
        return false
    },
    _eventHandler: function(name, $itemElement, e) {
        if (!this._decorators) {
            return false
        }
        var response = false;
        var decorators = this._decorators;
        var length = decorators.length;
        for (var i = 0; i < length; i++) {
            response = decorators[i][name]($itemElement, e);
            if (response) {
                break
            }
        }
        return response
    },
    handleClick: function($itemElement, e) {
        return this._eventHandler("handleClick", $itemElement, e)
    },
    handleKeyboardEvents: function(currentFocusedIndex, moveFocusUp) {
        return this._eventHandler("handleKeyboardEvents", currentFocusedIndex, moveFocusUp)
    },
    handleEnterPressing: function(e) {
        return this._eventHandler("handleEnterPressing", e)
    },
    contextMenuHandlerExists: function() {
        return this._handlerExists("handleContextMenu")
    },
    handleContextMenu: function($itemElement, e) {
        return this._eventHandler("handleContextMenu", $itemElement, e)
    },
    getExcludedItemSelectors: function() {
        var excludedSelectors = [];
        this._applyDecorators(DECORATOR_GET_EXCLUDED_SELECTORS_METHOD, excludedSelectors);
        return excludedSelectors.join(",")
    }
});
export default EditProvider;
