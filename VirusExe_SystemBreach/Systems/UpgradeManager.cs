using System;
using VirusExe.SystemBreach.Characters;
using VirusExe.SystemBreach.Rendering;

namespace VirusExe.SystemBreach.Systems
{
    // MUTATION LAB 강화 처리
    // 재료 아이템을 사용해 ATK/HEALTH/ENERGY 영구 강화
    public class UpgradeManager
    {
        private readonly ConsoleRenderer renderer; // 화면 출력을하는 렌더러

        public UpgradeManager(ConsoleRenderer renderer)
        {
            this.renderer = renderer; // 전달받은 렌더러 저장
        }

        public void Open(Player player)
        {
            bool upgrading = true; // 강화 루프 유지 여부
            while (upgrading) // 강화 터미널 유지
            {
                int input = renderer.ShowSelectionModal("MUTATION LAB", new string[] // 강화 메뉴 출력
                {
                    "보유 MEMORY_SHARD: " + player.Inventory.GetCount(ItemNames.MemoryShard),
                    "보유 CORE_FRAGMENT: " + player.Inventory.GetCount(ItemNames.CoreFragment),
                    "보유 ENERGY_CORE  : " + player.Inventory.GetCount(ItemNames.EnergyCore)
                }, new string[]
                {
                    "ATK 강화       필요: MEMORY_SHARD x3   효과: ATK +2",
                    "HEALTH 강화    필요: CORE_FRAGMENT x3  효과: Max HEALTH +20",
                    "ENERGY 강화    필요: ENERGY_CORE x2    효과: Max ENERGY +10",
                    "창닫기"
                }, ConsoleColor.White, 3); // Q는 창닫기 처리

                if (input == 0)
                {
                    UpgradeAttack(player); // ATK 강화 처리
                }
                else if (input == 1)
                {
                    UpgradeStability(player); // HEALTH 강화 처리
                }
                else if (input == 2)
                {
                    UpgradeEnergy(player); // ENERGY 강화 처리
                }
                else if (input == 3)
                {
                    upgrading = false; // 강화 루프 종료
                }
            }
        }

        private void UpgradeAttack(Player player)
        {
            if (player.Inventory.Remove(ItemNames.MemoryShard, 3)) // 재료 체크
            {
                player.IncreaseAttack(2); // ATK 증가
                ShowResult("ATK이 영구적으로 2 증가했습니다.", true);
            }
            else
            {
                ShowResult("MEMORY_SHARD가 부족합니다.", false);
            }
        }

        private void UpgradeStability(Player player)
        {
            if (player.Inventory.Remove(ItemNames.CoreFragment, 3)) // 재료 체크
            {
                player.IncreaseMaxStability(20); // 최대 HEALTH 증가
                ShowResult("Max HEALTH가 영구적으로 20 증가했습니다.", true);
            }
            else
            {
                ShowResult("CORE_FRAGMENT가 부족합니다.", false);
            }
        }

        private void UpgradeEnergy(Player player)
        {
            if (player.Inventory.Remove(ItemNames.EnergyCore, 2)) // 재료 체크
            {
                player.IncreaseMaxEnergy(10); // 최대 ENERGY 증가
                ShowResult("Max ENERGY가 영구적으로 10 증가했습니다.", true);
            }
            else
            {
                ShowResult("ENERGY_CORE가 부족합니다.", false);
            }
        }

        private void ShowResult(string message, bool success)
        {
            renderer.ShowMessageBox(success ? "강화 완료" : "강화 실패", new string[] { message }, success ? ConsoleColor.Green : ConsoleColor.Red); // 결과 박스 출력
            renderer.WaitKey("Q 창닫기"); 
        }
    }
}
