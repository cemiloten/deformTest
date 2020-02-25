using System.Collections.Generic;
using UnityEngine;

public class VertexRelax : MonoBehaviour
{
    [SerializeField] private float multiplier = 0.25f;

    private Mesh _mesh;

    private List<int>[] _vertexWithNeighbours;
    private Vector3[] _vertices;

    public void Initialize(Mesh mesh)
    {
        // // Copy mesh in RAM and use this one.
        // var mf = GetComponent<MeshFilter>();
        // _mesh = Instantiate(mf.mesh);
        _vertices = mesh.vertices;
        // mf.mesh = _mesh;
        //
        // GetAllNeighbours();
    }

    private void GetAllNeighbours(Mesh mesh)
    {
        int[] triangles = _mesh.triangles;
        _vertexWithNeighbours = new List<int>[_vertices.Length];

        // For each vertex in each triangle, populate the vertex's list with its neighbours.
        for (int i = 0; i < triangles.Length; i += 3)
        {
            // Vertex indices for each point of the triangle.
            int v0 = triangles[i];
            int v1 = triangles[i + 1];
            int v2 = triangles[i + 2];

            AddNeighboursToVertexList(vertex: v0, n1: v1, n2: v2);
            AddNeighboursToVertexList(vertex: v1, n1: v0, n2: v2);
            AddNeighboursToVertexList(vertex: v2, n1: v0, n2: v1);
        }
    }

    private void AddNeighboursToVertexList(int vertex, int n1, int n2)
    {
        List<int> neighbours = _vertexWithNeighbours[vertex];

        if (neighbours == null)
        {
            _vertexWithNeighbours[vertex] = new List<int> { n1, n2 };
        }
        else if (neighbours.Count == 0)
        {
            neighbours.Add(n1);
            neighbours.Add(n2);
        }
        else
        {
            if (!neighbours.Contains(n1))
                neighbours.Add(n1);

            if (!neighbours.Contains(n2))
                neighbours.Add(n2);
        }
    }

    public void RelaxVertices()
    {
        for (int i = 0; i < _vertexWithNeighbours.Length; ++i)
        {
            Vector3 vertexPosition = _vertices[i];
            Vector3 average = Vector3.zero;

            List<int> neighbours = _vertexWithNeighbours[i];
            foreach (int neighbourIndex in neighbours)
            {
                Vector3 neighbourPosition = _mesh.vertices[neighbourIndex];
                Vector3 vertexToNeighbour = neighbourPosition - vertexPosition;
                average += vertexToNeighbour;
            }

            average /= (float)neighbours.Count;
            Vector3 newPosition = vertexPosition + average * multiplier;

            _vertices[i] = newPosition;

            // Find duplicates and apply same change to them.
            List<int> duplicates = FindDuplicates(i, vertexPosition);
            foreach (int duplicateIndex in duplicates)
                _vertices[duplicateIndex] = newPosition;
        }

        _mesh.vertices = _vertices;
        _mesh.RecalculateBounds();
    }

    private List<int> FindDuplicates(int targetIndex, Vector3 targetPosition)
    {
        var duplicates = new List<int>();
        for (int v = 0; v < _vertices.Length; v++)
        {
            if (v != targetIndex && _vertices[v] == targetPosition)
                duplicates.Add(v);
        }
        return duplicates;
    }
}