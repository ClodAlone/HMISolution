
define(function () {
    return {
        AlarmCommandIsOnMouseDown: function (scope, command, control, screenId) {
            return false;
        },

        AlarmCommandExecute: function (scope, command, control, currentPopupId) {

            switch (command.type) {
                case 0: // ackAll
                    scope.busyVisible = true;
                    scope.$apply();
                    scope.corehubconnection.invoke("AckResetAll", false)
                        .then((ret) => {

                            scope.busyVisible = false;
                            scope.$apply();
                        })
                        .catch(err => {
                            console.error(err.toString());

                            scope.busyVisible = false;
                            scope.$apply();
                            scope.$broadcast(onError, { ex: err, value: "Error Executing Alarm Command : " + err.toString() });
                        });
                    break;
                case 1: // confirmAll
                    scope.busyVisible = true;
                    scope.$apply();
                    scope.corehubconnection.invoke("AckResetAll", true)
                        .then((ret) => {

                            scope.busyVisible = false;
                            scope.$apply();
                        })
                        .catch(err => {
                            console.error(err.toString());

                            scope.busyVisible = false;
                            scope.$apply();
                            scope.$broadcast(onError, { ex: err, value: "Error Executing Alarm Command : " + err.toString() });
                        });
                    break;
                case 2: // toggleSound
                    scope.busyVisible = true;
                    scope.$apply();
                    scope.corehubconnection.invoke("ToggleAlarmSound")
                        .then((ret) => {

                            scope.busyVisible = false;
                            scope.$apply();
                        })
                        .catch(err => {
                            console.error(err.toString());

                            scope.busyVisible = false;
                            scope.$apply();
                            scope.$broadcast(onError, { ex: err, value: "Error Executing Alarm Command : " + err.toString() });
                        });
                    break;

                case 3: // ShowStatisticReport
                    {
                        var options = {
                            name: command.reportName,
                            connectionString: command.connectionString,
                            parameters: command.parameters,
                            ReportAlarmDoc: 'ReportAlarmDoc',
                            ReportType: command.reportType,
                            PeriodType: command.AlarmReportPeriodType,
                            MaxTake: command.maxTake,
                            CommandTimeout: command.commandTimeout,
                            AlarmsSource: command.alarmsSource,
                            titletxt: i18next.t('Alarm Statistics'),
                            width: '100%',
                            height: '100%'
                        };

                        scope.activatePopup('reportView', JSON.stringify(options));
                    }
                    break;

                case 4: // PrintStatisticReport
                case 5: // SaveStatisticReport
                case 6: // SendStatisticReport

                    scope.busyVisible = true;
                    scope.$apply();
                    command.ReportAlarmDoc = 'ReportAlarmDoc';
                    scope.corehubconnection.invoke("SavePrintSendReport", JSON.stringify(command))
                        .then((ret) => {
                            
                            scope.busyVisible = false;
                            scope.$apply();
                            if (ret !== null)
                            {
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

        AlarmCommandCanExecute: function (scope, command, control, screenId) {
                return true;
        }
    };
});
