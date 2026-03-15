window.onload = init;
window.onbeforeunload = unload;
window.onresize = onzoomresize;
window.requestAnimationFrame = window.requestAnimationFrame || window.mozRequestAnimationFrame ||
                              window.webkitRequestAnimationFrame || window.msRequestAnimationFrame;

var bUseRequestAnimationFrame = requestAnimationFrame != null && requestAnimationFrame != undefined;

var DEBUG = false;

var ctx;
var ctxBackground;
var clientId;
var initialized = false;
var hashElements = new Dictionary();
var nErrors = 0;
var mouseOverElement = null;
var sendingCommandElement = null;
var bErrorOn = false;
var bResized = false;
var bRepaint = false;
var storageId = null;
var bSharingSession = false;
var bIsMobile = false;
var url;

var canvasOffscreen = document.createElement('canvas');
var ctxOffscreen;

var screenHub = $.connection.screenHub;

var vis = (function () {
    var stateKey, eventKey, keys = {
        hidden: "visibilitychange",
        webkitHidden: "webkitvisibilitychange",
        mozHidden: "mozvisibilitychange",
        msHidden: "msvisibilitychange"
    };
    for (stateKey in keys) {
        if (stateKey in document) {
            eventKey = keys[stateKey];
            break;
        }
    }
    return function (c) {
        if (c) document.addEventListener(eventKey, c);
        return !document[stateKey];
    }
})();

vis(function () {

    if (initialized) {
        if (vis())
        {
            screenHub.server.enableEvents(true);
        }
        else
            screenHub.server.enableEvents(false);
    }
});

//$.connection.hub.logging = true;

var timerSlowConnection;
$.connection.hub.connectionSlow(function () {
    console.log("Slow connection detected");
    LoadingPanel.SetText("Slow connection detected");
    LoadingPanel.Show();

    clearTimeout(timerSlowConnection);
    timerSlowConnection = setTimeout("slowConnectionTimeout()", 2000);
});

function slowConnectionTimeout() {
    clearTimeout(timerSlowConnection);
    LoadingPanel.Hide();
}

$.connection.hub.error(function (error) {
    console.log(error.message);
});

$.connection.hub.stateChanged(function (state) {
    if (state.newState == 0) {
        console.log("STATE: Connecting...");
        LoadingPanel.SetText("Connecting");
        LoadingPanel.Show();
    }
    else if (state.newState == 1) {
        console.log("STATE: Connected...");
        LoadingPanel.SetText("Connected");
        LoadingPanel.Hide();
    }
    else if (state.newState == 2) {
        console.log("STATE: Reconnecting...");
        LoadingPanel.SetText("Reconnecting...");
        LoadingPanel.Show();
    }
    else if (state.newState == 3) {
        console.log("STATE: Disconnected...");
        LoadingPanel.SetText("Disconnected...");
        LoadingPanel.Show();
    }
    else {
        console.log("STATE: Unknown");
        LoadingPanel.SetText("Unknown connection status");
        LoadingPanel.Show();
    }
});

function getURLParameter(name) {
    return decodeURIComponent((new RegExp('[?|&]' + name + '=' + '([^&;]+?)(&|#|;|$)').exec(location.search) || [, ""])[1].replace(/\+/g, '%20')) || null
}

function IsSafari() {

    var is_safari = navigator.userAgent.toLowerCase().indexOf('safari/') > -1;
    return is_safari;

}

function getParameterByName(name) {
    name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
    var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
        results = regex.exec(location.search);
    return results === null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
}

function init() {

    if (/Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent)) {
        bIsMobile = true;
    }

    LoadingPanel.Show();

	var url = getParameterByName("url");
	$.connection.hub.qs = { "url": url };

    var deferredStart = $.connection.hub.start();
    deferredStart.done(function () {
        console.log("My connection id is " + $.connection.hub.id);
        id = $.connection.hub.id;

        resizeCanvas();
        ctx = canvasMain.getContext("2d");
        ctxBackground = canvasBackground.getContext("2d");
        ctxOffscreen = canvasOffscreen.getContext("2d");

        if (!IsSafari()) {
            window.addEventListener("resize", OnWindowResize, false);
        }

        canvasMain.addEventListener("mousedown", oncanvasMainMouseDown, false);
        canvasMain.addEventListener("mouseup", oncanvasMainMouseUp, false);
        canvasMain.addEventListener("mousemove", oncanvasMainMouseMove, false);

        id = getURLParameter('id');
        storage = getURLParameter('s');
        w = getURLParameter('w');
        h = getURLParameter('h');
        if (id != null && storage != null) {
            var data = {
                clientId: id,
                storageId: storage,
                width: w,
                height: h
            };

            onSuccessRegistration(data);
            bSharingSession = true;
        }
        else {
            screenHub.server.register(canvasMain.width, canvasMain.height, bIsMobile, new Date().getTimezoneOffset()).done(onSuccessRegistration).fail(onFailureRegistration);
            // ScreenSinkService.Register(canvasMain.width, canvasMain.height, onSuccessRegistration, onFailureRegistration);
        }
    });
    deferredStart.fail(function () {
        // error
    });
}

function onzoomresize() {

    if (!initialized)
        return;
    screenHub.server.setZoomVisibilityItems(window.devicePixelRatio);
}

function resizeCanvas() {
    var bodyWidth = window.innerWidth;
    var bodyHeight = window.innerHeight;
    if (canvasMain.width > bodyWidth) {
        canvasContainer.width = canvasMain.width;
        canvasBackground.width = canvasMain.width;
        canvasOffscreen.width = canvasMain.width;
    }
    else {
        canvasContainer.width = bodyWidth;
        canvasBackground.width = bodyWidth;
        canvasOffscreen.width = bodyWidth;
    }
    if (canvasMain.height > bodyHeight) {
        canvasContainer.height = canvasMain.height;
        canvasBackground.height = canvasMain.height;
        canvasOffscreen.height = canvasMain.height;
    }
    else {
        canvasContainer.height = bodyHeight;
        canvasBackground.height = bodyHeight;
        canvasOffscreen.height = bodyHeight;
    }

    //canvasMain.width = bodyWidth;
    //canvasMain.height = bodyHeight;
    //canvasMain.width = mainDiv.clientWidth;
    //canvasMain.height = mainDiv.clientHeight;
    //canvasBackground.width = mainDiv.clientWidth;
    //canvasBackground.height = mainDiv.clientHeight;
}

function ElementData() {
    var image;
    var x;
    var y;
    var width;
    var height;
    var id;
    var imageData = null;
    var connected = null;
    var simulateEvent = null;
    var writable = null;
    var lastMessage = null;
    var dataType = 0;
    var imageLoaded = false;
    var isMouseOver = false;
    var isSendingCommand = false;
}

var timerRedrawAll;
function SetNeedRedraw() {
    if (bUseRequestAnimationFrame)
        bRepaint = true;

    //clearTimeout(timerRedrawAll);
    setTimeout("redrawAll()", 100);
}

function redrawAll() {
    if (bUseRequestAnimationFrame) {
        requestAnimationFrame(redrawAll);

        if (!bRepaint)
            return;
        bRepaint = false;
    }
    else {
        clearTimeout(timerRedrawAll);
    }

    ctxOffscreen.drawImage(canvasBackground, 0, 0);
    for (var i in hashElements.items) {
        if (hashElements.items[i].imageLoaded && hashElements.items[i].image != null) {

            if (!hashElements.items[i].connected) {
                ctxOffscreen.shadowColor = "rgb(255, 190, 190)";
                ctxOffscreen.shadowOffsetX = 0;
                ctxOffscreen.shadowOffsetY = 0;
                ctxOffscreen.shadowBlur = 50;
            }
            else {
                ctxOffscreen.shadowBlur = 0;
            }

            try
            {
                ctxOffscreen.drawImage(hashElements.items[i].image, hashElements.items[i].x, hashElements.items[i].y);
            }
            catch (e) {
            }

            if (hashElements.items[i].isSendingCommand) {
                ctxOffscreen.fillStyle = "rgba(127,127,127,0.5)";
                ctxOffscreen.fillRect(hashElements.items[i].x, hashElements.items[i].y, hashElements.items[i].width, hashElements.items[i].height);
            }
            else if (hashElements.items[i].isMouseOver) {
                ctxOffscreen.strokeStyle = "rgba(127,127,127,0.5)";
                ctxOffscreen.lineWidth = 2;
                ctxOffscreen.strokeRect(hashElements.items[i].x - 2, hashElements.items[i].y - 2, hashElements.items[i].width + 4, hashElements.items[i].height + 4);
            }
            //if (!hashElements.items[i].connected) {
            //    ctxOffscreen.fillStyle = "rgba(127,0,0,0.5)";
            //    ctxOffscreen.fillRect(hashElements.items[i].x - 2, hashElements.items[i].y - 2, hashElements.items[i].width + 4, hashElements.items[i].height + 4);
                if (hashElements.items[i].lastMessage!==null &&
                    typeof hashElements.items[i].lastMessage !== "undefined") 
                    ctxOffscreen.fillText(hashElements.items[i].lastMessage, hashElements.items[i].x + 2,
                        hashElements.items[i].y + 2);
            //}
        }
        //else {
        //    ctxOffscreen.strokeStyle = "rgba(0,0,0,0.5)";
        //    ctxOffscreen.lineWidth = 2;
        //    ctxOffscreen.strokeRect(hashElements.items[i].x, hashElements.items[i].y, hashElements.items[i].width, hashElements.items[i].height);
        //}
    }
    ctx.drawImage(canvasOffscreen, 0, 0);
}

function unload() {
    if (initialized) {
        initialized = false;
        // ScreenSinkService.Unregister(clientId, onSuccessUnregister, onFailedUnregister);
        //alert("your session will be logged out!");
        if (DEBUG) {
            clearInterval(timerTraceLog);
        }
        hashElements.clear();
    }
}

function onSuccessUnregister() {
    //LoadingPanel.SetText("Session Logged out");
    //LoadingPanel.Show();
}
function onFailedUnregister() {
}

function onSuccessRegistration(data) {


    clientId = data.id;
    storageId = data.storageid;

    //canvasContainer.width = data.width;
    //canvasContainer.height = data.height;
    canvasMain.width = data.width;
    canvasMain.height = data.height;
    resizeCanvas();
    //canvasBackground.width = data.width;
    //canvasBackground.height = data.height;

    initialized = true;

    LoadingPanel.SetText("Querying data...");

    screenHub.server.setZoomVisibilityItems(window.devicePixelRatio);

    screenHub.server.getElementData().done(onSuccessGetElementData).fail(onFailureGetElementData);
    // ScreenSinkService.GetElementData(clientId, onSuccessGetElementData, onFailureGetElementData);
    DrawOrGetBackground(false);
    if (bUseRequestAnimationFrame) {
        redrawAll();
    }

    if (DEBUG) {
        timerTraceLog = setInterval("TraceLog()", 5000);
    }
}

var timerTraceLog;
function TraceLog() {
    console.log("memory usage : current = " + window.performance.memory.usedJSHeapSize + ", total = " + window.performance.memory.totalJSHeapSize);
}

function DrawOrGetBackground(refresh) {

    ctxBackground.clearRect(0, 0, canvasBackground.width, canvasBackground.height);
    ctxBackground.beginPath();
    ctxBackground.rect(0, 0, canvasBackground.width, canvasBackground.height);
    ctxBackground.fillStyle = 'transparent';
    ctxBackground.fill();

    try{
        var elemenData = localStorage[storageId];
        if (elemenData === null || typeof(elemenData) === 'undefined') {
            screenHub.server.getBackground().done(onSuccessGetBackground).fail(onFailureGetBackground);
            // ScreenSinkService.GetBackground(clientId, onSuccessGetBackground, onFailureGetBackground);
            return;
        }
    }
    catch (e) {
        screenHub.server.getBackground().done(onSuccessGetBackground).fail(onFailureGetBackground);
        // ScreenSinkService.GetBackground(clientId, onSuccessGetBackground, onFailureGetBackground);
        return;
    }

    var img = new Image();
    img.onload = function () {
        ctxBackground.drawImage(img, 0, 0);
    };
    img.src = elemenData;

    if (refresh === true)
        screenHub.server.getBackground().done(onSuccessGetBackground).fail(onFailureGetBackground);
        // ScreenSinkService.GetBackground(clientId, onSuccessGetBackground, onFailureGetBackground);
}

function onSuccessGetBackground(elementData) {

    if (elementData === null || typeof (elementData) === 'undefined')
        return;

    try
    {
        localStorage[storageId] = elementData.imageData;
    }
    catch (e) {
        writetostatus("Error saving data to the local storage:" + e.message);
        localStorage.clear();
    }

    var img = new Image();
    img.onload = function () {
        ctxBackground.drawImage(img, 0, 0);
    };
    img.src = elementData.imageData;
}

function onFailureGetBackground() {
    writetostatus("Error getting the background: no response from service!!");
}


function onFailureRegistration() {
    LoadingPanel.Hide();
    writetostatus("Error registering: no response from service!!");
}

function onSuccessGetElementData(dataList) {
    LoadingPanel.Hide();

    try {
        // for (var i = 0; i < dataList.length; i++) {
        $.each(dataList, function () {
            var elementData = new ElementData();
            elementData.x = this.left;
            elementData.y = this.top;
            elementData.id = this.id;
            elementData.width = this.width;
            elementData.height = this.height;
            elementData.connected = this.connected;
            elementData.simulateEvent = this.simulateEvent;
            elementData.writable = this.writable;
            elementData.lastMessage = this.LastMessage;
            elementData.dataType = this.dataType;

            hashElements.setItem(this.id, elementData);

            try {
                var imageData = localStorage[storageId + this.id];
                if (imageData === null || typeof (imageData) === 'undefined') {
                    screenHub.server.getImageBase64(this.id).done(onSuccessGetImageBase64).fail(onFailureGetImageBase64);
                    // ScreenSinkService.GetImageBase64(clientId, dataList[i].id, onSuccessGetImageBase64, onFailureGetImageBase64);
                }
                else {
                    var image = new Image();
                    image.onload = function () {
                        elementData.image = image;
                        elementData.imageLoaded = true;
                        SetNeedRedraw();
                    };
                    image.src = imageData;
                    screenHub.server.getImageBase64(this.id).done(onSuccessGetImageBase64).fail(onFailureGetImageBase64);
                    // ScreenSinkService.GetImageBase64(clientId, dataList[i].id, onSuccessGetImageBase64, onFailureGetImageBase64);
                }
            }
            catch (e) {
                screenHub.server.getImageBase64(this.id).done(onSuccessGetImageBase64).fail(onFailureGetImageBase64);
                // ScreenSinkService.GetImageBase64(clientId, dataList[i].id, onSuccessGetImageBase64, onFailureGetImageBase64);
            }
        });
    }
    catch (e) {
        writetostatus("Error onSuccessGetElementData:" + e.message);
    }

    // SetPendingPollings();
}

//var timer;
//function SetPendingPollings() {
//    if (!initialized)
//        return;

//    //clearTimeout(timer);
//    timer = setTimeout("refresh()", 100);
//}

var timerCommands;
function SetPendingPollingCommands() {
    if (!initialized)
        return;

    //clearTimeout(timerCommands);
    timerCommands = setTimeout("refreshCommand()", 1000);
}

function refreshCommand() {
    if (!initialized)
        return;

    clearTimeout(timerCommands);
    PollingPendingCommands();
    LoadingPanel.Hide();
}

/*
function refresh() {

    if (!initialized)
        return;

    clearTimeout(timer);

    if (bResized) {
        bResized = false;
        DrawOrGetBackground(true);
        ScreenSinkService.GetBackground(clientId, onSuccessGetBackground, onFailureGetBackground);
    }

    PollingChanges();
    PollingStatusChanges();
}
*/

function onSuccessGetImageBase64(elementData) {
    if (elementData != null && hashElements.hasItem(elementData.id)) {

        try {
            if (localStorage[storageId + elementData.id] == elementData.imageData)
                return;
        }
        catch (e) {
        }

        hashElements.items[elementData.id].imageLoaded = false;

        if (elementData.imageData === null || typeof (elementData.imageData) === 'undefined') {
            SetNeedRedraw();
        }
        else {
            var image = new Image();
            image.onload = function () {
                hashElements.items[elementData.id].image = image;
                hashElements.items[elementData.id].imageLoaded = true;
                SetNeedRedraw();
            };
            image.src = elementData.imageData;
        }

        try {
            localStorage[storageId + elementData.id] = elementData.imageData;
        }
        catch (e) {
            writetostatus("Error saving data to the local storage:" + e.message);
            localStorage.clear();
        }
    }
}

function onFailureGetImageBase64() {
    writetostatus("Error GetImageBase64: no response from service!!");
}

function onFailureGetElementData() {
    LoadingPanel.Hide();
    writetostatus("Error getting element data: no response from service!!");
}



function OnWindowResize(e) {
    if (!initialized)
        return;
    /*
    LoadingPanel.SetText("Resizing...");
    LoadingPanel.Show();

    if (typeof e == 'undefined')
        e = window.event;



    LoadingPanel.Hide();
    */
    resizeCanvas();
    DrawOrGetBackground(false);
}

function oncanvasMainMouseDown(e) {
    if (!initialized)
        return;

    //inside my mouse events handler:
    var x = (e.offsetX !== undefined) ? e.offsetX : (e.layerX - e.target.offsetLeft);
    var y = (e.offsetY !== undefined) ? e.offsetY : (e.layerY - e.target.offsetTop);

    x -= canvasMain.offsetLeft;
    y -= canvasMain.offsetTop;

    screenHub.server.sendMouseDownEvent(Math.round(x), Math.round(y));
    // ScreenSinkService.SendMouseDownEvent(clientId, x, y);
}

function oncanvasMainMouseUp(e) {
    if (!initialized)
        return;

    //inside my mouse events handler:
    var x = (e.offsetX !== undefined) ? e.offsetX : (e.layerX - e.target.offsetLeft);
    var y = (e.offsetY !== undefined) ? e.offsetY : (e.layerY - e.target.offsetTop);

//    alert(String.format("event x {0} y {1}, ClientX {2} ClientY {3}, OffsetX {4} OffsetY {5}",
//                    x, y, event.clientX, event.clientY, event.offsetX, event.offsetY));

    if (e.shiftKey && e.altKey && e.ctrlKey) {
        localStorage.clear();
        writetostatus("localStorage has been cleared");
    }

    if (mouseOverElement === null || !mouseOverElement.writable || !mouseOverElement.connected) {
        if (mouseOverElement === null)
            screenHub.server.sendMouseUpEvent(Math.round(x), Math.round(y)).done(onSuccessMouseUp).fail(onFailureMouseUp);
            // ScreenSinkService.SendMouseUpEvent(clientId, x, y, onSuccessMouseUp, onFailureMouseUp);
        return;
    }

    x -= canvasMain.offsetLeft;
    y -= canvasMain.offsetTop;

    // alert(String.format("sending x {0} y {1}", x, y));

    if (mouseOverElement.simulateEvent && (mouseOverElement.dataType === -1 || mouseOverElement.dataType === 0))
    {
        sendingCommandElement = mouseOverElement;
        sendingCommandElement.isSendingCommand = true;
        SetNeedRedraw();

        LoadingPanel.SetText("Sending Command...");
        LoadingPanel.Show();

        screenHub.server.sendMouseUpEvent(Math.round(x), Math.round(y)).done(onSuccessMouseUp).fail(onFailureMouseUp);
        // ScreenSinkService.SendMouseUpEvent(clientId, x, y, onSuccessMouseUp, onFailureMouseUp);
    }
    else {
        LoadingPanel.SetText("Querying data info...");
        LoadingPanel.Show();

        screenHub.server.sendQueryDataInfo(mouseOverElement.id).done(onQueryDataInfo).fail(onFailureQueryDataInfo);
        // ScreenSinkService.SendQueryDataInfo(clientId, mouseOverElement.id, onQueryDataInfo, onFailureQueryDataInfo);
    }
}

function oncanvasMainMouseMove(e) {
    if (!initialized)
        return;

    //inside my mouse events handler:
    var x = (e.offsetX !== undefined) ? e.offsetX : (e.layerX - e.target.offsetLeft);
    var y = (e.offsetY !== undefined) ? e.offsetY : (e.layerY - e.target.offsetTop);

    //    alert(String.format("event x {0} y {1}, ClientX {2} ClientY {3}, OffsetX {4} OffsetY {5}",
    //                    x, y, event.clientX, event.clientY, event.offsetX, event.offsetY));

    x -= canvasMain.offsetLeft;
    y -= canvasMain.offsetTop;

    // alert(String.format("sending x {0} y {1}", x, y));
    var found = false;
    for (var i in hashElements.items) {
        if (x > hashElements.items[i].x && y > hashElements.items[i].y &&
            x < hashElements.items[i].x + hashElements.items[i].width &&
            y < hashElements.items[i].y + hashElements.items[i].height) {

            if (hashElements.items[i].writable) {
                hashElements.items[i].isMouseOver = true;
                if (mouseOverElement !== null && typeof(mouseOverElement) !== 'undefined' && 
                    mouseOverElement != hashElements.items[i])
                    mouseOverElement.isMouseOver = false;
                mouseOverElement = hashElements.items[i];
                SetNeedRedraw();
                found = true;
            }
        }
        else if (hashElements.items[i].isMouseOver == true) {
            hashElements.items[i].isMouseOver = false;
            SetNeedRedraw();
        }
    }
    if (!found)
        mouseOverElement = null;
}

function onQueryDataInfo(elementInfo) {
    LoadingPanel.Hide();
    if (elementInfo == null)
        return;

    if (mouseOverElement.dataType == 0) {
        // boolean value
        if (elementInfo.selection != null) {

            editList.ClearItems();

            for (var i in elementInfo.selection) {
                //$.each(elementInfo.selection, function () {
                editList.AddItem(elementInfo.selection[i]);
            }

            editingValueProvider = true;
            editingValueProviderX = elementInfo.X;
            editingValueProviderY = elementInfo.Y;

            editListValue.Show();
        }
        else {
            if (elementInfo.value == "True")
                editBool.SetValue(true);
            else
                editBool.SetValue(false);
            editBoolValue.Show();
        }
    }
    else if (mouseOverElement.dataType == 1) {
        // numeric value

        if (elementInfo.selection != null) {

            editList.ClearItems();

            for (var i in elementInfo.selection) {
                //$.each(elementInfo.selection, function () {
                editList.AddItem(elementInfo.selection[i]);
            }

            editingValueProvider = true;
            editingValueProviderX = elementInfo.X;
            editingValueProviderY = elementInfo.Y;

            editListValue.Show();
        }
        else {
            var dValue = parseFloat(elementInfo.value.replace(",", "."));
            //sliderValue.SetMinValue(elementInfo.minValue);
            //sliderValue.SetMaxValue(elementInfo.maxValue);
            sliderValue.SetValue(dValue);

            editSpin.SetMinValue(elementInfo.minValue);
            editSpin.SetMaxValue(elementInfo.maxValue);
            editSpin.SetValue(dValue);

            editValue.Show();
        }
    }
    else if (mouseOverElement.dataType == 2) {
        // string value

        if (elementInfo.selection != null) {

            editList.ClearItems();

            for (var i in elementInfo.selection) {
            //$.each(elementInfo.selection, function () {
                editList.AddItem(elementInfo.selection[i]);
            }

            editingValueProvider = true;
            editingValueProviderX = elementInfo.X;
            editingValueProviderY = elementInfo.Y;

            editListValue.Show();
        }
        else {
            editString.SetValue(elementInfo.value);

            editStringValue.Show();
        }
    }
    else {
        // unsupported datatype
        writetostatus("unsupported datatype");
    }
}

function OnValueChanged(s, e) {
    sliderValue.SetValue(editSpin.GetValue());
}

function OnPositionChanged(s, e) {
    editSpin.SetValue(sliderValue.GetValue());
}

function ValidateSelectionValue() {
    editListValue.Hide();
    LoadingPanel.SetText("Sending Data Value...");
    LoadingPanel.Show();

    try
    {
        var value = editList.GetValue();
        if (editingValueProvider) {
            editingValueProvider = false;
            screenHub.server.sendDataValueProvider(editingValueProviderX, editingValueProviderY, value).done(onSendDataValueProvider).fail(onFailureSendDataValue);
            // ScreenSinkService.SendDataValueProvider(clientId, editingValueProviderX, editingValueProviderY, value, onSendDataValueProvider, onFailureSendDataValue);
        }
        else
            screenHub.server.sendDataValue(mouseOverElement.id, value).done(onSendDataValue).fail(onFailureSendDataValue);
        // ScreenSinkService.SendDataValue(clientId, mouseOverElement.id, value, onSendDataValue, onFailureSendDataValue);
    }
    catch (e) {
        LoadingPanel.Hide();
    }
}

function ValidateNumericValue() {
    editValue.Hide();
    LoadingPanel.SetText("Sending Data Value...");
    LoadingPanel.Show();

    try
    {
        var value = editSpin.GetValue();
        screenHub.server.sendDataValue(mouseOverElement.id, value).done(onSendDataValue).fail(onFailureSendDataValue);
        // ScreenSinkService.SendDataValue(clientId, mouseOverElement.id, value, onSendDataValue, onFailureSendDataValue);
    }
    catch (e) {
        LoadingPanel.Hide();
    }
}

function ValidateStringValue() {
    editStringValue.Hide();
    LoadingPanel.SetText("Sending Data Value...");
    LoadingPanel.Show();

    try {
        var value = editString.GetValue();
        if (editingValueProvider)
        {
            editingValueProvider = false;
            screenHub.server.sendDataValueProvider(editingValueProviderX, editingValueProviderY, value).done(onSendDataValueProvider).fail(onFailureSendDataValue);
            // ScreenSinkService.SendDataValueProvider(clientId, editingValueProviderX, editingValueProviderY, value, onSendDataValueProvider, onFailureSendDataValue);
        }
        else
            screenHub.server.sendDataValue(mouseOverElement.id, value).done(onSendDataValue).fail(onFailureSendDataValue);
            // ScreenSinkService.SendDataValue(clientId, mouseOverElement.id, value, onSendDataValue, onFailureSendDataValue);
    }
    catch (e) {
        LoadingPanel.Hide();
    }
}

function ValidateBoolValue() {
    editBoolValue.Hide();
    LoadingPanel.SetText("Sending Data Value...");
    LoadingPanel.Show();

    try {
        var value = editBool.GetValue();
        screenHub.server.sendDataValue(mouseOverElement.id, value).done(onSendDataValue).fail(onFailureSendDataValue);
        // ScreenSinkService.SendDataValue(clientId, mouseOverElement.id, value, onSendDataValue, onFailureSendDataValue);
    }
    catch (e) {
        LoadingPanel.Hide();
    }
}

function onSendDataValue(success) {
    LoadingPanel.Hide();
    if (success != "OK")
        alert(success);
}

function onSendDataValueProvider(success) {
    LoadingPanel.Hide();
    if (success !== null && success !== 'undefined' && success != "")
        alert(success);
}

function onFailureSendDataValue() {
    LoadingPanel.Hide();
    writetostatus("Error Sending data value: no response from service!!");
}

function onFailureQueryDataInfo() {
    LoadingPanel.Hide();
    writetostatus("Error Querying data info: no response from service!!");
}

var repostPollingCommand = false;
var editingValueProvider = false;
var editingValueProviderX;
var editingValueProviderY;
function onSuccessMouseUp(elementInfo) {
    LoadingPanel.Hide();
    if (elementInfo !== null && typeof (elementInfo) !== 'undefined') {
        editingValueProvider = true;
        editingValueProviderX = elementInfo.X;
        editingValueProviderY = elementInfo.Y;

        if (elementInfo.selection != null) {

            editList.ClearItems();
            for (var i in elementInfo.selection) {
                //$.each(elementInfo.selection, function () {
                editList.AddItem(elementInfo.selection[i]);
            }

            editListValue.Show();
        }
        else {
            editString.SetValue(elementInfo.value);
            editStringValue.Show();
        }

        if (sendingCommandElement !== null && typeof (sendingCommandElement) !== 'undefined') {
            editingValueProvider = false;
            sendingCommandElement.isSendingCommand = false;
            sendingCommandElement = null;
        }
    }
    else if (sendingCommandElement !== null && typeof (sendingCommandElement) !== 'undefined') {
        editingValueProvider = false;
        sendingCommandElement.isSendingCommand = false;
        sendingCommandElement = null;

        LoadingPanel.SetText("Checking command...");
        LoadingPanel.Show();

        PollingPendingCommands();
        repostPollingCommand = true;
    }
    else
        editingValueProvider = false;

    SetNeedRedraw();
}

function onFailureMouseUp() {
    LoadingPanel.Hide();
    writetostatus("Error Sending Command: no response from service!!");
    if (sendingCommandElement !== null && typeof (sendingCommandElement) !== 'undefined') {
        sendingCommandElement.isSendingCommand = false;
        sendingCommandElement = null;
    }
    SetNeedRedraw();
}

/*
var pollingChanges = false;
var timerPollingChanges;
function PollingChanges() {
    clearTimeout(timerPollingChanges);

    if (!pollingChanges) {
        pollingChanges = true;
        ScreenSinkService.GetListChanges(clientId, onSuccessPollingChanges, onFailurePollingChanges);
    }
    else
    {
        timerPollingChanges = setTimeout("PollingChanges()", 500);
    }
}

var pollingStatusChanges = false;
var timerPollingStatusChanges;
function PollingStatusChanges() {
    clearTimeout(timerPollingStatusChanges);

    if (!pollingStatusChanges) {
        pollingStatusChanges = true;
        ScreenSinkService.GetListStatusChanges(clientId, onSuccessPollingStatusChanges, onFailurePollingStatusChanges);
    }
    else
    {
        timerPollingStatusChanges = setTimeout("PollingStatusChanges()", 500);
    }
}
*/

var pollingPendingCommands = false;
var timerPollingPendingCommands;
function PollingPendingCommands() {
    clearTimeout(timerPollingPendingCommands);

    if (!pollingPendingCommands) {
        pollingPendingCommands = true;
        screenHub.server.getListCommands(url).done(onSuccessPollingPendingCommands).fail(onFailurePollingPendingCommands);
        // ScreenSinkService.GetListCommands(clientId, url, onSuccessPollingPendingCommands, onFailurePollingPendingCommands);
    }
    else
    {
        timerPollingPendingCommands = setTimeout("PollingPendingCommands()", 100);
    }
}

function closeReturnCurrentPopup() {
    var parentWindow = window.parent;
    if (parentWindow == null) {
        window.location = "Default.aspx";
        return;
    }
    if (!popupScreen.IsVisible())
        window.location = "Default.aspx";
    CloseCurrentPopupScreen();
    parentWindow.SelectAndClosePopup();

}

function onSuccessPollingPendingCommands(elementChanged) {
    clearTimeout(timerPollingPendingCommands);
    pollingPendingCommands = false;
    if (elementChanged != null) {
        // for (var i = 0; i < elementChanged.length; i++) {
        $.each(elementChanged, function () {
            if (this.closeCurrent)
            {
                closeReturnCurrentPopup();
            }
            else if (this.isSynchro && !bIsMobile) {
                currentPopupScreen = this.url;
                var commandurl = "PopupScreen.aspx?url=" + this.url;
                //var commandurl = "PopupScreen.aspx?url=" + this.url + "&id=" + clientId +
                //    "&s=" + storageId + "&w=" + this.width + "&h=" + this.height;
                popupScreen.SetContentUrl(commandurl);
                LoadingPanel.Hide();
                popupScreen.SetWidth(this.width + 100);
                popupScreen.SetHeight(this.height + 100);

                popupScreen.Show();
                popupScreen.SetHeaderText = this.url;
            }
            else {
                LoadingPanel.SetText("Opening new screen...");
                LoadingPanel.Show();
                url = this.url;
                screenHub.server.openUri(this.url, canvasMain.width, canvasMain.height, new Date().getTimezoneOffset()).done(onSuccessOpenUri).fail(onFailureOpenUri);
                // ScreenSinkService.OpenUri(clientId, elementChanged[i].url, canvasMain.width, canvasMain.height, onSuccessOpenUri, onFailureOpenUri);
            }
            return;
        });

        if (repostPollingCommand) {
            repostPollingCommand = false;
            SetPendingPollingCommands();
        }
    }
    else {

        if (repostPollingCommand) {
            repostPollingCommand = false;
            SetPendingPollingCommands();
        }
    }
}

function onFailurePollingPendingCommands() {
    clearTimeout(timerPollingPendingCommands);
    pollingPendingCommands = false;
    LoadingPanel.Hide();
}

/*
function onSuccessPollingStatusChanges(dataList) {
    pollingStatusChanges = false;
    try {
        if (dataList != null) {
            for (var i = 0; i < dataList.length; i++) {

                if (hashElements.hasItem(dataList[i].id)) {

                    var element = hashElements.items[dataList[i].id];
                    element.x = dataList[i].left;
                    element.y = dataList[i].top;
                    element.width = dataList[i].width;
                    element.height = dataList[i].height;
                    element.connected = dataList[i].connected;
                    element.writable = dataList[i].writable;
                    element.simulateEvent = dataList[i].simulateEvent;
                    element.lastMessage = dataList[i].LastMessage;
                    element.dataType = dataList[i].dataType;

                    SetNeedRedraw();
                }
            }
        }
    }
    catch (e) {
        writetostatus("Error onSuccessGetElementData:" + e.message);
    }

    SetPendingPollings();
}

function onFailurePollingStatusChanges() {
    pollingStatusChanges = false;
    SetPendingPollings();
}

function onSuccessPollingChanges(elementChanged) {
    pollingChanges = false;
    if (elementChanged != null) {
        for (var i = 0; i < elementChanged.length; i++) {

            if (hashElements.hasItem(elementChanged[i])) {
                ScreenSinkService.GetImageBase64(clientId, elementChanged[i], onSuccessGetImageBase64, onFailureGetImageBase64);
            }
        }
    }

    SetPendingPollings();

    if (bErrorOn) {
        LoadingPanel.Hide();
        bErrorOn = false;
    }
}

function onFailurePollingChanges() {
    pollingChanges = false;
    SetPendingPollings();
    if (++nErrors > 5 && !bErrorOn) {
        bErrorOn = true;
        LoadingPanel.SetText("Communication lost, trying to restore...");
        LoadingPanel.Show();
    }
}
*/

screenHub.client.pendingStatusChanges = function (dataList) {
    try {
        if (dataList != null) {
            // for (var i = 0; i < dataList.length; i++) {
            $.each(dataList, function () {

                if (hashElements.hasItem(this.id)) {

                    var element = hashElements.items[this.id];
                    element.x = this.left;
                    element.y = this.top;
                    element.width = this.width;
                    element.height = this.height;
                    element.connected = this.connected;
                    element.writable = this.writable;
                    element.simulateEvent = this.simulateEvent;
                    element.lastMessage = this.LastMessage;
                    element.dataType = this.dataType;

                    SetNeedRedraw();
                }
            });
        }
    }
    catch (e) {
        writetostatus("Error onSuccessGetElementData:" + e.message);
    }

    // SetPendingPollings();
}

screenHub.client.pendingChanges = function (elementChanged) {

    if (elementChanged != null) {
        // for (var i = 0; i < elementChanged.length; i++) {
        $.each(elementChanged, function () {
            if (hashElements.hasItem(this)) {
                screenHub.server.getImageBase64(this).done(onSuccessGetImageBase64).fail(onFailureGetImageBase64);
                // ScreenSinkService.GetImageBase64(clientId, this, onSuccessGetImageBase64, onFailureGetImageBase64);
            }
        });
    }

    // SetPendingPollings();

    if (bErrorOn)
    {
        LoadingPanel.Hide();
        bErrorOn = false;
    }
}

function onSuccessOpenUri(size) {

    if (size === null) {

        alert("An error occured opening the screen or screen not found !");
        LoadingPanel.SetText("Opening Main screen...");
        LoadingPanel.Show();
        screenHub.server.getListCommands(url).done(onSuccessPollingPendingCommands).fail(onFailurePollingPendingCommands);
        // ScreenSinkService.OpenUri(clientId, null, canvasMain.width, canvasMain.height, onSuccessOpenUri, onFailureOpenUri);
        return;
    }

    LoadingPanel.SetText("Querying data...");

    //canvasContainer.width = size.Width;
    //canvasContainer.height = size.Height;
    canvasMain.width = size.width;
    canvasMain.height = size.height;
    resizeCanvas();
    //canvasBackground.width = size.Width;
    //canvasBackground.height = size.Height;

    storageId = size.storageid;

    hashElements = new Dictionary();
    ctxBackground.clearRect(0, 0, canvasBackground.width, canvasBackground.height);
    ctx.clearRect(0, 0, canvasMain.width, canvasMain.height);
    ctxOffscreen.clearRect(0, 0, canvasOffscreen.width, canvasOffscreen.height);

    screenHub.server.setZoomVisibilityItems(window.devicePixelRatio);

    screenHub.server.getElementData().done(onSuccessGetElementData).fail(onFailureGetElementData);
    // ScreenSinkService.GetElementData(clientId, onSuccessGetElementData, onFailureGetElementData);
    DrawOrGetBackground(false);
}

function onFailureOpenUri() {
}

var currentPopupScreen;
function CloseCurrentPopupScreen() {
    if (bSharingSession) {
        screenHub.server.closeUri(currentPopupScreen);
        // ScreenSinkService.CloseUri(clientId, currentPopupScreen);
    }
    else {
        if (initialized) {
            initialized = false;
            // ScreenSinkService.Unregister(clientId, onSuccessUnregister, onFailedUnregister);
            //alert("your session will be logged out!");
        }
    }
}

function writetostatus(input) {
    window.status = input
    return true
}

function circle(x, y, r) {
    ctx.beginPath();
    ctx.arc(x, y, r, 0, Math.PI * 2, true);
    ctx.fill();
}



function Dictionary() {
    this.length = 0;
    this.items = new Array();
    for (var i = 0; i < arguments.length; i += 2) {
        if (typeof (arguments[i + 1]) != 'undefined') {
            this.items[arguments[i]] = arguments[i + 1];
            this.length++;
        }
    }
    this.removeItem = function (in_key) {
        var tmp_value;
        if (typeof (this.items[in_key]) != 'undefined') {
            this.length--;
            var tmp_value = this.items[in_key];
            delete this.items[in_key];
        }
        return tmp_value;
    }
    this.getItem = function (in_key) {
        return this.items[in_key];
    }
    this.setItem = function (in_key, in_value) {
        if (typeof (in_value) != 'undefined') {
            if (typeof (this.items[in_key]) == 'undefined') {
                this.length++;
            }
            this.items[in_key] = in_value;
        }
        return in_value;
    }
    this.hasItem = function (in_key) {
        return typeof (this.items[in_key]) != 'undefined';
    }

    this.clear = function () {
        for (var i in this.items) {
            delete this.items[i];
        }

        this.length = 0;
    }
} 


