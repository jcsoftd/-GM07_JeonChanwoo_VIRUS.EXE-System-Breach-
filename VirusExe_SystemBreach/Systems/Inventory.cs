using System.Collections.Generic;

namespace VirusExe.SystemBreach.Systems
{
    // 아이템 보유 수량 관리
    // 아이템 이름 기준으로 수량 추가/삭제/조회 처리
    public class Inventory
    {
        private readonly Dictionary<string, int> items = new Dictionary<string, int>(); // 아이템 이름별 보유 수량

        public void Add(string itemName, int amount)
        {
            if (amount <= 0) // 추가 수량이 0 이하인지 체크
            {
                return; // 잘못된 수량이면 중단
            }
            if (!items.ContainsKey(itemName)) // 아직 등록되지 않은 아이템인지 체크
            {
                items[itemName] = 0; // 처음 등록되는 아이템 수량 초기화
            }
            items[itemName] += amount; // 기존 수량에 추가 수량 반영
        }

        public bool Remove(string itemName, int amount)
        {
            if (!Has(itemName, amount)) // 필요한 수량을 가지고 있는지 체크
            {
                return false; // 부족하면 실패
            }
            items[itemName] -= amount; // 아이템 수량 감소
            if (items[itemName] <= 0) // 수량이 0 이하인지 체크
            {
                items.Remove(itemName); // 수량이 없으면 목록에서 제거
            }
            return true; // 제거 성공
        }

        public bool Has(string itemName, int amount)
        {
            if (!items.ContainsKey(itemName)) // 아이템이 없는지 체크
            {
                return false; // 없으면 false
            }
            return items[itemName] >= amount; // 수량 충분 여부
        }

        public int GetCount(string itemName)
        {
            if (!items.ContainsKey(itemName)) // 아이템이 없는지 체크
            {
                return 0; // 없으면 0개
            }
            return items[itemName]; // 저장 수량
        }

        public Dictionary<string, int> GetSnapshot()
        {
            return new Dictionary<string, int>(items); // 내부 딕셔너리 복사본
        }
    }
}
