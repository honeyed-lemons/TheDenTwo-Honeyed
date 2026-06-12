using Content.Shared._DEN.SolutionExpulsion.EntitySystems;
using Robust.Shared.Prototypes;

namespace Content.Shared._DEN.Traits.TraitFunctions;

public sealed partial class AddSolutionExpulsionTrait : ITraitFunction
{
    /// <summary>
    /// Solution entities types this trait adds to the entity.
    /// </summary>
    [DataField(required: true)] public List<EntProtoId> SolutionExpellableEntities { get; private set; } = [];

    [ViewVariables] public List<IComponent>? AddedComponents = null;

    public void OnTraitAdded(EntityUid owner, EntityManager entityManager)
    {

        var prototypeManager = IoCManager.Resolve<IPrototypeManager>();
        var solutionExpulsion = entityManager.System<SolutionExpulsionSystem>();

        foreach (var solutionToExpel in SolutionExpellableEntities)
        {
            if (!prototypeManager.TryIndex(solutionToExpel, out var prototype))
                continue;

            if (solutionExpulsion.GetExpellableSolutions(owner, out var solutions)
                && solutions.ContainsKey(solutionToExpel))
                return;

            solutionExpulsion.AddExpellableSolution(owner, prototype);
        }
    }

    public void OnTraitRemoved(EntityUid owner, EntityManager entityManager)
    {
        var prototypeManager = IoCManager.Resolve<IPrototypeManager>();
        var solutionExpulsion = entityManager.System<SolutionExpulsionSystem>();

        foreach (var solutionToExpel in SolutionExpellableEntities)
        {
            if (!prototypeManager.TryIndex(solutionToExpel, out var prototype))
                continue;

            solutionExpulsion.RemoveExpellableSolution(owner, prototype);
        }
    }
}
