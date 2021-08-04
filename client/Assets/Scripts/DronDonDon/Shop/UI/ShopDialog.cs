using System.Collections.Generic;
using AgkUI.Binding.Attributes;
using AgkUI.Binding.Attributes.Method;
using AgkUI.Dialog.Attributes;
using AgkUI.Dialog.Service;
using DronDonDon.Core.UI.Dialog;
using IoC.Attribute;
using IoC.Util;
using Adept.Logger;
using AgkCommons.Event;
using AgkCommons.Input.Gesture.Model;
using AgkCommons.Input.Gesture.Model.Gestures;
using AgkCommons.Input.Gesture.Service;
using AgkUI.Core.Model;
using AgkUI.Core.Service;
using AgkUI.Element.Text;
using DronDonDon.Game.Levels.Event;
using DronDonDon.Inventory.Service;
using DronDonDon.Shop.Descriptor;
using DronDonDon.Shop.Event;
using DronDonDon.Shop.Service;
using UnityEngine;
using UnityEngine.UI;


namespace DronDonDon.Shop.UI
{
    [UIController("UI/Dialog/pfShopDialog@embeded")]
    [UIDialogFog(FogPrefabs.EMBEDED_SHADOW_FOG)]
    public class ShopDialog : MonoBehaviour
    {
        private static readonly IAdeptLogger _logger = LoggerFactory.GetLogger<ShopDialog>();

        [Inject] 
        private UIService _uiService;
        
        [Inject] 
        private ShopService _shopService;

        [Inject] 
        private ShopDescriptor _shopDescriptor;

        [Inject] 
        private InventoryService _inventoryService;
        
        [Inject]
        private IGestureService _gestureService;

        public ListPositionCtrl _listPositionCtrl;
        public List<ShopItemPanel> _ShopItemPanels = new List<ShopItemPanel>();
        [UICreated]
        public void Init()
        {
            CreateShopItem();
            _gestureService.AddSwipeHandler(OnSwiped,false);
        }

        private void OnSwiped(Swipe swipe)
        {
            _logger.Debug("asdc");
        }

        private void OnShopEventUpdated()
        {
        }
        
        private void CreateShopItem()
        {
            List<ShopItemDescriptor> shopItemDescriptors = _shopDescriptor.ShopItemDescriptors;
            GameObject itemContainer = GameObject.Find("ScrollContainer");
            List<ShopItemPanel> panels = new List<ShopItemPanel>();
            int i = 0;
            foreach (var itemDescriptor in shopItemDescriptors)
            {
                i++;
                bool isHasItem = _inventoryService.Inventory.HasItem(itemDescriptor.Id);
                _uiService.Create<ShopItemPanel>(UiModel
                        .Create<ShopItemPanel>(itemDescriptor, isHasItem)
                        .Container(itemContainer))
                    .Then(controller =>
                    {
                        _ShopItemPanels.Add(controller);
                    })
                    .Done();

            }
            _uiService.Create<ScrollController>(UiModel
                    .Create<ScrollController>(_ShopItemPanels)
                    .Container(itemContainer)).Then(controller => { _listPositionCtrl = controller.Control;})
                .Done();
           
            
        }
        /*[UIOnSwipe("SwipePlane")]
        private void OnSwipe(Swipe swipe)
        {
            _logger.Debug("Swipe!");
            if (swipe.Check(HorizontalSwipeDirection.LEFT)) {
                Debug.Log("Swipe button 1 left");
                MoveLeft();
            } else if (swipe.Check(HorizontalSwipeDirection.RIGHT)) {
                Debug.Log("Swipe button 1 right");
                MoveRight();
            }
        }*/

        [UIOnClick("ButtonContainer")]
        private void onContainer()
        {
            _logger.Debug("asdc");
        }

        [UIOnClick("LeftButton")]
        private void OnLeftClick()
        {
            _logger.Debug("clickleft");
            MoveLeft();
           
        }
        [UIOnClick("RightButton")]
        private void OnRightClick()
        {
            _logger.Debug("clickrekt");
            MoveRight();
        }

        private void MoveLeft()
        {
            _listPositionCtrl.gameObject.GetComponent<ListPositionCtrl>().SetUnitMove(1);
        }
        private void MoveRight()
        {
            _listPositionCtrl.gameObject.GetComponent<ListPositionCtrl>().SetUnitMove(-1);;
        }
    }
}