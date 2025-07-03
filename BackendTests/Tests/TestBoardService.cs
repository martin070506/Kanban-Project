using System;
using IntroSE.Kanban.Backend.ServiceLayer;
using IntroSE.Kanban.Backend.ServiceLayer.models;
using IntroSE.Kanban.Backend.ServiceLayer.Models;

namespace BackendTests.Tests
{
    /// <summary>
    /// Provides acceptance tests for the board-related functionality of the Kanban system.
    /// Uses a shared test user and board, resetting the board before each test and cleaning up at the end.
    /// </summary>
    public class TestBoardService : AbstractTestRunner
    {
        private const string TestEmail = "martin@gmail.com";
        private const string TestPassword = "Bb1234!5";
        private const string TestBoardName = "TestBoard";
        private FactoryService _factoryService = new FactoryService();

        public override void RunAllTests()
        {
            Console.WriteLine("Running Board Service Tests...");
            try
            {
                _factoryService.DataService.DeleteAll();

                RunTest("Test Creation Good, Non Dupe Board", TestCreateBoardGood());

                _factoryService.DataService.DeleteAll();

                RunTest("Test Creation Duplicate Boards", TestCreateBoardBadDupeName());

                _factoryService.DataService.DeleteAll();

                RunTest("Test Deletion, Existing Board", TestDeleteBoardDoesExist());

                _factoryService.DataService.DeleteAll();

                RunTest("Test Deletion, Non Existing Board", TestDeleteBoardDoesntExist());

                _factoryService.DataService.DeleteAll();

                RunTest(
                    "Test Get Column Name by Ordinal Exists",
                    TestGetNameByColumnOrdinalExists()
                );

                _factoryService.DataService.DeleteAll();
                ;
                RunTest(
                    "Test Get Column Name by Ordinal Does Not Exist Below",
                    TestGetNameByColumnOrdinalDoesNotExistBelow()
                );

                _factoryService.DataService.DeleteAll();

                RunTest(
                    "Test Get Column Name by Ordinal Does Not Exist Above",
                    TestGetNameByColumnOrdinalDoesNotExistAbove()
                );

                _factoryService.DataService.DeleteAll();

                RunTest("Test Get Column Limit No Limit", GetColumnLimitNoLimit());

                _factoryService.DataService.DeleteAll();

                RunTest("Test Get Column Limit After Set Limit", GetColumnLimitAfterSetLimit());

                _factoryService.DataService.DeleteAll();

                RunTest("Test Get Column Limit No Column", GetColumnLimitNoColumn());

                _factoryService.DataService.DeleteAll();

                RunTest("Test Set Column Limit Valid", SetColumnLimitValid());

                _factoryService.DataService.DeleteAll();

                RunTest("Test Set Column Limit Negative Limit", SetColumnLimitNegativeLimit());

                _factoryService.DataService.DeleteAll();

                RunTest(
                    "Test Set Column Limit Non Existent Board",
                    SetColumnLimitNonExistentBoard()
                );

                _factoryService.DataService.DeleteAll();

                RunTest(
                    "Test Set Column Limit Unauthorized User",
                    SetColumnLimitUnauthorizedUser()
                );

                _factoryService.DataService.DeleteAll();

                RunTest("Test Set Column Limit Invalid Column", SetColumnLimitInvalidColumn());

                _factoryService.DataService.DeleteAll();

                RunTest("Test Get Board Name Valid", GetBoardNameValid());

                _factoryService.DataService.DeleteAll();

                RunTest("Test Get Board Name Non Existent", GetBoardNameNonExistent());

                _factoryService.DataService.DeleteAll();

                RunTest("Test Transfer Ownership Valid", TransferOwnershipValid());

                _factoryService.DataService.DeleteAll();

                RunTest(
                    "Test Transfer Ownership Non Existent Board",
                    TransferOwnershipNonExistentBoard()
                );

                _factoryService.DataService.DeleteAll();

                RunTest("Test Transfer Ownership Invalid User", TransferOwnershipInvalidUser());
            }
            finally
            {
                _factoryService.DataService.DeleteAll();
            }
            Console.WriteLine("All Board Service Tests Completed.");
        }

        public bool TestCreateBoardGood()
        {
            _factoryService.UserService.Register(TestEmail, TestPassword);
            Response<BoardSL> res = _factoryService.BoardService.CreateBoard(TestEmail, "Test1");
            return res.ErrorMessage == null && res.ReturnValue is BoardSL;
        }

        public bool TestCreateBoardBadDupeName()
        {
            _factoryService.UserService.Register(TestEmail, TestPassword);

            _factoryService.BoardService.CreateBoard(TestEmail, "Test2");
            Response<BoardSL> res = _factoryService.BoardService.CreateBoard(TestEmail, "Test2");
            return res.ErrorMessage != null
                && (res.ReturnValue == null || res.ReturnValue.GetType() != typeof(BoardSL));
        }

        public bool TestDeleteBoardDoesntExist()
        {
            _factoryService.UserService.Register(TestEmail, TestPassword);

            _factoryService.BoardService.CreateBoard(TestEmail, "Test3");
            Response<bool> res = _factoryService.BoardService.DeleteBoard(TestEmail, "Test3XXX");
            return res.ErrorMessage != null;
        }

        public bool TestDeleteBoardDoesExist()
        {
            _factoryService.UserService.Register(TestEmail, TestPassword);

            _factoryService.BoardService.CreateBoard(TestEmail, "Test4");
            Response<bool> res = _factoryService.BoardService.DeleteBoard(TestEmail, "Test4");
            return res.ErrorMessage == null && res.ReturnValue is bool;
        }

        public bool TestGetNameByColumnOrdinalExists()
        {
            _factoryService.UserService.Register(TestEmail, TestPassword);

            _factoryService.BoardService.CreateBoard(TestEmail, "Test5");
            Response<string> res = _factoryService.BoardService.GetColumnNameById(
                TestEmail,
                "Test5",
                0
            );
            return res.ErrorMessage == null && res.ReturnValue is string;
        }

        public bool TestGetNameByColumnOrdinalDoesNotExistBelow()
        {
            _factoryService.UserService.Register(TestEmail, TestPassword);

            _factoryService.BoardService.CreateBoard(TestEmail, "Test6");
            Response<string> res = _factoryService.BoardService.GetColumnNameById(
                TestEmail,
                "Test6",
                -2
            );
            return res.ErrorMessage != null;
        }

        public bool TestGetNameByColumnOrdinalDoesNotExistAbove()
        {
            _factoryService.UserService.Register(TestEmail, TestPassword);

            _factoryService.BoardService.CreateBoard(TestEmail, "Test7");
            Response<string> res = _factoryService.BoardService.GetColumnNameById(
                TestEmail,
                "Test7",
                int.MaxValue
            );
            return res.ErrorMessage != null;
        }

        public bool GetColumnLimitNoLimit()
        {
            int columnOrdinal = 0;
            int defaultNoLimit = -1;
            _factoryService.UserService.Register(TestEmail, TestPassword);

            _factoryService.BoardService.CreateBoard(TestEmail, "Test8");
            Response<int?> res = _factoryService.BoardService.GetColumnLimit(
                TestEmail,
                "Test8",
                columnOrdinal
            );
            return res.ErrorMessage == null
                && res.ReturnValue is int
                && res.ReturnValue == defaultNoLimit;
        }

        public bool GetColumnLimitAfterSetLimit()
        {
            int columnOrdinal = 0;
            int newLimit = 100;
            _factoryService.UserService.Register(TestEmail, TestPassword);

            _factoryService.BoardService.CreateBoard(TestEmail, "Test9");
            _factoryService.BoardService.SetColumnLimit(
                TestEmail,
                "Test9",
                columnOrdinal,
                newLimit
            );
            Response<int?> res = _factoryService.BoardService.GetColumnLimit(
                TestEmail,
                "Test9",
                columnOrdinal
            );
            return res.ErrorMessage == null
                && res.ReturnValue is int
                && res.ReturnValue == newLimit;
        }

        public bool GetColumnLimitNoColumn()
        {
            int columnOrdinal = 21;
            _factoryService.UserService.Register(TestEmail, TestPassword);

            _factoryService.BoardService.CreateBoard(TestEmail, "Test10");
            Response<int?> res = _factoryService.BoardService.GetColumnLimit(
                TestEmail,
                "Test10",
                columnOrdinal
            );
            return res.ErrorMessage != null;
        }

        public bool SetColumnLimitValid()
        {
            int columnOrdinal = 0;
            int limitValue = 50;
            _factoryService.UserService.Register(TestEmail, TestPassword);

            _factoryService.BoardService.CreateBoard(TestEmail, "Test11");
            Response<BoardSL> res = _factoryService.BoardService.SetColumnLimit(
                TestEmail,
                "Test11",
                columnOrdinal,
                limitValue
            );
            return res.ErrorMessage == null && res.ReturnValue is BoardSL;
        }

        public bool SetColumnLimitNegativeLimit()
        {
            int columnOrdinal = 0;
            int limitValue = -10;
            _factoryService.UserService.Register(TestEmail, TestPassword);

            _factoryService.BoardService.CreateBoard(TestEmail, "Test12");
            Response<BoardSL> res = _factoryService.BoardService.SetColumnLimit(
                TestEmail,
                "Test12",
                columnOrdinal,
                limitValue
            );
            return res.ErrorMessage != null;
        }

        public bool SetColumnLimitNonExistentBoard()
        {
            int columnOrdinal = 0;
            int limitValue = 20;
            Response<BoardSL> res = _factoryService.BoardService.SetColumnLimit(
                TestEmail,
                "Test13XXXX",
                columnOrdinal,
                limitValue
            );
            return res.ErrorMessage != null;
        }

        public bool SetColumnLimitUnauthorizedUser()
        {
            _factoryService.UserService.Register(TestEmail, TestPassword);

            _factoryService.BoardService.CreateBoard(TestEmail, "Test14");
            Response<BoardSL> res = _factoryService.BoardService.SetColumnLimit(
                "anotheruser@gmail.com",
                "Test14",
                0,
                30
            );
            return res.ErrorMessage != null;
        }

        public bool SetColumnLimitInvalidColumn()
        {
            _factoryService.UserService.Register(TestEmail, TestPassword);

            _factoryService.BoardService.CreateBoard(TestEmail, "Test15");
            Response<BoardSL> res = _factoryService.BoardService.SetColumnLimit(
                TestEmail,
                "Test15",
                999,
                50
            );
            return res.ErrorMessage != null;
        }

        public bool GetBoardNameValid()
        {
            _factoryService.UserService.Register(TestEmail, TestPassword);

            BoardSL board = _factoryService
                .BoardService.CreateBoard(TestEmail, "Test16")
                .ReturnValue;
            Response<string> res = _factoryService.BoardService.GetBoardName(board.BoardID);
            return res.ErrorMessage == null && res.ReturnValue == "Test16";
        }

        public bool GetBoardNameNonExistent()
        {
            int RandomBoardId = 69;
            Response<string> res = _factoryService.BoardService.GetBoardName(RandomBoardId);
            return res.ErrorMessage != null;
        }

        public bool TransferOwnershipValid()
        {
            _factoryService.UserService.Register(TestEmail, TestPassword);

            BoardSL board = _factoryService
                .BoardService.CreateBoard(TestEmail, "Test17")
                .ReturnValue;

                _factoryService.UserService.Register("newowner17@gmail.com", TestPassword);
                _factoryService.UserBoardsService.JoinBoard("newowner17@gmail.com", board.boardID);
                Response<Object> res = _factoryService.BoardService.TransferOwnerShip(
                    TestEmail,
                    "newowner17@gmail.com",
                    "Test17"
                );

            return res.ErrorMessage == null && res.ReturnValue == null;
        }

        public bool TransferOwnershipNonExistentBoard()
        {
            _factoryService.UserService.Register(TestEmail, TestPassword);
            _factoryService.UserService.Register("newowner18@gmail.com", TestPassword);

            Response<Object> res = _factoryService.BoardService.TransferOwnerShip(
                TestEmail,
                "newowner18@gmail.com",
                "NonExistentBoard18"
            );

            return res.ErrorMessage != null;
        }

        public bool TransferOwnershipInvalidUser()
        {
            _factoryService.UserService.Register(TestEmail, TestPassword);

            _factoryService.BoardService.CreateBoard(TestEmail, "Test19");
            Response<Object> res = _factoryService.BoardService.TransferOwnerShip(
                TestEmail,
                "fakeuser19@gmail.com",
                "Test19"
                );

            return res.ErrorMessage != null;
        }
    }
}
