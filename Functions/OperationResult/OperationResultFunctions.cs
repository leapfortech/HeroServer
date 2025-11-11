using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HeroServer
{
    public class OperationResultFunctions
    {
        // GET
        public static async Task<OperationResult> GetById(int id)
        {
            return await new OperationResultDB().GetById(id);
        }

        public static async Task<IEnumerable<OperationResult>> GetByProjectId(int projectId, DateTime createDateTime)
        {
            return await new OperationResultDB().GetByProjectId(projectId, createDateTime);
        }

        // Register
        public static async Task<int> Register(OperationResult operationResult)
        {
            return await Add(operationResult);
        }

        // ADD
        public static async Task<int> Add(OperationResult operationResult)
        {
            return await new OperationResultDB().Add(operationResult);
        }

        // UPDATE
        public static async Task<bool> Update(OperationResult operationResult)
        {
            return await new OperationResultDB().Update(operationResult);
        }

        public static async Task<bool> UpdateStatus(int id, int status)
        {
            return await new MeetingDB().UpdateStatus(id, status);
        }
    }
}
