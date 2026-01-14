using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Transactions;

namespace HeroServer
{
    public class TaleFunctions
    {
        // GET
        public static async Task<List<Tale>> GetAllByStatus(int status)
        {
            return await new TaleDB().GetAllByStatus(status);
        }

        public static async Task<Tale> GetById(long id)
        {
            return await new TaleDB().GetById(id);
        }

        public static async Task<TaleFull> GetFullById(long id)
        {
            TaleFull taleFull = await new TaleDB().GetFullById(id);

            if (taleFull == null)
                return null;

            taleFull.TitleImage = await PostFunctions.GetTitleImageById(taleFull.PostId);

            return taleFull;
        }

        public static async Task<TaleFull> GetFullByPostId(long postId)
        {
            TaleFull taleFull = await new TaleDB().GetFullByPostId(postId);

            if (taleFull == null)
                return null;

            taleFull.TitleImage = await PostFunctions.GetTitleImageById(postId);

            return taleFull;
        }

        public static async Task<List<TaleFull>> GetFullsByStatus(int status)
        {
            TaleDataFull taleDataFull = await new TaleDB().GetDataFullByStatus(status);

            return await GetFulls(taleDataFull);
        }

        public static async Task<List<TaleFull>> GetFulls(TaleDataFull taleDataFull)
        {
            // ContactFull
            Dictionary<long, ContactFull> contactFullsDict = [];
            foreach (ContactFull contactFull in taleDataFull.ContactFulls)
            {
                if (contactFullsDict.TryGetValue(contactFull.PostId, out ContactFull value))
                    throw new Exception($"Duplicate Contact for PostId {contactFull.PostId}");

                contactFullsDict[contactFull.PostId] = contactFull;
            }

            // LinkFull
            Dictionary<long, List<LinkFull>> linkFullsDict = [];
            foreach (LinkFull linkFull in taleDataFull.LinkFulls)
            {
                if (linkFullsDict.TryGetValue(linkFull.PostId, out List<LinkFull> value))
                    value.Add(linkFull);
                else
                    linkFullsDict[linkFull.PostId] = [linkFull];
            }

            // CommentFull
            Dictionary<long, List<CommentFull>> commentFullsDict = [];
            foreach (CommentFull commentFull in taleDataFull.CommentFulls)
            {
                if (commentFullsDict.TryGetValue(commentFull.PostId, out List<CommentFull> value))
                    value.Add(commentFull);
                else
                    commentFullsDict[commentFull.PostId] = [commentFull];
            }

            // TaleFull
            List<TaleFull> taleFulls = [];
            foreach (TaleFull taleFull in taleDataFull.TaleFulls)
            {
                // ContactFull
                if (!contactFullsDict.TryGetValue(taleFull.PostId, out ContactFull contact))
                    contact = null;

                taleFull.ContactFull = contact;

                // LinkFulls
                if (!linkFullsDict.TryGetValue(taleFull.PostId, out List<LinkFull> links))
                    links = [];

                taleFull.LinkFulls = links;

                // CommentFulls
                if (!commentFullsDict.TryGetValue(taleFull.PostId, out List<CommentFull> comments))
                    comments = [];

                taleFull.CommentFulls = comments;

                // TitleImage
                taleFull.TitleImage = await PostFunctions.GetTitleImageById(taleFull.PostId);

                taleFulls.Add(taleFull);
            }

            return taleFulls;
        }

        // REGISTER
        public static async Task<long> Register(RegisterTaleRequest registerTaleRequest)
        {
            long id = -1;
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                registerTaleRequest.Post.PostSubtypeId = (long)PostSubtype.Tale;
                registerTaleRequest.Post.Id = await PostFunctions.Register(registerTaleRequest);

                if (registerTaleRequest.Tale == null)
                {
                    registerTaleRequest.Tale = new Tale(-1, registerTaleRequest.Post.Id, DateTime.Now, DateTime.Now, 1);
                }
                else
                {
                    registerTaleRequest.Tale.PostId = registerTaleRequest.Post.Id;
                    registerTaleRequest.Tale.Status = 1;
                }

                id = await Add(registerTaleRequest.Tale);

                scope.Complete();
            }

            return id;
        }

        // ADD
        public static async Task<long> Add(Tale tale)
        {
            return await new TaleDB().Add(tale);
        }

        // UPDATE
        public static async Task<bool> Update(Tale tale)
        {
            return await new TaleDB().Update(tale);
        }

        public static async Task<bool> UpdateStatus(long id, int status)
        {
            return await new TaleDB().UpdateStatus(id, status);
        }

        // DELETE

        public static async Task DeleteById(long id)
        {
            await new TaleDB().DeleteById(id);
        }

        public static async Task DeleteByPostId(long postId)
        {
            await new TaleDB().DeleteByPostId(postId);
        }
    }
}