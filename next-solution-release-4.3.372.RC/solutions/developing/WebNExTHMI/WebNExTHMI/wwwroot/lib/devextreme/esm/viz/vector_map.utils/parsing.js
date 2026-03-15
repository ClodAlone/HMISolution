/**
 * DevExtreme (esm/viz/vector_map.utils/parsing.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
function noop() {}

function eigen(x) {
    return x
}

function isFunction(target) {
    return "function" === typeof target
}

function wrapSource(source) {
    var buffer = wrapBuffer(source);
    var position = 0;
    var stream = {
        pos: function() {
            return position
        },
        skip: function(count) {
            position += count;
            return stream
        },
        ui8arr: function(length) {
            var i = 0;
            var list = [];
            list.length = length;
            for (; i < length; ++i) {
                list[i] = stream.ui8()
            }
            return list
        },
        ui8: function(_ui) {
            function ui8() {
                return _ui.apply(this, arguments)
            }
            ui8.toString = function() {
                return _ui.toString()
            };
            return ui8
        }((function() {
            var val = ui8(buffer, position);
            position += 1;
            return val
        })),
        ui16LE: function(_ui16LE) {
            function ui16LE() {
                return _ui16LE.apply(this, arguments)
            }
            ui16LE.toString = function() {
                return _ui16LE.toString()
            };
            return ui16LE
        }((function() {
            var val = ui16LE(buffer, position);
            position += 2;
            return val
        })),
        ui32LE: function(_ui32LE) {
            function ui32LE() {
                return _ui32LE.apply(this, arguments)
            }
            ui32LE.toString = function() {
                return _ui32LE.toString()
            };
            return ui32LE
        }((function() {
            var val = ui32LE(buffer, position);
            position += 4;
            return val
        })),
        ui32BE: function(_ui32BE) {
            function ui32BE() {
                return _ui32BE.apply(this, arguments)
            }
            ui32BE.toString = function() {
                return _ui32BE.toString()
            };
            return ui32BE
        }((function() {
            var val = ui32BE(buffer, position);
            position += 4;
            return val
        })),
        f64LE: function(_f64LE) {
            function f64LE() {
                return _f64LE.apply(this, arguments)
            }
            f64LE.toString = function() {
                return _f64LE.toString()
            };
            return f64LE
        }((function() {
            var val = f64LE(buffer, position);
            position += 8;
            return val
        }))
    };
    return stream
}

function parseCore(source, roundCoordinates, errors) {
    var shapeData = source[0] ? parseShape(wrapSource(source[0]), errors) : {};
    var dataBaseFileData = source[1] ? parseDBF(wrapSource(source[1]), errors) : {};
    var features = buildFeatures(shapeData.shapes || [], dataBaseFileData.records || [], roundCoordinates);
    var result;
    if (features.length) {
        result = {
            type: "FeatureCollection",
            features: features
        };
        result.bbox = shapeData.bBox
    } else {
        result = null
    }
    return result
}

function buildFeatures(shapeData, dataBaseFileData, roundCoordinates) {
    var features = [];
    var i;
    var ii = features.length = Math.max(shapeData.length, dataBaseFileData.length);
    var shape;
    for (i = 0; i < ii; ++i) {
        shape = shapeData[i] || {};
        features[i] = {
            type: "Feature",
            geometry: {
                type: shape.geoJSON_type || null,
                coordinates: shape.coordinates ? roundCoordinates(shape.coordinates) : []
            },
            properties: dataBaseFileData[i] || null
        }
    }
    return features
}

function createCoordinatesRounder(precision) {
    var factor = Number("1E" + precision);

    function round(x) {
        return Math.round(x * factor) / factor
    }
    return function process(values) {
        return values.map(values[0].length ? process : round)
    }
}

function buildParseArgs(source) {
    source = source || {};
    return ["shp", "dbf"].map((function(key) {
        return function(done) {
            if (source.substr) {
                key = "." + key;
                sendRequest(source + (source.substr(-key.length).toLowerCase() === key ? "" : key), (function(e, response) {
                    done(e, response)
                }))
            } else {
                done(null, source[key] || null)
            }
        }
    }))
}

function parse(source, parameters, callback) {
    var result;
    when(buildParseArgs(source), (function(errorArray, dataArray) {
        callback = isFunction(parameters) && parameters || isFunction(callback) && callback || noop;
        parameters = !isFunction(parameters) && parameters || {};
        var errors = [];
        errorArray.forEach((function(e) {
            e && errors.push(e)
        }));
        result = parseCore(dataArray, parameters.precision >= 0 ? createCoordinatesRounder(parameters.precision) : eigen, errors);
        callback(result, errors.length ? errors : null)
    }));
    return result
}
exports.parse = parse;

function when(actions, callback) {
    var errorArray = [];
    var dataArray = [];
    var counter = 1;
    actions.forEach((function(action, i) {
        ++counter;
        action((function(e, data) {
            errorArray[i] = e;
            dataArray[i] = data;
            massDone()
        }))
    }));
    false;
    massDone();

    function massDone() {
        --counter;
        if (0 === counter && true) {
            callback(errorArray, dataArray)
        }
    }
}
