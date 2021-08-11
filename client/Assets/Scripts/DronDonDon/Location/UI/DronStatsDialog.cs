
using AgkUI.Binding.Attributes;
using AgkUI.Binding.Attributes.Method;

using UnityEngine;


namespace DronDonDon.Location.UI
{
    [UIController("UI/Dialog/pfDronState@embeded")]
    public class DronStatsDialog :MonoBehaviour
    {
        [UIOnClick("StopButton")]
        private void CloseButton()
        {
       
        }
        
        [UIOnClick("ShieldButton")]
        private void ShieldBoost()
        {
          
        }
        
        [UIOnClick("SpeedButton")]
        private void SpeedBoost()
        {
            
        }
        
        
    }
}