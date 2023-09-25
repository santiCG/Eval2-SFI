using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public class Boton : MonoBehaviour
{
    public Camera camaraEscena;
    public GameObject menuConfig;
    public GameObject hud;

    public Button boton;

    public bool configurado = false;

    // Start is called before the first frame update
    void Start()
    {
        boton.onClick.AddListener(cambiarPosicionCamara);
        hud.SetActive(false);
    }

    void cambiarPosicionCamara()
    {
        camaraEscena.transform.position = new Vector3(76, 3, -28);
        menuConfig.SetActive(false);
        hud.SetActive(true);

        configurado = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
