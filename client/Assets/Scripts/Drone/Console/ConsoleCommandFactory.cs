using Adept.Logger;
using IngameDebugConsole.Console.External;
using JetBrains.Annotations;

namespace Drone.Console
{
    public static class ConsoleCommandFactory
    {
        private static readonly IAdeptLogger _logger = LoggerFactory.GetLogger("Console");

        [ConsoleMethod("unlockLevel", "unlocks levels from 1 to entered")]
        [UsedImplicitly]
        public static void UnlockLevel(int order)
        {
        }

        [ConsoleMethod("resetProgress", "reset all progress")]
        [UsedImplicitly]
        public static void ResetProgress()
        {
        }
    }
}