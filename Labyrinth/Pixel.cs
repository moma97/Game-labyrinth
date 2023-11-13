using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Labyrinth
{
    /// <summary>
    /// Maps pixel with corresponding coordinate. Defines rules for labyrinth game map such as walkable are, exit and start location.
    /// </summary>
    public class Pixel
    {
        public Coordinate Coordinate { get; private set; }

        private readonly int _originalValue;

        public int OriginalValue
        {
            get { return _originalValue; }
        }

        private readonly int _foggedValue;

        public int FoggedValue
        {
            get { return _foggedValue; }
        }

        private readonly bool _walkable;

        public bool Walkable
        {
            get { return _walkable; }
        }

        private readonly bool _exitArea;

        public bool ExitArea
        {
            get { return _exitArea; }
        }

        private int _currentValue;

        public int CurrentValue
        {
            get { return _currentValue; }
            set { _currentValue = value; }
        }

        public Pixel(Coordinate coordinate, int originalValue, bool walkable, bool exitArea, int foggedValue)
        {
            _originalValue = originalValue;
            Coordinate = coordinate;
            _walkable = walkable;
            _exitArea = exitArea;
            _foggedValue = foggedValue;
        }

        /// <summary>
        /// Set a new RGB value.
        /// </summary>
        /// <param name="newValue"></param>
        public void SetCurrentValue(int newValue)
        {
            _currentValue = newValue;
        }

        /// <summary>
        /// Sets current pixel value to fogged value
        /// </summary>

        public void SetFog()
        {
            _currentValue = FoggedValue;
        }

        /// <summary>
        /// Reset pixel value to the original value set on creation.
        /// </summary>
        public void ResetValue()
        {
            _currentValue = _originalValue;
        }
    }
}
