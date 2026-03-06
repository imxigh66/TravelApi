# Digital Tourism Platform
**Цифровая платформа для обмена опытом в сфере туризма**

Социальная платформа для путешественников — планируй маршруты совместно, делись впечатлениями и получай AI-рекомендации в едином пространстве.

---

## Стек

**Backend:** .NET 8, ASP.NET Core Web API, Entity Framework Core, MS SQL Server, MediatR (CQRS), FluentValidation, JWT + Refresh Tokens, SignalR, AWS S3, OpenAI API

**Frontend:** React.js, Axios, React Router, Mapbox, SignalR Client

**Архитектура:** Clean Architecture (4 слоя), CQRS, RESTful API, Swagger

---

## Возможности

- Социальная лента с постами и комментариями
- Каталог мест с рейтингами и тегами
- Планировщик поездок с разбивкой по дням
- Совместное редактирование маршрутов в реальном времени
- Встроенный мессенджер (SignalR)
- AI-рекомендации и оптимизация маршрутов
- Верификация бизнес-аккаунтов
- Экспорт маршрутов в PDF и календарь

---

## Запуск

### Backend

```bash
git clone <repository_url>
```

Настройте строку подключения в `appsettings.json`, затем:

```bash
dotnet ef database update
dotnet run
```

Swagger: `https://localhost:5001/swagger`

### Frontend

```bash
cd client
npm install
npm start
```

---

## Структура

```
Domain         — сущности и бизнес-модели
Application    — бизнес-логика, CQRS, DTO, валидация
Infrastructure — EF Core, S3, SMTP, SignalR, OpenAI
WebAPI         — контроллеры, точка входа
```
