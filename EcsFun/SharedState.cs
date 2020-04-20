namespace EcsFun
{
    public class SharedState
    {
        private int? selectedEntity;
        public event CreateEntity? CreateEntity;

        public int? SelectedEntity {
            get => selectedEntity;
            set {
                if (value == selectedEntity) return;
                selectedEntity = value;
                SelectedEntityChanged?.Invoke(value);
            }
        }

        public event SelectedEntityChanged? SelectedEntityChanged;

        public virtual void OnCreateEntity(float x, float y)
        {
            CreateEntity?.Invoke(x, y);
        }
    }

    public delegate void CreateEntity(float x, float y);

    public delegate void SelectedEntityChanged(int? selectedEntity);
}