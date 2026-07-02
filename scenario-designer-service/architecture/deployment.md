# Deployment — Развёртывание сервиса

| Поле       | Значение   |
| ---------- | ---------- |
| **Версия** | 1.0.0      |
| **Статус** | Active     |
| **Дата**   | 2026-07-02 |

---

## Порты

| Порт | Назначение                          | Доступ                          |
| ---- | ----------------------------------- | ------------------------------- |
| 8080 | API (REST, OpenAPI, Scalar)         | Публичный (через reverse proxy) |
| 8081 | Health checks (liveness, readiness) | Только внутренняя сеть          |

---

## Environment Variables

### Обязательные

| Переменная               | Описание                             | Пример                                       |
| ------------------------ | ------------------------------------ | -------------------------------------------- |
| `ASPNETCORE_ENVIRONMENT` | Окружение                            | `Production`                                 |
| `Jwt__Key`               | JWT signing key (минимум 32 символа) | Генерировать через `openssl rand -base64 32` |

### Опциональные

| Переменная                | По умолчанию               | Описание                     |
| ------------------------- | -------------------------- | ---------------------------- |
| `Cors__AllowedOrigins__0` | `*` (только в Development) | Разрешённый origin для CORS  |
| `OpenTelemetry__Endpoint` | `http://localhost:4317`    | OTLP endpoint для телеметрии |

---

## Docker

### Dockerfile

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["backend/ScenarioDesigner.csproj", "."]
RUN dotnet restore
COPY backend/ .
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
RUN adduser --disabled-password --gecos "" appuser
USER appuser
ENTRYPOINT ["dotnet", "ScenarioDesigner.dll"]
```

### docker-compose.yml

```yaml
services:
  api:
    build:
      context: .
      dockerfile: backend/Dockerfile
    env_file:
      - .env
    ports:
      - "8080:8080"
    restart: unless-stopped

  # Health-порт не пробрасывается наружу — только внутренняя сеть
```

---

### .env файл

Создайте файл `.env` в корне проекта (не коммитить в git!):

```bash
ASPNETCORE_ENVIRONMENT=Production
Jwt__Key=CHANGE_ME_TO_REAL_KEY_MIN_32_CHARS
Cors__AllowedOrigins__0=https://your-frontend.example.com
OpenTelemetry__Endpoint=http://otel-collector:4317
```

---

### Генерация JWT-ключа

```bash
# Linux/macOS

openssl rand -base64 32

# PowerShell

[Convert]::ToBase64String((1..32 | ForEach-Object { Get-Random -Minimum 0 -Maximum 256 }) -as [byte[]])
```

---

## Kubernetes

### Deployment

```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: scenario-designer
spec:
  replicas: 2
  selector:
    matchLabels:
      app: scenario-designer
  template:
    metadata:
      labels:
        app: scenario-designer
    spec:
      terminationGracePeriodSeconds: 30
      containers:
        - name: api
          image: scenario-designer:latest
          ports:
            - containerPort: 8080
            - containerPort: 8081
          env:
            - name: ASPNETCORE_ENVIRONMENT
              value: "Production"
            - name: Jwt__Key
              valueFrom:
                secretKeyRef:
                  name: scenario-designer-secrets
                  key: jwt-key
          livenessProbe:
            httpGet:
              path: /health/live
              port: 8081
            initialDelaySeconds: 5
            periodSeconds: 10
          readinessProbe:
            httpGet:
              path: /health/ready
              port: 8081
            initialDelaySeconds: 5
            periodSeconds: 10
```

### Service

```yaml
apiVersion: v1
kind: Service
metadata:
  name: scenario-designer
spec:
  selector:
    app: scenario-designer
  ports:
    - name: api
      port: 80
      targetPort: 8080
    - name: health
      port: 8081
      targetPort: 8081
      # Health-порт не пробрасывается наружу
```

### Ingress / Nginx

```yaml
apiVersion: networking.k8s.io/v1
kind: Ingress
metadata:
  name: scenario-designer
spec:
  rules:
    - host: api.example.com
      http:
        paths:
          - path: /
            pathType: Prefix
            backend:
              service:
                name: scenario-designer
                port:
                  number: 80
```

---

## Firewall / Reverse Proxy

### Nginx

```nginx
upstream scenario_designer {
    server 127.0.0.1:8080;
}

server {
    listen 443 ssl;
    server_name api.example.com;

    location / {
        proxy_pass http://scenario_designer;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
    }

    # Health-порт НЕ проксируется — только изнутри кластера
}
```

---

## Secrets Management

### Kubernetes Secrets

```bash
kubectl create secret generic scenario-designer-secrets \
  --from-literal=jwt-key=$(openssl rand -base64 32)
```

### Docker Secrets

```bash
echo $(openssl rand -base64 32) > ./jwt-key.txt
docker secret create jwt-key ./jwt-key.txt
```

---

## Graceful Shutdown

Сервис обрабатывает `SIGTERM` корректно через `IHostApplicationLifetime`:

- Health check `/health/ready` возвращает `Unhealthy` при shutdown
- Kestrel ждёт завершения текущих запросов
- `terminationGracePeriodSeconds: 30` в Kubernetes

---

## Связанные документы

- [`operability.md`](./operability.md) — runtime поведение: health checks, graceful shutdown, rate limiting
- [`architecture.md`](./architecture.md) — стек технологий, структура проекта
- [`auth-flow.md`](./auth-flow.md) — аутентификация и авторизация
