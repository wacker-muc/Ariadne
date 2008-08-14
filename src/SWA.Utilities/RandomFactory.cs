using System;
using System.Collections.Generic;
using System.Text;

namespace SWA.Utilities
{
    /// <summary>
    /// Creates Random number generators with distinct seeds.
    /// Use this class if your application may several Random objects at the same time.
    /// The simple Random() constructor would seed them all with the (identical) current time.
    /// </summary>
    public static class RandomFactory
    {
        private static Random r = new Random();

        /// <summary>
        /// Returns a new Random object with a random initial seed.
        /// </summary>
        /// <returns></returns>
        public static Random CreateRandom()
        {
            return new Random(r.Next());
        }

        /// <summary>
        /// Returns a new Random object with the specific given initial seed.
        /// </summary>
        /// <returns></returns>
        public static Random CreateRandom(int seed)
        {
            return new Random(seed);
        }
    }
}
