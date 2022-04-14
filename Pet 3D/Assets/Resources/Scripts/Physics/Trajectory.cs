using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Trajectory : MonoBehaviour
{
    Player player;
    private void Awake()
    {
        player = GetComponent<Player>();
    }

    private void Start()
    {
        CreatePhysicsScene();    
    }

    private Scene _simulationScene;
    private PhysicsScene _physicsScene;
    [SerializeField] private Transform _obstaclesParent;
    private Dictionary<Transform, Transform> _spawnedObjects = new Dictionary<Transform, Transform>();

    void CreatePhysicsScene()
    {
        _simulationScene = SceneManager.CreateScene("Simulation", new CreateSceneParameters(LocalPhysicsMode.Physics3D));
        _physicsScene = _simulationScene.GetPhysicsScene();

        foreach(Transform obj in _obstaclesParent)
        {
            var ghostObj = Instantiate(obj.gameObject,obj.position, obj.rotation);
            if(ghostObj.TryGetComponent(out Renderer rend)) rend.enabled = false;
            SceneManager.MoveGameObjectToScene(ghostObj, _simulationScene);
            if (!ghostObj.isStatic) _spawnedObjects.Add(obj, ghostObj.transform);
        }
    }

    private void Update()
    {
        foreach (var item in _spawnedObjects)
        {
            item.Value.position = item.Key.position;
            item.Value.rotation = item.Key.rotation;
        }
    }

    [SerializeField] private LineRenderer _line;
    [SerializeField] private int _maxPhysicsFrameIterations;

    public void SimulateTrajectory(Item itemPrefab, Vector3 pos, Vector3 velocity)
    {
        var ghostObj = Instantiate(itemPrefab.prefab, pos, Quaternion.identity);
        SceneManager.MoveGameObjectToScene(ghostObj.gameObject, _simulationScene);

        if (ghostObj.TryGetComponent(out Ball ball))
        {
            ball.Throw(player.debugPointerTip.transform.position, player.GetThrowDirection(), player.GetThrowForce(), true);
        }
        _line.positionCount = _maxPhysicsFrameIterations;

        for (int i = 0; i < _maxPhysicsFrameIterations; i++)
        {
            _physicsScene.Simulate(Time.fixedDeltaTime);
            _line.SetPosition(i, ghostObj.transform.position);
        }

        Destroy(ghostObj.gameObject);
    }

    public void RemoveTrajectory()
    {
        _line.positionCount = 0;
    }
}
