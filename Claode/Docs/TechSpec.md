# Tech Specification

> 이 프로젝트의 기술 스택 관련 결정 사항이다.
> Claude Code는 코드를 작성할 때 이 문서를 기준으로 한다.

---

## 프로젝트 제원

- 프로젝트명 : UnityAISetting
- 장르 : TPS
- 팀 구성 : 1인

## Unity 환경
- 버전 : 6000.3.9f1 LTS
- 렌더 파이프라인 : URP
- 빌드 플랫폼 : window

## 유저 입력 시스템

- new InputSystem
  - 구 `InputManager`는 사용하지 않는다.
- 특정 입력에 재한 기는 구현 시 `InputActionReference`를 사용해 InputAction을 참조한는 형태로 구현한다.
- OnEnable에서 InputAction에 대한 Enable(), OnDisable()을 사용한다.