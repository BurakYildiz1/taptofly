using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Skins/Skin Database", fileName = "SkinDatabase")]
public class SkinDatabase : ScriptableObject
{
    public List<SkinDefinition> skins = new();

    public SkinDefinition GetById(string id) => skins.FirstOrDefault(s => s.skinId == id);
    public IEnumerable<SkinDefinition> All => skins;
}
