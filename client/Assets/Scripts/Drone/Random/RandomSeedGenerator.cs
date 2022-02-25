using System;
using System.Security.Cryptography;

namespace Drone.Random
{
    public static class RandomSeedGenerator
    {
        private static readonly RNGCryptoServiceProvider RNG_CRYPTO_SERVICE_PROVIDER = new RNGCryptoServiceProvider();

        /// <summary>
        /// Create seed based on DateTime structure
        /// </summary>
        public static uint Time()
        {
            return (uint) DateTime.UtcNow.GetHashCode();
        }

        /// <summary>
        /// Create seed based on <see cref="Environment.TickCount"/> and <see cref="System.Guid"/>
        /// </summary>
        public static uint Guid()
        {
            return (uint) (Environment.TickCount ^ System.Guid.NewGuid().GetHashCode());
        }

        /// <summary>
        /// Create seed based on <see cref="System.Security.Cryptography.RNGCryptoServiceProvider"/>
        /// </summary>
        public static uint Crypto()
        {
            byte[] bytes = new byte[4];
            RNG_CRYPTO_SERVICE_PROVIDER.GetBytes(bytes);
            return (uint) BitConverter.ToInt32(bytes, 0);
        }
    }
}