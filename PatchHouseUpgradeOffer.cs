using System;
using System.Collections.Generic;
using System.Linq;
using FarmhouseUpgradeConfigurable;
using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;

namespace FarmhouseUpgradeMod.Patches
{
    [HarmonyPatch(typeof(GameLocation), "houseUpgradeOffer")]
    public class PatchHouseUpgradeOffer
    {
        static bool Prefix(GameLocation __instance)
        {
            try
            {
                int upgradeLevel = Game1.player.houseUpgradeLevel.Value;
                if (!ModEntry.UpgradeConfigs.TryGetValue(upgradeLevel, out UpgradeConfig config))
                    return true;

                string materialText = string.Join(", ", config.Materials.Select(m =>
                {
                    string name = ItemRegistry.GetData(m.Key.ToString())?.DisplayName ?? "Unknown Material";
                    return $"{name} ({m.Value})";
                }));

                string offerMessage = upgradeLevel switch
                {
                    0 => $"I can increase the size of your house and add a kitchen. It will cost {config.Cost}g and you'll also need to provide me with {materialText}. Are you interested?",
                    1 => $"I can increase the size of your house and add a nursery. It will cost {config.Cost}g and you'll also need to provide me with {materialText}. Are you interested?",
                    2 => $"I can add a cellar to your house. The cellar can be used to age certain products, like wine and cheese. It will cost {config.Cost}g. Are you interested?",
                    _ => "I can't upgrade your house any further."
                };

                Game1.drawObjectDialogue(offerMessage);
                return false;
            }
            catch (Exception ex)
            {
                ModEntry.Instance.Monitor.Log($"Error modifying house upgrade offer: {ex}", LogLevel.Error);
                return true;
            }
        }
    }
}
