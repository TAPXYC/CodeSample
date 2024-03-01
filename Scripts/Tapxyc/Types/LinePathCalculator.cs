namespace Tapxyc.Types
{
    using UnityEngine;

    public class LinePathCalculator
    {
        public readonly float Length;

        private Vector3[] path;




        public LinePathCalculator(Vector3[] path)
        {
            this.path = path;

            Length = 0;

            for (int i = 1; i < path.Length; i++)
                Length += (path[i] - path[i - 1]).magnitude;
        }





        public Vector3 GetPointAtDistance(float distance, out Vector3 forwardDirection)
        {
            Vector3 result = Vector3.zero;
            forwardDirection = Vector3.forward;

            distance = Mathf.Clamp(distance, 0, Length);

            float currentDistance = 0;


            for (int i = 1; i < path.Length; i++)
            {
                float segmentLenght = (path[i] - path[i - 1]).magnitude;

                currentDistance += segmentLenght;

                if (distance < currentDistance)
                {
                    float t = 1 - (currentDistance - distance) / segmentLenght;

                    result = Vector3.Lerp(path[i - 1], path[i], t);
                    forwardDirection = (path[i - 1] - path[i]).normalized;

                    break;
                }
            }

            return result;
        }


        public Vector3 GetPointAtDistanceX(float distance, out Vector3 forwardDirection)
        {
            Vector3 result = Vector3.zero;
            forwardDirection = Vector3.forward;

            distance = Mathf.Clamp(distance, 0, Length);

            float currentDistance = 0;

            for (int i = 1; i < path.Length; i++)
            {
                float segmentLenght = path[i].x - path[i - 1].x;

                currentDistance += segmentLenght;

                if (distance < currentDistance)
                {
                    float t = 1 - (currentDistance - distance) / segmentLenght;

                    result = Vector3.Lerp(path[i - 1], path[i], t);
                    forwardDirection = (path[i - 1] - path[i]).normalized;

                    break;
                }
            }

            return result;
        }
    }
}