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

        public static async Task<RadioFull> GetFullById(long id, long likeAppUserId)
        {
            RadioFull radioFull = await new RadioDB().GetFullById(id, likeAppUserId);

            if (radioFull == null)
                return null;

            radioFull.Images = await PostFunctions.GetImagesById(radioFull.PostId, true);
            radioFull.Thumbnail = await AppUserFunctions.GetThumbnail(radioFull.AppUserId);

            return radioFull;
        }

        public static async Task<RadioFull> GetFullByPostId(long postId, long likeAppUserId)
        {
            RadioFull radioFull = await new RadioDB().GetFullByPostId(postId, likeAppUserId);

            if (radioFull == null)
                return null;

            radioFull.Images = await PostFunctions.GetImagesById(radioFull.PostId, true);
            radioFull.Thumbnail = await AppUserFunctions.GetThumbnail(radioFull.AppUserId);

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

                // Images
                radioFull.Images = await PostFunctions.GetImagesById(radioFull.PostId, true);
                radioFull.Thumbnail = await AppUserFunctions.GetThumbnail(radioFull.AppUserId);

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
                registerRadioRequest.Post.PostTypeId = (long)PostType.Radio;
                registerRadioRequest.Post.Id = await PostFunctions.Register(registerRadioRequest);

                if (registerRadioRequest.Radio == null)
                {
                    registerRadioRequest.Radio = new Radio(-1, registerRadioRequest.Post.Id, DateTime.Now, DateTime.Now, 0);
                }
                else
                {
                    registerRadioRequest.Radio.PostId = registerRadioRequest.Post.Id;
                    registerRadioRequest.Radio.Status = 0;
                }

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
        public static async Task<bool> Update(RegisterRadioRequest registerRadioRequest)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                // Update Post
                await PostFunctions.UpdatePost(registerRadioRequest);

                // Update Radio
                // Soft Delete
                await new RadioDB().UpdateStatusByPostId(registerRadioRequest.Post.Id, 1, 0);

                registerRadioRequest.Radio.PostId = registerRadioRequest.Post.Id;
                registerRadioRequest.Radio.Status = 1;

                long radioId = -1;
                if (registerRadioRequest.Radio.Id == -1 || registerRadioRequest.Radio.Id == 0)
                {
                    radioId = await Add(registerRadioRequest.Radio);
                }
                else
                {
                    await Update(registerRadioRequest.Radio);
                    await UpdateStatus(registerRadioRequest.Radio.Id, 1);
                    radioId = registerRadioRequest.Radio.Id;
                }

                // Radio Types
                // Soft Delete
                await new RadioTypeDB().UpdateStatusByRadioId(radioId, 1, 0);

                if (registerRadioRequest.RadioTypes != null && registerRadioRequest.RadioTypes.Count > 0)
                {
                    for (int i = 0; i < registerRadioRequest.RadioTypes.Count; i++)
                    {
                        RadioType radioType = registerRadioRequest.RadioTypes[i];
                        radioType.RadioId = radioId;

                        if (radioType.Id == -1 || radioType.Id == 0)
                        {
                            radioType.Status = 1;
                            await new RadioTypeDB().Add(radioType);
                        }
                        else
                        {
                            await new RadioTypeDB().Update(radioType);
                            await new RadioTypeDB().UpdateStatus(radioType.Id, 1);
                        }
                    }
                }

                // Radio Languates
                // Soft Delete
                await new RadioLanguageDB().UpdateStatusByRadioId(radioId, 1, 0);

                if (registerRadioRequest.RadioLanguages != null && registerRadioRequest.RadioLanguages.Count > 0)
                {
                    for (int i = 0; i < registerRadioRequest.RadioLanguages.Count; i++)
                    {
                        RadioLanguage radioLanguage = registerRadioRequest.RadioLanguages[i];
                        radioLanguage.RadioId = radioId;

                        if (radioLanguage.Id == -1 || radioLanguage.Id == 0)
                        {
                            radioLanguage.Status = 1;
                            await new RadioLanguageDB().Add(radioLanguage);
                        }
                        else
                        {
                            await new RadioLanguageDB().Update(radioLanguage);
                            await new RadioLanguageDB().UpdateStatus(
                                radioLanguage.Id, 1);
                        }
                    }
                }

                scope.Complete();
                return true;
            }
        }

        public static async Task<bool> Accept(long postId, long radioId)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                bool postOk = await PostFunctions.UpdateStatus(postId, 1);
                bool radioOk = await UpdateStatus(radioId, 1);

                if (!postOk || !radioOk)
                    return false;

                scope.Complete();
            }

            return true;
        }

        public static async Task<bool> Reject(long postId, long radioId)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                bool postOk = await PostFunctions.UpdateStatus(postId, 4);
                bool radioOk = await UpdateStatus(radioId, 4);

                if (!postOk || !radioOk)
                    return false;

                scope.Complete();
            }

            return true;
        }

        public static async Task<bool> Update(Radio radio)
        {
            return await new RadioDB().Update(radio);
        }

        public static async Task<bool> UpdateStatus(long id, int status)
        {
            return await new RadioDB().UpdateStatus(id, status);
        }

        public static async Task<bool> UpdateStatusByPostId(long postId, int curStatus, int newStatus)
        {
            return await new RadioDB().UpdateStatusByPostId(postId, curStatus, newStatus);
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