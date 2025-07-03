using System;
using System.Collections.Generic;
using IntroSE.Kanban.Backend.ServiceLayer;
using IntroSE.Kanban.Backend.ServiceLayer.models;

namespace BackendTests.Tests
{
    /// <summary>
    /// Provides acceptance tests for the UserBoardsService.
    /// This version creates a shared test user and board in Setup(),
    /// resets the board state before each test, and cleans up in TearDown().
    /// </summary>
    internal class TestUserBoardsService : AbstractTestRunner
    {
        private const string TestEmail = "tal@gmail.com";
        private const string TestPassword = "Aasq123!2@";
        private const string TestBoardName = "Tal's Board";
        private FactoryService factoryService = new FactoryService();

        public override void RunAllTests()
        {
            Console.WriteLine("Running UserBoardsService Tests...");
            factoryService.DataService.DeleteAll();
            try
            {
                factoryService.DataService.DeleteAll();

                RunTest("Test Add User To Board Pass", TestAddUserToBoardPass());

                factoryService.DataService.DeleteAll();

                RunTest(
                    "Test Add User To Board User Not Logged In",
                    TestAddUserToBoardUserNotLogin()
                );

                factoryService.DataService.DeleteAll();

                RunTest("Test Add User To Non-Existent Board", TestAddUserToNonExistentBoard());

                factoryService.DataService.DeleteAll();

                RunTest(
                    "Test Add User To Board Already In Board",
                    TestAddUserToBoardAlreadyInBoard()
                );

                factoryService.DataService.DeleteAll();

                RunTest("Test Remove User From Board Pass", TestRemoveUserFromBoardPass());

                factoryService.DataService.DeleteAll();

                RunTest(
                    "Test Remove User From Board User Not Logged In",
                    TestRemoveUserFromBoardUserNotLogin()
                );

                factoryService.DataService.DeleteAll();

                RunTest(
                    "Test Remove User From Non-Existent Board",
                    TestRemoveUserFromNonExistentBoard()
                );

                factoryService.DataService.DeleteAll();

                RunTest(
                    "Test Remove User From Board Not In Board",
                    TestRemoveUserFromBoardNotInBoard()
                );

                factoryService.DataService.DeleteAll();

                RunTest("Test Get User Boards Pass", TestGetUserBoardsPass());

                // For tests where user logout is required, do not call ResetBoardState again.
                RunTest(
                    "Test Get User Boards Fail User Logged Out",
                    TestGetUserBoardsFailUserLogout()
                );

                factoryService.DataService.DeleteAll();

                RunTest(
                    "Test Get User Boards Fail Board Not Exist",
                    TestGetUserBoardsFailBoardNotExist()
                );
            }
            finally
            {
                factoryService.DataService.DeleteAll();
            }
            Console.WriteLine("All UserBoardsService Tests Completed.");
        }

        public bool TestAddUserToBoardPass()
        {
            string RandomEmail = "TestEmail1@gmail.com";
            factoryService.UserService.Register(TestEmail, TestPassword);
            factoryService.BoardService.CreateBoard(TestEmail, TestBoardName);
            factoryService.UserService.Register(RandomEmail, TestPassword);

            int boardID = factoryService
                .UserBoardsService.GetBoardID(TestEmail, TestBoardName)
                .ReturnValue;
            Response<object> res = factoryService.UserBoardsService.JoinBoard(
                RandomEmail,
                boardID
            );

            return res.ErrorMessage == null;
        }

        public bool TestAddUserToBoardUserNotLogin()
        {
            string RandomEmail = "TestEmail2@gmail.com";
            factoryService.UserService.Register(RandomEmail, TestPassword);
            factoryService.UserService.Register(TestEmail, TestPassword);

            factoryService.UserService.Logout(RandomEmail);
            factoryService.BoardService.CreateBoard(TestEmail, TestBoardName);

            int boardID = factoryService
                .UserBoardsService.GetBoardID(TestEmail, TestBoardName)
                .ReturnValue;
            Response<object> res = factoryService.UserBoardsService.JoinBoard(
                RandomEmail,
                boardID
            );
            return res.ErrorMessage != null;
        }

        public bool TestAddUserToNonExistentBoard()
        {
            int integerToAddForInvalidBoardID = 371;
            factoryService.UserService.Register(TestEmail, TestPassword);
            factoryService.BoardService.CreateBoard(TestEmail, TestBoardName);

            int boardID =
                factoryService.UserBoardsService.GetBoardID(TestEmail, TestBoardName).ReturnValue
                + integerToAddForInvalidBoardID;
            Response<object> res = factoryService.UserBoardsService.JoinBoard(TestEmail, boardID);
            return res.ErrorMessage != null;
        }

        public bool TestAddUserToBoardAlreadyInBoard()
        {
            factoryService.UserService.Register(TestEmail, TestPassword);
            factoryService.BoardService.CreateBoard(TestEmail, TestBoardName);

            int boardID = factoryService
                .UserBoardsService.GetBoardID(TestEmail, TestBoardName)
                .ReturnValue;
            factoryService.UserBoardsService.JoinBoard(TestEmail, boardID);
            Response<object> res = factoryService.UserBoardsService.JoinBoard(TestEmail, boardID);
            return res.ErrorMessage != null;
        }

        public bool TestRemoveUserFromBoardPass()
        {
            string RandomEmail = "TestEmail3@gmail.com";
            factoryService.UserService.Register(TestEmail, TestPassword);
            factoryService.UserService.Register(RandomEmail, TestPassword);
            factoryService.BoardService.CreateBoard(TestEmail, TestBoardName);

            int boardID = factoryService
                .UserBoardsService.GetBoardID(TestEmail, TestBoardName)
                .ReturnValue;
            factoryService.UserBoardsService.JoinBoard(RandomEmail, boardID);
            Response<object> res = factoryService.UserBoardsService.LeaveBoard(
                RandomEmail,
                boardID
            );
            return res.ErrorMessage == null;
        }

        public bool TestRemoveUserFromBoardUserNotLogin()
        {
            factoryService.UserService.Register(TestEmail, TestPassword);
            factoryService.BoardService.CreateBoard(TestEmail, TestBoardName);

            int boardID = factoryService
                .UserBoardsService.GetBoardID(TestEmail, TestBoardName)
                .ReturnValue;
            string RandomEmail = "TestEmail4@gmail.com";
            factoryService.UserService.Register(RandomEmail, TestPassword);
            factoryService.UserBoardsService.JoinBoard(RandomEmail, boardID);
            factoryService.UserService.Logout(RandomEmail);

            Response<object> res = factoryService.UserBoardsService.LeaveBoard(
                RandomEmail,
                boardID
            );
            return res.ErrorMessage != null;
        }

        public bool TestRemoveUserFromNonExistentBoard()
        {
            int RandomIndexToAdd = 313;
            factoryService.UserService.Register(TestEmail, TestPassword);
            factoryService.BoardService.CreateBoard(TestEmail, TestBoardName);

            int boardID =
                factoryService.UserBoardsService.GetBoardID(TestEmail, TestBoardName).ReturnValue
                + RandomIndexToAdd;
            Response<object> res = factoryService.UserBoardsService.LeaveBoard(TestEmail, boardID);
            return res.ErrorMessage != null;
        }

        public bool TestRemoveUserFromBoardNotInBoard()
        {
            factoryService.UserService.Register(TestEmail, TestPassword);
            factoryService.BoardService.CreateBoard(TestEmail, TestBoardName);

            int boardID = factoryService
                .UserBoardsService.GetBoardID(TestEmail, TestBoardName)
                .ReturnValue;
            string RandomEmail = "TestEmail5@gmail.com";
            factoryService.UserService.Register(RandomEmail, TestPassword);
            Response<object> res = factoryService.UserBoardsService.LeaveBoard(
                RandomEmail,
                boardID
            );
            return res.ErrorMessage != null;
        }

        /// <summary>
        /// Tests retrieving the list of boardByID a user is a member of successfully.
        /// </summary>
        /// <returns>True if the operation succeeds, false otherwise.</returns>
        public bool TestGetUserBoardsPass()
        {
            factoryService.UserService.Register(TestEmail, TestPassword);
            factoryService.BoardService.CreateBoard(TestEmail, TestBoardName);
            int boardID = factoryService
                .UserBoardsService.GetBoardID(TestEmail, TestBoardName)
                .ReturnValue;
            string RandomEmail = "TestEmail6@gmail.com";
            factoryService.UserService.Register(RandomEmail, TestPassword);
            factoryService.UserBoardsService.JoinBoard(RandomEmail, boardID);

            Response<List<int>> res = factoryService.UserBoardsService.GetUserBoards(RandomEmail);
            return res.ErrorMessage == null
                && res.ReturnValue != null
                && res.ReturnValue.Contains(boardID);
        }

        /// <summary>
        /// Tests retrieving the list of boardByID a user is a member of when the user is logged out.
        /// </summary>
        /// <returns>True if the operation fails, false otherwise.</returns>
        public bool TestGetUserBoardsFailUserLogout()
        {
            factoryService.UserService.Register(TestEmail, TestPassword);
            factoryService.BoardService.CreateBoard(TestEmail, TestBoardName);

            int boardID = factoryService
                .UserBoardsService.GetBoardID(TestEmail, TestBoardName)
                .ReturnValue;
            string RandomEmail = "TestEmail7@gmail.com";
            factoryService.UserService.Register(RandomEmail, TestPassword);
            factoryService.UserBoardsService.JoinBoard(RandomEmail, boardID);
            factoryService.UserService.Logout(RandomEmail);
            Response<List<int>> res = factoryService.UserBoardsService.GetUserBoards(RandomEmail);
            return res.ErrorMessage != null;
        }

        /// <summary>
        /// Tests retrieving the list of boardByID a user is a member of when the board does not exist.
        /// </summary>
        /// <returns>True if the operation fails, false otherwise.</returns>
        public bool TestGetUserBoardsFailBoardNotExist()
        {
            factoryService.UserService.Register(TestEmail, TestPassword);
            factoryService.BoardService.CreateBoard(TestEmail, TestBoardName);

            int RandomIndexToAddForNotValidID = 3611;
            int boardID =
                factoryService.UserBoardsService.GetBoardID(TestEmail, TestBoardName).ReturnValue
                + RandomIndexToAddForNotValidID;
            string RandomEmail = "TestEmail8@gmail.com";

            factoryService.UserService.Register(RandomEmail, TestPassword);
            factoryService.UserBoardsService.JoinBoard(RandomEmail, boardID);
            Response<List<int>> res = factoryService.UserBoardsService.GetUserBoards(RandomEmail);
            return res.ReturnValue.Count == 0;
        }
    }
}
