using System.Reflection;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using LogicGateInjector;
using SFS.World;
using SFS.Variables;
using System;
using Object = UnityEngine.Object;
using SFS.Parts.Modules;
using Cysharp.Threading.Tasks.Triggers;

namespace LogicGateModules
{
    public abstract class CustomModuleBase : MonoBehaviour
    {
        public List<Object> vars = new List<Object>();
        public abstract int Id();
        public void SetOutputs(bool value, float multiplier = 1f)
        {
            for (int i = 0; i < Mathf.Min(vars.Count, 4); i++) // by default, the first 4 variables should be outputs as GOs
            {
                if (vars[i] is GameObject go && go != null)
                {
                    go.transform.localPosition = new Vector3(go.transform.localPosition.x, value ? 0f : (0.1f * multiplier), go.transform.localPosition.z);
                }
            }
        }
        public bool[] GetInputs()
        {
            List<bool> result = new List<bool>();
            for (int i = 0; i < vars.Count; i++)
            {
                var variable = vars[i];
                if (variable is InputTrigger it && variable != null)
                {
                    result.Add(it.enab);
                }
            }
            return result.ToArray();
        }
    }

    public class AltimeterModule : CustomModuleBase
    {
        // variables:
        // 0-3: output(s)
        // 4: VariablesModule

        Player player;
        public override int Id() => 1;

        public void Update()
        {
            try
            {
                player = PlayerController.main.player.Value;
                if (player != null)
                {
                    VariablesModule vm = GetComponentInChildren<VariablesModule>();
                    double height = player.location.Value.Height;

                    SetOutputs(height > vm.doubleVariables.GetVariable("minHeight").Value && height < vm.doubleVariables.GetVariable("maxHeight").Value);
                }
            }
            catch (Exception) { }
        }
    }

    public class LogicEngineModule : CustomModuleBase
    {
        public override int Id() => 2;
        public EngineModule em;
        public bool eo;

        public void Update()
        {
            em = GetComponent<EngineModule>();

            bool canRun = true;

            foreach (FlowModule.Flow flow in em.source.sources)
            {
                if (!(flow.state == FlowModule.FlowState.CanFlow || flow.state == FlowModule.FlowState.IsFlowing))
                {
                    canRun = false;
                    break;
                }
            }

            em.engineOn.Value = GetInputs()[0] && canRun;
        }
    }
}
