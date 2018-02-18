using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrderCenterClient.Models
{
    public class PatientArea
    {
        public int Id { get; set; }

        public string AreaCode { get; set; }

        public bool IsActive { get; set; }
    }
}