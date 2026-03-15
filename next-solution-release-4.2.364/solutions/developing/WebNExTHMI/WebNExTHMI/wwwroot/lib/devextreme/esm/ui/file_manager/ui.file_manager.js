/**
 * DevExtreme (esm/ui/file_manager/ui.file_manager.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import $ from "../../core/renderer";
import {
    extend
} from "../../core/utils/extend";
import {
    isDefined,
    isEmptyObject,
    isFunction,
    type
} from "../../core/utils/type";
import {
    Deferred,
    when
} from "../../core/utils/deferred";
import {
    ensureDefined,
    equalByValue
} from "../../core/utils/common";
import messageLocalization from "../../localization/message";
import registerComponent from "../../core/component_registrator";
import Widget from "../widget/ui.widget";
import notify from "../notify";
import {
    findItemsByKeys,
    extendAttributes
} from "./ui.file_manager.common";
import FileItemsController from "./file_items_controller";
import {
    defaultPermissions,
    FileManagerCommandManager
} from "./ui.file_manager.command_manager";
import FileManagerContextMenu from "./ui.file_manager.context_menu";
import FileManagerFilesTreeView from "./ui.file_manager.files_tree_view";
import FileManagerDetailsItemList from "./ui.file_manager.item_list.details";
import FileManagerThumbnailsItemList from "./ui.file_manager.item_list.thumbnails";
import FileManagerToolbar from "./ui.file_manager.toolbar";
import FileManagerNotificationControl from "./ui.file_manager.notification";
import FileManagerEditingControl from "./ui.file_manager.editing";
import FileManagerBreadcrumbs from "./ui.file_manager.breadcrumbs";
import FileManagerAdaptivityControl from "./ui.file_manager.adaptivity";
import {
    normalizeOptions
} from "../../core/options/utils";
import {
    equals
} from "../../core/utils/comparator";
var FILE_MANAGER_CLASS = "dx-filemanager";
var FILE_MANAGER_WRAPPER_CLASS = FILE_MANAGER_CLASS + "-wrapper";
var FILE_MANAGER_CONTAINER_CLASS = FILE_MANAGER_CLASS + "-container";
var FILE_MANAGER_DIRS_PANEL_CLASS = FILE_MANAGER_CLASS + "-dirs-panel";
var FILE_MANAGER_EDITING_CONTAINER_CLASS = FILE_MANAGER_CLASS + "-editing-container";
var FILE_MANAGER_ITEMS_PANEL_CLASS = FILE_MANAGER_CLASS + "-items-panel";
var FILE_MANAGER_ITEM_CUSTOM_THUMBNAIL_CLASS = FILE_MANAGER_CLASS + "-item-custom-thumbnail";
var PARENT_DIRECTORY_KEY_PREFIX = "[*DXPDK*]$40F96F03-FBD8-43DF-91BE-F55F4B8BA871$";
var VIEW_AREAS = {
    folders: "navPane",
    items: "itemView"
};
class FileManager extends Widget {
    _initTemplates() {}
    _init() {
        super._init();
        this._initActions();
        this._providerUpdateDeferred = null;
        this._lockCurrentPathProcessing = false;
        this._controller = new FileItemsController({
            currentPath: this.option("currentPath"),
            currentPathKeys: this.option("currentPathKeys"),
            rootText: this.option("rootFolderName"),
            fileProvider: this.option("fileSystemProvider"),
            allowedFileExtensions: this.option("allowedFileExtensions"),
            uploadMaxFileSize: this.option("upload").maxFileSize,
            uploadChunkSize: this.option("upload").chunkSize,
            onInitialized: this._onControllerInitialized.bind(this),
            onDataLoading: this._onDataLoading.bind(this),
            onSelectedDirectoryChanged: this._onSelectedDirectoryChanged.bind(this),
            onPathPotentiallyChanged: this._checkPathActuality.bind(this),
            editingEvents: this._actions.editing
        })
    }
    _initMarkup() {
        super._initMarkup();
        this._firstItemViewLoad = true;
        this._lockSelectionProcessing = false;
        this._lockFocusedItemProcessing = false;
        this._itemKeyToFocus = void 0;
        this._commandManager = new FileManagerCommandManager(this.option("permissions"));
        this.$element().addClass(FILE_MANAGER_CLASS);
        this._createNotificationControl();
        this._initCommandManager()
    }
    _createNotificationControl() {
        var $notificationControl = $("<div>").addClass("dx-filemanager-notification-container").appendTo(this.$element());
        this._notificationControl = this._createComponent($notificationControl, FileManagerNotificationControl, {
            progressPanelContainer: this.$element(),
            contentTemplate: (container, notificationControl) => this._createWrapper(container, notificationControl),
            onActionProgress: e => this._onActionProgress(e),
            positionTarget: ".".concat(FILE_MANAGER_CONTAINER_CLASS),
            showProgressPanel: this.option("notifications.showPanel"),
            showNotificationPopup: this.option("notifications.showPopup")
        })
    }
    _createWrapper(container, notificationControl) {
        this._$wrapper = $("<div>").addClass(FILE_MANAGER_WRAPPER_CLASS).appendTo(container);
        this._createEditing(notificationControl);
        var $toolbar = $("<div>").appendTo(this._$wrapper);
        this._toolbar = this._createComponent($toolbar, FileManagerToolbar, {
            commandManager: this._commandManager,
            generalItems: this.option("toolbar.items"),
            fileItems: this.option("toolbar.fileSelectionItems"),
            itemViewMode: this.option("itemView").mode,
            onItemClick: args => this._actions.onToolbarItemClick(args)
        });
        this._createAdaptivityControl()
    }
    _createAdaptivityControl() {
        var $container = $("<div>").addClass(FILE_MANAGER_CONTAINER_CLASS).appendTo(this._$wrapper);
        this._adaptivityControl = this._createComponent($container, FileManagerAdaptivityControl, {
            drawerTemplate: container => this._createFilesTreeView(container),
            contentTemplate: container => this._createItemsPanel(container),
            onAdaptiveStateChanged: e => this._onAdaptiveStateChanged(e)
        });
        this._editing.setUploaderSplitterElement(this._adaptivityControl.getSplitterElement())
    }
    _createEditing(notificationControl) {
        var $editingContainer = $("<div>").addClass(FILE_MANAGER_EDITING_CONTAINER_CLASS).appendTo(this.$element());
        this._editing = this._createComponent($editingContainer, FileManagerEditingControl, {
            controller: this._controller,
            model: {
                getMultipleSelectedItems: this._getSelectedItemInfos.bind(this)
            },
            getItemThumbnail: this._getItemThumbnailInfo.bind(this),
            notificationControl: notificationControl,
            uploadDropZonePlaceholderContainer: this.$element(),
            rtlEnabled: this.option("rtlEnabled"),
            onSuccess: _ref => {
                var {
                    updatedOnlyFiles: updatedOnlyFiles
                } = _ref;
                return this._redrawComponent(updatedOnlyFiles)
            },
            onError: e => this._onEditingError(e)
        })
    }
    _createItemsPanel($container) {
        this._$itemsPanel = $("<div>").addClass(FILE_MANAGER_ITEMS_PANEL_CLASS).appendTo($container);
        this._createBreadcrumbs(this._$itemsPanel);
        this._createItemView(this._$itemsPanel);
        this._updateUploadDropZone()
    }
    _updateUploadDropZone() {
        var dropZone = this._commandManager.isCommandAvailable("upload") ? this._$itemsPanel : $();
        this._editing.setUploaderDropZone(dropZone)
    }
    _createFilesTreeView(container) {
        this._filesTreeViewContextMenu = this._createContextMenu(false, VIEW_AREAS.folders);
        var $filesTreeView = $("<div>").addClass(FILE_MANAGER_DIRS_PANEL_CLASS).appendTo(container);
        this._filesTreeView = this._createComponent($filesTreeView, FileManagerFilesTreeView, {
            storeExpandedState: true,
            contextMenu: this._filesTreeViewContextMenu,
            getDirectories: this.getDirectories.bind(this),
            getCurrentDirectory: this._getCurrentDirectory.bind(this),
            onDirectoryClick: _ref2 => {
                var {
                    itemData: itemData
                } = _ref2;
                return this._setCurrentDirectory(itemData)
            }
        });
        this._filesTreeView.updateCurrentDirectory()
    }
    _createItemView($container, viewMode) {
        this._itemViewContextMenu = this._createContextMenu(true, VIEW_AREAS.items);
        var itemViewOptions = this.option("itemView");
        var options = {
            selectionMode: this.option("selectionMode"),
            selectedItemKeys: this.option("selectedItemKeys"),
            focusedItemKey: this.option("focusedItemKey"),
            contextMenu: this._itemViewContextMenu,
            getItems: this._getItemViewItems.bind(this),
            onError: _ref3 => {
                var {
                    error: error
                } = _ref3;
                return this._showError(error)
            },
            onSelectionChanged: this._onItemViewSelectionChanged.bind(this),
            onFocusedItemChanged: this._onItemViewFocusedItemChanged.bind(this),
            onSelectedItemOpened: this._onSelectedItemOpened.bind(this),
            onContextMenuShowing: e => this._onContextMenuShowing(VIEW_AREAS.items, e),
            getItemThumbnail: this._getItemThumbnailInfo.bind(this),
            customizeDetailColumns: this.option("customizeDetailColumns"),
            detailColumns: this.option("itemView.details.columns")
        };
        var $itemView = $("<div>").appendTo($container);
        viewMode = viewMode || itemViewOptions.mode;
        var widgetClass = "thumbnails" === viewMode ? FileManagerThumbnailsItemList : FileManagerDetailsItemList;
        this._itemView = this._createComponent($itemView, widgetClass, options)
    }
    _createBreadcrumbs($container) {
        var $breadcrumbs = $("<div>").appendTo($container);
        this._breadcrumbs = this._createComponent($breadcrumbs, FileManagerBreadcrumbs, {
            rootFolderDisplayName: this.option("rootFolderName"),
            onCurrentDirectoryChanging: _ref4 => {
                var {
                    currentDirectory: currentDirectory
                } = _ref4;
                return this._setCurrentDirectory(currentDirectory, true)
            }
        });
        this._breadcrumbs.setCurrentDirectory(this._getCurrentDirectory())
    }
    _createContextMenu(isolateCreationItemCommands, viewArea) {
        var $contextMenu = $("<div>").appendTo(this._$wrapper);
        return this._createComponent($contextMenu, FileManagerContextMenu, {
            commandManager: this._commandManager,
            items: this.option("contextMenu.items"),
            onItemClick: args => this._actions.onContextMenuItemClick(args),
            onContextMenuShowing: e => this._onContextMenuShowing(viewArea, e),
            isolateCreationItemCommands: isolateCreationItemCommands,
            viewArea: viewArea
        })
    }
    _initCommandManager() {
        var actions = extend(this._editing.getCommandActions(), {
            refresh: () => this._refreshAndShowProgress(),
            thumbnails: () => this.option("itemView.mode", "thumbnails"),
            details: () => this.option("itemView.mode", "details"),
            clearSelection: () => this._clearSelection(),
            showNavPane: () => this._adaptivityControl.toggleDrawer()
        });
        this._commandManager.registerActions(actions)
    }
    _onItemViewSelectionChanged(_ref5) {
        var {
            selectedItemInfos: selectedItemInfos,
            selectedItems: selectedItems,
            selectedItemKeys: selectedItemKeys,
            currentSelectedItemKeys: currentSelectedItemKeys,
            currentDeselectedItemKeys: currentDeselectedItemKeys
        } = _ref5;
        this._lockSelectionProcessing = true;
        this.option("selectedItemKeys", selectedItemKeys);
        this._lockSelectionProcessing = false;
        this._actions.onSelectionChanged({
            selectedItems: selectedItems,
            selectedItemKeys: selectedItemKeys,
            currentSelectedItemKeys: currentSelectedItemKeys,
            currentDeselectedItemKeys: currentDeselectedItemKeys
        });
        this._updateToolbar(selectedItemInfos)
    }
    _onItemViewFocusedItemChanged(e) {
        this._lockFocusedItemProcessing = true;
        this.option("focusedItemKey", e.itemKey);
        this._lockFocusedItemProcessing = false;
        this._actions.onFocusedItemChanged({
            item: e.item,
            itemElement: e.itemElement
        })
    }
    _onAdaptiveStateChanged(_ref6) {
        var {
            enabled: enabled
        } = _ref6;
        this._commandManager.setCommandEnabled("showNavPane", enabled);
        this._updateToolbar()
    }
    _onActionProgress(_ref7) {
        var {
            message: message,
            status: status
        } = _ref7;
        this._toolbar.updateRefreshItem(message, status);
        this._updateToolbar()
    }
    _onEditingError(e) {
        var args = extendAttributes({}, e, ["errorCode", "errorText", "fileSystemItem"]);
        this._actions.onErrorOccurred(args);
        e.errorText = args.errorText
    }
    _refreshAndShowProgress() {
        return when(this._notificationControl.tryShowProgressPanel(), this._controller.refresh()).then(() => this._filesTreeView.refresh())
    }
    _updateToolbar(selectedItems) {
        var items = selectedItems || this._getSelectedItemInfos();
        this._toolbar.option("contextItems", ensureDefined(items, []))
    }
    _switchView(viewMode) {
        this._disposeWidget(this._itemView.option("contextMenu"));
        this._disposeWidget(this._itemView);
        this._createItemView(this._$itemsPanel, viewMode);
        this._toolbar.option({
            itemViewMode: viewMode
        })
    }
    _disposeWidget(widget) {
        widget.dispose();
        widget.$element().remove()
    }
    _clearSelection() {
        this._itemView.clearSelection()
    }
    _showError(message) {
        this._showNotification(message, false)
    }
    _showNotification(message, isSuccess) {
        notify({
            message: message,
            width: 450
        }, isSuccess ? "success" : "error", 5e3)
    }
    _redrawComponent(onlyFileItemsView) {
        this._itemView.refresh().then(() => !onlyFileItemsView && this._filesTreeView.refresh())
    }
    _getItemViewItems() {
        var showFolders = this.option("itemView").showFolders;
        var result = this._controller.getCurrentItems(!showFolders);
        this._updateToolbarWithSelectionOnFirstLoad(result);
        if (this.option("itemView.showParentFolder")) {
            result = when(result).then(items => this._getPreparedItemViewItems(items))
        }
        return result
    }
    _updateToolbarWithSelectionOnFirstLoad(itemsResult) {
        if (!this._firstItemViewLoad) {
            return
        }
        this._firstItemViewLoad = false;
        var selectedItemKeys = this.option("selectedItemKeys");
        if (selectedItemKeys.length > 0) {
            when(itemsResult).done(items => {
                var selectedItems = findItemsByKeys(items, selectedItemKeys);
                if (selectedItems.length > 0) {
                    this._updateToolbar(selectedItems)
                }
            })
        }
    }
    _getPreparedItemViewItems(items) {
        var selectedDir = this._getCurrentDirectory();
        if (selectedDir.fileItem.isRoot()) {
            return items
        }
        var parentDirItem = selectedDir.fileItem.createClone();
        parentDirItem.isParentFolder = true;
        parentDirItem.name = "..";
        parentDirItem.relativeName = "..";
        parentDirItem.key = "".concat(PARENT_DIRECTORY_KEY_PREFIX).concat(selectedDir.fileItem.key);
        var itemsCopy = [...items];
        itemsCopy.unshift({
            fileItem: parentDirItem,
            icon: "parentfolder"
        });
        return itemsCopy
    }
    _onContextMenuShowing(viewArea, e) {
        var _e$itemData;
        var eventArgs = extendAttributes({}, e, ["targetElement", "cancel", "event"]);
        eventArgs = extend(eventArgs, {
            viewArea: viewArea,
            fileSystemItem: null === (_e$itemData = e.itemData) || void 0 === _e$itemData ? void 0 : _e$itemData.fileItem,
            _isActionButton: e.isActionButton
        });
        this._actions.onContextMenuShowing(eventArgs);
        e.cancel = ensureDefined(eventArgs.cancel, false)
    }
    _getItemThumbnailInfo(fileInfo) {
        var func = this.option("customizeThumbnail");
        var thumbnail = isFunction(func) ? func(fileInfo.fileItem) : fileInfo.fileItem.thumbnail;
        if (thumbnail) {
            return {
                thumbnail: thumbnail,
                cssClass: FILE_MANAGER_ITEM_CUSTOM_THUMBNAIL_CLASS
            }
        }
        return {
            thumbnail: fileInfo.icon
        }
    }
    _getDefaultOptions() {
        return extend(super._getDefaultOptions(), {
            fileSystemProvider: null,
            currentPath: "",
            currentPathKeys: [],
            rootFolderName: messageLocalization.format("dxFileManager-rootDirectoryName"),
            selectionMode: "multiple",
            selectedItemKeys: [],
            focusedItemKey: void 0,
            toolbar: {
                items: ["showNavPane", "create", "upload", "switchView", {
                    name: "separator",
                    location: "after"
                }, "refresh"],
                fileSelectionItems: ["download", "separator", "move", "copy", "rename", "separator", "delete", "clearSelection", {
                    name: "separator",
                    location: "after"
                }, "refresh"]
            },
            contextMenu: {
                items: ["create", "upload", "rename", "move", "copy", "delete", "refresh", "download"]
            },
            itemView: {
                details: {
                    columns: ["thumbnail", "name", "dateModified", "size"]
                },
                mode: "details",
                showFolders: true,
                showParentFolder: true
            },
            customizeThumbnail: null,
            customizeDetailColumns: null,
            onContextMenuItemClick: null,
            onContextMenuShowing: null,
            onCurrentDirectoryChanged: null,
            onSelectedFileOpened: null,
            onSelectionChanged: null,
            onFocusedItemChanged: null,
            onToolbarItemClick: null,
            onErrorOccurred: null,
            onDirectoryCreating: null,
            onDirectoryCreated: null,
            onItemRenaming: null,
            onItemRenamed: null,
            onItemDeleting: null,
            onItemDeleted: null,
            onItemCopying: null,
            onItemCopied: null,
            onItemMoving: null,
            onItemMoved: null,
            onFileUploading: null,
            onFileUploaded: null,
            onItemDownloading: null,
            allowedFileExtensions: [],
            upload: {
                maxFileSize: 0,
                chunkSize: 2e5
            },
            permissions: extend({}, defaultPermissions),
            notifications: {
                showPanel: true,
                showPopup: true
            }
        })
    }
    option(options, value) {
        var optionsToCheck = normalizeOptions(options, value);
        var isGetter = arguments.length < 2 && "object" !== type(options);
        var isOptionDefined = name => isDefined(optionsToCheck[name]);
        var isOptionValueDiffers = name => {
            if (!isOptionDefined(name)) {
                return false
            }
            var previousValue = this.option(name);
            var value = optionsToCheck[name];
            return !equals(previousValue, value)
        };
        if (!isGetter && isOptionDefined("fileSystemProvider")) {
            this._providerUpdateDeferred = new Deferred;
            if (isOptionValueDiffers("currentPath") || isOptionValueDiffers("currentPathKeys")) {
                this._lockCurrentPathProcessing = true
            }
        }
        return super.option(...arguments)
    }
    _optionChanged(args) {
        var name = args.name;
        switch (name) {
            case "currentPath":
                var updateFunc = () => {
                    this._lockCurrentPathProcessing = false;
                    return this._controller.setCurrentPath(args.value)
                };
                this._lockCurrentPathProcessing = true;
                this._providerUpdateDeferred ? this._providerUpdateDeferred.then(updateFunc) : updateFunc();
                break;
            case "currentPathKeys":
                var _updateFunc = () => {
                    this._lockCurrentPathProcessing = false;
                    return this._controller.setCurrentPathByKeys(args.value)
                };
                this._lockCurrentPathProcessing = true;
                this._providerUpdateDeferred ? this._providerUpdateDeferred.then(_updateFunc) : _updateFunc();
                break;
            case "selectedItemKeys":
                if (!this._lockSelectionProcessing && this._itemView) {
                    this._itemView.option("selectedItemKeys", args.value)
                }
                break;
            case "focusedItemKey":
                if (!this._lockFocusedItemProcessing && this._itemView) {
                    this._itemView.option("focusedItemKey", args.value)
                }
                break;
            case "rootFolderName":
                this._controller.setRootText(args.value);
                this._invalidate();
                break;
            case "fileSystemProvider":
                if (!this._lockCurrentPathProcessing) {
                    this._providerUpdateDeferred = new Deferred
                }
                var pathKeys = this._lockCurrentPathProcessing ? void 0 : this.option("currentPathKeys");
                this._controller.updateProvider(args.value, pathKeys).then(() => this._providerUpdateDeferred.resolve()).always(() => {
                    this._providerUpdateDeferred = null;
                    this.repaint()
                });
                break;
            case "allowedFileExtensions":
                this._controller.setAllowedFileExtensions(args.value);
                this._invalidate();
                break;
            case "upload":
                this._controller.setUploadOptions(this.option("upload"));
                this._invalidate();
                break;
            case "permissions":
                this._commandManager.updatePermissions(this.option("permissions"));
                this._filesTreeViewContextMenu.tryUpdateVisibleContextMenu();
                this._itemViewContextMenu.tryUpdateVisibleContextMenu();
                this._toolbar.updateItemPermissions();
                this._updateUploadDropZone();
                break;
            case "selectionMode":
            case "customizeThumbnail":
            case "customizeDetailColumns":
                this._invalidate();
                break;
            case "itemView":
                if ("itemView.mode" === args.fullName) {
                    this._switchView(args.value)
                } else {
                    this._invalidate()
                }
                break;
            case "toolbar":
                var toolbarOptions = {};
                if ("toolbar" === args.fullName) {
                    if (args.value.items) {
                        toolbarOptions.generalItems = args.value.items
                    }
                    if (args.value.fileSelectionItems) {
                        toolbarOptions.fileItems = args.value.fileSelectionItems
                    }
                }
                if (0 === args.fullName.indexOf("toolbar.items")) {
                    toolbarOptions.generalItems = this.option("toolbar.items")
                }
                if (0 === args.fullName.indexOf("toolbar.fileSelectionItems")) {
                    toolbarOptions.fileItems = this.option("toolbar.fileSelectionItems")
                }
                this._toolbar.option(toolbarOptions);
                break;
            case "contextMenu":
                if ("contextMenu" === args.fullName && args.value.items || 0 === args.fullName.indexOf("contextMenu.items")) {
                    var contextMenuItems = this.option("contextMenu.items");
                    this._filesTreeViewContextMenu.option("items", contextMenuItems);
                    this._itemViewContextMenu.option("items", contextMenuItems)
                }
                break;
            case "notifications":
                this._notificationControl.option("showProgressPanel", this.option("notifications.showPanel"));
                this._notificationControl.option("showNotificationPopup", this.option("notifications.showPopup"));
                break;
            case "onContextMenuItemClick":
            case "onContextMenuShowing":
            case "onCurrentDirectoryChanged":
            case "onSelectedFileOpened":
            case "onSelectionChanged":
            case "onFocusedItemChanged":
            case "onToolbarItemClick":
            case "onErrorOccurred":
                this._actions[name] = this._createActionByOption(name);
                break;
            case "onDirectoryCreating":
            case "onDirectoryCreated":
            case "onItemRenaming":
            case "onItemRenamed":
            case "onItemDeleting":
            case "onItemDeleted":
            case "onItemCopying":
            case "onItemCopied":
            case "onItemMoving":
            case "onItemMoved":
            case "onFileUploading":
            case "onFileUploaded":
            case "onItemDownloading":
                this._actions.editing[name] = this._createActionByOption(name);
                break;
            case "rtlEnabled":
                this._editing.updateDialogRtl(args.value);
                super._optionChanged(args);
                break;
            default:
                super._optionChanged(args)
        }
    }
    _initActions() {
        this._actions = {
            onContextMenuItemClick: this._createActionByOption("onContextMenuItemClick"),
            onContextMenuShowing: this._createActionByOption("onContextMenuShowing"),
            onCurrentDirectoryChanged: this._createActionByOption("onCurrentDirectoryChanged"),
            onSelectedFileOpened: this._createActionByOption("onSelectedFileOpened"),
            onSelectionChanged: this._createActionByOption("onSelectionChanged"),
            onFocusedItemChanged: this._createActionByOption("onFocusedItemChanged"),
            onToolbarItemClick: this._createActionByOption("onToolbarItemClick"),
            onErrorOccurred: this._createActionByOption("onErrorOccurred"),
            editing: {
                onDirectoryCreating: this._createActionByOption("onDirectoryCreating"),
                onDirectoryCreated: this._createActionByOption("onDirectoryCreated"),
                onItemRenaming: this._createActionByOption("onItemRenaming"),
                onItemRenamed: this._createActionByOption("onItemRenamed"),
                onItemDeleting: this._createActionByOption("onItemDeleting"),
                onItemDeleted: this._createActionByOption("onItemDeleted"),
                onItemCopying: this._createActionByOption("onItemCopying"),
                onItemCopied: this._createActionByOption("onItemCopied"),
                onItemMoving: this._createActionByOption("onItemMoving"),
                onItemMoved: this._createActionByOption("onItemMoved"),
                onFileUploading: this._createActionByOption("onFileUploading"),
                onFileUploaded: this._createActionByOption("onFileUploaded"),
                onItemDownloading: this._createActionByOption("onItemDownloading")
            }
        }
    }
    executeCommand(commandName) {
        return this._commandManager.executeCommand(commandName)
    }
    _setCurrentDirectory(directoryInfo, checkActuality) {
        this._controller.setCurrentDirectory(directoryInfo, checkActuality)
    }
    _getCurrentDirectory() {
        return this._controller.getCurrentDirectory()
    }
    _onControllerInitialized(_ref8) {
        var {
            controller: controller
        } = _ref8;
        this._controller = this._controller || controller;
        this._syncToCurrentDirectory()
    }
    _onDataLoading(_ref9) {
        var {
            operation: operation
        } = _ref9;
        var options = null;
        if ("navigation" === operation) {
            options = {
                focusedItemKey: this._itemKeyToFocus,
                selectedItemKeys: this.option("selectedItemKeys")
            };
            this._itemKeyToFocus = void 0
        }
        this._itemView.refresh(options)
    }
    _onSelectedDirectoryChanged() {
        var currentDirectory = this._getCurrentDirectory();
        this._syncToCurrentDirectory();
        this._actions.onCurrentDirectoryChanged({
            directory: currentDirectory.fileItem
        })
    }
    _syncToCurrentDirectory() {
        var currentDirectory = this._getCurrentDirectory();
        if (this._filesTreeView) {
            this._filesTreeView.updateCurrentDirectory()
        }
        if (this._breadcrumbs) {
            this._breadcrumbs.setCurrentDirectory(currentDirectory)
        }
        this._checkPathActuality()
    }
    _checkPathActuality() {
        if (this._lockCurrentPathProcessing) {
            return
        }
        var currentPath = this._controller.getCurrentPath();
        var currentPathKeys = this._controller.getCurrentPathKeys();
        var options = {};
        if (this.option("currentPath") !== currentPath) {
            options.currentPath = currentPath
        }
        if (!equalByValue(this.option("currentPathKeys"), currentPathKeys)) {
            options.currentPathKeys = currentPathKeys
        }
        if (!isEmptyObject(options)) {
            this.option(options)
        }
    }
    getDirectories(parentDirectoryInfo, skipNavigationOnError) {
        return this._controller.getDirectories(parentDirectoryInfo, skipNavigationOnError)
    }
    _getSelectedItemInfos() {
        return this._itemView ? this._itemView.getSelectedItems() : []
    }
    refresh() {
        return this.executeCommand("refresh")
    }
    getCurrentDirectory() {
        var directoryInfo = this._getCurrentDirectory();
        return directoryInfo && directoryInfo.fileItem || null
    }
    getSelectedItems() {
        return this._getSelectedItemInfos().map(itemInfo => itemInfo.fileItem)
    }
    _onSelectedItemOpened(_ref10) {
        var {
            fileItemInfo: fileItemInfo
        } = _ref10;
        var fileItem = fileItemInfo.fileItem;
        if (!fileItem.isDirectory) {
            this._actions.onSelectedFileOpened({
                file: fileItem
            });
            return
        }
        if (fileItem.isParentFolder) {
            this._itemKeyToFocus = this._getCurrentDirectory().fileItem.key
        }
        var newCurrentDirectory = fileItem.isParentFolder ? this._getCurrentDirectory().parentDirectory : fileItemInfo;
        this._setCurrentDirectory(newCurrentDirectory);
        if (newCurrentDirectory) {
            this._filesTreeView.toggleDirectoryExpandedState(newCurrentDirectory.parentDirectory, true)
        }
    }
}
registerComponent("dxFileManager", FileManager);
export default FileManager;
