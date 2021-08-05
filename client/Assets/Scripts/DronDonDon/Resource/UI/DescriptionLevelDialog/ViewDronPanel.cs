using AgkUI.Binding.Attributes;
using AgkUI.Binding.Attributes.Method;
using AgkUI.Element.Text;
using DronDonDon.Inventory.Model;
using DronDonDon.Shop.Descriptor;
using IoC.Attribute;
using UnityEngine;

namespace DronDonDon.Resource.UI.DescriptionLevelDialog
{
    [UIController(PREFAB_NAME)]
    public class ViewDronPanel : MonoBehaviour
    {
        private const string PREFAB_NAME = "UI/Panel/pfChoiseDronPanel@embeded";

        [Inject] 
        private ShopDescriptor _shopDescriptor;
        
        [UIObjectBinding("Text")] 
        private GameObject _text;

        private InventoryItemModel _item;

        public InventoryItemModel Item
        {
            get => _item;
            set => _item = value;
        }
        
        [UICreated]
        private void Init(InventoryItemModel item)
        {
            _item = item;
            SetItemLabel(_shopDescriptor.ShopItemDescriptors.Find(x => x.Id.Equals(_item.Id)).Name);
        }
        
        private void SetItemLabel(string name)
        {
            _text.GetComponent<UILabel>().text = name;
        }

        [UIOnClick()]
        private void OnClick()
        {
            Debug.Log(_item.Id);
            //TODO отдавал в дрон сервис id выбранного дрона
        }
    }
}