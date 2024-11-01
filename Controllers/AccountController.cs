using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.Account;
using api.Interfaces;
using api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace api.Controllers
{
    [Microsoft.AspNetCore.Components.Route("api/account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ITokenService _tokenService;
        private readonly SignInManager<AppUser> _signInManager;

        public AccountController(UserManager<AppUser> userManager, ITokenService tokenService, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _signInManager = signInManager;

        }
        [HttpPost("api/account/register")]
        public async Task<IActionResult> Register([FromBody] RegisterAccountDto registerDto) {
            try {

                if (await _userManager.FindByEmailAsync(registerDto.EmailAddress) != null) {
                    ModelState.AddModelError("DuplicateEmail", "Email is taken");
                };
                if (await _userManager.FindByNameAsync(registerDto.Username) != null) { 
                    ModelState.AddModelError("DuplicateUserName", "UserName is taken");
                }
                if (!ModelState.IsValid) {
                    return BadRequest(ModelState);
                }


                
                var appUser = new AppUser
                {
                    UserName = registerDto.Username,
                    Email = registerDto.EmailAddress,
                };

                var createdUser = await _userManager.CreateAsync(appUser, registerDto.Password);
                if (!createdUser.Succeeded) {
                    return BadRequest(createdUser.Errors);
                }

                var roleResult = await _userManager.AddToRoleAsync(appUser, "User");
                if (!roleResult.Succeeded) {
                    return BadRequest(roleResult.Errors);
                }
                return Ok(
                    new NewUserResponseDto { 

                     UserName = appUser.UserName,
                     Email = appUser.Email,
                     Token = _tokenService.CreateToken(appUser)   
                    }
                );
               
            }
            catch (Exception e) {
                return StatusCode(500, e.Message);
            }
        }

        [HttpPost("api/account/login")]
        public async Task<IActionResult> Login([FromBody] LoginUserDto loginUserDto) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
             }

            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == loginUserDto.UserName);
            if (user == null) {
                ModelState.AddModelError("UserNotFound", "User " + loginUserDto.UserName + " not found");
                return NotFound(ModelState["UserNotFound"].Errors);
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginUserDto.Password, false);

            if (!result.Succeeded) {
                ModelState.AddModelError("PasswordNotValid", "Password incorrect");
                return Unauthorized(ModelState["PasswordNotValid"].Errors);
            }

            return Ok(
                new NewUserResponseDto
                {
                    UserName = user.UserName,
                    Email = user.Email,
                    Token = _tokenService.CreateToken(user)
                }
            );
         }
        
    }
}