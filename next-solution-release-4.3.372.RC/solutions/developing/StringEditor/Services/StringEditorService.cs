using DevExpress.Xpo;
using log4net;
using StringManager.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StringManager.Services
{
    public class StringEditorService: IStringEditorService
    {
        private readonly UnitOfWork _uow;

        public StringEditorService(UnitOfWork uow)
        {
            _uow = uow;
        }

        public bool UpdateStringID(string oldId, string newId)
        {
            if (_uow == null) return false;

            var stringIdList = new XPQuery<StringModel.UFStringLocaleText>(_uow, true)
                .Where(x => x.Text == oldId);

            if (stringIdList.Any())
            {
                var newIdExists = new XPQuery<StringModel.UFStringLocaleText>(_uow, true)
                    .Any(x => x.Text == newId);

                if (newIdExists)
                {
                    return false;
                }

                foreach (var stringId in stringIdList)
                {
                    stringId.Text = newId;
                }
            }
            return true;
        }
    }
}
