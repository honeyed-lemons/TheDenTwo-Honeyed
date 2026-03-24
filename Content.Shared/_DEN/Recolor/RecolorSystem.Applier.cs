using Content.Shared._DEN.Recolor.Components;
using Content.Shared.DoAfter;
using Content.Shared.Examine;
using Content.Shared.Interaction;
using Content.Shared.Verbs;
using Robust.Shared.ColorNaming;

namespace Content.Shared._DEN.Recolor;

public sealed partial class RecolorSystem
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
            || args.Handled
            || !CanRecolor(ent, args.User, args.Target.Value))
        {
            return;
        }

        args.Handled = TryStartApplyRecolorDoAfter(args.User, args.Target.Value, ent);
    }

    private bool TryStartApplyRecolorDoAfter(
        EntityUid user,
        EntityUid target,
        Entity<RecolorApplierComponent> applier)
    {
        var doAfterEvent = new ApplyRecolorDoAfterEvent
        {
            Color = applier.Comp.Color,
            PaintType = applier.Comp.PaintType,
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
            used: applier)
        {
            BreakOnDropItem = true,
            BreakOnMove = true,
            BreakOnHandChange = true,
        };

        return _doAfter.TryStartDoAfter(doAfterArgs);
    }

    private void OnApplyRecolorDoAfterEvent(Entity<RecolorApplierComponent> ent, ref ApplyRecolorDoAfterEvent args)
    {
        if (args.Target is null || args.Handled || args.Cancelled)
            return;

        Recolor(
            uid: args.Target.Value,
            color: args.Color,
            removable: args.Removable,
            paintType: args.PaintType,
            shader: args.Shader,
            shaderWhitelist: args.ShaderWhitelist,
            shaderBlacklist: args.ShaderBlacklist
        );

        _audio.PlayPredicted(ent.Comp.DoafterSound, ent, args.User);

        ent.Comp.UsesLeft -= 1;

        Dirty(ent);

        args.Handled = true;
    }

    private bool CanRecolor(Entity<RecolorApplierComponent> applier, EntityUid user, EntityUid target)
    {
        // Check if the applier is opened
        if (_openable.IsClosed(applier, user, predicted: true))
            return false;

        // Check whitelist and blacklist
        if (!_whitelist.CheckBoth(target, applier.Comp.EntityBlacklist, applier.Comp.EntityWhitelist))
        {
            _popup.PopupClient(Loc.GetString(applier.Comp.CantRecolorPopup, ("target", target)),applier, user);
            return false;
        }

        // Check if there's enough uses left
        if (applier.Comp is { UsesLeft: <= 0, MaxUses: not null })
        {
            _popup.PopupClient(Loc.GetString(applier.Comp.NoMoreUsesPopup, ("name", applier)),applier, user);
            return false;
        }

        return true;
    }

    private void OnGetApplierVerbs(Entity<RecolorApplierComponent> ent, ref GetVerbsEvent<UtilityVerb> args)
    {
        if (!args.CanAccess
            || !args.CanInteract
            || !CanRecolor(ent, args.User, args.Target))
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

    private void OnExamined(Entity<RecolorApplierComponent> ent, ref ExaminedEvent args)
    {
        if (!args.IsInDetailsRange)
            return;
        // Get the color's name. If the color itself has a color defined, use it
        var colorName = ent.Comp.ColorName ??
                        // Otherwise, use what colornaming THINKS it is.
                        ColorNaming.Describe(ent.Comp.Color, _localizationManager);

        args.PushMarkup(Loc.GetString(ent.Comp.ColorShowcaseExamine, ("color", ent.Comp.Color), ("colorName", colorName)));

        // If max uses isn't null (signifying this item has infinite uses), show uses count
        if (ent.Comp.MaxUses != null)
            args.PushMarkup(Loc.GetString(ent.Comp.UsesExamine, ("uses", ent.Comp.UsesLeft)));
    }
}
