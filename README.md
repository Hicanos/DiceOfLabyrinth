# DiceOfLabyrinth
가제: 미궁 다이스

## GitHub Commit Convention

GitHub Desktop에 Commit 시 사용하는 Commit 스타일 가이드입니다.


### 구성 요소 설명

*   **type:** 커밋의 종류를 나타냅니다.
    *   :white_check_mark:`Add`: 새로운 파일 추가
    *   :wastebasket:`Remove`: 기존 파일 삭제
    *   :sparkles:`Feat`: 새로운 기능 추가
    *   :hammer:`Fix`: 버그 수정
    *   :twisted_rightwards_arrows:`merge`: 머지 작업
    *   :rewind:`Revert`: 리버트
    *   :memo:`Docs`: 문서 수정
    *   :art:`Style`: 코드 포맷, 세미콜론 등 (코드 내용 변경 없음)
    *   :recycle:`Refactor`: 코드 리팩토링 (기능 변경 없음)
    *   :test_tube:`Test`: 테스트 코드 추가/수정
    *   :package:`Chore`: 빌드, 패키지 설정 등 기타 사항
    *   :camera_flash:`Design`: UI 디자인 변경 
    *   :rotating_light:`!HOTFIX`: 치명적인 버그 긴급 수정
*   **subject:** 변경 내용을 간결하게 요약합니다 (영문 작성 시 동사 원형 시작, 50자 이내, 마침표 없음 권장).
*   **body (본문):** 커밋 내용을 자세히 설명합니다 ("어떻게"보다는 "무엇을" "왜" 변경했는지 초점). 각 줄은 75자 이내 권장.
*   **footer (꼬리말):** : 필요 시 사용

### 커밋 메시지 예시

type: subject

body (optional)

footer (optional) 

```
Feat: Add player health system(한글 작성 가능)

플레이어 HP 바와 피해 처리가 구현되었습니다.
피해를 받으면 체력이 감소합니다.
체력이 0이 되면 게임 오버가 됩니다.
