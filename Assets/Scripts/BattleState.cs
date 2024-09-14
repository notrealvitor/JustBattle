public abstract class BattleState
{
    protected CombatManager combatManager;

    public BattleState(CombatManager combatManager)
    {
        this.combatManager = combatManager;
    }

    public abstract void EnterState();
    public abstract void UpdateState();
}