const WebSocket = require('ws');
const axios = require("axios");
const qs = require("qs");

const port = process.env.PORT || 3000;
const server = new WebSocket.Server({ port });

console.log(`WebSocket started on port ${port}`);

let clients = [];

const API_URL = "https://aleksandermilisenko23.thkit.ee/other/tic-tac-toe/api/";
const API_KEY = "baratrumdelaetvrumvrum2015"

server.on('connection', (ws) => {
    console.log('Client has connected');
    
    clients.push({
        client: ws,
        unique_identity: null,
        room_name: null,
        username: null,
        player_id: null,
        game_id: null,
        disconnected: false
    });
    
    const getClient = () => clients.find(clientData => clientData.client === ws);
    const client = getClient();
    const getAnotherPlayer = (roomName) => {
        return clients.find(clientFromList => clientFromList.room_name === roomName && clientFromList.client !== ws) || null;
    };
    let anotherPlayer = null;

    ws.on('message', async (message) => {
        try {
            if (!client) return;

            const response = JSON.parse(message);
            console.log("Message has received:", response);

            switch (response.message_type) {
                case "InitialJoinMessage":
                    console.log("Handling InitialJoinMessage");
                    const unique_identity = generateRandomString(10);

                    client.room_name = response.room_name;
                    client.username = response.username;
                    client.game_id = getAnotherPlayer(client.room_name).game_id;
                    client.unique_identity = unique_identity;

                    let databaseResponse = null;
                    try{
                        console.log({
                            apikey: API_KEY,
                            username: client.username,
                            unique_identity: client.unique_identity,
                            game: client.game_id
                        });
                        databaseResponse = await axios.post(API_URL + "create_user.php", qs.stringify({
                            apikey: API_KEY,
                            username: client.username,
                            unique_identity: client.unique_identity,
                            game: client.game_id
                        }), {
                            headers: {
                                'Content-Type': 'application/x-www-form-urlencoded'
                            }
                        })
                        console.log(databaseResponse.data);
                        client.player_id = databaseResponse.data.player_id;
                    }
                    catch (error) {
                        console.log("InitialJoinMessage", error);
                    }
                    
                    const anotherPlayer = getAnotherPlayer(response.room_name);
                    console.log("Another player in the room:", anotherPlayer ? anotherPlayer.username : "not found");

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
                    client.room_name = response.room_name;
                    client.username = response.username;
                    client.unique_identity = generateRandomString(10);
                    try{
                        console.log("dhk 1")
                        
                        const gameId = await axios.post(API_URL + "create_game.php", {
                            apikey: API_KEY,
                            room: response.room_name
                        }, {
                            headers:{
                                'Content-Type': 'application/x-www-form-urlencoded'
                            }
                        });
                        console.log("dhk 2")
                        client.game_id = gameId.data.id;
                        console.log("dhk 3")
                        console.log({
                            apikey: API_KEY,
                            username: client.username,
                            unique_identity: client.unique_identity,
                            game: client.game_id
                        });
                        const initialCreateMessageResponse = await axios.post(API_URL + "create_user.php", qs.stringify({
                            apikey: API_KEY,
                            username: client.username,
                            game: client.game_id,
                            unique_identity: client.unique_identity
                        }), {
                            headers: {
                                'Content-Type': 'application/x-www-form-urlencoded'
                            }
                        })
                        console.log("dhk 4", initialCreateMessageResponse.data);
                        client.player_id = initialCreateMessageResponse.data.player_id;
                    }
                    catch (error){
                        console.log("InitialCreateMessage", error);
                    }
                    console.log("Handling InitialCreateMessage:", client.username);
                    break;

                case "ReconnectMessage":
                    console.log(client.username, "is reconnecting");
                    const anotherPlayerReconnect = getAnotherPlayer(client.room_name);
                    if (anotherPlayerReconnect) {
                        anotherPlayerReconnect.client.send(JSON.stringify({
                            message_type: "PlayerReconnected"
                        }));
                    }
                    break;

                case "PlayerDisconnected":
                    console.log(client.username, "отключился");
                    clients.
                    break;

                case "PlayerMove":
                    const anotherPlayerMove = getAnotherPlayer(client.room_name);
                    if (anotherPlayerMove) {
                        try{
                            console.log({apikey: API_KEY,
                                x: response.x,
                                y: response.y,
                                game: client.game_id,
                                player: client.player_id})
                            let move = await axios.post(API_URL + "move.php", qs.stringify({
                                apikey: API_KEY,
                                x: response.x,
                                y: response.y,
                                game: client.game_id,
                                player: client.player_id
                            }), {
                                'Content-Type': 'application/x-www-form-urlencoded'
                            })
                            
                            console.log("smotri", move.data)
                            await axios.post(API_URL + "change_turn.php", qs.stringify({
                                apikey: API_KEY,
                                game: client.game_id,
                                player: client.player_id
                            }), {
                                headers:{
                                    'Content-Type': 'application/x-www-form-urlencoded'
                                }
                            })
                            const moveMessage = {
                                message_type: "PlayerMoved",
                                x: response.x,
                                y: response.y
                            };
                            console.log("Move has sended to player:", anotherPlayerMove.username);
                            anotherPlayerMove.client.send(JSON.stringify(moveMessage));
                        }
                        catch (error) {
                            console.log("PlayerMove", error)
                        }

                    }
                    break;
                case "Draw":
                    anotherPlayer = getAnotherPlayer(client.room_name);
                    if(anotherPlayer){
                        anotherPlayer.client.send(JSON.stringify({"message_type":"Draw"}));
                    }
                    console.log();
                    
                    break;
                case "Close":
                    console.log("Client wanted to close connection");
                    break;
                case "PlayerWon":
                    anotherPlayer = getAnotherPlayer(client.room_name);
                    if(anotherPlayer){
                        const wonMessage = {
                            message_type:"PlayerWon"
                        };
                        anotherPlayer.client.send(JSON.stringify(wonMessage));
                        try{
                            await axios.post(API_URL + "clear_game.php", {
                                apikey: API_KEY,
                                game: client.game_id
                            }, {
                                headers:{
                                    'Content-Type': 'application/x-www-form-urlencoded'
                                }
                            })
                        }
                        catch (error){
                            console.log("PlayerWon", error);
                        }
                    }
                    break;
                default:
                    console.log("Unknown type of message:", response.message_type);
                    break;
            }
        } catch (error) {
            console.error("Error by processing:", error);
            if (client) {
                client.client.send(JSON.stringify({ message_type: "MessageFailed", message: "Ошибка обработки сообщения" }));
            }
        }
    });

    ws.on('close', async () => {
        console.log('Client has disconnected');
        if (client) {
            const anotherPlayer = getAnotherPlayer(client.room_name);
            console.log(client);
            console.log(1);
            client.disconnected = true;
            console.log(2);
            if(!anotherPlayer){
                try{
                    console.log(3);
                    if(!client && !client.game_id) return;
                    console.log(4);
                    await axios.post(API_URL + "clear_game.php", {
                        apikey: API_KEY,
                        game: client.game_id
                    }, {
                        headers:{
                            "Content-Type": 'application/x-www-form-urlencoded'
                        }
                    })
                    console.log(5);
                }
                catch(error){
                    console.log("on close error", error);
                }
                return;
            }
            if(anotherPlayer.disconnected){
                console.log("clearing the game");
                try{
                    console.log({
                        apikey: API_KEY,
                        game: client.game_id
                    })
                    await axios.post(API_URL + "clear_game.php", {
                        apikey: API_KEY,
                        game: client.game_id
                    }, {
                        headers:{
                            "Content-Type": 'application/x-www-form-urlencoded'
                        }
                    })
                }
                catch(error){
                    console.log("on close error", error)
                }
                clients = clients.filter(clientData => clientData.client !== ws || clientData.client !== anotherPlayer.client);
            }
            else {
                if (anotherPlayer) {
                    anotherPlayer.client.send(JSON.stringify({
                        message_type: "PlayerDisconnected",
                        player_username: client.username
                    }));
                }
            }
        }
    });

    ws.on('error', (err) => {
        console.error('Error WebSocket:', err);
    });

    ws.send(JSON.stringify({ message_type: "ConnectedToWebSocket" }));
});


function generateRandomString(length) {
    const characters = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789';
    let result = '';
    for (let i = 0; i < length; i++) {
        result += characters.charAt(Math.floor(Math.random() * characters.length));
    }
    return result;
}