
const SpeechRecognition = window.SpeechRecognition || window.webkitSpeechRecognition;
var recognition = new SpeechRecognition();
recognition.continuous = true;

function initBusyIndicator($scope) {

    recognition.start();
    recognition.onresult = function (event) {
        var last = event.results.length - 1;
        var command = event.results[last][0].transcript;
        $scope.$broadcast(onSRNewCommand, command);
    }
    recognition.onerror = function (event) {
        console.log('Speech recognition error detected: ' + event.error);
    }
    recognition.onend = function () {
        console.log('Speech recognition service disconnected');
        recognition.start();
    }
}
