using System;
using System.Collections.Generic;
using BepInEx;
using HarmonyLib;
using UnityEngine;
using System.Linq;
using System.IO;
using UnityEngine.Assertions;

namespace CustomAdv;


class AddCustomAdv
{
    public static bool isCustomAdvLoaded()
    {
        foreach (var item in EClass.core.game.cards.listAdv)
        {
            if (item.id == "adv_mutsumi")
            {
                Plugin.ModLog("CustomAdv is loaded", PrivateLogLevel.Debug);
                return true;
            }
        }
        Plugin.ModLog("CustomAdv is not loaded", PrivateLogLevel.Debug);
        return false;
    }

    public static void AddCustomAdventurer()
    {
        Plugin.ModLog("Adding CustomAdv", PrivateLogLevel.Debug);
        Chara chara = CharaGen.Create("adv_mutsumi", -1);
        List<Zone> list = EClass.game.world.region.ListTowns();
        Zone homeZone = list.RandomItem<Zone>();

        //DEBUG
        //homeZone = EClass.pc.homeZone;

        chara.SetHomeZone(homeZone);
        chara.global.transition = new ZoneTransition
        {
            state = ZoneTransition.EnterState.RandomVisit
        };
        homeZone.AddCard(chara);
        EClass.game.cards.listAdv.Add(chara);
        //DEBUG
        //EClass.pc.party.AddMemeber(chara);
        //EClass.pc.homeBranch.AddMemeber(chara);
    }
}

// Game.Load
[HarmonyPatch(typeof(Scene), nameof(Scene.Init))]
class SceneInitPatch
{
    static void Postfix(Scene __instance, Scene.Mode newMode)
    {
        Plugin.ModLog("Scene.Init", PrivateLogLevel.Debug);
        if (newMode == Scene.Mode.StartGame && EClass.core != null && EClass.core.game != null && EClass.player != null && !AddCustomAdv.isCustomAdvLoaded())
        {
            //DEBUG
            //Thing test = ThingGen.Create("mutsumi_book");
            //Plugin.ModLog($"ThingGen.Create {test.id}", PrivateLogLevel.Debug);
            //EClass.pc.things.Add(test);
            // EClass.player.chara.EQ_ID("mutsumi_book", -1, Rarity.Artifact);
            AddCustomAdv.AddCustomAdventurer();
        }
    }
}

// Chara.RestockEquip
[HarmonyPatch(typeof(Chara), nameof(Chara.RestockEquip))]
class CharaRestockEquipPatch
{
    static void Prefix(Chara __instance, bool onCreate)
    {
        Plugin.ModLog("Chara.RestockEquip", PrivateLogLevel.Debug);
        if (__instance.id == "adv_mutsumi")
        {
            if (onCreate)
            {
                __instance.AddThing(__instance.EQ_ID("mutsumi_book", -1, Rarity.Artifact));
                __instance.EQ_ID("mutsumi_guitar", -1, Rarity.Artifact);
            }
        }
    }
}