using GoblinGridPuzzle.AutoLoads;
using GoblinGridPuzzle.Buildings;
using Godot;

namespace GoblinGridPuzzle.Components;

public partial class BuildingComponent : Node2D
{
    // variables
    [Export(PropertyHint.File, "*.tres")]
    private string _buildingResourcePath;

    public BuildingResource BuildingResource { get; private set; }

    public override void _Ready()
    {
        InitalizeVariables();
        AddToGroup(nameof(BuildingComponent));
        Callable.From(() => GameEvents.RaiseBuidingPlaced(this)).CallDeferred();
    }

    private void InitalizeVariables()
    {
        BuildingResource = GD.Load<BuildingResource>(_buildingResourcePath);
    }

    public Vector2I GetGridCellPosition()
    {
        var gridPosition = GlobalPosition / 64;
        gridPosition = gridPosition.Floor();
        return new Vector2I((int)gridPosition.X, (int)gridPosition.Y);
    }
}
