using IntroSE.Kanban.Backend.ServiceLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackendUnitTests
{
    class BoardHandlingTests
    {
        private FactoryService factoryService;
        private string testEmail;
        private string testPassword;
        private string boardName;

        [SetUp]
        public void Setup()
        {
            factoryService = new FactoryService();
            testEmail = "boarduser@example.com";
            testPassword = "ValidPass123";
            boardName = "My Test Board";

            factoryService.UserService.Register(testEmail, testPassword);
        }

        [TearDown]
        public void Cleanup()
        {
            try
            {
                factoryService.BoardService.DeleteBoard(testEmail, boardName);
            }
            catch { }

            try
            {
                factoryService.UserService.DeleteUser(testEmail);
            }
            catch { }
        }

        [Test]
        public void CreateBoard_ValidData_ShouldSucceed()
        {
            var response = factoryService.BoardService.CreateBoard(testEmail, boardName);
            Assert.IsNull(response.ErrorMessage);
            Assert.AreEqual(boardName, response.ReturnValue.name);
        }

        [Test]
        public void CreateBoard_EmptyName_ShouldFail()
        {
            var response = factoryService.BoardService.CreateBoard(testEmail, "");
            Assert.IsNotNull(response.ErrorMessage);
        }

        [Test]
        public void CreateBoard_DuplicateName_ShouldFail()
        {
            factoryService.BoardService.CreateBoard(testEmail, boardName);
            var response = factoryService.BoardService.CreateBoard(testEmail, boardName);
            Assert.IsNotNull(response.ErrorMessage);
        }

        [Test]
        public void DeleteBoard_Valid_ShouldSucceed()
        {
            factoryService.BoardService.CreateBoard(testEmail, boardName);
            var response = factoryService.BoardService.DeleteBoard(testEmail, boardName);
            Assert.AreEqual(true,response.ReturnValue);
        }

        [Test]
        public void DeleteBoard_NonExistent_ShouldFail()
        {
            var response = factoryService.BoardService.DeleteBoard(testEmail, "NonExistentBoard");
            Assert.AreNotEqual(true,response.ReturnValue);
        }

        [Test]
        public void GetColumnNameById_Valid_ShouldReturnBacklog()
        {
            factoryService.BoardService.CreateBoard(testEmail, boardName);
            var response = factoryService.BoardService.GetColumnNameById(testEmail, boardName, 0);
            Assert.AreEqual("backlog", response.ReturnValue.ToLower());
        }

        [Test]
        public void GetColumnNameById_InvalidOrdinal_ShouldFail()
        {
            factoryService.BoardService.CreateBoard(testEmail, boardName);
            var response = factoryService.BoardService.GetColumnNameById(testEmail, boardName, 99);
            Assert.IsNotNull(response.ErrorMessage);
        }

        [Test]
        public void SetAndGetColumnLimit_ShouldMatch()
        {
            factoryService.BoardService.CreateBoard(testEmail, boardName);
            var setLimitResponse = factoryService.BoardService.SetColumnLimit(testEmail, boardName, 0, 5);
            Assert.IsNull(setLimitResponse.ErrorMessage);

            var getLimitResponse = factoryService.BoardService.GetColumnLimit(testEmail, boardName, 0);
            Assert.AreEqual(5, getLimitResponse.ReturnValue);
        }

        [Test]
        public void SetColumnLimit_InvalidOrdinal_ShouldFail()
        {
            factoryService.BoardService.CreateBoard(testEmail, boardName);
            var response = factoryService.BoardService.SetColumnLimit(testEmail, boardName, 99, 5);
            Assert.IsNotNull(response.ErrorMessage);
        }

        [Test]
        public void GetBoardName_Valid_ShouldSucceed()
        {
            var createResponse = factoryService.BoardService.CreateBoard(testEmail, boardName);
            int boardId = createResponse.ReturnValue.boardID;
            var nameResponse = factoryService.BoardService.GetBoardName(boardId);
            Assert.AreEqual(boardName, nameResponse.ReturnValue);
        }

        [Test]
        public void GetBoardName_InvalidId_ShouldFail()
        {
            var response = factoryService.BoardService.GetBoardName(99999);
            Assert.IsNotNull(response.ErrorMessage);
        }

        [Test]
        public void GetBoardByID_Valid_ShouldReturnBoard()
        {
            var createResponse = factoryService.BoardService.CreateBoard(testEmail, boardName);
            int boardId = createResponse.ReturnValue.BoardID;
            var boardResponse = factoryService.BoardService.GetBoardByID(boardId);
            Assert.AreEqual(boardName, boardResponse.ReturnValue.name);
        }

        [Test]
        public void GetBoardByID_Invalid_ShouldFail()
        {
            var response = factoryService.BoardService.GetBoardByID(123456);
            Assert.IsNotNull(response.ErrorMessage);
        }

        [Test]
        public void GetBoardMembers_ShouldIncludeCreator()
        {
            factoryService.BoardService.CreateBoard(testEmail, boardName);
            var members = factoryService.BoardService.GetBoardMembers(testEmail, boardName);
            Assert.Contains(testEmail, members.ReturnValue);
        }
        [Test]
        public void CreateBoard_WithVeryLongName_ShouldSucceed()
        {
            string email = "longnameuser@example.com";
            string boardName = new string('A', 256);
            factoryService.UserService.Logout(email);
            factoryService.UserService.DeleteUser(email);
            factoryService.UserService.Register(email, testPassword);
            factoryService.BoardService.DeleteBoard(email, boardName);

            var response = factoryService.BoardService.CreateBoard(email, boardName);
           
            Assert.IsNull(response.ErrorMessage);
           
        }

        [Test]
        public void CreateBoard_WithWhitespaceName_ShouldFail()
        {
            string email = "whitespaceboard@example.com";
            factoryService.UserService.Register(email, testPassword);
            var response = factoryService.BoardService.CreateBoard(email, "   ");
            factoryService.UserService.Logout(email);
            factoryService.UserService.DeleteUser(email);
            Assert.IsNotNull(response.ErrorMessage);
            
        }

        [Test]
        public void SetColumnLimit_NegativeLimit_ShouldFail()
        {
            string email = "negativelimit@example.com";
            string boardName = "Board";
            factoryService.UserService.Register(email, testPassword);
            factoryService.BoardService.CreateBoard(email, boardName);
            var response = factoryService.BoardService.SetColumnLimit(email, boardName, 0, -1);
            Assert.IsNotNull(response.ErrorMessage);
            factoryService.BoardService.DeleteBoard(email, boardName);
            factoryService.UserService.Logout(email);
            factoryService.UserService.DeleteUser(email);
        }

        [Test]
        public void GetColumnName_InvalidNegativeIndex_ShouldFail()
        {
            string email = "invalidcolumn@example.com";
            string boardName = "Board";
            factoryService.UserService.Register(email, testPassword);
            factoryService.BoardService.CreateBoard(email, boardName);
            var response = factoryService.BoardService.GetColumnNameById(email, boardName, -2);
            Assert.IsNotNull(response.ErrorMessage);
            factoryService.BoardService.DeleteBoard(email, boardName);
            factoryService.UserService.Logout(email);
            factoryService.UserService.DeleteUser(email);
        }

        [Test]
        public void TransferOwnership_ToNonMember_ShouldFail()
        {
            string email1 = "owner@example.com";
            string email2 = "stranger@example.com";
            string boardName = "TransferBoard";
            factoryService.UserService.Register(email1, testPassword);
            factoryService.UserService.Register(email2, testPassword);
            factoryService.BoardService.CreateBoard(email1, boardName);
            var response = factoryService.BoardService.TransferOwnerShip(email1, email2, boardName);
            Assert.IsNotNull(response.ErrorMessage);
            factoryService.BoardService.DeleteBoard(email1, boardName);
            factoryService.UserService.Logout(email1);
            factoryService.UserService.DeleteUser(email1);
            factoryService.UserService.Logout(email2);
            factoryService.UserService.DeleteUser(email2);
        }

        [Test]
        public void GetBoardMembers_NonExistentBoard_ShouldFail()
        {
            string email = "ghost@example.com";
            factoryService.UserService.Register(email, testPassword);
            var response = factoryService.BoardService.GetBoardMembers(email, "UnknownBoard");
            Assert.IsNotNull(response.ErrorMessage);
            factoryService.UserService.Logout(email);
            factoryService.UserService.DeleteUser(email);
        }
        

        [Test]
        public void GetBoardMembers_UserNotMember_ShouldFail()
        {
            string ownerEmail = "owner@example.com";
            string strangerEmail = "stranger@example.com";
            string boardName = "TestBoard";

            factoryService.UserService.Register(ownerEmail, testPassword);
            factoryService.BoardService.CreateBoard(ownerEmail, boardName);
            factoryService.UserService.Register(strangerEmail, testPassword);

            var response = factoryService.BoardService.GetBoardMembers(strangerEmail, boardName);
            Assert.IsNotNull(response.ErrorMessage);

            factoryService.UserService.Logout(strangerEmail);
            factoryService.UserService.DeleteUser(strangerEmail);
            factoryService.UserService.Logout(ownerEmail);
            factoryService.BoardService.DeleteBoard(ownerEmail, boardName);
            factoryService.UserService.DeleteUser(ownerEmail);
        }

        

        [Test]
        public void TransferOwnership_ToSelf_ShouldSucceed()
        {
            string email = "selftransfer@example.com";
            string boardName = "SelfBoard";

            factoryService.UserService.Register(email, testPassword);
            factoryService.BoardService.CreateBoard(email, boardName);

            var response = factoryService.BoardService.TransferOwnerShip(email, email, boardName);
            Assert.IsNull(response.ErrorMessage);

            factoryService.UserService.Logout(email);
            factoryService.BoardService.DeleteBoard(email, boardName);
            factoryService.UserService.DeleteUser(email);


        }

        [Test]
        public void TransferOwnership_ByNonOwner_ShouldFail()
        {
            string owner = "realowner@example.com";
            string boardName = "SharedBoard";
            string otherUser = "notowner@example.com";

            factoryService.UserService.Register(owner, testPassword);
            factoryService.BoardService.CreateBoard(owner, boardName);
            factoryService.UserService.Register(otherUser, testPassword);
            factoryService.UserBoardsService.JoinBoard(otherUser, factoryService.UserBoardsService.GetBoardID(owner, boardName).ReturnValue);

            var response = factoryService.BoardService.TransferOwnerShip(otherUser, owner, boardName);
            Assert.IsNotNull(response.ErrorMessage);

            factoryService.UserService.Logout(otherUser);
            factoryService.UserService.DeleteUser(otherUser);
            factoryService.UserService.Logout(owner);
            factoryService.BoardService.DeleteBoard(owner, boardName);
            factoryService.UserService.DeleteUser(owner);
        }

        [Test]
        public void GetBoardName_InvalidID_ShouldFail()
        {
            var response = factoryService.BoardService.GetBoardName(-999);
            Assert.IsNotNull(response.ErrorMessage);
        }

        [Test]
        public void GetBoardByID_InvalidID_ShouldFail()
        {
            var response = factoryService.BoardService.GetBoardByID(999999); // assuming this ID doesn't exist
            Assert.IsNotNull(response.ErrorMessage);
        }
        [Test]
        public void CreateBoard_TransferOwnership_SuccessFlow()
        {
            string originalOwner = "combo_owner@example.com";
            string newOwner = "combo_newowner@example.com";
            string boardName = "ComboBoard1";

            factoryService.UserService.Register(originalOwner, testPassword);
            factoryService.BoardService.CreateBoard(originalOwner, boardName);
            factoryService.UserService.Register(newOwner, testPassword);
            int boardId = factoryService.UserBoardsService.GetBoardID(originalOwner, boardName).ReturnValue;
            factoryService.UserBoardsService.JoinBoard(newOwner, boardId);

            var transferResponse = factoryService.BoardService.TransferOwnerShip(originalOwner, newOwner, boardName);
            Assert.IsNull(transferResponse.ErrorMessage);

            factoryService.UserService.Logout(newOwner);
            factoryService.BoardService.DeleteBoard(newOwner, boardName);
            factoryService.UserService.DeleteUser(newOwner);
            factoryService.UserService.Logout(originalOwner);
            factoryService.UserService.DeleteUser(originalOwner);
        }

        [Test]
        public void Register_Join_TransferOwnership_SuccessFlow()
        {
            string owner = "register_owner@example.com";
            string joiner = "join_then_owner@example.com";
            string boardName = "ComboBoard2";

            factoryService.UserService.Register(owner, testPassword);
            factoryService.BoardService.CreateBoard(owner, boardName);
            factoryService.UserService.Register(joiner, testPassword);
            int boardId = factoryService.UserBoardsService.GetBoardID(owner, boardName).ReturnValue;
            factoryService.UserBoardsService.JoinBoard(joiner, boardId);

            var transferResponse = factoryService.BoardService.TransferOwnerShip(owner, joiner, boardName);
            Assert.IsNull(transferResponse.ErrorMessage);

            factoryService.UserService.Logout(joiner);
            factoryService.BoardService.DeleteBoard(joiner, boardName);
            factoryService.UserService.DeleteUser(joiner);
            factoryService.UserService.Logout(owner);
            factoryService.UserService.DeleteUser(owner);
        }

        [Test]
        public void CreateBoard_SetLimit_ThenTransferOwnership()
        {
            string email1 = "limit_owner@example.com";
            string email2 = "limit_receiver@example.com";
            string boardName = "LimitBoard";

            factoryService.UserService.Register(email1, testPassword);
            factoryService.BoardService.CreateBoard(email1, boardName);
            var setLimitRes = factoryService.BoardService.SetColumnLimit(email1, boardName, 0, 7);
            Assert.IsNull(setLimitRes.ErrorMessage);

            factoryService.UserService.Register(email2, testPassword);
            int boardId = factoryService.UserBoardsService.GetBoardID(email1, boardName).ReturnValue;
            factoryService.UserBoardsService.JoinBoard(email2, boardId);

            var transferRes = factoryService.BoardService.TransferOwnerShip(email1, email2, boardName);
            Assert.IsNull(transferRes.ErrorMessage);

            factoryService.UserService.Logout(email2);
            factoryService.BoardService.DeleteBoard(email2, boardName);
            factoryService.UserService.DeleteUser(email2);
            factoryService.UserService.Logout(email1);
            factoryService.UserService.DeleteUser(email1);
        }
    }
}
