using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace ArmorSwap
{
[HarmonyPatch(typeof(ArmorStand))]
public class ArmorStandPatches
{
    [HarmonyPrefix]
    [HarmonyPatch(nameof(ArmorStand.UseItem))]
    static bool ArmorstandQuickchange(ArmorStand __instance, Humanoid user, ItemDrop.ItemData item)
    {
        if (!CheckKeyDown(("left alt"))) return true;
        // add escape here for poseable armor stands as they 
        // might not work correctly
        if (!PrivateArea.CheckAccess(__instance.transform.position, flash: true)) {
                Debug.Log(message: "You are not permitted to touch this armor stand");
                return false; }
        Debug.Log(message: "Swapping equipped item(s) with item(s) on the target armor stand.");

        List<ItemDrop> itemList = ArmorSwap.StashStandEq(__instance);
        ArmorSwap.MovePlayerEqToStand(__instance, user);
        ArmorSwap.AddEquipStandItemsFromList(itemList, user);
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
}