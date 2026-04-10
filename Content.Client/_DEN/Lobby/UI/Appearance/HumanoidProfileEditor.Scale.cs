using System.Numerics;
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

        var height = ClampAxis(Profile.Height, heightBounds);
        var width = ClampAxis(Profile.Width, widthBounds);

        var sizeRatio = species.SizeRatio;
        var ratio = height / width;

        if (ratio < 1 / sizeRatio || ratio > sizeRatio)
        {
            var targetRatio = ratio < 1 / sizeRatio ? 1 / sizeRatio : sizeRatio;
            if (constrainWidth)
                width = height / targetRatio;
            if (constrainHeight)
                height = width * targetRatio;
        }

        height = ClampAxis(height, heightBounds);
        width = ClampAxis(width, widthBounds);

        Profile = Profile.WithHeight(Height.Value,_prototypeManager);
        Profile = Profile.WithWidth(Width.Value,_prototypeManager);

        Height.SetValueWithoutEvent(height);
        Width.SetValueWithoutEvent(width);

        SetDirty();
        UpdateScaleLabels();
        ReloadProfilePreview();
    }

    private static float ClampAxis(float toClamp, (float min, float max) bounds)
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
