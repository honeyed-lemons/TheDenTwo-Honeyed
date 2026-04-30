using Content.Shared._DEN.ReagentProduction.Prototypes;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;

namespace Content.Shared._DEN.ReagentProduction.Components;
/// <summary>
/// Entities with this component produce reagents based on
/// what types of <see cref="ReagentProductionTypePrototype"/> this component has.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class ReagentProducerComponent : Component
{
    /// <summary>
    /// A list of production types this component manages.
    /// </summary>
    [DataField, AutoNetworkedField]
    public List<ProtoId<ReagentProductionTypePrototype>> ProductionTypes = [];

    /// <summary>
    ///     The next time to fill solution
    /// </summary>
    [DataField(customTypeSerializer: typeof(TimeOffsetSerializer))]
    public TimeSpan NextUpdate;

    /// <summary>
    ///     The interval between updates.
    /// </summary>
    [DataField]
    public TimeSpan UpdateInterval = TimeSpan.FromSeconds(10);
}
