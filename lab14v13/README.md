# Лабораторна робота №14: Аналіз SRP/OCP у відкритих репозиторіях
**Виконав:** Кудрявцев (Варіант 13)

## 1. Обрані Open-Source репозиторії (різні за доменом)
1. **Serilog** ([https://github.com/serilog/serilog](https://github.com/serilog/serilog)) — *Домен: структуроване логування та діагностика в .NET.*
2. **nopCommerce** ([https://github.com/nopSolutions/nopCommerce](https://github.com/nopSolutions/nopCommerce)) — *Домен: електронна комерція (E-commerce платформа).*

---

## 2. Аналіз принципу єдиної відповідальності (SRP)

### 2.1. Два приклади вдалого застосування SRP

#### Приклад 1: `MessageTemplateParser` (Serilog)
* **Посилання на код:** [MessageTemplateParser.cs (рядки 24–78)](https://github.com/serilog/serilog/blob/dev/src/Serilog/Parsing/MessageTemplateParser.cs#L24-L78)
* **Де видно принцип:** Клас займається **виключно** лексичним розбором (парсингом) рядка шаблону повідомлення (наприклад, `"User {UserId} logged in"`) на послідовність токенів (`TextToken` та `PropertyToken`). Він не форматує вивід у JSON, не пише у файли та не фільтрує рівні логування.
* **Переваги для підтримки:** Парсер легко тестувати ізольовано (unit-тестами). Якщо з'явиться новий спосіб виводу логів (наприклад, у хмарне сховище), цей клас змінювати не доведеться.
* **Що зміниться при додаванні нової функції:** При додаванні нового синтаксису плейсхолдерів змінюється лише логіка токенізації всередині цього класу без ризику зламати запис у консоль чи файл.

#### Приклад 2: `PriceCalculationService` (nopCommerce)
* **Посилання на код:** [PriceCalculationService.cs (рядки 45–130)](https://github.com/nopSolutions/nopCommerce/blob/develop/src/Libraries/Nop.Services/Catalog/PriceCalculationService.cs#L45-L130)
* **Де видно принцип:** Клас відповідає лише за розрахунок фінальної вартості товару (врахування знижок, групових цін, атрибутів). Робота з БД делегована репозиторіям, а розрахунок податків винесений в окремий `ITaxService`.
* **Переваги для підтримки:** Бізнес-логіка ціноутворення локалізована в одному місці; зміни у схемі бази даних або валютних курсах не зачіпають формули знижок.
* **Що зміниться при додаванні нової функції:** При впровадженні нового типу акцій змінюється лише відповідний метод розрахунку ціни, тоді як кошик та оформлення замовлення продовжують працювати без змін.

---

### 2.2. Два приклади порушення SRP

#### Приклад 1: `OrderProcessingService` (nopCommerce)
* **Посилання на код:** [OrderProcessingService.cs (рядки 100–450, метод PlaceOrderAsync)](https://github.com/nopSolutions/nopCommerce/blob/develop/src/Libraries/Nop.Services/Orders/OrderProcessingService.cs#L100-L450)
* **Де видно порушення:** Клас є типовим «God Object» (понад 2500 рядків коду). Один метод `PlaceOrderAsync` одночасно: валідує кошик, списує кошти через платіжний шлюз, оновлює залишки на складі, генерує PDF-інвойси, відправляє email-сповіщення та нараховує бонусні бали.
* **Ризики для підтримки:** Висока зв'язаність (High Coupling) та величезна кількість залежностей у конструкторі (понад 20 сервісів). Будь-яка правка в логіці бонусів може випадково зламати процес оплати.
* **Що зміниться при додаванні нової функції:** Додавання нового каналу сповіщень (наприклад, SMS або Telegram при замовленні) змусить редагувати критичний фінансовий код оформлення замовлення.

#### Приклад 2: `LoggerConfiguration` (Serilog)
* **Посилання на код:** [LoggerConfiguration.cs (рядки 35–260)](https://github.com/serilog/serilog/blob/dev/src/Serilog/LoggerConfiguration.cs#L35-L260)
* **Де видно порушення:** Клас виступає одночасно як Fluent-білдер конфігурації, фабрика створення `ILogger`, реєстр деструктуризаторів об'єктів та менеджер життєвого циклу внутрішніх sinks (обробників виводу).
* **Ризики для підтримки:** Ускладнюється читання коду та розширення конфігурації без зміни самого ядра білдера.
* **Що зміниться при додаванні нової функції:** Поява нових глобальних параметрів конфігурування вимагає постійного розростання цього одного класу.

---

## 3. Аналіз принципу відкритості/закритості (OCP)

### Приклад 1: Інтерфейс `ILogEventSink` (Serilog)
* **Посилання на код:** [ILogEventSink.cs (рядки 18–26)](https://github.com/serilog/serilog/blob/dev/src/Serilog/Core/ILogEventSink.cs#L18-L26)
* **Де видно принцип:** Ядро Serilog закрите для модифікації, але відкрите для розширення через єдиний контракт `void Emit(LogEvent logEvent)`. Усі приймачі логів (`ConsoleSink`, `FileSink`, `SeqSink`) реалізують цей інтерфейс.
* **Переваги для підтримки:** Розробники по всьому світу створюють сотні сторонніх пакетів (`Serilog.Sinks.Telegram`, `Serilog.Sinks.ElasticSearch`), не змінюючи жодного рядка в основному репозиторії Serilog.
* **Що зміниться при додаванні нової функції:** Для запису логів у нову базу даних достатньо створити новий клас, що реалізує `ILogEventSink`, та підключити його через метод розширення `.WriteTo.Sink(...)`.

### Приклад 2: Плагінна архітектура платіжних методів `IPaymentMethod` (nopCommerce)
* **Посилання на код:** [IPaymentMethod.cs (рядки 15–95)](https://github.com/nopSolutions/nopCommerce/blob/develop/src/Libraries/Nop.Services/Payments/IPaymentMethod.cs#L15-L95)
* **Де видно принцип:** Процес оплати в ядрі магазину взаємодіє лише з абстракцією `IPaymentMethod` (методи `ProcessPaymentAsync`, `RefundAsync`). Конкретні платіжні системи (PayPal, Stripe, LiqPay) реалізовані як незалежні плагіни-стратегії.
* **Переваги для підтримки:** Повна ізоляція зовнішніх API платіжних шлюзів від ядра інтернет-магазину.
* **Що зміниться при додаванні нової функції:** При інтеграції нового банку створюється окремий проєкт-плагін з реалізацією `IPaymentMethod` без перекомпіляції чи зміни ядра `nopCommerce`.

---

## 4. Пропозиція рефакторингу для найгіршого прикладу (`OrderProcessingService`)

**Проблема:** Перевантажений метод `PlaceOrderAsync` в `nopCommerce` порушує SRP, змішуючи транзакційну фінансову логіку з побічними ефектами (відправка листів, PDF, бонуси).

**Рішення (застосування патерну Domain Events / Observer + SRP):**
Розділити монолітний метод на основний координатор оформлення замовлення та незалежні обробники події `OrderPlacedEvent`.

```csharp
// 1. Подія домену
public record OrderPlacedEvent(Order Order);

// 2. Абстракція обробника події (дотримання OCP та SRP)
public interface IOrderPlacedHandler
{
    Task HandleAsync(OrderPlacedEvent domainEvent);
}

// 3. Окремі класи з єдиною відповідальністю (SRP)
public class EmailNotificationHandler : IOrderPlacedHandler
{
    public async Task HandleAsync(OrderPlacedEvent e) 
        => await _emailService.SendOrderConfirmationAsync(e.Order);
}

public class LoyaltyPointsHandler : IOrderPlacedHandler
{
    public async Task HandleAsync(OrderPlacedEvent e) 
        => await _rewardService.AddPointsForOrderAsync(e.Order);
}

// 4. Очищений сервіс оформлення замовлення
public class OrderPlacementCoordinator
{
    private readonly IEnumerable<IOrderPlacedHandler> _handlers;
    // Тепер сервіс відповідає ТІЛЬКИ за фіксацію замовлення та запуск події
    public async Task PlaceOrderAsync(Order order)
    {
        await _paymentService.ChargeAsync(order);
        await _orderRepository.SaveAsync(order);

        var domainEvent = new OrderPlacedEvent(order);
        foreach (var handler in _handlers)
        {
            await handler.HandleAsync(domainEvent);
        }
    }
}