using GoblinGridPuzzle.Managers;
using GoblinGridPuzzle.Utilities.Constants;
using Godot;

namespace GoblinGridPuzzle.Game
{
    public partial class Main : Node
    {

        //class references
        private GridManager _gridManager;

        // scene references
        private PackedScene _buildingScene;

        // node references
        private Sprite2D _cursorNode;
        private Button _placeBuildingButtonNode;

        //complex variables
        private Vector2I? _hoveregGridCellPosition;

        public override void _Ready()
        {
            InitalizeVariables();
            ConnectSignals();
            _cursorNode.Visible = false;
        }

        private void InitalizeVariables()
        {
            _cursorNode = GetNode<Sprite2D>(GameConstants.CURSOR_PATH);
            _placeBuildingButtonNode = GetNode<Button>(GameConstants.PLACE_BUILDING_BUTON_PATH);
            _gridManager = GetNode<GridManager>(GameConstants.GRIDMANAGER_PATH);

            _buildingScene = GD.Load<PackedScene>(GameConstants.BUILDNG_PATH);

        }

        public override void _Process(double delta)
        {
            Vector2I gridPosition = _gridManager.GetMouseGridCellPosition();
            _cursorNode.GlobalPosition = gridPosition * GameConstants.GRID_SIZE;
            if (_cursorNode.Visible && (!_hoveregGridCellPosition.HasValue || _hoveregGridCellPosition != gridPosition))
            {
                _hoveregGridCellPosition = gridPosition;
                _gridManager.HighlightExpandedBuildableTiles(_hoveregGridCellPosition.Value, GameConstants.HIGHLIGHT_RADIUS);
            }
        }

        public override void _UnhandledInput(InputEvent evt)
        {
            if (_hoveregGridCellPosition.HasValue &&
            evt.IsActionPressed(GameConstants.LEFT_CLICK) &&
            _gridManager.IsTilePositionBuildable(_hoveregGridCellPosition.Value))
            {

                PlaceBuildingAtHoveredCellPosition();
                _cursorNode.Visible = false;
            }
        }

        private void ConnectSignals()
        {
            _placeBuildingButtonNode.Connect(Button.SignalName.Pressed, Callable.From(HandlePlacedBuildingPressed));
        }


        private void PlaceBuildingAtHoveredCellPosition()
        {
            if (!_hoveregGridCellPosition.HasValue) { return; }

            var building = _buildingScene.Instantiate<Node2D>();
            building.GlobalPosition = _hoveregGridCellPosition.Value * GameConstants.GRID_SIZE;
            AddChild(building);

            _hoveregGridCellPosition = null;
            _gridManager.ClearHighLightedTiles();
        }

        private void HandlePlacedBuildingPressed()
        {
            _cursorNode.Visible = true;
            _gridManager.HighLightBuildableTiles();
        }
    }
}