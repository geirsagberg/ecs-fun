using System;
using EcsFun.Components;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;

namespace EcsFun.Systems
{
    public class SpawnSystem : EntitySystem
    {
        private readonly Random random;

        public SpawnSystem(SharedState sharedState) : base(Aspect.All())
        {
            sharedState.CreateEntity += SpawnEntity;
            random = new Random();
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
        }

        public void SpawnEntity(float x, float y)
        {
            var entity = CreateEntity();
            var transform = new Transform2(x, y);
            entity.Attach(transform);
            var entityInfo = new EntityInfo {
                Hue = random.Next(360)
            };
            entity.Attach(entityInfo);
        }
    }
}