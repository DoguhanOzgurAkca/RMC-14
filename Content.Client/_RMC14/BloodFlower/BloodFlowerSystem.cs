using Content.Shared._RMC14.BloodFlower;
using Robust.Client.GameObjects;
using Robust.Shared.Timing;

namespace Content.Client._RMC14.BloodFlower;

public sealed class BloodFlowerSystem : SharedBloodFlowerSystem
{
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly SpriteSystem _sprite = default!;
    [Dependency] private readonly PointLightSystem _pointLight = default!;

    // Component to track the glow decay
    private readonly Dictionary<EntityUid, BloodFlowerGlowState> _activeGlows = new();

    private struct BloodFlowerGlowState
    {
        public TimeSpan StartTime;
        public float InitialEnergy;
        public float InitialRadius;
        public TimeSpan DecayDuration;
    }

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<BloodFlowerComponent, ComponentShutdown>(OnComponentShutdown);
    }

    private void OnComponentShutdown(EntityUid uid, BloodFlowerComponent component, ComponentShutdown args)
    {
        _activeGlows.Remove(uid);
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var currentTime = _timing.CurTime;
        var toRemove = new List<EntityUid>();

        foreach (var (uid, glowState) in _activeGlows)
        {
            if (!TryComp<PointLightComponent>(uid, out var light))
            {
                toRemove.Add(uid);
                continue;
            }

            var elapsed = currentTime - glowState.StartTime;
            if (elapsed >= glowState.DecayDuration)
            {
                // Decay complete - turn off light
                _pointLight.SetEnabled(uid, false, light);
                _pointLight.SetEnergy(uid, 0f, light);
                _pointLight.SetRadius(uid, 0f, light);
                toRemove.Add(uid);
                
                // Return sprite to normal state
                if (TryComp<SpriteComponent>(uid, out var sprite))
                {
                    _sprite.LayerSetRsiState((uid, sprite), BloodFlowerVisualLayers.Base, "base");
                }
                continue;
            }

            // Calculate decay progress (0 = start, 1 = complete)
            var progress = (float)(elapsed.TotalSeconds / glowState.DecayDuration.TotalSeconds);
            
            // Smooth exponential decay for more natural fading
            var decayFactor = 1f - MathF.Pow(progress, 2f);
            
            // Apply the decay
            _pointLight.SetEnergy(uid, glowState.InitialEnergy * decayFactor, light);
            _pointLight.SetRadius(uid, glowState.InitialRadius * decayFactor, light);
        }

        foreach (var uid in toRemove)
        {
            _activeGlows.Remove(uid);
        }
    }

    protected override void OnFlowerAnimate(EntityUid uid, BloodFlowerComponent component)
    {
        base.OnFlowerAnimate(uid, component);

        if (!TryComp<SpriteComponent>(uid, out var sprite) || !TryComp<PointLightComponent>(uid, out var light))
            return;

        // Don't restart if already glowing
        if (_activeGlows.ContainsKey(uid))
            return;

        // Set sprite to glowing state
        _sprite.LayerSetRsiState((uid, sprite), BloodFlowerVisualLayers.Base, "glowing");

        // Start the glow decay
        var glowState = new BloodFlowerGlowState
        {
            StartTime = _timing.CurTime,
            InitialEnergy = 4.0f,      // Bright red glow
            InitialRadius = 7.5f,      // Large radius (2.5x base size)
            DecayDuration = TimeSpan.FromSeconds(10.2f) // Smooth decay over ~10 seconds
        };

        // Enable light and set initial values
        _pointLight.SetEnabled(uid, true, light);
        _pointLight.SetEnergy(uid, glowState.InitialEnergy, light);
        _pointLight.SetRadius(uid, glowState.InitialRadius, light);

        _activeGlows[uid] = glowState;
    }
}

public enum BloodFlowerVisualLayers : byte
{
    Base,
}