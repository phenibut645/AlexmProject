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
	public TicTacToePage(AsyncPageCreate onPageCreated, int size)
	{
		Content = MainContainer;
		MainContainer.Children.Add(ServerState);
		MainContainer.Children.Add(GameArea);
		MainContainer.Children.Add(CurrentSide);
		MainContainer.Children.Add(PlayerSide);
		MainContainer.Children.Add(CancelGameButton);
        ThemePicker = GameService.GetThemePicker();
		MainContainer.Children.Add(ThemePicker);
		DefaultCellsInRow += size;
		DefaultCellsInColumn += size;
		DefaultCellsToWin += size;
		InitGameArea();
		AddEventListeners();
		GameStateService.TicTacToeTheme.CallEveryEvent();

		

		Task.Run(async () => {
			await onPageCreated();
		});
	}
}