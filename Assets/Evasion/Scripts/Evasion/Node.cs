using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Evasion
{
    [ExecuteInEditMode]
    public class Node : MonoBehaviour
    {
        public Node[] adjacent;
        [SerializeField] private LineRenderer adjacentLine;

        private void Update()
        {
            if (!adjacent.Any()) return;

            var lines = new List<Vector3>();
            foreach (var n in adjacent)
            {
                if (n == null || !n.gameObject.activeInHierarchy) continue;
                lines.Add(transform.position);
                lines.Add(n.gameObject.transform.position);
            }

            adjacentLine.SetPositions(lines.ToArray());
            adjacentLine.positionCount = lines.Count;
        }
    }
}