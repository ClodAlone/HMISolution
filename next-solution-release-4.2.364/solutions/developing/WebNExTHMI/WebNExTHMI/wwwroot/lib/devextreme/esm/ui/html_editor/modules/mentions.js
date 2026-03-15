/**
 * DevExtreme (esm/ui/html_editor/modules/mentions.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import $ from "../../../core/renderer";
import Quill from "devextreme-quill";
import {
    compileGetter
} from "../../../core/utils/data";
import {
    isString
} from "../../../core/utils/type";
import {
    extend
} from "../../../core/utils/extend";
import {
    getPublicElement
} from "../../../core/element";
import eventsEngine from "../../../events/core/events_engine";
import BaseModule from "./base";
import PopupModule from "./popup";
import Mention from "../formats/mention";
var MentionModule = BaseModule;
if (Quill) {
    var USER_ACTION = "user";
    var DEFAULT_MARKER = "@";
    var KEYS = {
        ARROW_UP: "upArrow",
        ARROW_DOWN: "downArrow",
        ARROW_LEFT: "leftArrow",
        ARROW_RIGHT: "rightArrow",
        ENTER: "enter",
        ESCAPE: "escape",
        SPACE: "space",
        PAGE_UP: "pageUp",
        PAGE_DOWN: "pageDown",
        END: "end",
        HOME: "home"
    };
    var NAVIGATION_KEYS = [KEYS.ARROW_LEFT, KEYS.ARROW_RIGHT, KEYS.PAGE_UP, KEYS.PAGE_DOWN, KEYS.END, KEYS.HOME];
    var ALLOWED_PREFIX_CHARS = [" ", "\n"];
    var DISABLED_STATE_CLASS = "dx-state-disabled";
    Quill.register({
        "formats/mention": Mention
    }, true);
    MentionModule = class extends PopupModule {
        _getDefaultOptions() {
            var baseConfig = super._getDefaultOptions();
            return extend(baseConfig, {
                itemTemplate: "item",
                valueExpr: "this",
                displayExpr: "this",
                template: null,
                searchExpr: null,
                searchTimeout: 500,
                minSearchLength: 0
            })
        }
        constructor(quill, options) {
            super(quill, options);
            this._mentions = {};
            options.mentions.forEach(item => {
                var marker = item.marker;
                if (!marker) {
                    item.marker = marker = DEFAULT_MARKER
                }
                var template = item.template;
                if (template) {
                    var preparedTemplate = this.editorInstance._getTemplate(template);
                    preparedTemplate && Mention.addTemplate(marker, preparedTemplate)
                }
                this._mentions[marker] = extend({}, this._getDefaultOptions(), item)
            });
            this._attachKeyboardHandlers();
            this.addCleanCallback(this.clean.bind(this));
            this.quill.on("text-change", this.onTextChange.bind(this))
        }
        _attachKeyboardHandlers() {
            this.quill.keyboard.addBinding({
                key: KEYS.ARROW_UP
            }, this._moveToItem.bind(this, "prev"));
            this.quill.keyboard.addBinding({
                key: KEYS.ARROW_DOWN
            }, this._moveToItem.bind(this, "next"));
            this.quill.keyboard.addBinding({
                key: [KEYS.ENTER, KEYS.SPACE]
            }, this._selectItemHandler.bind(this));
            var enterBindings = this.quill.keyboard.bindings[KEYS.ENTER];
            enterBindings.unshift(enterBindings.pop());
            this.quill.keyboard.addBinding({
                key: KEYS.ESCAPE
            }, this._escapeKeyHandler.bind(this));
            this.quill.keyboard.addBinding({
                key: [KEYS.ARROW_LEFT, KEYS.ARROW_RIGHT],
                shiftKey: true
            }, this._ignoreKeyHandler.bind(this));
            this.quill.keyboard.addBinding({
                key: NAVIGATION_KEYS
            }, this._ignoreKeyHandler.bind(this))
        }
        _moveToItem(direction) {
            var dataSource = this._list.getDataSource();
            if (this._isMentionActive && !dataSource.isLoading()) {
                var $focusedItem = $(this._list.option("focusedElement"));
                var defaultItemPosition = "next" === direction ? "first" : "last";
                var $nextItem = $focusedItem[direction]();
                $nextItem = $nextItem.length ? $nextItem : this._activeListItems[defaultItemPosition]();
                this._list.option("focusedElement", getPublicElement($nextItem));
                this._list.scrollToItem($nextItem)
            }
            return !this._isMentionActive
        }
        _ignoreKeyHandler() {
            return !this._isMentionActive
        }
        _fitIntoRange(value, start, end) {
            if (value > end) {
                return start
            }
            if (value < start) {
                return end
            }
            return value
        }
        _selectItemHandler() {
            if (this._isMentionActive) {
                this._list.selectItem(this._list.option("focusedElement"))
            }
            return !this._isMentionActive
        }
        _escapeKeyHandler() {
            if (this._isMentionActive) {
                this._popup.hide()
            }
            return !this._isMentionActive
        }
        renderList($container, options) {
            this.compileGetters(this.options);
            super.renderList($container, options)
        }
        compileGetters(_ref) {
            var {
                displayExpr: displayExpr,
                valueExpr: valueExpr
            } = _ref;
            this._valueGetter = compileGetter(displayExpr);
            this._idGetter = compileGetter(valueExpr)
        }
        _getListConfig(options) {
            var baseConfig = super._getListConfig(options);
            return extend(baseConfig, {
                itemTemplate: this.options.itemTemplate,
                onContentReady: () => {
                    if (this._hasSearch) {
                        this._popup.repaint();
                        this._focusFirstElement();
                        this._hasSearch = false
                    }
                }
            })
        }
        insertEmbedContent() {
            var markerLength = this._activeMentionConfig.marker.length;
            var textLength = markerLength + this._searchValue.length;
            var caretPosition = this.getPosition();
            var selectedItem = this._list.option("selectedItem");
            var value = {
                value: this._valueGetter(selectedItem),
                id: this._idGetter(selectedItem),
                marker: this._activeMentionConfig.marker
            };
            var Delta = Quill.import("delta");
            var startIndex = Math.max(0, caretPosition - markerLength);
            var retainCorrection = "\n" === this._getCharByIndex(startIndex) ? 1 : 0;
            startIndex += retainCorrection;
            var newDelta = (new Delta).retain(startIndex).delete(textLength).insert({
                mention: value
            }).insert(" ");
            this.quill.updateContents(newDelta);
            this.quill.setSelection(startIndex + 2)
        }
        _getLastInsertOperation(ops) {
            var lastOperation = ops[ops.length - 1];
            var isLastOperationInsert = "insert" in lastOperation;
            if (isLastOperationInsert) {
                return lastOperation
            }
            var isLastOperationDelete = "delete" in lastOperation;
            if (isLastOperationDelete && ops.length >= 2) {
                var penultOperation = ops[ops.length - 2];
                var isPenultOperationInsert = "insert" in penultOperation;
                var isSelectionReplacing = isLastOperationDelete && isPenultOperationInsert;
                if (isSelectionReplacing) {
                    return penultOperation
                }
            }
            return null
        }
        onTextChange(newDelta, oldDelta, source) {
            if (source === USER_ACTION) {
                var lastOperation = newDelta.ops[newDelta.ops.length - 1];
                if (this._isMentionActive && this._isPopupVisible) {
                    this._processSearchValue(lastOperation) && this._filterList(this._searchValue)
                } else {
                    var {
                        ops: ops
                    } = newDelta;
                    var lastInsertOperation = this._getLastInsertOperation(ops);
                    if (lastInsertOperation) {
                        this.checkMentionRequest(lastInsertOperation, ops)
                    }
                }
            }
        }
        get _isPopupVisible() {
            var _this$_popup;
            return null === (_this$_popup = this._popup) || void 0 === _this$_popup ? void 0 : _this$_popup.option("visible")
        }
        _processSearchValue(operation) {
            var isInsertOperation = "insert" in operation;
            if (isInsertOperation) {
                this._searchValue += operation.insert
            } else if (!this._searchValue.length || operation.delete > 1) {
                this._popup.hide();
                return false
            } else {
                this._searchValue = this._searchValue.slice(0, -1)
            }
            return true
        }
        checkMentionRequest(_ref2, ops) {
            var {
                insert: insert
            } = _ref2;
            var caret = this.quill.getSelection();
            if (!insert || !isString(insert) || !caret || this._isMarkerPartOfText(ops[0].retain)) {
                return
            }
            this._activeMentionConfig = this._mentions[insert];
            if (this._activeMentionConfig) {
                this._updateList(this._activeMentionConfig);
                this.savePosition(caret.index);
                this._popup.option("position", this._popupPosition);
                this._searchValue = "";
                this._popup.show()
            }
        }
        _isMarkerPartOfText(retain) {
            if (!retain || -1 !== ALLOWED_PREFIX_CHARS.indexOf(this._getCharByIndex(retain - 1))) {
                return false
            }
            return true
        }
        _getCharByIndex(index) {
            return this.quill.getContents(index, 1).ops[0].insert
        }
        _updateList(_ref3) {
            var {
                dataSource: dataSource,
                displayExpr: displayExpr,
                valueExpr: valueExpr,
                itemTemplate: itemTemplate,
                searchExpr: searchExpr
            } = _ref3;
            this.compileGetters({
                displayExpr: displayExpr,
                valueExpr: valueExpr
            });
            this._list.unselectAll();
            this._list.option({
                dataSource: dataSource,
                displayExpr: displayExpr,
                itemTemplate: itemTemplate,
                searchExpr: searchExpr
            })
        }
        _filterList(searchValue) {
            if (!this._isMinSearchLengthExceeded(searchValue)) {
                this._resetFilter();
                return
            }
            var searchTimeout = this._activeMentionConfig.searchTimeout;
            if (searchTimeout) {
                clearTimeout(this._searchTimer);
                this._searchTimer = setTimeout(() => {
                    this._search(searchValue)
                }, searchTimeout)
            } else {
                this._search(searchValue)
            }
        }
        _isMinSearchLengthExceeded(searchValue) {
            return searchValue.length >= this._activeMentionConfig.minSearchLength
        }
        _resetFilter() {
            clearTimeout(this._searchTimer);
            this._search(null)
        }
        _search(searchValue) {
            this._hasSearch = true;
            this._list.option("searchValue", searchValue)
        }
        _focusFirstElement() {
            if (!this._list) {
                return
            }
            var $firstItem = this._activeListItems.first();
            this._list.option("focusedElement", getPublicElement($firstItem));
            this._list.scrollToItem($firstItem)
        }
        get _popupPosition() {
            var position = this.getPosition();
            var {
                left: mentionLeft,
                top: mentionTop,
                height: mentionHeight
            } = this.quill.getBounds(position ? position - 1 : position);
            var {
                left: leftOffset,
                top: topOffset
            } = $(this.quill.root).offset();
            var positionEvent = eventsEngine.Event("positionEvent", {
                pageX: leftOffset + mentionLeft,
                pageY: topOffset + mentionTop
            });
            return {
                of: positionEvent,
                offset: {
                    v: mentionHeight
                },
                my: "top left",
                at: "top left",
                collision: {
                    y: "flip",
                    x: "flipfit"
                }
            }
        }
        _getPopupConfig() {
            return extend(super._getPopupConfig(), {
                hideOnParentScroll: false,
                onShown: () => {
                    this._isMentionActive = true;
                    this._hasSearch = false;
                    this._focusFirstElement()
                },
                onHidden: () => {
                    this._list.unselectAll();
                    this._list.option("focusedElement", null);
                    this._isMentionActive = false;
                    this._search(null)
                },
                focusStateEnabled: false
            })
        }
        get _activeListItems() {
            return this._list.itemElements().filter(":not(.".concat(DISABLED_STATE_CLASS, ")"))
        }
        clean() {
            Object.keys(this._mentions).forEach(marker => {
                if (this._mentions[marker].template) {
                    Mention.removeTemplate(marker)
                }
            })
        }
    }
}
export default MentionModule;
