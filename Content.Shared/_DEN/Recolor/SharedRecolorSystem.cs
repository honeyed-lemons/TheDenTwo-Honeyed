using Content.Shared.DoAfter;
using Robust.Shared.Serialization;

namespace Content.Shared._DEN.Recolor;

public abstract class SharedRecolorSystem : EntitySystem;

[Serializable, NetSerializable]
public sealed partial class ApplyRecolorDoAfterEvent : DoAfterEvent
{
    public Color Color;
    public bool Removable;
    public List<string>? ShaderBlacklist;
    public List<string>? ShaderWhitelist;
    public string? Shader;
    public override DoAfterEvent Clone()
    {
        return this;
    }
}

[Serializable, NetSerializable]
public enum RecolorVisuals
{
    Color,
    Shader,
    ShaderWhitelist,
    ShaderBlacklist,
}
