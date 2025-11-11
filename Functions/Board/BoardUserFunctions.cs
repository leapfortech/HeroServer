using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Transactions;

namespace HeroServer
{
    public class BoardUserFunctions
    {
        // GET
        public static async Task<IEnumerable<BoardUser>> GetAll()
        {
            return await new BoardUserDB().GetAll();
        }

        public static async Task<IEnumerable<BoardUserFull>> GetFulls()
        {
            return await new BoardUserDB().GetFulls();
        }

        public static async Task<BoardUser> GetById(int id)
        {
            return await new BoardUserDB().GetById(id);
        }

        public static async Task<int> GetCountAll()
        {
            return await new BoardUserDB().GetCountAll();
        }

        public static async Task<int> GetCountByStatus(int status)
        {
            return await new BoardUserDB().GetCountByStatus(status);
        }

        public static async Task<BoardUser> GetByIdStatus(int id, int status)
        {
            return await new BoardUserDB().GetByIdStatus(id, status);
        }

        public static async Task<BoardUser> GetByWebSysUserId(int webSysUserId)
        {
            return await new BoardUserDB().GetByWebSysUserId(webSysUserId);
        }

        public static async Task<int> GetIdByWebSysUserId(int webSysUserId)
        {
            return await new BoardUserDB().GetIdByWebSysUserId(webSysUserId);
        }

        public static async Task<int> GetIdByEmail(String eMail)
        {
            return await new BoardUserDB().GetIdByEmail(eMail);
        }

        // ADD
        public static async Task<int> Add(BoardUser boardUser)
        {
            return await new BoardUserDB().Add(boardUser);
        }

        // UPDATE
        public static async Task UpdateFull(BoardUserFull boardUserFull)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                await new BoardUserDB().UpdateStatusByWebSysUserId(boardUserFull.BoardUser.WebSysUserId, 0);

                boardUserFull.BoardUser.BoardUserStatusId = 1;
                if (!await new BoardUserDB().Update(boardUserFull.BoardUser))
                    throw new Exception("UAU¶Cannot Update BoardUser");

                boardUserFull.WebSysUser.WebSysUserStatusId = 1;
                if (!await new WebSysUserDB().Update(boardUserFull.WebSysUser))
                    throw new Exception("UAU¶Cannot Update BoardUser");

                scope.Complete();
            }
        }

        public static async Task Update(BoardUser boardUser)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                await new BoardUserDB().UpdateStatusByWebSysUserId(boardUser.WebSysUserId, 0);

                boardUser.BoardUserStatusId = 1;
                if (!await new BoardUserDB().Update(boardUser))
                    throw new Exception("UAU¶Cannot Update BoardUser");

                scope.Complete();
            }
        }

        public static async Task<bool> UpdateStatus(int id, int status)
        {
            return await new BoardUserDB().UpdateStatus(id, status);
        }

        // DELETE
        public static async Task DeleteById(int id, bool delAuthUser = true)
        {
            int webSysUserId = await new BoardUserDB().GetWebSysUserId(id);

            await new BoardUserDB().DeleteById(id);

            if (webSysUserId == -1)
                return;

            int appUserId = await AppUserFunctions.GetIdByWebSysUserId(webSysUserId);
            if (appUserId != -1)
                return;

            await NotificationFunctions.DeleteByWebSysUserId(webSysUserId);
            await WebSysUserFunctions.DeleteById(webSysUserId, delAuthUser);
        }

        public static async Task DeleteByEmail(String eMail, bool delAuthUser = true)
        {
            int boardUserId = await GetIdByEmail(eMail);
            if (boardUserId == -1)
                throw new Exception("Email NOT Found");
            await DeleteById(boardUserId, delAuthUser);
        }
    }
}