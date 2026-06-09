using Content.Shared.Chemistry.Components;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared._DEN.SolutionExpulsion.Components;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class SolutionExpellerComponent : Component
{
    [ViewVariables, AutoNetworkedField]
    public Dictionary<EntProtoId,EntityUid> SolutionEntities = [];
}
