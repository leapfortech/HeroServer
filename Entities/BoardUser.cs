using System;

namespace HeroServer
{
    public class BoardUser
    {
        public long Id { get; set; }
        public long WebSysUserId { get; set; }
        public String Alias { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public int BoardUserStatusId { get; set; }


        public BoardUser()
        {
        }

        public BoardUser(long id, long webSysUserId, String alias, DateTime createDateTime,
                         DateTime updateDateTime, int boardUserStatusId)
        {
            Id = id;
            WebSysUserId = webSysUserId;
            Alias = alias;
            CreateDateTime = createDateTime;
            UpdateDateTime = updateDateTime;
            BoardUserStatusId = boardUserStatusId;
        }

        public BoardUser(long id, long webSysUserId, String alias, int boardUserStatusId)
        {
            Id = id;
            WebSysUserId = webSysUserId;
            Alias = alias;
            BoardUserStatusId = boardUserStatusId;
        }
    }
}
