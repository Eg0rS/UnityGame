﻿using AgkUI.Binding.Attributes;
using AgkUI.Binding.Attributes.Method;
using AgkUI.Element.Text;
using Drone.Billing.Service;
using Drone.Shop.Descriptor;
using Drone.Shop.Service;
using UnityEngine;
using UnityEngine.UI;
using IoC.Attribute;

namespace Drone.Shop.UI
{
    [UIController("UI/Items/pfDronView@embeded")]
    public class ShopItemPanel : MonoBehaviour
    {
        [Inject]
        private ShopService _shopService;

        [Inject]
        private BillingService _billingService;

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

        private string _id;

        [UICreated]
        private void Init(ShopItemDescriptor itemDescriptor, bool isHasItem)
        {
            _id = itemDescriptor.Id;
            SetItemLabel(itemDescriptor.Name);
            SetItemCondition(itemDescriptor.Price, isHasItem);
            SetItemModel(itemDescriptor.Model);
            SetItemСharact(itemDescriptor.Energy, itemDescriptor.Durability);
        }

        [UIOnClick("BuyButton")]
        private void BuyClick()
        {
            if (_shopService.BuyDron(_id)) {
                _bought.SetActive(true);
                _buyButton.SetActive(false);
            } else {
                _bought.SetActive(false);
                _buyButton.SetActive(true);
            }
        }

        private void SetItemLabel(string label)
        {
            _label.GetComponent<UILabel>().text = label;
        }

        private void SetItemCondition(int price, bool isHasItem)
        {
            if (isHasItem) {
                _bought.SetActive(true);
                _buyButton.SetActive(false);
            } else {
                _bought.SetActive(false);
                _buyButton.SetActive(true);
                _price.GetComponent<UILabel>().text = price.ToString();
            }
        }

        private void SetItemModel(string model)
        {
            _model.GetComponent<RawImage>().texture = Resources.Load(model, typeof(Texture)) as Texture;
        }

        private void SetItemСharact(string energy, string durability)
        {
            _energyBar.GetComponent<Image>().sprite = Resources.Load(energy, typeof(Sprite)) as Sprite;
            _durabilityBar.GetComponent<Image>().sprite = Resources.Load(durability, typeof(Sprite)) as Sprite;
        }
    }
}