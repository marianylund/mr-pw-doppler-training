using System;

namespace DopplerSim.Tools
{
public class FFTTools
{
    public static readonly double TWO_PI = 6.283185307179586D;

    public FFTTools() {}

    public static CplxMatrix fftshift(CplxMatrix fft)
    {
      CplxMatrix s = new CplxMatrix(fft.rows, fft.cols);
      int halfx = fft.rows / 2;
      int halfy = fft.cols / 2;
      

      for (int i = 0; i < halfx; i++) {
        for (int j = 0; j < halfy; j++) {
          s.re[i][j] = fft.re[(i + halfx)][(j + halfy)];
          s.im[i][j] = fft.im[(i + halfx)][(j + halfy)];
        }
      }
      

      for (int i = halfx; i < fft.rows; i++) {
        for (int j = 0; j < halfy; j++) {
          s.re[i][j] = s.re[(i - halfx)][(j + halfy)];
          s.im[i][j] = s.im[(i - halfx)][(j + halfy)];
        }
      }
      

      for (int i = 0; i < halfx; i++) {
        for (int j = halfy; j < fft.cols; j++) {
          s.re[i][j] = s.re[(i + halfx)][(j - halfy)];
          s.im[i][j] = s.im[(i + halfx)][(j - halfy)];
        }
      }
      

      for (int i = halfx; i < fft.rows; i++) {
        for (int j = halfy; j < fft.cols; j++) {
          s.re[i][j] = s.re[(i - halfx)][(j - halfy)];
          s.im[i][j] = s.im[(i - halfx)][(j - halfy)];
        }
      }
      
      return s;
    }
    
    public static CplxMatrix ifft2(CplxMatrix ft)
    {
      FFT fft = new FFT() {};
      
      ft.transpose();
      

      for (int i = 0; i < ft.rows; i++) {
        fft.ifft(ft.cols, ft.re[i], ft.im[i]);
        ft.re[i] = fft.yRe;
        ft.im[i] = fft.yIm;
      }
      
      ft.transpose();
      

      for (int i = 0; i < ft.rows; i++) {
        fft.ifft(ft.cols, ft.re[i], ft.im[i]);
        ft.re[i] = fft.yRe;
        ft.im[i] = fft.yIm;
      }
      

      return ft;
    }
    
    public static double[] zeroes(int n) {
      double[] zero = new double[n];
      for (int i = 0; i < n; i++) {
        zero[i] = 0.0D;
      }
      return zero;
    }
    
    public static CplxMatrix fft2d(double[][] inMat)
    {
      int width = inMat[0].Length;
      int height = inMat.Length;
      
      FFT fft = new FFT() {};
      CplxMatrix cm = new CplxMatrix(height, width);
      

      for (int i = 0; i < height; i++) {
        fft.fft(width, inMat[i], zeroes(width));
        cm.re[i] = fft.yRe;
        cm.im[i] = fft.yIm;
      }
      
      cm.transpose();
      

      for (int i = 0; i < width; i++) {
        fft.fft(height, cm.re[i], cm.im[i]);
        cm.re[i] = fft.yRe;
        cm.im[i] = fft.yIm;
      }
      
      cm.transpose();
      
      return cm;
    }
    
    public static CplxMatrix convolve(CplxMatrix m1, CplxMatrix m2)
    {
      if ((m1.rows != m2.rows) || (m1.cols != m2.cols)) {
        return null;
      }
      CplxMatrix m = new CplxMatrix(m1.rows, m1.cols);
      for (int i = 0; i < m1.rows; i++) {
        for (int j = 0; j < m1.cols; j++) {
          m.re[i][j] = (m1.re[i][j] * m2.re[i][j] - m1.im[i][j] * m2.im[i][j]);
          m.im[i][j] = (m1.re[i][j] * m2.im[i][j] + m1.im[i][j] * m2.re[i][j]);
        }
      }
      return m;
    }
    
    public static CplxMatrix rampFilter(int length)
    {
      int center = length / 2;
      CplxMatrix f = new CplxMatrix(1, length);
      for (int i = 0; i < length; i++) {
        f.re[0][i] = (Math.Abs(center - i) / center);
      }
      return f;
    }
    
    public static double[] abs(double[] re, double[] im)
    {
      if (re.Length != im.Length) {
        return null;
      }
      
      double[] abs = new double[re.Length];
      for (int i = 0; i < re.Length; i++) {
        abs[i] = Math.Sqrt(Math.Pow(re[i], 2.0D) + Math.Pow(im[i], 2.0D));
      }
      
      return abs;
    }
    
    public static double[] hamming(int length)
    {
      double a = 0.54D;
      double b = 1.0D - a;
      double tpn = 6.283185307179586D / (length - 1.0D);
      
      double[] h = new double[length];
      for (int i = 0; i < length; i++) {
        h[i] = (a - b * Math.Cos(i * tpn));
      }
      return h;
    }
    
    public static double[] tmult(double[] one, double[] two)
    {
      if (one.Length != two.Length) {
        return null;
      }
      double[] outMat = new double[one.Length];
      for (int i = 0; i < one.Length; i++) {
        outMat[i] = one[i] * two[i];
      }
      return outMat;
    }
    
    public static double[] add(double[] one, double[] two)
    {
      if (one.Length != two.Length) {
        return null;
      }

      double[] outMat = new double[one.Length];
      for (int i = 0; i < one.Length; i++) {
        outMat[i] = one[i] + two[i];
      }
      return outMat;
    }
    
    public static double[] minus(double[] one, double[] two)
    {
      if (one.Length != two.Length) {
        return null;
      }
      double[] outMat = new double[one.Length];
      for (int i = 0; i < one.Length; i++) {
        outMat[i] = one[i] - two[i];
      }
      return outMat;
    }
    
    public static double max(double[] inMat)
    {
      double max = inMat[0];
      for (int i = 0; i < inMat.Length; i++) {
        if (inMat[i] > max) {
          max = inMat[i];
        }
      }
      return max;
    }
    
    public static double[] sine(int n, double f)
    {
      double[] outMat = new double[n];
      for (int i = 0; i < n; i++) {
        outMat[i] = Math.Sin(6.283185307179586D * f * i);
      }
      return outMat;
    }
    
    public static double[] cosine(int n, double f)
    {
      double[] outMat = new double[n];
      for (int i = 0; i < n; i++) {
        outMat[i] = Math.Cos(6.283185307179586D * f * i);
      }
      return outMat;
    }

    public static CplxMatrix hilbert(double[] signal)
    {
      FFT fft = new FFT() {};
      fft.fft(signal.Length, signal, zeroes(signal.Length));
      
      int half = signal.Length / 2;
      double[] out_re = new double[signal.Length];
      double[] out_im = new double[signal.Length];
      for (int m = 0; m < signal.Length; m++) {
        if ((m == 0) || (m == half)) {
          out_re[m] = fft.yRe[m];
          out_im[m] = fft.yIm[m];
        } else if ((m >= 1) && (m <= half - 1)) {
          out_re[m] = (2.0D * fft.yRe[m]);
          out_im[m] = (2.0D * fft.yIm[m]);
        } else {
          out_re[m] = 0.0D;
          out_im[m] = 0.0D;
        }
      }
      
      fft.ifft(signal.Length, out_re, out_im);
      
      return new CplxMatrix(1, signal.Length, new double[][] { fft.yRe }, new double[][] { fft.yIm });
    }
  }

}