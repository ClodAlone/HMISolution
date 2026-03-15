
define(function () {
    return {
        ReportCommandIsOnMouseDown: function (scope, command, control, screenId) {
            return false;
        },

        ReportCommandExecute: function (scope, command, control, currentPopupId) {
            switch (command.commandType) {
                case 0: // Show
                case 2: // PrintDialog
                    {
                        scope.busyVisible = true;
                        scope.$apply();

                        var title = command.reportName.replace(/^.*[\\\/]/, '').replace(/\..+$/, '');
                        scope.corehubconnection.invoke("GetReportSize", command.reportName)
                            .then((ret) => {
                                var w = parseFloat(ret.width);
                                var h = parseFloat(ret.height);
                                var width = (!isNaN(w) && w > 0) ? w : $(currentRoot).width() * 0.75;
                                var height = (!isNaN(h) && h > 0) ? h : $(currentRoot).height() * 0.75;
                                var options = {
                                    name: command.reportName,
                                    connectionString: command.connectionString,
                                    parameters: command.parameters,
                                    screenId: currentPopupId,
                                    CommandTimeout: command.commandTimeout,
                                    titletxt: i18next.t(title),
                                    width: width,
                                    height: height
                                };

                                scope.activatePopup('reportView', JSON.stringify(options));

                                scope.busyVisible = false;
                                scope.$apply();
                            })
                            .catch(err => {
                                console.error(err.toString());

                                scope.busyVisible = false;
                                scope.$apply();
                                scope.$broadcast(onError, { ex: err, value: "Error Executing Report Command : " + err.toString() });
                            });
                    }
                    break;
                case 1: // Print
                case 3: // Save
                case 4: // Send

                    scope.busyVisible = true;
                    scope.$apply();
                    scope.corehubconnection.invoke("SavePrintSendReport", JSON.stringify(command))
                        .then((ret) => {
                            scope.busyVisible = false;
                            scope.$apply();
                            if (ret !== null) {
                                console.error(ret);
                                scope.$broadcast(onError, { ex: err, value: "Error Executing Report Command : " + ret });
                            }
                        })
                        .catch(err => {
                            console.error(err.toString());

                            scope.busyVisible = false;
                            scope.$apply();
                            scope.$broadcast(onError, { ex: err, value: "Error Executing Report Command : " + err.toString() });
                        });
                    break;
            }
        },

        ReportCommandCanExecute: function (scope, command, control, screenId) {
            switch (command.commandType) {
                case 0:
                    if (!command.parameters)
                        return true;
                    var dynamicParameters = command.parameters.filter(function (item) { return item.SVGReferenceId != null; });
                    if (dynamicParameters.length == 0)
                        return true;
                    if (!(screenId in scope.dataValues))
                        return false;
                    return dynamicParameters.filter(function (item) { return scope.dataValues[screenId][item.SVGReferenceId].isGood }).length == dynamicParameters.length;
                case 2: //'PrintDialog':
                    return false;
                default:
                    return true;
            }
        }
    };
});
