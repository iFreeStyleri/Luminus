using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luminus.Server
{
    public class Message
    {
        [Key]
        public int Id { get; set; }
        public User User { get; set; }
        public string Text { get; set; }
        public DateTime Created { get; set; }
    }
}
