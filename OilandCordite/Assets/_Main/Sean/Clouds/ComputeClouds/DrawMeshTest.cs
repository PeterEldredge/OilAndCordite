using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawMeshTest : MonoBehaviour
{
    [SerializeField] Material _material;

    public Material material {
        get { return _material; }
    }

    [SerializeField] int _triangleCount = 100;

    public int triangleCount {
        get { return _triangleCount; }
        set { _triangleCount = value; }
    }

    [SerializeField] ComputeShader _compute;

    Mesh _mesh;
    ComputeBuffer _drawArgsBuffer;
    ComputeBuffer _positionBuffer;
    ComputeBuffer _normalBuffer;
    MaterialPropertyBlock _props;

    const int kThreadCount = 64;
    int ThreadGroupCount { get { return _triangleCount / kThreadCount; } }
    int TriangleCount { get { return kThreadCount * ThreadGroupCount; } }


    void OnValidate()
    {
        _triangleCount = Mathf.Max(kThreadCount, _triangleCount);
    }

    void Start()
    {
        // Mesh with single triangle.
        _mesh = new Mesh();
        _mesh.vertices = new Vector3 [3];
        _mesh.SetIndices(new [] {0, 1, 2}, MeshTopology.Triangles, 0);
        _mesh.UploadMeshData(true);

        // Allocate the indirect draw args buffer.
        _drawArgsBuffer = new ComputeBuffer(
            1, 5 * sizeof(uint), ComputeBufferType.IndirectArguments
        );

        // This property block is used only for avoiding a bug (issue #913828)
        _props = new MaterialPropertyBlock();
        _props.SetFloat("_UniqueID", Random.value);

        // Clone the given material before using.
        _material = new Material(_material);
        _material.name += " (cloned)";
    }

    void Update()
    {
        // Allocate/Reallocate the compute buffers when it hasn't been
        // initialized or the triangle count was changed from the last frame.
        if (_positionBuffer == null || _positionBuffer.count != TriangleCount * 3)
        {
            if (_positionBuffer != null) _positionBuffer.Release();
            if (_normalBuffer != null) _normalBuffer.Release();

            _positionBuffer = new ComputeBuffer(TriangleCount * 3, 16);
            _normalBuffer = new ComputeBuffer(TriangleCount * 3, 16);

            _drawArgsBuffer.SetData(new uint[5] {3, (uint)TriangleCount, 0, 0, 0});
        }

        // Invoke the update compute kernel.
        var kernel = _compute.FindKernel("CsClouds");

        _compute.SetFloat("Time", Time.time);
        _compute.SetBuffer(kernel, "PositionBuffer", _positionBuffer);
        _compute.SetBuffer(kernel, "NormalBuffer", _normalBuffer);

        _compute.Dispatch(kernel, ThreadGroupCount, 1, 1);

        // Draw the mesh with instancing.
        _material.SetMatrix("_LocalToWorld", transform.localToWorldMatrix);
        _material.SetMatrix("_WorldToLocal", transform.worldToLocalMatrix);

        _material.SetBuffer("_PositionBuffer", _positionBuffer);
        _material.SetBuffer("_NormalBuffer", _normalBuffer);

        Graphics.DrawMeshInstancedIndirect(
            _mesh, 0, _material,
            new Bounds(transform.position, transform.lossyScale * 5),
            _drawArgsBuffer, 0, _props
        );
    }

    void OnDestroy()
    {
        Destroy(_mesh);
        _drawArgsBuffer.Release();
        if (_positionBuffer != null) _positionBuffer.Release();
        if (_normalBuffer != null) _normalBuffer.Release();
        Destroy(_material);
    }
}
