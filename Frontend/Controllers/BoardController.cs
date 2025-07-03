using Frontend.Model;

using IntroSE.Kanban.Backend.ServiceLayer;
using IntroSE.Kanban.Backend.ServiceLayer.Models;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frontend.Controllers
{
    class BoardController
    {
        private BoardService boardService;
        private UserBoardsService userBoardsService;

        public BoardController(BoardService boardService, UserBoardsService userBoardsService)
        {
            this.boardService = boardService;
            this.userBoardsService = userBoardsService;
        }

        public void CreateBoard(string email, string name)
        {
            var response = boardService.CreateBoard(email, name);
            if (response.ErrorMessage != null)
            {
                throw new Exception(response.ErrorMessage);
            }
        }

        public void DeleteBoard(string email, string name)
        {
            var response = boardService.DeleteBoard(email, name);
            if (response.ErrorMessage != null)
            {
                throw new Exception(response.ErrorMessage);
            }
        }

        public ObservableCollection<BoardModel> GetUserBoards(string email)
        {
            var response = userBoardsService.GetUserBoards(email);
            if (response.ErrorMessage != null)
            {
                throw new Exception(response.ErrorMessage);
            }

            var boards = response.ReturnValue.Select(b => GetBoardByID(b));
            return new ObservableCollection<BoardModel>(boards);
        }


        private BoardModel GetBoardByID(int id)
        {
            var response = boardService.GetBoardByID(id);
            if (response.ErrorMessage != null)
            {
                throw new Exception(response.ErrorMessage);
            }
            return new BoardModel((BoardSL)response.ReturnValue);
        }
        public List<string> GetBoardMembers(string email, string name)
        {
            return boardService.GetBoardMembers(email, name).ReturnValue;
        }

    }
}
