/**
 * DevExtreme (esm/ui/grid_core/ui.grid_core.search.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import $ from "../../core/renderer";
import domAdapter from "../../core/dom_adapter";
import {
    isDefined
} from "../../core/utils/type";
import {
    compileGetter
} from "../../core/utils/data";
import gridCoreUtils from "./ui.grid_core.utils";
import messageLocalization from "../../localization/message";
import dataQuery from "../../data/query";
var SEARCH_PANEL_CLASS = "search-panel";
var SEARCH_TEXT_CLASS = "search-text";
var HEADER_PANEL_CLASS = "header-panel";
var FILTERING_TIMEOUT = 700;

function allowSearch(column) {
    return isDefined(column.allowSearch) ? column.allowSearch : column.allowFiltering
}

function parseValue(column, text) {
    var lookup = column.lookup;
    if (!column.parseValue) {
        return text
    }
    if (lookup) {
        return column.parseValue.call(lookup, text)
    }
    return column.parseValue(text)
}
export var searchModule = {
    defaultOptions: function() {
        return {
            searchPanel: {
                visible: false,
                width: 160,
                placeholder: messageLocalization.format("dxDataGrid-searchPanelPlaceholder"),
                highlightSearchText: true,
                highlightCaseSensitive: false,
                text: "",
                searchVisibleColumnsOnly: false
            }
        }
    },
    extenders: {
        controllers: {
            data: {
                publicMethods: function() {
                    return this.callBase().concat(["searchByText"])
                },
                _calculateAdditionalFilter: function() {
                    var filter = this.callBase();
                    var searchFilter = function(that, text) {
                        var i;
                        var column;
                        var columns = that._columnsController.getColumns();
                        var searchVisibleColumnsOnly = that.option("searchPanel.searchVisibleColumnsOnly");
                        var lookup;
                        var filters = [];
                        if (!text) {
                            return null
                        }

                        function onQueryDone(items) {
                            var valueGetter = compileGetter(lookup.valueExpr);
                            for (var _i = 0; _i < items.length; _i++) {
                                var value = valueGetter(items[_i]);
                                filters.push(column.createFilterExpression(value, null, "search"))
                            }
                        }
                        for (i = 0; i < columns.length; i++) {
                            column = columns[i];
                            if (searchVisibleColumnsOnly && !column.visible) {
                                continue
                            }
                            if (allowSearch(column) && column.calculateFilterExpression) {
                                lookup = column.lookup;
                                var filterValue = parseValue(column, text);
                                if (lookup && lookup.items) {
                                    dataQuery(lookup.items).filter(column.createFilterExpression.call({
                                        dataField: lookup.displayExpr,
                                        dataType: lookup.dataType,
                                        calculateFilterExpression: column.calculateFilterExpression
                                    }, filterValue, null, "search")).enumerate().done(onQueryDone)
                                } else if (void 0 !== filterValue) {
                                    filters.push(column.createFilterExpression(filterValue, null, "search"))
                                }
                            }
                        }
                        if (0 === filters.length) {
                            return ["!"]
                        }
                        return gridCoreUtils.combineFilters(filters, "or")
                    }(this, this.option("searchPanel.text"));
                    return gridCoreUtils.combineFilters([filter, searchFilter])
                },
                searchByText: function(text) {
                    this.option("searchPanel.text", text)
                },
                optionChanged: function(args) {
                    switch (args.fullName) {
                        case "searchPanel.text":
                        case "searchPanel":
                            this._applyFilter();
                            args.handled = true;
                            break;
                        default:
                            this.callBase(args)
                    }
                }
            }
        },
        views: {
            headerPanel: function() {
                var getSearchPanelOptions = function(that) {
                    return that.option("searchPanel")
                };
                return {
                    _getToolbarItems: function() {
                        var items = this.callBase();
                        return this._prepareSearchItem(items)
                    },
                    _prepareSearchItem: function(items) {
                        var that = this;
                        var dataController = that.getController("data");
                        var searchPanelOptions = getSearchPanelOptions(that);
                        if (searchPanelOptions && searchPanelOptions.visible) {
                            var toolbarItem = {
                                template: function(data, index, container) {
                                    var $search = $("<div>").addClass(that.addWidgetPrefix(SEARCH_PANEL_CLASS)).appendTo(container);
                                    that.getController("editorFactory").createEditor($search, {
                                        width: searchPanelOptions.width,
                                        placeholder: searchPanelOptions.placeholder,
                                        parentType: "searchPanel",
                                        value: that.option("searchPanel.text"),
                                        updateValueTimeout: FILTERING_TIMEOUT,
                                        setValue: function(value) {
                                            dataController.searchByText(value)
                                        },
                                        editorOptions: {
                                            inputAttr: {
                                                "aria-label": messageLocalization.format("".concat(that.component.NAME, "-ariaSearchInGrid"))
                                            }
                                        }
                                    });
                                    that.resize()
                                },
                                name: "searchPanel",
                                location: "after",
                                locateInMenu: "never",
                                sortIndex: 40
                            };
                            items.push(toolbarItem)
                        }
                        return items
                    },
                    getSearchTextEditor: function() {
                        var that = this;
                        var $element = that.element();
                        var $searchPanel = $element.find("." + that.addWidgetPrefix(SEARCH_PANEL_CLASS)).filter((function() {
                            return $(this).closest("." + that.addWidgetPrefix(HEADER_PANEL_CLASS)).is($element)
                        }));
                        if ($searchPanel.length) {
                            return $searchPanel.dxTextBox("instance")
                        }
                        return null
                    },
                    isVisible: function() {
                        var searchPanelOptions = getSearchPanelOptions(this);
                        return this.callBase() || searchPanelOptions && searchPanelOptions.visible
                    },
                    optionChanged: function(args) {
                        if ("searchPanel" === args.name) {
                            if ("searchPanel.text" === args.fullName) {
                                var editor = this.getSearchTextEditor();
                                if (editor) {
                                    editor.option("value", args.value)
                                }
                            } else {
                                this._invalidate()
                            }
                            args.handled = true
                        } else {
                            this.callBase(args)
                        }
                    }
                }
            }(),
            rowsView: {
                init: function() {
                    this.callBase.apply(this, arguments);
                    this._searchParams = []
                },
                _getFormattedSearchText: function(column, searchText) {
                    var value = parseValue(column, searchText);
                    var formatOptions = gridCoreUtils.getFormatOptionsByColumn(column, "search");
                    return gridCoreUtils.formatValue(value, formatOptions)
                },
                _getStringNormalizer: function() {
                    var isCaseSensitive = this.option("searchPanel.highlightCaseSensitive");
                    return function(str) {
                        return isCaseSensitive ? str : str.toLowerCase()
                    }
                },
                _findHighlightingTextNodes: function(column, cellElement, searchText) {
                    var _$items;
                    var $parent = cellElement.parent();
                    var $items;
                    var stringNormalizer = this._getStringNormalizer();
                    var normalizedSearchText = stringNormalizer(searchText);
                    var resultTextNodes = [];
                    if (!$parent.length) {
                        $parent = $("<div>").append(cellElement)
                    } else if (column) {
                        if (column.groupIndex >= 0 && !column.showWhenGrouped) {
                            $items = cellElement
                        } else {
                            var columnIndex = this._columnsController.getVisibleIndex(column.index);
                            $items = $parent.children("td").eq(columnIndex).find("*")
                        }
                    }
                    $items = null !== (_$items = $items) && void 0 !== _$items && _$items.length ? $items : $parent.find("*");
                    $items.each((function(_, element) {
                        var $contents = $(element).contents();
                        for (var i = 0; i < $contents.length; i++) {
                            var node = $contents.get(i);
                            if (3 === node.nodeType) {
                                var normalizedText = stringNormalizer(node.textContent || node.nodeValue);
                                if (normalizedText.indexOf(normalizedSearchText) > -1) {
                                    resultTextNodes.push(node)
                                }
                            }
                        }
                    }));
                    return resultTextNodes
                },
                _highlightSearchTextCore: function($textNode, searchText) {
                    var $searchTextSpan = $("<span>").addClass(this.addWidgetPrefix(SEARCH_TEXT_CLASS));
                    var text = $textNode.text();
                    var firstContentElement = $textNode[0];
                    var stringNormalizer = this._getStringNormalizer();
                    var index = stringNormalizer(text).indexOf(stringNormalizer(searchText));
                    if (index >= 0) {
                        if (firstContentElement.textContent) {
                            firstContentElement.textContent = text.substr(0, index)
                        } else {
                            firstContentElement.nodeValue = text.substr(0, index)
                        }
                        $textNode.after($searchTextSpan.text(text.substr(index, searchText.length)));
                        $textNode = $(domAdapter.createTextNode(text.substr(index + searchText.length))).insertAfter($searchTextSpan);
                        return this._highlightSearchTextCore($textNode, searchText)
                    }
                },
                _highlightSearchText: function(cellElement, isEquals, column) {
                    var that = this;
                    var stringNormalizer = this._getStringNormalizer();
                    var searchText = that.option("searchPanel.text");
                    if (isEquals && column) {
                        searchText = searchText && that._getFormattedSearchText(column, searchText)
                    }
                    if (searchText && that.option("searchPanel.highlightSearchText")) {
                        var textNodes = that._findHighlightingTextNodes(column, cellElement, searchText);
                        textNodes.forEach(textNode => {
                            if (isEquals) {
                                if (stringNormalizer($(textNode).text()) === stringNormalizer(searchText)) {
                                    $(textNode).replaceWith($("<span>").addClass(that.addWidgetPrefix(SEARCH_TEXT_CLASS)).text($(textNode).text()))
                                }
                            } else {
                                that._highlightSearchTextCore($(textNode), searchText)
                            }
                        })
                    }
                },
                _renderCore: function() {
                    this.callBase.apply(this, arguments);
                    if (this.option().rowTemplate || this.option("dataRowTemplate")) {
                        if (this.option("templatesRenderAsynchronously")) {
                            clearTimeout(this._highlightTimer);
                            this._highlightTimer = setTimeout(function() {
                                this._highlightSearchText(this.getTableElement())
                            }.bind(this))
                        } else {
                            this._highlightSearchText(this.getTableElement())
                        }
                    }
                },
                _updateCell: function($cell, parameters) {
                    var column = parameters.column;
                    var dataType = column.lookup && column.lookup.dataType || column.dataType;
                    var isEquals = "string" !== dataType;
                    if (allowSearch(column) && !parameters.isOnForm) {
                        if (this.option("templatesRenderAsynchronously")) {
                            if (!this._searchParams.length) {
                                clearTimeout(this._highlightTimer);
                                this._highlightTimer = setTimeout(function() {
                                    this._searchParams.forEach(function(params) {
                                        this._highlightSearchText.apply(this, params)
                                    }.bind(this));
                                    this._searchParams = []
                                }.bind(this))
                            }
                            this._searchParams.push([$cell, isEquals, column])
                        } else {
                            this._highlightSearchText($cell, isEquals, column)
                        }
                    }
                    this.callBase($cell, parameters)
                },
                dispose: function() {
                    clearTimeout(this._highlightTimer);
                    this.callBase()
                }
            }
        }
    }
};
