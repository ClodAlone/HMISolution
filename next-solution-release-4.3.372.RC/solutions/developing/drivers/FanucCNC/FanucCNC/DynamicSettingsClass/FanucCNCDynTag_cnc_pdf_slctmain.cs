using System;
using DriverCodeBase.Enumerators;
using System.Text;
using System.ComponentModel;

namespace FanucCNC
{
    public class FanucCNCDynTag_cnc_pdf_slctmain: FanucCNCDynTag_BaseFunction
	{
		const String CNCSourcePathParameter = "CNCSOURCEPATH";

		public FanucCNCDynTag_cnc_pdf_slctmain(FanucCNCDynTagSettings main, string settings) : base(main, settings)
        {
			StringLength = FanucCNCProtocol.MAX_STRING_SIZE_PARAMETER;
			CNCPathSupported = true;
		}

		public override void SplitSettings(string settings)
		{
			_CNCSourcePath = GetPartByName(CNCSourcePathParameter);

			IsValid = true;
		}

		public override string CreateSettings()
		{
			var parameters = new StringBuilder(); // base.ToString());

			if (!string.IsNullOrEmpty(_CNCSourcePath.Trim()))
				AddParameter(ref parameters, CNCSourcePathParameter, _CNCSourcePath.Trim());		
			return parameters.ToString();
		}

		public DriverErrorCodes PrepareWriteRequest(byte[] jobdata, short cnc_path, out string outPath)
		{			
			outPath = string.Format("{0}{1}", _CNCSourcePath, ASCIIEncoding.ASCII.GetString(jobdata).TrimEnd('\0').Trim());

			outPath = outPath.Replace("{cnc_path}", cnc_path.ToString());

			return DriverErrorCodes.ErrorNoError;
		}

		#region IDataErrorInfo Members
		public override String PerformValidation(String propertyName)
		{
			switch (propertyName)
			{
				case "FunctionCode":
					if (Main.VarType != UFUAModel.DataType.String)
						return Properties.Resources.ErrorOnlyStringDataTypeAllowed;
					break;

				case "CNCSourcePath":
					//if (string.IsNullOrWhiteSpace(_CNCSourcePath))
					//    return Properties.Resources.ErrorAddressEmpty;

					//if (Main.VarType != UFUAModel.DataType.String)
					//	return Properties.Resources.ErrorOnlyStringDataTypeAllowed;
					break;
				case "TagLinkType":
					if (Main.TagLinkType == (int)LinkType.Input || Main.TagLinkType == (int)LinkType.InputOutput)
						return Properties.Resources.ErrorLinkTypeInvalid;
					break;
			}

			return null;
		}
		#endregion

		#region Properties
		private string _CNCSourcePath;
		public string CNCSourcePath
		{
			get { return _CNCSourcePath; }
			set { _CNCSourcePath = value; }
		}
		#endregion

		#region INotifyPropertyChanged Members
		public override void OnPropertyChanged(string propertyName)
		{
			switch (propertyName)
			{
				case "DataType":
					OnPropertyChanged(new PropertyChangedEventArgs("CNCSourcePath"));
					break;
			}
		}
		#endregion
	}
}
