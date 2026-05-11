using System;

namespace HeroServer
{
    public class BoardUserFull
    {
        public BoardUser BoardUser { get; set; }
        public WebSysUser WebSysUser { get; set; }
        public Identity Identity { get; set; }

        public BoardUserFull()
        {
        }

        public BoardUserFull(BoardUser boardUser, WebSysUser webSysUser, Identity identity)
        {
            BoardUser = boardUser;
            WebSysUser = webSysUser;
            Identity = identity;
        }
    }
}
