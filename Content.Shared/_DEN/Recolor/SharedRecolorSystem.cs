using Content.Shared.DoAfter;
using Robust.Shared.Serialization;

#pragma warning disable IDE1006 // Naming Styles
namespace Content.Shared._DEN.Recolor;
#pragma warning restore IDE1006 // Naming Styles

public abstract partial class SharedRecolorSystem : EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();
    }
}

[Serializable, NetSerializable]
public enum RecolorVisuals : byte
{
    RecolorDirty
}

[Serializable, NetSerializable]
public sealed partial class ApplyRecolorDoAfterEvent : DoAfterEvent
{
    public Color Color;
    public string? Shader = null;
    public bool AffectLayersWithShaders = false;
    public bool Removeable = true;

    public override DoAfterEvent Clone() => this;
}
