using alexm_app.Enums.TicTacToe;
using alexm_app.Handlers.TicTacToe;
using alexm_app.Models;
using alexm_app.Services;
using alexm_app.Utils.TicTacToe;
using System.Diagnostics;

namespace alexm_app;

public partial class TicTacToePage : ContentPage
{
	public delegate Task AsyncPageCreate();
	public TicTacToePage(AsyncPageCreate onPageCreated)
	{
		Content = MainContainer;
		MainContainer.Children.Add(ServerState);
		MainContainer.Children.Add(GameArea);
		MainContainer.Children.Add(CurrentSide);
		MainContainer.Children.Add(PlayerSide);
        
		InitGameArea();
		AddEventListeners();
		GameStateService.TicTacToeTheme.CallEveryEvent();

		Task.Run(async () => {
			await onPageCreated();
		});
	}
}