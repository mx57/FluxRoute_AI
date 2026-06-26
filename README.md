<div align="center">

# BSDPI_AI

**Интеллектуальная система автоматического подбора DPI-bypass стратегий**

Приложение для Windows, которое использует **Thompson Sampling** и **генетическую эволюцию** для автоматического поиска и переключения оптимальных параметров обхода DPI (Deep Packet Inspection).

</div>

---

## Возможности

- **ИИ-оркестратор** — автоматический цикл: проверка сайтов → оценка → смена/эволюция стратегий
- **Thompson Sampling (Bandit)** — байесовский подход к выбору стратегии с учётом exploration/exploitation
- **Генетическая эволюция** — скрещивание и мутации лучших стратегий для генерации новых
- **Сетевой отпечаток** — привязка стратегий к конкретной сети (Ethernet, Wi-Fi, VPN)
- **Визуальный конструктор цепочек** — drag-and-drop canvas для построения pipeline обхода DPI
- **Два движка** — поддержка **zapret** (winws.exe) и **ByeDPI** (ciadpi.exe), включая гибридный режим
- **Telegram WS Proxy** — встроенный WebSocket-прокси для Telegram
- **Автообновление** — обновление движков из GitHub-релизов
- **Диагностика** — проверка файлов, процессов и сетевой связности

## Архитектура

```
BSDPI_AI.slnx
├── BSDPI_AI/              — WPF GUI (MVVM, 11 вкладок)
├── BSDPI_AI.AI/           — ИИ-подсистема: Thompson Sampling + генетическая эволюция
├── BSDPI_AI.Core/         — Ядро: движки DPI, оркестратор, ChainBuilder
├── BSDPI_AI.Updater/      — Автообновление engine/
└── BSDPI_AI.Core.Tests/   — Юнит-тесты
```

**Цепочка зависимостей:** `BSDPI_AI` → `BSDPI_AI.AI` → `BSDPI_AI.Core` ← `BSDPI_AI.Updater`

## Как работает ИИ

```
1. SyncBuiltins()         — сканирование engine/*.bat → реестр геномов
2. PickAndApplyInitial()  — Thompson Sampling → выбор лучшей стратегии
3. LoopAsync()            — цикл проверки каждые N минут:
   ├─ Capture()              — отпечаток сети
   ├─ ProbeCurrent()         — проверка сайтов через текущую стратегию
   ├─ RecordBanditSuccess/Failure — обновление Alpha/Beta
   ├─ MaybeEvolve()          — эволюция при достаточном количестве проб
   └─ SwitchToAlternative()  — смена стратегии при 2+ провалах подряд
```

## Сборка и запуск

**Требования:** .NET 10 SDK, Windows

```bash
dotnet restore BSDPI_AI.slnx
dotnet build BSDPI_AI.slnx
dotnet run --project BSDPI_AI
```

### Тесты

```bash
dotnet test BSDPI_AI.slnx
```

### Публикация релиза

```bash
dotnet publish BSDPI_AI/BSDPI_AI.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o ./publish
```

## Вкладки GUI

| # | Вкладка | Описание |
|---|---------|----------|
| 0 | Главная | Статус, запуск/остановка, домены |
| 1 | TG Прокси | Telegram WebSocket Proxy |
| 2 | Оркестратор | Классический оркестратор (рейтинг) |
| 3 | ИИ | ИИ-оркестратор, стратегии, эволюция |
| 4 | Обновление | Обновление engine/ |
| 5 | Диагностика | Проверка файлов, процессов |
| 6 | Сервис | Game filter, ipset, zapret service |
| 7 | О программе | Информация |
| 8 | Логи | Логи приложения |
| 9 | ByeDPI | Настройки ByeDPI |
| 10 | WARP | Генерация конфига Cloudflare Warp |
| 11 | Конструктор | Визуальный конструктор цепочек DPI bypass |

## Настройки ИИ

| Параметр | По умолчанию | Описание |
|----------|:------------:|----------|
| `Enabled` | `false` | Включить ИИ-оркестратор |
| `ExplorationRatePermil` | `100` | Exploration в ‰ (100‰ = 10%) |
| `MaxEvolvedStrategies` | `24` | Макс. эволюционированных стратегий |
| `EvolutionIntervalMinutes` | `60` | Интервал между эволюциями |
| `MinProbesBeforeEvolve` | `6` | Мин. проб перед эволюцией |
| `KeepHistoryDays` | `14` | Хранить историю N дней |
| `AutoDeleteBelowScore` | `60` | Автоудаление стратегий ниже этого порога |

## Лицензия

Проект распространяется под лицензией [GPL-3.0](LICENSE).

Based on [klondike0x/FluxRoute](https://github.com/klondike0x/FluxRoute) (GPLv3).

---

> **Дисклеймер.** **BSDPI_AI** является образовательным и исследовательским программным обеспечением, предназначенным для изучения сетевых технологий. Данное ПО **не является** инструментом для нарушения действующего законодательства. Использование данного ПО должно осуществляться в соответствии с применимым законодательством юрисдикции пользователя. Автор не несёт ответственности за любые последствия использования данного программного обеспечения.
