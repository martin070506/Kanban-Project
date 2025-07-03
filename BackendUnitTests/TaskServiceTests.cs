using IntroSE.Kanban.Backend.ServiceLayer;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackendUnitTests
{
    class TaskServiceTests
    {    
        private FactoryService factoryService;
        private const string email = "ValidEmail@gmail.com";
        private const string password = "Liav1234";
        private const string boardName = "ValidBoardName";
        private DateTime dueDate = DateTime.Now.AddDays(1);
        private const string title = "ValidTitle";
        private const string description = "ValidDescription";


        [SetUp]
        public void Setup()
        {
            factoryService = new FactoryService();
            factoryService.DataService.DeleteAll(); // Clear any existing data before each test
        }

        [Test]
        public void CreateTask_ValidData_Success()
        {
            factoryService.UserService.Register(email, password);
            factoryService.BoardService.CreateBoard(email, boardName);
            var response = factoryService.TaskService.CreateTask(email, boardName, dueDate, title, description);

            Assert.IsNotNull(response.ReturnValue);
            Assert.IsNull(response.ErrorMessage);
            Assert.AreEqual(dueDate, response.ReturnValue.DueDate);
            Assert.AreEqual(title, response.ReturnValue.Title);
            Assert.AreEqual(description, response.ReturnValue.Description);

            factoryService.UserService.DeleteUser(email);
        }


        [Test]
        public void CreateTask_UnregisteredUser_Fail()
        {
            var response = factoryService.TaskService.CreateTask("fake@email.com", "BoardX", dueDate, "Task", "Desc");
            Assert.IsNotNull(response.ErrorMessage);
            Assert.IsNull(response.ReturnValue);
        }

        [Test]
        public void CreateTask_NonExistentBoard_Fail()
        {
            factoryService.UserService.Register(email, password);

            var response = factoryService.TaskService.CreateTask(email, "MissingBoard", dueDate, "Task", "Desc");

            Assert.IsNotNull(response.ErrorMessage);
            Assert.IsNull(response.ReturnValue);

            factoryService.UserService.DeleteUser(email);
        }

        [Test]
        public void CreateTask_EmptyTitle_Fail()
        {
            factoryService.UserService.Register(email, password);
            factoryService.BoardService.CreateBoard(email, boardName);

            var response = factoryService.TaskService.CreateTask(email, boardName, dueDate, "", "Desc");

            Assert.IsNotNull(response.ErrorMessage);
            Assert.IsNull(response.ReturnValue);

            factoryService.UserService.DeleteUser(email);
        }

        [Test]
        public void CreateTask_DescriptionTooLong_Fail()
        {
            factoryService.UserService.Register(email, password);
            factoryService.BoardService.CreateBoard(email, boardName);

            string longDescription = new string('A', 301); 

            var response = factoryService.TaskService.CreateTask(email, boardName, dueDate, "Task", longDescription);

            Assert.IsNotNull(response.ErrorMessage);
            Assert.IsNull(response.ReturnValue);

            factoryService.UserService.DeleteUser(email);
        }

        [Test]
        public void CreateTask_DescriptionMaxLength_Success()
        {
            factoryService.UserService.Register(email, password);
            factoryService.BoardService.CreateBoard(email, boardName);

            string desc = new string('B', 300); 

            var response = factoryService.TaskService.CreateTask(email, boardName, dueDate, "EdgeTask", desc);

            Assert.IsNull(response.ErrorMessage);
            Assert.AreEqual(desc, response.ReturnValue.Description);

            factoryService.UserService.DeleteUser(email);
        }

        public void CreateTask_TitleTooLong_Fail()
        {
            factoryService.UserService.Register(email, password);
            factoryService.BoardService.CreateBoard(email, boardName);

            string longTitle = new string('A', 51);

            var response = factoryService.TaskService.CreateTask(email, boardName, dueDate, longTitle, description);

            Assert.IsNotNull(response.ErrorMessage);
            Assert.IsNull(response.ReturnValue);

            factoryService.UserService.DeleteUser(email);
        }

        [Test]
        public void CreateTask_TitleMaxLength_Success()
        {
            factoryService.UserService.Register(email, password);
            factoryService.BoardService.CreateBoard(email, boardName);

            string maxTitle = new string('B', 50);

            var response = factoryService.TaskService.CreateTask(email, boardName, dueDate, maxTitle, description);

            Assert.IsNull(response.ErrorMessage);
            Assert.AreEqual(maxTitle, response.ReturnValue.Title);

            factoryService.UserService.DeleteUser(email);
        }



        [Test]
        public void MoveTaskToNextStage_ValidData_Success()
        {
            factoryService.UserService.Register(email, password);
            factoryService.BoardService.CreateBoard(email, boardName);
            var resTask = factoryService.TaskService.CreateTask(email, boardName, dueDate, title, description);
            factoryService.TaskService.AssignTask(email, boardName, resTask.ReturnValue.Id, email);
            var res = factoryService.TaskService.MoveTaskToNextStage(email, boardName, resTask.ReturnValue.Id);

            Assert.IsNotNull(res.ReturnValue);
            Assert.IsNull(res.ErrorMessage);
            Assert.AreEqual("in progress", res.ReturnValue.State);

            factoryService.UserService.DeleteUser(email);
        }

        [Test]
        public void MoveTaskToNextStage_UnregisteredUser_Fail()
        {
            var response = factoryService.TaskService.MoveTaskToNextStage("no@user.com", boardName, 0);
            Assert.IsNotNull(response.ErrorMessage);
            Assert.IsNull(response.ReturnValue);
        }


        [Test]
        public void MoveTaskToNextStage_NonexistentTask_Fail()
        {
            factoryService.UserService.Register(email, password);
            factoryService.BoardService.CreateBoard(email, boardName);

            var response = factoryService.TaskService.MoveTaskToNextStage(email, boardName, 9999); // ID לא קיים

            Assert.IsNotNull(response.ErrorMessage);
            Assert.IsNull(response.ReturnValue);

            factoryService.UserService.DeleteUser(email);
        }


        [Test]
        public void MoveTaskToNextStage_UnassignedUser_Fail()
        {
            factoryService.UserService.Register(email, password);
            factoryService.BoardService.CreateBoard(email, boardName);
            var resTask = factoryService.TaskService.CreateTask(email, boardName, dueDate, title, description);

            var response = factoryService.TaskService.MoveTaskToNextStage("other@user.com", boardName, resTask.ReturnValue.Id);

            Assert.IsNotNull(response.ErrorMessage);
            Assert.IsNull(response.ReturnValue);

            factoryService.UserService.DeleteUser(email);
        }


        [Test]
        public void MoveTaskToNextStage_AlreadyDone_Success()
        {
            factoryService.UserService.Register(email, password);
            factoryService.BoardService.CreateBoard(email, boardName);
            var resTask = factoryService.TaskService.CreateTask(email, boardName, dueDate, title, description);
            factoryService.TaskService.AssignTask(email, boardName, resTask.ReturnValue.Id, email);

            // Move twice: backlog → in progress → done
            factoryService.TaskService.MoveTaskToNextStage(email, boardName, resTask.ReturnValue.Id);
            factoryService.TaskService.MoveTaskToNextStage(email, boardName, resTask.ReturnValue.Id);

            var response = factoryService.TaskService.MoveTaskToNextStage(email, boardName, resTask.ReturnValue.Id); // move from done

            Assert.IsNull(response.ErrorMessage);
            Assert.IsNotNull(response.ReturnValue);

            factoryService.UserService.DeleteUser(email);
        }


        [Test]
        public void MoveTaskToNextStage_InitialToNext_Success()
        {
            factoryService.UserService.Register(email, password);
            factoryService.BoardService.CreateBoard(email, boardName);
            var resTask = factoryService.TaskService.CreateTask(email, boardName, dueDate, title, description);
            factoryService.TaskService.AssignTask(email, boardName, resTask.ReturnValue.Id, email);

            var response = factoryService.TaskService.MoveTaskToNextStage(email, boardName, resTask.ReturnValue.Id);

            Assert.IsNull(response.ErrorMessage);
            Assert.AreEqual("in progress", response.ReturnValue.State); // assuming columns: backlog → in progress → done

            factoryService.UserService.DeleteUser(email);
        }



        [Test]
        public void EditTask_ValidData_Success()
        {
            factoryService.UserService.Register(email, password);
            factoryService.BoardService.CreateBoard(email, boardName);
            var resTask = factoryService.TaskService.CreateTask(email, boardName, dueDate, title, description);
            factoryService.TaskService.AssignTask(email, boardName, resTask.ReturnValue.Id, email);
            var newDueDate = DateTime.Now.AddDays(2);
            var newTitle = "UpdatedTitle";
            var newDescription = "UpdatedDescription";
            var response = factoryService.TaskService.EditTask(email, boardName, resTask.ReturnValue.Id, newDueDate, newTitle, newDescription, email);
            Assert.IsNull(response.ErrorMessage);
            Assert.IsNotNull(response.ReturnValue);
            Assert.AreEqual(newDueDate, response.ReturnValue.DueDate);
            Assert.AreEqual(newTitle, response.ReturnValue.Title);
            Assert.AreEqual(newDescription, response.ReturnValue.Description);
            factoryService.UserService.DeleteUser(email);
        }

        [Test]
        public void EditTask_UnassignedUser_Fail()
        {
            factoryService.UserService.Register(email, password);
            factoryService.BoardService.CreateBoard(email, boardName);
            var resTask = factoryService.TaskService.CreateTask(email, boardName, dueDate, title, description);

            var anotherUser = "someone@else.com";
            factoryService.UserService.Register(anotherUser, password);

            var response = factoryService.TaskService.EditTask(anotherUser, boardName, resTask.ReturnValue.Id,
                DateTime.Now.AddDays(1), "New", "Edit", anotherUser);

            Assert.IsNotNull(response.ErrorMessage);
            Assert.IsNull(response.ReturnValue);

            factoryService.UserService.DeleteUser(email);
            factoryService.UserService.DeleteUser(anotherUser);
        }


        [Test]
        public void EditTask_NonExistentTask_Fail()
        {
            factoryService.UserService.Register(email, password);
            factoryService.BoardService.CreateBoard(email, boardName);

            var response = factoryService.TaskService.EditTask(email, boardName, 9999, DateTime.Now.AddDays(1), "T", "D", email);

            Assert.IsNotNull(response.ErrorMessage);
            Assert.IsNull(response.ReturnValue);

            factoryService.UserService.DeleteUser(email);
        }


        [Test]
        public void EditTask_EmptyTitle_Fail()
        {
            factoryService.UserService.Register(email, password);
            factoryService.BoardService.CreateBoard(email, boardName);
            var resTask = factoryService.TaskService.CreateTask(email, boardName, dueDate, title, description);
            factoryService.TaskService.AssignTask(email, boardName, resTask.ReturnValue.Id, email);

            var response = factoryService.TaskService.EditTask(email, boardName, resTask.ReturnValue.Id,
                DateTime.Now.AddDays(1), "", "New Desc", email);

            Assert.IsNotNull(response.ErrorMessage);
            Assert.IsNull(response.ReturnValue);

            factoryService.UserService.DeleteUser(email);
        }


        [Test]
        public void ViewAllTaskType_ValidData_Success()
        {
            factoryService.UserService.Register(email, password);
            factoryService.BoardService.CreateBoard(email, boardName);
            var resTask1 = factoryService.TaskService.CreateTask(email, boardName, dueDate, title, description);
            var resTask2 = factoryService.TaskService.CreateTask(email, boardName, dueDate.AddDays(1), "AnotherTitle", "AnotherDescription");
            factoryService.TaskService.AssignTask(email, boardName, resTask1.ReturnValue.Id, email);
            factoryService.TaskService.AssignTask(email, boardName, resTask2.ReturnValue.Id, email);
            var response = factoryService.TaskService.ViewAllTaskType(email, "backlog");
            Assert.IsNotNull(response.ReturnValue);
            Assert.IsNull(response.ErrorMessage);
            Assert.AreEqual(2, response.ReturnValue.Count); // Assuming both tasks are in backlog
            factoryService.UserService.DeleteUser(email);
        }

        [Test]
        public void ViewAllTaskType_UnregisteredUser_Fails()
        {
            var response = factoryService.TaskService.ViewAllTaskType("fake@email.com", "backlog");
            Assert.IsNotNull(response.ErrorMessage);
            Assert.IsNull(response.ReturnValue);
        }


        [Test]
        public void ViewAllTaskType_InvalidColumn_Fails()
        {
            factoryService.UserService.Register(email, password);
            factoryService.BoardService.CreateBoard(email, boardName);
            var response = factoryService.TaskService.ViewAllTaskType(email, "nonexistent_column");
            Assert.IsNotNull(response.ErrorMessage);
            Assert.IsNull(response.ReturnValue);
            factoryService.UserService.DeleteUser(email);
        }


        [Test]
        public void ViewAllTaskType_NoTasksInStage_ReturnsEmpty()
        {
            factoryService.UserService.Register(email, password);
            factoryService.BoardService.CreateBoard(email, boardName);

            var response = factoryService.TaskService.ViewAllTaskType(email, "backlog");

            Assert.IsNotNull(response.ReturnValue);
            Assert.IsNull(response.ErrorMessage);
            Assert.AreEqual(0, response.ReturnValue.Count);
            factoryService.UserService.DeleteUser(email);
        }


        [Test]
        public void ViewAllTaskType_TasksAssignedToOtherUser_AreNotReturned()
        {
            factoryService.UserService.Register(email, password);
            var otherUser = "other@email.com";
            factoryService.UserService.Register(otherUser, password);

            factoryService.BoardService.CreateBoard(email, boardName);
            var resTask = factoryService.TaskService.CreateTask(email, boardName, dueDate, title, description);
            factoryService.TaskService.AssignTask(email, boardName, resTask.ReturnValue.Id, otherUser);

            var response = factoryService.TaskService.ViewAllTaskType(email, "backlog");

            Assert.IsNotNull(response.ReturnValue);
            Assert.IsNull(response.ErrorMessage);
            Assert.AreEqual(0, response.ReturnValue.Count);

            factoryService.UserService.DeleteUser(email);
            factoryService.UserService.DeleteUser(otherUser);
        }
    }
}
