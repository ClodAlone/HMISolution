using System;
using Moq;
using NUnit.Framework;
using Opc.Ua;

namespace OpcUaUnitTests;

[TestFixture]
public class ContentFilterEvaluateUnitTests
{
    private ContentFilter _sut;

    [SetUp]
    public void SetUp()
    {
        _sut = new ContentFilter();
    }

    [Test, Ignore("Not testable")]
    public void NegativeElementOperandIndex_Evaluate_TrueReturned()
    {
        //Arrange
        var filterTarget = new Mock<IAdvancedFilterTarget>();
        var namespaceTable = new NamespaceTable();
        var typeTree = new Mock<ITypeTable>();
        var context = new FilterContext(namespaceTable, typeTree.Object);

        _sut.Elements = new ContentFilterElementCollection(new[]
        {
            new ContentFilterElement
            {
                FilterOperator = FilterOperator.RelatedTo,
                FilterOperands = new ExtensionObjectCollection(new[]
                {
                    CreateExtensionObject(),
                    new ExtensionObject
                    {
                        Body = new ElementOperand
                        {
                            Index = 0,
                            //Index = -1
                        }
                    },
                    CreateExtensionObject(),
                    CreateExtensionObject(),
                    CreateExtensionObject(),
                    CreateExtensionObject()
                })
            }
        });

        //Act
        var actual = _sut.Evaluate(context, filterTarget.Object);

        //Assert
        Assert.IsFalse(actual);
    }

    private static ExtensionObject CreateExtensionObject()
    {
        return new ExtensionObject
        {
            Body = new LiteralOperand
            {
                Value = new Variant
                {
                    Value = new NodeId(Guid.NewGuid())
                }
            }
        };
    }
}