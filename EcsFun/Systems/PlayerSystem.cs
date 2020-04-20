using EcsFun.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;

namespace EcsFun.Systems
{
    public class PlayerSystem : EntityProcessingSystem
    {
        private ComponentMapper<Player> playerMapper;
        private ComponentMapper<Transform2> transformMapper;

        public PlayerSystem() : base(Aspect.All(typeof(Player), typeof(Transform2)))
        {
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            playerMapper = mapperService.GetMapper<Player>();
            transformMapper = mapperService.GetMapper<Transform2>();
        }

        public override void Process(GameTime gameTime, int entityId)
        {
            var keyboardState = Keyboard.GetState();
            var movement = new Vector2(
                keyboardState.IsKeyDown(Keys.D)
                    ? 1
                    : keyboardState.IsKeyDown(Keys.A)
                        ? -1
                        : 0,
                keyboardState.IsKeyDown(Keys.S)
                    ? 1
                    : keyboardState.IsKeyDown(Keys.W)
                        ? -1
                        : 0);

            transformMapper.Get(entityId).Position += movement * gameTime.GetElapsedSeconds() * 100;
        }
    }
}