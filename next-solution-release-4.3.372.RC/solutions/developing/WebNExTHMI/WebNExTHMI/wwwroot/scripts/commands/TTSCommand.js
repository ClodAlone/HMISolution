
var voices = window.speechSynthesis.getVoices();
window.speechSynthesis.onvoiceschanged = function () {
    voices = window.speechSynthesis.getVoices();
};

define(function () {
    return {
        TTSCommandIsOnMouseDown: function (scope, command, control, screenId) {
            return false;
        },

        TTSCommandExecute: function (scope, command, control, screenId) {

            if ('speechSynthesis' in window) {
                // Synthesis support. Make your web apps talk!

                var speak = i18next.t(command.speak);

                var msg = new SpeechSynthesisUtterance();
                // var voices = window.speechSynthesis.getVoices();
                // msg.voice = voices[10]; // Note: some voices don't support altering params
                // msg.voiceURI = 'native';
                if (command.voiceName) {

                    for (var i = 0; i < voices.length; ++i) {

                        if (voices[i].name === command.voiceName) {
                            msg.voice = voices[i];
                            break;
                        }
                    }
                }
                if (command.volume)
                    msg.volume = command.volume / 100; // 0 to 1
                if (command.rate)
                    msg.rate = command.rate; // 0.1 to 10
                // msg.pitch = 2; //0 to 2
                msg.text = speak;
                msg.lang = i18next.language;

                msg.onend = function (e) {
                    console.log('Finished in ' + event.elapsedTime + ' seconds.');
                };

                speechSynthesis.speak(msg);            }
            else
                scope.showMesageBox(i18next.t('TTSNotAvailable'));
        },

        TTSCommandCanExecute: function (scope, command, control, screenId) {
            if ('speechSynthesis' in window)
                return true;
            return false;
        }
    };
});

