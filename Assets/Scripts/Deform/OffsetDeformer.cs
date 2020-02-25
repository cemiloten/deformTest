using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace Deform
{
    [Deformer (Name = "Offset", Type = typeof(OffsetDeformer))]
    public class OffsetDeformer : Deformer
    {
        public Vector3 offset;

        public override DataFlags DataFlags => DataFlags.Vertices;

        public override JobHandle Process(MeshData data, JobHandle dependency = default)
        {
            return new OffsetJob()
            {
                offset = offset,
                vertices = data.DynamicNative.VertexBuffer
            }.Schedule(data.Length, 64, dependency);
        }

        [BurstCompile]
        private struct OffsetJob : IJobParallelFor
        {
            public float3 offset;
            public NativeArray<float3> vertices;

            public void Execute(int index)
            {
                vertices[index] += offset;
            }
        }
    }
}
