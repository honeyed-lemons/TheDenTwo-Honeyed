using Content.Shared.Body;
using Content.Shared.Dataset;
using Content.Shared.Humanoid.Markings;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using System.Numerics; // DEN

namespace Content.Shared.Humanoid.Prototypes;

[Prototype]
public sealed partial class SpeciesPrototype : IPrototype
{
    /// <summary>
    /// Prototype ID of the species.
    /// </summary>
    [IdDataField]
    public string ID { get; private set; } = default!;

    /// <summary>
    /// User visible name of the species.
    /// </summary>
    [DataField(required: true)]
    public string Name { get; private set; } = default!;

    /// <summary>
    ///     Descriptor. Unused...? This is intended
    ///     for an eventual integration into IdentitySystem
    ///     (i.e., young human person, young lizard person, etc.)
    /// </summary>
    [DataField]
    public string Descriptor { get; private set; } = "humanoid";

    /// <summary>
    /// Whether the species is available "at round start" (In the character editor)
    /// </summary>
    [DataField(required: true)]
    public bool RoundStart { get; private set; } = false;

    /// <summary>
    ///     Default skin tone for this species. This applies for non-human skin tones.
    /// </summary>
    [DataField]
    public Color DefaultSkinTone { get; private set; } = Color.White;

    /// <summary>
    ///     Default human skin tone for this species. This applies for human skin tones.
    ///     See <see cref="SkinColor.HumanSkinTone"/> for the valid range of skin tones.
    /// </summary>
    [DataField]
    public int DefaultHumanSkinTone { get; private set; } = 20;

    /// <summary>
    ///     Humanoid species variant used by this entity.
    /// </summary>
    [DataField(required: true)]
    public EntProtoId Prototype { get; private set; } = default!;

    /// <summary>
    /// Prototype used by the species for the dress-up doll in various menus.
    /// </summary>
    [DataField(required: true)]
    public EntProtoId DollPrototype { get; private set; } = default!;

    /// <summary>
    /// Method of skin coloration used by the species.
    /// </summary>
    [DataField(required: true)]
    public ProtoId<SkinColorationPrototype> SkinColoration { get; private set; }

    [DataField]
    public ProtoId<LocalizedDatasetPrototype> MaleFirstNames { get; private set; } = "NamesFirstMale";

    [DataField]
    public ProtoId<LocalizedDatasetPrototype> FemaleFirstNames { get; private set; } = "NamesFirstFemale";

    [DataField]
    public ProtoId<LocalizedDatasetPrototype> LastNames { get; private set; } = "NamesLast";

    [DataField]
    public SpeciesNaming Naming { get; private set; } = SpeciesNaming.FirstLast;

    [DataField]
    public List<Sex> Sexes { get; private set; } = new() { Sex.Male, Sex.Female };

    /// <summary>
    ///     Characters younger than this are too young to be hired by Nanotrasen.
    /// </summary>
    [DataField]
    public int MinAge = 18;

    /// <summary>
    ///     Characters younger than this appear young.
    /// </summary>
    [DataField]
    public int YoungAge = 30;

    /// <summary>
    ///     Characters older than this appear old. Characters in between young and old age appear middle aged.
    /// </summary>
    [DataField]
    public int OldAge = 60;

    /// <summary>
    ///     Characters cannot be older than this. Only used for restrictions...
    ///     although imagine if ghosts could age people WYCI...
    /// </summary>
    [DataField]
    public int MaxAge = 120;

    //DEN Start

    /// <summary>
    ///     Minimum and Maximum height this species can be.
    /// </summary>
    [DataField]
    public (float min, float max) DefaultHeightBounds = new(0.75f,1.5f);
    /// <summary>
    ///     Minimum and Maximum width this species can be.
    /// </summary>
    [DataField]
    public (float min, float max) DefaultWidthBounds = new(0.75f,1.5f);
    /// <summary>
    ///     The default height of this species.
    /// </summary>
    [DataField]
    public float DefaultHeight = 1;
    /// <summary>
    ///     The default width of this species.
    /// </summary>
    [DataField]
    public float DefaultWidth = 1;
    /// <summary>
    ///     The maximum difference between height and width permitted.
    /// </summary>
    [DataField]
    public float MaximumScaleDifference = 1.1f;
    /// <summary>
    ///     The default height of this species in centimeters.
    /// </summary>
    [DataField]
    public float DefaultHeightMetric = 176.1f; // this assumes every pixel of the default urist sprite is about 6.07 centimeters (so a full 32 pixels is 194.24)

    //DEN End
}

public enum SpeciesNaming : byte
{
    First,
    FirstLast,
    FirstDashFirst,
    TheFirstofLast,
}
