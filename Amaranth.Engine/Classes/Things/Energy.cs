using System;
using System.Collections.Generic;
using System.Text;

namespace Amaranth.Engine
{
    /// <summary>
    /// Represents "energy" gained by entities as time passes, based on their speed.
    /// </summary>
    [Serializable]
    public class Energy
    {
	    public const int MinSpeed       = 0;
	    public const int NormalSpeed    = 6;
        public const int MaxSpeed       = 12;

        public const int ActionCost = 240;

        public static int GetGain(int speed)
        {
            return EnergyGains[speed];
        }

        public bool HasEnergy { get { return mEnergy >= ActionCost; } }

        public EnergyTimerCollection Timers { get { return mTimers; } }

        public Energy(ISpeed speed)
        {
            if (speed == null) throw new ArgumentNullException("speed");

            mSpeed = speed;
        }

        public Energy(int speed)
        {
            mSpeed = new FixedSpeed(speed);
        }

        public void Clear()
        {
            mEnergy = 0;
        }

        public void Gain()
        {
            int speed = mSpeed.Speed;

            if (speed < MinSpeed) throw new ArgumentOutOfRangeException("speed");
            if (speed > MaxSpeed) throw new ArgumentOutOfRangeException("speed");

            mEnergy += EnergyGains[speed];

            mTimers.Gain(EnergyGains[speed]);
        }

        public void Fill()
        {
            mEnergy = ActionCost;
        }

        public void Randomize()
        {
            mEnergy = Rng.Int(ActionCost);
        }

        public void Spend()
        {
            mEnergy -= ActionCost;
        }

        private class FixedSpeed : ISpeed
        {
            public FixedSpeed(int speed)
            {
                mSpeed = speed;
            }

            #region ISpeed Members

            int ISpeed.Speed
            {
                get { return mSpeed; }
            }

            #endregion

            private int mSpeed;
        }

        private static readonly int[] EnergyGains = new int[]
		    {
			    15,     // 1/4 normal speed
			    20,     // 1/3 normal speed
                25,
			    30,	    // 1/2 normal speed
                40,
                50,
			    60,	    // normal speed
                80,
                100,
			    120,    // 2x normal speed
                150,
			    180,    // 3x normal speed
			    240     // 4x normal speed
		    };

        //### bob: old range
        /*
        public const int ActionCost = 360;
	    private static readonly int[] EnergyGains = new int[]
		    {
			    10,		// 1/6 normal speed
			    12,		// 1/5 normal speed
			    15,		// 1/4 normal speed
			    20,		// 1/3 normal speed
			    30,		// 1/2 normal speed
			    60,		// normal speed
			    120,	// 2x normal speed
			    180,	// 3x normal speed
			    240,	// 4x normal speed
			    300,	// 5x normal speed
			    360		// 6x normal speed
		    };
        */

        private ISpeed mSpeed;
	    private int mEnergy;

        private readonly EnergyTimerCollection mTimers = new EnergyTimerCollection();
    }
}
