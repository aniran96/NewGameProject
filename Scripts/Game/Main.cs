using GoblinGridPuzzle.Utilities.Constants;
using Godot;

namespace GoblinGridPuzzle.Game
{
    public partial class Main : Node2D
    {
        // scene references
        private PackedScene buildingScene;

        // node references
        private Sprite2D cursorNode;

        public override void _Ready()
        {
            cursorNode = GetNode<Sprite2D>(GameConstants.CURSOR);
            buildingScene = GD.Load<PackedScene>(GameConstants.BUILDNG_PATH);
        }

        public override void _Process(double delta)
        {
            Vector2 mousePosition = GetGlobalMousePosition();
            Vector2 gridPosition = mousePosition / 64;
            gridPosition = gridPosition.Floor();
            cursorNode.GlobalPosition = gridPosition * 64;
        }
    }
}