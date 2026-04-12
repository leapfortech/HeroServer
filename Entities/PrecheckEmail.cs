using System;
using System.Collections.Generic;

namespace HeroServer
{
    public class PrecheckEmail
    {
        public long Id { get; set; }
        public String Email { get; set; }
        public String Code { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public int Status { get; set; }

        public PrecheckEmail()
        { 
        }

        public PrecheckEmail(long id, String email, String code, DateTime createDateTime, DateTime updateDateTime, int status)
        {
            Id = id;
            Email = email;
            Code = code;
            CreateDateTime = createDateTime;
            UpdateDateTime = updateDateTime;
            Status = status;
        }
    }
}
