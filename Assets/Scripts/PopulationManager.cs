using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO; 

public class PopulationManager : MonoBehaviour
{
    StreamWriter file;
    List<PlayerMovement> population = new List <PlayerMovement>();
    public GameObject creaturePrefab;
    public int  populationSize = 100;
    public Transform spawnpoint;
    public int _genomeLength;
    public float mutationRate = 0.01f;
    public int generation=1;
    bool stop = false;
    [SerializeField] Text genText;
    [SerializeField] Text fitnessText;
    float avgFitness = 0;

    private void Start()
    {
        //init text
        genText.text = "Generation: 1";
        fitnessText.text = "Average Fitness: 0";

        //create first population
        initPopulation();
        Physics2D.IgnoreLayerCollision(7, 7,true);//so players dont collide with each other
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

    PlayerMovement getFittest()
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
        PlayerMovement fittest = population[index];
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
            }
            TotalFitness += population[i].fitness;
        }
        //get avg fit
        avgFitness = (float)System.Math.Round(TotalFitness / population.Count, 2);

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
            go.GetComponent<PlayerMovement>().initPlayer(new DNA(_genomeLength), spawnpoint.position);
            population.Add(go.GetComponent<PlayerMovement>());
        }
    } 

    void NextGeneration()
    {
        _genomeLength += 25;
        PlayerMovement fittest;
        PlayerMovement fittest2;

        //get parents
        fittest = getFittest();
        fittest2 = getFittest();

        //destroy previous population
        for (int i = 0; i < population.Count; i++)
        {
            Destroy(population[i].gameObject);
        }
        population.Clear();

        //add parents to next generation
        GameObject go = Instantiate(creaturePrefab, spawnpoint.position, Quaternion.identity);
        go.GetComponent<PlayerMovement>().initPlayer(fittest.dna, spawnpoint.position);
        population.Add(go.GetComponent<PlayerMovement>());

        GameObject go2 = Instantiate(creaturePrefab, spawnpoint.position, Quaternion.identity);
        go2.GetComponent<PlayerMovement>().initPlayer(fittest2.dna, spawnpoint.position);
        population.Add(go2.GetComponent<PlayerMovement>());

        //destroy temp fittest game objects
        Destroy(fittest.gameObject);
        Destroy(fittest2.gameObject);

        //add remaining population using fittest as parents
        while (population.Count < populationSize)
        {
            GameObject go3 = Instantiate(creaturePrefab, spawnpoint.position, Quaternion.identity);
            go3.GetComponent<PlayerMovement>().initPlayer(new DNA(fittest.dna, fittest.stepCount, fittest2.dna, fittest2.stepCount, mutationRate, _genomeLength), spawnpoint.position);
            population.Add(go3.GetComponent<PlayerMovement>());
        }
    }
}