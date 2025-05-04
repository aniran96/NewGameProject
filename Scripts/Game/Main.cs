using GoblinGridPuzzle.Buildings;
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
        private BuildingResource _towerResource;
        private BuildingResource _villageResource;
        private BuildingResource _toPlaceBuildingResource;

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
            if (_toPlaceBuildingResource != null &&
            _cursorNode.Visible &&
            (
                !_hoveregGridCellPosition.HasValue ||
                _hoveregGridCellPosition != gridPosition
                )
                )
            {
                _hoveregGridCellPosition = gridPosition;
                _gridManager.HighlightExpandedBuildableTiles(_hoveregGridCellPosition.Value, _toPlaceBuildingResource.BuildableRadius);
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

            _towerResource = GD.Load<BuildingResource>(GameConstants.TOWER_RESOURCE_PATH);
            _villageResource = GD.Load<BuildingResource>(GameConstants.VILLAGE_RESOURCE_PATH);
        }

        private void ConnectSignals()
        {
            _placeTowerButtonNode.Connect(Button.SignalName.Pressed, Callable.From(HandlePlacedTowerPressed));
            _placeVillageButtonNode.Connect(Button.SignalName.Pressed, Callable.From(HandlePlacedVillagePressed));
        }


        private void PlaceBuildingAtHoveredCellPosition()
        {
            if (!_hoveregGridCellPosition.HasValue) { return; }

            var building = _toPlaceBuildingResource.BuildingScene.Instantiate<Node2D>();
            building.GlobalPosition = _hoveregGridCellPosition.Value * GameConstants.GRID_SIZE;
            _ySortRootNode.AddChild(building);

            _hoveregGridCellPosition = null;
            _gridManager.ClearHighLightedTiles();
        }

        private void HandlePlacedTowerPressed()
        {
            _toPlaceBuildingResource = _towerResource;
            _cursorNode.Visible = true;
            _gridManager.HighLightBuildableTiles();
        }

        private void HandlePlacedVillagePressed()
        {
            _toPlaceBuildingResource = _villageResource;
            _cursorNode.Visible = true;
            _gridManager.HighLightBuildableTiles();
        }
    }
}