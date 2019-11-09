using UnityEngine;
using UnityEngine.SceneManagement;

public class PhysicsSceneTest : MonoBehaviour
{
    public Collider OtherTriggerColliderPrefab;

    private Scene LocalPhysicsScene;
    private PhysicsScene LocalPhysics;

    public bool PerformTrigger;

    // Update is called once per frame
    void Update()
    {
        if (PerformTrigger)
        {
            Debug.Log(MyLog.Format("Init"));
            this.LocalPhysicsScene = SceneManager.CreateScene("LocalPhysics", new CreateSceneParameters(LocalPhysicsMode.Physics3D));
            this.LocalPhysics = this.LocalPhysicsScene.GetPhysicsScene();

            var oldActiveScene = SceneManager.GetActiveScene();
            SceneManager.SetActiveScene(SceneManager.GetSceneByName("LocalPhysics"));
            for (var i = 0; i < 100; i++)
            {
                Instantiate(this.OtherTriggerColliderPrefab);
            }

            SceneManager.SetActiveScene(oldActiveScene);

            Physics.autoSimulation = false;
            this.LocalPhysics.Simulate(0.0001f);
            Physics.autoSimulation = true;
            SceneManager.UnloadSceneAsync("LocalPhysics");
            this.PerformTrigger = false;
        }
    }
}