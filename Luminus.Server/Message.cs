using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luminus.Server
{
    public class Message
    {
        public string Id { get; set; }
        public User User { get; set; }
        public string Text { get; set; }
    }
}
