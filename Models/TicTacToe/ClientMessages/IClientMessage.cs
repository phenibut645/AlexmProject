using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace alexm_app.Models.TicTacToe.ClientMessages
{
    public interface IClientMessage
    {
        public string Request { get; set; }
    }
}
