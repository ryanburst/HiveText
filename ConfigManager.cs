using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text.RegularExpressions;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

namespace ConfigManager
{
    public class Manager
    {
        private static object? configManager;

        internal static void Patch_FejdStartup(FejdStartup __instance)
        {
            Assembly? bepinexConfigManager = AppDomain.CurrentDomain.GetAssemblies()
                .FirstOrDefault(a => a.GetName().Name == "ConfigurationManager");

            Type? configManagerType = bepinexConfigManager?.GetType("ConfigurationManager.ConfigurationManager");
            configManager = configManagerType == null
                ? null
                : BepInEx.Bootstrap.Chainloader.ManagerObject.GetComponent(configManagerType);
        }

        internal static BaseUnityPlugin? _plugin = null!;

        internal static BaseUnityPlugin plugin
        {
            get
            {
                if (_plugin is not null) return _plugin;
                IEnumerable<TypeInfo> types;
                try
                {
                    types = Assembly.GetExecutingAssembly().DefinedTypes.ToList();
                }
                catch (ReflectionTypeLoadException e)
                {
                    types = e.Types.Where(t => t != null).Select(t => t.GetTypeInfo());
                }

                _plugin = (BaseUnityPlugin)BepInEx.Bootstrap.Chainloader.ManagerObject.GetComponent(types.First(t =>
                    t.IsClass && typeof(BaseUnityPlugin).IsAssignableFrom(t)));

                return _plugin;
            }
        }

        public static bool testing = true;
        private static bool hasConfigSync = true;
        private static object? _configSync;

        private static object? configSync
        {
            get
            {
                if (_configSync != null || !hasConfigSync) return _configSync;
                if (Assembly.GetExecutingAssembly().GetType("ServerSync.ConfigSync") is { } configSyncType)
                {
                    _configSync = Activator.CreateInstance(configSyncType, plugin.Info.Metadata.GUID + " PieceManager");
                    configSyncType.GetField("CurrentVersion")
                        .SetValue(_configSync, plugin.Info.Metadata.Version.ToString());
                    configSyncType.GetProperty("IsLocked")!.SetValue(_configSync, true);
                }
                else
                {
                    hasConfigSync = false;
                }

                return _configSync;
            }
        }

        private static ConfigEntry<T> config<T>(string group, string name, T value, ConfigDescription description)
        {
            ConfigEntry<T> configEntry = plugin.Config.Bind(group, name, value, description);

            configSync?.GetType().GetMethod("AddConfigEntry")!.MakeGenericMethod(typeof(T))
                .Invoke(configSync, new object[] { configEntry });

            return configEntry;
        }

        private static ConfigEntry<T> config<T>(string group, string name, T value, string description) =>
            config(group, name, value, new ConfigDescription(description));
    }
}