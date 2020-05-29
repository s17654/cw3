using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cw3.DTDs.Requests;
using cw3.Models;
using Microsoft.AspNetCore.Mvc;

namespace cw3.Services
{
    public interface IStudentsDbService
    {
        public bool PromoteStudent(PromoteStudentRequest promoteStudentRequest);
        public StudentEnrollment EnrollStudent(EnrollStudentRequest enrollStudentRequest);
        public Study GetStudy(string studyName);
        public Student GetStudent(string indexNumber, string password);
        public Student GetRefreshTokenOwner(string refreshToken);
        public int CreateRefreshToken(RefreshToken refreshToken);
        public int DeleteRefreshToken(string refreshToken);


    }
}