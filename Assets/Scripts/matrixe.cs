using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class matrixe : MonoBehaviour
{
    // 1 -> Top || 2 -> Bot || 3 -> Left || 4 -> Right || 5 -> Spawn || 6 -> Boss
    [SerializeField] private (bool, bool, bool, bool, bool, bool)[,] matrix;
    [SerializeField] private int size;
    [SerializeField] private GameObject neo;
    [SerializeField] private GameObject boss;
    private Random r = new Random();
    
    void Awake()
    {
        if (size % 2 == 0) size += 1;
        matrix = new (bool, bool, bool, bool, bool, bool)[size,size]; 
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                matrix[i, j] = (true, true, true, true, false, false);
            }
        }
        
        generatedungeon(size);
    }

    private void Start()
    {
//        PrintM();
        int cnt = 0;
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                if (!(matrix[i, j].Item1 && matrix[i, j].Item2 && matrix[i, j].Item3 && matrix[i, j].Item4))
                {
                    GameObject oo = Instantiate(neo, new Vector2(i*11, j*11), Quaternion.identity, transform);
                    oo.name = $"Room_{cnt}";
                    cnt++;
                    oo.transform.GetChild(1).gameObject.SetActive(matrix[i, j].Item1);
                    oo.transform.GetChild(2).gameObject.SetActive(matrix[i, j].Item2);
                    oo.transform.GetChild(3).gameObject.SetActive(matrix[i, j].Item3);
                    oo.transform.GetChild(4).gameObject.SetActive(matrix[i, j].Item4);
                                    
                    oo.GetComponent<cleanscript>().spawn = matrix[i, j].Item5;
                    oo.GetComponent<cleanscript>().boss = matrix[i, j].Item6;

                    if (matrix[i, j].Item6 || matrix[i,j].Item5)
                    {
                        
                        GameObject bossshit = Instantiate(boss, new Vector2(i*11+1, j*11+1), Quaternion.identity, transform);

                    }
                }
                if (/*!(matrix[i, j].Item1 && matrix[i, j].Item2 && matrix[i, j].Item2 && matrix[i, j].Item2)*/true)
                {
                    GameObject oo = Instantiate(neo, new Vector2(150+i*11, j*11), Quaternion.identity, transform);
                    oo.name = $"Room_{cnt}";
                    cnt++;
                    oo.transform.GetChild(1).gameObject.SetActive(matrix[i, j].Item1);
                    oo.transform.GetChild(2).gameObject.SetActive(matrix[i, j].Item2);
                    oo.transform.GetChild(3).gameObject.SetActive(matrix[i, j].Item3);
                    oo.transform.GetChild(4).gameObject.SetActive(matrix[i, j].Item4);
                                    
                    oo.GetComponent<cleanscript>().spawn = matrix[i, j].Item5;
                    oo.GetComponent<cleanscript>().boss = matrix[i, j].Item6;
                                    
                }
                
            }
        }
    }

    public void generatedungeon(int size)
    {
        int maxroom = (size * size) /3;
        int compteur = 5;
        bool boule = true;
        
//        Debug.Log("GenerateDungeon : Setting things up for start room");
        matrix[size / 2, size / 2].Item1 = false;
        matrix[size / 2, size / 2].Item2 = false;
        matrix[size / 2, size / 2].Item3 = false;
        matrix[size / 2, size / 2].Item4 = false;
        matrix[size / 2, size / 2].Item5 = true;
        matrix[size / 2, size / 2].Item6 = false;


        while (compteur < maxroom && boule)
        {
            (int x, int y)= recdungeon(size);
            if (x >= 0)
            {
                int a = generateroom(x, y, maxroom - compteur);
                compteur += a;
            }
            else boule = false;
        }

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                checkdoors(size,i,j);
            }
        }

        AddTheBossRoom(size);
    }
    public (int,int) recdungeon(int size)
    {
//        Debug.Log("Recdungeon : Called");
        List<(int,int)> a = new List<(int, int)>();
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                if(IsAccesible(size,i,j))
                    a.Add((i,j));
            }
        }
        int b = 0;
        int c = a.Count-1;
        if (c >= 0)
            b += r.Next(c);
        if (c == -1)
            return (-1, -1);
        (int x, int y) = a[b];
        return (x,y);
    }
    
    
    public bool isvalid(int size,int i ,int j)
    {
        return (i >= 0 && j >= 0 && i < size && j < size);
    }
    
    public bool ishere(int size, int i, int j)
    {
        if (isvalid(size, i, j))
        {
            return !(matrix[i, j].Item1 && matrix[i, j].Item2 && matrix[i, j].Item3 && matrix[i, j].Item4);
        }
        return false;
    }
    
    public bool IsAccesible(int size, int i, int j)
    {
        bool b = false;
        if (!ishere(size,i,j))
        {
            if (isvalid(size,i,j+1) && !matrix[i,j+1].Item2)
                b = true;
            else if (isvalid(size,i,j-1) && !matrix[i,j-1].Item1)
                b = true;
            else if (isvalid(size,i+1,j) && !matrix[i+1,j].Item3)
                b = true;
            else if (isvalid(size,i-1,j) && !matrix[i-1,j].Item4)
                b = true;
        }

        return b;
    }
    
    
    public int generateroom(int i, int j,int diff)
    {
//        Debug.Log("GenerateRoom : Called");
        int size = matrix.GetLength(0);
        int compteur = 0;
        
        
        checkdoors(size,i,j);
        int d = possibledirections(size, i, j);


        if (d >= 3 && diff >= 3) 
        { 
            int a = r.Next(4);
            if (a == 3)
                compteur += randomdoor3(i, j);
            if (a == 2) 
                compteur += randomdoor2(i, j);
            if (a == 1 )  
                compteur += randomdoor1(i, j);
        }
        else if (d >= 2 && diff >= 2) 
        { 
            int a = r.Next(3); 
            if (a == 2) 
                compteur += randomdoor2(i, j);
            if (a == 1|| a == 0) 
                compteur += randomdoor1(i, j);
        }
        else if (d >= 1 && diff >= 1)
        {
            int a = r.Next(2);
            if (a == 1)
                compteur += randomdoor1(i, j);
        }

        return compteur;
    }
    
    public int possibledirections(int size, int i, int j)
    {
        int d = 0;
        if (!ishere(size, i + 1, j) && isvalid(size, i + 1, j))
            d++;
        if (!ishere(size, i - 1, j) && isvalid(size, i - 1, j))
            d++;
        if (!ishere(size, i, j + 1) && isvalid(size, i, j + 1))
            d++;
        if (!ishere(size, i, j - 1) && isvalid(size, i, j - 1))
            d++;
        return d;
    }
    
    public void checkdoors(int size, int i, int j)
    {
        if (isvalid(size,i+1,j) && !matrix[i+1,j].Item3)
            matrix[i, j].Item4 = false;
        
        if (isvalid(size,i-1,j) && !matrix[i-1,j].Item4)
            matrix[i, j].Item3 = false;
        
        if (isvalid(size,i,j+1) && !matrix[i,j+1].Item2)
            matrix[i, j].Item1 = false;
        
        if (isvalid(size,i,j-1) && !matrix[i,j-1].Item1)
            matrix[i, j].Item2 = false;
        
        if (!isvalid(size,i+1,j) && !matrix[i,j].Item4)
            matrix[i, j].Item4 = true;
        
        if (!isvalid(size,i-1,j) && !matrix[i,j].Item3)
            matrix[i, j].Item3 = true;
        
        if (!isvalid(size,i,j+1) && !matrix[i,j].Item1)
            matrix[i, j].Item1 = true;
        
        if (!isvalid(size,i,j-1) && !matrix[i,j].Item2)
            matrix[i, j].Item2 = true;
    }


    public int randomdoor3(int i, int j)
    {
        int a = 0;
        a += randomdoor1(i, j);
        a += randomdoor2(i, j);
        return a;
    }
    
    
    //fonction qui creuse 2 portes 
    public int randomdoor2(int i, int j)
    {
        int a = 0;
        a += randomdoor1(i, j);
        a += randomdoor1(i, j);
        return a;
    }
    
    
    //fonction qui creuse 1 porte aléatoirement
    public int randomdoor1(int i, int j)
    {
        int size = matrix.GetLength(0);
        bool added = false;
        
        while (!added && possibledirections(size,i,j)!=0)
        {
            int a = r.Next(4);
                    
            if (a == 0 && isvalid(size, i, j + 1) && matrix[i, j].Item4)
            {
                added = true;
                matrix[i, j].Item4 = false;
            }
            if (a == 1 && isvalid(size, i, j - 1) && matrix[i, j].Item3)
            {
                added = true;
                matrix[i, j].Item3 = false;
            }
            if (a == 2 && isvalid(size, i+1, j ) && matrix[i, j].Item2)
            {
                added = true;
                matrix[i, j].Item2 = false;
            }
            if (a == 3 && isvalid(size, i-1, j) && matrix[i, j].Item1)
            { 
                added = true;
                matrix[i, j].Item1 = false;
            }
        }

        if (added)
        {
            return 1;
        }
        return 0;
    }

    public void AddTheBossRoom(int size)
    {
        int rand = r.Next(4);
        if (rand == 0)
        {
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    if (ishere(size, i, j) && IsBossCandidaite(size,i,j))
                    {
                        matrix[i, j].Item6 = true;
                        return;
                    }
                        
                }
            }
        }
        if (rand == 1)
        {
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    if (ishere(size, j, i) && IsBossCandidaite(size,j,i))
                    {
                        matrix[j, i].Item6 = true;
                        return;
                    }
                        
                }
            }
        }
        if (rand == 2)
        {
            for (int i = size; i > 0; i--)
            {
                for (int j = size; j > 0; j--)
                {
                    if (ishere(size, i, j) && IsBossCandidaite(size,i,j))
                    {
                        matrix[i, j].Item6 = true;
                        return;
                    }
                        
                }
            }
        }
        if (rand == 3)
        {
            for (int i = size; i > 0; i--)
            {
                for (int j = size; j > 0; j--)
                {
                    if (ishere(size, j, i) && IsBossCandidaite(size,j,i))
                    {
                        matrix[j, i].Item6 = true;
                        return;
                    }
                        
                }
            }
        }
    }

    public bool IsBossCandidaite(int size, int i, int j)
    {
        bool bool1 = !matrix[i, j].Item1;
        bool bool2 = !matrix[i, j].Item2;
        bool bool3 = !matrix[i, j].Item3;
        bool bool4 = !matrix[i, j].Item4;

        bool bool5 = bool1 && !bool2 && !bool3 && !bool4;
        bool bool6 = !bool1 && bool2 && !bool3 && !bool4;
        bool bool7 = !bool1 && !bool2 && bool3 && !bool4;
        bool bool8 = !bool1 && !bool2 && !bool3 && bool4;

        bool bool9 = isvalid(size, i, j + 1) && matrix[i, j + 1].Item5;
        bool bool10 = isvalid(size, i, j - 1) && matrix[i, j - 1].Item5;
        bool bool11 = isvalid(size, i + 1, j) && matrix[i + 1, j].Item5;
        bool bool12 = isvalid(size, i - 1, j) && matrix[i - 1, j].Item5;

        bool bool13 = bool9 || bool10 || bool11 || bool12;

        return (bool5 || bool6 || bool7 || bool8) && !bool13;
    }
}


