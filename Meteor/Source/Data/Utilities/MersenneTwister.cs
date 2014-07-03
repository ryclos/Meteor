using System;
using System.Collections.Generic;
using System.Text;


/*--------------------------------------------------------
 * MersenneTwister.cs - file description
 * 
 * Version: 1.0
 * Author: Akihilo Kramot (Takel), Takuji Nishimura
 * Created: 7/9/2009 7:26:06 PM
 * 
 * Revisions:
 * -------------------------------------------------------*/

namespace Meteor.Source.Data
{
    public static class MersenneTwister
    {
        /* C# Version Copyright (C) 2001 Akihilo Kramot (Takel).       */
        /* C# porting from a C-program for MT19937, originaly coded by */
        /* Takuji Nishimura, considering the suggestions by            */
        /* Topher Cooper and Marc Rieffel in July-Aug. 1997.           */
        /* This library is free software under the Artistic license:   */
        /*                                                             */
        /* You can find the original C-program at                      */
        /*     http://www.math.keio.ac.jp/~matumoto/mt.html            */
        /*                                                             */

        #region Constants

        private const int N = 624;
        private const int M = 397;
        private const uint MATRIX_A = 0x9908b0df;
        private const uint UPPER_MASK = 0x80000000;
        private const uint LOWER_MASK = 0x7fffffff;
        private const uint TEMPERING_MASK_B = 0x9d2c5680;
        private const uint TEMPERING_MASK_C = 0xefc60000;

        #endregion
        #region Tempering parameters

        private static uint TEMPERING_SHIFT_U(uint y)
        {
            return (y >> 11);
        }
        private static uint TEMPERING_SHIFT_S(uint y)
        {
            return (y << 7);
        }
        private static uint TEMPERING_SHIFT_T(uint y)
        {
            return (y << 15);
        }
        private static uint TEMPERING_SHIFT_L(uint y)
        {
            return (y >> 18);
        }

        #endregion
        #region Variables

        private static object srMersenneTwisterSynchronizationObject = new object();
        private static uint[] mt = new uint[N];
        private static short mti = N + 1;
        private static uint[] mag01 = 
        {
            0x00000000, 
            MATRIX_A 
        };

        #endregion
        #region Functions

        public static void Seed(uint dwSeed)
        {
            lock (srMersenneTwisterSynchronizationObject)
            {
                mt[0] = dwSeed & 0xFFFFFFFF;
                for (mti = 1; mti < N; mti++)
                {
                    mt[mti] = (69069 * mt[mti - 1]) & 0xFFFFFFFF;
                }
            }
        }
        public static double NextDouble()
        {
            lock (srMersenneTwisterSynchronizationObject)
            {
                uint y;
                if (mti >= N)
                {
                    short kk;

                    if (mti == N + 1)
                    {
                        Seed(4357);
                    }

                    for (kk = 0; kk < N - M; kk++)
                    {
                        y = (mt[kk] & UPPER_MASK) | (mt[kk + 1] & LOWER_MASK);
                        mt[kk] = mt[kk + M] ^ (y >> 1) ^ mag01[y & 0x1];
                    }

                    for (; kk < N - 1; kk++)
                    {
                        y = (mt[kk] & UPPER_MASK) | (mt[kk + 1] & LOWER_MASK);
                        mt[kk] = mt[kk + (M - N)] ^ (y >> 1) ^ mag01[y & 0x1];
                    }

                    y = (mt[N - 1] & UPPER_MASK) | (mt[0] & LOWER_MASK);
                    mt[N - 1] = mt[M - 1] ^ (y >> 1) ^ mag01[y & 0x1];

                    mti = 0;
                }

                y = mt[mti++];
                y ^= TEMPERING_SHIFT_U(y);
                y ^= TEMPERING_SHIFT_S(y) & TEMPERING_MASK_B;
                y ^= TEMPERING_SHIFT_T(y) & TEMPERING_MASK_C;
                y ^= TEMPERING_SHIFT_L(y);
                return ((double)y / 0xFFFFFFFF);
            }
        }

        #endregion
    }
}
