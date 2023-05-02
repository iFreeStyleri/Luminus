using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Luminus.Domain.Entities
{
    public class Message
    {
        [Key]
        public int Id { get; set; }
        public User User { get; set; }
        public string Text { get; set; }
        public DateTime Created { get; set; }
        public List<MessageFile> File { get; set; }
    }
}