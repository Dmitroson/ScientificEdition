using ScientificEdition.Data.Entities;

namespace ScientificEdition.Areas.Admin.Models.Users
{
    public class UsersListViewModel
    {
        public required string Role { get; set; }

        public IList<User> Users { get; set; } = [];
    }
}
