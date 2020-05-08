using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;

public class WinFlasher : MonoBehaviour
{

    private class Enumerator3DResult<T>
    {
        public Vector3 Coordinates;
        public T Item;
    }

    public GameObject winDetectorObject;
    public bool useWinDetector = true;
    public float flashInterval = 1;
    public GameObject baseLight;

    public Vector3Int counts = new Vector3Int(3, 1, 3);
    public Vector3 offset = new Vector3(1, 1, 1);

    public Color xVariance = new Color(0.1f, 0, 0);
    public Color yVariance = new Color(0, 0.1f, 0);
    public Color zVariance = new Color(0, 0, 0.1f);

    private GameObject[][][] baseLights;
    private WinConditionIndicator winDetector;

    // Start is called before the first frame update
    void Start()
    {
        if (useWinDetector)
        {
            winDetector = this.winDetectorObject?.GetComponent<WinConditionIndicator>();
        }

        baseLights = new GameObject[counts.x][][];
        for(var x = 0; x < counts.x; x++)
        {
            baseLights[x] = new GameObject[counts.y][];
            for (int y = 0; y < counts.y; y++)
            {
                baseLights[x][y] = new GameObject[counts.z];
                for (int z = 0; z < counts.z; z++)
                {
                    var newLight = Instantiate(baseLight, this.transform);
                    var singleOffset = new Vector3(x, y, z);
                    newLight.transform.localPosition = getLightLocationFromCoords(singleOffset);

                    var newColor = baseLight.GetComponent<Light>().color +
                        xVariance * x + yVariance * y + zVariance * z;
                    newLight.GetComponent<Light>().color = newColor;

                    baseLights[x][y][z] = newLight;
                }
            }
        }
    }

    private Vector3 getLightLocationFromCoords(Vector3 coords)
    {
        var resultCoords = new Vector3(coords.x, coords.y, coords.z);
        resultCoords.Scale(offset);
        return resultCoords;
    }

    private IEnumerable<Vector3Int> iterateLightCoords()
    {
        for (var x = 0; x < counts.x; x++)
        {
            for (int y = 0; y < counts.y; y++)
            {
                for (int z = 0; z < counts.z; z++)
                {
                    yield return new Vector3Int(x, y, z);
                }
            }
        }
    }

    private IEnumerable<Enumerator3DResult<Light>> iterateLights()
    {
        return iterateLightCoords()
            .Select(vector => new Enumerator3DResult<Light>
            {
                Item = baseLights[vector.x][vector.y][vector.z].GetComponent<Light>(),
                Coordinates = vector
            })
            .Where(light => light?.Item != null);
    }

    void Update()
    {
        if (!winDetector)
        {
            return;
        }
        if (winDetector.isWinning)
        {
            this.setLightEnabled((int)(Time.time / flashInterval) % 2 == 0);
        } else
        {
            this.setLightEnabled(false);
        }
    }

    private void setLightEnabled(bool enabled)
    {
        foreach (var light in this.iterateLights())
        {
            light.Item.enabled = enabled;
        }
    }

    private void OnDrawGizmosSelected()
    {
        var globalCoords = this.iterateLightCoords()
            .Select(coord => this.getLightLocationFromCoords(coord))
            .Select(localPosition => this.transform.TransformPoint(localPosition));
        foreach (var globalCoord in globalCoords)
        {
            Gizmos.DrawSphere(globalCoord, 0.5f);
        }
    }
}
