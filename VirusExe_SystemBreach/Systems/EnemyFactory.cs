using System;
using System.Collections.Generic;
using VirusExe.SystemBreach.Characters;

namespace VirusExe.SystemBreach.Systems
{
    // 몬스터 생성
    // 노드 타입과 TRACE에 맞춰 일반/엘리트/보스 Enemy 생성
    public class EnemyFactory
    {
        private readonly Random random = new Random(); // 적 종류 선택에 사용할 Random 객체
        private readonly List<string> normalEnemyBag = new List<string>(); // 중복 없는 일반 몬스터 랜덤 풀
        private readonly List<string> eliteEnemyBag = new List<string>(); // 중복 없는 엘리트 몬스터 랜덤 풀

        private readonly string[] normalEnemyNames = new string[] // 일반 몬스터 후보 목록
        {
            "SCAN_DAEMON",
            "MEM_LEAK_ANOMALY",
            "LOGIC_BOMB",
            "NULL_POINTER_VOID",
            "PROTOCOL_MUNCHER",
            "SANDBOX_ISOLATION",
            "CIPHER_BLOCK_CHAIN"
        };


        private readonly string[] eliteEnemyNames = new string[] // 엘리트 몬스터 후보 목록
        {
            "PROXY_SINGULARITY",
            "SYN_FLOOD_GATE",
            "IC_CRYPTO_GATE"
        };

        public Enemy CreateSecurityProcess(int systemInfection)
        {
            int bonus = systemInfection / 8; // 추적도에 따른 능력치 보너스
            string enemyName = GetNextNormalEnemyName(); // 중복 없는 랜덤 몬스터 선택

            switch (enemyName) // 일반 몬스터별 기본 능력치
            {
                case "SCAN_DAEMON":
                    return new Enemy(enemyName, 65 + bonus * 4, 10 + bonus, 16 + bonus, 36, 28, 44, false, false); // 기본 스캐너

                case "MEM_LEAK_ANOMALY":
                    return new Enemy(enemyName, 70 + bonus * 4, 8 + bonus, 14 + bonus, 38, 30, 46, false, false); // 지속 피해형 메모리 누수

                case "LOGIC_BOMB":
                    return new Enemy(enemyName, 60 + bonus * 3, 12 + bonus, 19 + bonus, 40, 32, 48, false, false); // 공격형 논리 폭탄

                case "NULL_POINTER_VOID":
                    return new Enemy(enemyName, 72 + bonus * 4, 9 + bonus, 17 + bonus, 42, 34, 50, false, false); // TRACE 위협형 공허 프로세스

                case "PROTOCOL_MUNCHER":
                    return new Enemy(enemyName, 68 + bonus * 4, 11 + bonus, 18 + bonus, 40, 32, 50, false, false); // 패킷 포식자

                case "SANDBOX_ISOLATION":
                    return new Enemy(enemyName, 78 + bonus * 5, 7 + bonus, 13 + bonus, 42, 35, 52, false, false); // 격리형 방어 프로토콜

                case "CIPHER_BLOCK_CHAIN":
                    return new Enemy(enemyName, 74 + bonus * 5, 9 + bonus, 15 + bonus, 44, 36, 54, false, false); // 암호화 체인 방어 모듈
            }

            return new Enemy("SCAN_DAEMON", 65 + bonus * 4, 10 + bonus, 16 + bonus, 36, 28, 44, false, false); // 예외 기본값
        }

        public Enemy CreateFirewall(int systemInfection)
        {
            int bonus = systemInfection / 5; // 추적도에 따른 엘리트 보너스
            string enemyName = GetNextEliteEnemyName(); // 중복 없는 랜덤 엘리트 선택

            switch (enemyName) // 엘리트 몬스터별 기본 능력치
            {
                case "PROXY_SINGULARITY":
                    return new Enemy(enemyName, 135 + bonus * 5, 15 + bonus, 25 + bonus, 85, 75, 110, true, false); // 프록시 특이점

                case "SYN_FLOOD_GATE":
                    return new Enemy(enemyName, 145 + bonus * 5, 14 + bonus, 23 + bonus, 88, 78, 115, true, false); // SYN Flood 게이트

                case "IC_CRYPTO_GATE":
                    return new Enemy(enemyName, 138 + bonus * 5, 18 + bonus, 29 + bonus, 90, 80, 118, true, false); // 암호화 IC 게이트
            }

            return new Enemy("PROXY_SINGULARITY", 135 + bonus * 5, 15 + bonus, 25 + bonus, 85, 75, 110, true, false); // 예외 기본값
        }

        public Enemy CreateBoss(int systemInfection)
        {
            int bonus = systemInfection / 3; // 추적도에 따라 보스가 강해집니다
            return new Enemy("KERNEL_CORE", 480 + bonus * 4, 20 + bonus, 34 + bonus, 0, 0, 0, true, true); // 보스 바이러스를
        }

        private string GetNextNormalEnemyName()
        {
            if (normalEnemyBag.Count == 0) // 모든 일반 몬스터를 한 번씩 사용했는지 체크
            {
                normalEnemyBag.AddRange(normalEnemyNames); // 랜덤 풀 초기화
                ShuffleNormalEnemyBag(); // 새 사이클 셔플
            }

            string enemyName = normalEnemyBag[0]; // 다음 몬스터 선택
            normalEnemyBag.RemoveAt(0); // 사용한 몬스터 제거
            return enemyName;
        }

        private string GetNextEliteEnemyName()
        {
            if (eliteEnemyBag.Count == 0) // 모든 엘리트 몬스터를 한 번씩 사용했는지 체크
            {
                eliteEnemyBag.AddRange(eliteEnemyNames); // 엘리트 랜덤 풀 초기화
                ShuffleEliteEnemyBag(); // 새 사이클 셔플
            }

            string enemyName = eliteEnemyBag[0]; // 다음 엘리트 선택
            eliteEnemyBag.RemoveAt(0); // 사용한 엘리트 제거
            return enemyName;
        }

        private void ShuffleNormalEnemyBag()
        {
            for (int i = normalEnemyBag.Count - 1; i > 0; i--) // Fisher-Yates 셔플
            {
                int j = random.Next(0, i + 1); // 교환 대상 선택
                string temp = normalEnemyBag[i]; // 임시 저장
                normalEnemyBag[i] = normalEnemyBag[j]; // 위치 교환
                normalEnemyBag[j] = temp; // 위치 교환 완료
            }
        }

        private void ShuffleEliteEnemyBag()
        {
            for (int i = eliteEnemyBag.Count - 1; i > 0; i--) // Fisher-Yates 셔플
            {
                int j = random.Next(0, i + 1); // 교환 대상 선택
                string temp = eliteEnemyBag[i]; // 임시 저장
                eliteEnemyBag[i] = eliteEnemyBag[j]; // 위치 교환
                eliteEnemyBag[j] = temp; // 위치 교환 완료
            }
        }
    }
}
