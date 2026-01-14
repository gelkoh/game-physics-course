using UnityEngine;

public class Crystal : MonoBehaviour
{
    [SerializeField] private ParticleSystem collectionVfx;
    
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Something collided with crystal");
        
        if (other.tag == "Player")
        {
            PlayCollectionEffect();
        }
    }

    private void PlayCollectionEffect() 
    {
        Instantiate(collectionVfx, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
