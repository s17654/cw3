
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using cw3.Models;

namespace cw3.DAL
{
    public class DbService : IDbService
    {
        public Student GetStudent(String indexNumber)
        {
            Student student = new Student();

            try
            {
                using (var connection = new SqlConnection(Program.ConString))
                using (var command = new SqlCommand())
                {
                    command.Connection = connection;

                    command.CommandText = "SELECT * FROM STUDENT WHERE IndexNumber = @indexNumber";
                    command.Parameters.AddWithValue("indexNumber", indexNumber);

                    connection.Open();

                    var reader = command.ExecuteReader();

                    if (!reader.Read())
                    {
                        return null;
                    }

                    student.FirstName = reader["FirstName"].ToString();
                    student.LastName = reader["LastName"].ToString();
                    student.IndexNumber = reader["IndexNumber"].ToString();
                    student.Birthday = (DateTime)reader["BirthDate"];

                    connection.Close();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                return null;
            }

            return student;
        }

        public IEnumerable<Student> GetStudents()
        {
            List<Student> students = new List<Student>();

            using (var connection = new SqlConnection(Program.ConString))
            using (var command = new SqlCommand())
            {
                command.Connection = connection;

                command.CommandText = "SELECT * FROM STUDENT";

                connection.Open();

                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Student student = new Student();

                    student.FirstName = reader["FirstName"].ToString();
                    student.LastName = reader["LastName"].ToString();
                    student.IndexNumber = reader["IndexNumber"].ToString();

                    students.Add(student);
                }

                connection.Close();
            }

            return students;
        }

        public IEnumerable<Enrollment> GetStudentEnrollment(string studentId)
        {
            List<Enrollment> enrollments = new List<Enrollment>();

            using (var connection = new SqlConnection(Program.ConString))
            using (var command = new SqlCommand())
            {
                command.Connection = connection;

                command.CommandText = $"SELECT * FROM dbo.ENROLLMENT WHERE IdEnrollment = (SELECT IDENROLLMENT FROM dbo.STUDENT WHERE INDEXNUMBER = @studentId)";
                // SQL Injection -> studentId = 1); DROP TABLE STUDENT; --
                command.Parameters.AddWithValue("studentId", studentId);

                connection.Open();

                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Enrollment enrollment = new Enrollment();

                    Console.WriteLine(reader);

                    enrollment.Semester = (int)reader["Semester"];
                    enrollment.IdEnrollment = (int)reader["IdEnrollment"];
                    enrollment.IdStudy = (int)reader["IdStudy"];
                    enrollment.StartDate = System.Convert.ToDateTime(reader["StartDate"].ToString());

                    enrollments.Add(enrollment);
                }

                connection.Close();
            }

            return enrollments;
        }

        public Study GetStudy(string studyName)
        {
            using (var connection = new SqlConnection(Program.ConString))
            using (var command = new SqlCommand())
            {
                command.Connection = connection;

                command.CommandText = "SELECT * FROM STUDIES WHERE NAME = @studyName";
                command.Parameters.AddWithValue("studyName", studyName);

                connection.Open();

                var reader = command.ExecuteReader();

                Study study = new Study();

                if (!reader.Read())
                {
                    return null;
                }

                study.IdStudy = (int)reader["IdStudy"];
                study.Name = reader["Name"].ToString();

                connection.Close();

                return study;
            }
        }
    }
}