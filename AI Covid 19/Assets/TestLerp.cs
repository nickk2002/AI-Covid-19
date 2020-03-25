using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestLerp : MonoBehaviour
{
    public float scaleTime = 3f; /// cat timp sa ii ia sa ajunga la scale-ul dorit
    public Vector3 desiredScale = new Vector3(5, 5, 5); /// ce scale vrei sa aiba la final
    IEnumerator ScaleObject()
    {
        float initialTime = Time.time;// iau timpul initial intr-o variabila float  
        Vector3 initialScale = transform.localScale;/// iau scale-ul initial

        while(Time.time - initialTime < scaleTime)/// atata timp cat nu au trecut scaleTime secunde
        {
            transform.localScale = Vector3.Lerp(initialScale, desiredScale, (Time.time - initialTime) / scaleTime);
            yield return null;
            // asteapta un frame
        }
        /// la final nu este chiar sigur ca am ajuns exat la scalul care trebuia asa ca fac direct
        transform.localScale = desiredScale;

    }
    void ScaleObjectWhileInfinit()
    {
        float initialTime = Time.time;// iau timpul initial intr-o variabila float  
        Vector3 initialScale = transform.localScale;/// iau scale-ul initial

        while (Time.time - initialTime < scaleTime)/// atata timp cat nu au trecut scaleTime secunde
        {
            transform.localScale = Vector3.Lerp(initialScale, desiredScale, (Time.time - initialTime) / scaleTime);
            // nu asteapta un frame, Time.time se actualizeaza undeva in spate per frame asa ca while-ul o sa dea crash la unity
            // salveaza scena inainte sa faci rulezi codul
        }
        /// la final nu este chiar sigur ca am ajuns exat la scalul care trebuia asa ca fac direct
        transform.localScale = desiredScale;
    }
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(ScaleObject());/// chem corutina de scalat
        // nu rula codul decat daca esti sigur ca vrei sa dai crash 
        // nu rula daca nu ai salvat tot din unity ScaleObjectWhileInfinit();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
