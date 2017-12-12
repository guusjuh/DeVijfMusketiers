#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(BossParticleTest))]
public class BossParticleTestEditor : Editor {

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        BossParticleTest bpt = (BossParticleTest)target;

        if(GUILayout.Button("Spawn Boss"))
        {
            if(bpt.chooseBoss == BossParticleTest.Bosses.Arnest)
            {
                bpt.SpawnBoss(BossParticleTest.Bosses.Arnest, new Vector2(0, 0), Quaternion.identity);
                bpt.SpawnParticle(BossParticleTest.Particles.arnestParticle, new Vector2(0, 0), Quaternion.identity);
            }

            if (bpt.chooseBoss == BossParticleTest.Bosses.Sketta)
            {
                bpt.SpawnBoss(BossParticleTest.Bosses.Sketta, new Vector2(0, 0), Quaternion.identity);
                bpt.SpawnParticle(BossParticleTest.Particles.skettaParticle, new Vector2(0, 0), Quaternion.identity);
            }
             
        }
    }
}
#endif
