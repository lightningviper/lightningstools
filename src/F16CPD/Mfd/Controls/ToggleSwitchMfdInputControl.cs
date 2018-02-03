using System;
using System.Collections.Generic;
using System.Linq;

namespace F16CPD.Mfd.Controls
{
    public class ToggleSwitchPositionChangedEventArgs : EventArgs
    {
        protected ToggleSwitchPositionMfdInputControl _newPosition;
        protected ToggleSwitchPositionMfdInputControl _previousPosition;

        public ToggleSwitchPositionChangedEventArgs()
        {
        }

        public ToggleSwitchPositionChangedEventArgs(ToggleSwitchPositionMfdInputControl previousPosition,
                                                    ToggleSwitchPositionMfdInputControl newPosition)
        {
            _previousPosition = previousPosition;
            _newPosition = newPosition;
        }

        public ToggleSwitchPositionMfdInputControl PreviousPosition
        {
            get { return _previousPosition; }
            set { _previousPosition = value; }
        }

        public ToggleSwitchPositionMfdInputControl NewPosition
        {
            get { return _newPosition; }
            set { _newPosition = value; }
        }
    }

    public class ToggleSwitchMfdInputControl : MfdInputControl
    {
        protected int CurPosition = -1;
        protected int PrevPosition = -1;
        protected List<ToggleSwitchPositionMfdInputControl> _positions = new List<ToggleSwitchPositionMfdInputControl>();

        public List<ToggleSwitchPositionMfdInputControl> Positions
        {
            get { return _positions; }
            set { _positions = value; }
        }

        public int CurrentPositionIndex
        {
            get { return CurPosition; }
            set
            {
                var curPosition = CurPosition;
                if (value < 0) throw new ArgumentOutOfRangeException();
                CurPosition = value <= _positions.Count - 1 ? value : 0;
                if (curPosition != CurPosition)
                {
                    PrevPosition = curPosition;
                }
                OnPositionChanged();
            }
        }

        public ToggleSwitchPositionMfdInputControl CurrentPosition
        {
            get { return _positions[CurPosition]; }
            set
            {
                if (value == null) throw new ArgumentNullException();
                var curPosition = CurPosition;
                var i = 0;
                foreach (var position in _positions)
                {
                    if (position.PositionName == value.PositionName)
                    {
                        CurPosition = i;
                    }
                    i++;
                }
                if (curPosition != CurPosition)
                {
                    PrevPosition = curPosition;
                }
                OnPositionChanged();
            }
        }

        public event EventHandler<ToggleSwitchPositionChangedEventArgs> PositionChanged;

        public int AddPosition(string positionName)
        {
            _positions.Add(new ToggleSwitchPositionMfdInputControl(positionName, this));
            return _positions.Count - 1;
        }

        public ToggleSwitchPositionMfdInputControl GetPositionByName(string positionName)
        {
            return _positions.FirstOrDefault(position => position.PositionName == positionName);
        }

        public void Toggle()
        {
            var curPosition = CurPosition;
            CurPosition++;
            if (CurPosition > _positions.Count - 1)
            {
                CurPosition = 0;
            }
            if (curPosition != CurPosition)
            {
                OnPositionChanged();
            }
        }

        protected virtual void OnPositionChanged()
        {
            if (PositionChanged != null)
            {
                ToggleSwitchPositionMfdInputControl prevPosition = null;
                ToggleSwitchPositionMfdInputControl curPosition = null;
                if (PrevPosition >= 0)
                {
                    prevPosition = Positions[PrevPosition];
                }
                if (CurPosition >= 0)
                {
                    curPosition = Positions[CurPosition];
                }
                PositionChanged(this, new ToggleSwitchPositionChangedEventArgs(prevPosition, curPosition));
            }
        }
    }
}