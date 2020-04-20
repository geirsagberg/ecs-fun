using System;
using EcsFun.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using MonoGame.Extended.Sprites;

namespace EcsFun.Systems
{
    public class RenderSystem : EntityDrawSystem
    {
        private readonly SharedState sharedState;
        private readonly SpriteBatch spriteBatch;
        private readonly Sprites sprites;
        private ComponentMapper<Transform2> transformMapper = null!;
        private ComponentMapper<EntityInfo> infoMapper = null!;

        public RenderSystem(SharedState sharedState, SpriteBatch spriteBatch, Sprites sprites) : base(Aspect.All(
            typeof(EntityInfo),
            typeof(Transform2)))
        {
            this.sharedState = sharedState;
            this.spriteBatch = spriteBatch;
            this.sprites = sprites;
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            transformMapper = mapperService.GetMapper<Transform2>();
            infoMapper = mapperService.GetMapper<EntityInfo>();
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();

            foreach (var entity in ActiveEntities) {
                var transform = transformMapper.Get(entity);
                var entityInfo = infoMapper.Get(entity);
                var sprite = entityInfo.Sprite == SpriteType.Anki ? sprites.Anki : sprites.Bronch;
                var color = ColorFromHSV(entityInfo.Hue, 1, 1);
                sprite.Color = color;
                spriteBatch.Draw(sprite, transform);
                if (sharedState.SelectedEntity == entity) {
                    spriteBatch.DrawRectangle(sprite.GetBoundingRectangle(transform), Color.Blue);
                }
            }

            spriteBatch.End();
        }

        public static Color ColorFromHSV(double hue, double saturation, double value)
        {
            var hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
            var f = hue / 60 - Math.Floor(hue / 60);

            value = value * 255;
            var v = Convert.ToInt32(value);
            var p = Convert.ToInt32(value * (1 - saturation));
            var q = Convert.ToInt32(value * (1 - f * saturation));
            var t = Convert.ToInt32(value * (1 - (1 - f) * saturation));

            if (hi == 0)
                return new Color(v, t, p, 255);
            else if (hi == 1)
                return new Color(q, v, p, 255);
            else if (hi == 2)
                return new Color(p, v, t, 255);
            else if (hi == 3)
                return new Color(p, q, v, 255);
            else if (hi == 4)
                return new Color(t, p, v, 255);
            else
                return new Color(v, p, q, 255);
        }
    }
}