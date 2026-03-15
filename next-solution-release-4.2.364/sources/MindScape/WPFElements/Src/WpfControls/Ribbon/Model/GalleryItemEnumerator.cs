using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Mindscape.WpfElements
{
  internal class GalleryEnumerator : IEnumerable
  {
    private readonly IEnumerable _source;

    internal GalleryEnumerator(IEnumerable source)
    {
      _source = source;
    }

    public IEnumerator GetEnumerator()
    {
      if (_source != null)
      {
        foreach (object o in _source)
        {
          GalleryGroup group = o as GalleryGroup;
          if (group != null)
          {
            foreach (object obj in group.Items)
            {
              yield return obj;
            }
          }
          else
          {
            yield return o;
          }
        }
      }
    }

    System.Collections.IEnumerator IEnumerable.GetEnumerator()
    {
      return this.GetEnumerator();
    }
  }
}
