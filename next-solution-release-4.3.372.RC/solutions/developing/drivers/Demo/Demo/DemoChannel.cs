using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBase;
using Opc.Ua;
using DriverCodeBase.Enumerators;

namespace Demo
{
    sealed class DemoChannel : Channel
    {
        #region constants
        public const uint StepSimulazione = 40;
        #endregion
        #region Constructors

        /// <summary>
        /// Initializes the ModbusChannel object.
        /// </summary>
        public DemoChannel(CommunicationDriver commdriver, DemoChannelSettings settings)
            : base(commdriver, settings, false/*steve 010811*/)
        {
            _SimulationRunning = true;
        }

        #endregion

        #region Members
        Random rndGen = new Random(2);

        DateTime LastTime = DateTime.UtcNow;
        object Ctime;
        Random rndSimul = new Random(7);
        private readonly double Range = 100.0;
        #endregion

        #region overrides
        public override void SubscribeJob(CommJob job, CommJobState state)
        {
            List<Tag> tl = (from t in job.TagsList.AsParallel() where (t.DynSettings.MethodID != -1)
                                select t).ToList();

            bool exclude = (tl.Count == job.TagsList.Count);

            if (!exclude)
            {
                base.SubscribeJob(job, state);
            }
        }

        public override bool Startup()
        {
            LastTime = DateTime.UtcNow;
            bool ret = base.Startup();
            return ret;
        }

        public override bool ProcessNewData(CommJob pendingjob)
        {
            return true;
        }

        public void StartSimulation()
        {
            _SimulationRunning = true;
        }
        public void StopSimulation()
        {
            _SimulationRunning = false;
        }

        void UpdateSin(Tag ctag, DemoCommJob job)
        {
            double value = Math.Sin(Convert.ToDouble(job.CommonVal) * 2 * Math.PI);
            double valMin = 1 / Math.Pow(2, 32);
            if (value < valMin && value > valMin * -1)
                value = 0;
            switch ((uint)ctag.TagNode.DataType.Identifier)
            {
                case (uint)BuiltInType.String:
                    {
                        if (ctag.TagNode.ArrayDimension == 0)
                            ctag.Value.Value = GetRandomString();
                        else
                        {
                            string[] a = new string[ctag.TagNode.ArrayDimension];
                            for (int i = 0; i < ctag.TagNode.ArrayDimension; i++)
                            {
                                a[i] = GetRandomString();
                            }
                            if (ctag.Value.Value != a)
                                ctag.Value.Value = a;
                        }
                    }
                    break;
                case (uint)BuiltInType.Byte:
                    {

                        if (ctag.TagNode.ArrayDimension == 0)
                        {
                            double d = (1 + value) * Range / 2;
                            byte nv = Convert.ToByte(d);
                            if (Convert.ToByte(ctag.Value.Value) != nv)
                            {
                                ctag.Value.Value = nv;
                            }
                        }
                        else
                        {
                            byte[] a = new byte[ctag.TagNode.ArrayDimension];
                            for (int i = 0; i < ctag.TagNode.ArrayDimension; i++)
                            {
                                double d = (1 + value) * Range / 2;
                                a[i] = Convert.ToByte(Convert.ToByte(d));
                            }
                            if (ctag.Value.Value != a)
                                ctag.Value.Value = a;
                        }
                    }
                    break;
                case (uint)BuiltInType.Double:
                    {
                        if (ctag.TagNode.ArrayDimension == 0)
                        {
                            if (Convert.ToDouble(ctag.Value.Value) != value)
                                ctag.Value.Value = value;
                        }
                        else
                        {
                            double[] a = new double[ctag.TagNode.ArrayDimension];
                            for (int i = 0; i < ctag.TagNode.ArrayDimension; i++)
                                a[i] = value;
                            if (ctag.Value.Value != a)
                                ctag.Value.Value = a;
                        }
                    }
                    break;
                case (uint)BuiltInType.Float:
                    {
                        if (ctag.TagNode.ArrayDimension == 0)
                        {
                            if (Convert.ToSingle(ctag.Value.Value) != Convert.ToSingle(value))
                                ctag.Value.Value = Convert.ToSingle(value);
                        }
                        else
                        {
                            float[] a = new float[ctag.TagNode.ArrayDimension];
                            for (int i = 0; i < ctag.TagNode.ArrayDimension; i++)
                                a[i] = Convert.ToSingle(value);
                            if (ctag.Value.Value != a)
                                ctag.Value.Value = a;
                        }
                    }
                    break;
                case (uint)BuiltInType.Int16:
                    {
                        if (ctag.TagNode.ArrayDimension == 0)
                        {
                            Int16 nv = Convert.ToInt16(value * Range / 2);
                            if (Convert.ToInt16(ctag.Value.Value) != nv)
                                ctag.Value.Value = nv;
                        }
                        else
                        {
                            Int16[] a = new Int16[ctag.TagNode.ArrayDimension];
                            for (int i = 0; i < ctag.TagNode.ArrayDimension; i++)
                                a[i] = Convert.ToInt16(value * Range / 2);
                            if (ctag.Value.Value != a)
                                ctag.Value.Value = a;
                        }

                    }
                    break;
                case (uint)BuiltInType.Int32:
                    {
                        if (ctag.TagNode.ArrayDimension == 0)
                        {
                            Int32 nv = Convert.ToInt32(value * Range / 2);
                            if (Convert.ToInt32(ctag.Value.Value) != nv)
                                ctag.Value.Value = nv;
                        }
                        else
                        {
                            Int32[] a = new Int32[ctag.TagNode.ArrayDimension];
                            for (int i = 0; i < ctag.TagNode.ArrayDimension; i++)
                                a[i] = Convert.ToInt32(value * Range / 2);
                            if (ctag.Value.Value != a)
                                ctag.Value.Value = a;
                        }
                    }
                    break;
                case (uint)BuiltInType.Int64:
                    {
                        if (ctag.TagNode.ArrayDimension == 0)
                        {
                            Int64 nv = Convert.ToInt64(value * Range / 2);
                            if (Convert.ToInt64(ctag.Value.Value) != nv)
                                ctag.Value.Value = nv;
                        }
                        else
                        {
                            Int64[] a = new Int64[ctag.TagNode.ArrayDimension];
                            for (int i = 0; i < ctag.TagNode.ArrayDimension; i++)
                                a[i] = Convert.ToInt64(value * Range / 2);
                            if (ctag.Value.Value != a)
                                ctag.Value.Value = a;
                        }

                    }
                    break;
                case (uint)BuiltInType.Integer:
                    {
                        if (ctag.TagNode.ArrayDimension == 0)
                        {
                            if ((int)(ctag.Value.Value) != (int)(value * Range / 2))
                                ctag.Value.Value = (int)(CosValue * Range / 2);
                        }
                        else
                        {
                            int[] a = new int[ctag.TagNode.ArrayDimension];
                            for (int i = 0; i < ctag.TagNode.ArrayDimension; i++)
                                a[i] = (int)(value * Range / 2);
                            if (ctag.Value.Value != a)
                                ctag.Value.Value = a;
                        }

                    }
                    break;
                case (uint)BuiltInType.SByte:
                    {
                        if (ctag.TagNode.ArrayDimension == 0)
                        {
                            sbyte nv = Convert.ToSByte(value * Range / 2);
                            if (Convert.ToSByte(ctag.Value.Value) != nv)
                                ctag.Value.Value = nv;
                        }
                        else
                        {
                            sbyte[] a = new sbyte[ctag.TagNode.ArrayDimension];
                            for (int i = 0; i < ctag.TagNode.ArrayDimension; i++)
                                a[i] = Convert.ToSByte(value * Range / 2);
                            if (ctag.Value.Value != a)
                                ctag.Value.Value = a;
                        }

                    }
                    break;
                case (uint)BuiltInType.UInt16:
                    {
                        if (ctag.TagNode.ArrayDimension == 0)
                        {
                            UInt16 nv = Convert.ToUInt16((1 + value) * Range / 2);
                            if (Convert.ToUInt16(ctag.Value.Value) != nv)
                                ctag.Value.Value = nv;
                        }
                        else
                        {
                            UInt16[] a = new UInt16[ctag.TagNode.ArrayDimension];
                            for (int i = 0; i < ctag.TagNode.ArrayDimension; i++)
                            {
                                UInt16 nv = Convert.ToByte((1 + value) * Range / 2);
                                a[i] = Convert.ToUInt16(nv);
                            }
                            if (ctag.Value.Value != a)
                                ctag.Value.Value = a;
                        }
                    }
                    break;
                case (uint)BuiltInType.UInt32:
                    {
                        if (ctag.TagNode.ArrayDimension == 0)
                        {
                            UInt32 nv = Convert.ToUInt32((1 + value) * Range / 2);
                            if (Convert.ToUInt32(ctag.Value.Value) != nv)
                                ctag.Value.Value = nv;
                        }
                        else
                        {
                            UInt32[] a = new UInt32[ctag.TagNode.ArrayDimension];
                            for (int i = 0; i < ctag.TagNode.ArrayDimension; i++)
                            {
                                UInt32 nv = Convert.ToByte((1 + value) * Range / 2);
                                a[i] = Convert.ToUInt32(nv);
                            }
                            if (ctag.Value.Value != a)
                                ctag.Value.Value = a;
                        }
                    }
                    break;
                case (uint)BuiltInType.UInt64:
                    {
                        if (ctag.TagNode.ArrayDimension == 0)
                        {
                            UInt64 nv = Convert.ToUInt64((1 + value) * Range / 2);
                            if (Convert.ToUInt64(ctag.Value.Value) != nv)
                                ctag.Value.Value = nv;
                        }
                        else
                        {
                            UInt64[] a = new UInt64[ctag.TagNode.ArrayDimension];
                            for (int i = 0; i < ctag.TagNode.ArrayDimension; i++)
                            {
                                UInt64 nv = Convert.ToByte((1 + value) * Range / 2);
                                a[i] = Convert.ToUInt64(nv);
                            }
                            if (ctag.Value.Value != a)
                                ctag.Value.Value = a;
                        }
                    }
                    break;
                case (uint)BuiltInType.UInteger:
                    {
                        if (ctag.TagNode.ArrayDimension == 0)
                        {
                            double d = (value + 1) * Range / 2;
                            if ((uint)(ctag.Value.Value) != (uint)(d))
                                ctag.Value.Value = (uint)(d);
                        }
                        else
                        {
                            uint[] a = new uint[ctag.TagNode.ArrayDimension];
                            for (int i = 0; i < ctag.TagNode.ArrayDimension; i++)
                            {
                                double d = (value + 1) * Range / 2;
                                a[i] = (uint)d;
                            }
                            if (ctag.Value.Value != a)
                                ctag.Value.Value = a;
                        }

                    }
                    break;
                default:
                    return;
            }
        }

        void UpdateRamp(Tag ctag, DemoCommJob job)
        {
            double value = Convert.ToDouble(job.CommonVal);
            switch ((uint)ctag.TagNode.DataType.Identifier)
            {
                case (uint)BuiltInType.String:
                    {
                        if (ctag.TagNode.ArrayDimension == 0)
                            ctag.Value.Value = GetRandomString();
                        else
                        {
                            string[] a = new string[ctag.TagNode.ArrayDimension];
                            for (int i = 0; i < ctag.TagNode.ArrayDimension; i++)
                            {
                                a[i] = GetRandomString();
                            }
                            if (ctag.Value.Value != a)
                                ctag.Value.Value = a;
                        }
                    }
                    break;
                case (uint)BuiltInType.Byte:
                    {
                        if (ctag.TagNode.ArrayDimension == 0)
                        {
                            byte nv = Convert.ToByte(value * Range);
                            if (Convert.ToByte(ctag.Value.Value) != nv)
                                ctag.Value.Value = nv;
                        }
                        else
                        {
                            byte[] a = new byte[ctag.TagNode.ArrayDimension];
                            a[0] = Convert.ToByte(value * Range);
                            for (int i = 1; i < ctag.TagNode.ArrayDimension; i++)
                            {
                                a[i] = a[i - 1];
                                a[i]++;
                                if (a[i] > Range)
                                    a[i] = 0;
                            }
                            if (ctag.Value.Value != a)
                                ctag.Value.Value = a;
                        }
                    }
                    break;
                case (uint)BuiltInType.Double:
                    {
                        if (ctag.TagNode.ArrayDimension == 0)
                        {
                            double nv = Convert.ToDouble(value * Range);
                            if (Convert.ToDouble(ctag.Value.Value) != nv)
                                ctag.Value.Value = nv;
                        }
                        else
                        {
                            double[] a = new double[ctag.TagNode.ArrayDimension];
                            a[0] = value * Range;
                            for (int i = 1; i < ctag.TagNode.ArrayDimension; i++)
                            {
                                a[i] = a[i - 1];
                                a[i]++;
                                if (a[i] > Range)
                                    a[i] = 0;
                            }
                            if (ctag.Value.Value != a)
                                ctag.Value.Value = a;
                        }
                    }
                    break;
                case (uint)BuiltInType.Float:
                    {
                        if (ctag.TagNode.ArrayDimension == 0)
                        {
                            float nv = Convert.ToSingle(value * Range);
                            if (Convert.ToSingle(ctag.Value.Value) != nv)
                                ctag.Value.Value = nv;
                        }
                        else
                        {
                            float[] a = new float[ctag.TagNode.ArrayDimension];
                            a[0] = Convert.ToSingle(value * Range);
                            for (int i = 1; i < ctag.TagNode.ArrayDimension; i++)
                            {
                                a[i] = a[i - 1];
                                a[i]++;
                                if (a[i] > Range)
                                    a[i] = 0;
                            }
                            if (ctag.Value.Value != a)
                                ctag.Value.Value = a;
                        }
                    }
                    break;
                case (uint)BuiltInType.Int16:
                    {
                        if (ctag.TagNode.ArrayDimension == 0)
                        {
                            Int16 nv = Convert.ToInt16(value * Range);
                            if (Convert.ToInt16(ctag.Value.Value) != nv)
                                ctag.Value.Value = nv;
                        }
                        else
                        {
                            Int16[] a = new Int16[ctag.TagNode.ArrayDimension];
                            a[0] = Convert.ToInt16(value * Range);
                            for (int i = 1; i < ctag.TagNode.ArrayDimension; i++)
                            {
                                a[i] = a[i - 1];
                                a[i]++;
                                if (a[i] > Range)
                                    a[i] = 0;
                            }
                            if (ctag.Value.Value != a)
                                ctag.Value.Value = a;
                        }
                    }
                    break;
                case (uint)BuiltInType.Int32:
                    {
                        if (ctag.TagNode.ArrayDimension == 0)
                        {
                            Int32 nv = Convert.ToInt32(value * Range);
                            if (Convert.ToInt32(ctag.Value.Value) != nv)
                                ctag.Value.Value = nv;
                        }
                        else
                        {
                            Int32[] a = new Int32[ctag.TagNode.ArrayDimension];
                            a[0] = Convert.ToInt32(value * Range);
                            for (int i = 1; i < ctag.TagNode.ArrayDimension; i++)
                            {
                                a[i] = a[i - 1];
                                a[i]++;
                                if (a[i] > Range)
                                    a[i] = 0;
                            }
                            if (ctag.Value.Value != a)
                                ctag.Value.Value = a;
                        }
                    }
                    break;
                case (uint)BuiltInType.Int64:
                    {
                        if (ctag.TagNode.ArrayDimension == 0)
                        {
                            Int64 nv = Convert.ToInt64(value * Range);
                            if (Convert.ToInt64(ctag.Value.Value) != nv)
                                ctag.Value.Value = nv;
                        }
                        else
                        {
                            Int64[] a = new Int64[ctag.TagNode.ArrayDimension];
                            a[0] = Convert.ToInt64(value * Range);
                            for (int i = 1; i < ctag.TagNode.ArrayDimension; i++)
                            {
                                a[i] = a[i - 1];
                                a[i]++;
                                if (a[i] > Range)
                                    a[i] = 0;
                            }
                            if (ctag.Value.Value != a)
                                ctag.Value.Value = a;
                        }
                    }
                    break;
                case (uint)BuiltInType.Integer:
                    {
                        if (ctag.TagNode.ArrayDimension == 0)
                        {
                            int nv = (int)(value * Range);
                            if ((int)(ctag.Value.Value) != nv)
                                ctag.Value.Value = nv;
                        }
                        else
                        {
                            int[] a = new int[ctag.TagNode.ArrayDimension];
                            a[0] = (int)(value * Range);
                            for (int i = 1; i < ctag.TagNode.ArrayDimension; i++)
                            {
                                a[i] = a[i - 1];
                                a[i]++;
                                if (a[i] > Range)
                                    a[i] = 0;
                            }
                            if (ctag.Value.Value != a)
                                ctag.Value.Value = a;
                        }
                    }
                    break;
                case (uint)BuiltInType.SByte:
                    {
                        if (ctag.TagNode.ArrayDimension == 0)
                        {
                            sbyte nv = Convert.ToSByte(value * Range);
                            if (Convert.ToSByte(ctag.Value.Value) != nv)
                                ctag.Value.Value = nv;
                        }
                        else
                        {
                            sbyte[] a = new sbyte[ctag.TagNode.ArrayDimension];
                            a[0] = Convert.ToSByte(value * Range);
                            for (int i = 1; i < ctag.TagNode.ArrayDimension; i++)
                            {
                                a[i] = a[i - 1];
                                a[i]++;
                                if (a[i] > Range)
                                    a[i] = 0;
                            }
                            if (ctag.Value.Value != a)
                                ctag.Value.Value = a;
                        }
                    }
                    break;
                case (uint)BuiltInType.UInt16:
                    {
                        if (ctag.TagNode.ArrayDimension == 0)
                        {
                            UInt16 nv = Convert.ToUInt16(value * Range);
                            if (Convert.ToUInt16(ctag.Value.Value) != nv)
                                ctag.Value.Value = nv;
                        }
                        else
                        {
                            UInt16[] a = new UInt16[ctag.TagNode.ArrayDimension];
                            a[0] = Convert.ToUInt16(value * Range);
                            for (int i = 1; i < ctag.TagNode.ArrayDimension; i++)
                            {
                                a[i] = a[i - 1];
                                a[i]++;
                                if (a[i] > Range)
                                    a[i] = 0;
                            }
                            if (ctag.Value.Value != a)
                                ctag.Value.Value = a;
                        }
                    }
                    break;
                case (uint)BuiltInType.UInt32:
                    {
                        if (ctag.TagNode.ArrayDimension == 0)
                        {
                            UInt32 nv = Convert.ToUInt32(value * Range);
                            if (Convert.ToUInt32(ctag.Value.Value) != nv)
                                ctag.Value.Value = nv;
                        }
                        else
                        {
                            UInt32[] a = new UInt32[ctag.TagNode.ArrayDimension];
                            a[0] = Convert.ToUInt32(value * Range);
                            for (int i = 1; i < ctag.TagNode.ArrayDimension; i++)
                            {
                                a[i] = a[i - 1];
                                a[i]++;
                                if (a[i] > Range)
                                    a[i] = 0;
                            }
                            if (ctag.Value.Value != a)
                                ctag.Value.Value = a;
                        }
                    }
                    break;
                case (uint)BuiltInType.UInt64:
                    {
                        if (ctag.TagNode.ArrayDimension == 0)
                        {
                            UInt64 nv = Convert.ToUInt64(value * Range);
                            if (Convert.ToUInt64(ctag.Value.Value) != nv)
                                ctag.Value.Value = nv;
                        }
                        else
                        {
                            UInt64[] a = new UInt64[ctag.TagNode.ArrayDimension];
                            a[0] = Convert.ToUInt64(value * Range);
                            for (int i = 1; i < ctag.TagNode.ArrayDimension; i++)
                            {
                                a[i] = a[i - 1];
                                a[i]++;
                                if (a[i] > Range)
                                    a[i] = 0;
                            }
                            if (ctag.Value.Value != a)
                                ctag.Value.Value = a;
                        }
                    }
                    break;
                case (uint)BuiltInType.UInteger:
                    {
                        if (ctag.TagNode.ArrayDimension == 0)
                        {
                            uint nv = (uint)(value * Range);
                            if ((uint)(ctag.Value.Value) != nv)
                                ctag.Value.Value = nv;
                        }
                        else
                        {
                            uint[] a = new uint[ctag.TagNode.ArrayDimension];
                            a[0] = (uint)(value * Range);
                            for (int i = 0; i < ctag.TagNode.ArrayDimension; i++)
                            {
                                a[i] = a[i - 1];
                                a[i]++;
                                if (a[i] > Range)
                                    a[i] = 0;
                            }
                            if (ctag.Value.Value != a)
                                ctag.Value.Value = a;
                        }
                    }
                    break;
                default:
                    return;
            }
        }

        void UpdateCos(Tag ctag, DemoCommJob job)
        {
            double value = Math.Cos(Convert.ToDouble(job.CommonVal) * 2 * Math.PI);
            double valMin = 1 / Math.Pow(2, 32);
            if (value < valMin && value > valMin * -1)
                value = 0;
            switch ((uint)ctag.TagNode.DataType.Identifier)
            {
                case (uint)BuiltInType.String:
                    {
                        if (ctag.TagNode.ArrayDimension == 0)
                            ctag.Value.Value = GetRandomString();
                        else
                        {
                            string[] a = new string[ctag.TagNode.ArrayDimension];
                            for (int i = 0; i < ctag.TagNode.ArrayDimension; i++)
                            {
                                a[i] = GetRandomString();
                            }
                            if (ctag.Value.Value != a)
                                ctag.Value.Value = a;
                        }
                    }
                    break;
                case (uint)BuiltInType.Byte:
                    {

                        if (ctag.TagNode.ArrayDimension == 0)
                        {
                            double d = (1 + value) * Range / 2;
                            byte nv = Convert.ToByte(d);
                            if (Convert.ToByte(ctag.Value.Value) != nv)
                            {
                                ctag.Value.Value = nv;
                            }
                        }
                        else
                        {
                            byte[] a = new byte[ctag.TagNode.ArrayDimension];
                            for (int i = 0; i < ctag.TagNode.ArrayDimension; i++)
                            {
                                double d = (1 + value) * Range / 2;
                                a[i] = Convert.ToByte(Convert.ToByte(d));
                            }
                            if (ctag.Value.Value != a)
                                ctag.Value.Value = a;
                        }
                    }
                    break;
                case (uint)BuiltInType.Double:
                    {
                        if (ctag.TagNode.ArrayDimension == 0)
                        {
                            if (Convert.ToDouble(ctag.Value.Value) != value)
                                ctag.Value.Value = value;
                        }
                        else
                        {
                            double[] a = new double[ctag.TagNode.ArrayDimension];
                            for (int i = 0; i < ctag.TagNode.ArrayDimension; i++)
                                a[i] = value;
                            if (ctag.Value.Value != a)
                                ctag.Value.Value = a;
                        }
                    }
                    break;
                case (uint)BuiltInType.Float:
                    {
                        if (ctag.TagNode.ArrayDimension == 0)
                        {
                            if (Convert.ToSingle(ctag.Value.Value) != Convert.ToSingle(value))
                                ctag.Value.Value = Convert.ToSingle(value);
                        }
                        else
                        {
                            float[] a = new float[ctag.TagNode.ArrayDimension];
                            for (int i = 0; i < ctag.TagNode.ArrayDimension; i++)
                                a[i] = Convert.ToSingle(value);
                            if (ctag.Value.Value != a)
                                ctag.Value.Value = a;
                        }
                    }
                    break;
                case (uint)BuiltInType.Int16:
                    {
                        if (ctag.TagNode.ArrayDimension == 0)
                        {
                            Int16 nv = Convert.ToInt16(value * Range / 2);
                            if (Convert.ToInt16(ctag.Value.Value) != nv)
                                ctag.Value.Value = nv;
                        }
                        else
                        {
                            Int16[] a = new Int16[ctag.TagNode.ArrayDimension];
                            for (int i = 0; i < ctag.TagNode.ArrayDimension; i++)
                                a[i] = Convert.ToInt16(value * Range / 2);
                            if (ctag.Value.Value != a)
                                ctag.Value.Value = a;
                        }

                    }
                    break;
                case (uint)BuiltInType.Int32:
                    {
                        if (ctag.TagNode.ArrayDimension == 0)
                        {
                            Int32 nv = Convert.ToInt32(value * Range / 2);
                            if (Convert.ToInt32(ctag.Value.Value) != nv)
                                ctag.Value.Value = nv;
                        }
                        else
                        {
                            Int32[] a = new Int32[ctag.TagNode.ArrayDimension];
                            for (int i = 0; i < ctag.TagNode.ArrayDimension; i++)
                                a[i] = Convert.ToInt32(value * Range / 2);
                            if (ctag.Value.Value != a)
                                ctag.Value.Value = a;
                        }
                    }
                    break;
                case (uint)BuiltInType.Int64:
                    {
                        if (ctag.TagNode.ArrayDimension == 0)
                        {
                            Int64 nv = Convert.ToInt64(value * Range / 2);
                            if (Convert.ToInt64(ctag.Value.Value) != nv)
                                ctag.Value.Value = nv;
                        }
                        else
                        {
                            Int64[] a = new Int64[ctag.TagNode.ArrayDimension];
                            for (int i = 0; i < ctag.TagNode.ArrayDimension; i++)
                                a[i] = Convert.ToInt64(value * Range / 2);
                            if (ctag.Value.Value != a)
                                ctag.Value.Value = a;
                        }

                    }
                    break;
                case (uint)BuiltInType.Integer:
                    {
                        if (ctag.TagNode.ArrayDimension == 0)
                        {
                            if ((int)(ctag.Value.Value) != (int)(value * Range / 2))
                                ctag.Value.Value = (int)(CosValue * Range / 2);
                        }
                        else
                        {
                            int[] a = new int[ctag.TagNode.ArrayDimension];
                            for (int i = 0; i < ctag.TagNode.ArrayDimension; i++)
                                a[i] = (int)(value * Range / 2);
                            if (ctag.Value.Value != a)
                                ctag.Value.Value = a;
                        }

                    }
                    break;
                case (uint)BuiltInType.SByte:
                    {
                        if (ctag.TagNode.ArrayDimension == 0)
                        {
                            sbyte nv = Convert.ToSByte(value * Range / 2);
                            if (Convert.ToSByte(ctag.Value.Value) != nv)
                                ctag.Value.Value = nv;
                        }
                        else
                        {
                            sbyte[] a = new sbyte[ctag.TagNode.ArrayDimension];
                            for (int i = 0; i < ctag.TagNode.ArrayDimension; i++)
                                a[i] = Convert.ToSByte(value * Range / 2);
                            if (ctag.Value.Value != a)
                                ctag.Value.Value = a;
                        }

                    }
                    break;
                case (uint)BuiltInType.UInt16:
                    {
                        if (ctag.TagNode.ArrayDimension == 0)
                        {
                            UInt16 nv = Convert.ToUInt16((1 + value) * Range / 2);
                            if (Convert.ToUInt16(ctag.Value.Value) != nv)
                                ctag.Value.Value = nv;
                        }
                        else
                        {
                            UInt16[] a = new UInt16[ctag.TagNode.ArrayDimension];
                            for (int i = 0; i < ctag.TagNode.ArrayDimension; i++)
                            {
                                UInt16 nv = Convert.ToByte((1 + value) * Range / 2);
                                a[i] = Convert.ToUInt16(nv);
                            }
                            if (ctag.Value.Value != a)
                                ctag.Value.Value = a;
                        }
                    }
                    break;
                case (uint)BuiltInType.UInt32:
                    {
                        if (ctag.TagNode.ArrayDimension == 0)
                        {
                            UInt32 nv = Convert.ToUInt32((1 + value) * Range / 2);
                            if (Convert.ToUInt32(ctag.Value.Value) != nv)
                                ctag.Value.Value = nv;
                        }
                        else
                        {
                            UInt32[] a = new UInt32[ctag.TagNode.ArrayDimension];
                            for (int i = 0; i < ctag.TagNode.ArrayDimension; i++)
                            {
                                UInt32 nv = Convert.ToByte((1 + value) * Range / 2);
                                a[i] = Convert.ToUInt32(nv);
                            }
                            if (ctag.Value.Value != a)
                                ctag.Value.Value = a;
                        }
                    }
                    break;
                case (uint)BuiltInType.UInt64:
                    {
                        if (ctag.TagNode.ArrayDimension == 0)
                        {
                            UInt64 nv = Convert.ToUInt64((1 + value) * Range / 2);
                            if (Convert.ToUInt64(ctag.Value.Value) != nv)
                                ctag.Value.Value = nv;
                        }
                        else
                        {
                            UInt64[] a = new UInt64[ctag.TagNode.ArrayDimension];
                            for (int i = 0; i < ctag.TagNode.ArrayDimension; i++)
                            {
                                UInt64 nv = Convert.ToByte((1 + value) * Range / 2);
                                a[i] = Convert.ToUInt64(nv);
                            }
                            if (ctag.Value.Value != a)
                                ctag.Value.Value = a;
                        }
                    }
                    break;
                case (uint)BuiltInType.UInteger:
                    {
                        if (ctag.TagNode.ArrayDimension == 0)
                        {
                            double d = (value + 1) * Range / 2;
                            if ((uint)(ctag.Value.Value) != (uint)(d))
                                ctag.Value.Value = (uint)(d);
                        }
                        else
                        {
                            uint[] a = new uint[ctag.TagNode.ArrayDimension];
                            for (int i = 0; i < ctag.TagNode.ArrayDimension; i++)
                            {
                                double d = (value + 1) * Range / 2;
                                a[i] = (uint)d;
                            }
                            if (ctag.Value.Value != a)
                                ctag.Value.Value = a;
                        }

                    }
                    break;
                default:
                    return;
            }
        }

        void UpdateCos(Tag ctag)
        {
            switch ((uint)ctag.TagNode.DataType.Identifier)
            {
                case (uint)BuiltInType.String:
                    {
                        if (ctag.TagNode.ArrayDimension == 0)
                            ctag.Value.Value = GetRandomString();
                        else
                        {
                            string[] a = new string[ctag.TagNode.ArrayDimension];
                            for (int i = 0; i < ctag.TagNode.ArrayDimension; i++)
                            {
                                a[i] = GetRandomString();
                            }
                            if (ctag.Value.Value != a)
                                ctag.Value.Value = a;
                        }
                    }
                    break;
                case (uint)BuiltInType.Byte:
                    {
                        
                        if (ctag.TagNode.ArrayDimension == 0)
                        {
                            double d = (1 + CosValue) * Range / 2;
                            byte nv = Convert.ToByte(d);
                            if (Convert.ToByte(ctag.Value.Value) != nv)
                            {
                                ctag.Value.Value = nv;
                            }
                        }
                        else
                        { 
                            byte[] a = new byte[ctag.TagNode.ArrayDimension];
                            for (int i = 0; i < ctag.TagNode.ArrayDimension; i++)
                            {
                                double d = (1 + CosValue) * Range / 2;
                                a[i] = Convert.ToByte(Convert.ToByte(d));
                            }
                            if (ctag.Value.Value != a)
                                ctag.Value.Value = a;
                        }
                    }
                    break;
                case (uint)BuiltInType.Double:
                    {
                        if (ctag.TagNode.ArrayDimension == 0)
                        {
                            if (Convert.ToDouble(ctag.Value.Value) != CosValue)
                                ctag.Value.Value = CosValue;
                        }
                        else
                        {
                            double[] a = new double[ctag.TagNode.ArrayDimension];
                            for (int i = 0; i < ctag.TagNode.ArrayDimension; i++)
                                a[i] = CosValue;
                            if (ctag.Value.Value != a)
                                ctag.Value.Value = a;
                        }
                    }
                    break;
                case (uint)BuiltInType.Float:
                    {
                        if (ctag.TagNode.ArrayDimension == 0)
                        {
                            if (Convert.ToSingle(ctag.Value.Value) != Convert.ToSingle(CosValue))
                                ctag.Value.Value = Convert.ToSingle(CosValue);
                        }
                        else
                        {
                            float[] a = new float[ctag.TagNode.ArrayDimension];
                            for (int i = 0; i < ctag.TagNode.ArrayDimension; i++)
                                a[i] = Convert.ToSingle(CosValue);
                            if (ctag.Value.Value != a)
                                ctag.Value.Value = a;
                        }
                    }
                    break;
                case (uint)BuiltInType.Int16:
                    {
                        if (ctag.TagNode.ArrayDimension == 0)
                        {
                            Int16 nv = Convert.ToInt16(CosValue * Range / 2);
                            if (Convert.ToInt16(ctag.Value.Value) != nv)
                                ctag.Value.Value = nv;
                        }
                        else
                        {
                            Int16[] a = new Int16[ctag.TagNode.ArrayDimension];
                            for (int i = 0; i < ctag.TagNode.ArrayDimension; i++)
                                a[i] = Convert.ToInt16(CosValue * Range / 2);
                            if (ctag.Value.Value != a)
                                ctag.Value.Value = a;
                        }
                        
                    }
                    break;
                case (uint)BuiltInType.Int32:
                    {
                        if (ctag.TagNode.ArrayDimension == 0)
                        {
                            Int32 nv = Convert.ToInt32(CosValue * Range / 2);
                            if (Convert.ToInt32(ctag.Value.Value) != nv)
                                ctag.Value.Value = nv;
                        }
                        else
                        {
                            Int32[] a = new Int32[ctag.TagNode.ArrayDimension];
                            for (int i = 0; i < ctag.TagNode.ArrayDimension; i++)
                                a[i] = Convert.ToInt32(CosValue * Range / 2);
                            if (ctag.Value.Value != a)
                                ctag.Value.Value = a;
                        }
                    }
                    break;
                case (uint)BuiltInType.Int64:
                    {
                        if (ctag.TagNode.ArrayDimension == 0)
                        {
                            Int64 nv = Convert.ToInt64(CosValue * Range / 2);
                            if (Convert.ToInt64(ctag.Value.Value) != nv)
                                ctag.Value.Value = nv;
                        }
                        else
                        {
                            Int64[] a = new Int64[ctag.TagNode.ArrayDimension];
                            for (int i = 0; i < ctag.TagNode.ArrayDimension; i++)
                                a[i] = Convert.ToInt64(CosValue * Range / 2);
                            if (ctag.Value.Value != a)
                                ctag.Value.Value = a;
                        }
                        
                    }
                    break;
                case (uint)BuiltInType.Integer:
                    {
                        if (ctag.TagNode.ArrayDimension == 0)
                        {
                            if ((int)(ctag.Value.Value) != (int)(CosValue * Range / 2))
                                ctag.Value.Value = (int)(CosValue * Range / 2);
                        }
                        else
                        {
                            int[] a = new int[ctag.TagNode.ArrayDimension];
                            for (int i = 0; i < ctag.TagNode.ArrayDimension; i++)
                                a[i] = (int)(CosValue * Range / 2);
                            if (ctag.Value.Value != a)
                                ctag.Value.Value = a;
                        }
                        
                    }
                    break;
                case (uint)BuiltInType.SByte:
                    {
                        if (ctag.TagNode.ArrayDimension == 0)
                        {
                            sbyte nv = Convert.ToSByte(CosValue * Range / 2);
                            if (Convert.ToSByte(ctag.Value.Value) != nv)
                                ctag.Value.Value = nv;
                        }
                        else
                        {
                            sbyte[] a = new sbyte[ctag.TagNode.ArrayDimension];
                            for (int i = 0; i < ctag.TagNode.ArrayDimension; i++)
                                a[i] = Convert.ToSByte(CosValue * Range / 2);
                            if (ctag.Value.Value != a)
                                ctag.Value.Value = a;
                        }
                        
                    }
                    break;
                case (uint)BuiltInType.UInt16:
                    {
                        if (ctag.TagNode.ArrayDimension == 0)
                        {
                            UInt16 nv = Convert.ToUInt16((1 + CosValue) * Range / 2);
                            if (Convert.ToUInt16(ctag.Value.Value) != nv)
                                ctag.Value.Value = nv;
                        }
                        else
                        {
                            UInt16[] a = new UInt16[ctag.TagNode.ArrayDimension];
                            for (int i = 0; i < ctag.TagNode.ArrayDimension; i++)
                            {
                                UInt16 nv = Convert.ToByte((1 + CosValue) * Range / 2);
                                a[i] = Convert.ToUInt16(nv);
                            }
                            if (ctag.Value.Value != a)
                                ctag.Value.Value = a;
                        }
                    }
                    break;
                case (uint)BuiltInType.UInt32:
                    {
                        if (ctag.TagNode.ArrayDimension == 0)
                        {
                            UInt32 nv = Convert.ToUInt32((1 + CosValue) * Range / 2);
                            if (Convert.ToUInt32(ctag.Value.Value) != nv)
                                ctag.Value.Value = nv;
                        }
                        else
                        {
                            UInt32[] a = new UInt32[ctag.TagNode.ArrayDimension];
                            for (int i = 0; i < ctag.TagNode.ArrayDimension; i++)
                            {
                                UInt32 nv = Convert.ToByte((1 + CosValue) * Range / 2);
                                a[i] = Convert.ToUInt32(nv);
                            }
                            if (ctag.Value.Value != a)
                                ctag.Value.Value = a;
                        }
                    }
                    break;
                case (uint)BuiltInType.UInt64:
                    {
                        if (ctag.TagNode.ArrayDimension == 0)
                        {
                            UInt64 nv = Convert.ToUInt64((1 + CosValue) * Range / 2);
                            if (Convert.ToUInt64(ctag.Value.Value) != nv)
                                ctag.Value.Value = nv;
                        }
                        else
                        {
                            UInt64[] a = new UInt64[ctag.TagNode.ArrayDimension];
                            for (int i = 0; i < ctag.TagNode.ArrayDimension; i++)
                            {
                                UInt64 nv = Convert.ToByte((1 + CosValue) * Range / 2);
                                a[i] = Convert.ToUInt64(nv);
                            }
                            if (ctag.Value.Value != a)
                                ctag.Value.Value = a;
                        }
                    }
                    break;
                case (uint)BuiltInType.UInteger:
                    {
                        if (ctag.TagNode.ArrayDimension == 0)
                        {
                            double d = (CosValue + 1) * Range / 2;
                            if ((uint)(ctag.Value.Value) != (uint)(d))
                                ctag.Value.Value = (uint)(d);
                        }
                        else
                        {
                            uint[] a = new uint[ctag.TagNode.ArrayDimension];
                            for (int i = 0; i < ctag.TagNode.ArrayDimension; i++)
                            {
                                double d = (CosValue + 1) * Range / 2;
                                a[i] = (uint) d;
                            }
                            if (ctag.Value.Value != a)
                                ctag.Value.Value = a;
                        }
                        
                    }
                    break;
                default:
                    return;
            }
        }
        void UpdateRamp(Tag ctag)
        {
            //System.Diagnostics.Trace.TraceInformation(DateTime.Now.ToString("HH:mm:ss.fff"));
            switch ((uint)ctag.TagNode.DataType.Identifier)
            {
                case (uint)BuiltInType.String:
                    {
                        if (ctag.TagNode.ArrayDimension == 0)
                            ctag.Value.Value = GetRandomString();
                        else
                        {
                            string[] a = new string[ctag.TagNode.ArrayDimension];
                            for (int i = 0; i < ctag.TagNode.ArrayDimension; i++)
                            {
                                a[i] = GetRandomString();
                            }
                            if (ctag.Value.Value != a)
                                ctag.Value.Value = a;
                        }
                    }
                    break;
                case (uint)BuiltInType.Byte:
                    {
                        if (ctag.TagNode.ArrayDimension == 0)
                        {
                            byte nv = Convert.ToByte(RampValue*Range);
                            if (Convert.ToByte(ctag.Value.Value) != nv)
                                ctag.Value.Value = nv;
                        }
                        else
                        { 
                            byte[] a = new byte[ctag.TagNode.ArrayDimension];
                            a[0] = Convert.ToByte(RampValue * Range);
                            for (int i = 1; i < ctag.TagNode.ArrayDimension; i++)
                            {
                                a[i] = a[i - 1];
                                a[i]++;
                                if (a[i] > Range)
                                    a[i] = 0;
                            }
                            if (ctag.Value.Value != a)
                                ctag.Value.Value = a;
                        }
                    }
                    break;
                case (uint)BuiltInType.Double:
                    {
                        if (ctag.TagNode.ArrayDimension == 0)
                        {
                            if (Convert.ToDouble(ctag.Value.Value) != RampValue)
                                ctag.Value.Value = RampValue * Range;
                        }
                        else
                        {
                            double[] a = new double[ctag.TagNode.ArrayDimension];
                            a[0] = RampValue * Range;
                            for (int i = 1; i < ctag.TagNode.ArrayDimension; i++)
                            {
                                a[i] = a[i - 1];
                                a[i]++;
                                if (a[i] > Range)
                                    a[i] = 0;
                            }
                            if (ctag.Value.Value != a)
                                ctag.Value.Value = a;
                        }
                    }
                    break;
                case (uint)BuiltInType.Float:
                    {
                        if (ctag.TagNode.ArrayDimension == 0)
                        {
                            float nv = Convert.ToSingle(RampValue * Range);
                            if (Convert.ToSingle(ctag.Value.Value) != nv)
                                ctag.Value.Value = nv;
                        }
                        else
                        {
                            float[] a = new float[ctag.TagNode.ArrayDimension];
                            a[0] = Convert.ToSingle(RampValue * Range);
                            for (int i = 1; i < ctag.TagNode.ArrayDimension; i++)
                            {
                                a[i] = a[i - 1];
                                a[i]++;
                                if (a[i] > Range)
                                    a[i] = 0;
                            }
                            if (ctag.Value.Value != a)
                                ctag.Value.Value = a;
                        }
                    }
                    break;
                case (uint)BuiltInType.Int16:
                    {
                        if (ctag.TagNode.ArrayDimension == 0)
                        {
                            Int16 nv = Convert.ToInt16(RampValue * Range);
                            if (Convert.ToInt16(ctag.Value.Value) != nv)
                                ctag.Value.Value = nv;
                        }
                        else
                        {
                            Int16[] a = new Int16[ctag.TagNode.ArrayDimension];
                            a[0] = Convert.ToInt16(RampValue * Range);
                            for (int i = 1; i < ctag.TagNode.ArrayDimension; i++)
                            {
                                a[i] = a[i - 1];
                                a[i]++;
                                if (a[i] > Range)
                                    a[i] = 0;
                            }
                            if (ctag.Value.Value != a)
                                ctag.Value.Value = a;
                        }
                    }
                    break;
                case (uint)BuiltInType.Int32:
                    {
                        if (ctag.TagNode.ArrayDimension == 0)
                        {
                            Int32 nv = Convert.ToInt32(RampValue * Range);
                            if (Convert.ToInt32(ctag.Value.Value) != nv)
                                ctag.Value.Value = nv;
                        }
                        else
                        {
                            Int32[] a = new Int32[ctag.TagNode.ArrayDimension];
                            a[0] = Convert.ToInt32(RampValue * Range);
                            for (int i = 1; i < ctag.TagNode.ArrayDimension; i++)
                            {
                                a[i] = a[i - 1];
                                a[i]++;
                                if (a[i] > Range)
                                    a[i] = 0;
                            }
                            if (ctag.Value.Value != a)
                                ctag.Value.Value = a;
                        }
                    }
                    break;
                case (uint)BuiltInType.Int64:
                    {
                        if (ctag.TagNode.ArrayDimension == 0)
                        {
                            Int64 nv = Convert.ToInt64(RampValue * Range);
                            if (Convert.ToInt64(ctag.Value.Value) != nv)
                                ctag.Value.Value = nv;
                        }
                        else
                        {
                            Int64[] a = new Int64[ctag.TagNode.ArrayDimension];
                            a[0] = Convert.ToInt64(RampValue * Range);
                            for (int i = 1; i < ctag.TagNode.ArrayDimension; i++)
                            {
                                a[i] = a[i - 1];
                                a[i]++;
                                if (a[i] > Range)
                                    a[i] = 0;
                            }
                            if (ctag.Value.Value != a)
                                ctag.Value.Value = a;
                        }
                    }
                    break;
                case (uint)BuiltInType.Integer:
                    {
                        if (ctag.TagNode.ArrayDimension == 0)
                        {
                            int nv = (int)(RampValue * Range);
                            if ((int)(ctag.Value.Value) != nv)
                                ctag.Value.Value = nv;
                        }
                        else
                        {
                            int[] a = new int[ctag.TagNode.ArrayDimension];
                            a[0] = (int)(RampValue * Range);
                            for (int i = 1; i < ctag.TagNode.ArrayDimension; i++)
                            {
                                a[i] = a[i - 1];
                                a[i]++;
                                if (a[i] > Range)
                                    a[i] = 0;
                            }
                            if (ctag.Value.Value != a)
                                ctag.Value.Value = a;
                        }
                    }
                    break;
                case (uint)BuiltInType.SByte:
                    {
                        if (ctag.TagNode.ArrayDimension == 0)
                        {
                            sbyte nv = Convert.ToSByte(RampValue * Range);
                            if (Convert.ToSByte(ctag.Value.Value) != nv)
                                ctag.Value.Value = nv;
                        }
                        else
                        {
                            sbyte[] a = new sbyte[ctag.TagNode.ArrayDimension];
                            a[0] = Convert.ToSByte(RampValue * Range);
                            for (int i = 1; i < ctag.TagNode.ArrayDimension; i++)
                            {
                                a[i] = a[i - 1];
                                a[i]++;
                                if (a[i] > Range)
                                    a[i] = 0;
                            }
                            if (ctag.Value.Value != a)
                                ctag.Value.Value = a;
                        }
                    }
                    break;
                case (uint)BuiltInType.UInt16:
                    {
                        if (ctag.TagNode.ArrayDimension == 0)
                        {
                            UInt16 nv = Convert.ToUInt16(RampValue * Range);
                            if (Convert.ToUInt16(ctag.Value.Value) != nv)
                                ctag.Value.Value = nv;
                        }
                        else
                        {
                            UInt16[] a = new UInt16[ctag.TagNode.ArrayDimension];
                            a[0] = Convert.ToUInt16(RampValue * Range);
                            for (int i = 1; i < ctag.TagNode.ArrayDimension; i++)
                            {
                                a[i] = a[i - 1];
                                a[i]++;
                                if (a[i] > Range)
                                    a[i] = 0;
                            }
                            if (ctag.Value.Value != a)
                                ctag.Value.Value = a;
                        }
                    }
                    break;
                case (uint)BuiltInType.UInt32:
                    {
                        if (ctag.TagNode.ArrayDimension == 0)
                        {
                            UInt32 nv = Convert.ToUInt32(RampValue * Range);
                            if (Convert.ToUInt32(ctag.Value.Value) != nv)
                                ctag.Value.Value = nv;
                        }
                        else
                        {
                            UInt32[] a = new UInt32[ctag.TagNode.ArrayDimension];
                            a[0] = Convert.ToUInt32(RampValue * Range);
                            for (int i = 1; i < ctag.TagNode.ArrayDimension; i++)
                            {
                                a[i] = a[i - 1];
                                a[i]++;
                                if (a[i] > Range)
                                    a[i] = 0;
                            }
                            if (ctag.Value.Value != a)
                                ctag.Value.Value = a;
                        }
                    }
                    break;
                case (uint)BuiltInType.UInt64:
                    {
                        if (ctag.TagNode.ArrayDimension == 0)
                        {
                            UInt64 nv = Convert.ToUInt64(RampValue * Range);
                            if (Convert.ToUInt64(ctag.Value.Value) != nv)
                                ctag.Value.Value = nv;
                        }
                        else
                        {
                            UInt64[] a = new UInt64[ctag.TagNode.ArrayDimension];
                            a[0] = Convert.ToUInt64(RampValue * Range);
                            for (int i = 1; i < ctag.TagNode.ArrayDimension; i++)
                            {
                                a[i] = a[i - 1];
                                a[i]++;
                                if (a[i] > Range)
                                    a[i] = 0;
                            }
                            if (ctag.Value.Value != a)
                                ctag.Value.Value = a;
                        }
                    }
                    break;
                case (uint)BuiltInType.UInteger:
                    {
                        if (ctag.TagNode.ArrayDimension == 0)
                        {
                            uint nv = (uint)(RampValue * Range);
                            if ((uint)(ctag.Value.Value) != nv)
                                ctag.Value.Value = nv;
                        }
                        else
                        {
                            uint[] a = new uint[ctag.TagNode.ArrayDimension];
                            a[0] = (uint)(RampValue * Range);
                            for (int i = 0; i < ctag.TagNode.ArrayDimension; i++)
                            {
                                a[i] = a[i - 1];
                                a[i]++;
                                if (a[i] > Range)
                                    a[i] = 0;
                            }
                            if (ctag.Value.Value != a)
                                ctag.Value.Value = a;
                        }
                    }
                    break;
                default:
                    return;
            }
        }
        void UpdateRandom(Tag ctag)
        {
            switch ((uint)ctag.TagNode.DataType.Identifier)
            {
                case (uint)BuiltInType.String:
                    {
                        if (ctag.TagNode.ArrayDimension == 0)
                            ctag.Value.Value = GetRandomString();
                        else
                        {
                            string[] a = new string[ctag.TagNode.ArrayDimension];
                            for (int i = 0; i < ctag.TagNode.ArrayDimension; i++)
                            {
                                a[i] = GetRandomString();
                            }
                            if (ctag.Value.Value != a)
                                ctag.Value.Value = a;
                        }
                    }
                    break;
                case (uint)BuiltInType.Boolean:
                    {
                        if (ctag.TagNode.ArrayDimension == 0)
                        {
                            bool nv = Convert.ToBoolean(rndGen.Next(0, 2));
                            if (ctag.Value.Value == null || (Convert.ToBoolean(ctag.Value.Value) != nv))
                                ctag.Value.Value = nv;
                        }
                        else
                        {
                            bool[] a = new bool[ctag.TagNode.ArrayDimension];
                            for (int i = 0; i < ctag.TagNode.ArrayDimension; i++)
                                a[i] = Convert.ToBoolean(rndGen.Next(0, 2));
                            if (ctag.Value.Value != a)
                                ctag.Value.Value = a;
                        }
                    }
                    break;
                case (uint)BuiltInType.Byte:
                    {
                        if (ctag.TagNode.ArrayDimension == 0)
                        {
                            byte nv = Convert.ToByte(RandomValue * Range);
                            if (ctag.Value.Value == null || Convert.ToByte(ctag.Value.Value) != nv)
                                ctag.Value.Value = nv;
                        }
                        else
                        {
                            byte[] a = new byte[ctag.TagNode.ArrayDimension];
                            for (int i = 0; i < ctag.TagNode.ArrayDimension; i++)
                                a[i] = Convert.ToByte(RandomValue * Range);
                            if (ctag.Value.Value != a)
                                ctag.Value.Value = a;
                        }
                    }
                    break;
                case (uint)BuiltInType.Double:
                    {
                        if (ctag.TagNode.ArrayDimension == 0)
                        {
                            double nv = (RandomValue * Range);
                            if (ctag.Value.Value == null || Convert.ToDouble(ctag.Value.Value) != nv)
                                ctag.Value.Value = nv;
                        }
                        else
                        {
                            double[] a = new double[ctag.TagNode.ArrayDimension];
                            for (int i = 0; i < ctag.TagNode.ArrayDimension; i++)
                                a[i] = (RandomValue * Range);
                            if (ctag.Value.Value != a)
                                ctag.Value.Value = a;
                        }
                    }
                    break;
                case (uint)BuiltInType.Float:
                    {
                        if (ctag.TagNode.ArrayDimension == 0)
                        {
                            float nv = Convert.ToSingle(RandomValue * Range);
                            if (ctag.Value.Value == null || Convert.ToSingle(ctag.Value.Value) != nv)
                                ctag.Value.Value = nv;
                        }
                        else
                        {
                            float[] a = new float[ctag.TagNode.ArrayDimension];
                            for (int i = 0; i < ctag.TagNode.ArrayDimension; i++)
                                a[i] = Convert.ToSingle(RandomValue * Range);
                            if (ctag.Value.Value != a)
                                ctag.Value.Value = a;
                        }
                    }
                    break;
                case (uint)BuiltInType.Int16:
                    {
                        if (ctag.TagNode.ArrayDimension == 0)
                        {
                            Int16 nv = Convert.ToInt16(RandomValue * Range);
                            if (ctag.Value.Value == null || Convert.ToInt16(ctag.Value.Value) != nv)
                                ctag.Value.Value = nv;
                        }
                        else
                        {
                            Int16[] a = new Int16[ctag.TagNode.ArrayDimension];
                            for (int i = 0; i < ctag.TagNode.ArrayDimension; i++)
                                a[i] = Convert.ToInt16(RandomValue * Range);
                            if (ctag.Value.Value != a)
                                ctag.Value.Value = a;
                        }
                    }
                    break;
                case (uint)BuiltInType.Int32:
                    {
                        if (ctag.TagNode.ArrayDimension == 0)
                        {
                            Int32 nv = Convert.ToInt32(RandomValue * Range);
                            if (ctag.Value.Value == null || Convert.ToInt32(ctag.Value.Value) != nv)
                                ctag.Value.Value = nv;
                        }
                        else
                        {
                            Int32[] a = new Int32[ctag.TagNode.ArrayDimension];
                            for (int i = 0; i < ctag.TagNode.ArrayDimension; i++)
                                a[i] = Convert.ToInt32(RandomValue * Range);
                            if (ctag.Value.Value != a)
                                ctag.Value.Value = a;
                        }
                    }
                    break;
                case (uint)BuiltInType.Int64:
                    {
                        if (ctag.TagNode.ArrayDimension == 0)
                        {
                            Int64 nv = Convert.ToInt64(RandomValue * Range);
                            if (ctag.Value.Value == null || Convert.ToInt64(ctag.Value.Value) != nv)
                                ctag.Value.Value = nv;
                        }
                        else
                        {
                            Int64[] a = new Int64[ctag.TagNode.ArrayDimension];
                            for (int i = 0; i < ctag.TagNode.ArrayDimension; i++)
                                a[i] = Convert.ToInt64(RandomValue * Range);
                            if (ctag.Value.Value != a)
                                ctag.Value.Value = a;
                        }
                    }
                    break;
                case (uint)BuiltInType.Integer:
                    {
                        if (ctag.TagNode.ArrayDimension == 0)
                        {
                            int nv = (int)(RandomValue * Range);
                            if (ctag.Value.Value == null || Convert.ToInt32(ctag.Value.Value) != nv)
                                ctag.Value.Value = nv;
                        }
                        else
                        {
                            int[] a = new int[ctag.TagNode.ArrayDimension];
                            for (int i = 0; i < ctag.TagNode.ArrayDimension; i++)
                                a[i] = (int)(RandomValue * Range);
                            if (ctag.Value.Value != a)
                                ctag.Value.Value = a;
                        }
                    }
                    break;
                case (uint)BuiltInType.SByte:
                    {
                        if (ctag.TagNode.ArrayDimension == 0)
                        {
                            sbyte nv = Convert.ToSByte(RandomValue * Range);
                            if (ctag.Value.Value == null || Convert.ToSByte(ctag.Value.Value) != nv)
                                ctag.Value.Value = nv;
                        }
                        else
                        {
                            sbyte[] a = new sbyte[ctag.TagNode.ArrayDimension];
                            for (int i = 0; i < ctag.TagNode.ArrayDimension; i++)
                                a[i] = Convert.ToSByte(RandomValue * Range);
                            if (ctag.Value.Value != a)
                                ctag.Value.Value = a;
                        }
                    }
                    break;
                case (uint)BuiltInType.UInt16:
                    {
                        if (ctag.TagNode.ArrayDimension == 0)
                        {
                            UInt16 nv = Convert.ToUInt16(RandomValue * Range);
                            if (ctag.Value.Value == null || Convert.ToUInt16(ctag.Value.Value) != nv)
                                ctag.Value.Value = nv;
                        }
                        else
                        {
                            UInt16[] a = new UInt16[ctag.TagNode.ArrayDimension];
                            for (int i = 0; i < ctag.TagNode.ArrayDimension; i++)
                                a[i] = Convert.ToUInt16(RandomValue * Range);
                            if (ctag.Value.Value != a)
                                ctag.Value.Value = a;
                        }
                    }
                    break;
                case (uint)BuiltInType.UInt32:
                    {
                        if (ctag.TagNode.ArrayDimension == 0)
                        {
                            UInt32 nv = Convert.ToUInt32(RandomValue * Range);
                            if (ctag.Value.Value == null || Convert.ToUInt32(ctag.Value.Value) != nv)
                                ctag.Value.Value = nv;
                        }
                        else
                        {
                            UInt32[] a = new UInt32[ctag.TagNode.ArrayDimension];
                            for (int i = 0; i < ctag.TagNode.ArrayDimension; i++)
                                a[i] = Convert.ToUInt32(RandomValue * Range);
                            if (ctag.Value.Value != a)
                                ctag.Value.Value = a;
                        }
                    }
                    break;
                case (uint)BuiltInType.UInt64:
                    {
                        if (ctag.TagNode.ArrayDimension == 0)
                        {
                            UInt64 nv = Convert.ToUInt64(RandomValue * Range);
                            if (ctag.Value.Value == null || Convert.ToUInt64(ctag.Value.Value) != nv)
                                ctag.Value.Value = nv;
                        }
                        else
                        {
                            UInt64[] a = new UInt64[ctag.TagNode.ArrayDimension];
                            for (int i = 0; i < ctag.TagNode.ArrayDimension; i++)
                                a[i] = Convert.ToUInt64(RandomValue * Range);
                            if (ctag.Value.Value != a)
                                ctag.Value.Value = a;
                        }
                    }
                    break;
                case (uint)BuiltInType.UInteger:
                    {
                        if (ctag.TagNode.ArrayDimension == 0)
                        {
                            uint nv = (uint)(RandomValue * Range);
                            if (ctag.Value.Value == null || Convert.ToUInt32(ctag.Value.Value) != nv)
                                ctag.Value.Value = nv;
                        }
                        else
                        {
                            uint[] a = new uint[ctag.TagNode.ArrayDimension];
                            for (int i = 0; i < ctag.TagNode.ArrayDimension; i++)
                                a[i] = (uint)(RandomValue * Range);
                            if (ctag.Value.Value != a)
                                ctag.Value.Value = a;
                        }
                    }
                    break;
                default:
                    return;
            }
        }
        void UpdateSin(Tag ctag)
        {
            switch ((uint)ctag.TagNode.DataType.Identifier)
            {
                case (uint)BuiltInType.String:
                    {
                        if (ctag.TagNode.ArrayDimension == 0)
                            ctag.Value.Value = GetRandomString();
                        else
                        {
                            string[] a = new string[ctag.TagNode.ArrayDimension];
                            for (int i = 0; i < ctag.TagNode.ArrayDimension; i++)
                            {
                                a[i] = GetRandomString();
                            }
                            if (ctag.Value.Value != a)
                                ctag.Value.Value = a;
                        }
                    }
                    break;
                case (uint)BuiltInType.Byte:
                    {

                        if (ctag.TagNode.ArrayDimension == 0)
                        {
                            double d = (1 + SinValue) * Range / 2;
                            byte nv = Convert.ToByte(d);
                            if (Convert.ToByte(ctag.Value.Value) != nv)
                                ctag.Value.Value = nv;
                        }
                        else
                        {
                            byte[] a = new byte[ctag.TagNode.ArrayDimension];
                            for (int i = 0; i < ctag.TagNode.ArrayDimension; i++)
                            {
                                double d = (1 + SinValue) * Range / 2;
                                a[i] = Convert.ToByte(Convert.ToByte(d));
                            }
                            if (ctag.Value.Value != a)
                                ctag.Value.Value = a;
                        }
                    }
                    break;
                case (uint)BuiltInType.Double:
                    {
                        if (ctag.TagNode.ArrayDimension == 0)
                        {
                            if (Convert.ToDouble(ctag.Value.Value) != SinValue)
                                ctag.Value.Value = SinValue;
                        }
                        else
                        {
                            double[] a = new double[ctag.TagNode.ArrayDimension];
                            for (int i = 0; i < ctag.TagNode.ArrayDimension; i++)
                                a[i] = SinValue;
                            if (ctag.Value.Value != a)
                                ctag.Value.Value = a;
                        }
                    }
                    break;
                case (uint)BuiltInType.Float:
                    {
                        if (ctag.TagNode.ArrayDimension == 0)
                        {
                            if (Convert.ToSingle(ctag.Value.Value) != Convert.ToSingle(SinValue))
                                ctag.Value.Value = Convert.ToSingle(SinValue);
                        }
                        else
                        {
                            float[] a = new float[ctag.TagNode.ArrayDimension];
                            for (int i = 0; i < ctag.TagNode.ArrayDimension; i++)
                                a[i] = Convert.ToSingle(SinValue);
                            if (ctag.Value.Value != a)
                                ctag.Value.Value = a;
                        }
                    }
                    break;
                case (uint)BuiltInType.Int16:
                    {
                        if (ctag.TagNode.ArrayDimension == 0)
                        {
                            Int16 nv = Convert.ToInt16(SinValue * Range / 2);
                            if (Convert.ToInt16(ctag.Value.Value) != nv)
                                ctag.Value.Value = nv;
                        }
                        else
                        {
                            Int16[] a = new Int16[ctag.TagNode.ArrayDimension];
                            for (int i = 0; i < ctag.TagNode.ArrayDimension; i++)
                                a[i] = Convert.ToInt16(SinValue * Range / 2);
                            if (ctag.Value.Value != a)
                                ctag.Value.Value = a;
                        }
                    }
                    break;
                case (uint)BuiltInType.Int32:
                    {
                        if (ctag.TagNode.ArrayDimension == 0)
                        {
                            Int32 nv = Convert.ToInt32(SinValue * Range / 2);
                            if (Convert.ToInt32(ctag.Value.Value) != nv)
                                ctag.Value.Value = nv;
                        }
                        else
                        {
                            Int32[] a = new Int32[ctag.TagNode.ArrayDimension];
                            for (int i = 0; i < ctag.TagNode.ArrayDimension; i++)
                                a[i] = Convert.ToInt32(SinValue * Range / 2);
                            if (ctag.Value.Value != a)
                                ctag.Value.Value = a;
                        }
                    }
                    break;
                case (uint)BuiltInType.Int64:
                    {
                        if (ctag.TagNode.ArrayDimension == 0)
                        {
                            Int64 nv = Convert.ToInt64(SinValue * Range / 2);
                            if (Convert.ToInt64(ctag.Value.Value) != nv)
                                ctag.Value.Value = nv;
                        }
                        else
                        {
                            Int64[] a = new Int64[ctag.TagNode.ArrayDimension];
                            for (int i = 0; i < ctag.TagNode.ArrayDimension; i++)
                                a[i] = Convert.ToInt64(SinValue * Range / 2);
                            if (ctag.Value.Value != a)
                                ctag.Value.Value = a;
                        }

                    }
                    break;
                case (uint)BuiltInType.Integer:
                    {
                        if (ctag.TagNode.ArrayDimension == 0)
                        {
                            if ((int)(ctag.Value.Value) != (int)(SinValue * Range / 2))
                                ctag.Value.Value = (int)(SinValue * Range / 2);
                        }
                        else
                        {
                            int[] a = new int[ctag.TagNode.ArrayDimension];
                            for (int i = 0; i < ctag.TagNode.ArrayDimension; i++)
                                a[i] = (int)(SinValue * Range / 2);
                            if (ctag.Value.Value != a)
                                ctag.Value.Value = a;
                        }

                    }
                    break;
                case (uint)BuiltInType.SByte:
                    {
                        if (ctag.TagNode.ArrayDimension == 0)
                        {
                            sbyte nv = Convert.ToSByte(SinValue * Range / 2);
                            if (Convert.ToSByte(ctag.Value.Value) != nv)
                                ctag.Value.Value = nv;
                        }
                        else
                        {
                            sbyte[] a = new sbyte[ctag.TagNode.ArrayDimension];
                            for (int i = 0; i < ctag.TagNode.ArrayDimension; i++)
                                a[i] = Convert.ToSByte(SinValue * Range / 2);
                            if (ctag.Value.Value != a)
                                ctag.Value.Value = a;
                        }

                    }
                    break;
                case (uint)BuiltInType.UInt16:
                    {
                        if (ctag.TagNode.ArrayDimension == 0)
                        {
                            UInt16 nv = Convert.ToUInt16((1 + SinValue) * Range / 2);
                            if (Convert.ToUInt16(ctag.Value.Value) != nv)
                                ctag.Value.Value = nv;
                        }
                        else
                        {
                            UInt16[] a = new UInt16[ctag.TagNode.ArrayDimension];
                            for (int i = 0; i < ctag.TagNode.ArrayDimension; i++)
                            {
                                UInt16 nv = Convert.ToByte((1 + SinValue) * Range / 2);
                                a[i] = Convert.ToUInt16(nv);
                            }
                            if (ctag.Value.Value != a)
                                ctag.Value.Value = a;
                        }
                    }
                    break;
                case (uint)BuiltInType.UInt32:
                    {
                        if (ctag.TagNode.ArrayDimension == 0)
                        {
                            UInt32 nv = Convert.ToUInt32((1 + SinValue) * Range / 2);
                            if (Convert.ToUInt32(ctag.Value.Value) != nv)
                                ctag.Value.Value = nv;
                        }
                        else
                        {
                            UInt32[] a = new UInt32[ctag.TagNode.ArrayDimension];
                            for (int i = 0; i < ctag.TagNode.ArrayDimension; i++)
                            {
                                UInt32 nv = Convert.ToByte((1 + SinValue) * Range / 2);
                                a[i] = Convert.ToUInt32(nv);
                            }
                            if (ctag.Value.Value != a)
                                ctag.Value.Value = a;
                        }
                    }
                    break;
                case (uint)BuiltInType.UInt64:
                    {
                        if (ctag.TagNode.ArrayDimension == 0)
                        {
                            UInt64 nv = Convert.ToUInt64((1 + SinValue) * Range / 2);
                            if (Convert.ToUInt64(ctag.Value.Value) != nv)
                                ctag.Value.Value = nv;
                        }
                        else
                        {
                            UInt64[] a = new UInt64[ctag.TagNode.ArrayDimension];
                            for (int i = 0; i < ctag.TagNode.ArrayDimension; i++)
                            {
                                UInt64 nv = Convert.ToByte((1 + SinValue) * Range / 2);
                                a[i] = Convert.ToUInt64(nv);
                            }
                            if (ctag.Value.Value != a)
                                ctag.Value.Value = a;
                        }
                    }
                    break;
                case (uint)BuiltInType.UInteger:
                    {
                        if (ctag.TagNode.ArrayDimension == 0)
                        {
                            double d = (SinValue + 1) * Range / 2;
                            if ((uint)(ctag.Value.Value) != (uint)(d))
                                ctag.Value.Value = (uint)(d);
                        }
                        else
                        {
                            uint[] a = new uint[ctag.TagNode.ArrayDimension];
                            for (int i = 0; i < ctag.TagNode.ArrayDimension; i++)
                            {
                                double d = (SinValue + 1) * Range / 2;
                                a[i] = (uint)d;
                            }
                            if (ctag.Value.Value != a)
                                ctag.Value.Value = a;
                        }

                    }
                    break;
                default:
                    return;
            }
        }
        public override void ExecuteJob(CommJob job)
        {
            DemoCommJob dj = job as DemoCommJob;
            if (dj == null) return;

            base.ExecuteJob(job);
            if(SynchroJob != null)
            {
                SynchroJob.EndSynchroExec.Set();
                SynchroJob = null;
            }

            if (SimulationRunning)
            {
                //do the simulation
                
                if (dj.SimulationRunning)
                {
                    var simtime = (dj.SimulationInterval < 0 ? Convert.ToDouble(((DemoStation)dj.Station).SimulationInterval) : Convert.ToDouble(dj.SimulationInterval));
                    DateTime UtcNow = DateTime.UtcNow;
                    if ((UtcNow - dj.lastTrigoTime).TotalMilliseconds >= simtime)
                    {
                        if (!dj.CommonVal.HasValue)
                        {
                            if (dj.DemoType == DemoTypes.Ramp)
                                dj.CommonVal = (double)((int)(rndGen.NextDouble() * StepSimulazione)) / StepSimulazione;
                            else
                                dj.CommonVal = 0;
                        }
                        else
                        {
                            double d = (double)((int)(Convert.ToDouble(dj.CommonVal) * StepSimulazione) + 1) / StepSimulazione;
                            if (d > 1)
                                d = 0;
                            dj.CommonVal = d;

                        }
                        dj.lastTrigoTime = UtcNow;
                    }

                    if ((UtcNow - dj.LastExecutionTime).TotalMilliseconds < simtime)
                    {
                        RemovePendingJob(job);
                        job.IsPending = false;
                        LastErrorCode = DriverCodeBase.Enumerators.DriverErrorCodes.ErrorNoError;
                        return;
                    }
                    else
                    {
                        foreach (var t in dj.TagsList)
                        {
                            if (t.TagNode.DataType.IdType == Opc.Ua.IdType.Numeric)
                            {
                                if (dj.DemoType == DemoTypes.Sin)
                                    UpdateSin(t, dj);
                                else if (dj.DemoType == DemoTypes.Cos)
                                    UpdateCos(t, dj);
                                else if (dj.DemoType == DemoTypes.Ramp)
                                    UpdateRamp(t, dj);
                                else if (dj.DemoType == DemoTypes.Random)
                                    UpdateRandom(t);
                            }
                        }
                        dj.BeenExecuted = true;
                        //System.Diagnostics.Trace.TraceInformation(DateTime.Now.ToLongTimeString());
                    }
                }
                else
                    LastErrorCode = DriverCodeBase.Enumerators.DriverErrorCodes.ErrorNoError;

            }
            else
            {
                LastErrorCode = DriverCodeBase.Enumerators.DriverErrorCodes.ErrorNoError;
            }

            ExecutedJobArgs eJob = new ExecutedJobArgs { ErrorCode = LastErrorCode, Job = job };
            OnJobExecuted(eJob);
            LastErrorCode = DriverCodeBase.Enumerators.DriverErrorCodes.ErrorNoError;
            SetNewDataEvent();

        }

        #endregion

        private string[] words = { "To be, ", "or not to be: ", "that is the question: ", "Whether 'tis nobler", " in the mind to suffer", 
                                    " The slings and arrows of ", "outrageous fortune, ", "Or to take arms", " against a sea", " of troubles, And ", 
                                    "by opposing end them?", " To die: to sleep;", " No more; ", "and by a sleep", " to say we end" };

        private string GetRandomString()
        {
            int idx = (int)(rndGen.NextDouble() * words.Length);
            return words[idx];
        }

        #region Abstracts Methods

        public override bool IsDeviceOpen(){ return true; }
        public override bool DeviceOpen() { return true; }
        public override bool DeviceClose() { return true; }
        public override bool DeviceRead(byte[] Buffer, uint Count) { return true; }
        public override bool DeviceWrite(byte[] Buffer, uint Count) { return true;  }
        public override uint GetBytesToRead() { return 1; }
        public override uint GetBytesToWrite() { return 1; }

        #endregion

        private bool _SimulationRunning = true;
        public bool SimulationRunning
        {
            get { return _SimulationRunning; }
            set
            {
                _SimulationRunning = value;
            }
        }

        private double _SinValue;
        public double SinValue
        {
            get { return _SinValue; }
            set
            {
                _SinValue = value;
            }
        }
        private double _CosValue;
        public double CosValue
        {
            get { return _CosValue; }
            set
            {
                _CosValue = value;
            }
        }
        private double _RampValue;
        public double RampValue
        {
            get { return _RampValue; }
            set
            {
                _RampValue = value;
            }
        }
        private double _RandomValue;
        public double RandomValue
        {
            get {
                _RandomValue = rndSimul.NextDouble();
                return _RandomValue; }
        }

        public override void  Dispose()
        {
 	         base.Dispose();
        }
    }

}
