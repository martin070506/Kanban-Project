using Frontend;
using Frontend.Controllers;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

internal class AddBoardWindowVM : Notifiable
{
    private ControllerFactory controllerFactory = ControllerFactory.Instance;
    private string errorMessage;
    private string email;
    private string boardName;

    public AddBoardWindowVM(string email)
    {
        this.email = email;
        this.boardName = string.Empty;
        this.errorMessage = string.Empty;
    }

    public string BoardName
    {
        get { return boardName; }
        set
        {
            boardName = value;
        }
    }

    public string ErrorMessage
    {
        get { return errorMessage; }
        set
        {
            errorMessage = value;
            RaisePropertyChanged("ErrorMessage");
        }
    }

    public bool CreateBoard(string email, string boardName)
    {
        try
        { 
            controllerFactory.boardController.CreateBoard(email, boardName);
            return true;
        }
        catch (Exception e)
        {
            ErrorMessage = e.Message;
            return false;
        }
    }
}


