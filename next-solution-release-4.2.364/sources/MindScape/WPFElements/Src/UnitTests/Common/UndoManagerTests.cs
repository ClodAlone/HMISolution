using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace Mindscape.WpfElements.UnitTests
{
  [TestFixture]
  public class UndoManagerTests
  {
    private static readonly Action<string> _nop = delegate(string s) { };
    private static List<string> _undone;
    private static Action<string> _addToList;

    [SetUp]
    public void SetUp()
    {
      _undone = new List<string>();
      _addToList = delegate(string s) { _undone.Add(s); };
    }

    [Test]
    public void InitiallyNothingToUndo()
    {
      UndoManager<string> undoManager = new UndoManager<string>(_nop);
      Assert.IsFalse(undoManager.CanUndo);
    }

    [Test]
    public void CanUndoAfterPush()
    {
      UndoManager<string> undoManager = new UndoManager<string>(_nop);
      undoManager.PushUndo("fie");
      Assert.IsTrue(undoManager.CanUndo);
    }

    [Test]
    public void CannotUndoMoreThanHasBeenDone()
    {
      UndoManager<string> undoManager = new UndoManager<string>(_nop);
      undoManager.PushUndo("fie");
      Assert.IsTrue(undoManager.CanUndo);
      undoManager.Undo();
      Assert.IsFalse(undoManager.CanUndo);
    }

    [Test]
    public void UndoReturnsPushedState()
    {
      UndoManager<string> undoManager = new UndoManager<string>(_addToList);
      undoManager.PushUndo("fie");
      undoManager.PushUndo("tchah");
      undoManager.Undo();
      undoManager.Undo();
      Assert.AreEqual(2, _undone.Count);
      Assert.AreEqual("tchah", _undone[0]);
      Assert.AreEqual("fie", _undone[1]);
    }

    [Test]
    public void ExcessiveUndoIsNoOp()
    {
      UndoManager<string> undoManager = new UndoManager<string>(_addToList);
      undoManager.PushUndo("fie");
      undoManager.PushUndo("tchah");
      undoManager.Undo();
      undoManager.Undo();
      undoManager.Undo();
      undoManager.Undo();
      Assert.AreEqual(2, _undone.Count);
      Assert.AreEqual("tchah", _undone[0]);
      Assert.AreEqual("fie", _undone[1]);
    }

    [Test]
    public void CanInterleaveDosAndUndos()
    {
      UndoManager<string> undoManager = new UndoManager<string>(_addToList);
      undoManager.PushUndo("fie");
      undoManager.PushUndo("tchah");
      undoManager.Undo();
      undoManager.PushUndo("zounds");
      undoManager.Undo();
      undoManager.Undo();
      Assert.AreEqual(3, _undone.Count);
      Assert.AreEqual("tchah", _undone[0]);
      Assert.AreEqual("zounds", _undone[1]);
      Assert.AreEqual("fie", _undone[2]);
    }

    private class UndoingVerifier
    {
      public UndoManager<string> UndoManager { get; set; }

      public void AssertUndoing(string s)
      {
        Assert.IsTrue(UndoManager.Undoing);
      }
    }

    [Test]
    public void UndoingIsSetCorrectly()
    {
      //int dummy = Assert.Counter;  // clear the counter

      UndoingVerifier verifier = new UndoingVerifier();
      verifier.UndoManager = new UndoManager<string>(verifier.AssertUndoing);
      verifier.UndoManager.PushUndo("fie");
      Assert.IsFalse(verifier.UndoManager.Undoing);
      verifier.UndoManager.Undo();
      Assert.IsFalse(verifier.UndoManager.Undoing);
      //Assert.AreEqual(3, Assert.Counter);
    }

    [Test]
    public void CannotUndoAfterClear()
    {
      UndoManager<string> undoManager = new UndoManager<string>(_addToList);
      undoManager.PushUndo("fie");
      Assert.IsTrue(undoManager.CanUndo);
      undoManager.Clear();
      Assert.IsFalse(undoManager.CanUndo);
      undoManager.Undo();
      Assert.AreEqual(0, _undone.Count);
    }

    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void MustProvideUndoCallback()
    {
      new UndoManager<string>(null);
    }
  }
}
