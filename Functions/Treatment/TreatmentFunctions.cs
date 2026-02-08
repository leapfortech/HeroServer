using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Transactions;

namespace HeroServer
{
    public class TreatmentFunctions
    {
        // GET
        public static async Task<List<Treatment>> GetAllByStatus(int status)
        {
            return await new TreatmentDB().GetAllByStatus(status);
        }

        public static async Task<Treatment> GetById(long id)
        {
            return await new TreatmentDB().GetById(id);
        }

        public static async Task<TreatmentFull> GetFullById(long id)
        {
            TreatmentFull treatmentFull = await new TreatmentDB().GetFullById(id);

            if (treatmentFull == null)
                return null;

            treatmentFull.Images = await PostFunctions.GetImagesById(treatmentFull.PostId, true);

            return treatmentFull;
        }

        public static async Task<TreatmentFull> GetFullByPostId(long postId)
        {
            TreatmentFull treatmentFull = await new TreatmentDB().GetFullByPostId(postId);

            if (treatmentFull == null)
                return null;

            treatmentFull.Images = await PostFunctions.GetImagesById(treatmentFull.PostId, true);

            return treatmentFull;
        }

        public static async Task<List<TreatmentFull>> GetFullsByStatus(int status)
        {
            TreatmentDataFull treatmentDataFull = await new TreatmentDB().GetDataFullByStatus(status);

            return await GetFulls(treatmentDataFull);
        }

        public static async Task<List<TreatmentFull>> GetFulls(TreatmentDataFull treatmentDataFull)
        {
            // ContactFull
            Dictionary<long, ContactFull> contactFullsDict = [];
            foreach (ContactFull contactFull in treatmentDataFull.ContactFulls)
            {
                if (contactFullsDict.TryGetValue(contactFull.PostId, out ContactFull value))
                    throw new Exception($"Duplicate Contact for PostId {contactFull.PostId}");

                contactFullsDict[contactFull.PostId] = contactFull;
            }

            // LinkFull
            Dictionary<long, List<LinkFull>> linkFullsDict = [];
            foreach (LinkFull linkFull in treatmentDataFull.LinkFulls)
            {
                if (linkFullsDict.TryGetValue(linkFull.PostId, out List<LinkFull> value))
                    value.Add(linkFull);
                else
                    linkFullsDict[linkFull.PostId] = [linkFull];
            }

            // CommentFull
            Dictionary<long, List<CommentFull>> commentFullsDict = [];
            foreach (CommentFull commentFull in treatmentDataFull.CommentFulls)
            {
                if (commentFullsDict.TryGetValue(commentFull.PostId, out List<CommentFull> value))
                    value.Add(commentFull);
                else
                    commentFullsDict[commentFull.PostId] = [commentFull];
            }

            // TreatmentFull
            List<TreatmentFull> treatmentFulls = [];
            foreach (TreatmentFull treatmentFull in treatmentDataFull.TreatmentFulls)
            {
                // ContactFull
                if (!contactFullsDict.TryGetValue(treatmentFull.PostId, out ContactFull contact))
                    contact = null;

                treatmentFull.ContactFull = contact;

                // LinkFulls
                if (!linkFullsDict.TryGetValue(treatmentFull.PostId, out List<LinkFull> links))
                    links = [];

                treatmentFull.LinkFulls = links;

                // CommentFulls
                if (!commentFullsDict.TryGetValue(treatmentFull.PostId, out List<CommentFull> comments))
                    comments = [];

                treatmentFull.CommentFulls = comments;

                // Images
                treatmentFull.Images = await PostFunctions.GetImagesById(treatmentFull.PostId, true);

                treatmentFulls.Add(treatmentFull);
            }

            return treatmentFulls;
        }

        // REGISTER
        public static async Task<long> Register(RegisterTreatmentRequest registerTreatmentRequest)
        {
            long id = -1;
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                registerTreatmentRequest.Post.PostSubtypeId = (long)PostSubtype.Treatment;
                registerTreatmentRequest.Treatment.PostId = await PostFunctions.Register(registerTreatmentRequest);

                registerTreatmentRequest.Treatment.Status = 1;
                id = await Add(registerTreatmentRequest.Treatment);

                for (int i = 0; i < registerTreatmentRequest.Diseases.Count; i++)
                {
                    registerTreatmentRequest.Diseases[i].TreatmentId = id;
                    registerTreatmentRequest.Diseases[i].Status = 1;

                    await new DiseaseDB().Add(registerTreatmentRequest.Diseases[i]);
                }

                scope.Complete();
            }

            return id;
        }

        // ADD
        public static async Task<long> Add(Treatment treatment)
        {
            return await new TreatmentDB().Add(treatment);
        }

        // UPDATE
        public static async Task<bool> Update(RegisterTreatmentRequest registerTreatmentRequest)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                // Update Post
                await PostFunctions.UpdatePost(registerTreatmentRequest);

                // Update Treatment
                // Soft Delete
                await new TreatmentDB().UpdateStatusByPostId(registerTreatmentRequest.Post.Id, 1, 0);

                registerTreatmentRequest.Treatment.PostId = registerTreatmentRequest.Post.Id;
                registerTreatmentRequest.Treatment.Status = 1;

                long treatmentId = -1;

                if (registerTreatmentRequest.Treatment.Id == -1 || registerTreatmentRequest.Treatment.Id == 0)
                {
                    treatmentId = await Add(registerTreatmentRequest.Treatment);
                }
                else
                {
                    await Update(registerTreatmentRequest.Treatment);
                    await UpdateStatus(registerTreatmentRequest.Treatment.Id, 1);
                    treatmentId = registerTreatmentRequest.Treatment.Id;
                }

                // Diseases
                // Soft Delete
                await new DiseaseDB().UpdateStatusByTreatmentId(treatmentId, 1, 0);

                if (registerTreatmentRequest.Diseases != null && registerTreatmentRequest.Diseases.Count > 0)
                {
                    for (int i = 0; i < registerTreatmentRequest.Diseases.Count; i++)
                    {
                        Disease disease = registerTreatmentRequest.Diseases[i];
                        disease.TreatmentId = treatmentId;

                        if (disease.Id == -1 || disease.Id == 0)
                        {
                            disease.Status = 1;
                            await new DiseaseDB().Add(disease);
                        }
                        else
                        {
                            await new DiseaseDB().Update(disease);
                            await new DiseaseDB().UpdateStatus(disease.Id, 1);
                        }
                    }
                }

                scope.Complete();
                return true;
            }
        }

        public static async Task<bool> Accept(long postId, long treatmentId)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                bool postOk = await PostFunctions.UpdateStatus(postId, 3);
                bool treatmentOk = await UpdateStatus(treatmentId, 3);

                if (!postOk || !treatmentOk)
                    return false;

                scope.Complete();
            }

            return true;
        }

        public static async Task<bool> Reject(long postId, long treatmentId)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                bool postOk = await PostFunctions.UpdateStatus(postId, 0);
                bool treatmentOk = await UpdateStatus(treatmentId, 0);

                if (!postOk || !treatmentOk)
                    return false;

                scope.Complete();
            }

            return true;
        }

        public static async Task<bool> Update(Treatment treatment)
        {
            return await new TreatmentDB().Update(treatment);
        }

        public static async Task<bool> UpdateStatus(long id, int status)
        {
            return await new TreatmentDB().UpdateStatus(id, status);
        }

        // DELETE

        public static async Task DeleteById(long id)
        {
            await new TreatmentDB().DeleteById(id);
        }

        public static async Task DeleteByPostId(long postId)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                long treatmentId = await new TreatmentDB().GetIdByPostId(postId);

                await new DiseaseDB().DeleteByTreatmentId(treatmentId);
                await new TreatmentDB().DeleteByPostId(postId);

                scope.Complete();
            }
        }
    }
}