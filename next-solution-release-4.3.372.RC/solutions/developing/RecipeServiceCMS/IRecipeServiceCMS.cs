using DocumentManager.ComponentService;
using Opc.Ua;
using Opc.Ua.Utilities;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using UFRecipeExecutionContext;

namespace RecipeServiceCMS
{
    [ServiceContract]
    public interface IRecipeServiceCMS : UFProcessServiceCMS.IProcessServiceCMS
    { }
}
