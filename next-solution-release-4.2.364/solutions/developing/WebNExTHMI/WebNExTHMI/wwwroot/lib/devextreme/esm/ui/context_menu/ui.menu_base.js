/**
 * DevExtreme (esm/ui/context_menu/ui.menu_base.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import $ from "../../core/renderer";
import {
    noop,
    asyncNoop
} from "../../core/utils/common";
import {
    isPlainObject,
    isObject,
    isDefined
} from "../../core/utils/type";
import {
    each
} from "../../core/utils/iterator";
import {
    extend
} from "../../core/utils/extend";
import {
    render
} from "../widget/utils.ink_ripple";
import HierarchicalCollectionWidget from "../hierarchical_collection/ui.hierarchical_collection_widget";
import MenuBaseEditStrategy from "./ui.menu_base.edit.strategy";
import devices from "../../core/devices";
import MenuItem from "../collection/item";
var DX_MENU_CLASS = "dx-menu";
var DX_MENU_NO_ICONS_CLASS = DX_MENU_CLASS + "-no-icons";
var DX_MENU_BASE_CLASS = "dx-menu-base";
var ITEM_CLASS = DX_MENU_CLASS + "-item";
var DX_ITEM_CONTENT_CLASS = ITEM_CLASS + "-content";
var DX_MENU_SELECTED_ITEM_CLASS = ITEM_CLASS + "-selected";
var DX_MENU_ITEM_WRAPPER_CLASS = ITEM_CLASS + "-wrapper";
var DX_MENU_ITEMS_CONTAINER_CLASS = DX_MENU_CLASS + "-items-container";
var DX_MENU_ITEM_EXPANDED_CLASS = ITEM_CLASS + "-expanded";
var DX_MENU_SEPARATOR_CLASS = DX_MENU_CLASS + "-separator";
var DX_MENU_ITEM_LAST_GROUP_ITEM = DX_MENU_CLASS + "-last-group-item";
var DX_ITEM_HAS_TEXT = ITEM_CLASS + "-has-text";
var DX_ITEM_HAS_ICON = ITEM_CLASS + "-has-icon";
var DX_ITEM_HAS_SUBMENU = ITEM_CLASS + "-has-submenu";
var DX_MENU_ITEM_POPOUT_CLASS = ITEM_CLASS + "-popout";
var DX_MENU_ITEM_POPOUT_CONTAINER_CLASS = DX_MENU_ITEM_POPOUT_CLASS + "-container";
var DX_MENU_ITEM_CAPTION_CLASS = ITEM_CLASS + "-text";
var SINGLE_SELECTION_MODE = "single";
var DEFAULT_DELAY = {
    show: 50,
    hide: 300
};
class MenuBase extends HierarchicalCollectionWidget {
    _getDefaultOptions() {
        return extend(super._getDefaultOptions(), {
            items: [],
            cssClass: "",
            activeStateEnabled: true,
            showSubmenuMode: {
                name: "onHover",
                delay: {
                    show: 50,
                    hide: 300
                }
            },
            animation: {
                show: {
                    type: "fade",
                    from: 0,
                    to: 1,
                    duration: 100
                },
                hide: {
                    type: "fade",
                    from: 1,
                    to: 0,
                    duration: 100
                }
            },
            selectByClick: false,
            focusOnSelectedItem: false,
            keyExpr: null,
            _itemAttributes: {
                role: "menuitem"
            },
            useInkRipple: false
        })
    }
    _itemDataKey() {
        return "dxMenuItemDataKey"
    }
    _itemClass() {
        return ITEM_CLASS
    }
    _setAriaSelected() {}
    _selectedItemClass() {
        return DX_MENU_SELECTED_ITEM_CLASS
    }
    _widgetClass() {
        return DX_MENU_BASE_CLASS
    }
    _focusTarget() {
        return this._itemContainer()
    }
    _clean() {
        this.option("focusedElement", null);
        super._clean()
    }
    _supportedKeys() {
        return extend(super._supportedKeys(), {
            space: () => {
                var $item = $(this.option("focusedElement"));
                if (!$item.length || !this._isSelectionEnabled()) {
                    return
                }
                this.selectItem($item[0])
            },
            pageUp: noop,
            pageDown: noop
        })
    }
    _isSelectionEnabled() {
        return this.option("selectionMode") === SINGLE_SELECTION_MODE
    }
    _init() {
        this._activeStateUnit = ".".concat(ITEM_CLASS);
        super._init();
        this._renderSelectedItem();
        this._initActions()
    }
    _getTextContainer(itemData) {
        var itemText = itemData.text;
        var $itemContainer = $("<span>").addClass(DX_MENU_ITEM_CAPTION_CLASS);
        var itemContent = isPlainObject(itemData) ? itemText : String(itemData);
        return itemText && $itemContainer.text(itemContent)
    }
    _getPopoutContainer(itemData) {
        var items = itemData.items;
        var $popOutContainer;
        if (items && items.length) {
            var $popOutImage = $("<div>").addClass(DX_MENU_ITEM_POPOUT_CLASS);
            $popOutContainer = $("<span>").addClass(DX_MENU_ITEM_POPOUT_CONTAINER_CLASS).append($popOutImage)
        }
        return $popOutContainer
    }
    _getDataAdapterOptions() {
        return {
            rootValue: 0,
            multipleSelection: false,
            recursiveSelection: false,
            recursiveExpansion: false,
            searchValue: ""
        }
    }
    _selectByItem(selectedItem) {
        if (!selectedItem) {
            return
        }
        var nodeToSelect = this._dataAdapter.getNodeByItem(selectedItem);
        this._dataAdapter.toggleSelection(nodeToSelect.internalFields.key, true)
    }
    _renderSelectedItem() {
        var selectedKeys = this._dataAdapter.getSelectedNodesKeys();
        var selectedKey = selectedKeys.length && selectedKeys[0];
        var selectedItem = this.option("selectedItem");
        if (!selectedKey) {
            this._selectByItem(selectedItem);
            return
        }
        var node = this._dataAdapter.getNodeByKey(selectedKey);
        if (false === node.selectable) {
            return
        }
        if (!selectedItem) {
            this.option("selectedItem", node.internalFields.item);
            return
        }
        if (selectedItem !== node.internalFields.item) {
            this._dataAdapter.toggleSelection(selectedKey, false);
            this._selectByItem(selectedItem)
        }
    }
    _initActions() {}
    _initMarkup() {
        super._initMarkup();
        this._addCustomCssClass(this.$element());
        this.option("useInkRipple") && this._renderInkRipple()
    }
    _renderInkRipple() {
        this._inkRipple = render()
    }
    _toggleActiveState($element, value, e) {
        super._toggleActiveState.apply(this, arguments);
        if (!this._inkRipple) {
            return
        }
        var config = {
            element: $element,
            event: e
        };
        if (value) {
            this._inkRipple.showWave(config)
        } else {
            this._inkRipple.hideWave(config)
        }
    }
    _getShowSubmenuMode() {
        var optionValue = this.option("showSubmenuMode");
        optionValue = isObject(optionValue) ? optionValue.name : optionValue;
        return this._isDesktopDevice() ? optionValue : "onClick"
    }
    _initSelectedItems() {}
    _isDesktopDevice() {
        return "desktop" === devices.real().deviceType
    }
    _initEditStrategy() {
        var Strategy = MenuBaseEditStrategy;
        this._editStrategy = new Strategy(this)
    }
    _addCustomCssClass($element) {
        $element.addClass(this.option("cssClass"))
    }
    _itemWrapperSelector() {
        return ".".concat(DX_MENU_ITEM_WRAPPER_CLASS)
    }
    _hoverStartHandler(e) {
        var $itemElement = this._getItemElementByEventArgs(e);
        if (!$itemElement || this._isItemDisabled($itemElement)) {
            return
        }
        e.stopPropagation();
        if ("onHover" === this._getShowSubmenuMode()) {
            clearTimeout(this._showSubmenusTimeout);
            this._showSubmenusTimeout = setTimeout(this._showSubmenu.bind(this, $itemElement), this._getSubmenuDelay("show"))
        }
    }
    _getAvailableItems($itemElements) {
        return super._getAvailableItems($itemElements).filter((function() {
            return "hidden" !== $(this).css("visibility")
        }))
    }
    _isItemDisabled($item) {
        return this._disabledGetter($item.data(this._itemDataKey()))
    }
    _showSubmenu($itemElement) {
        this._addExpandedClass($itemElement)
    }
    _addExpandedClass(itemElement) {
        $(itemElement).addClass(DX_MENU_ITEM_EXPANDED_CLASS)
    }
    _getSubmenuDelay(action) {
        var {
            delay: delay
        } = this.option("showSubmenuMode");
        if (!isDefined(delay)) {
            return DEFAULT_DELAY[action]
        }
        return isObject(delay) ? delay[action] : delay
    }
    _getItemElementByEventArgs(eventArgs) {
        var $target = $(eventArgs.target);
        if ($target.hasClass(this._itemClass()) || $target.get(0) === eventArgs.currentTarget) {
            return $target
        }
        while (!$target.hasClass(this._itemClass())) {
            $target = $target.parent();
            if ($target.hasClass("dx-submenu")) {
                return null
            }
        }
        return $target
    }
    _hoverEndHandler() {
        clearTimeout(this._showSubmenusTimeout)
    }
    _hasSubmenu(node) {
        return node && node.internalFields.childrenKeys.length
    }
    _renderContentImpl() {
        this._renderItems(this._dataAdapter.getRootNodes())
    }
    _renderItems(nodes, submenuContainer) {
        if (nodes.length) {
            this.hasIcons = false;
            var $nodeContainer = this._renderContainer(this.$element(), submenuContainer);
            var firstVisibleIndex = -1;
            var nextGroupFirstIndex = -1;
            each(nodes, (index, node) => {
                var isVisibleNode = false !== node.visible;
                if (isVisibleNode && firstVisibleIndex < 0) {
                    firstVisibleIndex = index
                }
                var isBeginGroup = firstVisibleIndex < index && (node.beginGroup || index === nextGroupFirstIndex);
                if (isBeginGroup) {
                    nextGroupFirstIndex = isVisibleNode ? index : index + 1
                }
                if (index === nextGroupFirstIndex && firstVisibleIndex < index) {
                    this._renderSeparator($nodeContainer)
                }
                this._renderItem(index, node, $nodeContainer)
            });
            if (!this.hasIcons) {
                $nodeContainer.addClass(DX_MENU_NO_ICONS_CLASS)
            }
        }
    }
    _renderContainer($wrapper) {
        var $container = $("<ul>");
        this.setAria("role", "none", $container);
        return $container.appendTo($wrapper).addClass(DX_MENU_ITEMS_CONTAINER_CLASS)
    }
    _createDOMElement($nodeContainer) {
        var $node = $("<li>");
        this.setAria("role", "none", $node);
        return $node.appendTo($nodeContainer).addClass(DX_MENU_ITEM_WRAPPER_CLASS)
    }
    _renderItem(index, node, $nodeContainer, $nodeElement) {
        var items = this.option("items");
        var $node = $nodeElement || this._createDOMElement($nodeContainer);
        if (items[index + 1] && items[index + 1].beginGroup) {
            $node.addClass(DX_MENU_ITEM_LAST_GROUP_ITEM)
        }
        var $itemFrame = super._renderItem(index, node.internalFields.item, $node);
        if (node.internalFields.item === this.option("selectedItem")) {
            $itemFrame.addClass(DX_MENU_SELECTED_ITEM_CLASS)
        }
        $itemFrame.attr("tabIndex", -1);
        if (this._hasSubmenu(node)) {
            this.setAria("haspopup", "true", $itemFrame)
        }
    }
    _renderItemFrame(index, itemData, $itemContainer) {
        var $itemFrame = $itemContainer.children(".".concat(ITEM_CLASS));
        return $itemFrame.length ? $itemFrame : super._renderItemFrame.apply(this, arguments)
    }
    _refreshItem($item, item) {
        var node = this._dataAdapter.getNodeByItem(item);
        var index = $item.data(this._itemIndexKey());
        var $nodeContainer = $item.closest("ul");
        var $nodeElement = $item.closest("li");
        this._renderItem(index, node, $nodeContainer, $nodeElement)
    }
    _addContentClasses(itemData, $itemFrame) {
        var hasText = itemData.text ? !!itemData.text.length : false;
        var hasIcon = !!itemData.icon;
        var hasSubmenu = itemData.items ? !!itemData.items.length : false;
        $itemFrame.toggleClass(DX_ITEM_HAS_TEXT, hasText);
        $itemFrame.toggleClass(DX_ITEM_HAS_ICON, hasIcon);
        if (!this.hasIcons) {
            this.hasIcons = hasIcon
        }
        $itemFrame.toggleClass(DX_ITEM_HAS_SUBMENU, hasSubmenu)
    }
    _getItemContent($itemFrame) {
        var $itemContent = super._getItemContent($itemFrame);
        if (!$itemContent.length) {
            $itemContent = $itemFrame.children(".".concat(DX_ITEM_CONTENT_CLASS))
        }
        return $itemContent
    }
    _postprocessRenderItem(args) {
        var $itemElement = $(args.itemElement);
        var selectedIndex = this._dataAdapter.getSelectedNodesKeys();
        if (!selectedIndex.length || !this._selectedGetter(args.itemData) || !this._isItemSelectable(args.itemData)) {
            this._setAriaSelected($itemElement, "false");
            return
        }
        var node = this._dataAdapter.getNodeByItem(args.itemData);
        if (node.internalFields.key === selectedIndex[0]) {
            $itemElement.addClass(this._selectedItemClass());
            this._setAriaSelected($itemElement, "true")
        } else {
            this._setAriaSelected($itemElement, "false")
        }
    }
    _isItemSelectable(item) {
        return false !== item.selectable
    }
    _renderSeparator($itemsContainer) {
        $("<li>").appendTo($itemsContainer).addClass(DX_MENU_SEPARATOR_CLASS)
    }
    _itemClickHandler(e) {
        if (e._skipHandling) {
            return
        }
        var itemClickActionHandler = this._createAction(this._updateSubmenuVisibilityOnClick.bind(this));
        this._itemDXEventHandler(e, "onItemClick", {}, {
            afterExecute: itemClickActionHandler.bind(this)
        });
        e._skipHandling = true
    }
    _updateSubmenuVisibilityOnClick(actionArgs) {
        this._updateSelectedItemOnClick(actionArgs);
        if ("onClick" === this._getShowSubmenuMode()) {
            this._addExpandedClass(actionArgs.args[0].itemElement)
        }
    }
    _updateSelectedItemOnClick(actionArgs) {
        var args = actionArgs.args ? actionArgs.args[0] : actionArgs;
        if (!this._isItemSelectionAllowed(args.itemData)) {
            return
        }
        var selectedItemKey = this._dataAdapter.getSelectedNodesKeys();
        var selectedNode = selectedItemKey.length && this._dataAdapter.getNodeByKey(selectedItemKey[0]);
        if (selectedNode) {
            this._toggleItemSelection(selectedNode, false)
        }
        if (!selectedNode || selectedNode.internalFields.item !== args.itemData) {
            this.selectItem(args.itemData)
        } else {
            this._fireSelectionChangeEvent(null, this.option("selectedItem"));
            this._setOptionWithoutOptionChange("selectedItem", null)
        }
    }
    _isItemSelectionAllowed(item) {
        var isSelectionByClickEnabled = this._isSelectionEnabled() && this.option("selectByClick");
        return !this._isContainerEmpty() && isSelectionByClickEnabled && this._isItemSelectable(item) && !this._itemsGetter(item)
    }
    _isContainerEmpty() {
        return this._itemContainer().is(":empty")
    }
    _syncSelectionOptions() {
        return asyncNoop()
    }
    _optionChanged(args) {
        switch (args.name) {
            case "showSubmenuMode":
                break;
            case "selectedItem":
                var node = this._dataAdapter.getNodeByItem(args.value);
                var selectedKey = this._dataAdapter.getSelectedNodesKeys()[0];
                if (node && node.internalFields.key !== selectedKey) {
                    if (false === node.selectable) {
                        break
                    }
                    if (selectedKey) {
                        this._toggleItemSelection(this._dataAdapter.getNodeByKey(selectedKey), false)
                    }
                    this._toggleItemSelection(node, true);
                    this._updateSelectedItems()
                }
                break;
            case "cssClass":
            case "position":
            case "selectByClick":
            case "animation":
            case "useInkRipple":
                this._invalidate();
                break;
            default:
                super._optionChanged(args)
        }
    }
    _toggleItemSelection(node, value) {
        var itemElement = this._getElementByItem(node.internalFields.item);
        itemElement && $(itemElement).toggleClass(DX_MENU_SELECTED_ITEM_CLASS);
        this._dataAdapter.toggleSelection(node.internalFields.key, value)
    }
    _getElementByItem(itemData) {
        var result;
        each(this._itemElements(), (_, itemElement) => {
            if ($(itemElement).data(this._itemDataKey()) !== itemData) {
                return true
            }
            result = itemElement;
            return false
        });
        return result
    }
    _updateSelectedItems(oldSelection, newSelection) {
        if (oldSelection || newSelection) {
            this._fireSelectionChangeEvent(newSelection, oldSelection)
        }
    }
    _fireSelectionChangeEvent(addedSelection, removedSelection) {
        this._createActionByOption("onSelectionChanged", {
            excludeValidators: ["disabled", "readOnly"]
        })({
            addedItems: [addedSelection],
            removedItems: [removedSelection]
        })
    }
    selectItem(itemElement) {
        var itemData = itemElement.nodeType ? this._getItemData(itemElement) : itemElement;
        var selectedKey = this._dataAdapter.getSelectedNodesKeys()[0];
        var selectedItem = this.option("selectedItem");
        var node = this._dataAdapter.getNodeByItem(itemData);
        if (node.internalFields.key !== selectedKey) {
            if (selectedKey) {
                this._toggleItemSelection(this._dataAdapter.getNodeByKey(selectedKey), false)
            }
            this._toggleItemSelection(node, true);
            this._updateSelectedItems(selectedItem, itemData);
            this._setOptionWithoutOptionChange("selectedItem", itemData)
        }
    }
    unselectItem(itemElement) {
        var itemData = itemElement.nodeType ? this._getItemData(itemElement) : itemElement;
        var node = this._dataAdapter.getNodeByItem(itemData);
        var selectedItem = this.option("selectedItem");
        if (node.internalFields.selected) {
            this._toggleItemSelection(node, false);
            this._updateSelectedItems(selectedItem, null);
            this._setOptionWithoutOptionChange("selectedItem", null)
        }
    }
}
MenuBase.ItemClass = MenuItem;
export default MenuBase;
