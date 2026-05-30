using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace HeroServer
{
    public class PuzzleDB
    {
        readonly SqlConnection conn = new SqlConnection(WebEnvConfig.ConnString);
        readonly String table = "[D-Puzzle]";

        private static Puzzle GetPuzzle(SqlDataReader reader)
        {
            return new Puzzle(Convert.ToInt64(reader["Id"]),
                              Convert.ToInt64(reader["PostId"]),
                              Convert.ToInt64(reader["PuzzleGameId"]),
                              Convert.ToInt64(reader["CountryId"]),
                              reader["Question"].ToString(),
                              reader["Hint"].ToString(),
                              Convert.ToInt32(reader["Difficulty"]),
                              Convert.ToInt32(reader["Delay"]),
                              Convert.ToInt32(reader["Points"]),
                              Convert.ToInt32(reader["PlayCount"]),
                              Convert.ToDateTime(reader["CreateDateTime"]),
                              Convert.ToDateTime(reader["UpdateDateTime"]),
                              Convert.ToInt32(reader["Status"]));
        }

        public static PuzzleFull GetPuzzleFull(SqlDataReader reader)
        {
            return new PuzzleFull(Convert.ToInt64(reader["Id"]),

                                  Convert.ToInt64(reader["PostId"]),
                                  Convert.ToInt64(reader["AppUserId"]),
                                  reader["AppUserAlias"].ToString(),
                                  Convert.ToInt64(reader["PostTypeId"]),
                                  Convert.ToInt64(reader["PostCountryId"]),
                                  Convert.ToInt64(reader["PostStateId"]),
                                  reader["Title"].ToString(),
                                  null,   //TitleImage
                                  reader["Summary"].ToString(),
                                  reader["Description"].ToString(),
                                  Convert.ToInt32(reader["ImageCount"]),
                                  Convert.ToInt32(reader["Favorite"]),
                                  Convert.ToInt32(reader["Like"]),
                                  Convert.ToInt32(reader["LikeCount"]),
                                  Convert.ToDateTime(reader["PublicationDateTime"]),
                                  Convert.ToInt32(reader["Status"]),
                                  null,   //ContactFull
                                  null,   //LinkFulls
                                  null,   //CommentFulls

                                  Convert.ToInt64(reader["PuzzleGameId"]),
                                  Convert.ToInt64(reader["CountryId"]),
                                  reader["Question"].ToString(),
                                  reader["Hint"].ToString(),
                                  Convert.ToInt32(reader["Difficulty"]),
                                  Convert.ToInt32(reader["Delay"]),
                                  Convert.ToInt32(reader["Points"]),
                                  Convert.ToInt32(reader["PlayCount"]),
                                  Convert.ToInt32(reader["Status"]),
                                  null,     //PuzzleAnswerFulls 
                                  null);  //Images);
        }


        // GET
        public async Task<PuzzleAllRsp> GetAllByDifficulty(PuzzleAllByDifficultyReq req)
        {
            int offset = (req.Page - 1) * req.PageSize;

            String strCmd = // Total count
                            "SELECT COUNT(DISTINCT P.Id) AS TotalCount " +
                            "FROM [D-Puzzle] AS P " +
                            "WHERE (@Status = -1 OR P.Status = @Status) " +
                            "AND (@PuzzleGameId = -1 OR P.PuzzleGameId = @PuzzleGameId) " +
                            "AND (@Difficulty = -1 OR P.Difficulty = @Difficulty); " +

                            // Data
                            "SELECT " +

                            // Post
                            "PO.Id AS PostIdData, " +
                            "PO.AppUserId, PO.PostTypeId, " +
                            "PO.CountryId AS PostCountryId, " +
                            "PO.StateId, PO.Title, PO.Summary, " +
                            "PO.Description AS PostDescription, " +
                            "PO.ImageCount, PO.LikeCount, " +
                            "PO.PublicationDateTime, " +
                            "PO.ApprovalDateTime, " +
                            "PO.ExpirationDateTime, " +
                            "PO.CreateDateTime AS PostCreateDateTime, " +
                            "PO.UpdateDateTime AS PostUpdateDateTime, " +
                            "PO.Status AS PostStatus, " +

                            // Puzzle
                            "P.Id, P.PostId, P.PuzzleGameId, P.CountryId, " +
                            "P.Question, P.Hint, P.Difficulty, P.Delay, " +
                            "P.Points, P.PlayCount, P.CreateDateTime, " +
                            "P.UpdateDateTime, P.Status, " +

                            // PuzzleAnswer
                            "PA.Id AS PuzzleAnswerId, " +
                            "PA.PuzzleId, " +
                            "PA.Description, " +
                            "PA.IsCorrect, " +
                            "PA.CreateDateTime AS PuzzleAnswerCreateDateTime, " +
                            "PA.UpdateDateTime AS PuzzleAnswerUpdateDateTime, " +
                            "PA.Status AS PuzzleAnswerStatus " +

                            "FROM " +
                            "(" +
                                "SELECT P.Id " +
                                "FROM [D-Puzzle] AS P " +
                                "WHERE (@Status = -1 OR P.Status = @Status) " +
                                "AND (@PuzzleGameId = -1 OR P.PuzzleGameId = @PuzzleGameId) " +
                                "AND (@Difficulty = -1 OR P.Difficulty = @Difficulty) " +
                                "ORDER BY P.CreateDateTime DESC " +
                                "OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY " +
                            ") AS PG " +

                            "INNER JOIN [D-Puzzle] AS P " +
                            "ON P.Id = PG.Id " +

                            "LEFT JOIN [D-Post] AS PO " +
                            "ON PO.Id = P.PostId " +

                            "LEFT JOIN [D-PuzzleAnswer] AS PA " +
                            "ON PA.PuzzleId = P.Id " +

                            "ORDER BY P.CreateDateTime DESC, PA.IsCorrect DESC;";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Status", SqlDbType.Int, req.Status);
            DBHelper.AddParam(command, "@PuzzleGameId", SqlDbType.BigInt, req.PuzzleGameId);
            DBHelper.AddParam(command, "@Difficulty", SqlDbType.Int, req.Difficulty);
            DBHelper.AddParam(command, "@Offset", SqlDbType.Int, offset);
            DBHelper.AddParam(command, "@PageSize", SqlDbType.Int, req.PageSize);

            PuzzleAllRsp response = null;

            using (conn)
            {
                await conn.OpenAsync();

                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    // Total count
                    int totalCount = 0;

                    if (await reader.ReadAsync())
                        totalCount = Convert.ToInt32(reader["TotalCount"]);

                    int totalPages = (int)Math.Ceiling((double)totalCount / req.PageSize);

                    // Data
                    await reader.NextResultAsync();

                    List<PuzzleInfo> puzzleInfos = new List<PuzzleInfo>();

                    Dictionary<long, PuzzleInfo> dicPuzzle = new Dictionary<long, PuzzleInfo>();

                    while (await reader.ReadAsync())
                    {
                        long puzzleId = Convert.ToInt64(reader["Id"]);

                        if (!dicPuzzle.ContainsKey(puzzleId))
                        {
                            Post post = PostDB.GetPost(reader);
                            post.Id = Convert.ToInt64(reader["PostIdData"]);


                            Puzzle puzzle = PuzzleDB.GetPuzzle(reader);

                            PuzzleInfo puzzleInfo = new PuzzleInfo(post, puzzle, new List<PuzzleAnswer>());

                            dicPuzzle.Add(puzzleId, puzzleInfo);

                            puzzleInfos.Add(puzzleInfo);
                        }

                        if (reader["PuzzleAnswerId"] != DBNull.Value)
                        {
                            PuzzleAnswer puzzleAnswer = PuzzleAnswerDB.GetPuzzleAnswer(reader);
                            puzzleAnswer.Id = Convert.ToInt64(reader["PuzzleAnswerId"]);

                            dicPuzzle[puzzleId].PuzzleAnswers.Add(puzzleAnswer);
                        }
                    }

                    response = new PuzzleAllRsp(req.Page, totalPages, puzzleInfos);
                }
            }

            return response;
        }

        public async Task<List<Puzzle>> GetAllByStatus(int status = -1)
        {
            String strCmd = $"SELECT * FROM {table}";
            if (status != -1)
                strCmd += " WHERE Status = @Status";

            SqlCommand command = new SqlCommand(strCmd, conn);

            if (status != -1)
                DBHelper.AddParam(command, "@Status", SqlDbType.Int, status);

            List<Puzzle> puzzles = [];
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                         Puzzle puzzle = GetPuzzle(reader);
                         puzzles.Add(puzzle);
                    }
                }
            }
            return puzzles;
        }

        public async Task<Puzzle> GetById(long id)
        {
            String strCmd = $"SELECT * FROM {table} WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Id", SqlDbType.BigInt, id);

            Puzzle puzzle = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                         puzzle = GetPuzzle(reader);
                    }
                }
            }
            return puzzle;
        }

        public async Task<long> GetIdByPostId(long postId)
        {
            String strCmd = $"SELECT Id FROM {table} WHERE PostId = @PostId";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@PostId", SqlDbType.BigInt, postId);

            long id = -1;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        id = Convert.ToInt64(reader["Id"]);
                    }
                }
            }
            return id;
        }

        // GET FULL
        public async Task<PuzzleFull> GetFullById(long id, long likeAppUserId, int includeCorrect)
        {
            String strCmd = $"SELECT {table}.Id, {table}.PostId," +
                             " Post.AppUserId, AppUser.Alias AS AppUserAlias, Post.PostTypeId," +
                             " Post.CountryId AS PostCountryId, Post.StateId AS PostStateId, Post.Title, Post.Summary, Post.Description," +
                             " Post.ImageCount," +
                             " CASE WHEN Fav.PostId IS NULL THEN 0 ELSE 1 END AS Favorite," +
                             " ISNULL(Lik.[Rank], -1) AS [Like]," +
                             " Post.LikeCount, Post.PublicationDateTime, Post.Status," +
                            $" {table}.PuzzleGameId, {table}.CountryId, {table}.Question, {table}.Hint," +
                            $" {table}.Difficulty, {table}.Delay, {table}.Points, {table}.PlayCount, {table}.Status" +
                            $" FROM {table}" +
                            $" INNER JOIN [D-Post] AS Post ON ({table}.PostId = Post.Id)" +
                            $" INNER JOIN [D-AppUser] AS AppUser ON (Post.AppUserId = AppUser.Id)" +
                            " LEFT JOIN [J-Favorite] AS Fav ON Fav.PostId = Post.Id AND Fav.AppUserId = @LikeAppUserId " +
                            " LEFT JOIN [D-Like] AS Lik ON Lik.PostId = Post.Id AND Lik.AppUserId = @LikeAppUserId " +
                            $" WHERE {table}.Id = @Id;";

            strCmd += "SELECT Id, PuzzleId, Description," +
                      " CASE WHEN @IncludeCorrect = 1 THEN IsCorrect ELSE -1 END AS IsCorrect," +
                      " Status" +
                      " FROM [D-PuzzleAnswer]" +
                      " WHERE PuzzleId IN" +
                     $" (SELECT Id FROM {table} WHERE Id = @Id)" +
                      " AND Status = 1" +
                      " ORDER BY CreateDateTime ASC;";

            strCmd += "SELECT Id, PostId, Name, Status" +
                       " FROM [D-Contact]" +
                      $" WHERE Status = 1 AND PostId = (SELECT PostId FROM {table} WHERE Id = @Id);";

            strCmd += "SELECT Link.Id, Link.LinkTypeId, Link.PostId, Link.Url, Link.Status" +
                       " FROM [D-Link] AS Link" +
                      $" WHERE Link.Status = 1 AND Link.PostId = (SELECT PostId FROM {table} WHERE Id = @Id);";

            strCmd += "SELECT Comment.Id, Comment.PostId, Comment.AppUserId, AppUser.Alias AS AppUserAlias," +
                      " Comment.Message, Comment.UpdateDateTime, Comment.Status" +
                      " FROM [D-Comment] AS Comment" +
                      " INNER JOIN [D-AppUser] AS AppUser ON (Comment.AppUserId = AppUser.Id)" +
                     $" WHERE Comment.Status = 1 AND Comment.PostId = (SELECT PostId FROM {table} WHERE Id = @Id);";

            SqlCommand command = new SqlCommand(strCmd, conn);
            DBHelper.AddParam(command, "@Id", SqlDbType.BigInt, id);
            DBHelper.AddParam(command, "@LikeAppUserId", SqlDbType.BigInt, likeAppUserId);
            DBHelper.AddParam(command, "@IncludeCorrect", SqlDbType.Int, includeCorrect);

            PuzzleFull puzzleFull = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (!await reader.ReadAsync())
                        return null;

                    puzzleFull = GetPuzzleFull(reader);

                    await reader.NextResultAsync();
                    puzzleFull.PuzzleAnswerFulls = [];
                    while (await reader.ReadAsync())
                        puzzleFull.PuzzleAnswerFulls.Add(PuzzleAnswerDB.GetPuzzleAnswerFull(reader));

                    await reader.NextResultAsync();
                    if (await reader.ReadAsync())
                        puzzleFull.ContactFull = ContactDB.GetContactFull(reader);

                    await reader.NextResultAsync();
                    List<LinkFull> linkFulls = [];
                    while (await reader.ReadAsync())
                        linkFulls.Add(LinkDB.GetLinkFull(reader));
                    puzzleFull.LinkFulls = linkFulls;

                    await reader.NextResultAsync();
                    List<CommentFull> commentFulls = [];
                    while (await reader.ReadAsync())
                        commentFulls.Add(CommentDB.GetCommentFull(reader));
                    puzzleFull.CommentFulls = commentFulls;
                }
            }

            return puzzleFull;
        }

        public async Task<PuzzleFull> GetFullByPostId(long postId, long likeAppUserId)
        {
            String strCmd = $"SELECT {table}.Id, {table}.PostId," +
                             " Post.AppUserId, AppUser.Alias AS AppUserAlias, Post.PostTypeId," +
                             " Post.CountryId AS PostCountryId, Post.StateId AS PostStateId, Post.Title, Post.Summary, Post.Description," +
                             " Post.ImageCount," +
                             " CASE WHEN Fav.PostId IS NULL THEN 0 ELSE 1 END AS Favorite," +
                             " ISNULL(Lik.[Rank], -1) AS [Like]," +
                             " Post.LikeCount, Post.PublicationDateTime, Post.Status," +
                            $" {table}.PuzzleGameId, {table}.CountryId, {table}.Question, {table}.Hint," +
                            $" {table}.Difficulty, {table}.Delay, {table}.Points, {table}.PlayCount, {table}.Status" +
                            $" FROM {table}" +
                            $" INNER JOIN [D-Post] AS Post ON ({table}.PostId = Post.Id)" +
                            $" INNER JOIN [D-AppUser] AS AppUser ON (Post.AppUserId = AppUser.Id)" +
                            " LEFT JOIN [J-Favorite] AS Fav ON Fav.PostId = Post.Id AND Fav.AppUserId = @LikeAppUserId " +
                            " LEFT JOIN [D-Like] AS Lik ON Lik.PostId = Post.Id AND Lik.AppUserId = @LikeAppUserId " +
                            $" WHERE {table}.PostId = @PostId;";

            strCmd += "SELECT Id, PuzzleId, Description, IsCorrect, Status" +
                      " FROM [D-PuzzleAnswer]" +
                      " WHERE PuzzleId IN" +
                     $" (SELECT Id FROM {table} WHERE PostId = @PostId)" +
                      " AND Status = 1" +
                      " ORDER BY CreateDateTime ASC;";

            strCmd += "SELECT Id, PostId, Name, Status" +
                       " FROM [D-Contact]" +
                       " WHERE Status = 1 AND PostId = @PostId;";

            strCmd += "SELECT Link.Id, Link.LinkTypeId, Link.PostId, Link.Url, Link.Status" +
              " FROM [D-Link] AS Link" +
              " WHERE Link.Status = 1 AND Link.PostId = @PostId;";

            strCmd += "SELECT Comment.Id, Comment.PostId, Comment.AppUserId, AppUser.Alias AS AppUserAlias," +
                      " Comment.Message, Comment.UpdateDateTime, Comment.Status" +
                      " FROM [D-Comment] AS Comment" +
                      " INNER JOIN [D-AppUser] AS AppUser ON(Comment.AppUserId = AppUser.Id)" +
                      " WHERE Comment.Status = 1 AND Comment.PostId = @PostId;";

            SqlCommand command = new SqlCommand(strCmd, conn);
            DBHelper.AddParam(command, "@PostId", SqlDbType.BigInt, postId);
            DBHelper.AddParam(command, "@LikeAppUserId", SqlDbType.BigInt, likeAppUserId);

            PuzzleFull puzzleFull = null;
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (!await reader.ReadAsync())
                        return null;

                    puzzleFull = GetPuzzleFull(reader);

                    await reader.NextResultAsync();
                    puzzleFull.PuzzleAnswerFulls = [];
                    while (await reader.ReadAsync())
                        puzzleFull.PuzzleAnswerFulls.Add(PuzzleAnswerDB.GetPuzzleAnswerFull(reader));

                    await reader.NextResultAsync();
                    if (await reader.ReadAsync())
                        puzzleFull.ContactFull = ContactDB.GetContactFull(reader);

                    await reader.NextResultAsync();
                    List<LinkFull> linkFulls = [];
                    while (await reader.ReadAsync())
                        linkFulls.Add(LinkDB.GetLinkFull(reader));
                    puzzleFull.LinkFulls = linkFulls;

                    await reader.NextResultAsync();
                    List<CommentFull> commentFulls = [];
                    while (await reader.ReadAsync())
                        commentFulls.Add(CommentDB.GetCommentFull(reader));
                    puzzleFull.CommentFulls = commentFulls;
                }
            }

            return puzzleFull;
        }

        public async Task<PuzzleDataFull> GetDataFullByStatus(int status)
        {
            String strCmd = $"SELECT {table}.Id, {table}.PostId," +
                             " Post.AppUserId, AppUser.Alias AS AppUserAlias, Post.PostTypeId," +
                             " Post.CountryId AS PostCountryId, Post.StateId AS PostStateId, Post.Title, Post.Summary, Post.Description," +
                             " Post.ImageCount, 0 AS Favorite, -1 AS [Like], Post.LikeCount, Post.PublicationDateTime, Post.Status," +
                            $" {table}.PuzzleGameId, {table}.CountryId, {table}.Question, {table}.Hint," +
                            $" {table}.Difficulty, {table}.Delay, {table}.Points, {table}.PlayCount, {table}.Status" +
                            $" FROM {table}" +
                            $" INNER JOIN [D-Post] AS Post ON ({table}.PostId = Post.Id)" +
                            $" INNER JOIN [D-AppUser] AS AppUser ON (Post.AppUserId = AppUser.Id)";

            if (status != -1)
                strCmd += $" WHERE {table}.Status = @Status;";
            else
                strCmd += ";";

            strCmd += "SELECT PuzzleAnswer.Id, PuzzleAnswer.PuzzleId, PuzzleAnswer.Description," +
                      " PuzzleAnswer.IsCorrect, PuzzleAnswer.Status" +
                      " FROM [D-PuzzleAnswer] AS PuzzleAnswer" +
                     $" JOIN {table} ON (PuzzleAnswer.PuzzleId = {table}.Id)" +
                      " WHERE 1 = 1" +
                      " AND PuzzleAnswer.Status = 1";

                        if (status != -1)
                            strCmd += $" AND {table}.Status = @Status";

                        strCmd += " ORDER BY PuzzleAnswer.CreateDateTime ASC;";

            strCmd += "SELECT Contact.Id, Contact.PostId, Contact.Name, Contact.Status" +
                      " FROM [D-Contact] AS Contact" +
                      $" INNER JOIN {table} ON (Contact.PostId = {table}.PostId)" +
                       " WHERE Contact.Status = 1";

            if (status != -1)
                strCmd += $" AND {table}.Status = @Status;";
            else
                strCmd += ";";

            strCmd += "SELECT Link.Id, Link.LinkTypeId, Link.PostId, Link.Url, Link.Status" +
                       " FROM [D-Link] AS Link" +
                      $" INNER JOIN {table} ON (Link.PostId = {table}.PostId)" +
                       " WHERE Link.Status = 1";

            if (status != -1)
                strCmd += $" AND {table}.Status = @Status;";
            else
                strCmd += ";";

            strCmd += "SELECT Comment.Id, Comment.PostId, Comment.AppUserId, AppUser.Alias AS AppUserAlias," +
                       " Comment.Message, Comment.UpdateDateTime, Comment.Status" +
                       " FROM [D-Comment] AS Comment" +
                       " INNER JOIN [D-AppUser] AS AppUser ON(Comment.AppUserId = AppUser.Id)" +
                      $" INNER JOIN {table}" +
                      $" ON (Comment.PostId = {table}.PostId)" +
                       " WHERE Comment.Status = 1";

            if (status != -1)
                strCmd += $" AND {table}.Status = @Status;";
            else
                strCmd += ";";

            SqlCommand command = new SqlCommand(strCmd, conn);

            if (status != -1)
                DBHelper.AddParam(command, "@Status", SqlDbType.Int, status);

            PuzzleDataFull puzzleDataFull = new PuzzleDataFull();
            using (conn)
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    List<PuzzleFull> puzzleFulls = [];
                    while (await reader.ReadAsync())
                        puzzleFulls.Add(GetPuzzleFull(reader));
                    puzzleDataFull.PuzzleFulls = puzzleFulls;

                    await reader.NextResultAsync();
                    List<PuzzleAnswerFull> puzzleAnswerFulls = [];
                    while (await reader.ReadAsync())
                        puzzleAnswerFulls.Add(PuzzleAnswerDB.GetPuzzleAnswerFull(reader));
                    puzzleDataFull.PuzzleAnswerFulls = puzzleAnswerFulls;

                    await reader.NextResultAsync();
                    List<ContactFull> contactFulls = [];
                    while (await reader.ReadAsync())
                        contactFulls.Add(ContactDB.GetContactFull(reader));
                    puzzleDataFull.ContactFulls = contactFulls;

                    await reader.NextResultAsync();
                    List<LinkFull> linkFulls = [];
                    while (await reader.ReadAsync())
                        linkFulls.Add(LinkDB.GetLinkFull(reader));
                    puzzleDataFull.LinkFulls = linkFulls;

                    await reader.NextResultAsync();
                    List<CommentFull> commentFulls = [];
                    while (await reader.ReadAsync())
                        commentFulls.Add(CommentDB.GetCommentFull(reader));
                    puzzleDataFull.CommentFulls = commentFulls;
                }
            }

            return puzzleDataFull;
        }

        public async Task<long> GetNextPuzzle(PuzzleNextRequest puzzleNextRequest)
        {
            String strCmd = @"SELECT TOP 1 P.Id
                              FROM [D-Puzzle] P
                              WHERE P.PuzzleGameId = @PuzzleGameId
                              AND P.CountryId = @CountryId
                              AND P.Difficulty = @Difficulty
                              AND P.Status = 1
                              AND P.Id NOT IN
                              (
                                SELECT PR.PuzzleId
                                FROM [J-PuzzleResult] PR
                                WHERE PR.PlayerId = @PlayerId
                              )
                              ORDER BY P.CreateDateTime ASC";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@PlayerId", SqlDbType.BigInt, puzzleNextRequest.PlayerId);
            DBHelper.AddParam(command, "@PuzzleGameId", SqlDbType.BigInt, puzzleNextRequest.PuzzleGameId);
            DBHelper.AddParam(command, "@CountryId", SqlDbType.BigInt, puzzleNextRequest.CountryId);
            DBHelper.AddParam(command, "@Difficulty", SqlDbType.Int, puzzleNextRequest.Difficulty);

            using (conn)
            {
                await conn.OpenAsync();

                long? result = (long?)await command.ExecuteScalarAsync();

                return result ?? -1;
            }
        }

        // INSERT
        public async Task<long> Add(Puzzle puzzle)
        {
            String strCmd = $"INSERT INTO {table}(Id, PostId, PuzzleGameId, CountryId, Question, Hint, Difficulty, Delay, Points, PlayCount, CreateDateTime, UpdateDateTime, Status)" + 
                            " OUTPUT INSERTED.Id" +
                            " VALUES (@Id, @PostId, @PuzzleGameId, @CountryId, @Question, @Hint, @Difficulty, @Delay, @Points, @PlayCount, @CreateDateTime, @UpdateDateTime, @Status)";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Id", SqlDbType.BigInt, SecurityFunctions.GetUid('Z'));
            DBHelper.AddParam(command, "@PostId", SqlDbType.BigInt, puzzle.PostId);
            DBHelper.AddParam(command, "@PuzzleGameId", SqlDbType.BigInt, puzzle.PuzzleGameId);
            DBHelper.AddParam(command, "@CountryId", SqlDbType.BigInt, puzzle.CountryId);
            DBHelper.AddParam(command, "@Question", SqlDbType.VarChar, puzzle.Question);
            DBHelper.AddParam(command, "@Hint", SqlDbType.VarChar, puzzle.Hint);
            DBHelper.AddParam(command, "@Difficulty", SqlDbType.Int, puzzle.Difficulty);
            DBHelper.AddParam(command, "@Delay", SqlDbType.Int, puzzle.Delay);
            DBHelper.AddParam(command, "@Points", SqlDbType.Int, puzzle.Points);
            DBHelper.AddParam(command, "@PlayCount", SqlDbType.Int, puzzle.PlayCount);
            DBHelper.AddParam(command, "@CreateDateTime", SqlDbType.DateTime, DateTime.Now);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime, DateTime.Now);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, puzzle.Status);

            using (conn)
            {
                await conn.OpenAsync();
                return (long)await command.ExecuteScalarAsync();
            }
        }

        // UPDATE
        public async Task<bool> Update(Puzzle puzzle)
        {
            String strCmd = $"UPDATE {table} SET PostId = @PostId, PuzzleGameId = @PuzzleGameId, CountryId = @CountryId, Question = @Question, Hint = @Hint, Difficulty = @Difficulty, Delay = @Delay, Points = @Points, PlayCount = @PlayCount, UpdateDateTime = @UpdateDateTime, Status = @Status WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@PostId", SqlDbType.BigInt, puzzle.PostId);
            DBHelper.AddParam(command, "@PuzzleGameId", SqlDbType.BigInt, puzzle.PuzzleGameId);
            DBHelper.AddParam(command, "@CountryId", SqlDbType.BigInt, puzzle.CountryId);
            DBHelper.AddParam(command, "@Question", SqlDbType.VarChar, puzzle.Question);
            DBHelper.AddParam(command, "@Hint", SqlDbType.VarChar, puzzle.Hint);
            DBHelper.AddParam(command, "@Difficulty", SqlDbType.Int, puzzle.Difficulty);
            DBHelper.AddParam(command, "@Delay", SqlDbType.Int, puzzle.Delay);
            DBHelper.AddParam(command, "@Points", SqlDbType.Int, puzzle.Points);
            DBHelper.AddParam(command, "@PlayCount", SqlDbType.Int, puzzle.PlayCount);
            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime, DateTime.Now);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, puzzle.Status);
            DBHelper.AddParam(command, "@Id", SqlDbType.BigInt, puzzle.Id);

            using (conn)
            {
                await conn.OpenAsync();
                return await command.ExecuteNonQueryAsync() == 1;
            }
        }

        public async Task<bool> UpdateStatus(long id, int status)
        {
            String strCmd = $"UPDATE {table}" +
                            " SET UpdateDateTime = @UpdateDateTime, Status = @Status" +
                            " WHERE Id = @Id";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime, DateTime.Now);
            DBHelper.AddParam(command, "@Status", SqlDbType.Int, status);
            DBHelper.AddParam(command, "@Id", SqlDbType.BigInt, id);

            using (conn)
            {
                await conn.OpenAsync();
                return await command.ExecuteNonQueryAsync() == 1;
            }
        }

        public async Task<bool> UpdateStatusByPostId(long postId, int curStatus, int newStatus)
        {
            String strCmd = $"UPDATE {table}" +
                            " SET UpdateDateTime = @UpdateDateTime, Status = @NewStatus" +
                            " WHERE PostId = @PostId AND Status = @CurStatus";

            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@UpdateDateTime", SqlDbType.DateTime2, DateTime.Now);
            DBHelper.AddParam(command, "@CurStatus", SqlDbType.Int, curStatus);
            DBHelper.AddParam(command, "@NewStatus", SqlDbType.Int, newStatus);
            DBHelper.AddParam(command, "@PostId", SqlDbType.BigInt, postId);

            using (conn)
            {
                await conn.OpenAsync();
                return await command.ExecuteNonQueryAsync() == 1;
            }
        }

        // DELETE
        public async Task<int> DeleteAll()
        {
            String strCmd = $"DELETE {table}";
            SqlCommand command = new SqlCommand(strCmd, conn);

            using (conn)
            {
                await conn.OpenAsync();
                return await command.ExecuteNonQueryAsync();
            }
        }

        public async Task<bool> DeleteById(long id)
        {
            String strCmd = $"DELETE {table} WHERE Id = @Id";
            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@Id", SqlDbType.BigInt, id);

            using (conn)
            {
                await conn.OpenAsync();
                return await command.ExecuteNonQueryAsync() == 1;
            }
        }

        public async Task<bool> DeleteByPostId(long postId)
        {
            String strCmd = $"DELETE {table} WHERE PostId = @PostId";
            SqlCommand command = new SqlCommand(strCmd, conn);

            DBHelper.AddParam(command, "@PostId", SqlDbType.BigInt, postId);

            using (conn)
            {
                await conn.OpenAsync();
                return await command.ExecuteNonQueryAsync() == 1;
            }
        }
    }
}
