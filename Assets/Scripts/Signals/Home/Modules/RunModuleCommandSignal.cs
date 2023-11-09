
namespace TDL.Signals
{
    public class RunModuleCommandSignal : ISignal
    {
        public string ModuleName { get; private set; }

        public RunModuleCommandSignal(string moduleName)
        {
            ModuleName = moduleName;
        }
    }
}