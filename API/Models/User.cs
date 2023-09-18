
using System.Globalization;
using System.Text.Json.Serialization;

namespace API.Models
{
    public class User
    {
        
        public int Id { get; set; } 

        public String Username { get; set; }
        public String Password { get; set; }
        
       
        public String Role { get; set; }
     

    }
}
