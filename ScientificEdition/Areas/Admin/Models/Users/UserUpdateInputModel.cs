using System.ComponentModel.DataAnnotations;

namespace ScientificEdition.Areas.Admin.Models.Users
{
    public class UserUpdateInputModel
    {
        public required string Id { get; set; }

        [Required]
        public required string FirstName { get; set; }

        [Required]
        public required string LastName { get; set; }

        [Required]
        public required string Email { get; set; }

        [Required]
        public string? Role { get; set; }

        public List<Guid> CategoryIds { get; set; } = [];
    }
}
