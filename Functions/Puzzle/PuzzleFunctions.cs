using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Transactions;

namespace HeroServer
{
    public class PuzzleFunctions
    {
        // GET
        public static async Task<PuzzleAllRsp> GetAllByDifficulty(PuzzleAllByDifficultyReq req)
        {
            return await new PuzzleDB().GetAllByDifficulty(req);
        }

        public static async Task<List<Puzzle>> GetAllByStatus(int status)
        {
            return await new PuzzleDB().GetAllByStatus(status);
        }

        public static async Task<Puzzle> GetById(long id)
        {
            return await new PuzzleDB().GetById(id);
        }

        public static async Task<PuzzleFull> GetFullById(long id, long likeAppUserId, int includeCorrect = 1)
        {
            PuzzleFull puzzleFull = await new PuzzleDB().GetFullById(id, likeAppUserId, includeCorrect);

            if (puzzleFull == null)
                return null;

            puzzleFull.Images = await PostFunctions.GetImagesById(puzzleFull.PostId, true);

            return puzzleFull;
        }

        public static async Task<PuzzleFull> GetFullByPostId(long postId, long likeAppUserId)
        {
            PuzzleFull puzzleFull = await new PuzzleDB().GetFullByPostId(postId, likeAppUserId);

            if (puzzleFull == null)
                return null;

            puzzleFull.Images = await PostFunctions.GetImagesById(puzzleFull.PostId, true);

            return puzzleFull;
        }

        public static async Task<List<PuzzleFull>> GetFullsByStatus(int status)
        {
            PuzzleDataFull puzzleDataFull = await new PuzzleDB().GetDataFullByStatus(status);

            return await GetFulls(puzzleDataFull);
        }

        public static async Task<List<PuzzleFull>> GetFulls(PuzzleDataFull puzzleDataFull)
        {
            // ContactFull
            Dictionary<long, ContactFull> contactFullsDict = [];
            foreach (ContactFull contactFull in puzzleDataFull.ContactFulls)
            {
                if (contactFullsDict.TryGetValue(contactFull.PostId, out ContactFull value))
                    throw new Exception($"Duplicate Contact for PostId {contactFull.PostId}");

                contactFullsDict[contactFull.PostId] = contactFull;
            }

            // LinkFull
            Dictionary<long, List<LinkFull>> linkFullsDict = [];
            foreach (LinkFull linkFull in puzzleDataFull.LinkFulls)
            {
                if (linkFullsDict.TryGetValue(linkFull.PostId, out List<LinkFull> value))
                    value.Add(linkFull);
                else
                    linkFullsDict[linkFull.PostId] = [linkFull];
            }

            // CommentFull
            Dictionary<long, List<CommentFull>> commentFullsDict = [];
            foreach (CommentFull commentFull in puzzleDataFull.CommentFulls)
            {
                if (commentFullsDict.TryGetValue(commentFull.PostId, out List<CommentFull> value))
                    value.Add(commentFull);
                else
                    commentFullsDict[commentFull.PostId] = [commentFull];
            }

            // PuzzleFull
            List<PuzzleFull> puzzleFulls = [];
            foreach (PuzzleFull puzzleFull in puzzleDataFull.PuzzleFulls)
            {
                // ContactFull
                if (!contactFullsDict.TryGetValue(puzzleFull.PostId, out ContactFull contact))
                    contact = null;

                puzzleFull.ContactFull = contact;

                // LinkFulls
                if (!linkFullsDict.TryGetValue(puzzleFull.PostId, out List<LinkFull> links))
                    links = [];

                puzzleFull.LinkFulls = links;

                // CommentFulls
                if (!commentFullsDict.TryGetValue(puzzleFull.PostId, out List<CommentFull> comments))
                    comments = [];

                puzzleFull.CommentFulls = comments;

                // Images
                puzzleFull.Images = await PostFunctions.GetImagesById(puzzleFull.PostId, true);

                puzzleFulls.Add(puzzleFull);
            }

            return puzzleFulls;
        }

        public static async Task<PuzzleFull> GetNextPuzzle(PuzzleNextRequest puzzleNextRequest)
        {
            long puzzleId = await new PuzzleDB().GetNextPuzzle(puzzleNextRequest);

            if (puzzleId == -1)
                return null;

            PuzzleFull puzzleFull = await GetFullById(puzzleId, -1, 0);
            //puzzleFull.Images = await PostFunctions.GetImagesById(puzzleFull.PostId, true);

            return puzzleFull;
        }

        // REGISTER
        public static async Task<long> Register(RegisterPuzzleRequest registerPuzzleRequest)
        {
            long id = -1;
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                registerPuzzleRequest.Post.PostTypeId = (long)PostType.Puzzle;
                registerPuzzleRequest.Post.PublicationDateTime = DateTime.Now;

                registerPuzzleRequest.Puzzle.PostId = await PostFunctions.Register(registerPuzzleRequest);

                registerPuzzleRequest.Puzzle.PlayCount = 0;
                registerPuzzleRequest.Puzzle.Status = 1;
                
                id = await Add(registerPuzzleRequest.Puzzle);

                for (int i = 0; i < registerPuzzleRequest.PuzzleAnswers.Count; i++)
                {
                    registerPuzzleRequest.PuzzleAnswers[i].PuzzleId = id;
                    registerPuzzleRequest.PuzzleAnswers[i].Status = 1;
                    await new PuzzleAnswerDB().Add(registerPuzzleRequest.PuzzleAnswers[i]);
                    await Task.Delay(2);
                }

                scope.Complete();
            }

            return id;
        }

        public static async Task<long> RegisterPlayer(long appUserId)
        {
            return await new PlayerDB().Add(new Player(-1, appUserId, DateTime.Now, DateTime.Now, 1));
        }

        // ADD
        public static async Task<long> Add(Puzzle puzzle)
        {
            return await new PuzzleDB().Add(puzzle);
        }

        // UPDATE
        public static async Task<bool> Update(RegisterPuzzleRequest registerPuzzleRequest)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                // Update Post
                await PostFunctions.UpdatePost(registerPuzzleRequest);

                // Update Puzzle
                await new PuzzleDB().UpdateStatusByPostId(registerPuzzleRequest.Post.Id, 1, 0);

                registerPuzzleRequest.Puzzle.PostId = registerPuzzleRequest.Post.Id;
                registerPuzzleRequest.Puzzle.Status = 1;

                long puzzleId = -1;

                if (registerPuzzleRequest.Puzzle.Id == -1 || registerPuzzleRequest.Puzzle.Id == 0)
                {
                    puzzleId = await Add(registerPuzzleRequest.Puzzle);
                }
                else
                {
                    await Update(registerPuzzleRequest.Puzzle);
                    await UpdateStatus(registerPuzzleRequest.Puzzle.Id, 1);

                    puzzleId = registerPuzzleRequest.Puzzle.Id;
                }

                // Update PuzzleAnswers
                if (registerPuzzleRequest.PuzzleAnswers != null &&
                    registerPuzzleRequest.PuzzleAnswers.Count > 0)
                {
                    for (int i = 0; i < registerPuzzleRequest.PuzzleAnswers.Count; i++)
                    {
                        PuzzleAnswer puzzleAnswer = registerPuzzleRequest.PuzzleAnswers[i];

                        puzzleAnswer.PuzzleId = puzzleId;
                        puzzleAnswer.Status = 1;

                        if (puzzleAnswer.Id == -1 || puzzleAnswer.Id == 0)
                        {
                            await new PuzzleAnswerDB().Add(puzzleAnswer);
                        }
                        else
                        {
                            await new PuzzleAnswerDB().Update(puzzleAnswer);
                            await new PuzzleAnswerDB().UpdateStatus(puzzleAnswer.Id, 1);
                        }
                    }
                }

                scope.Complete();

                return true;
            }
        }

        public static async Task<PuzzleResultResponse> SaveResult(PuzzleResultRequest puzzleResultRequest)
        {
            if (puzzleResultRequest == null)
                throw new ArgumentNullException(nameof(puzzleResultRequest));

            Puzzle currentPuzzle = await GetById(puzzleResultRequest.PuzzleId);
            if (currentPuzzle == null)
                throw new Exception("Puzzle not found");

            PuzzleAnswer puzzleAnswer = await new PuzzleAnswerDB().GetById(puzzleResultRequest.PuzzleAnswerId);
            if (puzzleAnswer == null)
                throw new Exception("Puzzle answer not found");

            bool correct = puzzleAnswer.IsCorrect == 1;
            int points = correct ? currentPuzzle.Points : 0;

            DateTime now = DateTime.Now;
            PuzzleResult puzzleResult = new PuzzleResult(-1, puzzleResultRequest.PlayerId, puzzleResultRequest.PuzzleId, points,
                                                         puzzleResultRequest.Time,points, now, now, now);

            await new PuzzleResultDB().Add(puzzleResult);

            PuzzleNextRequest nextRequest = new PuzzleNextRequest(puzzleResultRequest.PlayerId, currentPuzzle.PuzzleGameId, currentPuzzle.CountryId,
                                                                  currentPuzzle.Difficulty);

            PuzzleFull puzzleFull = await GetNextPuzzle(nextRequest);

            return new PuzzleResultResponse(correct ? 1 : 0, points, 0, 0, puzzleFull);
        }

        public static async Task<bool> Accept(long postId, long puzzleId)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                bool postOk = await PostFunctions.UpdateStatus(postId, 3);
                bool puzzleOk = await UpdateStatus(puzzleId, 3);

                if (!postOk || !puzzleOk)
                    return false;

                scope.Complete();
            }

            return true;
        }

        public static async Task<bool> Reject(long postId, long puzzleId)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                bool postOk = await PostFunctions.UpdateStatus(postId, 0);
                bool puzzleOk = await UpdateStatus(puzzleId, 0);

                if (!postOk || !puzzleOk)
                    return false;

                scope.Complete();
            }

            return true;
        }

        public static async Task<bool> Update(Puzzle puzzle)
        {
            return await new PuzzleDB().Update(puzzle);
        }

        public static async Task<bool> UpdateStatus(long id, int status)
        {
            return await new PuzzleDB().UpdateStatus(id, status);
        }

        public static async Task<bool> UpdateStatus(long postId, long puzzleId, int status)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                bool postOk = await PostFunctions.UpdateStatus(postId, status);
                bool puzzleOk = await UpdateStatus(puzzleId, status);

                if (!postOk || !puzzleOk)
                    return false;

                scope.Complete();
            }

            return true;
        }

        // DELETE

        public static async Task DeleteById(long id)
        {
            await new PuzzleDB().DeleteById(id);
        }

        public static async Task DeleteByPostId(long postId)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                long puzzleId = await new PuzzleDB().GetIdByPostId(postId);

                await new PuzzleAnswerDB().DeleteByPuzzleId(puzzleId);
                await new PuzzleDB().DeleteByPostId(postId);

                scope.Complete();
            }
        }
    }
}