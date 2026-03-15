#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion

namespace  Interfaces
{
 
  public interface IWorksheetFunction
  {

    // Properties
    _Application Application { get; }
    XlCreator Creator { get; }
    object Parent { get; }

    // Methods
    object _WSFunction(object Arg1, object Arg2, object Arg3, object Arg4, object Arg5, object Arg6, object Arg7, object Arg8, object Arg9, object Arg10, object Arg11, object Arg12, object Arg13, object Arg14, object Arg15, object Arg16, object Arg17, object Arg18, object Arg19, object Arg20, object Arg21, object Arg22, object Arg23, object Arg24, object Arg25, object Arg26, object Arg27, object Arg28, object Arg29, object Arg30);
    double Acos(double Arg1);
    double Acosh(double Arg1);
    bool And(object Arg1, object Arg2, object Arg3, object Arg4, object Arg5, object Arg6, object Arg7, object Arg8, object Arg9, object Arg10, object Arg11, object Arg12, object Arg13, object Arg14, object Arg15, object Arg16, object Arg17, object Arg18, object Arg19, object Arg20, object Arg21, object Arg22, object Arg23, object Arg24, object Arg25, object Arg26, object Arg27, object Arg28, object Arg29, object Arg30);
    string Asc(string Arg1);
    double Asin(double Arg1);
    double Asinh(double Arg1);
    double Atan2(double Arg1, double Arg2);
    double Atanh(double Arg1);
    double AveDev(object Arg1, object Arg2, object Arg3, object Arg4, object Arg5, object Arg6, object Arg7, object Arg8, object Arg9, object Arg10, object Arg11, object Arg12, object Arg13, object Arg14, object Arg15, object Arg16, object Arg17, object Arg18, object Arg19, object Arg20, object Arg21, object Arg22, object Arg23, object Arg24, object Arg25, object Arg26, object Arg27, object Arg28, object Arg29, object Arg30);
    double Average(object Arg1, object Arg2, object Arg3, object Arg4, object Arg5, object Arg6, object Arg7, object Arg8, object Arg9, object Arg10, object Arg11, object Arg12, object Arg13, object Arg14, object Arg15, object Arg16, object Arg17, object Arg18, object Arg19, object Arg20, object Arg21, object Arg22, object Arg23, object Arg24, object Arg25, object Arg26, object Arg27, object Arg28, object Arg29, object Arg30);
    string BahtText(double Arg1);
    double BetaDist(double Arg1, double Arg2, double Arg3, object Arg4, object Arg5);
    double BetaInv(double Arg1, double Arg2, double Arg3, object Arg4, object Arg5);
    double BinomDist(double Arg1, double Arg2, double Arg3, bool Arg4);
    double Ceiling(double Arg1, double Arg2);
    double ChiDist(double Arg1, double Arg2);
    double ChiInv(double Arg1, double Arg2);
    double ChiTest(object Arg1, object Arg2);
    object Choose(object Arg1, object Arg2, object Arg3, object Arg4, object Arg5, object Arg6, object Arg7, object Arg8, object Arg9, object Arg10, object Arg11, object Arg12, object Arg13, object Arg14, object Arg15, object Arg16, object Arg17, object Arg18, object Arg19, object Arg20, object Arg21, object Arg22, object Arg23, object Arg24, object Arg25, object Arg26, object Arg27, object Arg28, object Arg29, object Arg30);
    string Clean(string Arg1);
    double Combin(double Arg1, double Arg2);
    double Confidence(double Arg1, double Arg2, double Arg3);
    double Correl(object Arg1, object Arg2);
    double Cosh(double Arg1);
    double Count(object Arg1, object Arg2, object Arg3, object Arg4, object Arg5, object Arg6, object Arg7, object Arg8, object Arg9, object Arg10, object Arg11, object Arg12, object Arg13, object Arg14, object Arg15, object Arg16, object Arg17, object Arg18, object Arg19, object Arg20, object Arg21, object Arg22, object Arg23, object Arg24, object Arg25, object Arg26, object Arg27, object Arg28, object Arg29, object Arg30);
    double CountA(object Arg1, object Arg2, object Arg3, object Arg4, object Arg5, object Arg6, object Arg7, object Arg8, object Arg9, object Arg10, object Arg11, object Arg12, object Arg13, object Arg14, object Arg15, object Arg16, object Arg17, object Arg18, object Arg19, object Arg20, object Arg21, object Arg22, object Arg23, object Arg24, object Arg25, object Arg26, object Arg27, object Arg28, object Arg29, object Arg30);
    double CountBlank( Range Arg1);
    double CountIf( Range Arg1, object Arg2);
    double Covar(object Arg1, object Arg2);
    double CritBinom(double Arg1, double Arg2, double Arg3);
    double DAverage( Range Arg1, object Arg2, object Arg3);
    double Days360(object Arg1, object Arg2, object Arg3);
    double Db(double Arg1, double Arg2, double Arg3, double Arg4, object Arg5);
    string Dbcs(string Arg1);
    double DCount( Range Arg1, object Arg2, object Arg3);
    double DCountA( Range Arg1, object Arg2, object Arg3);
    double Ddb(double Arg1, double Arg2, double Arg3, double Arg4, object Arg5);
    double Degrees(double Arg1);
    double DevSq(object Arg1, object Arg2, object Arg3, object Arg4, object Arg5, object Arg6, object Arg7, object Arg8, object Arg9, object Arg10, object Arg11, object Arg12, object Arg13, object Arg14, object Arg15, object Arg16, object Arg17, object Arg18, object Arg19, object Arg20, object Arg21, object Arg22, object Arg23, object Arg24, object Arg25, object Arg26, object Arg27, object Arg28, object Arg29, object Arg30);
    object DGet( Range Arg1, object Arg2, object Arg3);
    double DMax( Range Arg1, object Arg2, object Arg3);
    double DMin( Range Arg1, object Arg2, object Arg3);
    string Dollar(double Arg1, object Arg2);
    double DProduct( Range Arg1, object Arg2, object Arg3);
    double DStDev( Range Arg1, object Arg2, object Arg3);
    double DStDevP( Range Arg1, object Arg2, object Arg3);
    double DSum( Range Arg1, object Arg2, object Arg3);
    double DVar( Range Arg1, object Arg2, object Arg3);
    double DVarP( Range Arg1, object Arg2, object Arg3);
    double Even(double Arg1);
    double ExponDist(double Arg1, double Arg2, bool Arg3);
    double Fact(double Arg1);
    double FDist(double Arg1, double Arg2, double Arg3);
    double Find(string Arg1, string Arg2, object Arg3);
    double FindB(string Arg1, string Arg2, object Arg3);
    double FInv(double Arg1, double Arg2, double Arg3);
    double Fisher(double Arg1);
    double FisherInv(double Arg1);
    string Fixed(double Arg1, object Arg2, object Arg3);
    double Floor(double Arg1, double Arg2);
    double Forecast(double Arg1, object Arg2, object Arg3);
    object Frequency(object Arg1, object Arg2);
    double FTest(object Arg1, object Arg2);
    double Fv(double Arg1, double Arg2, double Arg3, object Arg4, object Arg5);
    double GammaDist(double Arg1, double Arg2, double Arg3, bool Arg4);
    double GammaInv(double Arg1, double Arg2, double Arg3);
    double GammaLn(double Arg1);
    double GeoMean(object Arg1, object Arg2, object Arg3, object Arg4, object Arg5, object Arg6, object Arg7, object Arg8, object Arg9, object Arg10, object Arg11, object Arg12, object Arg13, object Arg14, object Arg15, object Arg16, object Arg17, object Arg18, object Arg19, object Arg20, object Arg21, object Arg22, object Arg23, object Arg24, object Arg25, object Arg26, object Arg27, object Arg28, object Arg29, object Arg30);
    object Growth(object Arg1, object Arg2, object Arg3, object Arg4);
    double HarMean(object Arg1, object Arg2, object Arg3, object Arg4, object Arg5, object Arg6, object Arg7, object Arg8, object Arg9, object Arg10, object Arg11, object Arg12, object Arg13, object Arg14, object Arg15, object Arg16, object Arg17, object Arg18, object Arg19, object Arg20, object Arg21, object Arg22, object Arg23, object Arg24, object Arg25, object Arg26, object Arg27, object Arg28, object Arg29, object Arg30);
    object HLookup(object Arg1, object Arg2, object Arg3, object Arg4);
    double HypGeomDist(double Arg1, double Arg2, double Arg3, double Arg4);
    object Index(object Arg1, double Arg2, object Arg3, object Arg4);
    double Intercept(object Arg1, object Arg2);
    double Ipmt(double Arg1, double Arg2, double Arg3, double Arg4, object Arg5, object Arg6);
    double Irr(object Arg1, object Arg2);
    bool IsErr(object Arg1);
    bool IsError(object Arg1);
    bool IsLogical(object Arg1);
    bool IsNA(object Arg1);
    bool IsNonText(object Arg1);
    bool IsNumber(object Arg1);
    double Ispmt(double Arg1, double Arg2, double Arg3, double Arg4);
    bool IsText(object Arg1);
    bool IsThaiDigit(string Arg1);
    double Kurt(object Arg1, object Arg2, object Arg3, object Arg4, object Arg5, object Arg6, object Arg7, object Arg8, object Arg9, object Arg10, object Arg11, object Arg12, object Arg13, object Arg14, object Arg15, object Arg16, object Arg17, object Arg18, object Arg19, object Arg20, object Arg21, object Arg22, object Arg23, object Arg24, object Arg25, object Arg26, object Arg27, object Arg28, object Arg29, object Arg30);
    double Large(object Arg1, double Arg2);
    object LinEst(object Arg1, object Arg2, object Arg3, object Arg4);
    double Ln(double Arg1);
    double Log(double Arg1, object Arg2);
    double Log10(double Arg1);
    object LogEst(object Arg1, object Arg2, object Arg3, object Arg4);
    double LogInv(double Arg1, double Arg2, double Arg3);
    double LogNormDist(double Arg1, double Arg2, double Arg3);
    object Lookup(object Arg1, object Arg2, object Arg3);
    double Match(object Arg1, object Arg2, object Arg3);
    double Max(object Arg1, object Arg2, object Arg3, object Arg4, object Arg5, object Arg6, object Arg7, object Arg8, object Arg9, object Arg10, object Arg11, object Arg12, object Arg13, object Arg14, object Arg15, object Arg16, object Arg17, object Arg18, object Arg19, object Arg20, object Arg21, object Arg22, object Arg23, object Arg24, object Arg25, object Arg26, object Arg27, object Arg28, object Arg29, object Arg30);
    double MDeterm(object Arg1);
    double Median(object Arg1, object Arg2, object Arg3, object Arg4, object Arg5, object Arg6, object Arg7, object Arg8, object Arg9, object Arg10, object Arg11, object Arg12, object Arg13, object Arg14, object Arg15, object Arg16, object Arg17, object Arg18, object Arg19, object Arg20, object Arg21, object Arg22, object Arg23, object Arg24, object Arg25, object Arg26, object Arg27, object Arg28, object Arg29, object Arg30);
    double Min(object Arg1, object Arg2, object Arg3, object Arg4, object Arg5, object Arg6, object Arg7, object Arg8, object Arg9, object Arg10, object Arg11, object Arg12, object Arg13, object Arg14, object Arg15, object Arg16, object Arg17, object Arg18, object Arg19, object Arg20, object Arg21, object Arg22, object Arg23, object Arg24, object Arg25, object Arg26, object Arg27, object Arg28, object Arg29, object Arg30);
    object MInverse(object Arg1);
    double MIrr(object Arg1, double Arg2, double Arg3);
    object MMult(object Arg1, object Arg2);
    double Mode(object Arg1, object Arg2, object Arg3, object Arg4, object Arg5, object Arg6, object Arg7, object Arg8, object Arg9, object Arg10, object Arg11, object Arg12, object Arg13, object Arg14, object Arg15, object Arg16, object Arg17, object Arg18, object Arg19, object Arg20, object Arg21, object Arg22, object Arg23, object Arg24, object Arg25, object Arg26, object Arg27, object Arg28, object Arg29, object Arg30);
    double NegBinomDist(double Arg1, double Arg2, double Arg3);
    double NormDist(double Arg1, double Arg2, double Arg3, bool Arg4);
    double NormInv(double Arg1, double Arg2, double Arg3);
    double NormSDist(double Arg1);
    double NormSInv(double Arg1);
    double NPer(double Arg1, double Arg2, double Arg3, object Arg4, object Arg5);
    double Npv(double Arg1, object Arg2, object Arg3, object Arg4, object Arg5, object Arg6, object Arg7, object Arg8, object Arg9, object Arg10, object Arg11, object Arg12, object Arg13, object Arg14, object Arg15, object Arg16, object Arg17, object Arg18, object Arg19, object Arg20, object Arg21, object Arg22, object Arg23, object Arg24, object Arg25, object Arg26, object Arg27, object Arg28, object Arg29, object Arg30);
    double Odd(double Arg1);
    bool Or(object Arg1, object Arg2, object Arg3, object Arg4, object Arg5, object Arg6, object Arg7, object Arg8, object Arg9, object Arg10, object Arg11, object Arg12, object Arg13, object Arg14, object Arg15, object Arg16, object Arg17, object Arg18, object Arg19, object Arg20, object Arg21, object Arg22, object Arg23, object Arg24, object Arg25, object Arg26, object Arg27, object Arg28, object Arg29, object Arg30);
    double Pearson(object Arg1, object Arg2);
    double Percentile(object Arg1, double Arg2);
    double PercentRank(object Arg1, double Arg2, object Arg3);
    double Permut(double Arg1, double Arg2);
    string Phonetic( Range Arg1);
    double Pi();
    double Pmt(double Arg1, double Arg2, double Arg3, object Arg4, object Arg5);
    double Poisson(double Arg1, double Arg2, bool Arg3);
    double Power(double Arg1, double Arg2);
    double Ppmt(double Arg1, double Arg2, double Arg3, double Arg4, object Arg5, object Arg6);
    double Prob(object Arg1, object Arg2, double Arg3, object Arg4);
    double Product(object Arg1, object Arg2, object Arg3, object Arg4, object Arg5, object Arg6, object Arg7, object Arg8, object Arg9, object Arg10, object Arg11, object Arg12, object Arg13, object Arg14, object Arg15, object Arg16, object Arg17, object Arg18, object Arg19, object Arg20, object Arg21, object Arg22, object Arg23, object Arg24, object Arg25, object Arg26, object Arg27, object Arg28, object Arg29, object Arg30);
    string Proper(string Arg1);
    double Pv(double Arg1, double Arg2, double Arg3, object Arg4, object Arg5);
    double Quartile(object Arg1, double Arg2);
    double Radians(double Arg1);
    double Rank(double Arg1,  Range Arg2, object Arg3);
    double Rate(double Arg1, double Arg2, double Arg3, object Arg4, object Arg5, object Arg6);
    string Replace(string Arg1, double Arg2, double Arg3, string Arg4);
    string ReplaceB(string Arg1, double Arg2, double Arg3, string Arg4);
    string Rept(string Arg1, double Arg2);
    string Roman(double Arg1, object Arg2);
    double Round(double Arg1, double Arg2);
    double RoundBahtDown(double Arg1);
    double RoundBahtUp(double Arg1);
    double RoundDown(double Arg1, double Arg2);
    double RoundUp(double Arg1, double Arg2);
    double RSq(object Arg1, object Arg2);
    object RTD(object progID, object server, object topic1, object topic2, object topic3, object topic4, object topic5, object topic6, object topic7, object topic8, object topic9, object topic10, object topic11, object topic12, object topic13, object topic14, object topic15, object topic16, object topic17, object topic18, object topic19, object topic20, object topic21, object topic22, object topic23, object topic24, object topic25, object topic26, object topic27, object topic28);
    double Search(string Arg1, string Arg2, object Arg3);
    double SearchB(string Arg1, string Arg2, object Arg3);
    double Sinh(double Arg1);
    double Skew(object Arg1, object Arg2, object Arg3, object Arg4, object Arg5, object Arg6, object Arg7, object Arg8, object Arg9, object Arg10, object Arg11, object Arg12, object Arg13, object Arg14, object Arg15, object Arg16, object Arg17, object Arg18, object Arg19, object Arg20, object Arg21, object Arg22, object Arg23, object Arg24, object Arg25, object Arg26, object Arg27, object Arg28, object Arg29, object Arg30);
    double Sln(double Arg1, double Arg2, double Arg3);
    double Slope(object Arg1, object Arg2);
    double Small(object Arg1, double Arg2);
    double Standardize(double Arg1, double Arg2, double Arg3);
    double StDev(object Arg1, object Arg2, object Arg3, object Arg4, object Arg5, object Arg6, object Arg7, object Arg8, object Arg9, object Arg10, object Arg11, object Arg12, object Arg13, object Arg14, object Arg15, object Arg16, object Arg17, object Arg18, object Arg19, object Arg20, object Arg21, object Arg22, object Arg23, object Arg24, object Arg25, object Arg26, object Arg27, object Arg28, object Arg29, object Arg30);
    double StDevP(object Arg1, object Arg2, object Arg3, object Arg4, object Arg5, object Arg6, object Arg7, object Arg8, object Arg9, object Arg10, object Arg11, object Arg12, object Arg13, object Arg14, object Arg15, object Arg16, object Arg17, object Arg18, object Arg19, object Arg20, object Arg21, object Arg22, object Arg23, object Arg24, object Arg25, object Arg26, object Arg27, object Arg28, object Arg29, object Arg30);
    double StEyx(object Arg1, object Arg2);
    string Substitute(string Arg1, string Arg2, string Arg3, object Arg4);
    double Subtotal(double Arg1,  Range Arg2, object Arg3, object Arg4, object Arg5, object Arg6, object Arg7, object Arg8, object Arg9, object Arg10, object Arg11, object Arg12, object Arg13, object Arg14, object Arg15, object Arg16, object Arg17, object Arg18, object Arg19, object Arg20, object Arg21, object Arg22, object Arg23, object Arg24, object Arg25, object Arg26, object Arg27, object Arg28, object Arg29, object Arg30);
    double Sum(object Arg1, object Arg2, object Arg3, object Arg4, object Arg5, object Arg6, object Arg7, object Arg8, object Arg9, object Arg10, object Arg11, object Arg12, object Arg13, object Arg14, object Arg15, object Arg16, object Arg17, object Arg18, object Arg19, object Arg20, object Arg21, object Arg22, object Arg23, object Arg24, object Arg25, object Arg26, object Arg27, object Arg28, object Arg29, object Arg30);
    double SumIf( Range Arg1, object Arg2, object Arg3);
    double SumProduct(object Arg1, object Arg2, object Arg3, object Arg4, object Arg5, object Arg6, object Arg7, object Arg8, object Arg9, object Arg10, object Arg11, object Arg12, object Arg13, object Arg14, object Arg15, object Arg16, object Arg17, object Arg18, object Arg19, object Arg20, object Arg21, object Arg22, object Arg23, object Arg24, object Arg25, object Arg26, object Arg27, object Arg28, object Arg29, object Arg30);
    double SumSq(object Arg1, object Arg2, object Arg3, object Arg4, object Arg5, object Arg6, object Arg7, object Arg8, object Arg9, object Arg10, object Arg11, object Arg12, object Arg13, object Arg14, object Arg15, object Arg16, object Arg17, object Arg18, object Arg19, object Arg20, object Arg21, object Arg22, object Arg23, object Arg24, object Arg25, object Arg26, object Arg27, object Arg28, object Arg29, object Arg30);
    double SumX2MY2(object Arg1, object Arg2);
    double SumX2PY2(object Arg1, object Arg2);
    double SumXMY2(object Arg1, object Arg2);
    double Syd(double Arg1, double Arg2, double Arg3, double Arg4);
    double Tanh(double Arg1);
    double TDist(double Arg1, double Arg2, double Arg3);
    string Text(object Arg1, string Arg2);
    string ThaiDayOfWeek(double Arg1);
    string ThaiDigit(string Arg1);
    string ThaiMonthOfYear(double Arg1);
    string ThaiNumSound(double Arg1);
    string ThaiNumString(double Arg1);
    double ThaiStringLength(string Arg1);
    double ThaiYear(double Arg1);
    double TInv(double Arg1, double Arg2);
    object Transpose(object Arg1);
    object Trend(object Arg1, object Arg2, object Arg3, object Arg4);
    string Trim(string Arg1);
    double TrimMean(object Arg1, double Arg2);
    double TTest(object Arg1, object Arg2, double Arg3, double Arg4);
    string USDollar(double Arg1, double Arg2);
    double Var(object Arg1, object Arg2, object Arg3, object Arg4, object Arg5, object Arg6, object Arg7, object Arg8, object Arg9, object Arg10, object Arg11, object Arg12, object Arg13, object Arg14, object Arg15, object Arg16, object Arg17, object Arg18, object Arg19, object Arg20, object Arg21, object Arg22, object Arg23, object Arg24, object Arg25, object Arg26, object Arg27, object Arg28, object Arg29, object Arg30);
    double VarP(object Arg1, object Arg2, object Arg3, object Arg4, object Arg5, object Arg6, object Arg7, object Arg8, object Arg9, object Arg10, object Arg11, object Arg12, object Arg13, object Arg14, object Arg15, object Arg16, object Arg17, object Arg18, object Arg19, object Arg20, object Arg21, object Arg22, object Arg23, object Arg24, object Arg25, object Arg26, object Arg27, object Arg28, object Arg29, object Arg30);
    double Vdb(double Arg1, double Arg2, double Arg3, double Arg4, double Arg5, object Arg6, object Arg7);
    object VLookup(object Arg1, object Arg2, object Arg3, object Arg4);
    double Weekday(object Arg1, object Arg2);
    double Weibull(double Arg1, double Arg2, double Arg3, bool Arg4);
    double ZTest(object Arg1, double Arg2, object Arg3);
  } // end of  IWorksheetFunction
}