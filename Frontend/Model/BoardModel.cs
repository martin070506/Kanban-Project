using Frontend.Controllers;
using IntroSE.Kanban.Backend.BusinessLayer.Board;
using IntroSE.Kanban.Backend.ServiceLayer.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frontend.Model
{
    internal class BoardModel
    {
        public string Name { get; set; }
        public string Owner { get; }
        public List<TaskModel> BacklogTasks { get; }
        public List<TaskModel> InProgressTasks { get; }
        public List<TaskModel> DoneTasks { get; }
        public LinkedList<string> Columns { get; }
        

        public BoardModel(BoardSL board)
        {
            BoardController boardController = ControllerFactory.Instance.boardController;
            TaskController taskController = ControllerFactory.Instance.taskController;
            Name = board.name;
            Owner = board.owner;
            string backlog = "backlog";
            BacklogTasks = taskController.getTasksByStatus(Owner, Name, backlog);
            string in_progress = "in progress";
            InProgressTasks = taskController.getTasksByStatus(Owner, Name, in_progress);
            string done = "done";
            DoneTasks = taskController.getTasksByStatus(Owner, Name, done);
            Columns = board.columns;
        }

        public List<string> GetMembers(UserModel um) 
        {
            return ControllerFactory.Instance.boardController.GetBoardMembers(um.Email, Name);
        }


    }
}
