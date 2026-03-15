/**
 * DevExtreme (esm/viz/core/export.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import {
    extend
} from "../../core/utils/extend";
import {
    getWindow
} from "../../core/utils/window";
import {
    patchFontOptions
} from "./utils";
import {
    HIDDEN_FOR_EXPORT
} from "../../core/utils/svg";
import {
    export as _export,
    image as imageExporter,
    svg as svgExporter,
    pdf as pdfExporter
} from "../../exporter";
import messageLocalization from "../../localization/message";
import {
    isDefined
} from "../../core/utils/type";
import {
    getTheme
} from "../themes";
import {
    start as hoverEventStart,
    end as hoverEventEnd
} from "../../events/hover";
import pointerEvents from "../../events/pointer";
import {
    logger
} from "../../core/utils/console";
import {
    getWidth
} from "../../core/utils/size";
var pointerActions = [pointerEvents.down, pointerEvents.move].join(" ");
var BUTTON_SIZE = 35;
var ICON_COORDS = [
    [9, 12, 26, 12, 26, 14, 9, 14],
    [9, 17, 26, 17, 26, 19, 9, 19],
    [9, 22, 26, 22, 26, 24, 9, 24]
];
var LIST_PADDING_TOP = 4;
var LIST_WIDTH = 120;
var VERTICAL_TEXT_MARGIN = 8;
var HORIZONTAL_TEXT_MARGIN = 15;
var MENU_ITEM_HEIGHT = 30;
var LIST_STROKE_WIDTH = 1;
var MARGIN = 10;
var SHADOW_OFFSET = 2;
var SHADOW_BLUR = 3;
var DEFAULT_EXPORT_FORMAT = "PNG";
var ALLOWED_IMAGE_FORMATS = [DEFAULT_EXPORT_FORMAT, "JPEG", "GIF"];
var ALLOWED_EXTRA_FORMATS = ["PDF", "SVG"];
var EXPORT_CSS_CLASS = "dx-export-menu";
var A4WidthCm = "21cm";
var EXPORT_DATA_KEY = "export-element-type";
var FORMAT_DATA_KEY = "export-element-format";
var GET_COLOR_REGEX = /data-backgroundcolor="([^"]*)"/;

function getValidFormats() {
    var imageFormats = imageExporter.testFormats(ALLOWED_IMAGE_FORMATS);
    return {
        unsupported: imageFormats.unsupported,
        supported: imageFormats.supported.concat(ALLOWED_EXTRA_FORMATS)
    }
}

function validateFormat(format, incidentOccurred, validFormats) {
    validFormats = validFormats || getValidFormats();
    format = String(format).toUpperCase();
    if (-1 !== validFormats.supported.indexOf(format)) {
        return format
    }
    if (-1 !== validFormats.unsupported.indexOf(format)) {
        incidentOccurred && incidentOccurred("W2108", [format])
    }
}

function getCreatorFunc(format) {
    if ("SVG" === format) {
        return svgExporter.getData
    } else if ("PDF" === format) {
        return pdfExporter.getData
    } else {
        return imageExporter.getData
    }
}

function print(imageSrc, options) {
    var document = getWindow().document;
    var iFrame = document.createElement("iframe");
    iFrame.onload = setPrint(imageSrc, options);
    iFrame.style.position = "fixed";
    iFrame.style.width = "0";
    iFrame.style.height = "0";
    iFrame.style.right = "0";
    iFrame.style.bottom = "0";
    document.body.appendChild(iFrame)
}

function calculatePrintPageWidth(iFrameBody) {
    iFrameBody.style.width = A4WidthCm;
    var width = getWidth(iFrameBody);
    iFrameBody.style.width = "";
    return width
}

function setPrint(imageSrc, options) {
    return function() {
        var window = this.contentWindow;
        var img = window.document.createElement("img");
        window.document.body.appendChild(img);
        var widthRatio = calculatePrintPageWidth(window.document.body) / options.width;
        if (widthRatio < 1) {
            window.document.body.style.transform = "scale(".concat(widthRatio, ")");
            window.document.body.style["transform-origin"] = "0 0"
        }
        var removeFrame = () => {
            this.parentElement.removeChild(this)
        };
        img.addEventListener("load", () => {
            window.focus();
            window.print()
        });
        img.addEventListener("error", removeFrame);
        window.addEventListener("afterprint", () => {
            setTimeout(removeFrame, 0)
        });
        img.src = imageSrc
    }
}

function getItemAttributes(options, type, itemIndex) {
    var x = BUTTON_SIZE - LIST_WIDTH;
    var y = BUTTON_SIZE + LIST_PADDING_TOP + LIST_STROKE_WIDTH + itemIndex * MENU_ITEM_HEIGHT;
    var attr = {
        rect: {
            width: LIST_WIDTH - 2 * LIST_STROKE_WIDTH,
            height: MENU_ITEM_HEIGHT,
            x: x + LIST_STROKE_WIDTH,
            y: y
        },
        text: {
            x: x + (options.rtl ? LIST_WIDTH - HORIZONTAL_TEXT_MARGIN : HORIZONTAL_TEXT_MARGIN),
            y: y + MENU_ITEM_HEIGHT - VERTICAL_TEXT_MARGIN
        }
    };
    if ("printing" === type) {
        attr.separator = {
            stroke: options.button.default.borderColor,
            "stroke-width": LIST_STROKE_WIDTH,
            cursor: "pointer",
            sharp: "v",
            d: "M " + x + " " + (y + MENU_ITEM_HEIGHT - LIST_STROKE_WIDTH) + " L " + (x + LIST_WIDTH) + " " + (y + MENU_ITEM_HEIGHT - LIST_STROKE_WIDTH)
        }
    }
    return attr
}

function createMenuItem(renderer, options, settings) {
    var itemData = {};
    var type = settings.type;
    var format = settings.format;
    var attr = getItemAttributes(options, type, settings.itemIndex);
    var fontStyle = patchFontOptions(options.font);
    fontStyle["pointer-events"] = "none";
    var menuItem = renderer.g().attr({
        class: EXPORT_CSS_CLASS + "-list-item"
    });
    itemData[EXPORT_DATA_KEY] = type;
    if (format) {
        itemData[FORMAT_DATA_KEY] = format
    }
    var rect = renderer.rect();
    rect.attr(attr.rect).css({
        cursor: "pointer",
        "pointer-events": "all"
    }).data(itemData);
    rect.on(hoverEventStart + ".export", () => rect.attr({
        fill: options.button.hover.backgroundColor
    })).on(hoverEventEnd + ".export", () => rect.attr({
        fill: null
    }));
    rect.append(menuItem);
    var text = renderer.text(settings.text).css(fontStyle).attr(attr.text).append(menuItem);
    if ("printing" === type) {
        renderer.path(null, "line").attr(attr.separator).append(menuItem)
    }
    return {
        g: menuItem,
        rect: rect,
        resetState: () => rect.attr({
            fill: null
        }),
        fixPosition: () => {
            var textBBox = text.getBBox();
            text.move(attr.text.x - textBBox.x - (options.rtl ? textBBox.width : 0))
        }
    }
}

function createMenuItems(renderer, options) {
    var items = [];
    if (options.printingEnabled) {
        items.push(createMenuItem(renderer, options, {
            type: "printing",
            text: messageLocalization.format("vizExport-printingButtonText"),
            itemIndex: items.length
        }))
    }
    items = options.formats.reduce((r, format) => {
        r.push(createMenuItem(renderer, options, {
            type: "exporting",
            text: messageLocalization.getFormatter("vizExport-exportButtonText")(format),
            format: format,
            itemIndex: r.length
        }));
        return r
    }, items);
    return items
}

function getBackgroundColorFromMarkup(markup) {
    var parsedMarkup = GET_COLOR_REGEX.exec(markup);
    return null === parsedMarkup || void 0 === parsedMarkup ? void 0 : parsedMarkup[1]
}
export var exportFromMarkup = function(markup, options) {
    options.format = validateFormat(options.format) || DEFAULT_EXPORT_FORMAT;
    options.fileName = options.fileName || "file";
    options.exportingAction = options.onExporting;
    options.exportedAction = options.onExported;
    options.fileSavingAction = options.onFileSaving;
    options.margin = isDefined(options.margin) ? options.margin : MARGIN;
    options.backgroundColor = isDefined(options.backgroundColor) ? options.backgroundColor : getBackgroundColorFromMarkup(markup) || getTheme().backgroundColor;
    _export(markup, options, getCreatorFunc(options.format))
};
export var getMarkup = widgets => combineMarkups(widgets).markup;
export var exportWidgets = function(widgets, options) {
    options = options || {};
    var markupInfo = combineMarkups(widgets, {
        gridLayout: options.gridLayout,
        verticalAlignment: options.verticalAlignment,
        horizontalAlignment: options.horizontalAlignment
    });
    options.width = markupInfo.width;
    options.height = markupInfo.height;
    exportFromMarkup(markupInfo.markup, options)
};
export var combineMarkups = function(widgets) {
    var options = arguments.length > 1 && void 0 !== arguments[1] ? arguments[1] : {};
    if (!Array.isArray(widgets)) {
        widgets = [
            [widgets]
        ]
    } else if (!Array.isArray(widgets[0])) {
        widgets = widgets.map(item => [item])
    }
    var compactView = !options.gridLayout;
    var exportItems = widgets.reduce((r, row, rowIndex) => {
        var rowInfo = row.reduce((r, item, colIndex) => {
            var size = item.getSize();
            var backgroundColor = item.option("backgroundColor") || getTheme(item.option("theme")).backgroundColor;
            backgroundColor && -1 === r.backgroundColors.indexOf(backgroundColor) && r.backgroundColors.push(backgroundColor);
            r.hOffset = r.width;
            r.width += size.width;
            r.height = Math.max(r.height, size.height);
            r.itemWidth = Math.max(r.itemWidth, size.width);
            r.items.push({
                markup: item.svg(),
                width: size.width,
                height: size.height,
                c: colIndex,
                r: rowIndex,
                hOffset: r.hOffset
            });
            return r
        }, {
            items: [],
            height: 0,
            itemWidth: 0,
            hOffset: 0,
            width: 0,
            backgroundColors: r.backgroundColors
        });
        r.rowOffsets.push(r.totalHeight);
        r.rowHeights.push(rowInfo.height);
        r.totalHeight += rowInfo.height;
        r.items = r.items.concat(rowInfo.items);
        r.itemWidth = Math.max(r.itemWidth, rowInfo.itemWidth);
        r.maxItemLen = Math.max(r.maxItemLen, rowInfo.items.length);
        r.totalWidth = compactView ? Math.max(r.totalWidth, rowInfo.width) : r.maxItemLen * r.itemWidth;
        return r
    }, {
        items: [],
        rowOffsets: [],
        rowHeights: [],
        itemWidth: 0,
        totalHeight: 0,
        maxItemLen: 0,
        totalWidth: 0,
        backgroundColors: []
    });
    var backgroundColor = 'data-backgroundcolor="'.concat(1 === exportItems.backgroundColors.length ? exportItems.backgroundColors[0] : "", '" ');
    var getVOffset = item => {
        var align = options.verticalAlignment;
        var dy = exportItems.rowHeights[item.r] - item.height;
        return exportItems.rowOffsets[item.r] + ("bottom" === align ? dy : "center" === align ? dy / 2 : 0)
    };
    var getHOffset = item => {
        if (compactView) {
            return item.hOffset
        }
        var align = options.horizontalAlignment;
        var colWidth = exportItems.itemWidth;
        var dx = colWidth - item.width;
        return item.c * colWidth + ("right" === align ? dx : "center" === align ? dx / 2 : 0)
    };
    var totalHeight = exportItems.totalHeight;
    var totalWidth = exportItems.totalWidth;
    return {
        markup: "<svg " + backgroundColor + 'height="' + totalHeight + '" width="' + totalWidth + '" version="1.1" xmlns="http://www.w3.org/2000/svg">' + exportItems.items.map(item => '<g transform="translate('.concat(getHOffset(item), ",").concat(getVOffset(item), ')">').concat(item.markup, "</g>")).join("") + "</svg>",
        width: totalWidth,
        height: totalHeight
    }
};
export var ExportMenu = function(params) {
    var renderer = this._renderer = params.renderer;
    this._incidentOccurred = params.incidentOccurred;
    this._exportTo = params.exportTo;
    this._print = params.print;
    this._shadow = renderer.shadowFilter("-50%", "-50%", "200%", "200%", SHADOW_OFFSET, 6, SHADOW_BLUR);
    this._shadow.attr({
        opacity: .8
    });
    this._group = renderer.g().attr({
        class: EXPORT_CSS_CLASS,
        [HIDDEN_FOR_EXPORT]: true
    }).linkOn(renderer.root, {
        name: "export-menu",
        after: "peripheral"
    });
    this._buttonGroup = renderer.g().attr({
        class: EXPORT_CSS_CLASS + "-button"
    }).append(this._group);
    this._listGroup = renderer.g().attr({
        class: EXPORT_CSS_CLASS + "-list"
    }).append(this._group);
    this._overlay = renderer.rect(-LIST_WIDTH + BUTTON_SIZE, BUTTON_SIZE + LIST_PADDING_TOP, LIST_WIDTH, 0);
    this._overlay.attr({
        "stroke-width": LIST_STROKE_WIDTH,
        cursor: "pointer",
        rx: 4,
        ry: 4,
        filter: this._shadow.id
    });
    this._overlay.data({
        "export-element-type": "list"
    });
    this.validFormats = getValidFormats();
    this._subscribeEvents()
};
extend(ExportMenu.prototype, {
    getLayoutOptions() {
        if (this._hiddenDueToLayout) {
            return {
                width: 0,
                height: 0,
                cutSide: "vertical",
                cutLayoutSide: "top"
            }
        }
        var bBox = this._buttonGroup.getBBox();
        bBox.cutSide = "vertical";
        bBox.cutLayoutSide = "top";
        bBox.height += MARGIN;
        bBox.position = {
            vertical: "top",
            horizontal: "right"
        };
        bBox.verticalAlignment = "top";
        bBox.horizontalAlignment = "right";
        return bBox
    },
    shift(_, y) {
        this._group.attr({
            translateY: this._group.attr("translateY") + y
        })
    },
    draw(width, height, canvas) {
        this._group.move(width - BUTTON_SIZE - SHADOW_OFFSET - SHADOW_BLUR + canvas.left, Math.floor(height / 2 - BUTTON_SIZE / 2));
        var layoutOptions = this.getLayoutOptions();
        if (layoutOptions.width > width || layoutOptions.height > height) {
            this.freeSpace()
        }
        return this
    },
    show() {
        this._group.linkAppend()
    },
    hide() {
        this._group.linkRemove()
    },
    setOptions(options) {
        this._options = options;
        if (options.formats) {
            options.formats = options.formats.reduce((r, format) => {
                format = validateFormat(format, this._incidentOccurred, this.validFormats);
                format && r.push(format);
                return r
            }, [])
        } else {
            options.formats = this.validFormats.supported.slice()
        }
        options.printingEnabled = void 0 === options.printingEnabled ? true : options.printingEnabled;
        if (options.enabled && (options.formats.length || options.printingEnabled)) {
            this.show();
            this._updateButton();
            this._updateList();
            this._hideList()
        } else {
            this.hide()
        }
    },
    dispose() {
        this._unsubscribeEvents();
        this._group.linkRemove().linkOff();
        this._group.dispose();
        this._shadow.dispose()
    },
    layoutOptions() {
        return this._options.enabled && {
            horizontalAlignment: "right",
            verticalAlignment: "top",
            weak: true
        }
    },
    measure() {
        this._fillSpace();
        var margin = this._options.button.margin;
        return [BUTTON_SIZE + margin.left + margin.right, BUTTON_SIZE + margin.top + margin.bottom]
    },
    move(rect) {
        var margin = this._options.button.margin;
        this._group.attr({
            translateX: Math.round(rect[0]) + margin.left,
            translateY: Math.round(rect[1]) + margin.top
        })
    },
    _fillSpace() {
        this._hiddenDueToLayout = false;
        this.show()
    },
    freeSpace() {
        this._incidentOccurred("W2107");
        this._hiddenDueToLayout = true;
        this.hide()
    },
    _hideList() {
        this._listGroup.remove();
        this._listShown = false;
        this._setButtonState("default");
        this._menuItems.forEach(item => item.resetState())
    },
    _showList() {
        this._listGroup.append(this._group);
        this._listShown = true;
        this._menuItems.forEach(item => item.fixPosition())
    },
    _setButtonState(state) {
        var style = this._options.button[state];
        this._button.attr({
            stroke: style.borderColor,
            fill: style.backgroundColor
        });
        this._icon.attr({
            fill: style.color
        })
    },
    _subscribeEvents() {
        this._renderer.root.on(pointerEvents.up + ".export", e => {
            var elementType = e.target[EXPORT_DATA_KEY];
            if (!elementType) {
                if (this._button) {
                    this._hideList()
                }
                return
            }
            if ("button" === elementType) {
                if (this._listShown) {
                    this._setButtonState("default");
                    this._hideList()
                } else {
                    this._setButtonState("focus");
                    this._showList()
                }
            } else if ("printing" === elementType) {
                this._print();
                this._hideList()
            } else if ("exporting" === elementType) {
                this._exportTo(e.target[FORMAT_DATA_KEY]);
                this._hideList()
            }
        });
        this._listGroup.on(pointerActions, e => e.stopPropagation());
        this._buttonGroup.on(pointerEvents.enter, () => this._setButtonState("hover"));
        this._buttonGroup.on(pointerEvents.leave, () => this._setButtonState(this._listShown ? "focus" : "default"));
        this._buttonGroup.on(pointerEvents.down + ".export", () => this._setButtonState("active"))
    },
    _unsubscribeEvents() {
        this._renderer.root.off(".export");
        this._listGroup.off();
        this._buttonGroup.off()
    },
    _updateButton() {
        var renderer = this._renderer;
        var options = this._options;
        var exportData = {
            "export-element-type": "button"
        };
        if (!this._button) {
            this._button = renderer.rect(0, 0, BUTTON_SIZE, BUTTON_SIZE).append(this._buttonGroup);
            this._button.attr({
                rx: 4,
                ry: 4,
                fill: options.button.default.backgroundColor,
                stroke: options.button.default.borderColor,
                "stroke-width": 1,
                cursor: "pointer"
            });
            this._button.data(exportData);
            this._icon = renderer.path(ICON_COORDS).append(this._buttonGroup);
            this._icon.attr({
                fill: options.button.default.color,
                cursor: "pointer"
            });
            this._icon.data(exportData);
            this._buttonGroup.setTitle(messageLocalization.format("vizExport-titleMenuText"))
        }
    },
    _updateList() {
        var options = this._options;
        var buttonDefault = options.button.default;
        var listGroup = this._listGroup;
        var items = createMenuItems(this._renderer, options);
        this._shadow.attr({
            color: options.shadowColor
        });
        this._overlay.attr({
            height: items.length * MENU_ITEM_HEIGHT + 2 * LIST_STROKE_WIDTH,
            fill: buttonDefault.backgroundColor,
            stroke: buttonDefault.borderColor
        });
        listGroup.clear();
        this._overlay.append(listGroup);
        items.forEach(item => item.g.append(listGroup));
        this._menuItems = items
    }
});

function getExportOptions(widget, exportOptions, fileName, format) {
    if (format || exportOptions.format) {
        format = validateFormat(format || exportOptions.format, widget._incidentOccurred)
    }
    var {
        width: width,
        height: height
    } = widget.getSize();
    return {
        format: format || DEFAULT_EXPORT_FORMAT,
        fileName: fileName || exportOptions.fileName || "file",
        proxyUrl: exportOptions.proxyUrl,
        backgroundColor: exportOptions.backgroundColor,
        width: width,
        height: height,
        margin: exportOptions.margin,
        svgToCanvas: exportOptions.svgToCanvas,
        forceProxy: exportOptions.forceProxy,
        exportingAction: widget._createActionByOption("onExporting"),
        exportedAction: widget._createActionByOption("onExported"),
        fileSavingAction: widget._createActionByOption("onFileSaving")
    }
}
export var plugin = {
    name: "export",
    init() {
        this._exportMenu = new ExportMenu({
            renderer: this._renderer,
            incidentOccurred: this._incidentOccurred,
            print: () => this.print(),
            exportTo: format => this.exportTo(void 0, format)
        });
        this._layout.add(this._exportMenu)
    },
    dispose() {
        this._exportMenu.dispose()
    },
    members: {
        _getExportMenuOptions() {
            return extend({}, this._getOption("export"), {
                rtl: this._getOption("rtlEnabled", true)
            })
        },
        _disablePointerEvents() {
            var pointerEventsValue = this._renderer.root.attr("pointer-events");
            this._renderer.root.attr({
                "pointer-events": "none"
            });
            return pointerEventsValue
        },
        exportTo(fileName, format) {
            var menu = this._exportMenu;
            var options = getExportOptions(this, this._getOption("export") || {}, fileName, format);
            menu && menu.hide();
            var pointerEventsValue = this._disablePointerEvents();
            var promise = _export(this._renderer.root.element, options, getCreatorFunc(options.format)).fail(logger.error).always(() => {
                this._renderer.root.attr({
                    "pointer-events": pointerEventsValue
                })
            });
            menu && menu.show();
            return promise
        },
        print() {
            var menu = this._exportMenu;
            var options = getExportOptions(this, this._getOption("export") || {});
            options.exportingAction = null;
            options.exportedAction = null;
            options.margin = 0;
            options.format = "PNG";
            options.forceProxy = true;
            options.fileSavingAction = eventArgs => {
                print("data:image/png;base64,".concat(eventArgs.data), {
                    width: options.width,
                    __test: options.__test
                });
                eventArgs.cancel = true
            };
            var pointerEventsValue = this._disablePointerEvents();
            menu && menu.hide();
            var promise = _export(this._renderer.root.element, options, getCreatorFunc(options.format)).fail(logger.error).always(() => {
                this._renderer.root.attr({
                    "pointer-events": pointerEventsValue
                })
            });
            menu && menu.show();
            return promise
        }
    },
    customize(constructor) {
        var proto = constructor.prototype;
        constructor.addChange({
            code: "EXPORT",
            handler() {
                this._exportMenu.setOptions(this._getExportMenuOptions());
                this._change(["LAYOUT"])
            },
            isThemeDependent: true,
            isOptionChange: true,
            option: "export"
        });
        proto._optionChangesMap.onExporting = "EXPORT";
        proto._optionChangesMap.onExported = "EXPORT";
        proto._optionChangesMap.onFileSaving = "EXPORT"
    },
    fontFields: ["export.font"]
};
