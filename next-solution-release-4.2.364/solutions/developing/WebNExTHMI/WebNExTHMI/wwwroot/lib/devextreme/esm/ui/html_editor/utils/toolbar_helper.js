/**
 * DevExtreme (esm/ui/html_editor/utils/toolbar_helper.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import $ from "../../../core/renderer";
import localizationMessage from "../../../localization/message";
import {
    getTableOperationHandler,
    hasEmbedContent,
    unfixTableWidth,
    getColumnElements,
    getAutoSizedElements,
    setLineElementsFormat,
    getLineElements,
    getRowElements
} from "./table_helper";
import {
    isDefined,
    isBoolean
} from "../../../core/utils/type";
import {
    each
} from "../../../core/utils/iterator";
import Form from "../../form";
import ButtonGroup from "../../button_group";
import ColorBox from "../../color_box";
import ScrollView from "../../scroll_view";
import {
    getOuterHeight,
    getWidth,
    getOuterWidth
} from "../../../core/utils/size";
import {
    getWindow
} from "../../../core/utils/window";
var MIN_HEIGHT = 400;
var BORDER_STYLES = ["none", "hidden", "dotted", "dashed", "solid", "double", "groove", "ridge", "inset", "outset"];
var USER_ACTION = "user";
var SILENT_ACTION = "silent";
var DIALOG_COLOR_CAPTION = "dxHtmlEditor-dialogColorCaption";
var DIALOG_BACKGROUND_CAPTION = "dxHtmlEditor-dialogBackgroundCaption";
var DIALOG_LINK_CAPTION = "dxHtmlEditor-dialogLinkCaption";
var DIALOG_IMAGE_CAPTION = "dxHtmlEditor-dialogImageCaption";
var DIALOG_TABLE_CAPTION = "dxHtmlEditor-dialogInsertTableCaption";
var DIALOG_LINK_FIELD_URL = "dxHtmlEditor-dialogLinkUrlField";
var DIALOG_LINK_FIELD_TEXT = "dxHtmlEditor-dialogLinkTextField";
var DIALOG_LINK_FIELD_TARGET = "dxHtmlEditor-dialogLinkTargetField";
var DIALOG_LINK_FIELD_TARGET_CLASS = "dx-formdialog-field-target";
var DIALOG_IMAGE_FIELD_URL = "dxHtmlEditor-dialogImageUrlField";
var DIALOG_IMAGE_FIELD_ALT = "dxHtmlEditor-dialogImageAltField";
var DIALOG_IMAGE_FIELD_WIDTH = "dxHtmlEditor-dialogImageWidthField";
var DIALOG_IMAGE_FIELD_HEIGHT = "dxHtmlEditor-dialogImageHeightField";
var DIALOG_TABLE_FIELD_COLUMNS = "dxHtmlEditor-dialogInsertTableRowsField";
var DIALOG_TABLE_FIELD_ROWS = "dxHtmlEditor-dialogInsertTableColumnsField";
var ICON_MAP = {
    insertHeaderRow: "header",
    clear: "clearformat"
};

function getFormatHandlers(module) {
    return {
        clear: _ref => {
            var {
                event: event
            } = _ref;
            var range = module.quill.getSelection();
            if (range) {
                var _getToolbarModule;
                module.saveValueChangeEvent(event);
                module.quill.removeFormat(range);
                null === (_getToolbarModule = getToolbarModule(module)) || void 0 === _getToolbarModule ? void 0 : _getToolbarModule.updateFormatWidgets()
            }
        },
        link: prepareLinkHandler(module),
        image: prepareImageHandler(module),
        color: prepareColorClickHandler(module, "color"),
        background: prepareColorClickHandler(module, "background"),
        orderedList: prepareShortcutHandler(module, "list", "ordered"),
        bulletList: prepareShortcutHandler(module, "list", "bullet"),
        alignLeft: prepareShortcutHandler(module, "align", "left"),
        alignCenter: prepareShortcutHandler(module, "align", "center"),
        alignRight: prepareShortcutHandler(module, "align", "right"),
        alignJustify: prepareShortcutHandler(module, "align", "justify"),
        codeBlock: getDefaultClickHandler(module, "code-block"),
        undo: _ref2 => {
            var {
                event: event
            } = _ref2;
            module.saveValueChangeEvent(event);
            module.quill.history.undo()
        },
        redo: _ref3 => {
            var {
                event: event
            } = _ref3;
            module.saveValueChangeEvent(event);
            module.quill.history.redo()
        },
        increaseIndent: _ref4 => {
            var {
                event: event
            } = _ref4;
            applyFormat(module, ["indent", "+1", USER_ACTION], event)
        },
        decreaseIndent: _ref5 => {
            var {
                event: event
            } = _ref5;
            applyFormat(module, ["indent", "-1", USER_ACTION], event)
        },
        superscript: prepareShortcutHandler(module, "script", "super"),
        subscript: prepareShortcutHandler(module, "script", "sub"),
        insertTable: prepareInsertTableHandler(module),
        insertHeaderRow: getTableOperationHandler(module.quill, "insertHeaderRow"),
        insertRowAbove: getTableOperationHandler(module.quill, "insertRowAbove"),
        insertRowBelow: getTableOperationHandler(module.quill, "insertRowBelow"),
        insertColumnLeft: getTableOperationHandler(module.quill, "insertColumnLeft"),
        insertColumnRight: getTableOperationHandler(module.quill, "insertColumnRight"),
        deleteColumn: getTableOperationHandler(module.quill, "deleteColumn"),
        deleteRow: getTableOperationHandler(module.quill, "deleteRow"),
        deleteTable: getTableOperationHandler(module.quill, "deleteTable"),
        cellProperties: prepareShowFormProperties(module, "cell"),
        tableProperties: prepareShowFormProperties(module, "table")
    }
}

function resetFormDialogOptions(editorInstance, _ref6) {
    var {
        contentTemplate: contentTemplate,
        title: title,
        minHeight: minHeight,
        minWidth: minWidth,
        maxWidth: maxWidth
    } = _ref6;
    editorInstance.formDialogOption({
        contentTemplate: contentTemplate,
        title: title,
        minHeight: null !== minHeight && void 0 !== minHeight ? minHeight : 0,
        minWidth: null !== minWidth && void 0 !== minWidth ? minWidth : 0,
        maxWidth: null !== maxWidth && void 0 !== maxWidth ? maxWidth : "none"
    })
}

function prepareShowFormProperties(module, type) {
    return $element => {
        var _$element, _module$quill$getModu;
        if (!(null !== (_$element = $element) && void 0 !== _$element && _$element.length)) {
            $element = $(getTargetTableNode(module, type))
        }
        var [tableBlot, rowBlot] = null !== (_module$quill$getModu = module.quill.getModule("table").getTable()) && void 0 !== _module$quill$getModu ? _module$quill$getModu : [];
        var formats = module.quill.getFormat(module.editorInstance.getSelection(true));
        var tablePropertiesFormConfig = getFormConfigConstructor(type)(module, {
            $element: $element,
            formats: formats,
            tableBlot: tableBlot,
            rowBlot: rowBlot
        });
        var {
            contentTemplate: contentTemplate,
            title: title,
            minHeight: minHeight,
            minWidth: minWidth,
            maxWidth: maxWidth
        } = module.editorInstance._formDialog._popup.option();
        var savedOptions = {
            contentTemplate: contentTemplate,
            title: title,
            minHeight: minHeight,
            minWidth: minWidth,
            maxWidth: maxWidth
        };
        var formInstance;
        module.editorInstance.formDialogOption({
            contentTemplate: container => {
                var $content = $("<div>").appendTo(container);
                var $form = $("<div>").appendTo($content);
                module.editorInstance._createComponent($form, Form, tablePropertiesFormConfig.formOptions);
                module.editorInstance._createComponent($content, ScrollView, {});
                formInstance = $form.dxForm("instance");
                return $content
            },
            title: localizationMessage.format("dxHtmlEditor-".concat(type, "Properties")),
            minHeight: MIN_HEIGHT,
            minWidth: Math.min(800, .9 * getWidth(getWindow()) - 1),
            maxWidth: .9 * getWidth(getWindow())
        });
        var promise = module.editorInstance.showFormDialog();
        promise.done((formData, event) => {
            module.saveValueChangeEvent(event);
            tablePropertiesFormConfig.applyHandler(formInstance);
            resetFormDialogOptions(module.editorInstance, savedOptions)
        });
        promise.fail(() => {
            module.quill.focus();
            resetFormDialogOptions(module.editorInstance, savedOptions)
        })
    }
}

function applyFormat(module, formatArgs, event) {
    module.editorInstance._saveValueChangeEvent(event);
    module.quill.format(...formatArgs)
}

function getTargetTableNode(module, partName) {
    var currentSelectionParts = module.quill.getModule("table").getTable();
    return "table" === partName ? currentSelectionParts[0].domNode : currentSelectionParts[2].domNode
}

function prepareLinkHandler(module) {
    return () => {
        module.quill.focus();
        var selection = module.quill.getSelection();
        var selectionHasEmbedContent = hasEmbedContent(module, selection);
        var formats = selection ? module.quill.getFormat() : {};
        var formData = {
            href: formats.link || "",
            text: selection && !selectionHasEmbedContent ? module.quill.getText(selection) : "",
            target: Object.prototype.hasOwnProperty.call(formats, "target") ? !!formats.target : true
        };
        module.editorInstance.formDialogOption("title", localizationMessage.format(DIALOG_LINK_CAPTION));
        var promise = module.editorInstance.showFormDialog({
            formData: formData,
            items: getLinkFormItems(module, selection)
        });
        promise.done((formData, event) => {
            if (selection && !selectionHasEmbedContent) {
                var text = formData.text || formData.href;
                var {
                    index: index,
                    length: length
                } = selection;
                formData.text = void 0;
                module.saveValueChangeEvent(event);
                length && module.quill.deleteText(index, length, SILENT_ACTION);
                module.quill.insertText(index, text, "link", formData, USER_ACTION);
                module.quill.setSelection(index + text.length, 0, USER_ACTION)
            } else {
                formData.text = !selection && !formData.text ? formData.href : formData.text;
                applyFormat(module, ["link", formData, USER_ACTION], event)
            }
        });
        promise.fail(() => {
            module.quill.focus()
        })
    }
}

function prepareImageHandler(module) {
    return () => {
        var formData = module.quill.getFormat();
        var isUpdateDialog = Object.prototype.hasOwnProperty.call(formData, "imageSrc");
        var defaultIndex = defaultPasteIndex(module);
        if (isUpdateDialog) {
            var {
                imageSrc: imageSrc
            } = module.quill.getFormat(defaultIndex - 1, 1);
            formData.src = formData.imageSrc;
            delete formData.imageSrc;
            if (!imageSrc || 0 === defaultIndex) {
                module.quill.setSelection(defaultIndex + 1, 0, SILENT_ACTION)
            }
        }
        var formatIndex = embedFormatIndex(module);
        module.editorInstance.formDialogOption("title", localizationMessage.format(DIALOG_IMAGE_CAPTION));
        var promise = module.editorInstance.showFormDialog({
            formData: formData,
            items: imageFormItems()
        });
        promise.done((formData, event) => {
            var index = defaultIndex;
            module.saveValueChangeEvent(event);
            if (isUpdateDialog) {
                index = formatIndex;
                module.quill.deleteText(index, 1, SILENT_ACTION)
            }
            module.quill.insertEmbed(index, "extendedImage", formData, USER_ACTION);
            module.quill.setSelection(index + 1, 0, USER_ACTION)
        }).always(() => {
            module.quill.focus()
        })
    }
}

function getLinkFormItems(module, selection) {
    return [{
        dataField: "href",
        label: {
            text: localizationMessage.format(DIALOG_LINK_FIELD_URL)
        }
    }, {
        dataField: "text",
        label: {
            text: localizationMessage.format(DIALOG_LINK_FIELD_TEXT)
        },
        visible: !hasEmbedContent(module, selection)
    }, {
        dataField: "target",
        editorType: "dxCheckBox",
        editorOptions: {
            text: localizationMessage.format(DIALOG_LINK_FIELD_TARGET)
        },
        cssClass: DIALOG_LINK_FIELD_TARGET_CLASS,
        label: {
            visible: false
        }
    }]
}

function embedFormatIndex(module) {
    var selection = module.quill.getSelection();
    if (selection) {
        if (selection.length) {
            return selection.index
        } else {
            return selection.index - 1
        }
    } else {
        return module.quill.getLength()
    }
}

function defaultPasteIndex(module) {
    var _selection$index;
    var selection = module.quill.getSelection();
    return null !== (_selection$index = null === selection || void 0 === selection ? void 0 : selection.index) && void 0 !== _selection$index ? _selection$index : module.quill.getLength()
}

function imageFormItems() {
    return [{
        dataField: "src",
        label: {
            text: localizationMessage.format(DIALOG_IMAGE_FIELD_URL)
        }
    }, {
        dataField: "width",
        label: {
            text: localizationMessage.format(DIALOG_IMAGE_FIELD_WIDTH)
        }
    }, {
        dataField: "height",
        label: {
            text: localizationMessage.format(DIALOG_IMAGE_FIELD_HEIGHT)
        }
    }, {
        dataField: "alt",
        label: {
            text: localizationMessage.format(DIALOG_IMAGE_FIELD_ALT)
        }
    }]
}

function prepareColorClickHandler(module, name) {
    return () => {
        var formData = module.quill.getFormat();
        var caption = "color" === name ? DIALOG_COLOR_CAPTION : DIALOG_BACKGROUND_CAPTION;
        module.editorInstance.formDialogOption("title", localizationMessage.format(caption));
        var promise = module.editorInstance.showFormDialog({
            formData: formData,
            items: [{
                dataField: name,
                editorType: "dxColorView",
                editorOptions: {
                    focusStateEnabled: false
                },
                label: {
                    visible: false
                }
            }]
        });
        promise.done((formData, event) => {
            applyFormat(module, [name, formData[name], USER_ACTION], event)
        });
        promise.fail(() => {
            module.quill.focus()
        })
    }
}

function prepareShortcutHandler(module, name, shortcutValue) {
    return _ref7 => {
        var _getToolbarModule2;
        var {
            event: event
        } = _ref7;
        var formats = module.quill.getFormat();
        var value = formats[name] === shortcutValue ? false : shortcutValue;
        applyFormat(module, [name, value, USER_ACTION], event);
        null === (_getToolbarModule2 = getToolbarModule(module)) || void 0 === _getToolbarModule2 ? void 0 : _getToolbarModule2.updateFormatWidgets(true)
    }
}

function getToolbarModule(module) {
    return module._updateFormatWidget ? module : module.quill.getModule("toolbar")
}

function getDefaultClickHandler(module, name) {
    return _ref8 => {
        var _getToolbarModule3;
        var {
            event: event
        } = _ref8;
        var formats = module.quill.getFormat();
        var value = formats[name];
        var newValue = !(isBoolean(value) ? value : isDefined(value));
        applyFormat(module, [name, newValue, USER_ACTION], event);
        null === (_getToolbarModule3 = getToolbarModule(module)) || void 0 === _getToolbarModule3 ? void 0 : _getToolbarModule3._updateFormatWidget(name, newValue, formats)
    }
}

function insertTableFormItems() {
    return [{
        dataField: "columns",
        editorType: "dxNumberBox",
        editorOptions: {
            min: 1
        },
        label: {
            text: localizationMessage.format(DIALOG_TABLE_FIELD_COLUMNS)
        }
    }, {
        dataField: "rows",
        editorType: "dxNumberBox",
        editorOptions: {
            min: 1
        },
        label: {
            text: localizationMessage.format(DIALOG_TABLE_FIELD_ROWS)
        }
    }]
}

function prepareInsertTableHandler(module) {
    return () => {
        var formats = module.quill.getFormat();
        var isTableFocused = module._tableFormats.some(format => Object.prototype.hasOwnProperty.call(formats, format));
        if (isTableFocused) {
            module.quill.focus();
            return
        }
        module.editorInstance.formDialogOption("title", localizationMessage.format(DIALOG_TABLE_CAPTION));
        var promise = module.editorInstance.showFormDialog({
            formData: {
                rows: 1,
                columns: 1
            },
            items: insertTableFormItems()
        });
        promise.done((formData, event) => {
            module.quill.focus();
            var table = module.quill.getModule("table");
            if (table) {
                module.saveValueChangeEvent(event);
                var {
                    columns: columns,
                    rows: rows
                } = formData;
                table.insertTable(columns, rows)
            }
        }).always(() => {
            module.quill.focus()
        })
    }
}

function getTablePropertiesFormConfig(module, _ref9) {
    var {
        $element: $element,
        formats: formats,
        tableBlot: tableBlot
    } = _ref9;
    var window = getWindow();
    var alignmentEditorInstance;
    var borderColorEditorInstance;
    var backgroundColorEditorInstance;
    var $table = $element;
    var editorInstance = module.editorInstance;
    var startTableWidth = isDefined(formats.tableWidth) ? parseInt(formats.tableWidth) : getOuterWidth($table);
    var tableStyles = window.getComputedStyle($table.get(0));
    var startTextAlign = "start" === tableStyles.textAlign ? "left" : tableStyles.textAlign;
    var formOptions = {
        colCount: 2,
        formData: {
            width: startTableWidth,
            height: isDefined(formats.tableHeight) ? parseInt(formats.tableHeight) : getOuterHeight($table),
            backgroundColor: formats.tableBackgroundColor || tableStyles.backgroundColor,
            borderStyle: formats.tableBorderStyle || tableStyles.borderTopStyle,
            borderColor: formats.tableBorderColor || tableStyles.borderTopColor,
            borderWidth: parseInt(isDefined(formats.tableBorderWidth) ? formats.tableBorderWidth : tableStyles.borderTopWidth),
            alignment: formats.tableAlign || startTextAlign
        },
        items: [{
            itemType: "group",
            caption: localizationMessage.format("dxHtmlEditor-border"),
            colCountByScreen: {
                xs: 2
            },
            colCount: 2,
            items: [{
                dataField: "borderStyle",
                label: {
                    text: localizationMessage.format("dxHtmlEditor-style")
                },
                editorType: "dxSelectBox",
                editorOptions: {
                    items: BORDER_STYLES,
                    placeholder: "Select style"
                }
            }, {
                dataField: "borderWidth",
                label: {
                    text: localizationMessage.format("dxHtmlEditor-borderWidth")
                },
                editorOptions: {
                    placeholder: localizationMessage.format("dxHtmlEditor-pixels")
                }
            }, {
                itemType: "simple",
                dataField: "borderColor",
                label: {
                    text: localizationMessage.format("dxHtmlEditor-borderColor")
                },
                colSpan: 2,
                template: e => {
                    var $content = $("<div>");
                    editorInstance._createComponent($content, ColorBox, {
                        editAlphaChannel: true,
                        value: e.component.option("formData").borderColor,
                        onInitialized: e => {
                            borderColorEditorInstance = e.component
                        }
                    });
                    return $content
                }
            }]
        }, {
            itemType: "group",
            caption: localizationMessage.format("dxHtmlEditor-dimensions"),
            colCountByScreen: {
                xs: 2
            },
            colCount: 2,
            items: [{
                dataField: "width",
                label: {
                    text: localizationMessage.format("dxHtmlEditor-width")
                },
                editorOptions: {
                    min: 0,
                    placeholder: localizationMessage.format("dxHtmlEditor-pixels")
                }
            }, {
                dataField: "height",
                label: {
                    text: localizationMessage.format("dxHtmlEditor-height")
                },
                editorOptions: {
                    min: 0,
                    placeholder: localizationMessage.format("dxHtmlEditor-pixels")
                }
            }]
        }, {
            itemType: "group",
            caption: localizationMessage.format("dxHtmlEditor-tableBackground"),
            items: [{
                itemType: "simple",
                dataField: "backgroundColor",
                label: {
                    text: localizationMessage.format("dxHtmlEditor-borderColor")
                },
                template: e => {
                    var $content = $("<div>");
                    editorInstance._createComponent($content, ColorBox, {
                        editAlphaChannel: true,
                        value: e.component.option("formData").backgroundColor,
                        onInitialized: e => {
                            backgroundColorEditorInstance = e.component
                        }
                    });
                    return $content
                }
            }]
        }, {
            itemType: "group",
            caption: localizationMessage.format("dxHtmlEditor-alignment"),
            items: [{
                itemType: "simple",
                label: {
                    text: localizationMessage.format("dxHtmlEditor-horizontal")
                },
                template: () => {
                    var $content = $("<div>");
                    editorInstance._createComponent($content, ButtonGroup, {
                        items: [{
                            value: "left",
                            icon: "alignleft"
                        }, {
                            value: "center",
                            icon: "aligncenter"
                        }, {
                            value: "right",
                            icon: "alignright"
                        }, {
                            value: "justify",
                            icon: "alignjustify"
                        }],
                        keyExpr: "value",
                        selectedItemKeys: [startTextAlign],
                        onInitialized: e => {
                            alignmentEditorInstance = e.component
                        }
                    });
                    return $content
                }
            }]
        }],
        showColonAfterLabel: true,
        labelLocation: "top",
        minColWidth: 400
    };
    return {
        formOptions: formOptions,
        applyHandler: formInstance => {
            var formData = formInstance.option("formData");
            var newWidth = formData.width === startTableWidth ? void 0 : formData.width;
            var newHeight = formData.height;
            applyTableDimensionChanges(module, {
                $table: $table,
                newHeight: newHeight,
                newWidth: newWidth,
                tableBlot: tableBlot
            });
            module.editorInstance.format("tableBorderStyle", formData.borderStyle);
            module.editorInstance.format("tableBorderWidth", formData.borderWidth + "px");
            module.editorInstance.format("tableBorderColor", borderColorEditorInstance.option("value"));
            module.editorInstance.format("tableBackgroundColor", backgroundColorEditorInstance.option("value"));
            module.editorInstance.format("tableTextAlign", alignmentEditorInstance.option("selectedItemKeys")[0])
        }
    }
}

function getCellPropertiesFormConfig(module, _ref10) {
    var {
        $element: $element,
        formats: formats,
        tableBlot: tableBlot,
        rowBlot: rowBlot
    } = _ref10;
    var window = getWindow();
    var alignmentEditorInstance;
    var verticalAlignmentEditorInstance;
    var borderColorEditorInstance;
    var backgroundColorEditorInstance;
    var $cell = $element;
    var startCellWidth = isDefined(formats.cellWidth) ? parseInt(formats.cellWidth) : getOuterWidth($cell);
    var editorInstance = module.editorInstance;
    var cellStyles = window.getComputedStyle($cell.get(0));
    var startTextAlign = "start" === cellStyles.textAlign ? "left" : cellStyles.textAlign;
    var formOptions = {
        colCount: 2,
        formData: {
            width: startCellWidth,
            height: isDefined(formats.cellHeight) ? parseInt(formats.cellHeight) : getOuterHeight($cell),
            backgroundColor: formats.cellBackgroundColor || cellStyles.backgroundColor,
            borderStyle: formats.cellBorderStyle || cellStyles.borderTopStyle,
            borderColor: formats.cellBorderColor || cellStyles.borderTopColor,
            borderWidth: parseInt(isDefined(formats.cellBorderWidth) ? formats.cellBorderWidth : cellStyles.borderTopWidth),
            alignment: formats.cellTextAlign || startTextAlign,
            verticalAlignment: formats.cellVerticalAlign || cellStyles.verticalAlign,
            verticalPadding: parseInt(isDefined(formats.cellPaddingTop) ? formats.cellPaddingTop : cellStyles.paddingTop),
            horizontalPadding: parseInt(isDefined(formats.cellPaddingLeft) ? formats.cellPaddingLeft : cellStyles.paddingLeft)
        },
        items: [{
            itemType: "group",
            caption: localizationMessage.format("dxHtmlEditor-border"),
            colCountByScreen: {
                xs: 2
            },
            colCount: 2,
            items: [{
                dataField: "borderStyle",
                label: {
                    text: localizationMessage.format("dxHtmlEditor-style")
                },
                editorType: "dxSelectBox",
                editorOptions: {
                    items: BORDER_STYLES
                }
            }, {
                dataField: "borderWidth",
                label: {
                    text: localizationMessage.format("dxHtmlEditor-borderWidth")
                },
                editorOptions: {
                    placeholder: localizationMessage.format("dxHtmlEditor-pixels")
                }
            }, {
                itemType: "simple",
                dataField: "borderColor",
                colSpan: 2,
                label: {
                    text: localizationMessage.format("dxHtmlEditor-borderColor")
                },
                template: e => {
                    var $content = $("<div>");
                    editorInstance._createComponent($content, ColorBox, {
                        editAlphaChannel: true,
                        value: e.component.option("formData").borderColor,
                        onInitialized: e => {
                            borderColorEditorInstance = e.component
                        }
                    });
                    return $content
                }
            }]
        }, {
            itemType: "group",
            caption: localizationMessage.format("dxHtmlEditor-dimensions"),
            colCount: 2,
            colCountByScreen: {
                xs: 2
            },
            items: [{
                dataField: "width",
                label: {
                    text: localizationMessage.format("dxHtmlEditor-width")
                },
                editorOptions: {
                    min: 0,
                    placeholder: localizationMessage.format("dxHtmlEditor-pixels")
                }
            }, {
                dataField: "height",
                label: {
                    text: localizationMessage.format("dxHtmlEditor-height")
                },
                editorOptions: {
                    min: 0,
                    placeholder: localizationMessage.format("dxHtmlEditor-pixels")
                }
            }, {
                dataField: "verticalPadding",
                label: {
                    text: localizationMessage.format("dxHtmlEditor-paddingVertical")
                },
                editorOptions: {
                    placeholder: localizationMessage.format("dxHtmlEditor-pixels")
                }
            }, {
                label: {
                    text: localizationMessage.format("dxHtmlEditor-paddingHorizontal")
                },
                dataField: "horizontalPadding",
                editorOptions: {
                    placeholder: localizationMessage.format("dxHtmlEditor-pixels")
                }
            }]
        }, {
            itemType: "group",
            caption: localizationMessage.format("dxHtmlEditor-tableBackground"),
            items: [{
                itemType: "simple",
                dataField: "backgroundColor",
                label: {
                    text: localizationMessage.format("dxHtmlEditor-borderColor")
                },
                template: e => {
                    var $content = $("<div>");
                    editorInstance._createComponent($content, ColorBox, {
                        editAlphaChannel: true,
                        value: e.component.option("formData").backgroundColor,
                        onInitialized: e => {
                            backgroundColorEditorInstance = e.component
                        }
                    });
                    return $content
                }
            }]
        }, {
            itemType: "group",
            caption: localizationMessage.format("dxHtmlEditor-alignment"),
            colCount: 2,
            items: [{
                itemType: "simple",
                label: {
                    text: localizationMessage.format("dxHtmlEditor-horizontal")
                },
                template: () => {
                    var $content = $("<div>");
                    editorInstance._createComponent($content, ButtonGroup, {
                        items: [{
                            value: "left",
                            icon: "alignleft"
                        }, {
                            value: "center",
                            icon: "aligncenter"
                        }, {
                            value: "right",
                            icon: "alignright"
                        }, {
                            value: "justify",
                            icon: "alignjustify"
                        }],
                        keyExpr: "value",
                        selectedItemKeys: [startTextAlign],
                        onInitialized: e => {
                            alignmentEditorInstance = e.component
                        }
                    });
                    return $content
                }
            }, {
                itemType: "simple",
                label: {
                    text: localizationMessage.format("dxHtmlEditor-vertical")
                },
                template: () => {
                    var $content = $("<div>");
                    editorInstance._createComponent($content, ButtonGroup, {
                        items: [{
                            value: "top",
                            icon: "verticalaligntop"
                        }, {
                            value: "middle",
                            icon: "verticalaligncenter"
                        }, {
                            value: "bottom",
                            icon: "verticalalignbottom"
                        }],
                        keyExpr: "value",
                        selectedItemKeys: [cellStyles.verticalAlign],
                        onInitialized: e => {
                            verticalAlignmentEditorInstance = e.component
                        }
                    });
                    return $content
                }
            }]
        }],
        showColonAfterLabel: true,
        labelLocation: "top",
        minColWidth: 400
    };
    return {
        formOptions: formOptions,
        applyHandler: formInstance => {
            var formData = formInstance.option("formData");
            var newWidth = formData.width === parseInt(startCellWidth) ? void 0 : formData.width;
            var newHeight = formData.height;
            applyCellDimensionChanges(module, {
                $cell: $cell,
                newHeight: newHeight,
                newWidth: newWidth,
                tableBlot: tableBlot,
                rowBlot: rowBlot
            });
            module.editorInstance.format("cellBorderWidth", formData.borderWidth + "px");
            module.editorInstance.format("cellBorderColor", borderColorEditorInstance.option("value"));
            module.editorInstance.format("cellBorderStyle", formData.borderStyle);
            module.editorInstance.format("cellBackgroundColor", backgroundColorEditorInstance.option("value"));
            module.editorInstance.format("cellTextAlign", alignmentEditorInstance.option("selectedItemKeys")[0]);
            module.editorInstance.format("cellVerticalAlign", verticalAlignmentEditorInstance.option("selectedItemKeys")[0]);
            module.editorInstance.format("cellPaddingLeft", formData.horizontalPadding + "px");
            module.editorInstance.format("cellPaddingRight", formData.horizontalPadding + "px");
            module.editorInstance.format("cellPaddingTop", formData.verticalPadding + "px");
            module.editorInstance.format("cellPaddingBottom", formData.verticalPadding + "px")
        }
    }
}

function getFormConfigConstructor(type) {
    return "cell" === type ? getCellPropertiesFormConfig : getTablePropertiesFormConfig
}

function applyTableDimensionChanges(module, _ref11) {
    var {
        $table: $table,
        newHeight: newHeight,
        newWidth: newWidth,
        tableBlot: tableBlot
    } = _ref11;
    if (isDefined(newWidth)) {
        var autoWidthColumns = getAutoSizedElements($table);
        if (autoWidthColumns.length > 0) {
            module.editorInstance.format("tableWidth", newWidth + "px")
        } else {
            var $columns = getColumnElements($table);
            var oldTableWidth = getOuterWidth($table);
            unfixTableWidth($table, {
                tableBlot: tableBlot
            });
            each($columns, (i, element) => {
                var $element = $(element);
                var newElementWidth = newWidth / oldTableWidth * getOuterWidth($element);
                var $lineElements = getLineElements($table, $element.index(), "horizontal");
                setLineElementsFormat(module, {
                    elements: $lineElements,
                    property: "width",
                    value: newElementWidth
                })
            })
        }
    }
    var autoHeightRows = getAutoSizedElements($table, "vertical");
    if ((null === autoHeightRows || void 0 === autoHeightRows ? void 0 : autoHeightRows.length) > 0) {
        tableBlot.format("tableHeight", newHeight + "px")
    } else {
        var $rows = getRowElements($table);
        var oldTableHeight = getOuterHeight($table);
        each($rows, (i, element) => {
            var $element = $(element);
            var newElementHeight = newHeight / oldTableHeight * getOuterHeight($element);
            var $lineElements = getLineElements($table, i, "vertical");
            setLineElementsFormat(module, {
                elements: $lineElements,
                property: "height",
                value: newElementHeight
            })
        })
    }
}

function applyCellDimensionChanges(module, _ref12) {
    var {
        $cell: $cell,
        newHeight: newHeight,
        newWidth: newWidth,
        tableBlot: tableBlot,
        rowBlot: rowBlot
    } = _ref12;
    var $table = $($cell.closest("table"));
    if (isDefined(newWidth)) {
        var index = $($cell).index();
        var $verticalCells = getLineElements($table, index);
        var widthDiff = newWidth - getOuterWidth($cell);
        var tableWidth = getOuterWidth($table);
        if (newWidth > tableWidth) {
            unfixTableWidth($table, {
                tableBlot: tableBlot
            })
        }
        setLineElementsFormat(module, {
            elements: $verticalCells,
            property: "width",
            value: newWidth
        });
        var $nextColumnCell = $cell.next();
        var shouldUpdateNearestColumnWidth = 0 === getAutoSizedElements($table).length;
        if (shouldUpdateNearestColumnWidth) {
            unfixTableWidth($table, {
                tableBlot: tableBlot
            });
            if (1 === $nextColumnCell.length) {
                $verticalCells = getLineElements($table, index + 1);
                var nextColumnWidth = getOuterWidth($verticalCells.eq(0)) - widthDiff;
                setLineElementsFormat(module, {
                    elements: $verticalCells,
                    property: "width",
                    value: Math.max(nextColumnWidth, 0)
                })
            } else {
                var $prevColumnCell = $cell.prev();
                if (1 === $prevColumnCell.length) {
                    $verticalCells = getLineElements($table, index - 1);
                    var prevColumnWidth = getOuterWidth($verticalCells.eq(0)) - widthDiff;
                    setLineElementsFormat(module, {
                        elements: $verticalCells,
                        property: "width",
                        value: Math.max(prevColumnWidth, 0)
                    })
                }
            }
        }
    }
    rowBlot.children.forEach(rowCell => {
        rowCell.format("cellHeight", newHeight + "px")
    });
    var autoHeightRows = getAutoSizedElements($table, "vertical");
    if (0 === autoHeightRows.length) {
        $table.css("height", "auto")
    }
}
export {
    getFormatHandlers,
    getDefaultClickHandler,
    ICON_MAP,
    applyFormat
};
