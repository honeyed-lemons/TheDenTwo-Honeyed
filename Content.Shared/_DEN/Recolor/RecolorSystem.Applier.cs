using Content.Shared._DEN.Recolor.Components;
using Content.Shared.DoAfter;
using Content.Shared.Interaction;
using Content.Shared.Verbs;

namespace Content.Shared._DEN.Recolor;

public abstract partial class RecolorSystem
{
    private static void OnComponentStartup(Entity<RecolorApplierComponent> ent, ref ComponentStartup args)
    {
        if (ent.Comp.MaxUses != null)
            ent.Comp.UsesLeft = ent.Comp.MaxUses.Value;
    }

    private void OnRecolorApplierAfterInteract(Entity<RecolorApplierComponent> ent, ref AfterInteractEvent args)
    {
        if (!args.CanReach
            || args.Target == null
            || CanRecolor(ent, args.User, args.Target.Value))
            return;

        TryStartApplyRecolorDoAfter(args.User, args.Target.Value, ent);
    }

    private void TryStartApplyRecolorDoAfter(
        EntityUid user,
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

        _doAfter.TryStartDoAfter(doAfterArgs);
    }

    private void OnApplyRecolorDoAfterEvent(Entity<RecolorApplierComponent> ent, ref ApplyRecolorDoAfterEvent args)
    {
        if (args.Target is null || args.Handled || args.Cancelled)
            return;

        Recolor(
            uid: args.Target.Value,
            color: args.Color,
            removable: args.Removable,
            shader: args.Shader,
            shaderWhitelist: args.ShaderWhitelist,
            shaderBlacklist: args.ShaderBlacklist
        );

        PlayUseSound(ent, args.User);

        ent.Comp.UsesLeft--;

        args.Handled = true;
    }

    private bool CanRecolor(Entity<RecolorApplierComponent> applier, EntityUid user, EntityUid target)
    {
        if (_whitelist.IsWhitelistPass(applier.Comp.EntityWhitelist, target)
            || !_openable.IsClosed(applier, user)
            || applier.Comp is not { UsesLeft: <= 0, MaxUses: not null })
            return true;

        PopupNoMoreUses(applier,user);
        return false;
    }

    private void OnGetVerbs(Entity<RecolorApplierComponent> ent, ref GetVerbsEvent<UtilityVerb> args)
    {
        if (!args.CanAccess
            || !args.CanInteract
            || CanRecolor(ent, args.User, args.Target))
            return;

        var user = args.User;
        var target = args.Target;

        var verb = new UtilityVerb
        {
            Act = () =>  TryStartApplyRecolorDoAfter(user, target, ent),
            Text = Loc.GetString(ent.Comp.VerbText),
            Icon = ent.Comp.VerbIcon,
        };

        args.Verbs.Add(verb);
    }

    private void PopupNoMoreUses(Entity<RecolorApplierComponent> ent, EntityUid user)
    {
        _popup.PopupPredicted(Loc.GetString(ent.Comp.NoMoreUsesPopup, ("name", ent)),ent, user);
    }

    private void PlayUseSound(Entity<RecolorApplierComponent> ent, EntityUid user)
    {
        _audio.PlayPredicted(ent.Comp.DoafterSound, ent, user);
    }
}
