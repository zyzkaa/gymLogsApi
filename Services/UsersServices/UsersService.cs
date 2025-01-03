using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.DTO;
using WebApp.Entities;

namespace WebApp.Services.UsersServices;

public class UsersService : IUsersService
{
    private readonly WebAppContext _dbContext;
    private readonly IHttpContextAccessor _httpContentAccessor;
    
    public UsersService(WebAppContext dbContext, IHttpContextAccessor httpContextAccessor)
    {   
        _dbContext = dbContext;
        _httpContentAccessor = httpContextAccessor;
    }

    public User GetUser()
    {
        return _dbContext.Users.FirstOrDefault(u => u.Id == int.Parse(_httpContentAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value));
    }

    private UserResponseDto ParseUserToDto(User user)
    {
        return new UserResponseDto()
        {
            Id = user.Id,
            Username = user.Username,
            Weight = user.Weight,
            Height = user.Height,
            Age = user.Age
        };
    }

    private UserResponseDto ReturnMessage(string message)
    {
        return new UserResponseDto()
        {
            Message = message
        };
    }
    
    public UserResponseDto RegisterUser(User newUser)
    {
        // var newUser = new User()
        // {
        //     Username = newUserDto.Username,
        //     Password = newUserDto.Password,
        //     Weight = newUserDto.Weight,
        //     Height = newUserDto.Height,
        //     Age = newUserDto.Age,
        //     CreatedAt = DateTime.Now
        // };
        
        _dbContext.Users.Add(newUser);
        _dbContext.SaveChanges();

        return ParseUserToDto(newUser);
    }

    public async Task<UserResponseDto> LoginUser(LoginUserDto loginUserDto)
    {
        var user = _dbContext.Users.FirstOrDefault(u => u.Username == loginUserDto.Username);
        
        if (user == null)
        {
            return ReturnMessage("username not found");
        }

        if (user.Password != loginUserDto.Password)
        {
            return ReturnMessage("wrong password");
        }
        
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        };
        var identity = new ClaimsIdentity(claims, "authScheme");
        var principal = new ClaimsPrincipal(identity);
        await _httpContentAccessor.HttpContext.SignInAsync(principal);
        
        return ParseUserToDto(user);

    }

    public async Task<UserResponseDto> LogoutUser()
    {
        var currentUserId = _httpContentAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
        await _httpContentAccessor.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return ReturnMessage("logged out");
    }

    public async Task<UserResponseDto> ChangeUserParameters(UserParametersDto userParametersDto)
    {
        var currentUserId = _httpContentAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
        var user = _dbContext.Users.FirstOrDefault(u => u.Id == int.Parse(currentUserId));
        
        user.Age = userParametersDto.Age;
        user.Height = userParametersDto.Height;
        user.Weight = userParametersDto.Weight;
        await _dbContext.SaveChangesAsync();
        
        return ParseUserToDto(user);
    }
}