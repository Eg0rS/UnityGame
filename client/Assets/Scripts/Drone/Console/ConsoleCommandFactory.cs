using System.Collections.Generic;
using System.Linq;
using Adept.Logger;
using Drone.Levels.Descriptor;
using Drone.Levels.Service;
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

            List<LevelDescriptor> levels = levelService.LevelsDescriptors.Levels.ToList();
            for (int i = 0; i < order; i++) {
                levelService.SetLevelProgress(levels[i].Id, new Dictionary<LevelTask, bool>(), 0);
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