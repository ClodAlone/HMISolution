var connection = new signalR.HubConnectionBuilder()
    .withUrl("/dataHub")
    .withHubProtocol(new signalR.protocols.msgpack.MessagePackHubProtocol())
    .build();

//connection.on("ReceiveMessage", function (user, message) {
//});

async function startConnection() {
    try {
        await connection.start();
        console.log("SignalR Connected.");
        loopCall();
    } catch (err) {
        console.log(err);
        setTimeout(startConnection, 5000);
    }
};

function loopCall() {
    console.log("Invoking GetData.");
    connection.invoke("GetData")
        .then((res) =>
        {
            console.log("GetData processed.");
            loopCall();
        })
        .catch(err =>
        {
            console.log(err);
        });
};

connection.onclose(async () => {
    await start();
});

// Start the connection.
startConnection();

//document.getElementById("sendButton").addEventListener("click", function (event) {
//    var user = document.getElementById("userInput").value;
//    var message = document.getElementById("messageInput").value;
//    connection.invoke("SendMessage", user, message).catch(function (err) {
//        return console.error(err.toString());
//    });
//    event.preventDefault();
//});