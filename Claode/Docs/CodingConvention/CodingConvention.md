# C# 코딩 컨벤션 — v1.5

아래는 일반적인 C# 및 Unity 관례를 바탕으로 정한 **프로젝트 기본 규칙**이며, Unity 엔진이 강제하는 규칙은 아니다.

## 이름 지정 규칙
| 요소 | 규칙 | 예시 |
|---|---|---|
| 네임스페이스 | PascalCase | `Game.Player` |
| 클래스 / 구조체 / 열거형 | PascalCase | `PlayerMovement`, `PlayerState` |
| 인터페이스 | `I` + PascalCase | `IDamageable` |
| 메서드 / 프로퍼티 / 이벤트 | PascalCase | `Move`, `MoveSpeed`, `OnDamaged` |
| 매개변수 / 지역 변수 | camelCase | `moveDirection` |
| private 인스턴스 필드 | `_camelCase` | `_moveSpeed` |
| private static 필드 | `s_camelCase` | `s_instance` |
| 상수 | PascalCase | `MaxHealth` |

## 확정 규칙 (v1.1)
- **private 인스턴스 필드는 `_camelCase`를 사용한다.** `[SerializeField] private` 필드도 동일하다.
- **짧고 명확한 단일 표현식 메서드에는 `=>`를 허용한다.** 의무는 아니다.

## 필드와 프로퍼티
- 필드는 기본적으로 `private`으로 선언하고 Inspector 노출에는 `[SerializeField] private`을 사용한다.
- 다른 객체가 값을 읽어야 한다면 public 읽기 전용 프로퍼티를 우선한다.
- 컴파일 시점 상수는 `const`, 초기화 후 변경되지 않는 필드는 적절한 경우 `readonly`를 사용한다.
- Unity 직렬화 또는 API상 명확한 이유가 없다면 외부에서 변경 가능한 public 필드를 피한다.

## 서식
- 들여쓰기는 공백 4칸, 중괄호는 Allman 스타일(여는 중괄호를 다음 줄에 배치)을 사용한다.
- `if`, `for`, `foreach`, `while`은 한 줄짜리 본문이라도 중괄호를 사용한다.
- 짧고 자명한 단일 표현식 메서드에는 `=>`를 허용한다. 여러 문장이나 분기가 있거나 가독성이 더 좋다면 블록 본문을 사용한다. 기존 메서드를 무조건 변환하지 않는다.
- 파일당 주요 타입 하나를 두고 파일명은 public MonoBehaviour/타입 이름과 일치시킨다.
- 주석은 코드 내용을 반복하지 않고 의도, 제약, 비자명한 판단을 설명한다.
- 외부 사용자가 계약이나 동작을 이해해야 하는 공개 API에는 XML 문서 주석을 사용한다.

## 메서드
- 메서드는 명확한 한 가지 책임을 갖고 동사 중심으로 이름을 짓는다.
- 가독성이 좋아진다면 조기 반환(early return)으로 중첩을 줄인다.
- 잘못된 값이 실제로 전달될 수 있는 외부 입력을 검증한다.
- 의미 없는 숫자 리터럴(magic number) 대신 이름 있는 상수나 직렬화 설정을 사용한다.

## 기본 예시
```csharp
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 5f;

    public float MoveSpeed => _moveSpeed;

    public void Move(Vector3 direction)
    {
        if (direction.sqrMagnitude < 0.0001f)
        {
            return;
        }

        transform.position += direction.normalized * (_moveSpeed * Time.deltaTime);
    }
}
```
이 코드는 스타일 설명용이다. 실제 이동 API는 프로젝트의 물리 및 게임플레이 요구사항에 따라 선택한다.

## 표현식 본문 예시
```csharp
private void Awake() => CacheComponents();

private void CacheComponents()
{
    _rigidbody = GetComponent<Rigidbody>();
}
```

## 캡슐화와 프로퍼티 접근 (v1.4)
- 인스턴스 필드는 기본적으로 `private` 및 `_camelCase`로 선언한다.
- Inspector 수정 값은 public 필드 대신 `[SerializeField] private`으로 선언한다.
- 다른 클래스에서 읽어야 할 값은 `public int CurrentHealth => _currentHealth;`와 같은 읽기 전용 프로퍼티를 제공한다.
- 상태 변경은 public setter 대신 `TakeDamage(int amount)`처럼 의도가 명확한 메서드를 우선한다.
- Unity 직렬화, 필수 API, DTO, 상호운용성이 필요한 경우 합당한 예외를 허용한다.
- 승인 없이 기존 직렬화 필드나 공개 API의 이름·형식을 바꾸지 않는다. 참조가 깨질 수 있다.

```csharp
[SerializeField] private int _maxHealth = 100;
private int _currentHealth;

public int MaxHealth => _maxHealth;
public int CurrentHealth => _currentHealth;

public void TakeDamage(int amount)
{
    if (amount <= 0)
    {
        return;
    }

    _currentHealth = Mathf.Max(0, _currentHealth - amount);
}
```

## 멤버 선언 순서 (v1.5)
새 C# 클래스는 가독성 또는 Unity 특성상 명확한 예외가 없는 한 다음 순서를 따른다.
1. 상수 및 static 필드 (`const`, `static`, `static readonly`)
2. Inspector 직렬화 private 필드 (`[SerializeField] private`)
3. 기타 private 인스턴스 필드
4. 프로퍼티 및 이벤트
5. Unity 생명주기 메서드 (`Awake`, `OnEnable`, `Start`, `Update`, `FixedUpdate`, `OnDisable`, `OnDestroy` 등)
6. public 메서드
7. private 메서드

각 그룹 안에서는 관련 멤버를 모으고 일관된 순서를 사용한다. 빈 `#region`이나 자리 표시용 섹션을 만들지 않는다. 집중 수정 중 무관한 기존 멤버를 재정렬하지 않는다. 선언에 붙은 특성(attribute)을 유지한다. override, 인터페이스 구현, 연관 메서드를 함께 두는 것이 더 명확하면 가독성을 우선하고 중요한 예외는 설명한다.

```csharp
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    private const float MinInputSqrMagnitude = 0.0001f;

    [SerializeField] private float _moveSpeed = 5f;

    private Rigidbody _rigidbody;

    public float MoveSpeed => _moveSpeed;

    private void Awake() => _rigidbody = GetComponent<Rigidbody>();

    public void Move(Vector3 direction)
    {
        if (direction.sqrMagnitude < MinInputSqrMagnitude)
        {
            return;
        }

        ApplyMovement(direction);
    }

    private void ApplyMovement(Vector3 direction)
    {
        _rigidbody.AddForce(direction.normalized * _moveSpeed, ForceMode.Acceleration);
    }
}
```
이 코드는 선언 순서 설명용이며 물리 업데이트 루프의 권장 구현은 아니다. 실제 Rigidbody 힘 적용은 적절한 고정 시간 단계에서 수행한다.
