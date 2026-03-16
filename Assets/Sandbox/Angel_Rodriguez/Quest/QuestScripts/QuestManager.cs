using System;
using System.Collections.Generic;
using UnityEngine;

//Manages the player's quests and their progresson
public class QuestManager : MonoBehaviour
{
   private Dictionary<QuestSO, Dictionary<QuestObjective, int>> questProgress = new();
    [SerializeField] private PickUp_Manager Inventory;

    [SerializeField] private QuestLogUI questLogUI;


    void Start()
    {
        DontDestroyOnLoad(gameObject);
        Inventory = GameObject.FindGameObjectWithTag("PickUp_Manager").GetComponent<PickUp_Manager>();
    }

     void OnEnable()
    {
        QuestEvents.IsQuestCompleted += IsQuestComplete;
    }
    void OnDisable()
    {
        QuestEvents.IsQuestCompleted -= IsQuestComplete;
    }


void Update()
    {
        //This turns the UI on and Off
        if(Input.GetButtonDown("Off/OnUI"))
        {
            if(this.gameObject.GetComponent<Canvas>().enabled == false)
            {
                this.gameObject.GetComponent<Canvas>().enabled = true;
            }
            else
            {
                this.gameObject.GetComponent<Canvas>().enabled = false;
            }
        }
    }
   
   // This method updates the progress of a specific quest objective based on the player's inventory and discovered locations.
   public void updateQuestProgress(QuestSO quest, QuestObjective objective)
   {
    if(!questProgress.ContainsKey(quest)){
       questProgress[quest] = new Dictionary<QuestObjective, int>();
    }

    var progressDictionary = questProgress[quest];
    int newAmount = 0;

    if(objective.targetItem != null)
    {
         newAmount = Inventory.GetAmountOfItem(objective.targetItem);
    }
    else if(objective.targetLocation != null && Inventory.HasDiscoveredLocation(objective.targetLocation))
    {
        newAmount = objective.requiredAmount;        
    }

    /*
    else if(objective.targetActor != null && GameManager.s_Instance.Ac)
        {
            
            newAmount = objective.requiredAmount;
        }
    */

    progressDictionary[objective] = newAmount;
    
   }

   // Gets progress text for current quest your viewing in the quest log
   public string GetProgressText(QuestSO quest, QuestObjective objective)
   {
      int currentProgress = GetCurrentProgress(quest, objective);
      if(currentProgress == objective.requiredAmount)
      {
         return "Completed";
      }
      else if (objective.targetItem != null)
        {
            return $"{currentProgress} / {objective.requiredAmount}";
        }
        else
        {
            return "In Progress";
        }
   }

   // Gets the current progress for a specific quest objective.
   public int GetCurrentProgress(QuestSO quest, QuestObjective objective)
   {
        if(questProgress.TryGetValue(quest, out var progressDictionary))
        {
            if(progressDictionary.TryGetValue(objective, out int amount))
            {
                return amount;
            }
        }
        return 0;
   }

   public bool IsQuestComplete(QuestSO quest)
   {
        if(!questProgress.TryGetValue(quest, out var progressDictionary))
        {
             return false;
        }
        foreach(var objective in quest.objectives)
        {
            updateQuestProgress(quest, objective);
        }

        foreach(var objective in quest.objectives)
        {
            if(progressDictionary[objective] < objective.requiredAmount)
            {
                return false;
            }
        }
        return true;
   }
}
