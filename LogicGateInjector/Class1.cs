using UnityEngine;
using System;
using SFS.Parts.Modules;
using System.Collections.Generic;

namespace LogicGateInjector
{
    public class LogicGateModule : MonoBehaviour
    {
        public InputTrigger[] inputs;

        public GameObject[] outputs;

        public GateType gate;

        public void SetAllOutputs(bool value, float mult = 1f)
        {
            foreach (GameObject go in outputs)
            {
                if (go)
                    go.transform.localPosition = new Vector3(go.transform.localPosition.x, value ? 0f : (0.1f * mult), go.transform.localPosition.z);
            }
        }

        public void FixedUpdate()
        {
            switch (gate)
            {
                case GateType.Wire:
                    bool found = false;
                    foreach (var input in inputs)
                    {
                        if (input)
                        {
                            if (input.enab)
                            {
                                found = true;
                            }
                        }
                    }
                    SetAllOutputs(found);
                    break;

                case GateType.AND:
                    SetAllOutputs(inputs[0].enab && inputs[1].enab);
                    break;

                case GateType.OR:
                    SetAllOutputs(inputs[0].enab || inputs[1].enab);
                    break;

                case GateType.NOT:
                    SetAllOutputs(!inputs[0].enab);
                    break;

                case GateType.XOR:
                    SetAllOutputs(inputs[0].enab ^ inputs[1].enab);
                    break;

                case GateType.MagnetOutput:
                    bool foundInp = false;
                    foreach (var input in inputs)
                    {
                        if (input)
                        {
                            if (input.enab)
                            {
                                foundInp = true;
                            }
                        }
                    }
                    SetAllOutputs(foundInp, 20f);
                    break;
            }
        }
    }

    public enum GateType
    {
        AND,
        NOT,
        OR,
        XOR,
        Wire,
        Output,
        MagnetOutput
    }

    public class ExternalModule : MonoBehaviour
    {
        public int moduleType;

        public List<UnityEngine.Object> vars = new List<UnityEngine.Object>();
    }

    public class OutputModule : MonoBehaviour
    {
        public GameObject modelEnabled;
        public GameObject modelDisabled;

        public InputTrigger input;

        public void Update()
        {
            if (input.enab)
            {
                modelEnabled.SetActive(true);
                modelDisabled.SetActive(false);
            } else
            {
                modelEnabled.SetActive(false);
                modelDisabled.SetActive(true);
            }
        }
    }



    [RequireComponent(typeof(Rigidbody2D), typeof(CircleCollider2D))]
    public class InputTrigger : MonoBehaviour 
    {
        private Rigidbody2D rb2d;
        private int layer;
        private CircleCollider2D trigger;

        public bool enab;

        public int inputID;

        public LogicGateModule lgm;

        public int inputCount;
        

        private void Start()
        {
            rb2d = GetComponent<Rigidbody2D>();
            rb2d.isKinematic = true;
            trigger = GetComponent<CircleCollider2D>();
            layer = LayerMask.NameToLayer("Docking Trigger");

            if (lgm)
                lgm.inputs[inputID] = this;
        }

        public void FixedUpdate()
        {
            if (inputCount > 0)
                enab = true;
            else
                enab = false;
        }

        public void OnTriggerEnter2D(Collider2D other)
        {
            if (lgm)
                if (lgm.gate == GateType.MagnetOutput && other.GetComponentInChildren<DockingPortTrigger>() != null) 
                {
                    return;
                } // Stop the magnets from interfering with the DP converter input 
            inputCount ++;
        }
        public void OnTriggerExit2D(Collider2D other)
        {
            if (lgm)
                if (lgm.gate == GateType.MagnetOutput && other.GetComponentInChildren<DockingPortTrigger>() != null)
                {
                    return;
                } // Stop the magnets from interfering with the DP converter input 
            inputCount --;
        }
    }
}
