﻿using Microsoft.AspNetCore.Identity;

namespace ScientificEdition.Data.Entities
{
    public class User : IdentityUser
    {
        public required string FirstName { get; set; }

        public required string LastName { get; set; }
    }
}