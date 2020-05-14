using System;
using System.Collections.Generic;
using System.Linq;
using cw3.Models;


namespace cw3.DAL
{
    public interface IDbService
    {
        public IEnumerable<Student> GetStudents();
        public IEnumerable<Enrollment> GetStudentEnrollment(string studentId);
        public Study GetStudy(string studyName);
    }
}