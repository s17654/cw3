using cw3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cw3.DAL
{
    public class MockDbService : IDbService
    {
        private static IEnumerable<Student> _students;

        static MockDbService()
        {
            _students = new List<Student>
            {
                new Student{FirstName="Jan", LastName="Kowalski", IndexNumber = "s123456"},
                new Student{FirstName="Anna", LastName="Malewski", IndexNumber = "s123123"},
                new Student{FirstName="Andrzej", LastName="Andrzejewicz", IndexNumber = "s123789"},
            };
        }

        public IEnumerable<Enrollment> GetStudentEnrollment(string studentId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Student> GetStudents()
        {
            return _students;
        }

        public Study GetStudy(string studyName)
        {
            throw new NotImplementedException();
        }
    }
}