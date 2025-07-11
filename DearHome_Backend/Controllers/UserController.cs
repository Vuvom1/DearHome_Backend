using System.Security.Claims;
using AutoMapper;
using DearHome_Backend.DTOs.UserDtos;
using DearHome_Backend.Models;
using DearHome_Backend.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DearHome_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;

        public UserController(IUserService userService, IMapper mapper, UserManager<User> userManager)
        {
            _userService = userService;
            _mapper = mapper;
            _userManager = userManager;
        }

        [HttpGet("all-customers")]
        public async Task<IActionResult> GetAllCustomers([FromQuery] int offSet, [FromQuery] int limit)
        {
            var customers = await _userService.GetAllCustomersAsync(offSet, limit);
            var customerDtos = _mapper.Map<IEnumerable<UserDto>>(customers);
            return Ok(customerDtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(Guid id)
        {
            var user = await _userService.GetUserAsync(id);
            if (user == null)
            {
                return NotFound("User not found.");
            }
            return Ok(user);
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUser()
        {
            // Extract user ID from the bearer token claims
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var userGuid))
            {
                return Unauthorized("Invalid authentication token.");
            }

            var user = await _userService.GetUserAsync(userGuid);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            return Ok(user);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto userLoginDto)
        {
            var token = await _userService.LoginAsync(userLoginDto.UserName, userLoginDto.Password);
            return Ok(token);
        }

        [HttpPost("login-google")]
        public async Task<IActionResult> LoginWithGoogle([FromBody] GoogleLoginDto googleLoginDto)
        {
            var accessToken = googleLoginDto.AccessToken;

            if (string.IsNullOrEmpty(accessToken))
            {
                return BadRequest("Access token is required.");
            }

            var token = await _userService.LoginWithGoogleAsync(accessToken);
            return Ok(token);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterDto userRegisterDto)
        {
            var user = _mapper.Map<User>(userRegisterDto);
            await _userService.RegisterAsync(user, userRegisterDto.Password, userRegisterDto.VerificationCode);
            return Ok("User registered successfully.");
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return BadRequest("Token is required.");
            }

            await _userService.LogoutAsync(token);
            return Ok("User logged out successfully.");
        }

        [HttpPost("send-verification-code")]
        public async Task<IActionResult> SendVerificationCode([FromBody] VerificationCodeDto verificationCodeDto)
        {
            if (string.IsNullOrEmpty(verificationCodeDto.Email))
            {
                return BadRequest("Email and verification code are required.");
            }

            await _userService.SendVerificationCodeAsync(verificationCodeDto.Email);

            return Ok("Verification code sent successfully.");
        }

        [HttpPut]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserDto updateUserDto)
        {
            var user = _mapper.Map<User>(updateUserDto);
            await _userService.UpdateAsync(user);

            return Ok();
        }

        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
        {
            // Get user ID from the token claims
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var userGuid))
            {
                return Unauthorized("Invalid authentication token.");
            }

            // Validate the request
            if (string.IsNullOrEmpty(changePasswordDto.CurrentPassword) || string.IsNullOrEmpty(changePasswordDto.NewPassword))
            {
                return BadRequest("Current password and new password are required.");
            }

            // Call service to change password
            await _userService.ChangePasswordAsync(userGuid, changePasswordDto.CurrentPassword, changePasswordDto.NewPassword, changePasswordDto.ConfirmNewPassword);

            return Ok("Password changed successfully.");
        }
    }
}
