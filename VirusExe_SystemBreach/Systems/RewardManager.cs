using System;
using VirusExe.SystemBreach.Characters;
using VirusExe.SystemBreach.DataGrid;

namespace VirusExe.SystemBreach.Systems
{
    // 보상 지급
    // 전투/노드 보상으로 EXP, KB, 아이템 지급
    public class RewardManager
    {
        private readonly Random random = new Random(); // 보상 확률 계산 Random

        public RewardResult GiveBattleReward(Player player, Enemy enemy, NodeType nodeType, int column)
        {
            int kb = random.Next(enemy.KbMin, enemy.KbMax + 1); // 적 보상 범위 안에서 KB 결정
            player.AddKb(kb); // 플레이어에게 KB 지급

            string levelMessage = player.AddExp(enemy.ExpReward); // EXP 지급 및 레벨업 체크
            RewardResult itemReward = TryGiveNodeItemReward(player, nodeType, column, GetNodeDropChance(nodeType)); // 노드 아이템 보상

            string itemName = itemReward.HasItem ? itemReward.ItemName : null; // 아이템 코드명 추출
            return new RewardResult(enemy.ExpReward, kb, itemName, levelMessage); // 전투 보상 결과
        }

        public RewardResult TryGiveNodeItemReward(Player player, NodeType nodeType, int column, int chancePercent)
        {
            if (player == null) return RewardResult.Empty(); // 플레이어 누락 방지
            if (chancePercent <= 0) return RewardResult.Empty(); // 확률 없음
            if (nodeType == NodeType.Boss) return RewardResult.Empty(); // 보스는 엔딩 전투라 드랍 제외

            int roll = random.Next(0, 100); // 드랍 주사위
            if (roll >= chancePercent) // 확률 실패 체크
            {
                return RewardResult.Empty(); // 보상 없음
            }

            string itemName = ItemPoolManager.RollNodeRewardName(random, column, player.Mutation); // 열 기준 보상 선택
            if (string.IsNullOrEmpty(itemName)) // 후보 없음 체크
            {
                return RewardResult.Empty(); // 보상 없음
            }

            player.Inventory.Add(itemName, 1); // 인벤토리에 지급
            return RewardResult.ItemOnly(itemName); // 아이템 보상 결과
        }

        private int GetNodeDropChance(NodeType nodeType)
        {
            if (nodeType == NodeType.Firewall) return 60; // FW 보상 확률
            if (nodeType == NodeType.Security) return 35; // SEC 보상 확률
            if (nodeType == NodeType.DataCache) return 70; // DAT 보상 확률
            if (nodeType == NodeType.Event) return 50; // EVT 기본 보상 확률
            return 0; // 기본 드랍 없음
        }
    }
}
