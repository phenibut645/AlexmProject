const net = require('net');

const server = net.createServer((socket) =>{
    console.log("client has connected");


    socket.on('data', (data) => {
        console.log(`Получены данные: ${data}`);
        socket.write(`Вы отправили: ${data}`);
    });

    socket.on('end', () => {
        console.log('Клиент отключился');
    });

    socket.on('error', (err) => {
        console.error(`Ошибка: ${err}`);
    });
});

server.listen(process.env.PORT || 3000, ()=>{
    console.log("server is listening port 3000")
})

server.on('error', (err) => {
    console.error(err);
})