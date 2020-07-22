using System;
using System.Collections.Generic;
using System.Linq;

namespace NoiseGenerator.Source {

    /// <summary>
    /// Perlin noise calculation class.
    /// Made using reference: https://mrl.nyu.edu/~perlin/noise/
    /// </summary>
    public static class PerlinNoise {

        /// <summary>
        /// Gradient vector permutation table of size 256.
        /// </summary>
        private static readonly int[] m_vectorPermutationTable = {
            151, 160, 137, 91, 90, 15, 131, 13, 201, 95, 96, 53, 194, 233, 7, 225,
            140, 36, 103, 30, 69, 142, 8, 99, 37, 240, 21, 10, 23, 190, 6, 148,
            247, 120, 234, 75, 0, 26, 197, 62, 94, 252, 219, 203, 117, 35, 11, 32,
            57, 177, 33, 88, 237, 149, 56, 87, 174, 20, 125, 136, 171, 168, 68, 175,
            74, 165, 71, 134, 139, 48, 27, 166, 77, 146, 158, 231, 83, 111, 229, 122,
            60, 211, 133, 230, 220, 105, 92, 41, 55, 46, 245, 40, 244, 102, 143, 54,
            65, 25, 63, 161, 1, 216, 80, 73, 209, 76, 132, 187, 208, 89, 18, 169,
            200, 196, 135, 130, 116, 188, 159, 86, 164, 100, 109, 198, 173, 186, 3, 64,
            52, 217, 226, 250, 124, 123, 5, 202, 38, 147, 118, 126, 255, 82, 85, 212,
            207, 206, 59, 227, 47, 16, 58, 17, 182, 189, 28, 42, 223, 183, 170, 213,
            119, 248, 152, 2, 44, 154, 163, 70, 221, 153, 101, 155, 167, 43, 172, 9,
            129, 22, 39, 253, 19, 98, 108, 110, 79, 113, 224, 232, 178, 185, 112, 104, 
            218, 246, 97, 228, 251, 34, 242, 193, 238, 210, 144, 12, 191, 179, 162, 241,
            81, 51, 145, 235, 249, 14, 239, 107, 49, 192, 214, 31, 181, 199, 106, 157,
            184, 84, 204, 176, 115, 121, 50, 45, 127, 4, 150, 254, 138, 236, 205, 93,
            222, 114, 67, 29, 24, 72, 243, 141, 128, 195, 78, 66, 215, 61, 156, 180
        };

        /// <summary>
        /// Gradient vector permutation table repeated twice.
        /// </summary>
        private static readonly int[] m_vectorPermutationTableTwice = m_vectorPermutationTable.Concat (m_vectorPermutationTable).ToArray();

        /// <summary>
        /// Calculates Perlin noise value on given coordinates.
        /// </summary>
        /// <param name="x">Coordinate x.</param>
        /// <param name="y">Coordinate y.</param>
        /// <returns>Value in range [-1; 1].</returns>
        public static double CalculateNoise (double x, double y) {

            // Coordinates of the square that contains this point.
            int rectX = (int)Math.Floor (x) & 255;
            int rectY = (int)Math.Floor (y) & 255;

            // Gradient vector hashes for each corner of the square from permutation table.
            int[] p = m_vectorPermutationTableTwice;
            int grad1 = p[p[rectX] + rectY];
            int grad2 = p[p[rectX + 1] + rectY];
            int grad3 = p[p[rectX] + rectY + 1];
            int grad4 = p[p[rectX + 1] + rectY + 1];

            // Relative fractional distance from the square walls.
            double fracX = x - Math.Floor (x);
            double fracY = y - Math.Floor (y);

            // Dot products of gradient vectors and distance vectors from each corner of the square.
            double dot1 = DotProduct (grad1, fracX, fracY);
            double dot2 = DotProduct (grad2, fracX - 1, fracY);
            double dot3 = DotProduct (grad3, fracX, fracY - 1);
            double dot4 = DotProduct (grad4, fracX - 1, fracY - 1);

            // Fractional distance values passed through fade function.
            double fadedX = FadeFunction (fracX);
            double fadedY = FadeFunction (fracY);

            // Interpolate 2 times on x axis (top and bottom of the square).
            double interX1 = LinearInterpolation (fadedX, dot1, dot2);
            double interX2 = LinearInterpolation (fadedX, dot3, dot4);

            // Interpolate 2 interpolated values on y axis to get result.
            double interY = LinearInterpolation (fadedY, interX1, interX2);

            // Return calculated final value.
            return interY;
        }

        /// <summary>
        /// Calculates dot product between unit gradient vector and distance vector.
        /// </summary>
        /// <param name="hash">Selects between 4 gradient vectors: (1, 1), (-1, 1), (1, -1), (-1, -1).</param>
        /// <param name="x">Coordinate x of distance vector.</param>
        /// <param name="y">Coordinate y of distance vector.</param>
        /// <returns>Dot product of vectors.</returns>
        private static double DotProduct (int hash, double x, double y) {
            return (hash & 3) switch {
                0 => x + y,
                1 => -x + y,
                2 => x - y,
                3 => -x - y,
                _ => throw new Exception ("This is impossible to reach.")
            };
        }

        /// <summary>
        /// Passes given value through fade function.
        /// </summary>
        /// <param name="value">Fractional part of coordinate.</param>
        /// <returns>Fade function value for given parameter.</returns>
        private static double FadeFunction (double value) {
            return 6 * Math.Pow (value, 5) - 15 * Math.Pow (value, 4) + 10 * Math.Pow (value, 3);
        }

        /// <summary>
        /// Interpolates between left and right points.
        /// </summary>
        /// <param name="amount">Fraction amount.</param>
        /// <param name="left">Left point.</param>
        /// <param name="right">Right point.</param>
        /// <returns>Interpolated value.</returns>
        private static double LinearInterpolation (double amount, double left, double right) {
            return (1 - amount) * left + amount * right;
        }

    }
}
