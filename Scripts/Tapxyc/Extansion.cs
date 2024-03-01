using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using DG.Tweening;
using System.Text;

#if UNITY_EDITOR
using UnityEditor;
#endif


public static class Extansions
{
    public static void SetLeft(this RectTransform rt, float left)
    {
        rt.offsetMin = new Vector2(left, rt.offsetMin.y);
    }

    public static void SetRight(this RectTransform rt, float right)
    {
        rt.offsetMax = new Vector2(-right, rt.offsetMax.y);
    }

    public static void SetTop(this RectTransform rt, float top)
    {
        rt.offsetMax = new Vector2(rt.offsetMax.x, -top);
    }

    public static void SetBottom(this RectTransform rt, float bottom)
    {
        rt.offsetMin = new Vector2(rt.offsetMin.x, bottom);
    }


    public static void SafeKill(this Tween tween)
    {
        if (tween != null)
            tween.Kill();
    }




    public static List<T> RandomizeList<T>(this List<T> list)
    {
        List<T> workedList = new List<T>(list);
        var helperList = new List<T>(list);

        while (helperList.Count > 0)
        {
            T randomElement = helperList.GetRandonElement();

            workedList.Add(randomElement);
            helperList.Remove(randomElement);
        }

        return workedList;
    }




    public static IEnumerable<TSource> OrderByPart<TSource>(this IEnumerable<TSource> source, Func<TSource, float> keySelector, float part)
    {
        var len = source.Count();

        List<TSource> listSource = source.ToList();

        for (int i = 1; i < len; i++)
        {
            for (int j = 0; j < len - i; j++)
            {
                float left = keySelector(listSource[j]);
                float right = keySelector(listSource[j + 1]);

                bool isNeedSwap = false;

                if (left.IsEqZero())
                    isNeedSwap = true;
                else if (right / left >= part)
                    isNeedSwap = true;

                if (isNeedSwap)
                {
                    TSource temp = listSource[j];
                    listSource[j] = listSource[j + 1];
                    listSource[j + 1] = temp;
                }
            }
        }

        return listSource as IEnumerable<TSource>;
    }






    #region bool

    /// <summary>
    /// Проверяет равняется ли значение нулю с учётом погрешности.
    /// </summary>
    /// <param name="_eps">Предельно допустимая погрешность</param>
    public static bool IsEqZero(this float thisNumber, float _eps = 1e-6f) => Mathf.Abs(thisNumber) <= _eps;



    /// <summary>
    /// Проверяет равенство двух числе с учетом погрешности.
    /// </summary>
    /// <param name="_eps">Предельно допустимая погрешность</param>
    public static bool IsEqFloat(this float thisNumber, float number, float _eps = 1e-6f) => Mathf.Abs(thisNumber - number) <= _eps;



    /// <summary>
    /// Проверяет коллецию на null или пустоту.
    /// </summary>
    public static bool IsNullOrEmpty<T>(this IEnumerable<T> collection)
    {
        if (collection == null) return true;

        return !collection.Any();
    }



    /// <summary>
    /// Сравнение значений векторов с учётом предельно допустимой погрешности.
    /// </summary>
    /// <param name="otherVector">Вектор для сравнения.</param>
    /// <param name="_eps">Предельно допустимая погрешность.</param>
    public static bool CompareVector3(this Vector3 vector, Vector3 otherVector, float _eps = 1e-6f) => vector.x.IsEqFloat(otherVector.x, _eps) && vector.y.IsEqFloat(otherVector.y, _eps) && vector.z.IsEqFloat(otherVector.z, _eps);

    #endregion




    #region int

    public static T IndexOrLast<T>(this T[] array, int index)
    {
        return array[Mathf.Min(index, array.Length - 1)];
    }

    #endregion


    #region Vector3


    /// <summary>
    /// Ограничивает значение вектора используя заданные мин и макс длины. Возвращает полученное значение с учётом ограничителей.
    /// </summary>
    /// <param name="min">Минимальная длинна вектора</param>
    /// <param name="max">Максимальная длинна вектора</param>
    public static Vector3 ClampMagnitude(this Vector3 vector, float min, float max)
    {
        //clamp max
        Vector3 result = Vector3.ClampMagnitude(vector, max);

        //clamp min
        if (result.magnitude < min)
            result = result.normalized * min;

        return result;
    }




    /// <summary>
    /// Ограничивает значение вектора между максильным и минимальным. Возвращает полученное значение с учётом ограничителей.
    /// </summary>
    /// <param name="min">Вектор минимально возможных значений</param>
    /// <param name="max">Вектор максимально возможных значений</param>
    public static Vector3 ClampVector(this Vector3 vector, Vector3 min, Vector3 max)
    {
        return new Vector3(
            Mathf.Clamp(vector.x, min.x, max.x),
            Mathf.Clamp(vector.y, min.y, max.y),
            Mathf.Clamp(vector.z, min.z, max.z)
            );
    }





    /// <summary>
    /// Возвращает случайную позицию типа Vector3 в допустимой области. Центром служит текущая позиция.
    /// </summary>
    /// <param name="size">Ветор размера допустимой области</param>
    public static Vector3 GetRandomPositionInside(this Vector3 position, Vector3 size)
    {
        float halfX = size.x / 2;
        float halfY = size.y / 2;
        float halfZ = size.z / 2;

        return new Vector3(UnityEngine.Random.Range(position.x - halfX, position.x + halfX), UnityEngine.Random.Range(position.y - halfY, position.y + halfY), UnityEngine.Random.Range(position.z - halfZ, position.z + halfZ));
    }




    /// <summary>
    /// Перевод текущего значения поворота по всем осям в градусах в стандартный промежуток от 0 до 360
    /// </summary>
    /// <param name="leftEdgeInterval">Минимальная точка отсчёта в градусах. По-умолчанию равна 0</param>
    public static Vector3 ToNormalRotation(this Vector3 rotation, float leftEgeIntervel = 0) => new Vector3(rotation.x.ToNormalCoordinates(leftEgeIntervel), rotation.y.ToNormalCoordinates(leftEgeIntervel), rotation.z.ToNormalCoordinates(leftEgeIntervel));






    public static Vector3 GetXZPosition(this Transform transform)
    {
        var position = transform.position;
        position.y = 0;
        return position;
    }


    #endregion



    #region float



    /// <summary>
    /// Проецирует исходный вектор относильного другого вектора. Возвращает конечную точку проекции
    /// </summary>
    /// <param name="projectedSpace">Вектор, вдоль которого происходит проекция</param>
    public static float ProjectDrag(this Vector3 vector, Vector3 projectedSpace)
    {
        Vector3 Project = Vector3.Project(vector, projectedSpace);
        return Project.x + Project.y + Project.z;
    }



    /// <summary>
    /// Проецирует исходный вектор относильного другого вектора. Возвращает конечную точку проекции
    /// </summary>
    /// <param name="projectedSpace">Вектор, вдоль которого происходит проекция</param>
    public static float ProjectDrag(this Vector2 vector, Vector2 projectedSpace)
    {
        Vector2 Project = Vector3.Project(vector, projectedSpace);
        return Project.x + Project.y;
    }



    /// <summary>
    /// Вычисляет размер в процентах от максимального. Возвращает значение от 0 до 1.
    /// </summary>
    /// <param name="minSize">Минимальный визуальный размер</param>
    /// <param name="maxSize">Максимальный визуальный размер</param>
    public static float PercentFromSize(this float currentVisualSize, float minSize, float maxSize) => (currentVisualSize - minSize) / (maxSize - minSize);



    /// <summary>
    /// Вычисляет визуальный размер от исходного размера в процентах.
    /// </summary>
    /// <param name="minSize">Минимальный визуальный размер</param>
    /// <param name="maxSize">Максимальный визуальный размер</param>
    public static float SizeFromPercent(this float percentCurrentSize, float minSize, float maxSize) => minSize + percentCurrentSize * (maxSize - minSize);



    /// <summary>
    /// Проецирует размеры одного объекта на другой. Возвращает исзменённый размер исходного объекта.
    /// </summary>
    /// <param name="minSelfSize">Минимальный визуальный размер исходного объекта</param>
    /// <param name="maxSelfSize">Максимальный визуальный размер исходного объекта</param>
    /// <param name="minOtherSize">Минимальный визуальный размер объекта для проекции</param>
    /// <param name="maxOtherSize">Максимальный визуальный размер объекта для проекции</param>
    public static float ProjectSize(this float size, float minSelfSize, float maxSelfSize, float minOtherSize, float maxOtherSize) => size.PercentFromSize(minSelfSize, maxSelfSize).SizeFromPercent(minOtherSize, maxOtherSize);



    /// <summary>
    /// Возвращает ограниченное значение числа относительно максимально допустимого значения.
    /// </summary>
    /// <param name="maxValue">Максимально допустимое значение.</param>
    public static float ClampMax(this float value, float maxValue) => value > maxValue ? maxValue : value;



    /// <summary>
    /// Возвращает ограниченное значение числа относительно минимально допустимого значения.
    /// </summary>
    /// <param name="maxValue">Минимально допустимое значение.</param>
    public static float ClampMin(this float value, float minValue) => value < minValue ? minValue : value;



    /// <summary>
    /// Перевод текущего значения поворота по одной из осей в градусах в стандартный промежуток от 0 до 360
    /// </summary>
    /// <param name="leftEdgeInterval">Минимальная точка отсчёта в градусах. По-умолчанию равна 0</param>
    private static float ToNormalCoordinates(this float coordinates, float leftEdgeInterval = 0)
    {
        float rightEdgeInterval = leftEdgeInterval + 360;

        if (coordinates < leftEdgeInterval)
            while (coordinates < leftEdgeInterval)
                coordinates += 360;
        else if (coordinates > rightEdgeInterval)
            while (coordinates > rightEdgeInterval)
                coordinates -= 360;

        return coordinates;
    }



    /// <summary>
    /// Вообще хрен его знает, что оно делает. Высчитывает какой-то коэффицент для плоскости, возвращает список координат x,y,z (Делал Евгений, мучайте его)
    /// </summary>
    private static List<float> coefPloskost(Vector3 point, Vector3 normVector)
    {
        List<float> res = new List<float>();
        res.Add(normVector.x);
        res.Add(normVector.y);
        res.Add(normVector.z);

        float D = -point.x * normVector.x - point.y * normVector.y - point.z * normVector.z;

        res.Add(D);


        return res;
    }



    /// <summary>
    /// Расстояние от исходной точки до точки на плоскости, заданное относительно плоскости и нормали (Делал Евгений, мучайте его)
    /// </summary>
    public static float Distance(this Vector3 point, Vector3 pointPloscost, Vector3 normVector)
    {
        List<float> ploskost = coefPloskost(pointPloscost, normVector);

        return Mathf.Abs(ploskost[0] * point.x + ploskost[1] * point.y + ploskost[2] * point.z + ploskost[3]) / Mathf.Sqrt(Mathf.Pow(ploskost[0], 2) + Mathf.Pow(ploskost[1], 2) + Mathf.Pow(ploskost[2], 2));
    }



    /// <summary>
    /// Возвращает разность двух значений по модулю.
    /// </summary>
    public static float DistanceTo(this float thisValue, float value) => Math.Abs(thisValue - value);



    #endregion



    #region void



    /// <summary>
    /// Возвращает ограниченное значение числа относительно минимально допустимого значения.
    /// </summary>
    /// <param name="maxValue">Минимально допустимое значение.</param>
    public static void SetDissolve(this Material material, bool isActive, float duration)
    {
        material.DOKill();

        material.DOFloat(isActive ? 0 : 1, "_Dissolve", duration)
            .SetEase(Ease.Linear);
    }



    /// <summary>
    /// Безопасное проигрывание системы частиц с предварительной проверкой на её наличие.
    /// </summary>
    public static void SafePlay(this ParticleSystem particleSystem)
    {
        if (particleSystem != null)
            particleSystem.Play();
    }



    #endregion



    #region  string



    /// <summary>
    /// Превращает число типа int в строковое представление числа. Разряды разделяются пробелом.
    /// </summary>
    public static string ToFormatString(this int number)
    {
        StringBuilder sb = new StringBuilder(number.ToString());

        for (int i = sb.Length; i > 0; i -= 3)
            sb.Insert(i, " ");

        return sb.ToString();
    }



    #endregion



    # region type<T>



    /// <summary>
    /// Проверяет наличие компонента в данном объекте. В случае отсутствие добавляет его. Вовращает выбранный компонент.
    /// </summary>
    public static T CheckAndAddComponent<T>(this GameObject baseObject) where T : Component
    {
        T checkingComponent = baseObject.GetComponent<T>();

        if (checkingComponent == null)
            checkingComponent = baseObject.AddComponent<T>();

        return checkingComponent;
    }



    #endregion



    #region Animation


    /// <summary>
    /// Безопасное проигрывание анимации с предварительной проверкой на её наличие.
    /// </summary>
    public static void SafeAnimationPlay(this Animation animation)
    {
        if (animation != null)
            animation.Play();
    }



    /// <summary>
    /// Безопасное проигрывание анимации с предварительной проверкой на её наличие.
    /// </summary>
    public static void SafeAnimationPlay(this Animation animation, string animationName)
    {
        if (animation != null)
            animation.Play(animationName);
    }


    #endregion



    #region  Animator


    /// <summary>
    /// Безопасный триггер аниматора с предварительной проверкой на его наличие.
    /// </summary>
    /// <param name="triggerParameter">Имя активируемого компонента</param>
    public static void SafeAnimatorTrigger(this Animator animatior, string triggerParameter)
    {
        if (animatior != null)
            animatior.SetTrigger(triggerParameter);
    }

    public static void SafeAnimatorInt(this Animator animatior, string intName, int value)
    {
        if (animatior != null)
            animatior.SetInteger(intName, value);
    }

    public static void SafeAnimatorFloat(this Animator animatior, string floatName, float value)
    {
        if (animatior != null)
            animatior.SetFloat(floatName, value);
    }



    public static void SafeAnimatorBool(this Animator animatior, string boolName, bool value)
    {
        if (animatior != null)
            animatior.SetBool(boolName, value);
    }
    #endregion



    #region  Mesh 

    private static float SignedVolumeOfTriangle(Vector3 p1, Vector3 p2, Vector3 p3)
    {
        float v321 = p3.x * p2.y * p1.z;
        float v231 = p2.x * p3.y * p1.z;
        float v312 = p3.x * p1.y * p2.z;
        float v132 = p1.x * p3.y * p2.z;
        float v213 = p2.x * p1.y * p3.z;
        float v123 = p1.x * p2.y * p3.z;

        return (1.0f / 6.0f) * (-v321 + v231 + v312 - v132 - v213 + v123);
    }


    /// <summary>
    /// Находит объём заданного меша
    /// </summary>
    public static float Volume(this Mesh mesh)
    {
        float volume = 0;

        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;

        for (int i = 0; i < triangles.Length; i += 3)
        {
            Vector3 p1 = vertices[triangles[i + 0]];
            Vector3 p2 = vertices[triangles[i + 1]];
            Vector3 p3 = vertices[triangles[i + 2]];
            volume += SignedVolumeOfTriangle(p1, p2, p3);
        }
        return Mathf.Abs(volume);
    }

    #endregion



    #region Collections

    public static T GetRandonElement<T>(this IEnumerable<T> collection)
    {
        int count = collection.Count();
        return collection.ElementAt(UnityEngine.Random.Range(0, count));
    }

    #endregion

#if UNITY_EDITOR



    /// Get string representation of serialized property
    /// <summary>
    /// Возвращает строковое предстваление сериализуемого свойства.
    /// </summary>
    public static string AsStringValue(this SerializedProperty property)
    {
        switch (property.propertyType)
        {
            case SerializedPropertyType.String:
                return property.stringValue;

            case SerializedPropertyType.Character:
            case SerializedPropertyType.Integer:
                if (property.type == "char") return System.Convert.ToChar(property.intValue).ToString();
                return property.intValue.ToString();

            case SerializedPropertyType.ObjectReference:
                return property.objectReferenceValue != null ? property.objectReferenceValue.ToString() : "null";

            case SerializedPropertyType.Boolean:
                return property.boolValue.ToString();

            case SerializedPropertyType.Enum:
                return property.GetValue().ToString();

            default:
                return string.Empty;
        }
    }



    #region Clamp float



    /// <summary>
    /// Ограничение максимального значения свойства типа float с учётом погрешности.
    /// </summary>
    /// <param name="clampProp">Свойство с максимальным допустимым значением</param>
    /// <param name="delta">Предельно допустимая погрешность</param>
    public static void ClampMax(this SerializedProperty self, SerializedProperty clampProp, float delta)
    {
        if (self.floatValue > clampProp.floatValue)
            self.floatValue = clampProp.floatValue - delta;

    }



    /// <summary>
    /// Ограничение минимального значения свойства типа float с учётом погрешности.
    /// </summary>
    /// <param name="clampProp">Свойство с минимальным допустимым значением</param>
    /// <param name="delta">Предельно допустимая погрешность</param>
    public static void ClampMin(this SerializedProperty self, SerializedProperty clampProp, float delta)
    {
        if (self.floatValue < clampProp.floatValue)
            self.floatValue = clampProp.floatValue + delta;
    }


    /// <summary>
    /// Ограничение максимального значения свойства типа float.
    /// </summary>
    /// <param name="max">Максимальное значение</param>

    public static void ClampMax(this SerializedProperty self, float max)
    {
        if (self.floatValue > max)
            self.floatValue = max;

    }




    /// <summary>
    /// Ограничение минимального значения свойства типа float.
    /// </summary>
    /// <param name="min">Минимальное значение</param>
    public static void ClampMin(this SerializedProperty self, float min)
    {
        if (self.floatValue < min)
            self.floatValue = min;
    }

    #endregion



    #region Clamp int


    /// <summary>
    /// Ограничение максимального значения свойства типа int с учётом погрешности.
    /// </summary>
    /// <param name="clampProp">Свойство с максимальным допустимым значением</param>
    /// <param name="delta">Предельно допустимая погрешность</param>
    public static void ClampMax(this SerializedProperty self, SerializedProperty clampProp, int delta)
    {
        if (self.intValue > clampProp.intValue)
            self.intValue = clampProp.intValue - delta;
    }


    /// <summary>
    /// Ограничение минимального значения свойства типа int с учётом погрешности.
    /// </summary>
    /// <param name="clampProp">Свойство с минимальным допустимым значением</param>
    /// <param name="delta">Предельно допустимая погрешность</param>
    public static void ClampMin(this SerializedProperty self, SerializedProperty clampProp, int delta)
    {
        if (self.intValue < clampProp.intValue)
            self.intValue = clampProp.intValue + delta;
    }




    /// <summary>
    /// Ограничение максимального значения свойства типа int.
    /// </summary>
    /// <param name="max">Максимальное значение</param>
    public static void ClampMax(this SerializedProperty self, int max)
    {
        if (self.intValue > max)
            self.intValue = max;

    }


    /// <summary>
    /// Ограничение минимального значения свойства типа int.
    /// </summary>
    /// <param name="min">Минимальное значение</param>
    public static void ClampMin(this SerializedProperty self, int min)
    {
        if (self.intValue < min)
            self.intValue = min;
    }

    #endregion



    /// Get raw object value out of the SerializedProperty
    /// <summary>
    /// Возвращает значение свойства из сериализованного свойства.
    /// </summary>
    public static object GetValue(this SerializedProperty property)
    {
        if (property == null) return null;

        object obj = property.serializedObject.targetObject;
        var elements = property.GetFixedPropertyPath().Split('.');

        foreach (var element in elements)
        {
            if (element.Contains("["))
            {
                var elementName = element.Substring(0, element.IndexOf("[", StringComparison.Ordinal));
                var index = Convert.ToInt32(element.Substring(element.IndexOf("[", StringComparison.Ordinal)).Replace("[", "").Replace("]", ""));
                obj = GetValueByArrayFieldName(obj, elementName, index);
            }
            else obj = GetValueByFieldName(obj, element);
        }
        return obj;


        object GetValueByArrayFieldName(object source, string name, int index)
        {
            if (!(GetValueByFieldName(source, name) is IEnumerable enumerable)) return null;
            var enumerator = enumerable.GetEnumerator();

            for (var i = 0; i <= index; i++) if (!enumerator.MoveNext()) return null;
            return enumerator.Current;
        }

        // Search "source" object for a field with "name" and get it's value
        object GetValueByFieldName(object source, string name)
        {
            if (source == null) return null;
            var type = source.GetType();

            while (type != null)
            {
                var fieldInfo = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                if (fieldInfo != null) return fieldInfo.GetValue(source);

                var propertyInfo = type.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (propertyInfo != null) return propertyInfo.GetValue(source, null);

                type = type.BaseType;
            }
            return null;
        }
    }




    // Property path for collection without ".Array.data[x]" in it
    /// <summary>
    /// Возвращает полный путь к свойству без ".Array.data[x]"
    /// </summary>
    public static string GetFixedPropertyPath(this SerializedProperty property) => property.propertyPath.Replace(".Array.data[", "[");

#endif
}

