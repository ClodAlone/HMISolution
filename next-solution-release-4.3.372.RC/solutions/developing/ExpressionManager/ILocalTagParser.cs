using OPCUAViewModel;

namespace ExpressionManager
{
    public interface ILocalTagParser
    {
        OPCUAEntityReference Parse(string tag);
    }
}