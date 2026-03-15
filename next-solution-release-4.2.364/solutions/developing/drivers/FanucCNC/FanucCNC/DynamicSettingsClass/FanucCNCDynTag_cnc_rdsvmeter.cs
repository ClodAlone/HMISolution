using System;
using System.Collections.Generic;
using DriverCodeBase;
using DriverCodeBase.Enumerators;
using FanucCNC.Focas_Library;

namespace FanucCNC
{
	public class FanucCNCDynTag_cnc_rdsvmeter : FanucCNCDynTag_BaseFunction
	{
        public FanucCNCDynTag_cnc_rdsvmeter(FanucCNCDynTagSettings main, string settings) : base(main, settings)
        {
			CNCPathSupported = true;
		}

		public DriverErrorCodes ParseReadData(Focas1.LOADELM_custom[] dataRead, short nrAxis, out byte[] data)
		{
			int moviconDataTypeSize = FanucCNCProtocol.GetDataTypeSize(Main.VarType);

			data = new byte[_NrAxis * moviconDataTypeSize];

			if (nrAxis > _NrAxis)
				nrAxis = _NrAxis;

			for (int n = 0; n < nrAxis; n++)
			{
				ConvertToMoviconDataType(dataRead[n].data, out byte[] d);

				Array.Copy(d, 0, data, (n * moviconDataTypeSize), moviconDataTypeSize);
			}

			return DriverErrorCodes.ErrorNoError;
		}

		public DriverErrorCodes PrepareReadRequest(out Focas1.LOADELM_custom[] dataToWrite, out short num_Axis)
		{
			dataToWrite = new Focas1.LOADELM_custom[_NrAxis];

			num_Axis = _NrAxis;

			return DriverErrorCodes.ErrorNoError;
		}

		public override void CalculateInternalParameters(List<Tag> tagList = null, int offsetValue = 0, bool aggregation = false)
		{
			if (Main.ArrayDimension == 0)
				_NrAxis = 1;
			else
				_NrAxis = (short)Main.ArrayDimension;
		}

		#region IDataErrorInfo Members
		public override String PerformValidation(String propertyName)
		{
			switch (propertyName)
			{
				case "FunctionCode":
					if (Main.VarType == UFUAModel.DataType.String)
						return Properties.Resources.ErrorInvalidDataFormat;
					break;

				case "TagLinkType":
					if (Main.TagLinkType != (int)LinkType.Input)
						return Properties.Resources.ErrorLinkTypeInvalid;
					break;
			}

			return null;
		}

		public short _NrAxis;
		public short NrAxis
		{
			get { return _NrAxis; }
		}
		#endregion
	}
}
