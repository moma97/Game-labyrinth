using Labyrinth.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Labyrinth
{
    public class GameLevel : GameLevelFactory
    {
        private DifficultyLevel _difficulty;

        public DifficultyLevel Difficulty
        {
            get { return _difficulty; }
        }

        private Dictionary<Coordinate, Pixel> _gameMapMeta;

        public Dictionary<Coordinate, Pixel> GameMapMeta
        {
            get { return _gameMapMeta; }
        }

        private Coordinate _playerStartLocation;

        public Coordinate PlayerStartLocation
        {
            get { return _playerStartLocation; }
        }

        private List<Coordinate> _visibilityCircleCoordinates = new List<Coordinate>();

        public List<Coordinate> VisibilityCircleCoordinates
        {
            get { return _visibilityCircleCoordinates; }
        }

        Dictionary<Coordinate, bool> _visibilityCircleSetFoggedCoordinates = new Dictionary<Coordinate, bool>();

        public Dictionary<Coordinate, bool> VisibilityCircleSetFoggedCoordinates
        {
            get { return _visibilityCircleSetFoggedCoordinates; }
        }

        private BitmapImage? _playerImage;

        public BitmapImage? PlayerImage
        {
            get { return _playerImage; }
        }

        private bool _persistentFog;

        public bool PersistentFog
        {
            get { return _persistentFog; }
        }


        public GameLevel(DifficultyLevel difficultyLevel)
        {
            _difficulty = difficultyLevel;
            GenerateGameLevel(difficultyLevel);
            _gameMapMeta = GetGameMapMeta();
            _playerStartLocation = GetPlayerStartLocation(difficultyLevel);
            _visibilityCircleCoordinates = GetVisibilityCoordinates();
            _visibilityCircleSetFoggedCoordinates = GetVisibilitySetFoggedCoordinates();
            _playerImage = GetPlayerImage(difficultyLevel);
            _persistentFog = GetPersistentFogActivated(difficultyLevel);
        }

        /// <summary>
        /// Get pixel by coordinate value.
        /// </summary>

        public Pixel? this[Coordinate key]
        {
            get
            {
                Pixel? outValue = null;
                GameMapMeta.TryGetValue(key, out outValue);
                return outValue;
            }
        }
    }
}
