namespace Tapxyc.GameData
{
    public abstract class BaseDataEditorCommand
    {
        public static bool CanSaveData = true;


        public abstract void BaseReloadData();

        public void Save()
        {
            if(CanSaveData )
                BaseOnSaveData();
        }

        protected abstract void BaseOnSaveData();


#if UNITY_EDITOR
        public abstract void Draw();
        public abstract void SetDefault();
#endif
    }
}