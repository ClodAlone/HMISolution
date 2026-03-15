function showAlphanumericPad(scope, ispassword, defaultValue, max) {

    var id = 'alphanumericid';
    var iDiv = jQuery('<input/>', {
        type: ispassword ? 'password' : 'text',
        id: id
    }).appendTo(ROOT);

    function keyboardPrompt(message, defaultValue) {
        iDiv.show().val(defaultValue).attr('placeholder', message).getkeyboard().reveal();
        return new Promise(accept => iDiv.on('accepted canceled', e => {
            iDiv.remove();
            accept(e.type === 'accepted' ? iDiv.val() : null);
        }));
    }

    var maxchars = parseFloat(max);
    if (isNaN(maxchars) || maxchars == 0)
        maxchars = false;

    iDiv.keyboard({
        openOn: null,
        stayOpen: true,
        maxLength: maxchars,
        layout: 'qwerty',
        keyBinding: 'mouseup touchend'
    }).addTyping();

    return keyboardPrompt('', defaultValue);
}
