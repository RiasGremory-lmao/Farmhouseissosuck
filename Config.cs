using FarmhouseUpgradeConfigurable;

public class ModConfig
{
    public Dictionary<int, UpgradeConfig> UpgradeConfigs { get; set; } = new Dictionary<int, UpgradeConfig>
    {
        { 0, new UpgradeConfig { Cost = 10000, Days = 3, Materials = new Dictionary<int, int> { { 388, 150 } } } },
        { 1, new UpgradeConfig { Cost = 65000, Days = 3, Materials = new Dictionary<int, int> { { 709, 100 } } } },
        { 2, new UpgradeConfig { Cost = 100000, Days = 3, Materials = new Dictionary<int, int> { { 390, 100 } } } }
    };
}
