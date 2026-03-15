using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mindscape.WpfElements
{
  internal static class SelectionRangeUtils
  {
    public static SelectionUpdateInfo GetSelectionUpdateInfo(int startIndex, int previousIndex, int currentIndex)
    {
      int startFillIndex = 0;
      int endFillIndex = -1;
      int startEmptyIndex = 0;
      int endEmptyIndex = -1;
      if (previousIndex != currentIndex)
      {
        if (startIndex <= previousIndex && previousIndex < currentIndex)
        {
          startFillIndex = previousIndex + 1;
          endFillIndex = currentIndex;
        }
        else if (startIndex <= currentIndex && currentIndex < previousIndex)
        {
          startEmptyIndex = currentIndex + 1;
          endEmptyIndex = previousIndex;
        }
        else if (currentIndex <= startIndex && startIndex <= previousIndex)
        {
          startFillIndex = currentIndex;
          endFillIndex = startIndex - 1;
          startEmptyIndex = startIndex + 1;
          endEmptyIndex = previousIndex;
        }
        else if (currentIndex < previousIndex && previousIndex <= startIndex)
        {
          startFillIndex = currentIndex;
          endFillIndex = previousIndex - 1;
        }
        else if (previousIndex < currentIndex && currentIndex <= startIndex)
        {
          startEmptyIndex = previousIndex;
          endEmptyIndex = currentIndex - 1;
        }
        else
        {
          startEmptyIndex = previousIndex;
          endEmptyIndex = startIndex - 1;
          startFillIndex = startIndex + 1;
          endFillIndex = currentIndex;
        }
      }
      return new SelectionUpdateInfo(startFillIndex, endFillIndex, startEmptyIndex, endEmptyIndex);
    }
  }

  internal struct SelectionUpdateInfo
  {
    private int _startFillIndex, _endFillIndex, _startEmptyIndex, _endEmptyIndex;

    public SelectionUpdateInfo(int startFillIndex, int endFillIndex, int startEmptyIndex, int endEmptyIndex)
    {
      _startFillIndex = startFillIndex;
      _endFillIndex = endFillIndex;
      _startEmptyIndex = startEmptyIndex;
      _endEmptyIndex = endEmptyIndex;
    }

    public int StartFillIndex
    {
      get { return _startFillIndex; }
    }

    public int EndFillIndex
    {
      get { return _endFillIndex; }
    }

    public int StartEmptyIndex
    {
      get { return _startEmptyIndex; }
    }

    public int EndEmptyIndex
    {
      get { return _endEmptyIndex; }
    }
  }
}
