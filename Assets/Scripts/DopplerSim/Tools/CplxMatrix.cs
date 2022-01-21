using System;

namespace DopplerSim.Tools
{
    public class CplxMatrix
    {
        public int rows { get; private set; }
        public int cols { get; private set; }

        public double[][] re;
        public double[][] im;

        public CplxMatrix(int rows, int cols)
        {
            this.rows = rows;
            this.cols = cols;
            re = MultiArray.New<Double>(rows, cols);
            im = MultiArray.New<Double>(rows, cols);
        }
    
        public CplxMatrix(int rows, int cols, double[][] re, double[][] im) {
            this.rows = rows;
            this.cols = cols;
            this.re = re;
            this.im = im;
        }
  
        public void transpose() {
            double[][] tempRe = MultiArray.New<Double>(rows, cols);
            double[][] tempIm = MultiArray.New<Double>(rows, cols);
    
            for (int i = 0; i < rows; i++) {
                for (int j = 0; j < cols; j++) {
                    tempRe[j][i] = re[i][j];
                    tempIm[j][i] = im[i][j];
                }
            }
    
            re = tempRe;
            im = tempIm;
    
            int tCols = cols;
            cols = rows;
            rows = tCols;
        }
  
        public double[][] abs() {
            double[][] abs = MultiArray.New<Double>(rows, cols);
            for (int i = 0; i < rows; i++) {
                for (int j = 0; j < cols; j++) {
                    abs[i][j] = Math.Sqrt(Math.Pow(re[i][j], 2.0D) + Math.Pow(im[i][j], 2.0D));
                }
            }
            return abs;
        }
    }
}