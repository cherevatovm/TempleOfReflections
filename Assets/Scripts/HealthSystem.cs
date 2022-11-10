using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HealthSystem : MonoBehaviour
{
    [SerializeField] Rigidbody2D rb;
    [SerializeField] Canvas deathScreenCanvas;
    [SerializeField] float restartDelay = 2f;
    public float currentHP;
    public float maxHP;
    public float currentMP;
    public float maxMP;

    void Start()
    {
        deathScreenCanvas.gameObject.SetActive(false);
    }

    void Restart()
    {
        deathScreenCanvas.gameObject.SetActive(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);        
    }

    public bool IsAlive() => currentHP > 0;

    public bool DoesHaveMP() => currentMP > 0;
    
    public void Death()
    {
        Debug.Log("Game over");
        rb.bodyType = RigidbodyType2D.Static;
        deathScreenCanvas.gameObject.SetActive(true);
        Invoke(nameof(Restart), restartDelay);
    }

    public void OutOfMP()
    {
        throw new System.NotImplementedException();
    }
}
