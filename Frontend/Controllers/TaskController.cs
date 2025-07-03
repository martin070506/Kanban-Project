using Frontend.Model;
using IntroSE.Kanban.Backend.ServiceLayer;
using IntroSE.Kanban.Backend.ServiceLayer.models;
using IntroSE.Kanban.Backend.ServiceLayer.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frontend.Controllers
{
    internal class TaskController
    {
        private TaskService taskService;

        public TaskController(TaskService taskService)
        {
            this.taskService = taskService;
        }

        public List<TaskModel> getTasksByStatus(string email, string boardName, string taskType)
        {
            List<TaskSL> helperList=taskService.ViewAllTaskTypeInBoard(email, boardName, taskType).ReturnValue;
            List<TaskModel> TaskModels = new List<TaskModel>();
            if (helperList != null) 
            {
                foreach (TaskSL task in helperList)
                    TaskModels.Add(new TaskModel(task));
            }
            
            return TaskModels;
        }

        public void createTask(string email,
            string boardName,
            DateTime dueDate,
            string title,
            string description)
        {

            var response = taskService.CreateTask(email,boardName, dueDate, title, description);
            if (response.ErrorMessage != null)
            {
                Debug.WriteLine(response.ErrorMessage);
                throw new Exception(response.ErrorMessage);
                
            }
        }
    }
}
