using Content.Shared.GameTicking;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.BloodFlower;

public abstract class SharedBloodFlowerSystem : EntitySystem
{
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly SharedGameTicker _gameTicker = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<BloodFlowerComponent, ComponentInit>(OnComponentInit);
    }

    private void OnComponentInit(EntityUid uid, BloodFlowerComponent component, ComponentInit args)
    {
        // Component initialized - client/server systems handle their own timing
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);
        // Client and server systems handle their own logic directly
        // No need to trigger events - timing is handled globally
    }
}