using System;
using System.Collections.Generic;
using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using ContentPatcher;

namespace FarmhouseUpgradeConfigurable;

[HarmonyPatch(typeof(GameLocation), "houseUpgradeAccept")]
public class PatchHouseUpgradeAccept
{
    private static bool Prefix(GameLocation __instance)
	{
		try
		{
			int upgradeLevel = Game1.player.houseUpgradeLevel.Value;
            if (ModEntry.UpgradeConfigs == null)
            {
                ModEntry.Instance.Monitor.Log("UpgradeConfigs is null! Ensure config.json is correctly loaded.", LogLevel.Error);
                return true;
            }

            if (!ModEntry.UpgradeConfigs.TryGetValue(upgradeLevel, out UpgradeConfig config))
            {
                ModEntry.Instance.Monitor.Log($"UpgradeConfigs does not contain an entry for level {upgradeLevel}.", LogLevel.Error);
                return true;
            }

            if (Game1.player.Money < config.Cost)
			{
				Game1.drawObjectDialogue("You do not have enough gold!");
				return false;
			}
			foreach (KeyValuePair<int, int> material in config.Materials)
			{
				if (!HasItem(Game1.player, material.Key, material.Value))
				{
					string materialName = ItemRegistry.GetData(material.Key.ToString())?.DisplayName ?? "Unknown Material";
					Game1.drawObjectDialogue($"You need more {materialName} ({material.Value} required)!");
					return false;
				}
			}
			Game1.player.Money -= config.Cost;
			Game1.player.daysUntilHouseUpgrade.Value = config.Days;
			foreach (KeyValuePair<int, int> material2 in config.Materials)
			{
                RemoveItem(Game1.player, material2.Key, material2.Value);
			}

            Game1.drawDialogue(Game1.getCharacterFromName("Robin"));
            return false;
		}
		catch (Exception value)
		{
			ModEntry.Instance.Monitor.Log($"Error processing house upgrade: {value}", (LogLevel)4);
			return true;
		}
	}

	private static bool HasItem(Farmer player, int itemId, int requiredAmount)
	{
		int count = 0;
		foreach (Item item in player.Items)
		{
			if (item != null && item.ParentSheetIndex == itemId)
			{
				count += item.Stack;
				if (count >= requiredAmount)
				{
					return true;
				}
			}
		}
		return false;
	}

	private static void RemoveItem(Farmer player, int itemId, int removeAmount)
	{
		for (int i = 0; i < player.Items.Count; i++)
		{
			Item item = player.Items[i];
			if (item != null && item.ParentSheetIndex == itemId)
			{
				if (item.Stack > removeAmount)
				{
					item.Stack -= removeAmount;
					break;
				}
				removeAmount -= item.Stack;
				player.Items[i] = null;
				if (removeAmount <= 0)
				{
					break;
				}
			}
		}
	}
}
