using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IntroSE.Kanban.Backend.ServiceLayer;
using IntroSE.Kanban.Backend.ServiceLayer.models;
using IntroSE.Kanban.Backend.ServiceLayer.Models;

namespace BackendTests.Tests
{
    /// <summary>
    /// Provides tests for the task-related capabilities of the Kanban system.
    /// This version uses a shared test user, board, and secondary user.
    /// Setup() creates the shared objects, ResetBoardState() refreshes the board before each test,
    /// and TearDown() cleans up at the end.
    /// </summary>
    internal class TestTaskService : AbstractTestRunner
    {
        private const string TestEmail = "liav@gmail.com";
        private const string SecondTestEmail = "martin@gmail.com";
        private const string UnregisterTestEmail = "tal@gmail.com";
        private const string TestPassword = "Liav1234";
        private const string SecondTestPassword = "Martin1234";
        private const string TestBoard = "liavBoard";

        private FactoryService factoryService = new FactoryService();

        public override void RunAllTests()
        {
            Console.WriteLine("Running Task Service Tests...");
            try
            {
                factoryService.DataService.DeleteAll();
                RunTest("Test Creating Task", CreateTaskTest_Valid());

                factoryService.DataService.DeleteAll();
                RunTest("Test Creating Task With Empty Title", CreateTaskTest_EmptyTitle());

                factoryService.DataService.DeleteAll();
                RunTest("Test Creating With Title Too Long", CreateTaskTest_TitleTooLong());

                factoryService.DataService.DeleteAll();
                RunTest("Test Creating With Description Empty", CreateTaskTest_DescriptionEmpty());

                factoryService.DataService.DeleteAll();
                RunTest(
                    "Test Creating With Description Too Long",
                    CreateTaskTest_DescriptionTooLong()
                );

                factoryService.DataService.DeleteAll();
                RunTest("Test Moving Task To Next Stage", MoveTaskToNextStageTest_Valid());

                factoryService.DataService.DeleteAll();
                RunTest(
                    "Test Moving Task To Next Stage With Invalid TaskID",
                    MoveTaskToNextStageTest_InvalidTaskID()
                );

                factoryService.DataService.DeleteAll();
                RunTest("Test Editing Task", EditTaskTest_Valid());

                factoryService.DataService.DeleteAll();
                RunTest("Test Editing Task With Title Too Long", EditTaskTest_TitleTooLong());

                factoryService.DataService.DeleteAll();
                RunTest(
                    "Test Editing Task With Description Too Long",
                    EditTaskTest_DescriptionTooLong()
                );

                // View all tasks tests do not require board reset
                RunTest("Test View All Task Type", TestViewAllTaskType());
                RunTest(
                    "Test View All Task with Bad Task Type",
                    TestViewAllTaskwithInvaidTaskType()
                );

                factoryService.DataService.DeleteAll();
                RunTest("Test Creating Task With Null Email", CreateTaskTest_NullEmail());

                factoryService.DataService.DeleteAll();
                RunTest("Test Creating Task With Null Board", CreateTaskTest_NullBoard());

                factoryService.DataService.DeleteAll();
                RunTest(
                    "Test Creating Task With Whitespace Title",
                    CreateTaskTest_WhitespaceTitle()
                );

                factoryService.DataService.DeleteAll();
                RunTest(
                    "Test Creating Task With Whitespace Description",
                    CreateTaskTest_WhitespaceDescription()
                );

                factoryService.DataService.DeleteAll();
                RunTest(
                    "Test Moving Task To Next Stage Without Creation",
                    MoveTaskToNextStageTest_WithoutCreation()
                );

                factoryService.DataService.DeleteAll();
                RunTest(
                    "Test Moving Task To Next Stage With Null Board",
                    MoveTaskToNextStageTest_NullBoard()
                );

                factoryService.DataService.DeleteAll();
                RunTest("Test Editing Task With Empty Title", EditTaskTest_EmptyTitle());

                factoryService.DataService.DeleteAll();
                RunTest(
                    "Test Editing Task With Empty Description",
                    EditTaskTest_EmptyDescription()
                );

                factoryService.DataService.DeleteAll();
                RunTest("Test Assign Task With Invalid Task ID", AssignTaskTest_InvalidTaskId());

                factoryService.DataService.DeleteAll();
                RunTest("Test Assign Task With Null Board", AssignTaskTest_NullBoard());

                RunTest("Test View All Tasks With Invalid Email", ViewAllTasks_InvalidEmail());
                RunTest("Test View All Tasks With Null Type", ViewAllTasks_NullType());
            }
            finally
            {
                factoryService.DataService.DeleteAll();
            }
            Console.WriteLine("All Task Service Tests Completed.");
        }

        public bool CreateTaskTest_Valid()
        {
            
            factoryService.UserService.Register(TestEmail, TestPassword);
            factoryService.BoardService.CreateBoard(TestEmail, TestBoard);
            Response<TaskSL> res = factoryService.TaskService.CreateTask(
                TestEmail,
                TestBoard,
                new DateTime(),
                "ValidTitle",
                "ValidDescription"
            );

            return res.ErrorMessage == null && res.ReturnValue is TaskSL;
        }

        public bool CreateTaskTest_EmptyTitle()
        {
            factoryService.UserService.Register(TestEmail, TestPassword);
            factoryService.BoardService.CreateBoard(TestEmail, TestBoard);

            Response<TaskSL> res = factoryService.TaskService.CreateTask(
                TestEmail,
                TestBoard,
                new DateTime(),
                "",
                "ValidDescription"
            );
            return res.ErrorMessage != null;
        }

        public bool CreateTaskTest_TitleTooLong()
        {
            factoryService.UserService.Register(TestEmail, TestPassword);
            factoryService.BoardService.CreateBoard(TestEmail, TestBoard);

            Response<TaskSL> res = factoryService.TaskService.CreateTask(
                TestEmail,
                TestBoard,
                new DateTime(),
                "InvalidTitle IIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIII",
                "ValidDescription"
            );
            return res.ErrorMessage != null;
        }

        public bool CreateTaskTest_DescriptionEmpty()
        {
            factoryService.UserService.Register(TestEmail, TestPassword);
            factoryService.BoardService.CreateBoard(TestEmail, TestBoard);
            Response<TaskSL> res = factoryService.TaskService.CreateTask(
                TestEmail,
                TestBoard,
                new DateTime(),
                "ValidTitle",
                ""
            );
            return res.ErrorMessage != null;
        }

        public bool CreateTaskTest_DescriptionTooLong()
        {
            factoryService.UserService.Register(TestEmail, TestPassword);
            factoryService.BoardService.CreateBoard(TestEmail, TestBoard);
            Response<TaskSL> res = factoryService.TaskService.CreateTask(
                TestEmail,
                TestBoard,
                new DateTime(),
                "ValidTitle",
                "InvalidDescription IIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIII"
                    + "IIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIII"
                    + "IIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIII"
                    + "IIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIII"
            );
            return res.ErrorMessage != null;
        }

        public bool MoveTaskToNextStageTest_Valid()
        {
            int taskID = 0;
            factoryService.UserService.Register(TestEmail, TestPassword);
            factoryService.BoardService.CreateBoard(TestEmail, TestBoard);

            Response<TaskSL> createRes = factoryService.TaskService.CreateTask(
                TestEmail,
                TestBoard,
                new DateTime(),
                "ValidTitle",
                "ValidDescription"
            );

            if (createRes.ReturnValue != null)
                taskID = createRes.ReturnValue.Id;

            factoryService.TaskService.AssignTask(TestEmail, TestBoard, taskID, TestEmail);
            Response<TaskSL> res = factoryService.TaskService.MoveTaskToNextStage(
                TestEmail,
                TestBoard,
                taskID
            );

            if (res.ErrorMessage != null)
                return false;
            if (res.ReturnValue == null || res.ReturnValue.GetType() != typeof(TaskSL))
                return false;
            return true;
        }

        public bool MoveTaskToNextStageTest_InvalidTaskID()
        {
            factoryService.UserService.Register(TestEmail, TestPassword);
            factoryService.BoardService.CreateBoard(TestEmail, TestBoard);
            Response<TaskSL> res = factoryService.TaskService.MoveTaskToNextStage(
                TestEmail,
                TestBoard,
                -1
            );
            return res.ErrorMessage != null;
        }

        public bool EditTaskTest_Valid()
        {
            int taskID = 0;
            factoryService.UserService.Register(TestEmail, TestPassword);
            factoryService.BoardService.CreateBoard(TestEmail, TestBoard);

            Response<TaskSL> createRes = factoryService.TaskService.CreateTask(
                TestEmail,
                TestBoard,
                new DateTime(),
                "ValidTitle",
                "ValidDescription"
            );
            if (createRes.ReturnValue != null)
                taskID = createRes.ReturnValue.Id;

            factoryService.TaskService.AssignTask(TestEmail, TestBoard, taskID, TestEmail);

            Response<TaskSL> editRes = factoryService.TaskService.EditTask(
                TestEmail,
                TestBoard,
                taskID,
                new DateTime(),
                "ValidTitle",
                "ValidDescription",
                TestEmail
            );
            return editRes.ErrorMessage == null && editRes.ReturnValue is TaskSL;
        }

        public bool EditTaskTest_UnassignedUser()
        {
            int taskID = 0;
            Response<TaskSL> taskRes = factoryService.TaskService.CreateTask(
                TestEmail,
                TestBoard,
                new DateTime(),
                "ValidTitle",
                "ValidDescription"
            );
            if (taskRes.ReturnValue != null)
                taskID = taskRes.ReturnValue.Id;

            Response<TaskSL> res = factoryService.TaskService.EditTask(
                TestEmail,
                TestBoard,
                taskID,
                new DateTime(),
                "ValidTitle",
                "ValidDescription",
                SecondTestEmail
            );

            return res.ErrorMessage != null;
        }

        /// <summary>
        /// Tests editing a task with a title that exceeds allowed length.
        /// Expects an error from the service due to invalid title input.
        /// </summary>
        /// <returns>True if an error is returned, false otherwise.</returns>
        public bool EditTaskTest_TitleTooLong()
        {
            int taskId = 0;
            factoryService.UserService.Register(TestEmail, TestPassword);
            factoryService.BoardService.CreateBoard(TestEmail, TestBoard);
            Response<TaskSL> createRes = factoryService.TaskService.CreateTask(
                TestEmail,
                TestBoard,
                new DateTime(),
                "ValidTitle",
                "ValidDescription"
            );
            if (createRes.ReturnValue != null)
                taskId = createRes.ReturnValue.Id;
            Response<TaskSL> editRes = factoryService.TaskService.EditTask(
                TestEmail,
                TestBoard,
                taskId,
                new DateTime(),
                new string('I', 52),
                "ValidDescription",
                TestEmail
            );
            return editRes.ErrorMessage != null;
        }

        public bool EditTaskTest_DescriptionTooLong()
        {
            int taskId = 0;
            factoryService.UserService.Register(TestEmail, TestPassword);
            factoryService.BoardService.CreateBoard(TestEmail, TestBoard);
            Response<TaskSL> createRes = factoryService.TaskService.CreateTask(
                TestEmail,
                TestBoard,
                new DateTime(),
                "ValidTitle",
                "ValidDescription"
            );
            if (createRes.ReturnValue != null)
                taskId = createRes.ReturnValue.Id;
            Response<TaskSL> editRes = factoryService.TaskService.EditTask(
                TestEmail,
                TestBoard,
                taskId,
                new DateTime(),
                "ValidTitle",
                new string('I', 314), //pi
                TestEmail
            );
            return editRes.ErrorMessage != null;
        }

        public bool TestViewAllTaskType()
        {
            factoryService.UserService.Register(TestEmail, TestPassword);
            factoryService.BoardService.CreateBoard(TestEmail, TestBoard);
            Response<List<TaskSL>> taskResponse = factoryService.TaskService.ViewAllTaskType(
                TestEmail,
                "in progress"
            );

            return taskResponse.ErrorMessage == null
                && taskResponse.ReturnValue != null
                && taskResponse.ReturnValue.GetType() == typeof(List<TaskSL>)
                && taskResponse.ReturnValue.Count >= 0;
        }

        public bool TestViewAllTaskwithInvaidTaskType()
        {
            factoryService.UserService.Register(TestEmail, TestPassword);
            factoryService.BoardService.CreateBoard(TestEmail, TestBoard);
            Response<List<TaskSL>> taskRes = factoryService.TaskService.ViewAllTaskType(
                TestEmail,
                "InvaidTaskType"
            );
            return taskRes.ErrorMessage != null
                && (taskRes.ReturnValue == null || !(taskRes.ReturnValue is List<TaskSL>));
        }

        public bool TestAssignTask_Valid()
        {
            factoryService.UserService.Register(TestEmail, TestPassword);
            factoryService.BoardService.CreateBoard(TestEmail, TestBoard);
            Response<TaskSL> createRes = factoryService.TaskService.CreateTask(
                TestEmail,
                TestBoard,
                new DateTime(),
                "ValidTitle",
                "ValidDescription"
            );
            factoryService.UserService.Register(TestEmail, TestPassword);
            factoryService.BoardService.CreateBoard(TestEmail, TestBoard);
            Response<object> assignRes = factoryService.TaskService.AssignTask(
                TestEmail,
                TestBoard,
                createRes.ReturnValue.Id,
                SecondTestEmail
            );
            return assignRes.ErrorMessage == null && assignRes.ReturnValue == null;
        }

        public bool TestAssignTask_UnregisterEmail()
        {
            factoryService.UserService.Register(TestEmail, TestPassword);
            factoryService.BoardService.CreateBoard(TestEmail, TestBoard);
            Response<TaskSL> createRes = factoryService.TaskService.CreateTask(
                TestEmail,
                TestBoard,
                new DateTime(),
                "ValidTitle",
                "ValidDescription"
            );
            Response<object> assignRes = factoryService.TaskService.AssignTask(
                TestEmail,
                TestBoard,
                createRes.ReturnValue.Id,
                UnregisterTestEmail
            );
            return assignRes.ErrorMessage != null;
        }

        public bool TestAssignTask_UnasignedTask()
        {
            factoryService.UserService.Register(TestEmail, TestPassword);
            factoryService.BoardService.CreateBoard(TestEmail, TestBoard);
            Response<TaskSL> guidRes = factoryService.TaskService.CreateTask(
                TestEmail,
                TestBoard,
                new DateTime(),
                "ValidTitle",
                "ValidDescription"
            );
            Response<object> res = factoryService.TaskService.AssignTask(
                SecondTestEmail,
                TestBoard,
                guidRes.ReturnValue.Id,
                SecondTestEmail
            );

            return res.ErrorMessage == null && res.ReturnValue == null;
        }

        public bool TestAssignTask_UnasignedUser()
        {
            factoryService.UserService.Register(TestEmail, TestPassword);
            factoryService.BoardService.CreateBoard(TestEmail, TestBoard);
            Response<TaskSL> taskRes = factoryService.TaskService.CreateTask(
                TestEmail,
                TestBoard,
                new DateTime(),
                "ValidTitle",
                "ValidDescription"
            );
            Response<object> res = factoryService.TaskService.AssignTask(
                SecondTestEmail,
                TestBoard,
                taskRes.ReturnValue.Id,
                SecondTestEmail
            );

            return res.ErrorMessage != null;
        }

        public bool CreateTaskTest_NullEmail()
        {
            factoryService.UserService.Register(TestEmail, TestPassword);
            factoryService.BoardService.CreateBoard(TestEmail, TestBoard);
            Response<TaskSL> res = factoryService.TaskService.CreateTask(
                null,
                TestBoard,
                new DateTime(),
                "Title",
                "Description"
            );
            return res.ErrorMessage != null;
        }

        public bool MoveTaskToNextStageTest_NullEmail()
        {
            factoryService.UserService.Register(TestEmail, TestPassword);
            factoryService.BoardService.CreateBoard(TestEmail, TestBoard);
            Response<TaskSL> res = factoryService.TaskService.MoveTaskToNextStage(
                null,
                TestBoard,
                0
            );
            return res.ErrorMessage != null;
        }

        public bool EditTaskTest_NullAssignee()
        {
            factoryService.UserService.Register(TestEmail, TestPassword);
            factoryService.BoardService.CreateBoard(TestEmail, TestBoard);
            int taskID = factoryService
                .TaskService.CreateTask(
                    TestEmail,
                    TestBoard,
                    new DateTime(),
                    "ValidTitle",
                    "ValidDescription"
                )
                .ReturnValue.Id;

            Response<TaskSL> res = factoryService.TaskService.EditTask(
                TestEmail,
                TestBoard,
                taskID,
                new DateTime(),
                "EditedTitle",
                "EditedDescription",
                null
            );
            return res.ErrorMessage != null;
        }

        public bool AssignTaskTest_NullTargetEmail()
        {
            factoryService.UserService.Register(TestEmail, TestPassword);
            factoryService.BoardService.CreateBoard(TestEmail, TestBoard);
            int taskID = factoryService
                .TaskService.CreateTask(
                    TestEmail,
                    TestBoard,
                    new DateTime(),
                    "Title",
                    "Description"
                )
                .ReturnValue.Id;

            Response<object> res = factoryService.TaskService.AssignTask(
                TestEmail,
                TestBoard,
                taskID,
                null
            );
            return res.ErrorMessage != null;
        }

        public bool AssignTaskTest_InvalidBoardName()
        {
            factoryService.UserService.Register(TestEmail, TestPassword);
            factoryService.BoardService.CreateBoard(TestEmail, TestBoard);
            int taskID = factoryService
                .TaskService.CreateTask(
                    TestEmail,
                    TestBoard,
                    new DateTime(),
                    "Title",
                    "Description"
                )
                .ReturnValue.Id;
            Response<object> res = factoryService.TaskService.AssignTask(
                TestEmail,
                "InvalidBoardName",
                taskID,
                SecondTestEmail
            );
            return res.ErrorMessage != null;
        }

        public bool CreateTaskTest_NullBoard()
        {
            factoryService.UserService.Register(TestEmail, TestPassword);
            factoryService.BoardService.CreateBoard(TestEmail, TestBoard);
            Response<TaskSL> res = factoryService.TaskService.CreateTask(
                TestEmail,
                null,
                new DateTime(),
                "Title",
                "Description"
            );
            return res.ErrorMessage != null;
        }

        public bool CreateTaskTest_WhitespaceTitle()
        {
            factoryService.UserService.Register(TestEmail, TestPassword);
            factoryService.BoardService.CreateBoard(TestEmail, TestBoard);
            Response<TaskSL> res = factoryService.TaskService.CreateTask(
                TestEmail,
                TestBoard,
                new DateTime(),
                "   ",
                "Description"
            );
            return res.ErrorMessage != null;
        }

        public bool CreateTaskTest_WhitespaceDescription()
        {
            factoryService.UserService.Register(TestEmail, TestPassword);
            factoryService.BoardService.CreateBoard(TestEmail, TestBoard);
            Response<TaskSL> res = factoryService.TaskService.CreateTask(
                TestEmail,
                TestBoard,
                new DateTime(),
                "Title",
                "   "
            );
            return res.ErrorMessage != null;
        }

        public bool MoveTaskToNextStageTest_WithoutCreation()
        {
            factoryService.UserService.Register(TestEmail, TestPassword);
            factoryService.BoardService.CreateBoard(TestEmail, TestBoard);
            Response<TaskSL> res = factoryService.TaskService.MoveTaskToNextStage(
                TestEmail,
                TestBoard,
                99999
            );
            return res.ErrorMessage != null;
        }

        public bool MoveTaskToNextStageTest_NullBoard()
        {
            factoryService.UserService.Register(TestEmail, TestPassword);
            factoryService.BoardService.CreateBoard(TestEmail, TestBoard);
            Response<TaskSL> res = factoryService.TaskService.MoveTaskToNextStage(
                TestEmail,
                null,
                0
            );
            return res.ErrorMessage != null;
        }

        public bool EditTaskTest_EmptyTitle()
        {
            factoryService.UserService.Register(TestEmail, TestPassword);
            factoryService.BoardService.CreateBoard(TestEmail, TestBoard);
            int taskID = factoryService.TaskService.CreateTask(
                    TestEmail,
                    TestBoard,
                    new DateTime(),
                    "Title",
                    "Description"
                ).ReturnValue.Id;

            factoryService.TaskService.AssignTask(TestEmail, TestBoard, taskID, TestEmail);

            Response<TaskSL> res = factoryService.TaskService.EditTask(
                TestEmail,
                TestBoard,
                taskID,
                new DateTime(),
                "",
                "Description",
                TestEmail
            );
            return res.ErrorMessage != null;
        }

        public bool EditTaskTest_EmptyDescription()
        {
            factoryService.UserService.Register(TestEmail, TestPassword);
            factoryService.BoardService.CreateBoard(TestEmail, TestBoard);
            int taskID = factoryService
                .TaskService.CreateTask(
                    TestEmail,
                    TestBoard,
                    new DateTime(),
                    "Title",
                    "Description"
                )
                .ReturnValue.Id;

            factoryService.TaskService.AssignTask(TestEmail, TestBoard, taskID, TestEmail);

            Response<TaskSL> res = factoryService.TaskService.EditTask(
                TestEmail,
                TestBoard,
                taskID,
                new DateTime(),
                "Title",
                "",
                TestEmail
            );
            return res.ErrorMessage != null;
        }

        public bool AssignTaskTest_InvalidTaskId()
        {
            factoryService.UserService.Register(TestEmail, TestPassword);
            factoryService.BoardService.CreateBoard(TestEmail, TestBoard);
            Response<object> res = factoryService.TaskService.AssignTask(
                TestEmail,
                TestBoard,
                -99,
                SecondTestEmail
            );
            return res.ErrorMessage != null;
        }

        public bool AssignTaskTest_NullBoard()
        {
            factoryService.UserService.Register(TestEmail, TestPassword);
            factoryService.BoardService.CreateBoard(TestEmail, TestBoard);
            int taskID = factoryService
                .TaskService.CreateTask(
                    TestEmail,
                    TestBoard,
                    new DateTime(),
                    "Title",
                    "Description"
                )
                .ReturnValue.Id;

            Response<object> res = factoryService.TaskService.AssignTask(
                TestEmail,
                null,
                taskID,
                SecondTestEmail
            );
            return res.ErrorMessage != null;
        }

        public bool ViewAllTasks_InvalidEmail()
        {
            factoryService.UserService.Register(TestEmail, TestPassword);
            factoryService.BoardService.CreateBoard(TestEmail, TestBoard);
            Response<List<TaskSL>> res = factoryService.TaskService.ViewAllTaskType(
                "invalid@email.com",
                "backlog"
            );
            return res.ErrorMessage != null;
        }

        public bool ViewAllTasks_NullType()
        {
            factoryService.UserService.Register(TestEmail, TestPassword);
            factoryService.BoardService.CreateBoard(TestEmail, TestBoard);
            Response<List<TaskSL>> res = factoryService.TaskService.ViewAllTaskType(
                TestEmail,
                null
            );
            return res.ErrorMessage != null;
        }
    }
}
