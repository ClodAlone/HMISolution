window.onload = init;
var audio;
var isMute = false;
var isPlaying = false;
var timerAlarmsBuzzing;

function writetostatus(input) {
    window.status = input
    return true
}

function init() {
    isMute = AlarmSinkService.IsAlarmBuzzingDisabled();
    audio = new Audio('audio/alarm.mp3');
    audio.loop = true;
    scheduleAlarmsSoundCheck();
}

function scheduleAlarmsSoundCheck() {
    clearTimeout(timerAlarmsBuzzing);
    if (!isMute) {
        AlarmSinkService.IsSoundBuzzing(onSuccessIsSoundBuzzing, onFailureIsSoundBuzzing);
        timeoutAlarmsBuzzing = setTimeout(
            function () { scheduleAlarmsSoundCheck(); },
            1000
        );
    }
}

function onSuccessIsSoundBuzzing(isBuzzing) {
    if (isBuzzing) {
        if (!isPlaying) {
            isPlaying = true;
            audio.play();
        }
    }
    else {
        if (isPlaying) {
            isPlaying = false;
            audio.pause();
        }
    }
}

function onFailureIsSoundBuzzing() {
    writetostatus("Error Checking Is Sound Buzzing: no response from service!!");
}
