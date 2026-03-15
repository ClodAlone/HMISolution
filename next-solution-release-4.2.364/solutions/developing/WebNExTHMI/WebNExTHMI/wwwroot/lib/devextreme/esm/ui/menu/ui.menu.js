/**
 * DevExtreme (esm/ui/menu/ui.menu.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import {
    getOuterWidth
} from "../../core/utils/size";
import $ from "../../core/renderer";
import eventsEngine from "../../events/core/events_engine";
import registerComponent from "../../core/component_registrator";
import {
    noop
} from "../../core/utils/common";
import {
    getPublicElement
} from "../../core/element";
import {
    each
} from "../../core/utils/iterator";
import {
    isPlainObject,
    isObject,
    isDefined
} from "../../core/utils/type";
import {
    extend
} from "../../core/utils/extend";
import {
    getElementMaxHeightByWindow
} from "../overlay/utils";
import {
    addNamespace
} from "../../events/utils/index";
import pointerEvents from "../../events/pointer";
import {
    end as hoverEventEnd
} from "../../events/hover";
import MenuBase from "../context_menu/ui.menu_base";
import Overlay from "../overlay/ui.overlay";
import Submenu from "./ui.submenu";
import Button from "../button";
import TreeView from "../tree_view";
var DX_MENU_CLASS = "dx-menu";
var DX_MENU_VERTICAL_CLASS = DX_MENU_CLASS + "-vertical";
var DX_MENU_HORIZONTAL_CLASS = DX_MENU_CLASS + "-horizontal";
var DX_MENU_ITEM_CLASS = DX_MENU_CLASS + "-item";
var DX_MENU_ITEMS_CONTAINER_CLASS = DX_MENU_CLASS + "-items-container";
var DX_MENU_ITEM_EXPANDED_CLASS = DX_MENU_ITEM_CLASS + "-expanded";
var DX_CONTEXT_MENU_CLASS = "dx-context-menu";
var DX_CONTEXT_MENU_CONTAINER_BORDER_CLASS = DX_CONTEXT_MENU_CLASS + "-container-border";
var DX_CONTEXT_MENU_CONTENT_DELIMITER_CLASS = "dx-context-menu-content-delimiter";
var DX_SUBMENU_CLASS = "dx-submenu";
var DX_STATE_DISABLED_CLASS = "dx-state-disabled";
var DX_STATE_HOVER_CLASS = "dx-state-hover";
var DX_STATE_ACTIVE_CLASS = "dx-state-active";
var DX_ADAPTIVE_MODE_CLASS = DX_MENU_CLASS + "-adaptive-mode";
var DX_ADAPTIVE_HAMBURGER_BUTTON_CLASS = DX_MENU_CLASS + "-hamburger-button";
var DX_ADAPTIVE_MODE_OVERLAY_WRAPPER_CLASS = DX_ADAPTIVE_MODE_CLASS + "-overlay-wrapper";
var FOCUS_UP = "up";
var FOCUS_DOWN = "down";
var FOCUS_LEFT = "left";
var FOCUS_RIGHT = "right";
var SHOW_SUBMENU_OPERATION = "showSubmenu";
var NEXTITEM_OPERATION = "nextItem";
var PREVITEM_OPERATION = "prevItem";
var DEFAULT_DELAY = {
    show: 50,
    hide: 300
};
var ACTIONS = ["onSubmenuShowing", "onSubmenuShown", "onSubmenuHiding", "onSubmenuHidden", "onItemContextMenu", "onItemClick", "onSelectionChanged", "onItemRendered"];
class Menu extends MenuBase {
    _getDefaultOptions() {
        return extend(super._getDefaultOptions(), {
            orientation: "horizontal",
            submenuDirection: "auto",
            showFirstSubmenuMode: {
                name: "onClick",
                delay: {
                    show: 50,
                    hide: 300
                }
            },
            hideSubmenuOnMouseLeave: false,
            onSubmenuShowing: null,
            onSubmenuShown: null,
            onSubmenuHiding: null,
            onSubmenuHidden: null,
            adaptivityEnabled: false
        })
    }
    _setOptionsByReference() {
        super._setOptionsByReference();
        extend(this._optionsByReference, {
            animation: true,
            selectedItem: true
        })
    }
    _itemElements() {
        var rootMenuElements = super._itemElements();
        var submenuElements = this._submenuItemElements();
        return rootMenuElements.add(submenuElements)
    }
    _submenuItemElements() {
        var elements = [];
        var itemSelector = ".".concat(DX_MENU_ITEM_CLASS);
        var currentSubmenu = this._submenus.length && this._submenus[0];
        if (currentSubmenu && currentSubmenu.itemsContainer()) {
            elements = currentSubmenu.itemsContainer().find(itemSelector)
        }
        return elements
    }
    _focusTarget() {
        return this.$element()
    }
    _isMenuHorizontal() {
        return "horizontal" === this.option("orientation")
    }
    _moveFocus(location) {
        var $items = this._getAvailableItems();
        var isMenuHorizontal = this._isMenuHorizontal();
        var $activeItem = this._getActiveItem(true);
        var argument;
        var operation;
        var navigationAction;
        var $newTarget;
        switch (location) {
            case FOCUS_UP:
                operation = isMenuHorizontal ? SHOW_SUBMENU_OPERATION : this._getItemsNavigationOperation(PREVITEM_OPERATION);
                argument = isMenuHorizontal ? $activeItem : $items;
                navigationAction = this._getKeyboardNavigationAction(operation, argument);
                $newTarget = navigationAction();
                break;
            case FOCUS_DOWN:
                operation = isMenuHorizontal ? SHOW_SUBMENU_OPERATION : this._getItemsNavigationOperation(NEXTITEM_OPERATION);
                argument = isMenuHorizontal ? $activeItem : $items;
                navigationAction = this._getKeyboardNavigationAction(operation, argument);
                $newTarget = navigationAction();
                break;
            case FOCUS_RIGHT:
                operation = isMenuHorizontal ? this._getItemsNavigationOperation(NEXTITEM_OPERATION) : SHOW_SUBMENU_OPERATION;
                argument = isMenuHorizontal ? $items : $activeItem;
                navigationAction = this._getKeyboardNavigationAction(operation, argument);
                $newTarget = navigationAction();
                break;
            case FOCUS_LEFT:
                operation = isMenuHorizontal ? this._getItemsNavigationOperation(PREVITEM_OPERATION) : SHOW_SUBMENU_OPERATION;
                argument = isMenuHorizontal ? $items : $activeItem;
                navigationAction = this._getKeyboardNavigationAction(operation, argument);
                $newTarget = navigationAction();
                break;
            default:
                return super._moveFocus(location)
        }
        if ($newTarget && 0 !== $newTarget.length) {
            this.option("focusedElement", getPublicElement($newTarget))
        }
    }
    _getItemsNavigationOperation(operation) {
        var navOperation = operation;
        if (this.option("rtlEnabled")) {
            navOperation = operation === PREVITEM_OPERATION ? NEXTITEM_OPERATION : PREVITEM_OPERATION
        }
        return navOperation
    }
    _getKeyboardNavigationAction(operation, argument) {
        var action = noop;
        switch (operation) {
            case SHOW_SUBMENU_OPERATION:
                if (!argument.hasClass(DX_STATE_DISABLED_CLASS)) {
                    action = this._showSubmenu.bind(this, argument)
                }
                break;
            case NEXTITEM_OPERATION:
                action = this._nextItem.bind(this, argument);
                break;
            case PREVITEM_OPERATION:
                action = this._prevItem.bind(this, argument)
        }
        return action
    }
    _clean() {
        super._clean();
        this.option("templatesRenderAsynchronously") && clearTimeout(this._resizeEventTimer)
    }
    _visibilityChanged(visible) {
        if (visible) {
            if (!this._menuItemsWidth) {
                this._updateItemsWidthCache()
            }
            this._dimensionChanged()
        }
    }
    _isAdaptivityEnabled() {
        return this.option("adaptivityEnabled") && "horizontal" === this.option("orientation")
    }
    _updateItemsWidthCache() {
        var $menuItems = this.$element().find("ul").first().children("li").children(".".concat(DX_MENU_ITEM_CLASS));
        this._menuItemsWidth = this._getSummaryItemsWidth($menuItems, true)
    }
    _dimensionChanged() {
        if (!this._isAdaptivityEnabled()) {
            return
        }
        var containerWidth = getOuterWidth(this.$element());
        this._toggleAdaptiveMode(this._menuItemsWidth > containerWidth)
    }
    _init() {
        super._init();
        this._submenus = []
    }
    _initActions() {
        this._actions = {};
        each(ACTIONS, (index, action) => {
            this._actions[action] = this._createActionByOption(action)
        })
    }
    _initMarkup() {
        this._visibleSubmenu = null;
        this.$element().addClass(DX_MENU_CLASS);
        super._initMarkup();
        this.setAria("role", "menubar")
    }
    _render() {
        super._render();
        this._initAdaptivity()
    }
    _renderHamburgerButton() {
        this._hamburger = new Button($("<div>").addClass(DX_ADAPTIVE_HAMBURGER_BUTTON_CLASS), {
            icon: "menu",
            activeStateEnabled: false,
            onClick: this._toggleTreeView.bind(this)
        });
        return this._hamburger.$element()
    }
    _toggleTreeView(state) {
        if (isPlainObject(state)) {
            state = !this._overlay.option("visible")
        }
        this._overlay.option("visible", state);
        this._toggleHamburgerActiveState(state)
    }
    _toggleHamburgerActiveState(state) {
        this._hamburger && this._hamburger.$element().toggleClass(DX_STATE_ACTIVE_CLASS, state)
    }
    _toggleAdaptiveMode(state) {
        var $menuItemsContainer = this.$element().find(".".concat(DX_MENU_HORIZONTAL_CLASS));
        var $adaptiveElements = this.$element().find(".".concat(DX_ADAPTIVE_MODE_CLASS));
        if (state) {
            this._hideVisibleSubmenu()
        } else {
            this._treeView && this._treeView.collapseAll();
            this._overlay && this._toggleTreeView(state)
        }
        $menuItemsContainer.toggle(!state);
        $adaptiveElements.toggle(state)
    }
    _removeAdaptivity() {
        if (!this._$adaptiveContainer) {
            return
        }
        this._toggleAdaptiveMode(false);
        this._$adaptiveContainer.remove();
        this._$adaptiveContainer = null;
        this._treeView = null;
        this._hamburger = null;
        this._overlay = null
    }
    _treeviewItemClickHandler(e) {
        this._actions.onItemClick(e);
        if (!e.node.children.length) {
            this._toggleTreeView(false)
        }
    }
    _getAdaptiveOverlayOptions() {
        var rtl = this.option("rtlEnabled");
        var position = rtl ? "right" : "left";
        return {
            _ignoreFunctionValueDeprecation: true,
            maxHeight: () => getElementMaxHeightByWindow(this.$element()),
            deferRendering: false,
            shading: false,
            animation: false,
            hideOnParentScroll: true,
            onHidden: () => {
                this._toggleHamburgerActiveState(false)
            },
            height: "auto",
            closeOnOutsideClick: e => !$(e.target).closest(".".concat(DX_ADAPTIVE_HAMBURGER_BUTTON_CLASS)).length,
            position: {
                collision: "flipfit",
                at: "bottom " + position,
                my: "top " + position,
                of: this._hamburger.$element()
            }
        }
    }
    _getTreeViewOptions() {
        var menuOptions = {};
        each(["rtlEnabled", "width", "accessKey", "activeStateEnabled", "animation", "dataSource", "disabled", "displayExpr", "displayExpr", "focusStateEnabled", "hint", "hoverStateEnabled", "itemsExpr", "items", "itemTemplate", "selectedExpr", "selectionMode", "tabIndex", "visible"], (_, option) => {
            menuOptions[option] = this.option(option)
        });
        each(["onItemContextMenu", "onSelectionChanged"], (_, actionName) => {
            menuOptions[actionName] = e => {
                this._actions[actionName](e)
            }
        });
        return extend(menuOptions, {
            dataSource: this.getDataSource(),
            animationEnabled: !!this.option("animation"),
            onItemClick: this._treeviewItemClickHandler.bind(this),
            onItemExpanded: e => {
                this._overlay.repaint();
                this._actions.onSubmenuShown(e)
            },
            onItemCollapsed: e => {
                this._overlay.repaint();
                this._actions.onSubmenuHidden(e)
            },
            selectNodesRecursive: false,
            selectByClick: this.option("selectByClick"),
            expandEvent: "click"
        })
    }
    _initAdaptivity() {
        if (!this._isAdaptivityEnabled()) {
            return
        }
        this._$adaptiveContainer = $("<div>").addClass(DX_ADAPTIVE_MODE_CLASS);
        var $hamburger = this._renderHamburgerButton();
        this._treeView = this._createComponent($("<div>"), TreeView, this._getTreeViewOptions());
        this._overlay = this._createComponent($("<div>"), Overlay, this._getAdaptiveOverlayOptions());
        this._overlay.$content().append(this._treeView.$element()).addClass(DX_ADAPTIVE_MODE_CLASS).addClass(this.option("cssClass"));
        this._overlay.$wrapper().addClass(DX_ADAPTIVE_MODE_OVERLAY_WRAPPER_CLASS);
        this._$adaptiveContainer.append($hamburger);
        this._$adaptiveContainer.append(this._overlay.$element());
        this.$element().append(this._$adaptiveContainer);
        this._updateItemsWidthCache();
        this._dimensionChanged()
    }
    _getDelay(delayType) {
        var delay = this.option("showFirstSubmenuMode").delay;
        if (!isDefined(delay)) {
            return DEFAULT_DELAY[delayType]
        } else {
            return isObject(delay) ? delay[delayType] : delay
        }
    }
    _keyboardHandler(e) {
        return super._keyboardHandler(e, !!this._visibleSubmenu)
    }
    _renderContainer() {
        var $wrapper = $("<div>");
        $wrapper.appendTo(this.$element()).addClass(this._isMenuHorizontal() ? DX_MENU_HORIZONTAL_CLASS : DX_MENU_VERTICAL_CLASS);
        return super._renderContainer($wrapper)
    }
    _renderSubmenuItems(node, $itemFrame) {
        var submenu = this._createSubmenu(node, $itemFrame);
        this._submenus.push(submenu);
        this._renderBorderElement($itemFrame);
        return submenu
    }
    _getKeyboardListeners() {
        return super._getKeyboardListeners().concat(this._visibleSubmenu)
    }
    _createSubmenu(node, $rootItem) {
        var $submenuContainer = $("<div>").addClass(DX_CONTEXT_MENU_CLASS).appendTo($rootItem);
        var items = this._getChildNodes(node);
        var result = this._createComponent($submenuContainer, Submenu, extend(this._getSubmenuOptions(), {
            _dataAdapter: this._dataAdapter,
            _parentKey: node.internalFields.key,
            items: items,
            onHoverStart: this._clearTimeouts.bind(this),
            position: this.getSubmenuPosition($rootItem)
        }));
        this._attachSubmenuHandlers($rootItem, result);
        return result
    }
    _getSubmenuOptions() {
        var $submenuTarget = $("<div>");
        var isMenuHorizontal = this._isMenuHorizontal();
        return {
            itemTemplate: this.option("itemTemplate"),
            target: $submenuTarget,
            orientation: this.option("orientation"),
            selectionMode: this.option("selectionMode"),
            cssClass: this.option("cssClass"),
            selectByClick: this.option("selectByClick"),
            hoverStateEnabled: this.option("hoverStateEnabled"),
            activeStateEnabled: this.option("activeStateEnabled"),
            focusStateEnabled: this.option("focusStateEnabled"),
            animation: this.option("animation"),
            showSubmenuMode: this.option("showSubmenuMode"),
            displayExpr: this.option("displayExpr"),
            disabledExpr: this.option("disabledExpr"),
            selectedExpr: this.option("selectedExpr"),
            itemsExpr: this.option("itemsExpr"),
            onFocusedItemChanged: e => {
                if (!e.component.option("visible")) {
                    return
                }
                this.option("focusedElement", e.component.option("focusedElement"))
            },
            onSelectionChanged: this._nestedItemOnSelectionChangedHandler.bind(this),
            onItemClick: this._nestedItemOnItemClickHandler.bind(this),
            onItemRendered: this._nestedItemOnItemRenderedHandler.bind(this),
            onLeftFirstItem: isMenuHorizontal ? null : this._moveMainMenuFocus.bind(this, PREVITEM_OPERATION),
            onLeftLastItem: isMenuHorizontal ? null : this._moveMainMenuFocus.bind(this, NEXTITEM_OPERATION),
            onCloseRootSubmenu: this._moveMainMenuFocus.bind(this, isMenuHorizontal ? PREVITEM_OPERATION : null),
            onExpandLastSubmenu: isMenuHorizontal ? this._moveMainMenuFocus.bind(this, NEXTITEM_OPERATION) : null
        }
    }
    _getShowFirstSubmenuMode() {
        if (!this._isDesktopDevice()) {
            return "onClick"
        }
        var optionValue = this.option("showFirstSubmenuMode");
        return isObject(optionValue) ? optionValue.name : optionValue
    }
    _moveMainMenuFocus(direction) {
        var $items = this._getAvailableItems();
        var itemCount = $items.length;
        var $currentItem = $items.filter(".".concat(DX_MENU_ITEM_EXPANDED_CLASS)).eq(0);
        var itemIndex = $items.index($currentItem);
        this._hideSubmenu(this._visibleSubmenu);
        itemIndex += direction === PREVITEM_OPERATION ? -1 : 1;
        if (itemIndex >= itemCount) {
            itemIndex = 0
        } else if (itemIndex < 0) {
            itemIndex = itemCount - 1
        }
        var $newItem = $items.eq(itemIndex);
        this.option("focusedElement", getPublicElement($newItem))
    }
    _nestedItemOnSelectionChangedHandler(args) {
        var selectedItem = args.addedItems.length && args.addedItems[0];
        var submenu = Submenu.getInstance(args.element);
        var onSelectionChanged = this._actions.onSelectionChanged;
        onSelectionChanged(args);
        selectedItem && this._clearSelectionInSubmenus(selectedItem[0], submenu);
        this._clearRootSelection();
        this._setOptionWithoutOptionChange("selectedItem", selectedItem)
    }
    _clearSelectionInSubmenus(item, targetSubmenu) {
        var cleanAllSubmenus = !arguments.length;
        each(this._submenus, (index, submenu) => {
            var $submenu = submenu._itemContainer();
            var isOtherItem = !$submenu.is(targetSubmenu && targetSubmenu._itemContainer());
            var $selectedItem = $submenu.find(".".concat(this._selectedItemClass()));
            if (isOtherItem && $selectedItem.length || cleanAllSubmenus) {
                $selectedItem.removeClass(this._selectedItemClass());
                var selectedItemData = this._getItemData($selectedItem);
                if (selectedItemData) {
                    selectedItemData.selected = false
                }
                submenu._clearSelectedItems()
            }
        })
    }
    _clearRootSelection() {
        var $prevSelectedItem = this.$element().find(".".concat(DX_MENU_ITEMS_CONTAINER_CLASS)).first().children().children().filter(".".concat(this._selectedItemClass()));
        if ($prevSelectedItem.length) {
            var prevSelectedItemData = this._getItemData($prevSelectedItem);
            prevSelectedItemData.selected = false;
            $prevSelectedItem.removeClass(this._selectedItemClass())
        }
    }
    _nestedItemOnItemClickHandler(e) {
        this._actions.onItemClick(e)
    }
    _nestedItemOnItemRenderedHandler(e) {
        this._actions.onItemRendered(e)
    }
    _attachSubmenuHandlers($rootItem, submenu) {
        var $submenuOverlayContent = submenu.getOverlayContent();
        var submenus = $submenuOverlayContent.find(".".concat(DX_SUBMENU_CLASS));
        var submenuMouseLeaveName = addNamespace(hoverEventEnd, this.NAME + "_submenu");
        submenu.option({
            onShowing: this._submenuOnShowingHandler.bind(this, $rootItem, submenu),
            onShown: this._submenuOnShownHandler.bind(this, $rootItem, submenu),
            onHiding: this._submenuOnHidingHandler.bind(this, $rootItem, submenu),
            onHidden: this._submenuOnHiddenHandler.bind(this, $rootItem, submenu)
        });
        each(submenus, (index, submenu) => {
            eventsEngine.off(submenu, submenuMouseLeaveName);
            eventsEngine.on(submenu, submenuMouseLeaveName, null, this._submenuMouseLeaveHandler.bind(this, $rootItem))
        })
    }
    _submenuOnShowingHandler($rootItem, submenu) {
        var $border = $rootItem.children(".".concat(DX_CONTEXT_MENU_CONTAINER_BORDER_CLASS));
        this._actions.onSubmenuShowing({
            rootItem: getPublicElement($rootItem),
            submenu: submenu
        });
        $border.show();
        $rootItem.addClass(DX_MENU_ITEM_EXPANDED_CLASS)
    }
    _submenuOnShownHandler($rootItem, submenu) {
        this._actions.onSubmenuShown({
            rootItem: getPublicElement($rootItem),
            submenu: submenu
        })
    }
    _submenuOnHidingHandler($rootItem, submenu, eventArgs) {
        var $border = $rootItem.children(".".concat(DX_CONTEXT_MENU_CONTAINER_BORDER_CLASS));
        var args = eventArgs;
        args.rootItem = getPublicElement($rootItem);
        args.submenu = submenu;
        this._actions.onSubmenuHiding(args);
        eventArgs = args;
        if (!eventArgs.cancel) {
            if (this._visibleSubmenu === submenu) {
                this._visibleSubmenu = null
            }
            $border.hide();
            $rootItem.removeClass(DX_MENU_ITEM_EXPANDED_CLASS)
        }
    }
    _submenuOnHiddenHandler($rootItem, submenu) {
        this._actions.onSubmenuHidden({
            rootItem: getPublicElement($rootItem),
            submenu: submenu
        })
    }
    _submenuMouseLeaveHandler($rootItem, eventArgs) {
        var target = $(eventArgs.relatedTarget).parents(".".concat(DX_CONTEXT_MENU_CLASS))[0];
        var contextMenu = this._getSubmenuByRootElement($rootItem).getOverlayContent()[0];
        if (this.option("hideSubmenuOnMouseLeave") && target !== contextMenu) {
            this._clearTimeouts();
            setTimeout(this._hideSubmenuAfterTimeout.bind(this), this._getDelay("hide"))
        }
    }
    _hideSubmenuAfterTimeout() {
        if (!this._visibleSubmenu) {
            return
        }
        var isRootItemHovered = $(this._visibleSubmenu.$element().context).hasClass(DX_STATE_HOVER_CLASS);
        var isSubmenuItemHovered = this._visibleSubmenu.getOverlayContent().find(".".concat(DX_STATE_HOVER_CLASS)).length;
        var hoveredElementFromSubMenu = this._visibleSubmenu.getOverlayContent().get(0).querySelector(":hover");
        if (!hoveredElementFromSubMenu && !isSubmenuItemHovered && !isRootItemHovered) {
            this._visibleSubmenu.hide()
        }
    }
    _getSubmenuByRootElement($rootItem) {
        if (!$rootItem) {
            return false
        }
        var $submenu = $rootItem.children(".".concat(DX_CONTEXT_MENU_CLASS));
        return $submenu.length && Submenu.getInstance($submenu)
    }
    getSubmenuPosition($rootItem) {
        var isHorizontalMenu = this._isMenuHorizontal();
        var submenuDirection = this.option("submenuDirection").toLowerCase();
        var rtlEnabled = this.option("rtlEnabled");
        var submenuPosition = {
            collision: "flip",
            of: $rootItem
        };
        switch (submenuDirection) {
            case "leftortop":
                submenuPosition.at = "left top";
                submenuPosition.my = isHorizontalMenu ? "left bottom" : "right top";
                break;
            case "rightorbottom":
                submenuPosition.at = isHorizontalMenu ? "left bottom" : "right top";
                submenuPosition.my = "left top";
                break;
            default:
                if (isHorizontalMenu) {
                    submenuPosition.at = rtlEnabled ? "right bottom" : "left bottom";
                    submenuPosition.my = rtlEnabled ? "right top" : "left top"
                } else {
                    submenuPosition.at = rtlEnabled ? "left top" : "right top";
                    submenuPosition.my = rtlEnabled ? "right top" : "left top"
                }
        }
        return submenuPosition
    }
    _renderBorderElement($item) {
        $("<div>").appendTo($item).addClass(DX_CONTEXT_MENU_CONTAINER_BORDER_CLASS).hide()
    }
    _itemPointerDownHandler(e) {
        var $target = $(e.target);
        var $closestItem = $target.closest(this._itemElements());
        if ($closestItem.hasClass("dx-menu-item-has-submenu")) {
            this.option("focusedElement", null);
            return
        }
        super._itemPointerDownHandler(e)
    }
    _hoverStartHandler(e) {
        var mouseMoveEventName = addNamespace(pointerEvents.move, this.NAME);
        var $item = this._getItemElementByEventArgs(e);
        var node = this._dataAdapter.getNodeByItem(this._getItemData($item));
        var isSelectionActive = isDefined(e.buttons) && 1 === e.buttons || !isDefined(e.buttons) && 1 === e.which;
        if (this._isItemDisabled($item)) {
            return
        }
        eventsEngine.off($item, mouseMoveEventName);
        if (!this._hasChildren(node)) {
            this._showSubmenuTimer = setTimeout(this._hideSubmenuAfterTimeout.bind(this), this._getDelay("hide"));
            return
        }
        if ("onHover" === this._getShowFirstSubmenuMode() && !isSelectionActive) {
            var submenu = this._getSubmenuByElement($item);
            this._clearTimeouts();
            if (!submenu.isOverlayVisible()) {
                eventsEngine.on($item, mouseMoveEventName, this._itemMouseMoveHandler.bind(this));
                this._showSubmenuTimer = this._getDelay("hide")
            }
        }
    }
    _hoverEndHandler(eventArg) {
        var $item = this._getItemElementByEventArgs(eventArg);
        var relatedTarget = $(eventArg.relatedTarget);
        super._hoverEndHandler(eventArg);
        this._clearTimeouts();
        if (this._isItemDisabled($item)) {
            return
        }
        if (relatedTarget.hasClass(DX_CONTEXT_MENU_CONTENT_DELIMITER_CLASS)) {
            return
        }
        if (this.option("hideSubmenuOnMouseLeave") && !relatedTarget.hasClass(DX_MENU_ITEMS_CONTAINER_CLASS)) {
            this._hideSubmenuTimer = setTimeout(() => {
                this._hideSubmenuAfterTimeout()
            }, this._getDelay("hide"))
        }
    }
    _hideVisibleSubmenu() {
        if (!this._visibleSubmenu) {
            return false
        }
        this._hideSubmenu(this._visibleSubmenu);
        return true
    }
    _showSubmenu($itemElement) {
        var submenu = this._getSubmenuByElement($itemElement);
        if (this._visibleSubmenu !== submenu) {
            this._hideVisibleSubmenu()
        }
        if (submenu) {
            this._clearTimeouts();
            submenu.show();
            this.option("focusedElement", submenu.option("focusedElement"))
        }
        this._visibleSubmenu = submenu;
        this._hoveredRootItem = $itemElement
    }
    _hideSubmenu(submenu) {
        submenu && submenu.hide();
        if (this._visibleSubmenu === submenu) {
            this._visibleSubmenu = null
        }
        this._hoveredRootItem = null
    }
    _itemMouseMoveHandler(e) {
        if (e.pointers && e.pointers.length) {
            return
        }
        var $item = $(e.currentTarget);
        if (!isDefined(this._showSubmenuTimer)) {
            return
        }
        this._clearTimeouts();
        this._showSubmenuTimer = setTimeout(() => {
            var submenu = this._getSubmenuByElement($item);
            if (submenu && !submenu.isOverlayVisible()) {
                this._showSubmenu($item)
            }
        }, this._getDelay("show"))
    }
    _clearTimeouts() {
        clearTimeout(this._hideSubmenuTimer);
        clearTimeout(this._showSubmenuTimer)
    }
    _getSubmenuByElement($itemElement, itemData) {
        var submenu = this._getSubmenuByRootElement($itemElement);
        if (submenu) {
            return submenu
        } else {
            itemData = itemData || this._getItemData($itemElement);
            var node = this._dataAdapter.getNodeByItem(itemData);
            return this._hasChildren(node) && this._renderSubmenuItems(node, $itemElement)
        }
    }
    _updateSubmenuVisibilityOnClick(actionArgs) {
        var args = actionArgs.args.length && actionArgs.args[0];
        if (!args || this._disabledGetter(args.itemData)) {
            return
        }
        var $itemElement = $(args.itemElement);
        var currentSubmenu = this._getSubmenuByElement($itemElement, args.itemData);
        this._updateSelectedItemOnClick(actionArgs);
        if (this._visibleSubmenu) {
            if (this._visibleSubmenu === currentSubmenu) {
                if ("onClick" === this.option("showFirstSubmenuMode")) {
                    this._hideSubmenu(this._visibleSubmenu)
                }
                return
            } else {
                this._hideSubmenu(this._visibleSubmenu)
            }
        }
        if (!currentSubmenu) {
            return
        }
        if (!currentSubmenu.isOverlayVisible()) {
            this._showSubmenu($itemElement);
            return
        }
    }
    _optionChanged(args) {
        if (ACTIONS.indexOf(args.name) >= 0) {
            this._initActions();
            return
        }
        switch (args.name) {
            case "orientation":
            case "submenuDirection":
                this._invalidate();
                break;
            case "showFirstSubmenuMode":
            case "hideSubmenuOnMouseLeave":
                break;
            case "showSubmenuMode":
                this._changeSubmenusOption(args.name, args.value);
                break;
            case "adaptivityEnabled":
                args.value ? this._initAdaptivity() : this._removeAdaptivity();
                break;
            case "width":
                if (this._isAdaptivityEnabled()) {
                    this._treeView.option(args.name, args.value);
                    this._overlay.option(args.name, args.value)
                }
                super._optionChanged(args);
                this._dimensionChanged();
                break;
            case "animation":
                if (this._isAdaptivityEnabled()) {
                    this._treeView.option("animationEnabled", !!args.value)
                }
                super._optionChanged(args);
                break;
            default:
                if (this._isAdaptivityEnabled() && (args.name === args.fullName || "items" === args.name)) {
                    this._treeView.option(args.fullName, args.value)
                }
                super._optionChanged(args)
        }
    }
    _changeSubmenusOption(name, value) {
        each(this._submenus, (index, submenu) => {
            submenu.option(name, value)
        })
    }
    selectItem(itemElement) {
        this._hideSubmenu(this._visibleSubmenu);
        super.selectItem(itemElement)
    }
    unselectItem(itemElement) {
        this._hideSubmenu(this._visibleSubmenu);
        super.selectItem(itemElement)
    }
}
registerComponent("dxMenu", Menu);
export default Menu;
