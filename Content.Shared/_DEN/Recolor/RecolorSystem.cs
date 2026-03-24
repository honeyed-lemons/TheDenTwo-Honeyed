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

    public override void Initialize()
    {
        base.Initialize();
        // Recolored events
        SubscribeLocalEvent<RecoloredComponent, ComponentStartup>(OnComponentStartup);
        SubscribeLocalEvent<RecoloredComponent, ComponentRemove>(OnComponentRemove);
        // Recolor Applier Events
        SubscribeLocalEvent<RecolorApplierComponent, AfterInteractEvent>(OnRecolorApplierAfterInteract);
        SubscribeLocalEvent<RecolorApplierComponent, ApplyRecolorDoAfterEvent>(OnApplyRecolorDoAfterEvent);
        SubscribeLocalEvent<RecolorApplierComponent, ComponentStartup>(OnComponentStartup);
        SubscribeLocalEvent<RecolorApplierComponent, GetVerbsEvent<UtilityVerb>>(OnGetApplierVerbs);
        SubscribeLocalEvent<RecolorApplierComponent, ExaminedEvent>(OnExamined);
        // Recolor Remover Events
        SubscribeLocalEvent<RecolorRemoverComponent, AfterInteractEvent>(OnRecolorRemoverAfterInteract);
        SubscribeLocalEvent<RecolorRemoverComponent, GetVerbsEvent<UtilityVerb>>(OnGetRemoverVerbs);
        SubscribeLocalEvent<RecolorRemoverComponent, RemoveRecolorDoAfterEvent>(OnRemoveRecolorDoAfterEvent);
    }

    private void OnComponentStartup(Entity<RecoloredComponent> ent, ref ComponentStartup args)
    {
        RefreshVisuals(ent);
    }

    private void OnComponentRemove(Entity<RecoloredComponent> ent, ref ComponentRemove args)
    {
        RemoveVisuals(ent);
    }

    [PublicAPI]
    public void Recolor(EntityUid uid,
        Color color,
        bool removable,
        string? shader = null,
        string? paintType = null,
        List<string>? shaderWhitelist = null,
        List<string>? shaderBlacklist = null)
    {
        if (HasComp<RecoloredComponent>(uid))
        {
            //Replace old recolored component. you can spray things with paint twice.. right?
            RemComp<RecoloredComponent>(uid);
        }

        EnsureComp<AppearanceComponent>(uid);

        var comp = new RecoloredComponent
        {
            Color = color,
            Removable = removable,
            Shader = shader,
            PaintType = paintType,
            ShaderBlacklist = shaderBlacklist,
            ShaderWhitelist = shaderWhitelist,
        };

        AddComp(uid, comp);
        Dirty<RecoloredComponent>((uid, comp));
    }

    [PublicAPI]
    public void RemoveRecolor(Entity<RecoloredComponent?> ent)
    {
        if (!Resolve(ent.Owner, ref ent.Comp, logMissing: false))
            return;

        RemComp(ent, ent.Comp);
    }

    private void RefreshVisuals(Entity<RecoloredComponent> ent)
    {
        if (!TryComp(ent, out AppearanceComponent? appearance))
            return;

        _appearance.SetData(ent, RecolorVisuals.Color, ent.Comp.Color, appearance);

        if (ent.Comp.Shader != null)
            _appearance.SetData(ent, RecolorVisuals.Shader, ent.Comp.Shader, appearance);
        if (ent.Comp.ShaderWhitelist != null)
            _appearance.SetData(ent, RecolorVisuals.ShaderWhitelist, ent.Comp.ShaderWhitelist, appearance);
        if (ent.Comp.ShaderBlacklist != null)
            _appearance.SetData(ent, RecolorVisuals.ShaderBlacklist, ent.Comp.ShaderBlacklist, appearance);
    }

    private void RemoveVisuals(Entity<RecoloredComponent> ent)
    {
        _appearance.RemoveData(ent, RecolorVisuals.Color);
        _appearance.RemoveData(ent, RecolorVisuals.Shader);
        _appearance.RemoveData(ent, RecolorVisuals.ShaderBlacklist);
        _appearance.RemoveData(ent, RecolorVisuals.ShaderWhitelist);
    }
}

[Serializable, NetSerializable]
public sealed partial class ApplyRecolorDoAfterEvent : DoAfterEvent
{
    public Color Color;
    public bool Removable;
    public List<string>? ShaderBlacklist;
    public List<string>? ShaderWhitelist;
    public string? Shader;
    public string? PaintType;

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
    Color,
    Shader,
    ShaderWhitelist,
    ShaderBlacklist,
}
