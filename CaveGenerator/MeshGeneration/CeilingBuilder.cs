﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CaveGeneration.MeshGeneration
{
    /// <summary>
    /// Responsible for generating all the data related to the ceiling / base of the map, namely its outline and all
    /// data required to construct the mesh. 
    /// </summary>
    class CeilingBuilder
    {
        Map map;
        IDictionary<int, List<Triangle>> triangleMap;

        public MeshData mesh { get; private set; }
        public List<Outline> outlines { get; private set; }

        public CeilingBuilder(Map map)
        {
            this.map = map;
        }

        /// <summary>
        /// Generates the data for the ceiling mesh, along with a table associating vertices (by index) to 
        /// the triangles containing them. 
        /// </summary>
        public void Build()
        {
            TriangulateMap();
            ComputeCeilingUVArray();
            ComputeMeshOutlines();
        }

        void TriangulateMap()
        {
            MapTriangulator mapTriangulator = new MapTriangulator(map);
            mapTriangulator.Triangulate();

            mesh = new MeshData();
            mesh.vertices = mapTriangulator.meshVertices;
            mesh.triangles = mapTriangulator.meshTriangles;

            triangleMap = mapTriangulator.vertexIndexToTriangles;
        }

        void ComputeCeilingUVArray()
        {
            Vector3[] vertices = mesh.vertices;
            Vector2[] uv = new Vector2[vertices.Length];
            float xMax = Map.maxSubmapSize;
            float yMax = Map.maxSubmapSize;
            for (int i = 0; i < vertices.Length; i++)
            {
                float percentX = vertices[i].x / xMax;
                float percentY = vertices[i].z / yMax;
                uv[i] = new Vector2(percentX, percentY);
            }
            mesh.uv = uv;
        }

        void ComputeMeshOutlines()
        {
            OutlineGenerator outlineGenerator = new OutlineGenerator(mesh.vertices, triangleMap);
            outlines = outlineGenerator.GenerateOutlines();
        }
    } 
}
