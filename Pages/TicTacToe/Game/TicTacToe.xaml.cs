using alexm_app.Enums.TicTacToe;
using alexm_app.Models;
using alexm_app.Utils.TicTacToe;
using System.Diagnostics;

namespace alexm_app;

public partial class TicTacToe : ContentPage
{

	public TicTacToe()
	{
		Content = MainContainer;
		MainContainer.Children.Add(ServerState);
		MainContainer.Children.Add(GameArea);
		MainContainer.Children.Add(CurrentSide);
		MainContainer.Children.Add(PlayerSide);
        AppContext.OnSideChange += AppContext_OnSideChange;
		InitGameArea();
		AddEventListeners();
		AppContext.TicTacToeTheme.CallEveryEvent();
		// Task.Run(async() => await ReportSender.SendMessage("Tic-Tac-Toe game started"));
		
		AppContext.CurrentGame = this;
		GameHandler.GamePageCreated();
		
		Debug.WriteLine("Connecting to web socket...");
		_ = SServerHandler.Connect();
	}


}