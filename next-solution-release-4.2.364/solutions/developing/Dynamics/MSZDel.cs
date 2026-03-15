using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.IO;
using System.IO.Compression;


namespace MSZ
{
    sealed class MSZDel
    {
        internal static uint SglAuthent([Out] UInt32[] AuthentCode)
        {
            Random random = new Random();
            UInt32[] RandNum = new UInt32[2];
            UInt32[] AppRandNum = new UInt32[2];
            UInt32[] LibRandNum = new UInt32[2];
            UInt32[] AuthentCodeLocal = new UInt32[8];
            UInt32[] AuthentCodeLast = new UInt32[4];
            UInt32 RetCode;
            uint i;

            for (i = 0; i < 8; i++)
            {
                AuthentCodeLocal[i] = AuthentCode[i];
                AuthentCode[i] = ((uint)random.Next() << 16) | (uint)random.Next();
            }

            random = new Random((int)AuthentCode[0]);
            RandNum[0] = (UInt32)random.Next();
            RandNum[1] = (UInt32)random.Next();

            AppRandNum[0] = RandNum[0];
            AppRandNum[1] = RandNum[1];

            try
            {
                RetCode = MSZDll.SglAuthentA(AuthentCodeLocal, AppRandNum, LibRandNum);
            }
            catch
            {
                return MSZDll.SGL_AUTHENTICATION_FAILED;
            }

            for (i = 0; i < 8; i++)
            {
                AuthentCode[i] = AuthentCodeLocal[i];
            }
            if (RetCode != MSZDll.SGL_SUCCESS)
            {
                return MSZDll.SGL_AUTHENTICATION_FAILED;
            }

            for (i = 0; i < 4; i++)
            {
                AuthentCodeLast[i] = AuthentCode[i + 8];
            }
            SglTeaEncipher(RandNum, RandNum, AuthentCodeLast);

            if ((RandNum[0] != AppRandNum[0]) ||
                (RandNum[1] != AppRandNum[1]))
            {
                return MSZDll.SGL_AUTHENTICATION_FAILED;
            }

            SglTeaEncipher(LibRandNum, LibRandNum, AuthentCodeLast);



            try
            {
                RetCode = MSZDll.SglAuthentB(LibRandNum);
            }
            catch
            {
                return MSZDll.SGL_AUTHENTICATION_FAILED;
            } 
            

            if (RetCode != MSZDll.SGL_SUCCESS)
            {
                return MSZDll.SGL_AUTHENTICATION_FAILED;
            }

            return MSZDll.SGL_SUCCESS;

        }

        internal static void SglTeaEncipher([In] UInt32[] InData, [Out] UInt32[] OutData, [In] UInt32[] Key)
        {

            UInt32 y = InData[0];
            UInt32 z = InData[1];
            UInt32 sum = 0;
            UInt32 delta = 0x9E3779B9;
            UInt32 a = Key[0];
            UInt32 b = Key[1];
            UInt32 c = Key[2];
            UInt32 d = Key[3];
            UInt32 n = 32;

            while (n-- > 0)
            {
                sum += delta;
                y += (z << 4) + a ^ z + sum ^ (z >> 5) + b;
                z += (y << 4) + c ^ y + sum ^ (y >> 5) + d;
            }

            OutData[0] = y;
            OutData[1] = z;

        }



        internal static void SglTeaDecipher([In] UInt32[] InData, [Out] UInt32[] OutData, [In] UInt32[] Key)
        {

            UInt32 y = InData[0];
            UInt32 z = InData[1];
            UInt32 sum = 0xC6EF3720;
            UInt32 delta = 0x9E3779B9;
            UInt32 a = Key[0];
            UInt32 b = Key[1];
            UInt32 c = Key[2];
            UInt32 d = Key[3];
            UInt32 n = 32;

            /* sum = delta<<5, in general sum = delta * n */

            while (n-- > 0)
            {
                z -= (y << 4) + c ^ y + sum ^ (y >> 5) + d;
                y -= (z << 4) + a ^ z + sum ^ (z >> 5) + b;
                sum -= delta;
            }

            OutData[0] = y;
            OutData[1] = z;

        }

    }
    internal class MSZDelRead
    {
        internal enum ApplicationType : uint
        {
            apMovicon, apMovTrace, apMoviconBA
        }

        #region Protected
        static UInt32[] AuthentCode = new UInt32[12] { 
                                            0xDA6C0B83,
	                                        0xEE7302FF,
	                                        0x5799F879,
	                                        0xA479F53B,
	                                        0xFFAF25F8,
	                                        0x351BA05A,
	                                        0xFE2734D8,
	                                        0x29CAD6A8,
	                                        0x42D19F80,
	                                        0xE0E8BA30,
	                                        0x78F00412,
	                                        0xF5027BEA  };

        static uint[] _ProductId = new uint[1] { uint.MaxValue };
        static uint[] _SerialNumber = new uint[1];
        static UInt32[] _ConfigData = new UInt32[8];
        static uint _rc;
        #endregion

        static MSZDelRead()
        {
            ReadInit();
            if (CheckKey())
            {
                ReadConfig();
            }
        }
        static bool ReadInit()
        {
            // check the linked SG-Lock library
            _rc = MSZDel.SglAuthent(AuthentCode);
            if (_rc != MSZDll.SGL_SUCCESS)
            {
                return false;
            }

            if (_ProductId[0] == uint.MaxValue)
            {
                // read the ProductId from attached SG-Lock
                _rc = MSZDll.SglReadProductId(_ProductId);
                if (_rc != MSZDll.SGL_SUCCESS)
                {
                    return false;
                }
            }

            // test if a SG-Lock is plugged in (just for demonstration - not really 
            // necessary here, because SglReadProduct does the same ... )
            _rc = MSZDll.SglSearchLock(_ProductId[0]);
            if (_rc != MSZDll.SGL_SUCCESS)
            {
                return false;
            }

            // let's read the serial number of the SG-Lock
            _rc = MSZDll.SglReadSerialNumber(_ProductId[0], _SerialNumber);
            if (_rc != MSZDll.SGL_SUCCESS)
            {
                return false;
            }
            return true;

        }
        static bool CheckKey()
        {
            // let's process a CRA (challenge response authentication) using key 
            // with index 0 in the SG-Lock
            UInt32[] ClearData = new UInt32[2];
            UInt32[] CryptDataLock = new UInt32[2];
            UInt32[] CryptDataApp = new UInt32[2];
            UInt32[] LockKeyNo_0 = new UInt32[4];

            // generate a 64-bit random number (build with two 32-bit random numbers)
            Random random = new Random();
            ClearData[0] = (UInt32)random.Next();
            ClearData[1] = (UInt32)random.Next();

#if DEBUG
            Console.WriteLine("\nCRA random number: " + ClearData[0] + " " + ClearData[1]);
#endif

            // copy the random number to encryption buffers for the SG-Lock and the application
            CryptDataLock[0] = ClearData[0];
            CryptDataLock[1] = ClearData[1];

            CryptDataApp[0] = ClearData[0];
            CryptDataApp[1] = ClearData[1];

            try
            {
                // let the SG-Lock encrypt the 64-bit data block with its key index 0
                _rc = MSZDll.SglCryptLock(_ProductId[0], 1, 0, 1, CryptDataLock);
            }
            catch
            {
                _rc = MSZDll.SGL_DGL_NOT_FOUND;
            }

            if (_rc == MSZDll.SGL_SUCCESS)
            {
                //  Console.WriteLine("\nSglCryptLock: Data encrypted!");
            }
            else
            {
#if DEBUG
                Console.WriteLine("\nSglCryptLock: Error no. rc=" + _rc);
#endif
                return false;
            }

            // Console.WriteLine("\n  Lock result: " + CryptDataLock[0] + " " + CryptDataLock[1]);

            // copy of the (assumed) 128-bit index 0 in Demo SG-Lock (if not changed by user) 
            LockKeyNo_0[0] = 0xE3D9E3FF;// 0xD94B6C2B;
            LockKeyNo_0[1] = 0x49EBB33E;// 0x17E88CEF;
            LockKeyNo_0[2] = 0xEDFCBE4D;// 0xDADBCF1D;
            LockKeyNo_0[3] = 0x60270ACF;// 0x202161A2;

            MSZDel.SglTeaEncipher(CryptDataApp, CryptDataApp, LockKeyNo_0);

            // Console.WriteLine("\n  App  result: " + CryptDataApp[0] + " " + CryptDataApp[1]);

            // test if the encryption results are the same - if yes: authentic - if no: not authentic
            if ((CryptDataApp[0] != CryptDataLock[0]) || (CryptDataApp[1] != CryptDataLock[1]))
            {
#if DEBUG
                Console.WriteLine("\n  CRA  result: NOT authentic!");
#endif
                return false;
            }
            else
            {
                // Console.WriteLine("\n  CRA  result: authentic!");
                return true;
            }

        }
        static bool ReadConfig()
        {
            UInt32[] ConfigData = new UInt32[8];
            try
            {
                _rc = MSZDll.SglReadConfig(_ProductId[0], MSZDll.SGL_READ_CONFIG_LOCK_INFO, ConfigData);
            }
            catch
            {
                return false;
            }

            if (_rc != MSZDll.SGL_SUCCESS)
                return false;
            if (ConfigData[0] != 1)
            {
                // Console.WriteLine("\n  CRA  result: readConfig OK!");
                _ConfigData = ConfigData;
            }

            return true;
        }
        internal static uint ReadMovSerialNumber()
        {
            if (!ReadInit())
                return 0;
            if (!ReadConfig())
                return 0;

            uint Adr = 0;
            uint[] Data = new uint[1];
            uint Cnt = (uint)Data.Count();
            try
            {
                _rc = MSZDll.SglReadData(_ProductId[0], Adr, Cnt, Data);
            }
            catch
            {
                return 0;
            }
            if (_rc != MSZDll.SGL_SUCCESS)
            {
                return 0;
            }

            return Data[0];
        }
        internal static bool WriteMovSerialNumber(uint serial)
        {
            uint[] Data = new uint[1];
            uint Adr = 0;
            uint Cnt = (uint)Data.Count();
            Data[0] = serial;

            _rc = MSZDel.SglAuthent(AuthentCode);
            if (_rc != MSZDll.SGL_SUCCESS)
                return false;
            _rc = MSZDll.SglWriteData(_ProductId[0], Adr, Cnt, Data);
            if (_rc != MSZDll.SGL_SUCCESS)
                return false;

            return true;
        }

        internal static uint[] ReadData()
        {
            if (!ReadInit())
            {
                return null;
            }
            if (!ReadConfig())
            {
                return null;
            }

            return ReadKData();
        }
        static uint[] ReadKData()
        {
            uint Adr = 15;
            uint[] Data = new uint[MReg];
            uint Cnt = (uint)Data.Count();

            try
            {
                _rc = MSZDll.SglReadData(_ProductId[0], Adr, Cnt, Data);
            }
            catch
            {
                return null;
            }
            if (_rc != MSZDll.SGL_SUCCESS)
            {
                return null;
            }

            return Data;
        }
        static void WriteKData(uint[] Data, uint productid = uint.MaxValue)
        {
            uint[] mProductId = new uint[1]; //_ProductId;


            mProductId[0] = _ProductId[0];

            if (productid != uint.MaxValue)
                mProductId[0] = productid;

            // check the linked SG-Lock library
            _rc = MSZDel.SglAuthent(AuthentCode);
            if (_rc == MSZDll.SGL_SUCCESS)
            {
                // Console.WriteLine("\nSglAuthent: SUCCESS");
            }
            else
            {
#if DEBUG
                Console.WriteLine("\nSglAuthent: Error no. rc=" + _rc);
#endif
                return;
            }

////            // read the ProductId from attached SG-Lock
//            if (System.Environment.Is64BitProcess)
//                _rc = MSZDll.SglReadProductId64Bit(_ProductId);
//            else
//                _rc = MSZDll.SglReadProductId32Bit(_ProductId);
//            if (_rc == MSZDll.SGL_SUCCESS)
//            {
//                //  Console.WriteLine("\nSglReadProductId: SUCCESS");
//                //  Console.WriteLine("  ProductId= " + _ProductId[0]);
//            }
//            else
//            {
//#if DEBUG
//                Console.WriteLine("\nSglSearchLock: Error no. rc=" + _rc);
//#endif
//                return;
//            }

            _rc = MSZDll.SglWriteProductId(_ProductId[0], mProductId[0]);
            if (_rc != MSZDll.SGL_SUCCESS)
            {
                //AfxMessageBox(IDS_NOWRITE, MB_ICONSTOP | MB_OK);
                //wait.Restore();
                return;
            }

            //*******************************************************//
            //le chiavi SGLock di tipo U3 permettono la programmazione di due codici 
            //key0 e key1 usati in movicon per la validazione (assegnatici da SGLock)
            //*******************************************************//
            //// 1. step: generate a 128-bit key
            uint[] TEA_Key ={
            ///*key0*/ 0xDF60F992, 0x70E71C09, 0xC79FD784, 0x444073B2
	            /*key1*/ 0xE3D9E3FF, 0x49EBB33E,	0xEDFCBE4D, 0x60270ACF 
            };
            //// ATTENTION ! ATTENTION ! ATTENTION ! ATTENTION !
            //// Do this only once when initializing the key prior to delivery
            //// of the dongle and NOT in the protected application !
            //// Writing the key into the SG-Lock module
            _rc = MSZDll.SglWriteKey(_ProductId[0], MSZDll.TEA_KEY_NUM, TEA_Key);
            if (_rc != MSZDll.SGL_SUCCESS)
            {
#if DEBUG
                Console.WriteLine("\nSglWriteData: Error no. rc=" + _rc);
                return;
#endif
            }
            uint Adr = 15;
            uint Cnt = (uint)Data.Count();

            _rc = MSZDll.SglWriteData(_ProductId[0], Adr, Cnt, Data);
            if (_rc != MSZDll.SGL_SUCCESS)
            {
#if DEBUG
                Console.WriteLine("\nSglWriteData: Error no. rc=" + _rc);
                return;
#endif
            }

        }

        internal static void WriteData(uint[] Data, uint productid = uint.MaxValue)
        {
            ////////////////INSERT HERE READFILEDATA!!!!!!!!!!!!!!!

            WriteKData(Data, productid);
        }
        internal static uint[] UConfigData
        {
            get
            {
                return _ConfigData;
            }
        }
        internal static uint[] SerialNumber
        {
            get
            {
                return _SerialNumber;
            }
        }

        static void Compress(FileInfo fi)
        {
            // Get the stream of the source file.
            using (FileStream inFile = fi.OpenRead())
            {
                // Prevent compressing hidden and 
                // already compressed files.
                if ((File.GetAttributes(fi.FullName)
                    & FileAttributes.Hidden)
                    != FileAttributes.Hidden & fi.Extension != ".gz")
                {
                    // Create the compressed file.
                    using (FileStream outFile =
                                File.Create(fi.FullName + ".gz"))
                    {
                        using (GZipStream Compress =
                            new GZipStream(outFile,
                            CompressionMode.Compress))
                        {
                            // Copy the source file into 
                            // the compression stream.
                            inFile.CopyTo(Compress);

                            Console.WriteLine("Compressed {0} from {1} to {2} bytes.",
                                fi.Name, fi.Length.ToString(), outFile.Length.ToString());
                        }
                    }
                }
            }
        }
        static void Decompress(FileInfo fi)
        {
            // Get the stream of the source file.
            using (FileStream inFile = fi.OpenRead())
            {
                // Get original file extension, for example
                // "doc" from report.doc.gz.
                string curFile = fi.FullName;
                string origName = curFile.Remove(curFile.Length -
                        fi.Extension.Length);

                //Create the decompressed file.
                using (FileStream outFile = File.Create(origName))
                {
                    using (GZipStream Decompress = new GZipStream(inFile,
                            CompressionMode.Decompress))
                    {
                        // Copy the decompression stream 
                        // into the output file.
                        Decompress.CopyTo(outFile);

                        Console.WriteLine("Decompressed: {0}", fi.Name);

                    }
                }
            }
        }
        static readonly int _MReg =  MSZDll.REGISTER_MAX_COUNT;
        internal static int MReg
        {
            get { return _MReg; }
        }
        static readonly int _MBReg = MSZDll.REGISTER_MAX_BYTECOUNT;
        internal static int MBReg
        {
            get { return _MBReg; }
        }

        internal static uint ProductId
        {
            get { return _ProductId[0]; }
            set { _ProductId[0] = value; }
        }
    }
}
