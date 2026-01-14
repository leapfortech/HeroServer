using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Transactions;

namespace HeroServer
{
    public class RadioFunctions
    {
        // GET
        public static async Task<List<Radio>> GetAllByStatus(int status)
        {
            return await new RadioDB().GetAllByStatus(status);
        }

        public static async Task<Radio> GetById(long id)
        {
            return await new RadioDB().GetById(id);
        }

        public static async Task<RadioFull> GetFullById(long id)
        {
            RadioFull radioFull = await new RadioDB().GetFullById(id);

            if (radioFull == null)
                return null;

            radioFull.TitleImage = await PostFunctions.GetTitleImageById(radioFull.PostId);

            return radioFull;
        }

        public static async Task<RadioFull> GetFullByPostId(long postId)
        {
            RadioFull radioFull = await new RadioDB().GetFullByPostId(postId);

            if (radioFull == null)
                return null;

            radioFull.TitleImage = await PostFunctions.GetTitleImageById(postId);

            return radioFull;
        }

        public static async Task<List<RadioFull>> GetFullsByStatus(int status)
        {
            RadioDataFull radioDataFull = await new RadioDB().GetDataFullByStatus(status);

            return await GetFulls(radioDataFull);
        }

        public static async Task<List<RadioFull>> GetFulls(RadioDataFull radioDataFull)
        {
            // ContactFull
            Dictionary<long, ContactFull> contactFullsDict = [];
            foreach (ContactFull contactFull in radioDataFull.ContactFulls)
            {
                if (contactFullsDict.TryGetValue(contactFull.PostId, out ContactFull value))
                    throw new Exception($"Duplicate Contact for PostId {contactFull.PostId}");

                contactFullsDict[contactFull.PostId] = contactFull;
            }

            // LinkFull
            Dictionary<long, List<LinkFull>> linkFullsDict = [];
            foreach (LinkFull linkFull in radioDataFull.LinkFulls)
            {
                if (linkFullsDict.TryGetValue(linkFull.PostId, out List<LinkFull> value))
                    value.Add(linkFull);
                else
                    linkFullsDict[linkFull.PostId] = [linkFull];
            }

            // CommentFull
            Dictionary<long, List<CommentFull>> commentFullsDict = [];
            foreach (CommentFull commentFull in radioDataFull.CommentFulls)
            {
                if (commentFullsDict.TryGetValue(commentFull.PostId, out List<CommentFull> value))
                    value.Add(commentFull);
                else
                    commentFullsDict[commentFull.PostId] = [commentFull];
            }

            // RadioFull
            List<RadioFull> radioFulls = [];
            foreach (RadioFull radioFull in radioDataFull.RadioFulls)
            {
                // ContactFull
                if (!contactFullsDict.TryGetValue(radioFull.PostId, out ContactFull contact))
                    contact = null;

                radioFull.ContactFull = contact;

                // LinkFulls
                if (!linkFullsDict.TryGetValue(radioFull.PostId, out List<LinkFull> links))
                    links = [];

                radioFull.LinkFulls = links;

                // CommentFulls
                if (!commentFullsDict.TryGetValue(radioFull.PostId, out List<CommentFull> comments))
                    comments = [];

                radioFull.CommentFulls = comments;

                // TitleImage
                radioFull.TitleImage = await PostFunctions.GetTitleImageById(radioFull.PostId);

                radioFulls.Add(radioFull);
            }

            return radioFulls;
        }

        // REGISTER
        public static async Task<long> Register(RegisterRadioRequest registerRadioRequest)
        {
            long id = -1;
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                registerRadioRequest.Post.PostSubtypeId = (long)PostSubtype.Radio;
                registerRadioRequest.Radio.PostId = await PostFunctions.Register(registerRadioRequest);

                registerRadioRequest.Radio.Status = 1;
                id = await Add(registerRadioRequest.Radio);

                for (int i = 0; i < registerRadioRequest.RadioTypes.Count; i++)
                {
                    registerRadioRequest.RadioTypes[i].RadioId = id;
                    registerRadioRequest.RadioTypes[i].Status = 1;

                    await new RadioTypeDB().Add(registerRadioRequest.RadioTypes[i]);
                }

                for (int i = 0; i < registerRadioRequest.RadioLanguages.Count; i++)
                {
                    registerRadioRequest.RadioLanguages[i].RadioId = id;
                    registerRadioRequest.RadioLanguages[i].Status = 1;

                    await new RadioLanguageDB().Add(registerRadioRequest.RadioLanguages[i]);
                }

                scope.Complete();
            }

            return id;
        }

        public static async Task<long> RegisterRadioListen(RadioListen radioListen)
        {
            return await new RadioListenDB().Add(radioListen);
        }

        // ADD
        public static async Task<long> Add(Radio radio)
        {
            return await new RadioDB().Add(radio);
        }

        // UPDATE
        public static async Task<bool> Update(Radio radio)
        {
            return await new RadioDB().Update(radio);
        }

        public static async Task<bool> UpdateStatus(long id, int status)
        {
            return await new RadioDB().UpdateStatus(id, status);
        }

        // DELETE

        public static async Task DeleteById(long id)
        {
            await new RadioDB().DeleteById(id);
        }

        public static async Task DeleteByPostId(long postId)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                long radioId = await new RadioDB().GetIdByPostId(postId);

                await new RadioTypeDB().DeleteByRadioId(radioId);
                await new RadioLanguageDB().DeleteByRadioId(radioId);
                await new RadioListenDB().DeleteByRadioId(radioId);

                await new RadioDB().DeleteByPostId(postId);

                scope.Complete();
            }
        }
    }
}