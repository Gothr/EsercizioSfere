using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using UnityEngine;
using UnityEngine.Networking;


public class Manager : MonoBehaviour
{
    [SerializeField] private GameObject sferaPrefab;
    public Queue<GameObject> sfereCounter;
    public DatiSfere elementi;


    private void Start()
    {
        sfereCounter = new Queue<GameObject>();


        try
        {
            HttpWebRequest request = WebRequest.CreateHttp("http://localhost:3000/GiocoSfere");
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                elementi = JsonUtility.FromJson<DatiSfere>(reader.ReadToEnd());
            }

            foreach (var e in elementi.elementi)
            {
                if (!e.dead)
                {
                    GameObject nuovaSfera = Instantiate(sferaPrefab);
                    nuovaSfera.transform.position = new Vector3(e.x, e.y, 0);
                    Debug.Log($"Index:{e.index}, x:{e.x}, y:x:{e.y}, Dead:{e.dead}");
                    sfereCounter.Enqueue(nuovaSfera);
                }
                else
                {
                    Debug.Log($"Sphere {e.index} is dead.");
                }

            }
        }
        catch (Exception e)
        {
            Debug.Log("Impossibile connettersi " + e);
        }
    }

    private void Update()
    {
        GameObject sferaDaEliminare;
        int lunghezzaQueue = sfereCounter.Count();

        if (Input.GetKeyDown(KeyCode.X))
        {
            sferaDaEliminare = sfereCounter.Dequeue();

            foreach (var e in elementi.elementi)
            {
                if (!e.dead)
                {
                    if (e.index == lunghezzaQueue - 1)
                    {
                        Destroy(sferaDaEliminare);

                        StartCoroutine(SendDataToServer(e.index));
                    }
                }
            }
        }
    }

    IEnumerator SendDataToServer(int index)
    {
        Dictionary<string, string> dati =
            new Dictionary<string, string>();

        dati.Add("index", index.ToString());


        UnityWebRequest webRequest =
            UnityWebRequest.Post("http://localhost:3000/set_dead", dati);

        yield return webRequest.SendWebRequest();

        Debug.Log(webRequest.downloadHandler.text);
    }
}
