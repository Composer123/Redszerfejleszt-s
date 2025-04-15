﻿using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Raktar.DataContext.DataTransferObjects;
using Raktar.Services;

namespace Raktar.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register(UserRegisterDTO userDto)
        {
            var user = await _userService.RegisterAsync(userDto);
            return CreatedAtAction(nameof(GetUserById), new { id = user.UserId }, user);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(UserLoginDTO userDto)
        {
            var token = await _userService.LoginAsync(userDto);
            return Ok(new { Token = token });
        }

        [HttpPut("{userId}/profile")]
        [Authorize(Roles = "Customer")]
        [Authorize(Roles = "Admin")]
        [Authorize(Roles = "Suppiler")]
        [Authorize(Roles = "Transporter")]
        public async Task<IActionResult> UpdateProfile(int userId, UserUpdateDTO userDto)
        {
            var user = await _userService.UpdateProfileAsync(userId, userDto);
            return Ok(user);
        }

        [HttpPut("{userId}/address")]
        [Authorize(Roles = "Customer")]
        [Authorize(Roles = "Admin")]
        [Authorize(Roles = "Suppiler")]
        [Authorize(Roles = "Transporter")]
        public async Task<IActionResult> UpdateAddress(int userId, SimpleAddressDTO addressDto)
        {
            var user = await _userService.UpdateAddressAsync(userId, addressDto);
            return Ok(user);
        }

        [HttpGet("roles")]
        [Authorize(Roles = "Customer")]
        [Authorize(Roles = "Admin")]
        [Authorize(Roles = "Suppiler")]
        [Authorize(Roles = "Transporter")]
        public async Task<IActionResult> GetRoles()
        {
            var roles = await _userService.GetRolesAsync();
            return Ok(roles);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Customer")]
        [Authorize(Roles = "Admin")]
        [Authorize(Roles = "Suppiler")]
        [Authorize(Roles = "Transporter")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }
    }
}