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
            return await new TreatmentDB().GetFullById(id);
        }

        public static async Task<TreatmentFull> GetFullByPostId(long postId)
        {
            return await new TreatmentDB().GetFullByPostId(postId);
        }

        public static async Task<List<TreatmentFull>> GetFullsByStatus(int status)
        {
            TreatmentDataFull treatmentDataFull = await new TreatmentDB().GetDataFullByStatus(status);

            return GetFulls(treatmentDataFull);
        }

        public static List<TreatmentFull> GetFulls(TreatmentDataFull treatmentDataFull)
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

                treatmentFulls.Add(treatmentFull);
            }

            return treatmentFulls;
        }

        // REGISTER
        public static async Task<long> Register(RegisterTreatmentRequest registerTreatmentRequest)
        {
            long treatmentId = -1;
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                registerTreatmentRequest.Treatment.Status = 1;

                treatmentId = await new TreatmentDB().Add(registerTreatmentRequest.Treatment);

                for (int i = 0; i < registerTreatmentRequest.Diseases.Count; i++)
                {
                    registerTreatmentRequest.Diseases[i].TreatmentId = treatmentId;
                    registerTreatmentRequest.Diseases[i].Status = 1;

                    await new DiseaseDB().Add(registerTreatmentRequest.Diseases[i]);
                }

                scope.Complete();
            }

            return treatmentId;
        }

        // ADD
        public static async Task<long> Add(Treatment treatment)
        {
            return await new TreatmentDB().Add(treatment);
        }

        // UPDATE
        public static async Task<bool> Update(Treatment treatment)
        {
            return await new TreatmentDB().Update(treatment);
        }

        public static async Task<bool> UpdateStatus(long id, int status)
        {
            return await new TreatmentDB().UpdateStatus(id, status);
        }

        // DELETE

        public static async Task Delete(long id)
        {
            await new TreatmentDB().DeleteById(id);
        }
    }
}