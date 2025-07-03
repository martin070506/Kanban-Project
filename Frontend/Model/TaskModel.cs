using IntroSE.Kanban.Backend.ServiceLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frontend.Model
{
    internal class TaskModel
    {
        public string Assignee { get; set; }
        public string Title { get; }
        public string DueDate { get; }
        public string State { get; }

        public TaskModel(TaskSL task)
        {
            Assignee = "Assignee: "+task.Assignee;
            Title = "Title: " + task.Title;
            DueDate ="Due By:" + task.DueDate.ToString();
            State = task.State;
        }


    }
}
