# Звіт з аналізу інкапсуляції в Open-Source проєкті
**Самостійна робота №4**

## 1. Обраний проєкт
- **Назва:** Entity Framework Core (EF Core)
- **Опис:** Офіційна ORM-платформа від Microsoft для зручної роботи з базами даних у середовищі .NET.
- **Посилання на GitHub:** [https://github.com/dotnet/efcore](https://github.com/dotnet/efcore)

---

## 2. Аналіз інкапсуляції

### Клас 1: `EntityEntry`
- **Посилання на файл:** [EntityEntry.cs (GitHub)](https://github.com/dotnet/efcore/blob/main/src/EFCore/ChangeTracking/EntityEntry.cs)
- **Опис класу:** Забезпечує публічне API для доступу до метаданих та поточного стану відстежуваної сутності.
- **Поля:** Усі внутрішні дані приховані у приватних та захищених полях (наприклад, `private readonly InternalEntityEntry _internalEntry;`). Це унеможливлює випадкову зміну стану ззовні.
- **Властивості:**
  - `public virtual object Entity => _internalEntry.Entity;` — властивість **тільки для читання** (read-only, без `set`). Посилання на об'єкт сутності задається при створенні та не може бути підмінене.
  - `public virtual EntityState State { get; set; }` — властивість із `get` та `set`. Сеттер не просто змінює значення, а викликає внутрішній метод `_internalEntry.SetEntityState(value)`, який виконує каскадне оновлення пов'язаних об'єктів.
- **Індексатори:**
  - `public virtual PropertyEntry Property(string propertyName)` — метод-індексатор для зручного доступу до властивостей сутності за їхнім текстовим ім'ям.

```csharp
// Фрагмент коду з EntityEntry.cs (Приклад інкапсуляції)
public class EntityEntry : IInfrastructure<InternalEntityEntry>
{
    private readonly InternalEntityEntry _internalEntry;

    public virtual object Entity
    {
        get => _internalEntry.Entity;
    }

    public virtual EntityState State
    {
        get => _internalEntry.EntityState;
        set => _internalEntry.SetEntityState(value);
    }
}