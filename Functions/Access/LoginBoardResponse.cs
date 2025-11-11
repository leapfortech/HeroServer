using System;

namespace HeroServer
{
    public class LoginBoardResponse
    {
        public BoardUser BoardUser { get; set; }
        public WebSysUser WebSysUser { get; set; }
        public int Granted { get; set; }
        public String Message { get; set; }
        public String Link { get; set; }

        public LoginBoardResponse()
        {
        }

        public LoginBoardResponse(BoardUser boardUser, WebSysUser webSysUser, int granted, String message = null, String link = null)
        {
            BoardUser = boardUser;
            WebSysUser = webSysUser;
            Granted = granted;
            Message = message;
            Link = link;
        }
    }
}
