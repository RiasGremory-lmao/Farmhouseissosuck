using System;
using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace FarmhouseUpgradeConfigurable;

[HarmonyPatch(typeof(CarpenterMenu), "robinConstructionMessage")]
public class PatchRobinConstructionMessage
{
	private static bool Prefix(CarpenterMenu __instance)
	{
		try
		{
			int upgradeLevel = Game1.player.houseUpgradeLevel.Value;
			if (!ModEntry.UpgradeConfigs.TryGetValue(upgradeLevel, out UpgradeConfig config))
			{
				return true;
			}
			if (1 == 0)
			{
			}
			string text = upgradeLevel switch
			{
				0 => "I'll start upgrading your house tomorrow! It should take about " + config.Days + " days!", 
				1 => "This upgrade will make your house more spacious! Just wait " + config.Days + " days!", 
				2 => "Your house will be fully upgraded in " + config.Days + " days! Enjoy the extra space!", 
				_ => "I'll get started on the upgrade soon!", 
			};
			if (1 == 0)
			{
			}
			string message = text;
			Game1.DrawDialogue(Game1.getCharacterFromName("Robin"), message);
			return false;
		}
		catch (Exception value)
		{
			ModEntry.Instance.Monitor.Log($"Error updating Robin's construction message: {value}", (LogLevel)4);
			return true;
		}
	}
}
