using System;
using System.Data;
using System.Data.SqlClient;
using cw3.DTDs.Requests;
using cw3.Models;

namespace cw3.Services
{
    public class StudentsDbService : IStudentsDbService
    {
        public bool PromoteStudent(PromoteStudentRequest promoteStudentRequest)
        {
            using (var connection = new SqlConnection(Program.ConString))
            using (var command = new SqlCommand())
            {
                command.Connection = connection;
                command.Connection.Open();

                command.Transaction = command.Connection.BeginTransaction();

                try
                {
                    command.CommandText =
                        "SELECT * FROM ENROLLMENT WHERE IdStudy = (SELECT IdStudy FROM STUDIES WHERE NAME = @StudyName) AND Semester = @Semester";
                    command.Parameters.AddWithValue("StudyName", promoteStudentRequest.Studies);
                    command.Parameters.AddWithValue("Semester", promoteStudentRequest.Semester);

                    var reader = command.ExecuteReader();

                    if (!reader.Read())
                    {
                        reader.Close();
                        return false;
                    }

                    reader.Close();
                    command.Parameters.Clear();
                }
                catch (Exception error)
                {
                    Console.WriteLine(error);
                    command.Transaction.Rollback();
                    return false;
                }

                try
                {
                    command.CommandText = "PromoteStudents";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@StudiesName", promoteStudentRequest.Studies);
                    command.Parameters.AddWithValue("@OldSemester", promoteStudentRequest.Semester);

                    command.ExecuteNonQuery();
                }
                catch (Exception error)
                {
                    Console.WriteLine(error);
                    command.Transaction.Rollback();
                    return false;
                }

                command.Transaction.Commit();
            }

            return true;
        }

        public StudentEnrollment EnrollStudent(EnrollStudentRequest enrollStudentRequest)
        {
            Student student = new Student
            {
                FirstName = enrollStudentRequest.FirstName,
                LastName = enrollStudentRequest.LastName,
                IndexNumber = enrollStudentRequest.IndexNumber,
                Birthday = enrollStudentRequest.BirthDate,
            };

            Study study;
            try
            {
                study = this.GetStudy(enrollStudentRequest.Studies);
            }
            catch (Exception exception)
            {
                throw exception;
            }

            DateTime enrollmentStartDate = DateTime.Now;

            using (var connection = new SqlConnection(Program.ConString))
            using (var command = new SqlCommand())
            {
                command.Connection = connection;
                command.Connection.Open();
                var transaction = command.Connection.BeginTransaction();
                command.Transaction = transaction;

                try
                {
                    command.CommandText = "SELECT IdEnrollment FROM ENROLLMENT WHERE IdStudy = @IdStudy AND Semester = 1";
                    command.Parameters.AddWithValue("IdStudy", study.IdStudy);

                    var reader = command.ExecuteReader();

                    if (!reader.Read())
                    {
                        command.CommandText =
                            "INSERT INTO ENROLLMENT (Semester, IdStudy, StartDate) VALUES (1, @IdStudy, @StartDate)";
                        command.Parameters.AddWithValue("IdStudy", study.IdStudy);
                        command.Parameters.AddWithValue("StartDate", enrollmentStartDate);

                        command.ExecuteNonQuery();
                    }

                    reader.Close();
                }
                catch (Exception error)
                {
                    transaction.Rollback();
                    Console.WriteLine(error);
                    throw new Exception("Enrollment read error");
                }

                try
                {
                    command.CommandText = "SELECT * FROM STUDENT WHERE IndexNumber = @studentIndexNumber";
                    command.Parameters.AddWithValue("studentIndexNumber", student.IndexNumber);

                    var reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        reader.Close();
                        transaction.Rollback();
                        throw new Exception("Student already exists");
                    }

                    reader.Close();
                }
                catch (Exception error)
                {
                    transaction.Rollback();
                    Console.WriteLine(error);
                    throw new Exception("Student fetch failed");
                }

                try
                {
                    command.CommandText =
                        "SELECT IdEnrollment FROM ENROLLMENT WHERE IdStudy = @IdStudy AND Semester = 1";

                    var reader = command.ExecuteReader();

                    if (!reader.Read())
                    {
                        reader.Close();
                        transaction.Rollback();
                        throw new Exception("No enrollment");
                    }

                    var enrollmentId = (int)reader["IdEnrollment"];
                    reader.Close();

                    command.CommandText = "INSERT INTO STUDENT (IndexNumber, FirstName, LastName, BirthDate, IdEnrollment) VALUES (@IndexNumber, @FirstName, @LastName, @BirthDate, @IdEnrollment)";
                    command.Parameters.AddWithValue("IndexNumber", student.IndexNumber);
                    command.Parameters.AddWithValue("FirstName", student.FirstName);
                    command.Parameters.AddWithValue("LastName", student.LastName);
                    command.Parameters.AddWithValue("BirthDate", student.Birthday);
                    command.Parameters.AddWithValue("IdEnrollment", enrollmentId);

                    command.ExecuteNonQuery();
                }
                catch (Exception error)
                {
                    transaction.Rollback();
                    Console.WriteLine(error);
                    throw new Exception("Student enrollment fail");
                }

                transaction.Commit();
            }

            return new StudentEnrollment()
            {
                Semester = 1,
                LastName = student.LastName,
                StartDate = enrollmentStartDate
            };
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
                    throw new Exception("No studies");
                }

                study.IdStudy = (int)reader["IdStudy"];
                study.Name = reader["Name"].ToString();

                connection.Close();

                return study;
            }
        }
    }
}