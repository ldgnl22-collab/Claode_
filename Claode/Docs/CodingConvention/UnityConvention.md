# Unity 개발 컨벤션 — v1.3

## 생명주기
- `Awake()`는 로컬 참조 초기화와 컴포넌트 캐싱에 사용한다.
- 이벤트는 기본적으로 `OnEnable()`에서 구독하고 `OnDisable()`에서 해제한다. 아래 이벤트 규칙을 따른다.
- `Start()`는 다른 객체의 `Awake()` 완료에 의존하는 초기화에 사용한다.
- `Update()`는 프레임 기반 로직과 입력 처리, `FixedUpdate()`는 Rigidbody 물리 처리에 사용한다.
- 명시적인 실행 순서 제어가 없다면 서로 다른 객체의 `Awake()` 실행 순서를 가정하지 않는다.

## 이벤트 구독 규칙 (v1.3 확정)
- 동일한 이벤트 소스와 핸들러로 `OnEnable()`에서 구독하고 `OnDisable()`에서 해제한다.
- 나중에 해제해야 하는 이벤트는 이름 있는 핸들러 메서드를 사용한다. 안전하게 해제할 수 없는 익명 람다 구독은 피한다.
- 컴포넌트가 반복 활성화/비활성화되어도 중복 구독이 쌓이지 않도록 한다.
- 이벤트 소스가 유효할 때만 구독한다. `OnEnable()` 이후 소스가 할당되거나 교체되면 교체 시점에 기존 구독 해제 및 새 구독을 처리한다.
- 비활성 상태에서도 이벤트를 받아야 한다면 이유를 문서화하고 `Awake()`/`OnDestroy()` 또는 명확한 소유 생명주기에 맞춰 관리한다.
- 발행자가 구독자보다 먼저 파괴될 수 있다면 생명주기를 안전하게 처리한다. `OnDisable()` 시점에 static 이벤트나 싱글톤 발행자가 항상 존재한다고 가정하지 않는다.
- Input System에서는 이벤트 콜백 구독과 Input Action의 활성화/비활성화를 구분한다. `InputAction`/`PlayerInput`의 소유권을 존중하고 다른 컴포넌트가 소유한 액션을 임의로 활성화/비활성화하지 않는다.

```csharp
using UnityEngine;

public class PlayerHealthView : MonoBehaviour
{
    [SerializeField] private PlayerHealth _playerHealth;

    private void OnEnable()
    {
        if (_playerHealth != null)
        {
            _playerHealth.HealthChanged += HandleHealthChanged;
        }
    }

    private void OnDisable()
    {
        if (_playerHealth != null)
        {
            _playerHealth.HealthChanged -= HandleHealthChanged;
        }
    }

    private void HandleHealthChanged(int currentHealth)
    {
        // UI 갱신
    }
}
```

## Input System
- 새로운 입력 코드는 Unity의 새 Input System (`UnityEngine.InputSystem`)을 사용한다.
- 신규 기능에 기존 `Input.GetAxis`, `Input.GetKey` 등의 레거시 API를 사용하지 않는다.
- 명시적 위임이 없다면 개발자가 Input Actions 및 키 바인딩 설정을 직접 관리한다.
- 액션 기반 입력이 필요한 작업에서 WASD나 Space 키를 코드에 하드코딩하여 폴링하지 않는다.
- 액션 콜백을 사용한다면 적절히 구독을 해제하고 중복 등록을 피한다.
- `PlayerInput`, 직렬화된 `InputActionReference`, 생성된 액션 래퍼 중 기존 프로젝트 구조에 맞는 방식을 선택한다. 이유 없이 여러 입력 패턴을 혼용하지 않는다.

## 물리와 이동
- 물리 기반 3D 객체에는 `Rigidbody`, 2D 객체에는 `Rigidbody2D` API를 사용한다.
- 일반적인 물리 이동에서 동적 Rigidbody의 Transform을 직접 변경하지 않는다.
- 적절한 프레임/업데이트 단계에서 입력을 읽거나 버퍼링하고 `FixedUpdate()`에서 물리 변경을 적용한다.
- 프레임 기반 이동에는 `Time.deltaTime`을 사용한다. `Time.fixedDeltaTime`은 선택한 API가 물리 단계당 수동 적분을 요구하는 경우에만 사용한다.
- 명확한 설계 없이 힘 기반, 속도 기반, Transform 기반 이동을 혼합하지 않는다.

## 참조 및 성능
### 컴포넌트 참조 규칙 (v1.2 확정)
- 같은 GameObject의 컴포넌트는 기본적으로 `Awake()`에서 `GetComponent<T>()`로 가져와 `_camelCase` private 필드에 저장한다.
- `Update()`, `FixedUpdate()` 또는 자주 호출되는 메서드에서 동일한 컴포넌트에 대해 `GetComponent<T>()`를 반복 호출하지 않는다.
- 필수적인 동일 객체 의존성에는 적절한 경우 `[RequireComponent(typeof(T))]`를 사용한다. 이것이 `Awake()` 캐싱을 대체하지는 않는다.
- 다른 GameObject, 프리팹, 의도적으로 지정하는 씬 컴포넌트 등 Inspector 할당이 필요한 참조는 `[SerializeField] private`을 사용한다.
- 런타임 생성, 동적 교체 또는 늦게 준비되는 컴포넌트는 실제로 유효해지는 생명주기 시점에 조회·갱신한다.
- `Awake()` 캐싱만으로 다른 GameObject의 `Awake()`가 이미 실행되었다고 가정하지 않는다.
- 기존 코드의 의존성 주입 또는 명시적 참조 패턴은 마이그레이션 요청이 없는 한 유지한다.

- 성능에 민감한 반복문에서는 불필요한 할당, LINQ, 문자열 연결, 반복 검색을 피한다(예측 가능하거나 측정된 경우).
- 빈번한 생성·파괴가 발생하고 필요성이 입증되면 오브젝트 풀링을 사용한다.
- 성급한 최적화를 피하고 프로파일링 결과 또는 알려진 제약이 없다면 가독성을 우선한다.
- MonoBehaviour가 다른 컴포넌트에 필수적으로 의존하면 적절한 경우 `[RequireComponent]`를 사용한다.

## Inspector와 직렬화
- 설정 가능한 필드는 `[SerializeField] private`을 우선한다.
- 직렬화 필드의 이름을 바꿔야 한다면 필요에 따라 `[FormerlySerializedAs]` 등을 사용해 마이그레이션을 고려한다.
- 필드명이나 타입을 바꾸더라도 기존 Inspector 데이터가 자동으로 보존된다고 가정하지 않는다.
- MonoBehaviour 생성자에서 Unity API를 호출하지 않는다.

## null 및 파괴된 객체
- `UnityEngine.Object` 참조에는 Unity 특유의 파괴된 객체 null 동작이 있음을 고려한다.
- 선택적 Inspector 참조가 누락되어 런타임 오류가 발생할 수 있다면 적절한 null 검사를 한다.
