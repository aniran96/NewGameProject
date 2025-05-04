using Godot;

namespace GoblinGridPuzzle.Buildings;

[GlobalClass]
public partial class BuildingResource : Resource
{
    [Export]
    public int BuildableRadius { get; private set; }
    [Export]
    public int HarvestableRadius { get; private set; }
    [Export]
    public PackedScene BuildingScene { get; private set; }
}
