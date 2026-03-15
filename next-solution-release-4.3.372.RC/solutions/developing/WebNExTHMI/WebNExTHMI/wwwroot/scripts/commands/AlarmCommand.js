var alarmServerCommandSubscribed = [] ;
var alarmServerCommandAvailable = [] ;
var listeningArray = {};

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
                    scope.corehubconnection.invoke("AckResetAll", false, true, currentPopupId)
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
                    scope.corehubconnection.invoke("AckResetAll", true, true, currentPopupId)
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
            if (command.type == 0 || command.type == 1) {
                if (alarmServerCommandSubscribed.indexOf(screenId) == -1) {
                    alarmServerCommandSubscribed.push(screenId);
                    var deregisterListenerViewClosedAlarm = scope.$on(onViewClosed + screenId, function (value, obj) {
                        let idx1 = alarmServerCommandSubscribed.indexOf(screenId);
                        if (idx1 != -1)
                            alarmServerCommandSubscribed.splice(idx1, 1);
                        let idx2 = alarmServerCommandAvailable.indexOf(screenId);
                        if (idx2 != -1)
                            alarmServerCommandAvailable.splice(idx2, 1);
                        if (screenId in listeningArray) {
                            listeningArray[screenId].forEach(function (item, index, array) {
                                item();
                            });
                            delete listeningArray[screenId];
                        }
                    });
                    listeningArray[screenId] = new Array();
                    listeningArray[screenId].push(deregisterListenerViewClosedAlarm);

                    scope.busyVisible = true;
                    scope.$apply();
                    var deregisterCanAlarmCommandExecute = scope.$on("CanAckResetCommandUpdate" + screenId, function (e, bCanExecute) {
                        let idx = alarmServerCommandAvailable.indexOf(screenId);
                        if (bCanExecute) {
                            if (idx == -1)
                                alarmServerCommandAvailable.push(screenId);
                        }
                        else
                            alarmServerCommandAvailable.splice(idx, 1);
                    });
                    listeningArray[screenId].push(deregisterCanAlarmCommandExecute);

                    scope.corehubconnection.invoke("SubscribeCanAckResetAll", screenId)
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
                    return false;
                }
                else
                    return alarmServerCommandAvailable.indexOf(screenId) != -1;
            }
            return true;
        }
    };
});
