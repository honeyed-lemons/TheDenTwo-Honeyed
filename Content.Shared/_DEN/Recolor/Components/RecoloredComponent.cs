using Robust.Shared.GameStates;

namespace Content.Shared._DEN.Recolor.Components;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class RecoloredComponent : Component
{
    /// <summary>
    /// The color to change the sprite to.
    /// </summary>
    [DataField, AutoNetworkedField]
    public Color Color = Color.White;

    /// <summary>
    /// Whether or not this component can be removed by an entity with RecolorRemoverComponent.
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
    [DataField, AutoNetworkedField]
    public string? Shader { get; set; }

}
