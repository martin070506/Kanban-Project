using IntroSE.Kanban.Backend.ServiceLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frontend.Controllers
{
    class ControllerFactory
    {
        public static ControllerFactory Instance { get; } = new ControllerFactory();

        public readonly UserController userController;
        public readonly BoardController boardController;
        public readonly TaskController taskController;
        public readonly DataService dataService;

        private ControllerFactory()
        {
            var serviceFactory = new FactoryService();
            userController = new UserController(serviceFactory.UserService);
            boardController = new BoardController(serviceFactory.BoardService, serviceFactory.UserBoardsService);
            taskController = new TaskController(serviceFactory.TaskService);
            dataService = serviceFactory.DataService;
        }
    }
}
