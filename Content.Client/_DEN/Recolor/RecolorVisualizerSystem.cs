using System.Linq;
using Content.Shared._DEN.Recolor.Components;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.Prototypes;

namespace Content.Client._DEN.Recolor;

public sealed class RecolorVisualizerSystem : VisualizerSystem<RecoloredComponent>
{
    [Dependency] private readonly IPrototypeManager _prototype = default!;
    protected override void OnAppearanceChange(EntityUid uid, RecoloredComponent component, ref AppearanceChangeEvent args)
    {
        base.OnAppearanceChange(uid, component, ref args);

        if (args.Sprite == null)
            return;

        ApplyRecolor((uid, component), args.Sprite);
    }

    private void ApplyRecolor(Entity<RecoloredComponent> ent, SpriteComponent sprite)
    {
        ShaderPrototype? shader = null;

        if (ent.Comp.Shader != null && _prototype.TryIndex<ShaderPrototype>(ent.Comp.Shader, out var proto))
            shader = proto;

        for (var i = 0; i < sprite.AllLayers.Count(); i++)
        {
            if (!SpriteSystem.TryGetLayer((ent, sprite), i, out var layer, false))
                continue;

            if (!ent.Comp.AffectLayersWithShaders && layer.ShaderPrototype != null)
                continue;

            if (shader != null)
            {
                var instance = shader.Instance();
                sprite.LayerSetShader(i, instance, ent.Comp.Shader);
            }

            SpriteSystem.LayerSetColor(layer, ent.Comp.Color);
        }
    }
}
