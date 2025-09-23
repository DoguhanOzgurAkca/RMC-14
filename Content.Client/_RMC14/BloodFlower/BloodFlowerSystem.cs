using Content.Shared._RMC14.BloodFlower;
using Content.Shared.GameTicking;
using Robust.Client.GameObjects;
using Robust.Shared.Timing;

namespace Content.Client._RMC14.BloodFlower;

public sealed class BloodFlowerSystem : SharedBloodFlowerSystem
{
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly SpriteSystem _sprite = default!;
    [Dependency] private readonly PointLightSystem _pointLight = default!;
    [Dependency] private readonly SharedGameTicker _gameTicker = default!;

    public override void Initialize()
    {
        base.Initialize();
        // No need for event subscriptions - we handle everything in Update()
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        // Handle all flowers globally based on round time for perfect synchronization
        var query = EntityQueryEnumerator<BloodFlowerComponent>();
        while (query.MoveNext(out var uid, out var component))
        {
            UpdateFlowerGlow(uid, component);
        }
    }

    private void UpdateFlowerGlow(EntityUid uid, BloodFlowerComponent component)
    {
        if (!TryComp<SpriteComponent>(uid, out var sprite) || !TryComp<PointLightComponent>(uid, out var light))
            return;

        // Calculate global animation timing based on round duration
        var roundDuration = _gameTicker.RoundDuration();
        var cycleTime = roundDuration.TotalSeconds % component.AnimationInterval;
        
        // Should animate for first 10.2 seconds of each 20-second cycle
        var shouldAnimate = cycleTime <= 10.2f;

        if (shouldAnimate)
        {
            // Calculate decay progress within the 10.2 second animation window
            var progress = (float)(cycleTime / 10.2f);
            
            // Smooth exponential decay for more natural fading
            var decayFactor = 1f - MathF.Pow(progress, 2f);
            
            // Set sprite to glowing state and apply light
            _sprite.LayerSetRsiState((uid, sprite), BloodFlowerVisualLayers.Base, "glowing");
            _pointLight.SetEnabled(uid, true, light);
            _pointLight.SetEnergy(uid, 4.0f * decayFactor, light);
            _pointLight.SetRadius(uid, 7.5f * decayFactor, light);
        }
        else
        {
            // Not animating - ensure light is off and sprite is unpowered
            _sprite.LayerSetRsiState((uid, sprite), BloodFlowerVisualLayers.Base, "unpowered");
            _pointLight.SetEnabled(uid, false, light);
            _pointLight.SetEnergy(uid, 0f, light);
            _pointLight.SetRadius(uid, 0f, light);
        }
    }
}

public enum BloodFlowerVisualLayers : byte
{
    Base,
}