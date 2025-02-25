const net = require('net');

const server = net.createServer((socket) =>{
    console.log("client has connected");
});

server.listen(3000, ()=>{
    console.log("server is listening port 3000")
})

server.on('error', (err) => {
    console.error(err);
})