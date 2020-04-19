using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using cw3.DAL;
using cw3.Models;
using cw3.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace cw3.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private const string ConString = "Data Source=db-mssql;Initial Catalog=s17654;Integrated Security=True";

        //Zadanie 4.1, 4.2 ,4.5
        [HttpGet]
        public IActionResult GetStudents()
        {
            var list = new List<Student>();
            using (SqlConnection con = new SqlConnection(ConString))
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
            using (SqlConnection con = new SqlConnection(ConString))
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