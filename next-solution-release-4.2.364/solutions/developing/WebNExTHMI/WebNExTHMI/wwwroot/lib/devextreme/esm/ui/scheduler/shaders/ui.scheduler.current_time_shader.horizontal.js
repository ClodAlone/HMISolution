/**
 * DevExtreme (esm/ui/scheduler/shaders/ui.scheduler.current_time_shader.horizontal.js)
 * Version: 21.2.6
 * Build date: Tue Mar 01 2022
 *
 * Copyright (c) 2012 - 2022 Developer Express Inc. ALL RIGHTS RESERVED
 * Read about DevExtreme licensing here: https://js.devexpress.com/Licensing/
 */
import {
    setWidth
} from "../../../core/utils/size";
import {
    getBoundingRect
} from "../../../core/utils/position";
import CurrentTimeShader from "../shaders/ui.scheduler.current_time_shader";
class HorizontalCurrentTimeShader extends CurrentTimeShader {
    renderShader() {
        var groupCount = this._workSpace._isHorizontalGroupedWorkSpace() ? this._workSpace._getGroupCount() : 1;
        for (var i = 0; i < groupCount; i++) {
            var isFirstShader = 0 === i;
            var $shader = isFirstShader ? this._$shader : this.createShader();
            if (this._workSpace.isGroupedByDate()) {
                this._customizeGroupedByDateShader($shader, i)
            } else {
                this._customizeShader($shader, i)
            }!isFirstShader && this._shader.push($shader)
        }
    }
    _customizeShader($shader, groupIndex) {
        var shaderWidth = this._workSpace.getIndicationWidth();
        this._applyShaderWidth($shader, shaderWidth);
        if (groupIndex >= 1) {
            var workSpace = this._workSpace;
            var indicationWidth = workSpace._getCellCount() * workSpace.getCellWidth();
            $shader.css("left", indicationWidth)
        } else {
            $shader.css("left", 0)
        }
    }
    _applyShaderWidth($shader, width) {
        var maxWidth = getBoundingRect(this._$container.get(0)).width;
        if (width > maxWidth) {
            width = maxWidth
        }
        if (width > 0) {
            setWidth($shader, width)
        }
    }
    _customizeGroupedByDateShader($shader, groupIndex) {
        var cellCount = this._workSpace.getIndicationCellCount();
        var integerPart = Math.floor(cellCount);
        var fractionPart = cellCount - integerPart;
        var isFirstShaderPart = 0 === groupIndex;
        var workSpace = this._workSpace;
        var shaderWidth = isFirstShaderPart ? workSpace.getIndicationWidth() : fractionPart * workSpace.getCellWidth();
        var shaderLeft;
        this._applyShaderWidth($shader, shaderWidth);
        if (isFirstShaderPart) {
            shaderLeft = workSpace._getCellCount() * workSpace.getCellWidth() * groupIndex
        } else {
            shaderLeft = workSpace.getCellWidth() * integerPart * workSpace._getGroupCount() + groupIndex * workSpace.getCellWidth()
        }
        $shader.css("left", shaderLeft)
    }
}
export default HorizontalCurrentTimeShader;
