using System;

namespace DopplerSim.Tools
{
  /// <summary>
  /// In java defined as a abstract class, but it is instantiated in FFTTools
  /// </summary>
    public class FFT
    {
        public static int MAXPRIMEFACTOR = 200;
        public static int MAXPRIMEFACTORDIV2 = (MAXPRIMEFACTOR + 1) / 2;
        public static int MAXFACTORCOUNT = 20;
        

        public static double c3_1 = Math.Cos(2.0943951023931953D) - 1.0D;
        public static double c3_2 = Math.Sin(2.0943951023931953D);
        
        public static double u5 = 1.2566370614359172D;
        public static double c5_1 = (Math.Cos(u5) + Math.Cos(2.0D * u5)) / 2.0D - 1.0D;
        public static double c5_2 = (Math.Cos(u5) - Math.Cos(2.0D * u5)) / 2.0D;
        public static double c5_3 = -Math.Sin(u5);
        public static double c5_4 = -(Math.Sin(u5) + Math.Sin(2.0D * u5));
        public static double c5_5 = Math.Sin(u5) - Math.Sin(2.0D * u5);
        public static double c8 = 1.0D / Math.Sqrt(2.0D);
        int[] sofar;
        int[] remain;
        double[] vRe;
        double[] vIm;
        double[] wRe;
        double[] wIm;
        public double[] xRe;
        public double[] xIm;
        public double[] yRe;
        public double[] yIm;
        
        public FFT() {

        }
        
        public int[] factorize(int n)
        {
          int nRadix = 6;
          int[] radices = new int[7];
          radices[1] = 2;
          radices[2] = 3;
          radices[3] = 4;
          radices[4] = 5;
          radices[5] = 8;
          radices[6] = 10;
          

          int[] factors = new int[MAXFACTORCOUNT];
          int j; 
          if (n == 1) {
            j = 1;
            factors[1] = 1;
          } else {
            j = 0;
          }
          int i = nRadix;
          
          while ((n > 1) && (i > 0)) {
            if (n % radices[i] == 0) {
              n /= radices[i];
              j++;
              factors[j] = radices[i];
            } else {
              i--;
            }
          }
          if (factors[j] == 2) {
            i = j - 1;
            while ((i > 0) && (factors[i] != 8)) i--;
            if (i > 0) {
              factors[j] = 4;
              factors[i] = 4;
            }
          }
          if (n > 1) {
            for (int k = 2; k < Math.Sqrt(n) + 1.0D; k++) {
              while (n % k == 0) {
                n /= k;
                j++;
                factors[j] = k;
              }
            }
            if (n > 1) {
              j++;
              factors[j] = n;
            }
          }
          int[] fact = new int[j + 1];
          for (i = 1; i <= j; i++) {
            fact[i] = factors[(j - i + 1)];
          }
          
          return fact; }
        
        double[] twiddleRe;
        double[] twiddleIm;
        
        public int[] transTableSetup(int nPoints) { int[] actual = factorize(nPoints);
          int nFact = actual.Length - 1;
          if (actual[1] > MAXPRIMEFACTOR) {
            Console.Write("Prime factor of FFT length too large: " + actual[1]);
            Console.Write("\nPlease modify the value of MAXPRIMEFACTOR\n");
            throw new ArgumentException();
          }
          remain[0] = nPoints;
          sofar[1] = 1;
          remain[1] = (nPoints / actual[1]);
          for (int i = 2; i <= nFact; i++) {
            sofar[i] = (sofar[(i - 1)] * actual[(i - 1)]);
            remain[i] = (remain[(i - 1)] / actual[i]);
          }
          
          return actual; }
        
        double[] trigRe;
        
        public void permute(int nPoint, int nFact, int[] fact, int[] remain) { int[] count = new int[nFact + 1];
          for (int i = 1; i <= nFact; i++) count[i] = 0;
          int k = 0;
          for (int i = 0; i <= nPoint - 2; i++) {
            yRe[i] = xRe[k];
            yIm[i] = xIm[k];
            int j = 1;
            k += remain[j];
            count[1] += 1;
            while (count[j] >= fact[j]) {
              count[j] = 0;
              k = k - remain[(j - 1)] + remain[(j + 1)];
              j++;
              count[j] += 1;
            }
          }
          yRe[(nPoint - 1)] = xRe[(nPoint - 1)];
          yIm[(nPoint - 1)] = xIm[(nPoint - 1)]; }
        
        double[] trigIm;
        
        public void initTrig(int radix) { trigRe = new double[radix];
          trigIm = new double[radix];
          double w = 6.283185307179586D / radix;
          trigRe[0] = 1.0D;trigIm[0] = 0.0D;
          double xre = Math.Cos(w);
          double xim = -Math.Sin(w);
          trigRe[1] = xre;trigIm[1] = xim;
          for (int i = 2; i < radix; i++) {
            trigRe[i] = (xre * trigRe[(i - 1)] - xim * trigIm[(i - 1)]);
            trigIm[i] = (xim * trigRe[(i - 1)] + xre * trigIm[(i - 1)]); } }
        
        double[] zRe;
        double[] zIm;
        double[] aRe;
        
        public void fft_41() { double t1_re = aRe[0] + aRe[2];double t1_im = aIm[0] + aIm[2];
          double t2_re = aRe[1] + aRe[3];double t2_im = aIm[1] + aIm[3];
          
          double m2_re = aRe[0] - aRe[2];double m2_im = aIm[0] - aIm[2];
          double m3_re = aIm[1] - aIm[3];double m3_im = aRe[3] - aRe[1];
          
          aRe[0] = (t1_re + t2_re);aIm[0] = (t1_im + t2_im);
          aRe[2] = (t1_re - t2_re);aIm[2] = (t1_im - t2_im);
          aRe[1] = (m2_re + m3_re);aIm[1] = (m2_im + m3_im);
          aRe[3] = (m2_re - m3_re);aIm[3] = (m2_im - m3_im); }
        
        double[] aIm;
        double[] bRe;
        double[] bIm;
        public void fft_42() { double t1_re = bRe[0] + bRe[2];double t1_im = bIm[0] + bIm[2];
          double t2_re = bRe[1] + bRe[3];double t2_im = bIm[1] + bIm[3];
          
          double m2_re = bRe[0] - bRe[2];double m2_im = bIm[0] - bIm[2];
          double m3_re = bIm[1] - bIm[3];double m3_im = bRe[3] - bRe[1];
          
          bRe[0] = (t1_re + t2_re);bIm[0] = (t1_im + t2_im);
          bRe[2] = (t1_re - t2_re);bIm[2] = (t1_im - t2_im);
          bRe[1] = (m2_re + m3_re);bIm[1] = (m2_im + m3_im);
          bRe[3] = (m2_re - m3_re);bIm[3] = (m2_im - m3_im);
        }
        
        public void fft_51() {
          double t1_re = aRe[1] + aRe[4];double t1_im = aIm[1] + aIm[4];
          double t2_re = aRe[2] + aRe[3];double t2_im = aIm[2] + aIm[3];
          double t3_re = aRe[1] - aRe[4];double t3_im = aIm[1] - aIm[4];
          double t4_re = aRe[3] - aRe[2];double t4_im = aIm[3] - aIm[2];
          double t5_re = t1_re + t2_re;double t5_im = t1_im + t2_im;
          aRe[0] += t5_re;aIm[0] += t5_im;
          double m1_re = c5_1 * t5_re;double m1_im = c5_1 * t5_im;
          double m2_re = c5_2 * (t1_re - t2_re);double m2_im = c5_2 * (t1_im - t2_im);
          
          double m3_re = -c5_3 * (t3_im + t4_im);double m3_im = c5_3 * (t3_re + t4_re);
          double m4_re = -c5_4 * t4_im;double m4_im = c5_4 * t4_re;
          double m5_re = -c5_5 * t3_im;double m5_im = c5_5 * t3_re;
          
          double s3_re = m3_re - m4_re;double s3_im = m3_im - m4_im;
          double s5_re = m3_re + m5_re;double s5_im = m3_im + m5_im;
          double s1_re = aRe[0] + m1_re;double s1_im = aIm[0] + m1_im;
          double s2_re = s1_re + m2_re;double s2_im = s1_im + m2_im;
          double s4_re = s1_re - m2_re;double s4_im = s1_im - m2_im;
          
          aRe[1] = (s2_re + s3_re);aIm[1] = (s2_im + s3_im);
          aRe[2] = (s4_re + s5_re);aIm[2] = (s4_im + s5_im);
          aRe[3] = (s4_re - s5_re);aIm[3] = (s4_im - s5_im);
          aRe[4] = (s2_re - s3_re);aIm[4] = (s2_im - s3_im);
        }
        
        public void fft_52() {
          double t1_re = bRe[1] + bRe[4];double t1_im = bIm[1] + bIm[4];
          double t2_re = bRe[2] + bRe[3];double t2_im = bIm[2] + bIm[3];
          double t3_re = bRe[1] - bRe[4];double t3_im = bIm[1] - bIm[4];
          double t4_re = bRe[3] - bRe[2];double t4_im = bIm[3] - bIm[2];
          double t5_re = t1_re + t2_re;double t5_im = t1_im + t2_im;
          bRe[0] += t5_re;bIm[0] += t5_im;
          double m1_re = c5_1 * t5_re;double m1_im = c5_1 * t5_im;
          double m2_re = c5_2 * (t1_re - t2_re);double m2_im = c5_2 * (t1_im - t2_im);
          
          double m3_re = -c5_3 * (t3_im + t4_im);double m3_im = c5_3 * (t3_re + t4_re);
          double m4_re = -c5_4 * t4_im;double m4_im = c5_4 * t4_re;
          double m5_re = -c5_5 * t3_im;double m5_im = c5_5 * t3_re;
          
          double s3_re = m3_re - m4_re;double s3_im = m3_im - m4_im;
          double s5_re = m3_re + m5_re;double s5_im = m3_im + m5_im;
          double s1_re = bRe[0] + m1_re;double s1_im = bIm[0] + m1_im;
          double s2_re = s1_re + m2_re;double s2_im = s1_im + m2_im;
          double s4_re = s1_re - m2_re;double s4_im = s1_im - m2_im;
          
          bRe[1] = (s2_re + s3_re);bIm[1] = (s2_im + s3_im);
          bRe[2] = (s4_re + s5_re);bIm[2] = (s4_im + s5_im);
          bRe[3] = (s4_re - s5_re);bIm[3] = (s4_im - s5_im);
          bRe[4] = (s2_re - s3_re);bIm[4] = (s2_im - s3_im);
        }
        
        public void fft_8() {
          aRe = new double[4];
          aIm = new double[4];
          bRe = new double[4];
          bIm = new double[4];
          
          aRe[0] = zRe[0];bRe[0] = zRe[1];
          aRe[1] = zRe[2];bRe[1] = zRe[3];
          aRe[2] = zRe[4];bRe[2] = zRe[5];
          aRe[3] = zRe[6];bRe[3] = zRe[7];
          
          aIm[0] = zIm[0];bIm[0] = zIm[1];
          aIm[1] = zIm[2];bIm[1] = zIm[3];
          aIm[2] = zIm[4];bIm[2] = zIm[5];
          aIm[3] = zIm[6];bIm[3] = zIm[7];
          
          fft_41();fft_42();
          
          double gem = c8 * (bRe[1] + bIm[1]);
          bIm[1] = (c8 * (bIm[1] - bRe[1]));
          bRe[1] = gem;
          gem = bIm[2];
          bIm[2] = (-bRe[2]);
          bRe[2] = gem;
          gem = c8 * (bIm[3] - bRe[3]);
          bIm[3] = (-c8 * (bRe[3] + bIm[3]));
          bRe[3] = gem;
          
          zRe[0] = (aRe[0] + bRe[0]);zRe[4] = (aRe[0] - bRe[0]);
          zRe[1] = (aRe[1] + bRe[1]);zRe[5] = (aRe[1] - bRe[1]);
          zRe[2] = (aRe[2] + bRe[2]);zRe[6] = (aRe[2] - bRe[2]);
          zRe[3] = (aRe[3] + bRe[3]);zRe[7] = (aRe[3] - bRe[3]);
          
          zIm[0] = (aIm[0] + bIm[0]);zIm[4] = (aIm[0] - bIm[0]);
          zIm[1] = (aIm[1] + bIm[1]);zIm[5] = (aIm[1] - bIm[1]);
          zIm[2] = (aIm[2] + bIm[2]);zIm[6] = (aIm[2] - bIm[2]);
          zIm[3] = (aIm[3] + bIm[3]);zIm[7] = (aIm[3] - bIm[3]);
        }
        
        public void fft_10() {
          aRe = new double[5];
          aIm = new double[5];
          bRe = new double[5];
          bIm = new double[5];
          
          aRe[0] = zRe[0];bRe[0] = zRe[5];
          aRe[1] = zRe[2];bRe[1] = zRe[7];
          aRe[2] = zRe[4];bRe[2] = zRe[9];
          aRe[3] = zRe[6];bRe[3] = zRe[1];
          aRe[4] = zRe[8];bRe[4] = zRe[3];
          
          aIm[0] = zIm[0];bIm[0] = zIm[5];
          aIm[1] = zIm[2];bIm[1] = zIm[7];
          aIm[2] = zIm[4];bIm[2] = zIm[9];
          aIm[3] = zIm[6];bIm[3] = zIm[1];
          aIm[4] = zIm[8];bIm[4] = zIm[3];
          
          fft_51();fft_52();
          
          zRe[0] = (aRe[0] + bRe[0]);zRe[5] = (aRe[0] - bRe[0]);
          zRe[6] = (aRe[1] + bRe[1]);zRe[1] = (aRe[1] - bRe[1]);
          zRe[2] = (aRe[2] + bRe[2]);zRe[7] = (aRe[2] - bRe[2]);
          zRe[8] = (aRe[3] + bRe[3]);zRe[3] = (aRe[3] - bRe[3]);
          zRe[4] = (aRe[4] + bRe[4]);zRe[9] = (aRe[4] - bRe[4]);
          
          zIm[0] = (aIm[0] + bIm[0]);zIm[5] = (aIm[0] - bIm[0]);
          zIm[6] = (aIm[1] + bIm[1]);zIm[1] = (aIm[1] - bIm[1]);
          zIm[2] = (aIm[2] + bIm[2]);zIm[7] = (aIm[2] - bIm[2]);
          zIm[8] = (aIm[3] + bIm[3]);zIm[3] = (aIm[3] - bIm[3]);
          zIm[4] = (aIm[4] + bIm[4]);zIm[9] = (aIm[4] - bIm[4]);
        }
        
        public void fft_odd(int radix) {
          int n = radix;
          int max = (n + 1) / 2;
          vRe = new double[max];
          vIm = new double[max];
          wRe = new double[max];
          wIm = new double[max];
          
          for (int j = 1; j < max; j++) {
            vRe[j] = (zRe[j] + zRe[(n - j)]);
            vIm[j] = (zIm[j] - zIm[(n - j)]);
            wRe[j] = (zRe[j] - zRe[(n - j)]);
            wIm[j] = (zIm[j] + zIm[(n - j)]);
          }
          
          for (int j = 1; j < max; j++) {
            zRe[j] = zRe[0];
            zIm[j] = zIm[0];
            zRe[(n - j)] = zRe[0];
            zIm[(n - j)] = zIm[0];
            int k = j;
            for (int i = 1; i < max; i++) {
              double rere = trigRe[k] * vRe[i];
              double imim = trigIm[k] * vIm[i];
              double reim = trigRe[k] * wIm[i];
              double imre = trigIm[k] * wRe[i];
              
              zRe[(n - j)] += rere + imim;
              zIm[(n - j)] += reim - imre;
              zRe[j] += rere - imim;
              zIm[j] += reim + imre;
              
              k += j;
              if (k >= n) k -= n;
            }
          }
          for (int j = 1; j < max; j++) {
            zRe[0] += vRe[j];
            zIm[0] += wIm[j];
          }
        }
        
        public void twiddleTransf(int sofarRadix, int radix, int remainRadix) {
          initTrig(radix);
          double omega = 6.283185307179586D / (sofarRadix * radix);
          double Cosw = Math.Cos(omega);
          double sinw = -Math.Sin(omega);
          double tw_re = 1.0D;
          double tw_im = 0.0D;
          int dataOffset = 0;
          int groupOffset = dataOffset;
          int adr = groupOffset;
          
          twiddleRe = new double[radix];
          twiddleIm = new double[radix];
          
          zRe = new double[radix];
          zIm = new double[radix];
          

          for (int dataNo = 0; dataNo < sofarRadix; dataNo++) {
            if (sofarRadix > 1) {
              twiddleRe[0] = 1.0D;
              twiddleIm[0] = 0.0D;
              twiddleRe[1] = tw_re;
              twiddleIm[1] = tw_im;
              for (int twNo = 2; twNo < radix; twNo++) {
                twiddleRe[twNo] = (tw_re * twiddleRe[(twNo - 1)] - tw_im * twiddleIm[(twNo - 1)]);
                twiddleIm[twNo] = (tw_im * twiddleRe[(twNo - 1)] + tw_re * twiddleIm[(twNo - 1)]);
              }
              double gem = Cosw * tw_re - sinw * tw_im;
              tw_im = sinw * tw_re + Cosw * tw_im;
              tw_re = gem;
            }
            for (int groupNo = 0; groupNo < remainRadix; groupNo++) {
              if ((sofarRadix > 1) && (dataNo > 0)) {
                zRe[0] = yRe[adr];
                zIm[0] = yIm[adr];
                int blockNo = 1;
                do {
                  adr += sofarRadix;
                  zRe[blockNo] = (twiddleRe[blockNo] * yRe[adr] - twiddleIm[blockNo] * yIm[adr]);
                  
                  zIm[blockNo] = (twiddleRe[blockNo] * yIm[adr] + twiddleIm[blockNo] * yRe[adr]);
                  

                  blockNo++;
                } while (blockNo < radix);
              } else {
                for (int blockNo = 0; blockNo < radix; blockNo++) {
                  zRe[blockNo] = yRe[adr];
                  zIm[blockNo] = yIm[adr];
                  adr += sofarRadix;
                }
              }
              if (radix == 2) {
                double gem = zRe[0] + zRe[1];
                zRe[1] = (zRe[0] - zRe[1]);zRe[0] = gem;
                gem = zIm[0] + zIm[1];
                zIm[1] = (zIm[0] - zIm[1]);zIm[0] = gem;
              } else if (radix == 3) {
                double t1_re = zRe[1] + zRe[2];double t1_im = zIm[1] + zIm[2];
                zRe[0] += t1_re;zIm[0] += t1_im;
                double m1_re = c3_1 * t1_re;double m1_im = c3_1 * t1_im;
                double m2_re = c3_2 * (zIm[1] - zIm[2]);
                double m2_im = c3_2 * (zRe[2] - zRe[1]);
                double s1_re = zRe[0] + m1_re;double s1_im = zIm[0] + m1_im;
                zRe[1] = (s1_re + m2_re);zIm[1] = (s1_im + m2_im);
                zRe[2] = (s1_re - m2_re);zIm[2] = (s1_im - m2_im);
              } else if (radix == 4) {
                double t1_re = zRe[0] + zRe[2];double t1_im = zIm[0] + zIm[2];
                double t2_re = zRe[1] + zRe[3];double t2_im = zIm[1] + zIm[3];
                
                double m2_re = zRe[0] - zRe[2];double m2_im = zIm[0] - zIm[2];
                double m3_re = zIm[1] - zIm[3];double m3_im = zRe[3] - zRe[1];
                
                zRe[0] = (t1_re + t2_re);zIm[0] = (t1_im + t2_im);
                zRe[2] = (t1_re - t2_re);zIm[2] = (t1_im - t2_im);
                zRe[1] = (m2_re + m3_re);zIm[1] = (m2_im + m3_im);
                zRe[3] = (m2_re - m3_re);zIm[3] = (m2_im - m3_im);
              } else if (radix == 5) {
                double t1_re = zRe[1] + zRe[4];double t1_im = zIm[1] + zIm[4];
                double t2_re = zRe[2] + zRe[3];double t2_im = zIm[2] + zIm[3];
                double t3_re = zRe[1] - zRe[4];double t3_im = zIm[1] - zIm[4];
                double t4_re = zRe[3] - zRe[2];double t4_im = zIm[3] - zIm[2];
                double t5_re = t1_re + t2_re;double t5_im = t1_im + t2_im;
                zRe[0] += t5_re;zIm[0] += t5_im;
                double m1_re = c5_1 * t5_re;double m1_im = c5_1 * t5_im;
                double m2_re = c5_2 * (t1_re - t2_re);
                double m2_im = c5_2 * (t1_im - t2_im);
                
                double m3_re = -c5_3 * (t3_im + t4_im);
                double m3_im = c5_3 * (t3_re + t4_re);
                double m4_re = -c5_4 * t4_im;double m4_im = c5_4 * t4_re;
                double m5_re = -c5_5 * t3_im;double m5_im = c5_5 * t3_re;
                
                double s3_re = m3_re - m4_re;double s3_im = m3_im - m4_im;
                double s5_re = m3_re + m5_re;double s5_im = m3_im + m5_im;
                double s1_re = zRe[0] + m1_re;double s1_im = zIm[0] + m1_im;
                double s2_re = s1_re + m2_re;double s2_im = s1_im + m2_im;
                double s4_re = s1_re - m2_re;double s4_im = s1_im - m2_im;
                
                zRe[1] = (s2_re + s3_re);zIm[1] = (s2_im + s3_im);
                zRe[2] = (s4_re + s5_re);zIm[2] = (s4_im + s5_im);
                zRe[3] = (s4_re - s5_re);zIm[3] = (s4_im - s5_im);
                zRe[4] = (s2_re - s3_re);zIm[4] = (s2_im - s3_im);
              } else if (radix == 8) {
                fft_8();
              } else if (radix == 10) {
                fft_10();
              } else {
                fft_odd(radix);
              }
              adr = groupOffset;
              for (int blockNo = 0; blockNo < radix; blockNo++) {
                yRe[adr] = zRe[blockNo];yIm[adr] = zIm[blockNo];
                adr += sofarRadix;
              }
              groupOffset += sofarRadix * radix;
              adr = groupOffset;
            }
            dataOffset += 1;
            groupOffset = dataOffset;
            adr = groupOffset;
          }
        }
        
        public void fft(int n, double[] xRe, double[] xIm) {
          this.xRe = xRe;
          this.xIm = xIm;
          
          yRe = new double[xRe.Length];
          yIm = new double[xIm.Length];
          
          remain = new int[MAXFACTORCOUNT];
          sofar = new int[MAXFACTORCOUNT];
          
          int[] actualRadix = transTableSetup(n);
          permute(n, actualRadix.Length - 1, actualRadix, remain);
          
          for (int count = 1; count <= actualRadix.Length - 1; count++) {
            twiddleTransf(sofar[count], actualRadix[count], remain[count]);
          }
        }
        
        public void ifft(int n, double[] xRe, double[] xIm)
        {
          fft(n, xRe, neg(xIm));
          divconj(n, yRe, yIm);
        }
        
        public double[] neg(double[] pos)
        {
          double[] neg = new double[pos.Length];
          for (int i = 0; i < pos.Length; i++) {
            neg[i] = (-pos[i]);
          }
          return neg;
        }
        
        public void divconj(int n, double[] xRe, double[] xIm) {
          for (int i = 0; i < n; i++) {
            yRe[i] = (xRe[i] / n);
            yIm[i] = (-xIm[i] / n);
          }
        }
    }
}