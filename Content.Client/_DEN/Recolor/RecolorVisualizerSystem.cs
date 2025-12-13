using Content.Shared._DEN.Recolor;
using Content.Shared.Sound;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.Prototypes;

#pragma warning disable IDE1006 // Naming Styles
namespace Content.Client._DEN.Recolor;
#pragma warning restore IDE1006 // Naming Styles

public sealed partial class RecolorVisualizerSystem : VisualizerSystem<RecoloredComponent>
{
    [Dependency] private readonly IPrototypeManager _prototype = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<RecoloredComponent, ComponentShutdown>(OnComponentShutdown);
    }

    protected override void OnAppearanceChange(EntityUid uid,
        RecoloredComponent component,
        ref AppearanceChangeEvent args)
    {
        base.OnAppearanceChange(uid, component, ref args);

        if (args.Sprite == null
            || AppearanceSystem.TryGetData(uid, RecolorVisuals.RecolorDirty, out var dirty)
            || dirty is not true)
            return;

        ApplyRecolor((uid, component));
        AppearanceSystem.SetData(uid, RecolorVisuals.RecolorDirty, false);
    }

    private void OnComponentShutdown(Entity<RecoloredComponent> ent, ref ComponentShutdown args)
    {
        if (TerminatingOrDeleted(ent.Owner))
            return;

        RemoveRecolor(ent);
    }

    private void ApplyRecolor(Entity<RecoloredComponent> ent)
    {
        if (!TryComp<SpriteComponent>(ent.Owner, out var sprite))
            return;

        ShaderPrototype? shader = null;
        if (ent.Comp.Shader != null
            && _prototype.TryIndex<ShaderPrototype>(ent.Comp.Shader, out var proto))
            shader = proto;

        foreach (var spriteLayer in sprite.AllLayers)
        {
            if (spriteLayer is not SpriteComponent.Layer layer
                || !ent.Comp.AffectLayersWithShaders && layer.Shader != null)
                continue;

            SpriteSystem.LayerSetColor(layer, sprite.Color);

            if (shader != null)
            {
                var instance = shader.Instance();
                sprite.LayerSetShader(layer, instance);
            }
        }
    }

    private void RemoveRecolor(Entity<RecoloredComponent> ent)
    {
        if (!TryComp<SpriteComponent>(ent.Owner, out var sprite)
            || !AppearanceSystem.TryGetData(ent.Owner, RecolorVisuals.RecolorDirty, out _))
            return;

        ShaderPrototype? shader = null;
        if (ent.Comp.Shader != null
            && _prototype.TryIndex<ShaderPrototype>(ent.Comp.Shader, out var proto))
            shader = proto;

        foreach (var spriteLayer in sprite.AllLayers)
        {
            if (spriteLayer is not SpriteComponent.Layer layer
                || shader != null && layer.Shader != shader.Instance())
                continue;

            sprite.LayerSetShader(layer, "");

            if (layer.Color == ent.Comp.Color && ent.Comp.PreviousColor != null)
                SpriteSystem.LayerSetColor(layer, ent.Comp.PreviousColor.Value);
        }
    }
}
