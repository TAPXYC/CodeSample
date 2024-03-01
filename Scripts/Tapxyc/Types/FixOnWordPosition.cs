namespace Tapxyc.Types
{
    using UnityEngine;
    using System;

    [Serializable]
    public class TransformPositionHandler
    {
        [SerializeField] Transform fixedObject;
        [SerializeField] bool fix;

        private Vector3? wordPosition;

        public void Execute()
        {
            if (fix)
            {
                if (wordPosition == null)
                    wordPosition = fixedObject.position;
                
                fixedObject.position = wordPosition.Value;
            }
            else
            {
                wordPosition = null;
            }
        }
    }


    [ExecuteAlways]
    public class FixOnWordPosition : MonoBehaviour
    {
        [SerializeField] TransformPositionHandler[] fixedObjects;

        void Update()
        {
            foreach (var fo in fixedObjects)
            {
                fo.Execute();
            }
        }
    }
}
