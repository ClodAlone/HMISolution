window.onload = init;
var loading = true;

function writetostatus(input) {
    window.status = input
    return true
}

function init() {
    loading = true;
    LoadingPanel.Show();
}

function AckAll() {
    LoadingPanel.Show();
    AlarmSinkService.AckAll(onSuccessAckAll, onFailureAckAll);
}

function onSuccessAckAll() {
    LoadingPanel.Hide();
}

function onFailureAckAll() {
    LoadingPanel.Hide();
    writetostatus("Error Ack All Command: no response from service!!");
}

function AckSelected() {
    LoadingPanel.Show();
    AlarmSinkService.AckSelected(selList, onSuccessConfirmAll, onFailureConfirmAll);
}
function ConfirmSelected() {
    LoadingPanel.Show();
    AlarmSinkService.ConfirmSelected(selList, onSuccessConfirmAll, onFailureConfirmAll);
}

function ConfirmAll() {
    LoadingPanel.Show();
    AlarmSinkService.ConfirmAll(onSuccessConfirmAll, onFailureConfirmAll);
}

function onSuccessConfirmAll() {
    LoadingPanel.Hide();
}

function onFailureConfirmAll() {
    LoadingPanel.Hide();
    writetostatus("Error Confirm All Command: no response from service!!");
}

function Refresh() {
    LoadingPanel.Show();
    AlarmSinkService.Refresh(onSuccessRefresh, onFailureRefresh);
}

function onSuccessRefresh() {
    LoadingPanel.Hide();
}

function onFailureRefresh() {
    LoadingPanel.Hide();
    writetostatus("Error Confirm All Command: no response from service!!");
}

var timeout;
function scheduleGridUpdate(grid) {
    window.clearTimeout(timeout);
    timeout = window.setTimeout(
        function () { grid.Refresh(); },
        2000
    );
}

function grid_SelectionChanged(s, e) {
    s.GetSelectedFieldValues("NodeIdString",GetSelectedFieldValuesCallback);
}
var selList = [];
function GetSelectedFieldValuesCallback(values) {
    selList = [];
    for (var i = 0; i < values.length; i++) {
        selList.push(values[i]);
    }
    
    var enable = selList.length > 0;
    btnActSelected.SetEnabled(enable)
    btnConfirmSelected.SetEnabled(enable)
}

function grid_Init(s, e) {
    scheduleGridUpdate(s);
}
function grid_BeginCallback(s, e) {
    window.clearTimeout(timeout);
}
function grid_EndCallback(s, e) {
    scheduleGridUpdate(s);

    AlarmSinkService.IsConnected(onSuccessIsConnected, onFailureIsConnected);
}

function onSuccessIsConnected(connected) {
    if (connected) {
        if (loading) {
            loading = false;
            LoadingPanel.Hide();
        }
    }
    else {
        if (!loading) {
            loading = true;
            LoadingPanel.Show();
        }
    }
}

function onFailureIsConnected() {
    writetostatus("Error Checking Is Connected: no response from service!!");
}
