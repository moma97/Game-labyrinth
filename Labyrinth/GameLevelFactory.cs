using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Net.NetworkInformation;
using System.Windows.Media;
using Labyrinth.Models;
using System.Threading;

namespace Labyrinth
{
    /// <summary>
    /// Maps game map coordinates with corresponding pixels. Sets all pixel properties.
    /// </summary>
    public abstract class GameLevelFactory
    {
        private Dictionary<Coordinate, Pixel> _gameMap = new Dictionary<Coordinate, Pixel>();

        private Dictionary<int, bool> Road = new Dictionary<int, bool>();

        private Dictionary<Coordinate, bool> ExitArea = new Dictionary<Coordinate, bool>();

        private List<Coordinate> _visibilityCircleCoordinates = new List<Coordinate>();

        Dictionary<Coordinate, bool> _visibilityCircleSetFoggedCoordinates = new Dictionary<Coordinate, bool>();

        private BitmapImage? _mapImage;

        public BitmapImage? MapImage
        {
            get { return _mapImage; }
            private set { _mapImage = value; }
        }

        private int[]? arrRoadDefinition;

        protected void GenerateGameLevel(DifficultyLevel difficultyLevel)
        {
            _mapImage = GetGameMapImage();
            SetRoadDefinition(GetRoadDefinition());
            MapExitArea(GetExitArea(difficultyLevel));
            MapGameMap();
            GenerateVisibilityCircle(GetVisibilityRadius(difficultyLevel));
        }

        protected List<Coordinate> GetVisibilityCoordinates()
        {
            return _visibilityCircleCoordinates;
        }

        protected Dictionary<Coordinate, bool> GetVisibilitySetFoggedCoordinates() 
        {
            return _visibilityCircleSetFoggedCoordinates;
        }

        protected bool GetPersistentFogActivated(DifficultyLevel difficultyLevel)
        {
            switch (difficultyLevel)
            {
                case DifficultyLevel.Easy:
                    return false;
                case DifficultyLevel.Medium:
                    return false;
                case DifficultyLevel.Hard:
                    return true;
                default:
                    return false;
            }
        }

        protected Coordinate GetPlayerStartLocation(DifficultyLevel difficultyLevel) 
        {
            switch (difficultyLevel) 
            {
                case DifficultyLevel.Easy:
                    return new Coordinate(1052, 385);
                case DifficultyLevel.Medium:
                    return new Coordinate(1052, 385);
                case DifficultyLevel.Hard:
                    return new Coordinate(1052, 385);
                default:
                    return new Coordinate(0, 0);
            }
        }

        protected BitmapImage? GetPlayerImage(DifficultyLevel difficultyLevel) 
        {
            switch (difficultyLevel)
            {
                case DifficultyLevel.Easy:
                    return Application.Current.Resources["PlayerLarge"] as BitmapImage;
                case DifficultyLevel.Medium:
                    return Application.Current.Resources["PlayerLarge"] as BitmapImage;
                case DifficultyLevel.Hard:
                    return Application.Current.Resources["PlayerLarge"] as BitmapImage;
                default:
                    return null;
            }
        }

        private int GetVisibilityRadius(DifficultyLevel difficultyLevel)
        {
            switch (difficultyLevel)
            {
                case DifficultyLevel.Easy:
                    return 90;
                case DifficultyLevel.Medium:
                    return 60;
                case DifficultyLevel.Hard:
                    return 30;
                default:
                    return 0;
            }
        }

        private Int32Rect GetExitArea(DifficultyLevel difficultyLevel) 
        {
            switch (difficultyLevel)
            {
                case DifficultyLevel.Easy:
                    return new Int32Rect(445, 5, 117, 20);
                case DifficultyLevel.Medium:
                    return new Int32Rect(445, 5, 117, 20);
                case DifficultyLevel.Hard:
                    return new Int32Rect(445, 5, 117, 20);
                default:
                    return new Int32Rect(0, 0, 0, 0);
            }
        }

        private BitmapImage? GetGameMapImage()
        {
            return (Application.Current.Resources["Labyrinth"] as BitmapImage);
        }

        private BitmapImage? GetRoadDefinition()
        {
            return Application.Current.Resources["TestLabyrinthRoadSampleArea"] as BitmapImage;
        }

        protected Dictionary<Coordinate, Pixel> GetGameMapMeta()
        {
            return _gameMap;
        }

        /// <summary>
        /// Defines what is road on the game map by extracting unique pixel values from supplied image representing road.
        /// </summary>

        private void SetRoadDefinition(BitmapImage? roadSampleArea)
        {
            if (roadSampleArea != null)
            {
                int[] arrRoadDefinitionTemp = new int[(roadSampleArea.PixelWidth * roadSampleArea.PixelHeight)];

                int stride = roadSampleArea.PixelWidth * 4;

                roadSampleArea.CopyPixels(arrRoadDefinitionTemp, stride, 0);

                arrRoadDefinition = arrRoadDefinitionTemp.Distinct().ToArray();

                for (int i = 0; i < arrRoadDefinition.Length; i++)
                {
                    Road.Add(arrRoadDefinition[i], true);
                }
            }
        }

        /// <summary>
        /// Generates all exit area coordinates within supplied rectangle.
        /// </summary>

        private void MapExitArea(Int32Rect exitLocation)
        {
            int arrCounter = 0;
            for (int y = 0; y < exitLocation.Height; y++)
            {
                for (int x = 0; x < exitLocation.Width; x++)
                {
                    ExitArea.Add(new Coordinate(x + exitLocation.X, y + exitLocation.Y), true);
                    arrCounter++;
                }
            }
        }

        /// <summary>
        /// Generates coordinates for a filled circle at size by radius.
        /// Also generates coordinates for second smaller circle 1/3 of radius
        /// to be used to determine which coordinates that needs to be fogged
        /// on player movement on game level map.
        /// </summary>

        private void GenerateVisibilityCircle(int radius)
        {
            for (int x = -radius; x < radius; x++)
            {
                int height = Convert.ToInt32(Math.Sqrt(radius * radius - x * x));

                for (int y = -height; y < height; y++)
                {
                    _visibilityCircleCoordinates.Add(new Coordinate(x, y));
                }
            }

            List<Coordinate> coordinatesInnerCircle = new List<Coordinate>();
            int radiusOffset = radius / 3;

            for (int x = -radius + radiusOffset; x < radius - radiusOffset; x++)
            {
                int height = Convert.ToInt32(Math.Sqrt(radius * radius - x * x));

                for (int y = -height + radiusOffset; y < height - radiusOffset; y++)
                {
                    coordinatesInnerCircle.Add(new Coordinate(x, y));
                }
            }

            List<Coordinate> coordinatesToFog = _visibilityCircleCoordinates.Except(coordinatesInnerCircle).ToList();

            foreach (Coordinate coordinate in coordinatesToFog) 
            {
                _visibilityCircleSetFoggedCoordinates.Add(coordinate, true);
            }
        }

        /// <summary>
        /// Evaluates all pixel values in game map to set pixel properties and then map corresponding pixels with coordinate keys in dictionary.
        /// Also sets fogged attribute of Pixel from image. Note, fog of war image needs to be same size as full map.
        /// </summary>
        /// 

        private void MapGameMap()
        {
            if (_mapImage != null)
            {
                BitmapImage? fogOfWarImage = Application.Current.Resources["FogOfWarCroppedSmall"] as BitmapImage;

                int[] arrFogOfWar;

                int[] arrFullMap = new int[_mapImage.PixelWidth * _mapImage.PixelHeight];
                int width = Convert.ToInt32(_mapImage.PixelWidth);
                int stride = width * sizeof(Int32);

                _mapImage.CopyPixels(arrFullMap, stride, 0);

                if (fogOfWarImage != null)
                {
                    arrFogOfWar = new int[fogOfWarImage.PixelWidth * fogOfWarImage.PixelHeight];
                    fogOfWarImage.CopyPixels(arrFogOfWar, stride, 0);
                }
                else
                {
                    arrFogOfWar = new int[_mapImage.PixelWidth * _mapImage.PixelHeight];
                }

                int arrPosition = 0;

                for (int y = 0; y < _mapImage.PixelHeight; y++)
                {
                    for (int x = 0; x < _mapImage.PixelWidth; x++)
                    {
                        Coordinate coordinate = new Coordinate(x, y);

                        bool walkable = false;
                        Road.TryGetValue(arrFullMap[arrPosition], out walkable);

                        bool exitArea = false;
                        ExitArea.TryGetValue(new Coordinate(x, y), out exitArea);

                        Pixel pixel = new Pixel(coordinate, arrFullMap[arrPosition], walkable, exitArea, arrFogOfWar[arrPosition]);
                        _gameMap.Add(coordinate, pixel);
                        arrPosition++;
                    }
                }
            }
        }
    }
}
