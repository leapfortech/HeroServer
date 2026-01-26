using Newtonsoft.Json;
using System;
using System.Text;

namespace HeroServer
{
    public class FeedCursor
    {
        public DateTime PublicationDateTime { get; set; }
        public long PostId { get; set; }

        public static FeedCursor DecodeCursor(String cursor)
        {
            if (String.IsNullOrEmpty(cursor))
                return null;

            String json = Encoding.UTF8.GetString(Convert.FromBase64String(cursor));
            return JsonConvert.DeserializeObject<FeedCursor>(json);
        }

        public static String EncodeCursor(PostFull post)
        {
            FeedCursor cursor = new FeedCursor
            {
                PublicationDateTime = post.PublicationDateTime,
                PostId = post.PostId
            };

            String json = JsonConvert.SerializeObject(cursor);
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(json));
        }
    }
}
