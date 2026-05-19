using EventBookingSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
namespace EventBookingSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;


        public AccountController(ApplicationDbContext context,IConfiguration configuration) {
            _context = context;
            _configuration=configuration;
        }

        
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult register(EventBookingSystem.Models.User user)
        {
            var isEmailExist = _context.Users.FirstOrDefault(u => u.email == user.email);

            if (isEmailExist == null)
            {
                user.password = BCrypt.Net.BCrypt.HashPassword(user.password);
                _context.Users.Add(user);
                _context.SaveChanges();
                return RedirectToAction("login", "Account");
            }
            else
            {
                ModelState.AddModelError("email", "Email already in use.");
                return View(user);
            }
        }

        [HttpGet]
        public IActionResult login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult login(string email, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.email == email);
            if (user != null)
            {
                if (BCrypt.Net.BCrypt.Verify(password, user.password))
                {
                    // 1. Create the JWT Token
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!);
                    var tokenDescriptor = new SecurityTokenDescriptor
                    {
                        Subject = new ClaimsIdentity(new[]
                        {
                            new Claim(ClaimTypes.NameIdentifier, user.user_id.ToString()),
                            new Claim(ClaimTypes.Email, user.email),
                            new Claim(ClaimTypes.Role, user.role)
                        }),
                        Expires = DateTime.UtcNow.AddHours(2),
                        Issuer = _configuration["Jwt:Issuer"],
                        Audience = _configuration["Jwt:Audience"],
                        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                    };
                    var token = tokenHandler.CreateToken(tokenDescriptor);
                    var jwtString = tokenHandler.WriteToken(token);

                    // 2. Append the token to an HttpOnly cookie so the browser sends it automatically
                    Response.Cookies.Append("jwt", jwtString, new CookieOptions { HttpOnly = true, Secure = true });

                    // 3. Check user role and redirect
                    if (user.role == "admin")
                    {
                        return RedirectToAction("Index", "Admin");
                    }

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("password", "Incorrect password.");
                    return View();
                }
            }
            else
            {
                ModelState.AddModelError("email", "User not found.");
                return View();
            }
        }

        [HttpPost]
        public IActionResult logout()
        {
            Response.Cookies.Delete("jwt");
            return RedirectToAction("login");
        }
        
    }
}
