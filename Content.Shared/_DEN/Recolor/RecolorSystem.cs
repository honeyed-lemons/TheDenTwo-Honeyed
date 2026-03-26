using Content.Shared._DEN.Recolor.Components;
using Content.Shared.DoAfter;
using Content.Shared.Examine;
using Content.Shared.Interaction;
using Content.Shared.Nutrition.EntitySystems;
using Content.Shared.Popups;
using Content.Shared.Verbs;
using Content.Shared.Whitelist;
using JetBrains.Annotations;
using Robust.Shared.Audio.Systems;
using Robust.Shared.ColorNaming;
using Robust.Shared.Serialization;

namespace Content.Shared._DEN.Recolor;

public sealed partial class RecolorSystem : EntitySystem
{
    [Dependency] private readonly ILocalizationManager _localizationManager = default!;

    [Dependency] private readonly EntityWhitelistSystem _whitelist = default!;
    [Dependency] private readonly OpenableSystem _openable = default!;
    [Dependency] private readonly SharedAppearanceSystem _appearance = default!;
    [Dependency] private readonly SharedAudioSystem _audio = default!;
    [Dependency] private readonly SharedDoAfterSystem _doAfter = default!;
    [Dependency] private readonly SharedPopupSystem _popup = default!;
    [Dependency] private readonly SharedUserInterfaceSystem _ui = default!;

    public override void Initialize()
    {
        base.Initialize();
        // Recolored events
        SubscribeLocalEvent<RecoloredComponent, ComponentStartup>(OnComponentStartup);
        SubscribeLocalEvent<RecoloredComponent, ComponentRemove>(OnComponentRemove);
        SubscribeLocalEvent<RecoloredComponent, ExaminedEvent>(OnExamined);

        // Recolor Applier Events
        SubscribeLocalEvent<RecolorApplierComponent, AfterInteractEvent>(OnRecolorApplierAfterInteract);
        SubscribeLocalEvent<RecolorApplierComponent, ApplyRecolorDoAfterEvent>(OnApplyRecolorDoAfterEvent);
        SubscribeLocalEvent<RecolorApplierComponent, ComponentStartup>(OnComponentStartup);
        SubscribeLocalEvent<RecolorApplierComponent, GetVerbsEvent<UtilityVerb>>(OnGetApplierVerbs);
        SubscribeLocalEvent<RecolorApplierComponent, ExaminedEvent>(OnExamined);

        // Recolor Applier Color Selector Events
        SubscribeLocalEvent<RecolorApplierColorSelectorComponent, BoundUIOpenedEvent>(OnBoundUIOpened);
        SubscribeLocalEvent<RecolorApplierColorSelectorComponent, RecolorApplierColorMessage>(OnRecolorApplierColorChanged);

        // Recolor Remover Events
        SubscribeLocalEvent<RecolorRemoverComponent, AfterInteractEvent>(OnRecolorRemoverAfterInteract);
        SubscribeLocalEvent<RecolorRemoverComponent, GetVerbsEvent<UtilityVerb>>(OnGetRemoverVerbs);
        SubscribeLocalEvent<RecolorRemoverComponent, RemoveRecolorDoAfterEvent>(OnRemoveRecolorDoAfterEvent);
    }

    /// <summary>
    /// Recolor an entity using provided recolorData.
    /// </summary>
    /// <param name="uid">Entity to recolor.</param>
    /// <param name="recolorData">Recolor data to use when recoloring.</param>
    [PublicAPI]
    public void Recolor(
        EntityUid uid,
        RecolorData recolorData)
    {
        if (HasComp<RecoloredComponent>(uid))
        {
            //Replace old recolored component. you can spray things with paint twice.. right?
            RemComp<RecoloredComponent>(uid);
        }

        EnsureComp<AppearanceComponent>(uid);

        var comp = new RecoloredComponent
        {
            RecolorData = recolorData,
        };

        AddComp(uid, comp);
        Dirty<RecoloredComponent>((uid, comp));
    }

    /// <summary>
    /// Recolor an entity with simple parameters.
    /// </summary>
    /// <param name="uid">Entity to recolor.</param>
    /// <param name="color">Color to recolor to.</param>
    /// <param name="removable">If the recoloring can be removed by regular means.</param>
    /// <param name="shader">Shader to replace default shaders with.</param>
    /// <param name="paintType">Paint type to use, purely for flavor.</param>
    /// <param name="examineText">Examine text LocId to use.</param>
    [PublicAPI]
    public void Recolor(
        EntityUid uid,
        Color color,
        bool removable,
        string? shader,
        string? paintType,
        string examineText = "recolored-examine")
    {
        if (HasComp<RecoloredComponent>(uid))
        {
            //Replace old recolored component. you can spray things with paint twice.. right?
            RemComp<RecoloredComponent>(uid);
        }

        EnsureComp<AppearanceComponent>(uid);

        var comp = new RecoloredComponent
        {
            RecolorData = new RecolorData
            {
                Color = color,
                Removable = removable,
                Shader = shader,
                PaintType = paintType,
            },
            ExamineText = examineText,
        };

        AddComp(uid, comp);
        Dirty<RecoloredComponent>((uid, comp));
    }

    /// <param name="ent">Entity to remove the recolor of.</param>
    [PublicAPI]
    public void RemoveRecolor(Entity<RecoloredComponent?> ent)
    {
        if (!Resolve(ent.Owner, ref ent.Comp, logMissing: false))
            return;

        RemComp(ent, ent.Comp);
    }

    private void OnComponentStartup(Entity<RecoloredComponent> ent, ref ComponentStartup args)
    {
        RefreshVisuals(ent);
    }

    private void OnComponentRemove(Entity<RecoloredComponent> ent, ref ComponentRemove args)
    {
        RemoveVisuals(ent);
    }

    private void OnExamined(Entity<RecoloredComponent> ent, ref ExaminedEvent args)
    {
        if (!args.IsInDetailsRange)
            return;

        var recolorData = ent.Comp.RecolorData;

        if (recolorData is { PaintType: not null})
        {
            var colorName = GetColorName(recolorData);

            args.PushMarkup(Loc.GetString(
                ent.Comp.ExamineText,
                ("color", recolorData.Color),
                ("colorName", colorName),
                ("paintType", recolorData.PaintType)));
        }
    }


    private string GetColorName(RecolorData recolorData)
    {
        // Get the color's name. If the color itself has a color defined, use it
        return recolorData.ColorName ??
                        // Otherwise, use what colornaming THINKS it is.
                        ColorNaming.Describe(recolorData.Color, _localizationManager);
    }

    private void RefreshVisuals(Entity<RecoloredComponent> ent)
    {
        if (!TryComp(ent, out AppearanceComponent? appearance))
            return;

        var recolorData = ent.Comp.RecolorData;

        _appearance.SetData(ent, RecolorVisuals.RecolorData, recolorData, appearance);
    }

    private void RemoveVisuals(Entity<RecoloredComponent> ent)
    {
        _appearance.RemoveData(ent, RecolorVisuals.RecolorData);
    }
}

/// <summary>
/// Stores information regarding recolored objects.
/// </summary>
[Serializable, NetSerializable]
[DataDefinition]
public sealed partial class RecolorData
{
    /// <summary>
    /// Color to recolor with.
    /// </summary>
    [DataField]
    public Color Color { get; set; } = Color.White;
    /// <summary>
    /// Whether the Recolor can be removed by the RecolorRemoverSystem.
    /// </summary>
    [DataField]
    public bool Removable { get; set; } = true;
    /// <summary>
    /// Name of the color, purely for flavor.
    /// </summary>
    [DataField]
    public string? ColorName { get; set; }
    /// <summary>
    /// Purely for flavor, used for locale information.
    /// </summary>
    [DataField]
    public string? PaintType { get; set; }
    /// <summary>
    /// Replaces layers shader with this shader.
    /// </summary>
    [DataField]
    public string? Shader { get; set; } = "Desaturated";
    /// <summary>
    /// If used, these will be the only shaders replaced.
    /// </summary>
    [DataField]
    public List<string>? ShaderBlacklist { get; set; }
    /// <summary>
    /// If used, these shaders will never be replaced.
    /// </summary>
    [DataField]
    public List<string>? ShaderWhitelist { get; set; }
}

[Serializable, NetSerializable]
public sealed partial class ApplyRecolorDoAfterEvent : DoAfterEvent
{
    public RecolorData RecolorData;

    public override DoAfterEvent Clone()
    {
        return this;
    }
}

[Serializable, NetSerializable]
public sealed partial class RemoveRecolorDoAfterEvent : SimpleDoAfterEvent;


[Serializable, NetSerializable]
public enum RecolorVisuals
{
    RecolorData
}
