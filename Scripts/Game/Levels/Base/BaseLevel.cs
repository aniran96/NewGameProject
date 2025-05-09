using Godot;
using GoblinGridPuzzle.Managers.Grid;
using GoblinGridPuzzle.Utilities.Constants;
using GoblinGridPuzzle.Mines.GOldenMines;

namespace GoblinGridPuzzle.Game.Levels.Base;

public partial class BaseLevel : Node
{
    //node references
    private GridManager _gridManagerNode;
    private GoldMine _goldMineNode;

    public override void _Ready()
    {
        InitalizeVariables();
        ConnectSignals();
    }

    private void InitalizeVariables()
    {
        _gridManagerNode = GetNode<GridManager>(GameConstants.GRIDMANAGER_PATH);
        _goldMineNode = GetNode<GoldMine>(GameConstants.GOLD_MINE_PATH);
    }

    private void ConnectSignals()
    {
        _gridManagerNode.GridStateUpdated += HandleGridStateUpdated;
    }

    private void HandleGridStateUpdated()
    {
        var goldMineTilePosition = _gridManagerNode.ConvertWorldPositionToTilePosition(_goldMineNode.GlobalPosition);
        if (_gridManagerNode.IsTilePositionBuildable(goldMineTilePosition))
        {
            _goldMineNode.SetActiveTexture();
            GD.Print("Win!");
        }
    }
}
