using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombData
{
    public struct BombObject
    {
        public int ID;
        public int StartPoint;
        public int StartPointB;
        public bool isOneWay;
    }

    List<BombObject> Bombs;

    public BombData()
    {
        Bombs = new List<BombObject>();

        Bombs.Add(new BombObject() { ID = 1, StartPoint = 115, StartPointB = 1 });
        Bombs.Add(new BombObject() { ID = 2, StartPoint = 5, StartPointB = 8 });
        Bombs.Add(new BombObject() { ID = 3, StartPoint = 121, StartPointB = 124 });
        Bombs.Add(new BombObject() { ID = 4, StartPoint = 9, StartPointB = 10 });
        Bombs.Add(new BombObject() { ID = 5, StartPoint = 125, StartPointB = 126 });
        Bombs.Add(new BombObject() { ID = 6, StartPoint = 116, StartPointB = 13 });
        Bombs.Add(new BombObject() { ID = 7, StartPoint = 129, StartPointB = 232 });
        Bombs.Add(new BombObject() { ID = 8, StartPoint = 18, StartPointB = 19 });
        Bombs.Add(new BombObject() { ID = 9, StartPoint = 33, StartPointB = 35 });
        Bombs.Add(new BombObject() { ID = 10, StartPoint = 149, StartPointB = 151 });
        Bombs.Add(new BombObject() { ID = 11, StartPoint = 134, StartPointB = 135 });
        Bombs.Add(new BombObject() { ID = 12, StartPoint = 111 });
        Bombs.Add(new BombObject() { ID = 13, StartPoint = 45 });
        Bombs.Add(new BombObject() { ID = 14, StartPoint = 43, StartPointB = 44 });
        Bombs.Add(new BombObject() { ID = 15, StartPoint = 159, StartPointB = 160 });
        Bombs.Add(new BombObject() { ID = 16, StartPoint = 161 });
        Bombs.Add(new BombObject() { ID = 17, StartPoint = 53, StartPointB = 54 });
        Bombs.Add(new BombObject() { ID = 18, StartPoint = 170, StartPointB = 169 });
        Bombs.Add(new BombObject() { ID = 19, StartPoint = 51, StartPointB = 61, isOneWay = true });
        Bombs.Add(new BombObject() { ID = 20, StartPoint = 50, StartPointB = 48 });
        Bombs.Add(new BombObject() { ID = 21, StartPoint = 94, StartPointB = 95 });
        Bombs.Add(new BombObject() { ID = 22, StartPoint = 211, StartPointB = 210 });
        Bombs.Add(new BombObject() { ID = 23, StartPoint = 164, StartPointB = 166 });
        Bombs.Add(new BombObject() { ID = 24, StartPoint = 167, StartPointB = 177, isOneWay = true });
        Bombs.Add(new BombObject() { ID = 25, StartPoint = 69, StartPointB = 68 });
        Bombs.Add(new BombObject() { ID = 26, StartPoint = 100 });
        Bombs.Add(new BombObject() { ID = 27, StartPoint = 216 });
        Bombs.Add(new BombObject() { ID = 28, StartPoint = 185, StartPointB = 184 });
        Bombs.Add(new BombObject() { ID = 29, StartPoint = 84 });
        Bombs.Add(new BombObject() { ID = 30, StartPoint = 200 });
    }

    public BombObject[] RetriveBombObjects(int a, int b) 
    {
        return new BombObject[] { Bombs[a], Bombs[b] };
    }
}