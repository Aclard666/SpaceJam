using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Physics;
using System.Collections.Generic;
using UnityEngine;
using Unity.Physics.Systems;

//[BurstCompile]
struct SetupComputeData_Job : IJob
{
    [ReadOnly] public NativeArray<float> massWriter;
    [ReadOnly] public NativeArray<Vector3> velWriter;
    [ReadOnly] public NativeArray<Vector3> transWriter;
    
    public ComputeBuffer _translations;
    public ComputeBuffer _velocities;
    public ComputeBuffer _masses;

    public void Execute()
    {
        _masses.SetData(massWriter);
        _velocities.SetData(velWriter);
        _translations.SetData(transWriter);
    }
}

//[BurstCompile]
struct SetupKernel_Job : IJob
{
    public ComputeShader shader;
        
    [ReadOnly] public ComputeBuffer _translations;
    [ReadOnly] public ComputeBuffer _velocities;
    [ReadOnly] public ComputeBuffer _masses;
    [ReadOnly] public ComputeBuffer _velocitiesOutput;

    [ReadOnly] public int objectCount;

    public void Execute()
    {
        int kernel = shader.FindKernel("Gravity");

        shader.SetBuffer(kernel, "_translations", _translations);
        shader.SetBuffer(kernel, "_velocities", _velocities);
        shader.SetBuffer(kernel, "_masses", _masses);
        shader.SetBuffer(kernel, "_velocitiesOutput", _velocitiesOutput);
        shader.SetInt("objectCount", objectCount);
        shader.SetFloat("dt", Time.fixedDeltaTime);
    }
}


[BurstCompile]
struct ReferencingFOData_Job : IJobChunk
{
    [ReadOnly] public ComponentTypeHandle<PhysicsMass> MassTypeHandle;
    [ReadOnly] public ComponentTypeHandle<Translation> TranslationTypeHandle;
    [ReadOnly] public ComponentTypeHandle<PhysicsVelocity> VelocityTypeHandle;
    
    public NativeArray<float> massWriter;
    public NativeArray<Vector3> velWriter;
    public NativeArray<Vector3> transWriter;
    
    public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
    {
        var chunkMass = chunk.GetNativeArray(MassTypeHandle);
        var chunkTranslation = chunk.GetNativeArray(TranslationTypeHandle);
        var chunkVelocity = chunk.GetNativeArray(VelocityTypeHandle);
        for (var i = 0; i < chunk.Count; i++)
        {
            massWriter[i] = 1.0f / chunkMass[i].InverseMass;
            velWriter[i] = chunkVelocity[i].Linear;
            transWriter[i] = chunkTranslation[i].Value;
        }
    }
}


struct customfloat3
{
    public float x;
    public float y;
    public float z;
}


//[BurstCompile]
struct LaunchKernel_Job : IJob
{
    public ComputeShader shader;
    [ReadOnly] public int RoundedUpObjectCount;
    public ComputeBuffer        velocitiesOutput;
    public NativeArray<Vector3> _velocitiesOutput;

    Vector3[] temp;

    //Check load Compute Data is ok
    //customfloat3[] temp2;
    //public ComputeBuffer        transOutput;


    public void Execute()
    {
        // Size and temp to retrieve
        int kindex = shader.FindKernel("Gravity");
        int threadGroupX = (RoundedUpObjectCount / 1024);
        temp = new Vector3[RoundedUpObjectCount];


        //Check load Compute Data is ok
        //temp2 = new customfloat3[RoundedUpObjectCount];

        // Launch 
        shader.Dispatch(kindex, threadGroupX, 1, 1);
        
        //Retrieve & Convert
        velocitiesOutput.GetData(temp);
        _velocitiesOutput.CopyFrom(temp);

        


        //Check load Compute Data is ok
        //transOutput.GetData(temp2);
    }
}





[UpdateBefore(typeof(BeginFixedStepSimulationEntityCommandBufferSystem))]
public class S_Gravity : SystemBase
{

    GameManager GM = GameManager.GetSingleton();
    
    NativeArray<float>      masses;
    NativeArray<Vector3>     velocities;
    NativeArray<Vector3>     translations;

    ComputeBuffer           _translations;
    ComputeBuffer           _velocities;
    ComputeBuffer           _masses;



    ComputeBuffer           velocitiesOutput;


    private EntityQuery m_Query;

    int ObjectCount = 0;
    int RoundedUpObjectCount = 0;

    protected override void OnCreate()
    {
        m_Query = GetEntityQuery(ComponentType.ReadOnly<Translation>(),
            ComponentType.ReadOnly<CD_FO>(), ComponentType.ReadOnly<PhysicsMass>(), ComponentType.ReadWrite<PhysicsVelocity>());
    }


    protected override void OnStartRunning()
    {
        base.OnStartRunning();

        ObjectCount                     = m_Query.CalculateEntityCount();

        //Reach a size multiple of 1024
        RoundedUpObjectCount            = ObjectCount + (1024 - (ObjectCount % 1024));

        _translations                   = new ComputeBuffer(RoundedUpObjectCount, sizeof(float) * 3);
        _velocities                     = new ComputeBuffer(RoundedUpObjectCount, sizeof(float) * 3);
        _masses                         = new ComputeBuffer(RoundedUpObjectCount, sizeof(float));


        masses                          = new NativeArray<float>(RoundedUpObjectCount, Allocator.TempJob,NativeArrayOptions.ClearMemory);
        velocities                      = new NativeArray<Vector3>(RoundedUpObjectCount, Allocator.TempJob);
        translations                    = new NativeArray<Vector3>(RoundedUpObjectCount, Allocator.TempJob);

        velocitiesOutput                = new ComputeBuffer(RoundedUpObjectCount, sizeof(float) * 3);

    }


    protected override void OnUpdate()
    {
        // For more simple code creating it the update allow us to use it in data application
        NativeArray<Vector3> _velocitiesOutput = new NativeArray<Vector3>(RoundedUpObjectCount, Allocator.TempJob);
        
        m_Query.CompleteDependency();

        var refjob = new ReferencingFOData_Job();

        refjob.velWriter                = velocities;
        refjob.transWriter              = translations;
        refjob.massWriter               = masses;

        refjob.MassTypeHandle           = GetComponentTypeHandle<PhysicsMass>(true);
        refjob.TranslationTypeHandle    = GetComponentTypeHandle<Translation>(true);
        refjob.VelocityTypeHandle       = GetComponentTypeHandle<PhysicsVelocity>(true);

        refjob.Run(m_Query);

        var setCD_job = new SetupComputeData_Job();

        setCD_job._masses = _masses;
        setCD_job._translations = _translations;
        setCD_job._velocities = _velocities;

        setCD_job.velWriter = velocities;
        setCD_job.transWriter = translations;
        setCD_job.massWriter = masses;

        setCD_job.Execute();
        
        var setK_job = new SetupKernel_Job();

        setK_job.objectCount = ObjectCount;
        setK_job.shader = GM._ref.CS_Gravity;
        setK_job._masses = _masses;
        setK_job._translations = _translations;
        setK_job._velocities = _velocities;
        setK_job._velocitiesOutput = velocitiesOutput;

        setK_job.Execute();
        
        //Debug.Log("Mass : " + masses[0]);
        //Debug.Log("Translations : " + translations[0]);
        //Debug.Log("Velocitie : " + velocities[0]);
        //Debug.Log("Mass : " + masses[1]);
        //Debug.Log("Translations : " + translations[1]);
        //Debug.Log("Velocitie : " + velocities[1]);
        //Debug.Log("Mass : " + masses[2]);
        //Debug.Log("Translations : " + translations[2]);
        //Debug.Log("Velocitie : " + velocities[2]);

        var launchK_job = new LaunchKernel_Job();

        launchK_job.RoundedUpObjectCount = RoundedUpObjectCount;
        launchK_job.shader = GM._ref.CS_Gravity;
        launchK_job.velocitiesOutput = velocitiesOutput;
        launchK_job._velocitiesOutput = _velocitiesOutput;


        //Check load Compute Data is ok
        //launchK_job.transOutput = _translations;

        launchK_job.Execute();
        
        Entities
        .WithAll<PhysicsMass, Translation>()
        .ForEach((int entityInQueryIndex, ref PhysicsVelocity vel) =>
        {
            vel.Linear = new float3( _velocitiesOutput[entityInQueryIndex].x, _velocitiesOutput[entityInQueryIndex].y, _velocitiesOutput[entityInQueryIndex].z);
        }).Run();
        _velocitiesOutput.Dispose();
    }

    protected override void OnStopRunning()
    {
        base.OnStopRunning();

        _translations   = null;
        _velocities     = null;
        _masses         = null;

        masses.Dispose();         
        velocities.Dispose();
        translations.Dispose();
    }

}
