using AgkUI.Binding.Attributes;
using AgkUI.Element.Text;
using DeliveryRush.Inventory.Model;
using DeliveryRush.Shop.Descriptor;
using IoC.Attribute;
using UnityEngine;
using UnityEngine.UI;

namespace DeliveryRush.Resource.UI.DescriptionLevelDialog
{
    [UIController(PREFAB_NAME)]
    public class ViewDronPanel : MonoBehaviour
    {
        private const string PREFAB_NAME = "UI/Panel/pfChoiseDronPanel@embeded";

        [Inject] private ShopDescriptor _shopDescriptor;

        [UIObjectBinding("Text")] 
        private GameObject _text;
        
        [UIObjectBinding("Model")]
        private GameObject _model;

        public string ItemId { get; private set; }

        [UICreated]
        private void Init(InventoryItemModel item)
        {
            ItemId = item.Id;
            SetItemLabel(_shopDescriptor.ShopItemDescriptors.Find(x => x.Id.Equals(ItemId)).Name);
            SetItemModel(_shopDescriptor.ShopItemDescriptors.Find(x => x.Id.Equals(ItemId)).Model);
            
        }
        private void SetItemModel(string model)
        {
            _model.GetComponent<RawImage>().texture = Resources.Load(model, typeof(Texture)) as Texture;
        }
        private void SetItemLabel(string title)
        {
            _text.GetComponent<UILabel>().text = title;
        }
    }
}