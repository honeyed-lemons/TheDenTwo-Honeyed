using Content.Shared._DEN.ReagentProduction.EntitySystems;
using Content.Shared._DEN.ReagentProduction.Prototypes;
using JetBrains.Annotations;
using Robust.Shared.Prototypes;

namespace Content.Shared._DEN.Traits.TraitFunctions;

[UsedImplicitly]
public sealed partial class AddReagentProductionTrait : ITraitFunction
{
    /// <summary>
    /// Reagent Production types this trait provides.
    /// </summary>
    [DataField(required: true)] public List<ProtoId<ReagentProductionTypePrototype>> ProductionTypes { get; private set; } = [];

    [ViewVariables] public List<IComponent>? AddedComponents = null;

    public void OnTraitAdded(EntityUid owner, EntityManager entityManager)
    {

        var prototypeManager = IoCManager.Resolve<IPrototypeManager>();
        var reagentProduction = entityManager.System<ReagentProductionSystem>();

        foreach (var productionType in ProductionTypes)
        {
            if (!prototypeManager.TryIndex(productionType, out var prototype))
                continue;

            reagentProduction.AddProductionType(owner, prototype);
        }
    }

    public void OnTraitRemoved(EntityUid owner, EntityManager entityManager)
    {
        var prototypeManager = IoCManager.Resolve<IPrototypeManager>();
        var reagentProduction = entityManager.System<ReagentProductionSystem>();

        foreach (var productionType in ProductionTypes)
        {
            if (!prototypeManager.TryIndex(productionType, out var prototype))
                continue;

            reagentProduction.RemoveProductionType(owner, prototype);
        }
    }
}
