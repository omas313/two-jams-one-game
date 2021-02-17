using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyMember : BattleParticipant
{
    public override string Name => _name;
    public override CharacterStats CharacterStats => _characterStats;
    public override bool IsDead => _characterStats.CurrentHP <= 0;

    [SerializeField] string _name;
    [SerializeField] int _speed;
    [SerializeField] int _startingHp;
    
    CharacterStats _characterStats;
    Enemy _selectedEnemyToAttack;


    [ContextMenu("kill")]
    public void CM_Kill()
    {
        _characterStats.ReduceCurrentHP(_characterStats.CurrentHP);
    }

    public override IEnumerator ReceiveAttack(AttackDefinition attack)
    {
        // do animations and other stuff
        yield return new WaitForSeconds(0.25f);
        _characterStats.ReduceCurrentHP(attack.Damage);
    }

    public override IEnumerator PerformAction(List<PartyMember> playerParty, List<Enemy> enemies)
    {
        _selectedEnemyToAttack = null;

        yield return new WaitUntil(() => _selectedEnemyToAttack != null);

        yield return PerformAttack(_selectedEnemyToAttack);
    }

    private IEnumerator PerformAttack(BattleParticipant attackReceiver)
    {
        // do animations and other stuff
        var randomAttack = attacks[UnityEngine.Random.Range(0, attacks.Length)];
        yield return attackReceiver.ReceiveAttack(randomAttack);
        Debug.Log($"{Name} {randomAttack.Name} does {randomAttack.Damage} damage to {attackReceiver.Name}");
        yield return new WaitForSeconds(0.25f);    
    }

    public override IEnumerator Die()
    {
        Debug.Log($"{Name} is dying...");
        yield return new WaitForSeconds(0.5f);
        GetComponentInChildren<SpriteRenderer>().color = Color.black;
    }
    
    private void Awake()
    {
        _characterStats = new CharacterStats 
        { 
            CurrentSpeed = _speed,
            CurrentHP = _startingHp
        };

        BattleEvents.EnemyTargetSelected += OnEnemyTargetSelected;
    }

    private void OnEnemyTargetSelected(Enemy enemy) => _selectedEnemyToAttack = enemy;
}
