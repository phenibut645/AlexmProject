const WebSocket = require('ws');

const port = process.env.PORT || 3000;
const server = new WebSocket.Server({ port });

console.log(`WebSocket сервер запущен на порту ${port}`);

server.on('connection', (ws) => {
  console.log('Клиент подключился');

  ws.on('message', (message) => {
    console.log(`Получено сообщение: ${message}`);
    ws.send(`Эхо: ${message}`);
  });

  ws.on('close', () => {
    console.log('Клиент отключился');
  });

  ws.on('error', (err) => {
    console.error('Ошибка WebSocket:', err);
  });
});