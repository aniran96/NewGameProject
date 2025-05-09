using Godot;

namespace GoblinGridPuzzle.Mines.GOldenMines;

public partial class GoldMine : Node2D
{
    //exported node references
    [ExportGroup("GoldMineTextureSetup")]
    [Export]
    private Sprite2D _goldMineSprite;
    [Export]
    private Texture2D _activeTexture;

    public void SetActiveTexture()
    {
        _goldMineSprite.Texture = _activeTexture;
    }

}
