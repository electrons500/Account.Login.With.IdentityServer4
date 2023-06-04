using System.ComponentModel.DataAnnotations;

namespace Webapi.Models.ApiModel
{
    public class LoginApiModel
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; } = null!;
        [Required]
        [DataType(DataType.Password)]
        [MinLength(6)]
        public string Password { get; set; } = null!;
    }
}
