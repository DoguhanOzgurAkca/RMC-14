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
        // Trigger initial animation check
        CheckForAnimation(uid, component);
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var query = EntityQueryEnumerator<BloodFlowerComponent>();
        while (query.MoveNext(out var uid, out var component))
        {
            CheckForAnimation(uid, component);
        }
    }

    protected virtual void CheckForAnimation(EntityUid uid, BloodFlowerComponent component)
    {
        // Calculate if flower should animate based on round time
        var roundDuration = _gameTicker.RoundDuration();
        var cycleTime = roundDuration.TotalSeconds % component.AnimationInterval;
        
        // Trigger animation at the start of each 15-second cycle
        // Animation plays for ~10.2 seconds based on meta.json
        var shouldAnimate = cycleTime <= 10.2f;

        if (shouldAnimate)
        {
            OnFlowerAnimate(uid, component);
        }
    }

    protected virtual void OnFlowerAnimate(EntityUid uid, BloodFlowerComponent component)
    {
        // Override in client/server systems for specific behavior
    }
}