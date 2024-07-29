//Thank you John Sorrentino on YT for the code base!

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class NN : MonoBehaviour
{
    int [] networkShape = {7,10,10,5};
    public Layer [] layers;

    private string savePath;

    public void Awake()
    {
        layers = new Layer[networkShape.Length - 1];

        for(int i = 0; i < layers.Length; i++)
        {
            layers[i] = new Layer(networkShape[i], networkShape[i+1]);
        }

        Random.InitState((int)System.DateTime.Now.Ticks);
        InitializeWeightsAndBiases();
        savePath = Application.persistentDataPath + "/maxNN.dat";
    }

    private void InitializeWeightsAndBiases()
    {
        System.Random rand = new System.Random();

        foreach (var layer in layers)
        {
            for (int i = 0; i < layer.n_neurons; i++)
            {
                for (int j = 0; j < layer.n_inputs; j++)
                {
                    layer.weightsArray[i, j] = (float)(rand.NextDouble() * 2 - 1); // Random values between -1 and 1
                }
                layer.biasesArray[i] = (float)(rand.NextDouble() * 2 - 1); // Random values between -1 and 1
            }
        }
    }

    public float[] Brain(float [] inputs)
    {
        for(int i = 0; i < layers.Length; i++)
        {
            if(i == 0)
            {
                layers[i].Forward(inputs);
                layers[i].Activation();
            } 
            else if(i == layers.Length - 1)
            {
                layers[i].Forward(layers[i - 1].nodeArray);
            }
            else
            {
                layers[i].Forward(layers[i - 1].nodeArray);
                layers[i].Activation();
            }    
        }

        return(layers[layers.Length - 1].nodeArray);
    }

    public void CopyToNewGen()
    {
        // Implement logic to copy current NN to new generation
        Layer[] newGenLayers = copyLayers();
        // Assign copied layers to new generation (This part of the logic will depend on your specific needs and design)
    }

    public Layer[] copyLayers()
    {
        Layer[] tmpLayers = new Layer[networkShape.Length - 1];
        for(int i = 0; i < layers.Length; i++)
        {
            tmpLayers[i] = new Layer(networkShape[i], networkShape[i+1]);
            System.Array.Copy(layers[i].weightsArray, tmpLayers[i].weightsArray, layers[i].weightsArray.GetLength(0) * layers[i].weightsArray.GetLength(1));
            System.Array.Copy(layers[i].biasesArray, tmpLayers[i].biasesArray, layers[i].biasesArray.GetLength(0));
        }
        return tmpLayers;
    }

    public void MutateNetwork(float mutationChance, float mutationAmount)
    {
        for(int i = 0; i < layers.Length; i++)
        {
            layers[i].MutateLayer(mutationChance, mutationAmount);
        }
    }

    public void MaxControl(bool newMax)
    {
        if (newMax)
        {
            SaveNetwork();
        }
        else
        {
            LoadNetwork();
        }
    }

    private void SaveNetwork()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(savePath);

        List<float[,]> weightsList = new List<float[,]>();
        List<float[]> biasesList = new List<float[]>();

        foreach (var layer in layers)
        {
            weightsList.Add(layer.weightsArray);
            biasesList.Add(layer.biasesArray);
        }

        bf.Serialize(file, new NetworkData(weightsList, biasesList));
        file.Close();
    }

    private void LoadNetwork()
    {
        if (File.Exists(savePath))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(savePath, FileMode.Open);
            NetworkData data = (NetworkData)bf.Deserialize(file);
            file.Close();

            for (int i = 0; i < layers.Length; i++)
            {
                layers[i].weightsArray = data.weightsList[i];
                layers[i].biasesArray = data.biasesList[i];
            }
        }
        else
        {
            Debug.LogError("No saved network found.");
        }
    }

    [System.Serializable]
    private class NetworkData
    {
        public List<float[,]> weightsList;
        public List<float[]> biasesList;

        public NetworkData(List<float[,]> weightsList, List<float[]> biasesList)
        {
            this.weightsList = weightsList;
            this.biasesList = biasesList;
        }
    }

    public class Layer
    {
        public float[,] weightsArray;
        public float[] biasesArray;
        public float[] nodeArray;

        public int n_inputs;
        public int n_neurons;

        public Layer(int n_inputs, int n_neurons)
        {
            this.n_inputs = n_inputs;
            this.n_neurons = n_neurons;

            weightsArray = new float[n_neurons, n_inputs];
            biasesArray = new float[n_neurons];
        }

        public void Forward(float[] inputsArray)
        {
            nodeArray = new float[n_neurons];

            for(int i = 0; i < n_neurons; i++)
            {
                for(int j = 0; j < n_inputs; j++)
                {
                    nodeArray[i] += weightsArray[i, j] * inputsArray[j];
                }
                nodeArray[i] += biasesArray[i];
            }
        }

        public void Activation()
        {
            for(int i = 0; i < nodeArray.Length; i++)
            {
                nodeArray[i] = (float)System.Math.Tanh(nodeArray[i]);
            }
        }

        public void MutateLayer(float mutationChance, float mutationAmount)
        {
            for(int i = 0; i < n_neurons; i++)
            {
                for(int j = 0; j < n_inputs; j++)
                {
                    if(UnityEngine.Random.value < mutationChance)
                    {
                        weightsArray[i, j] += UnityEngine.Random.Range(-1.0f, 1.0f) * mutationAmount;
                    }
                }

                if(UnityEngine.Random.value < mutationChance)
                {
                    biasesArray[i] += UnityEngine.Random.Range(-1.0f, 1.0f) * mutationAmount;
                }
            }
        }
    }
}
