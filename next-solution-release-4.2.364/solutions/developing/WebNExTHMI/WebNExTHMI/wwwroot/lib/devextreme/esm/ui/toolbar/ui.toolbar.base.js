/**
 * DevExtreme (esm/ui/toolbar/ui.toolbar.base.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import {
    getWidth,
    getOuterWidth,
    getHeight
} from "../../core/utils/size";
import $ from "../../core/renderer";
import {
    isMaterial,
    waitWebFont
} from "../themes";
import {
    noop
} from "../../core/utils/common";
import {
    isPlainObject,
    isDefined
} from "../../core/utils/type";
import registerComponent from "../../core/component_registrator";
import {
    inArray
} from "../../core/utils/array";
import {
    extend
} from "../../core/utils/extend";
import {
    each
} from "../../core/utils/iterator";
import {
    getBoundingRect
} from "../../core/utils/position";
import AsyncCollectionWidget from "../collection/ui.collection_widget.async";
import Promise from "../../core/polyfills/promise";
import {
    BindableTemplate
} from "../../core/templates/bindable_template";
import errors from "../../core/errors";
import fx from "../../animation/fx";
import {
    TOOLBAR_CLASS
} from "./constants";
var TOOLBAR_BEFORE_CLASS = "dx-toolbar-before";
var TOOLBAR_CENTER_CLASS = "dx-toolbar-center";
var TOOLBAR_AFTER_CLASS = "dx-toolbar-after";
var TOOLBAR_MINI_CLASS = "dx-toolbar-mini";
var TOOLBAR_ITEM_CLASS = "dx-toolbar-item";
var TOOLBAR_LABEL_CLASS = "dx-toolbar-label";
var TOOLBAR_BUTTON_CLASS = "dx-toolbar-button";
var TOOLBAR_ITEMS_CONTAINER_CLASS = "dx-toolbar-items-container";
var TOOLBAR_GROUP_CLASS = "dx-toolbar-group";
var TOOLBAR_COMPACT_CLASS = "dx-toolbar-compact";
var TOOLBAR_LABEL_SELECTOR = "." + TOOLBAR_LABEL_CLASS;
var TOOLBAR_MULTILINE_CLASS = "dx-toolbar-multiline";
var TEXT_BUTTON_MODE = "text";
var DEFAULT_BUTTON_TYPE = "default";
var DEFAULT_DROPDOWNBUTTON_STYLING_MODE = "contained";
var TOOLBAR_ITEM_DATA_KEY = "dxToolbarItemDataKey";
var ToolbarBase = AsyncCollectionWidget.inherit({
    compactMode: false,
    ctor: function(element, options) {
        this._userOptions = options || {};
        this.callBase(element, options);
        if ("height" in this._userOptions) {
            errors.log("W0001", this.NAME, "height", "20.1", "Functionality associated with this option is not intended for the Toolbar widget.")
        }
    },
    _getSynchronizableOptionsForCreateComponent: function() {
        return this.callBase().filter(item => "disabled" !== item)
    },
    _initTemplates: function() {
        this.callBase();
        var template = new BindableTemplate(function($container, data, rawModel) {
            if (isPlainObject(data)) {
                if (data.text) {
                    $container.text(data.text).wrapInner("<div>")
                }
                if (data.html) {
                    $container.html(data.html)
                }
                if ("dxDropDownButton" === data.widget) {
                    data.options = data.options || {};
                    if (!isDefined(data.options.stylingMode)) {
                        data.options.stylingMode = this.option("useFlatButtons") ? TEXT_BUTTON_MODE : DEFAULT_DROPDOWNBUTTON_STYLING_MODE
                    }
                }
                if ("dxButton" === data.widget) {
                    if (this.option("useFlatButtons")) {
                        data.options = data.options || {};
                        data.options.stylingMode = data.options.stylingMode || TEXT_BUTTON_MODE
                    }
                    if (this.option("useDefaultButtons")) {
                        data.options = data.options || {};
                        data.options.type = data.options.type || DEFAULT_BUTTON_TYPE
                    }
                }
            } else {
                $container.text(String(data))
            }
            this._getTemplate("dx-polymorph-widget").render({
                container: $container,
                model: rawModel,
                parent: this
            })
        }.bind(this), ["text", "html", "widget", "options"], this.option("integrationOptions.watchMethod"));
        this._templateManager.addDefaultTemplates({
            item: template,
            menuItem: template
        })
    },
    _getDefaultOptions: function() {
        return extend(this.callBase(), {
            renderAs: "topToolbar",
            grouped: false,
            useFlatButtons: false,
            useDefaultButtons: false,
            multiline: false
        })
    },
    _defaultOptionsRules: function() {
        return this.callBase().concat([{
            device: function() {
                return isMaterial()
            },
            options: {
                useFlatButtons: true
            }
        }])
    },
    _itemContainer: function() {
        return this._$toolbarItemsContainer.find(["." + TOOLBAR_BEFORE_CLASS, "." + TOOLBAR_CENTER_CLASS, "." + TOOLBAR_AFTER_CLASS].join(","))
    },
    _itemClass: function() {
        return TOOLBAR_ITEM_CLASS
    },
    _itemDataKey: function() {
        return TOOLBAR_ITEM_DATA_KEY
    },
    _buttonClass: function() {
        return TOOLBAR_BUTTON_CLASS
    },
    _dimensionChanged: function() {
        this._arrangeItems();
        this._applyCompactMode()
    },
    _initMarkup: function() {
        this._renderToolbar();
        this._renderSections();
        this.callBase();
        this.setAria("role", "toolbar")
    },
    _waitParentAnimationFinished: function() {
        var $element = this.$element();
        return new Promise(resolve => {
            var runCheck = () => {
                clearTimeout(this._waitParentAnimationTimeout);
                this._waitParentAnimationTimeout = setTimeout(() => (() => {
                    var readyToResolve = true;
                    $element.parents().each((_, parent) => {
                        if (fx.isAnimating($(parent))) {
                            readyToResolve = false;
                            return false
                        }
                    });
                    if (readyToResolve) {
                        resolve()
                    }
                    return readyToResolve
                })() || runCheck(), 15)
            };
            runCheck()
        })
    },
    _render: function() {
        this.callBase();
        this._renderItemsAsync();
        if (isMaterial()) {
            Promise.all([this._waitParentAnimationFinished(), this._checkWebFontForLabelsLoaded()]).then(this._dimensionChanged.bind(this))
        }
    },
    _postProcessRenderItems: function() {
        this._arrangeItems()
    },
    _renderToolbar: function() {
        this.$element().addClass(TOOLBAR_CLASS).toggleClass(TOOLBAR_MULTILINE_CLASS, this.option("multiline"));
        this._$toolbarItemsContainer = $("<div>").addClass(TOOLBAR_ITEMS_CONTAINER_CLASS).appendTo(this.$element())
    },
    _renderSections: function() {
        var $container = this._$toolbarItemsContainer;
        var that = this;
        each(["before", "center", "after"], (function() {
            var sectionClass = "dx-toolbar-" + this;
            var $section = $container.find("." + sectionClass);
            if (!$section.length) {
                that["_$" + this + "Section"] = $section = $("<div>").addClass(sectionClass).appendTo($container)
            }
        }))
    },
    _checkWebFontForLabelsLoaded: function() {
        var $labels = this.$element().find(TOOLBAR_LABEL_SELECTOR);
        var promises = [];
        $labels.each((_, label) => {
            var text = $(label).text();
            var fontWeight = $(label).css("fontWeight");
            promises.push(waitWebFont(text, fontWeight))
        });
        return Promise.all(promises)
    },
    _arrangeItems: function(elementWidth) {
        elementWidth = elementWidth || getWidth(this.$element());
        this._$centerSection.css({
            margin: "0 auto",
            float: "none"
        });
        var beforeRect = getBoundingRect(this._$beforeSection.get(0));
        var afterRect = getBoundingRect(this._$afterSection.get(0));
        this._alignCenterSection(beforeRect, afterRect, elementWidth);
        var $label = this._$toolbarItemsContainer.find(TOOLBAR_LABEL_SELECTOR).eq(0);
        var $section = $label.parent();
        if (!$label.length) {
            return
        }
        var labelOffset = beforeRect.width ? beforeRect.width : $label.position().left;
        var widthBeforeSection = $section.hasClass(TOOLBAR_BEFORE_CLASS) ? 0 : labelOffset;
        var widthAfterSection = $section.hasClass(TOOLBAR_AFTER_CLASS) ? 0 : afterRect.width;
        var elemsAtSectionWidth = 0;
        $section.children().not(TOOLBAR_LABEL_SELECTOR).each((function() {
            elemsAtSectionWidth += getOuterWidth(this)
        }));
        var freeSpace = elementWidth - elemsAtSectionWidth;
        var sectionMaxWidth = Math.max(freeSpace - widthBeforeSection - widthAfterSection, 0);
        if ($section.hasClass(TOOLBAR_BEFORE_CLASS)) {
            this._alignSection(this._$beforeSection, sectionMaxWidth)
        } else {
            var labelPaddings = getOuterWidth($label) - getWidth($label);
            $label.css("maxWidth", sectionMaxWidth - labelPaddings)
        }
    },
    _alignCenterSection: function(beforeRect, afterRect, elementWidth) {
        this._alignSection(this._$centerSection, elementWidth - beforeRect.width - afterRect.width);
        var isRTL = this.option("rtlEnabled");
        var leftRect = isRTL ? afterRect : beforeRect;
        var rightRect = isRTL ? beforeRect : afterRect;
        var centerRect = getBoundingRect(this._$centerSection.get(0));
        if (leftRect.right > centerRect.left || centerRect.right > rightRect.left) {
            this._$centerSection.css({
                marginLeft: leftRect.width,
                marginRight: rightRect.width,
                float: leftRect.width > rightRect.width ? "none" : "right"
            })
        }
    },
    _alignSection: function($section, maxWidth) {
        var $labels = $section.find(TOOLBAR_LABEL_SELECTOR);
        var labels = $labels.toArray();
        maxWidth -= this._getCurrentLabelsPaddings(labels);
        var currentWidth = this._getCurrentLabelsWidth(labels);
        var difference = Math.abs(currentWidth - maxWidth);
        if (maxWidth < currentWidth) {
            labels = labels.reverse();
            this._alignSectionLabels(labels, difference, false)
        } else {
            this._alignSectionLabels(labels, difference, true)
        }
    },
    _alignSectionLabels: function(labels, difference, expanding) {
        var getRealLabelWidth = function(label) {
            return getBoundingRect(label).width
        };
        for (var i = 0; i < labels.length; i++) {
            var $label = $(labels[i]);
            var currentLabelWidth = Math.ceil(getRealLabelWidth(labels[i]));
            var labelMaxWidth = void 0;
            if (expanding) {
                $label.css("maxWidth", "inherit")
            }
            var possibleLabelWidth = Math.ceil(expanding ? getRealLabelWidth(labels[i]) : currentLabelWidth);
            if (possibleLabelWidth < difference) {
                labelMaxWidth = expanding ? possibleLabelWidth : 0;
                difference -= possibleLabelWidth
            } else {
                labelMaxWidth = expanding ? currentLabelWidth + difference : currentLabelWidth - difference;
                $label.css("maxWidth", labelMaxWidth);
                break
            }
            $label.css("maxWidth", labelMaxWidth)
        }
    },
    _applyCompactMode: function() {
        var $element = this.$element();
        $element.removeClass(TOOLBAR_COMPACT_CLASS);
        if (this.option("compactMode") && this._getSummaryItemsWidth(this.itemElements(), true) > getWidth($element)) {
            $element.addClass(TOOLBAR_COMPACT_CLASS)
        }
    },
    _getCurrentLabelsWidth: function(labels) {
        var width = 0;
        labels.forEach((function(label, index) {
            width += getOuterWidth(label)
        }));
        return width
    },
    _getCurrentLabelsPaddings: function(labels) {
        var padding = 0;
        labels.forEach((function(label, index) {
            padding += getOuterWidth(label) - getWidth(label)
        }));
        return padding
    },
    _renderItem: function(index, item, itemContainer, $after) {
        var location = item.location || "center";
        var container = itemContainer || this["_$" + location + "Section"];
        var itemHasText = !!(item.text || item.html);
        var itemElement = this.callBase(index, item, container, $after);
        itemElement.toggleClass(this._buttonClass(), !itemHasText).toggleClass(TOOLBAR_LABEL_CLASS, itemHasText).addClass(item.cssClass);
        return itemElement
    },
    _renderGroupedItems: function() {
        var that = this;
        each(this.option("items"), (function(groupIndex, group) {
            var groupItems = group.items;
            var $container = $("<div>").addClass(TOOLBAR_GROUP_CLASS);
            var location = group.location || "center";
            if (!groupItems || !groupItems.length) {
                return
            }
            each(groupItems, (function(itemIndex, item) {
                that._renderItem(itemIndex, item, $container, null)
            }));
            that._$toolbarItemsContainer.find(".dx-toolbar-" + location).append($container)
        }))
    },
    _renderItems: function(items) {
        var grouped = this.option("grouped") && items.length && items[0].items;
        grouped ? this._renderGroupedItems() : this.callBase(items)
    },
    _getToolbarItems: function() {
        return this.option("items") || []
    },
    _renderContentImpl: function() {
        var items = this._getToolbarItems();
        this.$element().toggleClass(TOOLBAR_MINI_CLASS, 0 === items.length);
        if (this._renderedItemsCount) {
            this._renderItems(items.slice(this._renderedItemsCount))
        } else {
            this._renderItems(items)
        }
        this._applyCompactMode()
    },
    _renderEmptyMessage: noop,
    _clean: function() {
        this._$toolbarItemsContainer.children().empty();
        this.$element().empty()
    },
    _visibilityChanged: function(visible) {
        if (visible) {
            this._arrangeItems()
        }
    },
    _isVisible: function() {
        return getWidth(this.$element()) > 0 && getHeight(this.$element()) > 0
    },
    _getIndexByItem: function(item) {
        return inArray(item, this._getToolbarItems())
    },
    _itemOptionChanged: function(item, property, value) {
        this.callBase.apply(this, [item, property, value]);
        this._arrangeItems()
    },
    _optionChanged: function(args) {
        var name = args.name;
        switch (name) {
            case "width":
                this.callBase.apply(this, arguments);
                this._dimensionChanged();
                break;
            case "multiline":
                this.$element().toggleClass(TOOLBAR_MULTILINE_CLASS, args.value);
                break;
            case "renderAs":
            case "useFlatButtons":
            case "useDefaultButtons":
                this._invalidate();
                break;
            case "compactMode":
                this._applyCompactMode();
                break;
            case "grouped":
                break;
            default:
                this.callBase.apply(this, arguments)
        }
    },
    _dispose: function() {
        this.callBase();
        clearTimeout(this._waitParentAnimationTimeout)
    }
});
registerComponent("dxToolbarBase", ToolbarBase);
export default ToolbarBase;
