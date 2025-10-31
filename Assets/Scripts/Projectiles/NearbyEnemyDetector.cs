using System.Collections.Generic;
using UnityEngine;

public class NearbyEnemyDetector : MonoBehaviour
{
    public List<GameObject> enemyList = new List<GameObject>();
    public GameObject enemyToAutoAim;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "EnemyHurtBox")
        {
            GameObject newEnem = other.gameObject;
            if(newEnem.GetComponentInParent<EnemyBehaviour>().hitPriority > 0)
            {
                enemyList.Add(newEnem);
                if (enemyList.Count > 0)
                {
                    EnemyBehaviour enemyToAdd = enemyList[0].GetComponentInParent<EnemyBehaviour>();
                    for (int i = 0; i < enemyList.Count; i++)
                    {
                        if (enemyList[i].GetComponentInParent<EnemyBehaviour>().hitPriority > enemyToAdd.hitPriority)
                        {
                            enemyToAdd = enemyList[i].GetComponentInParent<EnemyBehaviour>();
                        }

                    }
                    enemyToAutoAim = enemyToAdd.gameObject;
                }
                else
                {
                    enemyToAutoAim = newEnem;
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        GameObject newEnem = other.gameObject;
        if(enemyList.Contains(newEnem))enemyList.Remove(newEnem);
        if (enemyList.Count > 0)
        {
            EnemyBehaviour enemyToAdd = enemyList[0].GetComponentInParent<EnemyBehaviour>();
            for (int i = 0; i < enemyList.Count; i++)
            {
                if (enemyList[i].GetComponentInParent<EnemyBehaviour>().hitPriority > enemyToAdd.hitPriority)
                {
                    enemyToAdd = enemyList[i].GetComponentInParent<EnemyBehaviour>();
                }

            }
            enemyToAutoAim = enemyToAdd.gameObject;
        }
    }
}
