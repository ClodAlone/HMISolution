using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RedundancyService.Helper;
using RedundancyService;
using System.Collections.Generic;

namespace RedundancyUnitTests.ActiveServerManager
{
    [TestClass]
    public class ChangedAlarmsHelperTests
    {
        [TestMethod]
        public void TestProperties()
        {
            var helper = new ChangedAlarmsFiller(alarmsCounter: 100, valuesCounter: 10);
            var sendChangedTags = new ChangedAlarmsHelper(helper.ChangedAlarms);
            Assert.AreEqual(sendChangedTags.ReadyElementsCounter, helper.MaxAlarms);
            Assert.AreEqual(sendChangedTags.ReadyValuesCounter, helper.MaxValues);
            Assert.AreEqual(sendChangedTags.ProcessedElementsCounter, 0);
            Assert.AreEqual(sendChangedTags.ProcessingElementsCounter, 0);
            Assert.AreEqual(sendChangedTags.ProcessingValuesCounter, 0);
            Assert.IsFalse(sendChangedTags.IsTerminated);

            var list = sendChangedTags.GetChangedElementsBlock();
            Assert.AreEqual(sendChangedTags.ReadyElementsCounter, helper.MaxAlarms);
            Assert.AreEqual(sendChangedTags.ReadyValuesCounter, helper.MaxValues);
            Assert.AreEqual(sendChangedTags.ProcessedElementsCounter, 0);
            Assert.AreEqual(sendChangedTags.ProcessingElementsCounter, helper.MaxAlarms);
            Assert.AreEqual(sendChangedTags.ProcessingValuesCounter, helper.MaxValues);
            Assert.IsFalse(sendChangedTags.IsTerminated);

            sendChangedTags.GoToNextChangedElementsBlock();
            Assert.AreEqual(sendChangedTags.ReadyElementsCounter, 0);
            Assert.AreEqual(sendChangedTags.ReadyValuesCounter, 0);
            Assert.AreEqual(sendChangedTags.ProcessedElementsCounter, helper.MaxAlarms);
            Assert.AreEqual(sendChangedTags.ProcessingElementsCounter, 0);
            Assert.AreEqual(sendChangedTags.ProcessingValuesCounter, 0);
            Assert.IsTrue(sendChangedTags.IsTerminated);
        }

        [TestMethod]
        public void TestLowBoundaries1()
        {
            var helper = new ChangedAlarmsFiller(alarmsCounter: 0, valuesCounter: 0);
            var sendChangedTags = new ChangedAlarmsHelper(helper.ChangedAlarms);
            Assert.AreEqual(sendChangedTags.ReadyElementsCounter, helper.MaxAlarms);
            Assert.AreEqual(sendChangedTags.ReadyValuesCounter, helper.MaxValues);
            Assert.AreEqual(sendChangedTags.ProcessedElementsCounter, 0);
            Assert.AreEqual(sendChangedTags.ProcessingElementsCounter, 0);
            Assert.AreEqual(sendChangedTags.ProcessingValuesCounter, 0);
            Assert.IsTrue(sendChangedTags.IsTerminated);

            var list = sendChangedTags.GetChangedElementsBlock();
            Assert.AreEqual(sendChangedTags.ReadyElementsCounter, helper.MaxAlarms);
            Assert.AreEqual(sendChangedTags.ReadyValuesCounter, helper.MaxValues);
            Assert.AreEqual(sendChangedTags.ProcessedElementsCounter, 0);
            Assert.AreEqual(sendChangedTags.ProcessingElementsCounter, helper.MaxAlarms);
            Assert.AreEqual(sendChangedTags.ProcessingValuesCounter, 0);
            Assert.IsTrue(sendChangedTags.IsTerminated);

            sendChangedTags.GoToNextChangedElementsBlock();
            Assert.AreEqual(sendChangedTags.ReadyElementsCounter, 0);
            Assert.AreEqual(sendChangedTags.ReadyValuesCounter, 0);
            Assert.AreEqual(sendChangedTags.ProcessedElementsCounter, helper.MaxAlarms);
            Assert.AreEqual(sendChangedTags.ProcessingElementsCounter, 0);
            Assert.AreEqual(sendChangedTags.ProcessingValuesCounter, 0);
            Assert.IsTrue(sendChangedTags.IsTerminated);
        }

        [TestMethod]
        public void TestLowBoundaries2()
        {
            var helper = new ChangedAlarmsFiller(alarmsCounter: 1, valuesCounter: 1);
            var sendChangedTags = new ChangedAlarmsHelper(helper.ChangedAlarms);
            Assert.AreEqual(sendChangedTags.ReadyElementsCounter, helper.MaxAlarms);
            Assert.AreEqual(sendChangedTags.ReadyValuesCounter, helper.MaxValues);
            Assert.AreEqual(sendChangedTags.ProcessedElementsCounter, 0);
            Assert.AreEqual(sendChangedTags.ProcessingElementsCounter, 0);
            Assert.AreEqual(sendChangedTags.ProcessingValuesCounter, 0);
            Assert.IsFalse(sendChangedTags.IsTerminated);

            var list = sendChangedTags.GetChangedElementsBlock();
            Assert.AreEqual(sendChangedTags.ReadyElementsCounter, helper.MaxAlarms);
            Assert.AreEqual(sendChangedTags.ReadyValuesCounter, helper.MaxValues);
            Assert.AreEqual(sendChangedTags.ProcessedElementsCounter, 0);
            Assert.AreEqual(sendChangedTags.ProcessingElementsCounter, helper.MaxAlarms);
            Assert.AreEqual(sendChangedTags.ProcessingValuesCounter, helper.MaxValues);
            Assert.IsFalse(sendChangedTags.IsTerminated);

            sendChangedTags.GoToNextChangedElementsBlock();
            Assert.AreEqual(sendChangedTags.ReadyElementsCounter, 0);
            Assert.AreEqual(sendChangedTags.ReadyValuesCounter, 0);
            Assert.AreEqual(sendChangedTags.ProcessedElementsCounter, helper.MaxAlarms);
            Assert.AreEqual(sendChangedTags.ProcessingElementsCounter, 0);
            Assert.AreEqual(sendChangedTags.ProcessingValuesCounter, 0);
            Assert.IsTrue(sendChangedTags.IsTerminated);
        }

        [TestMethod]
        public void TestGetCurrentSectionRange1()
        {
            var helper = new ChangedAlarmsFiller(alarmsCounter: 100, valuesCounter: 10);
            var sendChangedTags = new ChangedAlarmsHelper(helper.ChangedAlarms);
            
            int cc = 0;
            for (int ii = 0; ii < helper.MaxAlarms; ii++)
            {
                var list = sendChangedTags.GetChangedElementsBlock();
                sendChangedTags.SplitChangedElementsBlock();
                var expectedValues = helper.MaxValues;
                var expectedTags = Math.Max(1, helper.MaxAlarms / (int)Math.Pow(2, ii));
                if (expectedTags == 1)
                {
                    expectedValues = Math.Max(1, helper.MaxValues / (int)Math.Pow(2, cc));
                    cc++;
                }
                else
                    cc = 0;
                
                Assert.AreEqual(sendChangedTags.ProcessingElementsCounter, expectedTags);
                Assert.AreEqual(list.Count, expectedTags);
                Assert.AreEqual(sendChangedTags.ProcessingValuesCounter, expectedValues);
                Assert.AreEqual(list[0].AlarmsStatus.Count, expectedValues);
            }
        }

        [TestMethod]
        public void TestGetCurrentSectionRange2()
        {
            var helper = new ChangedAlarmsFiller(alarmsCounter: 33, valuesCounter: 5);
            var sendChangedTags = new ChangedAlarmsHelper(helper.ChangedAlarms);

            int cc = 0;
            for (int ii = 0; ii < helper.MaxAlarms; ii++)
            {
                var list = sendChangedTags.GetChangedElementsBlock();
                sendChangedTags.SplitChangedElementsBlock();
                var expectedValues = helper.MaxValues;
                var expectedTags = Math.Max(1, helper.MaxAlarms / (int)Math.Pow(2, ii));
                if (expectedTags == 1)
                {
                    expectedValues = Math.Max(1, helper.MaxValues / (int)Math.Pow(2, cc));
                    cc++;
                }
                else
                    cc = 0;

                Assert.AreEqual(sendChangedTags.ProcessingElementsCounter, expectedTags);
                Assert.AreEqual(list.Count, expectedTags);
                Assert.AreEqual(sendChangedTags.ProcessingValuesCounter, expectedValues);
                Assert.AreEqual(list[0].AlarmsStatus.Count, expectedValues);
            }
        }


        [TestMethod]
        public void TestIsTerminated1()
        {
            var helper = new ChangedAlarmsFiller(alarmsCounter: 100, valuesCounter: 10);

            int counterCycle = 0;
            int counterSplit = 0;

            var sendChangedTags = new ChangedAlarmsHelper(helper.ChangedAlarms);
            while (!sendChangedTags.IsTerminated)
            {
                counterCycle++;
                var list = sendChangedTags.GetChangedElementsBlock();
                sendChangedTags.GoToNextChangedElementsBlock();
            }
            Assert.AreEqual(counterCycle, 1);
            Assert.AreEqual(sendChangedTags.ReadyElementsCounter, 0);
            Assert.AreEqual(sendChangedTags.ReadyValuesCounter, 0);

            counterCycle = 0;
            sendChangedTags = new ChangedAlarmsHelper(helper.ChangedAlarms);
            for (; counterSplit < 4; counterSplit++)
                sendChangedTags.SplitChangedElementsBlock();
            
            int expectedProcessingTags = helper.MaxAlarms / (int)Math.Pow(2, counterSplit);
            int expectedCycle = (int)Math.Ceiling((double)helper.MaxAlarms / (double)expectedProcessingTags);
            int expectedProcessingTagsLastCycle = helper.MaxAlarms - ((helper.MaxAlarms / expectedProcessingTags) * expectedProcessingTags);
            if (expectedProcessingTagsLastCycle == 0)
                expectedProcessingTagsLastCycle = expectedProcessingTags;
            while (!sendChangedTags.IsTerminated)
            {
                counterCycle++;
                var list = sendChangedTags.GetChangedElementsBlock();
                if (counterCycle < expectedCycle)
                    Assert.AreEqual(sendChangedTags.ProcessingElementsCounter, expectedProcessingTags);
                else
                    Assert.AreEqual(sendChangedTags.ProcessingElementsCounter, expectedProcessingTagsLastCycle);
                Assert.AreEqual(sendChangedTags.ProcessingValuesCounter, helper.MaxValues);
                sendChangedTags.GoToNextChangedElementsBlock();
                Assert.AreEqual(sendChangedTags.ProcessedElementsCounter, Math.Min(counterCycle * expectedProcessingTags, helper.MaxAlarms));
            }

            Assert.AreEqual(sendChangedTags.ProcessedElementsCounter, helper.MaxAlarms);

            Assert.AreEqual(counterCycle, expectedCycle);
            Assert.AreEqual(sendChangedTags.ReadyElementsCounter, 0);
            Assert.AreEqual(sendChangedTags.ReadyValuesCounter, 0);
        }

        [TestMethod]
        public void TestIsTerminated2()
        {
            var helper = new ChangedAlarmsFiller(alarmsCounter: 100, valuesCounter: 10);

            int counterCycle = 0;
            int counterSplit = 0;

            var sendChangedTags = new ChangedAlarmsHelper(helper.ChangedAlarms);
            for (; counterSplit < 4; counterSplit++)
                sendChangedTags.SplitChangedElementsBlock();

            for (; counterSplit > 0; counterSplit--)
                sendChangedTags.MergeChangedElementsBlock();

            int expectedProcessingTags = helper.MaxAlarms / (int)Math.Pow(2, counterSplit);
            int expectedCycle = (int)Math.Ceiling((double)helper.MaxAlarms / (double)expectedProcessingTags);
            int expectedProcessingTagsLastCycle = helper.MaxAlarms - ((helper.MaxAlarms / expectedProcessingTags) * expectedProcessingTags);
            if (expectedProcessingTagsLastCycle == 0)
                expectedProcessingTagsLastCycle = expectedProcessingTags;
            while (!sendChangedTags.IsTerminated)
            {
                counterCycle++;
                var list = sendChangedTags.GetChangedElementsBlock();
                if (counterCycle < expectedCycle)
                    Assert.AreEqual(sendChangedTags.ProcessingElementsCounter, expectedProcessingTags);
                else
                    Assert.AreEqual(sendChangedTags.ProcessingElementsCounter, expectedProcessingTagsLastCycle);
                Assert.AreEqual(sendChangedTags.ProcessingValuesCounter, helper.MaxValues);
                sendChangedTags.GoToNextChangedElementsBlock();
                Assert.AreEqual(sendChangedTags.ProcessedElementsCounter, Math.Min(counterCycle * expectedProcessingTags, helper.MaxAlarms));
            }

            Assert.AreEqual(sendChangedTags.ProcessedElementsCounter, helper.MaxAlarms);

            Assert.AreEqual(counterCycle, expectedCycle);
            Assert.AreEqual(sendChangedTags.ReadyElementsCounter, 0);
            Assert.AreEqual(sendChangedTags.ReadyValuesCounter, 0);
        }

        [TestMethod]
        public void TestSplitAndMergeBlock()
        {
            var helper = new ChangedAlarmsFiller(alarmsCounter: 10, valuesCounter: 10);

            int counterSplit = 0;
            int excpected = 0;
            var sendChangedTags = new ChangedAlarmsHelper(helper.ChangedAlarms);
            while (sendChangedTags.SplitChangedElementsBlock())
            {
                if (!sendChangedTags.IsTerminated)
                    excpected++;
                counterSplit++;
            }
            Assert.AreEqual(counterSplit, excpected);

            counterSplit = 0;
            excpected = 0;
            sendChangedTags = new ChangedAlarmsHelper(helper.ChangedAlarms);
            while (sendChangedTags.SplitChangedElementsBlock())
                counterSplit++;
            while (sendChangedTags.MergeChangedElementsBlock())
                counterSplit--;
            Assert.AreEqual(counterSplit, excpected);
        }
    }
}
