using UnityEngine;
using System.Collections.Generic;

//This SO Sets data for quests
[CreateAssetMenu(fileName = "QuestSO", menuName = "ScriptableObjects/QuestSO", order = 2)]
public class QuestSO : ScriptableObject
{
    public string questName;
    [TextArea] public string Questdescription;

    public List<QuestObjective> objectives;
}

[System.Serializable]
public class QuestObjective
{
    [TextArea] public string Description;
    public Object target;


    public Item_Data targetItem => target as Item_Data;
    public ActorSO targetActor => target as ActorSO;
    public LocationSO targetLocation => target as LocationSO;
    public int requiredAmount;

}