using Content.Shared._DEN.Recolor;
using Content.Shared.DoAfter;
using Content.Shared.Interaction;

#pragma warning disable IDE1006 // Naming Styles
namespace Content.Server._DEN.Recolor;
#pragma warning restore IDE1006 // Naming Styles

public sealed partial class RecolorSystem : SharedRecolorSystem
{
    private void OnRecolorApplierAfterInteract(Entity<RecolorApplierComponent> ent, ref AfterInteractEvent args)
    {
        if (!args.CanReach || args.Target is not { Valid: true } target || HasComp<RecoloredComponent>(target))
            return;

        TryStartApplyRecolorDoAfter(args.User, target, ent);
    }

    private void TryStartApplyRecolorDoAfter(EntityUid user,
        EntityUid target,
        Entity<RecolorApplierComponent> applier)
    {
        var doAfterEvent = new ApplyRecolorDoAfterEvent
        {
            Color = applier.Comp.Color,
            Shader = applier.Comp.Shader,
            AffectLayersWithShaders = applier.Comp.AffectLayersWithShaders,
            Removeable = applier.Comp.Removeable
        };

        var doAfterArgs = new DoAfterArgs(EntityManager,
            user: user,
            seconds: (float)applier.Comp.DoAfterDuration.TotalSeconds,
            @event: doAfterEvent,
            eventTarget: applier,
            target: target,
            used: applier);

        _doAfterSystem.TryStartDoAfter(doAfterArgs);
    }

    private void OnApplyRecolorDoAfterEvent(Entity<RecolorApplierComponent> ent, ref ApplyRecolorDoAfterEvent args)
    {
        if (args.Target is null)
            return;

        Recolor(uid: args.Target.Value,
            color: args.Color,
            shader: args.Shader,
            affectLayersWithShaders: args.AffectLayersWithShaders,
            removeable: args.Removeable);
    }
}
