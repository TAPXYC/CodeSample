namespace Tapxyc.Types
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

#if UNITY_EDITOR
    using UnityEditor;
    using Tapxyc.Drawler;
#endif


    internal enum PositionType
    {
        FromArea,
        FromRadius,
        FromPositions
    }


    internal enum LookType
    {
        ToPoint,
        ToDirection,
        ToSelf
    }



    public class MoveTargetPoint
    {
        /// <summary>
        /// Куда идти
        /// </summary>
        public readonly Vector3 MovePosition;


        /// <summary>
        /// Куда смотреть как дойдешь
        /// </summary>
        public readonly Vector3 LookTarget;

        public MoveTargetPoint(Vector3 movePosition, Vector3 lookTarget)
        {
            MovePosition = movePosition;
            LookTarget = lookTarget;
        }
    }





    [Serializable]
    public class RandomPositionGetter
    {
        private class PositionHandler
        {
            public readonly int Index;
            public readonly Vector3 Position;


            public PositionHandler(int index, Vector3 position)
            {
                Index = index;
                Position = position;
            }
        }




        [SerializeField] Transform centerTransform;
        [SerializeField] PositionType positionType;
        [SerializeField] Transform[] positions;
        [SerializeField] Vector2 area;
        [SerializeField] Vector2 offset;
        [SerializeField] int amountCol = 1;
        [SerializeField] int amountRow = 1;
        [SerializeField] float radius = 1;
        [SerializeField] int amountToSpawn = 16;
        [SerializeField] int remembersPositions = 5;
        [SerializeField] LookType lookType = LookType.ToSelf;
        [SerializeField] Transform lookTargetTransform;
        [SerializeField, Range(0, 1)] float lookDirection;
        [SerializeField] bool inverseLookDirection = false;


        public Transform Center => centerTransform;
        public Transform LookTarget => lookTargetTransform;


        private PositionHandler[] _positions;
        private List<PositionHandler> _lastPositions = new List<PositionHandler>();
        private bool _inited = false;



        public void Init()
        {
            _lastPositions.Clear();
            _positions = CreatePositions();

            _inited = true;
        }


        public MoveTargetPoint GetRandomPosition()
        {
            if (!_inited)
                Init();

            var workedList = _positions.Where(p => !_lastPositions.Contains(p))
                                    .ToArray();

            int randomIndex = UnityEngine.Random.Range(0, workedList.Length);
            PositionHandler selectedPosition = workedList[randomIndex];

            _lastPositions.Add(selectedPosition);

            while (_lastPositions.Count > remembersPositions)
                _lastPositions.RemoveAt(0);

            return new MoveTargetPoint(selectedPosition.Position, GetLookPoint(selectedPosition));
        }



        public void ClearPositionsHandled()
        {
            _lastPositions.Clear();
        }



        /// <summary>
        /// Если нужна отрисовка - вызвать этот метод в OnDrawGizmos
        /// </summary>
        public void DrawGizmos(Color? positionColor = null)
        {
#if UNITY_EDITOR
            if (centerTransform != null)
            {
                if (positionColor == null)
                    positionColor = Color.green;

                PositionHandler[] positions = Application.isPlaying ? _positions : CreatePositions();

                if (!positions.IsNullOrEmpty())
                {
                    foreach (var position in positions)
                    {
                        Gizmos.color = _lastPositions.Contains(position) ? Color.yellow : positionColor.Value;
                        Gizmos.DrawWireSphere(position.Position, 0.5f);
                        Gizmos.DrawRay(position.Position, GetLookPoint(position) - position.Position);
                    }
                }
            }
#endif
        }




        private PositionHandler[] CreatePositions()
        {
            PositionHandler[] positions = null;

            switch (positionType)
            {
                case PositionType.FromArea:
                    int index = 0;

                    positions = Enumerable.Range(0, amountCol)
                                              .Select(i => Enumerable.Range(0, amountRow).Select(j =>
                                              {
                                                  float posX = amountCol - 1 == 0 ? area.x / 2 : area.x / (amountCol - 1) * i;
                                                  float posZ = amountRow - 1 == 0 ? area.y / 2 : area.y / (amountRow - 1) * j;

                                                  Vector3 relativePosition = new Vector3(posX, 0, posZ);
                                                  relativePosition -= new Vector3(area.x / 2, 0, area.y / 2);

                                                  return new PositionHandler(index++, GetCenter() + relativePosition);
                                              }))
                                              .SelectMany(p => p)
                                              .ToArray();

                    break;

                case PositionType.FromRadius:
                    positions = Enumerable.Range(0, amountToSpawn)
                                                            .Select(i =>
                                                            {
                                                                float angle = i * Mathf.PI * 2f / amountToSpawn;
                                                                return new PositionHandler(i, GetCenter() + new Vector3(Mathf.Cos(angle) * radius, 0, Mathf.Sin(angle) * radius));
                                                            })
                                                            .ToArray();

                    break;

                case PositionType.FromPositions:
                    positions = this.positions.ToList().RandomizeList().Select((p, i) => new PositionHandler(i, p.position)).ToArray();
                    break;

            }

            return positions;
        }


        private Vector3 GetCenter()
        {
            return centerTransform.position + new Vector3(offset.x, 0, offset.y);
        }


        private Vector3 GetLookPoint(PositionHandler selectedPosition)
        {
            float angle = lookDirection * Mathf.PI * 2f;
            Vector3 lookPoint = Vector3.zero;

            if (lookType == LookType.ToPoint)
                lookPoint = lookTargetTransform.position;

            if (lookType == LookType.ToDirection)
                lookPoint = selectedPosition.Position + new Vector3(Mathf.Cos(angle) * 1, 0, Mathf.Sin(angle) * 1);

            if (lookType == LookType.ToSelf)
                lookPoint = selectedPosition.Position;

            return lookPoint;
        }
    }


















#if UNITY_EDITOR

    [CustomPropertyDrawer(typeof(RandomPositionGetter))]
    public class RoundedPositionGetterDrawler : BaseModuleDrawler
    {
        private SerializedProperty _positionTypeProp;
        private SerializedProperty _offsetProp;
        private SerializedProperty _positionsProp;
        private SerializedProperty _areaProp;
        private SerializedProperty _amountColProp;
        private SerializedProperty _amountRowProp;
        private SerializedProperty _radiusProp;
        private SerializedProperty _amountToSpawnProp;
        private SerializedProperty _remembersPositionsProp;
        private SerializedProperty _centerTransformProp;

        private SerializedProperty _lookTypeProp;
        private SerializedProperty _lookTargetTransformProp;
        private SerializedProperty _lookDirectionProp;
        private SerializedProperty _inverseLookDirectionnProp;




        protected override void OnCreate(out string title, out int titleFontSize, out Color titleColor, out FontStyle titleFontStyle, out GUIStyle backgroundStyle, out Color? impotantFieldColor)
        {
            titleFontSize = 16;
            titleColor = Color.yellow;
            title = "position getter";
            titleFontStyle = FontStyle.Bold;
            backgroundStyle = null;
            impotantFieldColor = null;
        }


        protected override void OnInitDrawler(SerializedProperty property)
        {
            _offsetProp = property.FindPropertyRelative("offset");
            _positionTypeProp = property.FindPropertyRelative("positionType");

            _positionsProp = property.FindPropertyRelative("positions");

            _areaProp = property.FindPropertyRelative("area");
            _amountColProp = property.FindPropertyRelative("amountCol");
            _amountRowProp = property.FindPropertyRelative("amountRow");

            _radiusProp = property.FindPropertyRelative("radius");

            _amountToSpawnProp = property.FindPropertyRelative("amountToSpawn");
            _remembersPositionsProp = property.FindPropertyRelative("remembersPositions");

            _centerTransformProp = property.FindPropertyRelative("centerTransform");

            _lookTypeProp = property.FindPropertyRelative("lookType");
            _lookTargetTransformProp = property.FindPropertyRelative("lookTargetTransform");
            _lookDirectionProp = property.FindPropertyRelative("lookDirection");
            _inverseLookDirectionnProp = property.FindPropertyRelative("inverseLookDirection");
        }



        protected override void OnDrawBody(Rect rect, SerializedProperty property)
        {
            var positionType = (PositionType)_positionTypeProp.enumValueIndex;

            _amountColProp.ClampMin(1);
            _amountRowProp.ClampMin(1);

            _radiusProp.ClampMin(0f);

            _remembersPositionsProp.ClampMin(0);

            int maxIndex = positionType == PositionType.FromArea ? _amountColProp.intValue * _amountRowProp.intValue :
                                        positionType == PositionType.FromRadius ? _amountToSpawnProp.intValue :
                                                                                _positionsProp.arraySize;

            maxIndex = Mathf.Clamp(maxIndex - 1, 0, maxIndex);

            _remembersPositionsProp.ClampMax(maxIndex);

            if (_centerTransformProp.GetValue() == null)
                _centerTransformProp.objectReferenceValue = owner.transform;

            if (_lookTargetTransformProp.GetValue() == null)
                _lookTargetTransformProp.objectReferenceValue = owner.transform;

            DrawInspector(property.displayName, positionType);
        }



        private void DrawInspector(string propName, PositionType positionType)
        {
            BeginVerticalGroup(EditorStyles.helpBox);

            PrefixLabel("Place setting", subTittle);
            BeginVerticalGroup(EditorStyles.helpBox);
            Property(_centerTransformProp, isImportant: true);
            Space();

            BeginVerticalGroup(EditorStyles.helpBox);

            PrefixLabel("Distribution type", subTittle);
            Property(_positionTypeProp);

            Space();
            Line();
            Space();

            PrefixLabel(positionType.ToString(), subTittle);

            if (positionType == PositionType.FromArea)
                Property(_areaProp);
            if (positionType == PositionType.FromRadius)
                Property(_radiusProp);
            if (positionType == PositionType.FromPositions)
                Property(_positionsProp);

            if (positionType != PositionType.FromPositions)
            {
                Space();
                Property(_offsetProp);
            }

            EndVerticalGroup();

            Space();
            Line();
            Space();

            if (positionType == PositionType.FromArea)
            {
                Property(_amountColProp);
                Property(_amountRowProp);
            }
            if (positionType == PositionType.FromRadius)
                Property(_amountToSpawnProp);

            if (positionType != PositionType.FromPositions)
            {
                Space();
                Line();
                Space();
            }

            Property(_remembersPositionsProp);
            EndVerticalGroup();

            Space();
            Space();

            PrefixLabel("Look setting", subTittle);
            BeginVerticalGroup(EditorStyles.helpBox);

            var lookType = (LookType)_lookTypeProp.enumValueIndex;

            Property(_lookTypeProp);

            if (lookType != LookType.ToSelf)
            {
                Space();
                Line();
                Space();

                if (lookType == LookType.ToPoint)
                    Property(_lookTargetTransformProp, isImportant: true);
                else
                    Property(_lookDirectionProp);
            }

            EndVerticalGroup();
            EndVerticalGroup();
        }

    }

#endif
}
