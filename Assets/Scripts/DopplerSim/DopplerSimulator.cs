using System;
using Random = System.Random;
using DopplerSim.Tools;
using UnityEngine;

namespace DopplerSim
{
    public class DopplerSimulator
    {
        private int n_samples = 100;
        public int n_timepoints => 200;
        private double av_depth = 3.0D;
        public float SamplingDepth
        {
            get => (float)depth;
            set => depth = value;
        }
        private double depth;
        private double theta = 0.7853981633974483D;

        public float Angle
        {
            get => (float)(Mathf.Rad2Deg * theta);
            set => theta = value * Mathf.Deg2Rad;
        }
        private double vArt = 1.0D;
        public float ArterialVelocity
        {
            get => (float)vArt;
            set => vArt = value;
        }

        public float MaxVelocity // "Max Velocity: " + Math.round(max_vel * 10.0D) / 10.0D + " m/s"
        {
            get
            {
                if (theta < Math.PI/2.0D) // 1.5707963267948966D
                {
                    return (float)(PRF * 1540.0D / (4.0D * f0 * Math.Cos(theta)));
                }

                return Mathf.Infinity;
            }
        }

        public float Overlap
        {
            get => overlap;
            set => overlap = value;
        }

        private float overlap = 0f;

        public bool IsVelocityOverMax => vArt > MaxVelocity;
        
        private double vArtSD;
        private double vVein = 0.4D;
        private double vVeinSD;
        private double f0 = 4000000.0D;
        private double PRF = 20000.0D;

        public float PulseRepetitionFrequency
        {
            get => (float) PRF / 1000;
            set => PRF = value * 1000D;
        }
        
        public float MaxPRF => (1540.0f / (2.0f * ((float)depth * 7.0f / 100.0f)))  / 1000.0f; // "Max PRF: " + Math.round(PRFmax / 1000.0D) + " kHz

        // Plot1D pFreqF;
        // Plot1D pFreqR;
        // Plot1D pSampled;
        
        private double[][] timepoints;
        private MatrixPlot plotTime;

        public DopplerSimulator()
        {
            vArtSD = (0.1D * vArt);
            vVeinSD = (0.3D * vVein);
            depth = (av_depth / 7.0D + 0.0125D + 0.05D);

            //pSampled = new Plot1D(300, 100);
            //pFreqF = new Plot1D(300, 100);
            //pFreqR = new Plot1D(300, 100);
        }

        public Texture2D CreatePlot()
        {
            plotTime = new MatrixPlot(n_timepoints, n_samples);
            timepoints = MultiArray.New<Double>(n_timepoints, n_samples);
            for (int t = 0; t < n_timepoints; t++) {
                //Debug.Log(arterialPulse(1.0D, t));
                
                //timepoints[t] = generateDisplay(getVelocityComponents(depth), arterialPulse(1.0D, t));
                timepoints[t] = generateDisplay(new double[]{overlap, 0D, 1D}, arterialPulse(1.0D, t));
            }
            plotTime.setData(timepoints);
            return plotTime.texture;
        }

        // public void UpdatePlot(int timepoint)
        // {
        //     // do the generate Display on a separate thread
        //     plotTime.data[timepoint] = generateDisplay(getVelocityComponents(depth), arterialPulse(1.0D, timepoint));
        //     plotTime.setOneDataRow(timepoint);
        // }
        
        /// <summary>
        /// Instead of calculating the overlap based on the depth, get the values
        /// </summary>
        /// <param name="velocityComponents"> expects {art_overlap_total, ven_overlap_total, stationary}</param>
        public void UpdatePlot(int timepoint)
        {
            // do the generate Display on a separate thread
            plotTime.data[timepoint] = generateDisplay(new double[]{overlap, 0D, 1D}, arterialPulse(1.0D, timepoint));
            plotTime.setOneDataRow(timepoint);
        }
        
        // protected double[] getVelocityComponents(double depth) {
        //     double avpos = av_depth / 7.0D;
        //
        //     if (depth + 0.1D < avpos - 0.0125D - 0.1D)
        //         return new double[] { 0.0D, 0.0D, 1.0D };
        //     if (depth - 0.1D > avpos + 0.0125D + 0.1D) {
        //         return new double[] { 0.0D, 0.0D, 1.0D };
        //     }
        //     double ven_overlap1 = Math.Max(depth - 0.05D, avpos - 0.0125D - 0.1D);
        //     double ven_overlap2 = Math.Min(depth + 0.05D, avpos - 0.0125D);
        //     double ven_overlap_total = (ven_overlap2 - ven_overlap1) / 0.1D;
        //     if (ven_overlap_total < 0.0D) {
        //         ven_overlap_total = 0.0D;
        //     }
        //     double art_overlap1 = Math.Max(depth - 0.05D, avpos + 0.0125D);
        //     double art_overlap2 = Math.Min(depth + 0.05D, avpos + 0.0125D + 0.1D);
        //     double art_overlap_total = (art_overlap2 - art_overlap1) / 0.1D;
        //     if (art_overlap_total < 0.0D) {
        //         art_overlap_total = 0.0D;
        //     }
        //     double stationary = Math.Max(1.0D - ven_overlap_total - art_overlap_total, 0.0D);
        //
        //     return new double[] { art_overlap_total, ven_overlap_total, stationary };
        // }
        
        public static double arterialPulse(double f, double t) {
            double n = 13.0D;
            double phi = 0.3141592653589793D;

            double omega_t = 6.283185307179586D * f * t / 200.0D;
            double Q1 = Math.Pow(Math.Sin(omega_t), n);
            double Q2 = Math.Cos(omega_t - phi);

            return Q1 * Q2 / 0.4D;
        }
        
        protected double[] generateDisplay(double[] velComponents, double amplitude) {
            double[] samplesI = new double[n_samples];
            double[] samplesQ = new double[n_samples];
            double[] sampled_disp = new double[40];

            Random rand = new Random();
            double freq_ymax = 0.0D;

            for (int r = 0; r < 30; r++) {
                double vel;
                if (r < (int) Math.Round(velComponents[0] * 30.0D)) {
                    vel = vArt - Math.Abs(rand.NextGaussian()) * vArtSD;
                    if (vel < 0.0D)
                        vel = 0.0D;
                    vel *= amplitude;
                } else if ((r >= (int) Math.Round(velComponents[0] * 30.0D))
                           && (r < (int) Math.Round((velComponents[0] + velComponents[1]) * 30.0D))) {
                    vel = -(vVein - Math.Abs(rand.NextGaussian()) * vVeinSD);
                    if (vel > 0.0D) {
                        vel = 0.0D;
                    }
                } else {
                    vel = rand.NextGaussian() * 0.025D;
                }

                vel *= Math.Cos(theta);
                for (int i = 0; i < n_samples; i++) {
                    double[] IQ = sample(vel, i);
                    samplesI[i] += IQ[0];
                    samplesQ[i] += IQ[1];
                    if (i < 40)
                        sampled_disp[i] += samplesI[i];
                }
            }
            //pSampled.addData(sampled_disp);
            double sampl_max = FFTTools.max(sampled_disp);
            //pSampled.setYAxis(-sampl_max, sampl_max);

            double[] freqF = getFrequencies(samplesI, samplesQ, true);
            //pFreqF.addData(freqF);

            double[] freqR = getFrequencies(samplesI, samplesQ, false);
            //pFreqR.addData(freqR);

            freq_ymax = 188.49555921538757D;
            //pFreqF.setYAxis(0.0D, freq_ymax);
            //pFreqR.setYAxis(0.0D, freq_ymax);

            double[] frequencies = new double[n_samples];
            for (int i = 0; i < n_samples / 2; i++) {
                frequencies[i] = (freqR[(n_samples / 2 - i - 1)] / freq_ymax);
            }
            for (int i = n_samples / 2; i < n_samples; i++) {
                frequencies[i] = (freqF[(i - n_samples / 2)] / freq_ymax);
            }

            return frequencies;
        }
        
        private double[] sample(double vel, int i) {
            double I = 0.5D * Math.Cos(6.283185307179586D * f0 * 2.0D * vel / 1540.0D * i / PRF);
            double Q = 0.5D * Math.Sin(6.283185307179586D * f0 * 2.0D * vel / 1540.0D * i / PRF);

            return new double[] { I, Q };
        }
        
        private double[] getFrequencies(double[] I, double[] Q, bool forward) {
            double[] hh = FFTTools.hamming(n_samples);

            double[] Ih = FFTTools.tmult(I, hh);
            double[] Qh = FFTTools.tmult(Q, hh);

            CplxMatrix res = FFTTools.hilbert(Qh);

            double[] hQi = res.im[0];

            double[] demod;

            if (forward) {
                demod = FFTTools.minus(Ih, hQi);
            } else {
                demod = FFTTools.add(Ih, hQi);
            }

            FFT fft = new FFT(){};
            fft.fft(n_samples, demod, FFTTools.zeroes(n_samples));

            double[] abs = FFTTools.abs(fft.yRe, fft.yIm);

            int wall_thresh = 5;
            double wall = 0.8D / wall_thresh;
            double[] outMat = new double[n_samples / 2];
            for (int i = 0; i < n_samples / 2; i++) {
                if (i <= wall_thresh) {
                    outMat[i] = ((0.2D + wall * i) * abs[i]);
                } else {
                    outMat[i] = abs[i];
                }
            }

            return outMat;
        }
    }
}