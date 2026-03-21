using Robust.Shared.GameStates;

namespace Content.Shared._DEN.Recolor.Components;

[RegisterComponent]
public sealed partial class RecolorApplierComponent : Component
{
    /// <summary>
    /// The color to apply to the object being recolored.
    /// </summary>
    [DataField]
    public Color Color { get; set; }

    /// <summary>
    /// Whether or not the color applied can be removed via normal means.
    /// </summary>
    public bool Removable { get; set; }

    /// <summary>
    /// Whether or not the recolor should apply to layers that already have shaders.
    /// </summary>
    [DataField]
    public bool AffectLayersWithShaders;

    /// <summary>
    /// The shader to apply to the recolored entity.
    /// Sorry, we don't have ShaderPrototype in Shared, because ShaderPrototype is clientside.
    /// </summary>
    [DataField]
    public string? Shader = "Desaturated";

    /// <summary>
    /// How long it takes for this object to apply the recolor to the target.
    /// </summary>
    [DataField]
    public TimeSpan DoAfterDuration = TimeSpan.FromSeconds(3.0f);
}
