using Labyrinth.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Labyrinth.ViewModels.Base;
using Labyrinth.Models;


namespace Labyrinth.ViewModels
{
    internal class DifficultySelectionViewModel : BaseViewModel
    {

        public ICommand EasyCommand { get; }
        public ICommand MediumCommand { get; }
        public ICommand HardCommand { get; }

        public DifficultySelectionViewModel()
        {
            EasyCommand = new RelayCommand(SetEasyDifficulty);
            MediumCommand = new RelayCommand(SetMediumDifficulty);
            HardCommand = new RelayCommand(SetHardDifficulty);
        }

        /// <summary>
        /// Sets the difficulty level as Easy and sets corresponding property values for the labyrinth
        /// <param name="parameter"></param>
        private void SetEasyDifficulty(object parameter)
        {

            CurrentDifficulty.SetCurrentDifficulty(DifficultyLevel.Easy);
            MainViewModel.Instance.CurrentViewModel = new GameScreenViewModel();
           
        }

        /// <summary>
        ///Sets the difficulty level as medium and sets corresponding property values for the labyrinth
        /// </summary>
        /// <param name="parameter"></param>
        private void SetMediumDifficulty(object parameter)
        {
            CurrentDifficulty.SetCurrentDifficulty(DifficultyLevel.Medium);
            MainViewModel.Instance.CurrentViewModel = new GameScreenViewModel();
        }

        /// <summary>
        /// Sets the difficulty level as hard and sets corresponding property values for the labyrinth 
        /// </summary>
        /// <param name="parameter"></param>
        private void SetHardDifficulty(object parameter)
        {
            CurrentDifficulty.SetCurrentDifficulty(DifficultyLevel.Hard);
            MainViewModel.Instance.CurrentViewModel = new GameScreenViewModel();
        }


        /// <summary>
        /// Description of different levels
        /// </summary>
        public string EasyLevelText { get; set; } = "Mer tid, större synlighets fält";
        public string NormalLevelText { get; set; } = "Mindre tid, mindre synlighets fält";

        public string HardLevelText { get; set; } = "knappt någon tid, typ inget synlighets fält";

    }
}

 
