/**
 * DevExtreme (esm/ui/pivot_grid/xmla_store/xmla_store.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import $ from "../../../core/renderer";
import {
    getWindow
} from "../../../core/utils/window";
import Class from "../../../core/class";
import {
    format as stringFormat
} from "../../../core/utils/string";
import {
    errors
} from "../../../data/errors";
import {
    noop
} from "../../../core/utils/common";
import {
    extend
} from "../../../core/utils/extend";
import {
    isFunction,
    isNumeric,
    isDefined,
    isString
} from "../../../core/utils/type";
import {
    map,
    each
} from "../../../core/utils/iterator";
import {
    inArray
} from "../../../core/utils/array";
import {
    sendRequest,
    getExpandedLevel,
    storeDrillDownMixin,
    foreachTree
} from "../ui.pivot_grid.utils";
import {
    when,
    Deferred
} from "../../../core/utils/deferred";
import {
    getLanguageId
} from "../../../localization/language_codes";
var window = getWindow();
export var XmlaStore = Class.inherit(function() {
    var discover = '<Envelope xmlns="http://schemas.xmlsoap.org/soap/envelope/"><Body><Discover xmlns="urn:schemas-microsoft-com:xml-analysis"><RequestType>{2}</RequestType><Restrictions><RestrictionList><CATALOG_NAME>{0}</CATALOG_NAME><CUBE_NAME>{1}</CUBE_NAME></RestrictionList></Restrictions><Properties><PropertyList><Catalog>{0}</Catalog>{3}</PropertyList></Properties></Discover></Body></Envelope>';
    var mdx = "SELECT {2} FROM {0} {1} CELL PROPERTIES VALUE, FORMAT_STRING, LANGUAGE, BACK_COLOR, FORE_COLOR, FONT_FLAGS";

    function execXMLA(requestOptions, data) {
        var deferred = new Deferred;
        var beforeSend = requestOptions.beforeSend;
        var ajaxSettings = {
            url: requestOptions.url,
            dataType: "text",
            data: data,
            headers: {
                "Content-Type": "text/xml"
            },
            xhrFields: {},
            method: "POST"
        };
        if (isFunction(beforeSend)) {
            beforeSend(ajaxSettings)
        }
        sendRequest(ajaxSettings).fail((function() {
            deferred.reject(arguments)
        })).done((function(text) {
            var parser = new window.DOMParser;
            var xml;
            try {
                try {
                    xml = parser.parseFromString(text, "text/xml")
                } catch (e) {
                    xml = void 0
                }
                if (!xml || xml.getElementsByTagName("parsererror").length || 0 === xml.childNodes.length) {
                    throw new errors.Error("E4023", text)
                }
            } catch (e) {
                deferred.reject({
                    statusText: e.message,
                    stack: e.stack,
                    responseText: text
                })
            }
            deferred.resolve(xml)
        }));
        return deferred
    }

    function getLocaleIdProperty() {
        var languageId = getLanguageId();
        if (void 0 !== languageId) {
            return stringFormat("<LocaleIdentifier>{0}</LocaleIdentifier>", languageId)
        }
        return ""
    }

    function getAllMember(dimension) {
        return (dimension.hierarchyName || dimension.dataField) + ".[All]"
    }

    function getAllMembers(field) {
        var result = field.dataField + ".allMembers";
        var searchValue = field.searchValue;
        if (searchValue) {
            searchValue = searchValue.replace(/'/g, "''");
            result = "Filter(" + result + ", instr(" + field.dataField + ".currentmember.member_caption,'" + searchValue + "') > 0)"
        }
        return result
    }

    function crossJoinElements(elements) {
        var elementsString = elements.join(",");
        return elements.length > 1 ? stringFormat("CrossJoin({0})", elementsString) : elementsString
    }

    function generateCrossJoin(path, expandLevel, expandAllCount, expandIndex, slicePath, options, axisName, take) {
        var crossJoinArgs = [];
        var dimensions = options[axisName];
        var fields = [];
        var arg;
        var prevDimension;
        var member;
        for (var i = expandIndex; i <= expandLevel; i++) {
            var field = dimensions[i];
            var dataField = field.dataField;
            var prevHierarchyName = dimensions[i - 1] && dimensions[i - 1].hierarchyName;
            var hierarchyName = field.hierarchyName;
            var isLastDimensionInGroup = !hierarchyName || !dimensions[i + 1] || dimensions[i + 1].hierarchyName !== hierarchyName;
            var expandAllIndex = path.length + expandAllCount + expandIndex;
            arg = null;
            fields.push(field);
            if (i < path.length) {
                if (isLastDimensionInGroup) {
                    arg = "(" + dataField + "." + preparePathValue(path[i], dataField) + ")"
                }
            } else if (i <= expandAllIndex) {
                if (0 === i && 0 === expandAllCount) {
                    var allMember = getAllMember(dimensions[expandIndex]);
                    if (!hierarchyName) {
                        arg = getAllMembers(dimensions[expandIndex])
                    } else {
                        arg = allMember + "," + dimensions[expandIndex].dataField
                    }
                } else if (hierarchyName) {
                    member = preparePathValue(slicePath[slicePath.length - 1]);
                    if (isLastDimensionInGroup || i === expandAllIndex) {
                        if (prevHierarchyName === hierarchyName) {
                            if (slicePath.length) {
                                prevDimension = dimensions[slicePath.length - 1]
                            }
                            if (!prevDimension || prevDimension.hierarchyName !== hierarchyName) {
                                prevDimension = dimensions[i - 1];
                                member = ""
                            }
                            arg = (level = prevDimension.dataField, levelMember = member, nextLevel = dataField, memberExpression = void 0, memberExpression = levelMember ? levelMember : level, "Descendants({" + memberExpression + "}, " + nextLevel + ", SELF_AND_BEFORE)")
                        } else {
                            arg = getAllMembers(field)
                        }
                    }
                } else {
                    arg = getAllMembers(field)
                }
            } else {
                var isFirstDimensionInGroup = !hierarchyName || prevHierarchyName !== hierarchyName;
                if (isFirstDimensionInGroup) {
                    arg = "(" + getAllMember(field) + ")"
                }
            }
            if (arg) {
                arg = stringFormat("{{0}}", arg);
                if (take) {
                    var sortBy = (field.hierarchyName || field.dataField) + ("displayText" === field.sortBy ? ".MEMBER_CAPTION" : ".MEMBER_VALUE");
                    arg = stringFormat("Order({0}, {1}, {2})", arg, sortBy, "desc" === field.sortOrder ? "DESC" : "ASC")
                }
                crossJoinArgs.push(arg)
            }
        }
        var level, levelMember, nextLevel, memberExpression;
        return crossJoinElements(crossJoinArgs)
    }

    function fillCrossJoins(crossJoins, path, expandLevel, expandIndex, slicePath, options, axisName, cellsString, take, totalsOnly) {
        var expandAllCount = -1;
        var dimensions = options[axisName];
        var dimensionIndex;
        do {
            expandAllCount++;
            dimensionIndex = path.length + expandAllCount + expandIndex;
            var crossJoin = generateCrossJoin(path, expandLevel, expandAllCount, expandIndex, slicePath, options, axisName, take);
            if (!take && !totalsOnly) {
                crossJoin = stringFormat("NonEmpty({0}, {1})", crossJoin, cellsString)
            }
            crossJoins.push(crossJoin)
        } while (dimensions[dimensionIndex] && dimensions[dimensionIndex + 1] && dimensions[dimensionIndex].expanded)
    }

    function declare(expression, withArray, name, type) {
        name = name || "[DX_Set_" + withArray.length + "]";
        type = type || "set";
        withArray.push(stringFormat("{0} {1} as {2}", type, name, expression));
        return name
    }

    function generateAxisMdx(options, axisName, cells, withArray, parseOptions) {
        var dimensions = options[axisName];
        var crossJoins = [];
        var path = [];
        var expandedPaths = [];
        var expandIndex = 0;
        var expandLevel = 0;
        var result = [];
        var cellsString = stringFormat("{{0}}", cells.join(","));
        if (dimensions && dimensions.length) {
            if (options.headerName === axisName) {
                path = options.path;
                expandIndex = path.length
            } else if (options.headerName && options.oppositePath) {
                path = options.oppositePath;
                expandIndex = path.length
            } else {
                expandedPaths = ("columns" === axisName ? options.columnExpandedPaths : options.rowExpandedPaths) || expandedPaths
            }
            expandLevel = getExpandedLevel(options, axisName);
            fillCrossJoins(crossJoins, [], expandLevel, expandIndex, path, options, axisName, cellsString, "rows" === axisName ? options.rowTake : options.columnTake, options.totalsOnly);
            each(expandedPaths, (function(_, expandedPath) {
                fillCrossJoins(crossJoins, expandedPath, expandLevel, expandIndex, expandedPath, options, axisName, cellsString)
            }));
            for (var i = expandLevel; i >= path.length; i--) {
                if (dimensions[i].hierarchyName) {
                    parseOptions.visibleLevels[dimensions[i].hierarchyName] = parseOptions.visibleLevels[dimensions[i].hierarchyName] || [];
                    parseOptions.visibleLevels[dimensions[i].hierarchyName].push(dimensions[i].dataField)
                }
            }
        }
        if (crossJoins.length) {
            var expression = (elements = crossJoins, elementsString = elements.join(","), elements.length > 1 ? "Union(" + elementsString + ")" : elementsString);
            if ("rows" === axisName && options.rowTake) {
                expression = stringFormat("Subset({0}, {1}, {2})", expression, options.rowSkip > 0 ? options.rowSkip + 1 : 0, options.rowSkip > 0 ? options.rowTake : options.rowTake + 1)
            }
            if ("columns" === axisName && options.columnTake) {
                expression = stringFormat("Subset({0}, {1}, {2})", expression, options.columnSkip > 0 ? options.columnSkip + 1 : 0, options.columnSkip > 0 ? options.columnTake : options.columnTake + 1)
            }
            var axisSet = "[DX_".concat(axisName, "]");
            result.push(declare(expression, withArray, axisSet));
            if (options.totalsOnly) {
                result.push(declare("COUNT(".concat(axisSet, ")"), withArray, "[DX_".concat(axisName, "_count]"), "member"))
            }
        }
        var elements, elementsString;
        if ("columns" === axisName && cells.length && !options.skipValues) {
            result.push(cellsString)
        }
        return stringFormat("{0} DIMENSION PROPERTIES PARENT_UNIQUE_NAME,HIERARCHY_UNIQUE_NAME, MEMBER_VALUE ON {1}", crossJoinElements(result), axisName)
    }

    function generateAxisFieldsFilter(fields) {
        var filterMembers = [];
        each(fields, (function(_, field) {
            var dataField = field.dataField;
            var filterExpression = [];
            var filterValues = field.filterValues || [];
            var filterStringExpression;
            if (field.hierarchyName && isNumeric(field.groupIndex)) {
                return
            }
            each(filterValues, (function(_, filterValue) {
                var filterMdx = dataField + "." + preparePathValue(Array.isArray(filterValue) ? filterValue[filterValue.length - 1] : filterValue, dataField);
                if ("exclude" === field.filterType) {
                    filterExpression.push(filterMdx + ".parent");
                    filterMdx = "Descendants(" + filterMdx + ")"
                }
                filterExpression.push(filterMdx)
            }));
            if (filterValues.length) {
                filterStringExpression = stringFormat("{{0}}", filterExpression.join(","));
                if ("exclude" === field.filterType) {
                    filterStringExpression = "Except(" + getAllMembers(field) + "," + filterStringExpression + ")"
                }
                filterMembers.push(filterStringExpression)
            }
        }));
        return filterMembers.length ? crossJoinElements(filterMembers) : ""
    }

    function generateFrom(columnsFilter, rowsFilter, filter, cubeName) {
        var from = "[" + cubeName + "]";
        each([columnsFilter, rowsFilter, filter], (function(_, filter) {
            if (filter) {
                from = stringFormat("(SELECT {0} FROM {1})", filter + "on 0", from)
            }
        }));
        return from
    }

    function generateMdxCore(axisStrings, withArray, columns, rows, filters, slice, cubeName) {
        var options = arguments.length > 7 && void 0 !== arguments[7] ? arguments[7] : {};
        var mdxString = "";
        var withString = (withArray.length ? "with " + withArray.join(" ") : "") + " ";
        if (axisStrings.length) {
            var select;
            if (options.totalsOnly) {
                var countMembers = [];
                if (rows.length) {
                    countMembers.push("[DX_rows_count]")
                }
                if (columns.length) {
                    countMembers.push("[DX_columns_count]")
                }
                select = "{".concat(countMembers.join(","), "} on columns")
            } else {
                select = axisStrings.join(",")
            }
            mdxString = withString + stringFormat(mdx, generateFrom(generateAxisFieldsFilter(columns), generateAxisFieldsFilter(rows), generateAxisFieldsFilter(filters || []), cubeName), slice.length ? stringFormat("WHERE ({0})", slice.join(",")) : "", select)
        }
        return mdxString
    }

    function prepareDataFields(withArray, valueFields) {
        return map(valueFields, (function(cell) {
            if (isString(cell.expression)) {
                declare(cell.expression, withArray, cell.dataField, "member")
            }
            return cell.dataField
        }))
    }

    function addSlices(slices, options, headerName, path) {
        each(path, (function(index, value) {
            var dimension = options[headerName][index];
            if (!dimension.hierarchyName || dimension.hierarchyName !== options[headerName][index + 1].hierarchyName) {
                slices.push(dimension.dataField + "." + preparePathValue(value, dimension.dataField))
            }
        }))
    }

    function generateMDX(options, cubeName, parseOptions) {
        var columns = options.columns || [];
        var rows = options.rows || [];
        var values = options.values && options.values.length ? options.values : [{
            dataField: "[Measures]"
        }];
        var slice = [];
        var withArray = [];
        var axisStrings = [];
        var dataFields = prepareDataFields(withArray, values);
        parseOptions.measureCount = options.skipValues ? 1 : values.length;
        parseOptions.visibleLevels = {};
        if (options.headerName && options.path) {
            addSlices(slice, options, options.headerName, options.path)
        }
        if (options.headerName && options.oppositePath) {
            addSlices(slice, options, "rows" === options.headerName ? "columns" : "rows", options.oppositePath)
        }
        if (columns.length || dataFields.length) {
            axisStrings.push(generateAxisMdx(options, "columns", dataFields, withArray, parseOptions))
        }
        if (rows.length) {
            axisStrings.push(generateAxisMdx(options, "rows", dataFields, withArray, parseOptions))
        }
        return generateMdxCore(axisStrings, withArray, columns, rows, options.filters, slice, cubeName, options)
    }

    function createDrillDownAxisSlice(slice, fields, path) {
        each(path, (function(index, value) {
            var field = fields[index];
            if (field.hierarchyName && (fields[index + 1] || {}).hierarchyName === field.hierarchyName) {
                return
            }
            slice.push(field.dataField + "." + preparePathValue(value, field.dataField))
        }))
    }

    function getNumber(str) {
        return parseInt(str, 10)
    }

    function getFirstChildText(node, childTagName) {
        return getNodeText(function(node, tagName) {
            return (node.getElementsByTagName(tagName) || [])[0]
        }(node, childTagName))
    }

    function getNodeText(node) {
        return node && (node.textContent || node.text || node.innerHTML) || ""
    }

    function parseCells(xml, axes, measureCount) {
        var cells = [];
        var cell = [];
        var index = 0;
        var cellsOriginal = [];
        var cellElements = xml.getElementsByTagName("Cell");
        var errorDictionary = {};
        for (var i = 0; i < cellElements.length; i++) {
            var xmlCell = cellElements[i];
            var valueElement = xmlCell.getElementsByTagName("Value")[0];
            var errorElements = valueElement && valueElement.getElementsByTagName("Error") || [];
            var text = 0 === errorElements.length ? getNodeText(valueElement) : "#N/A";
            var value = parseFloat(text);
            var _isNumeric = text - value + 1 > 0;
            var cellOrdinal = getNumber(xmlCell.getAttribute("CellOrdinal"));
            if (errorElements.length) {
                errorDictionary[getNodeText(errorElements[0].getElementsByTagName("ErrorCode")[0])] = getNodeText(errorElements[0].getElementsByTagName("Description")[0])
            }
            cellsOriginal[cellOrdinal] = {
                value: _isNumeric ? value : text || null
            }
        }
        each(axes[1], (function() {
            var row = [];
            cells.push(row);
            each(axes[0], (function() {
                var measureIndex = index % measureCount;
                if (0 === measureIndex) {
                    cell = [];
                    row.push(cell)
                }
                cell.push(cellsOriginal[index] ? cellsOriginal[index].value : null);
                index++
            }))
        }));
        Object.keys(errorDictionary).forEach((function(key) {
            errors.log("W4002", errorDictionary[key])
        }));
        return cells
    }

    function preparePathValue(pathValue, dataField) {
        if (pathValue) {
            pathValue = isString(pathValue) && -1 !== pathValue.indexOf("&") ? pathValue : "[" + pathValue + "]";
            if (dataField && 0 === pathValue.indexOf(dataField + ".")) {
                pathValue = pathValue.slice(dataField.length + 1, pathValue.length)
            }
        }
        return pathValue
    }

    function getItem(hash, name, member, index) {
        var item = hash[name];
        if (!item) {
            item = {};
            hash[name] = item
        }
        if (!isDefined(item.value) && member) {
            item.text = member.caption;
            item.value = member.value;
            item.key = name ? name : "";
            item.levelName = member.levelName;
            item.hierarchyName = member.hierarchyName;
            item.parentName = member.parentName;
            item.index = index;
            item.level = member.level
        }
        return item
    }

    function getVisibleChildren(item, visibleLevels) {
        var result = [];
        var children = item.children && (item.children.length ? item.children : Object.keys(item.children.grandTotalHash || {}).reduce((result, name) => result.concat(item.children.grandTotalHash[name].children), []));
        var firstChild = children && children[0];
        if (firstChild && (visibleLevels[firstChild.hierarchyName] && -1 !== inArray(firstChild.levelName, visibleLevels[firstChild.hierarchyName]) || !visibleLevels[firstChild.hierarchyName] || 0 === firstChild.level)) {
            var newChildren = children.filter(child => child.hierarchyName === firstChild.hierarchyName);
            newChildren.grandTotalHash = children.grandTotalHash;
            return newChildren
        } else if (firstChild) {
            for (var i = 0; i < children.length; i++) {
                if (children[i].hierarchyName === firstChild.hierarchyName) {
                    result.push.apply(result, getVisibleChildren(children[i], visibleLevels))
                }
            }
        }
        return result
    }

    function fillDataSourceAxes(dataSourceAxis, axisTuples, measureCount, visibleLevels) {
        var result = [];
        each(axisTuples, (function(tupleIndex, members) {
            var parentItem = {
                children: result
            };
            var dataIndex = isDefined(measureCount) ? Math.floor(tupleIndex / measureCount) : tupleIndex;
            each(members, (function(_, member) {
                parentItem = function(dataIndex, member, parentItem) {
                    var children = parentItem.children = parentItem.children || [];
                    var hash = children.hash = children.hash || {};
                    var grandTotalHash = children.grandTotalHash = children.grandTotalHash || {};
                    if (member.parentName) {
                        parentItem = getItem(hash, member.parentName);
                        children = parentItem.children = parentItem.children || []
                    }
                    var currentItem = getItem(hash, member.name, member, dataIndex);
                    if (member.hasValue && !currentItem.added) {
                        currentItem.index = dataIndex;
                        currentItem.added = true;
                        children.push(currentItem)
                    }
                    if ((!parentItem.value || !parentItem.parentName) && member.parentName) {
                        grandTotalHash[member.parentName] = parentItem
                    } else if (grandTotalHash[parentItem.name]) {
                        delete grandTotalHash[member.parentName]
                    }
                    return currentItem
                }(dataIndex, member, parentItem)
            }))
        }));
        var parentItem = {
            children: result
        };
        parentItem.children = getVisibleChildren(parentItem, visibleLevels);
        var grandTotalIndex = function(parentItem, visibleLevels) {
            var grandTotalIndex;
            if (1 === parentItem.children.length && "" === parentItem.children[0].parentName) {
                grandTotalIndex = parentItem.children[0].index;
                var grandTotalHash = parentItem.children.grandTotalHash;
                parentItem.children = parentItem.children[0].children || [];
                parentItem.children.grandTotalHash = grandTotalHash;
                parentItem.children = getVisibleChildren(parentItem, visibleLevels)
            } else if (0 === parentItem.children.length) {
                grandTotalIndex = 0
            }
            return grandTotalIndex
        }(parentItem, visibleLevels);
        foreachTree(parentItem.children, (function(items) {
            var item = items[0];
            var children = getVisibleChildren(item, visibleLevels);
            if (children.length) {
                item.children = children
            } else {
                delete item.children
            }
            delete item.levelName;
            delete item.hierarchyName;
            delete item.added;
            delete item.parentName;
            delete item.level
        }), true);
        each(parentItem.children || [], (function(_, e) {
            dataSourceAxis.push(e)
        }));
        return grandTotalIndex
    }

    function checkError(xml) {
        var faultElementNS = xml.getElementsByTagName("soap:Fault");
        var faultElement = xml.getElementsByTagName("Fault");
        var errorElement = $([].slice.call(faultElement.length ? faultElement : faultElementNS)).find("Error");
        if (errorElement.length) {
            var description = errorElement.attr("Description");
            var error = new errors.Error("E4000", description);
            errors.log("E4000", description);
            return error
        }
        return null
    }

    function parseResult(xml, parseOptions) {
        var dataSource = {
            columns: [],
            rows: []
        };
        var measureCount = parseOptions.measureCount;
        var axes = function(xml, skipValues) {
            var axes = [];
            each(xml.getElementsByTagName("Axis"), (function(_, axisElement) {
                var name = axisElement.getAttribute("name");
                var axis = [];
                var index = 0;
                if (0 === name.indexOf("Axis") && isNumeric(getNumber(name.substr(4)))) {
                    axes.push(axis);
                    each(axisElement.getElementsByTagName("Tuple"), (function(_, tupleElement) {
                        var tupleMembers = tupleElement.childNodes;
                        var levelSum = 0;
                        var members = [];
                        var membersCount = skipValues ? tupleMembers.length : tupleMembers.length - 1;
                        var isAxisWithMeasure = 1 === axes.length;
                        if (isAxisWithMeasure) {
                            membersCount--
                        }
                        axis.push(members);
                        for (var i = membersCount; i >= 0; i--) {
                            var tuple = tupleMembers[i];
                            var level = getNumber(getFirstChildText(tuple, "LNum"));
                            members[i] = {
                                caption: getFirstChildText(tuple, "Caption"),
                                value: (valueText = getFirstChildText(tuple, "MEMBER_VALUE"), isNumeric(valueText) ? parseFloat(valueText) : valueText),
                                level: level,
                                index: index++,
                                hasValue: !levelSum && (!!level || 0 === i),
                                name: getFirstChildText(tuple, "UName"),
                                hierarchyName: tupleMembers[i].getAttribute("Hierarchy"),
                                parentName: getFirstChildText(tuple, "PARENT_UNIQUE_NAME"),
                                levelName: getFirstChildText(tuple, "LName")
                            };
                            levelSum += level
                        }
                        var valueText
                    }))
                }
            }));
            while (axes.length < 2) {
                axes.push([
                    [{
                        level: 0
                    }]
                ])
            }
            return axes
        }(xml, parseOptions.skipValues);
        dataSource.grandTotalColumnIndex = fillDataSourceAxes(dataSource.columns, axes[0], measureCount, parseOptions.visibleLevels);
        dataSource.grandTotalRowIndex = fillDataSourceAxes(dataSource.rows, axes[1], void 0, parseOptions.visibleLevels);
        dataSource.values = parseCells(xml, axes, measureCount);
        return dataSource
    }

    function parseDiscoverRowSet(xml, schema, dimensions, translatedDisplayFolders) {
        var result = [];
        var isMeasure = "MEASURE" === schema;
        var displayFolderField = isMeasure ? "MEASUREGROUP_NAME" : schema + "_DISPLAY_FOLDER";
        each(xml.getElementsByTagName("row"), (function(_, row) {
            var hierarchyName = "LEVEL" === schema ? getFirstChildText(row, "HIERARCHY_UNIQUE_NAME") : void 0;
            var levelNumber = getFirstChildText(row, "LEVEL_NUMBER");
            var displayFolder = getFirstChildText(row, displayFolderField);
            if (isMeasure) {
                displayFolder = translatedDisplayFolders[displayFolder] || displayFolder
            }
            if (("0" !== levelNumber || "true" !== getFirstChildText(row, schema + "_IS_VISIBLE")) && "2" !== getFirstChildText(row, "DIMENSION_TYPE")) {
                var dimension = isMeasure ? "DX_MEASURES" : getFirstChildText(row, "DIMENSION_UNIQUE_NAME");
                var dataField = getFirstChildText(row, schema + "_UNIQUE_NAME");
                result.push({
                    dimension: dimensions.names[dimension] || dimension,
                    groupIndex: levelNumber ? getNumber(levelNumber) - 1 : void 0,
                    dataField: dataField,
                    caption: getFirstChildText(row, schema + "_CAPTION"),
                    hierarchyName: hierarchyName,
                    groupName: hierarchyName,
                    displayFolder: displayFolder,
                    isMeasure: isMeasure,
                    isDefault: !!dimensions.defaultHierarchies[dataField]
                })
            }
        }));
        return result
    }

    function parseStringWithUnicodeSymbols(str) {
        str = str.replace(/_x(....)_/g, (function(whole, group1) {
            return String.fromCharCode(parseInt(group1, 16))
        }));
        var stringArray = str.match(/\[.+?\]/gi);
        if (stringArray && stringArray.length) {
            str = stringArray[stringArray.length - 1]
        }
        return str.replace(/\[/gi, "").replace(/\]/gi, "").replace(/\$/gi, "").replace(/\./gi, " ")
    }

    function sendQuery(storeOptions, mdxString) {
        mdxString = $("<div>").text(mdxString).html();
        return execXMLA(storeOptions, stringFormat('<Envelope xmlns="http://schemas.xmlsoap.org/soap/envelope/"><Body><Execute xmlns="urn:schemas-microsoft-com:xml-analysis"><Command><Statement>{0}</Statement></Command><Properties><PropertyList><Catalog>{1}</Catalog><ShowHiddenCubes>True</ShowHiddenCubes><SspropInitAppName>Microsoft SQL Server Management Studio</SspropInitAppName><Timeout>3600</Timeout>{2}</PropertyList></Properties></Execute></Body></Envelope>', mdxString, storeOptions.catalog, getLocaleIdProperty()))
    }
    return {
        ctor: function(options) {
            this._options = options
        },
        getFields: function() {
            var options = this._options;
            var catalog = options.catalog;
            var cube = options.cube;
            var localeIdProperty = getLocaleIdProperty();
            var dimensionsRequest = execXMLA(options, stringFormat(discover, catalog, cube, "MDSCHEMA_DIMENSIONS", localeIdProperty));
            var measuresRequest = execXMLA(options, stringFormat(discover, catalog, cube, "MDSCHEMA_MEASURES", localeIdProperty));
            var hierarchiesRequest = execXMLA(options, stringFormat(discover, catalog, cube, "MDSCHEMA_HIERARCHIES", localeIdProperty));
            var levelsRequest = execXMLA(options, stringFormat(discover, catalog, cube, "MDSCHEMA_LEVELS", localeIdProperty));
            var result = new Deferred;
            when(dimensionsRequest, measuresRequest, hierarchiesRequest, levelsRequest).then((function(dimensionsResponse, measuresResponse, hierarchiesResponse, levelsResponse) {
                execXMLA(options, stringFormat(discover, catalog, cube, "MDSCHEMA_MEASUREGROUPS", localeIdProperty)).done((function(measureGroupsResponse) {
                    var dimensions = function(xml) {
                        var result = {
                            names: {},
                            defaultHierarchies: {}
                        };
                        each($(xml).find("row"), (function() {
                            var $row = $(this);
                            var type = $row.children("DIMENSION_TYPE").text();
                            var dimensionName = "2" === type ? "DX_MEASURES" : $row.children("DIMENSION_UNIQUE_NAME").text();
                            result.names[dimensionName] = $row.children("DIMENSION_CAPTION").text();
                            result.defaultHierarchies[$row.children("DEFAULT_HIERARCHY").text()] = true
                        }));
                        return result
                    }(dimensionsResponse);
                    var hierarchies = parseDiscoverRowSet(hierarchiesResponse, "HIERARCHY", dimensions);
                    var levels = parseDiscoverRowSet(levelsResponse, "LEVEL", dimensions);
                    var measureGroups = function(xml) {
                        var measureGroups = {};
                        each(xml.getElementsByTagName("row"), (function(_, row) {
                            measureGroups[getFirstChildText(row, "MEASUREGROUP_NAME")] = getFirstChildText(row, "MEASUREGROUP_CAPTION")
                        }));
                        return measureGroups
                    }(measureGroupsResponse);
                    var fields = parseDiscoverRowSet(measuresResponse, "MEASURE", dimensions, measureGroups).concat(hierarchies);
                    var levelsByHierarchy = {};
                    each(levels, (function(_, level) {
                        levelsByHierarchy[level.hierarchyName] = levelsByHierarchy[level.hierarchyName] || [];
                        levelsByHierarchy[level.hierarchyName].push(level)
                    }));
                    each(hierarchies, (function(_, hierarchy) {
                        if (levelsByHierarchy[hierarchy.dataField] && levelsByHierarchy[hierarchy.dataField].length > 1) {
                            hierarchy.groupName = hierarchy.hierarchyName = hierarchy.dataField;
                            fields.push.apply(fields, levelsByHierarchy[hierarchy.hierarchyName])
                        }
                    }));
                    result.resolve(fields)
                })).fail(result.reject)
            })).fail(result.reject);
            return result
        },
        load: function(options) {
            var result = new Deferred;
            var storeOptions = this._options;
            var parseOptions = {
                skipValues: options.skipValues
            };
            var mdxString = generateMDX(options, storeOptions.cube, parseOptions);
            var rowCountMdx;
            if (options.rowSkip || options.rowTake || options.columnTake || options.columnSkip) {
                rowCountMdx = generateMDX(extend({}, options, {
                    totalsOnly: true,
                    rowSkip: null,
                    rowTake: null,
                    columnSkip: null,
                    columnTake: null
                }), storeOptions.cube, {})
            }
            var load = () => {
                if (mdxString) {
                    when(sendQuery(storeOptions, mdxString), rowCountMdx && sendQuery(storeOptions, rowCountMdx)).done((function(executeXml, rowCountXml) {
                        var error = checkError(executeXml) || rowCountXml && checkError(rowCountXml);
                        if (!error) {
                            var response = parseResult(executeXml, parseOptions);
                            if (rowCountXml) {
                                ! function(data, options, totalCountXml) {
                                    var axes = [];
                                    var columnOptions = options.columns || [];
                                    var rowOptions = options.rows || [];
                                    if (columnOptions.length) {
                                        axes.push({})
                                    }
                                    if (rowOptions.length) {
                                        axes.push({})
                                    }
                                    var cells = parseCells(totalCountXml, [
                                        [{}],
                                        [{}, {}]
                                    ], 1);
                                    if (!columnOptions.length && rowOptions.length) {
                                        data.rowCount = Math.max(cells[0][0][0] - 1, 0)
                                    }
                                    if (!rowOptions.length && columnOptions.length) {
                                        data.columnCount = Math.max(cells[0][0][0] - 1, 0)
                                    }
                                    if (rowOptions.length && columnOptions.length) {
                                        data.rowCount = Math.max(cells[0][0][0] - 1, 0);
                                        data.columnCount = Math.max(cells[1][0][0] - 1, 0)
                                    }
                                    if (void 0 !== data.rowCount && options.rowTake) {
                                        data.rows = [...Array(options.rowSkip)].concat(data.rows);
                                        data.rows.length = data.rowCount;
                                        for (var i = 0; i < data.rows.length; i++) {
                                            data.rows[i] = data.rows[i] || {}
                                        }
                                    }
                                    if (void 0 !== data.columnCount && options.columnTake) {
                                        data.columns = [...Array(options.columnSkip)].concat(data.columns);
                                        data.columns.length = data.columnCount;
                                        for (var _i = 0; _i < data.columns.length; _i++) {
                                            data.columns[_i] = data.columns[_i] || {}
                                        }
                                    }
                                }(response, options, rowCountXml)
                            }
                            result.resolve(response)
                        } else {
                            result.reject(error)
                        }
                    })).fail(result.reject)
                } else {
                    result.resolve({
                        columns: [],
                        rows: [],
                        values: [],
                        grandTotalColumnIndex: 0,
                        grandTotalRowIndex: 0
                    })
                }
            };
            if (options.delay) {
                setTimeout(load, options.delay)
            } else {
                load()
            }
            return result
        },
        supportPaging: function() {
            return true
        },
        getDrillDownItems: function(options, params) {
            var result = new Deferred;
            var storeOptions = this._options;
            var mdxString = function(options, cubeName, params) {
                var columns = options.columns || [];
                var rows = options.rows || [];
                var values = options.values && options.values.length ? options.values : [{
                    dataField: "[Measures]"
                }];
                var slice = [];
                var withArray = [];
                var axisStrings = [];
                var dataFields = prepareDataFields(withArray, values);
                var maxRowCount = params.maxRowCount;
                var customColumns = params.customColumns || [];
                var customColumnsString = customColumns.length > 0 ? " return " + customColumns.join(",") : "";
                createDrillDownAxisSlice(slice, columns, params.columnPath || []);
                createDrillDownAxisSlice(slice, rows, params.rowPath || []);
                if (columns.length || dataFields.length) {
                    axisStrings.push([(dataFields[params.dataIndex] || dataFields[0]) + " on 0"])
                }
                var coreMDX = generateMdxCore(axisStrings, withArray, columns, rows, options.filters, slice, cubeName);
                return coreMDX ? "drillthrough" + (maxRowCount > 0 ? " maxrows " + maxRowCount : "") + coreMDX + customColumnsString : coreMDX
            }(options, storeOptions.cube, params);
            if (mdxString) {
                when(sendQuery(storeOptions, mdxString)).done((function(executeXml) {
                    var error = checkError(executeXml);
                    if (!error) {
                        result.resolve(function(xml) {
                            var rows = xml.getElementsByTagName("row");
                            var result = [];
                            var columnNames = {};
                            for (var i = 0; i < rows.length; i++) {
                                var children = rows[i].childNodes;
                                var item = {};
                                for (var j = 0; j < children.length; j++) {
                                    var tagName = children[j].tagName;
                                    var name = columnNames[tagName] = columnNames[tagName] || parseStringWithUnicodeSymbols(tagName);
                                    item[name] = getNodeText(children[j])
                                }
                                result.push(item)
                            }
                            return result
                        }(executeXml))
                    } else {
                        result.reject(error)
                    }
                })).fail(result.reject)
            } else {
                result.resolve([])
            }
            return result
        },
        key: noop,
        filter: noop
    }
}()).include(storeDrillDownMixin);
