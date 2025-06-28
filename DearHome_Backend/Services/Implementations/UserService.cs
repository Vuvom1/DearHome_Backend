using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using DearHome_Backend.Constants;
using DearHome_Backend.Models;
using DearHome_Backend.Repositories.Interfaces;
using DearHome_Backend.Services.Implementations;
using DearHome_Backend.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Google.Apis.Auth;
using DearHome_Backend.DTOs.UserDtos;

namespace DearHome_Backend.Services.Inplementations;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IAddressRepository _addressRepository;
    private readonly IVerificationCodeRepository _verificationCodeRepository;
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IPasswordValidator<User> _passwordValidator;
    private readonly IEmailService _emailService;
    private readonly IConfiguration _configuration;

    public UserService(
        IUserRepository userRepository,
        IAddressRepository addressRepository,
        IVerificationCodeRepository verificationCodeRepository,
        UserManager<User> userManager,
        RoleManager<IdentityRole<Guid>> roleManager,
        IPasswordHasher<User> passwordHasher,
        IPasswordValidator<User> passwordValidator,
        IEmailService emailSender,
        IConfiguration configuration)

    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _addressRepository = addressRepository ?? throw new ArgumentNullException(nameof(addressRepository));
        _verificationCodeRepository = verificationCodeRepository ?? throw new ArgumentNullException(nameof(verificationCodeRepository));
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
        _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
        _passwordValidator = passwordValidator ?? throw new ArgumentNullException(nameof(passwordValidator));
        _emailService = emailSender ?? throw new ArgumentNullException(nameof(emailSender));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    public async Task<string> LoginAsync(string userName, string password)
    {

        var user = await _userManager.FindByNameAsync(userName);

        if (user == null)
        {
            throw new InvalidOperationException("Invalid username or password.");
        }

        // Check if the password is null or empty
        if (string.IsNullOrEmpty(user.PasswordHash))
        {
            throw new InvalidOperationException("Invalid username or password.");
        }

        // Verify the password
        var passwordVerificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
        if (passwordVerificationResult != PasswordVerificationResult.Success)
        {
        throw new InvalidOperationException("Invalid username or password.");
        }

        // Generate JWT token
        var token = GenerateToken(user);

        return token;
    }

    public async Task<string> LoginWithGoogleAsync(string accessToken)
    {
        using (var httpClient = new HttpClient())
        {
            // Get user info from Google API
            var response = await httpClient.GetAsync($"https://www.googleapis.com/oauth2/v1/userinfo?access_token={accessToken}");
            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException("Invalid Google access token.");
            }

            var userInfo = await response.Content.ReadFromJsonAsync<GoogleUserInfo>();
            if (userInfo == null || string.IsNullOrEmpty(userInfo.Email))
            {
                throw new InvalidOperationException("Failed to retrieve user information from Google.");
            }

            // Check if the user already exists
            var existingUser = await _userManager.FindByEmailAsync(userInfo.Email);
            if (existingUser != null)
            {
                // Generate JWT token for existing user
                var existingUserToken = GenerateToken(existingUser);
                return existingUserToken;
            }

            var user = new User
            {
                Email = userInfo.Email,
                NormalizedEmail = userInfo.Email.ToUpper(),
                UserName = userInfo.Email.Split('@')[0],
                NormalizedUserName = userInfo.Email.Split('@')[0].ToUpper(),
                Name = userInfo.Name,
                EmailConfirmed = true,
                ImageUrl = userInfo.Picture,
                PasswordHash = null, // Password is not needed for Google login
                PhoneNumber = "N/A", // Provide a default value for PhoneNumber
            };

            

            // Create a new user if it doesn't exist
            var result = await _userManager.CreateAsync(user, "DefaultPassword123!"); // Use a default password or generate one
            if (!result.Succeeded)
            {
                throw new InvalidOperationException("User registration failed: " + string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            // Assign roles to the new user
            var roleResult = await _userManager.AddToRoleAsync(user, UserRole.User.ToString());
            if (!roleResult.Succeeded)
            {
                throw new InvalidOperationException("Failed to assign roles: " + string.Join(", ", roleResult.Errors.Select(e => e.Description)));
            }

            // Generate JWT token for the new user
            var token = GenerateToken(user);
            return token;
        }
    }

    public async Task<User> RegisterAsync(User user, string password, string verificationCode)
    {

        // Validate the password
        var passwordValidationResult = await _passwordValidator.ValidateAsync(_userManager, user, password);
        if (passwordValidationResult != IdentityResult.Success)
        {
            throw new InvalidOperationException("Password validation failed: " + string.Join(", ", passwordValidationResult.Errors.Select(e => e.Description)));
        }
        // Check if the verification code is valid
        if (string.IsNullOrEmpty(user.Email))
        {
            throw new InvalidOperationException("User email cannot be null or empty.");
        }

        var verificationCodeEntity = await _verificationCodeRepository.GetVerificationCodeByEmailAsync(user.Email);
        if (verificationCodeEntity == null || verificationCodeEntity.Code != verificationCode)
        {
            throw new InvalidOperationException("Invalid verification code.");
        }
        // Check if the verification code is expired
        if (verificationCodeEntity.ExpirationDate < DateTime.UtcNow)
        {
            throw new InvalidOperationException("Verification code has expired.");
        }
        // Check if the user already exists
        var existingUser = await _userManager.FindByEmailAsync(user.Email);
        if (existingUser != null)
        {
            throw new InvalidOperationException("User already exists.");
        }

        // Save the user to the database
        var result = await _userManager.CreateAsync(user, password);
        if (!result.Succeeded)
        {
            throw new InvalidOperationException("User registration failed: " + string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        // Assign roles to the user
        var roleResult = await _userManager.AddToRoleAsync(user, UserRole.User.ToString());

        return user;
    }

    public async Task SendVerificationCodeAsync(string email)
    {
        if (string.IsNullOrEmpty(email))
        {
            throw new ArgumentNullException(nameof(email), "Email cannot be null or empty.");
        }
        var verificationCode = GenerateVerificationCode();
        var verificationCodeEntity = new VerificationCode
        {
            Email = email,
            Code = verificationCode,
            ExpirationDate = DateTime.UtcNow.AddMinutes(5)
        };
        var existingCode = await _verificationCodeRepository.GetVerificationCodeByEmailAsync(email);
        if (existingCode != null)
        {
            existingCode.Code = verificationCode;
            existingCode.ExpirationDate = DateTime.UtcNow.AddHours(17).AddMinutes(30);
            await _verificationCodeRepository.UpdateAsync(existingCode);
        }
        else
        {
            await _verificationCodeRepository.AddAsync(verificationCodeEntity);
        }

        // Send the verification code to the user's email
        await _emailService.SendVerificationCodeAsync(email, verificationCode);
    }
    private string GenerateToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtKey = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT key is not configured.");
        var key = Encoding.ASCII.GetBytes(jwtKey);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(
            [
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                new Claim(ClaimTypes.Role, string.Join(",", _userManager.GetRolesAsync(user).Result)),
                new Claim("imageUrl", user.ImageUrl ?? string.Empty),
            ]),
            Expires = DateTime.UtcNow.AddHours(10),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    private string GenerateVerificationCode()
    {
        var randomNumber = new byte[6];
        RandomNumberGenerator.Fill(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    public Task<User?> GetUserAsync(Guid id)
    {        
        return _userRepository.GetByIdAsync(id);
    }

    public async Task UpdateAsync(User user)
    {
        var existingUser = await _userRepository.GetByIdWithAddressesAsync(user.Id);

        if (existingUser == null)
        {
            throw new InvalidOperationException("User not found.");
        }

        //UserName and Email are not allowed to be updated
        user.UserName = existingUser.UserName;
        user.Email = existingUser.Email;
        user.SecurityStamp = existingUser.SecurityStamp;
        user.EmailConfirmed = existingUser.EmailConfirmed;
        user.PasswordHash = existingUser.PasswordHash;
        user.ConcurrencyStamp = Guid.NewGuid().ToString();  

        // Check if the phone number is already in use by another user
        if (user.PhoneNumber != existingUser.PhoneNumber)
        {
            var phoneNumberExists = await _userRepository.IsPhoneNumberExistsAsync(user.PhoneNumber!);
            if (phoneNumberExists)
            {
                throw new InvalidOperationException("Phone number already in use.");
            }
        }

        //Update the user addresses
        if (existingUser.Addresses != null)
        {
            foreach (var address in existingUser.Addresses)
            {
                var updatedAddress = user.Addresses?.FirstOrDefault(a => a.Id == address.Id);
                if (updatedAddress != null)
                {
                    address.Street = updatedAddress.Street;
                    address.Ward = updatedAddress.Ward;
                    address.City = updatedAddress.City;
                    address.District = updatedAddress.District;
                    address.City = updatedAddress.City;
                    address.PostalCode = updatedAddress.PostalCode;
                    address.IsDefault = updatedAddress.IsDefault;
        
                    await _addressRepository.UpdateAsync(address);
                }
                else                {
                    // If the address is not in the updated user, remove it
                    await _addressRepository.DeleteAsync(address);
                }
            }
        }

        //Add new addresses if (user.Addresses != null)
        {
            foreach (var address in user.Addresses!)
            {
                if (address.Id == Guid.Empty)
                {
                    address.UserId = user.Id;
                    await _addressRepository.AddAsync(address);
                }
            }
        }

        await _userRepository.UpdateAsync(user);
    }

    public Task<IEnumerable<User>> GetAllCustomersAsync(int offSet, int limit)
    {
        return _userRepository.GetAllCustomersAsync(offSet, limit);
    }

    public async Task LogoutAsync(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);
        var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

        if (userIdClaim != null)
        {
            var userId = Guid.Parse(userIdClaim.Value);
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user != null)
            {
                // Invalidate the token by updating the user's security stamp
                user.SecurityStamp = Guid.NewGuid().ToString();
                await _userManager.UpdateAsync(user);
            }
        }
        
    }

    public Task<int> GetTotalCustomersCountAsync()
    {
        return _userRepository.GetTotalCustomersCountAsync();
    }
}
