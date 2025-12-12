using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Transactions;

namespace HeroServer
{
    public class PuzzleFunctions
    {
        // GET
        public static async Task<List<Puzzle>> GetAllByStatus(int status)
        {
            return await new PuzzleDB().GetAllByStatus(status);
        }

        public static async Task<Puzzle> GetById(long id)
        {
            return await new PuzzleDB().GetById(id);
        }

        public static async Task<PuzzleFull> GetFullById(long id)
        {
            return await new PuzzleDB().GetFullById(id);
        }

        public static async Task<PuzzleFull> GetFullByPostId(long postId)
        {
            return await new PuzzleDB().GetFullByPostId(postId);
        }

        // REGISTER
        public static async Task<long> Register(long postId, RegisterPuzzleRequest registerPuzzleRequest)
        {
            long puzzleId = -1;
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                registerPuzzleRequest.Puzzle.PostId = postId;
                registerPuzzleRequest.Puzzle.Status = 1;

                puzzleId = await new PuzzleDB().Add(registerPuzzleRequest.Puzzle);

                for (int i = 0; i < registerPuzzleRequest.PuzzleAnswers.Count; i++)
                {
                    registerPuzzleRequest.PuzzleAnswers[i].PuzzleId = puzzleId;

                    await new PuzzleAnswerDB().Add(registerPuzzleRequest.PuzzleAnswers[i]);
                }

                scope.Complete();
            }

            return puzzleId;
        }

        // ADD
        public static async Task<long> Add(Puzzle puzzle)
        {
            return await new PuzzleDB().Add(puzzle);
        }

        // UPDATE
        public static async Task<bool> Update(Puzzle puzzle)
        {
            return await new PuzzleDB().Update(puzzle);
        }

        public static async Task<bool> UpdateStatus(long id, int status)
        {
            return await new PuzzleDB().UpdateStatus(id, status);
        }

        // DELETE

        public static async Task Delete(long id)
        {
            await new PuzzleDB().DeleteById(id);
        }
    }
}