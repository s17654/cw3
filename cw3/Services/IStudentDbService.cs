﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cw3.DTDs.Requests;
using Microsoft.AspNetCore.Mvc;

namespace cw3.Services
{
    public interface IStudentsDbService
    {
        public bool PromoteStudent(PromoteStudentRequest promoteStudentRequest);

        public StudentEnrollment EnrollStudent(EnrollStudentRequest enrollStudentRequest);

    }
}