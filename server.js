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
    
    let client = () => clients.find(clientData => clientData.client === ws);
    let another_player = null;
    
    ws.on('message', (message) => {
        
        try {
            if (!client()) return;
            const response = JSON.parse(message);
            console.log("Message", response);
            switch (response.message_type) {
                case "InitialJoinMessage":
                    console.log("HELLLO");
                    client().unique_identity = response.unique_identity;
                    client().room_name = response.room_name;
                    client().username = response.username;
                    console.log("InitialJoinMessage", client())
                    console.log(clients);
                    another_player = clients.find(clientFromList => clientFromList.room_name === response.room_name && clientFromList != ws);
                    console.log(another_player && "found another player");
                    const randomNumber = Math.floor(Math.random() * 2) + 1;
                    const forAnotherPlayerMessage = {
                        message_type: "PlayerConnected",
                        player_username: client().username,
                        turn: randomNumber === 1 ? true : false
                    };
                    const forCurrentClient = {
                        message_type: "ConnectionCompleted",
                        turn: randomNumber === 2 ? true : false
                    };
                    if (another_player) {
                        another_player.client.send(JSON.stringify(forAnotherPlayerMessage));
                    }
                    client().client.send(JSON.stringify(forCurrentClient));
                    break;
                    
                case "InitialCreateMessage":
                    client().unique_identity = response.unique_identity;
                    client().room_name = response.room_name;
                    client().username = response.username;
                    console.log("InitialCreateMessage", client().username)
                    break;

                case "PlayerMove":
                    if (another_player) {
                        const moveMessage = {
                            message_type: "PlayerMoved"
                        };
                        another_player.client.send(JSON.stringify(moveMessage));
                    }
                    break;
                    
                case "Close":
                    break;

                default:
                    break;
            }   
        }
        catch (error) {
            console.error("Ошибка обработки сообщения:", error);
            if (client()) {
                client().client.send(JSON.stringify({ message_type: "MessageFailed", message: message }));
            }
        }
    });

    ws.on('close', () => {
        console.log('Клиент отключился');
        clients = clients.filter(clientData => clientData.client !== ws);
    });

    ws.on('error', (err) => {
        console.error('Ошибка WebSocket:', err);
    });
    ws.send(JSON.stringify({message_type:"ConnectedToWebSocket"}));
});