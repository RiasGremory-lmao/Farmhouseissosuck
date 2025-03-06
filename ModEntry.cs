using System.Collections.Generic;
using GenericModConfigMenu;
using HarmonyLib;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using ContentPatcher;
using StardewValley;

namespace FarmhouseUpgradeConfigurable;

public class ModEntry : Mod
{
    private static Dictionary<int, UpgradeConfig> upgradeConfigs;

    public static ModEntry Instance { get; private set; }
    public static ModConfig Config { get; private set; }

    public static Dictionary<int, UpgradeConfig> UpgradeConfigs { get => upgradeConfigs; set => upgradeConfigs = value; }
        public override void Entry(IModHelper helper)
    {
        Instance = this;

        // Load the config or create a new one if missing
        try
        {
            UpgradeConfigs = helper.ReadConfig<Dictionary<int, UpgradeConfig>>();
            if (UpgradeConfigs == null || UpgradeConfigs.Count == 0)
            {
                Monitor.Log("config.json is empty! Generating default values.", LogLevel.Warn);
                UpgradeConfigs = GetDefaultConfig();
                helper.WriteConfig(UpgradeConfigs);
            }
        }
        catch (Exception ex)
        {
            Monitor.Log($"Error loading config.json, creating a new one. Details: {ex.Message}", LogLevel.Warn);
            UpgradeConfigs = GetDefaultConfig();
            helper.WriteConfig(UpgradeConfigs);
        }

        Harmony harmony = new Harmony(ModManifest.UniqueID);
        harmony.PatchAll();
        Monitor.Log("Farmhouse Upgrade Mod loaded successfully!", LogLevel.Info);
    }

    private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
    {
        var contentPatcherAPI = Helper.ModRegistry.GetApi<IContentPatcherAPI>("Pathoschild.ContentPatcher");

        if (contentPatcherAPI != null)
        {
            Monitor.Log("Content Patcher API found! Registering UpgradeDays token...", LogLevel.Info);

            contentPatcherAPI.RegisterToken(this.ModManifest, "UpgradeDays", () =>
            {
                if (Game1.player == null || Game1.player.houseUpgradeLevel == null)
                {
                    Monitor.Log("Game1.player is null or houseUpgradeLevel not set. Returning default value for UpgradeDays.", LogLevel.Warn);
                    return new[] { "3" }; // Mặc định là 3 ngày nếu không có dữ liệu
                }

                int upgradeLevel = Game1.player.houseUpgradeLevel.Value;
                if (!UpgradeConfigs.TryGetValue(upgradeLevel, out var config))
                {
                    Monitor.Log($"Upgrade level {upgradeLevel} not found in config, defaulting to 3 days.", LogLevel.Warn);
                    return new[] { "3" };
                }

                Monitor.Log($"UpgradeDays token set to {config.Days} days.", LogLevel.Info);
                return new[] { config.Days.ToString() };
            });
        }
        else
        {
            Monitor.Log("Could not find Content Patcher API. UpgradeDays token will not work!", LogLevel.Warn);
        }
    }
    private Dictionary<int, UpgradeConfig> GetDefaultConfig() => new()
    {
            {
                0,
                new UpgradeConfig
                {
                    Cost = 500,
                    Days = 2,
                    Materials = new Dictionary<int, int>
                    {
                        { 388, 150 },
                        { 390, 50 }
                    }
                }
            },
            {
                1,
                new UpgradeConfig
                {
                    Cost = 65000,
                    Days = 2,
                    Materials = new Dictionary<int, int>
                    {
                        { 709, 100 },
                        { 771, 50 }
                    }
                }
            },
            {
                2,
                new UpgradeConfig
                {
                    Cost = 100000,
                    Days = 2,
                    Materials = new Dictionary<int, int> { { 390, 100 } }
                }
            }
        };

    private void RegisterConfigMenu()
	{
        Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu")?.Register(ModManifest, delegate
		{
            UpgradeConfigs = GetDefaultConfig();
		}, delegate
		{
            Helper.WriteConfig(UpgradeConfigs);
		});
	}
}
