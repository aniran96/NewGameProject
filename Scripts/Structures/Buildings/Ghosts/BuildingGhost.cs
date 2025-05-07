using Godot;

namespace GoblinGridPuzzle.Structures.Buildings.Ghosts;

public partial class BuildingGhost : Node2D
{
    public void SetInvalid()
    {
        Modulate = Colors.Red;
    }

    public void SetValid()
    {
        Modulate = Colors.White;
    }
}
