using Content.Shared._DEN.Recolor;
using Content.Shared._DEN.Recolor.Components;
using Content.Shared.DoAfter;
using Content.Shared.Interaction;

namespace Content.Server._DEN.Recolor;

public sealed partial class RecolorSystem
{
    private void OnRecolorApplierAfterInteract(Entity<RecolorApplierComponent> ent, ref AfterInteractEvent args)
    {
        if (!args.CanReach || args.Target == null)
            return;

        TryStartApplyRecolorDoAfter(args.User, args.Target.Value, ent);
    }

    private void TryStartApplyRecolorDoAfter(EntityUid user,
        EntityUid target,
        Entity<RecolorApplierComponent> applier)
    {
        var doAfterEvent = new ApplyRecolorDoAfterEvent
        {
            Color = applier.Comp.Color,
            Shader = applier.Comp.Shader,
            ShaderBlacklist = applier.Comp.ShaderBlacklist,
            ShaderWhitelist = applier.Comp.ShaderWhitelist,
            Removable = applier.Comp.Removable,
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

        Recolor(
            uid: args.Target.Value,
            color: args.Color,
            removable: args.Removable,
            shader: args.Shader,
            shaderWhitelist: args.ShaderWhitelist,
            shaderBlacklist: args.ShaderBlacklist
            );
    }
}
