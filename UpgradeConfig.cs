using System.Collections.Generic;

public class UpgradeConfig
{
    public int Cost { get; set; }
    public int Days { get; set; }
    public Dictionary<int, int> Materials { get; set; } = new();
}
