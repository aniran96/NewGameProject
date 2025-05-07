using GoblinGridPuzzle.Managers.Grid;
using GoblinGridPuzzle.Resources.Buildings;
using GoblinGridPuzzle.UI;
using GoblinGridPuzzle.Utilities.Constants;
using Godot;

namespace GoblinGridPuzzle.Managers.Buildings;

public partial class BuildingManager : Node
{


    //class references
    private GridManager _gridManager;
    //exported node references
    [ExportGroup("required nodes")]
    [Export]
    private GridManager _gridManagerNode;
    [Export]
    private GameUI _gameUINode;
    [Export]
    private Node2D _ySortRootNode;
    [Export]
    private Sprite2D _cursorNode;

    // resource references
    private BuildingResource _toPlaceBuildingResource;


    //variables
    private int _currentResourceCount;
    private int _startingResourcecount = 4;

    private Vector2I? _hoveregGridCellPosition;

    public override void _Ready()
    {
        InitializeVariables();
        ConnectSignals();
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
            _gridManager.ClearHighLightedTiles();
            _gridManager.HighlightExpandedBuildableTiles(_hoveregGridCellPosition.Value, _toPlaceBuildingResource.BuildableRadius);
            _gridManager.HighlightResourceTiles(_hoveregGridCellPosition.Value, _toPlaceBuildingResource.ResourceRadius);
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

    private void InitializeVariables()
    {
        _currentResourceCount = _startingResourcecount;
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

        _hoveregGridCellPosition = null;
        _gridManager.ClearHighLightedTiles();
    }


    private void HandleResourceTilesUpdated(int resourceCount)
    {
        _currentResourceCount = resourceCount;
    }

    private void HandleBuildingResourceSelected(BuildingResource buildingResource)
    {
        _toPlaceBuildingResource = buildingResource;
        _cursorNode.Visible = true;
        _gridManager.HighLightBuildableTiles();
    }
}
