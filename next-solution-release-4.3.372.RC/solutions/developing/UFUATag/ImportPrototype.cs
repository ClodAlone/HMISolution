using System;
using System.Collections.Generic;
using System.Linq;

namespace UFUAModel
{
    public class ImportPrototype
    {
        #region Constructors
        public ImportPrototype()
        { }

        public ImportPrototype(UFUAModel.UFUATagPrototype prototype)
        {
            Name = prototype.Name;
            Description = prototype.Description;
            Elements = new List<ImportTag>(prototype.Members.Count);
            foreach (var member in prototype.Members)
                Elements.Add(new ImportTag(member));
        }
        #endregion

        #region Properties
        private string _Name;
        public string Name
        {
            get { return _Name; }
            set
            {
                _Name = value;
            }
        }
        private string _Description;
        public string Description
        {
            get { return _Description; }
            set
            {
                _Description = value;
            }
        }
        private List<ImportTag> _Elements;
        public List<ImportTag> Elements
        {
            get { return _Elements; }
            set
            {
                _Elements = value;
            }
        }

        #endregion

        #region Methods
        public bool IsCompatible(ImportPrototype instance, List<UFUAModel.ImportPrototype> prototypes)
        {
            if (Elements != null && instance.Elements != null)
            {
                if (Elements.Count != instance.Elements.Count)
                    return false;

                for (int ii = 0; ii < Elements.Count; ii++)
                {
                    if (!Elements[ii].IsCompatible(instance.Elements[ii], prototypes))
                        return false;
                }
            }
            else if (Elements == null && instance.Elements != null && instance.Elements.Count > 0)
                return false;
            else if (Elements != null && Elements.Count > 0 && instance.Elements == null)
                return false;

            return true;
        }

        public void ReplacePrototypeReferenceName(String oldName, String newName)
        {
            if (Elements != null && Elements.Count > 0)
            {
                var foundElements = (from e in Elements.AsParallel()
                                     where e.ModelType == ModelType.ObjectType &&
                                     e.PrototypeModel == oldName
                                     select e).ToList();
                foreach (var el in foundElements)
                    el.PrototypeModel = newName;
            }
        }
        #endregion
    }
}
