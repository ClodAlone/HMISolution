using System;
using DriverCodeBase;
using DriverBaseInterfaces;
using Opc.Ua;

namespace DICom
{
    public sealed class DIComTag : Tag
    {
        #region Constructors

        public DIComTag(TagDefinition tag, uint byteoffset, uint bitoffset)
            : base(tag, byteoffset, bitoffset)
        {
        }

        public DIComTag(TagDefinition tag)
            : base(tag)
        {

        }

        #endregion

        #region Override Properties/Functions

        public override bool BuildDynamicSettings(String dynamicSettings)
        {
            return DIComDynSettings.TryParse(dynamicSettings);
        }

        public override DynTagSettings DynSettings
        {
            get { return (DynTagSettings)DIComDynSettings; }
        }

        #endregion

        #region Properties

        readonly DIComDynTagSettings _DIComDynSettings = new DIComDynTagSettings();
        public DIComDynTagSettings DIComDynSettings
        {
            get { return _DIComDynSettings; }
        }

        /// <summary>   The read value. </summary>
        public override bool SetTagValue(ref byte[] buffer, int index, bool forceUpdate, uint elemsize = 0, uint buffertype = 0)
        {
            if (TagNode.DataType.IdType == IdType.Numeric)
            {
                bool bCopy = false;
                object val = new object();
                uint nType = (uint)TagNode.DataType.Identifier;
                if (forceUpdate)
                    ReadValue = null;
                switch (nType)
                {
                    case (uint)Opc.Ua.DataTypes.String:
                        {
                            //if (TagNode.ArrayDimension == 0)
                            //{
                                uint ssize = 0;
                                //switch (_V)
                                //{
                                //    case S7DataFormats.String:
                                        //{
                                            if (Size > buffer.Length)
                                                ssize = (uint)buffer.Length;
                                            else
                                                ssize = Size;
                                            byte[] tbuf = new byte[ssize];
                                            Array.Copy(buffer, index, tbuf, 0, ssize);
                                            val = new System.Text.UTF8Encoding().GetString(tbuf);
                                            //break;
                                        //}
                                    //case S7DataFormats.WString:
                                    //    {
                                    //        if (Size * 2 > buffer.Length)
                                    //            ssize = (uint)buffer.Length;
                                    //        else
                                    //            ssize = Size * 2;
                                    //        byte[] tbuf = new byte[ssize];
                                    //        Array.Copy(buffer, index, tbuf, 0, ssize);
                                    //        val = new System.Text.UTF8Encoding().GetString(System.Text.Encoding.Convert(System.Text.Encoding.Unicode, System.Text.Encoding.UTF8, tbuf));
                                    //        break;
                                    //    }
                                //}

                                bCopy = (ReadValue == null || (string)ReadValue != (string)val) || (LastValue != null && (string)LastValue != (string)val);
                            //}
                            //else
                            //{
                            //    string[] a = new string[TagNode.ArrayDimension];
                            //    Array b = ReadValue as Array;
                            //    bool check = (b != null && b.GetLength(0) == TagNode.ArrayDimension);
                            //    bCopy = !check;

                            //    int ssize = (int)(Size / TagNode.ArrayDimension);

                            //    for (int i = 0; i < TagNode.ArrayDimension; i++)
                            //    {
                            //        switch (_S7StringDataFormat)
                            //        {
                            //            case S7DataFormats.String:
                            //                {
                            //                    a[i] = new System.Text.UTF8Encoding().GetString(buffer, (i * ssize), ssize).Trim('\0');
                            //                    break;
                            //                }
                            //            case S7DataFormats.WString:
                            //                {
                            //                    string uni = new System.Text.UnicodeEncoding().GetString(buffer, (i * (ssize * 2)), ssize * 2).Trim('\0');
                            //                    a[i] = new System.Text.UTF8Encoding().GetString(System.Text.Encoding.Convert(System.Text.Encoding.Unicode, System.Text.Encoding.UTF8, new System.Text.UnicodeEncoding().GetBytes(uni)));
                            //                    break;
                            //                }
                            //        }
                            //        if (check && a[i] != (string)b.GetValue(i))
                            //            bCopy = true;
                            //    }
                            //    val = a;
                            //}
                            if (ReadValue == null || bCopy)
                            {
                                WriteVal = ReadValue = val;
                                Value.Value = val;
                                LastValue = Value.Value;
                                return true;
                            }
                        }
                        break;
                    default:
                        {
                            return base.SetTagValue(ref buffer, index, forceUpdate, elemsize, buffertype);
                        }
                        break;
                }

            }
            return (false);
        }

        #endregion        

    }
}
