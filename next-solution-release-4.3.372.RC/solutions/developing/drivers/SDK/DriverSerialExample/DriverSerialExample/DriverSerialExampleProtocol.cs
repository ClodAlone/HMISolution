////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	DriverSerialExampleProtocol.cs
//
// summary:	Implements the driver serial example protocol class
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBase;

namespace DriverSerialExample
{
    /// <summary>   Communication protocol of DriverSerialExample driver. </summary>
    public class DriverSerialExampleProtocol
    {
        /// <summary>   The file request. </summary>
        private const uint UINT_FileReq = 10;
        /// <summary>   Length of the request. </summary>
        private const uint UINT_RequestLen = 6;
        /// <summary>   Length of the write request. </summary>
        private const uint UINT_WriteRequestLen = 7;
        /// <summary>   Length of the write file request. </summary>
        private const uint UINT_WriteFileReqLen = 10;
        #region methods override

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Reurn request frame lenght for protocol's task "job". </summary>
        ///
        /// <param name="job">  . </param>
        ///
        /// <returns>   The frame length. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public uint GetFrameLength(DriverSerialExampleCommJob job)
        {
            if (job.Type == DriverCodeBase.Enumerators.LinkType.Input ||
                (job.Type == DriverCodeBase.Enumerators.LinkType.InputOutput &&
                job.TagsListToWrite.Count == 0))
                return UINT_RequestLen;
            else
                return UINT_WriteRequestLen + job.TotalJobSize;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Prepare "buffer" with values of tag (bolean) in taglist. </summary>
        ///
        /// <param name="tagList">  . </param>
        /// <param name="bitCount"> number of bits. </param>
        /// <param name="buffer">   [in,out]. </param>
        ///
        /// <returns>   The bit values. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private uint GetBitValues(List<Tag> tagList, UInt16 bitCount, ref byte[] buffer)
        {
            // Build an array of bytes with the bit values stored one bit per byte
            byte[] bitBuffer = new byte[bitCount];
            if (bitBuffer == null)
            {
                return 0;
            }
            uint processedBits = 0;
            for (int i = 0; (i < tagList.Count) && (processedBits < bitCount); i++)
            {
                uint k = tagList[i].Size;
                if ((processedBits + k) > bitCount)
                {
                    break;
                }
                object curVal = tagList[i].Value.Value;
                uint tagArrayDimension = tagList[i].TagNode.ArrayDimension;
                // Single bit
                if (tagArrayDimension == 0)
                {
                    bool boolValue = (bool)curVal;
                    if (boolValue)
                    {
                        bitBuffer[processedBits++] = 1;
                    }
                    else
                    {
                        bitBuffer[processedBits++] = 0;
                    }
                }
                // Array of bits
                else
                {
                    Array b = curVal as Array;
                    if (b != null && b.GetLength(0) == tagArrayDimension)
                    {
                        for (uint j = 0; (j < tagArrayDimension); j++)
                        {
                            bool boolValue = (bool)b.GetValue(j);
                            if (boolValue)
                            {
                                bitBuffer[processedBits++] = 1;
                            }
                            else
                            {
                                bitBuffer[processedBits++] = 0;
                            }
                        }
                    }
                }
            }

            // Compact the bit values in the byte buffer passed as argument
            if (processedBits > 0)
            {
                int byteDim = buffer.GetLength(0);
                for (int i = 0, j = 0, byteIndex = 0; (i < (int)processedBits) && (byteIndex < byteDim); i++)
                {
                    if (bitBuffer[i] > 0)
                    {
                        buffer[byteIndex] += (byte)(1 << j);
                    }
                    if (++j > 7)
                    {
                        j = 0;
                        byteIndex++;
                    }
                }
            }

            return (processedBits);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Load in memory "buffer" the request frame for protocol's task "job". </summary>
        ///
        /// <param name="job">      . </param>
        /// <param name="buffer">   [in,out]. </param>
        ///
        /// <returns>   An uint. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public uint PrepareRequest(DriverSerialExampleCommJob job, ref byte[] buffer)
        {
            
            if (job == null || (job.Station as DriverSerialExampleStation == null))
                return 0;
            uint WriteCh = 0;

            UInt16 ItemCount = 0;
            if (job.Type == DriverCodeBase.Enumerators.LinkType.Input ||
                (job.Type == DriverCodeBase.Enumerators.LinkType.InputOutput &&
                job.TagsListToWrite.Count == 0))
            {
                switch (job.FunctionCode)
                {
                    case FunctionCodes.Coils: // Coils
                        job.Executioncode = 1;
                        if(job.TagsList[0].TagNode.DataType.IdType == Opc.Ua.IdType.Numeric
                            && job.TagsList[0].TagNode.DataType == Opc.Ua.DataTypes.Boolean)
                            ItemCount = (UInt16)job.TotalJobSize;
                        else
                            ItemCount = (UInt16)(job.TotalJobSize*8);
                        break;
                    case FunctionCodes.MultipleRegisters: // Multiple registers
                        job.Executioncode = 3;
                        ItemCount = (UInt16)(job.TotalJobSize >> 1);
                        break;
                    default:
                        return 0;
                }
                
                //prepare a read request
                buffer[WriteCh++] = (byte)((DriverSerialExampleStation)job.Station).StationID;
                buffer[WriteCh++] = job.Executioncode;
                buffer[WriteCh++] = (byte)(job.StartAddress >> 8);
                buffer[WriteCh++] = (byte)job.StartAddress;
                buffer[WriteCh++] = (byte)(ItemCount >> 8);
                buffer[WriteCh++] = (byte)ItemCount;
                return WriteCh;
            }
            else
            {
                if (job.Type == DriverCodeBase.Enumerators.LinkType.UnconditionalOutput)
                {
                    if (job.TagsListToWrite.Count == 0)
                        job.TagsListToWrite.AddRange(job.TagsList);
                }
                if (job.TagsListToWrite.Count == 0)
                    return 0;

                //prepare a write request
                List<Tag> tmpList = new List<Tag>();
                job.TagsListToWrite.Sort(CompareTagByOffset);
                Tag cand = null;
                UInt16 nData = 0;
                do
                {
                    if (tmpList.Count != 0)
                    {
                        if ((cand.ByteOffset + cand.Size) != job.TagsListToWrite[0].ByteOffset)
                            break;
                    }
                    cand = job.TagsListToWrite[0];
                    tmpList.Add(cand);
                    job.TagsListToWrite.Remove(cand);
                    if (!job.TagsListOnWriting.Contains(cand))
                    {
                        job.TagsListOnWriting.Add(cand);
                    }

                    nData += (UInt16)cand.Size;
                    cand.LastValue = cand.Value.Value;
                } while (job.TagsListToWrite.Count > 0);

                UInt16 StartAddress = job.StartAddress;
                
                switch( job.FunctionCode ) {
	                case FunctionCodes.Coils: // Coils
		                if(tmpList[0].TagNode.DataType.IdType == Opc.Ua.IdType.Numeric
                            && tmpList[0].TagNode.DataType == Opc.Ua.DataTypes.Boolean)
		                {
			                //Bit tasks ALWAYS write one bit at once
                            job.Executioncode = 15;
                            ItemCount = nData;

			                StartAddress += (UInt16)tmpList[0].ByteOffset;
		                }
		                else
		                {
                            job.Executioncode = 15;
			                ItemCount = (UInt16)(nData * 8);
			                StartAddress += (UInt16)(tmpList[0].ByteOffset*8);
		                }
		                break;
	                case FunctionCodes.MultipleRegisters: // Multiple registers
                        job.Executioncode = 16;
		                ItemCount = (UInt16)(nData >> 1);
		                StartAddress += (UInt16)(tmpList[0].ByteOffset >> 1);
		                break;
	                default: 
		                return 0;
	            }

                buffer[WriteCh++] = (byte)((DriverSerialExampleStation)job.Station).StationID;
                buffer[WriteCh++] = job.Executioncode;
                buffer[WriteCh++] = (byte)(StartAddress >> 8);
                buffer[WriteCh++] = (byte)StartAddress;

                buffer[WriteCh++] = (byte)(ItemCount >> 8);
                buffer[WriteCh++] = (byte)ItemCount;
                nData = (byte)(job.Executioncode == 15 ? (nData / 8 + (nData % 8 > 0 ? 1 : 0)) : nData);
                buffer[WriteCh++] = (byte)nData;

                //add data to write, ask the job...
                byte [] jobdata = new byte[nData];

                if ((job.FunctionCode != FunctionCodes.Coils) ||
                    (tmpList[0].TagNode.DataType.IdType != Opc.Ua.IdType.Numeric) ||
                    (tmpList[0].TagNode.DataType != Opc.Ua.DataTypes.Boolean))
                {
                
                    int j = 0;

                    for (int i = 0; i < tmpList.Count; i++)
                    {
                        uint k = tmpList[i].Size;
                    
                        tmpList[i].GetTagBuffer(ref jobdata, false, j);
                        j += (int)k;
                    
                    }

                }
                else
                {
                    if (GetBitValues(tmpList, ItemCount, ref jobdata) < 1)
                    {
                        return 0;
                    }
                }

                if (job.FunctionCode != FunctionCodes.Coils )
                    CommJob.SwapByteBuffer(ref jobdata);

                for (int i = 0; i < nData; i++)
                {
                    buffer[WriteCh++] = jobdata[i];
                }

                return WriteCh;
            }
        }
        
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Compare tags by offset. Return 0 if x = y , 1 if x &gt; y and -1 if x &lt; y.
        /// </summary>
        ///
        /// <param name="x">    . </param>
        /// <param name="y">    . </param>
        ///
        /// <returns>   An int. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private static int CompareTagByOffset(Tag x, Tag y)
        {
            if (x == null)
            {
                if (y == null)
                    return 0; //==
                else
                    return -1;// x < y
            }
            else
            { 
                //x!= null
                if (y == null)
                    return 1; //x > y
                else
                {
                    if (x.ByteOffset > y.ByteOffset)
                        return 1;
                    else if (x.ByteOffset == y.ByteOffset)
                        return 0;
                    else
                        return -1;

                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Validation of the response data type input. If correct copy data in job. </summary>
        ///
        /// <param name="receivebuffer">    . </param>
        /// <param name="job">              [in,out]. </param>
        /// <param name="changed">          [in,out]. </param>
        ///
        /// <returns>   true if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool ParseData(byte [] receivebuffer, ref DriverSerialExampleCommJob job, ref List<Tag> changed)
        {
            /*
             * put in the returned data values the received buffer, 
             * from ID to the end of data, crc's have been removed
             */
            if (receivebuffer[0] != (byte)((DriverSerialExampleStation)job.Station).StationID)
            { 
                //error. wrong station ID!!!
            }

            if (job.Executioncode != receivebuffer[1])
            { 
                //error. unexpected function code in answer.
            }

            bool bitVars = ((job.TagsList[0].TagNode.DataType.IdType == Opc.Ua.IdType.Numeric)
                            && (job.TagsList[0].TagNode.DataType == Opc.Ua.DataTypes.Boolean));
            
            
            //check if correct amount of data have been received
            byte bytenumber = 0;
            if ((receivebuffer[1] == 1 || receivebuffer[1] == 2 ||
                receivebuffer[1] == 3 || receivebuffer[1] == 4 ||
                receivebuffer[1] == 20 || receivebuffer[1] == 7) &&
                ((bytenumber = receivebuffer[2]) != job.TotalJobSize))
            {
                /*
                 * Read operations.
                 * Error. The amount of data received is not congruent
                 * with job dimension.
                 */
            }
            
            switch (receivebuffer[1])
            {
                //read
                case 1://Read Coils
                case 3://Read Holding Registers
                    {
                        byte[] jobdata = new byte[bytenumber];
                        receivebuffer.ToList().CopyTo(3, jobdata, 0, bytenumber);
                        job.SetJobData(jobdata, ref changed);
                    }
                    break;
                //write
                case 15://Write multiple Coils
                case 16://Write Multiple Registers
                    //didn't receive an error, can be satisfied...
                    break;
            }

            return true;
        }
        #endregion

        

        
    }
}
