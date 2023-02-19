using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DNA
{
    public List<Vector2> genes = new List<Vector2>();

    //initialise DNA with random genes
    public DNA(int genomeLength=50)
    {
        //add random genes
        for (int i=0; i<genomeLength; i++)
        {
            genes.Add(new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f))); 
        }
    }

    public DNA(DNA parent, int stepCount, DNA parent2, int stepCount2, float mutationRate=0.01f)
    {
        //randomly chose parent to inherit from
        int parentBool = Random.Range(0,2);
        int crossPoint = 0;

         for (int i=0; i<parent.genes.Count; i++)
        {
            //mutate by randomising gene
            float mutationChance = Random.Range(0.0f, 1.0f);
            if (mutationChance<=mutationRate)
            {
                genes.Add(new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)));
            }
            else
            {
                if (parentBool == 0)
                {
                    //get cross point from 2-10 steps before parent end point
                    crossPoint = stepCount-Random.Range(2,10);;
                    //add parents genes before cross point
                    if (i<crossPoint)
                    {
                        genes.Add(parent.genes[i]); 
                    }
                    //add random genes after cross point
                    else
                    {
                        genes.Add(new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)));
                    }
                }
                else
                {
                    //get cross point from 2-10 steps before parent end point
                    crossPoint = stepCount2- Random.Range(2,5);
                    //add parents genes before cross point
                    if (i<crossPoint)
                    {
                        genes.Add(parent2.genes[i]);
                    }
                    //add random genes after cross point
                    else
                    {
                        genes.Add(new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)));
                    }
                }
            }
        }
    }
}
