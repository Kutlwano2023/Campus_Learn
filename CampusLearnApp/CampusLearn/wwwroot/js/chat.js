// chat.js - requires SignalR client library (add <script src="~/lib/microsoft/signalr/dist/browser/signalr.js"></script>)
// Usage: Chat.connect(conversationId, currentUserId, currentUserName, onNewMessageCallback)
var Chat = (function () {
    let connection = null;
    return {
        connect: function (conversationId, currentUserId, currentUserName, receiveCallback) {
            connection = new signalR.HubConnectionBuilder()
                .withUrl("/chathub")
                .withAutomaticReconnect()
                .build();

            connection.on("ReceiveMessage", function (message) {
                if (receiveCallback) receiveCallback(message);
            });

            connection.start().then(function () {
                console.log("SignalR connected");
                return connection.invoke("JoinConversation", conversationId);
            }).catch(function (err) {
                console.error(err.toString());
            });

            return connection;
        },
        sendMessage: function (conversationId, senderId, senderName, text) {
            if (!connection) {
                console.error("Connection not established");
                return;
            }
            return connection.invoke("SendMessage", conversationId, senderId, senderName, text);
        },
        leave: function (conversationId) {
            if (!connection) return;
            return connection.invoke("LeaveConversation", conversationId).then(() => connection.stop());
        }
    };
})();
