using System;

namespace HeroServer
{
    public class BoardUserFull
    {
        public BoardUser BoardUser { get; set; }
        public WebSysUser WebSysUser { get; set; }

        public BoardUserFull()
        {
        }

        public BoardUserFull(BoardUser boardUser, WebSysUser webSysUser)
        {
            BoardUser = boardUser;
            WebSysUser = webSysUser;
        }
    }
}
