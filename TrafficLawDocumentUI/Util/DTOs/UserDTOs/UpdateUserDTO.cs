﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Util.DTOs.UserDTOs
{
    public class UpdateUserDTO
    {
        [Required]
        public Guid Id { get; set; }

        [MaxLength(100)]
        public string? FullName { get; set; }

        [EmailAddress]
        [RegularExpression(
          @"^[a-zA-Z0-9._%+-]+@gmail\.com$", ErrorMessage = "Email must be a valid Gmail address."
        )]
        public string? Email { get; set; }

        [RegularExpression("^(User|Expert|Admin|Staff)$", ErrorMessage = "Role must be User, Expert, Staff or Admin.")]
        public string? Role { get; set; }

        public bool? IsActive { get; set; }
    }
}
