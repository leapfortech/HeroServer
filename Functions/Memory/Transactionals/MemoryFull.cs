using System;
using System.Collections.Generic;

namespace HeroServer
{
    public class MemoryFull : PostFull
    {
        public long Id { get; set; }
        public long MemoryTypeId { get; set; }
        public long CountryId { get; set; }
        public long StateId { get; set; }
        public DateTime? DateTime { get; set; }
        public String Location { get; set; }
        public int Status { get; set; }


        public MemoryFull(long id, long postId, long appUserId, String appUserAlias,
                             long postTypeId, long postCountryId, long postStateId,
                             String title, String titleImage, String description,
                             int imageCount, int favoriteCount, int[] reactionCounts, long reactionPhraseId, int commentCount,
                             DateTime publicationDateTime, int postStatus,
                             AppUserInfo appUserInfo, ContactFull contactFull, List<LinkFull> linkFulls, List<CommentFull> commentFulls, List<String> images,
                             long memoryTypeId, long countryId, long stateId,
                             DateTime? dateTime, String location, int status)
            : base(postId, appUserId, appUserAlias, postTypeId, postCountryId, postStateId, title, titleImage, description,
                   imageCount, favoriteCount, reactionCounts, reactionPhraseId, commentCount, publicationDateTime, postStatus,
                   appUserInfo, contactFull, linkFulls, commentFulls, images)
        {
            Id = id;
            MemoryTypeId = memoryTypeId;
            CountryId = countryId;
            StateId = stateId;
            DateTime = dateTime;
            Location = location;
            Status = status;
        }
    }
}

