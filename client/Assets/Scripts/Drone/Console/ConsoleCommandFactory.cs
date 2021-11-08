using System.Collections.Generic;
using System.Linq;
using Adept.Logger;
using Drone.LevelMap.Levels.Descriptor;
using Drone.LevelMap.Levels.IoC;
using Drone.LevelMap.Levels.Service;
using IngameDebugConsole.Console.External;
using JetBrains.Annotations;
using AppContext = IoC.AppContext;

namespace Drone.Console
{
    public static class ConsoleCommandFactory
    {
        private static readonly IAdeptLogger _logger = LoggerFactory.GetLogger("Console");

        [ConsoleMethod("unlockLevel", "unlocks levels from 1 to entered")]
        [UsedImplicitly]
        public static void UnlockLevel(int order)
        {
            LevelService levelService = AppContext.Resolve<LevelService>();
            LevelDescriptorRegistry levelDescriptorRegistry = AppContext.Resolve<LevelDescriptorRegistry>();
            List<LevelDescriptor> levels = new List<LevelDescriptor>(levelDescriptorRegistry.LevelDescriptors.OrderBy(x => x.Order));
            for (int i = 0; i < order; i++) {
                levelService.SetLevelProgress(levels[i].Id, 0, 0, 0, 0);
            }
        }
        [ConsoleMethod("resetProgress", "reset all progress")]
        [UsedImplicitly]
        public static void ResetProgress()
        {
            LevelService levelService = AppContext.Resolve<LevelService>();
            levelService.ResetPlayerProgress();
        }
    }
}