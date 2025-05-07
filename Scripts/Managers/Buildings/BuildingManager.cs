using GoblinGridPuzzle.Managers.Grid;
using GoblinGridPuzzle.Resources.Buildings;
using GoblinGridPuzzle.UI;
using GoblinGridPuzzle.Utilities.Constants;
using Godot;

namespace GoblinGridPuzzle.Managers.Buildings;

public partial class BuildingManager : Node
{
    // exported Scene References
    [Export]
    private PackedScene _buildingGhostScene;

    // resource references
    private BuildingResource _toPlaceBuildingResource;

    //exported node references
    [ExportGroup("required nodes")]
    [Export]
    private GridManager _gridManagerNode;
    [Export]
    private GameUI _gameUINode;
    [Export]
    private Node2D _ySortRootNode;

    //node references
    private Node2D _buildingGhost;



    //variables
    private int _currentResourceCount;
    private int _startingResourcecount = 4;
    private int _currentlyUsedResourceCount; // used in building
    private int AvailableResourceCount => (_startingResourcecount + _currentResourceCount) - _currentlyUsedResourceCount; // used to calculate remaining resources

    private Vector2I? _hoveregGridCellPosition;

    public override void _Ready()
    {
        ConnectSignals();
    }

    public override void _Process(double delta)
    {
        if (!IsInstanceValid(_buildingGhost)) { return; }
        Vector2I gridPosition = _gridManagerNode.GetMouseGridCellPosition();
        _buildingGhost.GlobalPosition = gridPosition * GameConstants.GRID_SIZE;
        if (_toPlaceBuildingResource != null &&
        (
            !_hoveregGridCellPosition.HasValue ||
            _hoveregGridCellPosition != gridPosition
            )
            )
        {
            _hoveregGridCellPosition = gridPosition;
            _gridManagerNode.ClearHighLightedTiles();
            _gridManagerNode.HighLightBuildableTiles();
            if (IsTilePositionBuildable(_hoveregGridCellPosition.Value))
            {
                _gridManagerNode.HighlightExpandedBuildableTiles
                (_hoveregGridCellPosition.Value, _toPlaceBuildingResource.BuildableRadius);
                _gridManagerNode.HighlightResourceTiles
                (_hoveregGridCellPosition.Value, _toPlaceBuildingResource.ResourceRadius);
            }
        }
    }

    public override void _UnhandledInput(InputEvent evt)
    {
        if (
            _hoveregGridCellPosition.HasValue &&
            _toPlaceBuildingResource != null &&
            evt.IsActionPressed(GameConstants.LEFT_CLICK) &&
            IsTilePositionBuildable(_hoveregGridCellPosition.Value)
        )
        {

            PlaceBuildingAtHoveredCellPosition();
        }
    }

    private void ConnectSignals()
    {
        _gridManagerNode.ResourceTilesUpdated += HandleResourceTilesUpdated;
        _gameUINode.BuildingResourceSelected += HandleBuildingResourceSelected;
    }

    private void PlaceBuildingAtHoveredCellPosition()
    {
        if (!_hoveregGridCellPosition.HasValue) { return; }

        var building = _toPlaceBuildingResource.BuildingScene.Instantiate<Node2D>();
        building.GlobalPosition = _hoveregGridCellPosition.Value * GameConstants.GRID_SIZE;
        _ySortRootNode.AddChild(building);
        _currentlyUsedResourceCount += _toPlaceBuildingResource.ResourceCost;

        _hoveregGridCellPosition = null;
        _gridManagerNode.ClearHighLightedTiles();
        _buildingGhost.QueueFree();
        _buildingGhost = null;
    }

    private bool IsTilePositionBuildable(Vector2I tilePosition)
    {
        return _gridManagerNode.IsTilePositionBuildable(tilePosition) &&
            AvailableResourceCount >= _toPlaceBuildingResource.ResourceCost;
    }


    private void HandleResourceTilesUpdated(int resourceCount)
    {
        _currentResourceCount = resourceCount;
    }

    private void HandleBuildingResourceSelected(BuildingResource buildingResource)
    {
        if (IsInstanceValid(_buildingGhost))
        {
            _buildingGhost.QueueFree();
        }
        _buildingGhost = _buildingGhostScene.Instantiate<Node2D>();
        _ySortRootNode.AddChild(_buildingGhost);
        var buildingSprite = buildingResource.SpriteScene.Instantiate<Sprite2D>();
        _buildingGhost.AddChild(buildingSprite);
        _toPlaceBuildingResource = buildingResource;
        _gridManagerNode.HighLightBuildableTiles();
    }
}
