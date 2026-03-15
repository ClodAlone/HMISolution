
var listAppendDecimalONOFF = new Array();

define(function () {
    return {
        ValueCommandIsOnMouseDown: function (scope, command, control, screenId) {
            return command.type == 4/*'Impulsive'*/ || command.type == 5/*'ImpulsiveLatch'*/;
        },

        ValueCommandExecute: async function (scope, command, control, screenId) {
            switch (command.type) {

                case /*'Set'*/0:
                    SetValue(scope, command.SVGReferenceId, command._value, false, screenId, command);
                    break;
                case /*'Increase'*/1:
                    var valueinc = 1;
                    if (command._value)
                        valueinc = parseFloat(command._value);
                    var valueInc = parseFloat(scope.dataValues[screenId][command.SVGReferenceId].invariantValue) + valueinc;
                    SetValue(scope, command.SVGReferenceId, valueInc, false, screenId, command);
                    break;
                case /*'Decrease'*/2:
                    var valuedec = 1;
                    if (command._value)
                        valuedec = parseFloat(command._value);
                    var valueDec = parseFloat(scope.dataValues[screenId][command.SVGReferenceId].invariantValue) - valuedec;
                    SetValue(scope, command.SVGReferenceId, valueDec, false, screenId, command);
                    break;
                case /*'Toggle'*/3:
                    var valueToggle = 0;
                    var n = normalizeDataValue(scope.dataValues[screenId][command.SVGReferenceId]);
                    if (n.bNormalized)
                        valueToggle = !n.dataValue;
                    else
                        valueToggle = !parseFloat(scope.dataValues[screenId][command.SVGReferenceId].invariantValue);
                    if (valueToggle && command._value)
                        valueToggle = command._value;
                    SetValue(scope, command.SVGReferenceId, valueToggle, false, screenId, command);
                    break;
                case /*'Impulsive'*/4:
                    SetValue(scope, command.SVGReferenceId, command._value, false, screenId, command);
                    var delayImpulsive;
                    if (command._time > 0) {
                        delayImpulsive = setTimeout(function () {
                            SetValue(scope, command.SVGReferenceId, 0, false, screenId, command);
                        }, command._time);
                    }
                    control.each(function (i, children) {
                        //$(this).mouseout(function () {
                        $(this.node).on("mouseleave", function () {
                            SetValue(scope, command.SVGReferenceId, 0, false, screenId, command);
                            if (delayImpulsive) {
                                clearTimeout(delayImpulsive);
                                delayImpulsive = undefined;
                            }
                        });
                        //this.touchleave(function () {
                        $(this.node).on("touchend mouseup", function () {
                            SetValue(scope, command.SVGReferenceId, 0, false, screenId, command);
                            if (delayImpulsive) {
                                clearTimeout(delayImpulsive);
                                delayImpulsive = undefined;
                            }
                        });
                        //this.touchcancel(function () {
                        //    SetValue(scope, command.SVGReferenceId, 0, false, screenId, command);
                        //});
                    });
                    break;
                case /*'ImpulsiveLatch'*/5:
                    SetValue(scope, command.SVGReferenceId, command._value, false, screenId, command);
                    var delayImpulsiveLatch;
                    if (command._time > 0) {
                        delayImpulsiveLatch = setTimeout(function () {
                            SetValue(scope, command.SVGReferenceId, 0, false, screenId, command);
                        }, command._time);
                    }
                    else {
                        control.each(function (i, children) {
                            $(this.node).on("mouseleave", function () {
                                SetValue(scope, command.SVGReferenceId, 0, false, screenId, command);
                                if (delayImpulsiveLatch) {
                                    clearTimeout(delayImpulsiveLatch);
                                    delayImpulsiveLatch = undefined;
                                }
                            });
                            //this.touchleave(function () {
                            $(this.node).on("touchend mouseup", function () {
                                SetValue(scope, command.SVGReferenceId, 0, false, screenId, command);
                                if (delayImpulsiveLatch) {
                                    clearTimeout(delayImpulsiveLatch);
                                    delayImpulsiveLatch = undefined;
                                }
                            });
                            //this.touchcancel(function () {
                            //    SetValue(scope, command.SVGReferenceId, 0, false, screenId, command);
                            //    if (delayImpulsive) {
                            //        clearTimeout(delayImpulsive);
                            //        delayImpulsive = undefined;
                            //    }
                            //});
                        });
                    }
                    break;
                case /*'TransferValue'*/6:
                    SetValue(scope, command.SVGTransferReferenceId, scope.dataValues[screenId][command.SVGReferenceId].invariantValue, false, screenId, command);
                    break;
                case /*'NumericPad'*/7:
                    var minValue = command.minValue;
                    var maxValue = command.maxValue;
                    if (command.SVGMinValueReferenceId && scope.dataValues[screenId][command.SVGMinValueReferenceId].isGood && !isNaN(parseFloat(scope.dataValues[screenId][command.SVGMinValueReferenceId].invariantValue))) {
                        minValue = scope.dataValues[screenId][command.SVGMinValueReferenceId].invariantValue;
                    }
                    if (command.SVGMaxValueReferenceId && scope.dataValues[screenId][command.SVGMaxValueReferenceId].isGood && !isNaN(parseFloat(scope.dataValues[screenId][command.SVGMaxValueReferenceId].invariantValue))) {
                        maxValue = scope.dataValues[screenId][command.SVGMaxValueReferenceId].invariantValue;
                    }
                    var valueentered = await scope.showNumericPad(scope, command._aspassword, minValue, maxValue, parseFloat(scope.dataValues[screenId][command.SVGReferenceId].invariantValue), command.decimals);
                    if (valueentered !== null) {
                        SetValue(scope, command.SVGReferenceId, valueentered, false, screenId, command);
                    }
                    break;
                case /*'AlphaNumericPad'*/8:
                    var minValue = command.minValue;
                    var maxValue = command.maxValue;
                    if (command.SVGMinValueReferenceId && scope.dataValues[screenId][command.SVGMinValueReferenceId].isGood && !isNaN(parseFloat(scope.dataValues[screenId][command.SVGMinValueReferenceId].invariantValue))) {
                        minValue = scope.dataValues[screenId][command.SVGMinValueReferenceId].invariantValue;
                    }
                    if (command.SVGMaxValueReferenceId && scope.dataValues[screenId][command.SVGMaxValueReferenceId].isGood && !isNaN(parseFloat(scope.dataValues[screenId][command.SVGMaxValueReferenceId].invariantValue))) {
                        maxValue = scope.dataValues[screenId][command.SVGMaxValueReferenceId].invariantValue;
                    }
                    var valueenteredalpha = await scope.showAlphanumericPad(scope, command._aspassword, scope.dataValues[screenId][command.SVGReferenceId].value, maxValue);
                    if (valueenteredalpha !== null) {
                        SetValue(scope, command.SVGReferenceId, valueenteredalpha, false, screenId, command);
                    }
                    break;
                case /*'ResetStatistics'*/9:
                    scope.busyVisible = true;
                    scope.$apply();
                    scope.corehubconnection.invoke("ResetStatistics", command.SVGReferenceId, screenId)
                        .then((ret) => {

                            scope.busyVisible = false;
                            scope.$apply();
                        })
                        .catch(err => {
                            console.error(err.toString());

                            scope.busyVisible = false;
                            scope.$apply();
                            scope.$broadcast(onError, { ex: err, value: "Error opening : " + err.toString() });
                        });
                    break;

                case /*AppendValue*/ 10:
                    if (command.decimalUnaware)
                        strAppend = scope.dataValues[screenId][command.SVGReferenceId].value + command._value;
                    else {
                        var foundAppend = listAppendDecimalONOFF.indexOf(command.SVGReferenceId);
                        var splitAppend = scope.dataValues[screenId][command.SVGReferenceId].invariantValue.split(invariantNumberDecimalSeparator);
                        if (foundAppend === -1) {

                            if (splitAppend.length > 1)
                                strAppend = splitAppend[0] + command._value + invariantNumberDecimalSeparator + splitAppend[1];
                            else
                                strAppend = splitAppend[0] + command._value;
                        }
                        else {

                            if (splitAppend.length > 1)
                                strAppend = splitAppend[0] + invariantNumberDecimalSeparator + splitAppend[1] + command._value;
                            else
                                strAppend = splitAppend[0] + invariantNumberDecimalSeparator + command._value;
                        }
                    }
                    SetValue(scope, command.SVGReferenceId, strAppend, false, screenId, command);
                    break;
                case /*AppendDecimalONOFF*/ 11:
                    var found = listAppendDecimalONOFF.indexOf(command.SVGReferenceId);
                    if (found === -1)
                        listAppendDecimalONOFF.push(command.SVGReferenceId);
                    else
                        listAppendDecimalONOFF.splice(found, 1);
                    break;
                case /*RemoveValue*/ 12:
                    if (command.decimalUnaware) {
                        strRemove = scope.dataValues[screenId][command.SVGReferenceId].value;
                        if (strRemove.length > 0) {
                            strRemove = strRemove.slice(0, strRemove.length - 1);

                            if (strRemove.length === 0) {
                                var isNumeric = parseFloat(scope.dataValues[screenId][command.SVGReferenceId].invariantValue);
                                if (!isNaN(isNumeric))
                                    strRemove = "0";
                            }
                        }
                    }
                    else {
                        var foundRemove = listAppendDecimalONOFF.indexOf(command.SVGReferenceId);
                        var splitRemove = scope.dataValues[screenId][command.SVGReferenceId].invariantValue.split(invariantNumberDecimalSeparator);
                        if (foundRemove === -1) {

                            if (splitRemove.length > 1) {

                                if (splitRemove[0].length > 0)
                                    strRemove = splitRemove[0].slice(0, splitRemove[0].length - 1) + invariantNumberDecimalSeparator + splitRemove[1];
                                else
                                    strRemove = splitRemove[0] + invariantNumberDecimalSeparator + splitRemove[1];
                            }
                            else {

                                strRemove = scope.dataValues[screenId][command.SVGReferenceId].value;
                                if (strRemove.length > 0) {
                                    strRemove = strRemove.slice(0, strRemove.length - 1);

                                    if (strRemove.length === 0) {
                                        var isNumeric = parseFloat(scope.dataValues[screenId][command.SVGReferenceId].invariantValue);
                                        if (!isNaN(isNumeric))
                                            strRemove = "0";
                                    }
                                }
                            }
                        }
                        else {

                            if (splitRemove.length > 1) {
                                if (splitRemove[1].length > 0)
                                    strRemove = splitRemove[0] + invariantNumberDecimalSeparator + splitRemove[1].slice(0, splitRemove[1].length - 1);
                                else
                                    strRemove = splitRemove[0] + invariantNumberDecimalSeparator + splitRemove[1];
                            }
                            else
                                strRemove = splitRemove[0];
                        }
                    }
                    SetValue(scope, command.SVGReferenceId, strRemove, false, screenId, command);
                    break;
                case /*SwapSign*/ 13:
                    var valueSwap = scope.dataValues[screenId][command.SVGReferenceId].value;
                    if (valueSwap.length > 0) {
                        if (valueSwap[0] === '-')
                            valueSwap = valueSwap.slice(1, valueSwap.length);
                        else
                            valueSwap = "-" + valueSwap;
                        SetValue(scope, command.SVGReferenceId, valueSwap, false, screenId, command);
                    }
                    break;
            }
        },

        ValueCommandCanExecute: function (scope, command, control, screenId) {
            return !command.pendingCommand && screenId in scope.dataValues &&  
                command.SVGReferenceId in scope.dataValues[screenId] && scope.dataValues[screenId][command.SVGReferenceId].isGood;
        }
    };
});

function SetValue(scope, id, value, busy, screenId, command) {

    if (!(screenId in scope.dataValues) || !(id in scope.dataValues[screenId]))
        return;
    if (!scope.dataValues[screenId][id].isGood)
        return;

    command.pendingCommand = true;
    if (busy) {
        scope.busyVisible = true;
        scope.$apply();
    }

    scope.corehubconnection.invoke("SetValue", id, String(value), screenId)
        .then((ret) => {

            command.pendingCommand = false;
            if (busy) {
                scope.busyVisible = false;
                scope.$apply();
            }
        })
        .catch (err => {
            console.error(err.toString());

            command.pendingCommand = false;
            if (busy) {
                scope.busyVisible = false;
                scope.$apply();
            }
            scope.$broadcast(onError, { ex: err, value: "Error setting value : " + err.toString() });
    });
}