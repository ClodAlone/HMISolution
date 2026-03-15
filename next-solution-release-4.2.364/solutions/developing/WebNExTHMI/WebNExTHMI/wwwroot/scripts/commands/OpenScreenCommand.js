
define(function () {
    return {
        OpenScreenCommandIsOnMouseDown: function (scope, command, control, screenId) {
            return false;
        },

        OpenScreenCommandExecute: function (scope, command, control, currentScreenId, currentPopupId) {
            if (!command.screenName) {

                if (currentPopupId) {
                    scope.$broadcast(onPopupCloseRequest + currentPopupId, currentPopupId);
                }
                else
                    scope.activateLastView();
            }
            else if (!isStopCommand(command)) {
                var xCoord = command.x;
                var yCoord = command.y;
                var isContextPopup = scope.IsContextPopup(command);
                if (isContextPopup && (isNaN(xCoord) || isNaN(yCoord))) {
                    xCoord = command.eventCoords.x;
                    yCoord = command.eventCoords.y;
                }

                var options = {
                    name: command.screenName, parameter: command.preserveCurrentParameterFile ? "*" : command.parameterFile, modal: scope.IsPopupModal(command),
                    x: xCoord, y: yCoord, callerScreenId: currentScreenId, contextPopup: isContextPopup
                };
                if (scope.IsPopupMode(command)) {
                    scope.activatePopup('screenView', JSON.stringify(options));
                }
                else {
                    scope.activateView('screenView', JSON.stringify(options));
                }
            }
            /*
            if (command.commandtype == 0)
                scope.showMainPage();
            else if (command.commandtype == 1) {
                var options = { name: parameters.commandparameter };
                scope.activatePopup('screenView', JSON.stringify(options));
            }
            else if (parameters.commandtype == 2) {
                if (parameters.currentPopupId)
                    scope.$broadcast(onPopupCloseRequest + parameters.currentPopupId, parameters.currentPopupId);
                else
                    scope.activateLastView();
            }
            else if (parameters.commandtype == 3) {
                var options = { name: parameters.commandparameter };
                scope.activateView('screenView', JSON.stringify(options));
            }
            else if (parameters.commandtype == 4) {
                var options = { name: parameters.commandparameter };
                scope.activatePopup('mapView', JSON.stringify(options), 600, 500);
            }
            */
        },

        OpenScreenCommandCanExecute: function (scope, command, control, screenId) {
                return true;
        }
    };

    function isStopCommand(command) {
        if (command.executionModeEx === null)
            return command.executionMode == 3;
        return command.executionModeEx == 5;
    }
});
