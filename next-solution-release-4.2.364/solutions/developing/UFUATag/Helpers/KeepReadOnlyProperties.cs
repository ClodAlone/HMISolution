using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UFUAModel.Helpers
{
    public class KeepReadOnlyProperties : IDisposable
    {
        #region Declarations
        readonly string name;
        readonly ModelType? modelType;
        readonly DataType? dataType;
        readonly string prototypeModel;
        readonly uint arrayDimension;
        readonly int? memberOrderId;

        UFUAModel.UFUATag tag;
        #endregion

        #region Constructors
        public KeepReadOnlyProperties(UFUAModel.UFUATag tag)
        {
            this.name = tag.Name;
            this.modelType = tag.ModelType;
            this.dataType = tag.DataType;
            this.prototypeModel = tag.PrototypeModel;
            this.arrayDimension = tag.ArrayDimension;
            this.memberOrderId = tag.MemberOrderId;

            this.tag = tag;
        }
        #endregion

        #region Methods
        void CopyReadOnlyProperties()
        {
            tag.Name = this.name;
            tag.ModelType = this.modelType;
            tag.DataType = this.dataType;
            tag.PrototypeModel = this.prototypeModel;
            tag.ArrayDimension = this.arrayDimension;
            tag.MemberOrderId = this.memberOrderId;
        }

        public static void CopyReadOnlyProperties(UFUAModel.UFUATag sourceTag, UFUAModel.UFUATag targetTag)
        {
            targetTag.Name = sourceTag.Name;
            targetTag.ModelType = sourceTag.ModelType;
            targetTag.DataType = sourceTag.DataType;
            targetTag.PrototypeModel = sourceTag.PrototypeModel;
            targetTag.ArrayDimension = sourceTag.ArrayDimension;
            targetTag.MemberOrderId = sourceTag.MemberOrderId;
        }
        #endregion

        #region IDisposable
        public void Dispose()
        {
            CopyReadOnlyProperties();
        }
        #endregion
    }
}
