using System.Linq;
using Content.Client.Clothing;
using Content.Client.Items.Systems;
using Content.Shared._DEN.Recolor;
using Content.Shared._DEN.Recolor.Components;
using Content.Shared.Clothing;
using Content.Shared.Hands;
using Content.Shared.Item;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.Prototypes;

namespace Content.Client._DEN.Recolor;

public sealed class RecolorVisualizerSystem : VisualizerSystem<RecoloredComponent>
{
    [Dependency] private readonly ItemSystem _item = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<RecoloredComponent, ComponentShutdown>(OnComponentShutdown);

        SubscribeLocalEvent<RecoloredComponent, GetInhandVisualsEvent>(ApplyRecolorInHands,
            after: [typeof(ItemSystem)]);

        SubscribeLocalEvent<RecoloredComponent, GetEquipmentVisualsEvent>(ApplyRecolorEquipment,
            after: [typeof(ClientClothingSystem)]);
    }

    protected override void OnAppearanceChange(EntityUid uid, RecoloredComponent component, ref AppearanceChangeEvent args)
    {
        base.OnAppearanceChange(uid, component, ref args);

        if (args.Sprite == null)
            return;

        ApplyRecolorSprite((uid, component), args.Sprite);
        _item.VisualsChanged(uid);
    }

    private void OnComponentShutdown(Entity<RecoloredComponent> ent, ref ComponentShutdown args)
    {
        if (TerminatingOrDeleted(ent.Owner))
            return;

        if (!TryComp(ent, out SpriteComponent? sprite))
            return;

        RemoveRecolor(ent,sprite);
        _item.VisualsChanged(ent);
    }

    private void ApplyRecolorInHands(Entity<RecoloredComponent> ent, ref GetInhandVisualsEvent args)
    {
        ApplyRecolorLayers(ent,args.Layers);
    }

    private void ApplyRecolorEquipment(Entity<RecoloredComponent> ent, ref GetEquipmentVisualsEvent args)
    {
        ApplyRecolorLayers(ent,args.Layers);
    }

    private void ApplyRecolorLayers(Entity<RecoloredComponent> ent, List<(string, PrototypeLayerData)> layers)
    {
        if(!TryComp<AppearanceComponent>(ent, out var appearance))
            return;

        var appearanceData = GetRecolorAppearanceData((ent.Owner,appearance));

        foreach (var (_, layerData) in layers)
        {
            // Apply Color
            layerData.Color = appearanceData.Color;

            //Test shader whitelists and blacklists
            if (!AllowedShader(layerData.Shader, appearanceData))
                continue;

            // Apply shaders
            layerData.Shader = appearanceData.Shader;
        }
    }

    private void ApplyRecolorSprite(Entity<RecoloredComponent> ent, SpriteComponent sprite)
    {
        if(!TryComp<AppearanceComponent>(ent, out var appearance))
            return;

        var appearanceData = GetRecolorAppearanceData((ent.Owner,appearance));

        for (var i = 0; i < sprite.AllLayers.Count(); i++)
        {
            if (!SpriteSystem.TryGetLayer((ent, sprite), i, out var layer, false))
                continue;

            // Apply color
            SpriteSystem.LayerSetColor(layer, appearanceData.Color);

            var layerShader = layer.ShaderPrototype;

            if (!AllowedShader(layerShader?.Id, appearanceData))
                continue;

            // Apply shaders
            if (appearanceData.Shader != null)
                sprite.LayerSetShader(i, appearanceData.Shader);
        }
    }

    private void RemoveRecolor(Entity<RecoloredComponent> ent, SpriteComponent sprite)
    {
        if(!TryComp<AppearanceComponent>(ent, out var appearance))
            return;

        var appearanceData = GetRecolorAppearanceData((ent.Owner,appearance));

        for (var i = 0; i < sprite.AllLayers.Count(); i++)
        {
            // TODO: Make it possible to get the previous color and shaders, currently impossible due to sprite system being fully clientside

            if (!SpriteSystem.TryGetLayer((ent, sprite), i, out var layer, false))
                continue;

            // Remove colors
            SpriteSystem.LayerSetColor(layer, Color.White);

            // Remove shaders
            var layerShader = layer.ShaderPrototype;

            if (!AllowedShader(layerShader?.Id, appearanceData))
                continue;

            sprite.LayerSetShader(i, null, null);
        }
    }

    private record RecolorAppearanceData(Color Color, string? Shader,List<string>? ShaderBlacklist, List<string>? ShaderWhitelist);

    private RecolorAppearanceData GetRecolorAppearanceData(Entity<AppearanceComponent> ent)
    {
        AppearanceSystem.TryGetData(ent, RecolorVisuals.Color, out Color color);
        AppearanceSystem.TryGetData(ent, RecolorVisuals.Shader, out string? shader);
        AppearanceSystem.TryGetData(ent, RecolorVisuals.ShaderBlacklist, out List<string>? shaderBlacklist);
        AppearanceSystem.TryGetData(ent, RecolorVisuals.ShaderWhitelist, out List<string>? shaderWhitelist);

        return new RecolorAppearanceData(color, shader, shaderBlacklist, shaderWhitelist);
    }

    private static bool AllowedShader(string? shader, RecolorAppearanceData appearanceData)
    {
        if (shader == null)
            return true;

        return (appearanceData.ShaderBlacklist == null || !appearanceData.ShaderBlacklist.Contains(shader))
               && (appearanceData.ShaderWhitelist == null || appearanceData.ShaderWhitelist.Contains(shader));
    }
}
