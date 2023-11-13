using Labyrinth.Commands;
using Labyrinth.Models;
using Labyrinth.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Labyrinth.ViewModels
{
    internal class EndCreditsViewModel : BaseViewModel
    {
        /// <summary>
        /// Text that the player sees on screen.
        /// </summary>
        public string ThankYouMessage { get; private set; } = "Grattis, du tog dig ut ur labyrinten! Tack för att du spelade!";
        public string GameStudioName { get; private set; } = "Spelet är skapat av Studio G6";

    }
}
