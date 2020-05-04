using UnityEngine;

namespace LowPolyHnS.Characters
{
#if UNITY_EDITOR
    using UnityEditor;

#endif

    [CreateAssetMenu(
        fileName = "Animatio Clip Group",
        menuName = "LowPolyHnS/Developer/Animation Clip Group",
        order = 200
    )]
    public class AnimationClipGroup : ScriptableObject
    {
        public string message = "";

#if UNITY_EDITOR
        public void AddAnimationClip(AnimationClip original)
        {
            AnimationClip clip = Instantiate(original);
            clip.name = original.name;

            AssetDatabase.AddObjectToAsset(clip, this);
            string path = AssetDatabase.GetAssetPath(this);
            AssetDatabase.ImportAsset(path);
        }
#endif
    }
}