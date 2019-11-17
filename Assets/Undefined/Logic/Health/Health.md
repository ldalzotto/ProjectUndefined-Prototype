# Health

## Health UI

````puml
skinparam node {
    backgroundColor<<InteractiveObject>> orange
    backgroundColor<<System>> yellow
    backgroundColor<<PlayerAction>> cyan
    backgroundColor<<Manager>> plum
}

package ListenedInteractiveObject {
    node InteractiveObject <<InteractiveObject>>
    node HealthSystem <<System>>
}
node HealthUIManager <<Manager>>

InteractiveObject -> HealthSystem
InteractiveObject --> HealthUIManager : register object \n for event listening
HealthSystem --> HealthUIManager : OnHealthValueChanged

````