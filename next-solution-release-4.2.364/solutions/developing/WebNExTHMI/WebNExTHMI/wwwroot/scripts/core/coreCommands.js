
const commandpath = './scripts/commands/';

var moduleLoaded = {};

function initCommands($scope) {

    $scope.isCommandOnMouseDown = function (command, control, screenId) {

        try {
            var module = commandpath + command.__type + '.js';

            if (moduleLoaded[module])
                return moduleLoaded[module][command.__type + 'IsOnMouseDown']($scope, command, control, screenId);

            require([module], function (moduleCommand) {
                moduleLoaded[module] = moduleCommand;
            });
        } catch (e) {

            console.log(e.toString());
        }

        return false;
    };

    $scope.executeCommand = function (command, control, screenId, popupparameter) {

        try {
            var module = commandpath + command.__type + '.js';

            if (moduleLoaded[module])
                return moduleLoaded[module][command.__type + 'Execute']($scope, command, control, screenId, popupparameter);

            require([module], function (moduleCommand) {
                moduleLoaded[module] = moduleCommand;
                moduleCommand[command.__type + 'Execute']($scope, command, control, screenId, popupparameter);
            });
        } catch (e) {

            console.log(e.toString());
            $scope.showMesageBox(i18next.t('CommandNotAvailable') + ' : ' + command.__type);
        }
    };

    $scope.canExecuteCommand = function (command, control, screenId) {

        try {
            var module = commandpath + command.__type + '.js';

            if (moduleLoaded[module])
                return moduleLoaded[module][command.__type + 'CanExecute']($scope, command, control, screenId);

            require([module], function (moduleCommand) {
                moduleLoaded[module] = moduleCommand;
            });
        } catch (e) {

            console.log(e.toString());
        }

        return false;
    };

    var commandExecutionModeEx = [
        "Normal",
        "ModalPopup",
        "FramePopup",
        "ContextPopup",
        "Shared",
        "Stop"
    ];

    $scope.IsPopupMode = function (command) {
        if (command.executionModeEx === null)
            return command.synchroPopup || command.executionMode == 1;

        if (command.executionModeEx < 0 || commandExecutionModeEx.length <= command.executionModeEx)
            return command.synchroPopup || command.synchroFrame;
        
        var commandType = commandExecutionModeEx[command.executionModeEx];
        return commandType === "ModalPopup" || commandType === "FramePopup" || commandType === "ContextPopup";
    }

    $scope.IsPopupModal = function (command) {
        if (command.executionModeEx === null)
            return !command.synchroFrame && (command.synchroPopup || command.executionMode == 1);

        if (command.executionModeEx < 0 || commandExecutionModeEx.length <= command.executionModeEx)
            return !command.synchroFrame && command.synchroPopup;

        var commandType = commandExecutionModeEx[command.executionModeEx];
        return commandType === "ModalPopup";
    }

    $scope.IsContextPopup = function (command) {
        if (command.executionModeEx === null)
            return command.synchroFrame && (command.synchroPopup || command.executionMode == 1);

        if (command.executionModeEx < 0 || commandExecutionModeEx.length <= command.executionModeEx)
            return command.synchroFrame && command.synchroPopup;

        return commandExecutionModeEx[command.executionModeEx] === "ContextPopup";
    }
}