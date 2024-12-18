using System;
using System.Collections.Generic;
using BepInEx;
using HarmonyLib;
using UnityEngine;
using System.Linq;
using System.IO;
using UnityEngine.Assertions;

namespace CustomAdv;

/*
 * In DramaSource sheet file, we use sister_change function to redirect to our own function
 * Before invoking the original sister_change function, call setFlag mutsumi_routing, {routeId}
 */

 /*
 * Used drama flags:
 *      mutsumi_routing
 *      mutsumi_isFriend 0: not friend, 1: faction member, 2: faction member and _affinity > 75 3: faction member and _affinity > 125 and mutsumi_cucumber > 1 4: complete all dialog
 *      mutsumi_cucumber 0：can't be built, 1: can be built 2: built but not harvested, 3: built and harvested
 *      mutsumi_harvest_length (int) Days
 *      mutsumi_harvest_date (int)
 */
// DramaOutcome.sister_change
[HarmonyPatch(typeof(DramaOutcome), nameof(DramaOutcome.sister_change))]
class DramaOutcomeRoutingPatch
{
    static void UpdateFlag(string name, int value, bool force = false)
    {
        if (!force && EClass.player.dialogFlags.TryGetValue(name, 0) >= value)
        {
            return;
        }
        EClass.player.dialogFlags[name] = value;
    }

    /*
     * Note: Each time we call ShowDialog, all the IF conditions will be calculated immediately, just before we really enter these dialogs.
     * So it's meaning less to call this sync function in dialog, all the updated flag values will not be reflected in branches.
     */
    public static void SyncParameter(Chara charaMutsumi)
    {
        // Update isFriend
        if (charaMutsumi.IsPCFactionOrMinion)
        {
            if (charaMutsumi._affinity >= 125 && EClass.player.dialogFlags.TryGetValue("mutsumi_cucumber", 0) > 1)
            {
                UpdateFlag("mutsumi_isFriend", 3);
            }
            else if (charaMutsumi._affinity >= 75)
            {
                UpdateFlag("mutsumi_isFriend", 2);
                UpdateFlag("mutsumi_cucumber", 1);
            }
            else
            {
                UpdateFlag("mutsumi_isFriend", 1);
            }
        }

        // Update cucumber harvest
        if (EClass.player.dialogFlags.TryGetValue("mutsumi_cucumber", 0) == 2 &&
            EClass.game.world.date.GetRaw() >= EClass.player.dialogFlags.TryGetValue("mutsumi_harvest_date", 0))
        {
            Plugin.ModLog("Cucumber is ready to harvest", PrivateLogLevel.Debug);
            UpdateFlag("mutsumi_cucumber", 3);
        }

        // Debug print
        Plugin.ModLog("mutsumi_isFriend: " + EClass.player.dialogFlags.TryGetValue("mutsumi_isFriend", 0), PrivateLogLevel.Debug);
        Plugin.ModLog("mutsumi_cucumber: " + EClass.player.dialogFlags.TryGetValue("mutsumi_cucumber", 0), PrivateLogLevel.Debug);
        Plugin.ModLog("mutsumi_harvest_date: " + EClass.player.dialogFlags.TryGetValue("mutsumi_harvest_date", 0), PrivateLogLevel.Debug);
        Plugin.ModLog("mutsumi_harvest_length: " + EClass.player.dialogFlags.TryGetValue("mutsumi_harvest_length", 0), PrivateLogLevel.Debug);
    }

    // routeId = 1
    static void CompleteDialog(DramaOutcome __instance)
    {
        Chara charaMutsumi = __instance.cc;
        charaMutsumi.ModAffinity(EClass.pc, 10, true);
        for (int i = 0; i < 20; i++)
        {
            charaMutsumi.LevelUp();
        }
        // STR, DEX, etc.
        for (int lvl = 0; lvl < 10; lvl++)
        {
            for (int i = 70; i < 79; i++)
            {
                charaMutsumi.ModExp(i, 65535);
            }
        }
        UpdateFlag("mutsumi_isFriend", 4);
    }

    // routeId = 2
    static void EnableCucumberHarvest(DramaOutcome __instance)
    {
        Chara charaMutsumi = __instance.cc;

        charaMutsumi.ModAffinity(EClass.pc, 10, true);
        UpdateFlag("mutsumi_harvest_length", 7, true); // Days
        HarvestCucumber();
    }

    // routeId = 3
    static void HarvestCucumber()
    {
        int cnt = EClass.player.dialogFlags.TryGetValue("mutsumi_isFriend", 0) == 4 ? EClass.rnd(5) + 1 : 1;
        EClass.pc.Pick(ThingGen.Create("lucky_cucumber").SetNum(cnt));
        EClass.pc.PlaySound("pick_thing", 1f, true);
        ResetCucumberHarvest();
    }

    // Private function
    static void ResetCucumberHarvest()
    {
        UpdateFlag("mutsumi_cucumber", 2, true);
        UpdateFlag("mutsumi_harvest_date",
            EClass.game.world.date.GetRaw() + EClass.player.dialogFlags.TryGetValue("mutsumi_harvest_length", 7) * 1440
            , true);
    }

    static bool Prefix(DramaOutcome __instance)
    {
        if (__instance.cc.id == "adv_mutsumi")
        {
            int routeId = EClass.player.dialogFlags.TryGetValue("mutsumi_routing", 0);
            switch (routeId)
            {
                case 1:
                    CompleteDialog(__instance);
                    break;
                case 2:
                    EnableCucumberHarvest(__instance);
                    break;
                case 3:
                    HarvestCucumber();
                    break;
                default:
                    UpdateFlag("mutsumi_routing", 0, true);
                    return true;
            }
            UpdateFlag("mutsumi_routing", 0, true);
            return false;
        }
        return true;
    }
}

/*
// Debug
// DramaManager.CheckIF
[HarmonyPatch(typeof(DramaManager), nameof(DramaManager.CheckIF))]
class DramaManagerCheckIFPatch
{
    static void Postfix(DramaManager __instance, string IF, ref bool __result)
    {
        Plugin.ModLog($"CheckIF {IF}, {__result}", PrivateLogLevel.Error);
    }
}*/