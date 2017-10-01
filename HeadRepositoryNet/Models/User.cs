using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HeadRepositoryNet.Models
{
    [Table("Users")]
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string Username { get; set; }

        //[NotMappedAttribute]
        [Required]
        [JsonIgnore]
        public string Password { get ; set; }

        [JsonProperty("password")]
        private string PasswordAlternateSetter
        {
            set { Password = value; }
        }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string Email { get; set; }

        public bool Admin { get; set; }

        public bool HaveAccess { get; set; }
    }
}
