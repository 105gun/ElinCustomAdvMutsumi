using System;
using System.Collections.Generic;
using BepInEx;
using HarmonyLib;
using UnityEngine;
using System.Linq;
using System.IO;
using UnityEngine.Assertions;
using CustomAdv;

namespace CustomAdvMod;

/*
 * Well, I found that the current implementation of ModUtil is incorrect.
 * When you load your own chara data from excel file, it simply call SourceChara.CreateRow in the end.
 * Which still need some extra initialization to make it possible to be rendered in the game:
 * RenderRow.OnImportData(_tiles) & SourceCard.AddRow(elementMap, renderData...), or maybe more.
 * In future it might be fixed. But for now, here is just a simple workaround.
 */

/*
// SourceChara.CreateRow
[HarmonyPatch(typeof(SourceChara), nameof(SourceChara.CreateRow))]
class CreateRowPatch
{
    static void Postfix(SourceChara __instance, ref SourceChara.Row __result)
    {
        if (__result.id == "adv_mutsumi") // id of my custom Chara
        {
            if (__result.elementMap == null)
            {
                __result._tiles = new int[0];
                Core.Instance.sources.cards.AddRow(__result, true);
            }
        }
    }
}
*/
/*
[HarmonyPatch(typeof(SourceThing), nameof(SourceThing.CreateRow))]
class SourceThingCreateRowPatch
{
    static void Postfix(SourceThing __instance, ref SourceThing.Row __result)
    {
        if (__result.id == "mutsumi_book") // id of my custom Thing
        {
            Plugin.ModLog($"SourceThingCreateRowPatch.Postfix {__result.id}", PrivateLogLevel.Debug);
            // print TileType.dict
            foreach (var item in TileType.dict)
            {
                Plugin.ModLog($"TileType.dict {item.Key} {item.Value}", PrivateLogLevel.Debug);
            }
            Plugin.ModLog($"?????? {__result.tiles.Count()}", PrivateLogLevel.Debug);
            __result._tiles = new int[0];
            __result._tileType = "";
            Core.Instance.sources.cards.AddRow(__result, false);
            //EClass.sources.cards.map.Add(__result.id, __result);
        }
    }
}
//RenderRow.SetTiles
[HarmonyPatch(typeof(RenderRow), nameof(RenderRow.SetTiles))]
class SetTilesPatch
{
    static void Prefix(RenderRow __instance)
    {
        Plugin.ModLog($"SetTilesPatch.Postfix {__instance._tiles} {__instance.tiles}", PrivateLogLevel.Debug);
        Plugin.ModLog($"SetTilesPatch.Postfix {__instance._tiles.Count()} {__instance.tiles.Count()}", PrivateLogLevel.Debug);
        // Print them
        foreach (var item in __instance._tiles)
        {
            Plugin.ModLog($"SetTilesPatch.Postfix _tiles {item}", PrivateLogLevel.Debug);
        }

        foreach (var item in __instance.tiles)
        {
            Plugin.ModLog($"SetTilesPatch.Postfix tiles {item}", PrivateLogLevel.Debug);
        }
        

    }
}


/*
// SourceElement.CreateRow
[HarmonyPatch(typeof(SourceElement), nameof(SourceElement.CreateRow))]
class CreateElementPatch
{
    static void Postfix(SourceElement __instance, ref SourceElement.Row __result)
    {
        Plugin.ModLog($"CreateElementPatch.Postfix {__result.id}", CardboardBoxLogLevel.Error);
    }
}*/