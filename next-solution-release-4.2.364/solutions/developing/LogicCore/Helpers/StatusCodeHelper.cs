using System;
using Opc.Ua;

namespace LogicCore.Helpers
{
    public static class StatusCodeHelper
    {
        public static bool IsGoodOrUncertainLastUsable(StatusCode code)
        {
            return StatusCode.IsGood(code) || code == StatusCodes.UncertainLastUsableValue;
        }
    }
}
