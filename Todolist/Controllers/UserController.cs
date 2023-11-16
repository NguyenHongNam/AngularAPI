using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Todolist.Data;
using Todolist.Helper;
using Todolist.Models;
using System;
namespace Todolist.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly TodoListDBContext _todoListDBContext;
        public UserController(TodoListDBContext todoListDBContext)
        {
            _todoListDBContext = todoListDBContext;
        }
        [HttpGet]
        public IActionResult GetUser()
        {
            var user = _todoListDBContext.Users.ToList();
            return Ok(user);
        }

        [HttpGet("{id}")]
        public IActionResult GetUserById(int id)
        {
            var user = _todoListDBContext.Users.SingleOrDefault(t => t.UserID == id);
            if (user != null)
            {
                return Ok(user);
            }
            else
            {
                return NotFound();
            }
        }
        [HttpPut("{id}")]
        public IActionResult EditUser(int id, Users users)
        {
            var user = _todoListDBContext.Users.SingleOrDefault(t => t.UserID == id);
            if (user != null)
            {
                user.UserName = user.UserName;
                user.Password = user.Password;
                user.Email = user.Email;
                _todoListDBContext.SaveChanges();

                return NoContent();
            }
            else
            {
                return NotFound();
            }
        }
        [HttpDelete("{id}")]
        public IActionResult DeleteUser(int id)
        {
            try
            {
                var user = _todoListDBContext.Users.SingleOrDefault(t => t.UserID == id);
                if (user == null)
                {
                    return NotFound();
                }
                _todoListDBContext.Remove(user);
                _todoListDBContext.SaveChanges();
                return Ok();
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] Users user)
        {
            if(user == null)
                return BadRequest();
            var u = await _todoListDBContext.Users.FirstOrDefaultAsync(x => x.UserName == user.UserName);
            if (u == null)
                return NotFound(new {Message = "Không tìm thấy tài khoản!"});
            if(!PasswordHasher.VerifyPassword(user.Password, u.Password))
            {
                return BadRequest(new { Message = "Sai mật khẩu" });
            }
            u.Token = CreateJwt(u);
            return Ok(new
            {
                Token = u.Token,
                Message = "Đăng nhập thành công!"
            });
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] Users user) {
            if (user == null)
                return BadRequest();
            //Check username
            if(await CheckUserNameAsync(user.UserName))
                return BadRequest(new {Message="Tên đã tồn tại!"});

            //Check Email
            if (await CheckEmailAsync(user.Email))
                return BadRequest(new { Message = "Tên đã tồn tại!" });


            user.Password = PasswordHasher.HashPassword(user.Password);
            user.Token = "";
            user.Role = "User";
            _todoListDBContext.Users.Add(user);
            _todoListDBContext.SaveChanges();
            return Ok(new
            {
                Message = "Đăng ký thành công!"
            });
        }

        private string CreateJwt( Users users)
        {
            var JwtTokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("veryverysecret.....");
            var identity = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Role, users.Role),
                new Claim(ClaimTypes.Name,$"{users.UserName}")
            });
            var credentials = new SigningCredentials(new SymmetricSecurityKey(key),SecurityAlgorithms.HmacSha256);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = identity,
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = credentials
            };
            var token = JwtTokenHandler.CreateToken(tokenDescriptor);
            return JwtTokenHandler.WriteToken(token);
        }

        private Task<bool> CheckUserNameAsync(string username)
        =>  _todoListDBContext.Users.AnyAsync(x => x.UserName == username);

        private Task<bool> CheckEmailAsync(string email)
        => _todoListDBContext.Users.AnyAsync(x => x.Email == email);

    }
}

