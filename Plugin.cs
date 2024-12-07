﻿using System;
using System.Collections.Generic;
using BepInEx;
using HarmonyLib;
using UnityEngine;
using System.Linq;
using System.IO;
using UnityEngine.Assertions;

namespace CustomAdv;

public enum PrivateLogLevel
{
    None,
    Error,
    Warning,
    Info,
    Debug
};

[BepInPlugin($"{MOD_AUTHOR}.{MOD_NAME_LOWER}.mod", MOD_NAME, MOD_VERSION)]
public class Plugin : BaseUnityPlugin
{
    public const string MOD_AUTHOR = "105gun";
    public const string MOD_NAME = "CustomAdvMutsumi";
    public const string MOD_NAME_LOWER = "customadvmutsumi";
    public const string MOD_VERSION = "1.0.0.0";
    static PrivateLogLevel pluginLogLevel = PrivateLogLevel.Debug;

    private void Start()
    {
        ModLog("Initializing");
        var harmony = new Harmony($"{MOD_AUTHOR}.{MOD_NAME_LOWER}.mod");
        harmony.PatchAll();
        // LoadData(Info);
        ModLog("Initialization completed");
    }

    public static void LoadData(PluginInfo Info)
    {
		var dir = Path.GetDirectoryName(Info.Location);
		var excel = dir + "/Data/SourceCard.xlsx";
		var sources = Core.Instance.sources;
		ModUtil.ImportExcel(excel, "Thing", sources.things);
		ModUtil.ImportExcel(excel, "Chara", sources.charas);
		ModUtil.ImportExcel(excel, "CharaText", sources.charaText);
    }
    public static void ModLog(string message, PrivateLogLevel logLevel = PrivateLogLevel.Info)
    {
        if (logLevel > pluginLogLevel)
        {
            return;
        }
        switch (logLevel)
        {
            case PrivateLogLevel.Error:
                message = $"[{MOD_NAME}][Error] {message}";
                break;
            case PrivateLogLevel.Warning:
                message = $"[{MOD_NAME}][Warning] {message}";
                break;
            case PrivateLogLevel.Info:
                message = $"[{MOD_NAME}][Info] {message}";
                break;
            case PrivateLogLevel.Debug:
                message = $"[{MOD_NAME}][Debug] {message}";
                break;
            default:
                break;
        }
        System.Console.WriteLine(message);
    }
}