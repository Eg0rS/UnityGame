using Drone.Location.Model;
using Drone.Location.Model.BaseModel;
using Drone.Location.Model.ShieldBooster;
using UnityEngine;

namespace Drone.Location.World.ShieldBooster
{
    public class ShieldBoosterController : MonoBehaviour, IWorldObjectController<ShieldBoosterModel>
    {
        public WorldObjectType ObjectType { get; private set; }

        public void Init(ShieldBoosterModel model)
        {
            ObjectType = model.ObjectType;
        }
        
        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.GetComponent<PrefabModel>().ObjectType == (WorldObjectType.DRON)) {
                other.gameObject.SetActive(false);
            }
        }
    }
}