using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MyEvents_API.Context;
using MyEvents_API.Helpers;
using MyEvents_API.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;

namespace MyEvents_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _appdbcontext;

        public UserController(AppDbContext appdbcontext)
        {
            _appdbcontext = appdbcontext;
        }

        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] User_Registration objuserreg)
        {
            if (objuserreg == null)
            {
                return BadRequest(new {Message="No User Data!!"});
            }

            var email = await _appdbcontext.UserRegistration.FirstOrDefaultAsync(x => x.Email == objuserreg.Email);
            if (email == null)
            {
                return NotFound(new { Message = "User Not Found!" });
            }
            else
            {
                if (!PasswordHasher.VerifyPassword(objuserreg.Password, email.Password))
                {
                    return BadRequest(new { Message = "Incorrect Password!!" });
                }
                return Ok(new { Message = "Login Successfull!!" });
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> UserRegister([FromBody] User_Registration objUreg)
        {
            if (objUreg == null)
            {
                return BadRequest();
            }
            else
            {
                //check Email
                if(await CheckEmailExistAsync(objUreg.Email))
                {
                    return BadRequest(new { Message = "Email Already Exist!!" });
                }
                //check UserName
                if (await CheckUserNameExistAsync(objUreg.UserName))
                {
                    return BadRequest(new { Message = "UserName Already Exist!!" });
                }
                //Check password Strength
                var pass=CheckPasswordStrength(objUreg.Password);
                if(!string.IsNullOrEmpty(pass))
                {
                    return BadRequest(new {Message=pass.ToString()});
                }

                objUreg.Password=PasswordHasher.HashPassword(objUreg.Password);
                //objUreg.Role=
                //objUreg.Token=
                await _appdbcontext.UserRegistration.AddAsync(objUreg);
                await _appdbcontext.SaveChangesAsync();
                return Ok(new { message = "User Registered!" });
            }
        }

        private Task<bool>CheckEmailExistAsync(string email)=>
            _appdbcontext.UserRegistration.AnyAsync(x => x.Email == email);

        private Task<bool> CheckUserNameExistAsync(string username) =>
           _appdbcontext.UserRegistration.AnyAsync(x => x.Email == username);

        private string CheckPasswordStrength(string password)
        {
            StringBuilder sb=new StringBuilder();
            if(password.Length<8)
                sb.Append("Minimum password length should be 8"+ Environment.NewLine);
            if(!(Regex.IsMatch(password,"[a-z]")) && Regex.IsMatch(password,"[A-Z]") && Regex.IsMatch(password,"[0-9]"))
                sb.Append("Password Should be Alphanumeric" + Environment.NewLine);
            if (!Regex.IsMatch(password, "[<,>,.,;,:,',',{,},(,),_,-,+,=,|,/,!,@,#,$,%,^,&,*,`,~]"))
                sb.Append("Password should contains special characters" + Environment.NewLine);
            return sb.ToString();
        }
    }

    private string  CreateJwt(User_Registration usereg)
    {
        var JwtTokenHandle = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes("veryveryscrect.....");
        var identity = new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.Role,usereg.Role),
            new Claim(ClaimTypes.Name,usereg.UserName)
        });
        var credentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);
        var tokenDescription = new SecurityTokenDescriptor
        {
            Subject = identity,
            Expires = DateTime.Now.AddDays(1),
            SigningCredentials = credentials,
        };
        var token = JwtTokenHandle.CreateToken(tokenDescription);
        return JwtTokenHandle.WriteToken(token);
    }
}
