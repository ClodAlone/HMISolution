/**
 * DevExtreme (esm/viz/axes/axes_utils.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
var _max = Math.max;
export var calculateCanvasMargins = function(bBoxes, canvas) {
    var cLeft = canvas.left;
    var cTop = canvas.top;
    var cRight = canvas.width - canvas.right;
    var cBottom = canvas.height - canvas.bottom;
    return bBoxes.reduce((function(margins, bBox) {
        if (!bBox || bBox.isEmpty) {
            return margins
        }
        return {
            left: _max(margins.left, cLeft - bBox.x),
            top: _max(margins.top, cTop - bBox.y),
            right: _max(margins.right, bBox.x + bBox.width - cRight),
            bottom: _max(margins.bottom, bBox.y + bBox.height - cBottom)
        }
    }), {
        left: 0,
        right: 0,
        top: 0,
        bottom: 0
    })
};
export var measureLabels = function(items) {
    items.forEach((function(item) {
        var label = item.getContentContainer();
        item.labelBBox = label ? label.getBBox() : {
            x: 0,
            y: 0,
            width: 0,
            height: 0
        }
    }))
};
