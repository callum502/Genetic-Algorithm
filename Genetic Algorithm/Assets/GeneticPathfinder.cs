using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneticPathfinder : MonoBehaviour
{
    public DNA dna;
    public float Speed;

    bool hasBeenInited = false;
    public bool hasFinished = false;
    public bool reachedtarget = false;

    int geneIndex = 0;

    int checkpointsHit=0;

    public Transform target;
    public Transform[] checkpoint;
    Vector2 nextPoint;

    public int stepCount;

    //init player with dna
    public void initPlayer(DNA new_dna)
    {
        Time.timeScale = 5;
        dna = new_dna;
        nextPoint = transform.position;
        hasBeenInited = true;
    }

    private void Update()
    {
        if(hasBeenInited && !hasFinished)
        {
            //if reached target set finished
            if (Vector2.Distance(transform.position, target.position) < 0.5f)
            {
                hasFinished=true;
                reachedtarget = true;
                
            }
            //if reached end of genes set finished
            if (geneIndex == dna.genes.Count)
            {
                hasFinished=true;
            }
            //if moved get next target from genes
            if ((Vector2)transform.position == nextPoint)
            {
                nextPoint = (Vector2)transform.position + dna.genes[geneIndex];
                geneIndex++;
                stepCount++;
            }
            //else move to target
            else
            {
                transform.position = Vector2.MoveTowards(transform.position, nextPoint, Speed * Time.deltaTime);
            }
        }
    }

    public float fitness
    {
        get
        {
            float dist=0;
            float distVal=0;

            //get distance to next checkpoint
            for(int i=0; i<6; i++)
            {
                 if(checkpointsHit==i)
                 {
                     dist = Vector2.Distance(transform.position, checkpoint[i].position);
                 }
            }

            //if all checkpoints hit get distance to end goal
            if(checkpointsHit==6)
            {
                dist = Vector2.Distance(transform.position, target.position);
            }

            //clamp distance to 1 or greater
            if (dist <1 )
            {
                dist = 1.0f;
            }
            //get distVal from distance
            distVal = 1.0f/dist;

            //return total fitness
            return checkpointsHit + distVal;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if  (collision.gameObject.layer == 8)
        {
            hasFinished=true;
        }

        if(checkpointsHit<6)
        {
            if  (collision.gameObject.layer == checkpointsHit + 10)
            {
                checkpointsHit++;
            }
        }
    }
}
