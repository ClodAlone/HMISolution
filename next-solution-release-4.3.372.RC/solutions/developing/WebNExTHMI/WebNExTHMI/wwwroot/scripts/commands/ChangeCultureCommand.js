
define(function () {
    return {
        ChangeCultureCommandIsOnMouseDown: function (scope, command, control, screenId) {
            return false;
        },

        ChangeCultureCommandExecute: function (scope, command, control, screenId) {
            scope.changeLanguage(command.cultureName);
        },

        ChangeCultureCommandCanExecute: function (scope, command, control, screenId) {
            return true;
        }
    };
});
