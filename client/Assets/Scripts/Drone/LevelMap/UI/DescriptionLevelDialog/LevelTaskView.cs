using AgkUI.Binding.Attributes;
using AgkUI.Element.Buttons;
using AgkUI.Element.Text;
using Drone.Levels.Descriptor;
using UnityEngine;

namespace Drone.LevelMap.UI.DescriptionLevelDialog
{
    [UIController(PREFAB_NAME)]
    public class LevelTaskView : MonoBehaviour
    {
        private const string PREFAB_NAME = "UI_Prototype/Dialog/DescriptionLevel/Task/pfTask@embeded";
       
        [UIComponentBinding("Star")]
        private ToggleButton _star;
        [UIComponentBinding("Label")]
        private UILabel _label;
        [UICreated]
        private void Init(LevelTask task, bool isDone)
        {
            _star.Interactable = false;
            _star.IsOn = isDone;
            _label.text = task.Description;
        }
    }
}