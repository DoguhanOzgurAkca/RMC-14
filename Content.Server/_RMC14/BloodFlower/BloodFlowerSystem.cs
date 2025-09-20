using Content.Shared._RMC14.BloodFlower;

namespace Content.Server._RMC14.BloodFlower;

public sealed class BloodFlowerSystem : SharedBloodFlowerSystem
{
    public override void Initialize()
    {
        base.Initialize();
    }

    protected override void OnFlowerAnimate(EntityUid uid, BloodFlowerComponent component)
    {
        base.OnFlowerAnimate(uid, component);
        // Server-side logic can be added here if needed
    }
}