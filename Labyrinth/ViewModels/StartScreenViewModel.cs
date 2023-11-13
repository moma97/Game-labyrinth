using Labyrinth.Commands;
using Labyrinth.ViewModels.Base;
using Labyrinth.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Input;

namespace Labyrinth.ViewModels
    {
        internal class StartScreenViewModel : BaseViewModel
        {
           
        private BackgroundWorker backgroundWorker = new BackgroundWorker();
        private BackgroundWorker backgroundWorker1 = new BackgroundWorker();
        private System.Timers.Timer Timer = new System.Timers.Timer(250);
        private int counter = 1;

        public string TxtBlockLoad { get; set; } = "Laddar spelet";
        public string TxtPercent { get; set; }


        /// <summary>
        /// Showing interactive loadingscreen
        /// </summary>
        public StartScreenViewModel()
        {
            
            backgroundWorker.DoWork += BackgroundWorker_DoWork;
            backgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;
            backgroundWorker.RunWorkerAsync();

            Timer.Elapsed += Timer_Elapsed;           
            Timer.Interval = 380;
            Timer.Start();
        }
      
      
        /// <summary>
        /// Building 3 different labyrinths with different property values for each one
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BackgroundWorker_DoWork(object? sender, DoWorkEventArgs e)
        {
            GameEngine.BuildGameLevels();

        }

        /// <summary>
        /// Automatically navigates to selectionscreenviewmodel when done
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BackgroundWorker_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
        {

            MainViewModel.Instance.CurrentViewModel = new SelectionScreenViewModel();
        }


        /// <summary>
        /// Display time passed as percent done
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (counter <= 99)
            {
                TxtPercent = counter.ToString() + "%";
                counter++;
            }
            else
            {
                
                Timer.Stop();
            }




        }

    }



}   




