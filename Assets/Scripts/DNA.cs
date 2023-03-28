using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DNA
{
    public List<int> genes = new List<int>();

    //initialise DNA with random genes
    public DNA(int genomeLength=50)
    {
        //add random genes
        for (int i=0; i<genomeLength; i++)
        {
            genes.Add(Random.Range(0, 4)); 
        }
    }

    public DNA(DNA parent, int stepCount, DNA parent2, int stepCount2, float mutationRate=0.01f, int genomeLength=50)
    {
        //randomly chose parent to inherit from
        int parentBool = Random.Range(0,3);
        int crossPoint = 0;

         for (int i=0; i<genomeLength; i++)
        {
            //mutate by randomising gene
            float mutationChance = Random.Range(0.0f, 1.0f);
            if (mutationChance<=mutationRate)
            {
                genes.Add(Random.Range(0, 4));
            }
            else
            {
                if (parentBool == 0)
                {
                    //get cross point from 2-10 steps before parent end point
                    crossPoint = stepCount;
                    //add parents genes before cross point
                    if (i<crossPoint)
                    {
                        genes.Add(parent.genes[i]); 
                    }
                    //add random genes after cross point
                    else
                    {
                        genes.Add(Random.Range(0, 4));
                    }
                }
                else
                {
                    //get cross point from 2-5 steps before parent end point
                    crossPoint = stepCount2;
                    //add parents genes before cross point
                    if (i<crossPoint)
                    {
                        genes.Add(parent2.genes[i]);
                    }
                    //add random genes after cross point
                    else
                    {
                        genes.Add(Random.Range(0, 4));
                    }
                }
            }
        }
    }
}
