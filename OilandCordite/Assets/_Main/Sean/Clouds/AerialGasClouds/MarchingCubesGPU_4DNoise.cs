using UnityEngine;
using UnityEngine.Rendering;
using System.Collections;
using System.Collections.Generic;
using ImprovedPerlinNoiseProject;

public class MarchingCubesGPU_4DNoise : MonoBehaviour
{
    #region Volume Samples and Buffer Size (Edit with caution)
    
    /*
    * N is the n dimension for both the generative noise and marching cubes hypercube
    * Increasing size results in expected performance hits, recommended value (40 - 72)
    * N NEEDS TO BE DIVISIBLE BY 8 
    */
    const int N = 40;

    // buffer size for resulting mesh
    const int SIZE = N * N * N * 3 * 5;

    #endregion

    #region Simulation and Generation Properties

    [Header("Simulation Properties")]
    public bool simulate = true;

    [Header("Control Point Properties")]
    [SerializeField] private int numberOfControlPoints;

    [SerializeField] private float distanceThreshold;

    [Header("Noise Properties")]
    [SerializeField] private float noiseSpeed = 2.0f;
    [SerializeField] private float noiseGain = 1.64f;
    [SerializeField] private float noiseFrequency = 0.04f;
    [SerializeField] private float noiseLacunarity = 1.5f;

    [Header("Materials")]
    [SerializeField] private Material material;
    [SerializeField] private Shader drawBufferShader;

    [Header("Compute Shaders")]
    [SerializeField] private ComputeShader perlinCompute;
    [SerializeField] private ComputeShader marchingCubes;
    [SerializeField] private ComputeShader normalCompute;
    [SerializeField] private ComputeShader bufferClearCompute;
    
    #endregion

    #region Private Variables

    Vector3[] ControlPoints;
    ComputeBuffer m_noiseBuffer, m_meshBuffer, m_controlPoints;
    ComputeBuffer m_cubeEdgeFlags, m_triangleConnectionTable;
    GPUPerlinNoise perlin;
    float time;
    Vector4 localOffset;
    Vector4 lossyScale;
    bool simulateOnStart = true;
    bool isInitialized = false;

    #endregion

    #region Public Methods

    public void UpdateBuffers() 
    {
        TryReleaseBuffers();
        m_noiseBuffer = new ComputeBuffer(N * N * N, sizeof(float));
        m_meshBuffer = new ComputeBuffer(SIZE, sizeof(float) * 7);
        m_controlPoints = new ComputeBuffer(numberOfControlPoints, sizeof(float) * 3);

        m_cubeEdgeFlags = new ComputeBuffer(256, sizeof(int));
        m_cubeEdgeFlags.SetData(MarchingCubesTables.CubeEdgeFlags);
        m_triangleConnectionTable = new ComputeBuffer(256 * 16, sizeof(int));
        m_triangleConnectionTable.SetData(MarchingCubesTables.TriangleConnectionTable);

        perlin = new GPUPerlinNoise(Random.Range(0, 32));
        perlin.LoadResourcesFor4DNoise();
    }

    public void SetControlPoints(Vector3[] points) 
    {
        this.ControlPoints = points;
    }

    public void FinishInitializing() 
    {
        this.isInitialized = true;
    }

    public void InitializeOnStart() 
    {
        if(simulateOnStart) 
        {
            simulateOnStart = false;
            localOffset = new Vector4(this.transform.position.x, this.transform.position.y, this.transform.position.z, 0.0f);
            lossyScale = new Vector4(this.transform.lossyScale.x, this.transform.lossyScale.y, this.transform.lossyScale.z, 0.0f);

            m_controlPoints.SetData(ControlPoints);

            bufferClearCompute.SetInt("_Width", N);
            bufferClearCompute.SetInt("_Height", N);
            bufferClearCompute.SetInt("_Depth", N);
            bufferClearCompute.SetBuffer(0, "_Buffer", m_meshBuffer);
            bufferClearCompute.Dispatch(0, N / 8, N / 8, N / 8);

            perlinCompute.SetInt("_Width", N);
            perlinCompute.SetInt("_Height", N);
            perlinCompute.SetFloat("_Frequency", noiseFrequency);
            perlinCompute.SetFloat("_Lacunarity", noiseLacunarity);
            perlinCompute.SetFloat("_Gain", noiseGain);
            perlinCompute.SetFloat("_Time", time * noiseSpeed);
            perlinCompute.SetTexture(0, "_PermTable1D", perlin.PermutationTable1D);
            perlinCompute.SetTexture(0, "_PermTable2D", perlin.PermutationTable2D);
            perlinCompute.SetTexture(0, "_Gradient4D", perlin.Gradient4D);
            perlinCompute.SetBuffer(0, "_Result", m_noiseBuffer);
            perlinCompute.SetInt("_ControlRange",numberOfControlPoints);
            perlinCompute.SetBuffer(0, "_ControlPoints", m_controlPoints);
            perlinCompute.SetFloat("_DistanceThreshold", distanceThreshold);
            perlinCompute.SetVector("_Offset", localOffset);
            perlinCompute.Dispatch(0, N / 8, N / 8, N / 8);

            marchingCubes.SetInt("_Width",N);
            marchingCubes.SetInt("_Height", N);
            marchingCubes.SetInt("_Depth", N);
            marchingCubes.SetInt("_Border", 1);
            marchingCubes.SetFloat("_Target", 0.0f);
            marchingCubes.SetBuffer(0, "_Voxels", m_noiseBuffer);
            marchingCubes.SetBuffer(0, "_Buffer", m_meshBuffer);
            marchingCubes.SetBuffer(0, "_CubeEdgeFlags", m_cubeEdgeFlags);
            marchingCubes.SetBuffer(0, "_TriangleConnectionTable", m_triangleConnectionTable);
            marchingCubes.SetVector("_Offset", localOffset);
            marchingCubes.SetVector("_Scale", lossyScale);
            marchingCubes.Dispatch(0, N / 8, N / 8, N / 8);
        }
    }

    #endregion
    
    #region Private Methods

    void Start()
    {
        PostRenderEvent.AddEvent(Camera.main, DrawClouds);
        material = new Material(drawBufferShader);
    }

    void Update()
    {
        if(simulate && isInitialized) 
        {
            time += Time.deltaTime;

            localOffset = new Vector4(this.transform.position.x, this.transform.position.y, this.transform.position.z, 0.0f);
            lossyScale = new Vector4(this.transform.lossyScale.x, this.transform.lossyScale.y, this.transform.lossyScale.z, 0.0f);

            m_controlPoints.SetData(this.ControlPoints);

            bufferClearCompute.SetInt("_Width", N);
            bufferClearCompute.SetInt("_Height", N);
            bufferClearCompute.SetInt("_Depth", N);
            bufferClearCompute.SetBuffer(0, "_Buffer", m_meshBuffer);
            bufferClearCompute.Dispatch(0, N / 8, N / 8, N / 8);

            perlinCompute.SetInt("_Width", N);
            perlinCompute.SetInt("_Height", N);
            perlinCompute.SetFloat("_Frequency", noiseFrequency);
            perlinCompute.SetFloat("_Lacunarity", noiseLacunarity);
            perlinCompute.SetFloat("_Gain", noiseGain);
            perlinCompute.SetFloat("_Time", time * noiseSpeed);
            perlinCompute.SetTexture(0, "_PermTable1D", perlin.PermutationTable1D);
            perlinCompute.SetTexture(0, "_PermTable2D", perlin.PermutationTable2D);
            perlinCompute.SetTexture(0, "_Gradient4D", perlin.Gradient4D);
            perlinCompute.SetBuffer(0, "_Result", m_noiseBuffer);
            perlinCompute.SetInt("_ControlRange", numberOfControlPoints);
            perlinCompute.SetBuffer(0, "_ControlPoints", m_controlPoints);
            perlinCompute.SetFloat("_DistanceThreshold", distanceThreshold);
            perlinCompute.SetVector("_Offset", localOffset);
            perlinCompute.Dispatch(0, N / 8, N / 8, N / 8);

            marchingCubes.SetInt("_Width",N);
            marchingCubes.SetInt("_Height", N);
            marchingCubes.SetInt("_Depth", N);
            marchingCubes.SetInt("_Border", 1);
            marchingCubes.SetFloat("_Target", 0.0f);
            marchingCubes.SetBuffer(0, "_Voxels", m_noiseBuffer);
            marchingCubes.SetBuffer(0, "_Buffer", m_meshBuffer);
            marchingCubes.SetBuffer(0, "_CubeEdgeFlags", m_cubeEdgeFlags);
            marchingCubes.SetBuffer(0, "_TriangleConnectionTable", m_triangleConnectionTable);
            marchingCubes.SetVector("_Offset", localOffset);
            marchingCubes.SetVector("_Scale", lossyScale);
            marchingCubes.Dispatch(0, N / 8, N / 8, N / 8);

            simulateOnStart = false;
        }            
    }

    void OnDestroy()
    {
        TryReleaseBuffers();
        PostRenderEvent.RemoveEvent(Camera.main, DrawClouds);
    }

    private void OnRenderObject() 
    {
        material.SetBuffer("_Buffer", m_meshBuffer);
        material.SetPass(0);
        Graphics.DrawProcedural(material, this.gameObject.GetComponent<Collider>().bounds, MeshTopology.Triangles, SIZE, camera: Camera.main);
    }

    void DrawClouds(Camera camera)
    {
    }

    private void TryReleaseBuffers() 
    {
        if (m_controlPoints != null) 
        {
            m_controlPoints.Release();
        }

        if (m_noiseBuffer != null) 
        {
            m_noiseBuffer.Release();
        }

        if (m_meshBuffer != null) 
        {
            m_meshBuffer.Release();
        }

        if (m_cubeEdgeFlags != null) 
        {
            m_cubeEdgeFlags.Release();
        }

        if (m_triangleConnectionTable != null) 
        {
            m_triangleConnectionTable.Release();
        }
    }

    #endregion
}