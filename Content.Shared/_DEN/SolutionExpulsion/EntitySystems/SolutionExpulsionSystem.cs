using Content.Shared._DEN.SolutionExpulsion.Components;
using Content.Shared._DEN.SolutionExpulsion.Events;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.DoAfter;
using Content.Shared.FixedPoint;
using Content.Shared.IdentityManagement;
using Content.Shared.Popups;
using Content.Shared.Verbs;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Shared._DEN.SolutionExpulsion.EntitySystems;

public sealed partial class SolutionExpulsionSystem : EntitySystem
{
    [Dependency] private IPrototypeManager _protoMan = default!;
    [Dependency] private SharedDoAfterSystem _doAfter = default!;
    [Dependency] private SharedPopupSystem _popup = default!;
    [Dependency] private SharedSolutionContainerSystem _solutionContainer = default!;

    private static readonly VerbCategory ReagentFillCategory = new("verb-categories-fill", "/Textures/Interface/VerbIcons/spill.svg.192dpi.png");

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<RefillableSolutionComponent, GetVerbsEvent<InteractionVerb>>(AddVerbs);

        SubscribeLocalEvent<SolutionExpellerComponent, SolutionExpulsionEvents.SolutionExpulsionFillEvent>(FinishFillDoAfter);

        SubscribeLocalEvent<SolutionExpellerComponent, SolutionExpulsionEvents.SolutionExpellableAdded>(SolutionExpellerAdded);
        SubscribeLocalEvent<SolutionExpellerComponent, SolutionExpulsionEvents.SolutionExpellableRemoved>(SolutionExpulsionRemoved);
    }

    /// <summary>
    /// Add an expellable solution to an entity.
    /// </summary>
    /// <param name="entity">The entity to add the expellable solution to.</param>
    /// <param name="expellableSolutionPrototype">The entity prototype of the solution you want the entity to expel.</param>
    public void AddExpellableSolution(EntityUid entity, EntProtoId expellableSolutionPrototype)
    {
        EnsureComp<SolutionExpellerComponent>(entity);

        RaiseLocalEvent(entity, new SolutionExpulsionEvents.SolutionExpellableAdded(expellableSolutionPrototype));
    }

    /// <summary>
    /// Remove an expellable solution from an entity, if there are no more solutions remaining to expel,
    /// the SolutionExpellerComponent on the entity is deleted.
    /// </summary>
    /// <param name="entity">The entity to remove the expellable solution from.</param>
    /// <param name="expellableSolutionPrototype">The entity prototype of the solution you don't want the entity to expel.</param>
    public void RemoveExpellableSolution(EntityUid entity, EntProtoId expellableSolutionPrototype)
    {
        EnsureComp<SolutionExpellerComponent>(entity);

        RaiseLocalEvent(entity, new SolutionExpulsionEvents.SolutionExpellableRemoved(expellableSolutionPrototype));
    }

    private void SolutionExpellerAdded(Entity<SolutionExpellerComponent> ent, ref SolutionExpulsionEvents.SolutionExpellableAdded args)
    {
        if (!_protoMan.TryIndex(args.ExpellableSolutionPrototype, out _))
            return;

        var expellableSolutionEntity = PredictedSpawnAttachedTo(args.ExpellableSolutionPrototype,Transform(ent).Coordinates);

        if (!TryComp<SolutionExpellableComponent>(expellableSolutionEntity, out _))
            return;

        // Add the expellable solution entity to the expeller so they can be iterated over for verbs.
        ent.Comp.SolutionEntities.Add(args.ExpellableSolutionPrototype,expellableSolutionEntity);
        Dirty(ent);
    }

    private void SolutionExpulsionRemoved(Entity<SolutionExpellerComponent> ent, ref SolutionExpulsionEvents.SolutionExpellableRemoved args)
    {
        if (!_protoMan.TryIndex(args.ExpellableSolutionPrototype, out _))
            return;

        // Remove the entity from the expellers list and get it so we can remove the real solution too!
        if (!ent.Comp.SolutionEntities.Remove(args.ExpellableSolutionPrototype, out var expellableSolutionEntity))
            return;

        // Delete the solution entity entirely, we don't need it anymore!
        PredictedDel(expellableSolutionEntity);

        // If there are no more solutions to worry about, kill the component
        if (ent.Comp.SolutionEntities.Count == 0)
            RemCompDeferred<SolutionExpellerComponent>(ent.Owner);

        Dirty(ent);
    }

    private void AddVerbs(Entity<RefillableSolutionComponent> container, ref GetVerbsEvent<InteractionVerb> args)
    {
        var user = args.User;

        if (!TryComp<SolutionExpellerComponent>(user, out var expellerComponent))
            return;

        foreach (var (_, expellableSolutionEntity) in expellerComponent.SolutionEntities)
        {
            if (!TryComp<SolutionExpellableComponent>(expellableSolutionEntity, out var solutionExpellableComponent))
                return;

            var icon = solutionExpellableComponent.NsfwVerbIcon
                ? new SpriteSpecifier.Texture(new ResPath("/Textures/_DEN/Interface/VerbIcons/lewd.svg.192dpi.png"))
                : null;

            var verb = new InteractionVerb
            {
                Category = ReagentFillCategory,
                Act = () => StartFillDoAfter((user, expellerComponent ), container, (expellableSolutionEntity, solutionExpellableComponent)),
                Text = Loc.GetString(solutionExpellableComponent.VerbText),
                Priority = -1,
                CloseMenu = false,
                Icon = icon,
            };

            args.Verbs.Add(verb);
        }
    }

    private void StartFillDoAfter(
        Entity<SolutionExpellerComponent> user,
        Entity<RefillableSolutionComponent> target,
        Entity<SolutionExpellableComponent> solution
    )
    {
        _doAfter.TryStartDoAfter(
            new DoAfterArgs(EntityManager,
                user,
                solution.Comp.DoAfterDuration,
                new SolutionExpulsionEvents.SolutionExpulsionFillEvent(GetNetEntity(solution)),
                user,
                target: target)
            {
                BreakOnMove = true,
                BreakOnDropItem = true,
            });
    }

    private void FinishFillDoAfter(Entity<SolutionExpellerComponent> ent, ref SolutionExpulsionEvents.SolutionExpulsionFillEvent args)
    {
        if (args.Target == null || args.Cancelled || args.Handled)
            return;

        if (!TryComp<SolutionExpellableComponent>(GetEntity(args.ExpellableSolution), out var expellableComponent))
            return;

        // Get the solution we're expelling
        if (!TryComp<SolutionComponent>(GetEntity(args.ExpellableSolution), out var expellableSolutionComponent))
            return;

        var expellableSolution = expellableSolutionComponent.Solution;

        // Get the solution of the container
        if (!_solutionContainer.TryGetRefillableSolution(args.Target.Value,
                out var targetSolutionComponent,
                out var targetSolution))
            return;


        // If there's no cum to cum you cant cum, okay?
        if (expellableSolution.Volume <= 0)
        {
            _popup.PopupPredicted(Loc.GetString(expellableComponent.PopupEmpty),args.Args.User,args.Args.User);
            return;
        }

        // Get available volume in target solution
        var targetAvailableVolume = targetSolution.MaxVolume - targetSolution.Volume;

        // If theres no room just silently return
        if (targetAvailableVolume <= 0)
            return;

        // Get amount to add, attempts to add the largest amount with the maximum set from production type
        var amountToAdd =
            FixedPoint2.Clamp(targetAvailableVolume, FixedPoint2.Zero, expellableComponent.MaximumExpulsion);

        var split = expellableSolution.SplitSolution(amountToAdd);
        var quantity = _solutionContainer.AddSolution(targetSolutionComponent.Value, split);

        _popup.PopupPredicted(
            Loc.GetString(
                expellableComponent.PopupSuccess,
                ("amount", quantity),
                ("target", Identity.Entity(args.Target.Value, EntityManager))),
            args.Args.User,
            args.Args.User,
            PopupType.Medium);

        args.Handled = true;
    }
}
