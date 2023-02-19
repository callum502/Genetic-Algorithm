using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO; 

public class PopulationManager : MonoBehaviour
{
    StreamWriter file;
    List<GeneticPathfinder> population = new List <GeneticPathfinder>();
    public GameObject creaturePrefab;
    public int  populationSize = 100;
    public Transform spawnpoint;
    public int _genomeLength;
    public float mutationRate = 0.01f;
    public int generation=0;
    bool stop = false;
    [SerializeField] Text genText;
    [SerializeField] Text fitnessText;
    float avgFitness = 0;

    private void Start()
    {
        //open file
        file = new StreamWriter("Results.csv");
        file.WriteLine("Generation, Best fit, Average fit, finished");
        //create first population
        initPopulation();
    }

    private void Update()
    {
        //stop when termination point is reached
        if (!stop)
        {
            //when players finish moving create new gen
            if (!HasActive())
            {
                UpdateUI();
                NextGeneration();
            }
        }
    }

    GeneticPathfinder getFittest()
    {
        //return fittest member and remove them from poulation list
        float maxFitness = float.MinValue;
        int index = 0;
        for (int i=0; i<population.Count; i++)
        {
            if (population[i].fitness > maxFitness)
            {
                maxFitness = population[i].fitness;
                index = i;
            }
        }
        GeneticPathfinder fittest = population[index];
        population.Remove(fittest);
        return fittest;
    }

    bool HasActive()
    {
        //if any players are still moving return true
        for (int i=0; i<population.Count; i++)
        {
            if(!population[i].hasFinished)
            {
                return true;
            }
        }
        return false;
    }

    void UpdateUI()
    {
        //update and display generation number
        generation++;
        genText.text = "Generation: ";
        genText.text = genText.text + (generation.ToString());
        float TotalFitness = 0;
        avgFitness = 0;

        //if player reaches end stop creating new generations
        for (int i = 0; i < population.Count; i++)
        {
            if(population[i].reachedtarget)
            {
                stop = true;
                file.Close();
            }
            TotalFitness += population[i].fitness;
        }
        //get avg fit
        avgFitness = TotalFitness/population.Count;
        
        //display avg fit
        fitnessText.text = "Average Fitness: ";
        fitnessText.text = fitnessText.text + (avgFitness.ToString());
    }
    
    void initPopulation()
    {
        //use new random dna to make first population
        for (int i = 0; i<populationSize; i++)
        {
            GameObject go = Instantiate(creaturePrefab, spawnpoint.position, Quaternion.identity);
            go.GetComponent<GeneticPathfinder>().initPlayer(new DNA(_genomeLength));
            population.Add(go.GetComponent<GeneticPathfinder>());
        }
    } 

    void NextGeneration()
    {
        GeneticPathfinder fittest;
        GeneticPathfinder fittest2;

        //get parents
        fittest = getFittest();
        fittest2 = getFittest();

        WriteToFile(generation, fittest.fitness, avgFitness);
        //destroy previous population
        for (int i = 0; i < population.Count; i++)
        {
            Destroy(population[i].gameObject);
        }
        population.Clear();

        //add parents to next generation
        GameObject go = Instantiate(creaturePrefab, spawnpoint.position, Quaternion.identity);
        go.GetComponent<GeneticPathfinder>().initPlayer(fittest.dna);
        population.Add(go.GetComponent<GeneticPathfinder>());

        GameObject go2 = Instantiate(creaturePrefab, spawnpoint.position, Quaternion.identity);
        go2.GetComponent<GeneticPathfinder>().initPlayer(fittest2.dna);
        population.Add(go2.GetComponent<GeneticPathfinder>());

        //destroy temp fittest game objects
        Destroy(fittest.gameObject);
        Destroy(fittest2.gameObject);

        //add remaining population using fittest as parents
        while (population.Count < populationSize)
        {
            GameObject go3 = Instantiate(creaturePrefab, spawnpoint.position, Quaternion.identity);
            go3.GetComponent<GeneticPathfinder>().initPlayer(new DNA(fittest.dna, fittest.stepCount, fittest2.dna, fittest2.stepCount, mutationRate));
            population.Add(go3.GetComponent<GeneticPathfinder>());
        }
    }

    //write results to file
    void WriteToFile(int generation, float bestFit, float avgFit)
    {
        //when writing to a file, use WriteLine, and seperate columns with commas
        file.WriteLine(generation.ToString() + "," + bestFit.ToString() + "," + avgFit.ToString());
    }
}