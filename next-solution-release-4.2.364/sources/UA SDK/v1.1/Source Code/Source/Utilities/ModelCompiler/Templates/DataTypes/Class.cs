namespace X {
// ***START***
#region _BrowseName_ Class
/// <summary>
/// _Description_
/// </summary>
/// <exclude />
[System.CodeDom.Compiler.GeneratedCodeAttribute("Opc.Ua.ModelCompiler", "1.0.0.0")]
// _SerializationAttribute_
public partial class _BrowseName_ : _BaseType_
{
	#region Constructors
	/// <summary>
	/// The default constructor.
	/// </summary>
	public _BrowseName_()
	{
		Initialize();
	}
    
	/// <summary>
	/// Called by the .NET framework during deserialization.
	/// </summary>
    [OnDeserializing]
	private void Initialize(StreamingContext context)
	{
		Initialize();
	}

	/// <summary>
	/// Sets private members to default values.
	/// </summary>
	private void Initialize()
	{
		// ListOfFieldInitializers
	}
	#endregion

	#region Public Properties
	// ListOfProperties
	#endregion

    #region IEncodeable Members
    /// <summary cref="IEncodeable.TypeId" />
    public override ExpandedNodeId TypeId
    {
        get { return m_TypeId; }
    }

    private static ExpandedNodeId m_TypeId = new ExpandedNodeId(DataTypes._BrowseName_, _NamespaceUri_);

    /// <summary cref="IEncodeable.BinaryEncodingId" />
    public override ExpandedNodeId BinaryEncodingId
    {
        get { return m_BinaryEncodingId; }
    }

    private static ExpandedNodeId m_BinaryEncodingId = new ExpandedNodeId(Objects._BrowseName__Encoding_DefaultBinary, _NamespaceUri_);
    
    /// <summary cref="IEncodeable.XmlEncodingId" />
    public override ExpandedNodeId XmlEncodingId
    {
        get { return m_XmlEncodingId; }
    }
    
    private static ExpandedNodeId m_XmlEncodingId = new ExpandedNodeId(Objects._BrowseName__Encoding_DefaultXml, _NamespaceUri_);

    /// <summary cref="IEncodeable.Encode(IEncoder)" />
    public override void Encode(IEncoder encoder)
    {
        base.Encode(encoder);

        encoder.PushNamespace(_NamespaceUri_);

		// ListOfEncoding

        encoder.PopNamespace();
    }
    
    /// <summary cref="IEncodeable.Decode(IDecoder)" />
    public override void Decode(IDecoder decoder)
    {
        base.Decode(decoder);

        decoder.PushNamespace(_NamespaceUri_);

		// ListOfDecoding

        decoder.PopNamespace();
    }

    /// <summary cref="IEncodeable.IsEqual(IEncodeable)" />
    public override bool IsEqual(IEncodeable encodeable)
    {
        if (Object.ReferenceEquals(this, encodeable))
        {
            return true;
        }
        
        _BrowseName_ value = encodeable as _BrowseName_;
        
        if (value == null)
        {
            return false;
        }

        if (typeof(_BrowseName_).BaseType != typeof(object))
        {
            if (!base.IsEqual(encodeable))
            {
                return false;
            }
        }
        
		// ListOfComparisons

        return true;
    }
    
    /// <summary cref="ICloneable.Clone" />
    public override object Clone()
    {
        _BrowseName_ clone = (_BrowseName_)base.Clone();

        // ListOfCopies

        return clone;
    }
    #endregion
    
	#region Private Fields
	// ListOfFields
	#endregion
}
// CollectionClass
#endregion
// ***END***
}