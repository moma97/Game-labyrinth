using Labyrinth.ViewModels;
using Labyrinth.Views;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using Labyrinth.Enums1;

namespace Labyrinth.Converters1
{
    internal class UserControlToViewConverter : IValueConverter
    {
        // Converts the UserControl enum to a ViewModel.
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is UserControl1 && value != null)
            {
                object userControl = (UserControl1)value;
                return userControl switch
                {
                    UserControl1.Start => new StartScreenViewModel(),
                    UserControl1.Selection => new SelectionScreenViewModel(),
                    UserControl1.Difficulty => new DifficultySelectionViewModel(),
                    UserControl1.Game => new GameScreenViewModel(),
                    UserControl1.GameOver => new GameOverViewModel(),
                    UserControl1.End => new EndCreditsViewModel(),
                };
            }
            return new StartScreenViewModel();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
