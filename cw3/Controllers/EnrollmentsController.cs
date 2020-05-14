using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using cw3.DTDs.Requests;
using cw3.DTDs.Responses;
using cw3.Models;
using cw3.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace cw3.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnrollmentsController : ControllerBase
    {
        private readonly IStudentsDbService _dbService;

        public EnrollmentsController(IStudentsDbService service)
        {
            _dbService = service;
        }

        [HttpPost("promotions")]
        public IActionResult PromoteStudent(PromoteStudentRequest req)
        {
            if (this._dbService.PromoteStudent(req))
            {
                return Ok();
            }
            return BadRequest();
        }

        [HttpPost]
        public IActionResult EnrollStudent(EnrollStudentRequest req)
        {
            StudentEnrollment se = this._dbService.EnrollStudent(req);

            EnrollStudentResponse esr = new EnrollStudentResponse()
            {
                Semester = se.Semester,
                LastName = se.LastName,
                StartDate = se.StartDate
            };

            return Ok();
        }
    }
}