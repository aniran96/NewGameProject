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
        private PackedScene _towerScene;
        private PackedScene _villageScene;
        private PackedScene _toPlaceBuildingScene;

        // node references
        private Sprite2D _cursorNode;
        private Button _placeTowerButtonNode;
        private Button _placeVillageButtonNode;
        private Node2D _ySortRootNode;

        //complex variables
        private Vector2I? _hoveregGridCellPosition;

        public override void _Ready()
        {
            InitalizeVariables();
            ConnectSignals();
            _cursorNode.Visible = false;
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

        private void InitalizeVariables()
        {
            _ySortRootNode = GetNode<Node2D>(GameConstants.YSORTROOT_PATH);
            _cursorNode = GetNode<Sprite2D>(GameConstants.CURSOR_PATH);
            _placeTowerButtonNode = GetNode<Button>(GameConstants.PLACE_BUILDING_BUTON_PATH);
            _placeVillageButtonNode = GetNode<Button>(GameConstants.PLACE_VILLAGE_BUTTON_PATH);
            _gridManager = GetNode<GridManager>(GameConstants.GRIDMANAGER_PATH);

            _towerScene = GD.Load<PackedScene>(GameConstants.BUILDNG_PATH);
            _villageScene = GD.Load<PackedScene>(GameConstants.VILLAGE_PATH);
        }

        private void ConnectSignals()
        {
            _placeTowerButtonNode.Connect(Button.SignalName.Pressed, Callable.From(HandlePlacedTowerPressed));
            _placeVillageButtonNode.Connect(Button.SignalName.Pressed, Callable.From(HandlePlacedVillagePressed));
        }


        private void PlaceBuildingAtHoveredCellPosition()
        {
            if (!_hoveregGridCellPosition.HasValue) { return; }

            var building = _toPlaceBuildingScene.Instantiate<Node2D>();
            building.GlobalPosition = _hoveregGridCellPosition.Value * GameConstants.GRID_SIZE;
            _ySortRootNode.AddChild(building);

            _hoveregGridCellPosition = null;
            _gridManager.ClearHighLightedTiles();
        }

        private void HandlePlacedTowerPressed()
        {
            _toPlaceBuildingScene = _towerScene;
            _cursorNode.Visible = true;
            _gridManager.HighLightBuildableTiles();
        }

        private void HandlePlacedVillagePressed()
        {
            _toPlaceBuildingScene = _villageScene;
            _cursorNode.Visible = true;
            _gridManager.HighLightBuildableTiles();
        }
    }
}