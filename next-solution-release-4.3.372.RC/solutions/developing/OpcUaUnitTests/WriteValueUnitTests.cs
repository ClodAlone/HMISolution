using System;
using NUnit.Framework;
using Opc.Ua;

namespace OpcUaUnitTests;

[TestFixture]
public class WriteValueUnitTests
{
    [Test]
    public void CreateWriteValue_ValidateValue_NullReturned()
    {
        //Arrange
        var value = new WriteValue
        {
            NodeId = new NodeId(Guid.NewGuid()),
            AttributeId = 5,
            IndexRange = "7",
            Value = new DataValue(new Variant(10))
        };

        //Act
        var actual = WriteValue.Validate(value);

        //Assert
        Assert.IsNull(actual);
    }
}