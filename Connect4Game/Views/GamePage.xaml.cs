using Connect4Game.ViewModels;

namespace Connect4Game.Views;

public partial class GamePage : ContentPage
{
    public GamePage(GameViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}