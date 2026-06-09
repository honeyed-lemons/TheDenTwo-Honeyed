using Content.Shared.FixedPoint;
using Robust.Shared.GameStates;

namespace Content.Shared._DEN.SolutionExpulsion.Components;

[RegisterComponent, NetworkedComponent]
public sealed partial class SolutionExpellableComponent : Component
{
    /// <summary>
    /// Maximum amount of the solution you can expel at once
    /// </summary>
    [DataField]
    public FixedPoint2 MaximumExpulsion = 10;

    [DataField]
    public TimeSpan DoAfterDuration = TimeSpan.FromSeconds(3);

    /// <summary>
    /// Solution name to use, should be kept seperate from other expellable solutions unless you specifically want them mixing.
    /// </summary>
    [DataField]
    public string DefaultSolutionName = "testicles";

    /// <summary>
    /// Determines if the verb icon is NSFW or not.. I'd love to specify an actual texture here but YOU CANT SPECIFY SPECIFIC TEXTURES IN YAML !!!!!!!!!!
    /// </summary>
    [DataField]
    public bool NsfwVerbIcon;

    /// <summary>
    /// Text to display on the verb.
    /// </summary>
    [DataField]
    public string VerbText = "cum-verb-text";

    /// <summary>
    /// Popup that occurs when your solution is empty
    /// </summary>
    [DataField]
    public string PopupEmpty = "cum-verb-dry";

    /// <summary>
    /// Popup that occurs when successfully expel the solution.
    /// </summary>
    [DataField]
    public string PopupSuccess = "cum-verb-success";
}
