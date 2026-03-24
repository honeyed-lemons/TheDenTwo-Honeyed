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
    /// Type of paint used, purely for flavor.
    /// </summary>
    [DataField, AutoNetworkedField]
    public string? PaintType;

    /// <summary>
    /// Whether or not this component can be removed by an entity with RecolorRemoverComponent.
    /// </summary>
    [DataField, AutoNetworkedField]
    public bool Removable { get; set; }

    /// <summary>
    /// Don't apply to layers with these shaders. (Sorry about the lack of shader prototype)
    /// </summary>
    [DataField, AutoNetworkedField]
    public List<string>? ShaderBlacklist;

    /// <summary>
    /// Only apply to layers with these shaders.
    /// </summary>
    [DataField, AutoNetworkedField]
    public List<string>? ShaderWhitelist;

    /// <summary>
    /// The shader to apply to the recolored entity.
    /// Sorry, we don't have ShaderPrototype in Shared, because ShaderPrototype is clientside.
    /// </summary>
    [DataField, AutoNetworkedField]
    public string? Shader { get; set; }
}
