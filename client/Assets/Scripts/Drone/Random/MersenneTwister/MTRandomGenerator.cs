using System;
using System.IO;
using System.Xml.Serialization;
using Drone.Random.Interface;
using UnityEngine;

namespace Drone.Random.MersenneTwister
{
    public class MTRandomGenerator : IRandomGenerator
    {
        #region Interface

        public uint Seed { get; private set; }

        public void NewSeed(uint seed)
        {
            Seed = seed;
            InitGenerator(Seed);
        }

        public float GetFloat()
        {
            return (float) GenerateDouble();
        }

        public int GetInt()
        {
            return (int) GenerateInt();
        }

        public float Range(float min, float max)
        {
            return (float) (GenerateDouble() * (max - min) + min);
        }

        public int Range(int min, int max)
        {
            return (int) (GenerateDouble() * (max - min) + min);
        }

        public Vector2 GetInsideCircle(float radius = 1)
        {
            float x = Range(-1f, 1f) * radius;
            float y = Range(-1f, 1f) * radius;
            return new Vector2(x, y);
        }

        public Vector3 GetInsideSphere(float radius = 1)
        {
            float x = Range(-1f, 1f) * radius;
            float y = Range(-1f, 1f) * radius;
            float z = Range(-1f, 1f) * radius;
            return new Vector3(x, y, z);
        }

        public Quaternion GetRotation()
        {
            return GetRotationOnSurface(GetInsideSphere());
        }

        public Quaternion GetRotationOnSurface(Vector3 surface)
        {
            return new Quaternion(surface.x, surface.y, surface.z, GetFloat());
        }

        #endregion

        #region Constants

        private const uint DEFAULT_SEED = 5489;

        /// <summary>
        /// The following constant, is used within genrand_real1(), which returns values in [0,1]
        /// </summary>
        private const double K_MT_1 = 1.0 / 4294967295.0;

        #endregion

        #region Period parameters

        /// <summary>
        /// State size
        /// </summary>
        private const int N = 624;
        /// <summary>
        /// Shift size
        /// </summary>  
        private const int M = 397;
        ///* XOR mask*/
        private const uint MATRIX_A = 0x9908b0dfu;
        ///* most significant w-r bits */
        private const uint UPPER_MASK = 0x80000000u;
        ///* least significant r bits */
        private const uint LOWER_MASK = 0x7fffffffu;

        #endregion

        #region GeneratorState

        private class MTState
        {
            public int mti;

            public uint[] mt = new uint[N];

            public MTState()
            {
                this.mti = MTRandomGenerator.N + 1;
            }
        }

        private MTState _state = new MTState();

        #endregion

        #region Initialization

        /// <summary>
        /// Initialize PRNG with the default seed
        /// </summary>
        public MTRandomGenerator()
        {
            InitGenerator(DEFAULT_SEED);
        }

        /// <summary>
        /// Initialize PRNG with seed
        /// </summary>
        public MTRandomGenerator(uint seed)
        {
            Seed = seed;
            InitGenerator(seed);
        }

        /// <summary>
        /// Initialize the Generator with an XML file created by MTRandomGenerator.SaveState()
        /// </summary>
        public MTRandomGenerator(string fileName)
        {
            LoadState(fileName);
        }

        #endregion

        #region Implementation

        /// <summary>
        /// Initialize Generator with seed
        /// </summary>
        private void InitGenerator(uint seed)
        {
            _state.mt[0] = (seed & 0xffffffffu);
            for (_state.mti = 1; _state.mti < N; _state.mti++) {
                uint i = _state.mt[_state.mti - 1];
                _state.mt[_state.mti] =
                        Convert.ToUInt32((1812433253uL * Convert.ToUInt64(i ^ (i >> 30)) + Convert.ToUInt64(_state.mti)) & 0xffffffffuL);
            }
        }

        /// <summary>
        /// Save the PRNG state to a file as XML
        /// </summary>
        private void SaveState(string fileName)
        {
            try {
                XmlSerializer serializer = new XmlSerializer(typeof(MTState));
                FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write);

                serializer.Serialize(fs, _state);
                fs.Close();
            } catch (Exception ex) {
                throw new SaveLoadStateException(ex.Message, Environment.StackTrace, 0);
            }
        }

        /// <summary>
        /// Load the PRNG state from a file created by SaveState()
        /// </summary>
        private void LoadState(string fileName)
        {
            try {
                XmlSerializer serializer = new XmlSerializer(typeof(MTState));
                FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);

                _state = (MTState) serializer.Deserialize(fs);
                fs.Close();
            } catch (Exception ex) {
                throw new SaveLoadStateException(ex.Message, Environment.StackTrace, 1);
            }
        }

        private uint GenerateInt()
        {
            uint y;

            if ((_state.mti >= N)) {
                int kk;
                if (_state.mti == N + 1) {
                    InitGenerator(Seed);
                }

                for (kk = 0; kk < N - M; kk++) {
                    y = (_state.mt[kk] & UPPER_MASK) | (_state.mt[kk + 1] & LOWER_MASK);
                    _state.mt[kk] = _state.mt[kk + M] ^ (y >> 1) ^ (Convert.ToUInt32(y & 1u) * MATRIX_A);
                }

                for (; kk < N - 1; kk++) {
                    y = (_state.mt[kk] & UPPER_MASK) | (_state.mt[kk + 1] & LOWER_MASK);
                    _state.mt[kk] = _state.mt[kk + (M - N)] ^ (y >> 1) ^ (Convert.ToUInt32(y & 1u) * MATRIX_A);
                }

                y = (_state.mt[N - 1] & UPPER_MASK) | (_state.mt[0] & LOWER_MASK);
                _state.mt[N - 1] = _state.mt[M - 1] ^ (y >> 1) ^ (Convert.ToUInt32(y & 1u) * MATRIX_A);
                _state.mti = 0;
            }

            y = _state.mt[_state.mti++];

            y ^= (y >> 11);

            y ^= (y << 7) & 0x9d2c5680u;

            y ^= (y << 15) & 0xefc60000u;

            return y ^ (y >> 18);
        }

        private uint GenerateInt(uint n)
        {
            uint used = N;
            for (int degree = 0; degree < 5; degree++) {
                used |= (used >> (int) Math.Pow(2, degree));
            }

            uint i;
            do {
                i = GenerateInt() & used;
            } while (i > N);

            return i;
        }

        private uint GenerateIntRange(uint lower, uint upper)
        {
            if (lower > upper) {
                (lower, upper) = (upper, lower);
            }
            return lower + GenerateInt(upper - lower);
        }

        /// <summary>
        /// Generates a random number on [0,1]
        /// </summary>
        /// <returns>
        ///  GenerateInt() * (1.0/4294967295.0) -> divided by 2^32-1 
        /// </returns>
        private double GenerateDouble()
        {
            return GenerateInt() * K_MT_1;
        }

        #endregion

        #region Exeption

        private class SaveLoadStateException : Exception
        {
            private readonly string _msg;
            private readonly string _stkTrace;

            public SaveLoadStateException(string baseMessage, string stackTrace, int state)
            {
                string reason = state == 0 ? "save" : "load";
                _msg = $"Failed to {reason} generator state " + "\r\n" + baseMessage;
                _stkTrace = stackTrace;
            }

            public override string StackTrace
            {
                get { return _stkTrace; }
            }

            public override string Message
            {
                get { return _msg; }
            }
        }

        #endregion
    }
}