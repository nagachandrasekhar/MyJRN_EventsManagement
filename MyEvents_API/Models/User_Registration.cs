using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace MyEvents_API.Models
{
    public class User_Registration
    {

        [Key]
        public int UserId { get; set; }
     
        public string First_Name { get; set; }
     
        public string Last_Name { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [PasswordPropertyText]
        public string Password { get; set; }

        public string UserName { get; set; }
       
        public string Role { get; set; }
        public string Token { get; set; }
        public string PhoneNumber { get; set; }
   
        public string Address { get; set; }
        
        public string City { get; set; }
       
        public string State { get; set; }
  
        public string ZipCode { get; set; }
        public string Country { get; set; }
        public DateTime Created_Date { get; set; }
        public string CreatedBy { get; set; }
        public DateTime Updated_Date { get; set; }
        public string UpdatedBy { get; set; }
    }

}
