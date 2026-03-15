using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StringManager.UndoRedo
{
    internal class Memento : IEquatable<Memento>
    {
        readonly List<ExpandoObject> listTexts;
        readonly List<String> listLocales;

        #region Constructors
        public Memento(IList<ExpandoObject> listTexts, IList<String> listLocales)
        {
            this.listTexts = new List<ExpandoObject>();
            foreach (var expando in listTexts)
            {
                var dynObject = new ExpandoObject();
                this.listTexts.Add(dynObject);

                var source = expando as IDictionary<string, object>;
                var target = dynObject as IDictionary<string, object>;
                foreach (var key in source.Keys)
                    target[key] = source[key];
            }
            
            this.listLocales = listLocales.ToList();
        }
        #endregion

        #region Properties
        public List<ExpandoObject> Texts
        {
            get
            {
                return listTexts.ToList();
            }
        }

        public List<String> Locales
        {
            get
            {
                return listLocales.ToList();
            }
        }
        #endregion

        #region Overrides
        /// <summary>
        /// Returns the hash code for a particular Memento.
        /// </summary>
        /// <returns>A hash code.</returns>
        public override int GetHashCode()
        {
            return listTexts.GetHashCode() ^ listLocales.GetHashCode();
        }

        /// <summary>
        /// Determines if this object and another object are equal.
        /// </summary>
        /// <param name="obj">The object to compare.</param>
        /// <returns><b>true</b> if this instance and another object are equal; otherwise <b>false</b>.</returns>
        public override bool Equals(object obj)
        {
            return this.Equals(obj as Memento);
        }

        #endregion

        #region IEquatable<Memento>
        public bool Equals(Memento other)
        {
            if (ReferenceEquals(null, other))
                return false;

            if (Texts.Count != other.Texts.Count ||
                Locales.Count != other.Locales.Count)
                return false;

            for (int ii = 0; ii < Locales.Count; ii++)
            {
                if (!other.Locales.Contains(Locales[ii]))
                    return false;
            }

            for (int ii = 0; ii < Texts.Count; ii++)
            {
                var source = Texts[ii] as IDictionary<string, object>;
                var target = other.Texts[ii] as IDictionary<string, object>;

                if (source.Count != target.Count)
                    return false;

                foreach (var key in source.Keys)
                {
                    if (!target.ContainsKey(key))
                        return false;

                    if (source[key] == null && target[key] == null)
                        continue;

                    if (source[key] == null && target[key] != null ||
                        source[key] != null && target[key] == null)
                        return false;

                    if (source[key].GetType() != target[key].GetType())
                        return false;

                    if (source[key] != target[key])
                        return false;
                }
            }

            return true;
        }
        #endregion

        #region Operators overload
        /// <summary>
        /// Memento == operator overload
        /// </summary>
        /// <param name="a">The first item to compare.</param>
        /// <param name="b">The second item to compare.</param>
        /// <returns><b>true</b> if equal; otherwise <b>false</b>.</returns>
        public static bool operator ==(Memento a, Memento b)
        {
            return a.Equals(b);
        }

        /// <summary>
        /// Memento != operator overload
        /// </summary>
        /// <param name="a">The first item to compare.</param>
        /// <param name="b">The second item to comare.</param>
        /// <returns><b>true</b> if not equal; otherwise <b>false</b>.</returns>
        public static bool operator !=(Memento a, Memento b)
        {
            return !a.Equals(b);
        }
        #endregion
    }
}
