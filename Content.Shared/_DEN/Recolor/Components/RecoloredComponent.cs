using Robust.Shared.GameStates;

namespace Content.Shared._DEN.Recolor.Components;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class RecoloredComponent : Component
{
    /// <summary>
    /// RecolorData this component is storing.
    /// </summary>
    [DataField, AutoNetworkedField]
    public RecolorData RecolorData;

    /// <summary>
    /// Examine text as a locid.
    /// </summary>
    [DataField, AutoNetworkedField]
    public LocId ExamineText = "recolored-examine";
}
