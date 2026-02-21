using Content.Shared.Construction.Prototypes;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Shared._DEN.QuickConstruction.Prototypes;
[Prototype]
public sealed partial class QuickConstructionCategoryPrototype : IPrototype
{
    [ViewVariables]
    [IdDataField]
    public string ID { get; private set; } = default!;

    /// <summary>
    /// Name used when nested under another category
    /// </summary>
    [DataField]
    public LocId Name = string.Empty;
    /// <summary>
    /// Icon used when nested under another category
    /// </summary>
    [DataField]
    public SpriteSpecifier? Icon = SpriteSpecifier.Invalid;
    /// <summary>
    /// List of ConstructionPrototype(s) to list under the category, can be left empty.
    /// </summary>
    [DataField]
    public List<ProtoId<ConstructionPrototype>> ConstructionEntries = [];
    /// <summary>
    /// List of QuickConstructionCategoryPrototype(s) to list under the category, can be left empty.
    /// </summary>
    [DataField]
    public List<ProtoId<QuickConstructionCategoryPrototype>> CategoryEntries = [];
}
