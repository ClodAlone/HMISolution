
function showNumericPad(scope, min, max, ispassword, defaultInvariantValue, decimalDigits) {
    var bHasDecimalDigits = !notNumeric(decimalDigits) && decimalDigits > 0;
    var decimalSeparator = getDecimalSeparator(scope);
    var id = 'numericid';
    var iDiv = jQuery('<input/>', {
        type: ispassword ? 'password' : 'text',
        id: id
    }).appendTo(ROOT);

    function keyboardPrompt(message, defaultInvariantValue) {
        var localValue = mathHelpers.roundToSignificantDigits(defaultInvariantValue, decimalDigits);
        if (bHasDecimalDigits) {
            localValue = localValue.toString().replace(/\.([0-9]+)$/, decimalSeparator + "$1");
        }

        iDiv.show().val(localValue).attr('placeholder', message).getkeyboard().reveal();
        return new Promise(accept => iDiv.on('accepted canceled', e => {
            iDiv.remove();
            accept(e.type === 'accepted' ? getInvariantValue(iDiv.val()) : null);
        }));
    }

    function getInvariantValue(finalValue) {
        finalValue = mathHelpers.roundToSignificantDigits(parseFloat(finalValue.replace(decimalSeparator, invariantNumberDecimalSeparator)), decimalDigits);

        return finalValue;
    }

    iDiv.keyboard({
        openOn: null,
        stayOpen: true,
        layout: 'num',
        keyBinding: 'mouseup touchend',
        validate: function (keyboard, value, isClosing) {
            if (!isClosing)
                return true;

            value = getInvariantValue(value);

            var v = parseFloat(value);
            if (isNaN(v)) {

                alert(i18next.t('EnterValidValue') + min);
                return false;
            }

            if (decimalDigits == 0)
                v = Math.round(v);

            if (min) {
                if (v < parseFloat(min)) {

                    alert(i18next.t('MinValueMustBeGreater') + min);
                    return false;
                }
            }
            if (max) {
                if (v > parseFloat(max)) {

                    alert(i18next.t('MaxValueMustBeLesser') + max);
                    return false;
                }
            }

            return true;
        }
    }).addTyping();

    return keyboardPrompt('', defaultInvariantValue);
}
