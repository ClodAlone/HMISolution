/**
 * DevExtreme (esm/ui/scheduler/timezones/utils.timezones_data.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import query from "../../../data/query";
import errors from "../../../core/errors";
import tzData from "./timezones_data";
import {
    sign
} from "../../../core/utils/math";
var getConvertedUntils = value => value.split("|").map(until => {
    if ("Infinity" === until) {
        return null
    }
    return 1e3 * parseInt(until, 36)
});
var parseTimezone = timeZoneConfig => {
    var offsets = timeZoneConfig.offsets;
    var offsetIndices = timeZoneConfig.offsetIndices;
    var untils = timeZoneConfig.untils;
    var offsetList = offsets.split("|").map(value => parseInt(value));
    var offsetIndexList = offsetIndices.split("").map(value => parseInt(value));
    var dateList = getConvertedUntils(untils).map((accumulator = 0, value => accumulator += value));
    var accumulator;
    return {
        offsetList: offsetList,
        offsetIndexList: offsetIndexList,
        dateList: dateList
    }
};
class TimeZoneCache {
    constructor() {
        this.map = new Map
    }
    tryGet(id) {
        if (!this.map.get(id)) {
            var config = timeZoneDataUtils.getTimezoneById(id);
            if (!config) {
                return false
            }
            var timeZoneInfo = parseTimezone(config);
            this.map.set(id, timeZoneInfo)
        }
        return this.map.get(id)
    }
}
var tzCache = new TimeZoneCache;
var timeZoneDataUtils = {
    _tzCache: tzCache,
    _timeZones: tzData.zones,
    getDisplayedTimeZones: function(timestamp) {
        var timeZones = this._timeZones.map(timezone => {
            var timeZoneInfo = parseTimezone(timezone);
            var offset = this.getUtcOffset(timeZoneInfo, timestamp);
            var title = "(GMT ".concat(this.formatOffset(offset), ") ").concat(this.formatId(timezone.id));
            return {
                offset: offset,
                title: title,
                id: timezone.id
            }
        });
        return query(timeZones).sortBy("offset").toArray()
    },
    formatOffset: function(offset) {
        var hours = Math.floor(offset);
        var minutesInDecimal = offset - hours;
        var signString = sign(offset) >= 0 ? "+" : "-";
        var hoursString = "0".concat(Math.abs(hours)).slice(-2);
        var minutesString = minutesInDecimal > 0 ? ":".concat(60 * minutesInDecimal) : ":00";
        return signString + hoursString + minutesString
    },
    formatId: function(id) {
        return id.split("/").join(" - ").split("_").join(" ")
    },
    getTimezoneById: function(id) {
        if (!id) {
            return
        }
        var tzList = this._timeZones;
        for (var i = 0; i < tzList.length; i++) {
            var currentId = tzList[i].id;
            if (currentId === id) {
                return tzList[i]
            }
        }
        errors.log("W0009", id);
        return
    },
    getTimeZoneOffsetById: function(id, timestamp) {
        var timeZoneInfo = tzCache.tryGet(id);
        return timeZoneInfo ? this.getUtcOffset(timeZoneInfo, timestamp) : void 0
    },
    getTimeZoneDeclarationTuple: function(id, year) {
        var timeZoneInfo = tzCache.tryGet(id);
        return timeZoneInfo ? this.getTimeZoneDeclarationTupleCore(timeZoneInfo, year) : []
    },
    getTimeZoneDeclarationTupleCore: function(timeZoneInfo, year) {
        var offsetList = timeZoneInfo.offsetList;
        var offsetIndexList = timeZoneInfo.offsetIndexList;
        var dateList = timeZoneInfo.dateList;
        var tupleResult = [];
        for (var i = 0; i < dateList.length; i++) {
            var currentDate = dateList[i];
            var currentYear = new Date(currentDate).getFullYear();
            if (currentYear === year) {
                var offset = offsetList[offsetIndexList[i + 1]];
                tupleResult.push({
                    date: currentDate,
                    offset: -offset / 60
                })
            }
            if (currentYear > year) {
                break
            }
        }
        return tupleResult
    },
    getUtcOffset: function(timeZoneInfo, dateTimeStamp) {
        var offsetList = timeZoneInfo.offsetList;
        var offsetIndexList = timeZoneInfo.offsetIndexList;
        var dateList = timeZoneInfo.dateList;
        var lastIntervalStartIndex = dateList.length - 1 - 1;
        var index = lastIntervalStartIndex;
        while (index >= 0 && dateTimeStamp < dateList[index]) {
            index--
        }
        var offset = offsetList[offsetIndexList[index + 1]];
        return -offset / 60 || offset
    }
};
export default timeZoneDataUtils;
