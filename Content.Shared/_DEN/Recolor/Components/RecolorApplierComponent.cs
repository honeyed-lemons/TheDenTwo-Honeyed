using Content.Shared.Whitelist;
using Robust.Shared.Audio;
using Robust.Shared.GameStates;
using Robust.Shared.Utility;

namespace Content.Shared._DEN.Recolor.Components;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class RecolorApplierComponent : Component
{

    //Recolor Applier Specific Datafields

    /// <summary>
    /// How long it takes for this object to apply the recolor to the target.
    /// </summary>
    [DataField]
    public TimeSpan DoAfterDuration = TimeSpan.FromSeconds(2.0f);

    /// <summary>
    /// Maximum amount of uses the applier can spray, if left null the applier can apply infinitely.
    /// </summary>
    [DataField, AutoNetworkedField]
    public int? MaxUses;

    /// <summary>
    /// Current amount of uses the applier can spray.
    /// </summary>
    [DataField, AutoNetworkedField]
    public int UsesLeft;

    /// <summary>
    /// LocId used for the "you're outta paint" popup.
    /// </summary>
    [DataField, AutoNetworkedField]
    public LocId NoMoreUsesPopup = "spray-paint-empty";

    /// <summary>
    /// Sound to play when the doafter is over.
    /// </summary>
    [DataField, AutoNetworkedField]
    public SoundSpecifier? DoafterSound = new SoundPathSpecifier("/Audio/Effects/Spray2.ogg");

    /// <summary>
    /// Entity Whitelist to determine what items can be repainted.
    /// </summary>
    [DataField, AutoNetworkedField]
    public EntityWhitelist? EntityWhitelist;

    /// <summary>
    /// LocId used for the apply recolor verb.
    /// </summary>
    [DataField, AutoNetworkedField]
    public LocId VerbText = "verb-spray-paint";

    /// <summary>
    /// Icon used for the apply recolor verb.
    /// </summary>
    [DataField, AutoNetworkedField]
    public SpriteSpecifier VerbIcon = new SpriteSpecifier.Texture(new ResPath("/Textures/_DEN/Interface/VerbIcons/paint-spray-can.svg.192dpi.png"));

    // Recolor Datafields

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
    /// Don't apply shader to layers with these shaders. (Sorry about the lack of shader prototype)
    /// </summary>
    [DataField]
    public List<string>? ShaderBlacklist;

    /// <summary>
    /// Only apply shader to layers with these shaders.
    /// </summary>
    [DataField]
    public List<string>? ShaderWhitelist;

    /// <summary>
    /// The shader to apply to the recolored entity.
    /// Sorry, we don't have ShaderPrototype in Shared, because ShaderPrototype is clientside.
    /// </summary>
    [DataField]
    public string? Shader = "Desaturated";
}
