
define(function () {
    return {
        RecipeCommandIsOnMouseDown: function (scope, command, control, screenId) {
            return false;
        },
        RecipeCommandExecute: async function (scope, command, control, screenId) {
            scope.busyVisible = true;
            scope.$apply();

            scope.corehubconnection.invoke("RecipeCommandExecute", command.recipeName, command.commandType, command.syncTimeout, command.synchronous)
                .then((ret) => {
                    scope.busyVisible = false;
                    scope.$apply();
                })
                .catch(err => {
                    console.error(err.toString());

                    scope.busyVisible = false;
                    scope.$apply();
                    scope.$broadcast(onError, { ex: err, value: getClientErrorText(err, "ErrorExecutingRecipeCommand") });
                });
        },

        RecipeCommandCanExecute: function (scope, command, control, screenId) {
            return !command.pendingCommand && screenId in scope.dataValues; //&& command.SVGReferenceId in scope.dataValues[screenId] && scope.dataValues[screenId][command.SVGReferenceId].isGood;
        }
    };
});