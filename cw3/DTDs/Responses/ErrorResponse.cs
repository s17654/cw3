using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace cw3.DTDs.Responses
{
    public class ErrorResponse
    {
        [Required]
        public string Message { get; set; }
    }
}