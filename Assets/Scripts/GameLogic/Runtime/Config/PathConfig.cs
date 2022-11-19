using UnityEngine;
using UnityEngine.Assertions;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TowerDefense.GameLogic.Runtime.Configs
{
    [CreateAssetMenu(menuName = "Tower Defense/Path", fileName = "New Path")]
    public class PathConfig : ScriptableObject
    {
        public Vector3[] Waypoints => points;

        [SerializeField] private Vector3[] points;

#if UNITY_EDITOR

        public Vector3[] NodePositions => nodePositions;
        public Vector3[] HandlePositions => handlePositions;
        public int[] Subdivisions => subdivisions;

        [SerializeField] private Vector3[] nodePositions;
        [SerializeField] private Vector3[] handlePositions;
        [SerializeField] private int[] subdivisions;

#endif

        public void Save(Vector3[] points, Transform[] nodes, int[] subdivisions)
        {
            Assert.IsTrue(points != null && points.Length >= 2, "Need at least 2 points to create a path!");

            this.points = points;

            nodePositions = new Vector3[nodes.Length];
            handlePositions = new Vector3[nodes.Length];

            for (int i = 0; i < nodes.Length; i++)
            {
                nodePositions[i] = nodes[i].position;
                handlePositions[i] = nodes[i].GetChild(0).position;
            }

            this.subdivisions = subdivisions;
        }

#if UNITY_EDITOR

        public void DebugDraw()
        {
            if (points != null && points.Length > 1)
            {
                Handles.color = Color.cyan;

                for (int i = 0; i < points.Length; i++)
                {
                    if (i < points.Length - 1)
                    {
                        Handles.DrawLine(points[i], points[i + 1]);
                    }

                    Handles.DrawWireCube(points[i], Vector3.one * .25f);
                }
            }
        }

#endif
    }
}