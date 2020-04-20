using System;
using System.Collections.Concurrent;
using System.Linq;
using EcsFun.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using MonoGame.Extended.Input.InputListeners;
using Myra.Graphics2D.UI;
using Thickness = Myra.Graphics2D.Thickness;

namespace EcsFun.Systems
{
    public class ControlSystem : EntitySystem
    {
        private CheckBox isPlayerCheckBox;
        private readonly SharedState sharedState;
        private readonly Random random;
        private readonly GraphicsDeviceManager graphics;
        private readonly Sprites sprites;
        private ComponentMapper<Transform2> transformMapper = null!;

        private readonly ConcurrentDictionary<Texture2D, Color[]> colorsByTexture =
            new ConcurrentDictionary<Texture2D, Color[]>();

        private readonly Texture2D textureAnki;
        private ComponentMapper<Player> playerMapper;
        private Grid grid;
        private bool skipNextMouseClicked;
        private CheckBox isPhysicalCheckBox;
        private ComponentMapper<EntityInfo> infoMapper = null!;
        private ComponentMapper<Physical> physicalMapper = null!;

        public ControlSystem(SharedState sharedState, MouseListener mouseListener,
            Random random, GraphicsDeviceManager graphics, Sprites sprites) : base(Aspect.All(typeof(Transform2),
            typeof(EntityInfo)))
        {
            this.sharedState = sharedState;
            this.random = random;
            this.graphics = graphics;
            this.sprites = sprites;
            mouseListener.MouseClicked += OnMouseClicked;
        }

        private void SetupGui()
        {
            var button = new TextButton {
                GridColumn = 0, GridRow = 0, Text = "Create entity", Padding = new Thickness(8)
            };
            button.Click += delegate {
                sharedState.OnCreateEntity(random.Next(100, graphics.PreferredBackBufferWidth - 100),
                    random.Next(100, graphics.PreferredBackBufferHeight - 100));
            };

            var stack = new VerticalStackPanel {
                Width = 150
            };
            stack.Widgets.Add(button);

            grid = new Grid {
                RowSpacing = 8,
                ColumnSpacing = 8,
                Visible = false
            };

            grid.Widgets.Add(new Label {
                Text = "IsPlayer",
                GridRow = 0,
                GridColumn = 0,
            });
            isPlayerCheckBox = new CheckBox {
                GridRow = 0,
                GridColumn = 1
            };
            isPlayerCheckBox.Click += (sender, args) => {
                if (sender is CheckBox checkBox)
                    TogglePlayer(checkBox.IsPressed);
                skipNextMouseClicked = true;
            };
            grid.Widgets.Add(isPlayerCheckBox);

            grid.Widgets.Add(new Label {
                Text = "IsPhysical",
                GridRow = 1,
                GridColumn = 0,
            });
            isPhysicalCheckBox = new CheckBox {
                GridRow = 1,
                GridColumn = 1
            };
            isPhysicalCheckBox.Click += (sender, args) => {
                if (sender is CheckBox checkBox)
                    TogglePhysical(checkBox.IsPressed);
                skipNextMouseClicked = true;
            };
            grid.Widgets.Add(isPhysicalCheckBox);

            stack.Widgets.Add(grid);

            Desktop.Root = stack;
        }

        private void TogglePhysical(bool isPhysical)
        {
            if (sharedState.SelectedEntity != null) {
                var entity = GetEntity(sharedState.SelectedEntity.Value);
                if (entity != null) {
                    if (entity.Has<Physical>() && !isPhysical) {
                        entity.Detach<Physical>();
                    } else if (!entity.Has<Physical>() && isPhysical) {
                        entity.Attach(new Physical());
                    }
                }
            }
        }

        private void TogglePlayer(bool isPlayer)
        {
            if (sharedState.SelectedEntity != null) {
                var entity = GetEntity(sharedState.SelectedEntity.Value);
                if (entity != null) {
                    if (entity.Has<Player>() && !isPlayer) {
                        entity.Detach<Player>();
                    } else if (!entity.Has<Player>() && isPlayer) {
                        entity.Attach(new Player());
                    }
                }
            }
        }

        private void OnMouseClicked(object? sender, MouseEventArgs e)
        {
            if (skipNextMouseClicked) {
                skipNextMouseClicked = false;
                return;
            }

            sharedState.SelectedEntity = ActiveEntities.Cast<int?>()
                .FirstOrDefault(entity => IsEntityClicked(entity!.Value, e.Position));
        }

        private bool IsEntityClicked(int id, Point coords)
        {
            var sprite = infoMapper.Get(id).Sprite == SpriteType.Anki ? sprites.Anki : sprites.Bronch;
            var transform = transformMapper.Get(id);
            var boundingBox = sprite.GetBoundingRectangle(transform);
            if (boundingBox.Contains(coords)) {
                var texture = sprite.TextureRegion.Texture;
                var colors = colorsByTexture.GetOrAdd(texture, t => {
                    var array = new Color[t.Width * t.Height];
                    t.GetData(array);
                    return array;
                });
                var (x, y) = Matrix2.Invert(transform.WorldMatrix)
                    .Transform(coords.ToVector2() + sprite.Origin);
                var index = (int) x + (int) y * texture.Width;
                if (index < colors.Length) {
                    var color = colors[index];
                    return color != Color.Transparent;
                }
            }

            return false;
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            transformMapper = mapperService.GetMapper<Transform2>();
            playerMapper = mapperService.GetMapper<Player>();
            infoMapper = mapperService.GetMapper<EntityInfo>();
            physicalMapper = mapperService.GetMapper<Physical>();

            sharedState.SelectedEntityChanged += OnSelectedEntityChanged;

            SetupGui();
        }

        private void OnSelectedEntityChanged(int? selectedentity)
        {
            if (selectedentity == null) {
                grid.Visible = false;
            } else {
                isPlayerCheckBox.IsPressed = playerMapper.Has(selectedentity.Value);
                isPhysicalCheckBox.IsPressed = physicalMapper.Has(selectedentity.Value);
                grid.Visible = true;
            }
        }
    }
}