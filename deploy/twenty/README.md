# Twenty CRM (self-hosted) next to HikeConnect

**Languages:** [English](#english) · [Русский](#русская-версия)

---

## English

This folder templates a **Twenty** stack that shares the Docker bridge network **`hike_net`** with HikeConnect so `HikeConnect.Api` can reach the CRM at **`http://server:3000`** (service name `server`, internal port 3000).

The HTTP port on the **host** is published only on **loopback** (`127.0.0.1:3000`), so the CRM UI is not exposed to the internet. Use **SSH local forwarding** from your PC to administer Twenty.

References: [Docker Compose](https://docs.twenty.com/developers/self-hosting/docker-compose), [Setup](https://docs.twenty.com/developers/self-hosting/setup), [APIs](https://docs.twenty.com/developers/extend/capabilities/apis).

## Prerequisites

- Docker with Compose v2.
- **At least ~2 GB RAM** available for Twenty (Postgres, Redis, server, worker) in addition to HikeConnect.
- The **`hike_net`** network must exist before starting this compose file (see below).

## 1. Shared network `hike_net`

The root [`docker-compose.yml`](../../docker-compose.yml) declares `hike_net` with a fixed name so the network is literally `hike_net` on the Docker host.

1. From the **repository root**, start HikeConnect once (creates `hike_net`), **or** create the network manually:

   ```bash
   docker network create hike_net
   ```

2. Verify:

   ```bash
   docker network inspect hike_net
   ```

**Existing deployments:** If you previously ran HikeConnect without `name: hike_net`, Docker may have created a project-prefixed network (for example `hikeconnect_hike_net`). After pulling this change, recreate the HikeConnect stack so containers attach to **`hike_net`**, or attach the `api` container to `hike_net` manually.

## 2. Configure secrets

```bash
cd deploy/twenty
cp .env.example .env
```

Edit `.env`: set **`PG_DATABASE_PASSWORD`**, **`APP_SECRET`**, and **`TAG`**.

## 3. `SERVER_URL` and SSH tunnel (admin UI)

When you only open Twenty through **SSH local forward**, set:

- **`SERVER_URL=http://localhost:3000`**

On your PC (replace user and host):

```bash
ssh -L 3000:127.0.0.1:3000 user@your-server
```

Keep the session open, then open **`http://localhost:3000`** in a browser on the PC.

If local port **3000** is already in use on the PC, use another local port, for example:

```bash
ssh -L 13000:127.0.0.1:3000 user@your-server
```

Then open `http://localhost:13000`. You may need to align **`SERVER_URL`** with how Twenty resolves redirects and links; when possible, prefer **3000** on the laptop to match `SERVER_URL`.

**HikeConnect.Api** must use the **internal** base URL **`http://server:3000`** (not `localhost`), because `localhost` inside the `api` container is not Twenty.

## 4. Start Twenty

From **`deploy/twenty`**:

```bash
docker compose up -d
```

Checks:

- On the server: `curl -sS http://127.0.0.1:3000/healthz`
- From the `api` container (after HikeConnect is on `hike_net`): `curl -sS http://server:3000/healthz`

## 5. UI: custom object, roles, test records, API key

Complete the coursework steps in Twenty’s UI (see your practice brief):

1. **Custom object** (e.g. behavioral profile / lead) with **5–7 fields** aligned with the survey and user contact data.
   Add two service fields for idempotent sync from HikeConnect:
   - `sourceUserId` (**Text, required, unique**) - value from `User.Id`.
   - `sourceProfileId` (**Text, optional, unique**) - value from `BehavioralProfile.Id`.
2. **Two roles** with different permissions on that object.
3. **5–7 test records** with realistic values.
4. Optional: one **automation** (e.g. notify on new record).

**API key for stage 4 (HikeConnect integration):**

1. In Twenty: **Settings → API & Webhooks → + Create key**.
2. Copy the key immediately (shown once). Store it in server secrets / vault, not in git.
3. Workspace-specific API docs and playground appear under the same area after a key exists.
4. Authenticate HTTP calls with: `Authorization: Bearer YOUR_API_KEY`  
   Core GraphQL is at **`/graphql`** on the same host (see [APIs](https://docs.twenty.com/developers/extend/capabilities/apis)).

### Stage 4 upsert rule (HikeConnect -> Twenty)

For each profile sync request:

1. Find record by `sourceUserId`.
2. If found, `update` profile fields and keep service keys consistent.
3. If not found, `create` a new record with both keys (`sourceUserId`, `sourceProfileId`).
4. If `sourceProfileId` points to another record than `sourceUserId`, log a data-quality conflict and stop automatic create to avoid duplicates.

## 6. Screenshots for the report

Save images under **`report-assets/`** (create the folder if needed). Suggested filenames:

| File | What to capture |
|------|-----------------|
| `01-twenty-custom-object-fields.png` | Custom object field list / settings. |
| `02-twenty-roles-permissions.png` | Roles and permission matrix for that object. |
| `03-twenty-test-records.png` | List view with 5–7 test rows. |
| `04-twenty-api-key-settings.png` | Settings → API & Webhooks (blur or crop the secret). |
| `05-twenty-automation.png` | Optional: automation rule if you add one. |
| `06-ssh-tunnel-terminal.png` | Optional: terminal showing `ssh -L ...` (no secrets). |

Do **not** commit real API keys or passwords in screenshots; mask or crop.

## Upgrading

Re-read [Twenty Docker Compose](https://docs.twenty.com/developers/self-hosting/docker-compose) when changing **`TAG`**; verify env vars and health paths for that release.

---

## Русская версия

В этом каталоге — шаблон стека **Twenty**, подключённого к той же Docker bridge-сети **`hike_net`**, что и HikeConnect. Тогда `HikeConnect.Api` сможет обращаться к CRM по адресу **`http://server:3000`** (имя сервиса `server`, внутренний порт 3000).

HTTP-порт на **хосте** опубликован только на **loopback** (`127.0.0.1:3000`), поэтому UI CRM не торчит наружу в интернет. Для администрирования Twenty с ПК используйте **локальное перенаправление портов по SSH**.

Ссылки: [Docker Compose](https://docs.twenty.com/developers/self-hosting/docker-compose), [Setup](https://docs.twenty.com/developers/self-hosting/setup), [APIs](https://docs.twenty.com/developers/extend/capabilities/apis).

### Требования

- Docker с Compose v2.
- **Хотя бы ~2 GB RAM** под Twenty (Postgres, Redis, server, worker) **в дополнение** к HikeConnect.
- Сеть **`hike_net`** должна существовать до запуска этого compose-файла (см. ниже).

### 1. Общая сеть `hike_net`

В корневом [`docker-compose.yml`](../../docker-compose.yml) для `hike_net` задано фиксированное имя, поэтому на Docker-хосте сеть называется именно **`hike_net`**.

1. Из **корня репозитория** один раз поднимите HikeConnect (создастся `hike_net`), **или** создайте сеть вручную:

   ```bash
   docker network create hike_net
   ```

2. Проверка:

   ```bash
   docker network inspect hike_net
   ```

**Уже развёрнутые окружения:** если раньше HikeConnect запускался без `name: hike_net`, Docker мог создать сеть с префиксом проекта (например `hikeconnect_hike_net`). После обновления пересоздайте/перезапустите стек HikeConnect, чтобы контейнеры оказались в **`hike_net`**, либо вручную подключите контейнер `api` к `hike_net`.

### 2. Секреты

```bash
cd deploy/twenty
cp .env.example .env
```

Отредактируйте `.env`: задайте **`PG_DATABASE_PASSWORD`**, **`APP_SECRET`** и **`TAG`**.

### 3. `SERVER_URL` и SSH-туннель (админ-UI)

Если вы открываете Twenty **только** через SSH local forward, установите:

- **`SERVER_URL=http://localhost:3000`**

На вашем ПК (замените user/host):

```bash
ssh -L 3000:127.0.0.1:3000 user@your-server
```

Держите сессию открытой и откройте **`http://localhost:3000`** в браузере на ПК.

Если локальный порт **3000** на ПК занят, используйте другой, например:

```bash
ssh -L 13000:127.0.0.1:3000 user@your-server
```

Тогда в браузере — `http://localhost:13000`. Возможно, понадобится согласовать **`SERVER_URL`** с тем, как Twenty строит редиректы и ссылки; по возможности оставляйте **3000** на ПК, чтобы совпадало с `SERVER_URL`.

**HikeConnect.Api** должен использовать **внутренний** base URL **`http://server:3000`** (не `localhost`), потому что `localhost` внутри контейнера `api` — это не Twenty.

### 4. Запуск Twenty

Из каталога **`deploy/twenty`**:

```bash
docker compose up -d
```

Проверки:

- На сервере: `curl -sS http://127.0.0.1:3000/healthz`
- Из контейнера `api` (когда HikeConnect в `hike_net`): `curl -sS http://server:3000/healthz`

### 5. UI: кастомный объект, роли, тестовые записи, API key

Выполните шаги практики в UI Twenty:

1. **Кастомный объект** (например профиль / лид) с **5–7 полями** по опросу и контактным данным пользователя.
   Добавьте два служебных поля для идемпотентной синхронизации из HikeConnect:
   - `sourceUserId` (**Text, обязательно, unique**) - значение `User.Id`.
   - `sourceProfileId` (**Text, опционально, unique**) - значение `BehavioralProfile.Id`.
2. **Две роли** с разными правами на этот объект.
3. **5–7 тестовых записей** с реалистичными данными.
4. Опционально: одна **автоматизация** (например уведомление при новой записи).

**API key для этапа 4 (интеграция HikeConnect):**

1. В Twenty: **Settings → API & Webhooks → + Create key**.
2. Скопируйте ключ сразу (он показывается один раз). Храните в секретах на сервере, не в git.
3. Документация API и playground для вашего workspace появятся в том же разделе после создания ключа.
4. Заголовок авторизации: `Authorization: Bearer YOUR_API_KEY`  
   Core GraphQL доступен по пути **`/graphql`** на том же хосте (см. [APIs](https://docs.twenty.com/developers/extend/capabilities/apis)).

### Правило upsert для этапа 4 (HikeConnect -> Twenty)

Для каждой синхронизации профиля:

1. Ищите запись по `sourceUserId`.
2. Если запись найдена, выполняйте `update` полей профиля и сохраняйте консистентность служебных ключей.
3. Если запись не найдена, выполняйте `create` с обоими ключами (`sourceUserId`, `sourceProfileId`).
4. Если `sourceProfileId` указывает на другую запись, чем `sourceUserId`, логируйте data-quality конфликт и не создавайте дубль автоматически.

### 6. Скриншоты для отчёта

Сохраняйте изображения в **`report-assets/`** (если нужно — создайте папку). Рекомендуемые имена:

| Файл | Что снять |
|------|-----------|
| `01-twenty-custom-object-fields.png` | Список полей кастомного объекта / настройки. |
| `02-twenty-roles-permissions.png` | Роли и матрица прав на этот объект. |
| `03-twenty-test-records.png` | Список с 5–7 тестовыми строками. |
| `04-twenty-api-key-settings.png` | Settings → API & Webhooks (секрет замазать/обрезать). |
| `05-twenty-automation.png` | Опционально: правило автоматизации. |
| `06-ssh-tunnel-terminal.png` | Опционально: терминал с `ssh -L ...` (без секретов). |

Не коммитьте реальные API-ключи/пароли в скриншотах — замаскируйте или обрежьте.

### Обновление версии

При смене **`TAG`** перечитайте [Twenty Docker Compose](https://docs.twenty.com/developers/self-hosting/docker-compose); проверьте env vars и путь healthcheck для выбранного релиза.
