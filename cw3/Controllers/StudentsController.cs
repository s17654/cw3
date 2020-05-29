using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using cw3.DAL;
using cw3.DTDs.Requests;
using cw3.DTDs.Responses;
using cw3.Models;
using cw3.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace cw3.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly IStudentsDbService _dbService;
        private readonly IConfiguration _configuration;

        public StudentsController(IStudentsDbService dbService, IConfiguration configuration)
        {
            _dbService = dbService;
            _configuration = configuration;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public IActionResult Login(LoginRequest request)
        {
            var student = _dbService.GetStudent(request.Username, request.Password);
            if (student == null)
                return NotFound(new ErrorResponse
                {
                    Message = "Username or password dosen't exists or is incorrect."
                });

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, student.IndexNumber),
                new Claim(ClaimTypes.Name, student.FirstName + "_" + student.LastName),
                new Claim(ClaimTypes.Role, "student")
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["SecretKey"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken
            (
                issuer: "s17654",
                audience: "Students",
                claims: claims,
                expires: DateTime.Now.AddMinutes(10),
                signingCredentials: credentials
            );
            var response = new LoginResponse
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                RefreshToken = Guid.NewGuid().ToString()
            };
            if (_dbService.CreateRefreshToken(
                new RefreshToken { Id = response.RefreshToken, IndexNumber = student.IndexNumber }) > 0)
                return Ok(response);
            else
                return StatusCode(500, new ErrorResponse
                {
                    Message = "Error during post authorization"
                });
        }

        [HttpPost("refresh-token/{refreshToken}")]
        [AllowAnonymous]
        public IActionResult RefreshToken(string refreshToken)
        {
            var student = _dbService.GetRefreshTokenOwner(refreshToken);
            if (student == null)
                return NotFound(new ErrorResponse
                {
                    Message = "Refresh roken dosen't exists or is incorrect"
                });

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, student.IndexNumber),
                new Claim(ClaimTypes.Name, student.FirstName + "_" + student.LastName),
                new Claim(ClaimTypes.Role, "student")
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["SecretKey"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken
            (
                issuer: "s16556",
                audience: "Students",
                claims: claims,
                expires: DateTime.Now.AddMinutes(10),
                signingCredentials: credentials
            );
            var response = new LoginResponse
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                RefreshToken = Guid.NewGuid().ToString()
            };
            if (_dbService.CreateRefreshToken(
                new RefreshToken
                {
                    Id = response.RefreshToken,
                    IndexNumber = student.IndexNumber
                }) == 0)
                return StatusCode(500, new ErrorResponse
                {
                    Message = "Error during post authorization"
                });

            if (_dbService.DeleteRefreshToken(refreshToken) == 0)
                return StatusCode(500, new ErrorResponse
                {
                    Message = "Error during post authorization"
                });
            return Ok(response);
        }


        //Zadanie 4.1, 4.2 ,4.5
        [HttpGet]
        public IActionResult GetStudents()
        {
            var list = new List<Student>();
            using (SqlConnection con = new SqlConnection(Program.ConString))
            using (SqlCommand com = new SqlCommand())
            {
                com.Connection = con;
                com.CommandText = "select * from students";

                con.Open();
                SqlDataReader dr = com.ExecuteReader();
                while (dr.Read())
                {
                    var st = new Student();
                    st.IndexNumber = dr["IndexNumber"].ToString();
                    st.FirstName = dr["FirstName"].ToString();
                    st.LastName = dr["LastName"].ToString();
                    list.Add(st);
                }
            }
            return Ok(list);
        }

        //Zadanie 4.3
        [HttpGet("{indexNumber}")]
        public IActionResult GetEnrollments(string indexNumber)
        {
            var list = new List<Enrollment>();
            using (SqlConnection con = new SqlConnection(Program.ConString))
            using (SqlCommand com = new SqlCommand())
            {
                com.Connection = con;
                com.CommandText = "select * from Enrollment e join student s on s.IdEnrollment=e.IdEnrollment where s.IndexNumber=@index";
                com.Parameters.AddWithValue("index", indexNumber);

                con.Open();
                var dr = com.ExecuteReader();
                while (dr.Read())
                {
                    var en = new Enrollment();
                    en.IdEnrollment = int.Parse(dr["IdEnrollment"].ToString());
                    en.Semester = int.Parse(dr["Semester"].ToString());
                    en.StartDate = DateTime.Parse(dr["StartDate"].ToString());
                    list.Add(en);
                }
            }
            return Ok(list);
        }

        //**********************************************//
        //**  PONIŻEJ WYKOMENTOWANY KOD Z CWICZEN 3   **//
        //**********************************************//


        //    private readonly IDbService _dbService;

        //    public StudentsController(IDbService dbService)
        //    {
        //        _dbService = dbService;
        //    }

        //    [HttpGet]
        //    public IActionResult GetStudents(string orderBy)
        //    {
        //        return Ok(_dbService.GetStudents());
        //    }

        //    //[HttpGet]
        //    //public string GetStudent()
        //    //{
        //    //    return "Kowalski, Malewski, Andrzejewski";
        //    //}

        //    [HttpGet("{id}")]
        //    public IActionResult GetStudents(int id)
        //    {
        //        if (id == 1)
        //        {
        //            return Ok("Kowalski");
        //        }
        //        else if (id == 2)
        //        {
        //            return Ok("Malewski");
        //        }

        //        return NotFound("Nie znaleziono studenta");
        //    }

        //    //[HttpGet]
        //    //public string GetStudents(string orderBy)
        //    //{
        //    //    return $"Kowalski, Malewski, Andrzejewski sortowanie={orderBy}";
        //    //}

        //    [HttpPost]
        //    public IActionResult CreateStudent(Student student)
        //    {
        //        student.IndexNumber = $"s{new Random().Next(1, 20000)}";
        //        return Ok(student);
        //    }

        //    [HttpPut("{id}")]
        //    public IActionResult updateStudent(int id)
        //    {
        //        return Ok("Aktualizacja dokończona");
        //    }

        //    [HttpPut("{id}")]
        //    public IActionResult deleteStudent(int id)
        //    {
        //        return Ok("Usuwanie zakończone");
        //    }
    }
}