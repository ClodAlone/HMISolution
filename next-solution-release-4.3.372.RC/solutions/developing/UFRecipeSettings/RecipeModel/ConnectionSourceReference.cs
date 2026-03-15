using System;
using UFRecipeSettings.Documents;

namespace UFRecipeSettings
{
    public class ConnectionSourceReference : IComparable
    {
        #region Declarations
        readonly UFRecipeDocument recipeDocument;
        #endregion

        #region Constructors
        public ConnectionSourceReference(UFRecipeDocument doc)
        {
            recipeDocument = doc;
        }
        #endregion
       
        #region Properties
        public String ReadableConnectionString
        {
            get
            {
                return recipeDocument.RecipeEntity.ReadableConnectionString;
            }
            set
            {
                if (recipeDocument.RecipeEntity.ReadableConnectionString == value)
                    return;
                
                recipeDocument.RecipeEntity.ReadableConnectionString = value;
            }
        }

        public UFRecipeDocument RecipeDocument
        {
            get
            {
                return recipeDocument;
            }
        }
        #endregion

        #region IComparable Members
        /// <summary>
        /// Compares the instance to another object.
        /// </summary>
        /// <remarks>
        /// Compares the instance to another object.
        /// </remarks>
        /// <param name="obj">The object to compare to *this* object</param>
        public int CompareTo(object obj)
        {
            // check for reference equality.
            if (Object.ReferenceEquals(obj, this))
            {
                return 0;
            }

            // check for null.
            if (obj == null)
            {
                return +1;
            }

            // check for RecipeStartAddressReference
            if (obj is ConnectionSourceReference)
            {
                var connectionsource = (obj as ConnectionSourceReference);
                if (String.IsNullOrEmpty(ReadableConnectionString) && String.IsNullOrEmpty(connectionsource.ReadableConnectionString))
                    return 0;

                else if (!String.IsNullOrEmpty(ReadableConnectionString))
                    return ReadableConnectionString == connectionsource.ReadableConnectionString ? 0 : -1;
            }

            // objects not comparable.
            return -1;
        }

        #endregion

        #region Overridden Methods
        /// <summary>
        /// Determines if the specified object is equal to the object.
        /// </summary>
        /// <remarks>
        /// Determines if the specified object is equal to the object.
        /// </remarks>
        /// <param name="obj">The object to compare to *this* object</param>
        public override bool Equals(object obj)
        {
            return CompareTo(obj) == 0;
        }

        /// <summary>
        /// Returns a unique hashcode for the object.
        /// </summary>
        /// <remarks>
        /// Returns a unique hashcode for the object.
        /// </remarks>
        public override int GetHashCode()
        {
            if (String.IsNullOrEmpty(ReadableConnectionString))
                return base.GetHashCode();

            return ReadableConnectionString.GetHashCode();
        }
        #endregion

        #region Static Members
        /// <summary>
        /// Returns true if the objects are not equal.
        /// </summary>
        /// <remarks>
        /// Returns true if the objects are not equal.
        /// </remarks>
        /// <param name="a">The first object being compared</param>
        /// <param name="b">The second object being compared to</param>
        public static bool operator ==(ConnectionSourceReference a, ConnectionSourceReference b)
        {
            return object.Equals(a, b);
        }

        /// <summary>
        /// Returns true if the objects are not equal.
        /// </summary>
        /// <remarks>
        /// Returns true if the objects are not equal.
        /// </remarks>
        /// <param name="a">The first object being compared</param>
        /// <param name="b">The second object being compared to</param>
        public static bool operator !=(ConnectionSourceReference a, ConnectionSourceReference b)
        {
            return !(a == b);
        }

        /// <summary>
        /// Returns true if the object a is less than object b.
        /// </summary>
        /// <remarks>
        /// Returns true if the object a is less than object b.
        /// </remarks>
        /// <param name="a">The first value being compared</param>
        /// <param name="b">The second value being compared to</param>
        public static bool operator <(ConnectionSourceReference a, ConnectionSourceReference b)
        {
            return a.CompareTo(b) < 0;
        }

        /// <summary>
        /// Returns true if the object a is greater than object b.
        /// </summary>
        /// <remarks>
        /// Returns true if the object a is greater than object b.
        /// </remarks>
        /// <param name="a">The first value being compared</param>
        /// <param name="b">The second value being compared to</param>
        public static bool operator >(ConnectionSourceReference a, ConnectionSourceReference b)
        {
            return a.CompareTo(b) > 0;
        }
        #endregion
    }
}
