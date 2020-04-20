using EcsFun.Components;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;

namespace EcsFun.Systems
{
    public class PhysicsSystem : EntityProcessingSystem
    {
        private readonly GraphicsDeviceManager graphics;
        private ComponentMapper<Physical> physicalMapper = null!;
        private ComponentMapper<Transform2> transformMapper = null!;

        public PhysicsSystem(GraphicsDeviceManager graphics) : base(Aspect.All(typeof(Physical), typeof(Transform2)))
        {
            this.graphics = graphics;
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            physicalMapper = mapperService.GetMapper<Physical>();
            transformMapper = mapperService.GetMapper<Transform2>();
        }

        public override void Process(GameTime gameTime, int entityId)
        {
            var physical = physicalMapper.Get(entityId);
            var transform = transformMapper.Get(entityId);

            if (transform.Position.Y > graphics.PreferredBackBufferHeight)
                physical.Velocity = new Vector2(physical.Velocity.X, -physical.Velocity.Y);

            if (physical.AffectedByGravity)
                physical.Velocity += new Vector2(0, 9.81f * gameTime.GetElapsedSeconds() * 10);

            transform.Position += physical.Velocity * gameTime.GetElapsedSeconds();
        }
    }
}