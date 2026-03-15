using System.Collections.Generic;

namespace Bornander.UI.Commands
{
    public interface IWarningRepository<T>
    {
        IEnumerable<T> Ignored { get; }

        void Ignore(T warning);

        void Acknowledge(T warning);
    }
}
