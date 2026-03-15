using System.ComponentModel;
using System.Globalization;

namespace Mindscape.WpfElements
{
  internal interface IMaskedTextStrategy
  {
    string GetText(bool includeLiterals, bool includePrompts);
    bool SetText(string text, out int textPosition, out MaskedTextResultHint hint);
    string ToDisplayString();
    string Mask { get; }
    bool IsEditPosition(int position);
    int FindEditPositionFrom(int position, bool direction);
    bool VerifyChar(char input, int position, out MaskedTextResultHint hint);
    bool InsertAt(string input, int position, out int textPosition, out MaskedTextResultHint hint);
    bool RemoveAt(int start, int end, out int textPosition, out MaskedTextResultHint hint);
    bool Replace(string input, int start, int end, out int textPosition, out MaskedTextResultHint hint);
    char PromptChar { get; set; }

    bool MaskCompleted { get; }
    bool MaskFull { get; }

    CultureInfo Culture { get; }
  }
}
