using Labyrinth.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace Labyrinth
{
    /// <summary>
    /// Holds all game levels and invokes game map drawing methods
    /// </summary>
    public static class GameEngine
    {
		private static GameLevel? _easyLevel;

		private static GameLevel? EasyLevel
		{
			get { return _easyLevel; }
		}

        private static GameLevel? _mediumLevel;

        private static GameLevel? MediumLevel
        {
            get { return _mediumLevel; }
        }

        private static GameLevel? _hardLevel;

        private static GameLevel? HardLevel
        {
            get { return _hardLevel; }
        }

        private static GameLevel? _currentGameLevel;

        public static GameLevel? CurrentGameLevel
        {
            get { return _currentGameLevel; }
        }

        private static WriteableBitmap? _currentMapImage;

        public static WriteableBitmap? CurrentMapImage
        {
            get { return _currentMapImage; }
        }

        private static Coordinate _playerLocation;

        public static Coordinate PlayerLocation
        {
            get { return _playerLocation; }
        }

        private static Coordinate _playerPreviousLocation;

        private static Coordinate PlayerPreviousLocation
        {
            get { return _playerPreviousLocation; }
        }

        /// <summary>
        /// Build and assign game levels to variables
        /// </summary>

        public static void BuildGameLevels()
		{
            _easyLevel = new GameLevel(DifficultyLevel.Easy);

            _mediumLevel = new GameLevel(DifficultyLevel.Medium);

            _hardLevel = new GameLevel(DifficultyLevel.Hard);
		}

        /// <summary>
        /// Assigns current game level game map as current map and applies fog
        /// </summary>

        private static void SetGameMapFog(GameLevel gameLevel)
        {
            _currentMapImage = new WriteableBitmap(gameLevel.MapImage);
            foreach (KeyValuePair<Coordinate, Pixel> mapLocation in gameLevel.GameMapMeta)
            {
                _currentMapImage.WritePixels(new Int32Rect(mapLocation.Key.X, mapLocation.Key.Y, 1, 1), new int[] { mapLocation.Value.FoggedValue }, 4, 0);
            }
        }

        /// <summary>
        /// Switches game level by difficulty. Sets and resets class variables holding current game level.
        /// </summary>

        public static void ChangeGameLevel(DifficultyLevel difficulty)
        {
            switch (difficulty)
            {
                case DifficultyLevel.Easy:
                    _currentGameLevel = EasyLevel;
                    break;
                case DifficultyLevel.Medium:
                    _currentGameLevel = MediumLevel;
                    break;
                case DifficultyLevel.Hard:
                    _currentGameLevel = HardLevel;
                    break;
                default:
                    break;
            }

            if (_currentGameLevel != null)
            {
                _playerPreviousLocation = new Coordinate(-1, -1);
                _playerLocation = _currentGameLevel.PlayerStartLocation;
                _currentMapImage = _currentGameLevel.MapImage != null ? new WriteableBitmap(_currentGameLevel.MapImage.Clone()) : null;
                _playerLocation = _currentGameLevel.PlayerStartLocation;
                SetGameMapFog(_currentGameLevel);
                MovePlayer(PlayerLocation);
            }
        }

        /// <summary>
        /// Draws visible circle in game and player representation on game map. When applicable fog is persistent i.e
        /// visible area gets re-fogged when out of scope of visible circle.
        /// Visibility circle and fog is drawn relative to new player location supplied by input parameter.
        /// </summary>

        public static void MovePlayer(Coordinate newPlayerLocation)
        {
            if (_currentGameLevel != null && _currentMapImage != null)
            {
                if (_playerPreviousLocation.X == -1)
                {
                    _playerPreviousLocation = newPlayerLocation;
                }

                object tLockFog = new object();
                object tLockVisible = new object();
                int mapWidth = _currentGameLevel.MapImage != null ? _currentGameLevel.MapImage.PixelWidth : 0;
                int mapHeight = _currentGameLevel.MapImage != null ? _currentGameLevel.MapImage.PixelHeight : 0;

                int playerWidth = _currentGameLevel.PlayerImage != null ? _currentGameLevel.PlayerImage.PixelWidth / 2 : 0;
                int playerHeight = _currentGameLevel.PlayerImage != null ? _currentGameLevel.PlayerImage.PixelHeight / 2 : 0;

                List<Pixel> pixelsToFog = new List<Pixel>();
                List<Pixel> pixelsToMakeVisible = new List<Pixel>();


                Parallel.ForEach(_currentGameLevel.VisibilityCircleCoordinates, coordinate =>
                {
                    bool withinScope;

                    if (_currentGameLevel.PersistentFog)
                    {
                        withinScope = _currentGameLevel.VisibilityCircleSetFoggedCoordinates.ContainsKey(coordinate);
                    }
                    else
                    {
                        withinScope = false;
                    }
                    

                    if (withinScope)
                    {
                        Coordinate previousRelativeLocation = new Coordinate(coordinate.X + _playerPreviousLocation.X + playerWidth,
                                                                            coordinate.Y + _playerPreviousLocation.Y + playerHeight);

                        if (previousRelativeLocation.X > -1 && previousRelativeLocation.Y > -1
                        && previousRelativeLocation.X < mapWidth && previousRelativeLocation.Y < mapHeight)
                        {
                            Pixel pixel = _currentGameLevel.GameMapMeta[previousRelativeLocation];
                            lock (tLockFog)
                            {
                                pixelsToFog.Add(pixel);
                            }
                        }
                    }

                    Coordinate relativeLocation = new Coordinate(coordinate.X + newPlayerLocation.X + playerWidth,
                                                                    coordinate.Y + newPlayerLocation.Y + playerHeight);

                    if (relativeLocation.X > -1 && relativeLocation.Y > -1
                    && relativeLocation.X < mapWidth && relativeLocation.Y < mapHeight)
                    {
                        Pixel pixel = _currentGameLevel.GameMapMeta[relativeLocation];
                        lock (tLockVisible)
                        {
                            pixelsToMakeVisible.Add(pixel);
                        }
                    }

                });

                foreach (Pixel pixel in pixelsToFog)
                {
                    _currentMapImage.WritePixels(new Int32Rect(pixel.Coordinate.X, pixel.Coordinate.Y, 1, 1), new int[] { pixel.FoggedValue }, 4, 0);
                }

                foreach (Pixel pixel in pixelsToMakeVisible)
                {
                    _currentMapImage.WritePixels(new Int32Rect(pixel.Coordinate.X, pixel.Coordinate.Y, 1, 1), new int[] { pixel.OriginalValue }, 4, 0);
                }

                _playerPreviousLocation = newPlayerLocation;
                _playerLocation = newPlayerLocation;

                if (_currentGameLevel.PlayerImage != null)
                {
                    DrawImageOnCurrentMapAtCoordinate(PlayerLocation, _currentGameLevel.PlayerImage);
                }
            }
        }

        /// <summary>
        /// Changes pixel value for one specific pixel at a specific coordinate
        /// </summary>

        public static void ChangeCurrentMapImagePixelAtCoordinate(Coordinate coordinate, int newPixelValue)
        {
            if (_currentGameLevel != null && _currentMapImage != null) 
            {
                if (_currentGameLevel.GameMapMeta.ContainsKey(coordinate))
                {
                    _currentMapImage.WritePixels(new Int32Rect(coordinate.X, coordinate.Y, 1, 1), new int[] { newPixelValue }, 4, 0);
                }
            }
        }

        /// <summary>
        /// Resets one specific pixel to its original value
        /// </summary>

        public static void ResetCurrentMapImagePixelAtCoordinate(Coordinate coordinate)
        {
            if (_currentGameLevel != null && _currentMapImage != null)
            {
                if (_currentGameLevel.GameMapMeta.ContainsKey(coordinate))
                {
                    int[] pixelValue = new int[] { _currentGameLevel.GameMapMeta[coordinate].OriginalValue };
                    _currentMapImage.WritePixels(new Int32Rect(coordinate.X, coordinate.Y, 1, 1), pixelValue, 4, 0);
                }
            }
        }

        /// <summary>
        /// Draws an image on the game map at a specific coordinate
        /// </summary>

        public static void DrawImageOnCurrentMapAtCoordinate(Coordinate coordinate, BitmapImage bitmapImage)
        {
            if (_currentMapImage != null)
            {
                int[] pixelArray = new int[bitmapImage.PixelWidth * bitmapImage.PixelHeight];
                int stride = bitmapImage.PixelWidth * 4;

                bitmapImage.CopyPixels(pixelArray, stride, 0);

                int xStart = coordinate.X - bitmapImage.PixelWidth >= 0 ? coordinate.X : bitmapImage.PixelWidth;
                int yStart = coordinate.Y - bitmapImage.PixelHeight >= 0 ? coordinate.Y : bitmapImage.PixelHeight;

                int arrayPosition = 0;

                int nonPlayerPixel = pixelArray[0];

                for (int y = 0; y < bitmapImage.PixelHeight; y++)
                {
                    for (int x = 0; x < bitmapImage.PixelWidth; x++)
                    {
                        if (nonPlayerPixel != pixelArray[arrayPosition])
                        {
                            _currentMapImage.WritePixels(new Int32Rect(xStart + x, yStart + y, 1, 1), new int[] { pixelArray[arrayPosition] }, 4, 0);
                        }
                        arrayPosition++;
                    }
                }
            }
        }

        /// <summary>
        /// Resets pixels to their original value in a specified rectangle area
        /// </summary>

        public static void ResetPixelsByRectangleAtCoordinate(Coordinate coordinate, Int32Rect pixelResetArea)
        {
            if (_currentGameLevel != null && _currentMapImage != null)
            {
                for (int y = 0; y < pixelResetArea.Height; y++)
                {
                    for (int x = 0; x < pixelResetArea.Height; x++)
                    {
                        Coordinate currentCoordinate = new Coordinate(coordinate.X + x, coordinate.Y + y);
                        if (_currentGameLevel.GameMapMeta.ContainsKey(currentCoordinate))
                        {
                            int[] pixelValue = new int[] { _currentGameLevel.GameMapMeta[currentCoordinate].OriginalValue };
                            _currentMapImage.WritePixels(new Int32Rect(currentCoordinate.X, currentCoordinate.Y, 1, 1), pixelValue, 4, 0);
                        }
                    }
                }
            }
        }

    }
}
