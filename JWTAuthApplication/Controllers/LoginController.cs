using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using JWTAuthApplication.Models;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using System.Text;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using BCrypt.Net; 

namespace JWTAuthApplication.Controllers
{
    public class LoginController : Controller
    { 
        private readonly USERDESKContext dbContext;
        private readonly JwtSettings jwtSettings;
        public LoginController(USERDESKContext _dbContext, IOptions<JwtSettings> jwtSettings)
        {
            dbContext = _dbContext;
            this.jwtSettings = jwtSettings.Value;
        }
        [HttpPost]
        public IActionResult Register(Usertable user)
        {
            var dupEmail = dbContext.Usertable.Any(d => d.EmailId == user.EmailId);
            var dupUsername = dbContext.Usertable.Any(d => d.Username == user.Username);
            EmailAddressAttribute e = new EmailAddressAttribute();
            if (!e.IsValid(user.EmailId))
                return BadRequest("Invalid email address");
            if (dupEmail || dupUsername)
                return BadRequest("Username or Email already exists");
            if (user.Password.Length < 6)
                return BadRequest("Password less than 6 letters");
            string pattern = @"^(?=.*[A-Z])(?=.*\d)(?=.*\W).+$";
            if (!Regex.IsMatch(user.Password, pattern))
                return BadRequest("Password must contain at least one capital letter, one number and one special character");

            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(user.Password);
            user.Password = hashedPassword;

            dbContext.Usertable.Add(user);
            dbContext.SaveChanges();
           
            return RedirectToAction("Login", "Login");
        }

        [HttpPost]
        public IActionResult Login(Usertable user)
        {
            var storedUser = dbContext.Usertable.FirstOrDefault(d => d.EmailId == user.EmailId);
            if (storedUser != null && BCrypt.Net.BCrypt.Verify(user.Password, storedUser.Password))
            {
                var token = GenerateJwtToken(user);
                return RedirectToAction("Welcome", new { token });
                //return Ok(token); 
            }

            else if (storedUser != null)
            {
                // User exists, but password is incorrect
                return Ok("Invalid password");
            }
            else
            {
                TempData["Message"] = "User is not registered. Please register first.";
                return RedirectToAction("Register");
            }
        }

        private string GenerateJwtToken(Usertable user)
        {
            var securityKey = new byte[32];

            var claims = new List<Claim>();

            if (user.Id != null)
            {
                claims.Add(new Claim(ClaimTypes.Name, user.Id.ToString()));
            }

            if (!string.IsNullOrEmpty(user.Username))
            {
                claims.Add(new Claim(ClaimTypes.Name, user.Username));
            }

            if (!string.IsNullOrEmpty(user.EmailId))
            {
                claims.Add(new Claim(ClaimTypes.Name, user.EmailId));
            }

            if (claims.Count == 0)
            {
                return null;
            }
            
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(securityKey);
            }

            var credentials = new SigningCredentials(new SymmetricSecurityKey(securityKey), SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(jwtSettings.Issuer, jwtSettings.Issuer, claims,
                expires: DateTime.Now.AddDays(7), signingCredentials: credentials);
           
            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        public IActionResult Register()
        {
            return View();
        }

        
        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Welcome()
        {
            return View();
        }

    }

}