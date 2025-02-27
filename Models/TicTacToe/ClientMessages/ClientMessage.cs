using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace alexm_app.Models.TicTacToe.ClientMessages
{
    public class ClientMessage
    {
        public string Message
        {
            get
            {
                string message = $"{{\"request\":\"{Request}\"";
                int count = 0;
                foreach(string partial in MessagePartials)
                {
                    count++;
                    message += count == MessagePartials.Count ? "" : ",";
                    message += $"{partial}";
                }
                message += "}";
                return message;
            }
        }
        public string Request { get; set; } = "undefined";

        public List<string> MessagePartials = new List<string>();
    }
}
