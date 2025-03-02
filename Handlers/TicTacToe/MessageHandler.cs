using alexm_app.Models.TicTacToe.ClientMessages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace alexm_app.Utils.TicTacToe
{
    public static class MessageHandler
    {
        public static void PushToMessage(ClientMessage clientMessage, string key, int value)
        {
            string message = $"\"{key}\":{value}";
            clientMessage.MessagePartials.Add(message);
        }
        public static void PushToMessage(ClientMessage clientMessage, string key, string value)
        {
            string message = $"\"{key}\":\n{value}\n";
            clientMessage.MessagePartials.Add(message);
        }
    }
}
