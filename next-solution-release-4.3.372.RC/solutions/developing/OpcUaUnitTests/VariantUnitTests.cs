using NUnit.Framework;
using Opc.Ua;
using System.Text;

namespace OpcUaUnitTests;

[TestFixture]
public class VariantUnitTests
{
    [Test]
    public void VariantWithDataValue_ToString_NullStringReturned()
    {
        //Arrange
        var sut = new Variant(new DataValue());

        //Act
        var actual = sut.ToString();

        //Assert
        Assert.AreEqual(actual, "(null)");
    }

    [Test]
    public void VariantWithByteStringValueAndNegativeRank_ToString_DifferentStringRepresentationReturned()
    {
        //Arrange
        //native byteString with rank 1 representation is "{00}"
        var value = new [] { new byte[1] };
        var sut = new Variant(value);

        //Act
        var actual = sut.ToString();

        //Assert
        Assert.AreEqual(actual, "{{0}}");
    }

    [Test]
    public void VariantWithByteStringValueAndZeroRank_ToString_DifferentStringRepresentationReturned()
    {
        //Arrange
        //native byteString representation of pippo is "706970706F", rank 0
        var value = Encoding.ASCII.GetBytes("pippo");
        var sut = new Variant(value);

        //Act
        var actual = sut.ToString();

        //Assert
        Assert.AreEqual(actual, "{112 |105 |112 |112 |111}");
    }

    [Test]
    public void VariantWithStringValue_ToString_NullStringReturned()
    {
        //Arrange
        //standard representation should be {1|2|3} (without spaces)
        var value = new []{1,2,3};
        var sut = new Variant(value);

        //Act
        var actual = sut.ToString();

        //Assert
        Assert.AreEqual(actual, "{1 |2 |3}");
    }


}