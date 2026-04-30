using Content.Shared._DEN.ReagentProduction.EntitySystems;
using Content.Shared._DEN.ReagentProduction.Prototypes;
using Content.Shared.DoAfter;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;


namespace Content.Shared._DEN.ReagentProduction.Events;

public sealed class ReagentProductionEvents
{
    /// <summary>
    /// When this event is called, a production type is added to the entity it's called on.
    /// Do not call this directly, and instead use <see cref="ReagentProductionSystem.AddProductionType"/>
    /// </summary>
    /// <param name="productionType">Production type to add.</param>
    [Serializable, NetSerializable,]
    public sealed class ReagentProductionTypeAdded(ProtoId<ReagentProductionTypePrototype> productionType) : EntityEventArgs
    {
        public ProtoId<ReagentProductionTypePrototype> ProductionType { get; } = productionType;
    }

    /// <summary>
    /// When this event is called, a production type is removed from the entity it's called on.
    /// Do not call this directly, and instead use <see cref="ReagentProductionSystem.RemoveProductionType"/>
    /// </summary>
    /// <param name="productionType">Production type to remove.</param>
    [Serializable, NetSerializable,]
    public sealed class ReagentProductionTypeRemoved(ProtoId<ReagentProductionTypePrototype> productionType) : EntityEventArgs
    {
        public ProtoId<ReagentProductionTypePrototype> ProductionType { get; } = productionType;
    }
}
/// <summary>
/// Classic doafter event, called when attempting to fill a solution container with a specific production type.
/// </summary>
[Serializable, NetSerializable,]
public sealed partial class ReagentProductionFillEvent : DoAfterEvent
{
    /// <summary>
    /// Production type to fill with.
    /// </summary>
    public ProtoId<ReagentProductionTypePrototype> ProductionType;

    public ReagentProductionFillEvent( ProtoId<ReagentProductionTypePrototype> productionType)
    {
        ProductionType = productionType;
    }

    public override DoAfterEvent Clone()
    {
        return this;
    }
}

