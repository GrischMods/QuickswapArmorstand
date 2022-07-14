using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace Grisch.QuickswapArmorstand;

[HarmonyPatch(typeof(ArmorStand))]
public class ArmorStandPatches
{
    [HarmonyPrefix]
    [HarmonyPatch(nameof(ArmorStand.UseItem))]
    static bool ArmorstandQuickchange(ArmorStand __instance, Humanoid user, ItemDrop.ItemData item)
    {
        if (
            !CheckKeyDown(("left alt"))
            // add escape here for poseable armor stands as they 
            // might not work correctly
        ) return true;
        
        Debug.Log("Quickchanging gear");

        List<ItemDrop> itemList = QuickSwap.StashStandEq(__instance);
        QuickSwap.MovePlayerEqToStand(__instance, user);
        QuickSwap.AddEquipStandItemsFromList(itemList, user);
        return false;
    }
    
    private static bool CheckKeyDown(string value)
    {
        try
        {
            return Input.GetKey(value.ToLower());
        }
        catch
        {
            return false;
        }
    }
}