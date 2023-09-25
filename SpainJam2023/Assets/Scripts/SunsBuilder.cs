using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunsBuilder : MonoBehaviour
{
    private float sunRadio = 0.5f;
    public float SunRadio
    {
        get { return sunRadio; }
        set { sunRadio = value; }
    }

    private Vector3 posFirst;
    public Vector3 PosFirst
    {
        get { return posFirst; }
        set { posFirst = value; }
    }

    private float minDistance = 900f;
    public float MinDistance
    {
        get { return minDistance; }
        set { minDistance = value; }
    }

    private float maxDistance = 1300f;
    public float MaxDistance
    {
        get { return maxDistance; }
        set { maxDistance = value; }
    }

    private int fastPartsArea = 5;
    public int FastPartsArea
    {
        get { return fastPartsArea; }
        set { fastPartsArea = value; }
    }

    private Collider2D colliderArea;
    public Collider2D ColliderArea
    {
        get { return colliderArea; }
        set { colliderArea = value; }
    }

    private List<Vector3> listSuns = new List<Vector3>();

    private SortedDictionary<float, Vector3> mapSunDistances = new SortedDictionary<float, Vector3>();

    public List<Vector3> GenerateSequentialsSuns(int countMax)
    {
        mapSunDistances.Clear();
        listSuns.Clear();
        listSuns.Add(posFirst);

        Debug.Log("SUNS. GenerateSuns.INI");
        int idx = 0;
        while (true)
        {
            Vector3 posParent = listSuns[idx];
            int maxChilds = Random.Range(2, 5);

            Debug.Log("SUNS. GenerateSuns.Cicle[" + idx + "]: posParent = " + posParent + " // maxChilds = " + maxChilds);

            for (int numChild = 0; numChild < maxChilds; numChild++)
            {
                float randomChildDistance = Random.Range(minDistance * 2, maxDistance * 2);
                Vector3 posChild = GetCircleRandomPosition(posParent, randomChildDistance);

                Debug.Log("SUNS. GenerateSuns.Cicle[" + idx + "].child[" + numChild + "]: randomChildDistance = " + randomChildDistance + " // posChild = " + posChild);

                if (IsCirlceInRect(posChild, minDistance, colliderArea.bounds.min, colliderArea.bounds.max) && !CanSunCollide(posChild)) AddSun(posChild);

                if (listSuns.Count >= countMax) break;
            }
            idx++;
            if (listSuns.Count >= countMax) break;
            if (idx >= countMax) break;
            if (idx >= listSuns.Count) break;
        }

        Debug.Log("SUNS. GenerateSuns.END");
        return listSuns;
    }

    public List<Vector3> GenerateRandomSuns(int countMax)
    {
        listSuns.Clear();
        listSuns.Add(posFirst);

        for (int i = 0; i < countMax; i++)
        {
            Vector2 posSun = new Vector2(Random.Range(colliderArea.bounds.min.x + minDistance, colliderArea.bounds.max.x - minDistance), Random.Range(colliderArea.bounds.min.y + minDistance, colliderArea.bounds.max.y - minDistance));
            if (IsCirlceInRect(posSun, minDistance, colliderArea.bounds.min, colliderArea.bounds.max) && !CanSunCollide(posSun)) AddSun(posSun);

            if (listSuns.Count >= countMax) break;
        }
        return listSuns;
    }

    public List<Vector3> GenerateCircularSuns(int countMax)
    {
        listSuns.Clear();
        listSuns.Add(posFirst);

        Debug.Log("SUNS. GenerateSuns.INI");
        int idx = 0;
        while (true)
        {
            Vector3 posParent = listSuns[idx];
            int maxChilds = Random.Range(3, 5);

            Debug.Log("SUNS. GenerateSuns.Cicle[" + idx + "]: posParent = " + posParent + " // maxChilds = " + maxChilds);

            for (int numChild = 0; numChild < maxChilds; numChild++)
            {
                Vector3 posChild = GetChildSunPosition(posParent);
                if (posChild != Vector3.zero) AddSun(posChild);
                if (listSuns.Count >= countMax) break;
            }
            idx++;
            if (listSuns.Count >= countMax) break;
            if (idx >= countMax) break;
            if (idx >= listSuns.Count) break;
        }

        Debug.Log("SUNS. GenerateSuns.END");
        return listSuns;
    }

    private Vector3 GetChildSunPosition(Vector3 sunParent)
    {
        float angle = GetRandomAngle();

        for (int angleStep = 0; angleStep < 360; angleStep += 15)
        {
            Vector3 posChild = GetCirclePosition(sunParent, Random.Range(minDistance * 2, maxDistance * 2), angle);
            if (IsCirlceInRect(posChild, minDistance, colliderArea.bounds.min, colliderArea.bounds.max) && !CanSunCollide(posChild))
            {
                return posChild;
            }
            angle += angleStep;
        }
        return Vector3.zero;
    }

    private void AddSun(Vector3 posSun)
    {
        float distance = Vector3.Distance(posFirst, posSun);
        listSuns.Add(posSun);
        mapSunDistances.Add(distance, posSun);
    }

    public Vector3 IndentifyFarSun()
    {
        Vector3 posBase = listSuns[0];
        Vector3 posFar = posBase;
        float distanceFar = 0f;

        foreach(Vector3 posSun in listSuns)
        {
            float distanceSun = Vector3.Distance(posBase, posSun);
            if (distanceSun > distanceFar)
            {
                posFar = posSun;
                distanceFar = distanceSun; 
            }
        }
        return posFar;
    }

    public Vector3 IdentifyRandomFastSun()
    {
        List<float> listDistances = new List<float>(mapSunDistances.Keys);
        int idxRandomFast = Random.Range(listDistances.Count / fastPartsArea * (fastPartsArea-1), listDistances.Count);
        if (idxRandomFast >= listDistances.Count) idxRandomFast = listDistances.Count - 1;
        if (idxRandomFast < 0) idxRandomFast = 0;

        float distanceRandom = listDistances[idxRandomFast];
        return mapSunDistances[distanceRandom];
    }


    // Función para obtener una posición aleatoria en el perímetro de la circunferencia.
    Vector2 GetCircleRandomPosition(Vector2 center, float radio)
    {
        // Genera un ángulo aleatorio en radianes (entre 0 y 2PI).
        float randomAngle = Random.Range(0f, 2f * Mathf.PI);

        // Calcula las coordenadas (x, y) del punto en el perímetro de la circunferencia.
        float x = center.x + radio * Mathf.Cos(randomAngle);
        float y = center.y + radio * Mathf.Sin(randomAngle);

        // Retorna un Vector2 con las coordenadas obtenidas.
        return new Vector2(x, y);
    }

    // Función para obtener una posición en el perímetro de la circunferencia dada por un angulo en grados
    Vector2 GetCirclePosition(Vector2 center, float radio, float angleDeg)
    {
        // Genera un ángulo aleatorio en radianes (entre 0 y 2PI).
        float angleRad = angleDeg * Mathf.Deg2Rad;

        // Calcula las coordenadas (x, y) del punto en el perímetro de la circunferencia.
        float x = center.x + radio * Mathf.Cos(angleRad);
        float y = center.y + radio * Mathf.Sin(angleRad);

        // Retorna un Vector2 con las coordenadas obtenidas.
        return new Vector2(x, y);
    }

    float GetRandomAngle()
    {
        // Genera un ángulo aleatorio en radianes (entre 0 y 360).
        return Random.Range(0f, 360f);
    }

    // Identifica si el Sol puede chocar con otro
    bool CanSunCollide(Vector3 posSun)
    {
        for (int i = 0; i < listSuns.Count; i++)
        {
            if (CirclesIntersect(posSun, minDistance, listSuns[i], minDistance)) return true;
        }
        return false;
    }

    // Identifica si dos circumferencias se tocan
    bool CirclesIntersect(Vector2 center1, float radius1, Vector2 center2, float radius2)
    {
        // Calcula la distancia entre los centros de las circunferencias.
        float distancia = Vector2.Distance(center1, center2);

        // Compara la distancia con la suma de los radios.
        if (distancia <= radius1 + radius2)
        {
            // Las circunferencias se tocan o se superponen.
            return true;
        }
        else
        {
            // Las circunferencias no se tocan.
            return false;
        }
    }

    bool IsCirlceInRect(Vector2 center, float radio, Vector2 rectMin, Vector2 rectMax)
    {
        return (center.x >= rectMin.x + radio &&
                center.x <= rectMax.x - radio &&
                center.y >= rectMin.y + radio &&
                center.y <= rectMax.y - radio);
    }
}
