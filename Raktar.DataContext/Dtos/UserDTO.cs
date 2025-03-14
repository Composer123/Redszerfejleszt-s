﻿using Raktar.DataContext.Entities;
using System.ComponentModel.DataAnnotations;

namespace Raktar.DataContext.Dtos
{
    public class UserDTO
    {
        public int UserId { get; set; }

        public int TelephoneNumber { get; set; }

        public string Email { get; set; }

        public string? Username { get; set; }

        public byte[] Password { get; set; }

        public ICollection<Order> Orders { get; set; } = new List<Order>();

        public ICollection<Role> Roles { get; set; } = new List<Role>();
    }

    public class UserRegisterDTO
    {
       
        [Phone]
        public int TelephoneNumber { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(50)]
        public string? Username { get; set; }

        [Required]
        [MinLength(6)]
        public byte[] Password { get; set; }

        public ICollection<Order> Orders { get; set; } = new List<Order>();

        public ICollection<Role> Roles { get; set; } = new List<Role>();
    }
    public class UserLoginDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
    public class UserUpdateDTO
    {
        [Required]
        [StringLength(50)]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Phone]
        public string TelephoneNumber { get; set; }

        public ICollection<Role> Roles { get; set; }
    }
}
