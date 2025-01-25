using UnityEngine;
using UnityEngine.SceneManagement;


public class LevelLoad : MonoBehaviour
{

    public Animator transition;

    public bool acabado = false;

    // Update is called once per frame
    void Update()
    {
        if (acabado){
            transition.SetTrigger("Start");
        }
    }
}

