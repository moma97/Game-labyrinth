using Labyrinth.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Labyrinth
{
    public static class CurrentDifficulty
    {
        /// <summary>
        /// Properties of labyrinth and player 
        /// </summary>
        public static int TimeLimitSeconds { get; private set; } 
        public static int VisibilityCircleRadius { get; private set; } 
        public static int WalkSpeed { get; private set; } 
        public static DifficultyLevel CurrentDifficultyLevel { get; private set; }
        /// <summary>
        /// Sets values based on selected difficultylevel 
        /// </summary>
        /// <param name="level"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static void SetCurrentDifficulty(DifficultyLevel level)
        {
            CurrentDifficultyLevel = level;
            switch (level)
            {
                case DifficultyLevel.Easy:
                    TimeLimitSeconds = 300;
                    VisibilityCircleRadius = 90;
                    WalkSpeed = 5;
                    break;
                case DifficultyLevel.Medium:
                    TimeLimitSeconds = 240;
                    VisibilityCircleRadius = 60;
                    WalkSpeed = 3;
                    break;
                case DifficultyLevel.Hard:
                    TimeLimitSeconds = 180;
                    VisibilityCircleRadius = 30;
                    WalkSpeed = 2;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(level), level, null);
            }
        }
    }
}
