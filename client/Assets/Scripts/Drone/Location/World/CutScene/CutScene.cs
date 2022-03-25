using UnityEngine;

namespace Drone.Location.World.CutScene
{
    public class CutScene : MonoBehaviour
    {
        public CutSceneType Type;
    }

    public enum CutSceneType
    {
        NONE,
        START,
        FINISH
    }
}