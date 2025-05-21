using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Events;
using static MapManager;
[Serializable]
public class Coord : MonoBehaviour
{
    #region Variables
    public MapManager _mapManager;

    //Attributs de la structure Coord
    public int x;
    public int y;
    public int x_vector;
    public int y_vector;
    public EventCoord _event;
    public Hero _heroOnCoord;
    public bool _isVisited;
    public bool _isVisible;
    public string _description;
    public string _visitedBy;
    public bool _isWall;
    #endregion
    #region Constructors
    public Coord(int x, int y, EventCoord _event, MapManager mapManager = null, string description = "Case non-visitée", Hero hero = Hero.Nothing)
    {
        this.x = x;
        this.y = y;
        this.x_vector = x;
        this.y_vector = y;
        this._event = _event;
        this._heroOnCoord = hero;
        this._isVisited = false;
        this._visitedBy = "";
        this._isVisible = false;
        if (mapManager == null)
        {
            this._mapManager = FindFirstObjectByType<MapManager>();
        }
        else
        {
            this._mapManager = mapManager;
        }
        string default_description = _mapManager._gm._language.GetText("__case__non_visitee__");
        this._description = description == default_description ? default_description : description;

    }
    #endregion
    #region Methods
    public void AddDescription(string txt)
    {
        _description += " " + txt;
    }
    public void AddVisitor(string name)
    {
        _visitedBy = name;
    }
    public EventCoord GetEvent()
    {
        return _event;
    }
    public Hero GetHero()
    {
        return _heroOnCoord;
    }
    public int GetDistance(Coord coord)
    {
        return Mathf.Abs(x - coord.x) + Mathf.Abs(y - coord.y);
    }
    public HashSet<Coord> GetNeighbours(int range)
    {
        HashSet<Coord> _accessibles = new HashSet<Coord>();
        Queue<(Coord, int)> _queue = new Queue<(Coord, int)>(); // int -> distance actuelle
        HashSet<Coord> _visited = new HashSet<Coord>();

        _queue.Enqueue((this, 0));
        _visited.Add(this);

        Vector2Int[] directions = {
            new Vector2Int(1, 0), new Vector2Int(-1, 0),
            new Vector2Int(0, 1), new Vector2Int(0, -1)
        };

        while (_queue.Count > 0)
        {
            var (currentPos, dist) = _queue.Dequeue();

            if (dist > range)
                continue;

            _accessibles.Add(currentPos);

            foreach (Vector2Int dir in directions)
            {
                int a = currentPos.x + dir.x;
                int b = currentPos.y + dir.y;
                Coord voisin = _mapManager.m_CoordSet.Get(a, b);
                if (voisin == null)
                { continue;}
                if (_visited.Contains(voisin) || voisin._event == EventCoord.Wall )
                { continue; }

                _visited.Add(voisin);
                _queue.Enqueue((voisin, dist + 1));
            }
        }

        return _accessibles;
    }
    public HashSet<Coord> GetNeighboursPreciseDistance(int exactDistance, HashSet<Coord> all, bool onlyVisible = false)
    {

            HashSet<Coord> _result = new HashSet<Coord>();
            Queue<(Coord, int)> _queue = new Queue<(Coord, int)>();
            HashSet<Coord> _visited = new HashSet<Coord>();

            _queue.Enqueue((this, 0));
            _visited.Add(this);

            Vector2Int[] directions = {
        new Vector2Int(1, 0), new Vector2Int(-1, 0),
        new Vector2Int(0, 1), new Vector2Int(0, -1)
    };

            while (_queue.Count > 0)
            {
                var (currentPos, dist) = _queue.Dequeue();

                if (dist == exactDistance)
                {
                if (!all.Contains(currentPos))
                {
                    if (onlyVisible)
                    {
                        if (currentPos._isVisible)
                        {
                            _result.Add(currentPos);
                        }
                    }
                    else
                    {
                        _result.Add(currentPos);
                    }
                    
                }
                    continue; // On n'explore pas plus loin une fois la distance atteinte
                }

                foreach (Vector2Int dir in directions)
                {
                    int a = currentPos.x + dir.x;
                    int b = currentPos.y + dir.y;
                    Coord voisin = _mapManager.m_CoordSet.Get(a, b);
                    if (voisin == null || _visited.Contains(voisin) || voisin._event == EventCoord.Wall)
                        continue;

                    _visited.Add(voisin);
                    _queue.Enqueue((voisin, dist + 1));
                }
            }

            return _result;
        }
    public HashSet<Coord> GetNeighboursVisibles(int range)
    {
        HashSet<Coord> _accessibles = new HashSet<Coord>();
        Queue<(Coord, int)> _queue = new Queue<(Coord, int)>(); // int -> distance actuelle
        HashSet<Coord> _visited = new HashSet<Coord>();

        _queue.Enqueue((this, 0));
        _visited.Add(this);

        Vector2Int[] directions = {
            new Vector2Int(1, 0), new Vector2Int(-1, 0),
            new Vector2Int(0, 1), new Vector2Int(0, -1)
        };

        while (_queue.Count > 0)
        {
            var (currentPos, dist) = _queue.Dequeue();

            if (dist > range)
                continue;

            _accessibles.Add(currentPos);

            foreach (Vector2Int dir in directions)
            {
                int a = currentPos.x + dir.x;
                int b = currentPos.y + dir.y;
                Coord voisin = _mapManager.m_CoordSet.Get(a, b);
                if (voisin == null)
                { continue; }
                if (_visited.Contains(voisin) || voisin._event == EventCoord.Wall || voisin._isVisible == false)
                { continue; }

                _visited.Add(voisin);
                _queue.Enqueue((voisin, dist + 1));
            }
        }

        return _accessibles;
    }
    public void SetHero(Hero hero)
        {
            _heroOnCoord = hero;         
            AddVisitor(_mapManager._gm._charactersManagers.GetName(HeroToInt(hero)));
        }
    public void RemoveHero()
    {
        _heroOnCoord = Hero.Nothing;
    }
    public int HeroToInt(Hero hero)
    {
        switch (hero)
        {
            case Hero.Nothing:
                return -1;
            case Hero.Hero1:
                return 0;
            case Hero.Hero2:
                return 1;
            case Hero.Hero3:
                return 2;
            case Hero.Hero4:
                return 3;
            default:
                return -1;
        }
    }
    //Les méthodes suivantes permettent de comparer deux coordonnées. Grâce à ça on peut directement écrire if(coord1 == coord2).
    #region Equals
    public static bool operator ==(Coord left, Coord right)
    {
        // Si les deux sont null, ils sont égaux
        if (ReferenceEquals(left, null) && ReferenceEquals(right, null))
            return true;

        // Si un seul est null, ils ne sont pas égaux
        if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
            return false;

        // Comparaison des valeurs si les deux ne sont pas null
        return left.x == right.x && left.y == right.y;
    }
    public static bool operator !=(Coord left, Coord right)
    {
        return !(left == right);
    }
    #endregion
}
public static class DictionaryExtensions
{
    public static Coord Get(this Dictionary<(int, int), Coord> dict, int x, int y)
    {
        dict.TryGetValue((x, y), out Coord value);
        return value; // Retourne `null` pour les types référence, ou une valeur par défaut pour les types valeur.
    }
}
#endregion