using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Labyrinth
{
    /// <summary>
    /// Holds an x and y coordinate and overrides equal method to compare both x and y value with other compatible types
    /// </summary>
    public struct Coordinate : IEquatable<Coordinate>
    {
        public int X { get; private set; }
        public int Y { get; private set; }

        public Coordinate(int x, int y)
        {
            X = x;
            Y = y;
        }

        public bool Equals(Coordinate other)
        {
            return other.X == X && other.Y == Y;
        }
    }
}
