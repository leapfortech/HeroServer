using System.Collections.Generic;

namespace HeroServer
{
    public class BoardUserFullAllRsp
    {
        public int Page { get; set; }
        public int TotalPages { get; set; }
        public List<BoardUserFull> BoardUserFulls { get; set; }

        public BoardUserFullAllRsp()
        {
        }

        public BoardUserFullAllRsp(int page, int totalPages, List<BoardUserFull> boardUserFulls)
        {
            Page = page;
            TotalPages = totalPages;
            BoardUserFulls = boardUserFulls;
        }
    }
}