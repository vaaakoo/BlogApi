using BlogServer.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using System.Text;
using BlogServer.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;


namespace BlogServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _authContext;


        public UserController(AppDbContext context)
        {
            _authContext = context;


            if (!_authContext.Users.Any(u => u.Email == "admin"))
            {
                SeedAdminUser();
            }

        }
        private void SeedAdminUser()
        {
            var adminUser = new User
            {
                FirstName = "Admin",
                LastName = "Admin",
                Password = "admin",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin"),
                IdNumber = "00000000000", 
                Email = "admin"
            };

            _authContext.Users.Add(adminUser);
            _authContext.SaveChanges();
        }




        [HttpPost("register")]
        public async Task<IActionResult> AddUser([FromBody] User userObj)
        {
            if (userObj == null)
                return BadRequest();

            //check username
            if (userObj.Email == "admin" && userObj.Password == "admin")
            {
                return BadRequest(new { Message = "შეცდომაა!" });
            }

            if (!IsValidEmail(userObj.Email))
                return BadRequest(new { Message = "მეილის ფორმატი არასწორია!" });

            // check email
            if (await CheckEmailExistAsync(userObj.Email))
                return BadRequest(new { Message = "ეს მეილი გამოყენებულია! გთხოვთ გაიაროთ ავტორიზაცია." });

            //check username
            if (await CheckIdNumberExistAsync(userObj.IdNumber))
                return BadRequest(new { Message = "შეცდომა! პირადი ნომერი უნდა შეიცავდეს 11 ციფრს!" });


            var passMessage = CheckPasswordStrength(userObj.Password);
            if (!string.IsNullOrEmpty(passMessage))
                return BadRequest(new { Message = passMessage.ToString() });


            userObj.PasswordHash = BCrypt.Net.BCrypt.HashPassword(userObj.Password);


            await _authContext.AddAsync(userObj);
            await _authContext.SaveChangesAsync();

            return Ok(new
            {
                Status = 200,
                Message = "რეგისტრაცია გავლილია!"
            });
        }


        /*[HttpPut("updateDoctor/{id}")]
        public async Task<IActionResult> UpdateDoctorAsync(int id, [FromBody] Doctor updatedDoctor)
        {
            try
            {
                var existingDoctor = _authContext.Users.FirstOrDefault(u => u.Id == id && u.Category != null);

                if (existingDoctor == null)
                {
                    return NotFound();
                }
                
                if (!string.IsNullOrEmpty(updatedDoctor.FirstName))
                {
                    existingDoctor.FirstName = updatedDoctor.FirstName;
                }

                if (!string.IsNullOrEmpty(updatedDoctor.LastName))
                {
                    existingDoctor.LastName = updatedDoctor.LastName;
                }

                if (!string.IsNullOrEmpty(updatedDoctor.Email))
                {
                    // Check if email is valid
                    if (!IsValidEmail(updatedDoctor.Email))
                        return BadRequest(new { Message = "Invalid email format" });
                    if (await CheckEmailExistAsync(updatedDoctor.Email))
                        return BadRequest(new { Message = "Email Already Exist" });
                    existingDoctor.Email = updatedDoctor.Email;
                }

                if (!string.IsNullOrEmpty(updatedDoctor.Password))
                {
                    var passMessage = CheckPasswordStrength(updatedDoctor.Password);
                    if (!string.IsNullOrEmpty(passMessage))
                        return BadRequest(new { Message = passMessage.ToString() });
                    existingDoctor.Password = updatedDoctor.Password;
                    existingDoctor.PasswordHash = BCrypt.Net.BCrypt.HashPassword(existingDoctor.Password);
                }

                if (!string.IsNullOrEmpty(updatedDoctor.IdNumber))
                {
                    //check username
                    if (await CheckIdNumberExistAsync(updatedDoctor.IdNumber))
                        return BadRequest(new { Message = "Invalid or already existing ID number. ID number must be 11 digits." });
                    existingDoctor.IdNumber = updatedDoctor.IdNumber;
                }

                if (!string.IsNullOrEmpty(updatedDoctor.Category))
                {
                    existingDoctor.Category = updatedDoctor.Category;
                }
                if (updatedDoctor.starNum.HasValue)
                {
                    existingDoctor.starNum = updatedDoctor.starNum.GetValueOrDefault();
                }


                _authContext.SaveChanges();

                return Ok(existingDoctor);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
                return StatusCode(500, new { Message = "Internal Server Error" });
            }
        }*/


        /*[HttpDelete("deleteDoctor/{id}")]
        public IActionResult DeleteDoctor(int id)
        {
            try
            {
                var doctorToDelete = _authContext.Users.FirstOrDefault(u => u.Id == id && u.Category != null);

                if (doctorToDelete == null)
                {
                    return NotFound();
                }

                _authContext.Users.Remove(doctorToDelete);
                _authContext.SaveChanges();

                return Ok(new { Message = "Doctor deleted successfully" });
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
                return StatusCode(500, new { Message = "Internal Server Error" });
            }
        }*/


        [HttpGet("getUser")]
        public IActionResult GetUsers()
        {
            try
            {
                var users = _authContext.Users.ToList();
                return Ok(users);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);

                return StatusCode(500, new { Message = "Internal Server Error" });
            }
        }

        [HttpGet("getUser/{id}")]
        public IActionResult GetUserById(int id)
        {
            try
            {
                var user = _authContext.Users.FirstOrDefault(u => u.Id == id);

                if (user == null)
                {
                    return NotFound();
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
                return StatusCode(500, new { Message = "Internal Server Error" });
            }
        }

        private string CheckPasswordStrength(string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                return "Password cannot be empty.";
            }

            // Define the regular expressions for password requirements
            var hasMinimumLength = new Regex(@".{12,}");
            var hasLowerChar = new Regex(@"[a-z]");
            var hasUpperChar = new Regex(@"[A-Z]");
            var hasDigit = new Regex(@"\d");
            var hasSpecialChar = new Regex(@"[!@#$%^&*()_+{}\[\]:;<>,.?~\\-]");

            // Check if the password meets each requirement
            if (!hasMinimumLength.IsMatch(password))
            {
                return "პაროლი უნდა შეიცავდეს მინიმუმ 12 სიმბოლოს.";
            }

            if (!hasLowerChar.IsMatch(password))
            {
                return "პაროლი უნდა შეიცავდეს პატარა ანბანის ასოს!";
            }

            if (!hasUpperChar.IsMatch(password))
            {
                return "პაროლი უნდა შეიცავდეს ერთ დიდ ანბანის ასოს.";
            }

            if (!hasDigit.IsMatch(password))
            {
                return "პაროლი უნდა შეიცავდეს ციფრს!";
            }

            if (!hasSpecialChar.IsMatch(password))
            {
                return "პაროლი უნდა შეიცავდეს ერთ სიმბოლოს!";
            }

            return string.Empty;
        }


        private async Task<bool> CheckEmailExistAsync(string? email)
            => await _authContext.Users.AnyAsync(x => x.Email.Equals(email));


        private async Task<bool> CheckIdNumberExistAsync(string? idNumber)
        {
            if (idNumber == null || idNumber.Length != 11 || !idNumber.All(char.IsDigit))
            {
                return true;
            }

            return await _authContext.Users.AnyAsync(x => x.IdNumber == idNumber);
        }


        private bool IsValidEmail(string email)
        {
            string emailRegex = @"^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$";

            return System.Text.RegularExpressions.Regex.IsMatch(email, emailRegex);
        }
    }


}
