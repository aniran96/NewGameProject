using GoblinGridPuzzle.Resources.Buildings;
using GoblinGridPuzzle.Utilities.Constants;
using Godot;

namespace GoblinGridPuzzle.UI;

public partial class GameUI : MarginContainer
{
    // signals
    [Signal]
    public delegate void BuildingResourceSelectedEventHandler(BuildingResource buildingResource);

    //exported resource references
    [Export]
    private BuildingResource[] _buildingResources;

    //node references
    private HBoxContainer _buttonHBoxNode;

    public override void _Ready()
    {
        InitalizeVariables();
    }

    private void CreateBuildingButtons()
    {
        foreach (var buildingResource in _buildingResources)
        {
            var buildingButton = new Button();
            buildingButton.Text = $"Place {buildingResource.DisplayName}";
            _buttonHBoxNode.AddChild(buildingButton);

            buildingButton.Pressed += () =>
            {
                EmitSignal(SignalName.BuildingResourceSelected, buildingResource);
            };
        }
    }

    private void InitalizeVariables()
    {
        _buttonHBoxNode = GetNode<HBoxContainer>(GameConstants.BUTTON_HBOX_PATH);
        CreateBuildingButtons();
    }
}
