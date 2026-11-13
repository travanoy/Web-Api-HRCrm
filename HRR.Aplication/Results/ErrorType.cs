using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRR.Aplication.Results;

public enum ErrorType
{
    NotFound,
    Conflict,
    Validation,
    BadRequest,
    Unauthorized
}
