using BepInEx;

using HarmonyLib;

using System;
using System.Collections.Generic;
using System.Reflection;

using UnityEngine;


namespace ArmorSwap
{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    public class ArmorSwap : BaseUnityPlugin
    {
        public const string PluginGuid = "Eardwulf.valheim.ArmorSwap";
        public const string PluginName = "ArmorSwap";
        public const string PluginVersion = "0.0.3";

        Harmony _harmony;

        public void Awake()
        {
            _harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), harmonyInstanceId: PluginGuid);
        }

        public static List<ItemDrop> StashStandEq(ArmorStand stand)
        {
            List<ItemDrop> itemlist = new List<ItemDrop>();
            for (int i = 0; i < stand.m_slots.Count; i++)
            {
                if (!stand.HaveAttachment(i)) continue;

                string itemstring = stand.m_nview.GetZDO().GetString(i.ToString() + "_item");
                ItemDrop itemDrop = ObjectDB.instance.GetItemPrefab(itemstring).GetComponent<ItemDrop>();
                ItemDrop.LoadFromZDO(i, itemDrop.m_itemData, stand.m_nview.GetZDO());
                itemlist.Add(itemDrop);
                stand.DestroyAttachment(i);
                stand.m_nview.GetZDO().Set(i.ToString() + "_item", "");
                stand.m_nview.InvokeRPC(ZNetView.Everybody, "RPC_SetVisualItem", new object[] { i, "", 0 });
                stand.UpdateSupports();
                stand.m_cloths = stand.GetComponentsInChildren<Cloth>();
            }

            return itemlist;
        }

        public static void MovePlayerEqToStand(ArmorStand stand, Humanoid player)
        {
            List<ItemDrop.ItemData> playerItems = new List<ItemDrop.ItemData>
            {
                player.m_rightItem,
                player.m_leftItem,
                player.m_chestItem,
                player.m_legItem,
                player.m_helmetItem,
                player.m_shoulderItem
            };
            stand.CancelInvoke(nameof(ArmorStand.UpdateAttach));
            foreach (ItemDrop.ItemData eq in playerItems)
            {
                if (eq is null) continue;
                int slot = FindFreeSlot(stand, eq);
                if (slot < 0) throw new IndexOutOfRangeException("This should never happen! Call a grownup!");
                stand.m_queuedItem = eq;
                stand.m_queuedSlot = slot;
                stand.UpdateAttach();
            }

            stand.InvokeRepeating(nameof(ArmorStand.UpdateAttach), 0f, 0.1f);
        }

        public static void AddEquipStandItemsFromList(List<ItemDrop> itemlist, Humanoid player)
        {
            foreach (ItemDrop itemdrop in itemlist)
            {
                player.m_inventory.AddItem(itemdrop.m_itemData);
                player.EquipItem(itemdrop.m_itemData);
            }
        }

        private static int FindFreeSlot(ArmorStand stand, ItemDrop.ItemData item)
        {
            for (int i = 0; i < stand.m_slots.Count; i++)
            {
                if (stand.CanAttach(stand.m_slots[i], item)) return i;
            }

            return -1;
        }

        public void OnDestroy()
        {
            _harmony?.UnpatchSelf();
        }
    }
}

