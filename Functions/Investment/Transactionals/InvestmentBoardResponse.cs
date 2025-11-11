using System;

namespace HeroServer
{
    public class InvestmentBoardResponse
    {
        public int InvestmentId { get; set; }
        public int AppUserId { get; set; }
        public int BoardUserId { get; set; }
        public int InvestmentMotiveId { get; set; }
        public String BoardComment { get; set; }

        public InvestmentBoardResponse()
        {
        }

        public InvestmentBoardResponse(int investmentId, int appUserId, int boardUserId, int investmentMotiveId, String boardComment)
        {
            InvestmentId = investmentId;
            AppUserId = appUserId;
            BoardUserId = boardUserId;
            InvestmentMotiveId = investmentMotiveId;
            BoardComment = boardComment;
        }
    }
}
