﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContracts
{
    public interface IReceiveErrorsService
    {
        Task<MemoryStream> GetErrorsExcel(List<string> errors);
    }
}