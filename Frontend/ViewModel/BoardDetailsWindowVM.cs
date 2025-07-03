using Frontend.Controllers;
using Frontend.Model;
using IntroSE.Kanban.Backend.ServiceLayer.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

internal class BoardDetailsWindowVM
{
    public BoardModel Board { get; }
    public UserModel CurrentUser { get; }
    public string Title { get; }
    public string Owner { get; }
    public List<string> Members { get; }
    public List<TaskModel> BacklogTasks { get; }
    public List<TaskModel> InProgressTasks { get; }
    public List<TaskModel> DoneTasks { get; }

    public BoardDetailsWindowVM(BoardModel board,UserModel CurrentUser)
    {
        Board = board;
        CurrentUser = CurrentUser;
        Title = board.Name;
        Owner = board.Owner;
        Members = board.GetMembers(CurrentUser);
        BacklogTasks = ControllerFactory.Instance.taskController.getTasksByStatus(CurrentUser.Email, Title, "backlog");
        InProgressTasks = ControllerFactory.Instance.taskController.getTasksByStatus(CurrentUser.Email, Title, "in progress");
        DoneTasks  = ControllerFactory.Instance.taskController.getTasksByStatus(CurrentUser.Email, Title, "done");
    }
   





}



