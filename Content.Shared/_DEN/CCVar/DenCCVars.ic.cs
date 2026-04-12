using Robust.Shared.Configuration;

namespace Content.Shared._DEN.CCVar;

public sealed partial class DenCCVars
{
    /// <summary>
    ///     Whether to randomize height when randomizing a character.
    /// </summary>
    public static readonly CVarDef<bool> RandomizeHeight =
        CVarDef.Create("ic.random_height", f, CValsear.SERVER | CVar.REPLICATED);
}
