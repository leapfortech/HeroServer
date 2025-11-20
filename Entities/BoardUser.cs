using System;
using System.Collections.Generic;

namespace HeroServer
{
    public class BoardUser
    {
        public long Id { get; set; }
        public long WebSysUserId { get; set; }
        public long EntityId { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public int BoardUserStatusId { get; set; }


        public BoardUser()
        {
        }

        public BoardUser(long id, long webSysUserId, long entityId, DateTime createDateTime, DateTime updateDateTime, int boardUserStatusId)
        {
            Id = id;
            WebSysUserId = webSysUserId;
            EntityId = entityId;
            CreateDateTime = createDateTime;
            UpdateDateTime = updateDateTime;
            BoardUserStatusId = boardUserStatusId;
        }

        public BoardUser(long id, long webSysUserId, long entityId, int boardUserStatusId)
        {
            Id = id;
            WebSysUserId = webSysUserId;
            EntityId = entityId;
            BoardUserStatusId = boardUserStatusId;
        }
    }
}
