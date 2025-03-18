using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace alexm_app.Models
{
    public class Friend
    {
        public string Name { get; set; }
        public string Photo { get; set; } = "default_photo.png";
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Description { get; set; }
        public string Message { get; set; } = "";
    }
}
