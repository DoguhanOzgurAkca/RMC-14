using Robust.Shared.GameStates;

namespace Content.Shared._RMC14.BloodFlower;

[RegisterComponent]
[Access(typeof(SharedBloodFlowerSystem))]
public sealed partial class BloodFlowerComponent : Component
{
    /// <summary>
    /// How often the flower should animate, in seconds
    /// </summary>
    [DataField]
    public float AnimationInterval { get; set; } = 20f;
}