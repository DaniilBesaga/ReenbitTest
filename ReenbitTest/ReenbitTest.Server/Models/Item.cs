using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ReenbitTest.Server.Models
{
    public class Item
    {
        public IFormFile File {  get; set; }
        public string Email { get; set; }
    }
}
