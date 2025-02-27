const WebSocket = require('ws');

const port = process.env.PORT || 3000;
const server = new WebSocket.Server({ port });

console.log(`WebSocket сервер запущен на порту ${port}`);

let clients = [];

server.on('connection', (ws) => {
    console.log('Клиент подключился');

    clients.push({
        client: ws,
        unique_identity: null,
        room_name: null,
        username: null
    });

    const getClient = () => clients.find(clientData => clientData.client === ws);

    const getAnotherPlayer = (roomName) => {
        return clients.find(clientFromList => clientFromList.room_name === roomName && clientFromList.client !== ws) || null;
    };

    ws.on('message', (message) => {
        try {
            const client = getClient();
            if (!client) return;

            const response = JSON.parse(message);
            console.log("Получено сообщение:", response);

            switch (response.message_type) {
                case "InitialJoinMessage":
                    console.log("Обработка InitialJoinMessage");
                    client.unique_identity = response.unique_identity;
                    client.room_name = response.room_name;
                    client.username = response.username;

                    const anotherPlayer = getAnotherPlayer(response.room_name);
                    console.log("Другой игрок в комнате:", anotherPlayer ? anotherPlayer.username : "не найден");

                    const randomNumber = Math.floor(Math.random() * 2) + 1;
                    const forAnotherPlayerMessage = {
                        message_type: "PlayerConnected",
                        player_username: client.username,
                        turn: randomNumber === 1
                    };
                    const forCurrentClient = {
                        message_type: "ConnectionCompleted",
                        turn: randomNumber === 2
                    };

                    if (anotherPlayer) {
                        anotherPlayer.client.send(JSON.stringify(forAnotherPlayerMessage));
                    }
                    client.client.send(JSON.stringify(forCurrentClient));
                    break;

                case "InitialCreateMessage":
                    client.unique_identity = response.unique_identity;
                    client.room_name = response.room_name;
                    client.username = response.username;
                    console.log("Обработка InitialCreateMessage:", client.username);
                    break;

                case "ReconnectMessage":
                    console.log(client.username, "переподключается");
                    const anotherPlayerReconnect = getAnotherPlayer(client.room_name);
                    if (anotherPlayerReconnect) {
                        anotherPlayerReconnect.client.send(JSON.stringify({
                            message_type: "PlayerReconnected"
                        }));
                    }
                    break;

                case "PlayerDisconnected":
                    console.log(client.username, "отключился");
                    break;

                case "PlayerMove":
                    const anotherPlayerMove = getAnotherPlayer(client.room_name);
                    if (anotherPlayerMove) {
                        const moveMessage = {
                            message_type: "PlayerMoved",
                            x: response.x,
                            y: response.y
                        };
                        console.log("Отправка хода игроку:", anotherPlayerMove.username);
                        anotherPlayerMove.client.send(JSON.stringify(moveMessage));
                    }
                    break;

                case "Close":
                    console.log("Клиент запросил закрытие соединения");
                    break;

                default:
                    console.log("Неизвестный тип сообщения:", response.message_type);
                    break;
            }
        } catch (error) {
            console.error("Ошибка обработки сообщения:", error);
            const client = getClient();
            if (client) {
                client.client.send(JSON.stringify({ message_type: "MessageFailed", message: "Ошибка обработки сообщения" }));
            }
        }
    });

    ws.on('close', () => {
        console.log('Клиент отключился');
        const client = getClient();
        if (client) {
            const anotherPlayer = getAnotherPlayer(client.room_name);
            if (anotherPlayer) {
                anotherPlayer.client.send(JSON.stringify({
                    message_type: "PlayerDisconnected",
                    player_username: client.username
                }));
            }
            clients = clients.filter(clientData => clientData.client !== ws);
        }
    });

    ws.on('error', (err) => {
        console.error('Ошибка WebSocket:', err);
    });

    ws.send(JSON.stringify({ message_type: "ConnectedToWebSocket" }));
});