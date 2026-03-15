/**
 * DevExtreme (esm/ui/number_box/number_box.caret.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import {
    fitIntoRange
} from "../../core/utils/math";
import {
    escapeRegExp
} from "../../core/utils/common";
import number from "../../localization/number";
import {
    getRealSeparatorIndex,
    getNthOccurrence,
    splitByIndex
} from "./utils";
export var getCaretBoundaries = function(text, format) {
    if ("string" === typeof format) {
        var signParts = format.split(";");
        var sign = number.getSign(text, format);
        signParts[1] = signParts[1] || "-" + signParts[0];
        format = signParts[sign < 0 ? 1 : 0];
        format = (str = format, str.replace(/'([^']*)'/g, str => str.split("").map(() => " ").join("").substr(2)));
        var prefixStubLength = /^[^#0.,]*/.exec(format)[0].length;
        var postfixStubLength = /[^#0.,]*$/.exec(format)[0].length;
        return {
            start: prefixStubLength,
            end: text.length - postfixStubLength
        }
    } else {
        return {
            start: 0,
            end: text.length
        }
    }
    var str
};
var _getDigitCountBeforeIndex = function(index, text) {
    var decimalSeparator = number.getDecimalSeparator();
    var regExp = new RegExp("[^0-9" + escapeRegExp(decimalSeparator) + "]", "g");
    var textBeforePosition = text.slice(0, index);
    return textBeforePosition.replace(regExp, "").length
};
var _reverseText = function(text) {
    return text.split("").reverse().join("")
};
var _getDigitPositionByIndex = function(digitIndex, text) {
    if (!digitIndex) {
        return -1
    }
    var regExp = /[0-9]/g;
    var counter = 1;
    var index = null;
    var result = regExp.exec(text);
    while (result) {
        index = result.index;
        if (counter >= digitIndex) {
            return index
        }
        counter++;
        result = regExp.exec(text)
    }
    return null === index ? text.length : index
};
var _trimNonNumericCharsFromEnd = function(text) {
    return text.replace(/[^0-9e]+$/, "")
};
export var getCaretWithOffset = function(caret, offset) {
    if (void 0 === caret.start) {
        caret = {
            start: caret,
            end: caret
        }
    }
    return {
        start: caret.start + offset,
        end: caret.end + offset
    }
};
export var getCaretAfterFormat = function(text, formatted, caret, format) {
    caret = getCaretWithOffset(caret, 0);
    var point = number.getDecimalSeparator();
    var isSeparatorBasedText = isSeparatorBasedString(text);
    var realSeparatorOccurrenceIndex = getRealSeparatorIndex(format).occurrence;
    var pointPosition = isSeparatorBasedText ? 0 : getNthOccurrence(text, point, realSeparatorOccurrenceIndex);
    var newPointPosition = getNthOccurrence(formatted, point, realSeparatorOccurrenceIndex);
    var textParts = splitByIndex(text, pointPosition);
    var formattedParts = splitByIndex(formatted, newPointPosition);
    var isCaretOnFloat = -1 !== pointPosition && caret.start > pointPosition;
    if (isCaretOnFloat) {
        var relativeIndex = caret.start - pointPosition - 1;
        var digitsBefore = _getDigitCountBeforeIndex(relativeIndex, textParts[1]);
        var newPosition = formattedParts[1] ? newPointPosition + 1 + _getDigitPositionByIndex(digitsBefore, formattedParts[1]) + 1 : formatted.length;
        return getCaretInBoundaries(newPosition, formatted, format)
    } else {
        var formattedIntPart = _trimNonNumericCharsFromEnd(formattedParts[0]);
        var positionFromEnd = textParts[0].length - caret.start;
        var digitsFromEnd = _getDigitCountBeforeIndex(positionFromEnd, _reverseText(textParts[0]));
        var newPositionFromEnd = _getDigitPositionByIndex(digitsFromEnd, _reverseText(formattedIntPart));
        var newPositionFromBegin = formattedIntPart.length - (newPositionFromEnd + 1);
        return getCaretInBoundaries(newPositionFromBegin, formatted, format)
    }
};

function isSeparatorBasedString(text) {
    return 1 === text.length && !!text.match(/^[,.][0-9]*$/g)
}
export var isCaretInBoundaries = function(caret, text, format) {
    caret = getCaretWithOffset(caret, 0);
    var boundaries = getCaretInBoundaries(caret, text, format);
    return caret.start >= boundaries.start && caret.end <= boundaries.end
};
export function getCaretInBoundaries(caret, text, format) {
    caret = getCaretWithOffset(caret, 0);
    var boundaries = getCaretBoundaries(text, format);
    var adjustedCaret = {
        start: fitIntoRange(caret.start, boundaries.start, boundaries.end),
        end: fitIntoRange(caret.end, boundaries.start, boundaries.end)
    };
    return adjustedCaret
}
export var getCaretOffset = function(previousText, newText, format) {
    var previousBoundaries = getCaretBoundaries(previousText, format);
    var newBoundaries = getCaretBoundaries(newText, format);
    return newBoundaries.start - previousBoundaries.start
};
