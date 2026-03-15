#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Syncfusion.Windows.Chart
{
    /// <summary>
    /// Represents UtilityFunctions class
    /// </summary>
    public class UtilityFunctions
    {
        #region GAMMA FUNCTION RELATED CONSTANTS
        private static double[] gammaCoefs = { 76.18009172947146, -86.50532032941677,
                                           24.01409824083091, -1.231739572450155,
                                           0.1208650973866179e-2, -0.5395239384953e-5};
        private static double gamma = 5.0d;
        private static double sqrt2Pi = Math.Sqrt(2 * Math.PI);
        private static double epsilon_GammaSer = 1.0e-7; // the accuracy of cumulative gamma function calculation used during series summing
        private static int max_ItGammaSer = 1000;// maximum iteration count in cumulative gamma function evaluation.
        private static double tiny_gamma = 1.0e-77;
        #endregion

        #region BETA FUNCTION RELATED CONSTANTS
        private static double epsilon_BetaSer = 1.0e-16;
        private static double epsilon_BetaInverse = epsilon_BetaSer;
        private static int max_ItBetaSer = 1000;
        private static int max_ItBetaBrent = 1000;
        private static double tiny_beta = 1.0e-99;
        #endregion

        #region FACTORIAL RELATED CONSTANTS
        private static double[] factrl = { 1, 1, 2, 6, 24, 120, 720, 5040, 40320, 362880, 3628800, 39916800, 479001600, 6227020800, 87178291200, 1307674368000, 20922789888000, 355687428096000, 6.402373705728E+15, 1.21645100408832E+17, 2.43290200817664E+18, 5.10909421717094E+19, 1.12400072777761E+21, 2.5852016738885E+22, 6.20448401733239E+23, 1.5511210043331E+25, 4.03291461126606E+26, 1.08888694504184E+28, 3.04888344611714E+29, 8.8417619937397E+30, 2.65252859812191E+32, 8.22283865417792E+33, 2.63130836933694E+35, 8.68331761881189E+36, 2.95232799039604E+38, 1.03331479663861E+40, 3.71993326789901E+41, 1.37637530912263E+43, 5.23022617466601E+44, 2.03978820811974E+46, 8.15915283247898E+47, 3.34525266131638E+49, 1.40500611775288E+51, 6.04152630633738E+52, 2.65827157478845E+54, 1.1962222086548E+56, 5.50262215981209E+57, 2.58623241511168E+59, 1.24139155925361E+61, 6.08281864034268E+62, 3.04140932017134E+64, 1.55111875328738E+66, 8.06581751709439E+67, 4.27488328406003E+69, 2.30843697339241E+71, 1.26964033536583E+73, 7.10998587804863E+74, 4.05269195048772E+76, 2.35056133128288E+78, 1.3868311854569E+80, 8.32098711274139E+81, 5.07580213877225E+83, 3.14699732603879E+85, 1.98260831540444E+87, 1.26886932185884E+89, 8.24765059208247E+90, 5.44344939077443E+92, 3.64711109181887E+94, 2.48003554243683E+96, 1.71122452428141E+98, 1.19785716699699E+100, 8.50478588567862E+101, 6.12344583768861E+103, 4.47011546151268E+105, 3.30788544151939E+107, 2.48091408113954E+109, 1.88549470166605E+111, 1.45183092028286E+113, 1.13242811782063E+115, 8.94618213078297E+116, 7.15694570462638E+118, 5.79712602074737E+120, 4.75364333701284E+122, 3.94552396972066E+124, 3.31424013456535E+126, 2.81710411438055E+128, 2.42270953836727E+130, 2.10775729837953E+132, 1.85482642257398E+134, 1.65079551609085E+136, 1.48571596448176E+138, 1.3520015276784E+140, 1.24384140546413E+142, 1.15677250708164E+144, 1.08736615665674E+146, 1.03299784882391E+148, 9.91677934870949E+149, 9.61927596824821E+151, 9.42689044888324E+153, 9.33262154439441E+155 };
        private static double[] factrlLn = { 0, 0, 0.693147180559945, 1.79175946922805, 3.17805383034795, 4.78749174278205, 6.5792512120101, 8.52516136106541, 10.6046029027453, 12.8018274800815, 15.1044125730755, 17.5023078458739, 19.9872144956619, 22.5521638531234, 25.1912211827387, 27.8992713838409, 30.6718601060807, 33.5050734501369, 36.3954452080331, 39.3398841871995, 42.3356164607535, 45.3801388984769, 48.4711813518352, 51.6066755677644, 54.7847293981123, 58.0036052229805, 61.261701761002, 64.5575386270063, 67.8897431371815, 71.257038967168, 74.6582363488302, 78.0922235533153, 81.557959456115, 85.0544670175815, 88.5808275421977, 92.1361756036871, 95.7196945421432, 99.3306124547874, 102.968198614514, 106.631760260643, 110.320639714757, 114.034211781462, 117.771881399745, 121.533081515439, 125.317271149357, 129.123933639127, 132.952575035616, 136.802722637326, 140.673923648234, 144.565743946345, 148.477766951773, 152.409592584497, 156.360836303079, 160.331128216631, 164.320112263195, 168.327445448428, 172.352797139163, 176.395848406997, 180.456291417544, 184.53382886145, 188.628173423672, 192.739047287845, 196.86618167289, 201.009316399282, 205.168199482641, 209.342586752537, 213.532241494563, 217.736934113954, 221.95644181913, 226.190548323728, 230.439043565777, 234.701723442818, 238.978389561834, 243.268849002983, 247.572914096187, 251.890402209723, 256.22113555001, 260.564940971863, 264.921649798553, 269.29109765102, 273.673124285694, 278.067573440366, 282.47429268763, 286.893133295427, 291.32395009427, 295.766601350761, 300.220948647014, 304.686856765669, 309.164193580147, 313.652829949879, 318.152639620209, 322.663499126726, 327.185287703775, 331.717887196929, 336.261181979199, 340.815058870799, 345.379407062267, 349.95411804077, 354.539085519441, 359.134205369576 };
        #endregion

        #region NORMAL DISTR CONSTANTS
        private static double[] a_inv_norm = { -3.969683028665376e+01,
                                          2.209460984245205e+02,
                                          -2.759285104469687e+02,
                                          1.383577518672690e+02,
                                          -3.066479806614716e+01,
                                          2.506628277459239e+00 };

        private static double[] b_inv_norm = {
                                          -5.447609879822406e+01,
                                          1.615858368580409e+02,
                                          -1.556989798598866e+02,
                                          6.680131188771972e+01,
                                          -1.328068155288572e+01
                                        };

        private static double[] c_inv_norm = {
                                          -7.784894002430293e-03,
                                          -3.223964580411365e-01,
                                          -2.400758277161838e+00,
                                          -2.549732539343734e+00,
                                          4.374664141464968e+00,
                                          2.938163982698783e+00
                                        };

        private static double[] d_inv_norm = {
                                          7.784695709041462e-03,
                                          3.224671290700398e-01,
                                          2.445134137142996e+00,
                                          3.754408661907416e+00
                                        };

        private static double p_low_inv_norm = 0.02425;
        private static double p_high_inv_norm = 1.0 - p_low_inv_norm;
        #endregion

        #region UTILITY FUNCTIONS
        /// <summary>
        /// Natural logarithm of gamma function ( for y > 0 ).
        /// </summary>
        /// <param name="y"></param>
        public static double GammaLn(double y)
        {
            Debug.Assert(y > 0, " Gamma function ( GammaLn( double y ) ) argument is less or equal zero. ");
            double temp = (y + gamma + 0.5d);

            double x = y;

            double sum = 1.000000000190015d;
            for (int i = 0; i < gammaCoefs.Length; i++)
                sum += gammaCoefs[i] / (++x);

            return (y + 0.5) * Math.Log(temp) - temp + Math.Log(sqrt2Pi * sum / y);
        }

        /// <summary>
        /// Gamma function ( for y > 0 ).
        /// </summary>
        /// <param name="y"></param>
        public static double Gamma(double y)
        {
            return Math.Exp(GammaLn(y));
        }

        /// <summary>
        /// Factorial n! ( for n >= 0 ).
        /// </summary>
        /// <param name="n"></param>
        public static double Factorial(int n)
        {
            Debug.Assert(n >= 0, " Factorial ( Factorial( int n ) ) argument is less or equal zero. ");
            if (n < factrl.Length)
                return factrl[n];
            else
                return Math.Exp(GammaLn((double)(n + 1)));
        }

        /// <summary>
        /// Logarithm of factorial n! ( for n >= 0 ).
        /// </summary>
        /// <param name="n"></param>
        public static double FactorialLn(int n)
        {
            Debug.Assert(n >= 0, " Factorial ( FactorialLn( int n ) ) argument is less or equal zero. ");
            if (n < factrlLn.Length)
                return factrlLn[n];
            else
                return GammaLn((double)(n + 1));
        }

        /// <summary>
        /// Binomial coefficient n!/(k!(n-k)!) ( for n >= k >= 0 ).
        /// </summary>
        /// <param name="n"></param>
        /// <param name="k"></param>
        public static double Binomial(int n, int k)
        {
            return Math.Floor(0.5 + Math.Exp(FactorialLn(n) - FactorialLn(k) - FactorialLn(n - k)));
        }

        /// <summary>
        /// Logarithm of Beta function.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static double BetaLn(double a, double b)
        {
            return GammaLn(a) + GammaLn(b) - GammaLn(a + b);
        }

        /// <summary>
        /// Beta function.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public static double Beta(double a, double b)
        {
            return Math.Exp(BetaLn(a, b));
        }
        #endregion

        #region DISTRIBUTION FUNCTIONS

        #region NORMAL DISTRIBUTION
        /// <summary>
        /// Returns Normal Distribution density function.
        /// </summary>
        /// <param name="x">Value at which the distribution density is evaluated.</param>
        /// <param name="m">Expected value of distribution (Mean value)</param>
        /// <param name="sigma">Variance of distribution</param>
        /// <returns></returns>
        public static double NormalDistributionDensity(double x, double m, double sigma)
        {
            if (sigma <= 0)
                throw new ArgumentException("Standart deviation sigma in NormalDistributionDensity should be sigma > 0.", "sigma - standart deviation.");

            return Math.Exp(NormalDistributionDensityLn(x, m, sigma));
        }
        /// <summary>
        /// Returns Logarithm of Normal Distribution density function.
        /// </summary>
        /// <param name="x">Value at which the distribution density is evaluated.</param>
        /// <param name="m">Expected value of distribution (Mean value)</param>
        /// <param name="sigma">Variance of distribution</param>
        /// <returns></returns>
        public static double NormalDistributionDensityLn(double x, double m, double sigma)
        {
            if (sigma <= 0)
                throw new ArgumentException("Standart deviation sigma in NormalDistributionDensityLn should be sigma > 0.", "sigma - standart deviation.");

            return -(x - m) * (x - m) / (2 * sigma * sigma) - Math.Log(sigma * Math.Sqrt(2 * Math.PI));
        }

        /// <summary>
        /// Error function.
        /// </summary>
        /// <param name="x">.</param>
        /// <returns>Returns error function.</returns>
        public static double Erf(double x)
        {
            return x < 0.0 ? -GammaCumulativeDistribution(0.5d, x * x) : GammaCumulativeDistribution(0.5d, x * x);
        }
        /// <summary>
        /// Inverse Normal Distribution function.
        /// This is rational approximation of Normal Distribution function.
        /// The absolute value of the relative error is less than 1.15·10-9 in the entire region.
        /// Lower tail quantile for standard normal distribution function.
        /// This function returns an approximation of the inverse cumulative
        /// standard normal distribution function.  I.e., given P, it returns
        /// an approximation to the X satisfying P = Pr{Z is smaller than X} where Z is a
        /// random variable from the standard normal distribution.
        /// </summary>
        /// <param name="p">Probability at which function is evaluated. p must be in ( 0,1 ) range. </param>
        /// <returns> Returns Inverse cumulative distribution.</returns>
        public static double InverseNormalDistribution(double p)
        {
            Debug.Assert((p > 0.0) && (p < 1.0), " Bad value of p in InverseNormalDistribution( double p ).( p must be in 0 < p < 1 range. )");
            if ((p <= 0.0) && (p >= 1.0))
                throw new ArgumentException("Probaility p in InverseNormalDistribution should be in range (0,1).", "p - probability.");
            double x, q;
            if ((0 < p) && (p < p_low_inv_norm))
            {
                q = Math.Sqrt(-2 * Math.Log(p));
                x = (((((c_inv_norm[0] * q + c_inv_norm[1]) * q + c_inv_norm[2]) * q + c_inv_norm[3]) * q + c_inv_norm[4]) * q + c_inv_norm[5]) / ((((d_inv_norm[0] * q + d_inv_norm[1]) * q + d_inv_norm[2]) * q + d_inv_norm[3]) * q + 1);
            }
            else

                if ((p_low_inv_norm <= p) && (p <= p_high_inv_norm))
                {
                    q = p - 0.5;
                    double r = q * q;
                    x = (((((a_inv_norm[0] * r + a_inv_norm[1]) * r + a_inv_norm[2]) * r + a_inv_norm[3]) * r + a_inv_norm[4]) * r + a_inv_norm[5]) * q / (((((b_inv_norm[0] * r + b_inv_norm[1]) * r + b_inv_norm[2]) * r + b_inv_norm[3]) * r + b_inv_norm[4]) * r + 1);
                }
                else
                ////if( p_high_inv_norm < p < 1)
                {
                    q = Math.Sqrt(-2 * Math.Log(1 - p));
                    x = -(((((c_inv_norm[0] * q + c_inv_norm[1]) * q + c_inv_norm[2]) * q + c_inv_norm[3]) * q + c_inv_norm[4]) * q + c_inv_norm[5]) / ((((d_inv_norm[0] * q + d_inv_norm[1]) * q + d_inv_norm[2]) * q + d_inv_norm[3]) * q + 1);
                }
            return x;
        }
        /// <summary>
        /// Normal Distribution function.
        /// </summary>
        /// <param name="x">Value at which the distribution is evaluated.</param>
        /// <returns> Returns cumulative distribution. ( Returns probability that normally distributed random variable (X - mean)/sigma is smaller than x.).</returns>
        public static double NormalDistribution(double x)
        {
            return 0.5 * (1 + Erf(1.41421356237309505 * x));
        }
        /// <summary>
        /// Inverse Error function.
        /// This is rational approximation of erf function.
        /// The absolute value of the relative error is less than 1.15·10-9 in the entire region.
        /// </summary>
        /// <param name="x">value x is in (-1 , 1) range. </param>
        /// <returns>Returns Value that corresponds to given x. </returns>
        public static double InverseErf(double x)
        {
            return (1.0 / 1.41421356237309505) * InverseNormalDistribution(0.5 * (x + 1));
        }
        #endregion

        #region GAMMA DISTRIBUTION
        /// <summary>
        /// Returns cumulative gamma distribution. http://en.wikipedia.org/wiki/Gamma_distribution
        /// ( for x >= 0, a > 0 )
        /// </summary>
        /// <param name="a"></param>
        /// <param name="x"></param>
        /// <returns>Returns cumulative gamma distribution. (http://en.wikipedia.org/wiki/Gamma_distribution) ( for x >= 0, a > 0 )</returns>
        public static double GammaCumulativeDistribution(double a, double x)
        {
            Debug.Assert((x >= 0) && (a > 0), " Incorrect parameters in GammaCumulativeDistribution( double a, double x ). ");
            if ((a <= 0.0))
                throw new ArgumentException("Parameter a in GammaCumulativeDistribution should be a > 0.", "a");
            if ((x < 0.0))
                throw new ArgumentException("Parameter x in GammaCumulativeDistribution should be x >= 0.", "x");

            if (x < (a + 1.0))
                return GammaCumulativeS(a, x);
            else
                return 1.0 - GammaCumulativeCF(a, x);
        }
        #endregion

        #region BETA DISTRIBUTION

        /// <summary>
        /// Returns cumulative beta distribution. 
        /// ( for x >= 0, a > 0, b > 0 )
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="x"></param>
        /// <returns>Returns cumulative beta distribution. http://en.wikipedia.org/wiki/Beta_distribution) ( for x >= 0, a > 0, b > 0 )</returns>
        public static double BetaCumulativeDistribution(double a, double b, double x)
        {
            Debug.Assert((x >= 0) && (x <= 1) && (a > 0) && (b > 0), " Incorrect parameters in BetaCumulativeDistribution( double a, double x ). ");
            if ((x < 0.0) || (x > 1.0))
                throw new ArgumentException("Parameter x in BetaCumulativeDistribution should be in range [0,1].", "x");
            if ((a <= 0.0))
                throw new ArgumentException("Parameter a in BetaCumulativeDistribution should be a > 0.", "a");
            if ((b <= 0.0))
                throw new ArgumentException("Parameter b in BetaCumulativeDistribution should be b > 0.", "b");

            if (x == 1.0) return 1.0;
            if (x == 0.0) return 0.0;
            double swtc = (a + 1) / (a + b + 2);
            double coeff = Math.Exp(-BetaLn(a, b) - Math.Log(a) - Math.Log(b) + a * Math.Log(x) + b * Math.Log(1 - x));
            if (x < swtc)
            {
                return b * coeff * BtaCumulativeCF(a, b, x);
            }
            else
            {
                return (1.0 - a * coeff * BtaCumulativeCF(b, a, 1 - x));
            }
        }

        /// <summary>
        /// Returns inverse cumulative beta distribution. 
        /// ( for 1 >= p >= 0 , a > 0, b > 0 )
        /// </summary>
        /// <param name="a">Beta function parameter </param>
        /// <param name="b">Beta function parameter</param>
        /// <param name="p">Probability</param>
        /// <returns>Returns inverse cumulative beta distribution. http://en.wikipedia.org/wiki/Beta_distribution) ( for p in [0,1], a > 0, b > 0 )</returns>
        public static double InverseBetaCumulativeDistribution(double a, double b, double p)
        {
            Debug.Assert((p >= 0) && (p <= 1) && (a > 0) && (b > 0), " Incorrect parameters in BetaCumulativeDistribution( double a, double b,  double x ). ");
            if ((p < 0.0) && (p > 1.0))
                throw new ArgumentException("Probaility p in InverseBetaCumulativeDistribution should be in range [0,1].", "p - probability.");
            if ((a <= 0.0))
                throw new ArgumentException("Parameter a in InverseBetaCumulativeDistribution should be a > 0.", "a");
            if ((b <= 0.0))
                throw new ArgumentException("Parameter b in InverseBetaCumulativeDistribution should be b > 0.", "b");

            if (p == 1.0) return 1.0;
            if (p == 0.0) return 0.0;
            return InverseBtaCumulativeBrent(a, b, p, 0.0, 1.0, epsilon_BetaInverse);
        }
        #endregion

        #region STUDENT DISTRIBUTION

        /// <summary>
        /// Returns cumulative T distribution. 
        /// ( for degreeOfFreedom > 0 )
        /// </summary>
        /// <param name="tValue"></param>
        /// <param name="degreeOfFreedom"></param>
        /// <param name="oneTail"></param>
        /// <returns>Returns T cumulative distribution. http://en.wikipedia.org/wiki/T_distribution) ( for degreeOfFreedom > 0 )</returns>
        public static double TCumulativeDistribution(double tValue, double degreeOfFreedom, bool oneTail)
        {
            if ((degreeOfFreedom <= 0.0))
                throw new ArgumentException("Parameter degreeOfFreedom in TCumulativeDistribution should be degreeOfFreedom > 0.", "degreeOfFreedom");

            double x = 1 / (1 + tValue * tValue / degreeOfFreedom);
            if (oneTail)
            {
                if (tValue > 0)
                    return 1 - 0.5 * BetaCumulativeDistribution(((double)degreeOfFreedom) / 2, 0.5, x);
                else
                    return 0.5 * BetaCumulativeDistribution(((double)degreeOfFreedom) / 2, 0.5, x);
            }
            else
            {
                return 1 - BetaCumulativeDistribution(((double)degreeOfFreedom) / 2, 0.5, x);
            }
        }

        /// <summary>
        /// Inverse cumulative T distribution. 
        /// ( for degreeOfFreedom > 0 )
        /// </summary>
        /// <param name="p">Probability (must be in range [0, 1]. )</param>
        /// <param name="degreeOfFreedom"></param>
        /// <param name="oneTail"></param>
        /// <returns>Inverse T cumulative distribution. http://en.wikipedia.org/wiki/T_distribution) ( for degreeOfFreedom > 0 )</returns>
        public static double InverseTCumulativeDistribution(double p, double degreeOfFreedom, bool oneTail)
        {
            if ((degreeOfFreedom <= 0.0))
                throw new ArgumentException("Parameter degreeOfFreedom in InverseTCumulativeDistribution should be degreeOfFreedom > 0.", "degreeOfFreedom");
            if ((p <= 0.0) && (p >= 1.0))
                throw new ArgumentException("Probaility p in InverseTCumulativeDistribution should be in range (0,1).", "p - probability.");

            double x = 0.0;
            if (oneTail)
            {
                if (p > 0.5)
                {
                    x = InverseBetaCumulativeDistribution(((double)degreeOfFreedom) / 2, 0.5, 2 * (1 - p));
                    return Math.Sqrt(degreeOfFreedom * (1 / x - 1));
                }
                else
                {
                    x = InverseBetaCumulativeDistribution(((double)degreeOfFreedom) / 2, 0.5, 2 * p);
                    return -Math.Sqrt(degreeOfFreedom * (1 / x - 1));
                }
            }
            else
            {
                x = InverseBetaCumulativeDistribution(((double)degreeOfFreedom) / 2, 0.5, 1 - p);
                return Math.Sqrt(degreeOfFreedom * (1 / x - 1));
            }
        }
        #endregion

        #region F DISTRIBUTION

        /// <summary>
        /// Returns cumulative F distribution. 
        /// ( for firstDegreeOfFreedom >= 1 and firstDegreeOfFreedom >= 1 )
        /// </summary>
        /// <param name="fValue"></param>
        /// <param name="firstDegreeOfFreedom"></param>
        /// <param name="secondDegreeOfFreedom"></param>
        /// <returns>Returns T cumulative distribution. http://en.wikipedia.org/wiki/F_distribution) ( for degreeOfFreedom > 0 )</returns>
        public static double FCumulativeDistribution(double fValue, double firstDegreeOfFreedom, double secondDegreeOfFreedom)
        {
            Debug.Assert(fValue >= 0, " Wrong f value in  FCumulativeDistribution(double fValue, int firstDegreeOfFreedom, int secondDegreeOfFreedom)");
            if ((firstDegreeOfFreedom <= 0.0))
                throw new ArgumentException("Parameter firstDegreeOfFreedom in FCumulativeDistribution should be firstDegreeOfFreedom > 0.", "firstDegreeOfFreedom");
            if ((secondDegreeOfFreedom <= 0.0))
                throw new ArgumentException("Parameter secondDegreeOfFreedom in FCumulativeDistribution should be secondDegreeOfFreedom > 0.", "secondDegreeOfFreedom");
            if ((fValue < 0.0))
                throw new ArgumentException("Probaility fValue in FCumulativeDistribution should be in range [0,infinity).", "fValue");

            if (fValue == 0.0) return 1.0;
            double x = secondDegreeOfFreedom / (secondDegreeOfFreedom + fValue * firstDegreeOfFreedom);
            return BetaCumulativeDistribution(((double)secondDegreeOfFreedom) / 2, ((double)firstDegreeOfFreedom) / 2, x);
        }

        /// <summary>
        /// Inverse cumulative F distribution. 
        /// ( for firstDegreeOfFreedom >= 1 and firstDegreeOfFreedom >= 1 )
        /// </summary>
        /// <param name="p">Probability (must be in range [0, 1]. )</param>
        /// <param name="firstDegreeOfFreedom"></param>
        /// <param name="secondDegreeOfFreedom"></param>
        /// <returns>Inverse F cumulative distribution. http://en.wikipedia.org/wiki/F_distribution) ( for firstDegreeOfFreedom >= 1 and firstDegreeOfFreedom >= 1 )</returns>
        public static double InverseFCumulativeDistribution(double p, double firstDegreeOfFreedom, double secondDegreeOfFreedom)
        {
            if ((firstDegreeOfFreedom <= 0.0))
                throw new ArgumentException("Parameter firstDegreeOfFreedom in InverseFCumulativeDistribution should be firstDegreeOfFreedom > 0.", "firstDegreeOfFreedom");
            if ((secondDegreeOfFreedom <= 0.0))
                throw new ArgumentException("Parameter secondDegreeOfFreedom in InverseFCumulativeDistribution should be secondDegreeOfFreedom > 0.", "secondDegreeOfFreedom");
            if ((p <= 0.0) && (p > 1.0))
                throw new ArgumentException("Probaility p in InverseFCumulativeDistribution should be in range (0,1].", "p - probability.");

            if (p == 1.0) return 0.0;
            if (p == 0.0) return double.PositiveInfinity;
            double x = InverseBetaCumulativeDistribution(((double)secondDegreeOfFreedom) / 2, ((double)firstDegreeOfFreedom) / 2, p);
            return ((double)secondDegreeOfFreedom / (double)firstDegreeOfFreedom) * (1 / x - 1);
        }
        #endregion
        #endregion

        #region HELPING FUNCTIONS
        /// <summary>
        /// Gammas the cumulative S.
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="x">The x.</param>
        /// <returns></returns>
        private static double GammaCumulativeS(double a, double x)
        {
            if (x < 0.0)
            {
                Debug.Assert(false, " x < 0 in GammaCumulativeSeries( double a, double x ) ");
                return 0.0;
            }
            else
            {
                double sum = 1 / a;
                double ta = a, sm = sum;
                for (int n = 1; (n <= max_ItGammaSer) && (Math.Abs(sum * epsilon_GammaSer) < Math.Abs(sm)); n++)
                {
                    ta++;
                    sm *= x / ta;
                    sum += sm;
                }
                return sum * Math.Exp(-x + a * Math.Log(x) - GammaLn(a));
            }
        }

        /// <summary>
        /// Gammas the cumulative CF.
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="x">The x.</param>
        /// <returns></returns>
        private static double GammaCumulativeCF(double a, double x)
        {
            if (x < 0.0)
            {
                Debug.Assert(false, " x < 0 in GammaCumulativeCF( double a, double x ) ");
                return 0.0;
            }
            else
            {
                double b_x = x + 1 - a, f_x = tiny_gamma, c_x = f_x, d_x = 0.0, a_x = 1.0;

                for (int i = 1; (i <= max_ItGammaSer); i++)
                {
                    d_x = b_x + a_x * d_x;
                    if (Math.Abs(d_x) < tiny_gamma) d_x = tiny_gamma;
                    c_x = b_x + a_x / c_x;
                    if (Math.Abs(c_x) < tiny_gamma) c_x = tiny_gamma;

                    d_x = 1 / d_x;
                    double delta_x = c_x * d_x;
                    f_x *= delta_x;

                    if ((Math.Abs(delta_x - 1) < epsilon_GammaSer)) break;
                    b_x += 2;
                    a_x = -i * (i - a);
                }

                return f_x * Math.Exp(-x + a * Math.Log(x) - GammaLn(a));
            }
        }

        /// <summary>
        /// Btas the cumulative CF.
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="b">The b.</param>
        /// <param name="x">The x.</param>
        /// <returns></returns>
        private static double BtaCumulativeCF(double a, double b, double x)
        {
            if (x < 0.0 || x > 1.0)
            {
                Debug.Assert(false, " x < 0 or x > 1.0 in BtaCumulativeCF( double a, double x ) ");
                return 0.0;
            }
            else
            {
                double b_x = 1.0, f_x = tiny_beta, c_x = f_x, d_x = 0.0, a_x = 1.0;

                for (int i = 1; (i <= max_ItBetaSer); i++)
                {
                    d_x = b_x + a_x * d_x;
                    if (Math.Abs(d_x) < tiny_beta) d_x = tiny_beta;
                    c_x = b_x + a_x / c_x;
                    if (Math.Abs(c_x) < tiny_beta) c_x = tiny_beta;

                    d_x = 1 / d_x;
                    double delta_x = c_x * d_x;
                    f_x *= delta_x;

                    if ((Math.Abs(delta_x - 1) < epsilon_BetaSer)) break;

                    int m = i / 2;
                    double am = a + m;
                    double a2m = am + m;
                    if (i % 2 == 1)
                        a_x = -am * (am + b) * x / (a2m * (a2m + 1));
                    else
                        a_x = m * (b - m) * x / (a2m * (a2m - 1));
                }

                return f_x;
            }
        }

        /// <summary>
        /// Inverses the bta cumulative brent.
        /// </summary>
        /// <param name="aa">The aa.</param>
        /// <param name="bb">The bb.</param>
        /// <param name="prblty">The prblty.</param>
        /// <param name="x1">The x1.</param>
        /// <param name="x2">The x2.</param>
        /// <param name="tol">The tol.</param>
        /// <returns></returns>
        private static double InverseBtaCumulativeBrent(double aa, double bb, double prblty, double x1, double x2, double tol)
        {
            double a = x1, b = x2, c = x2, d = 0.0, e = 0.0, min1, min2;
            double fa = BetaCumulativeDistribution(aa, bb, a) - prblty, fb = BetaCumulativeDistribution(aa, bb, b) - prblty, fc, p, q, r, s, tol1, xm;
            Debug.Assert((fa > 0.0 && fb < 0.0) || (fa < 0.0 && fb > 0.0), " The probability should be in [0,1] range. double InverseBtaCumulativeBrent( double aa, double bb, double p, double x1, double x2, double tol ) ");

            fc = fb;
            for (int i = 1; i <= max_ItBetaBrent; i++)
            {
                if ((fb > 0.0 && fc > 0.0) || (fb < 0.0 && fc < 0.0))
                {
                    c = a;
                    fc = fa;
                    e = d = b - a;
                }
                if (Math.Abs(fc) < Math.Abs(fb))
                {
                    a = b;
                    b = c;
                    c = a;
                    fa = fb;
                    fb = fc;
                    fc = fa;
                }
                tol1 = 2.0 * epsilon_BetaSer * Math.Abs(b) + 0.5 * tol;
                xm = 0.5 * (c - b);
                if (Math.Abs(xm) <= tol1 || fb == 0.0) return b;
                if (Math.Abs(e) >= tol1 && Math.Abs(fa) > Math.Abs(fb))
                {
                    s = fb / fa;
                    if (a == c)
                    {
                        p = 2.0 * xm * s;
                        q = 1.0 - s;
                    }
                    else
                    {
                        q = fa / fc;
                        r = fb / fc;
                        p = s * (2.0 * xm * q * (q - r) - (b - a) * (r - 1.0));
                        q = (q - 1.0) * (r - 1.0) * (s - 1.0);
                    }
                    if (p > 0.0) q = -q;
                    p = Math.Abs(p);
                    min1 = 3.0 * xm * q - Math.Abs(tol1 * q);
                    min2 = Math.Abs(e * q);
                    if (2.0 * p < (min1 < min2 ? min1 : min2))
                    {
                        e = d;
                        d = p / q;
                    }
                    else
                    {////bisection
                        d = xm;
                        e = d;
                    }
                }
                else
                {////bisection
                    d = xm;
                    e = d;
                }
                a = b;
                fa = fb;
                b += d;
                fb = BetaCumulativeDistribution(aa, bb, b) - prblty;
            }
            return 0.0;
        }
        #endregion
    }
}
