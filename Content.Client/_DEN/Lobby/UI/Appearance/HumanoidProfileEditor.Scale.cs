using System.Numerics;
using Content.Shared.Humanoid.Prototypes;
using Content.Shared.Preferences;
using Robust.Client.UserInterface.Controls;

namespace Content.Client.Lobby.UI;

public sealed partial class HumanoidProfileEditor
{
    private void UpdateScaleSliders()
    {
        if (Profile == null)
            return;

        var species = _prototypeManager.Index(Profile.Species);
        var height = Profile?.Height ?? species.DefaultHeight;
        var width = Profile?.Width ?? species.DefaultWidth;

        SetAxisSliderBoundsAndValue(Height, height, species.DefaultHeightBounds);
        SetAxisSliderBoundsAndValue(Width, width, species.DefaultWidthBounds);

        UpdateScale(true, true);
        SetDirty();
    }

    private static void SetAxisSliderBoundsAndValue(Slider slider, float value, (float min, float max) bounds)
    {
        slider.MaxValue = bounds.max;
        slider.MinValue = bounds.min;
        slider.SetValueWithoutEvent(value);
    }

    private void UpdateScale(bool constrainHeight, bool constrainWidth)
    {
        if (Profile == null)
            return;

        var species = _prototypeManager.Index(Profile.Species);

        var heightBounds = species.DefaultHeightBounds;
        var widthBounds = species.DefaultWidthBounds;

        var height = ClampScale(Profile.Height, heightBounds);
        var width = ClampScale(Profile.Width, widthBounds);

        var constrainedScale = ConstrainedScale(
            height,
            width,
            species,
            constrainHeight,
            constrainWidth);

        height = ClampScale(constrainedScale.height, heightBounds);
        width = ClampScale(constrainedScale.width, widthBounds);

        Profile = Profile.WithHeight(Height.Value,_prototypeManager);
        Profile = Profile.WithWidth(Width.Value,_prototypeManager);

        Height.SetValueWithoutEvent(height);
        Width.SetValueWithoutEvent(width);

        SetDirty();
        UpdateScaleLabels();
        ReloadProfilePreview();
    }

    private static (float height, float width) ConstrainedScale(
        float height,
        float width,
        SpeciesPrototype species,
        bool constrainHeight,
        bool constrainWidth)
    {
        var ratio = height / width;
        var maximumDifference = species.MaximumScaleDifference;
        if (ratio < 1 / maximumDifference || ratio > maximumDifference)
        {
            var targetRatio = ratio < 1 / maximumDifference ? 1 / maximumDifference : maximumDifference;
            if (constrainWidth)
                width = height / targetRatio;
            if (constrainHeight)
                height = width * targetRatio;
        }

        height = ClampScale(height, species.DefaultHeightBounds);
        width = ClampScale(width, species.DefaultWidthBounds);

        return (height, width);
    }

    private static float ClampScale(float toClamp, (float min, float max) bounds)
    {
        return Math.Clamp(toClamp, bounds.min, bounds.max);
    }

    private void UpdateScaleLabels()
    {
        if (Profile == null)
            return;

        var species = _prototypeManager.Index(Profile.Species);

        var height = Profile.Height;
        var heightInCm = species.DefaultHeightMetric * height;
        var feetAndInches = ConvertMetricHeightToImperial(heightInCm);

        UristHeightDisplay.Text = Loc.GetString("humanoid-profile-editor-scale-height-label",
            ("cm", 176.1), // This is hardcoded because i really really cba.
            ("feet", 5),
            ("inches", 9));

        CharHeightDisplay.Text = Loc.GetString("humanoid-profile-editor-scale-height-label",
            ("cm", Math.Round(heightInCm, 1)),
            ("feet", feetAndInches.feet),
            ("inches", feetAndInches.inches));

        ExactScale.Text = Loc.GetString("humanoid-profile-editor-exact-scale",
            ("height", Math.Round(Height.Value, 2)),
            ("width", Math.Round(Width.Value, 2)));
    }

    public static (int feet, int inches) ConvertMetricHeightToImperial(float heightInCm)
    {
        var totalInches = heightInCm / 2.54;

        var feet = (int)Math.Floor(totalInches / 12);

        var inches = (int)totalInches - feet * 12;

        return (feet, inches);
    }
    private void ResetHeight()
    {
        if (Profile == null)
            return;

        var species = _prototypeManager.Index(Profile.Species);

        Height.Value = species.DefaultHeight;
        UpdateScaleSliders();
    }

    private void ResetWidth()
    {
        if (Profile == null)
            return;

        var species = _prototypeManager.Index(Profile.Species);

        Width.Value = species.DefaultWidth;
        UpdateScaleSliders();
    }
}
