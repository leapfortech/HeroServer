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
            return await new RadioDB().GetFullById(id);
        }

        public static async Task<RadioFull> GetFullByPostId(long postId)
        {
            return await new RadioDB().GetFullByPostId(postId);
        }

        // REGISTER
        public static async Task<long> Register(long postId, RegisterRadioRequest registerRadioRequest)
        {
            long radioId = -1;
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                registerRadioRequest.Radio.PostId = postId;
                registerRadioRequest.Radio.Status = 1;

                radioId = await new RadioDB().Add(registerRadioRequest.Radio);

                for (int i = 0; i < registerRadioRequest.RadioTypes.Count; i++)
                {
                    registerRadioRequest.RadioTypes[i].RadioId = radioId;
                    registerRadioRequest.RadioTypes[i].Status = 1;

                    await new RadioTypeDB().Add(registerRadioRequest.RadioTypes[i]);
                }

                for (int i = 0; i < registerRadioRequest.RadioLanguages.Count; i++)
                {
                    registerRadioRequest.RadioLanguages[i].RadioId = radioId;
                    registerRadioRequest.RadioLanguages[i].Status = 1;

                    await new RadioLanguageDB().Add(registerRadioRequest.RadioLanguages[i]);
                }

                scope.Complete();
            }

            return radioId;
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

        public static async Task Delete(long id)
        {
            await new RadioDB().DeleteById(id);
        }
    }
}