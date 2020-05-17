using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "GameCreationWizardEditorProfile", menuName = "CreationWizard/GameCreationWizardEditorProfile", order = 1)]
public class GameCreationWizardEditorProfile : TreeChoiceHeaderTab<ICreationWizardEditor>
{
    private Dictionary<string, ICreationWizardEditor> configurations;

    public override Dictionary<string, ICreationWizardEditor> Configurations => this.configurations;


    private void OnEnable()
    {
        if (configurations != null) { this.configurations.Clear(); }
        else { this.configurations = new Dictionary<string, ICreationWizardEditor>(); }

        var creationWizardTypes =
                typeof(ICreationWizardEditor)
                            .Assembly.GetTypes()
                            .Where(t => typeof(ICreationWizardEditor).IsAssignableFrom(t) && !t.IsAbstract && !t.IsInterface)
                            .ToList();
        foreach (var creationWizardType in creationWizardTypes)
        {
            this.Configurations.Add(creationWizardType.Name, (ICreationWizardEditor)Activator.CreateInstance(creationWizardType));
        }
    }
}
