using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Luminus.Domain.Entities
{
    public class MessageFile
    {
        [Key]
        public int Id { get; set; }
        public string FileName { get; set; }
        public Message Message { get; set; }
    }
}