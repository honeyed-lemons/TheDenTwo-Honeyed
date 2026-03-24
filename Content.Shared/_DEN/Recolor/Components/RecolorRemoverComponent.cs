using Robust.Shared.Audio;
using Robust.Shared.GameStates;

namespace Content.Shared._DEN.Recolor.Components;

[RegisterComponent, NetworkedComponent]
public sealed partial class RecolorRemoverComponent : Component
{
    /// <summary>
    /// How long it takes for this object to remove the recolor on the target.
    /// </summary>
    [DataField]
    public TimeSpan DoAfterDuration = TimeSpan.FromSeconds(2.0f);

    /// <summary>
    /// Sound to play when the doafter is over.
    /// </summary>
    [DataField]
    public SoundSpecifier? DoafterSound;

}
