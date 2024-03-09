using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using IdentityApi.Models.Helpers;

namespace IdentityApi.Models.DbModels
{
    public class User
    {
        public int Id { get; set; }
        
        [Required]
        public string Name { get; set; } = null!;

        [Required]
        public string HashedPassword { get; set; } = null!;
        
        [Required]
        public RolesEnum Role {get; set; } = RolesEnum.User;
    }
}