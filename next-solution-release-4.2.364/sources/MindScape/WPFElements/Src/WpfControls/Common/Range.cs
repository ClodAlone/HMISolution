
namespace Mindscape.WpfElements
{
  internal class Range<T>
  {
    private readonly T _minimum;
    private readonly T _maximum;

    public Range(T mininum, T maximum)
    {
      _minimum = mininum;
      _maximum = maximum;
    }

    public T Minimum
    {
      get { return _minimum; }
    }

    public T Maximum
    {
      get { return _maximum; }
    }
  }
}
