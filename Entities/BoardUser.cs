using System;
using System.Collections.Generic;

namespace HeroServer
{
    public class BoardUser
    {
        public int Id { get; set; }
        public int WebSysUserId { get; set; }
        public String FirstName1 { get; set; }
        public String FirstName2 { get; set; }
        public String LastName1 { get; set; }
        public String LastName2 { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public int BoardUserStatusId { get; set; }


        public BoardUser()
        {
        }

        public BoardUser(int id, int webSysUserId, String firstName1, String firstName2, String lastName1, String lastName2, DateTime createDateTime, DateTime updateDateTime, int boardUserStatusId)
        {
            Id = id;
            WebSysUserId = webSysUserId;
            FirstName1 = firstName1;
            FirstName2 = firstName2;
            LastName1 = lastName1;
            LastName2 = lastName2;
            CreateDateTime = createDateTime;
            UpdateDateTime = updateDateTime;
            BoardUserStatusId = boardUserStatusId;
        }

        public BoardUser(int id, int webSysUserId, String firstName1, String firstName2, String lastName1, String lastName2, int boardUserStatusId)
        {
            Id = id;
            WebSysUserId = webSysUserId;
            FirstName1 = firstName1;
            FirstName2 = firstName2;
            LastName1 = lastName1;
            LastName2 = lastName2;
            CreateDateTime = DateTime.Now;
            UpdateDateTime = DateTime.Now;
            BoardUserStatusId = boardUserStatusId;
        }

        public String GetCompleteName() => FirstName1 + (String.IsNullOrEmpty(FirstName2) ? "" : " " + FirstName2) + " " + LastName1 + (String.IsNullOrEmpty(LastName2) ? "" : " " + LastName2);
    }
}
