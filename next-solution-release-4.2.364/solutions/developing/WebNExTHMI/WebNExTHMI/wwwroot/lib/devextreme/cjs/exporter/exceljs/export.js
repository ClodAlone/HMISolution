/**
 * DevExtreme (cjs/exporter/exceljs/export.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
"use strict";
exports.Export = void 0;
var _type = require("../../core/utils/type");
var _export_format = require("./export_format");
var _export_merged_ranges_manager = require("./export_merged_ranges_manager");
var _extend = require("../../core/utils/extend");
var _export_load_panel = require("../common/export_load_panel");
var _window = require("../../core/utils/window");
var MAX_DIGIT_WIDTH_IN_PIXELS = 7;
var MAX_EXCEL_COLUMN_WIDTH = 255;
var Export = {
    getFullOptions: function(options) {
        var fullOptions = (0, _extend.extend)({}, options);
        if (!((0, _type.isDefined)(fullOptions.worksheet) && (0, _type.isObject)(fullOptions.worksheet))) {
            throw Error('The "worksheet" field must contain an object.')
        }
        if (!(0, _type.isDefined)(fullOptions.topLeftCell)) {
            fullOptions.topLeftCell = {
                row: 1,
                column: 1
            }
        } else if ((0, _type.isString)(fullOptions.topLeftCell)) {
            var _fullOptions$workshee = fullOptions.worksheet.getCell(fullOptions.topLeftCell),
                row = _fullOptions$workshee.row,
                col = _fullOptions$workshee.col;
            fullOptions.topLeftCell = {
                row: row,
                column: col
            }
        }
        if (!(0, _type.isDefined)(fullOptions.keepColumnWidths)) {
            fullOptions.keepColumnWidths = true
        }
        if (!(0, _type.isDefined)(fullOptions.loadPanel)) {
            fullOptions.loadPanel = {}
        }
        if (!(0, _type.isDefined)(fullOptions.loadPanel.enabled)) {
            fullOptions.loadPanel.enabled = true
        }
        return fullOptions
    },
    convertDateForExcelJS: function(date) {
        return new Date(Date.UTC(date.getFullYear(), date.getMonth(), date.getDate(), date.getHours(), date.getMinutes(), date.getSeconds(), date.getMilliseconds()))
    },
    setNumberFormat: function(excelCell, numberFormat) {
        excelCell.numFmt = numberFormat
    },
    getCellStyles: function(dataProvider) {
        var _this = this;
        var styles = dataProvider.getStyles();
        styles.forEach((function(style) {
            var numberFormat = _this.tryConvertToExcelNumberFormat(style.format, style.dataType);
            if ((0, _type.isDefined)(numberFormat)) {
                numberFormat = numberFormat.replace(/&quot;/g, '"')
            }
            style.numberFormat = numberFormat
        }));
        return styles
    },
    tryConvertToExcelNumberFormat: function(format, dataType) {
        var newFormat = _export_format.ExportFormat.formatObjectConverter(format, dataType);
        var currency = newFormat.currency;
        format = newFormat.format;
        dataType = newFormat.dataType;
        return _export_format.ExportFormat.convertFormat(format, newFormat.precision, dataType, currency)
    },
    setAlignment: function(excelCell, wrapText, horizontalAlignment) {
        excelCell.alignment = excelCell.alignment || {};
        if ((0, _type.isDefined)(wrapText)) {
            excelCell.alignment.wrapText = wrapText
        }
        if ((0, _type.isDefined)(horizontalAlignment)) {
            excelCell.alignment.horizontal = horizontalAlignment
        }
        excelCell.alignment.vertical = "top"
    },
    setColumnsWidth: function(worksheet, widths, startColumnIndex) {
        if (!(0, _type.isDefined)(widths)) {
            return
        }
        for (var i = 0; i < widths.length; i++) {
            var columnWidth = widths[i];
            if ("number" === typeof columnWidth && isFinite(columnWidth)) {
                worksheet.getColumn(startColumnIndex + i).width = Math.min(MAX_EXCEL_COLUMN_WIDTH, Math.floor(columnWidth / MAX_DIGIT_WIDTH_IN_PIXELS * 100) / 100)
            }
        }
    },
    export: function(options, helpers) {
        var _component$_getIntern, _this2 = this;
        var customizeCell = options.customizeCell,
            component = options.component,
            worksheet = options.worksheet,
            topLeftCell = options.topLeftCell,
            autoFilterEnabled = options.autoFilterEnabled,
            keepColumnWidths = options.keepColumnWidths,
            selectedRowsOnly = options.selectedRowsOnly,
            loadPanel = options.loadPanel,
            mergeRowFieldValues = options.mergeRowFieldValues,
            mergeColumnFieldValues = options.mergeColumnFieldValues;
        var internalComponent = (null === (_component$_getIntern = component._getInternalInstance) || void 0 === _component$_getIntern ? void 0 : _component$_getIntern.call(component)) || component;
        var initialLoadPanelEnabledOption = internalComponent.option("loadPanel") && internalComponent.option("loadPanel").enabled;
        if (initialLoadPanelEnabledOption) {
            component.option("loadPanel.enabled", false)
        }
        var exportLoadPanel;
        if (loadPanel.enabled && (0, _window.hasWindow)()) {
            var $targetElement = helpers._getLoadPanelTargetElement(component);
            var $container = helpers._getLoadPanelContainer(component);
            exportLoadPanel = new _export_load_panel.ExportLoadPanel(component, $targetElement, $container, loadPanel);
            exportLoadPanel.show()
        }
        var wrapText = !!component.option("wordWrapEnabled");
        worksheet.properties.outlineProperties = {
            summaryBelow: false,
            summaryRight: false
        };
        var cellRange = {
            from: {
                row: topLeftCell.row,
                column: topLeftCell.column
            },
            to: {
                row: topLeftCell.row,
                column: topLeftCell.column
            }
        };
        var dataProvider = component.getDataProvider(selectedRowsOnly);
        return new Promise((function(resolve) {
            dataProvider.ready().done((function() {
                var columns = dataProvider.getColumns();
                var dataRowsCount = dataProvider.getRowsCount();
                if (keepColumnWidths) {
                    _this2.setColumnsWidth(worksheet, dataProvider.getColumnsWidths(), cellRange.from.column)
                }
                var mergedRangesManager = new _export_merged_ranges_manager.MergedRangesManager(dataProvider, helpers, mergeRowFieldValues, mergeColumnFieldValues);
                var styles = _this2.getCellStyles(dataProvider);
                for (var rowIndex = 0; rowIndex < dataRowsCount; rowIndex++) {
                    var row = worksheet.getRow(cellRange.from.row + rowIndex);
                    helpers._trySetOutlineLevel(dataProvider, row, rowIndex);
                    _this2.exportRow(dataProvider, helpers, mergedRangesManager, rowIndex, columns.length, row, cellRange.from.column, customizeCell, wrapText, styles);
                    if (rowIndex >= 1) {
                        cellRange.to.row++
                    }
                }
                mergedRangesManager.applyMergedRages(worksheet);
                cellRange.to.column += columns.length > 0 ? columns.length - 1 : 0;
                var worksheetViewSettings = worksheet.views[0] || {};
                if (component.option("rtlEnabled")) {
                    worksheetViewSettings.rightToLeft = true
                }
                if (helpers._isFrozenZone(dataProvider)) {
                    if (-1 === Object.keys(worksheetViewSettings).indexOf("state")) {
                        (0, _extend.extend)(worksheetViewSettings, helpers._getWorksheetFrozenState(dataProvider, cellRange))
                    }
                    helpers._trySetAutoFilter(dataProvider, worksheet, cellRange, autoFilterEnabled)
                }
                if (Object.keys(worksheetViewSettings).length > 0) {
                    worksheet.views = [worksheetViewSettings]
                }
                resolve(cellRange)
            })).always((function() {
                if (initialLoadPanelEnabledOption) {
                    component.option("loadPanel.enabled", initialLoadPanelEnabledOption)
                }
                if (loadPanel.enabled && (0, _window.hasWindow)()) {
                    exportLoadPanel.dispose()
                }
            }))
        }))
    },
    exportRow: function(dataProvider, helpers, mergedRangesManager, rowIndex, cellCount, row, startColumnIndex, customizeCell, wrapText, styles) {
        for (var cellIndex = 0; cellIndex < cellCount; cellIndex++) {
            var cellData = dataProvider.getCellData(rowIndex, cellIndex, true);
            var excelCell = row.getCell(startColumnIndex + cellIndex);
            mergedRangesManager.updateMergedRanges(excelCell, rowIndex, cellIndex);
            var cellInfo = mergedRangesManager.findMergedCellInfo(rowIndex, cellIndex);
            if ((0, _type.isDefined)(cellInfo) && excelCell !== cellInfo.masterCell) {
                excelCell.style = cellInfo.masterCell.style;
                excelCell.value = cellInfo.masterCell.value
            } else {
                if ((0, _type.isDate)(cellData.value)) {
                    excelCell.value = this.convertDateForExcelJS(cellData.value)
                } else {
                    excelCell.value = cellData.value
                }
                if ((0, _type.isDefined)(excelCell.value)) {
                    var _styles$dataProvider$ = styles[dataProvider.getStyleId(rowIndex, cellIndex)],
                        bold = _styles$dataProvider$.bold,
                        horizontalAlignment = _styles$dataProvider$.alignment,
                        numberFormat = _styles$dataProvider$.numberFormat;
                    if ((0, _type.isDefined)(numberFormat)) {
                        this.setNumberFormat(excelCell, numberFormat)
                    } else if ((0, _type.isString)(excelCell.value) && /^[@=+-]/.test(excelCell.value)) {
                        this.setNumberFormat(excelCell, "@")
                    }
                    helpers._trySetFont(excelCell, bold);
                    this.setAlignment(excelCell, wrapText, horizontalAlignment)
                }
            }
            if ((0, _type.isFunction)(customizeCell)) {
                customizeCell(helpers._getCustomizeCellOptions(excelCell, cellData.cellSourceData))
            }
        }
    }
};
exports.Export = Export;
