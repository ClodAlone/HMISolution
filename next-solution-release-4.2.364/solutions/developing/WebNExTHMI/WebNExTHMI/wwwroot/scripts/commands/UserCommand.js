
define(function () {
    return {
        UserCommandIsOnMouseDown: function (scope, command, control, screenId) {
            return false;
        },

        UserCommandExecute: function (scope, command, control, screenId) {
            switch (command.commandType) {
                case 0: // 'Login':
                    scope.showLogin();
                    break;
                case 1: //'Logout':
                    scope.corehubconnection.invoke("LogoutUser", scope.currentLanguage);
                    break;
            }
        },

        UserCommandCanExecute: function (scope, command, control, screenId) {
            switch (command.commandType) {
                case 0: //'Login':
                    return true;
                case 1: //'Logout':
                    return scope.currentUser && scope.currentUser !== "";
                default:
                    return false;
            }
        }
    };
});
