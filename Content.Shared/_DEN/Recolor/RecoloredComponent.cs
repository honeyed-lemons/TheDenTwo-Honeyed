using Robust.Shared.Prototypes;

namespace Content.Shared._DEN.Recolor;

[RegisterComponent]
public sealed partial class RecoloredComponent : Component
{
    /// <summary>
    /// The color to change the sprite to.
    /// </summary>
    [DataField]
    public Color Color = Color.White;

    /// <summary>
    /// Whether or not this component can be removed by an entity with RecolorRemoverComponent.
    /// </summary>
    [DataField]
    public bool Removeable = true;

    /// <summary>
    /// Whether or not the recolor should apply to layers that already have shaders.
    /// </summary>
    [DataField]
    public bool AffectLayersWithShaders = false;

    /// <summary>
    /// The shader to apply to the recolored entity.
    /// Sorry, we don't have ShaderPrototype in Shared, because ShaderPrototype is clientside.
    /// </summary>
    [DataField]
    public string? Shader = null;

    /// <summary>
    /// The previous color of the sprite, before recoloring.
    /// </summary>
    public Color? PreviousColor;
}
