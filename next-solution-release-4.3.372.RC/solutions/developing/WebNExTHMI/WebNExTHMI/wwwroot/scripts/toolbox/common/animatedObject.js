var animatedObject = {
    onLanguageChanged: function (scope, parameters, animatedText, getCurrentThreshold, IsStringContextTag, setTextToContextTag) {
        return scope.$on(onActiveLanguageChanged, function (e, value) {
            if (IsStringContextTag()) {
                setTextToContextTag();
                return;
            }
            if (animatedText) {
                var finalText = parameters.DefaultText;
                var currentThreshold = getCurrentThreshold();
                if (currentThreshold) {
                    finalText = currentThreshold.Text;
                    if (currentThreshold.SVGReferenceId) {
                        try {
                            var scopeReference = scope.dataValues[parameters.screenId][currentThreshold.SVGReferenceId];
                            if (scopeReference.bIsString)
                                finalText = scopeReference.invariantValue.toString();
                        }
                        catch { }
                    }
                }
                $(animatedText).text(i18next.t1(finalText));
            }
        });
    }
}