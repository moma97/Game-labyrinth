using Labyrinth.Commands;
using Labyrinth.Converters1;
using Labyrinth.ViewModels.Base;
using Labyrinth.Views.GameScreenComponents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace Labyrinth.ViewModels
{

    
    internal sealed class MainViewModel : BaseViewModel
    {
        private static MainViewModel? _instance;
        public static MainViewModel Instance { get => _instance ?? (_instance = new MainViewModel()); }
        public BaseViewModel CurrentViewModel { get; set; } = new StartScreenViewModel();
        public ICommand ChangeUserControlCommand { get; set; }
        // If player chooses the button to exit the game.
        public ICommand QuitGameCommand { get; set; }
        UserControlToViewConverter Converter = new UserControlToViewConverter();

        public MainViewModel()
        {
            QuitGameCommand = new RelayCommand(x => QuitGame());
            ChangeUserControlCommand = new RelayCommand(x => ChangeViewModel((BaseViewModel)Converter.Convert(x, null, null, null)));
        }
        // Changes the current View.
        private void ChangeViewModel(BaseViewModel viewModel)
        {
            CurrentViewModel = viewModel;
        }
        //Shuts the whole game down
        private void QuitGame()
        {
            App.Current.Shutdown();
        }
    }

}
