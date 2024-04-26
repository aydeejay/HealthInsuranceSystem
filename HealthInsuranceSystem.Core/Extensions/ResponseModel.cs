using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthInsuranceSystem.Core.Extensions
{
    public class ResponseModel
    {
        public bool IsSuccessful { get; set; }
        public string? Message { get; set; }
        public string? HasErrors { get; set; }
        public bool Data { get; set; }
    }
    public class ResponseModel<T> : ResponseModel
    {
        public T Data { get; set; }

    }
}

