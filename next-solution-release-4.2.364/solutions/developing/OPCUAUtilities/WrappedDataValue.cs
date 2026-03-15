using Opc.Ua;
using Opc.Ua.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;

namespace Opc.Ua.Utilities
{
    [DataContract(Name = "W", Namespace = "")]
    public class WrappedDataValue
    {
        #region Declarations
        DataValue dataValue;
        #endregion

        #region Public Constructors
        /// <summary>
        /// Creates a deep copy of the value.
        /// </summary>
        /// <remarks>
        /// Creates a new instance of the class while copying the contents
        /// of another instance.
        /// </remarks>
        /// <param name="value">The DataValue to copy.</param>
        /// <exception cref="ArgumentNullException">Thrown when the value is null</exception>
        public WrappedDataValue(DataValue value)
        {
            dataValue = new DataValue(value);
        }

        /// <summary>
        /// Initializes the object with a value, a status code and a source timestamp
        /// </summary>
        /// <remarks>
        /// Initializes the object with a value, a status code and a source timestamp
        /// </remarks>
        /// <param name="sourceTimestamp">The timestamp to set</param>
        /// <param name="statusCode">The status code to set</param>
        /// <param name="value">The variant value to set</param>
        public WrappedDataValue(Variant value, StatusCode statusCode, DateTime sourceTimestamp)
        {
            dataValue = new DataValue(value, statusCode, sourceTimestamp);
        }
        #endregion

        #region Serialization/Deserialization
        [OnDeserializing]
        void EnsureValidDataValue(StreamingContext c)
        {
            dataValue = new DataValue();
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// The value of data value.
        /// </summary>
        /// <remarks>
        /// The value of data value.
        /// </remarks>
        [DataMember(Name = "V", Order = 3, IsRequired = false)]
        public byte[] Value
        {
            get
            {
                using (var encoder = new BinaryEncoder(ServiceMessageContext.GlobalContext))
                {
                    encoder.WriteVariant(null, dataValue.WrappedValue);
                    return encoder.CloseAndReturnBuffer();
                }
            }
            set
            {
                using (var encoder = new BinaryDecoder(value, ServiceMessageContext.GlobalContext))
                {
                    try
                    {
                        dataValue.WrappedValue = encoder.ReadVariant(null);
                    }
                    catch
                    {
                        dataValue.StatusCode = StatusCodes.BadDecodingError;
                    }
                }
            }
        }

        /// <summary>
        /// The status code associated with the value.
        /// </summary>
        /// <remarks>
        /// The status code associated with the value.
        /// </remarks>
        [DataMember(Name = "Q", Order = 4, IsRequired = false)]
        public uint StatusCode
        {
            get
            {
                return dataValue.StatusCode.Code;
            }
            set
            {
                dataValue.StatusCode = new StatusCode(value);
            }
        }

        /// <summary>
        /// The source timestamp associated with the value.
        /// </summary>
        /// <remarks>
        /// The source timestamp associated with the value.
        /// </remarks>
        [DataMember(Name = "D", Order = 5, IsRequired = false)]
        public DateTime SourceTimestamp
        {
            get
            {
                return dataValue.SourceTimestamp;
            }
            set
            {
                dataValue.SourceTimestamp = value;
            }
        }

        public DataValue DataValue
        {
            get
            {
                return dataValue;
            }
        }
        #endregion
    }

    [CollectionDataContract(Name = "C", Namespace = "")]
    public partial class WrappedDataValueCollection : List<WrappedDataValue>, ICloneable
    {
        #region Public Constructors
        /// <summary>
        /// Initializes an empty collection.
        /// </summary>
        /// <remarks>
        /// Initializes an empty collection.
        /// </remarks>
        public WrappedDataValueCollection() { }

        /// <summary>
        /// Initializes the collection from another collection.
        /// </summary>
        /// <remarks>
        /// Initializes the collection from another collection.
        /// </remarks>
        /// <param name="collection">A collection of <see cref="WrappedDataValue"/> objects to pre-populate this new collection with</param>
        public WrappedDataValueCollection(IEnumerable<WrappedDataValue> collection) : base(collection) { }

        /// <summary>
        /// Initializes the collection from another collection.
        /// </summary>
        /// <remarks>
        /// Initializes the collection from another collection.
        /// </remarks>
        /// <param name="collection">A collection of <see cref="DataValue"/> objects to pre-populate this new collection with</param>
        public WrappedDataValueCollection(IEnumerable<DataValue> collection)
        {
            if (collection != null)
                AddRange(collection);
        }

        /// <summary>
        /// Initializes the collection with the specified capacity.
        /// </summary>
        /// <remarks>
        /// Initializes the collection with the specified capacity.
        /// </remarks>
        /// <param name="capacity">The max capacity of this collection</param>
        public WrappedDataValueCollection(int capacity) : base(capacity) { }
        #endregion

        #region Public Methods
        //
        // Summary:
        //     Adds an object to the end of the System.Collections.Generic.List`1.
        //
        // Parameters:
        //   item:
        //     The object to be added to the end of the System.Collections.Generic.List`1. The
        //     value can be null for reference types.
        public void Add(DataValue item)
        {
            this.Add(new WrappedDataValue(item));
        }
        
        //
        // Summary:
        //     Adds the elements of the specified collection to the end of the System.Collections.Generic.List`1.
        //
        // Parameters:
        //   collection:
        //     The collection whose elements should be added to the end of the System.Collections.Generic.List`1.
        //     The collection itself cannot be null, but it can contain elements that are null,
        //     if type T is a reference type.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     collection is null.
        public void AddRange(IEnumerable<DataValue> collection)
        {
            if (collection != null)
            {
                foreach (var item in collection)
                    this.Add(item);
            }
        }
        #endregion

        #region Public Static Methods
        /// <summary>
        /// Converts an array to a collection.
        /// </summary>
        /// <remarks>
        /// Converts an array to a collection.
        /// </remarks>
        /// <param name="values">An array of <see cref="WrappedDataValue"/> objects to return as a collection</param>
        public static WrappedDataValueCollection ToWrappedDataValueCollection(WrappedDataValue[] values)
        {
            if (values != null)
            {
                return new WrappedDataValueCollection(values);
            }

            return new WrappedDataValueCollection();
        }

        /// <summary>
        /// Converts an array to a collection.
        /// </summary>
        /// <remarks>
        /// Converts an array to a collection.
        /// </remarks>
        /// <param name="values">An array of <see cref="DataValue"/> objects to return as a collection</param>
        public static WrappedDataValueCollection ToWrappedDataValueCollection(DataValue[] values)
        {
            if (values != null)
            {
                return new WrappedDataValueCollection(values);
            }

            return new WrappedDataValueCollection();
        }

        /// <summary>
        /// Converts an array to a collection.
        /// </summary>
        /// <remarks>
        /// Converts an array to a collection.
        /// </remarks>
        /// <param name="values">An array of <see cref="WrappedDataValue"/> objects to return as a collection</param>
        public static DataValueCollection ToDataValueCollection(WrappedDataValue[] values)
        {
            if (values != null)
            {
                var collection = new DataValueCollection();
                foreach (var item in values)
                    collection.Add(item.DataValue);

                return collection;
            }

            return new DataValueCollection();
        }

        /// <summary>
        /// Converts an array to a collection.
        /// </summary>
        /// <remarks>
        /// Converts an array to a collection.
        /// </remarks>
        /// <param name="values">An array of <see cref="WrappedDataValue"/> objects to return as a collection</param>
        public static DataValueCollection ToDataValueCollection(IEnumerable<WrappedDataValue> values)
        {
            if (values != null)
            {
                var collection = new DataValueCollection();
                foreach (var item in values)
                    collection.Add(item.DataValue);

                return collection;
            }

            return new DataValueCollection();
        }
        #endregion

        #region Operators
        /// <summary>
        /// Converts an array to a collection.
        /// </summary>
        /// <remarks>
        /// Converts an array to a collection.
        /// </remarks>
        /// <param name="values">An array of <see cref="WrappedDataValue"/> objects to return as a collection</param>
        public static implicit operator WrappedDataValueCollection(WrappedDataValue[] values)
        {
            return ToWrappedDataValueCollection(values);
        }

        /// <summary>
        /// Converts an array to a collection.
        /// </summary>
        /// <remarks>
        /// Converts an array to a collection.
        /// </remarks>
        /// <param name="values">An array of <see cref="DataValue"/> objects to return as a collection</param>
        public static implicit operator WrappedDataValueCollection(DataValue[] values)
        {
            return ToWrappedDataValueCollection(values);
        }

        /// <summary>
        /// Converts an array to a collection.
        /// </summary>
        /// <remarks>
        /// Converts an array to a collection.
        /// </remarks>
        /// <param name="values">A collection of <see cref="WrappedDataValueCollection"/> objects to return as a collection</param>
        public static implicit operator DataValueCollection(WrappedDataValueCollection collection)
        {
            return ToDataValueCollection(collection);
        }
        #endregion

        #region ICloneable
        /// <summary>
        /// Creates a deep copy of the collection.
        /// </summary>
        /// <remarks>
        /// Creates a deep copy of the collection.
        /// </remarks>
        public object Clone()
        {
            WrappedDataValueCollection clone = new WrappedDataValueCollection(this.Count);

            foreach (WrappedDataValue element in this)
            {
                clone.Add((WrappedDataValue)Utils.Clone(element));
            }

            return clone;
        }
        #endregion
    }
}
