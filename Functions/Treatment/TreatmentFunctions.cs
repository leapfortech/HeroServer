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