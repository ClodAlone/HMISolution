using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Utilities.DirectorySizeHelper;
using System.Linq;

namespace UtilitiesUnitTests
{
    [TestClass]
    public class DirectorySizeTests
    {
        [TestMethod]
        public void TestInvalidCharsInPath()
        {
            var invalidPathChars = System.IO.Path.GetInvalidPathChars();
            string path = String.Format(@"C:\Temp{0}\", invalidPathChars.GetValue(0));
            DirectorySize directorySize = new DirectorySize(path);
            Assert.IsTrue(directorySize.LastBytesSize == 0);
        }

        [TestMethod]
        public void TestInvalidDisk()
        {
            char disk = 'A';
            string path = String.Format(@"{0}:\", disk);
            while (System.IO.Directory.Exists(path))
            {
                disk = (char)((int)disk + 1);
                path = String.Format(@"{0}:\", disk);
            }
            
            DirectorySize directorySize = new DirectorySize(path);
            Assert.IsTrue(directorySize.LastBytesSize == 0);
        }

        [TestMethod]
        public void TestSystemPath()
        {
            string path = System.Environment.SystemDirectory;
            DirectorySize directorySize = new DirectorySize(path);
            Assert.IsTrue(directorySize.LastBytesSize > 0);
        }

        [TestMethod]
        public void TestLastBytesSize()
        {
            using (var helper = new DirectoryFilesHelper(filesNumber: 1, bytesSize: 1))
            {
                DirectorySize directorySize = new DirectorySize(helper.Path);
                
                Assert.IsTrue(directorySize.LastBytesSize == 0);
                helper.AddNewGroupFiles();
                Assert.IsTrue(directorySize.LastBytesSize == 0);

                directorySize.CheckFilesSize();
                Assert.IsTrue(directorySize.LastBytesSize == 1);
                helper.AddNewGroupFiles();
                Assert.IsTrue(directorySize.LastBytesSize == 1);

                directorySize.CheckFilesSize();
                Assert.IsTrue(directorySize.LastBytesSize == 2);
                helper.AddNewGroupFiles();
                Assert.IsTrue(directorySize.LastBytesSize == 2);

                directorySize.CheckFilesSize();
                Assert.IsTrue(directorySize.LastBytesSize == 3);
            }

            using (var helper = new DirectoryFilesHelper(filesNumber: 10, bytesSize: 1))
            {
                DirectorySize directorySize = new DirectorySize(helper.Path);

                Assert.IsTrue(directorySize.LastBytesSize == 0);
                helper.AddNewGroupFiles();
                Assert.IsTrue(directorySize.LastBytesSize == 0);

                directorySize.CheckFilesSize();
                Assert.IsTrue(directorySize.LastBytesSize == 10);
                helper.AddNewGroupFiles();
                Assert.IsTrue(directorySize.LastBytesSize == 10);

                directorySize.CheckFilesSize();
                Assert.IsTrue(directorySize.LastBytesSize == 20);
                helper.AddNewGroupFiles();
                Assert.IsTrue(directorySize.LastBytesSize == 20);

                directorySize.CheckFilesSize();
                Assert.IsTrue(directorySize.LastBytesSize == 30);
            }

            using (var helper = new DirectoryFilesHelper(filesNumber: 1, bytesSize: 10))
            {
                DirectorySize directorySize = new DirectorySize(helper.Path);

                Assert.IsTrue(directorySize.LastBytesSize == 0);
                helper.AddNewGroupFiles();
                Assert.IsTrue(directorySize.LastBytesSize == 0);

                directorySize.CheckFilesSize();
                Assert.IsTrue(directorySize.LastBytesSize == 10);
                helper.AddNewGroupFiles();
                Assert.IsTrue(directorySize.LastBytesSize == 10);

                directorySize.CheckFilesSize();
                Assert.IsTrue(directorySize.LastBytesSize == 20);
                helper.AddNewGroupFiles();
                Assert.IsTrue(directorySize.LastBytesSize == 20);

                directorySize.CheckFilesSize();
                Assert.IsTrue(directorySize.LastBytesSize == 30);
            }
        }

        [TestMethod]
        public void TestSizeOnDischargeOldestFiles()
        {
            using (var helper = new DirectoryFilesHelper(filesNumber: 100, bytesSize: 1))
            {
                DirectorySize directorySize = new DirectorySize(helper.Path);
                helper.AddNewGroupFiles();
                directorySize.CheckFilesSize();
                Assert.IsTrue(directorySize.LastBytesSize == 100);
                
                directorySize.DischargeOldestFiles(bytesToRemove: 1);
                directorySize.CheckFilesSize();
                Assert.IsTrue(directorySize.LastBytesSize == 99);

                directorySize.DischargeOldestFiles(bytesToRemove: 9);
                directorySize.CheckFilesSize();
                Assert.IsTrue(directorySize.LastBytesSize == 90);

                directorySize.DischargeOldestFiles(bytesToRemove: 90);
                directorySize.CheckFilesSize();
                Assert.IsTrue(directorySize.LastBytesSize == 0);
            }

            using (var helper = new DirectoryFilesHelper(filesNumber: 100, bytesSize: 1))
            {
                DirectorySize directorySize = new DirectorySize(helper.Path);
                helper.AddNewGroupFiles();
                directorySize.CheckFilesSize();
                Assert.IsTrue(directorySize.LastBytesSize == 100);

                directorySize.DischargeOldestFiles(bytesToRemove: 1000);
                directorySize.CheckFilesSize();
                Assert.IsTrue(directorySize.LastBytesSize == 0);
            }

            using (var helper = new DirectoryFilesHelper(filesNumber: 1, bytesSize: 100))
            {
                DirectorySize directorySize = new DirectorySize(helper.Path);
                helper.AddNewGroupFiles();
                directorySize.CheckFilesSize();
                Assert.IsTrue(directorySize.LastBytesSize == 100);

                directorySize.DischargeOldestFiles(bytesToRemove: 1);
                directorySize.CheckFilesSize();
                Assert.IsTrue(directorySize.LastBytesSize == 100);
            }

            using (var helper = new DirectoryFilesHelper(filesNumber: 1, bytesSize: 100))
            {
                DirectorySize directorySize = new DirectorySize(helper.Path);
                helper.AddNewGroupFiles();
                directorySize.CheckFilesSize();
                Assert.IsTrue(directorySize.LastBytesSize == 100);

                directorySize.DischargeOldestFiles(bytesToRemove: 1000);
                directorySize.CheckFilesSize();
                Assert.IsTrue(directorySize.LastBytesSize == 0);
            }
        }

        [TestMethod]
        public void TestCreationTimeOnDischargeOldestFiles()
        {
            using (var helper = new DirectoryFilesHelper(filesNumber: 10, bytesSize: 1))
            {
                DirectorySize directorySize = new DirectorySize(helper.Path);
                for (int ii = 0; ii < 10; ii++)
                {
                    helper.AddNewFileName(String.Format("MyTestFile{0}", ii));
                }
                
                var wholeFiles = helper.GetWholeFilesName();
                foreach (var file in wholeFiles)
                {
                    directorySize.DischargeOldestFiles(bytesToRemove: 1);
                    var files = helper.GetWholeFilesName();
                    Assert.IsFalse(files.Contains(file));
                }

                Assert.IsTrue(directorySize.LastBytesSize == 0);
            }
        }

        [TestMethod]
        public void TestExtensionParameter()
        {
            using (var helper = new DirectoryFilesHelper(filesNumber: 10, bytesSize: 10, extension: "log"))
            {
                DirectorySize directorySize = new DirectorySize(helper.Path, String.Format("*.{0}", helper.Extension));
                Assert.IsTrue(directorySize.LastBytesSize == 0);
                
                helper.AddNewGroupFiles();
                directorySize.CheckFilesSize();
                Assert.IsTrue(directorySize.LastBytesSize == 100);

                helper.AddNewFileName("MyFile1.txt");
                directorySize.CheckFilesSize();
                Assert.IsTrue(directorySize.LastBytesSize == 100);

                helper.AddNewFileName("MyFile1.log");
                directorySize.CheckFilesSize();
                Assert.IsTrue(directorySize.LastBytesSize == 110);

                directorySize.DischargeOldestFiles(110);
                directorySize.CheckFilesSize();
                Assert.IsTrue(directorySize.LastBytesSize == 0);
                Assert.IsTrue(helper.GetFilesNameFilterByExtension().Count == 0);
                Assert.IsTrue(helper.GetWholeFilesName().Count == 1);
                Assert.AreEqual(helper.GetWholeFilesName()[0], "MyFile1.txt");
            }
        }

        [TestMethod]
        public void TestBytesRemovedParameter()
        {
            using (var helper = new DirectoryFilesHelper(filesNumber: 10, bytesSize: 10))
            {
                DirectorySize directorySize = new DirectorySize(helper.Path);
                helper.AddNewGroupFiles();
                var bytesRemoved = directorySize.DischargeOldestFiles(10);
                Assert.AreEqual(bytesRemoved, 10);

                bytesRemoved = directorySize.DischargeOldestFiles(1);
                Assert.AreEqual(bytesRemoved, 0);

                bytesRemoved = directorySize.DischargeOldestFiles(1000);
                Assert.AreEqual(bytesRemoved, 90);

                Assert.AreEqual(helper.GetWholeFilesName().Count, 0);
            }
        }
    }
}
