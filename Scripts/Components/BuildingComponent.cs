using GoblinGridPuzzle.AutoLoads;
using Godot;

namespace GoblinGridPuzzle.Components;

public partial class BuildingComponent : Node2D
{
    // variables
    [Export]
    public int BuildableRadius { get; private set; }

    public override void _Ready()
    {
        AddToGroup(nameof(BuildingComponent));
        Callable.From(() => GameEvents.RaiseBuidingPlaced(this)).CallDeferred();
    }

    public Vector2I GetGridCellPosition()
    {
        var gridPosition = GlobalPosition / 64;
        gridPosition = gridPosition.Floor();
        return new Vector2I((int)gridPosition.X, (int)gridPosition.Y);
    }
}
