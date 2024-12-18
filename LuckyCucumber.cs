using System;
using System.Collections.Generic;
using BepInEx;
using HarmonyLib;
using UnityEngine;
using System.Linq;
using System.IO;
using UnityEngine.Assertions;

namespace CustomAdv;

// FoodEffect.ProcTrait
[HarmonyPatch(typeof(FoodEffect), nameof(FoodEffect.ProcTrait))]
class FoodEffectProcTraitPatch
{
    static void Prefix(FoodEffect __instance, Chara c, Card t)
    {
        if (t.id == "lucky_cucumber")
        {
            c.PlaySound("ding_skill", 1f, true);
            c.LevelUp();
            Point point = c.pos;

            if (EClass.rnd(5) == 0)
            {
                int luckyPoint = Mathf.Clamp(EClass.rnd(c.LUC), 5, 100);
                for (int i = 0; i < Mathf.Sqrt(luckyPoint); i++)
                {
                    c.LevelUp();
                }
                ActEffect.ProcAt(EffectId.Explosive, luckyPoint, t.blessedState, c, null, point, true);
                EClass._map.ModFire(point.x, point.z, luckyPoint);
                Msg.SetColor("ono");
                Msg.SayRaw("cucumber_1".lang());
            }
            else
            {
                Msg.SetColor("ono");
                Msg.SayRaw("cucumber_0".lang());
            }
        }
    }
}