using System;
using NUnit.Framework;

namespace Tools.UnitTests
{
    [TestFixture]
    public class ProjectUpdaterUnitTests
    {
        [Test]
        public void TestReplaceUnsupportedOperators()
        {
            var expression = @"=[2:Tags/2:Variable1] AND [2:Tags/2:Variable2]";
            var result = ProjectUpdater.ProjectUpdater.ReplaceExpression(expression);
            Assert.AreEqual(@"=AND([2:Tags/2:Variable1], [2:Tags/2:Variable2])", result);
            
            expression = @"=[2:Tags/2:Variable1] OR [2:Tags/2:Variable2]";
            result = ProjectUpdater.ProjectUpdater.ReplaceExpression(expression);
            Assert.AreEqual(@"=OR([2:Tags/2:Variable1], [2:Tags/2:Variable2])", result);

            expression = @"=[2:Tags/2:Variable1] XOR [2:Tags/2:Variable2]";
            result = ProjectUpdater.ProjectUpdater.ReplaceExpression(expression);
            Assert.AreEqual(@"=XOR([2:Tags/2:Variable1], [2:Tags/2:Variable2])", result);
        }

        [Test]
        public void TestReplaceMiscOperators()
        {
            var expression = @"=([2:Tags/2:Variable1] OR [2:Tags/2:Variable2]) AND (([2:Tags/2:Variable3]=1) AND ([2:Tags/2:Variable4]=2))";
            var result = ProjectUpdater.ProjectUpdater.ReplaceExpression(expression);
            Assert.AreEqual(@"=AND((OR([2:Tags/2:Variable1], [2:Tags/2:Variable2])), (AND(([2:Tags/2:Variable3]=1), ([2:Tags/2:Variable4]=2))))", result);

            expression = @"=([2:Tags/2:Variable1] OR [2:Tags/2:Variable2]) AND (([2:Tags/2:Variable3]=1) XOR ([2:Tags/2:Variable4]=2))";
            result = ProjectUpdater.ProjectUpdater.ReplaceExpression(expression);
            Assert.AreEqual(@"=AND((OR([2:Tags/2:Variable1], [2:Tags/2:Variable2])), (XOR(([2:Tags/2:Variable3]=1), ([2:Tags/2:Variable4]=2))))", result);

            expression = @"=IF(([x]=2) OR ([x]=3),1,0)";
            result = ProjectUpdater.ProjectUpdater.ReplaceExpression(expression);
            Assert.AreEqual(@"=IF(OR(([x]=2), ([x]=3)),1,0)", result);

            expression = @"=IF(([x]=2) AND ([x]=3),1,0)";
            result = ProjectUpdater.ProjectUpdater.ReplaceExpression(expression);
            Assert.AreEqual(@"=IF(AND(([x]=2), ([x]=3)),1,0)", result);

            expression = @"=IF  (   ( [x] = 2  )    OR   ([x]  =   3) ,   1,   0)";
            result = ProjectUpdater.ProjectUpdater.ReplaceExpression(expression);
            Assert.AreEqual(@"=IF ( OR(( [x] = 2 ), ([x] = 3)) , 1, 0)", result);
        }

        [Test]
        public void TestReplaceUnsupportedFunctions()
        {
            var expression = @"=VARP([2:Tags/2:Variable1], [x])";
            var result = ProjectUpdater.ProjectUpdater.ReplaceExpression(expression);
            Assert.AreEqual(@"=VAR.P([2:Tags/2:Variable1], [x])", result);
        }

        [Test]
        public void TestReplaceBitFunctions()
        {
            var expression = @"=BITAND([2:Tags/2:Variable1], [x])";
            var result = ProjectUpdater.ProjectUpdater.ReplaceExpression(expression);
            Assert.AreEqual(@"=BITAND.QWORD([2:Tags/2:Variable1], [x])", result);
            
            expression = @"=BITOR([2:Tags/2:Variable1], [x])";
            result = ProjectUpdater.ProjectUpdater.ReplaceExpression(expression);
            Assert.AreEqual(@"=BITOR.QWORD([2:Tags/2:Variable1], [x])", result);

            expression = @"=BITXOR([2:Tags/2:Variable1], [x])";
            result = ProjectUpdater.ProjectUpdater.ReplaceExpression(expression);
            Assert.AreEqual(@"=BITXOR.QWORD([2:Tags/2:Variable1], [x])", result);
        }

        [Test]
        public void TestReplaceMiscFunctionsAndOperators()
        {
            var expression = @"=AND(BITAND([2:Tags/2:Variable1],2) AND ([2:Tags/2:Variable2]=1))";
            var result = ProjectUpdater.ProjectUpdater.ReplaceExpression(expression);
            Assert.AreEqual(@"=AND(AND(BITAND.QWORD([2:Tags/2:Variable1],2), ([2:Tags/2:Variable2]=1)))", result);
        }

        [Test]
        public void TestReplaceWithWhiteSpaces()
        {
            var expression = @"=[x] +    1   ";
            var result = ProjectUpdater.ProjectUpdater.ReplaceExpression(expression);
            Assert.AreEqual(expression, result);

            expression = @" =  AVG   (1, [2:Tags/2:Variable1]  OR  [2:Tags/2:Variable2])  ";
            result = ProjectUpdater.ProjectUpdater.ReplaceExpression(expression);
            Assert.AreEqual(@"= AVERAGE (1, OR([2:Tags/2:Variable1], [2:Tags/2:Variable2]))", result);
        }

        [Test]
        public void TestReplaceOperators()
        {
            var expression = @"=[x]  >=    1   ";
            var result = ProjectUpdater.ProjectUpdater.ReplaceExpression(expression);
            Assert.AreEqual(expression, result);

            expression = @"=[x]  > =    1   ";
            result = ProjectUpdater.ProjectUpdater.ReplaceExpression(expression);
            Assert.AreEqual(@"=[x] >= 1", result);

            expression = @"=[x]  <=    1   ";
            result = ProjectUpdater.ProjectUpdater.ReplaceExpression(expression);
            Assert.AreEqual(expression, result);

            expression = @"=[x]  < =    1   ";
            result = ProjectUpdater.ProjectUpdater.ReplaceExpression(expression);
            Assert.AreEqual(@"=[x] <= 1", result);

            expression = @"=[x]  =>    1   ";
            result = ProjectUpdater.ProjectUpdater.ReplaceExpression(expression);
            Assert.AreEqual(@"=[x] >= 1", result);

            expression = @"=[x]  = >    1   ";
            result = ProjectUpdater.ProjectUpdater.ReplaceExpression(expression);
            Assert.AreEqual(@"=[x] >= 1", result);

            expression = @"=[x]  =<    1   ";
            result = ProjectUpdater.ProjectUpdater.ReplaceExpression(expression);
            Assert.AreEqual(@"=[x] <= 1", result);

            expression = @"=[x]  = <    1   ";
            result = ProjectUpdater.ProjectUpdater.ReplaceExpression(expression);
            Assert.AreEqual(@"=[x] <= 1", result);

            expression = @"= CONCATENATE([x], "" =< "")";
            result = ProjectUpdater.ProjectUpdater.ReplaceExpression(expression);
            Assert.AreEqual(expression, result);

            expression = @"= CONCATENATE(  [x], "" =< "")    + [x] => 1   ";
            result = ProjectUpdater.ProjectUpdater.ReplaceExpression(expression);
            Assert.AreEqual(@"= CONCATENATE( [x], "" =< "") + [x] >= 1", result);

            expression = @"= CONCATENATE([x], """" =< "")";
            result = ProjectUpdater.ProjectUpdater.ReplaceExpression(expression);
            Assert.AreEqual(expression, result);

            expression = @"= CONCATENATE([x], "" =< """")";
            result = ProjectUpdater.ProjectUpdater.ReplaceExpression(expression);
            Assert.AreEqual(expression, result);

            expression = @"= CONCATENATE([x], """" =< """")";
            result = ProjectUpdater.ProjectUpdater.ReplaceExpression(expression);
            Assert.AreEqual(expression, result);

            expression = @"==>=<=>>=";
            result = ProjectUpdater.ProjectUpdater.ReplaceExpression(expression);
            Assert.AreEqual(@"=>=<=>=>=", result);
        }

        [Test]
        public void TestExpressionWithAliases()
        {
            var expression = @"= IF([x] = <<ALS_PLC_Closed_ABS>>, 1, 0)";
            var result = ProjectUpdater.ProjectUpdater.ReplaceExpression(expression);
            Assert.AreEqual(expression, result);

            expression = @"=<<ALIAS>> AND [2:Tags/2:boolTag])";
            result = ProjectUpdater.ProjectUpdater.ReplaceExpression(expression);
            if (result.IndexOf("<<ALIAS>>") == -1)
                Assert.Fail("Aliases in function fail!");

            expression = @"=BITAND(<<ALIAS>>, [2:Tags/2:CommandTag/2:TagAnalog_Int16])";
            result = ProjectUpdater.ProjectUpdater.ReplaceExpression(expression);
            if (result.IndexOf("<<ALIAS>>") == -1)
                Assert.Fail("Aliases in function fail!");
        }
    }
}
