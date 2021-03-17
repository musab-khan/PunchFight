// © 2015 Mario Lelas
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace MLSpace
{
    /// <summary>
    /// adds menu item that sets up package tags, layers and collision matrix
    /// </summary>
    public class RagdollManagerPackageSetup : MonoBehaviour
    {
        [MenuItem("Tools/Ragdoll Manager Package/Setup Tags Layers")]
        private static void Setup()
        {
            var defines = GetDefinesList(buildTargetGroups[0]);
            if (!defines.Contains("DEBUG_INFO"))
            {
                SetEnabled("DEBUG_INFO", true, false);
                SetEnabled("DEBUG_INFO", true, true);
            }


            const int PLAYERLAYER = 8;
            const int NPCLAYER = 9;
            const int COLLIDERLAYER = 10;
            const int COLLIDERINACTIVELAYER = 11;
            const int FIREBALLLAYER = 12;
            const int TRIGGERLAYER = 13;
            const int MINELAYER = 14;
            const int DYNAMICCOLLIDER = 15;

            EditorUtils.AddLayer("PlayerLayer", PLAYERLAYER);
            EditorUtils.AddLayer("NPCLayer", NPCLAYER);
            EditorUtils.AddLayer("ColliderLayer", COLLIDERLAYER);
            EditorUtils.AddLayer("ColliderInactiveLayer", COLLIDERINACTIVELAYER);
            EditorUtils.AddLayer("FireBallLayer", FIREBALLLAYER);
            EditorUtils.AddLayer("TriggerLayer", TRIGGERLAYER);
            EditorUtils.AddLayer("MineLayer", MINELAYER);
            EditorUtils.AddLayer("DynamicCollider", DYNAMICCOLLIDER);


            // trigger ignores all layers except npc and player for this case
            // fire ball layer ignores all but default
            for (int i = 0; i < 32; i++)
            {
                Physics.IgnoreLayerCollision(FIREBALLLAYER, i, true);
                Physics.IgnoreLayerCollision(TRIGGERLAYER, i, true);
                Physics.IgnoreLayerCollision(COLLIDERINACTIVELAYER, i, true);
                Physics.IgnoreLayerCollision(MINELAYER, i, true);
                Physics.IgnoreLayerCollision(DYNAMICCOLLIDER, i, true);
            }
            Physics.IgnoreLayerCollision(FIREBALLLAYER, 0, false);
            Physics.IgnoreLayerCollision(DYNAMICCOLLIDER, 0, false);
            Physics.IgnoreLayerCollision(FIREBALLLAYER, DYNAMICCOLLIDER, false);
            Physics.IgnoreLayerCollision(TRIGGERLAYER, NPCLAYER, false);
            Physics.IgnoreLayerCollision(TRIGGERLAYER, PLAYERLAYER, false);
            Physics.IgnoreLayerCollision(MINELAYER, COLLIDERLAYER, false);
            Physics.IgnoreLayerCollision(MINELAYER, COLLIDERINACTIVELAYER, false);
            Physics.IgnoreLayerCollision(DYNAMICCOLLIDER, COLLIDERLAYER, false);
            Physics.IgnoreLayerCollision(DYNAMICCOLLIDER, COLLIDERINACTIVELAYER, false);
            Physics.IgnoreLayerCollision(DYNAMICCOLLIDER, DYNAMICCOLLIDER, false);

            EditorUtils.AddTag("NPC");

            InputAxis idleAxis = new InputAxis();
            idleAxis.name = "Idle";
            idleAxis.positiveButton = "f";
            idleAxis.descriptiveName = "Toggle npcs idle mode";
            EditorUtils.AddAxis(idleAxis);

            InputAxis pauseAxis = new InputAxis();
            pauseAxis.descriptiveName = "Toggle pause";
            pauseAxis.name = "Pause";
            pauseAxis.positiveButton = "p";
            EditorUtils.AddAxis(pauseAxis);
        }





        private static BuildTargetGroup[] buildTargetGroups = new BuildTargetGroup[]
            {
                BuildTargetGroup.Standalone,
                BuildTargetGroup.Android,
                BuildTargetGroup.iOS,
            };

        private static BuildTargetGroup[] mobileBuildTargetGroups = new BuildTargetGroup[]
            {
                BuildTargetGroup.Android,
                BuildTargetGroup.iOS,
				BuildTargetGroup.PSM, 
				BuildTargetGroup.Tizen, 
				BuildTargetGroup.WSA 
            };


        /// <summary>
        /// get current defines list
        /// </summary>
        /// <param name="group">BuildTargetGroup group</param>
        /// <returns>defines list</returns>
        private static List<string> GetDefinesList(BuildTargetGroup group)
        {
            return new List<string>(PlayerSettings.GetScriptingDefineSymbolsForGroup(group).Split(';'));
        }

        /// <summary>
        /// set and enable define
        /// </summary>
        /// <param name="defineName"></param>
        /// <param name="enable"></param>
        /// <param name="mobile"></param>
        private static void SetEnabled(string defineName, bool enable, bool mobile)
        {
            foreach (var group in mobile ? mobileBuildTargetGroups : buildTargetGroups)
            {
                var defines = GetDefinesList(group);
                if (enable)
                {
                    if (defines.Contains(defineName))
                    {
                        return;
                    }
                    defines.Add(defineName);
                }
                else
                {
                    if (!defines.Contains(defineName))
                    {
                        return;
                    }
                    while (defines.Contains(defineName))
                    {
                        defines.Remove(defineName);
                    }
                }
                string definesString = string.Join(";", defines.ToArray());
                PlayerSettings.SetScriptingDefineSymbolsForGroup(group, definesString);
            }
        }
    } 
}
