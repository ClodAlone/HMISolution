using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

using WinWrap.Basic;
using System.Threading;

namespace ScriptVariableValues
{
    [ClassInterface(ClassInterfaceType.AutoDual)]
    [ComVisible(true)]
    public class VariableValues : IReDispatch, System.Runtime.InteropServices.ComTypes.ITypeInfo, IDisposable
    {
        readonly Dictionary<String, int> idLookup = new Dictionary<String, int>(StringComparer.OrdinalIgnoreCase);
        readonly Dictionary<int, String> idReverseLookup = new Dictionary<int, String>();
        readonly Dictionary<int, object> values = new Dictionary<int, object>();
        readonly List<String> resolvedItems = new List<String>();
        readonly List<String> needToBeresolvedItems = new List<String>();
        readonly List<String> isReadOnlyItems = new List<String>();

        readonly String instancename;
        bool bResolveVariables;
        // bool bFoundVariableNameSuspect;

        public static readonly String initialQuality = new Opc.Ua.StatusCode(Opc.Ua.StatusCodes.BadWaitingForInitialData).ToString();

        public VariableValues(bool bResolveVar = true)
        {
            bResolveVariables = bResolveVar;
        }

        public VariableValues(String n, bool bResolveVar = true)
        {
            instancename = n.Replace('\\', '_');
            bResolveVariables = bResolveVar;
        }

        public String GetName()
        {
            return instancename;
        }

        public bool isDataService { get; set; }

        List<String> idLookupChecking;
        int nCounter = 0;
        public void PrepareAddNewVariable()
        {
            idLookupChecking = new List<String>();
            needToBeresolvedItems.Clear();
        }

        public void EndAddNewVariable(bool useParallel = true)
        {
            lock(values)
            {
                List<string> list;
                if(useParallel)
                    list = (from c in idLookup.Keys.AsParallel()
                            where !idLookupChecking.Contains(c)
                            select c).ToList();
                else
                    list = (from c in idLookup.Keys/*.AsParallel()*/
                            where !idLookupChecking.Contains(c)
                            select c).ToList();

                list.ForEach(name =>
                    {
                        var hash = idLookup[name];
                        idLookup.Remove(name);
                        idReverseLookup.Remove(hash);
                        values.Remove(hash);
                        if (resolvedItems.Contains(name))
                            resolvedItems.Remove(name);
                    });
                idLookupChecking = null;
            }
        }

        public void SetVariableValue(String Name, Object value)
        {
            lock(values)
            {
                Name = Name.Replace('\\', '_');

                if (!idLookup.ContainsKey(Name) || !values.ContainsKey(idLookup[Name]))
                    return;
                values[idLookup[Name]] = value;
            }
        }

        public Object GetVariableValue(String Name)
        {
            lock(values)
            {
                Name = Name.Replace('\\', '_');

                if (!idLookup.ContainsKey(Name) || !values.ContainsKey(idLookup[Name]))
                    return null;
#if DEBUG
                System.Diagnostics.Debug.WriteLine(String.Format("Script returned variable value {0}", values[idLookup[Name]]));
#endif
                return values[idLookup[Name]];
            }
        }

        void RemoveVariable(String Name)
        {
            lock(values)
            {
                var realName = Name;
                Name = Name.Replace('\\', '_');
                if (!idLookup.ContainsKey(Name))
                    return;

                var id = idLookup[Name];
                idLookup.Remove(Name);

                if (!idReverseLookup.ContainsKey(id))
                    return;
                idReverseLookup.Remove(id);
            }
        }

        List<String> idLookupList;
        public void AddList(List<String> list)
        {
            if (idLookupList == null)
                idLookupList = new List<String>();
            else
                idLookupList.Clear();
            list.ForEach(e =>
            {
                var name = e.Replace('\\', '_');
                idLookupList.Add(name);
                var quality = String.Format(qualityTag, name);
                idLookupList.Add(quality);
                var timeStamp = String.Format(timestampTag, name);
                idLookupList.Add(timeStamp);
            });
        }

        public void AddVariable(String Name, bool bNeedToResolve = true, bool bIsReadOnly = false)
        {
            lock(values)
            {
                var realName = Name;
                Name = Name.Replace('\\', '_');
                if (!idLookup.ContainsKey(Name))
                {

                    // int hash = Name.GetHashCode();
                    int hash = ++nCounter;
                    idLookup.Add(Name, hash);
                    idReverseLookup.Add(hash, realName);
                    values.Add(hash, null);
                }

                if (bNeedToResolve && !needToBeresolvedItems.Contains(Name))
                    needToBeresolvedItems.Add(Name);

                if (idLookupChecking != null && !idLookupChecking.Contains(Name))
                    idLookupChecking.Add(Name);

                if (bIsReadOnly && !isReadOnlyItems.Contains(Name))
                    isReadOnlyItems.Add(Name);
            }
        }

        public void TerminateResolvedItem()
        {
            resolvedItems.Clear();
        }

        // array specific code
        int[] GetIndicies(int rank, IntPtr rgvarg)
        {
            int[] indicies = new int[rank];
            for (int i = 0; i < rank; ++i)
            {
                // indices are in reverse order
                object index = Marshal.GetObjectForNativeVariant(rgvarg);
                indicies[rank - i - 1] = Convert.ToInt32(index);
                rgvarg += IntPtr.Size == 4 ? 16 : 24;
            }

            return indicies;
        }

        #region IReDispatch Members

        public void GetTypeInfoCount2(out int pctinfo)
        {
            pctinfo = 1;
        }

        public void GetTypeInfo2(int iTInfo, int lcid, out System.Runtime.InteropServices.ComTypes.ITypeInfo ppTInfo)
        {
            if (iTInfo != 0) throw Marshal.GetExceptionForHR((int)HR.DISP_E_BADINDEX);
            ppTInfo = this;
        }

        public void ForceResolveVariables()
        {
            bResolveVariables = true;
        }

        static readonly String qualityTag = "{0}Quality";
        static readonly String timestampTag = "{0}Timestamp";
       

        public void GetIDsOfNames2(ref Guid riid, string[] rgszNames, int cNames, int lcid, int[] rgDispId)
        {
            if (cNames != 1) throw Marshal.GetExceptionForHR((int)HR.E_INVALIDARG);
            String name = rgszNames[0];
            //if (!bFoundVariableNameSuspect && bResolveVariables && name.Contains('_'))
            //{
            //    bFoundVariableNameSuspect = true;
            //    OnFoundVariableNameSuspect();
            //}

            if (!bResolveVariables && idLookupList != null)
            {
                if (idLookupList.Contains(name) && !idLookup.ContainsKey(name))
                    AddVariable(name, true, false);
                else
                {
                    var quality = String.Format(qualityTag, name);
                    var timeStamp = String.Format(timestampTag, name);
                    if (idLookupList.Contains(quality) && !idLookup.ContainsKey(quality))
                        AddVariable(quality, false, true);
                    if (idLookupList.Contains(timeStamp) && !idLookup.ContainsKey(timeStamp))
                        AddVariable(timeStamp, false, true);
                }
            }

            if (idLookup.ContainsKey(name))
            {
                rgDispId[0] = idLookup[name];
                if (bResolveVariables && !resolvedItems.Contains(name) && needToBeresolvedItems.Contains(name))
                {
                    resolvedItems.Add(name);
                    OnResolveVariable(new ResolveVariableEventArgs(idReverseLookup[rgDispId[0]]));
                }
            }
            else
            {
                try
                {
                    AddVariable(name, false);
                    OnResolveVariable(new ResolveVariableEventArgs(name));
                    lock(values)
                    {
                        rgDispId[0] = idLookup[name];
                        if (!values.ContainsKey(rgDispId[0]))
                            SetVariableValue(name, null);
                    }
                }
                catch
                {
                    RemoveVariable(name);
                    throw Marshal.GetExceptionForHR((int)HR.E_INVALIDARG);
                }
            }
        }

        public void VerifyResolvedVariable(String name)
        {
            String Name = name.Replace('\\', '_');
            if (!resolvedItems.Contains(Name)/* && needToBeresolvedItems.Contains(Name)*/)
            {
                // resolvedItems.Add(name);
                try
                { 
                    AddVariable(name, false);
                    OnResolveVariable(new ResolveVariableEventArgs(name));
                }
                catch
                {
                    RemoveVariable(name);
                    throw Marshal.GetExceptionForHR((int)HR.E_INVALIDARG);
                }
            }
        }

        public void Invoke2(int dispIdMember, ref Guid riid, int lcid, short wFlags, ref System.Runtime.InteropServices.ComTypes.DISPPARAMS pDispParams, ref object pVarResult, ref System.Runtime.InteropServices.ComTypes.EXCEPINFO pExcepInfo, ref int puArgErr)
        {
            Object value = null;
            if (bResolveVariables)
            {
                var timeout = DateTime.UtcNow + TimeSpan.FromSeconds(10);
                lock (values)
                {
                    value = values[dispIdMember];
                }

                var isQuality = false;
                var isTimestamp = false;
                if (isReadOnlyItems.Contains(idReverseLookup[dispIdMember]))
                {
                    if (idReverseLookup[dispIdMember].EndsWith(String.Format(qualityTag, String.Empty)))
                        isQuality = true;
                    else if (idReverseLookup[dispIdMember].EndsWith(String.Format(timestampTag, String.Empty)))
                        isTimestamp = true;
                }

                if (isQuality)
                {
                    var quality = value as String;
                    while ((quality == null || quality == initialQuality) && timeout > DateTime.UtcNow)
                    {
                        Thread.Sleep(200);
                        lock (values)
                        {
                            value = values[dispIdMember];
                        }

                        quality = value as String;
                    }
                }
                else if (isTimestamp)
                {
                    var timestamp = value is DateTime ? (DateTime)value : DateTime.MinValue;
                    while (timestamp == DateTime.MinValue && timeout > DateTime.UtcNow)
                    {
                        Thread.Sleep(200);
                        lock (values)
                        {
                            value = values[dispIdMember];
                        }

                        timestamp = value is DateTime ? (DateTime)value : DateTime.MinValue;
                    }
                }
                else
                {
                    while (value == null && timeout > DateTime.UtcNow)
                    {
                        var quality = String.Format(qualityTag, idReverseLookup[dispIdMember]);
                        if (isReadOnlyItems.Contains(quality))
                        {
                            lock (values)
                            {
                                value = values[dispIdMember];
                                quality = values[idLookup[quality]] as String;
                            }

                            if (!String.IsNullOrEmpty(quality) && quality.StartsWith("Good"))
                                break;
                        }

                        Thread.Sleep(200);
                        lock (values)
                        {
                            value = values[dispIdMember];
                        }
                    }
                }
            }
            //else
            //    value = 0;

            // array specific code
            Array array = value as Array;
            int rank = array == null ? 0 : array.Rank;
            // ---

            const int DISPID_PROPERTYPUT = -3;
            const short INVOKE_PROPERTYGET = (short)System.Runtime.InteropServices.ComTypes.INVOKEKIND.INVOKE_PROPERTYGET;
            const short INVOKE_PROPERTYPUT = (short)System.Runtime.InteropServices.ComTypes.INVOKEKIND.INVOKE_PROPERTYPUT;
            const short INVOKE_PROPERTYPUTREF = (short)System.Runtime.InteropServices.ComTypes.INVOKEKIND.INVOKE_PROPERTYPUTREF;
            const short INVOKE_FUNC = (short)System.Runtime.InteropServices.ComTypes.INVOKEKIND.INVOKE_FUNC;
            if ((wFlags & INVOKE_PROPERTYPUTREF) != 0)
            {
                // set a reference
                if (pDispParams.cArgs != 1 && pDispParams.cArgs != 1 + rank) throw Marshal.GetExceptionForHR((int)HR.E_INVALIDARG);
                if (pDispParams.cNamedArgs != 1) throw Marshal.GetExceptionForHR((int)HR.E_INVALIDARG);
                if (Marshal.ReadInt32(pDispParams.rgdispidNamedArgs) != DISPID_PROPERTYPUT) throw Marshal.GetExceptionForHR((int)HR.E_INVALIDARG);

                if (bResolveVariables && isReadOnlyItems.Contains(idReverseLookup[dispIdMember]))
                    throw new Exception(Properties.Resources.CannotWriteReadOnly);

                object NewValue = Marshal.GetObjectForNativeVariant(pDispParams.rgvarg);
                if (pDispParams.cArgs == 1)
                {
                    lock(values)
                    {
                        values[dispIdMember] = NewValue;
                    }
                    OnWriteVariable(new WriteVariableEventArgs(idReverseLookup[dispIdMember], NewValue));
                }
                else
                {
                    // array specific code
                    int[] indicies = GetIndicies(rank, pDispParams.rgvarg + (IntPtr.Size == 4 ? 16 : 24));
                    NewValue = Convert.ChangeType(NewValue, array.GetType().GetElementType());
                    array.SetValue(NewValue, indicies);
                    OnWriteVariable(new WriteVariableEventArgs(idReverseLookup[dispIdMember], array));
                }
            }
            else if ((wFlags & INVOKE_PROPERTYPUT) != 0)
            {
                // assign a value
                if (pDispParams.cArgs != 1 && pDispParams.cArgs != 1 + rank) throw Marshal.GetExceptionForHR((int)HR.E_INVALIDARG);
                if (pDispParams.cNamedArgs != 1) throw Marshal.GetExceptionForHR((int)HR.E_INVALIDARG);
                if (Marshal.ReadInt32(pDispParams.rgdispidNamedArgs) != DISPID_PROPERTYPUT) throw Marshal.GetExceptionForHR((int)HR.E_INVALIDARG);

                if (bResolveVariables && isReadOnlyItems.Contains(idReverseLookup[dispIdMember]))
                    throw new Exception(Properties.Resources.CannotWriteReadOnly);

                object NewValue = Marshal.GetObjectForNativeVariant(pDispParams.rgvarg);
                if (NewValue != null)
                {
                    Type t = NewValue.GetType();
                    if (!t.IsValueType)
                    {
                        // assign the default property value
                        System.Reflection.BindingFlags bf =
                            System.Reflection.BindingFlags.GetProperty |
                            System.Reflection.BindingFlags.Instance |
                            System.Reflection.BindingFlags.Public;
                        NewValue = t.InvokeMember("", bf, null, NewValue, null);
                    }
                }
                if (pDispParams.cArgs == 1)
                {
                    lock(values)
                    {
                        values[dispIdMember] = NewValue;
                    }
                    OnWriteVariable(new WriteVariableEventArgs(idReverseLookup[dispIdMember], NewValue));
                }
                else
                {
                    // array specific code
                    int[] indicies = GetIndicies(rank, pDispParams.rgvarg + (IntPtr.Size == 4 ? 16 : 24));
                    NewValue = Convert.ChangeType(NewValue, array.GetType().GetElementType());
                    array.SetValue(NewValue, indicies);
                    OnWriteVariable(new WriteVariableEventArgs(idReverseLookup[dispIdMember], array));
                }
            }
            else if ((wFlags & INVOKE_PROPERTYGET) != 0 || (wFlags & INVOKE_FUNC) != 0)
            {
                // get a value
                if (pDispParams.cArgs != 0 && pDispParams.cArgs != rank) throw Marshal.GetExceptionForHR((int)HR.E_INVALIDARG);
                if (pDispParams.cNamedArgs != 0) throw Marshal.GetExceptionForHR((int)HR.E_INVALIDARG);
                if (pDispParams.cArgs == 0)
                    pVarResult = value;
                else
                {
                    // array specific code
                    int[] indicies = GetIndicies(rank, pDispParams.rgvarg);
                    pVarResult = array.GetValue(indicies);
                }
            }
            else
                throw Marshal.GetExceptionForHR((int)HR.E_INVALIDARG);
        }

        #endregion

        // the ITypeInfo interface provides the info needed by auto completion

        private string GetNameFromID(int id)
        {
            /*
            foreach (KeyValuePair<string, int> kvp in idLookup)
                if (kvp.Value == id)
                    return kvp.Key;
            */
            if (idReverseLookup.ContainsKey(id))
                return idReverseLookup[id].Replace('\\', '_');
            if (idLookup.Count > 0)
            {
                var found = (from c in idLookup/*.AsParallel()*/ where c.Value == id select c.Key).ToList();
                if (found.Count > 0)
                    return found[0];
            }
            if (idLookupList != null && idLookupList.Count >= id)
                return idLookupList[id - 1].Replace('\\', '_');

            return null;
        }

        #region ITypeInfo Members

        public void AddressOfMember(int memid, System.Runtime.InteropServices.ComTypes.INVOKEKIND invKind, out IntPtr ppv)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void CreateInstance(object pUnkOuter, ref Guid riid, out object ppvObj)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void GetContainingTypeLib(out System.Runtime.InteropServices.ComTypes.ITypeLib ppTLB, out int pIndex)
        {
            ppTLB = null;
            pIndex = 0;
            // throw new Exception("The method or operation is not implemented.");
        }

        public void GetDllEntry(int memid, System.Runtime.InteropServices.ComTypes.INVOKEKIND invKind, IntPtr pBstrDllName, IntPtr pBstrName, IntPtr pwOrdinal)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void GetDocumentation(int index, out string strName, out string strDocString, out int dwHelpContext, out string strHelpFile)
        {
            if (index == -1)
            {
                // type's name
                strName = GetType().Name;
            }
            else if (!bResolveVariables)
            {
                // index is an id
                strName = GetNameFromID(index);
            }
            else
                strName = String.Empty;

            strDocString = null;
            dwHelpContext = 0;
            strHelpFile = null;
        }

        public void GetFuncDesc(int index, out IntPtr ppFuncDesc)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void GetIDsOfNames(string[] rgszNames, int cNames, int[] pMemId)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void GetImplTypeFlags(int index, out System.Runtime.InteropServices.ComTypes.IMPLTYPEFLAGS pImplTypeFlags)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void GetMops(int memid, out string pBstrMops)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void GetNames(int memid, string[] rgBstrNames, int cMaxNames, out int pcNames)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void GetRefTypeInfo(int hRef, out System.Runtime.InteropServices.ComTypes.ITypeInfo ppTI)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void GetRefTypeOfImplType(int index, out int href)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void GetTypeAttr(out IntPtr ppTypeAttr)
        {
            System.Runtime.InteropServices.ComTypes.TYPEATTR typeattr = new System.Runtime.InteropServices.ComTypes.TYPEATTR();
            typeattr.memidConstructor = -1;
            typeattr.memidDestructor = -1;
            typeattr.guid = new Guid("{00020400-0000-0000-C000-000000000046}"); // IID_IDispatch
            typeattr.cbSizeInstance = IntPtr.Size;
            typeattr.typekind = System.Runtime.InteropServices.ComTypes.TYPEKIND.TKIND_DISPATCH;
            typeattr.cFuncs = 0;
            typeattr.cVars = (short)(idLookup.Count + (idLookupList != null ? idLookupList.Count : 0));
            //typeattr.wTypeFlags = 0;
            // allocate unmanaged memory for structure and copy
            ppTypeAttr = Marshal.AllocHGlobal(Marshal.SizeOf(typeattr));
            Marshal.StructureToPtr(typeattr, ppTypeAttr, false);
        }

        public void GetTypeComp(out System.Runtime.InteropServices.ComTypes.ITypeComp ppTComp)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void GetVarDesc(int index, out IntPtr ppVarDesc)
        {
            // index starts at 0, ids start at 1
            System.Runtime.InteropServices.ComTypes.VARDESC vardesc = new System.Runtime.InteropServices.ComTypes.VARDESC();
            vardesc.memid = index + 1;
            vardesc.elemdescVar.tdesc.vt = (short)System.Runtime.InteropServices.VarEnum.VT_VARIANT;
            vardesc.varkind = System.Runtime.InteropServices.ComTypes.VARKIND.VAR_DISPATCH;
            // allocate unmanaged memory for structure and copy
            ppVarDesc = Marshal.AllocHGlobal(Marshal.SizeOf(vardesc));
            Marshal.StructureToPtr(vardesc, ppVarDesc, false);
        }

        public void Invoke(object pvInstance, int memid, short wFlags, ref System.Runtime.InteropServices.ComTypes.DISPPARAMS pDispParams, IntPtr pVarResult, IntPtr pExcepInfo, out int puArgErr)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void ReleaseFuncDesc(IntPtr pFuncDesc)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void ReleaseTypeAttr(IntPtr pTypeAttr)
        {
            Marshal.FreeHGlobal(pTypeAttr);
        }

        public void ReleaseVarDesc(IntPtr pVarDesc)
        {
            Marshal.FreeHGlobal(pVarDesc);
        }

        #endregion

        #region OnWriteVariable
        public event EventHandler<WriteVariableEventArgs> WriteVariable;
        /// <summary>
        /// Triggers the WriteVariable event.
        /// </summary>
        public virtual void OnWriteVariable(WriteVariableEventArgs ea)
        {
            if (WriteVariable != null)
                WriteVariable(null/*this*/, ea);
        }
        #endregion

        #region OnResolveVariable
        public event EventHandler<ResolveVariableEventArgs> ResolveVariable;
        /// <summary>
        /// Triggers the ResolveVariable event.
        /// </summary>
        public virtual void OnResolveVariable(ResolveVariableEventArgs ea)
        {
            //if (!bResolveVariables)
            //    return;

            if (ResolveVariable != null)
                ResolveVariable(null/*this*/, ea);
            else
                throw new Exception("Script Variables : No Resolver Listening !");
        }
        #endregion

        //#region OnFoundVariableNameSuspect
        //public event EventHandler FoundVariableNameSuspect;
        ///// <summary>
        ///// Triggers the FoundVariableNameSuspect event.
        ///// </summary>
        //public virtual void OnFoundVariableNameSuspect()
        //{
        //    if (FoundVariableNameSuspect != null)
        //        FoundVariableNameSuspect(null/*this*/, EventArgs.Empty);
        //}
        //#endregion

        public void Dispose()
        {
            // throw new NotImplementedException();
        }
    }
}