using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HeroServer
{
    public class HappeningFunctions
    {
        // GET
        public static async Task<List<Happening>> GetAllByStatus(int status)
        {
            return await new HappeningDB().GetAllByStatus(status);
        }

        public static async Task<Happening> GetById(long id)
        {
            return await new HappeningDB().GetById(id);
        }

        public static async Task<HappeningFull> GetFullById(long id)
        {
            return await new HappeningDB().GetFullById(id);
        }

        public static async Task<HappeningFull> GetFullByPostId(long postId)
        {
            return await new HappeningDB().GetFullByPostId(postId);
        }

        public static async Task<List<HappeningFull>> GetFullsByStatus(int status)
        {
            HappeningDataFull happeningDataFull = await new HappeningDB().GetDataFullByStatus(status);

            return GetFulls(happeningDataFull);
        }

        public static List<HappeningFull> GetFulls(HappeningDataFull happeningDataFull)
        {
            // ContactFull
            Dictionary<long, ContactFull> contactFullsDict = [];
            foreach (ContactFull contactFull in happeningDataFull.ContactFulls)
            {
                if (contactFullsDict.TryGetValue(contactFull.PostId, out ContactFull value))
                    throw new Exception($"Duplicate Contact for PostId {contactFull.PostId}");

                contactFullsDict[contactFull.PostId] = contactFull;
            }

            // LinkFull
            Dictionary<long, List<LinkFull>> linkFullsDict = [];
            foreach (LinkFull linkFull in happeningDataFull.LinkFulls)
            {
                if (linkFullsDict.TryGetValue(linkFull.PostId, out List<LinkFull> value))
                    value.Add(linkFull);
                else
                    linkFullsDict[linkFull.PostId] = [linkFull];
            }

            // CommentFull
            Dictionary<long, List<CommentFull>> commentFullsDict = [];
            foreach (CommentFull commentFull in happeningDataFull.CommentFulls)
            {
                if (commentFullsDict.TryGetValue(commentFull.PostId, out List<CommentFull> value))
                    value.Add(commentFull);
                else
                    commentFullsDict[commentFull.PostId] = [commentFull];
            }

            // HappeningFull
            List<HappeningFull> happeningFulls = [];
            foreach (HappeningFull happeningFull in happeningDataFull.HappeningFulls)
            {
                // ContactFull
                if (!contactFullsDict.TryGetValue(happeningFull.PostId, out ContactFull contact))
                    contact = null;

                happeningFull.ContactFull = contact;

                // LinkFulls
                if (!linkFullsDict.TryGetValue(happeningFull.PostId, out List<LinkFull> links))
                    links = [];

                happeningFull.LinkFulls = links;

                // CommentFulls
                if (!commentFullsDict.TryGetValue(happeningFull.PostId, out List<CommentFull> comments))
                    comments = [];

                happeningFull.CommentFulls = comments;

                happeningFulls.Add(happeningFull);
            }

            return happeningFulls;
        }

        // REGISTER
        public static async Task<long> Register(Happening happening)
        {
            happening.Status = 1;

            return await Add(happening);
        }

        // ADD
        public static async Task<long> Add(Happening happening)
        {
            return await new HappeningDB().Add(happening);
        }

        // UPDATE
        public static async Task<bool> Update(Happening happening)
        {
            return await new HappeningDB().Update(happening);
        }

        public static async Task<bool> UpdateStatus(long id, int status)
        {
            return await new HappeningDB().UpdateStatus(id, status);
        }

        // DELETE

        public static async Task Delete(long id)
        {
            await new HappeningDB().DeleteById(id);
        }
    }
}