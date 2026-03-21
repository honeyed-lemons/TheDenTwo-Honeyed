using Content.Server.DoAfter;
using Content.Shared._DEN.Recolor;
using Content.Shared._DEN.Recolor.Components;
using Content.Shared.Interaction;
using JetBrains.Annotations;
using Robust.Server.GameObjects;

namespace Content.Server._DEN.Recolor;

public sealed partial class RecolorSystem : SharedRecolorSystem
{
    [Dependency] private readonly AppearanceSystem _appearance = default!;
    [Dependency] private readonly DoAfterSystem _doAfterSystem = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<RecoloredComponent, ComponentStartup>(OnComponentStartup);
        SubscribeLocalEvent<RecoloredComponent, ComponentShutdown>(OnComponentShutdown);

        SubscribeLocalEvent<RecolorApplierComponent, ApplyRecolorDoAfterEvent>(OnApplyRecolorDoAfterEvent);
        SubscribeLocalEvent<RecolorApplierComponent, AfterInteractEvent>(OnRecolorApplierAfterInteract);
    }

    private void OnComponentStartup(Entity<RecoloredComponent> ent, ref ComponentStartup args)
    {
        DirtyVisuals(ent);
    }

    private void OnComponentShutdown(Entity<RecoloredComponent> ent, ref ComponentShutdown args)
    {
        DirtyVisuals(ent);
    }

    private void DirtyVisuals(Entity<RecoloredComponent> ent)
    {
        if (!TryComp(ent, out AppearanceComponent? appearance))
            return;

        _appearance.SetData(ent, RecolorVisuals.Color, true, appearance);
    }

    [PublicAPI]
    public void Recolor(EntityUid uid,
        Color color,
        bool affectLayersWithShaders,
        bool removable,
        string? shader = null)
    {
        if (HasComp<RecoloredComponent>(uid))
        {
            //Replace old recolored component. you can spray things with paint twice.. right?
            RemComp<RecoloredComponent>(uid);
        }

        EnsureComp<AppearanceComponent>(uid);

        var comp = new RecoloredComponent
        {
            Color = color,
            Shader = shader,
            AffectLayersWithShaders = affectLayersWithShaders,
            Removable = removable,
        };

        AddComp(uid, comp);
    }

    [PublicAPI]
    public void RemoveRecolor(Entity<RecoloredComponent?> ent)
    {
        if (!Resolve(ent.Owner, ref ent.Comp, logMissing: false))
            return;

        RemComp(ent, ent.Comp);
    }
}
