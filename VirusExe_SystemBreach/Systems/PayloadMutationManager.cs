using VirusExe.SystemBreach.Characters;
using VirusExe.SystemBreach.Rendering;

namespace VirusExe.SystemBreach.Systems
{
    // 변이 선택 실행
    // Player의 PendingMutation 상태를 보고 변이 UI와 적용 흐름 처리
    public class PayloadMutationManager
    {
        private readonly ConsoleRenderer renderer;

        public PayloadMutationManager(ConsoleRenderer renderer)
        {
            this.renderer = renderer;
        }

        public bool TryRun(Player player)
        {
            if (player == null) return false; // 플레이어 체크
            if (!player.PendingMutation) return false; // 대기 변이 체크
            if (player.HasMutation) return false; // 이미 변이됨 체크

            renderer.PlayPayloadMutationDetectedSequence(); // 변이 감지 연출
            VirusMutation mutation = renderer.ShowPayloadMutationSelection(player); // 변이 선택
            bool applied = player.ApplyMutation(mutation); // 변이 적용

            if (applied) 
                renderer.PlayPayloadMutationCompleteSequence(mutation); 

            return applied;
        }
    }
}
