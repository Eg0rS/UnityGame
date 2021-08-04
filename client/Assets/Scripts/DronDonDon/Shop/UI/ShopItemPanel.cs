using System.Collections.Generic;
using AgkCommons.Extension;
using AgkUI.Binding.Attributes;
using AgkUI.Element.Text;
using DronDonDon.Shop.Descriptor;
using UnityEngine;

namespace DronDonDon.Shop.UI
{
    [UIController("UI/Items/pfDronView@embeded")]
    public class ShopItemPanel : MonoBehaviour
    {
        [UIObjectBinding("Label")] 
        private GameObject _label;
        
        [UIObjectBinding("DurabilityBar")]
        private GameObject _durabilityBar;
        
        [UIObjectBinding("EnergyBar")]
        private GameObject _energyBar;
        
        [UIObjectBinding("BuyButton")]
        private GameObject _buyButton;
        
        [UIObjectBinding("Bought")]
        private GameObject _bought;
        
        [UIObjectBinding("Price")]
        private GameObject _price;

        [UIObjectBinding("Model")]
        private GameObject _model;
        
        
        
        [UICreated]
        private void Init(ShopItemDescriptor itemDescriptor, bool isHasItem)
        {
            Debug.Log("создан");
            SetItemLabel(itemDescriptor.Name);
            SetItemCondition(itemDescriptor.Price, isHasItem);
            // SetItemModel(itemDescriptor.Model);
            //SetItemСharact(itemDescriptor.Energy, itemDescriptor.Durability);

        }

        public void SetItemLabel(string name)
        {
            _label.GetComponent<UILabel>().text = name;
        }

        public void SetItemCondition(int price, bool isHasItem)
        {
            if (isHasItem)
            {
                _bought.SetActive(true);
                _buyButton.SetActive(false);
            }
            else
            {
                _bought.SetActive(false);
                _buyButton.SetActive(true);
                _price.GetComponent<UILabel>().text = price.ToString();
            }
        }
    }
}