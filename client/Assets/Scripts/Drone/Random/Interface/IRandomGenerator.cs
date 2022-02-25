using UnityEngine;

namespace Drone.Random.Interface
{
    public interface IRandomGenerator
    {
        uint Seed { get; }

        /// <summary>
        /// Create new random sequence based on new seed
        /// </summary>
        void NewSeed(uint seed);

        /// <returns>
        /// A random float number between [0.0f, 1.0f] 
        /// </returns>
        float GetFloat();

        /// <returns>
        /// A random int number between [int.MinValue, int.MaxValue]
        /// </returns>
        int GetInt();

        /// <returns>
        /// A random float number between [min, max] 
        /// </returns>
        float Range(float min, float max);

        /// <returns>
        /// A random int number between [int.MinValue, int.MaxValue]
        /// </returns>
        int Range(int min, int max);

        /// <returns>
        /// A point inside the circle with given radius
        /// </returns>
        Vector2 GetInsideCircle(float radius);

        /// <returns>
        /// A point inside the sphere with given radius
        /// </returns>
        Vector3 GetInsideSphere(float radius);

        /// <returns>
        /// A random Quaternion struct where X,Y,Z,W has numbers in [0.0f, 1.0f]
        /// </returns>
        Quaternion GetRotation();

        /// <returns>
        /// A random Quaternion struct where W has numbers in [0.0f, 1.0f]
        /// </returns>
        Quaternion GetRotationOnSurface(Vector3 surface);
    }
}