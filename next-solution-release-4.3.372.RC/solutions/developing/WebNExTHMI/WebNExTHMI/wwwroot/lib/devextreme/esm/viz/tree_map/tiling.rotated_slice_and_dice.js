/**
 * DevExtreme (esm/viz/tree_map/tiling.rotated_slice_and_dice.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import {
    getAlgorithm,
    addAlgorithm
} from "./tiling";
var sliceAndDiceAlgorithm = getAlgorithm("sliceanddice");

function rotatedSliceAndDice(data) {
    data.isRotated = !data.isRotated;
    return sliceAndDiceAlgorithm.call(this, data)
}
addAlgorithm("rotatedsliceanddice", rotatedSliceAndDice);
