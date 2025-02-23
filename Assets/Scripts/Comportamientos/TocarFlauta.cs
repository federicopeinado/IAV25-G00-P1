/*    
   Copyright (C) 2020-2025 Federico Peinado
   http://www.federicopeinado.com

   Este fichero forma parte del material de la asignatura Inteligencia Artificial para Videojuegos.
   Esta asignatura se imparte en la Facultad de Informática de la Universidad Complutense de Madrid (España).

   Autor: Federico Peinado 
   Contacto: email@federicopeinado.com
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UCM.IAV.Movimiento
{
    public class TocarFlauta : MonoBehaviour
    {
        public float radio = 5f;

        private List<GameObject> rats = new List<GameObject>();

        public GameObject efectoParticulaSuelo;
        public GameObject efectoParticulaAire;
        private GameObject particleSuelo = null;
        private GameObject particleAire = null;

        private SphereCollider trigger;
        bool isActive = false;
        //[SerializeField]
        //Separacion perroSepar;

        private AudioSource audioSource; // Se supone que ya hay un Component.audio que heredamos

        // Start is called before the first frame update
        void Start()
        {
            audioSource = transform.gameObject.GetComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.loop = true;

            trigger = transform.gameObject.AddComponent<SphereCollider>();
            trigger.isTrigger = true;
            trigger.enabled = false;
            trigger.radius = radio;
        }

        void Update()
        {
            // si tocamos la flauta
            if (Input.GetKeyDown(KeyCode.Space) && !isActive)
            {
                // activamos particulas
                activateParticle(ref particleSuelo, ref efectoParticulaSuelo);               
                activateParticle(ref particleAire, ref efectoParticulaAire);               
                
                // establecemos su escala y rotacion local
                particleSuelo.transform.localScale = new Vector3(radio * 2, radio * 2, radio * 2);
                particleSuelo.transform.localRotation = Quaternion.AngleAxis(90, new Vector3(1, 0, 0));

                isActive = true; // activamos el trigger
                trigger.enabled = true;

                audioSource.Play(); // activamos sonido de flauta
            }
            else if (Input.GetKeyUp(KeyCode.Space)) // si dejamos de tocar la flauta
            {
                isActive = false; // activamos el trigger
                trigger.enabled = false;

                destroyParticle(ref particleAire);
                destroyParticle(ref particleSuelo);

                // limpiamos la lista de ratas a las que le afecta el seguir al flautista
                foreach (GameObject rat in rats)
                    if (rat != null)
                        deactivateFollowing(rat);
                rats.Clear();

                audioSource.Pause(); // paramos sonido de flauta
            }
        }

        private void OnTriggerEnter(Collider ratColl)
        {
            // Se activa el seguimiento de las ratas al contacto con el trigger
            Merodear ratComp = ratColl.gameObject.GetComponent<Merodear>();
            if (isActive && ratComp != null && !rats.Contains(ratColl.gameObject))
            {
                activateFollowing(ratColl.gameObject);
                rats.Add(ratColl.gameObject);
            }
        }

        private void OnTriggerExit(Collider ratColl)
        {
            // Si las ratas salen del trigger, se reactivan sus comportamientos por defecto
            Merodear ratComp = ratColl.gameObject.GetComponent<Merodear>();
            if (isActive && ratComp != null && rats.Contains(ratColl.gameObject))
            {
                deactivateFollowing(ratColl.gameObject);
                rats.Remove(ratColl.gameObject);
            }
        }

        private void activateFollowing(GameObject rat)
        {
            // Activamos o desactivamos los comportamientos que ocurren si se toca la flauta
            rat.GetComponent<Merodear>().enabled = false;

            rat.GetComponent<Separacion>().enabled = true;
            //perroSepar.enabled = true;

            Llegada l = rat.GetComponent<Llegada>();

            if (l.objetivo == null)
                l.objetivo = transform.gameObject;
            l.enabled = true;
        }

        private void deactivateFollowing(GameObject rat)
        {
            // Activamos o desactivamos los comportamientos que ocurren si no se toca la flauta
            rat.GetComponent<Merodear>().enabled = true;
            rat.GetComponent<Llegada>().enabled = false;
            rat.GetComponent<Separacion>().enabled = false;
            //perroSepar.enabled = false;
        }

        private void activateParticle(ref GameObject particle, ref GameObject efecto)
        {
            particle = Instantiate(efecto, transform);
            particle.SetActive(true);
        }

        private void destroyParticle(ref GameObject particle)
        {
            if (particle != null)
            { // desactivamos las part�culas
                particle.transform.parent = null;
                Destroy(particle);
                particle = null;
            }
        }
    }
}