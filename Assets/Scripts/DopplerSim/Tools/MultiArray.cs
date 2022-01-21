
namespace DopplerSim.Tools
{
    public static class MultiArray
    {
        /// <summary>
        /// Creates a two dimensional array with the given rows and columns
        /// </summary>
        /// <typeparam name="T">Type of the array</typeparam>
        /// <returns>Two dimensional array with the given rows and columns</returns>
        public static T[][] New<T>(int rows, int cols)
        {
            T[][] newMatrix = new T[rows][];
            for (int i = 0; i < rows; i++)
            {
                newMatrix[i] = new T[cols];
            }
            return newMatrix;
        }
    }
}
