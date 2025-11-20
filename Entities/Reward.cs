using System;

namespace HeroServer
{
    public class Reward
    {
        public long Id { get; set; }
        public long PlayerId { get; set; }
        public long RewardTypeId { get; set; }
        public DateTime? EarnedDateTime { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public int Status { get; set; }

        public Reward() { }

        public Reward(long id, long playerId, long rewardTypeId, DateTime? earnedDateTime, DateTime createDateTime,
                      DateTime updateDateTime, int status)
        {
            Id = id;
            PlayerId = playerId;
            RewardTypeId = rewardTypeId;
            EarnedDateTime = earnedDateTime;
            CreateDateTime = createDateTime;
            UpdateDateTime = updateDateTime;
            Status = status;
        }
    }
}
