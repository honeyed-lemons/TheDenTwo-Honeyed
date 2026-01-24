using Content.Server.DoAfter;
using Content.Shared._DEN.Recolor;
using Content.Shared.Interaction;
using JetBrains.Annotations;
using Robust.Server.GameObjects;

#pragma warning disable IDE1006 // Naming Styles
namespace Content.Server._DEN.Recolor;
#pragma warning restore IDE1006 // Naming Styles

public sealed partial class RecolorSystem : SharedRecolorSystem
{
    [Dependency] private readonly AppearanceSystem _appearance = default!;
    [Dependency] private readonly DoAfterSystem _doAfterSystem = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<RecolorApplierComponent, AfterInteractEvent>(OnRecolorApplierAfterInteract);
        SubscribeLocalEvent<RecolorApplierComponent, ApplyRecolorDoAfterEvent>(OnApplyRecolorDoAfterEvent);

        SubscribeLocalEvent<RecoloredComponent, ComponentStartup>(OnRecoloredStartup);
        SubscribeLocalEvent<RecoloredComponent, ComponentShutdown>(OnRecoloredShutdown);
    }

    private void OnRecoloredStartup(Entity<RecoloredComponent> ent, ref ComponentStartup args)
    {
        _appearance.SetData(ent, RecolorVisuals.RecolorDirty, true);
    }

    private void OnRecoloredShutdown(Entity<RecoloredComponent> ent, ref ComponentShutdown args)
    {
        _appearance.SetData(ent, RecolorVisuals.RecolorDirty, true);
    }

    [PublicAPI]
    public void Recolor(EntityUid uid,
        Color color,
        string? shader = null,
        bool affectLayersWithShaders = false,
        bool removeable = true)
    {
        if (HasComp<RecoloredComponent>(uid))
            return;

        EnsureComp<AppearanceComponent>(uid);
        var recoloredComponent = new RecoloredComponent
        {
            Color = color,
            Shader = shader,
            AffectLayersWithShaders = affectLayersWithShaders,
            Removeable = removeable
        };

        AddComp(uid, recoloredComponent);
    }

    [PublicAPI]
    public void RemoveRecolor(Entity<RecoloredComponent?> ent)
    {
        if (!Resolve(ent.Owner, ref ent.Comp, logMissing: false))
            return;

        RemComp<RecoloredComponent>(ent);
    }
}
