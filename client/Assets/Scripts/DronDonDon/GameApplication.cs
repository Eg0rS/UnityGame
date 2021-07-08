using AgkCommons.Event;
using AgkCommons.Util;
using DronDonDon.Core.Event;
using DronDonDon.Core.Filter;
using JetBrains.Annotations;

namespace DronDonDon
{
    public class GameApplication : GameEventDispatcher
    {
        [PublicAPI]
        public static GameApplication Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
            UnityMainThreadDispatcher.AddDispatcherToScene();
            DontDestroyOnLoad(gameObject);
            RunFilter();
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            Dispatch(new GamePausedEvent(GamePausedEvent.GAME_PAUSED, pauseStatus));
        }

        private void OnApplicationQuit()
        {
            Dispatch(new GameQuitEvent(GameQuitEvent.BEFORE_QUIT));
        }

        private void RunFilter()
        {
            AppFilterChain filterChain = gameObject.AddComponent<AppFilterChain>();
            filterChain.AddFilter(new IoCFilter());
            
            filterChain.AddFilter(new ConfigLoadFilter());
            filterChain.AddFilter(new AppSettingsFilter());
            filterChain.AddFilter(new StartGameFilter());
            filterChain.Next();
        }
    }
}