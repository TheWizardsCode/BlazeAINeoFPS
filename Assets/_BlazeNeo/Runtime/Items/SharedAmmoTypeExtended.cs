using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NeoFPS.ModularFirearms;
using NeoFPS;

namespace NeoFPS
{
    [CreateAssetMenu(fileName = "ExtendedAmmoType", menuName = "NeoFPS/Inventory/Wizards Code/Ammo Type", order = NeoFpsMenuPriorities.inventory_ammoType)]
    public class SharedAmmoTypeExtended : SharedAmmoType
    {
        [SerializeField, Tooltip("If the player drops this item (usually because of death or similar) what pickup object should be used?")]
        private InventoryItemPickup m_DefaultPickup;

        /// <summary>
        /// Fill up as many default pickup items as are needed and return them as an array.
        /// This does not remove them from the inventory, this is a convenience method
        /// to enable ammo to be dropped by other systems which should position the object
        /// appropriately and maange inventory stocks.
        /// </summary>
        public virtual InventoryItemPickup[] GetDefaultPickups(int quantity)
        {
            int quantityPerPickup = m_DefaultPickup.m_ItemPrefab.quantity; // TODO: this requires m_ItemPrefab to be public, which it is not by default.
            int pickupQuantity = quantity / quantityPerPickup; //TODO: this will silently drop fractional amounts
            InventoryItemPickup[] pickups = new InventoryItemPickup[pickupQuantity];
            for (int i = 0; i < pickupQuantity; i++)
            {
                pickups[i] = Instantiate(m_DefaultPickup.gameObject).GetComponent<InventoryItemPickup>();
            }
            return pickups;
        }
    }
}
