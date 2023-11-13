using Labyrinth.Commands;
using Labyrinth.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Windows.Controls;
using System.Timers;
using Labyrinth.Models;
using System.Media;

namespace Labyrinth.ViewModels
{
    internal class GameScreenViewModel : BaseViewModel
    {
        public ICommand? LeftCommand { get; set; }
        public ICommand? RightCommand { get; set; }
        public ICommand? UpCommand { get; set; }
        public ICommand? DownCommand { get; set; }
        public ICommand? PauseGame { get; set; }
        public ICommand ToggleSoundCommand { get; private set; }
        public ICommand? ChangeToSelectionScreen { get; set; }

        private WriteableBitmap? _currentMap;

        public WriteableBitmap? CurrentMap
        { get { return _currentMap; } }

        private BitmapImage? _marker;
        public BitmapImage? Marker
        {
            get { return _marker; }
            private set { _marker = value; }
        }
        private Coordinate NewPlayerLocation { get; set; }
        private Coordinate MarkerLocation { get; set; } = new Coordinate(500, 25);

        public GameScreenViewModel()
        {
            GameEngine.ChangeGameLevel(CurrentDifficulty.CurrentDifficultyLevel);
            if (GameEngine.CurrentMapImage != null)
            {
                _currentMap = GameEngine.CurrentMapImage;
                Marker = Application.Current.Resources["Marker"] as BitmapImage;
                if (Marker != null)
                {
                    GameEngine.DrawImageOnCurrentMapAtCoordinate(MarkerLocation, Marker);
                }
                LeftCommand = new RelayCommand(x => MoveCharacterLeft());
                RightCommand = new RelayCommand(x => MoveCharacterRight());
                UpCommand = new RelayCommand(x => MoveCharacterUp());
                DownCommand = new RelayCommand(x => MoveCharacterDown());
                PauseGame = new RelayCommand(x => PauseGameScreen());
                ToggleSoundCommand = new RelayCommand(ToggleSound);
            

            ChangeToSelectionScreen = new RelayCommand(x => ChangeToSelectionViewModel());
            };
            timer = new Timer(500);
            timer.Elapsed += TimerElapsed;
            StartTimer();
        }

        // Changes ViewModel to SelectionScreen and stops and disposes of Timer. 
        private void ChangeToSelectionViewModel()
        {
            timer.Stop();
            timer.Dispose();
            MainViewModel.Instance.CurrentViewModel = new SelectionScreenViewModel();
        }
        // bool for knowing if the game is paused.
        bool pausedGame;
        /// <summary>
        /// Sends in a new coordinate to move the player's character on the map. Checks if the player's coordinate is an exitarea.
        /// </summary>
        public async void MoveCharacter()
        {
            await Task.Delay(5);
            GameEngine.MovePlayer(NewPlayerLocation);
            Task.WaitAll();
            bool exitArea = GameEngine.CurrentGameLevel.GameMapMeta[GameEngine.PlayerLocation].ExitArea;
            if (exitArea && IsSoundOn)
            {
                // Play a sound when player reaches exitArea
                var exitSoundPlayer = new SoundPlayer(Properties.Resources._353546__maxmakessounds__success);
                exitSoundPlayer.Play();
                MainViewModel.Instance.CurrentViewModel = new EndCreditsViewModel();
            }
            else if (exitArea)
            {

                MainViewModel.Instance.CurrentViewModel = new EndCreditsViewModel();

            }

        }
        /// <summary>
        /// If the player moves in any direction: Checks if the game isn't paused, then gives new coordinates to the location of the player depending on the current speed that is set. Also checks that
        /// the new coordinates aren't outside of the map and therefore inaccessible. It also checks if the new coordinate is walkable or if a wall is located there. 
        /// </summary>
        public async void MoveCharacterRight()
        {
            if (!pausedGame)
            {
                await Task.Delay(5);
                int xCoordinate = GameEngine.PlayerLocation.X + CurrentDifficulty.WalkSpeed < CurrentMap.PixelWidth ? GameEngine.PlayerLocation.X + CurrentDifficulty.WalkSpeed : CurrentMap.PixelWidth;
                bool walkable = xCoordinate + GameEngine.CurrentGameLevel.PlayerImage.PixelWidth < CurrentMap.PixelWidth
                    ? GameEngine.CurrentGameLevel.GameMapMeta[new Coordinate(xCoordinate + GameEngine.CurrentGameLevel.PlayerImage.PixelWidth, GameEngine.PlayerLocation.Y)].Walkable : false;
                if (walkable)
                {
                    NewPlayerLocation = new Coordinate(xCoordinate, GameEngine.PlayerLocation.Y);
                    MoveCharacter();
                    PlaySound(walkable);
                }
            }
        }
        /// <summary>
        /// See summary for MoveCharacterRight.
        /// </summary>
        public async void MoveCharacterLeft()
        {
            if (!pausedGame)
            {
                await Task.Delay(5);
                int xCoordinate = GameEngine.PlayerLocation.X - CurrentDifficulty.WalkSpeed > -1 ? GameEngine.PlayerLocation.X - CurrentDifficulty.WalkSpeed : 0;
                bool walkable = GameEngine.CurrentGameLevel.GameMapMeta[new Coordinate(xCoordinate, GameEngine.PlayerLocation.Y)].Walkable;

                if (walkable)
                {
                    NewPlayerLocation = new Coordinate(xCoordinate, GameEngine.PlayerLocation.Y);
                    MoveCharacter();
                    PlaySound(walkable);
                }
            }
        }
        /// <summary>
        /// See summary for MoveCharacterRight.
        /// </summary>
        public async void MoveCharacterUp()

        {
            if (!pausedGame)
            {
                await Task.Delay(5);
                int yCoordinate = GameEngine.PlayerLocation.Y - CurrentDifficulty.WalkSpeed > -1 ? GameEngine.PlayerLocation.Y - CurrentDifficulty.WalkSpeed : 0;
                bool walkable = GameEngine.CurrentGameLevel.GameMapMeta[new Coordinate(GameEngine.PlayerLocation.X, yCoordinate)].Walkable;

                if (walkable)
                {
                    NewPlayerLocation = new Coordinate(GameEngine.PlayerLocation.X, yCoordinate);
                    MoveCharacter();
                    PlaySound(walkable);
                }
            }
        }
        /// <summary>
        /// See summary for MoveCharacterRight.
        /// </summary>
        public async void MoveCharacterDown()
        {
            if (!pausedGame)
            {
                await Task.Delay(5);
                int yCoordinate = GameEngine.PlayerLocation.Y + CurrentDifficulty.WalkSpeed < CurrentMap.PixelHeight ? GameEngine.PlayerLocation.Y + CurrentDifficulty.WalkSpeed : CurrentMap.PixelHeight;
                bool walkable = GameEngine.CurrentGameLevel.GameMapMeta[new Coordinate(GameEngine.PlayerLocation.X, yCoordinate + GameEngine.CurrentGameLevel.PlayerImage.PixelHeight)].Walkable;
                if (walkable)
                {
                    NewPlayerLocation = new Coordinate(GameEngine.PlayerLocation.X, yCoordinate);
                    MoveCharacter();
                    PlaySound(walkable);
                }
            }
        }

        private readonly Timer timer;
        private DateTime endTime;
        private DateTime subtractedTime;
        public object Minutes { get; set; } = 0;
        public object Seconds { get; set; } = 0;
        public string? ShowMinutes { get; set; }
        public string? ShowSeconds { get; set; }
        /// <summary>
        /// Occurs when Timer interval is elapsed, subtracts the current time from the Endtime.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TimerElapsed(object? sender, ElapsedEventArgs e)
        {
            subtractedTime = DateTime.Now;
            Minutes = endTime.Subtract(subtractedTime).Minutes;
            Seconds = endTime.Subtract(subtractedTime).Seconds;
            ShowTime();
            if (endTime.CompareTo(subtractedTime) < 0)
            {
                StopTimer();
            };
        }
        /// <summary>
        /// Decides how the time is presented in the view.
        /// </summary>
        private void ShowTime()
        {
            if ((int)Minutes < 10)
            {
                ShowMinutes = $"0{Minutes} :";
            }
            else ShowMinutes = $"{Minutes} :";

            if ((int)Seconds < 10)
            {
                ShowSeconds = $"0{Seconds}";
            }
            else ShowSeconds = $"{Seconds}";
        }
        /// <summary>
        /// Starts the Timer and sets the endtime in seconds depending on the current difficulty. 
        /// </summary>
        private void StartTimer()
        {
            timer.Start();
            endTime = DateTime.Now.AddSeconds(CurrentDifficulty.TimeLimitSeconds);
        }

        /// <summary>
        /// Stops the Timer and changes to GameOver if the difference between endtime and current time is below 0, resulting in the players time running out. 
        /// </summary>
        private void StopTimer()
        {  
            timer.Stop();
            timer.Dispose();
            MainViewModel.Instance.CurrentViewModel = new GameOverViewModel();
        }

        /// <summary>
        ///Depending on if the game is paused (bool true/false) the Timer stops or starts again, also sets a new endtime determined by how many seconds the player has left. 
        /// </summary>
        private void PauseGameScreen()
        {
            pausedGame = !pausedGame;
            int secondsLeft = (int)Minutes * 60 + (int)Seconds;

            if (!pausedGame)
            {
                timer.Start();
                endTime = DateTime.Now.AddSeconds(secondsLeft);
            }
            else timer.Stop();
        }

        // Plays a sound when the player takes a step, with a frequency of every fourth step.
        private int stepCounter = 0;

        private void PlaySound(bool walkable)
        {
            if (IsSoundOn && walkable && stepCounter % 4 == 0)
            {
                var player = new SoundPlayer(Properties.Resources._697182__znukem__single_footstep);
                player.Play();
            }

            stepCounter++; // Increase step counter
        }


        private bool isSoundOn = true;


        public bool IsSoundOn
        {
            get { return isSoundOn; }
            set
            {
                if (isSoundOn != value)
                {
                    isSoundOn = value;
                }


            }

        }

        /// <summary>
        /// Switching the property to the oppposite, if it had the value true it will change to false
        /// </summary>
        /// <param name="parameter"></param>
        private void ToggleSound(object parameter)
        {
            IsSoundOn = !isSoundOn;

        }

    }

}



