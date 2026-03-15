
define(function () {
    return {
        OpenPadCommandIsOnMouseDown: function (scope, command, control, screenId) {
            return false;
        },

        OpenPadCommandExecute: async function (scope, command, control, screenId) {
            var minValue = command.minValue;
            var maxValue = command.maxValue;
            if (command.SVGMinValueReferenceId && scope.dataValues[screenId][command.SVGMinValueReferenceId].isGood && !isNaN(parseFloat(scope.dataValues[screenId][command.SVGMinValueReferenceId].invariantValue))) {
                minValue = scope.dataValues[screenId][command.SVGMinValueReferenceId].invariantValue;
            }
            if (command.SVGMaxValueReferenceId && scope.dataValues[screenId][command.SVGMaxValueReferenceId].isGood && !isNaN(parseFloat(scope.dataValues[screenId][command.SVGMaxValueReferenceId].invariantValue))) {
                maxValue = scope.dataValues[screenId][command.SVGMaxValueReferenceId].invariantValue;
            }
            switch (command.openpadtype) {

                case /*'NumericPad'*/0:
                    var valueentered = await scope.showNumericPad(scope, command._aspassword, minValue, maxValue, parseFloat(scope.dataValues[screenId][command.SVGReferenceId].invariantValue), command.decimals);
                    if (valueentered !== null) {
                        SetValue(scope, command.SVGReferenceId, valueentered, false, screenId, command);
                    }
                    break;
                case /*'AlphaNumericPad'*/1:
                    var valueenteredalpha = await scope.showAlphanumericPad(scope, command._aspassword, scope.dataValues[screenId][command.SVGReferenceId].value, maxValue);
                    if (valueenteredalpha !== null) {
                        SetValue(scope, command.SVGReferenceId, valueenteredalpha, false, screenId, command);
                    }
                    break;
            }
        },

        OpenPadCommandCanExecute: function (scope, command, control, screenId) {
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

