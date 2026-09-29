using System;
using System.Collections.Generic;

namespace HeroServer
{
    public class HappeningFull : PostFull
    {
        public long Id { get; set; }
        public long HappeningTypeId { get; set; }
        public long CountryId { get; set; }
        public long StateId { get; set; }
        public int IsPublic { get; set; }
        public int HasSignup { get; set; }
        public int HasPayment { get; set; }
        public String PaymentDetails { get; set; }
        public DateTime? StartDateTime { get; set; }
        public DateTime? EndDateTime { get; set; }
        public String Location { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public int Status { get; set; }


        public HappeningFull(long id, long postId, long appUserId, String appUserAlias,
                             long postTypeId, long postCountryId, long postStateId,
                             String title, String titleImage, String description,
                             int imageCount, int favoriteCount, int[] reactionCounts, long reactionPhraseId, int commentCount,
                             DateTime publicationDateTime, int postStatus,
                             AppUserInfo appUserInfo, ContactFull contactFull, List<LinkFull> linkFulls, List<CommentFull> commentFulls, List<String> images,
                             long happeningTypeId, long countryId, long stateId, int isPublic, int hasSignup, int hasPayment, String paymentDetails,
                             DateTime? startDateTime, DateTime? endDateTime, String location, double? latitude, double? longitude, int status)
            : base(postId, appUserId, appUserAlias, postTypeId, countryId, stateId, title, titleImage, description,
                   imageCount, favoriteCount, reactionCounts, reactionPhraseId, commentCount, publicationDateTime, postStatus,
                   appUserInfo, contactFull, linkFulls, commentFulls, images)
        {
            Id = id;
            HappeningTypeId = happeningTypeId;
            CountryId = countryId;
            StateId = stateId;
            IsPublic = isPublic;
            HasSignup = hasSignup;
            HasPayment = hasPayment;
            PaymentDetails = paymentDetails;
            StartDateTime = startDateTime;
            EndDateTime = endDateTime;
            Location = location;
            Latitude = latitude;
            Longitude = longitude;
            Status = status;
        }
    }
}

